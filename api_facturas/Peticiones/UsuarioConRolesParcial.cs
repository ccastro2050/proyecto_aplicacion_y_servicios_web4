// ============================================================
// UsuarioConRolesParcial — la PETICIÓN del PATCH.
//
// La diferencia con UsuarioConRolesActualizar —la del PUT— es una
// sola y es la que define los dos verbos:
//
//   PUT   `roles` es [Required]: la lista que llega es la que queda.
//   PATCH `roles` es opcional: si no llega, los roles NO SE TOCAN.
//
// Así, `{"contrasena": "nueva123"}` le cambia la clave a alguien
// sin rozarle los roles — que con el PUT sería imposible sin
// reenviar la lista entera y arriesgarse a equivocarla.
//
// El body vacío `{}` pasa la validación de forma y lo rechaza el
// servicio con un 400: «no mandó nada que cambiar» no es un error
// de FORMA.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class UsuarioConRolesParcial
{
    /// <summary>Si no llega o llega vacía, la contraseña no se cambia —
    /// igual que en el PUT.</summary>
    [StringLength(200, ErrorMessage = "El campo contrasena admite hasta 200 caracteres.")]
    public string? Contrasena { get; set; }

    /// <summary>Si NO llega, los roles se quedan como están. Si llega, tiene
    /// que traer al menos uno: un usuario sin rol no puede entrar a nada.</summary>
    [MinLength(1, ErrorMessage = "El usuario requiere mínimo 1 rol.")]
    public List<int>? Roles { get; set; }
}
