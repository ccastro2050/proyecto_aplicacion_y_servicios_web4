// ============================================================
// IRepositorioFactura — el CONTRATO de datos de factura.
//
// Distinto a los otros repositorios: aquí NO hay SQL de tablas —
// la factura es maestro-detalle y su lógica vive en la BD, así
// que este contrato son 4 llamadas a PROCEDIMIENTOS ALMACENADOS.
//
// Excepciones que puede lanzar la implementación (traduciendo los
// THROW de los SPs — ver 3_plan.md §3.4 de la v2):
//   NoEncontradoExcepcion → la factura no existe        → 404
//   ConflictoExcepcion    → ya está anulada             → 409
//   SqlException          → stock insuficiente, FK, etc. → 500
// ============================================================

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioFactura
{
    /// <summary>Todas las facturas con nombres y detalle anidado
    /// (sp_listar_facturas_y_productosporfactura).</summary>
    Task<List<Factura>> ListarAsync();

    /// <summary>UNA factura completa (sp_consultar_factura_y_productosporfactura).
    /// NoEncontradoExcepcion si el número no existe.</summary>
    Task<Factura> ConsultarAsync(int numero);

    /// <summary>Crea encabezado + renglones en UNA transacción del SP
    /// (sp_insertar_factura_y_productosporfactura). El trigger calcula
    /// subtotales, total y stock. 'productosJson' es el detalle como
    /// JSON: [{"codigo":"PR001","cantidad":2},…]. Devuelve la factura
    /// creada, ya calculada.</summary>
    Task<Factura> CrearAsync(int fkidcliente, int fkidvendedor, string productosJson);

    /// <summary>Borrado LÓGICO (sp_anular_factura): restaura stock y pone
    /// estado='anulada'. Devuelve el JSON del SP tal cual (mensaje,
    /// número, total anulado…). ConflictoExcepcion si ya estaba anulada.</summary>
    Task<string> AnularAsync(int numero);

    /// <summary>Reemplaza la factura ENTERA —encabezado y renglones— en
    /// una transacción del SP (sp_actualizar_factura_y_productosporfactura).
    /// El SP devuelve el stock de los renglones viejos, borra el detalle,
    /// inserta el nuevo y deja que el trigger recalcule todo.
    /// NoEncontradoExcepcion si el número no existe.</summary>
    Task<Factura> ReemplazarAsync(int numero, int fkidcliente, int fkidvendedor,
                                  string productosJson);

    /// <summary>Borrado FÍSICO (sp_borrar_factura_y_productosporfactura): la
    /// fila desaparece y el stock vuelve a los productos. Devuelve el JSON del
    /// SP. No confundir con AnularAsync, que la deja en la base.</summary>
    Task<string> EliminarAsync(int numero);
}
