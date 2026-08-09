namespace FrontFacturas.Modelos;

// ============================================================
// Producto — la clase del FRONT.
//
// Se parece a la de la API porque el CONTRATO es el mismo, no porque sea la
// misma clase. Están las dos en C# y en carpetas vecinas, así que una
// referencia de proyecto FUNCIONARÍA: está prohibida.
//
// Si se compartiera, un cambio interno de la API rompería el front sin que
// nadie tocara el contrato — y el contrato es lo único que los une.
// ============================================================
public class Producto
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
    public decimal Valorunitario { get; set; }
}

/// <summary>El sobre de error que devuelve la API. El front lo lee para
/// mostrar el mensaje del dominio, no el código HTTP.</summary>
public class ErrorApi
{
    public int Estado { get; set; }
    public string Mensaje { get; set; } = "";
}
