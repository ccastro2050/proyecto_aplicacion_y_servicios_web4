// Rol — rebanada de la v3, CALCADA del molde (la lección completa
// está en los archivos gemelos de producto/persona).

namespace ApiFacturas.Modelos;

public class Rol
{
    /// <summary>La llave primaria: la genera la BD (IDENTITY).</summary>
    public int Id { get; set; }

    public required string Nombre { get; set; }
}
