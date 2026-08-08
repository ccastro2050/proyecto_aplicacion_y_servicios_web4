// Empresa — rebanada de la v3, CALCADA del molde (la lección completa
// está en los archivos gemelos de producto/persona).

namespace ApiFacturas.Modelos;

public class Empresa
{
    /// <summary>Identificador único (la llave primaria).</summary>
    public required string Codigo { get; set; }

    public required string Nombre { get; set; }
}
