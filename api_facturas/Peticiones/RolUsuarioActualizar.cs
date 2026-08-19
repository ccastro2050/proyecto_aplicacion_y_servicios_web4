// ============================================================
// RolUsuarioActualizar — la PETICIÓN del PATCH del puente.
//
// Ningún campo es obligatorio: PATCH manda SOLO el lado que se
// mueve. `{"fkidrol": 3}` le cambia el rol a la misma persona;
// `{"fkemail": "otro@correo.com"}` le pasa ese rol a otra persona.
//
// El body vacío `{}` pasa la validación de forma y lo rechaza el
// servicio con un 400: «no mandó nada que cambiar» no es un error
// de FORMA.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class RolUsuarioActualizar
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El campo fkemail debe tener entre 1 y 100 caracteres.")]
    public string? Fkemail { get; set; }

    public int? Fkidrol { get; set; }
}
