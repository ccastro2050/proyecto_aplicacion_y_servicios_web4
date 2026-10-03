// ============================================================
// RepositorioRolUsuarioSqlServer — la capa de DATOS del puente rol_usuario.
//
// SQL a mano + DAPPER como micro-ejecutor (constitución, Art. 2).
// El DELETE filtra por LAS DOS columnas: borra una pareja exacta,
// nunca "todo lo del lado A o B" (regla dura de la spec).
// Dialecto SQL Server: TOP (@limite) al PRINCIPIO del SELECT (T-SQL no tiene LIMIT).
// ============================================================

using ApiFacturas.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiFacturas.Repositorios;

public class RepositorioRolUsuarioSqlServer : IRepositorioRolUsuario
{
    private readonly string _cadenaConexion;

    public RepositorioRolUsuarioSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<List<RolUsuario>> ObtenerTodosAsync(int limite)
    {
        const string sql = @"SELECT TOP (@limite) fkemail, fkidrol FROM rol_usuario ORDER BY fkemail, fkidrol";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<RolUsuario>(sql, new { limite })).ToList();
    }

    public async Task<List<RolUsuario>> ObtenerPorUsuarioAsync(string fkemail)
    {
        const string sql = @"SELECT fkemail, fkidrol FROM rol_usuario WHERE fkemail = @fkemail";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<RolUsuario>(sql, new { fkemail })).ToList();
    }

    public async Task<List<RolUsuario>> ObtenerPorRolAsync(int fkidrol)
    {
        const string sql = @"SELECT fkemail, fkidrol FROM rol_usuario WHERE fkidrol = @fkidrol";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<RolUsuario>(sql, new { fkidrol })).ToList();
    }

    public async Task CrearAsync(RolUsuario asignacion)
    {
        // Duplicado → viola la PK compuesta → excepción del motor → 500:
        const string sql = @"INSERT INTO rol_usuario (fkemail, fkidrol) VALUES (@Fkemail, @Fkidrol)";
        await using var conexion = CrearConexion();
        await ErroresSqlServer.TraducirAsync(
            () => conexion.ExecuteAsync(sql, asignacion));
    }

    public async Task<int> EliminarAsync(string fkemail, int fkidrol)
    {
        // LA PAREJA EXACTA: las dos columnas en el WHERE.
        const string sql = @"DELETE FROM rol_usuario WHERE fkemail = @fkemail AND fkidrol = @fkidrol";
        await using var conexion = CrearConexion();
        return await ErroresSqlServer.TraducirAsync(
            () => conexion.ExecuteAsync(sql, new { fkemail, fkidrol }));
    }

    public async Task<int> ReemplazarAsync(string fkemailViejo, int fkidrolViejo,
                                           string fkemailNuevo, int fkidrolNuevo)
    {
        // EN UNA TABLA PUENTE, «ACTUALIZAR» ES MOVER LA FILA.
        //
        // Las dos columnas son la llave primaria, así que no hay un campo
        // suelto que cambiar: se borra la pareja vieja y se inserta la nueva.
        // Y va en UNA transacción, porque si el INSERT fallara —la pareja
        // nueva ya existe— el DELETE no puede quedarse hecho: se habría
        // perdido una asignación sin poner ninguna.
        const string sqlBorrar = @"DELETE FROM rol_usuario
                                   WHERE fkemail = @viejoEmail AND fkidrol = @viejoRol";
        const string sqlInsertar = @"INSERT INTO rol_usuario (fkemail, fkidrol)
                                     VALUES (@nuevoEmail, @nuevoRol)";

        await using var conexion = CrearConexion();
        await conexion.OpenAsync();
        await using var transaccion = await conexion.BeginTransactionAsync();
        try
        {
            var filas = await conexion.ExecuteAsync(sqlBorrar,
                new { viejoEmail = fkemailViejo, viejoRol = fkidrolViejo }, transaccion);

            if (filas == 0)
            {
                // La pareja vieja no existía: no hay nada que mover. Se
                // devuelve 0 y el SERVICIO decide que eso es un 404.
                await transaccion.RollbackAsync();
                return 0;
            }

            await conexion.ExecuteAsync(sqlInsertar,
                new { nuevoEmail = fkemailNuevo, nuevoRol = fkidrolNuevo }, transaccion);
            await transaccion.CommitAsync();
            return filas;
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;   // la traduce quien llama (FK inexistente, pareja repetida…)
        }
    }
}
