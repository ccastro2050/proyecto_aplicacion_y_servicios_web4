// VendedorController — la capa HTTP de vendedor (v3). CALCADO del molde
// de producto/persona: mismos 6 métodos, misma tabla de códigos.

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using ApiFacturas.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/vendedor")]
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
[ExigePermiso("interfaz.vendedores")]
public class VendedorController : ControllerBase
{
    private readonly IServicioVendedor _servicio;

    public VendedorController(IServicioVendedor servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int limite = 1000)
    {
        try
        {
            var lista = await _servicio.ListarAsync(limite);
            if (lista.Count == 0) { return NoContent(); }
            return Ok(new { tabla = "vendedor", limite, total = lista.Count, datos = lista });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id)
    {
        try
        {
            return Ok(await _servicio.ObtenerAsync(id));
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Vendedor no encontrado.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] VendedorCrear body)
    {
        try
        {
            var entidad = new Vendedor
            {
                Carnet = body.Carnet!.Value,
                Direccion = body.Direccion!,
                Fkcodpersona = body.Fkcodpersona!,
            };
            await _servicio.CrearAsync(entidad);
            return Ok(new { estado = 200, mensaje = "Vendedor creado exitosamente." });
        }
        catch (Exception e)
        {
            // PK duplicada o FK inexistente: la BD rechaza → 500 con detalle:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reemplazar(int id, [FromBody] VendedorReemplazo body)
    {
        try
        {
            var datos = new Dictionary<string, object>
            {
                ["carnet"] = body.Carnet!.Value,
                ["direccion"] = body.Direccion!,
                ["fkcodpersona"] = body.Fkcodpersona!,
            };
            var filas = await _servicio.ActualizarAsync(id, datos);
            return Ok(new { estado = 200, mensaje = "Vendedor reemplazado exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Vendedor no encontrado.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] VendedorActualizar body)
    {
        try
        {
            var datos = new Dictionary<string, object>();
            if (body.Carnet != null) { datos["carnet"] = body.Carnet.Value; }
            if (body.Direccion != null) { datos["direccion"] = body.Direccion; }
            if (body.Fkcodpersona != null) { datos["fkcodpersona"] = body.Fkcodpersona; }
            var filas = await _servicio.ActualizarAsync(id, datos);
            return Ok(new { estado = 200, mensaje = "Vendedor actualizado exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Vendedor no encontrado.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var filas = await _servicio.EliminarAsync(id);
            return Ok(new { estado = 200, mensaje = "Vendedor eliminado exitosamente.", filasEliminadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Vendedor no encontrado.", detalle = e.Message });
        }
        catch (Exception e)
        {
            // Ej.: eliminar con hijos (FK) → la BD rechaza:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
