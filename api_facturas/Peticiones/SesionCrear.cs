// Las credenciales, EN EL CUERPO de la petición.
//
// Y no en la URL, que es donde estaba el endpoint `verificar-contrasena` de la
// versión anterior:
//
//     POST /api/usuario/verificar-contrasena?valor_usuario=…&valor_contrasena=…
//
// Una contraseña en la URL queda en el historial del navegador, en los logs
// del servidor y en los de cualquier proxy del camino. El cuerpo de un POST
// no se registra. Ese endpoint era «el cimiento del login» y se queda como
// está —no se toca lo cerrado— pero el inicio de sesión de verdad usa esto.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class SesionCrear
{
    [Required(ErrorMessage = "El campo email es obligatorio.")]
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "El campo email debe tener entre 1 y 100 caracteres.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El campo contrasena es obligatorio.")]
    [StringLength(200, MinimumLength = 1,
        ErrorMessage = "El campo contrasena debe tener entre 1 y 200 caracteres.")]
    public string? Contrasena { get; set; }
}
