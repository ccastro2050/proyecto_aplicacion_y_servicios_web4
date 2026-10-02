// ============================================================
// PermisosController — «¿a qué puedo entrar yo?»
//
// Es lo que la interfaz gráfica consulta para armar su menú, y existe por una
// razón práctica: sin esto, el front tendría que pedir `rutarol` completo y
// cruzarlo con los roles del token — repitiendo en el navegador el JOIN que la
// base ya sabe hacer.
//
// Y HAY QUE DECIR LO QUE ESTE ENDPOINT NO ES:
//
//   NO es el control de acceso. Es una lista para dibujar un menú. Quien
//   esconda un botón con esta lista y no ponga [ExigePermiso] en el
//   controlador, no protegió nada: el menú es HTML que ya está en el
//   navegador de quien pregunta, y la dirección se puede escribir a mano.
//
// Solo exige TOKEN —cualquiera identificado puede preguntar por sus propios
// permisos— y no exige permiso: pedirle permiso para saber sus permisos sería
// un círculo.
// ============================================================

using ApiFacturas.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/permisos")]
[Authorize]
public class PermisosController : ControllerBase
{
    private readonly IRepositorioAcceso _acceso;

    public PermisosController(IRepositorioAcceso acceso)
    {
        _acceso = acceso;
    }

    // ------------------------------------------------------------
    // GET /api/permisos/mios  →  las rutas de QUIEN PREGUNTA
    // ------------------------------------------------------------
    // El correo sale del TOKEN, no de la URL. Y eso no es un detalle: si
    // viniera por parámetro, cualquiera podría preguntar por los permisos de
    // otro —y de paso averiguar qué correos existen—.
    [HttpGet("mios")]
    public async Task<IActionResult> Mios()
    {
        try
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
            {
                return StatusCode(401, new { estado = 401, mensaje = "No hay una sesion valida." });
            }

            var rutas = await _acceso.RutasPermitidasAsync(email);
            return Ok(new
            {
                email,
                total = rutas.Count,
                datos = rutas,
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
