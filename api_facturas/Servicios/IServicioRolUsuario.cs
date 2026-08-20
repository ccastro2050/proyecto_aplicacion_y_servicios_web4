// IServicioRolUsuario — contrato de negocio del puente rol_usuario (v3).

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioRolUsuario
{
    Task<List<RolUsuario>> ListarAsync(int limite);
    Task<List<RolUsuario>> ObtenerPorUsuarioAsync(string fkemail);
    Task<List<RolUsuario>> ObtenerPorRolAsync(int fkidrol);
    Task CrearAsync(RolUsuario asignacion);
    Task<int> EliminarAsync(string fkemail, int fkidrol);

    /// <summary>PUT: mueve la asignación a la pareja que llega entera.
    /// NoEncontradoExcepcion si la pareja vieja no existe.</summary>
    Task<int> ReemplazarAsync(string fkemail, int fkidrol, RolUsuario nueva);

    /// <summary>PATCH: mueve SOLO el lado que llegó —el usuario o el rol—
    /// y conserva el otro. ArgumentException si no llegó ninguno.</summary>
    Task<int> ActualizarAsync(string fkemail, int fkidrol,
                              string? nuevoEmail, int? nuevoRol);
}
