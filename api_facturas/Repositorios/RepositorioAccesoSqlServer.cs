// ============================================================
// RepositorioAccesoSqlServer — el control de acceso en T-SQL.
//
// Mismo contrato, mismo procedimiento, y UNA diferencia de dialecto que vale
// la pena ver porque es de las que fallan en silencio:
//
//   PostgreSQL devuelve  {"tiene_acceso": true}    <- un BOOLEANO
//   SQL Server devuelve  {"tiene_acceso": 1}       <- un NUMERO
//
// `GetBoolean()` sobre un `1` lanza una excepción: en JSON, `1` y `true` no
// son el mismo tipo. Si se copia el repositorio de PostgreSQL tal cual, el
// control de acceso revienta con un 500 — o peor, si alguien "arregla" la
// excepción con un `catch` que devuelva `false`, NADIE entra a nada y parece
// un problema de permisos.
//
// Se mira el `SET @p_resultado` del procedimiento, no se supone.
// ============================================================

using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiFacturas.Repositorios;

public class RepositorioAccesoSqlServer : IRepositorioAcceso
{
    private readonly string _cadenaConexion;

    public RepositorioAccesoSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public async Task<bool> TieneAccesoAsync(string email, string nombreRuta)
    {
        await using var conexion = new SqlConnection(_cadenaConexion);

        // Paso 1: el nombre a id. Si la ruta no esta declarada, NADIE entra:
        // fallar cerrado, no abierto.
        var idRuta = await conexion.ExecuteScalarAsync<int?>(
            "SELECT id FROM ruta WHERE ruta = @nombre", new { nombre = nombreRuta });
        if (idRuta is null)
        {
            return false;
        }

        // Paso 2: el procedimiento, con su parametro OUTPUT.
        var parametros = new DynamicParameters(
            new { p_email = email, p_fkidruta = idRuta.Value });
        parametros.Add("p_resultado", dbType: DbType.String,
                       direction: ParameterDirection.Output, size: -1);

        await conexion.ExecuteAsync("verificar_acceso_ruta", parametros,
            commandType: CommandType.StoredProcedure);

        var json = parametros.Get<string?>("p_resultado");
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("tiene_acceso", out var v))
        {
            return false;
        }

        // AQUI ESTA LA DIFERENCIA: se admiten las dos formas, para que este
        // repositorio no se rompa si el procedimiento cambia de 1/0 a
        // true/false. No es tolerancia a lo mal escrito: es que el contrato
        // del procedimiento dice «tiene_acceso», no «tiene_acceso booleano».
        return v.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => v.GetInt32() == 1,
            _ => false,
        };
    }

    public async Task<List<string>> RutasPermitidasAsync(string email)
    {
        await using var conexion = new SqlConnection(_cadenaConexion);

        // Este SI es un JOIN escrito en C#, y no decide nada: es una lista para
        // dibujar un menu. La DECISION la toma verificar_acceso_ruta.
        const string sql = @"SELECT DISTINCT r.ruta
                             FROM usuario u
                             INNER JOIN rol_usuario ru ON u.email = ru.fkemail
                             INNER JOIN rutarol rr ON ru.fkidrol = rr.fkidrol
                             INNER JOIN ruta r ON r.id = rr.fkidruta
                             WHERE u.email = @email
                             ORDER BY r.ruta";
        var filas = await conexion.QueryAsync<string>(sql, new { email });
        return filas.ToList();
    }
}
