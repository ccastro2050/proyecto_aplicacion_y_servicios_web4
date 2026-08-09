// UsuarioCrear — la PETICIÓN del POST de usuario: el ÚNICO momento en
// que una contraseña entra por la API (y sale de aquí directo al hash).

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class UsuarioCrear
{
    [Required(ErrorMessage = "El campo email es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El campo email debe tener entre 1 y 100 caracteres.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El campo contrasena es obligatorio.")]
    [StringLength(200, MinimumLength = 6, ErrorMessage = "El campo contrasena debe tener entre 6 y 200 caracteres.")]
    public string? Contrasena { get; set; }
}
