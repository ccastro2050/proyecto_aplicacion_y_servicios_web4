// ============================================================
// FacturaActualizar — la PETICIÓN del PATCH de factura.
//
// AQUÍ NO HAY UN SOLO [Required], y ésa es toda la diferencia con
// FacturaReemplazo: PATCH manda SOLO lo que cambia. Un body con
// `{"fkidvendedor": 3}` es válido; el cliente y los renglones se
// quedan como estaban.
//
// Los tipos son anulables (`int?`, `List<…>?`) para poder
// distinguir «no lo mandó» de «lo mandó vacío» — sin eso, un
// `fkidvendedor` ausente llegaría como 0 y borraría el que había.
//
// Quién mezcla lo viejo con lo nuevo es el SERVICIO, no este
// archivo ni el controlador: mezclar es una regla de negocio.
// Vea `ServicioFactura.ActualizarAsync`.
//
// Y el body vacío `{}` también es válido aquí: lo rechaza el
// servicio con un 400, no la validación de forma, porque «no
// mandó nada que cambiar» no es un error de FORMA.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class FacturaActualizar
{
    public int? Fkidcliente { get; set; }

    public int? Fkidvendedor { get; set; }

    // Si viene, tiene que traer al menos un renglón: una factura sin
    // renglones no existe. Si NO viene, no se toca el detalle.
    [MinLength(1, ErrorMessage = "La factura requiere mínimo 1 producto.")]
    public List<ProductoDeFacturaCrear>? Productos { get; set; }
}
