// RepositorioRolUsuarioPostgres — la capa de DATOS del puente rol_usuario (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
// El DELETE filtra por LAS DOS columnas: borra una pareja exacta,
// nunca "todo lo del usuario/la ruta" (regla dura de la spec).

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioRolUsuarioPostgres : IRepositorioRolUsuario
{
    private readonly string _cadenaConexion;

    public RepositorioRolUsuarioPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static RolUsuario Armar(NpgsqlDataReader lector)
    {
        return new RolUsuario
        {
            Fkemail = lector.GetString(0),
            Fkidrol = lector.GetInt32(1),
        };
    }

    private async Task<List<RolUsuario>> ConsultarAsync(string sql, Action<NpgsqlParameterCollection> configurar)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        configurar(comando.Parameters);
        await using var lector = await comando.ExecuteReaderAsync();
        var lista = new List<RolUsuario>();
        while (await lector.ReadAsync()) { lista.Add(Armar(lector)); }
        return lista;
    }

    public Task<List<RolUsuario>> ObtenerTodosAsync(int limite)
    {
        return ConsultarAsync(
            @"SELECT fkemail, fkidrol FROM rol_usuario ORDER BY fkemail, fkidrol LIMIT @limite",
            p => p.AddWithValue("@limite", limite));
    }

    public Task<List<RolUsuario>> ObtenerPorUsuarioAsync(string fkemail)
    {
        return ConsultarAsync(
            @"SELECT fkemail, fkidrol FROM rol_usuario WHERE fkemail = @a",
            p => p.AddWithValue("@a", fkemail));
    }

    public Task<List<RolUsuario>> ObtenerPorRolAsync(int fkidrol)
    {
        return ConsultarAsync(
            @"SELECT fkemail, fkidrol FROM rol_usuario WHERE fkidrol = @b",
            p => p.AddWithValue("@b", fkidrol));
    }

    public async Task CrearAsync(RolUsuario asignacion)
    {
        const string sql = @"INSERT INTO rol_usuario (fkemail, fkidrol) VALUES (@a, @b)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@a", asignacion.Fkemail);
        comando.Parameters.AddWithValue("@b", asignacion.Fkidrol);
        // Duplicado → viola la PK compuesta → PostgresException → 500:
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(string fkemail, int fkidrol)
    {
        // LA PAREJA EXACTA: las dos columnas en el WHERE.
        const string sql = @"DELETE FROM rol_usuario WHERE fkemail = @a AND fkidrol = @b";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@a", fkemail);
        comando.Parameters.AddWithValue("@b", fkidrol);
        return await comando.ExecuteNonQueryAsync();
    }
}
