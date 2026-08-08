// ============================================================
// ServicioPersona — la capa de NEGOCIO de persona.
//
// CALCADO de ServicioProducto (allí está la lección completa de
// la inversión de dependencias y de las excepciones de negocio).
// Mismas reglas: límite > 0, código no vacío, PATCH sin campos
// → ArgumentException, "no existe" → NoEncontradoExcepcion.
// ============================================================

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioPersona : IServicioPersona
{
    private readonly IRepositorioPersona _repositorio;

    public ServicioPersona(IRepositorioPersona repositorio)
    {
        _repositorio = repositorio;
    }

    private static string ValidarCodigo(string codigo)
    {
        codigo = codigo.Trim();
        if (codigo == "")
        {
            throw new ArgumentException("El código de la persona no puede estar vacío.");
        }
        return codigo;
    }

    public async Task<List<Persona>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodasAsync(limite);
    }

    public async Task<Persona> ObtenerAsync(string codigo)
    {
        codigo = ValidarCodigo(codigo);
        var persona = await _repositorio.ObtenerPorCodigoAsync(codigo);
        if (persona == null)
        {
            throw new NoEncontradoExcepcion($"No existe una persona con codigo = {codigo}");
        }
        return persona;
    }

    public async Task CrearAsync(Persona persona)
    {
        await _repositorio.CrearAsync(persona);
    }

    public async Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        codigo = ValidarCodigo(codigo);
        if (datos.Count == 0)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }
        var filasAfectadas = await _repositorio.ActualizarAsync(codigo, datos);
        if (filasAfectadas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una persona con codigo = {codigo}");
        }
        return filasAfectadas;
    }

    public async Task<int> EliminarAsync(string codigo)
    {
        codigo = ValidarCodigo(codigo);
        var filasEliminadas = await _repositorio.EliminarAsync(codigo);
        if (filasEliminadas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe una persona con codigo = {codigo}");
        }
        return filasEliminadas;
    }
}
