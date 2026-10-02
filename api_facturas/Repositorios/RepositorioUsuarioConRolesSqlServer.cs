// ============================================================
// RepositorioUsuarioConRolesSqlServer — el gemelo en T-SQL.
//
// Mismo contrato, mismos cinco procedimientos, otro dialecto. Y las
// diferencias con la versión de PostgreSQL son la lección de dialectos:
//
//   El OUTPUT      PostgreSQL devuelve el INOUT como una fila, y se lee con
//                  ExecuteScalarAsync. SQL Server usa un parámetro OUTPUT de
//                  verdad, que Dapper declara con DynamicParameters y
//                  `size: -1` para NVARCHAR(MAX).
//
//   EL ERROR       PostgreSQL manda TODO con SQLSTATE P0001, así que hay que
//                  distinguir por el TEXTO del mensaje. SQL Server NUMERA sus
//                  THROW —50006, 50008— y el número es preciso: no se rompe
//                  porque alguien traduzca un mensaje.
//
//   LA LLAMADA     `CALL sp(...)` contra `CommandType.StoredProcedure`.
//
// Y lo que NO cambia: el JSON que devuelven. Los dos procedimientos entregan
// {"email":…,"roles":[{"idrol":…,"nombre":…}]}, así que el modelo y el
// servicio son los mismos — que es exactamente lo que la fábrica promete.
// ============================================================

using System.Data;
using System.Text.Json;
using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiFacturas.Repositorios;

public class RepositorioUsuarioConRolesSqlServer : IRepositorioUsuarioConRoles
{
    private readonly string _cadenaConexion;

    private static readonly JsonSerializerOptions _opcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public RepositorioUsuarioConRolesSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private async Task<string> EjecutarSpAsync(string nombreSp, object? entrada)
    {
        var parametros = entrada == null
            ? new DynamicParameters()
            : new DynamicParameters(entrada);
        parametros.Add("p_resultado", dbType: DbType.String,
                       direction: ParameterDirection.Output, size: -1);

        await using var conexion = new SqlConnection(_cadenaConexion);
        try
        {
            await conexion.ExecuteAsync(nombreSp, parametros,
                commandType: CommandType.StoredProcedure);
        }
        // 50006 = el THROW de eliminar_usuario_con_roles
        // 50008 = el de consultar_usuario_con_roles
        catch (SqlException e) when (e.Number == 50006 || e.Number == 50008)
        {
            throw new NoEncontradoExcepcion(e.Message);      // → 404
        }
        // 2627 = clave primaria duplicada · 547 = clave foránea violada.
        // Son los equivalentes de 23505 y 23503 en PostgreSQL.
        catch (SqlException e) when (e.Number == 2627 || e.Number == 2601)
        {
            throw new ConflictoExcepcion("Ese registro ya existe: " + e.Message);
        }
        catch (SqlException e) when (e.Number == 547)
        {
            throw new ConflictoExcepcion(
                "La operación rompe una relación de la base de datos: " + e.Message);
        }

        return parametros.Get<string?>("p_resultado") ?? "null";
    }

    public async Task<List<UsuarioConRoles>> ListarAsync()
    {
        var json = await EjecutarSpAsync("listar_usuarios_con_roles", null);
        return JsonSerializer.Deserialize<List<UsuarioConRoles>>(json, _opcionesJson)
               ?? new List<UsuarioConRoles>();
    }

    public async Task<UsuarioConRoles> ConsultarAsync(string email)
    {
        var json = await EjecutarSpAsync("consultar_usuario_con_roles",
                                         new { p_email = email });
        return JsonSerializer.Deserialize<UsuarioConRoles>(json, _opcionesJson)!;
    }

    public async Task<UsuarioConRoles> CrearAsync(string email, string contrasena, string rolesJson)
    {
        var json = await EjecutarSpAsync("crear_usuario_con_roles", new
        {
            p_email = email,
            p_contrasena = contrasena,
            p_roles_json = rolesJson,
        });
        return JsonSerializer.Deserialize<UsuarioConRoles>(json, _opcionesJson)!;
    }

    public async Task<UsuarioConRoles> ActualizarAsync(string email, string? contrasena, string rolesJson)
    {
        // OJO: aquí el parámetro se llama `p_roles`, no `p_roles_json`. El
        // procedimiento de crear usa uno y el de actualizar el otro — así está
        // en la base, y se mira antes de escribirlo.
        var json = await EjecutarSpAsync("actualizar_usuario_con_roles", new
        {
            p_email = email,
            p_contrasena = contrasena ?? "",
            p_roles = rolesJson,
        });
        return JsonSerializer.Deserialize<UsuarioConRoles>(json, _opcionesJson)!;
    }

    public async Task<string> EliminarAsync(string email)
    {
        return await EjecutarSpAsync("eliminar_usuario_con_roles",
                                     new { p_email = email });
    }
}
