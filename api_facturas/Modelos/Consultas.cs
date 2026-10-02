// ============================================================
// Los modelos de las DIEZ CONSULTAS de la v4.
//
// Uno por consulta, y no un diccionario generico: el nombre de cada propiedad
// ES la documentacion de qué devuelve. Con un `Dictionary<string, object>` la
// API responderia lo mismo y nadie sabria qué esperar sin ejecutarla.
//
// Son `record` porque no tienen comportamiento ni identidad: son el resultado
// de una pregunta, no una entidad del dominio. Un `record` lo dice en una
// palabra.
// ============================================================

namespace ApiFacturas.Modelos;

/// <summary>1 — Cuánto se ha vendido de cada producto.
/// Cruza producto, productosporfactura, factura y cliente.</summary>
public record VentaPorProducto(
    string Codigo, string Nombre, int Unidades, decimal Ingreso,
    int Facturas, int Clientes);

/// <summary>2 — Cuánto ha comprado cada cliente.
/// Cruza persona, cliente, factura y productosporfactura.</summary>
public record VentaPorCliente(
    int Id, string Cliente, string Email, int Facturas,
    decimal Comprado, int Unidades);

/// <summary>3 — Cuánto ha vendido cada vendedor.
/// Cruza persona, vendedor, factura y productosporfactura.</summary>
public record VentaPorVendedor(
    int Id, string Vendedor, int Carnet, int Facturas, decimal Vendido);

/// <summary>4 — Cuánto se le ha facturado a cada empresa.
/// Cruza empresa, cliente, factura y productosporfactura.</summary>
public record VentaPorEmpresa(
    string Codigo, string Empresa, int Clientes, int Facturas, decimal Facturado);

/// <summary>5 — El valor promedio de una factura, por vendedor.
/// Cruza persona, vendedor, factura y productosporfactura.</summary>
public record TicketPorVendedor(
    string Vendedor, int Facturas, decimal Total, decimal TicketPromedio);

/// <summary>6 — Los productos que NUNCA se han vendido.
/// Cruza producto, productosporfactura, factura y cliente, con LEFT JOIN.
///
/// Que devuelva CERO FILAS no es un error: es la respuesta. Significa que todo
/// el catálogo se ha vendido alguna vez.</summary>
public record ProductoSinVender(
    string Codigo, string Nombre, int Stock, decimal Valorunitario);

/// <summary>7 — Cuánto se ha anulado, por cliente.
/// Cruza persona, cliente, factura y productosporfactura.
///
/// Se llena en cuanto alguien anula una factura desde la interfaz de la v2 —
/// que es lo bonito: el tablero reacciona a lo que pasa en el sistema.</summary>
public record AnulacionPorCliente(
    string Cliente, int Anuladas, decimal ValorAnulado);

/// <summary>8 — Hasta dónde llega cada usuario.
/// Cruza usuario, rol_usuario, rol, rutarol y ruta: CINCO tablas.</summary>
public record AlcanceDeUsuario(
    string Email, int Roles, int Interfaces, string SusRoles);

/// <summary>9 — Las interfaces a las que NO llega nadie.
/// Cruza ruta, rutarol, rol y rol_usuario, con LEFT JOIN.
///
/// Es una consulta de AUDITORÍA: una interfaz protegida a la que ningún
/// usuario llega, o sobra, o alguien se quedó sin el permiso que necesitaba.</summary>
public record InterfazSinUsuarios(
    int Id, string Ruta, string Descripcion,
    int RolesConAcceso, int UsuariosConAcceso);

/// <summary>10 — El crédito de cada cliente contra lo que lleva consumido.
/// Cruza persona, empresa, cliente y factura.</summary>
public record CreditoContraConsumo(
    string Cliente, string Empresa, decimal Credito,
    decimal Consumido, decimal Disponible);
