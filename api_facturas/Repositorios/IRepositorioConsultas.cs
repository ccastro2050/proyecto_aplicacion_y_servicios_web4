using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

// ============================================================
// IRepositorioConsultas — las DIEZ preguntas de la v4.
//
// Un metodo por consulta, con su nombre: `VentasPorProductoAsync`. NO un
// `EjecutarAsync(string nombre)` generico, y la razon es la misma por la que
// la API tiene un endpoint por recurso y no `/api/{tabla}`: con el metodo
// generico, quien lee esta interfaz no sabe que consultas existen ni que
// devuelve cada una, y el compilador no puede ayudarle.
// ============================================================
public interface IRepositorioConsultas
{
    Task<List<VentaPorProducto>> VentasPorProductoAsync();

    Task<List<VentaPorCliente>> VentasPorClienteAsync();

    Task<List<VentaPorVendedor>> VentasPorVendedorAsync();

    Task<List<VentaPorEmpresa>> VentasPorEmpresaAsync();

    Task<List<TicketPorVendedor>> TicketPorVendedorAsync();

    Task<List<ProductoSinVender>> ProductosSinVenderAsync();

    Task<List<AnulacionPorCliente>> AnulacionesPorClienteAsync();

    Task<List<AlcanceDeUsuario>> AlcanceDeUsuariosAsync();

    Task<List<InterfazSinUsuarios>> InterfacesSinUsuariosAsync();

    Task<List<CreditoContraConsumo>> CreditoContraConsumoAsync();
}
