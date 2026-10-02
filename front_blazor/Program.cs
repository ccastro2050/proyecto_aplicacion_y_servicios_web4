using FrontFacturas.Components;
using FrontFacturas.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server: el componente se renderiza en el servidor y el navegador
// recibe el HTML ya armado, manteniendo una conexión para los eventos.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ============================================================
// DE DÓNDE SALEN LOS DATOS
//
// De la API, por HTTP, y de ningún otro sitio. La dirección viene de la
// configuración: fuera de Docker vale lo de appsettings.json; dentro, el
// compose la sobreescribe con el NOMBRE del servicio —`api-facturas`—, porque
// `localhost` dentro de un contenedor es el contenedor mismo.
// ============================================================
var urlApi = builder.Configuration["UrlApi"] ?? "http://localhost:8035";

// ============================================================
// QUÉ VERSIÓN ES ESTA, Y QUÉ INTERFACES TIENE
//
// Es el ÚNICO sitio donde esto se escribe. El menú y el pie de página son el
// MISMO ARCHIVO en las cuatro versiones: leen de aquí.
//
// Antes el número de versión y las entradas estaban a mano en el layout, y ya
// había fallado — el Inicio decía «Versión 1 del proyecto» en los cuatro
// proyectos, porque era un texto en un archivo que nadie volvía a mirar.
// ============================================================
builder.Services.AddSingleton(new MenuApp
{
    Version = 4,
    Entradas =
    [
        // v4 — el tablero va PRIMERO: es lo que se mira al entrar.
        new() { Ruta = "tablero", Texto = "Tablero", Permiso = "interfaz.inicio" },
        new() { Ruta = "productos", Texto = "Productos", Permiso = "interfaz.productos" },
        new() { Ruta = "empresas", Texto = "Empresas", Permiso = "interfaz.empresas" },
        new() { Ruta = "personas", Texto = "Personas", Permiso = "interfaz.personas" },
        new() { Ruta = "roles", Texto = "Roles", Permiso = "interfaz.roles" },
        new() { Ruta = "rutas", Texto = "Rutas", Permiso = "interfaz.rutas" },
        new() { Ruta = "usuarios", Texto = "Usuarios", Permiso = "interfaz.usuarios" },
        new() { Ruta = "clientes", Texto = "Clientes", Permiso = "interfaz.clientes" },
        new() { Ruta = "vendedores", Texto = "Vendedores", Permiso = "interfaz.vendedores" },
        new() { Ruta = "facturas", Texto = "Facturas", Permiso = "interfaz.facturas" },
        new() { Ruta = "usuario-con-roles", Texto = "Usuarios y roles", Permiso = "interfaz.usuarios" },
        new() { Ruta = "rol-usuario", Texto = "Roles por usuario", Permiso = "interfaz.usuarios" },
        new() { Ruta = "ruta-rol", Texto = "Permisos por rol", Permiso = "interfaz.permisos" },
    ],
});


// ============================================================
// v3 — LA SESION
//
// EstadoSesion es `scoped`, que en Blazor Server significa «uno por
// CIRCUITO»: cada navegador conectado tiene el suyo, y el token de uno no se
// mezcla con el de otro.
//
// Si fuera `singleton` —que es el error facil, porque «total, es una sola
// aplicacion»— habria UN token para todos los que entren: el ultimo que se
// identifique le cambiaria la sesion a los demas.
// ============================================================
builder.Services.AddScoped<EstadoSesion>();

// El servicio de la sesion es el UNICO que funciona sin token: no puede exigir
// lo que todavia no existe.
builder.Services.AddHttpClient<ServicioSesion>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

// ============================================================
// UN SERVICIO POR RECURSO, y DOCE lineas porque son doce recursos.
//
// Un `ApiService.Listar("producto")` generico seria mas corto: con una tabla ni
// se nota la diferencia. Con doce, el que lee el codigo ya no sabe que rutas
// existen ni que devuelve cada una — y eso es justo lo que no se quiere.
//
// Las primeras seis son de la v1 (sin clave foranea); las otras seis, de la v2.
// ============================================================
builder.Services.AddHttpClient<ServicioProducto>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioEmpresa>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioPersona>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioRol>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioRuta>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioUsuario>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioCliente>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioVendedor>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioFactura>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioUsuarioConRoles>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioRolUsuario>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<ServicioRutaRol>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

// v4 — las diez consultas del tablero.
builder.Services.AddHttpClient<ServicioConsultas>(cliente =>
{
    cliente.BaseAddress = new Uri(urlApi);
    cliente.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
