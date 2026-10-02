// ============================================================
// PersonaController — la capa HTTP de persona.
//
// CALCADO de ProductoController (allí está explicado dónde vive
// el GET, el 422 automático y la traducción de excepciones).
// Mismos 6 métodos, misma tabla de códigos:
//   Body con errores de forma → 422 (Program.cs)
//   ArgumentException         → 400
//   NoEncontradoExcepcion     → 404
//   SqlException y demás      → 500  ← aquí cae la llave foránea al
//                                      eliminar una persona que es
//                                      cliente o vendedor (¡pruébelo!)
// ============================================================

using ApiFacturas.Excepciones;
using ApiFacturas.Modelos;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using ApiFacturas.Autorizacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/persona")]
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
[ExigePermiso("interfaz.personas")]
public class PersonaController : ControllerBase
{
    private readonly IServicioPersona _servicio;

    public PersonaController(IServicioPersona servicio)
    {
        _servicio = servicio;
    }

    // ------------------------------------------------------------
    // GET /api/persona[?limite=N]  →  listar
    // ------------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int limite = 1000)
    {
        try
        {
            var personas = await _servicio.ListarAsync(limite);

            if (personas.Count == 0)
            {
                return NoContent();   // 204: tabla vacía no es un error
            }
            return Ok(new
            {
                tabla = "persona",
                limite,
                total = personas.Count,
                datos = personas,
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
    // GET /api/persona/{codigo}  →  obtener una
    // ------------------------------------------------------------
    [HttpGet("{codigo}")]
    public async Task<IActionResult> Obtener(string codigo)
    {
        try
        {
            var persona = await _servicio.ObtenerAsync(codigo);
            return Ok(persona);
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Persona no encontrada.", detalle = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // POST /api/persona  →  crear (body completo, con código)
    // ------------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] PersonaCrear body)
    {
        try
        {
            var persona = new Persona
            {
                Codigo = body.Codigo!,
                Nombre = body.Nombre!,
                Email = body.Email!,
                Telefono = body.Telefono!,
            };

            await _servicio.CrearAsync(persona);
            return Ok(new { estado = 200, mensaje = "Persona creada exitosamente." });
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
            // Ej.: código duplicado — la BD rechaza por llave primaria:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // PUT /api/persona/{codigo}  →  reemplazo COMPLETO
    // ------------------------------------------------------------
    [HttpPut("{codigo}")]
    public async Task<IActionResult> Reemplazar(string codigo, [FromBody] PersonaReemplazo body)
    {
        try
        {
            var datos = new Dictionary<string, object>
            {
                ["nombre"] = body.Nombre!,
                ["email"] = body.Email!,
                ["telefono"] = body.Telefono!,
            };

            var filas = await _servicio.ActualizarAsync(codigo, datos);
            return Ok(new { estado = 200, mensaje = "Persona reemplazada exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Persona no encontrada.", detalle = e.Message });
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

    // ------------------------------------------------------------
    // PATCH /api/persona/{codigo}  →  actualización PARCIAL
    // ------------------------------------------------------------
    [HttpPatch("{codigo}")]
    public async Task<IActionResult> Actualizar(string codigo, [FromBody] PersonaActualizar body)
    {
        try
        {
            // La lista blanca: solo estas 3 columnas pueden viajar al SQL.
            var datos = new Dictionary<string, object>();
            if (body.Nombre != null) { datos["nombre"] = body.Nombre; }
            if (body.Email != null) { datos["email"] = body.Email; }
            if (body.Telefono != null) { datos["telefono"] = body.Telefono; }

            var filas = await _servicio.ActualizarAsync(codigo, datos);
            return Ok(new { estado = 200, mensaje = "Persona actualizada exitosamente.", filasAfectadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Persona no encontrada.", detalle = e.Message });
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

    // ------------------------------------------------------------
    // DELETE /api/persona/{codigo}  →  eliminar
    // ------------------------------------------------------------
    [HttpDelete("{codigo}")]
    public async Task<IActionResult> Eliminar(string codigo)
    {
        try
        {
            var filas = await _servicio.EliminarAsync(codigo);
            return Ok(new { estado = 200, mensaje = "Persona eliminada exitosamente.", filasEliminadas = filas });
        }
        catch (ArgumentException e)
        {
            return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
        }
        catch (NoEncontradoExcepcion e)
        {
            return StatusCode(404, new { estado = 404, mensaje = "Persona no encontrada.", detalle = e.Message });
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
            // Aquí cae la LLAVE FORÁNEA: eliminar P001 (que es cliente)
            // hace que la BD rechace el DELETE — integridad referencial:
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
