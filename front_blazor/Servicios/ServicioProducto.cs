using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioProducto — el ÚNICO sitio del front que sabe de HTTP.
//
// Las interfaces gráficas llaman métodos con nombre de dominio (ListarAsync,
// CrearAsync) y reciben objetos o un mensaje de error. No ven códigos de
// estado, ni rutas, ni JSON.
//
// Por qué un servicio POR RECURSO y no uno genérico: un
// `ApiService.Listar("producto")` es más corto, y con una sola tabla ni se
// nota. Con doce, el que lee el código ya no sabe qué rutas existen.
// ============================================================
public class ServicioProducto(HttpClient cliente, EstadoSesion sesion)
{
    private const string Ruta = "api/producto";
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

    /// <summary>Lo que devuelve cada operación: o salió bien, o hay un
    /// mensaje para mostrarle a la persona. Nunca una excepción suelta que
    /// tumbe la interfaz.</summary>
    public record Resultado<T>(bool Bien, T? Dato, string? Mensaje)
    {
        public static Resultado<T> Ok(T dato) => new(true, dato, null);
        public static Resultado<T> Falla(string mensaje) => new(false, default, mensaje);
    }

    public async Task<Resultado<List<Producto>>> ListarAsync()
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync(Ruta);

            // 204: la tabla está vacía. NO es un error, y tratarlo como error
            // es el tropiezo clásico: el cuerpo viene vacío y el deserializador
            // revienta.
            if (r.StatusCode == HttpStatusCode.NoContent)
                return Resultado<List<Producto>>.Ok([]);

            if (!r.IsSuccessStatusCode)
                return Resultado<List<Producto>>.Falla(await MensajeDe(r));

            // ============================================================
            // EL SOBRE DEL CONTRATO: { tabla, limite, total, datos[] }
            //
            // La API NO devuelve un arreglo pelado. Deserializar a
            // List<Producto> directo falla en silencio y la interfaz gráfica sale
            // vacía sin decir por qué — se descubrió levantándolo, no
            // leyéndolo. Ver 6_contracts.md.
            // ============================================================
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos")
                             .Deserialize<List<Producto>>(Opciones) ?? [];
            return Resultado<List<Producto>>.Ok(datos);
        }
        catch (Exception)
        {
            // La API apagada llega aquí. La interfaz gráfica tiene que SEGUIR EN PIE:
            // es un criterio de aceptación, no una cortesía.
            return Resultado<List<Producto>>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    public async Task<Resultado<Producto>> ObtenerAsync(string codigo)
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync($"{Ruta}/{codigo}");
            if (!r.IsSuccessStatusCode)
                return Resultado<Producto>.Falla(await MensajeDe(r));
            var p = await r.Content.ReadFromJsonAsync<Producto>(Opciones);
            return p is null
                ? Resultado<Producto>.Falla("La respuesta llegó vacía.")
                : Resultado<Producto>.Ok(p);
        }
        catch (Exception)
        {
            return Resultado<Producto>.Falla("No se pudo conectar con el servicio.");
        }
    }

    public async Task<Resultado<Producto>> CrearAsync(Producto p)
    {
        try
        {
            Autorizar();
            var r = await cliente.PostAsJsonAsync(Ruta, p);
            if (!r.IsSuccessStatusCode)
                return Resultado<Producto>.Falla(await MensajeDe(r));
            return Resultado<Producto>.Ok(p);
        }
        catch (Exception)
        {
            return Resultado<Producto>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>El PUT: reemplazo COMPLETO. Si falta un campo, la API responde
    /// 422 — y eso es lo que distingue este método del siguiente.</summary>
    public async Task<Resultado<Producto>> ReemplazarAsync(string codigo, Producto p)
    {
        try
        {
            Autorizar();
            var r = await cliente.PutAsJsonAsync($"{Ruta}/{codigo}", p);
            if (!r.IsSuccessStatusCode)
                return Resultado<Producto>.Falla(await MensajeDe(r));
            return Resultado<Producto>.Ok(p);
        }
        catch (Exception)
        {
            return Resultado<Producto>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>El PATCH: parcial. El MISMO cuerpo que el PUT rechazaría con
    /// 422 aquí responde 200.</summary>
    public async Task<Resultado<Producto>> ActualizarAsync(string codigo, object parcial)
    {
        try
        {
            Autorizar();
            var r = await cliente.PatchAsJsonAsync($"{Ruta}/{codigo}", parcial);
            if (!r.IsSuccessStatusCode)
                return Resultado<Producto>.Falla(await MensajeDe(r));
            return Resultado<Producto>.Ok(new Producto { Codigo = codigo });
        }
        catch (Exception)
        {
            return Resultado<Producto>.Falla("No se pudo conectar con el servicio.");
        }
    }

    public async Task<Resultado<bool>> RetirarAsync(string codigo)
    {
        try
        {
            Autorizar();
            var r = await cliente.DeleteAsync($"{Ruta}/{codigo}");
            if (!r.IsSuccessStatusCode)
                return Resultado<bool>.Falla(await MensajeDe(r));
            return Resultado<bool>.Ok(true);
        }
        catch (Exception)
        {
            return Resultado<bool>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>Saca el mensaje del DOMINIO del sobre de error de la API. Si no
    /// viene, dice algo entendible — nunca «Error 422».</summary>
    private static async Task<string> MensajeDe(HttpResponseMessage r)
    {
        try
        {
            var e = await r.Content.ReadFromJsonAsync<ErrorApi>(Opciones);
            if (!string.IsNullOrWhiteSpace(e?.Mensaje)) return e!.Mensaje;
        }
        catch { /* el cuerpo no era el sobre esperado */ }

        return r.StatusCode switch
        {
            HttpStatusCode.NotFound => "No se encontró ese producto.",
            HttpStatusCode.UnprocessableEntity => "Faltan datos obligatorios.",
            HttpStatusCode.BadRequest => "Los datos enviados no son válidos.",
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
            _ => "El servicio respondió con un problema. Intente de nuevo."
        };
    }
}
