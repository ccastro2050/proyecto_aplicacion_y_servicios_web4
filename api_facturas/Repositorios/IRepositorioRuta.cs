// IRepositorioRuta — contrato de datos de la rebanada ruta (v3).
// CALCADO del molde: mismas 5 operaciones, motor invisible para el servicio.

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioRuta
{
    Task<List<Ruta>> ObtenerTodasAsync(int limite);
    Task<Ruta?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Ruta entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
