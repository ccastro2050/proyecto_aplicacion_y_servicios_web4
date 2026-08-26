# SOLID, programación por capas y patrones de diseño — en este proyecto

> Documento conceptual del curso: los 5 principios SOLID, la arquitectura
> por capas y los patrones de diseño que este código usa, cada uno con su
> ejemplo REAL en la versión en curso — y en qué versión se termina de
> demostrar.

---

## 1. Programación por capas (la arquitectura del proyecto)

Organizar el sistema en **niveles con responsabilidades distintas**, donde
cada capa solo conoce a la inmediatamente inferior y siempre a través de un
contrato. Así se ve el **viaje de UNA petición** por dentro de la API — el
"diagrama de palitos" del curso:

```
            EL CLIENTE (navegador, Swagger, curl)
                 │
                 │  ① GET /api/producto/PR001
                 ▼
┌─────────────────────────────────────────────────────┐
│ CAPA 1 — CONTROLLER (HTTP)                          │
│ Controllers/ProductoController.cs                   │
│ Recibe la petición (el framework ya validó el body  │
│ contra la petición del verbo) y traduce el          │
│ resultado a códigos HTTP y JSON. NO tiene negocio.  │
│ NO tiene SQL.                                       │
└────────────────┬────────────────────────────────────┘
                 │  ② _servicio.ObtenerPorCodigoAsync("PR001")
                 ▼
┌─────────────────────────────────────────────────────┐
│ CAPA 2 — SERVICIO (negocio)                         │
│ Servicios/ServicioProducto.cs                       │
│ Las reglas del dominio: qué se puede y qué no (el   │
│ 404 "no existe" NACE aquí). NO conoce ASP.NET.      │
│ NO sabe qué motor hay debajo.                       │
└────────────────┬────────────────────────────────────┘
                 │  ③ _repositorio.ObtenerPorCodigoAsync("PR001")
                 │     — a través de la INTERFAZ IRepositorioProducto
                 ▼
┌─────────────────────────────────────────────────────┐
│ CAPA 3 — REPOSITORIO (datos)                        │
│ Repositorios/RepositorioProducto{Motor}.cs (v4: la  │
│ fábrica elige SqlServer o Postgres)                 │
│ El SQL a mano (Dapper): traduce filas ↔ objetos     │
│ Producto. NO conoce HTTP. NO decide negocio.        │
└────────────────┬────────────────────────────────────┘
                 │  ④ SELECT … FROM producto WHERE codigo = @codigo
                 ▼
          ┌───────────────┐
          │ BASE DE DATOS │  SQL Server o PostgreSQL — la MISMA bdfacturas
          └───────┬───────┘
                  │
   y la respuesta hace el viaje DE VUELTA:
   fila → objeto Producto (repositorio) → objeto (servicio)
        → JSON + 200 (controller) → cliente
```

Qué hace — y qué tiene PROHIBIDO — cada capa:

| Capa | Su trabajo | Prohibido para ella | En la v1 |
|---|---|---|---|
| **Controller** | HTTP: rutas, códigos de estado, JSON | SQL y reglas de negocio | `Controllers/ProductoController.cs` |
| **Servicio** | Las reglas del negocio (¿existe? ¿se puede?) | Saber de HTTP o del motor de BD | `Servicios/ServicioProducto.cs` |
| **Repositorio** | El SQL y el mapeo fila ↔ objeto | Saber de HTTP o decidir negocio | `Repositorios/RepositorioProductoSqlServer.cs` |

**La regla:** las dependencias apuntan en una sola dirección y cruzan por
**interfaces**. El controller conoce al servicio; el servicio conoce la
interfaz del repositorio; **nadie** conoce dos capas hacia abajo (el
controller no sabe que existe SQL Server).

**El mismo viaje cuando algo sale mal** — `GET /api/producto/PR999`:

1. El **repositorio** no encuentra la fila y devuelve `null` — un HECHO,
   sin opinión.
2. El **servicio** decide qué significa ese hecho: "ese producto no
   existe" — y lo dice lanzando `NoEncontradoExcepcion` (una DECISIÓN de
   negocio).
3. El **controller** captura la excepción y la traduce al idioma HTTP:
   **404** con su JSON.

Cada capa aportó exactamente lo suyo: datos → hecho, negocio → decisión,
HTTP → código de estado.

**¿Para qué?** Cada capa se puede cambiar, probar y entender POR SEPARADO.
La prueba viviente en la v1: `pruebas/` corre el servicio real con un
repositorio falso en memoria — sin BD. Eso solo es posible porque las capas
están bien cortadas.

## 2. Los 5 principios SOLID, uno por uno

### S — Responsabilidad única (Single Responsibility)
Cada clase tiene UNA razón para cambiar: el controller si cambia el
protocolo HTTP; el servicio si cambian las reglas de negocio; el
repositorio si cambia el SQL; las peticiones del verbo si cambian las reglas
de forma del body. Ninguna clase hace dos de esas cosas.

```csharp
// ❌ Sin S: un "controller" con tres razones de cambio (HTTP + negocio + SQL)
[HttpGet("{codigo}")]
public async Task<IActionResult> Obtener(string codigo)
{
    await using var conexion = new SqlConnection(...);   // SQL aquí = mezcla
    // ...y el if de "¿existe?" aquí = negocio mezclado
}

// ✅ Con S (la v1): un archivo por razón de cambio
//   Controllers/   → cambia solo si cambia el HTTP
//   Servicios/     → cambia solo si cambian las reglas
//   Repositorios/  → cambia solo si cambia el SQL
//   Peticiones/    → cambia solo si cambian las reglas del body
```

### O — Abierto/Cerrado (Open/Closed)
Abierto a extensión, cerrado a modificación. **El examen fue la v4**:
agregar PostgreSQL fue AGREGAR clases
(`RepositorioProductoPostgres : IRepositorioProducto`, y sus 10 gemelas)
y tocar SOLO el ensamblador — controller, servicio e interfaces quedaron
idénticos (`git diff v3` lo demuestra: la frontera no se cruzó).

```csharp
// La v4 AGREGÓ sin modificar: clases nuevas con la misma interfaz...
public class RepositorioProductoPostgres : IRepositorioProducto { /* … */ }

// ...y el ensamblador (Program.cs, ÚNICO archivo tocado) elige el motor
// UNA vez, a través de la fábrica (ver §3, patrones):
IFabricaRepositorios fabrica = motor switch
{
    "sqlserver" => new FabricaSqlServer(cadenaSqlServer),
    "postgres"  => new FabricaPostgres(cadenaPostgres),
    _ => throw new InvalidOperationException($"Motor desconocido: '{motor}'."),
};
```

### L — Sustitución de Liskov
Cualquier implementación de la interfaz puede ocupar el lugar de otra sin
romper nada. Ya pasa en la v1: `RepositorioFalsoEnMemoria` sustituye a
`RepositorioProductoSqlServer` en las pruebas y el servicio ni se entera.

```csharp
// El repositorio FALSO de las pruebas (criterio 6): sin BD, misma interfaz
public class RepositorioFalsoEnMemoria : IRepositorioProducto
{
    private readonly Dictionary<string, Producto> _datos = new();

    public Task<Producto?> ObtenerPorCodigoAsync(string codigo)
        => Task.FromResult(_datos.GetValueOrDefault(codigo));
    // ...los otros 4 métodos...
}

// y el servicio NI SE ENTERA:
var servicio = new ServicioProducto(new RepositorioFalsoEnMemoria());
```

### I — Segregación de interfaces
Interfaces pequeñas y específicas: `IRepositorioProducto` tiene SOLO los 5
métodos de datos de producto — no un "IRepositorioDeTodo" que obligue a
implementar métodos que no se usan.

```csharp
// ✅ La interfaz de la v1: SOLO los 5 métodos de datos de producto
public interface IRepositorioProducto
{
    Task<List<Producto>> ObtenerTodosAsync(int limite);
    Task<Producto?> ObtenerPorCodigoAsync(string codigo);
    Task CrearAsync(Producto producto);
    Task<int> ActualizarAsync(string codigo, Dictionary<string, object> datos);
    Task<int> EliminarAsync(string codigo);
}

// ❌ El anti-ejemplo: un "IRepositorioDeTodo" de 40 métodos que obliga a
//    implementar lo que no se usa.
```

### D — Inversión de dependencias
Las capas de arriba dependen de ABSTRACCIONES, no de clases concretas:

```csharp
public ServicioProducto(IRepositorioProducto repositorio)  // ← interfaz, no clase
```

Solo el **ensamblador** (la sección de DI en `Program.cs`) conoce las
clases concretas. Eso es literalmente "invertir" la dependencia: el detalle
(SQL Server) depende del contrato, no al revés.

## 3. Patrones de diseño (los que trabajan en este proyecto)

**¿Qué es un patrón de diseño?** Una solución **con nombre**, probada y
reutilizable, para un problema de diseño que aparece una y otra vez. No es
código para copiar y pegar: es la FORMA de una solución — qué clases y qué
interfaces participan, y quién conoce a quién — que cada proyecto escribe
en su propio código. El catálogo clásico es el del "Gang of Four" (GoF,
1994): 23 patrones en tres familias — **creacionales** (cómo se construyen
los objetos), **estructurales** (cómo se componen) y **de comportamiento**
(cómo colaboran). Otros, como Repositorio y DTO, vienen del catálogo de
arquitectura empresarial de Fowler (PoEAA, 2002).

La relación con lo anterior: **SOLID dice qué cualidades debe tener el
diseño; los patrones son recetas concretas que las consiguen; las capas
son el plano general donde unos y otras viven.** Y el nombre importa:
decir "esto es una fábrica abstracta" comunica un diseño completo en tres
palabras.

Los que ya trabajan en este código:

| Patrón | Familia | Dónde vive aquí |
|---|---|---|
| **Repositorio** (Repository) | arquitectónico (PoEAA) | `Repositorios/`: todo el acceso a datos detrás de una interfaz |
| **Inyección de dependencias** | creacional (IoC) | los constructores + el ensamblador de `Program.cs` |
| **Fábrica abstracta** (Abstract Factory) | creacional (GoF) | `Fabricas/` (v4): la familia completa de repositorios por motor |
| **DTO** — objeto de petición | arquitectónico (PoEAA) | `Peticiones/`: un objeto por verbo que valida la forma del body |
| **Estrategia** (Strategy) | comportamiento (GoF) | implícito: implementaciones intercambiables tras cada interfaz |

### Repositorio — el negocio pide datos a un contrato, no a un motor

```csharp
// El contrato (Repositorios/IRepositorioProducto.cs):
Task<Producto?> ObtenerPorCodigoAsync(string codigo);

// ServicioProducto lo usa SIN saber si detrás hay SQL Server, PostgreSQL
// o un diccionario en memoria (las pruebas). Por eso la v4 pudo cambiar
// de motor sin tocarlo.
```

### Inyección de dependencias — nadie hace `new` de lo que necesita

```csharp
// La dependencia LLEGA por el constructor (una interfaz, no una clase):
public ServicioProducto(IRepositorioProducto repositorio) { … }

// y el ÚNICO que sabe qué entregar es el ensamblador (Program.cs):
builder.Services.AddScoped<IServicioProducto, ServicioProducto>();
```

### Fábrica abstracta — UNA decisión, la familia completa (v4)

```csharp
// Un punto del código decide el motor…
IFabricaRepositorios fabrica = motor switch
{
    "sqlserver" => new FabricaSqlServer(cadenaSqlServer),
    "postgres"  => new FabricaPostgres(cadenaPostgres),
    _ => throw new InvalidOperationException($"Motor desconocido: '{motor}'."),
};
// …y la fábrica entrega los 11 repositorios COHERENTES entre sí:
builder.Services.AddScoped<IRepositorioProducto>(_ => fabrica.CrearRepositorioProducto());
// Agregar MariaDB (v5) costará UNA clase y UN case — eso compra el patrón.
```

### DTO por verbo — el body aterriza en un objeto que solo valida forma

```csharp
public class ProductoCrear
{
    [Required(ErrorMessage = "El campo codigo es obligatorio.")]
    public string? Codigo { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int? Stock { get; set; }
    // Si no cumple → 422 con errores[] — y el modelo de dominio ni se entera.
}
```

### Estrategia — el patrón que va de regalo

La pareja "interfaz + implementaciones intercambiables"
(`RepositorioProductoSqlServer` / `…Postgres` / el falso de las pruebas)
es la esencia de Strategy. Aquí la elección no cambia en caliente: se hace
UNA vez al arrancar (el switch de la fábrica) — pero el mecanismo es el
mismo: quien usa la interfaz jamás pregunta cuál implementación le tocó.

## 4. El mapa SOLID ↔ versiones del curso

| Principio | Se ve desde | Se termina de demostrar en |
|---|---|---|
| S | v1 (una clase por responsabilidad) | v2 (más entidades, mismas responsabilidades) |
| O | v1 (la interfaz existe) | **v4** (segundo motor sin tocar lo construido — cumplido) |
| L | v1 (el repositorio falso de las pruebas) | **v4** (motores intercambiables de verdad — cumplido) |
| I | v1 (interfaces mínimas) | v3 (los puentes: contratos sin verbos que no aplican) |
| D | v1 (constructores reciben interfaces) | **v4** (la fábrica reemplazó al ensamblador simple — cumplido) |

## 5. Referencias

1. Martin, R. — *Design Principles and Design Patterns* (el texto original
   de SOLID).
2. Gamma, Helm, Johnson y Vlissides — *Design Patterns* (GoF, 1994): el
   catálogo original de los 23 patrones.
3. Fowler, M. — *Patterns of Enterprise Application Architecture* (PoEAA,
   2002): Repositorio, DTO y compañía.
4. Microsoft — Inyección de dependencias en .NET:
   <https://learn.microsoft.com/dotnet/core/extensions/dependency-injection>
5. En este repositorio: [PARADIGMA_POO.md](PARADIGMA_POO.md) (los pilares
   sobre los que SOLID se apoya) y el spec kit de la versión en curso.
