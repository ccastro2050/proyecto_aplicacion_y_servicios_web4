// ============================================================
// RepositorioFacturaSqlServer — la capa de DATOS de factura.
//
// La API como TRADUCTORA: llama los PROCEDIMIENTOS ALMACENADOS de
// la BD con Dapper (CommandType.StoredProcedure + el parámetro
// @p_resultado OUTPUT vía DynamicParameters), deserializa su JSON y
// traduce los THROW NUMERADOS: 50003/50010 "no existe" → 404 ·
// 50010 "ya está anulada" → 409. SQL Server sí numera sus errores —
// el filtro por número es más preciso que el patrón (la lección de
// dialectos). Arriba de aquí nadie conoce SqlException.
// ============================================================

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiFacturas.Repositorios;

public class RepositorioFacturaSqlServer : IRepositorioFactura
{
    private readonly string _cadenaConexion;

    private static readonly JsonSerializerOptions _opcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public RepositorioFacturaSqlServer(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private SqlConnection CrearConexion() => new(_cadenaConexion);

    /// <summary>Ejecuta el SP con Dapper y devuelve el JSON del
    /// parámetro @p_resultado OUTPUT (size -1 = NVARCHAR(MAX)).</summary>
    private async Task<string> EjecutarSpAsync(string nombreSp, object? entrada)
    {
        var parametros = entrada == null
            ? new DynamicParameters()
            : new DynamicParameters(entrada);
        parametros.Add("p_resultado", dbType: DbType.String,
                       direction: ParameterDirection.Output, size: -1);

        await using var conexion = CrearConexion();
        try
        {
            await conexion.ExecuteAsync(nombreSp, parametros,
                commandType: CommandType.StoredProcedure);
        }
        // Números reales de db/: 50003 = "no existe" de sp_consultar ·
        // 50010 = los dos errores de sp_anular:
        catch (SqlException e) when ((e.Number == 50003 || e.Number == 50010)
                                     && e.Message.Contains("no existe"))
        {
            throw new NoEncontradoExcepcion(e.Message);      // → 404
        }
        catch (SqlException e) when (e.Number == 50010 && e.Message.Contains("anulada"))
        {
            throw new ConflictoExcepcion(e.Message);         // → 409
        }
        // Lo demás (stock insuficiente, mínimo de renglones, FK) → 500.

        return parametros.Get<string?>("p_resultado") ?? "null";
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
        var json = await EjecutarSpAsync("sp_listar_facturas_y_productosporfactura", null);
        return JsonSerializer.Deserialize<List<Factura>>(json, _opcionesJson) ?? new List<Factura>();
    }

    public async Task<Factura> ConsultarAsync(int numero)
    {
        var json = await EjecutarSpAsync("sp_consultar_factura_y_productosporfactura",
            new { p_numero = numero });
        return ArmarFactura(json);
    }

    public async Task<Factura> CrearAsync(int fkidcliente, int fkidvendedor, string productosJson)
    {
        // El detalle viaja como JSON y el SP lo abre con OPENJSON — un
        // solo viaje a la BD, UNA transacción (la lección ACID):
        var json = await EjecutarSpAsync("sp_insertar_factura_y_productosporfactura",
            new { p_fkidcliente = fkidcliente, p_fkidvendedor = fkidvendedor,
                  p_productos = productosJson });
        return ArmarFactura(json);
    }

    public async Task<string> AnularAsync(int numero)
    {
        return await EjecutarSpAsync("sp_anular_factura", new { p_numero = numero });
    }
}
