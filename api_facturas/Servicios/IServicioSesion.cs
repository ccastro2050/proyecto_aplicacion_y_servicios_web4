using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioSesion
{
    /// <summary>Devuelve la sesión si las credenciales sirven, o `null` si no.
    ///
    /// UN SOLO `null` PARA LOS DOS CASOS —el correo no existe y la contraseña
    /// está mal— y es deliberado: ver abajo.</summary>
    Task<Sesion?> IniciarAsync(string email, string contrasena);
}
