// ServicioRol — la capa de NEGOCIO de rol (v3). Calcado del molde:
// valida argumentos, delega por la interfaz, traduce "no existe".

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioRol : IServicioRol
{
    private readonly IRepositorioRol _repositorio;

    public ServicioRol(IRepositorioRol repositorio)
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

    public async Task<List<Rol>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodosAsync(limite);
    }

    public async Task<Rol> ObtenerAsync(int id)
    {
        ValidarId(id);
        var entidad = await _repositorio.ObtenerPorIdAsync(id);
        if (entidad == null)
        {
            throw new NoEncontradoExcepcion($"No existe un rol con id = {id}");
        }
        return entidad;
    }

    public async Task CrearAsync(Rol entidad)
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
            throw new NoEncontradoExcepcion($"No existe un rol con id = {id}");
        }
        return filas;
    }

    public async Task<int> EliminarAsync(int id)
    {
        ValidarId(id);
        var filas = await _repositorio.EliminarAsync(id);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe un rol con id = {id}");
        }
        return filas;
    }
}
