namespace FrontFacturas.Modelos;

/// <summary>Lo que devuelve POST /api/sesion.</summary>
public class RespuestaSesion
{
    public string Token { get; set; } = "";

    public string Email { get; set; } = "";

    public List<string> Roles { get; set; } = [];

    public DateTime Expira { get; set; }
}
