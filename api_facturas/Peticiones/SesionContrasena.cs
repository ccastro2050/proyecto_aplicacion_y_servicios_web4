// ============================================================
// SesionContrasena — la PETICIÓN del PATCH de /api/sesion:
// cambiarME la contraseña a mí mismo.
//
// POR QUÉ PIDE LA ACTUAL, aunque quien llama ya traiga un token
// válido: porque un token robado o una sesión abierta en un
// computador ajeno bastarían para dejar al dueño afuera. La
// contraseña actual es la prueba de que quien escribe es la
// persona, no quien encontró la pantalla abierta.
//
// Y fíjese en qué NO lleva esta petición: el email. Ése sale del
// token. Si viniera en el body, cualquiera podría cambiarle la
// contraseña a otro — eso es el PATCH de /api/usuario/{email},
// que es de administrador y exige su permiso.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class SesionContrasena
{
    [Required(ErrorMessage = "El campo contrasenaActual es obligatorio.")]
    public string? ContrasenaActual { get; set; }

    [Required(ErrorMessage = "El campo contrasenaNueva es obligatoria.")]
    [StringLength(200, MinimumLength = 6,
        ErrorMessage = "El campo contrasenaNueva debe tener entre 6 y 200 caracteres.")]
    public string? ContrasenaNueva { get; set; }
}
