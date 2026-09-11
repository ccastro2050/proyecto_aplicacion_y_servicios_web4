// ============================================================
// PermisosDeRol — las PETICIONES de /api/permisos.
//
// Dos clases y una sola diferencia, que es la de los dos verbos:
//
//   PUT   `Rutas` es [Required]: la lista que llega ES la que queda.
//   PATCH no manda la lista: manda `Agregar` y `Quitar`.
//
// El PATCH se hace así —y no con una lista parcial— porque sobre un
// conjunto «parcial» no significa nada: una lista con dos rutas no
// dice si las otras se quitan o se dejan. «Agregue estas, quite
// aquellas» sí lo dice.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace ApiFacturas.Peticiones;

/// <summary>El POST: conceder UNA ruta a UN rol.</summary>
public class PermisoCrear
{
    [Required(ErrorMessage = "El campo fkidrol es obligatorio.")]
    public int? Fkidrol { get; set; }

    [Required(ErrorMessage = "El campo fkidruta es obligatorio.")]
    public int? Fkidruta { get; set; }
}

/// <summary>El PUT: la lista que llega reemplaza a la que había.</summary>
public class PermisosReemplazo
{
    /// <summary>Puede venir VACÍA: dejar un rol sin ninguna ruta es una
    /// decisión legítima. Lo que no puede es faltar — eso sería no saber si
    /// se quiso vaciar o si se olvidó.</summary>
    [Required(ErrorMessage = "El campo rutas es obligatorio (puede ser una lista vacía).")]
    public List<int>? Rutas { get; set; }
}

/// <summary>El PATCH: lo que entra y lo que sale, sin tocar el resto.</summary>
public class PermisosParcial
{
    public List<int>? Agregar { get; set; }

    public List<int>? Quitar { get; set; }
}
