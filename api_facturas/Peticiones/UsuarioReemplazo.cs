// UsuarioReemplazo — la PETICIÓN del PUT: reemplazo completo = la
// contraseña OBLIGATORIA (el email va en la URL y no cambia).

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class UsuarioReemplazo
{
    [Required(ErrorMessage = "El campo contrasena es obligatorio.")]
    [StringLength(200, MinimumLength = 6, ErrorMessage = "El campo contrasena debe tener entre 6 y 200 caracteres.")]
    public string? Contrasena { get; set; }
}
