# V1 · FASE 2 — Generar `3_plan.md`

> Seleccione todo (`Ctrl+A`), cópielo y péguelo en una conversación **nueva**.
>
> **Qué produce:** el plan técnico de la v1 — **el CÓMO**. El QUÉ ya está
> decidido en el `2_spec` y no se cambia aquí. Va en
> `docs/spec_kit/versiones/v1_sin_fk/3_plan.md`.
>
> **Entrada: ~11.400 tokens.** Salida esperada: ~3.600.
>
> **Conversación nueva, de verdad.** Esta fase recibe el `2_spec` ya generado;
> si se corre en la misma conversación que las anteriores, el reparto por
> fases no se está probando — y se nota, porque la IA cita documentos que esta
> fase no le dio.

---

## EL PROMPT

```
Usted escribe el PLAN TÉCNICO de la versión 1 de un sistema de
facturación: el CÓMO. El QUÉ ya está decidido en el 2_spec que le
adjunto, y no se cambia aquí.

MATERIAL (después de este prompt, en este orden):
  1. La plantilla 3_plan.plantilla.md — LA ESTRUCTURA QUE DEBE SEGUIR
  2. _ENCABEZADO_COMUN.md   — EL BLOQUE QUE VA ARRIBA DE TODO, y que la
                              plantilla le pide: la cinta de la versión,
                              la tabla de los documentos de la carpeta y
                              el bloque "De dónde sale este documento"
  3. 1_constitution.md      — LOS OCHO ARTÍCULOS, para el chequeo de la
                              compuerta 2. Se verifica contra esto, no
                              contra lo que usted recuerde
  4. 2_spec.md (recortado)  — EL ALCANCE, los requisitos y los criterios,
                              ya fijados. No los amplíe
  5. ARQUITECTURA_PROCESOS.md — los procesos, los puertos, por qué esto
                              no es un monolito, y Blazor Server con lo
                              que implica
  6. REQUISITOS_NO_FUNCIONALES.md — lo que el plan tiene que cumplir, con
                              su forma de comprobarlo

REGLAS:
- ANTES DE ESCRIBIR, REVISE SI LE ALCANZA EL MATERIAL, y dígalo. Son tres
  cosas distintas, y las tres se reportan:
    (a) un documento que el prompt nombra en el MATERIAL y no llegó;
    (b) algo que LA PLANTILLA pide —una sección, una tabla, una marca
        {{ASI}}— y que ningún documento del material responde;
    (c) algo que ESTE PROMPT le exige y que el material no sustenta.
  Pídalo por su nombre antes de empezar: «para la sección X me falta Y».
  NO lo reemplace con lo que usted supondría que dice.
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
- NO AMPLÍE EL ALCANCE DEL 2_spec. Si el plan necesita algo que el 2_spec
  no pide, eso es una señal: escríbalo como [NECESITA ACLARACIÓN: ...] en
  vez de agregarlo.
- LAS PIEZAS VAN CON SU RUTA DE ARCHIVO REAL, no «el controlador». Una
  tabla con nombres genéricos no le sirve a quien va a escribirlos.
- LAS DECISIONES VAN COMO TABLA DE OPCIONES con la elegida marcada ✅, y
  las descartadas SE ESCRIBEN CON SU MEJOR ARGUMENTO. Una tabla donde las
  alternativas son obviamente malas no demuestra que se pensó: demuestra
  que se justificó lo que ya se había hecho.
- CUANDO PUEDA PONER UN NÚMERO, PÓNGALO. «Diez esperas de diez segundos
  son minuto y medio para abrir una página» convence; «es más eficiente»
  no.
- EL CHEQUEO DE CONSTITUCIÓN es obligatorio: una fila por artículo que
  esta versión toca, diciendo CÓMO lo cumple este plan. Es la compuerta 2
  del método, y se pasa antes de construir.
- EL REGISTRO DE DEPENDENCIAS lleva su propia subsección, con las líneas
  exactas. Sin ellas el proyecto COMPILA, arranca, y falla el día que
  alguien usa el endpoint — y el compilador no puede atraparlo.
- LAS INTERFACES GRÁFICAS VAN EN EL PLAN con el mismo detalle que la API.
  Media versión no es una versión.
- LA SECCIÓN DE TROPIEZOS, si la escribe, SEPARA LOS QUE FALLAN DE LOS
- NO ESCRIBA CÓDIGO CON NOMBRES QUE EL MATERIAL NO TRAIGA. Ni nombres de
  columna, ni de tabla, ni credenciales, ni versiones de paquete, ni
  nombres de artículo de la constitución. Si hace falta un ejemplo y el
  material no trae el dato, use un marcador —<columna>, <clave>— y
  anótelo en las SUGERENCIAS. Una columna inventada que se ve plausible
  se copia al código y falla al primer SELECT.
- ESTE DOCUMENTO ES EL CÓMO, NO LA IMPLEMENTACIÓN. Van las piezas con su
  ruta, las firmas y fragmentos cortos que ilustren una decisión — no
  clases completas. El código lo genera el segundo prompt, a partir del
  spec kit.
- NUNCA CONSTRUYA SQL CON INTERPOLACIÓN DE CADENAS, ni para los nombres
  de columna. Parametrizar solo los valores y concatenar los
  identificadores sigue siendo inyección: el nombre del campo viene del
  cuerpo de la petición. Si un PATCH parcial necesita columnas variables,
  dígalo como decisión a resolver, con sus opciones.
- EL CHEQUEO DE CONSTITUCIÓN SE HACE CONTRA LA CONSTITUCIÓN QUE LE PASO,
  artículo por artículo y con su nombre REAL. Si un artículo no se puede
  verificar con lo que hay en el material, la fila dice
  [NECESITA ACLARACIÓN: ...] — no ✅. Un chequeo con ocho ✅ que no se
  revisaron es peor que no tener chequeo.
- Y ANTES DE PONER ✅ EN UN ARTÍCULO, COMPRUEBE QUE EL PROPIO PLAN NO LO
- NO CONVIERTA URLs EN ENLACES DENTRO DE LOS BLOQUES DE CÓDIGO. Una
  imagen de Docker escrita como [mcr.microsoft.com/...](https://...)
  rompe el YAML: la línea deja de ser válida. Dentro de un bloque de
  código, las direcciones van en texto plano.
- LA ÚLTIMA SECCIÓN ES «Lo que este plan deja FUERA, a propósito», y es
  OBLIGATORIA. Cada fila con su razón, y la buena razón es casi siempre
  «el problema todavía no existe» — no «no tuvimos tiempo». Sin esa
  sección, alguien agrega lo que no era de esta versión creyendo que
  ayuda.
- LOS SERVICIOS TAMBIÉN LLEVAN SU INTERFAZ, no solo los repositorios:
  IServicioX además de IRepositorioX. Un controlador que recibe la clase
  concreta del servicio no se puede probar con un falso, y el artículo de
  arquitectura exige una interfaz entre cada par de capas.
  VIOLA. Si el artículo exige una clase de petición por verbo, un plan
  que usa tipos dinámicos no lo cumple, por más que la tabla diga que sí.
  QUE NO FALLAN. Un error que revienta se arregla en cinco minutos; uno
  que responde 200 con un dato equivocado cuesta una tarde. Y NO INVENTE
  tropiezos: si el material no los nombra, deje la sección anunciada y
  vacía.

SALIDA: el contenido de 3_plan.md, dentro del bloque de código, sin
preámbulo ni explicación posterior.

--- MATERIAL ---
```

<!--
PLANTILLA — 3_plan.md
En Spec Kit oficial: plan.md  ·  comando /speckit.plan
Sale de: ARQUITECTURA_PROCESOS.md + REQUISITOS_NO_FUNCIONALES.md
         (+ el 2_spec ya generado, que fija el alcance)

ESTRUCTURA CANÓNICA, tomada de las v1, v2 y v3 de este repositorio —no de la
v4, que está comprimida y es la excepción—:

   1. La pila: qué cambia y qué NO
   2. La estructura y las capas, con el flujo de una petición
   3. Lo que esta versión agrega, ARCHIVO POR ARCHIVO
   4. Las decisiones de diseño, cada una con su razón
   5. Los tropiezos que esta versión tiene preparados
   6. Chequeo de constitución
   7. Lo que este plan deja FUERA, a propósito

Es el CÓMO. El 2_spec dice qué; éste dice con qué piezas y qué se decidió.
En Spec Kit oficial, este documento contiene las tres fases (0, 1 y 2) que
producen research.md, data-model.md / contracts/ / quickstart.md, y tasks.md.

Ejemplos llenos: ../docs/spec_kit/versiones/v1_sin_fk/3_plan.md (13.140 car)
                 ../docs/spec_kit/versiones/v2_con_fk/3_plan.md (10.841 car)
-->

# Plan técnico — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 3_plan.md va en negrita y sin enlace. -->

---

## 1. La pila: qué cambia y qué no

<!-- En la v1 esto es «el stack». De la v2 en adelante lo valioso es decir
     QUÉ NO CAMBIA: una versión que no agrega un paquete es una versión que
     no se desvió. -->

| | |
|---|---|
| **Lo que se agrega a la pila** | {{NADA_O_LO_QUE_SEA}} |
| **Lo que sigue igual** | {{LA_LISTA}} |

> {{POR_QUE_NO_HACE_FALTA_NADA_NUEVO}}

<!-- Si la versión NO agrega ni un paquete, dígalo con esas palabras y explique
     por qué eso es una buena noticia: significa que la decisión de la v1
     alcanzaba. -->

## 2. La estructura y las capas

```
{{ESTRUCTURA_DE_CARPETAS}}
```

### El flujo de una petición, de punta a punta

```
{{HTTP_→_CONTROLADOR_→_SERVICIO_→_REPOSITORIO_→_BASE_DE_DATOS}}
```

| Paso | Quién | Qué hace | Qué tiene PROHIBIDO |
|---|---|---|---|
| 1 | {{...}} | {{...}} | {{...}} |

<!-- OBLIGATORIO en la v1, porque es donde se aprende. De la v2 en adelante
     basta recordarlo y señalar lo que cambia. -->

> {{LA_REGLA_QUE_NO_SE_NEGOCIA}}

## 3. Lo que esta versión agrega, archivo por archivo

<!-- OBLIGATORIO, y es el corazón del documento. UNA SUBSECCIÓN POR PIEZA, con
     sus archivos y sus rutas REALES. Una tabla con nombres genéricos —«el
     controlador», «el servicio»— no le sirve a quien va a escribirlos. -->

### 3.1 {{LA_PRIMERA_PIEZA}}

| Archivo | Qué hace |
|---|---|
| `{{RUTA/ARCHIVO}}` | {{...}} |

{{LO_QUE_HAY_QUE_SABER_DE_ESTA_PIEZA}}

### 3.2 {{LA_SEGUNDA_PIEZA}}

{{...}}

### 3.{{n}} El ensamblador — {{QUE_LE_PASA_EN_ESTA_VERSION}}

<!-- Casi siempre es «lo único de la versión anterior que crece». Y es la
     casilla que más se olvida: sin registrar la dependencia, el proyecto
     COMPILA y falla el día que alguien usa el endpoint. -->

```{{LENGUAJE}}
{{LAS_LINEAS_QUE_SE_AGREGAN}}
```

### 3.{{n}} Las interfaces gráficas

<!-- OBLIGATORIO si la versión tiene pantallas, y va en el plan con el mismo
     detalle que la API. Media versión no es una versión. -->

{{...}}

## 4. Las decisiones de diseño, con su razón

<!-- UNA SUBSECCIÓN POR DECISIÓN. Cada una con TABLA DE OPCIONES y la elegida
     marcada ✅ — porque lo que enseña no es la decisión, son las descartadas,
     y hay que escribirlas con su MEJOR argumento.

     Y cuando se pueda poner un número, póngalo: «diez esperas de 10 segundos
     son minuto y medio para abrir una página» convence; «es más eficiente»
     no. -->

### 4.1 {{LA_DECISION_QUE_DEFINE_LA_VERSION}}

| Opción | Qué pasa |
|---|---|
| **{{OPCION_A}}** | {{SU_MEJOR_ARGUMENTO}}. Y {{LO_QUE_CUESTA}} |
| **{{LA_ELEGIDA}}** ✅ | {{POR_QUE_ESTA}} |

> {{LA_CONSECUENCIA_QUE_SE_VE_DESPUES}}

### 4.2 {{LA_SEGUNDA_DECISION}}

{{...}}

### 4.{{n}} {{LA_PREGUNTA_INCOMODA_QUE_ALGUIEN_VA_A_HACER}}

{{LA_RESPUESTA}}

<!-- RECOMENDADO y muy agradecido. En el ejemplo responde «¿para qué un
     servicio que no valida nada?» — porque el día que haga falta una regla
     hay dónde ponerla, y si el controlador le hablara directo al repositorio
     esa regla terminaría en el controlador. La capa no se salta porque hoy
     esté vacía. -->

## 5. Los {{CUANTOS}} tropiezos que esta versión tiene preparados

<!-- OBLIGATORIO de la v2 en adelante, y es la sección que más tiempo ahorra.
     LO QUE LA HACE VALIOSA: separar los que FALLAN de los que NO FALLAN. Un
     error que revienta se arregla en cinco minutos; uno que responde 200 con
     un dato equivocado cuesta una tarde.

     Si no conoce ninguno todavía, deje la sección y vuelva cuando aparezca. -->

| | Qué pasa | Cómo se ve |
|---|---|---|
| **1** | {{...}} | {{...}} |
| **2** | {{...}} | {{...}} |

> **{{CUANTOS_FALLAN_EN_SILENCIO_Y_POR_QUE_ESO_IMPORTA}}**

<!-- Y si alguno apareció MIDIENDO y no leyendo, dígalo: es la clase de dato
     que no está en ninguna documentación del lenguaje ni del motor. -->

## 6. Chequeo de constitución

<!-- OBLIGATORIO. Es la compuerta 2 del método: el plan se cuadra contra la
     constitución ANTES de construir. Una fila por artículo que esta versión
     toca. -->

| Artículo | Qué exige | ¿Lo cumple este plan? |
|---|---|---|
| {{N}} | {{...}} | ✅ {{COMO}} |
| {{N}} | {{...}} | ✅ {{COMO}} |

> {{SI_ALGUN_ARTICULO_QUEDA_EN_TENSION_DECIRLO_AQUI}}

## 7. Lo que este plan deja FUERA, a propósito

<!-- OBLIGATORIO. Cada fila con su razón, y la buena razón es casi siempre
     «el problema todavía no existe» — no «no tuvimos tiempo». -->

| | Por qué |
|---|---|
| **{{COSA_1}}** | {{LA_RAZON}} |
| **{{COSA_2}}** | {{LA_RAZON}} |
| **{{LO_PENDIENTE_DEL_ALCANCE}}** | **Pendientes**, y declaradas como tal en [2_spec.md](2_spec.md) |
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

# Constitución del proyecto

> **Documento permanente.** Estas reglas rigen TODAS las versiones del
> curso. Cada versión tiene además su propia especificación en
> [versiones/](versiones/0_mapa_versiones.md); ante conflicto, la
> constitución gana.

---

## Artículo 1 — Construcción incremental y acumulativa sin anticipación

- **Versiones incrementales:** v1, v2, … Cada una cuenta con su spec kit
  propio. Una versión se considera TERMINADA solo cuando cumple sus criterios
  de aceptación; en ese momento se realiza commit, tag (`v1`, `v2`, …) y
  únicamente después se escribe la especificación siguiente.
- **Versiones acumulativas y YAGNI:** la v2 contiene a la v1, exigiendo prueba
  de regresión en cada entrega. Se prohíbe la anticipación (*You Aren't Gonna
  Need It*): no se deben incluir fábricas multi-motor, capas «por si acaso» ni
  tablas adicionales antes de la versión que las requiera. El código de cada
  versión solo puede nombrar lo que su especificación explicita.
- **Estado del repositorio:** el repositorio contiene siempre la versión en
  curso, funcionando.

## Artículo 2 — Stack: C# y ASP.NET Core

- Lenguaje **C#** sobre **ASP.NET Core (.NET 10)**: controladores con
  atributos, inyección de dependencias del framework, `async`/`await` en todo
  el acceso a datos e interfaz gráfica con Blazor Server.
- **Acceso a datos sin ORM de entidades** — y **por qué**: el SQL se escribe a
  mano, visible y siempre parametrizado (`@parametro`) para mantener el control
  explícito sobre las consultas y evitar abstracciones innecesarias. Se utiliza
  **Dapper** como micro-ejecutor sobre ADO.NET para mapear filas a objetos sin
  generar SQL jamás.
- Paquetes externos permitidos: `Microsoft.Data.SqlClient`, `Dapper` y
  `Swashbuckle.AspNetCore` (Swagger) — y **ninguno más** sin que una
  especificación lo pida.

## Artículo 3 — Arquitectura en capas, desde el día 1

```
Controlador  →  IServicio   →  IRepositorio
                    ↑                ↑
                Servicio        Repositorio
```

- **Incompatibilidades y aislamiento:** el controlador no puede escribir SQL ni
  tomar decisiones de negocio; el servicio no puede saber qué es un código 404
  ni qué motor existe debajo; el repositorio no decide nada y ejecuta el SQL y
  los procedimientos. En el front no puede haber una sola conexión a la base de
  datos (`SqlConnection`).
- **Solo el ensamblador** decide **qué implementación** se usa: la fábrica
  (`Program.cs` / `IFabricaRepositorios`) es el único punto que elige el motor.
  Todo lo demás recibe sus dependencias por constructor e interfaces
  (`IRepositorio*`), nunca instancia implementaciones de las que existan
  alternativas.
- **Traducción de errores:** las validaciones de forma se rechazan en la
  petición y el controlador; las excepciones del negocio y del servicio
  (`ArgumentException`, `NoEncontradoExcepcion`, `ConflictoExcepcion`) viajan
  hasta el controlador, donde se traducen a códigos HTTP (400, 404, 409); las
  violaciones de la base de datos caen en el `catch` final como 500.
- **Lo que NO tiene esta arquitectura:** sin Entity Framework ni ORM, sin
  caché, sin colas, eventos ni mensajería —todo es síncrono dentro de la
  petición, sin perjuicio de que el acceso a datos sea asincrónico con
  `async`/`await`—, sin microservicios y sin repositorio genérico
  `Repositorio<T>`.

## Artículo 4 — Un solo comando

`docker compose up -d --build` deja TODO el sistema de la versión funcionando,
desde la primera versión, sin requerir SDK ni instalaciones locales
adicionales.

## Artículo 5 — Diseño de la base de datos en fase 0

Las 12 tablas y sus restricciones vienen dadas del curso de Bases de Datos
—**89 líneas de código**— y **se copian, no se generan**. Todo lo que convierte
ese modelo en un sistema que defiende sus reglas —los 3 disparadores y 16
procedimientos almacenados, **760 líneas de código, el 89 % del script**— se
escribió en este proyecto en la fase 0.

> **Razón de diseño:** la base de datos se diseña una sola vez en la fase 0
> porque un modelo cambiante por entrega obligaría a migrar datos y reescribir
> procedimientos. **Ninguna IA genera el esquema** porque el modelo ya está
> definido a partir del proceso de elicitación, y la IA no participó en dicha
> etapa.

## Artículo 6 — Todo en español, comentado para principiantes

- Nombres, rutas, mensajes, comentarios y documentación: **en español**.
- El código lleva **comentarios línea a línea**: qué significa cada
  construcción del lenguaje y para qué sirve aquí. El repositorio es material
  de estudio, no solo software.

## Artículo 7 — Contratos exactos

Los endpoints, formatos y códigos de estado de cada versión están en su
`6_contracts.md` y se cumplen **al pie de la letra**, garantizando respuestas
consistentes e impidiendo que cada versión invente sus propios códigos.

## Artículo 8 — Convenciones fijas

| Cosa | Convención |
|---|---|
| Puertos del proyecto | API **8035** · Front **8099** · SQL Server **11466** · PostgreSQL **15462** — ninguno se repite, y el registro es [`PUERTOS.md`](../../../PUERTOS.md) |
| Rutas | Diagnóstico `/` · Documentación interactiva `/swagger` · Recursos `/api/{recurso}` |
| Nombres | C#: PascalCase en español · Interfaces: prefijo `I` · Base de datos: minúsculas sin separadores (`productosporfactura`) · Claves foráneas: prefijo `fk` (`fkidcliente`) · API: minúsculas con guion (`/api/rol-usuario`) · Procedimientos `sp_` · Disparadores `trg_`. Carpetas: `Controllers/`, `Modelos/`, `Peticiones/`, `Servicios/`, `Repositorios/`, `Excepciones/`, `pruebas/` |
| Sobre de respuesta | Lecturas: `{tabla, limite, total, datos}` · Errores: `{estado, mensaje, detalle}` (y `errores[]` en el 422) |
| Errores | **200** éxito · **204** sin contenido · **400** regla de negocio sobre los parámetros, por el servicio · **401** identidad no autenticada · **403** autenticado sin permiso · **404** recurso no encontrado, por el servicio · **405** verbo no permitido en la ruta · **409** conflicto con el estado del recurso · **422** cuerpo mal formado o incompleto, por ASP.NET · **500** fallo no traducido o rechazo de la base de datos |
| Credenciales (didácticas) | Usuario `sa` · Clave `Paradigmas123!` · Base `bdfacturas` |

# Especificación — Versión 1: Las seis tablas sin clave foránea

| Documento | Versión | Estado |
|---|---|---|
| **2_spec.md** | **v1.0** | **Definitivo** |

---

> **RECORTE:** de este 2_spec se le pasan solo las tres secciones que el
> plan necesita: el alcance, los requisitos y los criterios.

---

## 2. Alcance

> **Qué define la v1:** **Operar los cinco verbos HTTP sobre las tablas que no tienen clave foránea, tanto desde la API como desde la interfaz gráfica**.
>
> **Las tablas con clave foránea NO son de esta versión: son de la v2**, porque requieren el manejo de conflictos (código 409), controles de interfaz tipo desplegable y transacciones compuestas. Si alguna aparece aquí como trabajo nuevo, es que **la v1 quedó incompleta**.

**Incluye:**

- **Los seis recursos sin clave foránea**: `producto`, `empresa`, `persona`, `rol`, `ruta` y `usuario`.
- **Los cinco verbos HTTP por recurso**: Listar (`GET`), Consultar por clave (`GET`), Crear (`POST`), Reemplazar completo (`PUT`) y Actualizar parcial (`PATCH`), además de Eliminar (`DELETE`).
- **Las interfaces gráficas**: Seis pantallas dedicadas en Blazor Server con dirección propia (por ejemplo, `/productos`, `/personas`).
- **El diagnóstico**: Endpoint `/` que informa si la API está viva y qué versión ejecuta.

**No incluye, y es deliberado:**

| | Dónde está |
|---|---|
| **Claves foráneas y relaciones (`factura`, `productosporfactura`, etc.)** | La **v2**. Requieren validación de integridad referencial, selects desplegables y transacciones. |
| **Código de estado 409 ante llaves duplicadas** | La **v2**. En la v1 una llave repetida en `POST` responde 500 desde la base de datos de forma deliberada. |
| **Control de acceso, autenticación y contraseñas cifradas** | La **v3**. En la v1 la tabla `usuario` opera como un CRUD simple sin hashing ni verificación de tokens. |
| **Reportes y consultas complejas del negocio** | La **v4**. Requieren cruces de datos agregados que un CRUD simple no responde. |
| **Soporte para segundo motor de base de datos (PostgreSQL)** | La **v5**. En la v1 solo se soporta SQL Server mediante inyección directa en `Program.cs`. |

---

## 3. Requisitos funcionales

### RF1 — Listar recursos con límite opcional
El sistema debe permitir listar los registros de cualquiera de los seis recursos independientes (`producto`, `empresa`, `persona`, `rol`, `ruta`, `usuario`).
Recibe la petición HTTP `GET /api/{recurso}` con un parámetro opcional en la URL `?limite=N`. Responde con un sobre estructurado con los campos `tabla`, `limite`, `total` y la lista de `datos`.

> Defiende la consistencia en las lecturas masivas. Si no se especifica un límite, el servicio aplica el valor por defecto configurado.

### RF2 — Consultar un recurso por su clave
El sistema debe permitir recuperar un único registro mediante su identificador.
Recibe `GET /api/{recurso}/{id}`. Si el recurso existe, responde con el objeto correspondiente y código HTTP 200. Si no existe, responde con código HTTP 404 y un sobre de error `{estado, mensaje, detalle}`.

> Defiende que la ausencia de un recurso sea comunicada explícitamente mediante excepciones del dominio (`NoEncontradoExcepcion`) traducidas a HTTP 404.

### RF3 — Crear un recurso
El sistema debe permitir registrar una nueva entidad en cualquiera de los seis recursos independientes.
Recibe `POST /api/{recurso}` con el cuerpo JSON correspondiente a la petición de creación (ej. `ProductoCrear`). Si los datos son válidos, inserta el registro y responde 200/201. Si faltan datos obligatorios de forma, ASP.NET rechaza la petición con código HTTP 422 y la lista completa de campos faltantes.

> Si se intenta crear un registro con una clave duplicada, en esta versión el sistema devuelve deliberadamente un código HTTP 500 debido al rechazo directo de la restricción de la base de datos. La conversión a 409 es una lección reservada para la v2.

### RF4 — Reemplazar completamente un recurso (PUT)
El sistema debe permitir sobreescribir todos los campos de un registro existente.
Recibe `PUT /api/{recurso}/{id}` con la estructura completa de reemplazo (ej. `ProductoReemplazo`). Los campos no enviados o enviados en nulo sobreescriben los valores almacenados dejando dichos atributos vacíos.

> Muestra la diferencia conceptual con el PATCH: el modelo de petición `Reemplazo` aplica anotaciones `[Required]` a los campos obligatorios del recurso; si el cuerpo no los incluye, responde 422.

### RF5 — Actualizar parcialmente un recurso (PATCH)
El sistema debe permitir modificar únicamente los campos especificados en el cuerpo de la petición.
Recibe `PATCH /api/{recurso}/{id}` con el cuerpo de actualización (ej. `ProductoActualizar`). Si se envía un JSON vacío `{}` no realiza cambios pero la regla de negocio del servicio rechaza la operación devolviendo código HTTP 400.

> Defiende la regla de negocio que establece que "actualizar nada" es una operación inválida. A diferencia del PUT, en el PATCH los campos omitidos conservan sus valores originales.

### RF6 — Eliminar un recurso
El sistema debe permitir remover un registro físicamente de la base de datos en los seis recursos independientes.
Recibe `DELETE /api/{recurso}/{id}`. Si el registro existe, se elimina y confirma la operación. Si no existe, devuelve código HTTP 404.

> Permite la limpieza de datos en las seis tablas independientes. Las restricciones de borrado lógico o protección por facturación corresponden a entidades con claves foráneas.

### RF7 — Diagnóstico del sistema
El sistema debe informar su estado de disponibilidad y la versión que se encuentra ejecutando.
Recibe `GET /`. Responde con un objeto JSON que contiene el estado del servicio y el identificador de la versión ("v1").

> Permite la verificación rápida del estado de la API mediante herramientas de monitoreo o pruebas de humo automatizadas.

### RF8 — Pantallas dedicadas por recurso en la interfaz gráfica
El sistema debe proveer una interfaz web responsiva construida en Blazor Server con una pantalla específica para cada uno de los seis recursos.
Cada pantalla responde en una ruta navegable propia (`/productos`, `/empresas`, `/personas`, `/roles`, `/rutas`, `/usuarios`) y permite listar, crear, editar y eliminar registros usando los endpoints de la API.

> Se prohíbe el uso de rutas genéricas parametrizadas como `/tabla/{nombre}`. Cada recurso debe contar con su propia dirección navegable para permitir el enlazado directo y la gestión de permisos en versiones futuras.

---

## 5. Criterios de aceptación

### La API

| # | Criterio | Cómo se comprueba |
|---|---|---|
| **1** | **Diagnóstico:** El endpoint raíz responde la versión activa | Correr `Invoke-RestMethod -Uri http://localhost:8035/` y verificar que retorne estado activo y versión "v1". |
| **2** | **Documentación:** Swagger está publicado y operativo | Abrir `http://localhost:8035/swagger` en un navegador y confirmar que devuelva código HTTP 200 con las 37 operaciones de los 6 recursos. |
| **3** | **Validación de forma (422):** Datos incompletos se rechazan en el controlador | Enviar `POST /api/producto` con un cuerpo JSON vacío `{}`. Confirmar respuesta HTTP 422 con un sobre que contenga la lista de campos requeridos faltantes. |
| **4** | **Regla de actualización vacía (400):** Modificación sin datos es rechazada por el servicio | Enviar `PATCH /api/producto/PR001` con un cuerpo JSON `{}`. Confirmar que la API responda HTTP 400 indicando que no se enviaron campos para actualizar. |
| **5** | **Recurso inexistente (404):** Búsqueda de id inexistente devuelve sobre de error | Enviar `GET /api/producto/NOEXISTE`. Confirmar código HTTP 404 con la estructura `{estado: 404, mensaje: "...", detalle: "..."}`. |
| **6** | **Comportamiento explícito de llave duplicada (500):** En la v1 la duplicidad la ataja la BD | Enviar `POST /api/producto` utilizando un código de producto ya existente. Confirmar que responda HTTP 500 debido al rechazo directo de la restricción `PRIMARY KEY`. |
| **7** | **Cobertura de verbos:** Los 6 recursos ejecutan CRUD completo | Ejecutar de forma secuencial `GET`, `POST`, `GET {id}`, `PUT`, `PATCH` y `DELETE` sobre `/api/empresa`, `/api/persona`, `/api/rol`, `/api/ruta` y `/api/usuario`. Verificar respuestas conforme al contrato. |

### Las interfaces gráficas — **la versión no está cerrada sin estos**

| # | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **8** | Navegar directamente a cada una de las 6 direcciones en la barra del navegador (`/productos`, `/personas`, `/empresas`, `/roles`, `/rutas`, `/usuarios`). | La pantalla carga la interfaz correspondiente, muestra el título del recurso y lista las filas existentes obtenidas desde la API. |
| **9** | En la pantalla `/productos`, diligenciar el formulario de creación con datos válidos y pulsar "Guardar". | La interfaz envía el `POST` a la API, la tabla se actualiza automáticamente mostrando el nuevo producto en la lista sin recargar la página completa. |
| **10** | Detener el contenedor de la API (`docker stop api_facturas`) manteniendo encendidos la base de datos y la interfaz gráfica, y recargar `/productos`. | La interfaz gráfica se mantiene en pie, muestra la estructura de navegación de la aplicación y presenta un aviso en pantalla indicando que el servicio no está disponible, sin mostrar filas en la tabla. |

> **Los criterios 8, 9 y 10 no se pueden simular con maquetas estáticas:** el criterio 8 valida el enrutamiento independiente en Blazor; el criterio 9 exige la integración real de extremo a extremo con persistencia de datos; el criterio 10 demuestra el cumplimiento del RNF3 de desacoplamiento arquitectónico verificando que no existe conexión directa entre el front y la base de datos.

---

# Arquitectura — los procesos y el despliegue

> **Qué es este documento.** Cuántos procesos hay, cómo se hablan, por qué
> **esto no es un monolito** —ni microservicios—, y qué implica el front que se
> escogió.
>
> **A qué plantilla alimenta:** el `3_plan`.
>
> **Por qué está separado de las capas.** Porque son **dos lectores**: el plan
> necesita los procesos y el despliegue; la constitución necesita las capas y
> sus prohibiciones, que están en
> [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md). Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Material académico simulado**, pero el reparto es real y se puede comprobar
> abriendo las carpetas.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. Tres procesos

| Proceso | Qué es | Puerto |
|---|---|---|
| **`front-blazor`** | Blazor Server. El único que le habla a la persona | 8099 |
| **`api-facturas`** | ASP.NET Core. El único que habla con la base de datos | 8035 |
| **`sqlserver`** | El motor, con **las reglas del negocio adentro** | 11466 |

En la v5 se le suma **`postgres`** (15462), y el interruptor `MOTOR_BD` decide
cuál de los dos atiende.

> **La base de datos NO es un tercero ajeno: es parte del sistema**, y de las
> tres la que más código propio tiene. El detalle está en
> [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §4.

---

## 2. ¿Esto es un monolito?

**Es la pregunta que más se responde mal, y casi siempre por el mismo motivo:
porque todo está en un repositorio.**

> **Un repositorio no es una unidad de despliegue.** El repositorio dice **cómo
> se guarda** el código; la arquitectura dice **cómo se ejecuta**. Son dos cosas
> distintas, y tener todo junto en GitHub se llama **monorepo** — no monolito.

### Las tres palabras, separadas

| | Qué significa | ¿Aplica aquí? |
|---|---|---|
| **Monolito** | **UNA** sola unidad desplegable: todo se compila, se despliega y se cae junto | **Sí** a la API · **Sí** al front · **No** al sistema |
| **Monorepo** | **UN** solo repositorio, con varias unidades adentro | **Sí** |
| **Microservicios** | Muchos servicios pequeños, cada uno con **su propia base**, descubrimiento y despliegue independiente | **No** |

### La respuesta, en una línea

> **La API es un monolito. El front TAMBIÉN. El sistema no.**
>
> Las tres cosas son ciertas **a la vez**, y confundir el nivel es de donde sale
> el error. «Monolito» se predica de **una unidad desplegable** — no de un
> repositorio, no de un sistema entero.

**Por qué la API es un monolito:** es **una** unidad. Sus tres capas y sus
quince controladores viven en el mismo proceso, se compilan juntos y se
despliegan juntos. Si hay que cambiar una línea de `ProductoController`, se
vuelve a desplegar **toda** la API.

**Por qué el front también:** exactamente lo mismo. Sus **15 pantallas** son un
solo proyecto, un solo contenedor y un solo despliegue. Cambiar un color obliga a
volver a publicarlo entero.

**Y por qué el sistema no lo es:** porque son **DOS monolitos**, no uno. Se
compilan por separado, se despliegan por separado, **se caen por separado** —
apague la API y el front sigue en pie— y se hablan **solo por HTTP**.

> **Y aquí está lo que de verdad hay que llevarse: «monolito» no es un insulto.**
> Casi todo software empieza siendo uno, y para este tamaño es **la decisión
> correcta**. Lo que importa no es si algo es monolito, sino **cuántas unidades
> desplegables hay y cómo se hablan**.
>
> Un monolito bien ordenado por dentro —en capas, con interfaces— se llama
> **monolito modular**, y es lo que son estos dos. Partirlos en servicios
> pequeños antes de necesitarlo solo agrega problemas de red a un sistema que
> todavía no los tenía.

**Y por qué no son microservicios:** dos monolitos no son microservicios. Faltan
todas las señas: no hay una base por servicio —hay **una** base compartida—, no
hay descubrimiento, no hay colas, y no hay despliegue independiente de partes de
la API ni del front.

### El nombre que sí le queda

**Arquitectura de tres niveles** *(three-tier)*:

| Nivel | Proceso | Qué decide |
|---|---|---|
| **Presentación** | `front-blazor` | **nada**: pregunta y obedece |
| **Aplicación** | `api-facturas` | la forma de la petición y el flujo |
| **Datos** | `sqlserver` / `postgres` | **el stock, el total, la anulación** |

> **Y aquí hay una diferencia con el three-tier de manual que vale la pena
> notar:** en el libro, el nivel de datos **guarda y ya**. En este sistema
> **defiende las reglas** — por eso la pregunta *«¿dónde va esta validación?»*
> tiene **tres** respuestas posibles y no dos. Ver
> [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §3.

### Cómo se comprueba, que es lo que lo vuelve un hecho

```powershell
# 1 · DOS unidades desplegables propias (y un proyecto de pruebas).
Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch 'obj' }

# 2 · Se despliegan por separado: apague UNA y la otra sigue.
docker compose stop api-facturas
Start-Process http://localhost:8099/facturas
#    El front sigue en pie, con su menú y sin una sola fila.
#    En un monolito eso no se puede: se cae todo junto.

# 3 · Se hablan SOLO por HTTP. Esto tiene que dar 0.
Select-String -Path front_blazor\**\*.cs,front_blazor\**\*.razor `
  -Pattern 'SqlConnection|Npgsql' | Measure-Object | Select-Object Count
```

> **El paso 2 es la prueba.** Si apagar la API tumbara también el front, serían
> una sola unidad — y entonces sí sería un monolito, por más carpetas separadas
> que tuviera.

---

## 3. El front: por qué Blazor Server y qué implica

| | |
|---|---|
| **Qué es** | El componente vive **en el servidor**; el navegador recibe HTML y mantiene un **circuito** abierto |
| **Qué gana** | No hay que escribir JavaScript para la interacción, y el estado de la pantalla vive en C# |
| **Qué cuesta** | Si el circuito se corta —se recarga la página, se pierde la red— **la pantalla pierde su estado** |

> **Eso último se nota al emitir una factura:** los renglones que se van
> agregando viven **en el circuito**, no en la base de datos. Oprimir F5 a mitad **los
> pierde**.
>
> **No es un defecto: es la consecuencia de haber escogido Blazor Server.** Si
> hiciera falta que el borrador sobreviviera, habría que guardarlo en otra parte
> —la sesión, el navegador o la base de datos—, y eso es trabajo que esta versión no
> hizo. Está declarado, sin construir, en
> [`3_HISTORIAS_PROPUESTAS.md`](elicitacion/3_HISTORIAS_PROPUESTAS.md).
# Requisitos no funcionales — Facturación (`bdfacturas`)

> **Qué es este documento.** Lo que el sistema tiene que **ser**, no lo que tiene
> que hacer. Cada uno con un **número y una forma de comprobarlo** — porque un
> requisito no funcional que no se puede medir no es un requisito, es un deseo.
>
> **El concepto detrás:**
> [`CONCEPTOS_REQUISITOS_NO_FUNCIONALES.md`](../conceptos/CONCEPTOS_REQUISITOS_NO_FUNCIONALES.md).
>
> **De dónde salen.** Casi todos están en
> [`1_constitution.md`](../spec_kit/1_constitution.md), que es donde rigen: la
> constitución es el documento de los requisitos no funcionales de este
> proyecto, y aquí se los recoge con su forma de medirlos.
>
> **Material académico simulado.**
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Cómo leer este documento

| Columna | Qué dice |
|---|---|
| **Requisito** | Qué tiene que ser cierto, **con un número** cuando se puede |
| **Cómo se comprueba** | El comando o la observación. Si no hay ninguna, el requisito está mal escrito |
| **Dónde está escrito** | El artículo de la constitución o el documento que lo fija |

> **La prueba de que un RNF está bien escrito es que alguien pueda decir «no se
> cumple» y tener razón.** «El sistema será rápido» no admite esa frase; «la
> mediana de `GET /api/producto` responde por debajo de 200 ms» sí.

---

## 2. Arranque y entorno

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-01** | El sistema entero se levanta con **un solo comando** | `docker compose up -d --build` en una máquina limpia | Art. 4 |
| **RNF-02** | No hace falta instalar .NET, SQL Server ni nada más: **solo Docker** | Una máquina sin SDK levanta el sistema igual | Art. 4 |
| **RNF-03** | El mismo comando en otro computador da **el mismo resultado** | Dos máquinas distintas, mismo `docker compose up` | Art. 4 |
| **RNF-04** | Los puertos son fijos y **no se repiten** con otros proyectos del semestre | API 8035 · front 8099 · SQL Server 11466 · PostgreSQL 15462, contra `PUERTOS.md` | Art. 8 |

---

## 3. Separación y arquitectura

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-05** | El front **no toca la base de datos** | Apague `api-facturas` con la base de datos encendida: el front sigue en pie, con su menú y **sin una sola fila**. Y `grep SqlConnection front_blazor` da **0** | Art. 3 |
| **RNF-06** | Cada capa depende de una **interfaz**, no de una clase | Los servicios reciben `IRepositorio*` por constructor | Art. 3 |
| **RNF-07** | Cambiar de motor **no toca** controladores ni servicios | `git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios`: **vacío** | Art. 3 |
| **RNF-08** | El SQL está **a la vista**, sin ORM que lo genere | No hay Entity Framework en el `.csproj`; hay Dapper | Art. 2 |

---

## 4. Contratos y respuestas

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-09** | **Toda** respuesta es JSON, también el error | Ningún error devuelve HTML | Art. 7 |
| **RNF-10** | El sobre de lectura es `{tabla, limite, total, datos}` | `GET /api/producto` | Art. 8 |
| **RNF-11** | El sobre de error es `{estado, mensaje, detalle}`, más `errores[]` en el 422 | `POST /api/producto` con `{}` | Art. 8 |
| **RNF-12** | La API está **documentada e interactiva** | `http://localhost:8035/swagger` responde 200 | Art. 8 |
| **RNF-13** | Lo que `6_contracts.md` declara es **exactamente** lo que la API publica | `auditar_aysw4_por_version.py`, que compara el contrato contra `swagger.json` | Art. 7 |

---

## 5. Seguridad

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-14** | Las contraseñas se guardan **cifradas**, nunca en claro | `SELECT contrasena FROM usuario`: hashes BCrypt | v3 |
| **RNF-15** | Ninguna respuesta devuelve una contraseña ni su hash | `GET /api/usuario` no trae el campo. La clase `Usuario` **no lo tiene** | v3 |
| **RNF-16** | El login responde **lo mismo** para «el correo no existe» y «la clave está mal» | Dos peticiones, misma respuesta 401 y mismo texto | v3, criterio 2 |
| **RNF-17** | El permiso se consulta **en cada petición**, no se lee del token | Quite un permiso a un rol: surte efecto sin volver a entrar | v3 |
| **RNF-18** | **Cero secretos en el código** | La cadena de conexión llega por variable de entorno; `.env` en `.gitignore` | Art. 4 |

> **RNF-18 tiene una excepción declarada**, y conviene no imitarla: en este
> repositorio las credenciales **están quemadas en el `docker-compose.yml`**, y
> es a propósito — es material de clase y tiene que levantar sin configurar
> nada. En el proyecto de aula **no se vale**, y la rúbrica lo califica.

---

## 6. Idioma y legibilidad

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-19** | Todo en castellano: nombres, comentarios y mensajes | Leer cualquier archivo | Art. 6 |
| **RNF-20** | El código está **comentado** para que un principiante lo siga | Cada archivo abre diciendo qué es y qué papel cumple | Art. 6 |
| **RNF-21** | La interfaz **no habla en jerga**: ni «PUT», ni «422», ni «FK» | Recorrer las pantallas | v1 |

> **RNF-20 se califica desde la v2**, y se evalúa **hablando**: el profesor abre
> un archivo y quien lo entregó cuenta qué hace y por qué está así. Ver la
> rúbrica en `ProyectosDeAula/docs/0_METODOLOGIA.md`.

---

## 7. Accesibilidad e imagen

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-22** | Los colores de la interfaz salen del **manual de marca**, no del gusto de quien programa | Buscar `#rrggbb` fuera de `marca.css`: no debe haber | v4 |
| **RNF-23** | El texto cumple **WCAG AA** (4.5:1) | Calculado en [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) §2 | v4 |
| **RNF-24** | La interfaz es **responsive** | Abrirla a ancho de teléfono | v4 |

> **RNF-22 y RNF-23 NO se cumplían cuando se escribió este documento**, y
> escribirlo fue lo que lo destapó: había **cuatro colores de Bootstrap** en la
> hoja del proyecto, y el borde del campo enfocado daba **2.09** de contraste
> cuando WCAG pide 3.0 para un indicador.
>
> Los dos quedaron corregidos — el foco en azul cordillera (8.76) y los cuatro
> colores saliendo de la paleta. Hoy se cumplen, y §6 del manual dice cómo
> volver a contarlos.
>
> Un documento de requisitos que solo lista lo que ya se cumple no sirve para
> nada: el valor está en que la comprobación se pueda correr y falle.

---

## 8. Lo que este sistema NO promete

| No se promete | Por qué |
|---|---|
| Un tiempo de respuesta | No hay medición de carga, y prometerlo sin medirlo sería inventarlo |
| Alta disponibilidad | Un contenedor por servicio, sin réplicas |
| Respaldo automático | Hay un `backupdb/` para hacerlo a mano |
| Escalar a muchos usuarios a la vez | Blazor Server mantiene un circuito por persona |
| Funcionar sin Docker | Se puede, pero no está soportado ni probado |

> **Esta tabla es tan requisito como las otras.** Un sistema que no dice lo que
> no hace deja que cada quien suponga lo que quiera — y el día que falle, la
> culpa será de la suposición de alguien.
