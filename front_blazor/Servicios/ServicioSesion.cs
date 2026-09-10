using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioSesion — el unico que habla con /api/sesion.
//
// Y es el UNICO servicio del front que funciona SIN token: no puede exigir lo
// que todavia no tiene. Los otros doce lo mandan en cada peticion.
// ============================================================
public class ServicioSesion(HttpClient cliente)
{
    private static readonly JsonSerializerOptions Opciones =
        new() { PropertyNameCaseInsensitive = true };

    public record Resultado<T>(bool Bien, T? Dato, string? Mensaje)
    {
        public static Resultado<T> Ok(T dato) => new(true, dato, null);
        public static Resultado<T> Falla(string mensaje) => new(false, default, mensaje);
    }

    public async Task<Resultado<RespuestaSesion>> IniciarAsync(string email, string contrasena)
    {
        try
        {
            var r = await cliente.PostAsJsonAsync("api/sesion",
                new { email, contrasena });

            if (r.StatusCode == HttpStatusCode.Unauthorized)
            {
                // El MISMO mensaje para el correo que no existe y para la
                // contrasena equivocada: lo decide la API, y la interfaz no lo
                // "mejora" averiguando cual fue.
                return Resultado<RespuestaSesion>.Falla(
                    "El correo o la contrasena no son correctos.");
            }
            if (!r.IsSuccessStatusCode)
            {
                return Resultado<RespuestaSesion>.Falla(
                    "No se pudo iniciar sesion. Intente de nuevo.");
            }

            var s = await r.Content.ReadFromJsonAsync<RespuestaSesion>(Opciones);
            return s is null || string.IsNullOrWhiteSpace(s.Token)
                ? Resultado<RespuestaSesion>.Falla("La respuesta llego sin token.")
                : Resultado<RespuestaSesion>.Ok(s);
        }
        catch (Exception)
        {
            return Resultado<RespuestaSesion>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    /// <summary>Las rutas de quien pregunta. El correo NO va como parametro: la
    /// API lo saca del token, para que nadie pueda preguntar por los permisos
    /// de otro.</summary>
    public async Task<List<string>> MisRutasAsync(string token)
    {
        try
        {
            var peticion = new HttpRequestMessage(HttpMethod.Get, "api/permisos/mios");
            peticion.Headers.Authorization = new("Bearer", token);
            var r = await cliente.SendAsync(peticion);
            if (!r.IsSuccessStatusCode)
            {
                return [];
            }
            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            return sobre.GetProperty("datos").Deserialize<List<string>>(Opciones) ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }
}
