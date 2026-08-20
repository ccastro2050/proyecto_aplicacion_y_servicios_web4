// ============================================================
// FacturaReemplazo — la PETICIÓN del PUT de factura.
//
// Misma forma que FacturaCrear, y eso NO es una copia inútil: es
// la semántica de PUT. Reemplazar exige la ficha COMPLETA, así que
// los tres campos siguen siendo obligatorios — un PUT al que le
// falte la lista de productos muere en 422, igual que un POST.
//
// El número NO viaja en el body: va en la URL. Mandarlo en los dos
// sitios abre la pregunta de cuál manda si no coinciden.
//
// Y lo que sigue sin viajar: subtotales, total y fecha. Los calcula
// la BD (SP + trigger), igual que al crear.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class FacturaReemplazo
{
    [Required(ErrorMessage = "El campo fkidcliente es obligatorio.")]
    public int? Fkidcliente { get; set; }

    [Required(ErrorMessage = "El campo fkidvendedor es obligatorio.")]
    public int? Fkidvendedor { get; set; }

    [Required(ErrorMessage = "El campo productos es obligatorio.")]
    [MinLength(1, ErrorMessage = "La factura requiere mínimo 1 producto.")]
    public List<ProductoDeFacturaCrear>? Productos { get; set; }
}
