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
using System.Text;
using ApiFacturas.Autorizacion;
using ApiFacturas.Modelos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
// 1bis. EL CONTROL DE ACCESO (v3)
// ------------------------------------------------------------
// Dos cosas distintas, y el orden en que se nombran no es casual:
//
//   AUTENTICACION  ¿quien es usted?   -> el token, y 401 si no hay
//   AUTORIZACION   ¿que puede hacer?  -> verificar_acceso_ruta, y 403 si no
//
// Se confunden porque en ingles las dos empiezan igual -de ahi que se escriban
// authn y authz-. Aqui estan separadas a proposito: lo de abajo resuelve la
// primera; el atributo [ExigePermiso] resuelve la segunda.

// La configuracion del token. Sale de appsettings.json, y en Docker la
// sobreescribe el compose con Jwt__Key, Jwt__Issuer...
var configuracionJwt = new ConfiguracionJwt();
builder.Configuration.GetSection("Jwt").Bind(configuracionJwt);
if (string.IsNullOrWhiteSpace(configuracionJwt.Key) || configuracionJwt.Key.Length < 32)
{
    // HMAC-SHA256 pide al menos 32 bytes de clave. Fallar aqui, al arrancar, es
    // mucho mejor que fallar al firmar el primer token -que seria un 500 que
    // nadie relaciona con la configuracion-.
    throw new InvalidOperationException(
        "La clave Jwt:Key falta o tiene menos de 32 caracteres.");
}
builder.Services.AddSingleton(configuracionJwt);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            // Las cuatro validaciones, y las cuatro importan:
            ValidateIssuer = true,            // que lo haya emitido ESTA API
            ValidateAudience = true,          // que sea para ESTA API
            ValidateLifetime = true,          // que no haya vencido
            ValidateIssuerSigningKey = true,  // QUE NADIE LO HAYA ALTERADO
            ValidIssuer = configuracionJwt.Issuer,
            ValidAudience = configuracionJwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuracionJwt.Key)),

            // Por defecto ASP.NET perdona 5 minutos de reloj desadaptado. Se
            // baja a cero para que «vencido» signifique vencido: si no, el
            // criterio del token expirado da 200 durante cinco minutos y
            // parece que el codigo esta mal.
            ClockSkew = TimeSpan.Zero,
        };

        // El 401 de la API, con el mismo sobre que todos los demas errores. Sin
        // esto, ASP.NET responde un 401 con el cuerpo VACIO y la interfaz
        // grafica no tiene nada que mostrarle a la persona.
        opciones.Events = new JwtBearerEvents
        {
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();
                contexto.Response.StatusCode = 401;
                contexto.Response.ContentType = "application/json";
                await contexto.Response.WriteAsync(
                    "{\"estado\":401,\"mensaje\":\"No hay una sesion valida. "
                    + "Inicie sesion.\"}");
            },
        };
    });

builder.Services.AddAuthorization();

// El repositorio del acceso: la unica puerta a verificar_acceso_ruta.
builder.Services.AddScoped<IRepositorioAcceso>(
    _ => fabrica.CrearRepositorioAcceso());
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();
// El recurso MAESTRO-DETALLE sobre la tabla puente: el usuario Y sus roles en
// una sola operacion. Por la FABRICA, como todos: con el motor en postgres, uno
// que siguiera hablando con SQL Server no se notaria hasta que los dos motores
// dejaran de tener los mismos datos.
builder.Services.AddScoped<IRepositorioUsuarioConRoles>(
    _ => fabrica.CrearRepositorioUsuarioConRoles());
builder.Services.AddScoped<IServicioUsuarioConRoles, ServicioUsuarioConRoles>();

// ------------------------------------------------------------
// LA v4 — las diez consultas multitabla
// ------------------------------------------------------------
// No agregan ninguna tabla: cruzan las que ya estan. Lo que agregan es la
// pregunta que ningun endpoint anterior podia responder.
builder.Services.AddScoped<IRepositorioConsultas>(
    _ => fabrica.CrearRepositorioConsultas());
builder.Services.AddScoped<IServicioConsultas, ServicioConsultas>();



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
builder.Services.AddSwaggerGen(opciones =>
{
    // ============================================================
    // v3 — EL BOTON «Authorize» DE SWAGGER.
    //
    // Sin esto, al exigir token TODO responde 401 desde Swagger y no hay
    // donde pegarlo: la API funciona y la herramienta con la que se
    // sustenta el proyecto deja de servir.
    //
    // Y ensena algo que no es obvio: Swagger NO ADIVINA como se autentica
    // una API. Hay que declararlo, y eso es parte del contrato — un
    // documento OpenAPI que no dice como se entra esta incompleto.
    //
    // COMO SE USA, y conviene escribirlo porque la primera vez cuesta:
    //
    //   1. POST /api/sesion con { "email": "...", "contrasena": "..." }
    //   2. Copie el valor de `token` de la respuesta (sin las comillas)
    //   3. Boton «Authorize», arriba a la derecha, y pegue SOLO el token
    //      -sin la palabra Bearer: la pone Swagger-
    //   4. Ya puede probar los demas endpoints
    // ============================================================
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue AQUI el `token` que devuelve POST /api/sesion. "
                    + "Solo el token: la palabra «Bearer» la pone Swagger.",
    });

    // Y esto es lo que le pone el candado a cada endpoint. Sin el, el boton
    // aparece pero el token no viaja — y todo sigue en 401, que es el peor
    // de los dos errores porque parece que si se configuro.
    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        }
    });
});             // arma el documento OpenAPI

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
// ------------------------------------------------------------
// EL CONTROL DE ACCESO, Y EL ORDEN IMPORTA (v3)
// ------------------------------------------------------------
// UseAuthentication va ANTES de UseAuthorization, y no es un gusto: el primero
// lee el token y averigua QUIEN es; el segundo decide si PUEDE. Al reves, el
// segundo no tendria a quien consultar — y dejaria pasar todo.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Arrancar y quedarse escuchando (el puerto lo fija ASPNETCORE_URLS
// en el Dockerfile: 8035):
app.Run();
