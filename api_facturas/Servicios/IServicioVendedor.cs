// IServicioVendedor — contrato de negocio de vendedor (v3). Calcado del molde.
// ArgumentException → 400 · NoEncontradoExcepcion → 404 · resto → 500.

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioVendedor
{
    Task<List<Vendedor>> ListarAsync(int limite);
    Task<Vendedor> ObtenerAsync(int id);
    Task CrearAsync(Vendedor entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
