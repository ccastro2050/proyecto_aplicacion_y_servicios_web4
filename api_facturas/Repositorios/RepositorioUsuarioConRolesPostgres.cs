// ============================================================
// RepositorioUsuarioConRolesPostgres — la capa de DATOS, y aquí NO hay SQL
// de tablas: hay CINCO llamadas a procedimientos.
//
// Calcado de RepositorioFacturaPostgres, que ya está verificado: el CALL
// devuelve una fila cuya única columna es el INOUT p_resultado, y
// ExecuteScalarAsync la lee como texto JSON.
//
// La traducción de errores va POR PATRÓN del mensaje porque los
// RAISE EXCEPTION de plpgsql no traen número: todos llegan con SQLSTATE
// P0001. El procedimiento dice «Usuario x no existe» → 404.
// ============================================================

using System.Text.Json;
using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using Dapper;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioUsuarioConRolesPostgres : IRepositorioUsuarioConRoles
{
    private readonly string _cadenaConexion;

    private static readonly JsonSerializerOptions _opcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public RepositorioUsuarioConRolesPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<string> EjecutarSpAsync(string sqlCall, object parametros)
    {
        await using var conexion = new NpgsqlConnection(_cadenaConexion);
        try
        {
            var resultado = await conexion.ExecuteScalarAsync<string?>(sqlCall, parametros);
            return resultado ?? "null";
        }
        catch (PostgresException e) when (e.SqlState == "P0001"
                                          && e.MessageText.Contains("no existe"))
        {
            throw new NoEncontradoExcepcion(e.MessageText);  // → 404
        }
        catch (PostgresException e) when (e.SqlState == "23505"
                                          || e.SqlState == "23503")
        {
            // 23505 = clave duplicada (el correo ya existe)
            // 23503 = clave foránea inexistente (un idrol que no está)
            throw new ConflictoExcepcion(e.MessageText);     // → 409
        }
    }

    public async Task<List<UsuarioConRoles>> ListarAsync()
    {
        var json = await EjecutarSpAsync("CALL listar_usuarios_con_roles(NULL)", new { });
        return JsonSerializer.Deserialize<List<UsuarioConRoles>>(json, _opcionesJson)
               ?? new List<UsuarioConRoles>();
    }

    public async Task<UsuarioConRoles> ConsultarAsync(string email)
    {
        var json = await EjecutarSpAsync(
            "CALL consultar_usuario_con_roles(@p_email, NULL)", new { p_email = email });
        return JsonSerializer.Deserialize<UsuarioConRoles>(json, _opcionesJson)!;
    }

    /// <summary>El usuario Y sus roles en UNA transacción. Si se hiciera en dos
    /// llamadas —crear el usuario, después asignarle los roles— un fallo en la
    /// segunda dejaría un usuario sin ningún rol.</summary>
    public async Task<UsuarioConRoles> CrearAsync(string email, string contrasena, string rolesJson)
    {
        var json = await EjecutarSpAsync(
            "CALL crear_usuario_con_roles(@p_email, @p_contrasena, @p_roles::json, NULL)",
            new { p_email = email, p_contrasena = contrasena, p_roles = rolesJson });
        return JsonSerializer.Deserialize<UsuarioConRoles>(json, _opcionesJson)!;
    }

    /// <summary>El procedimiento solo cambia la contraseña si llega con algo:
    /// vacía significa «déjela como está». Y REEMPLAZA los roles — borra los
    /// que había y pone los que llegan, en la misma transacción.</summary>
    public async Task<UsuarioConRoles> ActualizarAsync(string email, string? contrasena, string rolesJson)
    {
        var json = await EjecutarSpAsync(
            "CALL actualizar_usuario_con_roles(@p_email, @p_contrasena, @p_roles::json, NULL)",
            new { p_email = email, p_contrasena = contrasena ?? "", p_roles = rolesJson });
        return JsonSerializer.Deserialize<UsuarioConRoles>(json, _opcionesJson)!;
    }

    /// <summary>Borra el detalle y el maestro juntos. Borrar solo el usuario
    /// chocaría con la clave foránea de rol_usuario.</summary>
    public async Task<string> EliminarAsync(string email)
    {
        return await EjecutarSpAsync(
            "CALL eliminar_usuario_con_roles(@p_email, NULL)", new { p_email = email });
    }
}
