using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioUsuarioConRoles
{
    Task<List<UsuarioConRoles>> ListarAsync();

    Task<UsuarioConRoles> ConsultarAsync(string email);

    Task<UsuarioConRoles> CrearAsync(string email, string contrasena, List<int> idsRol);

    Task<UsuarioConRoles> ActualizarAsync(string email, string? contrasena, List<int> idsRol);

    Task<string> EliminarAsync(string email);
}
