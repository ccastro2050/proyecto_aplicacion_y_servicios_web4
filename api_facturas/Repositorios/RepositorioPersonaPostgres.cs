// ============================================================
// RepositorioPersonaPostgres — la capa de DATOS de persona.
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
//
// CALCADO de RepositorioProductoSqlServer (allí está explicado
// ADO.NET, los parámetros @ y el "await using"). Cambian: la
// tabla, las columnas y el modelo. El SQL sigue las mismas dos
// reglas de la constitución: parametrizado y asíncrono.
// ============================================================

using ApiFacturas.Modelos;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioPersonaPostgres : IRepositorioPersona
{
    private readonly string _cadenaConexion;

    public RepositorioPersonaPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    // ------------------------------------------------------------
    // Ayudantes privados (el mismo par que en producto)
    // ------------------------------------------------------------

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    private static Persona ArmarPersona(NpgsqlDataReader lector)
    {
        return new Persona
        {
            Codigo = lector.GetString(0),
            Nombre = lector.GetString(1),
            Email = lector.GetString(2),
            Telefono = lector.GetString(3),
        };
    }

    // ------------------------------------------------------------
    // Los 5 métodos del contrato
    // ------------------------------------------------------------

    public async Task<List<Persona>> ObtenerTodasAsync(int limite)
    {
        const string sql = @"SELECT codigo, nombre, email, telefono
                             FROM persona ORDER BY codigo LIMIT @limite";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@limite", limite);

        await using var lector = await comando.ExecuteReaderAsync();
        var personas = new List<Persona>();
        while (await lector.ReadAsync())
        {
            personas.Add(ArmarPersona(lector));
        }
        return personas;
    }

    public async Task<Persona?> ObtenerPorCodigoAsync(string codigo)
    {
        const string sql = @"SELECT codigo, nombre, email, telefono
                             FROM persona WHERE codigo = @codigo";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);

        await using var lector = await comando.ExecuteReaderAsync();
        if (await lector.ReadAsync())
        {
            return ArmarPersona(lector);
        }
        return null;
    }

    public async Task CrearAsync(Persona persona)
    {
        const string sql = @"INSERT INTO persona (codigo, nombre, email, telefono)
                             VALUES (@codigo, @nombre, @email, @telefono)";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", persona.Codigo);
        comando.Parameters.AddWithValue("@nombre", persona.Nombre);
        comando.Parameters.AddWithValue("@email", persona.Email);
        comando.Parameters.AddWithValue("@telefono", persona.Telefono);

        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        // SET dinámico con lista blanca (los nombres de columna salen de
        // las PETICIONES, nunca del cliente) — ver la explicación completa
        // en RepositorioProductoSqlServer.ActualizarAsync:
        var asignaciones = new List<string>();
        foreach (var columna in datos.Keys)
        {
            asignaciones.Add($"{columna} = @{columna}");
        }
        var sql = $"UPDATE persona SET {string.Join(", ", asignaciones)} " +
                  "WHERE codigo = @codigo_clave";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        foreach (var (columna, valor) in datos)
        {
            comando.Parameters.AddWithValue($"@{columna}", valor);
        }
        comando.Parameters.AddWithValue("@codigo_clave", codigo);

        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(string codigo)
    {
        const string sql = "DELETE FROM persona WHERE codigo = @codigo";

        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@codigo", codigo);
        // Si la persona es cliente/vendedor, aquí explota la PostgresException
        // de llave foránea — sube tal cual y el controlador responde 500
        // con el mensaje del motor (la lección de integridad referencial):
        return await comando.ExecuteNonQueryAsync();
    }
}
