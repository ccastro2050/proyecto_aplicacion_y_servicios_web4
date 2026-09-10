// ============================================================
// ConfiguracionJwt — los cuatro valores con los que se firma y se valida.
//
// Salen de appsettings.json, y en Docker los sobreescribe el compose con
// variables de entorno (Jwt__Key, Jwt__Issuer…). Tenerlos en una clase y no
// esparcidos como strings es lo que permite que el día que cambien, cambien en
// un sitio.
// ============================================================

namespace ApiFacturas.Modelos;

public class ConfiguracionJwt
{
    /// <summary>La clave con la que se FIRMA. Tiene que ser larga —HMAC-SHA256
    /// pide al menos 32 bytes— y no se comparte.
    ///
    /// Y LO QUE HAY QUE TENER CLARO: esta clave NO cifra el token, lo FIRMA.
    /// El contenido del token se lee sin ninguna clave: es base64, no un
    /// secreto. Lo que la firma garantiza es que nadie lo alteró.</summary>
    public string Key { get; set; } = "";

    /// <summary>Quién emitió el token. Se valida al recibirlo: un token de otra
    /// API, aunque esté bien firmado con otra clave, no entra aquí.</summary>
    public string Issuer { get; set; } = "";

    /// <summary>Para quién es el token.</summary>
    public string Audience { get; set; } = "";

    /// <summary>Cuánto dura. Un token no se puede revocar —está firmado y ya
    /// salió— así que lo único que lo apaga es que venza. De ahí que la
    /// duración sea corta: es el costo de no poder revocarlo.</summary>
    public int DuracionMinutos { get; set; } = 60;
}
