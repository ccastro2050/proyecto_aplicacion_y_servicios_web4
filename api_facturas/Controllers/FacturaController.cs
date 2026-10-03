// ============================================================
// FacturaController — la capa HTTP de factura (7 endpoints):
// los CINCO verbos del recurso, mas el POST de anular.
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
    // ==================================================================
    // [HttpPut("{numero:int}")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v2 (§C) dice: «api/factura — maestro-detalle por
    // procedimientos (4 ENDPOINTS)» y «NO HAY PUT NI PATCH. Una factura
    // emitida es un documento. No se corrige: se anula y se hace otra».
    //
    // POR QUÉ ENTONCES ESTÁ ESCRITO: porque hay dos cosas que aprender y las
    // dos importan. Una es cómo SE PROGRAMA este verbo —léalo abajo, está
    // completo y comentado—. La otra es que una API NO lleva todos los verbos
    // en todos los recursos: el contrato manda, y el contrato dice que éste no.
    // Borrarlo enseñaría solo la segunda; dejarlo apagado enseña las dos.
    //
    // Para encenderlo basta con quitar las barras de estas líneas: lo que hay
    // debajo en el servicio y en el repositorio SÍ está activo.
    //
    // // ------------------------------------------------------------
    // // PUT /api/factura/{numero}  →  reemplazo COMPLETO (SP + trigger)
    // // ------------------------------------------------------------
    // // Reemplazar exige la ficha ENTERA: cliente, vendedor y TODOS los
    // // renglones. Un PUT al que le falte la lista muere en 422 antes de
    // // llegar aquí — esa es la semántica de PUT, y es lo que lo separa
    // // del PATCH de más abajo.
    // //
    // // Adentro, el SP devuelve el stock de los renglones viejos, borra el
    // // detalle e inserta el nuevo, todo en UNA transacción.
    // [HttpPut("{numero:int}")]
    public async Task<IActionResult> Reemplazar(int numero, [FromBody] FacturaReemplazo body)
    {
        try
        {
            var productosJson = JsonSerializer.Serialize(
                body.Productos!.Select(p => new { codigo = p.Codigo, cantidad = p.Cantidad }));

            var factura = await _servicio.ReemplazarAsync(
                numero, body.Fkidcliente!.Value, body.Fkidvendedor!.Value, productosJson);

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
            // Stock insuficiente (mensaje del trigger) o FK inexistente:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
    // ==================================================================
    // [HttpPatch("{numero:int}")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // Lo mismo que el PUT de arriba: el contrato de la v2 (§C) no lo pide.
    // Y aquí abajo está escrito cómo sería el PATCH de un maestro-detalle,
    // que es la parte que vale la pena leer aunque esté apagada.
    //
    // POR QUÉ ENTONCES ESTÁ ESCRITO: porque hay dos cosas que aprender y las
    // dos importan. Una es cómo SE PROGRAMA este verbo —léalo abajo, está
    // completo y comentado—. La otra es que una API NO lleva todos los verbos
    // en todos los recursos: el contrato manda, y el contrato dice que éste no.
    // Borrarlo enseñaría solo la segunda; dejarlo apagado enseña las dos.
    //
    // Para encenderlo basta con quitar las barras de estas líneas: lo que hay
    // debajo en el servicio y en el repositorio SÍ está activo.
    //
    // // ------------------------------------------------------------
    // // PATCH /api/factura/{numero}  →  actualización PARCIAL
    // // ------------------------------------------------------------
    // // AQUÍ ESTÁ LA LECCIÓN DE ESTE CONTROLADOR, y por eso vale la pena
    // // leerlo aunque ya se haya leído el PATCH de producto:
    // //
    // // El procedimiento de la base exige los tres datos — no sabe escribir
    // // «solo el vendedor». Así que un PATCH sobre un maestro-detalle LEE la
    // // factura, MEZCLA lo que llegó y la reenvía completa.
    // //
    // // Y esa mezcla la hace el SERVICIO, no este archivo: decidir qué se
    // // conserva es una regla de negocio; el controlador solo traduce HTTP.
    // //
    // // Un body `{}` pasa la validación de forma —ningún campo es
    // // obligatorio— y lo rechaza el servicio con un 400: «no mandó nada
    // // que cambiar» no es un error de FORMA.
    // [HttpPatch("{numero:int}")]
    public async Task<IActionResult> Actualizar(int numero, [FromBody] FacturaActualizar body)
    {
        try
        {
            // Si no mandó productos, va null y el servicio conserva los que
            // hay. OJO: null NO es lo mismo que lista vacía.
            string? productosJson = body.Productos == null
                ? null
                : JsonSerializer.Serialize(
                    body.Productos.Select(p => new { codigo = p.Codigo, cantidad = p.Cantidad }));

            var factura = await _servicio.ActualizarAsync(
                numero, body.Fkidcliente, body.Fkidvendedor, productosJson);

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
    // ==================================================================
    // [HttpDelete("{numero:int}")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v2 lo dice en su tabla de arriba: «UN RECURSO SIN
    // PUT NI PATCH NI DELETE — api/factura. Una factura emitida no se
    // corrige: se anula». El POST de anular, que sí va, está más abajo.
    //
    // POR QUÉ ENTONCES ESTÁ ESCRITO: porque hay dos cosas que aprender y las
    // dos importan. Una es cómo SE PROGRAMA este verbo —léalo abajo, está
    // completo y comentado—. La otra es que una API NO lleva todos los verbos
    // en todos los recursos: el contrato manda, y el contrato dice que éste no.
    // Borrarlo enseñaría solo la segunda; dejarlo apagado enseña las dos.
    //
    // Para encenderlo basta con quitar las barras de estas líneas: lo que hay
    // debajo en el servicio y en el repositorio SÍ está activo.
    //
    // // ------------------------------------------------------------
    // // DELETE /api/factura/{numero}  →  borrado FÍSICO (SP)
    // // ------------------------------------------------------------
    // // LA FILA SE VA. Compárelo con el POST de anular, que está justo
    // // debajo: anular deja la factura en la base, con su número y su
    // // fecha, marcada como anulada. Las dos devuelven el stock.
    // //
    // // La API ofrece las dos **a propósito**, para que se vea la
    // // diferencia entre un borrado físico y uno lógico sobre el mismo
    // // recurso. Cuál usa un negocio de verdad es otra conversación.
    // [HttpDelete("{numero:int}")]
    public async Task<IActionResult> Eliminar(int numero)
    {
        try
        {
            var json = await _servicio.EliminarAsync(numero);
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
        catch (Exception e)
        {
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
