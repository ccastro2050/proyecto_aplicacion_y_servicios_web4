// UsuarioActualizar — la PETICIÓN del PATCH: contraseña opcional
// (el body {} vacío es 400 y lo decide el servicio, como siempre).

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

public class UsuarioActualizar
{
    [StringLength(200, MinimumLength = 6, ErrorMessage = "El campo contrasena debe tener entre 6 y 200 caracteres.")]
    public string? Contrasena { get; set; }
}
