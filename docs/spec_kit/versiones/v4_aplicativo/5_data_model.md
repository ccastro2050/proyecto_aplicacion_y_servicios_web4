# Modelo de datos — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la capa de consultas y el tablero |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | **5_data_model.md** (este) | La MISMA bdfacturas: cero tablas nuevas |
> | [6_contracts.md](6_contracts.md) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

> **La v4 no agrega ni una tabla, ni una columna, ni un índice.** Agrega
> **preguntas** sobre lo que ya hay. Este documento existe para decir eso y para
> mostrar qué parte del modelo toca cada una.

## 1. Lo que ya existía, y basta

Las **doce tablas** de la v1 y la v2, con sus claves foráneas. El cruce de las
diez consultas es posible **porque esas claves están**: sin `fkidcliente` en
`factura` no hay forma de saber quién compró.

> **Es el pago de la v2.** Allá las claves foráneas se veían como una molestia
> —desplegables, 409 cuando no existe el padre—. Aquí se cobran.

## 2. Qué toca cada consulta

| # | Tablas | La columna que hace el cruce posible |
|---|---|---|
| 1 | `producto` → `productosporfactura` → `factura` → `cliente` | `productosporfactura.fkcodproducto` |
| 2 | `persona` → `cliente` → `factura` → `productosporfactura` | `cliente.fkidpersona` · `factura.fkidcliente` |
| 3 | `persona` → `vendedor` → `factura` → `productosporfactura` | `vendedor.fkidpersona` · `factura.fkidvendedor` |
| 4 | `empresa` → `cliente` → `factura` → `productosporfactura` | `cliente.fkcodempresa` |
| 5 | `persona` → `vendedor` → `factura` → `productosporfactura` | las mismas de la 3, con el promedio |
| 6 | `producto` ⟕ `productosporfactura` → `factura` → `cliente` | el `LEFT JOIN`: lo que **no** está |
| 7 | `persona` → `cliente` → `factura` → `productosporfactura` | `factura.estado = 'ANULADA'` |
| 8 | `usuario` → `rol_usuario` → `rol` → `rutarol` → `ruta` | **la cadena completa del control de acceso** |
| 9 | `ruta` ⟕ `rutarol` → `rol` → `rol_usuario` | el `LEFT JOIN` otra vez |
| 10 | `persona` → `empresa` → `cliente` → `factura` | `cliente.credito` contra `SUM(factura.total)` |

> **La 8 recorre la cadena del control de acceso de punta a punta.** Es la misma
> que `verificar_acceso_ruta` camina en cada petición de la v3 — ahora se ve
> entera, de una.

## 3. Lo que el dialecto cambia en estas diez

| | SQL Server (este proyecto) | El otro motor (v5) |
|---|---|---|
| **Concatenar texto agrupado** | `STRING_AGG(…)` **sin `DISTINCT`**: T-SQL no lo acepta | `STRING_AGG(DISTINCT …)` sí funciona |
| **El tipo de `COUNT()`** | `int` — un promedio decimal/int **trunca** en silencio | `bigint` — el modelo con `int` revienta, ruidosamente |

> Las dos están resueltas **en el SQL**, con `STRING_AGG` sin `DISTINCT` y con
> `CAST(… AS INT)`, para que los dos motores devuelvan **exactamente la misma
> forma**. Es lo que permite que la v5 no toque ni un modelo.
