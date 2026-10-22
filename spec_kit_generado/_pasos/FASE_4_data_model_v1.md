# V1 · FASE 4 — Generar `5_data_model.md`

> Seleccione todo (`Ctrl+A`), cópielo y péguelo en una conversación **nueva**.
>
> **Qué produce:** el modelo de datos de la v1 — **qué parte del esquema puede
> nombrar esta versión, y por qué**. Va en
> `docs/spec_kit/versiones/v1_sin_fk/5_data_model.md`.
>
> **Entrada: ~8.000 tokens.** Salida esperada: ~1.600. Es la fase más liviana
> del generador, porque no recibe ningún documento ya generado.
>
> **Se puede correr sin esperar las fases 2 y 3**: su material es todo de
> dominio.

---

## EL PROMPT

```
Usted escribe el MODELO DE DATOS de la versión 1 de un sistema de
facturación: qué parte del modelo usa esta versión y por qué.

MATERIAL (después de este prompt, en este orden):
  1. La plantilla 5_data_model.plantilla.md — LA ESTRUCTURA QUE DEBE SEGUIR
  2. _ENCABEZADO_COMUN.md  — EL BLOQUE QUE VA ARRIBA DE TODO, y que la
                             plantilla le pide: la cinta de la versión, la
                             tabla de los documentos de la carpeta y el
                             bloque "De dónde sale este documento"
  3. DISENO_BD.md          — el modelo completo, YA DISEÑADO: las cuatro
                             etapas, las llaves, los tipos, los
                             disparadores, los procedimientos y las
                             restricciones
  4. DATOS_DE_PRUEBA.md    — qué filas se siembran y de dónde salió cada
                             una

REGLAS — Y LA PRIMERA ES LA IMPORTANTE:
- NO PROPONGA NI UN CREATE TABLE, ni una columna, ni un índice. El modelo
  ya está diseñado y salió de la elicitación con el usuario experto. Este
  documento DESCRIBE qué parte usa la versión; no lo rediseña. Si cree que
  al modelo le falta algo, escríbalo como [NECESITA ACLARACIÓN: ...] y no
  lo cambie.
- ANTES DE ESCRIBIR, REVISE SI LE ALCANZA EL MATERIAL, y dígalo. Son tres
  cosas distintas, y las tres se reportan:
    (a) un documento que el prompt nombra en el MATERIAL y no llegó;
    (b) algo que LA PLANTILLA pide —una sección, una tabla, una marca
        {{ASI}}— y que ningún documento del material responde;
    (c) algo que ESTE PROMPT le exige y que el material no sustenta.
  Pídalo por su nombre antes de empezar: «para la sección X me falta Y».
- NO CITE DOCUMENTOS QUE NO LE PASÉ. Si una afirmación sale de un
  documento que no está en el MATERIAL, no la escriba y no invente la
  referencia: márquela como [NECESITA ACLARACIÓN: ...]. Una cita con
  número de sección que nadie puede abrir es peor que ninguna cita.
- Y SI ESO NO EXISTE O NO SE LE PUEDE ENTREGAR, no lo invente: deje
  [NECESITA ACLARACIÓN: ...] en esa sección, y DESPUÉS del bloque de
  código escriba, COMO TEXTO CORRIENTE y NO en otro bloque, un apartado
  titulado «SUGERENCIAS PARA EL INGENIERO(A) DE SOFTWARE» con, por cada
  hueco: cuál de los tres casos es, qué documento de dominio haría falta
  —o qué tendría que agregarse a uno que ya existe—, qué debería
  responder, y qué parte del spec kit queda sin sustento mientras no
  exista.
- ESCRIBA UN SOLO DOCUMENTO .md, EN UN SOLO BLOQUE DE CÓDIGO, Y EL BLOQUE
  DE AFUERA LLEVA CUATRO COMILLAS INVERTIDAS, no tres: abre con
  ````markdown y cierra con ````. Es obligatorio porque el documento
  puede llevar adentro sus propios bloques de tres comillas —XML, SQL,
  C#, PowerShell, diagramas— y con tres afuera el primero de adentro
  CIERRA el de afuera: a partir de ahí todo sale crudo y las tablas se
  rompen. No genere dos archivos .md, no parta el documento en varios
  bloques, y no ponga las sugerencias en un bloque aparte. Adentro va
  ÚNICAMENTE el documento pedido.
- NADA DE MARCADORES DE CITA. No agregue [cite: 1], [cite: 3, 5] ni
  referencias parecidas: el documento sale limpio, como para publicarlo.
- LOS DIAGRAMAS VAN DENTRO DE UN BLOQUE DE CÓDIGO, para que las flechas
  y la sangría no se descuadren al renderizar.
- En castellano. Trato de usted.
- EL TÍTULO CON # VA DE PRIMERO, y el bloque de cita del encabezado
  DEBAJO. Al revés, el documento abre con una cita suelta y el título
  aparece a media página.
- SI ESTA ES LA VERSIÓN 1, NO ESCRIBA LA LÍNEA DE «Acumulativa»: la v1 no
  hereda nada, y decir «contiene todo lo de v1 a v0» o «las 0 operaciones
  existentes» es un disparate. El encabezado común trae el bloque exacto
  para la v1, aparte del de las demás versiones.
- DIGA DE ENTRADA, EN LA PRIMERA LÍNEA, si esta versión toca la base de
  datos o no. Si no la toca, dígalo con esas palabras: es una afirmación
  útil, no un documento vacío.
- LA SECCIÓN QUE HACE AL DOCUMENTO es «lo que ESTA versión puede
  nombrar». El esquema está completo desde el primer día; lo que cambia
  por versión es QUÉ PARTE PUEDE TOCAR EL CÓDIGO. Por cada tabla del
  esquema diga si esta versión la nombra y POR QUÉ — y si no, a qué
  versión pertenece.
- LAS TABLAS QUE MENCIONE TIENEN QUE EXISTIR en DISENO_BD.md. No invente
  nombres plausibles: cotéjelos uno por uno.
- SOLO LOS DISPARADORES Y PROCEDIMIENTOS QUE ESTA VERSIÓN USA, no el
  catálogo completo. Y por cada uno: cuándo se dispara y qué hace.
- LOS DATOS SEMBRADOS van con los valores REALES de DATOS_DE_PRUEBA.md,
  porque el 7_quickstart los va a usar. Una fila inventada aquí hace
  perder media hora allá.
- SI UNA DIFERENCIA DE DIALECTO NO FALLA sino que devuelve un valor
  equivocado, dígalo con esas palabras. Y si se descubrió ejecutando y no
  leyendo, dígalo también.

SALIDA: el contenido de 5_data_model.md, dentro del bloque de código, sin
preámbulo ni explicación posterior.

--- MATERIAL ---
```

---

<!--
PLANTILLA — 5_data_model.md
En Spec Kit oficial: data-model.md  ·  comando /speckit.plan  ·  FASE 1
Sale de: DISENO_BD.md + DATOS_DE_PRUEBA.md

ESTRUCTURA CANÓNICA, tomada de las v1, v2 y v3 —no de la v4—:

   1. Qué parte del modelo puede nombrar ESTA versión, y qué la hace distinta
   2. El mapa de dependencias: por qué estas tablas y no otras
   3. Qué cambia esta versión en la base de datos (casi siempre: nada)
   4. Los disparadores y procedimientos que ESTA versión usa
   5. Los datos sembrados
   6. Las restricciones, y cómo se ven desde la API
   7. Lo que el dialecto del motor cambia

LO QUE LA IA NO DEBE HACER AQUÍ: proponer un CREATE TABLE.
El modelo YA ESTÁ en DISENO_BD.md, y salió de la elicitación con el usuario
experto. Este documento DESCRIBE qué parte del modelo usa esta versión y por
qué — no lo rediseña. Una IA que propone el esquema está rehaciendo un trabajo
hecho, sin haber estado en esa conversación.

Ejemplos llenos: ../docs/spec_kit/versiones/v1_sin_fk/5_data_model.md (5.572 car)
                 ../docs/spec_kit/versiones/v2_con_fk/5_data_model.md (8.098 car)
-->

# Modelo de datos — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 5_data_model.md va en negrita y sin enlace. -->

---

> {{QUE_LE_PASA_AL_MODELO_EN_ESTA_VERSION}}

<!-- La primera línea responde una sola pregunta: ¿esta versión toca la base de
     datos o no? Las dos respuestas son buenas, y las dos se escriben.
     Un 5_data_model que no dice nada porque «no cambió nada» es una
     oportunidad perdida: justamente eso es lo que hay que afirmar. -->

## 1. La base de datos viene DADA, completa

{{QUE_TRAE_EL_ESQUEMA_DESDE_EL_PRIMER_DIA}}

> **Y esto no se negocia: el esquema se copia, no se genera.** Ninguna IA
> propone un `CREATE TABLE` aquí — el modelo ya está hecho y salió de la
> elicitación.

### Lo que ESTA versión puede nombrar

<!-- OBLIGATORIO, y es la sección que hace al documento. El esquema está
     completo desde el día uno; lo que cambia por versión es QUÉ PARTE PUEDE
     TOCAR EL CÓDIGO. -->

| Tabla | ¿La nombra esta versión? | Por qué |
|---|---|---|
| `{{TABLA}}` | **sí** | {{...}} |
| `{{TABLA}}` | no — es de la v{{M}} | {{QUE_LA_HACE_DE_ESA_VERSION}} |

> {{EL_CRITERIO_QUE_DECIDE_QUE_TABLA_ES_DE_QUE_VERSION}}

## 2. El mapa de dependencias

<!-- Por qué estas tablas van antes que aquéllas. En la v1 es «no dependen de
     nada»; en la v2, «estas dependen de las de la v1». -->

```
{{DIAGRAMA_DE_DEPENDENCIAS}}
```

| Desde | Hacia | Cardinalidad | Participación |
|---|---|---|---|
| `{{...}}` | `{{...}}` | {{N:1}} | **total** / parcial |

> **La participación total es la que se olvida**, y es la que produce datos
> huérfanos. {{EL_EJEMPLO_DE_ESTA_VERSION}}

## 3. Qué cambia esta versión en la base de datos

| | |
|---|---|
| **Tablas nuevas** | {{NINGUNA_O_CUALES}} |
| **Columnas nuevas** | {{...}} |
| **Datos sembrados nuevos** | {{...}} |

> {{SI_NO_CAMBIA_NADA_DECIRLO_CON_ESAS_PALABRAS_Y_POR_QUE_ESO_ES_BUENO}}

## 4. Los disparadores y procedimientos que ESTA versión usa

<!-- Solo los que la versión toca, no el catálogo completo. Y por cada uno:
     cuándo se dispara y qué hace. -->

| {{DISPARADOR_O_PROCEDIMIENTO}} | Cuándo | Qué hace |
|---|---|---|
| `{{...}}` | {{...}} | {{...}} |

> **Por qué en la base de datos y no en el código.** {{LA_RAZON}}

<!-- La razón que vale: una regla que solo vive en la aplicación protege a
     quien pasa por la aplicación. La que está en la base de datos protege
     también a quien entre por el cliente del motor. -->

## 5. Los datos sembrados

<!-- Con qué filas se prueba, y de dónde salió cada una. Esto es lo que el
     7_quickstart va a usar, así que los valores tienen que ser los reales. -->

| Tabla | Cuántas filas | Lo que hay que saber |
|---|---|---|
| `{{...}}` | {{N}} | {{...}} |

> {{LA_FILA_QUE_DEMUESTRA_ALGO}}

<!-- RECOMENDADO: señalar la fila que prueba una decisión del modelo. En el
     ejemplo, una persona que es cliente Y vendedor a la vez demuestra por qué
     esos papeles son tablas aparte y no una columna «tipo». -->

## 6. Las restricciones, y cómo se ven desde la API

<!-- El puente entre el modelo y el contrato: qué restricción produce qué
     código de estado. -->

| La restricción | Qué impide | Qué responde la API |
|---|---|---|
| `{{...}}` | {{...}} | **{{CODIGO}}** |

> {{LA_QUE_RESPONDE_DISTINTO_EN_ESTA_VERSION_Y_POR_QUE}}

## 7. Lo que el dialecto del motor cambia

<!-- OBLIGATORIO si el proyecto corre o va a correr contra más de un motor.
     Y la pregunta que esta sección contesta: ¿la diferencia se resolvió en el
     SQL, o se dejó subir hasta los modelos? Lo primero mantiene el contrato
     idéntico entre motores; lo segundo ata el contrato al motor. -->

| | {{MOTOR_DE_ESTA_VERSION}} | {{EL_OTRO_MOTOR}} |
|---|---|---|
| **{{DIFERENCIA}}** | {{...}} | {{...}} |

> {{DONDE_SE_RESOLVIERON_Y_QUE_PERMITE_ESO}}

<!-- Y si una de estas diferencias NO falla sino que devuelve un valor
     equivocado, dígalo con esas palabras. Si se descubrió EJECUTANDO y no
     leyendo, dígalo también: es la clase de dato que no está en la
     documentación del motor. -->

<!-- SI EL MODELO TIENE ALGO QUE AVERIGUAR, NO SE INVENTA:
     [NECESITA ACLARACIÓN: ¿{{la pregunta}}?] — y va a la próxima reunión. -->
# El encabezado común de los ocho documentos de una versión

> **Esto no es un documento del spec kit.** Es el bloque que va **arriba de
> todo** en `2_spec`, `3_plan`, `4_research`, `5_data_model`, `6_contracts`,
> `7_quickstart`, `8_tasks` y `9_checklist` — idéntico en los ocho, salvo dos
> cosas que se marcan.
>
> Está aquí una sola vez para no repetirlo ocho veces en las plantillas.

---

## El bloque, para copiar

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
> El orden del método es **dominio → spec kit → código**: lo de arriba se escribió antes, y este documento lo aplica a esta versión. Si los dos se contradicen, primero se averigua cuál está mal —ver [`FUENTES.md`](../../../dominio/FUENTES.md) §2—.
```

---

## Las dos cosas que cambian de documento a documento

### 1. El documento actual va en negrita y SIN enlace

En `2_spec.md`, la primera fila de la tabla se escribe así:

```markdown
> | **2_spec.md** (este) | QUÉ agrega la v{{N}} y sus criterios de aceptación |
```

Y en `6_contracts.md`, la que cambia es la quinta. **Es un detalle, y se nota
cuando falta**: un enlace que apunta al archivo que uno ya tiene abierto hace
dudar de si se hizo clic.

### 2. La tabla «De dónde sale este documento»

Es distinta en cada uno, y **no se elige: está medida**. Cada documento del
spec kit se alimenta de documentos de dominio concretos:

| El documento | Sale de |
|---|---|
| `2_spec` | `REQUISITOS_FUNCIONALES` · `REGLAS_DE_NEGOCIO` · `PLAN_V{{N}}` |
| `3_plan` | `ARQUITECTURA` · `REQUISITOS_NO_FUNCIONALES` |
| `4_research` | `FUENTES` · `PENDIENTES` |
| `5_data_model` | `DISENO_BD` · `DATOS_DE_PRUEBA` |
| `6_contracts` | `REQUISITOS_FUNCIONALES` · `POLITICA_DE_ERRORES` · `GLOSARIO` |
| `7_quickstart` | `DATOS_DE_PRUEBA` · `POLITICA_DE_ERRORES` |
| `8_tasks` | `PLAN_V{{N}}` · `PLAN_DE_TRABAJO` |
| `9_checklist` | `PLAN_V{{N}}` · `REQUISITOS_FUNCIONALES` |

> **Esta tabla es la que hace que el generador funcione.** Es la razón por la
> que cada fase recibe solo dos o tres documentos de dominio en vez de los
> diecinueve: **ya se sabe cuál alimenta a cuál.** El mapa completo está en
> [`MAPA_DE_DOCUMENTOS.md`](../docs/dominio/MAPA_DE_DOCUMENTOS.md).

---

## Por qué este bloque existe

Quien abre cualquiera de los ocho ve de entrada cuatro cosas: **en qué versión
está**, **que la versión contiene a las anteriores**, **qué más hay en la
carpeta** y **de qué documento de dominio salió lo que va a leer**.

> Un spec kit en el que cada archivo arranca distinto obliga a reorientarse
> ocho veces. Éste se lee una vez y después se reconoce.
# Diseño de base de datos — `bdfacturas`

> **Qué es este documento.** El modelo de datos en las cuatro etapas que
> [`CONCEPTOS_DISENO_BD.md`](../conceptos/CONCEPTOS_DISENO_BD.md) define:
> conceptual → selección del motor → lógico → físico. Y **por qué** quedó así.
>
> **Qué NO es.** No es el script. [`db/bdfacturas.sql`](../../db/bdfacturas.sql)
> es la **salida** de la etapa física, no su sustituto: un script dice *qué*
> quedó, no *por qué*.
>
> **Material académico simulado.** El esquema es verosímil y funciona; el dominio
> lo fijó el curso.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Conceptual — las entidades y cómo se relacionan

Doce tablas en cuatro grupos:

| Grupo | Tablas | Qué resuelve |
|---|---|---|
| **Catálogos** | `producto` · `empresa` · `persona` | Lo que existe antes de cualquier venta |
| **Papeles** | `cliente` · `vendedor` | Lo que una persona *hace* |
| **La venta** | `factura` · `productosporfactura` | El maestro y su detalle |
| **Acceso** | `usuario` · `rol` · `ruta` · `rol_usuario` · `rutarol` | Quién entra y a qué |

### Las relaciones, con su cardinalidad

| Desde | Hacia | Cardinalidad | Participación |
|---|---|---|---|
| `cliente` | `persona` | N:1 | **total** — todo cliente es una persona |
| `cliente` | `empresa` | N:1 | **parcial** — hay clientes sin empresa |
| `vendedor` | `persona` | N:1 | **total** |
| `factura` | `cliente` | N:1 | **total** |
| `factura` | `vendedor` | N:1 | **total** |
| `productosporfactura` | `factura` | N:1 | **total** — no hay renglón sin factura |
| `productosporfactura` | `producto` | N:1 | **total** |
| `usuario` ↔ `rol` | vía `rol_usuario` | **N:M** | parcial por los dos lados |
| `ruta` ↔ `rol` | vía `rutarol` | **N:M** | parcial por los dos lados |

> **La participación total es la que se olvida**, y es la que produce datos
> huérfanos. «Todo renglón pertenece a una factura» no es cardinalidad: es que
> `fknumfactura` es `NOT NULL`. Si fuera opcional, la base de datos aceptaría renglones
> que no son de nadie, y nadie los vería nunca más.

### La decisión que define el modelo: `cliente` y `vendedor` aparte

Lo fácil habría sido una columna `tipo` en `persona` con los valores `cliente` y
`vendedor`. Se descartó, y por una razón concreta:

> **Una persona puede ser las dos cosas.** Con una columna `tipo` habría que
> duplicar la fila —el mismo ser humano, dos veces— y entonces el correo y el
> teléfono quedan en dos sitios que se desincronizan. Con dos tablas que apuntan
> a `persona`, el dato de la persona vive **una sola vez** y los papeles son
> filas aparte.

**Y no es una hipótesis: está en los datos sembrados.** `persona` tiene 6 filas,
y entre `cliente` y `vendedor` hay 7:

| Persona | ¿cliente? | ¿vendedor? |
|---|---|---|
| P001 Ana Torres | sí | — |
| P002 Carlos Pérez | — | sí |
| P003 María Gómez | sí | — |
| P004 Juan Díaz | — | sí |
| P005 Laura Rojas | sí | — |
| **P006 Pedro Castillo** | **sí** | **sí** |

Pedro Castillo compra y vende. Con una columna `tipo` en `persona` habría dos
filas con su nombre, su correo y su teléfono — y el día que cambie de número,
una de las dos se queda vieja.

---

## 2. Selección del motor

**SQL Server** de la v1 a la v4. **PostgreSQL** se suma en la v5.

| Qué pide este dominio | Qué se necesitaba |
|---|---|
| Un total que no puede descuadrarse | **Transacciones** y **disparadores** |
| Un stock que no puede quedar negativo | Un rechazo que la aplicación no pueda evadir |
| Una factura que no se corrige | Integridad referencial con `ON DELETE` explícito |

Un motor sin transacciones queda descartado de entrada: esto es facturación, y
media factura no existe. Ver [`PRINCIPIOS_ACID.md`](../conceptos/PRINCIPIOS_ACID.md).

> **Y por qué DOS motores y no uno.** Porque la v5 existe para demostrar que el
> sistema está **abierto al cambio**: el mismo contrato, la misma interfaz, otro
> motor debajo, y sin tocar un controlador. Eso no se puede probar con un solo
> motor — se afirma.

---

## 3. Lógico — las llaves y los tipos

### Las llaves primarias, y por qué no son todas iguales

| Tabla | Llave | Quién la pone | Por qué |
|---|---|---|---|
| `producto` · `empresa` · `persona` | `codigo` NVARCHAR(10) | **quien crea la fila** | Son códigos del negocio: `PR001`, `E001`. La gente los dice en voz alta |
| `usuario` | `email` | quien crea la fila | El correo ya identifica a la persona; un `id` aparte sería un dato más que mantener |
| `rol` · `ruta` · `cliente` · `vendedor` · `factura` | `id` / `numero` INT **IDENTITY** | **la base de datos** | No hay un código natural. Inventarlo sería pedirle a alguien que lleve la cuenta |
| `productosporfactura` | `(fknumfactura, fkcodproducto)` | — | **Compuesta**: el renglón *es* esa pareja |
| `rol_usuario` · `rutarol` | las dos columnas | — | Igual: una asignación existe o no existe |

> **Las tres llaves compuestas no llevan `id` propio a propósito.** Un `id` en
> una tabla puente permite dos filas con la misma pareja y distinto id — que es
> exactamente lo que la tabla existe para impedir.

### Los tipos que importan

| Columna | Tipo | Por qué ése |
|---|---|---|
| `valorunitario` · `subtotal` · `total` · `credito` | `DECIMAL(18,2)` | **Nunca `FLOAT` para dinero.** `0.1 + 0.2` en coma flotante no da `0.3`, y en una factura eso es un centavo que nadie encuentra |
| `fecha` | `DATETIME2` con `DEFAULT GETDATE()` | La pone la base de datos. Si la mandara el cliente, dependería del reloj de su máquina |
| `estado` | `NVARCHAR(10)` con `DEFAULT N'activa'` | Dos valores: `activa` y `anulada` |
| `contrasena` | `NVARCHAR(200)` | Guarda el **hash**, no la clave. 200 deja espacio para un algoritmo futuro más largo |

### Lo derivado que SÍ se guarda, y por qué

Normalmente un dato derivado no se guarda. Aquí hay dos excepciones, y las dos
son el mismo caso:

| Dato | Se podría calcular | Y aun así se guarda porque… |
|---|---|---|
| `productosporfactura.subtotal` | `cantidad × valorunitario` | …el precio de **hoy** no es el de cuando se emitió. Recalcularlo **reescribiría las ventas del año pasado** |
| `factura.total` | sumando los subtotales | …es el valor del documento. Y lo mantiene un disparador, así que no puede quedar desactualizado |

> **La regla no es «nunca guardes derivados», es «guárdalos cuando congelarlos
> sea el requisito»** — y aquí lo es.

---

## 4. Físico — lo que el script hace

### Los tres disparadores

| Disparador | Cuándo | Qué hace |
|---|---|---|
| `trg_prodfact_insert` | al agregar un renglón | Comprueba stock (`THROW 50001` si no alcanza) · calcula el `subtotal` · **baja** el stock · recalcula el `total` |
| `trg_prodfact_update` | al cambiar un renglón | Lo mismo, ajustando la diferencia |
| `trg_prodfact_delete` | al quitar un renglón | **Devuelve** el stock · recalcula el `total` |

> **Por qué en la base de datos y no en C#.** Porque la regla tiene que valer también para
> quien entre por SSMS. Una validación que solo vive en la aplicación protege a
> quien pasa por la aplicación.

### Los dos `ON DELETE CASCADE`, y los que NO lo llevan

| Relación | Al borrar el padre | Por qué |
|---|---|---|
| `productosporfactura` → `factura` | **CASCADE** | Un renglón sin su factura no significa nada |
| `rutarol` → `ruta` y → `rol` | **CASCADE** | Una asignación a un rol que ya no existe tampoco |
| `factura` → `cliente` / `vendedor` | **sin cascada** | Borrar un cliente **no** puede llevarse sus facturas. La base de datos lo impide, y está bien que lo impida |
| `productosporfactura` → `producto` | **sin cascada** | Igual: un producto vendido no se puede borrar |

> **Esos dos «sin cascada» son una decisión, no un olvido.** Significan que un
> cliente con facturas **no se puede borrar**, y que la API responde el error del
> motor. Es el comportamiento correcto para un sistema donde la factura es un
> documento.

### Los 16 procedimientos

| Grupo | Para qué |
|---|---|
| `sp_*_factura_y_productosporfactura` (5) | Maestro y detalle **en una transacción**: listar, consultar, insertar, actualizar, borrar |
| `sp_anular_factura` | El borrado lógico de la factura, con devolución de stock |
| `*_usuario_con_roles` (5) + `actualizar_roles_usuario` | El usuario y sus roles juntos |
| `verificar_acceso_ruta` | La pregunta del control de acceso, en cada petición |
| `listar/crear/eliminar_rutarol` | Los permisos |

> **La factura se opera SIEMPRE por procedimiento, nunca con SQL suelto desde
> C#.** Y no es gusto: insertar el encabezado y tres renglones son cuatro
> escrituras, y tienen que ser **una sola operación**. Dentro del procedimiento
> hay una transacción; desde C# serían cuatro viajes y cuatro oportunidades de
> quedar a medias.

---

## 5. Lo que este modelo NO resuelve

| No está | Consecuencia |
|---|---|
| Impuestos, descuentos | El total es la suma de subtotales |
| Devoluciones parciales | Una factura se anula entera |
| Entradas de inventario | El stock baja al vender y sube al anular; cómo llegó ahí, no se modela |
| Histórico de precios | Solo el precio de hoy en `producto` y el congelado en cada renglón |
| Borrado lógico en catálogos | `producto`, `empresa` y `persona` se borran de verdad |

> Esa última fila **es distinta en el proyecto de aula**, donde el borrado sí es
> lógico. Copiar el ejemplo tal cual deja mal al equipo.

---

## 6. Cómo comprobar todo esto

```powershell
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa `
  -P "Paradigmas123!" -C -d bdfacturas -Q "SELECT name FROM sys.tables ORDER BY name;"
```

| Qué | Dónde |
|---|---|
| El script completo | [`db/bdfacturas.sql`](../../db/bdfacturas.sql) |
| El mismo esquema en PostgreSQL | [`db/bdfacturas_postgres.sql`](../../db/bdfacturas_postgres.sql) |
| Las reglas que defiende | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Los datos sembrados | [`DATOS_DE_PRUEBA.md`](DATOS_DE_PRUEBA.md) |
| El modelo por versión | el `5_data_model.md` de cada una |
# Datos de prueba — `bdfacturas`

> **Qué es este archivo.** Qué trae la base de datos **recién sembrada**: cuántas filas,
> cuáles, y **para qué sirve cada una**. Porque varias están puestas a propósito
> para que algo se pueda probar.
>
> **Qué NO es.** No es el `INSERT`. Ése está en
> [`db/bdfacturas.sql`](../../db/bdfacturas.sql), al final. Aquí se dice **por
> qué** esos datos y no otros.
>
> **Material académico simulado**: los nombres son inventados, las cifras son
> verosímiles, y el sistema funciona con ellos.
>
> **Medido contra la base de datos recién levantada**, no copiado del script.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Las cuentas

| Tabla | Filas | Para qué esa cantidad |
|---|---|---|
| `empresa` | 3 | Suficientes para un desplegable con opciones de verdad |
| `persona` | 6 | **Una de ellas cumple los dos papeles** — ver §2 |
| `producto` | 8 | Con stocks distintos, para que «stock insuficiente» se pueda provocar |
| `cliente` | 4 | Uno de ellos **sin empresa**, para probar la llave foránea opcional |
| `vendedor` | 3 | |
| `factura` | 6 | Con 1, 2 y 3 renglones: los tres casos del maestro-detalle |
| `productosporfactura` | 12 | |
| `rol` | 5 | |
| `ruta` | 15 | Las interfaces protegibles |
| `usuario` | 8 | Con repartos de roles distintos — ver §4 |
| `rol_usuario` | 21 | |
| `rutarol` | 25 | Ni todos los roles a todas las rutas: así el 403 se puede provocar |

> **Ninguna factura nace anulada.** Las 6 están `activa`, para que la primera
> anulación que se haga sea la que se está probando.

---

## 2. Las personas, y la que importa

| Código | Nombre | ¿cliente? | ¿vendedor? |
|---|---|---|---|
| P001 | Ana Torres | sí | — |
| P002 | Carlos Pérez | — | sí |
| P003 | María Gómez | sí | — |
| P004 | Juan Díaz | — | sí |
| P005 | Laura Rojas | sí | — |
| **P006** | **Pedro Castillo** | **sí** | **sí** |

> **Pedro Castillo está ahí a propósito.** Es la prueba viva de por qué
> `cliente` y `vendedor` son tablas aparte y no una columna `tipo` en `persona`:
> con un `tipo` habría dos filas con su nombre, su correo y su teléfono, y el
> día que cambie de número una de las dos se queda vieja. Ver
> [`DISENO_BD.md`](DISENO_BD.md) §1.

---

## 3. Los productos y sus stocks

| Código | Producto | Stock sembrado |
|---|---|---|
| PR001 | Laptop Lenovo IdeaPad | 17 |
| PR002 | Monitor Samsung 24" | 27 |
| PR003 | Teclado Logitech K380 | 42 |
| PR004 | Mouse HP | 56 |
| PR005 | Impresora Epson EcoTank1 | **14** |
| PR006 | Auriculares Sony WH-CH510 | 23 |
| PR007 | Tablet Samsung Tab A9 | 15 |
| PR008 | Disco Duro Seagate 1TB | 32 |

> **Estos son los stocks DESPUÉS de las seis facturas sembradas**, no los
> iniciales: el disparador ya descontó lo vendido. Es decir, la base de datos llega en un
> estado coherente, no con unos números sueltos.

> **PR005 con 14 es el más cómodo para provocar el `THROW 50001`:** pida 50 y la
> base responderá *«Stock disponible: 14, cantidad solicitada: 50»*.

---

## 4. Los usuarios y sus roles

| Correo | Contraseña | Roles |
|---|---|---|
| `admin@correo.com` | `admin123` | Administrador |
| `vendedor1@correo.com` | `vendedor123` | Vendedor, Cajero |
| `cliente1@correo.com` | `cliente123` | Cliente |
| `jefe@correo.com` | — | Administrador, Cajero, Contador |
| `nuevo@correo.com` | — | Administrador, Vendedor, Cajero |
| `test_encript@correo.com` | — | Administrador |
| `carlos.castro@usbmed.edu.co` | — | los cinco |
| `carloscastro5033@correo.itm.edu.co` | — | los cinco |

> **Las contraseñas están en la base de datos CIFRADAS.** Las tres de arriba se conocen
> porque el curso las publica para poder entrar; las demás se sembraron con un
> hash y **nadie sabe su clave** — y eso también es correcto: así debe ser.

> **Los tres primeros usuarios son los que importan para probar**, y están
> escogidos para que el 403 se pueda ver sin preparar nada:
>
> | Entre como | Y verá | Pero NO verá |
> |---|---|---|
> | `admin@correo.com` | todo | — |
> | `vendedor1@correo.com` | Facturas, Clientes | Usuarios, Personas, Productos |
> | `cliente1@correo.com` | Productos | Facturas, Clientes — **al revés que el vendedor** |
>
> Que el cliente vea lo que el vendedor no, y al revés, es deliberado: si uno
> viera un subconjunto del otro, un error de permisos podría pasar
> desapercibido.

---

## 5. Las 15 rutas protegibles

```
interfaz.inicio      interfaz.usuarios    interfaz.facturas
interfaz.clientes    interfaz.vendedores  interfaz.personas
interfaz.empresas    interfaz.productos   interfaz.roles
interfaz.permisos    interfaz.rutas       permiso.crear
permiso.eliminar     ruta.crear           ruta.eliminar
```

> **Fíjese en que no son direcciones de la API.** `interfaz.facturas` no es
> `/api/factura`: es el **nombre de una pantalla**. Una fila de `ruta` es una
> etiqueta que el control de acceso usa para decidir. Buscar
> `/api/interfaz.facturas` no lleva a ninguna parte.

> Y hay cinco que no empiezan por `interfaz.`: `permiso.*` y `ruta.*` protegen
> **acciones**, no pantallas. Repartir permisos es algo que se hace, no algo que
> se mira.

---

## 6. Cómo volver a este estado

Las pruebas ensucian: una factura emitida se queda, y el stock baja.

```powershell
docker compose down          # apaga y CONSERVA los datos
docker compose down -v       # ⚠ borra el volumen: la base de datos vuelve a sembrarse
docker compose up -d
```

> **`down -v` borra lo que haya.** Si alguien estaba trabajando con datos
> propios, los pierde. Para una prueba puntual es mejor crear la fila, probar y
> borrarla — y si se tocó una fila sembrada, reponerla.

---

## 7. Comprobar las cuentas

```powershell
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa `
  -P "Paradigmas123!" -C -d bdfacturas_sqlserver_local -Q `
  "SELECT 'factura', COUNT(*) FROM factura UNION ALL SELECT 'renglones', COUNT(*) FROM productosporfactura;"
```

Recién sembrada debe decir **6 y 12**. Si dice más, alguien dejó datos de una
prueba — y conviene saber quién antes de borrarlos.
