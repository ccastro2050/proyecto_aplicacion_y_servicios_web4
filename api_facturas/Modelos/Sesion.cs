namespace ApiFacturas.Modelos;

// ============================================================
// Sesion — lo que recibe quien se identifica.
//
// Los roles van aquí para que la INTERFAZ GRÁFICA pueda armar su menú sin
// hacer otra petición. Pero ojo con la conclusión fácil:
//
//   QUE LOS ROLES VIAJEN EN LA RESPUESTA NO SIGNIFICA QUE LOS PERMISOS
//   VIAJEN EN EL TOKEN.
//
// El permiso se consulta CUANDO SE USA, contra la base, en cada operación. Si
// estuviera en el token, quitarle un permiso a un rol no surtiría efecto hasta
// que el token venciera — y eso es el criterio 7 de esta versión.
// ============================================================
public class Sesion
{
    public string Token { get; set; } = "";

    public string Email { get; set; } = "";

    /// <summary>Para que el menú de la interfaz sepa qué mostrar. Comodidad,
    /// no protección: la protección está en la API.</summary>
    public List<string> Roles { get; set; } = [];

    /// <summary>Cuándo vence, en UTC. La interfaz lo usa para avisar antes de
    /// que una operación falle con 401.</summary>
    public DateTime Expira { get; set; }
}
