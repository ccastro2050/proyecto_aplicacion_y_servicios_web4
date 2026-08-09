using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioUsuario — el UNICO sitio del front que sabe de HTTP para `usuario`.
//
// Un servicio POR RECURSO, no un ApiService generico con la tabla como
// parametro: con once recursos, el generico deja de decir que rutas existen.
// ============================================================
public class ServicioUsuario(HttpClient cliente, EstadoSesion sesion)
{
    private const string Ruta = "api/usuario";
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

    public record Resultado<T>(bool Bien, T? Dato, string? Mensaje)
    {
        public static Resultado<T> Ok(T dato) => new(true, dato, null);
        public static Resultado<T> Falla(string m) => new(false, default, m);
    }

    public async Task<Resultado<List<Usuario>>> ListarAsync()
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync(Ruta);

            // 204: la tabla esta vacia. NO es un error, y tratarlo como error
            // es el tropiezo clasico: el cuerpo viene vacio y revienta.
            if (r.StatusCode == HttpStatusCode.NoContent)
                return Resultado<List<Usuario>>.Ok([]);

            if (!r.IsSuccessStatusCode)
                return Resultado<List<Usuario>>.Falla(await MensajeDe(r));

            // EL SOBRE DEL CONTRATO: { tabla, limite, total, datos[] }.
            // La API no devuelve un arreglo pelado.
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos")
                             .Deserialize<List<Usuario>>(Opciones) ?? [];
            return Resultado<List<Usuario>>.Ok(datos);
        }
        catch (Exception)
        {
            // La API apagada llega aqui. La interfaz gráfica tiene que SEGUIR EN PIE.
            return Resultado<List<Usuario>>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    public async Task<Resultado<Usuario>> CrearAsync(Usuario e)
    {
        try
        {
            Autorizar();
            var r = await cliente.PostAsJsonAsync(Ruta, e);
            return r.IsSuccessStatusCode
                ? Resultado<Usuario>.Ok(e)
                : Resultado<Usuario>.Falla(await MensajeDe(r));
        }
        catch (Exception) { return Resultado<Usuario>.Falla("No se pudo conectar con el servicio."); }
    }

    /// <summary>El PUT: reemplazo COMPLETO. Si falta un campo, la API
    /// responde 422 — y eso distingue este metodo del siguiente.</summary>
    public async Task<Resultado<Usuario>> ReemplazarAsync(string clave, Usuario e)
    {
        try
        {
            Autorizar();
            var r = await cliente.PutAsJsonAsync($"{Ruta}/{clave}", e);
            return r.IsSuccessStatusCode
                ? Resultado<Usuario>.Ok(e)
                : Resultado<Usuario>.Falla(await MensajeDe(r));
        }
        catch (Exception) { return Resultado<Usuario>.Falla("No se pudo conectar con el servicio."); }
    }

    /// <summary>El PATCH: parcial. El MISMO cuerpo que el PUT rechazaria con
    /// 422 aqui responde 200.</summary>
    public async Task<Resultado<bool>> ActualizarAsync(string clave, object parcial)
    {
        try
        {
            Autorizar();
            var r = await cliente.PatchAsJsonAsync($"{Ruta}/{clave}", parcial);
            return r.IsSuccessStatusCode
                ? Resultado<bool>.Ok(true)
                : Resultado<bool>.Falla(await MensajeDe(r));
        }
        catch (Exception) { return Resultado<bool>.Falla("No se pudo conectar con el servicio."); }
    }

    public async Task<Resultado<bool>> EliminarAsync(string clave)
    {
        try
        {
            Autorizar();
            var r = await cliente.DeleteAsync($"{Ruta}/{clave}");
            return r.IsSuccessStatusCode
                ? Resultado<bool>.Ok(true)
                : Resultado<bool>.Falla(await MensajeDe(r));
        }
        catch (Exception) { return Resultado<bool>.Falla("No se pudo conectar con el servicio."); }
    }

    /// <summary>Saca el mensaje del DOMINIO del sobre de error. Nunca
    /// devuelve «Error 422»: eso no le dice nada a una persona.</summary>
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
            HttpStatusCode.NotFound => "No se encontro ese registro.",
            HttpStatusCode.UnprocessableEntity => "Faltan datos obligatorios.",
            HttpStatusCode.BadRequest => "Los datos enviados no son validos.",
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
