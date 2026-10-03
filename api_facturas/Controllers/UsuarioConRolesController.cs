// ============================================================
// UsuarioConRolesController — el recurso MAESTRO-DETALLE de la v2 sobre una
// tabla puente.
//
// Convive con UsuarioController y con RolUsuarioController, y la diferencia
// vale la pena:
//
//   api/usuario        el CRUD de la tabla `usuario` sola         (v1)
//   api/rol-usuario    el CRUD de la tabla puente, pareja a pareja (v2)
//   api/usuario-con-roles  el usuario Y sus roles en UNA operación (v2)
//
// Los tres existen porque los tres resuelven cosas distintas. El tercero es el
// que usa la interfaz gráfica, porque es el único que no deja a medias un
// usuario cuando falla la segunda llamada.
//
// LA RUTA LLEVA GUIONES: `api/usuario-con-roles`. El nombre del procedimiento
// usa subrayados (`crear_usuario_con_roles`) y son dos convenciones distintas
// —URL y SQL—. Asumir una por la otra da 404.
// ============================================================

using ApiFacturas.Excepciones;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using ApiFacturas.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/usuario-con-roles")]
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
public class UsuarioConRolesController : ControllerBase
{
    private readonly IServicioUsuarioConRoles _servicio;

    public UsuarioConRolesController(IServicioUsuarioConRoles servicio)
    {
        _servicio = servicio;
    }

    // ------------------------------------------------------------
    // GET /api/usuario-con-roles  →  todos, con sus roles anidados
    // ------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        try
        {
            var lista = await _servicio.ListarAsync();
            return Ok(new
            {
                tabla = "usuario_con_roles",
                total = lista.Count,
                datos = lista,
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/usuario-con-roles/{email}
    // ------------------------------------------------------------
    [HttpGet("{email}")]
    public async Task<IActionResult> Consultar(string email)
    {
        try
        {
            return Ok(await _servicio.ConsultarAsync(email));
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

    // ------------------------------------------------------------
    // POST /api/usuario-con-roles  →  el usuario Y sus roles, UNA transacción
    // ------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] UsuarioConRolesCrear body)
    {
        try
        {
            var creado = await _servicio.CrearAsync(
                body.Email!, body.Contrasena!, body.Roles!);
            return Ok(creado);
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (ConflictoExcepcion e)
        {
            return StatusCode(409, new
            {
                estado = 409,
                mensaje = "Ese correo ya está registrado, o uno de los roles no existe.",
                detalle = e.Message,
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // PUT /api/usuario-con-roles/{email}  →  reemplaza los roles
    // ------------------------------------------------------------
    // Es PUT y no PATCH porque los roles se REEMPLAZAN: la lista que llega es
    // la lista que queda. La contraseña vacía es la única excepción, y está
    // documentada en la petición.
    [HttpPut("{email}")]
    public async Task<IActionResult> Reemplazar(string email, [FromBody] UsuarioConRolesActualizar body)
    {
        try
        {
            var actualizado = await _servicio.ActualizarAsync(email, body.Contrasena, body.Roles!);
            return Ok(actualizado);
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
            return StatusCode(409, new { estado = 409, mensaje = "Uno de los roles no existe.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
    // ==================================================================
    // [HttpPatch("{email}")]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v2 (§F) dice: «api/usuario-con-roles — el recurso
    // que no es una tabla (5 ENDPOINTS)». Este PATCH sería el sexto.
    // El PUT de arriba ya reemplaza la lista de roles, que es lo que pide.
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
    // // PATCH /api/usuario-con-roles/{email}  →  cambio PARCIAL
    // // ------------------------------------------------------------
    // // LA DIFERENCIA CON EL PUT DE ARRIBA, que es la lección de este par:
    // //
    // //   PUT   exige la lista de roles — la que llega es la que queda.
    // //   PATCH no la exige: si no llega, LOS ROLES NO SE TOCAN.
    // //
    // // Por eso `{"contrasena": "nueva123"}` le cambia la clave a alguien sin
    // // rozarle los roles, algo que con el PUT obligaría a reenviar la lista
    // // entera — y a arriesgarse a equivocarla.
    // //
    // // Quién decide qué se conserva es el SERVICIO: aquí solo se lee lo que
    // // llegó y se le pasa; mezclar es una regla de negocio, no de HTTP.
    // [HttpPatch("{email}")]
    public async Task<IActionResult> Actualizar(string email,
                                                [FromBody] UsuarioConRolesParcial body)
    {
        try
        {
            if (body.Contrasena == null && body.Roles == null)
            {
                // Forma válida y nada que hacer: 400, no 422.
                return StatusCode(400, new
                {
                    estado = 400,
                    mensaje = "Parámetros inválidos.",
                    detalle = "No se envió ningún campo para actualizar.",
                });
            }

            // Si no mandó roles, se conservan los que tiene: se leen y se
            // reenvían, porque el procedimiento de la BD reemplaza la lista
            // entera y no sabe «dejarla como está».
            var roles = body.Roles;
            if (roles == null)
            {
                var actual = await _servicio.ConsultarAsync(email);
                roles = actual.Roles.Select(r => r.IdRol).ToList();
            }

            var actualizado = await _servicio.ActualizarAsync(email, body.Contrasena, roles);
            return Ok(actualizado);
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
            return StatusCode(409, new { estado = 409, mensaje = "Uno de los roles no existe.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // DELETE /api/usuario-con-roles/{email}  →  detalle y maestro juntos
    // ------------------------------------------------------------
    [HttpDelete("{email}")]
    public async Task<IActionResult> Eliminar(string email)
    {
        try
        {
            // El JSON del SP ES la respuesta: se emite tal cual.
            var json = await _servicio.EliminarAsync(email);
            return Content(json, "application/json");
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
}
