// Petición del verbo — la FRONTERA de entrada (el 422 sale solo).
// Calcada del molde de producto/persona.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class RutaReemplazo
{
    [Required(ErrorMessage = "El campo ruta es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El campo ruta debe tener entre 1 y 100 caracteres.")]
    public string? Ruta { get; set; }

    [Required(ErrorMessage = "El campo descripcion es obligatorio.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "El campo descripcion debe tener entre 1 y 200 caracteres.")]
    public string? Descripcion { get; set; }
}
