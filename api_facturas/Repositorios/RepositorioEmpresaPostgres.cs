// RepositorioEmpresaPostgres — la capa de DATOS de empresa (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
// CALCADO de RepositorioProductoSqlServer: ADO.NET, SQL parametrizado, async.

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioEmpresaPostgres : IRepositorioEmpresa
{
    private readonly string _cadenaConexion;

    public RepositorioEmpresaPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static Empresa Armar(NpgsqlDataReader lector)
    {
        return new Empresa
        {
            Codigo = lector.GetString(0),
            Nombre = lector.GetString(1),
        };
    }

    public async Task<List<Empresa>> ObtenerTodasAsync(int limite)
    {
        const string sql = @"SELECT codigo, nombre
                             FROM empresa ORDER BY codigo LIMIT @limite";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@limite", limite);
        await using var lector = await comando.ExecuteReaderAsync();
        var lista = new List<Empresa>();
        while (await lector.ReadAsync()) { lista.Add(Armar(lector)); }
        return lista;
    }

    public async Task<Empresa?> ObtenerPorCodigoAsync(string codigo)
    {
        const string sql = @"SELECT codigo, nombre FROM empresa WHERE codigo = @codigo";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);
        await using var lector = await comando.ExecuteReaderAsync();
        if (await lector.ReadAsync()) { return Armar(lector); }
        return null;
    }

    public async Task CrearAsync(Empresa entidad)
    {
        const string sql = @"INSERT INTO empresa (codigo, nombre)
                             VALUES (@codigo, @nombre)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", entidad.Codigo);
        comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        // SET dinámico con lista blanca (los nombres salen de las PETICIONES):
        var asignaciones = new List<string>();
        foreach (var columna in datos.Keys) { asignaciones.Add($"{columna} = @{columna}"); }
        var sql = $"UPDATE empresa SET {string.Join(", ", asignaciones)} WHERE codigo = @pk_clave";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        foreach (var (columna, valor) in datos) { comando.Parameters.AddWithValue($"@{columna}", valor); }
        comando.Parameters.AddWithValue("@pk_clave", codigo);
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(string codigo)
    {
        const string sql = "DELETE FROM empresa WHERE codigo = @codigo";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);
        return await comando.ExecuteNonQueryAsync();
    }
}
