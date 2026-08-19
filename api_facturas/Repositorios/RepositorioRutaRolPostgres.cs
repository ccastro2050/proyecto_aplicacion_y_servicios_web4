// RepositorioRutaRolPostgres — la capa de DATOS del puente rutarol (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
// El DELETE filtra por LAS DOS columnas: borra una pareja exacta,
// nunca "todo lo del usuario/la ruta" (regla dura de la spec).

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioRutaRolPostgres : IRepositorioRutaRol
{
    private readonly string _cadenaConexion;

    public RepositorioRutaRolPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static RutaRol Armar(NpgsqlDataReader lector)
    {
        return new RutaRol
        {
            Fkidruta = lector.GetInt32(0),
            Fkidrol = lector.GetInt32(1),
        };
    }

    private async Task<List<RutaRol>> ConsultarAsync(string sql, Action<NpgsqlParameterCollection> configurar)
    {
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        configurar(comando.Parameters);
        await using var lector = await comando.ExecuteReaderAsync();
        var lista = new List<RutaRol>();
        while (await lector.ReadAsync()) { lista.Add(Armar(lector)); }
        return lista;
    }

    public Task<List<RutaRol>> ObtenerTodosAsync(int limite)
    {
        return ConsultarAsync(
            @"SELECT fkidruta, fkidrol FROM rutarol ORDER BY fkidruta, fkidrol LIMIT @limite",
            p => p.AddWithValue("@limite", limite));
    }

    public Task<List<RutaRol>> ObtenerPorRutaAsync(int fkidruta)
    {
        return ConsultarAsync(
            @"SELECT fkidruta, fkidrol FROM rutarol WHERE fkidruta = @a",
            p => p.AddWithValue("@a", fkidruta));
    }

    public Task<List<RutaRol>> ObtenerPorRolAsync(int fkidrol)
    {
        return ConsultarAsync(
            @"SELECT fkidruta, fkidrol FROM rutarol WHERE fkidrol = @b",
            p => p.AddWithValue("@b", fkidrol));
    }

    public async Task CrearAsync(RutaRol asignacion)
    {
        const string sql = @"INSERT INTO rutarol (fkidruta, fkidrol) VALUES (@a, @b)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@a", asignacion.Fkidruta);
        comando.Parameters.AddWithValue("@b", asignacion.Fkidrol);
        // Duplicado → viola la PK compuesta → PostgresException → 500:
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(int fkidruta, int fkidrol)
    {
        // LA PAREJA EXACTA: las dos columnas en el WHERE.
        const string sql = @"DELETE FROM rutarol WHERE fkidruta = @a AND fkidrol = @b";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@a", fkidruta);
        comando.Parameters.AddWithValue("@b", fkidrol);
        return await comando.ExecuteNonQueryAsync();
    }
}
