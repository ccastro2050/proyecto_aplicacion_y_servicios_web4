// IRepositorioVendedor — contrato de datos de la rebanada vendedor (v3).
// CALCADO del molde: mismas 5 operaciones, motor invisible para el servicio.

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioVendedor
{
    Task<List<Vendedor>> ObtenerTodosAsync(int limite);
    Task<Vendedor?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Vendedor entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
