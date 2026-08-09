// IRepositorioRol — contrato de datos de la rebanada rol (v3).
// CALCADO del molde: mismas 5 operaciones, motor invisible para el servicio.

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioRol
{
    Task<List<Rol>> ObtenerTodosAsync(int limite);
    Task<Rol?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Rol entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
