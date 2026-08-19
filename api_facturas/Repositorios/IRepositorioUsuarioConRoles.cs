// El CONTRATO de datos de usuario-con-roles. Cinco operaciones, una por
// procedimiento almacenado — ni una más.

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioUsuarioConRoles
{
    Task<List<UsuarioConRoles>> ListarAsync();

    Task<UsuarioConRoles> ConsultarAsync(string email);

    Task<UsuarioConRoles> CrearAsync(string email, string contrasena, string rolesJson);

    Task<UsuarioConRoles> ActualizarAsync(string email, string? contrasena, string rolesJson);

    Task<string> EliminarAsync(string email);
}
