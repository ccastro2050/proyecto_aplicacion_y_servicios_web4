// ============================================================
// ConsultasController — las DIEZ consultas multitabla de la v4.
//
// UN ENDPOINT CON NOMBRE POR CONSULTA, no un `/api/consultas/{nombre}`
// generico. Es la misma razon por la que la API tiene `/api/producto` y no
// `/api/{tabla}`:
//
//   * Swagger muestra las diez con su nombre y su forma. Con la ruta generica
//     muestra un hueco, y este proyecto se sustenta proyectando Swagger: si no
//     dice que hay, no hay que sustentar.
//   * El dia que una consulta necesite un parametro -un rango de fechas- se le
//     agrega a ESA, sin tocar las otras nueve.
//   * Un nombre mal escrito da 404 en vez de un 500 raro.
//
// TODAS EXIGEN TOKEN Y PERMISO. Un tablero de ventas es informacion del
// negocio: quien no puede ver las facturas tampoco puede ver cuanto se vendio.
// Van bajo `interfaz.inicio`, que es el permiso del tablero.
// ============================================================

using ApiFacturas.Autorizacion;
using ApiFacturas.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFacturas.Controllers;

[ApiController]
[Route("api/consultas")]
[Authorize]
[ExigePermiso("interfaz.inicio")]
public class ConsultasController : ControllerBase
{
    private readonly IServicioConsultas _servicio;

    public ConsultasController(IServicioConsultas servicio)
    {
        _servicio = servicio;
    }
    // ==================================================================
    // [HttpGet]  —  APAGADO POR ASUNTOS DIDÁCTICOS
    // ==================================================================
    //
    // ESTE MÉTODO NO VA EN LA API, Y ESTÁ AQUÍ A PROPÓSITO: es material de
    // clase, no código muerto que a alguien se le olvidó borrar.
    //
    // POR QUÉ NO VA:
    // El contrato de la v4 dice: «LOS 10 ENDPOINTS de /api/consultas».
    // El catálogo sería el once. El tablero sabe cuáles pedir porque están
    // en su propio código, igual que esta lista.
    //
    // POR QUÉ ENTONCES ESTÁ ESCRITO: porque hay dos cosas que aprender y las
    // dos importan. Una es cómo SE PROGRAMA este verbo —léalo abajo, está
    // completo y comentado—. La otra es que una API NO lleva todos los verbos
    // en todos los recursos: el contrato manda, y el contrato dice que éste no.
    // Borrarlo enseñaría solo la segunda; dejarlo apagado enseña las dos.
    //
    // Para encenderlo basta con quitar las barras de estas líneas: lo que hay
    // debajo en el servicio y en el repositorio SÍ está activo.
    //
    // // ------------------------------------------------------------
    // // GET /api/consultas  →  EL CATÁLOGO: cuáles hay
    // // ------------------------------------------------------------
    // // Sin esto, para armar el tablero había que saberse las diez de memoria o
    // // leer este archivo. Ahora la interfaz pregunta y pinta lo que le digan.
    // //
    // // La lista está escrita aquí y no en la base **a propósito**: estas diez
    // // consultas son código —cada una tiene su SQL y su endpoint—, no datos que
    // // alguien pueda agregar. Una lista en la base prometería que se puede
    // // crear una consulta nueva guardando una fila, y no es así.
    // [HttpGet]
    // public IActionResult Catalogo()
    // {
    // var consultas = new[]
    // {
    // new { nombre = "ventas-por-producto",     titulo = "Ventas por producto",          descripcion = "Cuánto se vendió de cada producto, en unidades y en dinero." },
    // new { nombre = "ventas-por-cliente",      titulo = "Ventas por cliente",           descripcion = "Cuánto compró cada cliente y en cuántas facturas." },
    // new { nombre = "ventas-por-vendedor",     titulo = "Ventas por vendedor",          descripcion = "Cuánto vendió cada vendedor." },
    // new { nombre = "ventas-por-empresa",      titulo = "Ventas por empresa",           descripcion = "Las ventas agrupadas por la empresa del cliente." },
    // new { nombre = "ticket-por-vendedor",     titulo = "Ticket promedio por vendedor", descripcion = "Cuánto vale en promedio una factura de cada vendedor." },
    // new { nombre = "productos-sin-vender",    titulo = "Productos sin vender",         descripcion = "Los que nunca aparecieron en una factura activa." },
    // new { nombre = "anulaciones-por-cliente", titulo = "Anulaciones por cliente",      descripcion = "Quién anula más, y cuánto dinero se devolvió." },
    // new { nombre = "alcance-de-usuarios",     titulo = "Alcance de los usuarios",      descripcion = "A cuántas interfaces llega cada usuario por sus roles." },
    // new { nombre = "interfaces-sin-usuarios", titulo = "Interfaces sin usuarios",      descripcion = "Rutas a las que hoy no llega nadie." },
    // new { nombre = "credito-contra-consumo",  titulo = "Crédito contra consumo",       descripcion = "Cuánto crédito tiene cada cliente contra lo que ha comprado." },
    // };
    //
    // return Ok(new
    // {
    // total = consultas.Length,
    // datos = consultas.Select(c => new
    // {
    // c.nombre,
    // c.titulo,
    // c.descripcion,
    // ruta = $"/api/consultas/{c.nombre}",
    // }),
    // });
    // }

    // ------------------------------------------------------------
    // GET /api/consultas/ventas-por-producto
    // Cuánto se ha vendido de cada producto
    // Cruza: producto · productosporfactura · factura · cliente
    // ------------------------------------------------------------
    [HttpGet("ventas-por-producto")]
    public async Task<IActionResult> VentasPorProducto()
    {
        try
        {
            var datos = await _servicio.VentasPorProductoAsync();
            return Ok(new { consulta = "ventas_por_producto", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/ventas-por-cliente
    // Cuánto ha comprado cada cliente
    // Cruza: persona · cliente · factura · productosporfactura
    // ------------------------------------------------------------
    [HttpGet("ventas-por-cliente")]
    public async Task<IActionResult> VentasPorCliente()
    {
        try
        {
            var datos = await _servicio.VentasPorClienteAsync();
            return Ok(new { consulta = "ventas_por_cliente", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/ventas-por-vendedor
    // Cuánto ha vendido cada vendedor
    // Cruza: persona · vendedor · factura · productosporfactura
    // ------------------------------------------------------------
    [HttpGet("ventas-por-vendedor")]
    public async Task<IActionResult> VentasPorVendedor()
    {
        try
        {
            var datos = await _servicio.VentasPorVendedorAsync();
            return Ok(new { consulta = "ventas_por_vendedor", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/ventas-por-empresa
    // Cuánto se le ha facturado a cada empresa
    // Cruza: empresa · cliente · factura · productosporfactura
    // ------------------------------------------------------------
    [HttpGet("ventas-por-empresa")]
    public async Task<IActionResult> VentasPorEmpresa()
    {
        try
        {
            var datos = await _servicio.VentasPorEmpresaAsync();
            return Ok(new { consulta = "ventas_por_empresa", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/ticket-por-vendedor
    // El valor promedio de una factura, por vendedor
    // Cruza: persona · vendedor · factura · productosporfactura
    // ------------------------------------------------------------
    [HttpGet("ticket-por-vendedor")]
    public async Task<IActionResult> TicketPorVendedor()
    {
        try
        {
            var datos = await _servicio.TicketPorVendedorAsync();
            return Ok(new { consulta = "ticket_por_vendedor", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/productos-sin-vender
    // Los productos que nunca se han vendido
    // Cruza: producto · productosporfactura · factura · cliente
    // ------------------------------------------------------------
    [HttpGet("productos-sin-vender")]
    public async Task<IActionResult> ProductosSinVender()
    {
        try
        {
            var datos = await _servicio.ProductosSinVenderAsync();
            return Ok(new { consulta = "productos_sin_vender", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/anulaciones-por-cliente
    // Cuánto se ha anulado, por cliente
    // Cruza: persona · cliente · factura · productosporfactura
    // ------------------------------------------------------------
    [HttpGet("anulaciones-por-cliente")]
    public async Task<IActionResult> AnulacionesPorCliente()
    {
        try
        {
            var datos = await _servicio.AnulacionesPorClienteAsync();
            return Ok(new { consulta = "anulaciones_por_cliente", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/alcance-de-usuarios
    // Hasta dónde llega cada usuario
    // Cruza: usuario · rol_usuario · rol · rutarol · ruta
    // ------------------------------------------------------------
    [HttpGet("alcance-de-usuarios")]
    public async Task<IActionResult> AlcanceDeUsuarios()
    {
        try
        {
            var datos = await _servicio.AlcanceDeUsuariosAsync();
            return Ok(new { consulta = "alcance_de_usuarios", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/interfaces-sin-usuarios
    // Las interfaces a las que no llega nadie
    // Cruza: ruta · rutarol · rol · rol_usuario
    // ------------------------------------------------------------
    [HttpGet("interfaces-sin-usuarios")]
    public async Task<IActionResult> InterfacesSinUsuarios()
    {
        try
        {
            var datos = await _servicio.InterfacesSinUsuariosAsync();
            return Ok(new { consulta = "interfaces_sin_usuarios", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }

    // ------------------------------------------------------------
    // GET /api/consultas/credito-contra-consumo
    // El crédito de cada cliente contra lo que lleva consumido
    // Cruza: persona · empresa · cliente · factura
    // ------------------------------------------------------------
    [HttpGet("credito-contra-consumo")]
    public async Task<IActionResult> CreditoContraConsumo()
    {
        try
        {
            var datos = await _servicio.CreditoContraConsumoAsync();
            return Ok(new { consulta = "credito_contra_consumo", total = datos.Count, datos });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { estado = 500, mensaje = "Error interno.", detalle = e.Message });
        }
    }
}
