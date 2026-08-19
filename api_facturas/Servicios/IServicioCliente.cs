// IServicioCliente — contrato de negocio de cliente (v3). Calcado del molde.
// ArgumentException → 400 · NoEncontradoExcepcion → 404 · resto → 500.

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioCliente
{
    Task<List<Cliente>> ListarAsync(int limite);
    Task<Cliente> ObtenerAsync(int id);
    Task CrearAsync(Cliente entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
