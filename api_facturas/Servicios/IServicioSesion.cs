using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioSesion
{
    /// <summary>Devuelve la sesión si las credenciales sirven, o `null` si no.
    ///
    /// UN SOLO `null` PARA LOS DOS CASOS —el correo no existe y la contraseña
    /// está mal— y es deliberado: ver abajo.</summary>
    Task<Sesion?> IniciarAsync(string email, string contrasena);

    /// <summary>RENUEVA el token de alguien que YA está identificado: no pide
    /// contraseña porque el token válido ya la probó. Se usa para estirar la
    /// sesión antes de que venza, sin volver a la pantalla de entrar.
    /// Devuelve null si el usuario dejó de existir mientras tanto.</summary>
    Task<Sesion?> RenovarAsync(string email);
}
