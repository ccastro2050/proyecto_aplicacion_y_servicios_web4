// ============================================================
// RepositorioAccesoPostgres — llama al procedimiento que ya existía.
//
// `verificar_acceso_ruta` estaba en la base desde el primer día, sin que nadie
// lo llamara. Eso es lo que cambia en la v3: no se agrega una tabla, se empieza
// a preguntar.
//
// EL PROCEDIMIENTO RECIBE EL ID DE LA RUTA, no su nombre. Así que hay un paso
// antes: traducir `interfaz.usuarios` al id que le corresponde. Se hace aquí y
// no en el atributo, porque es una consulta.
// ============================================================

using System.Text.Json;
using Dapper;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioAccesoPostgres : IRepositorioAcceso
{
    private readonly string _cadenaConexion;

    public RepositorioAccesoPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public async Task<bool> TieneAccesoAsync(string email, string nombreRuta)
    {
        await using var conexion = new NpgsqlConnection(_cadenaConexion);

        // Paso 1: el nombre a id. Si la ruta no está en la tabla, NADIE tiene
        // acceso — y eso es lo correcto: una ruta que no se declaró no se
        // concedió. Fallar cerrado, no abierto.
        var idRuta = await conexion.ExecuteScalarAsync<int?>(
            "SELECT id FROM ruta WHERE ruta = @nombre", new { nombre = nombreRuta });
        if (idRuta is null)
        {
            return false;
        }

        // Paso 2: el procedimiento. Devuelve un JSON con `tiene_acceso`.
        var json = await conexion.ExecuteScalarAsync<string?>(
            "CALL verificar_acceso_ruta(@p_email, @p_fkidruta, NULL)",
            new { p_email = email, p_fkidruta = idRuta.Value });

        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("tiene_acceso", out var v)
               && v.GetBoolean();
    }

    public async Task<List<string>> RutasPermitidasAsync(string email)
    {
        await using var conexion = new NpgsqlConnection(_cadenaConexion);

        // Este SÍ es un JOIN escrito en C#, y conviene decir por qué no
        // contradice la regla de arriba: no decide NADA. Es una lista para
        // dibujar un menú. La DECISIÓN —si una operación entra o no— la toma
        // `verificar_acceso_ruta`, y solo él.
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
