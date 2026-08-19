namespace FrontFacturas.Modelos;

// ============================================================
// Cliente — la primera clase del front CON CLAVE FORÁNEA.
//
// `Fkcodpersona` no es un texto libre: es el código de una fila de `persona`
// que TIENE que existir. Y de ahí sale la regla de la v2 en la interfaz
// gráfica: ese campo NO se escribe a mano, se elige de un desplegable que
// trae los valores de la API.
//
// Si se escribe a mano, la API responde 409 (la clave foránea no existe) y la
// persona no tiene cómo adivinar qué códigos valen.
//
// `Fkcodempresa` es NULLABLE —`string?`— porque un cliente puede ser una
// persona natural, sin empresa. El desplegable, por eso, lleva una opción
// vacía: «(ninguna)».
// ============================================================
public class Cliente
{
    /// <summary>La genera la BD (SERIAL): no se pide al crear.</summary>
    public int Id { get; set; }

    public decimal Credito { get; set; }

    /// <summary>FK → persona.codigo. Obligatoria.</summary>
    public string Fkcodpersona { get; set; } = "";

    /// <summary>FK → empresa.codigo. OPCIONAL: puede quedar en null.</summary>
    public string? Fkcodempresa { get; set; }
}
