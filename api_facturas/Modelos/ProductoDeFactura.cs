// ============================================================
// ProductoDeFactura — UN renglón del detalle de una factura.
//
// Es lo que el SP devuelve por cada fila de productosporfactura,
// con el nombre y el precio del producto ya resueltos (JOIN en la
// BD). El subtotal viene CALCULADO por el trigger.
// ============================================================

using System.Text.Json.Serialization;

namespace ApiFacturas.Modelos;

public class ProductoDeFactura
{
    [JsonPropertyName("codigo_producto")]
    public string? CodigoProducto { get; set; }

    [JsonPropertyName("nombre_producto")]
    public string? NombreProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal Valorunitario { get; set; }

    /// <summary>cantidad × valorunitario — lo calculó el trigger.</summary>
    public decimal Subtotal { get; set; }
}
