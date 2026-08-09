// ============================================================
// IServicioPersona — el CONTRATO de negocio de persona.
//
// CALCADO de IServicioProducto. Mismas excepciones de negocio:
//   ArgumentException     → 400
//   NoEncontradoExcepcion → 404
//   SqlException y demás  → 500
// ============================================================

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioPersona
{
    /// <summary>Hasta 'limite' personas. ArgumentException si limite &lt;= 0.</summary>
    Task<List<Persona>> ListarAsync(int limite);

    /// <summary>La Persona con ese código. NoEncontradoExcepcion si no existe.</summary>
    Task<Persona> ObtenerAsync(string codigo);

    /// <summary>Crea la persona (el body ya fue validado por PersonaCrear).</summary>
    Task CrearAsync(Persona persona);

    /// <summary>Escribe los campos enviados (PUT todos, PATCH un subconjunto).
    /// ArgumentException si no llegó ningún campo · NoEncontradoExcepcion si
    /// el código no existe · devuelve filas afectadas.</summary>
    Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos);

    /// <summary>Elimina. NoEncontradoExcepcion si no existe · devuelve filas eliminadas.</summary>
    Task<int> EliminarAsync(string codigo);
}
