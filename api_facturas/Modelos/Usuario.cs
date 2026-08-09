// ============================================================
// Usuario — el MODELO de la entidad con SECRETO (v3).
//
// Fíjese en lo que NO tiene: propiedad de contraseña. Regla RNF3
// de la v3: lo que no está en el modelo de lectura NO PUEDE
// filtrarse a una respuesta HTTP — ni la contraseña ni su hash
// viajan jamás. El secreto vive solo en la BD (hasheado) y lo
// maneja el repositorio.
// ============================================================

namespace ApiFacturas.Modelos;

public class Usuario
{
    /// <summary>El correo (la llave primaria) — lo ÚNICO que se lee.</summary>
    public required string Email { get; set; }
}
