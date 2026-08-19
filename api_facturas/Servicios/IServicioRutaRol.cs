// IServicioRutaRol — contrato de negocio del puente rutarol (v3).

using ApiFacturas.Modelos;

namespace ApiFacturas.Servicios;

public interface IServicioRutaRol
{
    Task<List<RutaRol>> ListarAsync(int limite);
    Task<List<RutaRol>> ObtenerPorRutaAsync(int fkidruta);
    Task<List<RutaRol>> ObtenerPorRolAsync(int fkidrol);
    Task CrearAsync(RutaRol asignacion);
    Task<int> EliminarAsync(int fkidruta, int fkidrol);

    /// <summary>PUT: mueve el permiso a la pareja que llega entera.
    /// NoEncontradoExcepcion si la pareja vieja no existe.</summary>
    Task<int> ReemplazarAsync(int fkidruta, int fkidrol, RutaRol nueva);

    /// <summary>PATCH: mueve SOLO el lado que llegó —la ruta o el rol— y
    /// conserva el otro. ArgumentException si no llegó ninguno.</summary>
    Task<int> ActualizarAsync(int fkidruta, int fkidrol,
                              int? nuevaRuta, int? nuevoRol);

    /// <summary>Reemplaza TODAS las rutas de un rol (el PUT de /api/permisos).
    /// Devuelve cuántas quedaron.</summary>
    Task<int> ReemplazarDeRolAsync(int fkidrol, List<int> idsRuta);
}
