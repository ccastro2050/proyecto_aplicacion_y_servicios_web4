namespace FrontFacturas.Modelos;

// ============================================================
// RutaRol — la segunda tabla puente: qué ROL entra a qué RUTA.
//
// Mismo trato que RolUsuario: PK compuesta, sin editar, borrado con las dos
// claves.
//
// OJO CON LO QUE ESTA TABLA ES Y NO ES EN LA v2:
//
//   En la v2 se ADMINISTRA: se listan las parejas, se agregan y se quitan.
//   En la v3 se HACE VALER: el sistema le pregunta a `verificar_acceso_ruta`
//   si el rol de quien pide tiene esa ruta, y responde 403 si no.
//
// Administrar la tabla de permisos y aplicar los permisos son dos cosas
// distintas, y están en dos versiones distintas. Tener esta interfaz gráfica
// NO significa que el sistema ya controle el acceso: hoy cualquiera entra.
// ============================================================
public class RutaRol
{
    /// <summary>FK → ruta.id.</summary>
    public int Fkidruta { get; set; }

    /// <summary>FK → rol.id.</summary>
    public int Fkidrol { get; set; }
}
