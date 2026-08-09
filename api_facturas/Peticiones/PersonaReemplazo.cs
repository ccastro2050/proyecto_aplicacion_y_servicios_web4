// ============================================================
// PersonaReemplazo — la PETICIÓN del PUT de persona.
//
// PUT = reemplazo COMPLETO: TODOS los campos obligatorios (el
// código va en la URL y no cambia). El mismo contraste con PATCH
// que enseñó producto, ahora en persona.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class PersonaReemplazo
{
    [Required(ErrorMessage = "El campo nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "El campo nombre debe tener entre 1 y 100 caracteres.")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "El campo email es obligatorio.")]
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "El campo email debe tener entre 1 y 100 caracteres.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El campo telefono es obligatorio.")]
    [StringLength(20, MinimumLength = 1,
        ErrorMessage = "El campo telefono debe tener entre 1 y 20 caracteres.")]
    public string? Telefono { get; set; }
}
