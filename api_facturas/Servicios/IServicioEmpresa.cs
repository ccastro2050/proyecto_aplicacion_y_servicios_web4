// IServicioEmpresa — contrato de negocio de empresa (v3). Calcado del molde.
// ArgumentException → 400 · NoEncontradoExcepcion → 404 · resto → 500.

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioEmpresa
{
    Task<List<Empresa>> ListarAsync(int limite);
    Task<Empresa> ObtenerAsync(string codigo);
    Task CrearAsync(Empresa entidad);
    Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos);
    Task<int> EliminarAsync(string codigo);
}
