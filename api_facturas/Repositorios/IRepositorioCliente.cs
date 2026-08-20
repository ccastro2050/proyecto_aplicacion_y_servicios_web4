// IRepositorioCliente — contrato de datos de la rebanada cliente (v3).
// CALCADO del molde: mismas 5 operaciones, motor invisible para el servicio.

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioCliente
{
    Task<List<Cliente>> ObtenerTodosAsync(int limite);
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Cliente entidad);
    Task<int> ActualizarAsync(int id, Dictionary<string, object> datos);
    Task<int> EliminarAsync(int id);
}
