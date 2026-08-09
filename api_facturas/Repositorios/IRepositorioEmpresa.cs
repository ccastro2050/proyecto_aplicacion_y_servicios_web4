// IRepositorioEmpresa — contrato de datos de la rebanada empresa (v3).
// CALCADO del molde: mismas 5 operaciones, motor invisible para el servicio.

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioEmpresa
{
    Task<List<Empresa>> ObtenerTodasAsync(int limite);
    Task<Empresa?> ObtenerPorCodigoAsync(string codigo);
    Task CrearAsync(Empresa entidad);
    Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos);
    Task<int> EliminarAsync(string codigo);
}
