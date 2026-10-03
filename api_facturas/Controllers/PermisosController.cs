// ============================================================
// PermisosController — «¿a qué puedo entrar yo?» y «¿a qué puede entrar
// un rol?», con los cinco verbos para repartirlos.
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

using ApiFacturas.Autorizacion;
using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
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
    private readonly IServicioRutaRol _rutarol;

    public PermisosController(IRepositorioAcceso acceso, IServicioRutaRol rutarol)
    {
        _acceso = acceso;
        _rutarol = rutarol;
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

    // ==================================================================
    // LA VISTA POR ROL — los cuatro verbos que faltaban
    // ==================================================================
    //
    // Lo de arriba responde «¿a qué puedo entrar YO?». Lo de aquí abajo
    // responde «¿a qué puede entrar un ROL?», y permite cambiarlo.
    //
    // ¿Y no es eso lo mismo que `/api/rutarol`? No, y la diferencia ya existe
    // en este proyecto: `rol-usuario` maneja PAREJAS y `usuario-con-roles`
    // maneja un usuario con TODOS sus roles. Aquí pasa igual — `rutarol` es la
    // pareja, `permisos` es el rol con todas sus rutas. La pantalla de
    // administrar permisos piensa en la segunda, no en la primera.
    //
    // Todo esto sí exige permiso: ver los propios permisos no necesita
    // ninguno, pero repartir los de los demás es trabajo de administrador.

    // ------------------------------------------------------------
    // GET /api/permisos/rol/{idrol}  →  las rutas de ESE rol
    // ------------------------------------------------------------
    [HttpGet("rol/{idrol:int}")]
    [ExigePermiso("interfaz.permisos")]
    public async Task<IActionResult> DeRol(int idrol)
    {
        try
        {
            var parejas = await _rutarol.ObtenerPorRolAsync(idrol);
            return Ok(new
            {
                idrol,
                total = parejas.Count,
                rutas = parejas.Select(p => p.Fkidruta).ToList(),
            });
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

    // ------------------------------------------------------------
    // POST /api/permisos  →  CONCEDER una ruta a un rol
    // ------------------------------------------------------------
    [HttpPost]
    [ExigePermiso("interfaz.permisos")]
    public async Task<IActionResult> Conceder([FromBody] PermisoCrear body)
    {
        try
        {
            await _rutarol.CrearAsync(new RutaRol
            {
                Fkidruta = body.Fkidruta!.Value,
                Fkidrol = body.Fkidrol!.Value,
            });
            return Ok(new { estado = 200, mensaje = "Permiso concedido exitosamente." });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (Exception e)
        {
            // El rol ya tenía esa ruta (PK duplicada) o alguno no existe (FK):
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // PUT /api/permisos/rol/{idrol}  →  REEMPLAZAR todas sus rutas
    // ------------------------------------------------------------
    // La lista que llega ES la que queda: lo que no esté, se revoca. Va en
    // UNA transacción, porque entre borrar las viejas e insertar las nuevas
    // el rol no puede entrar a ninguna parte.
    [HttpPut("rol/{idrol:int}")]
    [ExigePermiso("interfaz.permisos")]
    public async Task<IActionResult> Reemplazar(int idrol, [FromBody] PermisosReemplazo body)
    {
        try
        {
            var cuantas = await _rutarol.ReemplazarDeRolAsync(idrol, body.Rutas!);
            return Ok(new
            {
                estado = 200,
                mensaje = "Permisos reemplazados exitosamente.",
                idrol,
                total = cuantas,
            });
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

    // ------------------------------------------------------------
    // PATCH /api/permisos/rol/{idrol}  →  agregar y/o quitar
    // ------------------------------------------------------------
    // Sobre un CONJUNTO, «parcial» no puede ser una lista a medias: dos
    // rutas no dicen si las otras se quitan o se dejan. Por eso el PATCH
    // manda `agregar` y `quitar`, que sí lo dicen.
    //
    // Se resuelve leyendo lo que hay, aplicando las dos listas y reenviando
    // el conjunto completo — la misma transacción del PUT.
    [HttpPatch("rol/{idrol:int}")]
    [ExigePermiso("interfaz.permisos")]
    public async Task<IActionResult> Actualizar(int idrol, [FromBody] PermisosParcial body)
    {
        try
        {
            if (body.Agregar == null && body.Quitar == null)
            {
                return StatusCode(400, new
                {
                    estado = 400,
                    mensaje = "Parámetros inválidos.",
                    detalle = "No se envió ningún campo para actualizar.",
                });
            }

            var actuales = (await _rutarol.ObtenerPorRolAsync(idrol))
                           .Select(p => p.Fkidruta).ToHashSet();

            foreach (var id in body.Agregar ?? new List<int>()) { actuales.Add(id); }
            foreach (var id in body.Quitar ?? new List<int>()) { actuales.Remove(id); }

            var cuantas = await _rutarol.ReemplazarDeRolAsync(idrol, actuales.ToList());
            return Ok(new
            {
                estado = 200,
                mensaje = "Permisos actualizados exitosamente.",
                idrol,
                total = cuantas,
            });
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

    // ------------------------------------------------------------
    // DELETE /api/permisos/rol/{idrol}/ruta/{idruta}  →  REVOCAR una
    // ------------------------------------------------------------
    // Las dos llaves van en la URL porque las dos identifican el permiso:
    // revocar «la ruta 5» sin decir de qué rol no significa nada.
    [HttpDelete("rol/{idrol:int}/ruta/{idruta:int}")]
    [ExigePermiso("interfaz.permisos")]
    public async Task<IActionResult> Revocar(int idrol, int idruta)
    {
        try
        {
            var filas = await _rutarol.EliminarAsync(idruta, idrol);
            return Ok(new
            {
                estado = 200,
                mensaje = "Permiso revocado exitosamente.",
                filasEliminadas = filas,
            });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Permiso no encontrado.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
