// Petición del verbo — la FRONTERA de entrada (el 422 sale solo).
// Calcada del molde de producto/persona.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class ClienteReemplazo
{
    [Range(0, double.MaxValue, ErrorMessage = "El campo credito debe ser un número mayor o igual a 0.")]
    public decimal? Credito { get; set; }

    [Required(ErrorMessage = "El campo fkcodpersona es obligatorio.")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "El campo fkcodpersona debe tener entre 1 y 10 caracteres.")]
    public string? Fkcodpersona { get; set; }

    [StringLength(10, MinimumLength = 1, ErrorMessage = "El campo fkcodempresa debe tener entre 1 y 10 caracteres.")]
    public string? Fkcodempresa { get; set; }
}
