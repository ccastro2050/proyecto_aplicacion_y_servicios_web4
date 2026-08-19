// ============================================================
// ServicioUsuarioConRoles — las REGLAS. No sabe de HTTP ni de SQL.
//
// Su trabajo propio es UNO y vale la pena verlo: traduce la lista de enteros
// que viene del formulario —[1, 3]— al JSON que el procedimiento espera
// —[{"fkidrol":1},{"fkidrol":3}]—.
//
// Por qué aquí y no en el controlador: porque la forma del JSON es una regla
// del dominio, no del transporte. Y por qué no en el repositorio: porque el
// repositorio solo tiene que saber ejecutar, no decidir.
// ============================================================

using System.Text.Json;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioUsuarioConRoles : IServicioUsuarioConRoles
{
    private readonly IRepositorioUsuarioConRoles _repositorio;

    public ServicioUsuarioConRoles(IRepositorioUsuarioConRoles repositorio)
    {
        _repositorio = repositorio;
    }

    /// <summary>[1, 3] → [{"fkidrol":1},{"fkidrol":3}] — la clave `fkidrol` es
    /// la que el procedimiento abre con json_array_elements. Si se escribe
    /// `idrol`, el SP no encuentra nada y el usuario queda SIN roles, sin un
    /// solo error: se mira el plpgsql, no se adivina.</summary>
    private static string ARolesJson(List<int> idsRol) =>
        JsonSerializer.Serialize(idsRol.Select(id => new { fkidrol = id }));

    public Task<List<UsuarioConRoles>> ListarAsync() => _repositorio.ListarAsync();

    public Task<UsuarioConRoles> ConsultarAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");
        return _repositorio.ConsultarAsync(email.Trim());
    }

    public Task<UsuarioConRoles> CrearAsync(string email, string contrasena, List<int> idsRol)
    {
        if (idsRol.Count == 0)
            throw new ArgumentException("El usuario requiere mínimo un rol.");

        // Roles repetidos reventarían la PK compuesta de rol_usuario con un
        // 409 confuso. Se quitan aquí: marcar dos veces la misma casilla no es
        // un error que merezca un mensaje.
        return _repositorio.CrearAsync(email.Trim(), contrasena,
                                       ARolesJson(idsRol.Distinct().ToList()));
    }

    public Task<UsuarioConRoles> ActualizarAsync(string email, string? contrasena, List<int> idsRol)
    {
        if (idsRol.Count == 0)
            throw new ArgumentException("El usuario requiere mínimo un rol.");
        return _repositorio.ActualizarAsync(email.Trim(), contrasena,
                                            ARolesJson(idsRol.Distinct().ToList()));
    }

    public Task<string> EliminarAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");
        return _repositorio.EliminarAsync(email.Trim());
    }
}
