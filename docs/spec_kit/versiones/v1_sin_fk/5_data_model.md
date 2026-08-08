# Modelo de datos — Versión 1: la BD completa (dada) y las SEIS tablas sin FK

> **Versión 1** · La base de datos NO se diseña en esta versión: **viene
> dada** ([4_research.md](4_research.md) D4). Este documento describe lo que
> hay y lo único que la v1 puede tocar.

---

## 1. La base de datos `bdfacturas` (dada, completa)

El script **provisto** `db/bdfacturas.sql` (dialecto SQL Server) crea la
base `bdfacturas_sqlserver_local` completa. SQL Server lo ejecuta SOLO la
PRIMERA vez (está montado en `/docker-entrypoint-initdb.d/` y corre
cuando el volumen de datos nace vacío).

**12 tablas** en dos módulos:

```
FACTURACIÓN                          SEGURIDAD (RBAC)
persona ←── cliente ←── factura      usuario ──┐
   ↑          ↑            ↑                   ├── rol_usuario ── rol
empresa ──────┘       productosporfactura      │                   │
vendedor ── persona        ↑                   ruta ── rutarol ────┘
                        producto
```

Además: **triggers** que mantienen `factura.total` y `producto.stock` al
insertar/editar/borrar renglones del detalle, y **procedimientos
almacenados** de consulta — todos esperando a las versiones siguientes.

**Datos de ejemplo:** 8 productos (PR001…PR008), 6 personas, 6 facturas con
detalle, usuarios y roles. Credenciales de BD (didácticas): `sa` /
`Paradigmas123!`.

## 2. Lo ÚNICO que la v1 puede nombrar: la tabla `producto`

| Columna | Tipo (SQL Server) | Regla |
|---|---|---|
| `codigo` | `VARCHAR(10)` | **PK** — texto de 1 a 10 caracteres |
| `nombre` | `VARCHAR(100)` | No nulo, no vacío |
| `stock` | `INT` | No nulo, ≥ 0 (regla de la API) |
| `valorunitario` | `DECIMAL(18,2)` | No nulo, ≥ 0 (regla de la API) |

En C#, esa fila viaja como el modelo entidad:

```csharp
public class Producto
{
    public required string Codigo { get; set; }
    public required string Nombre { get; set; }
    public int Stock { get; set; }
    public decimal Valorunitario { get; set; }   // decimal = el tipo para dinero
}
```

## 3. Las dos murallas de validación

1. **La API** (las peticiones por verbo con anotaciones): forma, tipos y
   rangos → 422 con lista de errores, ANTES de tocar la BD.
2. **La BD** (PK, NOT NULL, FK y triggers): la última línea de defensa —
   un código duplicado viola la PK y el motor lo rechaza aunque la API
   tuviera un bug (la API lo reporta como 500 con el error del motor).

## 4. Reglas de esta versión

- El código de la v1 **solo puede nombrar las SEIS tablas sin clave
  foránea** —`producto`, `empresa`, `persona`, `rol`, `ruta`, `usuario`—. Las
  otras seis existen en la base pero son territorio de la v2.
- **`usuario` y `rol` SÍ son de esta versión**, aunque sean del control de
  acceso: el criterio es no tener clave foránea, y no la tienen. Lo que llega
  en la v3 **no es su CRUD** —ese es este— sino la sesión y el permiso.
- La BD **no se modifica**: ni columnas nuevas, ni índices, ni datos
  semilla distintos. Si algo parece faltar, es de otra versión.
- El reset completo es de Docker, no de SQL:
  `docker compose down -v && docker compose up -d` (borra el volumen y el
  inicializador vuelve a crear todo).

---

## Lo que el motor cambia, y se descubrió EJECUTANDO

> Las seis diferencias de abajo no salieron de leer documentación: salieron de
> correr el mismo código contra los dos motores y mirar qué respondía distinto.
> Están aquí porque la v5 —el segundo motor— vive de que estén escritas.

| | En SQL Server (este proyecto) | En PostgreSQL (la v5) |
|---|---|---|
| **La llave autoincremental** | `INT IDENTITY(1,1)` | `SERIAL` |
| **El valor recién insertado** | `SCOPE_IDENTITY()`, en un `SELECT` aparte | `RETURNING`, en el mismo `INSERT` |
| **La clave foránea violada** | error **547** | `SQLSTATE` **23503** |
| **El duplicado** | **2627** (llave primaria) y **2601** (índice único) — **son dos** | `SQLSTATE` **23505**, uno solo |
| **Concatenar texto agrupado** | `STRING_AGG(...)` **sin `DISTINCT`**: T-SQL no lo acepta | `STRING_AGG(DISTINCT ...)` sí |
| **El tipo de `COUNT()` y `SUM(int)`** | `int` | `bigint` |

> **El 2601 es el que se olvida.** Quien venga de PostgreSQL traduce el 23505
> a 2627 y se queda tranquilo: la llave primaria duplicada responde 409. Pero
> un **índice único** violado —dos usuarios con el mismo correo— levanta
> **2601**, que no estaba en la traducción, y entonces el 409 se vuelve un
> **500**. Funciona en la prueba obvia y falla en la otra.

> **Y el tipo de `COUNT()` muerde al revés.** En PostgreSQL devuelve `bigint`,
> así que un modelo con `int` revienta al deserializar — ruidoso, se arregla
> en diez minutos. En SQL Server devuelve `int`, y entonces un
> `SUM(total) / COUNT(*)` con `total` decimal **trunca** el promedio sin
> quejarse. El primero se cae; el segundo da un número equivocado. El segundo
> es peor.
