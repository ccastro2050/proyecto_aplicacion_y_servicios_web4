# Modelo de datos — Versión 4: la MISMA bdfacturas, en PostgreSQL

> La v4 no agrega ni una tabla ni una columna: agrega un DIALECTO.
> `db/bdfacturas_postgres.sql` crea en PostgreSQL la misma base que
> `db/bdfacturas.sql` crea en SQL Server: 12 tablas, el trigger de
> totales/stock, los SPs de factura y las mismas semillas (mismos ids).
> La BD se llama `bdfacturas_postgres_local`.

---

## 1. Equivalencias de dialecto (lo que cambia al portar el DDL)

| Concepto | SQL Server | PostgreSQL |
|---|---|---|
| Autonumérico | `INT IDENTITY(1,1)` | `SERIAL` (secuencia `tabla_col_seq`) |
| Texto | `NVARCHAR(n)` | `VARCHAR(n)` (UTF-8 nativo) |
| Decimal | `DECIMAL(...)` | `NUMERIC` |
| Fecha-hora | `DATETIME2` + `GETDATE()` | `TIMESTAMP` + `CURRENT_TIMESTAMP` |
| Insertar ids explícitos | `SET IDENTITY_INSERT t ON` | insertar y luego `setval('t_col_seq', MAX(id))` |
| Error de negocio | `THROW 5000x, 'mensaje', 1` | `RAISE EXCEPTION 'mensaje'` (SQLSTATE `P0001`) |
| Abrir JSON de entrada | `OPENJSON(@json)` | `json_array_elements(p_json)` |
| Armar JSON de salida | `FOR JSON PATH` | `json_build_object` / `json_agg` / `row_to_json` |
| SP con salida | `@p_resultado NVARCHAR(MAX) OUTPUT` | `INOUT p_resultado JSON` (el `CALL` la devuelve como fila) |
| Top-N | `SELECT TOP (@n)` | `LIMIT @n` |

Las 12 tablas, sus PKs, FKs, el `UNIQUE(ruta)`, el default de `credito`,
el `ON DELETE CASCADE` de `productosporfactura` — **idénticos** en
estructura y nombre. Los modelos C# no notan la diferencia.

## 2. El trigger y los SPs (los mismos actores, otro acento)

- **`actualizar_totales_y_stock()`** (función + trigger `BEFORE
  INSERT/UPDATE/DELETE` sobre `productosporfactura`): valida stock
  suficiente, calcula `subtotal`, descuenta/restaura `stock` y recalcula
  el `total` — exactamente el papel del trigger de SQL Server.
- **Los 6 SPs de factura** conservan nombre y semántica:
  `sp_insertar_factura_y_productosporfactura` (mínimo de renglones,
  inserta cabecera + detalle, el trigger hace las cuentas),
  `sp_consultar…` y `sp_listar…` (nombres de cliente/vendedor resueltos,
  detalle adentro), `sp_actualizar…`, `sp_borrar…` y **`sp_anular_factura`**
  ("no existe" y "ya está anulada" como errores — los que la API traduce
  a 404/409).
- **Los SPs de usuarios/roles/permisos** (`crear_usuario_con_roles`,
  `verificar_acceso_ruta`, `listar_rutarol`, …) también viajan en el
  script: la v4 no los llama, pero mantienen la BD en paridad con el
  gemelo de Python — y son el terreno que pisará la API genérica (v6).

## 3. Los mensajes que la API traduce (paridad de negocio)

| Situación | SQL Server (v2/v3) | PostgreSQL (v4) | API |
|---|---|---|---|
| Consultar factura inexistente | THROW 50003 `…no existe` | `Factura N no existe` | **404** |
| Anular factura inexistente | THROW 50010 `…no existe` | `Factura N no existe` | **404** |
| Anular factura ya anulada | THROW 50010 `…ya está anulada` | `Factura N ya está anulada` | **409** |
| Stock insuficiente | THROW 50001 (trigger) | `Stock insuficiente para producto…` (trigger) | **500** |
| Mínimo de renglones | THROW 50002 | `La factura requiere minimo…` | (no llega: la petición lo corta en 422) |
| FK / PK / UNIQUE violadas | error del motor | SQLSTATE 23503/23505 | **500** |

## 4. Semillas (idénticas a SQL Server — RNF3)

| Tabla | Filas | Igual que en SQL Server |
|---|---|---|
| producto | 8 | PR001 stock 17 … PR008 |
| persona | 6 | P001 Ana Torres … P006 |
| empresa | 3 | E001, E002, E999 |
| cliente | 4 | ids **1, 2, 3, 5** (el hueco del 4 incluido) |
| vendedor | 3 | ids 1-3, carnets 1001-1003 |
| factura | 6 | numeros 1-6 con su detalle (12 renglones) |
| rol | 5 | Administrador … Cliente |
| ruta | 15 | /home … /ruta/eliminar (UNIQUE) |
| usuario | 8 | 2 hash costo 12, 2 TEXTO PLANO (la lección), 4 hash 10/11 |
| rol_usuario | 21 | las mismas parejas |
| rutarol | 25 | las mismas parejas |

Tras cada bloque con ids explícitos, `setval` deja la secuencia en el
MAX — así el próximo insert da el mismo id que daría SQL Server (cliente
nuevo → 6, vendedor nuevo → 4, factura nueva → 7).
