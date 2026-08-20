// Petición del verbo — la FRONTERA de entrada (el 422 sale solo).
// Calcada del molde de producto/persona.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class VendedorReemplazo
{
    [Required(ErrorMessage = "El campo carnet es obligatorio.")]
    [Range(0, int.MaxValue, ErrorMessage = "El campo carnet debe ser un entero mayor o igual a 0.")]
    public int? Carnet { get; set; }

    [Required(ErrorMessage = "El campo direccion es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El campo direccion debe tener entre 1 y 100 caracteres.")]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = "El campo fkcodpersona es obligatorio.")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "El campo fkcodpersona debe tener entre 1 y 10 caracteres.")]
    public string? Fkcodpersona { get; set; }
}
