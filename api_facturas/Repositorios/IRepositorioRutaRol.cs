// IRepositorioRutaRol — contrato del puente rutarol (v3): el patrón
// nuevo — sin Actualizar, búsquedas por cada lado, y el DELETE
// exige LA PAREJA (ambas columnas de la PK compuesta).

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioRutaRol
{
    Task<List<RutaRol>> ObtenerTodosAsync(int limite);
    Task<List<RutaRol>> ObtenerPorRutaAsync(int fkidruta);
    Task<List<RutaRol>> ObtenerPorRolAsync(int fkidrol);
    Task CrearAsync(RutaRol asignacion);
    Task<int> EliminarAsync(int fkidruta, int fkidrol);   // ¡AMBAS columnas!

    /// <summary>MUEVE el permiso: borra la pareja vieja e inserta la nueva, en
    /// ese orden y en UNA transacción. En una tabla puente «actualizar» no es
    /// otra cosa — las dos columnas SON la llave.
    /// Devuelve las filas afectadas (0 = la pareja vieja no existía).</summary>
    Task<int> ReemplazarAsync(int fkidrutaViejo, int fkidrolViejo,
                              int fkidrutaNuevo, int fkidrolNuevo);

    /// <summary>REEMPLAZA todas las rutas de un rol por la lista que llega:
    /// borra las que tiene e inserta las nuevas, en UNA transacción. A mitad de
    /// camino un rol se queda sin poder entrar a nada, así que no puede
    /// quedarse a mitad. Devuelve cuántas rutas quedaron.</summary>
    Task<int> ReemplazarDeRolAsync(int fkidrol, List<int> idsRuta);
}
