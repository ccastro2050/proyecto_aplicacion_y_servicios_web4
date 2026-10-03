// ============================================================
// RepositorioUsuarioPostgres — la capa de DATOS de usuario.
//
// AQUÍ (y solo aquí) vive el hash: cómo se persiste un secreto es un
// detalle de la capa de datos. Dos reglas: (1) se guarda BCrypt
// (costo 12), jamás texto plano; (2) ningún SELECT proyecta la
// columna contrasena hacia afuera. SQL a mano + Dapper (Art. 2).
// Dialecto PostgreSQL.
// ============================================================

using ApiFacturas.Modelos;
using Dapper;
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

    private NpgsqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<List<Usuario>> ObtenerTodosAsync(int limite)
    {
        // SOLO email: la contraseña no sale ni en hash (RNF del secreto).
        const string sql = @"SELECT email FROM usuario ORDER BY email LIMIT @limite";
        await using var conexion = CrearConexion();
        return (await conexion.QueryAsync<Usuario>(sql, new { limite })).ToList();
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        const string sql = @"SELECT email FROM usuario WHERE email = @email";
        await using var conexion = CrearConexion();
        return await conexion.QueryFirstOrDefaultAsync<Usuario>(sql, new { email });
    }

    public async Task CrearAsync(string email, string contrasena)
    {
        // El hash se calcula AQUÍ, justo antes de persistir:
        var hash = BC.HashPassword(contrasena, workFactor: 12);
        const string sql = @"INSERT INTO usuario (email, contrasena) VALUES (@email, @hash)";
        await using var conexion = CrearConexion();
        await conexion.ExecuteAsync(sql, new { email, hash });
    }

    public async Task<int> ActualizarContrasenaAsync(string email, string contrasena)
    {
        var hash = BC.HashPassword(contrasena, workFactor: 12);
        const string sql = @"UPDATE usuario SET contrasena = @hash WHERE email = @email";
        await using var conexion = CrearConexion();
        return await conexion.ExecuteAsync(sql, new { hash, email });
    }

    public async Task<int> EliminarAsync(string email)
    {
        // Si el usuario tiene roles asignados, la FK rechaza → 500:
        const string sql = "DELETE FROM usuario WHERE email = @email";
        await using var conexion = CrearConexion();
        return await conexion.ExecuteAsync(sql, new { email });
    }

    public async Task<bool?> VerificarContrasenaAsync(string email, string contrasena)
    {
        // El hash SE LEE pero no sale del repositorio: se compara aquí.
        const string sql = @"SELECT contrasena FROM usuario WHERE email = @email";
        await using var conexion = CrearConexion();
        var hash = await conexion.QueryFirstOrDefaultAsync<string>(sql, new { email });

        if (hash == null) { return null; }          // el usuario no existe → 404

        // BC.Verify devuelve false ante hash malformado (los usuarios
        // semilla con texto plano dan 401 — a propósito, es la lección):
        try { return BC.Verify(contrasena, hash); }
        catch { return false; }
    }
}
