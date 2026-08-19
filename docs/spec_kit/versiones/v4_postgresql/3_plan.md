# Plan — Versión 4: el segundo motor (PostgreSQL) y la fábrica

> Cómo se construye lo especificado en [2_spec.md](2_spec.md). El stack es
> el mismo de siempre (C#/ASP.NET Core, ADO.NET, sin ORM); lo nuevo es el
> proveedor **Npgsql** y el patrón **fábrica abstracta**.

---

## 1. Inventario de archivos

**Nuevos (15 de código + 1 de BD):**

```
api_facturas/Fabricas/IFabricaRepositorios.cs      ← la interfaz (11 métodos CrearRepositorioX)
api_facturas/Fabricas/FabricaSqlServer.cs          ← entrega los 11 *SqlServer
api_facturas/Fabricas/FabricaPostgres.cs           ← entrega los 11 *Postgres
api_facturas/Repositorios/Repositorio{Producto,Persona,Factura,Empresa,
    Cliente,Vendedor,Usuario,Rol,Ruta,RolUsuario,RutaRol}Postgres.cs   (11)
db/bdfacturas_postgres.sql                         ← la MISMA BD, dialecto PostgreSQL
```

**Crecen (los únicos existentes que se tocan):**

| Archivo | Qué crece |
|---|---|
| `ApiFacturas.csproj` | ★ paquete **Npgsql** |
| `docker-compose.yml` | ★ servicio `postgres` (16-alpine, puerto 15462, healthcheck) + variables `Motor` y `ConnectionStrings__Postgres` en la API |
| `appsettings.json` | ★ cadena `Postgres` y clave `Motor` (defaults para correr sin Docker) |
| `Program.cs` | ★ el ensamblador se REESCRIBE alrededor de la fábrica (ver §4) + diagnóstico v4 con `motor` |
| `pruebas/Programa.cs` | ★ criterio 5: las fábricas eligen sin conectarse |

**Intocables (RNF2):** Controllers/, Servicios/, Peticiones/, Modelos/,
Excepciones/. Ese es el punto de la versión.

## 2. Los 10 repositorios "calcados" (todos menos factura)

La traducción SqlServer → Postgres es **mecánica** — la tabla completa:

| SQL Server (v1–v3) | PostgreSQL (v4) |
|---|---|
| `using Microsoft.Data.SqlClient` | `using Npgsql` |
| `SqlConnection` / `SqlCommand` / `SqlDataReader` | `NpgsqlConnection` / `NpgsqlCommand` / `NpgsqlDataReader` |
| `SqlParameterCollection` | `NpgsqlParameterCollection` |
| `SELECT TOP (@limite) …` | `SELECT … LIMIT @limite` (va al FINAL) |
| Todo lo demás (parámetros `@`, async, `await using`, DBNull del cliente, el SET dinámico del PATCH) | **idéntico** |

BCrypt no se entera del cambio: `RepositorioUsuarioPostgres` hashea y
verifica EXACTAMENTE igual (el hash es del repositorio, no del motor —
RNF2 de la v3).

## 3. El repositorio de factura Postgres (el único con diseño propio)

Los SPs de PostgreSQL son `PROCEDURE` con parámetro `INOUT p_resultado
JSON` (paridad con el proyecto gemelo de Python). Diferencias frente al
dialecto SQL Server:

| Aspecto | SQL Server | PostgreSQL |
|---|---|---|
| Invocación | `CommandType.StoredProcedure` | texto `CALL sp_x(@p1, …, NULL)` |
| El JSON de salida | parámetro OUTPUT `@p_resultado` | el `CALL` devuelve UNA fila con los INOUT → `ExecuteScalarAsync()` |
| El detalle JSON de entrada | `NVARCHAR` que el SP abre con OPENJSON | `@p_productos::json` (cast en el texto del CALL) |
| Errores de negocio | `THROW 50003/50010` + número | `RAISE EXCEPTION` → SQLSTATE **`P0001`** + patrón del mensaje |

La traducción de errores (mismas excepciones de negocio de siempre):

```csharp
catch (PostgresException e) when (e.SqlState == "P0001"
                                  && e.MessageText.Contains("no existe"))
{
    throw new NoEncontradoExcepcion(e.MessageText);      // → 404
}
catch (PostgresException e) when (e.SqlState == "P0001"
                                  && e.MessageText.Contains("anulada"))
{
    throw new ConflictoExcepcion(e.MessageText);         // → 409
}
// Stock insuficiente, mínimo de renglones, FK, PK, UNIQUE → suben → 500.
```

## 4. El ensamblador con fábrica (Program.cs)

La lista de la v3 ("este dolor es el argumento de la v4") se cura así:

```csharp
// UN punto del código decide el motor:
var motor = builder.Configuration["Motor"] ?? "sqlserver";
IFabricaRepositorios fabrica = motor switch
{
    "sqlserver" => new FabricaSqlServer(cadenaSqlServer),
    "postgres"  => new FabricaPostgres(cadenaPostgres),
    _ => throw new InvalidOperationException(
             $"Motor desconocido: '{motor}' (use sqlserver o postgres)."),
};

// Las 11 rebanadas, ahora CIEGAS al motor:
builder.Services.AddScoped<IRepositorioProducto>(_ => fabrica.CrearRepositorioProducto());
builder.Services.AddScoped<IServicioProducto, ServicioProducto>();
// … (mismo par para las otras 10 rebanadas)
```

La cuenta didáctica: agregar MariaDB en la v5 costará **una clase**
(`FabricaMariaDb`) **y un case** — no 11 registros nuevos. Eso compra la
fábrica.

## 5. El compose con dos motores

- Servicio `postgres` (imagen `postgres:16-alpine`): PostgreSQL SÍ ejecuta
  automáticamente los scripts montados en `/docker-entrypoint-initdb.d/`
  la primera vez — **no necesita contenedor inicializador** (contraste
  didáctico con `sqlserver-init`).
- Puerto publicado **15462** (reservado del curso; la reconstrucción del
  estudiante usa 15562).
- Healthcheck con `pg_isready`; la API además espera a `postgres` sano.
- El interruptor: `Motor: ${MOTOR_BD:-postgres}` — variable de compose con
  default. `MOTOR_BD=sqlserver docker compose up -d api-facturas` recrea
  SOLO la API apuntando al otro motor (los DOS motores siempre están
  arriba; lo que cambia es a cuál le habla la API).

## 6. La prueba de capas crece (criterio 5)

Las fábricas se pueden probar SIN base de datos: construir un repositorio
no abre conexiones. El proyecto `pruebas/` verifica que cada fábrica
entrega instancias del dialecto correcto (`is RepositorioProductoPostgres`,
etc.) con cadenas de conexión de mentira.
