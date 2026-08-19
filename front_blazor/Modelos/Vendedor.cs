namespace FrontFacturas.Modelos;

// ============================================================
// Vendedor — misma lección que Cliente, con una FK sola.
//
// `Fkcodpersona` es obligatoria: un vendedor SIEMPRE es una persona. Así que
// aquí el desplegable NO lleva opción vacía.
// ============================================================
public class Vendedor
{
    /// <summary>La genera la BD (SERIAL).</summary>
    public int Id { get; set; }

    public int Carnet { get; set; }

    public string Direccion { get; set; } = "";

    /// <summary>FK → persona.codigo. Obligatoria.</summary>
    public string Fkcodpersona { get; set; } = "";
}
