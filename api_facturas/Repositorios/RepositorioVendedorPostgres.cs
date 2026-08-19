// RepositorioVendedorPostgres — la capa de DATOS de vendedor (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
// CALCADO de RepositorioProductoSqlServer: ADO.NET, SQL parametrizado, async.

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioVendedorPostgres : IRepositorioVendedor
{
    private readonly string _cadenaConexion;

    public RepositorioVendedorPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static Vendedor Armar(NpgsqlDataReader lector)
    {
        return new Vendedor
        {
            Id = lector.GetInt32(0),
            Carnet = lector.GetInt32(1),
            Direccion = lector.GetString(2),
            Fkcodpersona = lector.GetString(3),
        };
    }

    public async Task<List<Vendedor>> ObtenerTodosAsync(int limite)
    {
        const string sql = @"SELECT id, carnet, direccion, fkcodpersona
                             FROM vendedor ORDER BY id LIMIT @limite";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@limite", limite);
        await using var lector = await comando.ExecuteReaderAsync();
        var lista = new List<Vendedor>();
        while (await lector.ReadAsync()) { lista.Add(Armar(lector)); }
        return lista;
    }

    public async Task<Vendedor?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id, carnet, direccion, fkcodpersona FROM vendedor WHERE id = @id";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        await using var lector = await comando.ExecuteReaderAsync();
        if (await lector.ReadAsync()) { return Armar(lector); }
        return null;
    }

    public async Task CrearAsync(Vendedor entidad)
    {
        const string sql = @"INSERT INTO vendedor (carnet, direccion, fkcodpersona)
                             VALUES (@carnet, @direccion, @fkcodpersona)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@carnet", entidad.Carnet);
        comando.Parameters.AddWithValue("@direccion", entidad.Direccion);
        comando.Parameters.AddWithValue("@fkcodpersona", entidad.Fkcodpersona);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarAsync(int id, Dictionary<string, object> datos)
    {
        // SET dinámico con lista blanca (los nombres salen de las PETICIONES):
        var asignaciones = new List<string>();
        foreach (var columna in datos.Keys) { asignaciones.Add($"{columna} = @{columna}"); }
        var sql = $"UPDATE vendedor SET {string.Join(", ", asignaciones)} WHERE id = @pk_clave";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        foreach (var (columna, valor) in datos) { comando.Parameters.AddWithValue($"@{columna}", valor); }
        comando.Parameters.AddWithValue("@pk_clave", id);
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(int id)
    {
        const string sql = "DELETE FROM vendedor WHERE id = @id";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        return await comando.ExecuteNonQueryAsync();
    }
}
