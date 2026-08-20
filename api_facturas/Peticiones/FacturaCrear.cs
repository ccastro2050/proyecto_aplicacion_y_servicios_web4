// ============================================================
// FacturaCrear — la PETICIÓN del POST de factura (maestro-detalle).
//
// Novedad de la v2: una petición con LISTA ANIDADA. ASP.NET valida
// la lista ([Required] + [MinLength(1)]) Y cada elemento (las
// anotaciones de ProductoDeFacturaCrear) — un body con productos
// vacíos o cantidad 0 muere en 422 antes del controlador.
//
// Fíjese en lo que NO viaja: ni subtotales, ni total, ni fecha —
// todo eso lo calcula la BD (trigger + SP). El cliente solo dice
// QUÉ compra QUIÉN.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class FacturaCrear
{
    [Required(ErrorMessage = "El campo fkidcliente es obligatorio.")]
    public int? Fkidcliente { get; set; }

    [Required(ErrorMessage = "El campo fkidvendedor es obligatorio.")]
    public int? Fkidvendedor { get; set; }

    [Required(ErrorMessage = "El campo productos es obligatorio.")]
    [MinLength(1, ErrorMessage = "La factura requiere mínimo 1 producto.")]
    public List<ProductoDeFacturaCrear>? Productos { get; set; }
}

// La petición de UN renglón (anidada en la lista de arriba).
// Vive en este mismo archivo porque solo existe como parte de
// FacturaCrear — nunca llega sola.
public class ProductoDeFacturaCrear
{
    [Required(ErrorMessage = "El campo codigo del producto es obligatorio.")]
    [StringLength(10, MinimumLength = 1,
        ErrorMessage = "El campo codigo debe tener entre 1 y 10 caracteres.")]
    public string? Codigo { get; set; }

    [Required(ErrorMessage = "El campo cantidad es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El campo cantidad debe ser un entero mayor o igual a 1.")]
    public int? Cantidad { get; set; }
}
