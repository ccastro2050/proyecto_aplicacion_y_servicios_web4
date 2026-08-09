// ServicioEmpresa — la capa de NEGOCIO de empresa (v3). Calcado del molde:
// valida argumentos, delega por la interfaz, traduce "no existe".

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioEmpresa : IServicioEmpresa
{
    private readonly IRepositorioEmpresa _repositorio;

    public ServicioEmpresa(IRepositorioEmpresa repositorio)
    {
        _repositorio = repositorio;
    }

    private static string ValidarCodigo(string codigo)
    {
        codigo = codigo.Trim();
        if (codigo == "")
        {
            throw new ArgumentException("El código no puede estar vacío.");
        }
        return codigo;
    }

    public async Task<List<Empresa>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodasAsync(limite);
    }

    public async Task<Empresa> ObtenerAsync(string codigo)
    {
        codigo = ValidarCodigo(codigo);
        var entidad = await _repositorio.ObtenerPorCodigoAsync(codigo);
        if (entidad == null)
        {
            throw new NoEncontradoExcepcion($"No existe una empresa con codigo = {codigo}");
        }
        return entidad;
    }

    public async Task CrearAsync(Empresa entidad)
    {
        await _repositorio.CrearAsync(entidad);
    }

    public async Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        codigo = ValidarCodigo(codigo);
        if (datos.Count == 0)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }
        var filas = await _repositorio.ActualizarAsync(codigo, datos);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una empresa con codigo = {codigo}");
        }
        return filas;
    }

    public async Task<int> EliminarAsync(string codigo)
    {
        codigo = ValidarCodigo(codigo);
        var filas = await _repositorio.EliminarAsync(codigo);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una empresa con codigo = {codigo}");
        }
        return filas;
    }
}
