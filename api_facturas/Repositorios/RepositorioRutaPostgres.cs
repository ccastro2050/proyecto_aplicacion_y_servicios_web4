// RepositorioRutaPostgres — la capa de DATOS de ruta (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
// CALCADO de RepositorioProductoSqlServer: ADO.NET, SQL parametrizado, async.

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioRutaPostgres : IRepositorioRuta
{
    private readonly string _cadenaConexion;

    public RepositorioRutaPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static Ruta Armar(NpgsqlDataReader lector)
    {
        return new Ruta
        {
            Id = lector.GetInt32(0),
            Valor = lector.GetString(1),
            Descripcion = lector.GetString(2),
        };
    }

    public async Task<List<Ruta>> ObtenerTodasAsync(int limite)
    {
        const string sql = @"SELECT id, ruta, descripcion
                             FROM ruta ORDER BY id LIMIT @limite";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@limite", limite);
        await using var lector = await comando.ExecuteReaderAsync();
        var lista = new List<Ruta>();
        while (await lector.ReadAsync()) { lista.Add(Armar(lector)); }
        return lista;
    }

    public async Task<Ruta?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id, ruta, descripcion FROM ruta WHERE id = @id";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        await using var lector = await comando.ExecuteReaderAsync();
        if (await lector.ReadAsync()) { return Armar(lector); }
        return null;
    }

    public async Task CrearAsync(Ruta entidad)
    {
        const string sql = @"INSERT INTO ruta (ruta, descripcion)
                             VALUES (@ruta, @descripcion)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@ruta", entidad.Valor);
        comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarAsync(int id, Dictionary<string, object> datos)
    {
        // SET dinámico con lista blanca (los nombres salen de las PETICIONES):
        var asignaciones = new List<string>();
        foreach (var columna in datos.Keys) { asignaciones.Add($"{columna} = @{columna}"); }
        var sql = $"UPDATE ruta SET {string.Join(", ", asignaciones)} WHERE id = @pk_clave";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        foreach (var (columna, valor) in datos) { comando.Parameters.AddWithValue($"@{columna}", valor); }
        comando.Parameters.AddWithValue("@pk_clave", id);
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(int id)
    {
        const string sql = "DELETE FROM ruta WHERE id = @id";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        return await comando.ExecuteNonQueryAsync();
    }
}
