// ============================================================
// ServicioConsultas — y aqui la capa de servicio casi no hace nada.
//
// Conviene decirlo en vez de disimularlo: estas diez operaciones son de SOLO
// LECTURA y no tienen ninguna regla de negocio que aplicar. El servicio pasa
// la llamada al repositorio y ya.
//
// ENTONCES, ¿POR QUE EXISTE? Por dos razones, y ninguna es «porque las otras
// rebanadas lo tienen»:
//
//   1. El controlador sigue sin conocer el repositorio. Si llamara directo a
//      IRepositorioConsultas, la frontera se rompe en UN sitio — y las
//      fronteras se rompen asi, de a una excepcion razonable.
//
//   2. El dia que una de estas consultas necesite una regla -«solo el ultimo
//      trimestre», «no mostrar clientes dados de baja»- hay donde ponerla, y
//      no hay que reacomodar el controlador para meterla.
//
// Un servicio que solo delega NO es codigo muerto: es una frontera sostenida.
// Lo que seria un error es ponerle logica al controlador «porque total, es
// solo un SELECT».
// ============================================================

using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioConsultas : IServicioConsultas
{
    private readonly IRepositorioConsultas _repositorio;

    public ServicioConsultas(IRepositorioConsultas repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<List<VentaPorProducto>> VentasPorProductoAsync() => _repositorio.VentasPorProductoAsync();
    public Task<List<VentaPorCliente>> VentasPorClienteAsync() => _repositorio.VentasPorClienteAsync();
    public Task<List<VentaPorVendedor>> VentasPorVendedorAsync() => _repositorio.VentasPorVendedorAsync();
    public Task<List<VentaPorEmpresa>> VentasPorEmpresaAsync() => _repositorio.VentasPorEmpresaAsync();
    public Task<List<TicketPorVendedor>> TicketPorVendedorAsync() => _repositorio.TicketPorVendedorAsync();
    public Task<List<ProductoSinVender>> ProductosSinVenderAsync() => _repositorio.ProductosSinVenderAsync();
    public Task<List<AnulacionPorCliente>> AnulacionesPorClienteAsync() => _repositorio.AnulacionesPorClienteAsync();
    public Task<List<AlcanceDeUsuario>> AlcanceDeUsuariosAsync() => _repositorio.AlcanceDeUsuariosAsync();
    public Task<List<InterfazSinUsuarios>> InterfacesSinUsuariosAsync() => _repositorio.InterfacesSinUsuariosAsync();
    public Task<List<CreditoContraConsumo>> CreditoContraConsumoAsync() => _repositorio.CreditoContraConsumoAsync();
}
