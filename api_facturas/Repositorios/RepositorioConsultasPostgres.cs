// ============================================================
// RepositorioConsultasPostgres — las DIEZ consultas de la v4.
//
// Aqui NO hay procedimientos almacenados, y es deliberado: una consulta de
// reporte cambia cada vez que alguien quiere verla de otra forma, y tenerla en
// un procedimiento obliga a tocar la base para cambiar un ORDER BY. Los
// procedimientos de este proyecto existen donde hay una REGLA que proteger
// -la facturacion, que es una transaccion-, no donde solo hay un SELECT.
//
// Cada consulta cruza CUATRO O MAS tablas. No es un capricho del enunciado:
// una consulta de una tabla la responde el CRUD que ya existe. Lo que estas
// agregan es la pregunta que NINGUN endpoint anterior podia responder.
//
// EL SQL VA A LA VISTA, como en todo el proyecto (Art. 2): sin ORM. Quien lea
// esto ve exactamente lo que la base va a ejecutar.
//
//
// Y LOS `CAST(... AS INT)` TAMPOCO SON DECORACION, que es el segundo tropiezo
// de esta version: en PostgreSQL, COUNT() y SUM() sobre una columna entera
// devuelven BIGINT -64 bits-, y el modelo pide `int`. Dapper no puede
// construir el record y responde 500 con «A parameterless default constructor
// or one matching signature (... System.Int64 unidades ...)».
//
// No lo caza el compilador: el modelo compila, el SQL es valido, y falla al
// materializar la PRIMERA FILA. Se descubre ejecutandolo.
//
// Se arregla en el SQL y no cambiando el modelo a `long`, por dos razones: el
// contrato de la API se mantiene -`unidades` es un entero, y que la base lo
// devuelva en 64 bits es un detalle del motor que no tiene por que subir- y
// CAST(x AS INT) es SQL estandar, igual en los dos dialectos. El `::int` de
// PostgreSQL no lo es.
//
// Y LOS ALIAS NO SON DECORACION: `p.codigo AS Codigo`. Dapper mapea columna a
// propiedad POR NOMBRE, y sin el alias la propiedad llega en su valor por
// defecto -0, null- EN SILENCIO: la API responde 200 con el dato vacio.//
// LAS DOS CONSULTAS QUE NO SON IGUALES EN LOS DOS MOTORES, y se descubrieron
// ejecutandolas, no leyendo la documentacion:
//
//   AlcanceDeUsuarios   PostgreSQL acepta STRING_AGG(DISTINCT x, ', ').
//                       T-SQL responde «Incorrect syntax near ','»: su
//                       STRING_AGG no admite DISTINCT. Se resuelve con una
//                       subconsulta que ya trae el DISTINCT hecho.
//
//   TicketPorVendedor   En T-SQL, COUNT() devuelve int y dividir un decimal
//                       entre un int TRUNCA. Hace falta el CAST explicito.
//                       PostgreSQL promueve el tipo solo.
//
// Las otras ocho son identicas palabra por palabra. Y eso tambien es un dato:
// el SQL estandar llega mas lejos de lo que se suele creer — lo que cambia son
// las funciones de agregacion y las reglas de tipos.
// ============================================================

using ApiFacturas.Modelos;
using Dapper;
using Npgsql;

namespace ApiFacturas.Repositorios;

public class RepositorioConsultasPostgres : IRepositorioConsultas
{
    private readonly string _cadenaConexion;

    public RepositorioConsultasPostgres(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    private NpgsqlConnection Crear() => new(_cadenaConexion);

    public async Task<List<VentaPorProducto>> VentasPorProductoAsync()
    {
        const string sql = @"SELECT p.codigo AS Codigo, p.nombre AS Nombre,
       CAST(SUM(pf.cantidad) AS INT) AS Unidades, SUM(pf.subtotal) AS Ingreso,
       CAST(COUNT(DISTINCT f.numero) AS INT) AS Facturas, CAST(COUNT(DISTINCT c.id) AS INT) AS Clientes
FROM producto p
INNER JOIN productosporfactura pf ON pf.fkcodproducto = p.codigo
INNER JOIN factura f ON f.numero = pf.fknumfactura
INNER JOIN cliente c ON c.id = f.fkidcliente
WHERE f.estado = 'activa'
GROUP BY p.codigo, p.nombre
ORDER BY SUM(pf.subtotal) DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<VentaPorProducto>(sql)).ToList();
    }

    public async Task<List<VentaPorCliente>> VentasPorClienteAsync()
    {
        const string sql = @"SELECT c.id AS Id, pe.nombre AS Cliente, pe.email AS Email,
       CAST(COUNT(DISTINCT f.numero) AS INT) AS Facturas, SUM(pf.subtotal) AS Comprado,
       CAST(SUM(pf.cantidad) AS INT) AS Unidades
FROM cliente c
INNER JOIN persona pe ON pe.codigo = c.fkcodpersona
INNER JOIN factura f ON f.fkidcliente = c.id
INNER JOIN productosporfactura pf ON pf.fknumfactura = f.numero
WHERE f.estado = 'activa'
GROUP BY c.id, pe.nombre, pe.email
ORDER BY SUM(pf.subtotal) DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<VentaPorCliente>(sql)).ToList();
    }

    public async Task<List<VentaPorVendedor>> VentasPorVendedorAsync()
    {
        const string sql = @"SELECT v.id AS Id, pe.nombre AS Vendedor, v.carnet AS Carnet,
       CAST(COUNT(DISTINCT f.numero) AS INT) AS Facturas, SUM(pf.subtotal) AS Vendido
FROM vendedor v
INNER JOIN persona pe ON pe.codigo = v.fkcodpersona
INNER JOIN factura f ON f.fkidvendedor = v.id
INNER JOIN productosporfactura pf ON pf.fknumfactura = f.numero
WHERE f.estado = 'activa'
GROUP BY v.id, pe.nombre, v.carnet
ORDER BY SUM(pf.subtotal) DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<VentaPorVendedor>(sql)).ToList();
    }

    public async Task<List<VentaPorEmpresa>> VentasPorEmpresaAsync()
    {
        const string sql = @"SELECT e.codigo AS Codigo, e.nombre AS Empresa,
       CAST(COUNT(DISTINCT c.id) AS INT) AS Clientes, CAST(COUNT(DISTINCT f.numero) AS INT) AS Facturas,
       SUM(pf.subtotal) AS Facturado
FROM empresa e
INNER JOIN cliente c ON c.fkcodempresa = e.codigo
INNER JOIN factura f ON f.fkidcliente = c.id
INNER JOIN productosporfactura pf ON pf.fknumfactura = f.numero
WHERE f.estado = 'activa'
GROUP BY e.codigo, e.nombre
ORDER BY SUM(pf.subtotal) DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<VentaPorEmpresa>(sql)).ToList();
    }

    public async Task<List<TicketPorVendedor>> TicketPorVendedorAsync()
    {
        // OJO: esta consulta NO es igual en los dos motores. Ver el
        // comentario de la cabecera.
        const string sql = @"SELECT pe.nombre AS Vendedor,
       CAST(COUNT(DISTINCT f.numero) AS INT) AS Facturas, SUM(pf.subtotal) AS Total,
       ROUND(SUM(pf.subtotal) / NULLIF(COUNT(DISTINCT f.numero), 0), 2) AS TicketPromedio
FROM vendedor v
INNER JOIN persona pe ON pe.codigo = v.fkcodpersona
INNER JOIN factura f ON f.fkidvendedor = v.id
INNER JOIN productosporfactura pf ON pf.fknumfactura = f.numero
WHERE f.estado = 'activa'
GROUP BY pe.nombre
ORDER BY 4 DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<TicketPorVendedor>(sql)).ToList();
    }

    public async Task<List<ProductoSinVender>> ProductosSinVenderAsync()
    {
        const string sql = @"SELECT p.codigo AS Codigo, p.nombre AS Nombre,
       p.stock AS Stock, p.valorunitario AS Valorunitario
FROM producto p
LEFT JOIN productosporfactura pf ON pf.fkcodproducto = p.codigo
LEFT JOIN factura f ON f.numero = pf.fknumfactura AND f.estado = 'activa'
LEFT JOIN cliente c ON c.id = f.fkidcliente
GROUP BY p.codigo, p.nombre, p.stock, p.valorunitario
HAVING COUNT(c.id) = 0
ORDER BY p.valorunitario DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<ProductoSinVender>(sql)).ToList();
    }

    public async Task<List<AnulacionPorCliente>> AnulacionesPorClienteAsync()
    {
        const string sql = @"SELECT pe.nombre AS Cliente,
       CAST(COUNT(DISTINCT f.numero) AS INT) AS Anuladas, SUM(pf.subtotal) AS ValorAnulado
FROM factura f
INNER JOIN cliente c ON c.id = f.fkidcliente
INNER JOIN persona pe ON pe.codigo = c.fkcodpersona
INNER JOIN productosporfactura pf ON pf.fknumfactura = f.numero
WHERE f.estado = 'anulada'
GROUP BY pe.nombre
ORDER BY SUM(pf.subtotal) DESC";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<AnulacionPorCliente>(sql)).ToList();
    }

    public async Task<List<AlcanceDeUsuario>> AlcanceDeUsuariosAsync()
    {
        // OJO: esta consulta NO es igual en los dos motores. Ver el
        // comentario de la cabecera.
        const string sql = @"SELECT u.email AS Email,
       CAST(COUNT(DISTINCT r.id) AS INT) AS Roles, CAST(COUNT(DISTINCT rt.id) AS INT) AS Interfaces,
       STRING_AGG(DISTINCT r.nombre, ', ') AS SusRoles
FROM usuario u
INNER JOIN rol_usuario ru ON ru.fkemail = u.email
INNER JOIN rol r ON r.id = ru.fkidrol
INNER JOIN rutarol rr ON rr.fkidrol = r.id
INNER JOIN ruta rt ON rt.id = rr.fkidruta
GROUP BY u.email
ORDER BY COUNT(DISTINCT rt.id) DESC, u.email";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<AlcanceDeUsuario>(sql)).ToList();
    }

    public async Task<List<InterfazSinUsuarios>> InterfacesSinUsuariosAsync()
    {
        const string sql = @"SELECT rt.id AS Id, rt.ruta AS Ruta, rt.descripcion AS Descripcion,
       CAST(COUNT(DISTINCT r.id) AS INT) AS RolesConAcceso,
       CAST(COUNT(DISTINCT ru.fkemail) AS INT) AS UsuariosConAcceso
FROM ruta rt
LEFT JOIN rutarol rr ON rr.fkidruta = rt.id
LEFT JOIN rol r ON r.id = rr.fkidrol
LEFT JOIN rol_usuario ru ON ru.fkidrol = r.id
GROUP BY rt.id, rt.ruta, rt.descripcion
HAVING COUNT(DISTINCT ru.fkemail) = 0
ORDER BY rt.ruta";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<InterfazSinUsuarios>(sql)).ToList();
    }

    public async Task<List<CreditoContraConsumo>> CreditoContraConsumoAsync()
    {
        const string sql = @"SELECT pe.nombre AS Cliente,
       COALESCE(e.nombre, '(sin empresa)') AS Empresa,
       c.credito AS Credito,
       COALESCE(SUM(f.total), 0) AS Consumido,
       c.credito - COALESCE(SUM(f.total), 0) AS Disponible
FROM cliente c
INNER JOIN persona pe ON pe.codigo = c.fkcodpersona
LEFT JOIN empresa e ON e.codigo = c.fkcodempresa
LEFT JOIN factura f ON f.fkidcliente = c.id AND f.estado = 'activa'
GROUP BY pe.nombre, e.nombre, c.credito
ORDER BY 5";
        await using var conexion = Crear();
        return (await conexion.QueryAsync<CreditoContraConsumo>(sql)).ToList();
    }
}
