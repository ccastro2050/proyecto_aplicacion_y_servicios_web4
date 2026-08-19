// ServicioCliente — la capa de NEGOCIO de cliente (v3). Calcado del molde:
// valida argumentos, delega por la interfaz, traduce "no existe".

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioCliente : IServicioCliente
{
    private readonly IRepositorioCliente _repositorio;

    public ServicioCliente(IRepositorioCliente repositorio)
    {
        _repositorio = repositorio;
    }

    private static void ValidarId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("El id debe ser un entero mayor que cero.");
        }
    }

    public async Task<List<Cliente>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodosAsync(limite);
    }

    public async Task<Cliente> ObtenerAsync(int id)
    {
        ValidarId(id);
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad == null)
        {
            throw new NoEncontradoExcepcion($"No existe un cliente con id = {id}");
        }
        return entidad;
    }

    public async Task CrearAsync(Cliente entidad)
    {
        await _repositorio.CrearAsync(entidad);
    }

    public async Task<int> ActualizarAsync(int id, Dictionary<string, object> datos)
    {
        ValidarId(id);
        if (datos.Count == 0)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }
        var filas = await _repositorio.ActualizarAsync(id, datos);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un cliente con id = {id}");
        }
        return filas;
    }

    public async Task<int> EliminarAsync(int id)
    {
        ValidarId(id);
        var filas = await _repositorio.EliminarAsync(id);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un cliente con id = {id}");
        }
        return filas;
    }
}
