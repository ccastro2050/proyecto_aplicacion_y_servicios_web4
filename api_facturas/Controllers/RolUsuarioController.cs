// RolUsuarioController — la capa HTTP del puente rol_usuario (v3).
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
[Route("api/rol-usuario")]
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
[ExigePermiso("interfaz.usuarios")]
public class RolUsuarioController : ControllerBase
{
    private readonly IServicioRolUsuario _servicio;

    public RolUsuarioController(IServicioRolUsuario servicio)
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
            return Ok(new { tabla = "rol_usuario", limite, total = lista.Count, datos = lista });
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

    [HttpGet("usuario/{email}")]
    public async Task<IActionResult> PorLadoA(string email)
    {
        try
        {
            var lista = await _servicio.ObtenerPorUsuarioAsync(email);
            return Ok(new { tabla = "rol_usuario", total = lista.Count, datos = lista });
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
            return Ok(new { tabla = "rol_usuario", total = lista.Count, datos = lista });
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
    public async Task<IActionResult> Crear([FromBody] RolUsuarioCrear body)
    {
        try
        {
            var asignacion = new RolUsuario
            {
                Fkemail = body.Fkemail!,
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
    // ==================================================================
    // [HttpPut("{email}/{idrol:int}")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v2 (§D) dice: «NO HAY PUT NI PATCH. La fila no
    // tiene campos: EXISTE O NO EXISTE». Para mover una asignación se
    // quita y se pone otra.
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
    // // PUT /api/rolusuario/{email}/{idrol}  →  MOVER la asignación
    // // ------------------------------------------------------------
    // // EN UNA TABLA PUENTE «ACTUALIZAR» ES MOVER LA FILA, y ésa es la
    // // lección de estos dos verbos.
    // //
    // // Las dos columnas SON la llave primaria: no hay un campo suelto que
    // // cambiar. Así que el PUT recibe la pareja nueva ENTERA, borra la
    // // vieja e inserta la nueva — en una transacción, porque si el INSERT
    // // falla el DELETE no puede quedarse hecho.
    // [HttpPut("{email}/{idrol:int}")]
    public async Task<IActionResult> Reemplazar(string email, int idrol,
                                                [FromBody] RolUsuarioCrear body)
    {
        try
        {
            var nueva = new RolUsuario { Fkemail = body.Fkemail!, Fkidrol = body.Fkidrol!.Value };
            var filas = await _servicio.ReemplazarAsync(email, idrol, nueva);
            return Ok(new { estado = 200, mensaje = "Asignación reemplazada exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Asignación no encontrada.", detalle = e.Message });
        }
        catch (Exception e)
        {
            // La pareja nueva ya existía (PK duplicada) o el rol/usuario no
            // existe (FK): la base rechaza y el rollback deja todo como estaba.
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
    // ==================================================================
    // [HttpPatch("{email}/{idrol:int}")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v2 (§D): «No hay PUT ni PATCH. La fila no tiene
    // campos: existe o no existe».
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
    // // PATCH /api/rolusuario/{email}/{idrol}  →  mover UN lado
    // // ------------------------------------------------------------
    // // Igual que el PUT, pero llega solo el lado que cambia y el otro se
    // // conserva: `{"fkidrol": 3}` le cambia el rol a la misma persona;
    // // `{"fkemail": "otro@correo.com"}` le pasa ese rol a otra.
    // [HttpPatch("{email}/{idrol:int}")]
    public async Task<IActionResult> Actualizar(string email, int idrol,
                                                [FromBody] RolUsuarioActualizar body)
    {
        try
        {
            var filas = await _servicio.ActualizarAsync(email, idrol, body.Fkemail, body.Fkidrol);
            return Ok(new { estado = 200, mensaje = "Asignación actualizada exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Asignación no encontrada.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpDelete("{email}/{idrol:int}")]
    public async Task<IActionResult> Eliminar(string email, int idrol)
    {
        try
        {
            var filas = await _servicio.EliminarAsync(email, idrol);
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
