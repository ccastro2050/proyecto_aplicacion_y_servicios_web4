// ============================================================
// Programa.cs — Prueba de capas (criterio 6 de la v1).
//
// Verifica que ServicioProducto funciona con un repositorio
// FALSO en memoria que implementa IRepositorioProducto — sin
// SQL Server corriendo.
//
// Si esto pasa, las capas quedaron bien cortadas (polimorfismo +
// inversión de dependencias): el servicio depende de la INTERFAZ,
// no del motor.
//
// Este archivo usa "instrucciones de nivel superior" de C#: el
// programa se escribe directo, sin envolverlo en una clase Main.
// Regla del lenguaje: esas instrucciones van PRIMERO y las clases
// de apoyo (el repositorio falso) se declaran AL FINAL del archivo.
// ============================================================

using ApiFacturas.Excepciones;
using ApiFacturas.Fabricas;
using ApiFacturas.Modelos;
using ApiFacturas.Repositorios;
using ApiFacturas.Servicios;

// ------------------------------------------------------------
// La prueba: el MISMO ServicioProducto de la API, pero armado con
// el repositorio falso (polimorfismo). Un guion de verificaciones.
// ------------------------------------------------------------

var servicio = new ServicioProducto(new RepositorioFalsoEnMemoria());

// El ciclo completo contra el repositorio falso. Las lecturas
// devuelven OBJETOS Producto: se pregunta por sus propiedades.
await servicio.CrearAsync(new Producto { Codigo = "T1", Nombre = "Test", Stock = 5, Valorunitario = 100m });
Verificar((await servicio.ListarAsync(10))[0].Codigo == "T1", "crear + listar");
Verificar((await servicio.ObtenerAsync("T1")).Nombre == "Test", "obtener por código");
Verificar(await servicio.ActualizarAsync("T1", new() { ["stock"] = 9 }) == 1, "actualizar");
Verificar((await servicio.ObtenerAsync("T1")).Stock == 9, "el stock quedó en 9");
Verificar(await servicio.EliminarAsync("T1") == 1, "eliminar");

// Las excepciones de negocio también funcionan sin BD:
try { await servicio.ObtenerAsync("NOEXISTE"); Verificar(false, "debió lanzar NoEncontradoExcepcion"); }
catch (NoEncontradoExcepcion) { /* esperado */ }

try { await servicio.ActualizarAsync("T1", new()); Verificar(false, "debió lanzar ArgumentException"); }
catch (ArgumentException) { /* esperado */ }

try { await servicio.ListarAsync(0); Verificar(false, "debió lanzar ArgumentException"); }
catch (ArgumentException) { /* esperado */ }

// ------------------------------------------------------------
// v2 — la prueba CRECE: el mismo guion, ahora sobre PERSONA con
// SU repositorio falso. Si el molde se replicó bien, esto pasa
// sin sorpresas (esa es la evidencia del criterio 6 de la v2).
// ------------------------------------------------------------

var servicioPersona = new ServicioPersona(new RepositorioPersonaFalsoEnMemoria());

await servicioPersona.CrearAsync(new Persona { Codigo = "T1", Nombre = "Test", Email = "t@t.co", Telefono = "300" });
Verificar((await servicioPersona.ListarAsync(10))[0].Codigo == "T1", "persona: crear + listar");
Verificar((await servicioPersona.ObtenerAsync("T1")).Nombre == "Test", "persona: obtener por código");
Verificar(await servicioPersona.ActualizarAsync("T1", new() { ["telefono"] = "301" }) == 1, "persona: actualizar");
Verificar((await servicioPersona.ObtenerAsync("T1")).Telefono == "301", "persona: el teléfono quedó en 301");
Verificar(await servicioPersona.EliminarAsync("T1") == 1, "persona: eliminar");

try { await servicioPersona.ObtenerAsync("NOEXISTE"); Verificar(false, "persona: debió lanzar NoEncontradoExcepcion"); }
catch (NoEncontradoExcepcion) { /* esperado */ }

try { await servicioPersona.ActualizarAsync("T1", new()); Verificar(false, "persona: debió lanzar ArgumentException"); }
catch (ArgumentException) { /* esperado */ }

// ------------------------------------------------------------
// v3 — el molde una vez más, ahora EMPRESA (criterio 6 de la v3).
// ------------------------------------------------------------

var servicioEmpresa = new ServicioEmpresa(new RepositorioEmpresaFalsoEnMemoria());

await servicioEmpresa.CrearAsync(new Empresa { Codigo = "T1", Nombre = "Test S.A." });
Verificar((await servicioEmpresa.ListarAsync(10))[0].Codigo == "T1", "empresa: crear + listar");
Verificar((await servicioEmpresa.ObtenerAsync("T1")).Nombre == "Test S.A.", "empresa: obtener");
Verificar(await servicioEmpresa.ActualizarAsync("T1", new() { ["nombre"] = "Test SAS" }) == 1, "empresa: actualizar");
Verificar(await servicioEmpresa.EliminarAsync("T1") == 1, "empresa: eliminar");

try { await servicioEmpresa.ObtenerAsync("NOEXISTE"); Verificar(false, "empresa: debió lanzar NoEncontradoExcepcion"); }
catch (NoEncontradoExcepcion) { /* esperado */ }

Console.WriteLine("CRITERIO 6 OK: producto, persona y empresa funcionan con repositorios falsos, sin SQL Server");

// ------------------------------------------------------------
// v4 — la fábrica elige el motor SIN abrir conexiones (criterio 5
// de la v4). Construir un repositorio solo guarda la cadena; por
// eso se puede verificar el patrón con cadenas de mentira.
// ------------------------------------------------------------

IFabricaRepositorios fabricaSql = new FabricaSqlServer("Server=inexistente");
IFabricaRepositorios fabricaPg = new FabricaPostgres("Host=inexistente");

Verificar(fabricaSql.CrearRepositorioProducto() is RepositorioProductoSqlServer, "fábrica sqlserver: producto del dialecto correcto");
Verificar(fabricaSql.CrearRepositorioFactura() is RepositorioFacturaSqlServer, "fábrica sqlserver: factura del dialecto correcto");
Verificar(fabricaSql.CrearRepositorioUsuario() is RepositorioUsuarioSqlServer, "fábrica sqlserver: usuario del dialecto correcto");
Verificar(fabricaPg.CrearRepositorioProducto() is RepositorioProductoPostgres, "fábrica postgres: producto del dialecto correcto");
Verificar(fabricaPg.CrearRepositorioFactura() is RepositorioFacturaPostgres, "fábrica postgres: factura del dialecto correcto");
Verificar(fabricaPg.CrearRepositorioUsuario() is RepositorioUsuarioPostgres, "fábrica postgres: usuario del dialecto correcto");

Console.WriteLine("CRITERIO 5 OK: cada fábrica entrega los repositorios de su motor, sin abrir conexiones");

// Mini-verificador (función local): si la condición es falsa, reporta
// y sale con error (terminar con 0 = pasó; con 1 = falló).
static void Verificar(bool condicion, string descripcion)
{
    if (!condicion)
    {
        Console.Error.WriteLine($"FALLÓ: {descripcion}");
        Environment.Exit(1);
    }
}

// ------------------------------------------------------------
// El REPOSITORIO FALSO: cumple el mismo contrato que el de SQL
// Server, pero guarda los Producto en un diccionario en memoria —
// cero SQL, cero red. Como el servicio depende de la INTERFAZ,
// no nota la diferencia.
// ------------------------------------------------------------
class RepositorioFalsoEnMemoria : IRepositorioProducto
{
    // El "almacén": código → Producto.
    private readonly Dictionary<string, Producto> _datos = new();

    // Los métodos del contrato son async (devuelven Task); como aquí
    // no hay nada que esperar, se responde con Task.FromResult
    // ("promesa ya cumplida con este valor"):

    public Task<List<Producto>> ObtenerTodosAsync(int limite)
    {
        // Ordenar por código y tomar los primeros 'limite' (como el TOP):
        var lista = _datos.Values.OrderBy(p => p.Codigo).Take(limite).ToList();
        return Task.FromResult(lista);
    }

    public Task<Producto?> ObtenerPorCodigoAsync(string codigo)
    {
        // TryGetValue busca la llave; si no está, producto queda null:
        _datos.TryGetValue(codigo, out var producto);
        return Task.FromResult(producto);
    }

    public Task CrearAsync(Producto producto)
    {
        _datos[producto.Codigo] = producto;
        return Task.CompletedTask;   // promesa cumplida, sin valor
    }

    public Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        if (!_datos.TryGetValue(codigo, out var producto))
        {
            return Task.FromResult(0);   // 0 filas = no existía
        }
        // Se escriben SOLO los campos que llegaron, con los setters
        // de las propiedades del modelo:
        if (datos.TryGetValue("nombre", out var nombre)) { producto.Nombre = (string)nombre; }
        if (datos.TryGetValue("stock", out var stock)) { producto.Stock = (int)stock; }
        if (datos.TryGetValue("valorunitario", out var valor)) { producto.Valorunitario = (decimal)valor; }
        return Task.FromResult(1);
    }

    public Task<int> EliminarAsync(string codigo)
    {
        // Remove devuelve true si la llave existía; se traduce a 1/0 filas:
        return Task.FromResult(_datos.Remove(codigo) ? 1 : 0);
    }
}

// ------------------------------------------------------------
// v2 — el repositorio falso de PERSONA: el gemelo del de arriba,
// calcado igual que se calcó toda la rebanada. Cumple
// IRepositorioPersona con un diccionario en memoria.
// ------------------------------------------------------------
class RepositorioPersonaFalsoEnMemoria : IRepositorioPersona
{
    private readonly Dictionary<string, Persona> _datos = new();

    public Task<List<Persona>> ObtenerTodasAsync(int limite)
    {
        var lista = _datos.Values.OrderBy(p => p.Codigo).Take(limite).ToList();
        return Task.FromResult(lista);
    }

    public Task<Persona?> ObtenerPorCodigoAsync(string codigo)
    {
        _datos.TryGetValue(codigo, out var persona);
        return Task.FromResult(persona);
    }

    public Task CrearAsync(Persona persona)
    {
        _datos[persona.Codigo] = persona;
        return Task.CompletedTask;
    }

    public Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        if (!_datos.TryGetValue(codigo, out var persona))
        {
            return Task.FromResult(0);
        }
        if (datos.TryGetValue("nombre", out var nombre)) { persona.Nombre = (string)nombre; }
        if (datos.TryGetValue("email", out var email)) { persona.Email = (string)email; }
        if (datos.TryGetValue("telefono", out var telefono)) { persona.Telefono = (string)telefono; }
        return Task.FromResult(1);
    }

    public Task<int> EliminarAsync(string codigo)
    {
        return Task.FromResult(_datos.Remove(codigo) ? 1 : 0);
    }
}

// ------------------------------------------------------------
// v3 — el repositorio falso de EMPRESA (calcado de los otros dos).
// ------------------------------------------------------------
class RepositorioEmpresaFalsoEnMemoria : IRepositorioEmpresa
{
    private readonly Dictionary<string, Empresa> _datos = new();

    public Task<List<Empresa>> ObtenerTodasAsync(int limite)
    {
        var lista = _datos.Values.OrderBy(e => e.Codigo).Take(limite).ToList();
        return Task.FromResult(lista);
    }

    public Task<Empresa?> ObtenerPorCodigoAsync(string codigo)
    {
        _datos.TryGetValue(codigo, out var empresa);
        return Task.FromResult(empresa);
    }

    public Task CrearAsync(Empresa entidad)
    {
        _datos[entidad.Codigo] = entidad;
        return Task.CompletedTask;
    }

    public Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos)
    {
        if (!_datos.TryGetValue(codigo, out var empresa)) { return Task.FromResult(0); }
        if (datos.TryGetValue("nombre", out var nombre)) { empresa.Nombre = (string)nombre; }
        return Task.FromResult(1);
    }

    public Task<int> EliminarAsync(string codigo)
    {
        return Task.FromResult(_datos.Remove(codigo) ? 1 : 0);
    }
}
