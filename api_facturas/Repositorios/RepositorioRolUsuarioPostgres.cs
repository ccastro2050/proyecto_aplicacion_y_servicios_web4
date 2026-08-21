// ============================================================
// RepositorioRolUsuarioPostgres — la capa de DATOS del puente rol_usuario.
//
// SQL a mano + DAPPER como micro-ejecutor (constitución, Art. 2).
// El DELETE filtra por LAS DOS columnas: borra una pareja exacta,
// nunca "todo lo del lado A o B" (regla dura de la spec).
// Dialecto PostgreSQL: LIMIT @limite al final del SELECT.
// ============================================================

using ApiFacturas.Modelos;
using Dapper;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioRolUsuarioPostgres : IRepositorioRolUsuario
{
    private readonly string _cadenaConexion;

    public RepositorioRolUsuarioPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private NpgsqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<List<RolUsuario>> ObtenerTodosAsync(int limite)
    {
        const string sql = @"SELECT fkemail, fkidrol FROM rol_usuario ORDER BY fkemail, fkidrol LIMIT @limite";
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
        await conexion.ExecuteAsync(sql, asignacion);
    }

    public async Task<int> EliminarAsync(string fkemail, int fkidrol)
    {
        // LA PAREJA EXACTA: las dos columnas en el WHERE.
        const string sql = @"DELETE FROM rol_usuario WHERE fkemail = @fkemail AND fkidrol = @fkidrol";
        await using var conexion = CrearConexion();
        return await conexion.ExecuteAsync(sql, new { fkemail, fkidrol });
    }
}
