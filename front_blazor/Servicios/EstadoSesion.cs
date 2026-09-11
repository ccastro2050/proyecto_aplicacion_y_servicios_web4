namespace FrontFacturas.Servicios;

// ============================================================
// EstadoSesion — quien esta identificado, en ESTE circuito.
//
// Blazor Server mantiene una conexion abierta con el navegador —un
// «circuito»— y los servicios `scoped` viven exactamente lo que vive ese
// circuito. Asi que el token se guarda aqui, en memoria del servidor, y
// NUNCA baja al navegador.
//
// POR QUE NO EN localStorage, QUE ES LO QUE SE HACE EN OTROS FRAMEWORKS:
// porque entonces cualquier script que corra en la pagina lo puede leer. En
// Blazor Server no hace falta: el navegador solo recibe HTML y eventos.
//
// LO QUE SE PIERDE, Y HAY QUE DECIRLO: al recargar la pagina con F5 el
// circuito se cae y la sesion se va. Es el costo de no bajar el token al
// navegador, y para este curso es el cambio correcto.
// ============================================================
public class EstadoSesion
{
    public string? Token { get; private set; }

    public string? Email { get; private set; }

    public List<string> Roles { get; private set; } = [];

    /// <summary>Las rutas a las que este usuario puede entrar, para armar el
    /// menu. COMODIDAD, no proteccion: ver el comentario de NavMenu.</summary>
    public List<string> RutasPermitidas { get; private set; } = [];

    public bool Identificado => !string.IsNullOrWhiteSpace(Token);

    /// <summary>Se dispara cuando entra o sale alguien, para que el menu y el
    /// encabezado se vuelvan a dibujar sin que nadie recargue nada.</summary>
    public event Action? Cambio;

    public void Entrar(string token, string email, List<string> roles, List<string> rutas)
    {
        Token = token;
        Email = email;
        Roles = roles;
        RutasPermitidas = rutas;
        Cambio?.Invoke();
    }

    public void Salir()
    {
        Token = null;
        Email = null;
        Roles = [];
        RutasPermitidas = [];
        Cambio?.Invoke();
    }

    /// <summary>¿El menu deberia mostrar esta entrada?
    ///
    /// OJO: esto NO decide si se puede entrar. Lo decide la API. Si alguien
    /// escribe la direccion a mano, la interfaz se abre y la API responde 403 —y
    /// eso es lo correcto: la interfaz no es la que protege.</summary>
    public bool PuedeVer(string ruta) => RutasPermitidas.Contains(ruta);
}
