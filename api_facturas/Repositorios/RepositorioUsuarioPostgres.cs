// ============================================================
// RepositorioUsuarioPostgres — la capa de DATOS de usuario (v4).
// Dialecto PostgreSQL: Npgsql en vez de SqlClient y LIMIT en vez de
// TOP — TODO lo demás es idéntico al gemelo *SqlServer (esa es la gracia).
//
// AQUÍ (y solo aquí) vive el hash: cómo se persiste un secreto es
// un detalle de la capa de datos — servicio y controller no saben
// qué algoritmo es. Dos reglas:
//   1. Se guarda BCrypt (costo 12), jamás texto plano.
//   2. Ningún SELECT proyecta la columna contrasena hacia afuera.
// ============================================================

using ApiFacturas.Modelos;
using Npgsql;
using BC = BCrypt.Net.BCrypt;   // el paquete BCrypt.Net-Next

namespace ApiFacturas.Repositorios;

public class RepositorioUsuarioPostgres : IRepositorioUsuario
{
    private readonly string _cadenaConexion;

    public RepositorioUsuarioPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync()
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync();
        return conexion;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync(int limite)
    {
        // SOLO email: la contraseña no sale ni en hash (RNF3).
        const string sql = @"SELECT email FROM usuario ORDER BY email LIMIT @limite";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@limite", limite);
        await using var lector = await comando.ExecuteReaderAsync();
        var usuarios = new List<Usuario>();
        while (await lector.ReadAsync())
        {
            usuarios.Add(new Usuario { Email = lector.GetString(0) });
        }
        return usuarios;
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        const string sql = @"SELECT email FROM usuario WHERE email = @email";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@email", email);
        await using var lector = await comando.ExecuteReaderAsync();
        if (await lector.ReadAsync())
        {
            return new Usuario { Email = lector.GetString(0) };
        }
        return null;
    }

    public async Task CrearAsync(string email, string contrasena)
    {
        // El hash se calcula AQUÍ, justo antes de persistir:
        var hash = BC.HashPassword(contrasena, workFactor: 12);

        const string sql = @"INSERT INTO usuario (email, contrasena) VALUES (@email, @hash)";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@email", email);
        comando.Parameters.AddWithValue("@hash", hash);
        await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> ActualizarContrasenaAsync(string email, string contrasena)
    {
        var hash = BC.HashPassword(contrasena, workFactor: 12);

        const string sql = @"UPDATE usuario SET contrasena = @hash WHERE email = @email";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@hash", hash);
        comando.Parameters.AddWithValue("@email", email);
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<int> EliminarAsync(string email)
    {
        const string sql = "DELETE FROM usuario WHERE email = @email";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@email", email);
        // Si el usuario tiene roles asignados, la FK de rol_usuario
        // rechaza el DELETE → PostgresException → 500 (integridad referencial):
        return await comando.ExecuteNonQueryAsync();
    }

    public async Task<bool?> VerificarContrasenaAsync(string email, string contrasena)
    {
        // El hash SE LEE pero no sale del repositorio: se compara aquí.
        const string sql = @"SELECT contrasena FROM usuario WHERE email = @email";
        await using var conexion = await AbrirConexionAsync();
        await using var comando = new NpgsqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@email", email);
        var hash = (string?)await comando.ExecuteScalarAsync();

        if (hash == null) { return null; }          // el usuario no existe → 404

        // BC.Verify devuelve false ante hash malformado (los usuarios
        // semilla con texto plano dan 401 — a propósito, es la lección):
        try { return BC.Verify(contrasena, hash); }
        catch { return false; }
    }
}
