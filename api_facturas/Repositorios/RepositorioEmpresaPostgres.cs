// ============================================================
// RepositorioEmpresaPostgres — la capa de DATOS de la empresa.
//
// SQL escrito A MANO y SIEMPRE parametrizado; DAPPER como
// micro-ejecutor: QueryAsync<T> mapea columna→propiedad por nombre
// y ExecuteAsync devuelve filas afectadas — sin Entity Framework:
// nada genera SQL por nosotros (constitución, Art. 2).
// Dialecto PostgreSQL: LIMIT @limite al final del SELECT.
// ============================================================

using ApiFacturas.Modelos;
using Dapper;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioEmpresaPostgres : IRepositorioEmpresa
{
    private readonly string _cadenaConexion;

    public RepositorioEmpresaPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    /// <summary>Conexión cerrada: Dapper la abre y cierra por operación;
    /// el "await using" del llamador la libera aunque haya error.</summary>
    private NpgsqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<List<Empresa>> ObtenerTodasAsync(int limite)
    {
        const string sql = @"SELECT codigo, nombre
                             FROM empresa ORDER BY codigo LIMIT @limite";
        await using var conexion = CrearConexion();
        var filas = await conexion.QueryAsync<Empresa>(sql, new { limite });
        return filas.ToList();
    }

    public async Task<Empresa?> ObtenerPorCodigoAsync(string codigo)
    {
        const string sql = @"SELECT codigo, nombre
                             FROM empresa WHERE codigo = @codigo";
        await using var conexion = CrearConexion();
        // Una fila → el modelo; cero filas → null (el SERVICIO decide qué
        // significa ese null — aquí solo hay hechos):
        return await conexion.QueryFirstOrDefaultAsync<Empresa>(sql, new { codigo });
    }

    public async Task CrearAsync(Empresa entidad)
    {
        const string sql = @"INSERT INTO empresa (codigo, nombre)
                             VALUES (@Codigo, @Nombre)";
        await using var conexion = CrearConexion();
        // El OBJETO del modelo como fuente de parámetros (@Propiedad):
        await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        // SET dinámico SOLO con las columnas que llegaron (PUT manda todas,
        // PATCH un subconjunto). Los NOMBRES salen de las PETICIONES (lista
        // blanca) — jamás del cliente; los VALORES van parametrizados:
        var asignaciones = string.Join(", ", datos.Keys.Select(c => $"{c} = @{c}"));
        var sql = $"UPDATE empresa SET {asignaciones} WHERE codigo = @pk_clave";
        var parametros = new DynamicParameters(datos);
        parametros.Add("pk_clave", codigo);
        await using var conexion = CrearConexion();
        // ExecuteAsync devuelve las FILAS AFECTADAS (0 = no existía):
        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarAsync(string codigo)
    {
        // Si otras tablas lo referencian, la FK del motor rechaza → 500:
        const string sql = "DELETE FROM empresa WHERE codigo = @codigo";
        await using var conexion = CrearConexion();
        return await conexion.ExecuteAsync(sql, new { codigo });
    }
}
