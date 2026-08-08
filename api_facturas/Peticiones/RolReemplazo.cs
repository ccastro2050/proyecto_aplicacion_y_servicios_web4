// Petición del verbo — la FRONTERA de entrada (el 422 sale solo).
// Calcada del molde de producto/persona.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class RolReemplazo
{
    [Required(ErrorMessage = "El campo nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "El campo nombre debe tener entre 1 y 50 caracteres.")]
    public string? Nombre { get; set; }
}
