// Petición del verbo — la FRONTERA de entrada (el 422 sale solo).
// Calcada del molde de producto/persona.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class EmpresaActualizar
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El campo nombre debe tener entre 1 y 100 caracteres.")]
    public string? Nombre { get; set; }
}
