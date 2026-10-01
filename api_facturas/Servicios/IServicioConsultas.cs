using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioConsultas
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
