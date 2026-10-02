using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioFactura — el servicio que NO tiene los cinco verbos.
//
// Y eso no es una omision: es el contrato. Una factura emitida no se
// REEMPLAZA ni se EDITA, porque es un documento con valor legal. Y no se
// BORRA: se ANULA, que es otra cosa — la fila sigue ahi, cambia de estado, y
// el stock vuelve.
//
//   ListarAsync     GET  /api/factura                 (con detalle anidado)
//   ConsultarAsync  GET  /api/factura/{numero}
//   CrearAsync      POST /api/factura                 (maestro + detalle, UN envio)
//   AnularAsync     POST /api/factura/{numero}/anular  <- una ACCION, no un DELETE
//
// Por que el anular es POST y no DELETE: porque DELETE significa «que deje de
// existir». Anular significa «que quede como anulada». Si fuera DELETE, el
// que lea el codigo esperaria que la factura desapareciera.
// ============================================================
public class ServicioFactura(HttpClient cliente, EstadoSesion sesion)
{
    private const string Ruta = "api/factura";
    /// <summary>Pone el token en la cabecera antes de cada peticion.
    ///
    /// Se llama en TODOS los metodos, sin excepcion: un metodo al que se le
    /// olvide responde 401 y el que lo lea va a creer que la sesion vencio.
    ///
    /// Y si no hay token, la cabecera se limpia en vez de dejar la anterior:
    /// despues de salir, las peticiones tienen que fallar con 401, no seguir
    /// funcionando con un token que ya nadie deberia tener.</summary>
    private void Autorizar()
    {
        cliente.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(sesion.Token)
                ? null
                : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sesion.Token);
    }


    private static readonly JsonSerializerOptions Opciones =
        new() { PropertyNameCaseInsensitive = true };

    /// <summary>O salio bien, o hay un mensaje para mostrarle a la persona.
    /// Nunca una excepcion suelta que tumbe la interfaz grafica.</summary>
    public record Resultado<T>(bool Bien, T? Dato, string? Mensaje)
    {
        public static Resultado<T> Ok(T dato) => new(true, dato, null);
        public static Resultado<T> Falla(string mensaje) => new(false, default, mensaje);
    }

    public async Task<Resultado<List<Factura>>> ListarAsync()
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync(Ruta);
            if (r.StatusCode == HttpStatusCode.NoContent)
                return Resultado<List<Factura>>.Ok([]);
            if (!r.IsSuccessStatusCode)
                return Resultado<List<Factura>>.Falla(await MensajeDe(r));

            // El sobre de factura NO trae `limite` (el SP devuelve todas), pero
            // SI trae `datos` — que es lo unico que el front necesita.
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos")
                             .Deserialize<List<Factura>>(Opciones) ?? [];
            return Resultado<List<Factura>>.Ok(datos);
        }
        catch (Exception)
        {
            return Resultado<List<Factura>>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    /// <summary>Una sola factura. A diferencia del listado, esta NO viene en
    /// sobre: el SP devuelve el objeto y el controlador lo emite tal cual.</summary>
    public async Task<Resultado<Factura>> ConsultarAsync(int numero)
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync($"{Ruta}/{numero}");
            if (!r.IsSuccessStatusCode)
                return Resultado<Factura>.Falla(await MensajeDe(r));
            var f = await r.Content.ReadFromJsonAsync<Factura>(Opciones);
            return f is null
                ? Resultado<Factura>.Falla("La respuesta llego vacia.")
                : Resultado<Factura>.Ok(f);
        }
        catch (Exception)
        {
            return Resultado<Factura>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>UN SOLO ENVIO con el maestro y el detalle juntos.
    ///
    /// Es el criterio que no se puede simular: si el front enviara un POST por
    /// renglon, agregar tres y quitar uno dejaria TRES en la base. El
    /// procedimiento almacenado inserta el encabezado y los renglones en UNA
    /// transaccion — o entra todo, o no entra nada.</summary>
    public async Task<Resultado<Factura>> CrearAsync(FacturaNueva nueva)
    {
        try
        {
            Autorizar();
            var r = await cliente.PostAsJsonAsync(Ruta, nueva);
            if (!r.IsSuccessStatusCode)
                return Resultado<Factura>.Falla(await MensajeDe(r));

            // La factura vuelve CON la fecha, los subtotales y el total que
            // calculo la base. Por eso vale la pena leer la respuesta y no
            // limitarse a recargar la lista.
            var f = await r.Content.ReadFromJsonAsync<Factura>(Opciones);
            return f is null
                ? Resultado<Factura>.Falla("La factura se creo, pero la respuesta llego vacia.")
                : Resultado<Factura>.Ok(f);
        }
        catch (Exception)
        {
            return Resultado<Factura>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>El borrado LOGICO. La fila no se va: queda 'anulada', y el SP
    /// devuelve el stock de cada renglon.</summary>
    public async Task<Resultado<bool>> AnularAsync(int numero)
    {
        try
        {
            Autorizar();
            var r = await cliente.PostAsync($"{Ruta}/{numero}/anular", null);
            if (!r.IsSuccessStatusCode)
                return Resultado<bool>.Falla(await MensajeDe(r));
            return Resultado<bool>.Ok(true);
        }
        catch (Exception)
        {
            return Resultado<bool>.Falla("No se pudo conectar con el servicio.");
        }
    }

    private static async Task<string> MensajeDe(HttpResponseMessage r)
    {
        try
        {
            var e = await r.Content.ReadFromJsonAsync<ErrorApi>(Opciones);
            if (!string.IsNullOrWhiteSpace(e?.Mensaje)) return e!.Mensaje;
        }
        catch { }

        return r.StatusCode switch
        {
            HttpStatusCode.NotFound => "No se encontro esa factura.",
            HttpStatusCode.UnprocessableEntity => "Faltan datos obligatorios.",
            HttpStatusCode.BadRequest => "Los datos enviados no son validos.",
            // 409 al anular: ya estaba anulada. Anular dos veces no es un error
            // del sistema — es un aviso para la persona.
            HttpStatusCode.Conflict => "Esa factura ya estaba anulada.",
            // v3 — LOS DOS CODIGOS DEL CONTROL DE ACCESO, y decirlos bien
            // es la mitad de la leccion:
            //
            //   401  «no se quien es usted»   -> no hay token, o vencio
            //   403  «se quien es, y no puede» -> el token sirve, el rol no
            //
            // Si los dos dijeran «error del servicio», la persona no tendria
            // forma de saber si le falta entrar o le falta permiso.
            HttpStatusCode.Unauthorized =>
                "Su sesion no es valida o ya vencio. Vuelva a iniciar sesion.",
            HttpStatusCode.Forbidden =>
                "Su rol no tiene permiso para esta operacion.",
            _ => "El servicio respondio con un problema. Puede ser que no haya "
               + "stock suficiente para uno de los productos."
        };
    }
}
