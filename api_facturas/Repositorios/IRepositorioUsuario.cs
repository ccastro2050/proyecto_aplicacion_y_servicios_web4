// ============================================================
// IRepositorioUsuario — el contrato de datos de la entidad con
// secreto (v3). Distinto al molde: la contraseña entra en claro
// por parámetros y NUNCA sale (las lecturas devuelven Usuario,
// que solo tiene email).
// ============================================================

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioUsuario
{
    /// <summary>Hasta 'limite' usuarios — SOLO sus emails.</summary>
    Task<List<Usuario>> ObtenerTodosAsync(int limite);

    /// <summary>El usuario (solo email), o null si no existe.</summary>
    Task<Usuario?> ObtenerPorEmailAsync(string email);

    /// <summary>Inserta el usuario guardando el HASH de la contraseña.</summary>
    Task CrearAsync(string email, string contrasena);

    /// <summary>Re-hashea y guarda la contraseña nueva (la usan PUT y
    /// PATCH). Devuelve filas afectadas (0 = el email no existe).</summary>
    Task<int> ActualizarContrasenaAsync(string email, string contrasena);

    /// <summary>Elimina. Devuelve filas eliminadas (0 = no existía).</summary>
    Task<int> EliminarAsync(string email);

    /// <summary>Compara la contraseña contra el hash almacenado.
    /// null = el usuario no existe · true/false = coincide o no.</summary>
    Task<bool?> VerificarContrasenaAsync(string email, string contrasena);
}
