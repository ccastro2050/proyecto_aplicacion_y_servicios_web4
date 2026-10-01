namespace FrontFacturas.Modelos;

// ============================================================
// Los modelos de las DIEZ CONSULTAS de la v4, en el front.
//
// Se parecen a los de la API porque el CONTRATO es el mismo, no porque sean
// las mismas clases. Una referencia de proyecto FUNCIONARIA —estan las dos en
// C# y en carpetas vecinas— y esta prohibida: si se compartieran, un cambio
// interno de la API rompería el front sin que nadie tocara el contrato.
// ============================================================

public class VentaPorProducto
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Unidades { get; set; }
    public decimal Ingreso { get; set; }
    public int Facturas { get; set; }
    public int Clientes { get; set; }
}

public class VentaPorCliente
{
    public int Id { get; set; }
    public string Cliente { get; set; } = "";
    public string Email { get; set; } = "";
    public int Facturas { get; set; }
    public decimal Comprado { get; set; }
    public int Unidades { get; set; }
}

public class VentaPorVendedor
{
    public int Id { get; set; }
    public string Vendedor { get; set; } = "";
    public int Carnet { get; set; }
    public int Facturas { get; set; }
    public decimal Vendido { get; set; }
}

public class VentaPorEmpresa
{
    public string Codigo { get; set; } = "";
    public string Empresa { get; set; } = "";
    public int Clientes { get; set; }
    public int Facturas { get; set; }
    public decimal Facturado { get; set; }
}

public class TicketPorVendedor
{
    public string Vendedor { get; set; } = "";
    public int Facturas { get; set; }
    public decimal Total { get; set; }
    public decimal TicketPromedio { get; set; }
}

public class ProductoSinVender
{
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public int Stock { get; set; }
    public decimal Valorunitario { get; set; }
}

public class AnulacionPorCliente
{
    public string Cliente { get; set; } = "";
    public int Anuladas { get; set; }
    public decimal ValorAnulado { get; set; }
}

public class AlcanceDeUsuario
{
    public string Email { get; set; } = "";
    public int Roles { get; set; }
    public int Interfaces { get; set; }
    public string SusRoles { get; set; } = "";
}

public class InterfazSinUsuarios
{
    public int Id { get; set; }
    public string Ruta { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int RolesConAcceso { get; set; }
    public int UsuariosConAcceso { get; set; }
}

public class CreditoContraConsumo
{
    public string Cliente { get; set; } = "";
    public string Empresa { get; set; } = "";
    public decimal Credito { get; set; }
    public decimal Consumido { get; set; }
    public decimal Disponible { get; set; }
}
