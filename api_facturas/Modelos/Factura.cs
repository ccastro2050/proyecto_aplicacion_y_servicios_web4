// ============================================================
// Factura — el MODELO del maestro-detalle que devuelven los SPs.
//
// A diferencia de Producto/Persona (que la API arma con SELECT),
// una Factura la arma LA BASE DE DATOS: los procedimientos
// almacenados devuelven un JSON con el encabezado, los NOMBRES de
// cliente/vendedor ya resueltos (los JOINs los hizo el SP) y el
// detalle anidado. Esta clase es ese JSON, tipado.
//
// [JsonPropertyName] enlaza las claves snake_case del SP
// ("nombre_cliente") con las propiedades PascalCase de C# — y
// también fija esas mismas claves al responder al cliente (el
// contrato de 6_contracts.md usa las claves del SP).
// ============================================================

using System.Text.Json.Serialization;

namespace ApiFacturas.Modelos;

public class Factura
{
    /// <summary>Número de la factura (IDENTITY: lo genera la BD).</summary>
    public int Numero { get; set; }

    /// <summary>Fecha en texto ISO (así viaja en el JSON del SP).</summary>
    public string? Fecha { get; set; }

    /// <summary>Total = Σ subtotales — lo calcula el TRIGGER, jamás la API.</summary>
    public decimal Total { get; set; }

    /// <summary>'activa' o 'anulada' (el borrado de facturas es lógico).</summary>
    public string? Estado { get; set; }

    public int Fkidcliente { get; set; }

    [JsonPropertyName("nombre_cliente")]
    public string? NombreCliente { get; set; }

    public int Fkidvendedor { get; set; }

    [JsonPropertyName("nombre_vendedor")]
    public string? NombreVendedor { get; set; }

    /// <summary>El DETALLE: los renglones de productosporfactura.</summary>
    public List<ProductoDeFactura> Productos { get; set; } = new();
}
