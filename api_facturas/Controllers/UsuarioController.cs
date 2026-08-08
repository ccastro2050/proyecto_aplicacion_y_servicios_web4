// ============================================================
// UsuarioController — la capa HTTP de usuario (v3).
//
// El CRUD del molde (con la petición de cada verbo reducida a la
// contraseña) + el endpoint especial verificar-contrasena: el
// cimiento del login real que llegará con JWT en su versión.
// Las respuestas de lectura JAMÁS incluyen la contraseña (el
// modelo Usuario no la tiene — no PUEDE filtrarse).
// ============================================================

using ApiFacturas.Excepciones;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using ApiFacturas.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/usuario")]
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
public class UsuarioController : ControllerBase
{
    private readonly IServicioUsuario _servicio;

    public UsuarioController(IServicioUsuario servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int limite = 1000)
    {
        try
        {
            var usuarios = await _servicio.ListarAsync(limite);
            if (usuarios.Count == 0) { return NoContent(); }
            return Ok(new { tabla = "usuario", limite, total = usuarios.Count, datos = usuarios });
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

    [HttpGet("{email}")]
    public async Task<IActionResult> Obtener(string email)
    {
        try
        {
            return Ok(await _servicio.ObtenerAsync(email));
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Usuario no encontrado.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuarioCrear body)
    {
        try
        {
            await _servicio.CrearAsync(body.Email!, body.Contrasena!);
            return Ok(new { estado = 200, mensaje = "Usuario creado exitosamente." });
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

    [HttpPut("{email}")]
    public async Task<IActionResult> Reemplazar(string email, [FromBody] UsuarioReemplazo body)
    {
        try
        {
            var filas = await _servicio.ActualizarContrasenaAsync(email, body.Contrasena);
            return Ok(new { estado = 200, mensaje = "Usuario reemplazado exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Usuario no encontrado.", detalle = e.Message });
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

    [HttpPatch("{email}")]
    public async Task<IActionResult> Actualizar(string email, [FromBody] UsuarioActualizar body)
    {
        try
        {
            var filas = await _servicio.ActualizarContrasenaAsync(email, body.Contrasena);
            return Ok(new { estado = 200, mensaje = "Usuario actualizado exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Usuario no encontrado.", detalle = e.Message });
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

    [HttpDelete("{email}")]
    public async Task<IActionResult> Eliminar(string email)
    {
        try
        {
            var filas = await _servicio.EliminarAsync(email);
            return Ok(new { estado = 200, mensaje = "Usuario eliminado exitosamente.", filasEliminadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Usuario no encontrado.", detalle = e.Message });
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
            // Con roles asignados, la FK de rol_usuario rechaza → 500:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
    // ==================================================================
    // [HttpPost("verificar-contrasena")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v3 (§A) dice: «POST /api/sesion — EL ÚNICO
    // ENDPOINT NUEVO», y ningún contrato declara éste.
    //
    // Y hay una razón de fondo que conviene leer antes de encenderlo: ES UN
    // ORÁCULO DE CONTRASEÑAS. Responde si una clave es correcta, de a una,
    // sin límite de intentos — y el criterio 2 de esa misma versión obliga a
    // que el login dé EL MISMO mensaje para «el correo no existe» y «la clave
    // está mal», justamente para no confirmarle a un desconocido qué correos
    // existen. Este endpoint deshace eso.
    //
    // El login NO lo necesita: `ServicioSesion.IniciarAsync` llama al servicio
    // de usuario DIRECTAMENTE, en el mismo proceso. Nunca pasó por HTTP.
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
    // // POST /api/usuario/verificar-contrasena — el cimiento del login
    // // ------------------------------------------------------------
    // [HttpPost("verificar-contrasena")]
    // public async Task<IActionResult> VerificarContrasena(
    // [FromQuery(Name = "valor_usuario")] string valorUsuario,
    // [FromQuery(Name = "valor_contrasena")] string valorContrasena)
    // {
    // try
    // {
    // var (codigo, mensaje) = await _servicio.VerificarContrasenaAsync(valorUsuario, valorContrasena);
    // if (codigo == 200)
    // {
    // return Ok(new { estado = 200, mensaje, usuario = valorUsuario });
    // }
    // return StatusCode(codigo, new { estado = codigo, mensaje, usuario = valorUsuario });
    // }
    // catch (ArgumentException e)
    // {
    // return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
    // }
    // catch (ConflictoExcepcion e)
    // {
    // // 409, y NO 422: el dato tiene la forma correcta -lo paso la
    // // validacion de la peticion- y lo que se rompe es el ESTADO de la
    // // base. Tres causas posibles: la clave foranea apunta a una fila
    // // que no existe, la clave ya esta usada, o hay otra fila que
    // // depende de esta y el motor no deja borrarla.
    // return StatusCode(409, new
    // {
    // estado = 409,
    // mensaje = "La operacion choca con los datos que ya existen.",
    // detalle = e.Message,
    // });
    // }
    // catch (Exception e)
    // {
    // return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
    // }
    // }
}
