// ============================================================
// Program.cs — el PUNTO DE ENTRADA de la API (el "main" de .NET).
//
// Aquí se arma la aplicación: se registran los servicios (el
// ENSAMBLADOR de las capas), se configura cómo responder cuando
// una petición no valida (422), y se encienden las rutas.
//
// El recorrido completo de una petición está explicado en
// docs/FLUJO_DE_UNA_PETICION.md.
// ============================================================

// "using" trae tipos de otros espacios de nombres para poder usarlos:
using ApiFacturas.Fabricas;
using ApiFacturas.Repositorios;
using ApiFacturas.Servicios;
using Microsoft.AspNetCore.Mvc;

// El "builder" es el constructor de la aplicación: a él se le
// registra TODO antes de arrancar.
var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1. EL ENSAMBLADOR — el único lugar que conoce clases concretas
// ------------------------------------------------------------
// Aquí se le dice al contenedor de dependencias de .NET qué clase
// concreta entregar cuando alguien pida una INTERFAZ:
//   - pide IRepositorioProducto → recibe RepositorioProductoSqlServer
//   - pide IServicioProducto    → recibe ServicioProducto
// El controlador y el servicio JAMÁS hacen "new" de clases concretas:
// las reciben por constructor (inyección de dependencias).
// v4: llegó el segundo motor y la promesa se cumplió — SOLO estas
// líneas cambiaron. El motor se decide en UN switch y los
// repositorios los entrega la FÁBRICA (Fabricas/).

// Las cadenas de conexión: vienen de appsettings.json, y en Docker las
// sobreescriben las variables ConnectionStrings__SqlServer / __Postgres.
var cadenaSqlServer = builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'SqlServer'.");
var cadenaPostgres = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Postgres'.");

// v4 — EL ÚNICO PUNTO DEL CÓDIGO QUE DECIDE EL MOTOR. La clave 'Motor'
// la fija el compose (interruptor MOTOR_BD, default postgres); corriendo
// local sin Docker sale de appsettings.json (sqlserver):
var motor = builder.Configuration["Motor"] ?? "sqlserver";
IFabricaRepositorios fabrica = motor switch
{
    "sqlserver" => new FabricaSqlServer(cadenaSqlServer),
    "postgres" => new FabricaPostgres(cadenaPostgres),
    _ => throw new InvalidOperationException(
        $"Motor desconocido: '{motor}' (use sqlserver o postgres)."),
};

// AddScoped = "una instancia por petición HTTP" (cada request estrena la suya):
builder.Services.AddScoped<IRepositorioProducto>(_ => fabrica.CrearRepositorioProducto());
builder.Services.AddScoped<IServicioProducto, ServicioProducto>();

// v2 — el ensamblador CRECE (y es lo único de la v1 que crece):
// las rebanadas nuevas se registran igual que la primera.
builder.Services.AddScoped<IRepositorioPersona>(_ => fabrica.CrearRepositorioPersona());
builder.Services.AddScoped<IServicioPersona, ServicioPersona>();
builder.Services.AddScoped<IRepositorioFactura>(_ => fabrica.CrearRepositorioFactura());
builder.Services.AddScoped<IServicioFactura, ServicioFactura>();

// v3 — las 8 rebanadas que completan la BD. (En la v3 esta lista
// "dolía": cada línea repetía el motor. La v4 curó el dolor: la lista
// sigue — una rebanada es una rebanada — pero el motor ya no aparece
// en ninguna: lo decide la fábrica, en un solo lugar.)
builder.Services.AddScoped<IRepositorioEmpresa>(_ => fabrica.CrearRepositorioEmpresa());
builder.Services.AddScoped<IServicioEmpresa, ServicioEmpresa>();
builder.Services.AddScoped<IRepositorioCliente>(_ => fabrica.CrearRepositorioCliente());
builder.Services.AddScoped<IServicioCliente, ServicioCliente>();
builder.Services.AddScoped<IRepositorioVendedor>(_ => fabrica.CrearRepositorioVendedor());
builder.Services.AddScoped<IServicioVendedor, ServicioVendedor>();
builder.Services.AddScoped<IRepositorioUsuario>(_ => fabrica.CrearRepositorioUsuario());
builder.Services.AddScoped<IServicioUsuario, ServicioUsuario>();
builder.Services.AddScoped<IRepositorioRol>(_ => fabrica.CrearRepositorioRol());
builder.Services.AddScoped<IServicioRol, ServicioRol>();
builder.Services.AddScoped<IRepositorioRuta>(_ => fabrica.CrearRepositorioRuta());
builder.Services.AddScoped<IServicioRuta, ServicioRuta>();
builder.Services.AddScoped<IRepositorioRolUsuario>(_ => fabrica.CrearRepositorioRolUsuario());
builder.Services.AddScoped<IServicioRolUsuario, ServicioRolUsuario>();
builder.Services.AddScoped<IRepositorioRutaRol>(_ => fabrica.CrearRepositorioRutaRol());
builder.Services.AddScoped<IServicioRutaRol, ServicioRutaRol>();

// ------------------------------------------------------------
// 2. Los controladores y la validación de la petición (el 422)
// ------------------------------------------------------------
// AddControllers activa el sistema de controladores ([ApiController]).
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opciones =>
    {
        // Cuando un body NO cumple las reglas de su petición (las anotaciones
        // [Required], [Range]... de Peticiones/), ASP.NET arma solo la
        // respuesta de error. Aquí la personalizamos para que sea un
        // 422 con la lista de errores — el formato del contrato:
        opciones.InvalidModelStateResponseFactory = contexto =>
        {
            // Recorrer el ModelState y sacar cada mensaje de error:
            var errores = new List<string>();
            foreach (var campo in contexto.ModelState)
            {
                foreach (var error in campo.Value.Errors)
                {
                    errores.Add(error.ErrorMessage);
                }
            }
            // ObjectResult = "responde este objeto como JSON, con este código":
            return new ObjectResult(new
            {
                estado = 422,
                mensaje = "Datos inválidos.",
                errores
            })
            { StatusCode = 422 };
        };
    });

// ------------------------------------------------------------
// 2b. Swagger — la documentación interactiva de la API
// ------------------------------------------------------------
// Swashbuckle lee los controladores y sus clases de datos y genera una página
// donde se ven TODOS los endpoints y se pueden probar desde el
// navegador (http://localhost:8035/swagger).
builder.Services.AddEndpointsApiExplorer();   // descubre los endpoints
builder.Services.AddSwaggerGen();             // arma el documento OpenAPI

// Construir la aplicación con todo lo registrado:
var app = builder.Build();

// Encender Swagger: el JSON (OpenAPI) y la página interactiva:
app.UseSwagger();
app.UseSwaggerUI();

// ------------------------------------------------------------
// 3. Las rutas
// ------------------------------------------------------------

// GET / — diagnóstico (usable como healthcheck). MapGet registra una
// ruta directa sin necesidad de un controlador:
app.MapGet("/", () => Results.Json(new
{
    mensaje = "API Facturas funcionando",
    version = "v4",
    motor,      // v4: a cuál motor le está hablando la API (el interruptor)
    contratos = "docs/spec_kit/versiones/v4_postgresql/6_contracts.md"
}));

// MapControllers enciende las rutas declaradas con atributos en los
// controladores ([Route], [HttpGet], [HttpPost]...):
app.MapControllers();

// Arrancar y quedarse escuchando (el puerto lo fija ASPNETCORE_URLS
// en el Dockerfile: 8035):
app.Run();
