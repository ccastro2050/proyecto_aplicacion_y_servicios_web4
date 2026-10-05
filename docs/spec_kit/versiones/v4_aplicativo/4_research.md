# Investigación — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la capa de consultas y el tablero |
> | **4_research.md** (este) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas: cero tablas nuevas |
> | [6_contracts.md](6_contracts.md) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

## D1 — ¿Vista de base de datos, procedimiento almacenado, o SQL en el repositorio?

| Opción | Argumento |
|---|---|
| **Una vista por consulta** | El cruce queda en la base de datos, versionado con el esquema. Y **diez vistas más** que mantener en los dos dialectos de la v5 |
| **Un procedimiento por consulta** | Igual, y encima esconde el SQL justo en la versión que existe para mostrarlo |
| **SQL en el repositorio** ✅ | Se lee al lado del código que lo usa, y el dialecto queda confinado a **un archivo por motor** |

> La constitución exige **el SQL a la vista**. Diez vistas lo esconderían en
> otro lado, y la v5 tendría que crearlas dos veces.

## D2 — ¿Un endpoint con un parámetro, o diez endpoints?

Lo corto sería `GET /api/consultas?nombre=ventas-por-producto`.

**Decisión: diez endpoints con nombre.** Es la misma regla que gobierna todo el
proyecto —**APIs específicas, no genéricas**— y aquí la razón se ve mejor que
en el CRUD: **cada consulta devuelve una forma distinta**. Con un parámetro, el
contrato tendría que decir «depende», y un contrato que dice «depende» no es un
contrato.

> Y en Swagger se ven **diez operaciones con su esquema de respuesta**, en vez
> de una con un campo de texto libre.

## D3 — ¿Por qué `LEFT JOIN` en dos de ellas?

Las consultas 6 y 9 preguntan por **ausencias**: productos que *no* se
vendieron, interfaces a las que *no* llega nadie. Un `INNER JOIN` solo trae lo
que existe en las dos tablas, así que **no puede responder una ausencia**.

> **Y por eso pueden devolver cero filas sin que nada esté mal.** La interfaz lo
> dice con palabras —«cero filas, y eso es una respuesta»— porque una tabla
> vacía sin explicación se lee como una falla.

## D4 — El tipo de los agregados, que es donde se perdió una tarde

| | `COUNT()` / `SUM(int)` | La consecuencia |
|---|---|---|
| **PostgreSQL** | `bigint` | El modelo con `int` revienta: *«A parameterless default constructor or one matching signature … System.Int64 unidades»* |
| **SQL Server** | `int` | `SUM(decimal) / COUNT(*)` **trunca** el promedio |

**Decisión: `CAST(… AS INT)` en el SQL.**

| Alternativa | Por qué no |
|---|---|
| Cambiar los modelos a `long` | Resuelve PostgreSQL y deja el truncamiento de T-SQL intacto. Y ata el modelo —que es del contrato— a un detalle del motor |
| Convertir en el repositorio, en C# | Dos conversiones distintas en dos archivos, y la del motor que falle se descubre en producción |
| **`CAST` en el SQL** ✅ | Es estándar, va en el único archivo que ya es específico del motor, y deja las dos respuestas **idénticas** |

## D5 — ¿Librería de gráficos?

**No.** La razón está en [3_plan.md](3_plan.md) §3.3, y lo que importa aquí es
lo que se renunció: con una librería se tendrían *tooltips*, animaciones y ejes
calculados. Se cambiaron por **ver exactamente lo que el navegador pinta** y
por **funcionar sin red**.

## D6 — Lo que NO se investigó, y por qué

| | |
|---|---|
| **Caché** | Ver [3_plan.md](3_plan.md) §4: el problema todavía no existe, y un número viejo es peor que una espera |
| **Un motor de informes** (SSRS, Metabase) | Resuelve el problema entero y **esconde exactamente lo que esta versión existe para enseñar** |
| **OLAP / cubos** | La escala del curso no lo pide, y traería un vocabulario nuevo sin necesidad |
