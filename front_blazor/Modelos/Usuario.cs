namespace FrontFacturas.Modelos;

// ============================================================
// Usuario — la clase DEL FRONT.
//
// Se parece a la de la API porque el CONTRATO es el mismo, no porque sea la
// misma clase. Estan las dos en C# y en carpetas vecinas, asi que una
// referencia de proyecto FUNCIONARIA: esta prohibida.
// ============================================================
public class Usuario
{
    public string Email { get; set; } = "";
    public string Contrasena { get; set; } = "";
}
