// IServicioRuta — contrato de negocio de ruta (v3). Calcado del molde.
// ArgumentException → 400 · NoEncontradoExcepcion → 404 · resto → 500.

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioRuta
{
    Task<List<Ruta>> ListarAsync(int limite);
    Task<Ruta> ObtenerAsync(int id);
    Task CrearAsync(Ruta entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
