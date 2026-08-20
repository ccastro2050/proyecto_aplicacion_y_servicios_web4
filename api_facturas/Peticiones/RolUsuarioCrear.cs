// RolUsuarioCrear — la PETICIÓN del POST del puente rol_usuario. Solo hay
// Crear: una asignación no se edita (se quita y se pone otra).

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class RolUsuarioCrear
{
    [Required(ErrorMessage = "El campo fkemail es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El campo fkemail debe tener entre 1 y 100 caracteres.")]
    public string? Fkemail { get; set; }

    [Required(ErrorMessage = "El campo fkidrol es obligatorio.")]
    public int? Fkidrol { get; set; }
}
