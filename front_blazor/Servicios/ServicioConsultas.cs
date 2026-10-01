using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontFacturas.Modelos;

namespace FrontFacturas.Servicios;

// ============================================================
// ServicioConsultas — las diez consultas del tablero.
//
// UN METODO POR CONSULTA, con su nombre y su tipo de vuelta. No un
// `ObtenerAsync<T>(string nombre)` generico, por la misma razon que la API
// tiene diez endpoints con nombre: el que lee esto sabe qué consultas hay y
// qué devuelve cada una, y el compilador le avisa si se equivoca.
//
// EL SOBRE DE ESTAS ES DISTINTO al de los recursos: { consulta, total,
// datos[] }. No trae `tabla` ni `limite` — una consulta no es una tabla y no
// se pagina. Se mira el contrato, no se supone.
// ============================================================
public class ServicioConsultas(HttpClient cliente, EstadoSesion sesion)
{
    private const string Ruta = "api/consultas";

    private static readonly JsonSerializerOptions Opciones =
        new() { PropertyNameCaseInsensitive = true };

    public record Resultado<T>(bool Bien, T? Dato, string? Mensaje)
    {
        public static Resultado<T> Ok(T dato) => new(true, dato, null);
        public static Resultado<T> Falla(string mensaje) => new(false, default, mensaje);
    }

    private void Autorizar()
    {
        cliente.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(sesion.Token)
                ? null
                : new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", sesion.Token);
    }

    /// <summary>El unico metodo que habla HTTP. Los diez de abajo le dicen qué
    /// consulta pedir y qué tipo esperar — asi el manejo del sobre, de los
    /// errores y del token esta escrito UNA vez.</summary>
    private async Task<Resultado<List<T>>> PedirAsync<T>(string consulta)
    {
        try
        {
            Autorizar();
            var r = await cliente.GetAsync($"{Ruta}/{consulta}");

            if (r.StatusCode == HttpStatusCode.NoContent)
                return Resultado<List<T>>.Ok([]);
            if (!r.IsSuccessStatusCode)
                return Resultado<List<T>>.Falla(await MensajeDe(r));

            var sobre = await r.Content.ReadFromJsonAsync<JsonElement>();
            var datos = sobre.GetProperty("datos").Deserialize<List<T>>(Opciones) ?? [];
            return Resultado<List<T>>.Ok(datos);
        }
        catch (Exception)
        {
            return Resultado<List<T>>.Falla(
                "No se pudo conectar con el servicio. Intente de nuevo en un momento.");
        }
    }

    public Task<Resultado<List<VentaPorProducto>>> VentasPorProductoAsync() =>
        PedirAsync<VentaPorProducto>("ventas-por-producto");

    public Task<Resultado<List<VentaPorCliente>>> VentasPorClienteAsync() =>
        PedirAsync<VentaPorCliente>("ventas-por-cliente");

    public Task<Resultado<List<VentaPorVendedor>>> VentasPorVendedorAsync() =>
        PedirAsync<VentaPorVendedor>("ventas-por-vendedor");

    public Task<Resultado<List<VentaPorEmpresa>>> VentasPorEmpresaAsync() =>
        PedirAsync<VentaPorEmpresa>("ventas-por-empresa");

    public Task<Resultado<List<TicketPorVendedor>>> TicketPorVendedorAsync() =>
        PedirAsync<TicketPorVendedor>("ticket-por-vendedor");

    public Task<Resultado<List<ProductoSinVender>>> ProductosSinVenderAsync() =>
        PedirAsync<ProductoSinVender>("productos-sin-vender");

    public Task<Resultado<List<AnulacionPorCliente>>> AnulacionesPorClienteAsync() =>
        PedirAsync<AnulacionPorCliente>("anulaciones-por-cliente");

    public Task<Resultado<List<AlcanceDeUsuario>>> AlcanceDeUsuariosAsync() =>
        PedirAsync<AlcanceDeUsuario>("alcance-de-usuarios");

    public Task<Resultado<List<InterfazSinUsuarios>>> InterfacesSinUsuariosAsync() =>
        PedirAsync<InterfazSinUsuarios>("interfaces-sin-usuarios");

    public Task<Resultado<List<CreditoContraConsumo>>> CreditoContraConsumoAsync() =>
        PedirAsync<CreditoContraConsumo>("credito-contra-consumo");

    private static async Task<string> MensajeDe(HttpResponseMessage r)
    {
        try
        {
            var e = await r.Content.ReadFromJsonAsync<ErrorApi>(Opciones);
            if (!string.IsNullOrWhiteSpace(e?.Mensaje)) return e!.Mensaje;
        }
        catch { }

        return r.StatusCode switch
        {
            HttpStatusCode.Unauthorized =>
                "Su sesion no es valida o ya vencio. Vuelva a iniciar sesion.",
            HttpStatusCode.Forbidden =>
                "Su rol no tiene permiso para ver el tablero.",
            _ => "El servicio respondio con un problema. Intente de nuevo."
        };
    }
}
