// Lo que la API acepta al editar. La diferencia con Crear está en la
// contraseña, y es deliberada.

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class UsuarioConRolesActualizar
{
    /// <summary>OPCIONAL, y es la regla que hay que leer dos veces: si llega
    /// VACÍA o no llega, la contraseña NO se cambia. Vacío significa «déjela
    /// como está», no «bórrela».
    ///
    /// Sin esto, editar los roles de alguien le borraría la contraseña.</summary>
    [StringLength(200, ErrorMessage = "El campo contrasena admite hasta 200 caracteres.")]
    public string? Contrasena { get; set; }

    /// <summary>Los roles REEMPLAZAN los que había: no se suman. Mandar una
    /// lista con uno deja uno.</summary>
    [Required(ErrorMessage = "El campo roles es obligatorio.")]
    [MinLength(1, ErrorMessage = "El usuario requiere mínimo 1 rol.")]
    public List<int>? Roles { get; set; }
}
