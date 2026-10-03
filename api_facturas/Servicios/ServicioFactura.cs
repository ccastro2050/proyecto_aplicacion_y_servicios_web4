// ============================================================
// ServicioFactura — la capa de NEGOCIO de factura.
//
// Note lo DELGADO que es comparado con ServicioProducto: aquí no
// hay cálculos porque la regla RNF2 de la v2 lo prohíbe — los
// subtotales, el total y el stock son de la BD (trigger + SPs).
// El servicio solo cuida los argumentos y delega por la interfaz.
// ============================================================

using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;

namespace ApiFacturas.Servicios;

public class ServicioFactura : IServicioFactura
{
    private readonly IRepositorioFactura _repositorio;

    public ServicioFactura(IRepositorioFactura repositorio)
    {
        _repositorio = repositorio;
    }

    private static void ValidarNumero(int numero)
    {
        if (numero <= 0)
        {
            throw new ArgumentException("El número de factura debe ser un entero mayor que cero.");
        }
    }

    public async Task<List<Factura>> ListarAsync()
    {
        return await _repositorio.ListarAsync();
    }

    public async Task<Factura> ConsultarAsync(int numero)
    {
        ValidarNumero(numero);
        // Si no existe, el REPOSITORIO ya lanza NoEncontradoExcepcion
        // (tradujo el THROW del SP) — aquí no hay nada más que hacer:
        return await _repositorio.ConsultarAsync(numero);
    }

    public async Task<Factura> CrearAsync(int fkidcliente, int fkidvendedor, string productosJson)
    {
        // El body ya pasó por la petición FacturaCrear (ids presentes,
        // lista con mínimo 1 renglón, cantidades >= 1). Si el cliente o
        // el vendedor no existen, la FK de la BD rechaza (500); si falta
        // stock, el TRIGGER rechaza con su mensaje (500). Delegar:
        return await _repositorio.CrearAsync(fkidcliente, fkidvendedor, productosJson);
    }

    public async Task<string> AnularAsync(int numero)
    {
        ValidarNumero(numero);
        return await _repositorio.AnularAsync(numero);
    }

    public async Task<Factura> ReemplazarAsync(int numero, int fkidcliente,
                                               int fkidvendedor, string productosJson)
    {
        ValidarNumero(numero);
        // El PUT exige la ficha completa —así lo validó la petición—, así que
        // aquí no hay nada que mezclar: se manda tal cual.
        return await _repositorio.ReemplazarAsync(numero, fkidcliente, fkidvendedor,
                                                  productosJson);
    }

    public async Task<Factura> ActualizarAsync(int numero, int? fkidcliente,
                                               int? fkidvendedor, string? productosJson)
    {
        ValidarNumero(numero);

        if (fkidcliente == null && fkidvendedor == null && productosJson == null)
        {
            throw new ArgumentException("No se envió ningún campo para actualizar.");
        }

        // ================================================================
        // EL PATCH DE UN MAESTRO-DETALLE
        // ================================================================
        // El procedimiento de la base exige los tres datos: no sabe escribir
        // «solo el vendedor». Así que esto LEE la factura, MEZCLA lo que llegó
        // y la reenvía completa.
        //
        // Y aquí está la razón de que el PATCH viva en el SERVICIO y no en el
        // controlador: mezclar lo viejo con lo nuevo es una decisión de
        // negocio, no de HTTP. El controlador solo traduce.
        var actual = await _repositorio.ConsultarAsync(numero);

        var productos = productosJson ?? System.Text.Json.JsonSerializer.Serialize(
            actual.Productos.Select(p => new { codigo = p.CodigoProducto,
                                               cantidad = p.Cantidad }));

        return await _repositorio.ReemplazarAsync(
            numero,
            fkidcliente  ?? actual.Fkidcliente,
            fkidvendedor ?? actual.Fkidvendedor,
            productos);
    }

    public async Task<string> EliminarAsync(int numero)
    {
        ValidarNumero(numero);
        return await _repositorio.EliminarAsync(numero);
    }
}
