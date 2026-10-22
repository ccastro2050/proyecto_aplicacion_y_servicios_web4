# El encabezado común de los ocho documentos de una versión

> **Esto no es un documento del spec kit.** Es el bloque que va **arriba de
> todo** en `2_spec`, `3_plan`, `4_research`, `5_data_model`, `6_contracts`,
> `7_quickstart`, `8_tasks` y `9_checklist` — idéntico en los ocho, salvo las
> dos cosas que se marcan abajo.
>
> Está aquí una sola vez para no repetirlo ocho veces en las plantillas.

---

## EL ORDEN: primero el título, después el bloque

```markdown
# <Tipo de documento> — Versión N: <título de la versión>

> **Versión N** del desarrollo incremental …
```

> **El `#` del título va PRIMERO, y el bloque de cita debajo.** Al revés el
> documento abre con una cita suelta y el título aparece a media página.

---

## El bloque, para la v2 en adelante

```markdown
> **Versión {{N}}** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v{{N-1}} — las {{OPERACIONES_ANTERIORES}} operaciones
> existentes no se tocan y sus contratos siguen vigentes tal cual. La v{{N}}
> **suma {{OPERACIONES_NUEVAS}}**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v{{N}} y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: {{EN_UNA_LINEA_QUE_SE_CONSTRUYE}} |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | {{QUE_PASA_CON_EL_MODELO_EN_ESTA_VERSION}} |
> | [6_contracts.md](6_contracts.md) | {{LAS_OPERACIONES_QUE_AGREGA}} |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de {{LO_NUEVO}} |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA{{N}}.md](GUIA_IA{{N}}.md) | Construirla con IA, sobre su proyecto v{{N-1}} |
>
> ### De dónde sale este documento
>
> | Documento de dominio | Qué aporta |
> |---|---|
> | [`{{DOC_DOMINIO_1}}`](../../../dominio/{{DOC_DOMINIO_1}}) | {{QUE_APORTA_1}} |
> | [`{{DOC_DOMINIO_2}}`](../../../dominio/{{DOC_DOMINIO_2}}) | {{QUE_APORTA_2}} |
>
> El orden del método es **dominio → spec kit → código**: lo de arriba se escribió antes, y este documento lo aplica a esta versión. Si los dos se contradicen, primero se averigua cuál está mal —ver [`FUENTES_AUTORIDAD.md`](../../../dominio/FUENTES_AUTORIDAD.md) §1—.
```

---

## El bloque para la **v1**, que es distinto

> **ATENCIÓN: en la v1 NO hay línea de «Acumulativa».** La v1 no hereda nada,
> así que escribir «contiene todo lo de v1 a v0» o «las 0 operaciones
> existentes» es un disparate — y es el error más fácil de cometer copiando el
> bloque de las otras versiones.

```markdown
> **Versión 1** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Es la primera:** no hereda nada, y todo lo que entregue tendrá que seguir
> funcionando en las versiones siguientes. La v1 **publica {{OPERACIONES}}**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ entrega la v1 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: {{EN_UNA_LINEA_QUE_SE_CONSTRUYE}} |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | {{QUE_PARTE_DEL_MODELO_PUEDE_NOMBRAR}} |
> | [6_contracts.md](6_contracts.md) | {{LAS_OPERACIONES_QUE_PUBLICA}} |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de la primera línea de código |
> | [GUIA_IA1.md](GUIA_IA1.md) | Construirla con IA, desde cero |
>
> ### De dónde sale este documento
>
> | Documento de dominio | Qué aporta |
> |---|---|
> | [`{{DOC_DOMINIO_1}}`](../../../dominio/{{DOC_DOMINIO_1}}) | {{QUE_APORTA_1}} |
> | [`{{DOC_DOMINIO_2}}`](../../../dominio/{{DOC_DOMINIO_2}}) | {{QUE_APORTA_2}} |
>
> El orden del método es **dominio → spec kit → código**: lo de arriba se escribió antes, y este documento lo aplica a esta versión. Si los dos se contradicen, primero se averigua cuál está mal —ver [`FUENTES_AUTORIDAD.md`](../../../dominio/FUENTES_AUTORIDAD.md) §1—.
```

---

## Las dos cosas que cambian de documento a documento

### 1. El documento actual va en negrita y SIN enlace

En `2_spec.md`, la primera fila de la tabla se escribe así:

```markdown
> | **2_spec.md** (este) | QUÉ entrega la v1 y sus criterios de aceptación |
```

Y en `6_contracts.md`, la que cambia es la quinta. **Es un detalle, y se nota
cuando falta**: un enlace que apunta al archivo que uno ya tiene abierto hace
dudar de si se hizo clic.

### 2. La tabla «De dónde sale este documento»

Es distinta en cada uno, y **no se elige: está medida**. Cada documento del
spec kit se alimenta de documentos de dominio concretos:

| El documento | Sale de |
|---|---|
| `2_spec` | `RF_V{{N}}` · `REGLAS_DE_NEGOCIO` · `PLAN_V{{N}}` |
| `3_plan` | `ARQUITECTURA_PROCESOS` · `REQUISITOS_NO_FUNCIONALES` |
| `4_research` | `FUENTES_AUTORIDAD` · `PENDIENTES` |
| `5_data_model` | `DISENO_BD` · `DATOS_DE_PRUEBA` |
| `6_contracts` | `RF_V{{N}}` · `CODIGOS_DE_ESTADO` · `ERRORES_DEL_MOTOR` · `GLOSARIO` |
| `7_quickstart` | `DATOS_DE_PRUEBA` · `CODIGOS_DE_ESTADO` |
| `8_tasks` | `PLAN_V{{N}}` · `PLAN_DE_TRABAJO` |
| `9_checklist` | `PLAN_V{{N}}` · `RF_V{{N}}` |

> **Esta tabla es la que hace que el generador funcione.** Es la razón por la
> que cada fase recibe solo dos o tres documentos de dominio en vez de los
> treinta y uno: **ya se sabe cuál alimenta a cuál.** El mapa completo está en
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](../docs/dominio/METODOLOGIA_CONSTRUCCION_DOMINIOS.md) §7.

---

## Por qué este bloque existe

Quien abre cualquiera de los ocho ve de entrada cuatro cosas: **en qué versión
está**, **qué hereda**, **qué más hay en la carpeta** y **de qué documento de
dominio salió lo que va a leer**.

> Un spec kit en el que cada archivo arranca distinto obliga a reorientarse
> ocho veces. Éste se lee una vez y después se reconoce.
