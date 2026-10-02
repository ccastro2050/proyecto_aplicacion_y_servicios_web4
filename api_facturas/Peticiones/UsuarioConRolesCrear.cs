// Lo que la API ACEPTA al crear un usuario con sus roles. Es una LISTA
// BLANCA: lo que no esté aquí, no entra.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class UsuarioConRolesCrear
{
    [Required(ErrorMessage = "El campo email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo email debe ser un correo válido.")]
    [StringLength(100, MinimumLength = 1,
        ErrorMessage = "El campo email debe tener entre 1 y 100 caracteres.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "El campo contrasena es obligatorio.")]
    [StringLength(200, MinimumLength = 1,
        ErrorMessage = "El campo contrasena debe tener entre 1 y 200 caracteres.")]
    public string? Contrasena { get; set; }

    /// <summary>Los ids de rol. Mínimo uno: un usuario sin ningún rol no puede
    /// hacer nada, así que crearlo así es crear basura.</summary>
    [Required(ErrorMessage = "El campo roles es obligatorio.")]
    [MinLength(1, ErrorMessage = "El usuario requiere mínimo 1 rol.")]
    public List<int>? Roles { get; set; }
}
