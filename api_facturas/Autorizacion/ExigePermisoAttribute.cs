// ============================================================
// ExigePermisoAttribute — el 403, y el sitio donde el permiso se hace valer.
//
// Se pone sobre un controlador o sobre una acción:
//
//     [ExigePermiso("interfaz.usuarios")]
//
// Y entonces, ANTES de que el método del controlador llegue a ejecutarse, se
// le pregunta a `verificar_acceso_ruta` si quien pide tiene esa ruta. Si no,
// 403 y el método no corre.
//
// TRES DECISIONES QUE VALE LA PENA LEER DOS VECES:
//
// 1. SE CONSULTA EN CADA PETICIÓN, no al iniciar sesión. Es más trabajo —una
//    consulta por operación— y es lo que hace que quitarle un permiso a un rol
//    SURTA EFECTO SIN que la persona vuelva a identificarse. Si el permiso
//    estuviera en el token, seguiría entrando hasta que el token venciera.
//    Es el criterio 7, y está escrito justamente para forzar esta decisión.
//
// 2. 403, NO 401. El token es válido y se sabe perfectamente quién pregunta:
//    lo que falta es el permiso. 401 significa «no sé quién es usted»; 403,
//    «sé quién es, y no puede».
//
// 3. ES UN FILTRO, no una línea al principio de cada método. Si fuera una
//    línea, el día que alguien escriba un endpoint nuevo y se le olvide, ese
//    endpoint queda abierto — y nadie lo nota, porque funciona. El filtro no
//    se olvida: está en el atributo, a la vista.
// ============================================================

using ApiFacturas.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiFacturas.Autorizacion;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ExigePermisoAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _ruta;

    public ExigePermisoAttribute(string ruta)
    {
        _ruta = ruta;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext contexto)
    {
        // Quién pregunta. Lo puso el middleware de autenticación al validar el
        // token; si no hay token válido, el [Authorize] ya respondió 401 y aquí
        // no se llega. Pero se comprueba igual: un filtro que supone es un
        // filtro que un día falla abierto.
        var email = contexto.HttpContext.User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email))
        {
            contexto.Result = new ObjectResult(new
            {
                estado = 401,
                mensaje = "No hay una sesion valida.",
            })
            { StatusCode = 401 };
            return;
        }

        var acceso = contexto.HttpContext.RequestServices
            .GetRequiredService<IRepositorioAcceso>();

        if (!await acceso.TieneAccesoAsync(email, _ruta))
        {
            // 403: se sabe quién es, y no puede.
            contexto.Result = new ObjectResult(new
            {
                estado = 403,
                mensaje = "Su rol no tiene permiso para esta operacion.",
                ruta = _ruta,
            })
            { StatusCode = 403 };
        }
    }
}
