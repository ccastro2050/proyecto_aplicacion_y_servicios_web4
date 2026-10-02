// RutaRolController — la capa HTTP del puente rutarol (v3).
// Sin PUT/PATCH (una asignación no se edita); el DELETE recibe LA
// PAREJA en la URL y borra exactamente esa.

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using ApiFacturas.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/rutarol")]
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
[ExigePermiso("interfaz.permisos")]
public class RutaRolController : ControllerBase
{
    private readonly IServicioRutaRol _servicio;

    public RutaRolController(IServicioRutaRol servicio)
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
            return Ok(new { tabla = "rutarol", limite, total = lista.Count, datos = lista });
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

    [HttpGet("ruta/{idruta:int}")]
    public async Task<IActionResult> PorLadoA(int idruta)
    {
        try
        {
            var lista = await _servicio.ObtenerPorRutaAsync(idruta);
            return Ok(new { tabla = "rutarol", total = lista.Count, datos = lista });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Sin asignaciones.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpGet("rol/{idrol:int}")]
    public async Task<IActionResult> PorLadoB(int idrol)
    {
        try
        {
            var lista = await _servicio.ObtenerPorRolAsync(idrol);
            return Ok(new { tabla = "rutarol", total = lista.Count, datos = lista });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Sin asignaciones.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] RutaRolCrear body)
    {
        try
        {
            var asignacion = new RutaRol
            {
                Fkidruta = body.Fkidruta!.Value,
                Fkidrol = body.Fkidrol!.Value,
            };
            await _servicio.CrearAsync(asignacion);
            return Ok(new { estado = 200, mensaje = "Asignación creada exitosamente." });
        }
        catch (ConflictoExcepcion e)
        {
            // 409, y NO 422: el dato tiene la forma correcta -lo paso la
            // validacion de la peticion- y lo que se rompe es el ESTADO de la
            // base. Tres causas posibles: la clave foranea apunta a una fila
            // que no existe, la clave ya esta usada, o hay otra fila que
            // depende de esta y el motor no deja borrarla.
            return StatusCode(409, new
            {
                estado = 409,
                mensaje = "La operacion choca con los datos que ya existen.",
                detalle = e.Message,
            });
        }
        catch (Exception e)
        {
            // Duplicado (PK compuesta) o llave inexistente (FK) → 500:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpDelete("{idruta:int}/{idrol:int}")]
    public async Task<IActionResult> Eliminar(int idruta, int idrol)
    {
        try
        {
            var filas = await _servicio.EliminarAsync(idruta, idrol);
            return Ok(new { estado = 200, mensaje = "Asignación eliminada exitosamente.", filasEliminadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Asignación no encontrada.", detalle = e.Message });
        }
        catch (ConflictoExcepcion e)
        {
            // 409, y NO 422: el dato tiene la forma correcta -lo paso la
            // validacion de la peticion- y lo que se rompe es el ESTADO de la
            // base. Tres causas posibles: la clave foranea apunta a una fila
            // que no existe, la clave ya esta usada, o hay otra fila que
            // depende de esta y el motor no deja borrarla.
            return StatusCode(409, new
            {
                estado = 409,
                mensaje = "La operacion choca con los datos que ya existen.",
                detalle = e.Message,
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
