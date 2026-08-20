// ============================================================
// RepositorioRutaRolSqlServer — la capa de DATOS del puente rutarol.
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

public class RepositorioRutaRolSqlServer : IRepositorioRutaRol
{
    private readonly string _cadenaConexion;

    public RepositorioRutaRolSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<List<RutaRol>> ObtenerTodosAsync(int limite)
    {
        const string sql = @"SELECT TOP (@limite) fkidruta, fkidrol FROM rutarol ORDER BY fkidruta, fkidrol";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<RutaRol>(sql, new { limite })).ToList();
    }

    public async Task<List<RutaRol>> ObtenerPorRutaAsync(int fkidruta)
    {
        const string sql = @"SELECT fkidruta, fkidrol FROM rutarol WHERE fkidruta = @fkidruta";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<RutaRol>(sql, new { fkidruta })).ToList();
    }

    public async Task<List<RutaRol>> ObtenerPorRolAsync(int fkidrol)
    {
        const string sql = @"SELECT fkidruta, fkidrol FROM rutarol WHERE fkidrol = @fkidrol";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<RutaRol>(sql, new { fkidrol })).ToList();
    }

    public async Task CrearAsync(RutaRol asignacion)
    {
        // Duplicado → viola la PK compuesta → excepción del motor → 500:
        const string sql = @"INSERT INTO rutarol (fkidruta, fkidrol) VALUES (@Fkidruta, @Fkidrol)";
        await using var conexion = CrearConexion();
        await ErroresSqlServer.TraducirAsync(
            () => conexion.ExecuteAsync(sql, asignacion));
    }

    public async Task<int> EliminarAsync(int fkidruta, int fkidrol)
    {
        // LA PAREJA EXACTA: las dos columnas en el WHERE.
        const string sql = @"DELETE FROM rutarol WHERE fkidruta = @fkidruta AND fkidrol = @fkidrol";
        await using var conexion = CrearConexion();
        return await ErroresSqlServer.TraducirAsync(
            () => conexion.ExecuteAsync(sql, new { fkidruta, fkidrol }));
    }

    public async Task<int> ReemplazarAsync(int fkidrutaViejo, int fkidrolViejo,
                                           int fkidrutaNuevo, int fkidrolNuevo)
    {
        // EN UNA TABLA PUENTE, «ACTUALIZAR» ES MOVER LA FILA: las dos columnas
        // son la llave primaria, así que se borra la pareja vieja y se inserta
        // la nueva. Va en UNA transacción porque si el INSERT fallara —la
        // pareja nueva ya existe— el DELETE no puede quedarse hecho: se habría
        // quitado un permiso sin poner el otro.
        const string sqlBorrar = @"DELETE FROM rutarol
                                   WHERE fkidruta = @rutaVieja AND fkidrol = @rolViejo";
        const string sqlInsertar = @"INSERT INTO rutarol (fkidruta, fkidrol)
                                     VALUES (@rutaNueva, @rolNuevo)";

        await using var conexion = CrearConexion();
        await conexion.OpenAsync();
        await using var transaccion = await conexion.BeginTransactionAsync();
        try
        {
            var filas = await conexion.ExecuteAsync(sqlBorrar,
                new { rutaVieja = fkidrutaViejo, rolViejo = fkidrolViejo }, transaccion);

            if (filas == 0)
            {
                // La pareja vieja no existía: no hay nada que mover. Se
                // devuelve 0 y el SERVICIO decide que eso es un 404.
                await transaccion.RollbackAsync();
                return 0;
            }

            await conexion.ExecuteAsync(sqlInsertar,
                new { rutaNueva = fkidrutaNuevo, rolNuevo = fkidrolNuevo }, transaccion);
            await transaccion.CommitAsync();
            return filas;
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;   // la traduce quien llama (FK inexistente, pareja repetida…)
        }
    }

    public async Task<int> ReemplazarDeRolAsync(int fkidrol, List<int> idsRuta)
    {
        // TODO O NADA: borrar lo que tiene e insertar lo nuevo son dos pasos, y
        // entre uno y otro el rol no puede entrar a ninguna parte. Si el
        // segundo fallara —una ruta que no existe— y el primero quedara hecho,
        // habríamos dejado a un rol sin permisos sin que nadie lo pidiera.
        const string sqlBorrar = @"DELETE FROM rutarol WHERE fkidrol = @fkidrol";
        const string sqlInsertar = @"INSERT INTO rutarol (fkidruta, fkidrol)
                                     VALUES (@fkidruta, @fkidrol)";

        await using var conexion = CrearConexion();
        await conexion.OpenAsync();
        await using var transaccion = await conexion.BeginTransactionAsync();
        try
        {
            await conexion.ExecuteAsync(sqlBorrar, new { fkidrol }, transaccion);

            foreach (var idruta in idsRuta.Distinct())
            {
                await conexion.ExecuteAsync(sqlInsertar,
                    new { fkidruta = idruta, fkidrol }, transaccion);
            }

            await transaccion.CommitAsync();
            return idsRuta.Distinct().Count();
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;   // ruta inexistente (FK) → la traduce quien llama
        }
    }
}
