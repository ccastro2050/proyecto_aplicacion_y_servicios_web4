// ============================================================
// RepositorioFacturaPostgres — la capa de DATOS de factura.
//
// La API como TRADUCTORA: no escribe SQL de tablas — llama los
// PROCEDIMIENTOS ALMACENADOS de la BD (CALL de texto; el INOUT
// p_resultado vuelve como fila y Dapper lo lee con
// ExecuteScalarAsync), deserializa su JSON a los modelos y traduce
// los RAISE EXCEPTION (SQLSTATE P0001, sin número) por PATRÓN del
// mensaje: "no existe" → 404 · "ya está anulada" → 409.
// Arriba de aquí nadie conoce PostgresException.
// ============================================================

using System.Text.Json;
using System.Text.Json.Serialization;
using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using Dapper;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioFacturaPostgres : IRepositorioFactura
{
    private readonly string _cadenaConexion;

    private static readonly JsonSerializerOptions _opcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public RepositorioFacturaPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private NpgsqlConnection CrearConexion() => new(_cadenaConexion);

    /// <summary>Ejecuta el CALL (el último argumento NULL es el INOUT
    /// p_resultado) y devuelve el JSON que el SP dejó ahí.</summary>
    private async Task<string> EjecutarSpAsync(string sqlCall, object parametros)
    {
        await using var conexion = CrearConexion();
        try
        {
            // El CALL devuelve una fila con los INOUT; su única columna
            // es p_resultado — ExecuteScalar la lee:
            var resultado = await conexion.ExecuteScalarAsync<string?>(sqlCall, parametros);
            return resultado ?? "null";
        }
        catch (PostgresException e) when (e.SqlState == "P0001"
                                          && e.MessageText.Contains("no existe"))
        {
            throw new NoEncontradoExcepcion(e.MessageText);  // → 404
        }
        catch (PostgresException e) when (e.SqlState == "P0001"
                                          && e.MessageText.Contains("anulada"))
        {
            throw new ConflictoExcepcion(e.MessageText);     // → 409
        }
        // Lo demás (stock insuficiente del trigger, FK) sube tal cual → 500.
    }

    // El SP de consultar/crear responde {"factura":{…},"productos":[…]}.
    // Esta clase privada calza ese sobre para deserializarlo y devolver
    // UNA Factura con su detalle adentro:
    private class RespuestaFacturaSp
    {
        [JsonPropertyName("factura")]
        public Factura? Factura { get; set; }

        [JsonPropertyName("productos")]
        public List<ProductoDeFactura>? Productos { get; set; }
    }

    private static Factura ArmarFactura(string json)
    {
        var respuesta = JsonSerializer.Deserialize<RespuestaFacturaSp>(json, _opcionesJson)!;
        var factura = respuesta.Factura!;
        factura.Productos = respuesta.Productos ?? new List<ProductoDeFactura>();
        return factura;
    }

    public async Task<List<Factura>> ListarAsync()
    {
        var json = await EjecutarSpAsync(
            "CALL sp_listar_facturas_y_productosporfactura(NULL)", new { });
        return JsonSerializer.Deserialize<List<Factura>>(json, _opcionesJson) ?? new List<Factura>();
    }

    public async Task<Factura> ConsultarAsync(int numero)
    {
        var json = await EjecutarSpAsync(
            "CALL sp_consultar_factura_y_productosporfactura(@p_numero, NULL)",
            new { p_numero = numero });
        return ArmarFactura(json);
    }

    public async Task<Factura> CrearAsync(int fkidcliente, int fkidvendedor, string productosJson)
    {
        // El detalle viaja como JSON en UN solo viaje — UNA transacción
        // de la BD (la lección ACID). El ::json tipa el texto:
        var json = await EjecutarSpAsync(
            "CALL sp_insertar_factura_y_productosporfactura("
            + "@p_fkidcliente, @p_fkidvendedor, @p_productos::json, 1, NULL)",
            new { p_fkidcliente = fkidcliente, p_fkidvendedor = fkidvendedor,
                  p_productos = productosJson });
        return ArmarFactura(json);
    }

    public async Task<string> AnularAsync(int numero)
    {
        return await EjecutarSpAsync(
            "CALL sp_anular_factura(@p_numero, NULL)", new { p_numero = numero });
    }
}
