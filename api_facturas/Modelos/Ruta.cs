// Ruta — rebanada de la v3, CALCADA del molde. Detalle de C#: una
// propiedad no puede llamarse igual que su clase, así que el path se
// llama Valor y [JsonPropertyName] lo publica como "ruta" en el JSON
// (el contrato manda).

using System.Text.Json.Serialization;

namespace ApiFacturas.Modelos;

public class Ruta
{
    /// <summary>La llave primaria: la genera la BD (IDENTITY).</summary>
    public int Id { get; set; }

    /// <summary>El path del sistema, ej. "/home" (columna: ruta, UNIQUE).</summary>
    [JsonPropertyName("ruta")]
    public required string Valor { get; set; }

    public required string Descripcion { get; set; }
}
