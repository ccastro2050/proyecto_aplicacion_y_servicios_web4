// ============================================================
// SesionController — los CINCO verbos de la sesión.
//
// El POST es el único al que se entra SIN token.
//
// Y tiene que ser así, porque no puede exigir lo que todavía no existe. Junto
// con el diagnóstico `/`, son los dos [AllowAnonymous] de toda la API.
//
// LA RESPUESTA AL FALLAR ES LA MISMA EN LOS DOS CASOS —correo inexistente y
// contraseña equivocada— y es el criterio 2. Un 404 para el primero le
// confirmaría a un desconocido qué correos sí existen.
// ============================================================

using System.Security.Claims;
using ApiFacturas.Peticiones;
using ApiFacturas.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/sesion")]
// La clase EXIGE token. El unico abierto es el POST de entrar, que lleva
// su propio [AllowAnonymous]: no puede exigir lo que todavia no existe.
[Authorize]
public class SesionController : ControllerBase
{
    private readonly IServicioSesion _servicio;
    private readonly IServicioUsuario _usuarios;

    public SesionController(IServicioSesion servicio, IServicioUsuario usuarios)
    {
        _servicio = servicio;
        _usuarios = usuarios;
    }

    /// <summary>El correo de QUIEN LLAMA, sacado del token — nunca del body ni
    /// de la URL. Es lo que hace que estos endpoints hablen siempre de uno
    /// mismo y no se puedan usar para tocarle la sesion a otro.</summary>
    private string? MiCorreo() => User.Identity?.Name;
    // ==================================================================
    // [HttpGet]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v3 (§A) dice: «POST /api/sesion — EL ÚNICO
    // ENDPOINT NUEVO». La interfaz sabe quién es por el token que guardó
    // al entrar; no necesita volver a preguntarlo.
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
    // // GET /api/sesion  →  ¿quién soy?
    // // ------------------------------------------------------------
    // // No va a la base: LEE EL TOKEN que vino en la petición. Si llegó hasta
    // // aquí es que estaba firmado por esta API y no había vencido — de eso se
    // // encarga [Authorize] antes de entrar al método.
    // //
    // // Sirve para lo que toda interfaz necesita al cargar: saber si la sesión
    // // sigue viva, y con qué nombre y qué roles pintar el menú.
    // [HttpGet]
    // public IActionResult Quien()
    // {
    // var expiracion = User.FindFirst("exp")?.Value;
    //
    // return Ok(new
    // {
    // estado = 200,
    // mensaje = "Sesion valida.",
    // email = MiCorreo(),
    // // Los roles salen del token, no de una consulta: así es como los
    // // lee [Authorize(Roles = "...")].
    // roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
    // // `exp` viene en segundos desde 1970 (hora UNIX), que es como el
    // // estándar de JWT la guarda:
    // expira = expiracion == null
    // ? (DateTime?)null
    // : DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiracion)).UtcDateTime,
    // });
    // }

    // ------------------------------------------------------------
    // POST /api/sesion  →  el token, si las credenciales sirven
    // ------------------------------------------------------------
    [HttpPost]
    [AllowAnonymous]
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
    // ==================================================================
    // [HttpPut]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v3 (§A): «POST /api/sesion — el único endpoint
    // nuevo». Renovar el token es una decisión de otra versión.
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
    // // PUT /api/sesion  →  RENOVAR el token
    // // ------------------------------------------------------------
    // // Reemplaza el token actual por uno nuevo, y por eso es PUT: lo que se
    // // reemplaza es la sesión entera.
    // //
    // // NO PIDE CONTRASEÑA, y es correcto: quien llama ya trajo un token válido,
    // // y validarlo es comprobar que en su momento dio la contraseña. Pedirla
    // // otra vez sería no creerle al token que la API misma firmó.
    // //
    // // Lo que sí hace es VOLVER A LEER LOS ROLES. Si a alguien le quitaron uno,
    // // el token nuevo sale sin él — renovar no es solo correr la fecha.
    // [HttpPut]
    // public async Task<IActionResult> Renovar()
    // {
    // try
    // {
    // var sesion = await _servicio.RenovarAsync(MiCorreo() ?? "");
    // if (sesion is null)
    // {
    // // El token era válido pero el usuario ya no está: lo borraron
    // // mientras su sesión seguía abierta.
    // return StatusCode(401, new
    // {
    // estado = 401,
    // mensaje = "La sesion ya no es valida. Inicie sesion.",
    // });
    // }
    //
    // return Ok(new
    // {
    // estado = 200,
    // mensaje = "Sesion renovada.",
    // token = sesion.Token,
    // email = sesion.Email,
    // roles = sesion.Roles,
    // expira = sesion.Expira,
    // });
    // }
    // catch (Exception e)
    // {
    // return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
    // }
    // }
    // ==================================================================
    // [HttpPatch]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v3 (§A): «POST /api/sesion — el único endpoint
    // nuevo». Cambiar la contraseña propia existe por otro camino:
    // `PATCH /api/usuario/{email}`, que sí está en el contrato de la v1.
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
    // // PATCH /api/sesion  →  cambiarME la contraseña
    // // ------------------------------------------------------------
    // // Es PATCH porque de la sesión se cambia UNA cosa, y va sobre
    // // /api/sesion —no sobre /api/usuario/{email}— porque habla de UNO MISMO:
    // // el correo sale del token, no del body.
    // //
    // // Y pide la contraseña ACTUAL aunque ya haya token: un computador ajeno
    // // con la sesión abierta no debería bastar para dejar al dueño afuera.
    // [HttpPatch]
    // public async Task<IActionResult> CambiarContrasena([FromBody] SesionContrasena body)
    // {
    // try
    // {
    // var email = MiCorreo() ?? "";
    //
    // // La actual tiene que coincidir. Se usa el MISMO método con el que
    // // se entra, así que la comparación vuelve a ser contra el hash.
    // var sesion = await _servicio.IniciarAsync(email, body.ContrasenaActual!);
    // if (sesion is null)
    // {
    // return StatusCode(401, new
    // {
    // estado = 401,
    // mensaje = "La contrasena actual no es correcta.",
    // });
    // }
    //
    // await _usuarios.ActualizarContrasenaAsync(email, body.ContrasenaNueva);
    //
    // // El token viejo SIGUE SIRVIENDO hasta que venza: cambiar la
    // // contraseña no lo invalida, por lo mismo que explica el DELETE de
    // // abajo. Quien quiera cortar las otras sesiones tiene que renovar y
    // // que la interfaz bote el token viejo.
    // return Ok(new
    // {
    // estado = 200,
    // mensaje = "Contrasena actualizada.",
    // email,
    // });
    // }
    // catch (ArgumentException e)
    // {
    // return StatusCode(400, new { estado = 400, mensaje = "Parámetros inválidos.", detalle = e.Message });
    // }
    // catch (Exception e)
    // {
    // return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
    // }
    // }
    // ==================================================================
    // [HttpDelete]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v3 (§A): «POST /api/sesion — el único endpoint
    // nuevo». Y hay una razón de fondo, escrita abajo: un JWT no se puede
    // apagar desde el servidor, así que salir es botar el token en el
    // cliente — que es lo que hace la interfaz.
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
    // // DELETE /api/sesion  →  salir
    // // ------------------------------------------------------------
    // // ESTE ENDPOINT ENSEÑA ALGO INCÓMODO Y CIERTO SOBRE JWT, y por eso vale
    // // más leer este comentario que el código:
    // //
    // // Un token JWT **no se puede apagar desde el servidor**. No hay una tabla
    // // de sesiones que borrar: el token se valida con la firma y la fecha que
    // // trae adentro, y mientras no venza sigue sirviendo aunque la API diga que
    // // la sesión se cerró.
    // //
    // // ¿Entonces para qué existe? Para que el cliente tenga a quién avisarle, y
    // // para dejar escrito qué SÍ cerraría una sesión de verdad:
    // //
    // //   · que la interfaz bote el token — es lo que pasa aquí;
    // //   · una lista negra de tokens revocados, que obliga a consultar la base
    // //     en cada petición y le quita a JWT su única ventaja;
    // //   · o tokens muy cortos con renovación, que es el PUT de arriba.
    // //
    // // Lo que NO se puede es fingir que el servidor lo apagó.
    // [HttpDelete]
    // public IActionResult Salir()
    // {
    // return Ok(new
    // {
    // estado = 200,
    // mensaje = "Sesion cerrada. Descarte el token: seguira siendo valido hasta que venza.",
    // email = MiCorreo(),
    // });
    // }
}
