// RepositorioRolPostgres — la capa de DATOS de rol (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
// CALCADO de RepositorioProductoSqlServer: ADO.NET, SQL parametrizado, async.

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioRolPostgres : IRepositorioRol
{
    private readonly string _cadenaConexion;

    public RepositorioRolPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static Rol Armar(NpgsqlDataReader lector)
    {
        return new Rol
        {
            Id = lector.GetInt32(0),
            Nombre = lector.GetString(1),
        };
    }

    public async Task<List<Rol>> ObtenerTodosAsync(int limite)
    {
        const string sql = @"SELECT id, nombre
                             FROM rol ORDER BY id LIMIT @limite";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@limite", limite);
        await using var lector = await comando.ExecuteReaderAsync();
        var lista = new List<Rol>();
        while (await lector.ReadAsync()) { lista.Add(Armar(lector)); }
        return lista;
    }

    public async Task<Rol?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id, nombre FROM rol WHERE id = @id";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        await using var lector = await comando.ExecuteReaderAsync();
        if (await lector.ReadAsync()) { return Armar(lector); }
        return null;
    }

    public async Task CrearAsync(Rol entidad)
    {
        const string sql = @"INSERT INTO rol (nombre)
                             VALUES (@nombre)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarAsync(int id, Dictionary<string, object> datos)
    {
        // SET dinámico con lista blanca (los nombres salen de las PETICIONES):
        var asignaciones = new List<string>();
        foreach (var columna in datos.Keys) { asignaciones.Add($"{columna} = @{columna}"); }
        var sql = $"UPDATE rol SET {string.Join(", ", asignaciones)} WHERE id = @pk_clave";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        foreach (var (columna, valor) in datos) { comando.Parameters.AddWithValue($"@{columna}", valor); }
        comando.Parameters.AddWithValue("@pk_clave", id);
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(int id)
    {
        const string sql = "DELETE FROM rol WHERE id = @id";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        return await comando.ExecuteNonQueryAsync();
    }
}
