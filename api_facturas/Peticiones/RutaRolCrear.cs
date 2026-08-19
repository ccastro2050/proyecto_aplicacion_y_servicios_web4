// RutaRolCrear — la PETICIÓN del POST del puente rutarol. Solo hay
// Crear: una asignación no se edita (se quita y se pone otra).

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class RutaRolCrear
{
    [Required(ErrorMessage = "El campo fkidruta es obligatorio.")]
    public int? Fkidruta { get; set; }

    [Required(ErrorMessage = "El campo fkidrol es obligatorio.")]
    public int? Fkidrol { get; set; }
}
