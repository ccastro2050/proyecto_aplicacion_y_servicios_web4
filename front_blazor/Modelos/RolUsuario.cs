namespace FrontFacturas.Modelos;

// ============================================================
// RolUsuario — una tabla PUENTE, y por eso no se parece a las otras.
//
// No tiene `Id`: su clave primaria son las DOS columnas juntas (fkemail,
// fkidrol). De ahí salen tres diferencias en la interfaz gráfica:
//
//   1. No hay EDITAR. Una asignación existe o no existe; cambiarla es
//      quitarla y poner otra.
//   2. El borrado necesita LAS DOS claves:
//      DELETE /api/rol-usuario/{email}/{idrol}
//   3. Los dos campos son desplegables. Los dos son FK.
// ============================================================
public class RolUsuario
{
    /// <summary>FK → usuario.email.</summary>
    public string Fkemail { get; set; } = "";

    /// <summary>FK → rol.id.</summary>
    public int Fkidrol { get; set; }
}

// ============================================================
// UsuarioConRoles — el usuario con sus roles YA PEGADOS, como lo devuelve
// `listar_usuarios_con_roles`.
//
// Es el patrón de Factura —un maestro con una lista dentro— aplicado a una
// tabla puente. El agrupamiento lo hace el PROCEDIMIENTO: el front no recorre
// una lista plana juntando por correo, y la API tampoco.
// ============================================================
public class UsuarioConRoles
{
    public string Email { get; set; } = "";

    public List<RolDeUsuario> Roles { get; set; } = [];
}

// ============================================================
// RolDeUsuario — un rol como viene DENTRO del usuario.
//
// NO es la clase `Rol`, y la diferencia es una letra con consecuencias: el
// procedimiento devuelve `idrol`, no `id`. Reusar `Rol` dejaría el
// identificador en 0 —y lo dejaría EN SILENCIO, sin un solo error— así que
// los botones de quitar apuntarían al rol equivocado.
// ============================================================
public class RolDeUsuario
{
    [System.Text.Json.Serialization.JsonPropertyName("idrol")]
    public int IdRol { get; set; }

    public string Nombre { get; set; } = "";
}

/// <summary>Lo que se ENVÍA al crear o editar: el correo, la contraseña y los
/// ids de rol marcados. Nada más — los nombres de los roles no viajan.</summary>
public class UsuarioConRolesEnvio
{
    public string Email { get; set; } = "";

    public string Contrasena { get; set; } = "";

    public List<int> Roles { get; set; } = [];
}
