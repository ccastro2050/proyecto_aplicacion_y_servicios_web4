// ============================================================
// PersonaCrear — la PETICIÓN del POST de persona.
//
// CALCADA de ProductoCrear (la lección de qué es una petición y
// por qué el 422 sale solo está allí). Persona no tiene campos
// numéricos: aquí no hay [Range] — todo es texto con longitudes.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class PersonaCrear
{
    [Required(ErrorMessage = "El campo codigo es obligatorio.")]
    [StringLength(10, MinimumLength = 1,
        ErrorMessage = "El campo codigo debe tener entre 1 y 10 caracteres.")]
    public string? Codigo { get; set; }

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
