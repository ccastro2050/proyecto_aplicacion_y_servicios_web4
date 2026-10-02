// ============================================================
// SesionController — el único endpoint al que se entra SIN token.
//
// Y tiene que ser así, porque no puede exigir lo que todavía no existe. Junto
// con el diagnóstico `/`, son los dos [AllowAnonymous] de toda la API.
//
// LA RESPUESTA AL FALLAR ES LA MISMA EN LOS DOS CASOS —correo inexistente y
// contraseña equivocada— y es el criterio 2. Un 404 para el primero le
// confirmaría a un desconocido qué correos sí existen.
// ============================================================

using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/sesion")]
[AllowAnonymous]
public class SesionController : ControllerBase
{
    private readonly IServicioSesion _servicio;

    public SesionController(IServicioSesion servicio)
    {
        _servicio = servicio;
    }

    // ------------------------------------------------------------
    // POST /api/sesion  →  el token, si las credenciales sirven
    // ------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Iniciar([FromBody] SesionCrear body)
    {
        try
        {
            var sesion = await _servicio.IniciarAsync(body.Email!, body.Contrasena!);

            if (sesion is null)
            {
                // 401 y no 404, y el MISMO mensaje para los dos casos.
                return StatusCode(401, new
                {
                    estado = 401,
                    mensaje = "El correo o la contrasena no son correctos.",
                });
            }

            return Ok(new
            {
                estado = 200,
                mensaje = "Sesion iniciada.",
                token = sesion.Token,
                email = sesion.Email,
                roles = sesion.Roles,
                expira = sesion.Expira,
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
