// ============================================================
// RepositorioRutaSqlServer — la capa de DATOS de la ruta.
//
// SQL escrito A MANO y SIEMPRE parametrizado; DAPPER como
// micro-ejecutor: QueryAsync<T> mapea columna→propiedad por nombre
// y ExecuteAsync devuelve filas afectadas — sin Entity Framework:
// nada genera SQL por nosotros (constitución, Art. 2).
// Dialecto SQL Server: TOP (@limite) al PRINCIPIO del SELECT (T-SQL no tiene LIMIT).
// ============================================================

using ApiFacturas.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiFacturas.Repositorios;

public class RepositorioRutaSqlServer : IRepositorioRuta
{
    private readonly string _cadenaConexion;

    public RepositorioRutaSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    /// <summary>Conexión cerrada: Dapper la abre y cierra por operación;
    /// el "await using" del llamador la libera aunque haya error.</summary>
    private SqlConnection CrearConexion() => new(_cadenaConexion);

    public async Task<List<Ruta>> ObtenerTodasAsync(int limite)
    {
        // "ruta AS Valor": la columna se llama como la clase, así que la
        // propiedad C# es Valor — el alias deja que Dapper la mapee sola.
        const string sql = @"SELECT TOP (@limite) id, ruta AS Valor, descripcion
                             FROM ruta ORDER BY id";
        await using var conexion = CrearConexion();
        var filas = await conexion.QueryAsync<Ruta>(sql, new { limite });
        return filas.ToList();
    }

    public async Task<Ruta?> ObtenerPorIdAsync(int id)
    {
        const string sql = @"SELECT id, ruta AS Valor, descripcion
                             FROM ruta WHERE id = @id";
        await using var conexion = CrearConexion();
        // Una fila → el modelo; cero filas → null (el SERVICIO decide qué
        // significa ese null — aquí solo hay hechos):
        return await conexion.QueryFirstOrDefaultAsync<Ruta>(sql, new { id });
    }

    public async Task CrearAsync(Ruta entidad)
    {
        const string sql = @"INSERT INTO ruta (ruta, descripcion) VALUES (@Valor, @Descripcion)";
        await using var conexion = CrearConexion();
        // El OBJETO del modelo como fuente de parámetros (@Propiedad):
        await conexion.ExecuteAsync(sql, entidad);
    }

    public async Task<int> ActualizarAsync(int id, Dictionary<string, object> datos)
    {
        // SET dinámico SOLO con las columnas que llegaron (PUT manda todas,
        // PATCH un subconjunto). Los NOMBRES salen de las PETICIONES (lista
        // blanca) — jamás del cliente; los VALORES van parametrizados:
        var asignaciones = string.Join(", ", datos.Keys.Select(c => $"{c} = @{c}"));
        var sql = $"UPDATE ruta SET {asignaciones} WHERE id = @pk_clave";
        var parametros = new DynamicParameters(datos);
        parametros.Add("pk_clave", id);
        await using var conexion = CrearConexion();
        // ExecuteAsync devuelve las FILAS AFECTADAS (0 = no existía):
        return await conexion.ExecuteAsync(sql, parametros);
    }

    public async Task<int> EliminarAsync(int id)
    {
        // Si otras tablas lo referencian, la FK del motor rechaza → 500:
        const string sql = "DELETE FROM ruta WHERE id = @id";
        await using var conexion = CrearConexion();
        return await conexion.ExecuteAsync(sql, new { id });
    }
}
