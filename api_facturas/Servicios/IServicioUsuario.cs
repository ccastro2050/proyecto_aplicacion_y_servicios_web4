// IServicioUsuario — contrato de negocio de usuario (v3).
// ArgumentException → 400 · NoEncontradoExcepcion → 404 · resto → 500.
// VerificarContrasenaAsync devuelve el trío del contrato: 200/401/404.

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioUsuario
{
    Task<List<Usuario>> ListarAsync(int limite);
    Task<Usuario> ObtenerAsync(string email);
    Task CrearAsync(string email, string contrasena);
    Task<int> ActualizarContrasenaAsync(string email, string? contrasena);
    Task<int> EliminarAsync(string email);

    /// <summary>(200, "Contraseña válida.") · (401, "Contraseña incorrecta.")
    /// · (404, "Usuario no encontrado.")</summary>
    Task<(int Codigo, string Mensaje)> VerificarContrasenaAsync(string email, string contrasena);
}
