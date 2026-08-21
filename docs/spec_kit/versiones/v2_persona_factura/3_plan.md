# Plan técnico — Versión 2: persona y factura (C#/ASP.NET Core + SQL Server)

> **Nota (agosto de 2026):** el curso adoptó **Dapper** como
> micro-ejecutor en TODOS los repositorios: el SQL sigue escrito a mano
> y parametrizado; cambió el mapeo (`QueryAsync`/`ExecuteAsync` en vez
> del ciclo DataReader) y los SPs se llaman con `DynamicParameters`.
> Las tablas de "calco" entre dialectos siguen valiendo para los
> PROVEEDORES (Npgsql/SqlClient/MySqlConnector) que Dapper usa por debajo.


> **Versión 2** · CÓMO construir lo especificado en [2_spec.md](2_spec.md).
> El porqué de cada decisión: [4_research.md](4_research.md) · contratos
> exactos: [6_contracts.md](6_contracts.md) · orden: [8_tasks.md](8_tasks.md).
> El stack NO cambia (es el de la [v1](../v1_producto_sqlserver/3_plan.md) §1:
> .NET 10 + ADO.NET + SQL Server, sin ORM).

---

## 1. Qué archivos se AGREGAN (la v1 no se toca, salvo Program.cs)

```
api_facturas/
├── Program.cs                        ★ CRECE: 4 AddScoped nuevos + version "v2"
├── Modelos/
│   ├── Persona.cs                    NUEVO: la entidad (4 propiedades string)
│   ├── Factura.cs                    NUEVO: el maestro que devuelven los SPs
│   └── ProductoDeFactura.cs          NUEVO: un renglón del detalle
├── Peticiones/
│   ├── PersonaCrear.cs               NUEVO: POST persona (todo obligatorio)
│   ├── PersonaReemplazo.cs           NUEVO: PUT persona (todo obligatorio, sin código)
│   ├── PersonaActualizar.cs          NUEVO: PATCH persona (todo opcional)
│   └── FacturaCrear.cs               NUEVO: POST factura (+ ProductoDeFacturaCrear anidada)
├── Controllers/
│   ├── PersonaController.cs          NUEVO: calcado de ProductoController
│   └── FacturaController.cs          NUEVO: 4 endpoints (2 GET, POST, POST anular)
├── Servicios/
│   ├── IServicioPersona.cs · ServicioPersona.cs        NUEVOS (calcados)
│   └── IServicioFactura.cs · ServicioFactura.cs        NUEVOS
├── Repositorios/
│   ├── IRepositorioPersona.cs · RepositorioPersonaSqlServer.cs   NUEVOS (calcados)
│   └── IRepositorioFactura.cs · RepositorioFacturaSqlServer.cs   NUEVOS (solo SPs)
├── Excepciones/
│   └── ConflictoExcepcion.cs         NUEVO: el 409 (ya está anulada)
└── pruebas/
    └── Programa.cs                   ★ CRECE: repo falso de persona + sus verificaciones
```

Todo lo demás (docker-compose, Dockerfile, db/, la rebanada de producto)
queda **idéntico a la v1**.

## 2. Rebanada 1: persona — calcar, no diseñar

Regla de oro: **abrir el archivo gemelo de producto y replicarlo** cambiando
entidad, campos y textos. Nada más.

| Pieza | Se calca de | Cambios |
|---|---|---|
| `Persona.cs` | `Producto.cs` | 4 propiedades: `Codigo`, `Nombre`, `Email`, `Telefono` (todas `string`) |
| `PersonaCrear/Reemplazo/Actualizar` | las 3 de producto | `[Required]` + `[StringLength]`: codigo 1–10 · nombre ≤100 · email ≤100 · telefono ≤20 (sin `[Range]`: no hay números) |
| `IRepositorioPersona` / `RepositorioPersonaSqlServer` | los de producto | tabla `persona`, mismos 5 métodos, mismos SQL con `@parametros` |
| `IServicioPersona` / `ServicioPersona` | los de producto | mismas reglas (límite > 0, código no vacío, PATCH vacío → `ArgumentException`) |
| `PersonaController` | `ProductoController` | `[Route("api/persona")]`, mismos 6 métodos y try/catch |

## 3. Rebanada 2: factura — la API como traductora de SPs

### 3.1 Los modelos de lectura (lo que los SPs devuelven)

Los SPs retornan **JSON por el parámetro `@p_resultado OUTPUT`**. El
repositorio lo deserializa con `System.Text.Json` a modelos tipados
(claves del JSON en snake_case → `[JsonPropertyName]`):

```csharp
public class Factura {
    public int Numero; public string Fecha; public decimal Total;
    public string Estado; public int Fkidcliente;
    [JsonPropertyName("nombre_cliente")] public string NombreCliente;
    public int Fkidvendedor;
    [JsonPropertyName("nombre_vendedor")] public string NombreVendedor;
    public List<ProductoDeFactura> Productos;   // el detalle anidado
}
public class ProductoDeFactura {
    [JsonPropertyName("codigo_producto")] public string CodigoProducto;
    [JsonPropertyName("nombre_producto")] public string NombreProducto;
    public int Cantidad; public decimal Valorunitario; public decimal Subtotal;
}
```
(Como propiedades `{ get; set; }` — abreviadas aquí. `PropertyNameCaseInsensitive`
cubre las claves simples.)

### 3.2 La petición del POST (validación anidada)

```csharp
public class FacturaCrear {
    [Required] public int? Fkidcliente { get; set; }
    [Required] public int? Fkidvendedor { get; set; }
    [Required, MinLength(1, ErrorMessage = "La factura requiere mínimo 1 producto.")]
    public List<ProductoDeFacturaCrear>? Productos { get; set; }
}
public class ProductoDeFacturaCrear {
    [Required, StringLength(10, MinimumLength = 1)] public string? Codigo { get; set; }
    [Required, Range(1, int.MaxValue)] public int? Cantidad { get; set; }
}
```
ASP.NET valida la lista Y cada elemento → lista vacía o cantidad 0 mueren
en **422** antes del controlador. (El SP tiene su propio mínimo con
`THROW 50002` — queda como respaldo, no como el camino normal.)

### 3.3 El repositorio de factura: CommandType.StoredProcedure

`IRepositorioFactura` — 4 métodos, nada de SQL de tablas:

```csharp
Task<List<Factura>> ListarAsync();            // sp_listar_facturas_y_productosporfactura
Task<Factura> ConsultarAsync(int numero);     // sp_consultar (el sobre {factura, productos} se APLANA a una Factura)
Task<Factura> CrearAsync(int fkidcliente, int fkidvendedor, string productosJson);
Task<string> AnularAsync(int numero);         // sp_anular_factura — su JSON se re-emite tal cual
```

Patrón de llamada (igual en los 4):
```csharp
await using var conexion = new SqlConnection(_cadena);
await using var comando = new SqlCommand("sp_consultar_factura_y_productosporfactura", conexion)
    { CommandType = CommandType.StoredProcedure };
comando.Parameters.AddWithValue("@p_numero", numero);
var salida = new SqlParameter("@p_resultado", SqlDbType.NVarChar, -1)
    { Direction = ParameterDirection.Output };          // -1 = NVARCHAR(MAX)
comando.Parameters.Add(salida);
await conexion.OpenAsync();
await comando.ExecuteNonQueryAsync();
return JsonSerializer.Deserialize<…>((string)salida.Value, _opcionesJson)!;
```
Para `CrearAsync`, `@p_productos` recibe el JSON de los renglones
(`[{"codigo":"PR001","cantidad":2}]`) — el servicio lo serializa desde la
petición ya validada.

### 3.4 Los THROW del SP se traducen a excepciones de negocio

Números reales de `db/bdfacturas.sql`: **50003** = el "no existe" de
`sp_consultar` · **50010** = los dos errores de `sp_anular`:

```csharp
catch (SqlException ex) when ((ex.Number == 50003 || ex.Number == 50010)
                              && ex.Message.Contains("no existe"))
    { throw new NoEncontradoExcepcion(ex.Message); }        // → 404
catch (SqlException ex) when (ex.Number == 50010 && ex.Message.Contains("anulada"))
    { throw new ConflictoExcepcion(ex.Message); }           // → 409
// Cualquier otro SqlException (stock insuficiente del trigger, FK) sube tal
// cual y el controller lo vuelve 500 con el mensaje en `detalle`.
```
La traducción vive en el REPOSITORIO: los números de error del motor son un
detalle de datos, no de negocio ni de HTTP.

### 3.5 Servicio y controller de factura

`ServicioFactura`: valida `numero > 0` y **no calcula nada** (RNF2). El
JSON de renglones para el SP lo arma el **controller** desde la petición ya
validada (lista blanca `codigo`/`cantidad`) — así el servicio no conoce las
clases de `Peticiones/`, igual que en el molde de la v1.
`FacturaController` (`[Route("api/factura")]`): 4 métodos con el try/catch
de siempre + una fila nueva en la tabla de traducción:
`ConflictoExcepcion → 409`.

## 4. Program.cs: el ensamblador crece (y nada más)

```csharp
builder.Services.AddScoped<IRepositorioPersona>(_ => new RepositorioPersonaSqlServer(cadena));
builder.Services.AddScoped<IServicioPersona, ServicioPersona>();
builder.Services.AddScoped<IRepositorioFactura>(_ => new RepositorioFacturaSqlServer(cadena));
builder.Services.AddScoped<IServicioFactura, ServicioFactura>();
```
Y el diagnóstico pasa a `"version": "v2"`. El 422 personalizado, Swagger y
`MapControllers` quedan como están — los controllers nuevos se registran
solos por sus atributos.

## 5. Docker

Sin cambios: mismos 3 servicios, mismos puertos (API 8032 · SQL Server
11463 al host). `dotnet watch` recompila al agregar los archivos nuevos.
