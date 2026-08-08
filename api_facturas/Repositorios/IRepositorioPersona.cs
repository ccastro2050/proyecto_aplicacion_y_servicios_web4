// ============================================================
// IRepositorioPersona — el CONTRATO de datos de persona.
//
// CALCADO de IRepositorioProducto (la lección de interfaces e
// inversión de dependencias está allí). Mismos 5 métodos async.
// ============================================================

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioPersona
{
    /// <summary>Hasta 'limite' personas ordenadas por código.</summary>
    Task<List<Persona>> ObtenerTodasAsync(int limite);

    /// <summary>La Persona con ese código, o null si no existe.</summary>
    Task<Persona?> ObtenerPorCodigoAsync(string codigo);

    /// <summary>Inserta la persona (llega como objeto del modelo).</summary>
    Task CrearAsync(Persona persona);

    /// <summary>Escribe los campos del diccionario (los usan PUT y PATCH).
    /// Devuelve filas afectadas (0 = no existe).</summary>
    Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos);

    /// <summary>Elimina la persona. Devuelve filas eliminadas (0 = no existía).
    /// Si la persona es cliente o vendedor, la BD rechaza por llave
    /// foránea (SqlException → el controlador la vuelve 500).</summary>
    Task<int> EliminarAsync(string codigo);
}
