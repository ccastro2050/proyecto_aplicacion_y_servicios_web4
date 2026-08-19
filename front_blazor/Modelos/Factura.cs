using System.Text.Json.Serialization;

namespace FrontFacturas.Modelos;

// ============================================================
// Factura — el MAESTRO de un maestro-detalle.
//
// Las otras clases del front son planas. Esta trae una LISTA dentro:
// `Productos`. Eso es lo que la hace distinta, y es el caso que la v2
// existe para enseñar.
//
// TRES COSAS QUE EL FRONT NO CALCULA Y NO ENVÍA:
//
//   Total      lo pone el TRIGGER trg_actualizar_totales_y_stock.
//              Si el front lo enviara, habría dos fuentes de verdad —y el día
//              que no coincidan, gana la que nadie revisó.
//   Subtotal   igual: cantidad × valorunitario lo calcula la BD.
//   Fecha      la pone la BD al insertar.
//
// Y los NOMBRES —`nombre_cliente`, `nombre_producto`— tampoco los busca el
// front con una segunda petición: ya vienen resueltos porque los JOINs los
// hizo el procedimiento almacenado.
//
// [JsonPropertyName] es lo que enlaza las claves snake_case del SP con las
// propiedades PascalCase de C#. Sin él, `NombreCliente` llega null — y llega
// null EN SILENCIO: la columna sale en blanco y no hay ningún error.
// ============================================================
public class Factura
{
    /// <summary>El número: lo genera la BD (SERIAL).</summary>
    public int Numero { get; set; }

    public string? Fecha { get; set; }

    /// <summary>Σ subtotales. LO CALCULA EL TRIGGER — el front solo lo muestra.</summary>
    public decimal Total { get; set; }

    /// <summary>'activa' o 'anulada'. Una factura no se borra: se anula.</summary>
    public string? Estado { get; set; }

    public int Fkidcliente { get; set; }

    [JsonPropertyName("nombre_cliente")]
    public string? NombreCliente { get; set; }

    public int Fkidvendedor { get; set; }

    [JsonPropertyName("nombre_vendedor")]
    public string? NombreVendedor { get; set; }

    /// <summary>EL DETALLE: los renglones, anidados dentro del maestro.</summary>
    public List<ProductoDeFactura> Productos { get; set; } = [];
}

/// <summary>Un renglón del detalle, como lo devuelve el SP.</summary>
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

// ============================================================
// LO QUE SE ENVÍA AL CREAR, que NO es la clase de arriba.
//
// Se envía menos: el cliente, el vendedor y una lista de {codigo, cantidad}.
// Nada de total, ni de subtotales, ni de fecha, ni de nombres.
//
// Son dos clases porque son dos contratos distintos: uno de ida y otro de
// vuelta. Reusar `Factura` para enviar obligaría a dejar en null siete
// propiedades y el que lo lea no sabría cuáles importan.
// ============================================================
public class FacturaNueva
{
    public int Fkidcliente { get; set; }

    public int Fkidvendedor { get; set; }

    public List<RenglonNuevo> Productos { get; set; } = [];
}

/// <summary>Un renglón del formulario: SOLO el código y la cantidad.</summary>
public class RenglonNuevo
{
    public string Codigo { get; set; } = "";

    public int Cantidad { get; set; }

    /// <summary>Para mostrarle el nombre y el subtotal a la persona MIENTRAS
    /// arma la factura. No se envía —los marca [JsonIgnore]— porque el que
    /// manda es el cálculo de la base.</summary>
    [JsonIgnore]
    public string NombreParaMostrar { get; set; } = "";

    [JsonIgnore]
    public decimal PrecioParaMostrar { get; set; }
}
