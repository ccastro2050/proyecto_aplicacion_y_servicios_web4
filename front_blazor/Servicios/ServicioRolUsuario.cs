using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioRolUsuario — una tabla PUENTE, y por eso le faltan dos verbos.
//
//   ListarAsync   GET    /api/rol-usuario
//   CrearAsync    POST   /api/rol-usuario
//   EliminarAsync DELETE /api/rol-usuario/{email}/{idrol}   <- DOS claves
//
// No hay ReemplazarAsync ni ActualizarAsync: una asignacion no tiene campos
// que cambiar. Existe o no existe. Cambiar «Ana es vendedora» por «Ana es
// administradora» es quitar una pareja y poner otra.
//
// OJO CON LA RUTA: es `api/rol-usuario` CON GUION, aunque la tabla se llame
// `rol_usuario` con subrayado. Asumir el nombre de la tabla da 404 — paso.
// ============================================================
public class ServicioRolUsuario(HttpClient cliente, EstadoSesion sesion)
{
    private const string Ruta = "api/rol-usuario";
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

    public async Task<Resultado<List<RolUsuario>>> ListarAsync()
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync(Ruta);
            if (r.StatusCode == HttpStatusCode.NoContent)
                return Resultado<List<RolUsuario>>.Ok([]);
            if (!r.IsSuccessStatusCode)
                return Resultado<List<RolUsuario>>.Falla(await MensajeDe(r));
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos")
                             .Deserialize<List<RolUsuario>>(Opciones) ?? [];
            return Resultado<List<RolUsuario>>.Ok(datos);
        }
        catch (Exception)
        {
            return Resultado<List<RolUsuario>>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    public async Task<Resultado<bool>> CrearAsync(RolUsuario e)
    {
        try
        {
            Autorizar();
            var r = await cliente.PostAsJsonAsync(Ruta, e);
            if (!r.IsSuccessStatusCode)
                return Resultado<bool>.Falla(await MensajeDe(r));
            return Resultado<bool>.Ok(true);
        }
        catch (Exception)
        {
            return Resultado<bool>.Falla("No se pudo conectar con el servicio.");
        }
    }

    /// <summary>El borrado necesita LAS DOS claves, porque la PK es compuesta.</summary>
    public async Task<Resultado<bool>> EliminarAsync(string email, int idrol)
    {
        try
        {
            Autorizar();
            var r = await cliente.DeleteAsync($"{Ruta}/{email}/{idrol}");
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
            HttpStatusCode.NotFound => "No se encontro esa asignacion.",
            HttpStatusCode.UnprocessableEntity => "Elija un usuario y un rol.",
            HttpStatusCode.BadRequest => "Los datos enviados no son validos.",
            // 409 aqui tiene DOS causas, y conviene no mentir sobre cual:
            // la pareja ya existe (PK compuesta repetida), o el usuario/rol
            // ya no existe (FK).
            HttpStatusCode.Conflict => "Ese usuario ya tiene ese rol, o uno de los dos ya no existe.",
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
