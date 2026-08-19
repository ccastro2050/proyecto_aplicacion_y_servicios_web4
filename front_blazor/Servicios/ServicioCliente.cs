using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioCliente — CRUD completo, calcado de ServicioRol.
//
// La diferencia con los servicios de la v1 no esta aqui: esta en la interfaz
// grafica, que tiene que CARGAR los desplegables de persona y de empresa. El
// servicio sigue hablando de un solo recurso — y por eso Clientes.razor
// inyecta TRES servicios, no uno.
// ============================================================
public class ServicioCliente(HttpClient cliente, EstadoSesion sesion)
{
    private const string Ruta = "api/cliente";
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

    public async Task<Resultado<List<Cliente>>> ListarAsync()
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync(Ruta);

            // 204: la tabla esta vacia. NO es un error.
            if (r.StatusCode == HttpStatusCode.NoContent)
                return Resultado<List<Cliente>>.Ok([]);

            if (!r.IsSuccessStatusCode)
                return Resultado<List<Cliente>>.Falla(await MensajeDe(r));

            // EL SOBRE: { tabla, limite, total, datos[] } — no un arreglo pelado.
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos")
                             .Deserialize<List<Cliente>>(Opciones) ?? [];
            return Resultado<List<Cliente>>.Ok(datos);
        }
        catch (Exception)
        {
            return Resultado<List<Cliente>>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    public async Task<Resultado<Cliente>> CrearAsync(Cliente e)
    {
        try
        {
            Autorizar();
            var r = await cliente.PostAsJsonAsync(Ruta, e);
            if (!r.IsSuccessStatusCode)
                return Resultado<Cliente>.Falla(await MensajeDe(r));
            return Resultado<Cliente>.Ok(e);
        }
        catch (Exception)
        {
            return Resultado<Cliente>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>PUT: reemplazo COMPLETO. Si falta un campo, 422.</summary>
    public async Task<Resultado<Cliente>> ReemplazarAsync(int clave, Cliente e)
    {
        try
        {
            Autorizar();
            var r = await cliente.PutAsJsonAsync($"{Ruta}/{clave}", e);
            if (!r.IsSuccessStatusCode)
                return Resultado<Cliente>.Falla(await MensajeDe(r));
            return Resultado<Cliente>.Ok(e);
        }
        catch (Exception)
        {
            return Resultado<Cliente>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>PATCH: parcial. El MISMO cuerpo que el PUT rechaza, aqui pasa.</summary>
    public async Task<Resultado<bool>> ActualizarAsync(int clave, object parcial)
    {
        try
        {
            Autorizar();
            var r = await cliente.PatchAsJsonAsync($"{Ruta}/{clave}", parcial);
            if (!r.IsSuccessStatusCode)
                return Resultado<bool>.Falla(await MensajeDe(r));
            return Resultado<bool>.Ok(true);
        }
        catch (Exception)
        {
            return Resultado<bool>.Falla("No se pudo conectar con el servicio.");
        }
    }

    public async Task<Resultado<bool>> EliminarAsync(int clave)
    {
        try
        {
            Autorizar();
            var r = await cliente.DeleteAsync($"{Ruta}/{clave}");
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
        catch { /* el cuerpo no era el sobre esperado */ }

        return r.StatusCode switch
        {
            HttpStatusCode.NotFound => "No se encontro ese cliente.",
            HttpStatusCode.UnprocessableEntity => "Faltan datos obligatorios.",
            HttpStatusCode.BadRequest => "Los datos enviados no son validos.",
            // 409 es el codigo de la CLAVE FORANEA que no existe. Es nuevo en la
            // v2: en la v1 no habia ninguna tabla que pudiera darlo.
            HttpStatusCode.Conflict => "La persona o la empresa que eligio ya no existe.",
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
            _ => "El servicio respondio con un problema. Intente de nuevo."
        };
    }
}
