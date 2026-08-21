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
        await conexion.ExecuteAsync(sql, asignacion);
    }

    public async Task<int> EliminarAsync(int fkidruta, int fkidrol)
    {
        // LA PAREJA EXACTA: las dos columnas en el WHERE.
        const string sql = @"DELETE FROM rutarol WHERE fkidruta = @fkidruta AND fkidrol = @fkidrol";
        await using var conexion = CrearConexion();
        return await conexion.ExecuteAsync(sql, new { fkidruta, fkidrol });
    }
}
