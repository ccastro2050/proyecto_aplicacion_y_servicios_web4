// ============================================================
// PersonaActualizar — la PETICIÓN del PATCH de persona.
//
// PATCH = parcial: NINGÚN campo es obligatorio; el que llegue se
// valida. El body {} vacío no es 422 sino 400 (regla de negocio,
// la decide el servicio) — igual que en producto.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class PersonaActualizar
{
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "El campo nombre debe tener entre 1 y 100 caracteres.")]
    public string? Nombre { get; set; }

    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "El campo email debe tener entre 1 y 100 caracteres.")]
    public string? Email { get; set; }

    [StringLength(20, MinimumLength = 1,
        ErrorMessage = "El campo telefono debe tener entre 1 y 20 caracteres.")]
    public string? Telefono { get; set; }
}
