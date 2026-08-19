// Cliente — rebanada de la v3, CALCADA del molde (la lección completa
// está en los archivos gemelos de producto/persona).

namespace ApiFacturas.Modelos;

public class Cliente
{
    /// <summary>La llave primaria: la genera la BD (IDENTITY).</summary>
    public int Id { get; set; }

    public decimal Credito { get; set; }

    public required string Fkcodpersona { get; set; }

    public string? Fkcodempresa { get; set; }
}
