// ServicioRutaRol — la capa de NEGOCIO del puente rutarol (v3).
// Las búsquedas sin resultados son 404 (el recurso preguntado no
// tiene asignaciones) — lo decide el negocio, no el repositorio.

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioRutaRol : IServicioRutaRol
{
    private readonly IRepositorioRutaRol _repositorio;

    public ServicioRutaRol(IRepositorioRutaRol repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<RutaRol>> ListarAsync(int limite)
    {
        if (limite <= 0)
        {
            throw new ArgumentException("El límite debe ser un entero mayor que cero.");
        }
        return await _repositorio.ObtenerTodosAsync(limite);
    }

    public async Task<List<RutaRol>> ObtenerPorRutaAsync(int fkidruta)
    {
        if (fkidruta <= 0)
        {
            throw new ArgumentException("El id de ruta debe ser un entero mayor que cero.");
        }
        var lista = await _repositorio.ObtenerPorRutaAsync(fkidruta);
        if (lista.Count == 0)
        {
            throw new NoEncontradoExcepcion($"No hay asignaciones en rutarol para ruta = {fkidruta}");
        }
        return lista;
    }

    public async Task<List<RutaRol>> ObtenerPorRolAsync(int fkidrol)
    {
        if (fkidrol <= 0)
        {
            throw new ArgumentException("El id de rol debe ser un entero mayor que cero.");
        }
        var lista = await _repositorio.ObtenerPorRolAsync(fkidrol);
        if (lista.Count == 0)
        {
            throw new NoEncontradoExcepcion($"No hay asignaciones en rutarol para rol = {fkidrol}");
        }
        return lista;
    }

    public async Task CrearAsync(RutaRol asignacion)
    {
        await _repositorio.CrearAsync(asignacion);
    }

    public async Task<int> EliminarAsync(int fkidruta, int fkidrol)
    {
        if (fkidruta <= 0)
        {
            throw new ArgumentException("El id de ruta debe ser un entero mayor que cero.");
        }
        var filas = await _repositorio.EliminarAsync(fkidruta, fkidrol);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion($"No existe la pareja ({fkidruta}, {fkidrol}) en rutarol");
        }
        return filas;
    }

    public async Task<int> ReemplazarAsync(int fkidruta, int fkidrol, RutaRol nueva)
    {
        // El PUT trae la pareja ENTERA: no hay nada que conservar.
        return await MoverAsync(fkidruta, fkidrol, nueva.Fkidruta, nueva.Fkidrol);
    }

    public async Task<int> ActualizarAsync(int fkidruta, int fkidrol,
                                           int? nuevaRuta, int? nuevoRol)
    {
        // EL PATCH DE UNA TABLA PUENTE: llega UNO de los dos lados y el otro se
        // conserva. Pasarle una ruta a otro rol es cambiar el idrol; cambiarle
        // la ruta al mismo rol es cambiar el idruta.
        if (nuevaRuta == null && nuevoRol == null)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }
        return await MoverAsync(fkidruta, fkidrol,
                                nuevaRuta ?? fkidruta, nuevoRol ?? fkidrol);
    }

    public async Task<int> ReemplazarDeRolAsync(int fkidrol, List<int> idsRuta)
    {
        if (fkidrol <= 0)
        {
            throw new ArgumentException("El id de rol debe ser un entero mayor que cero.");
        }
        if (idsRuta.Any(x => x <= 0))
        {
            throw new ArgumentException("Los ids de ruta deben ser enteros mayores que cero.");
        }

        // UNA LISTA VACÍA SÍ SE ACEPTA, y hay que decir por qué: dejar a un rol
        // sin ninguna ruta es una decisión legítima —un rol recién creado, o
        // uno al que se le revoca todo—. Lo que no se acepta es un id que no
        // sea un id.
        return await _repositorio.ReemplazarDeRolAsync(fkidrol, idsRuta);
    }

    /// <summary>Lo comun al PUT y al PATCH: validar y mover.</summary>
    private async Task<int> MoverAsync(int rutaVieja, int rolViejo,
                                       int rutaNueva, int rolNuevo)
    {
        if (rutaVieja <= 0 || rutaNueva <= 0)
        {
            throw new ArgumentException("El id de ruta debe ser un entero mayor que cero.");
        }
        if (rolViejo <= 0 || rolNuevo <= 0)
        {
            throw new ArgumentException("El id de rol debe ser un entero mayor que cero.");
        }

        var filas = await _repositorio.ReemplazarAsync(rutaVieja, rolViejo,
                                                       rutaNueva, rolNuevo);
        if (filas == 0)
        {
            throw new NoEncontradoExcepcion(
                $"No existe la pareja ({rutaVieja}, {rolViejo}) en rutarol");
        }
        return filas;
    }
}
