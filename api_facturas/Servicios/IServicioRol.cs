// IServicioRol — contrato de negocio de rol (v3). Calcado del molde.
// ArgumentException → 400 · NoEncontradoExcepcion → 404 · resto → 500.

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioRol
{
    Task<List<Rol>> ListarAsync(int limite);
    Task<Rol> ObtenerAsync(int id);
    Task CrearAsync(Rol entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
