// ============================================================
// UsuarioConRoles — el MAESTRO-DETALLE de una tabla puente.
//
// Es el mismo patrón de Factura, aplicado a `usuario` + `rol_usuario`: el
// procedimiento almacenado devuelve el usuario con sus roles YA PEGADOS, y la
// API no arma el JOIN.
//
// Por qué no una fila por pareja: porque la interfaz gráfica necesita «Ana
// tiene estos tres roles», no tres renglones que alguien tenga que agrupar.
// Agrupar en C# lo que la BD ya agrupa es tener la consulta en dos sitios.
// ============================================================

using System.Text.Json.Serialization;

namespace ApiFacturas.Modelos;

public class UsuarioConRoles
{
    public string? Email { get; set; }

    /// <summary>EL DETALLE: los roles, anidados dentro del usuario.</summary>
    public List<RolDeUsuario> Roles { get; set; } = new();
}

// ============================================================
// RolDeUsuario — un rol como lo devuelve el SP.
//
// OJO CON LA CLAVE: el SP devuelve `idrol`, no `id`. Reusar la clase `Rol`
// —que tiene `Id`— dejaría el identificador en 0, y lo dejaría EN SILENCIO:
// no hay error, solo un número equivocado. Se mira el json_build_object del
// procedimiento, no se adivina.
// ============================================================
public class RolDeUsuario
{
    [JsonPropertyName("idrol")]
    public int IdRol { get; set; }

    public string? Nombre { get; set; }
}
