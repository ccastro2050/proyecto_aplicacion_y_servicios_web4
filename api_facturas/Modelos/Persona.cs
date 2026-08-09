// ============================================================
// Persona — el MODELO de la primera rebanada nueva de la v2.
//
// CALCADO de Modelos/Producto.cs (la lección completa de qué es
// un modelo entidad está allí). La v2 demuestra que el molde de
// la v1 se replica sin diseñar nada nuevo: cambian la entidad y
// sus propiedades, nada más.
// ============================================================

namespace ApiFacturas.Modelos;

public class Persona
{
    /// <summary>Identificador único, ej. "P001" (la llave primaria).</summary>
    public required string Codigo { get; set; }

    /// <summary>Nombre completo, ej. "Ana Torres".</summary>
    public required string Nombre { get; set; }

    /// <summary>Correo electrónico.</summary>
    public required string Email { get; set; }

    /// <summary>Teléfono (texto: puede empezar por 0 o llevar +57).</summary>
    public required string Telefono { get; set; }
}
