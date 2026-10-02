// ============================================================
// FacturaController — la capa HTTP de factura (4 endpoints).
//
// Novedades de la v2 respecto al patrón de producto/persona:
// - Una fila NUEVA en la tabla de traducción:
//     ConflictoExcepcion → 409 (anular una factura ya anulada)
// - El POST no es un INSERT: dispara el SP transaccional (y el
//   trigger calcula todo). La respuesta trae los números que la
//   BD calculó — este controlador jamás multiplicó nada.
//
// Tabla completa: 422 forma (Program.cs) · 400 ArgumentException ·
// 404 NoEncontradoExcepcion · 409 ConflictoExcepcion · 500 resto.
// ============================================================

using System.Text.Json;
using ApiFacturas.Excepciones;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using ApiFacturas.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/factura")]
// v3 — LA PUERTA. Antes de la v3 este controlador era publico:
// cualquiera que llegara a la direccion hacia cualquier cosa.
//
//   [Authorize]      exige TOKEN. Sin token o con uno alterado o
//                    vencido: 401 -«no se quien es usted»-.
//   [ExigePermiso]   exige PERMISO. Con token valido pero sin el
//                    permiso: 403 -«se quien es, y no puede»-.
//
// Y el permiso se consulta EN CADA PETICION contra la base, no se
// lee del token: por eso quitarle el permiso a un rol surte efecto
// sin que la persona vuelva a identificarse.
[Authorize]
[ExigePermiso("interfaz.facturas")]
public class FacturaController : ControllerBase
{
    private readonly IServicioFactura _servicio;

    public FacturaController(IServicioFactura servicio)
    {
        _servicio = servicio;
    }

    // ------------------------------------------------------------
    // GET /api/factura  →  listar (con detalle anidado, vía SP)
    // ------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var facturas = await _servicio.ListarAsync();
            return Ok(new
            {
                tabla = "factura",
                total = facturas.Count,
                datos = facturas,
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/factura/{numero}  →  consultar una (vía SP)
    // ------------------------------------------------------------
    // "{numero:int}" = restricción de ruta: si no es entero, ni entra.
    [HttpGet("{numero:int}")]
    public async Task<IActionResult> Consultar(int numero)
    {
        try
        {
            var factura = await _servicio.ConsultarAsync(numero);
            return Ok(factura);
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Factura no encontrada.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // POST /api/factura  →  crear maestro-detalle (SP + trigger)
    // ------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] FacturaCrear body)
    {
        try
        {
            // Armar el JSON de renglones para el SP DESDE LA PETICIÓN ya
            // validada (lista blanca: solo codigo y cantidad viajan).
            // Las claves minúsculas son las que el SP abre con OPENJSON:
            var productosJson = JsonSerializer.Serialize(
                body.Productos!.Select(p => new { codigo = p.Codigo, cantidad = p.Cantidad }));

            var factura = await _servicio.CrearAsync(
                body.Fkidcliente!.Value, body.Fkidvendedor!.Value, productosJson);

            // La factura vuelve con fecha, subtotales y total CALCULADOS
            // por la BD — la evidencia del criterio 4:
            return Ok(factura);
        }
        catch (Exception e)
        {
            // Aquí caen: stock insuficiente (mensaje del trigger),
            // fkidcliente/fkidvendedor inexistentes (error de FK):
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // POST /api/factura/{numero}/anular  →  borrado LÓGICO (SP)
    // ------------------------------------------------------------
    // Es POST (una ACCIÓN de negocio), no DELETE: la factura no se
    // borra — cambia de estado y el stock se restaura.
    [HttpPost("{numero:int}/anular")]
    public async Task<IActionResult> Anular(int numero)
    {
        try
        {
            // El JSON del SP ES la respuesta del contrato — se emite tal
            // cual (Content con el tipo correcto), sin retiparlo:
            var json = await _servicio.AnularAsync(numero);
            return Content(json, "application/json");
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Factura no encontrada.", detalle = e.Message });
        }
        catch (ConflictoExcepcion e)
        {
            // LA FILA NUEVA: conflicto con el estado actual del recurso:
            return StatusCode(409, new { estado = 409, mensaje = "La factura ya está anulada.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
