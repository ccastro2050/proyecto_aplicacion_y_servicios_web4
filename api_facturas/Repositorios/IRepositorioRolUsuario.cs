// IRepositorioRolUsuario — contrato del puente rol_usuario (v3): el patrón
// nuevo — sin Actualizar, búsquedas por cada lado, y el DELETE
// exige LA PAREJA (ambas columnas de la PK compuesta).

using ApiFacturas.Modelos;

namespace ApiFacturas.Repositorios;

public interface IRepositorioRolUsuario
{
    Task<List<RolUsuario>> ObtenerTodosAsync(int limite);
    Task<List<RolUsuario>> ObtenerPorUsuarioAsync(string fkemail);
    Task<List<RolUsuario>> ObtenerPorRolAsync(int fkidrol);
    Task CrearAsync(RolUsuario asignacion);
    Task<int> EliminarAsync(string fkemail, int fkidrol);   // ¡AMBAS columnas!

    /// <summary>MUEVE la asignación: borra la pareja vieja e inserta la nueva,
    /// en ese orden. En una tabla puente «actualizar» no es otra cosa — las dos
    /// columnas SON la llave, y cambiar una es cambiar de fila.
    /// Devuelve las filas afectadas (0 = la pareja vieja no existía).</summary>
    Task<int> ReemplazarAsync(string fkemailViejo, int fkidrolViejo,
                              string fkemailNuevo, int fkidrolNuevo);
}
