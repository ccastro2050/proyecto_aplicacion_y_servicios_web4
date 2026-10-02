namespace FrontFacturas.Servicios;

// ============================================================
// MenuApp — QUE VERSIÓN es esta y QUÉ INTERFACES tiene.
//
// Existe para que el menú y el pie de página sean **el mismo archivo** en las
// cuatro versiones. Antes el número de versión y las entradas del menú estaban
// escritos a mano en el layout, y eso ya había fallado: el Inicio decía
// «Versión 1 del proyecto» en los cuatro proyectos, porque era un texto en un
// archivo que nadie volvía a mirar.
//
// Lo que cambia entre versiones son DATOS, y los datos van en el `Program.cs`
// —que es version-específico por naturaleza—, no en el marcado.
// ============================================================
public class MenuApp
{
    /// <summary>1, 2, 3 o 4. Lo muestra el pie de página.</summary>
    public required int Version { get; init; }

    /// <summary>Las interfaces que ESTA versión tiene. El menú las recorre en
    /// orden, y no sabe cuántas son.</summary>
    public required List<EntradaMenu> Entradas { get; init; }
}

// ============================================================
// EntradaMenu — una entrada del menú.
//
// `Permiso` solo lo usa la v3 en adelante, y conviene decir por qué está desde
// la v1: el dato de qué permiso protege cada interfaz **ya existe** en la tabla
// `ruta` de la base desde el primer día. Tenerlo aquí no es anticipar el
// control de acceso —nadie lo consulta hasta la v3— es nombrar algo que ya
// estaba.
// ============================================================
public class EntradaMenu
{
    /// <summary>La dirección, sin barra inicial: `productos`.</summary>
    public required string Ruta { get; init; }

    /// <summary>Lo que lee la persona: «Productos». El menú nombra RECURSOS del
    /// dominio, no tablas ni rutas de la API.</summary>
    public required string Texto { get; init; }

    /// <summary>El valor de la tabla `ruta` que protege esta interfaz:
    /// `interfaz.productos`. Lo usa la v3 para decidir si la muestra.</summary>
    public required string Permiso { get; init; }
}
