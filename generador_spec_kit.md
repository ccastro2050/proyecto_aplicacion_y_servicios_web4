# Generador de spec kits

> **Qué es este documento.** Diez prompts, en orden, que convierten la
> **documentación de dominio** en los **nueve documentos del spec kit** de una
> versión, usando las plantillas de
> [`spec_kit_plantillas/`](spec_kit_plantillas/).
>
> **El dominio se entrega por partes, a propósito.** Cada prompt recibe solo
> los dos o tres documentos que alimentan al documento que va a producir. Eso
> no es un truco para gastar menos: es lo que hace que el resultado sirva.
>
>
> Versión 1.0 · 22 de octubre de 2026.

---

## 1. Por qué por partes, con los números medidos

Lo que pesa la documentación de este repositorio, contado el 22 de octubre de
2026:

| | Documentos | Caracteres | Tokens aprox. |
|---|---|---|---|
| **El dominio** (`docs/dominio/`) | 19 | 186.821 | **~51.900** |
| **Un spec kit de una versión** | 10 | 58.666 | **~16.300** |

Pegarlo todo y pedir los nueve documentos de salida es **una conversación de
unos 68.000 tokens**. Técnicamente cabe. Y el resultado es malo por una razón
que no es el tamaño:

> **El noveno documento sale peor que el primero.** Cuando la conversación ya
> lleva cincuenta mil tokens de material, la atención se reparte, y el
> `9_checklist` termina siendo un resumen de lo que se dijo en vez de una lista
> de comprobaciones del sistema.

Y hay una segunda razón, más importante:

> **Ningún documento del spec kit necesita todo el dominio.** El
> `5_data_model` no necesita la política de errores. El `6_contracts` no
> necesita el cronograma. Mandar lo que no se usa no es solo gasto: es ruido
> que compite con lo que sí importa.

**De ahí sale el diseño:** diez conversaciones, cada una con su parte del
dominio. La más cargada recibe **~11.400 tokens**, la más liviana **~5.700**.

---

## 2. El mapa: qué documento de dominio alimenta a cuál

Esto **no se eligió**: está escrito en cada documento del spec kit de este
repositorio, en su bloque «De dónde sale este documento», y el mapa completo
está en [`docs/dominio/MAPA_DE_DOCUMENTOS.md`](docs/dominio/MAPA_DE_DOCUMENTOS.md).

| Fase | Produce | Se le pega | Entrada | Salida |
|---|---|---|---|---|
| **0** | `1_constitution.md` | `ARQUITECTURA_CAPAS` + `REQUISITOS_NO_FUNCIONALES` + `CODIGOS_DE_ESTADO` + `FUENTES_PROCEDENCIA` | ~8.400 | ~1.900 |
| **1** | `2_spec.md` | `RF_Vn` + `REGLAS_DE_NEGOCIO` + `PLAN_Vn` | ~6.500 | ~2.100 |
| **2** | `3_plan.md` | `ARQUITECTURA_PROCESOS` + `REQUISITOS_NO_FUNCIONALES` + el `2_spec` | ~8.300 | ~1.600 |
| **3** | `4_research.md` | `FUENTES_AUTORIDAD` + `PENDIENTES` + el `3_plan` | ~8.300 | ~1.400 |
| **4** | `5_data_model.md` | `DISENO_BD` + `DATOS_DE_PRUEBA` | ~5.700 | ~1.200 |
| **5** | `6_contracts.md` | `RF_Vn` + `CODIGOS_DE_ESTADO` + `ERRORES_DEL_MOTOR` + `GLOSARIO` | ~7.300 | ~2.100 |
| **6** | `7_quickstart.md` | `DATOS_DE_PRUEBA` + `CODIGOS_DE_ESTADO` + el `6_contracts` | ~6.100 | ~1.800 |
| **7** | `8_tasks.md` | `PLAN_Vn` + `PLAN_DE_TRABAJO` + el `2_spec` **§2 y §5** | ~8.900 | ~1.400 |
| **8** | `9_checklist.md` | `PLAN_Vn` + `RF_Vn` + el `2_spec` **§2 y §5** | ~7.000 | ~1.100 |
| **9** | `GUIA_IAn.md` | — **ésta no se genera** | — | ~1.700 |

### El orden no es el numérico puro, y hay una razón oficial

**`4_research` es la Fase 0 de Spec Kit.** Su salida oficial es
*«research.md with all NEEDS CLARIFICATION resolved»*: existe para **averiguar
lo que no se sabe antes de diseñar**, no para levantar el acta al final. Por
eso se genera después del plan y **antes** del modelo de datos.

Y **`2_spec` va de primero** porque es el alcance: los otros siete lo
obedecen.

---

## 3. Las tres reglas que van en todos los prompts

Están escritas dentro de cada prompt de abajo, pero conviene leerlas una vez
aquí porque son lo que separa un spec kit útil de texto verosímil.

### Regla 1 — No se inventan requisitos

> Si el dominio no dice cuánto dura una sesión, el documento sale con
> `[NECESITA ACLARACIÓN: duración de la sesión]` — **no con «una hora»**.

Un `[NECESITA ACLARACIÓN]` es una pregunta para la próxima reunión con el
usuario experto: es trabajo útil. Un dato inventado es una afirmación que nadie
revisa hasta que falla.

Y no es una regla del curso: Spec Kit trae el marcador `NEEDS CLARIFICATION`
en su plantilla de especificación, tiene un comando dedicado a resolverlos
(`/speckit.clarify`) y obliga a su Fase 0 a cerrarlos todos antes de diseñar.

### Regla 2 — No se rediseña lo que ya está diseñado

El modelo de datos salió de la elicitación con el usuario experto. **Una IA que
propone un `CREATE TABLE` está rehaciendo un trabajo hecho, sin haber estado en
esa conversación.** El `5_data_model` describe qué parte del modelo usa la
versión; no lo reinventa.

### Regla 3 — Cada afirmación se puede rastrear

Un criterio de aceptación dice **cómo se comprueba**. Una decisión dice **qué
se descartó**. Un pendiente se declara como pendiente.

> Lo que no se puede rastrear hasta el dominio o hasta una medición, no entra.

---

## 4. Antes de empezar

| | |
|---|---|
| **Tenga el dominio escrito** | El generador no inventa el dominio: lo transforma. Si no hay `REQUISITOS_FUNCIONALES`, no hay `2_spec` |
| **Tenga decidido el alcance de la versión** | Eso es el `PLAN_Vn`, y lo escribe el equipo |
| **Una conversación nueva por fase** | Si se hacen dos fases en la misma conversación, la segunda arrastra el material de la primera y vuelve el problema que esto resuelve |
| **Abra el ejemplo al lado** | [`docs/spec_kit/versiones/v4_aplicativo/`](docs/spec_kit/versiones/v4_aplicativo/) es un spec kit completo y real. Comparar contra él es la forma más rápida de ver si lo generado quedó bien |

> **Un atajo que conviene conocer:** si su proyecto es de un módulo de los que
> están en [`ProyectosDeAula/docs/`](ProyectosDeAula/docs/), su documentación
> de dominio todavía es más corta que la del ejemplo. Entonces caben dos fases
> seguidas en una conversación sin problema. **Empiece de todos modos de a una**
> y júntelas cuando ya sepa cómo sale cada documento.

---

## Fase 0 — `1_constitution.md`

**Se hace UNA VEZ para todo el proyecto**, no por versión. Va en
`docs/spec_kit/1_constitution.md`, **fuera** de `versiones/` — igual que en Spec
Kit oficial, donde vive en `.specify/memory/`.

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `ARQUITECTURA_CAPAS.md` | `docs/dominio/` |
| `REQUISITOS_NO_FUNCIONALES.md` | `docs/dominio/` |
| `1_constitution.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe la CONSTITUCIÓN de un proyecto de software: el documento
de reglas permanentes que rigen todas sus versiones.

MATERIAL (al final de este mensaje):
  1. ARQUITECTURA.md            — las capas y la regla de dependencias
  2. REQUISITOS_NO_FUNCIONALES.md — rendimiento, seguridad, portabilidad
  3. La plantilla 1_constitution.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- NO invente reglas que el material no respalde. Si algo falta, escriba
  [NECESITA ACLARACIÓN: la pregunta concreta] y siga. No lo rellene.
- CADA ARTÍCULO LLEVA SU RAZÓN, en una línea. Un artículo que solo manda
  se incumple en cuanto estorba; uno que explica por qué, se discute.
- Respete la estructura de la plantilla: sus artículos, sus tablas y sus
  encabezados. Los comentarios <!-- --> de la plantilla son
  instrucciones para usted: NO los copie al resultado.
- Reemplace TODAS las marcas {{ASI}}. Si no sabe qué va en una, es un
  [NECESITA ACLARACIÓN], no una suposición.
- El Artículo 8 (convenciones fijas) es el que más se consulta: déjelo
  completo, con puertos, rutas, nombres y forma de las respuestas.

SALIDA: un solo archivo Markdown, el contenido de 1_constitution.md,
sin preámbulo ni explicación posterior.

--- MATERIAL ---
[pegue aquí los tres documentos]
```

### Cómo se comprueba

| | |
|---|---|
| Ocho artículos o menos, y cada uno con su razón | |
| Ninguna marca `{{` sin reemplazar | |
| Ningún comentario `<!--` de la plantilla copiado al resultado | |
| El Artículo 8 tiene los puertos **de su proyecto** | |
| No menciona tecnologías que su dominio no nombra | |

### Lo que suele salir mal

- **Quince artículos.** Casi siempre dos o tres se pueden juntar; una
  constitución que no se lee entera no rige nada.
- **Artículos sin razón**, que es el modo más fácil de que queden como adorno.
- **Tecnología de más**: una IA tiende a agregar el paquete que normalmente
  acompaña al stack. Si el dominio no lo nombra, no va.

---

## Fase 1 — `2_spec.md`

**Es el más importante del kit**, y por eso va de primero: es el alcance, y los
siete siguientes lo obedecen.

### Qué se le pega

| Documento | De dónde sale | Qué aporta |
|---|---|---|
| `RF_V{n}.md` | `docs/dominio/` | qué tiene que hacer el sistema |
| `REGLAS_DE_NEGOCIO.md` | `docs/dominio/` | lo que no puede dejar de ser cierto |
| `PLAN_V{n}.md` | `docs/dominio/` | **qué entra en ESTA versión** y qué se deja |
| `2_spec.plantilla.md` | `spec_kit_plantillas/` | |

### El prompt

```
Usted escribe la ESPECIFICACIÓN de la versión {n} de un sistema de
{SU DOMINIO}. Es un desarrollo incremental: la versión {n} contiene
todo lo de las anteriores y suma lo suyo.

MATERIAL (al final de este mensaje):
  1. RF_V{n}.md               — qué tiene que hacer el sistema
  2. REGLAS_DE_NEGOCIO.md      — lo que no puede dejar de ser cierto
  3. PLAN_V{n}.md              — EL ALCANCE DE ESTA VERSIÓN
  4. La plantilla 2_spec.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- EL ALCANCE LO FIJA PLAN_V{n}.md. Si un requisito funcional no está en
  el alcance de esta versión, NO entra — aunque sea importante.
- NO invente requisitos ni criterios. Lo que falte:
  [NECESITA ACLARACIÓN: la pregunta concreta].
- LOS CRITERIOS DE ACEPTACIÓN SON EL CORAZÓN. Cada uno dice CÓMO SE
  COMPRUEBA, no solo qué debe pasar. Un criterio que no se puede
  comprobar es un deseo: no lo escriba.
- El criterio 1 es LA REGRESIÓN: que todo lo de las versiones
  anteriores siga respondiendo igual.
- La sección «Por qué esto no lo puede hacer lo que ya existe» es
  obligatoria: es lo que justifica que la versión exista.
- Si algo del alcance NO se va a construir, va en «Lo que esta versión
  deja PENDIENTE», declarado como pendiente. Nunca descrito como si
  existiera.
- Respete la estructura de la plantilla. Los comentarios <!-- --> son
  instrucciones para usted: NO los copie.
- Reemplace TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 2_spec.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| **Cada criterio dice cómo se comprueba** | La columna no está vacía ni dice «probando» |
| El criterio 1 es la regresión | |
| Está escrito por qué la versión existe | §1 «Por qué esto no lo puede hacer…» |
| Lo pendiente está declarado | §5, y no descrito como hecho |
| No se colaron requisitos de otras versiones | Cotejar contra `PLAN_V{n}.md` |
| Ninguna marca `{{` ni comentario `<!--` | |

### Lo que suele salir mal

- **Criterios que no se pueden comprobar**: «el sistema es rápido», «la
  interfaz es intuitiva». Se arreglan preguntando «¿con qué comando lo veo?».
- **Alcance inflado**: entra todo el `REQUISITOS_FUNCIONALES` en vez de solo lo
  del `PLAN_V{n}`. Es el fallo más común de esta fase, y el más caro, porque
  las siete fases siguientes heredan el alcance equivocado.
- **La regresión olvidada.** Sin ella, «acumulativa» es una palabra.

---

## Fase 2 — `3_plan.md`

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `ARQUITECTURA_PROCESOS.md` | `docs/dominio/` |
| `REQUISITOS_NO_FUNCIONALES.md` | `docs/dominio/` |
| **El `2_spec.md` de la fase 1** | lo que acaba de generar |
| `3_plan.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe el PLAN TÉCNICO de la versión {n}: el CÓMO. El QUÉ ya
está decidido en el 2_spec que le adjunto, y no se cambia aquí.

MATERIAL:
  1. ARQUITECTURA_PROCESOS.md       — las capas y la regla de dependencias
  2. REQUISITOS_NO_FUNCIONALES.md — lo que el plan tiene que cumplir
  3. 2_spec.md                    — el alcance, ya fijado
  4. La plantilla 3_plan.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- NO amplíe el alcance del 2_spec. Si el plan necesita algo que el
  2_spec no pide, eso es una señal: escríbalo como
  [NECESITA ACLARACIÓN: ...] en vez de agregarlo.
- LA SECCIÓN 1 ES UNA TABLA DE OPCIONES con la elegida marcada ✅, y
  las descartadas van CON SU MEJOR ARGUMENTO. Una tabla donde las
  alternativas son obviamente malas no demuestra que se pensó.
- Las piezas van con su RUTA DE ARCHIVO real, no «el controlador».
- Cuando pueda poner un número, póngalo: «diez esperas de 10 segundos
  son minuto y medio para abrir una página» convence; «es más
  eficiente» no.
- La sección «Lo que este plan deja FUERA» es obligatoria, y cada fila
  lleva su razón.
- Respete la plantilla. Los comentarios <!-- --> son para usted: NO los
  copie. Reemplace TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 3_plan.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| La decisión de fondo está como tabla, con ✅ | |
| Las descartadas tienen argumento a favor | Si ninguna lo tiene, pídalo otra vez |
| Las piezas tienen ruta de archivo | |
| El alcance es el del `2_spec`, ni más ni menos | |
| «Lo que deja FUERA» está, con razones | |

### Lo que suele salir mal

- **Hombres de paja**: las opciones descartadas escritas para perder. Se nota
  porque ninguna tiene una ventaja real.
- **Plan que crece**: aparecen piezas que el `2_spec` no pidió.
- **Rutas genéricas**, que no le sirven a quien va a escribir el código.

---

## Fase 3 — `4_research.md` · **la Fase 0 de Spec Kit**

> **Ojo con el orden.** Este documento tiene el número 4, pero en Spec Kit es
> la **Fase 0** del plan: existe para **resolver lo que no se sabe antes de
> diseñar**. Por eso va aquí y no al final.

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `FUENTES_AUTORIDAD.md` | `docs/dominio/` |
| `PENDIENTES.md` | `docs/dominio/` |
| **El `3_plan.md` de la fase 2** | lo que acaba de generar |
| `4_research.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe la INVESTIGACIÓN de la versión {n}: las decisiones
técnicas, con sus alternativas y lo que se descartó.

MATERIAL:
  1. FUENTES_AUTORIDAD.md — qué se heredó, qué se escribió aquí, qué manda
  2. PENDIENTES.md — lo que se decidió NO hacer, con su razón
  3. 3_plan.md     — las decisiones que este documento sustenta
  4. La plantilla 4_research.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- Una decisión por sección, numeradas D1, D2, D3…
- LAS OPCIONES DESCARTADAS SE ESCRIBEN CON SU MEJOR ARGUMENTO. Un
  research donde las alternativas son malas no demuestra que se pensó:
  demuestra que se justificó lo ya hecho.
- NO invente tropiezos ni mediciones. Si no sabe qué falló, no lo
  escriba. Un tropiezo inventado es peor que ninguno, porque el
  siguiente lector lo va a buscar.
- Cuando una decisión la fije la constitución o el dominio, dígalo y
  cite el documento.
- La sección «Lo que NO se investigó, y por qué» es obligatoria.
- SI QUEDAN PREGUNTAS ABIERTAS, van en una tabla final de
  [NECESITA ACLARACIÓN: ...] con a quién se le pregunta. Ésa es la
  salida esperada de este documento: lo que falta aclarar ANTES de
  diseñar.
- Respete la plantilla. No copie los comentarios <!-- -->. Reemplace
  TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 4_research.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| Secciones D1, D2, D3… una por decisión | |
| Cada descartada tiene su argumento a favor | |
| **Ningún tropiezo inventado** | Si menciona un error, ¿pasó de verdad? |
| «Lo que NO se investigó» está | |
| Las preguntas abiertas están listadas, no resueltas a la ligera | |

### Lo que suele salir mal

- **Tropiezos inventados.** Es la fase donde más pasa, porque el formato los
  pide. Si no hubo ninguno todavía, la sección se deja y se llena cuando
  aparezca — que va a aparecer.
- **Decisiones escritas después de hechas**, con el aire de haber sido
  evaluadas. Se detecta porque no hay ninguna alternativa con una ventaja.

---

## Fase 4 — `5_data_model.md`

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `DISENO_BD.md` | `docs/dominio/` |
| `DATOS_DE_PRUEBA.md` | `docs/dominio/` |
| `5_data_model.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe el MODELO DE DATOS de la versión {n}: qué parte del
modelo usa esta versión y por qué.

MATERIAL:
  1. DISENO_BD.md       — el modelo completo, ya diseñado
  2. DATOS_DE_PRUEBA.md — qué filas se siembran y de dónde salieron
  3. La plantilla 5_data_model.plantilla.md

REGLAS — Y LA PRIMERA ES LA IMPORTANTE:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- NO PROPONGA NI UN CREATE TABLE. El modelo ya está diseñado y salió de
  la elicitación con el usuario experto. Este documento DESCRIBE qué
  parte usa la versión; no lo rediseña. Si cree que al modelo le falta
  algo, escríbalo como [NECESITA ACLARACIÓN: ...] y no lo cambie.
- En castellano. Trato de usted.
- Si esta versión NO toca la base de datos, DÍGALO en la primera línea
  y con esas palabras. Es una afirmación útil, no un documento vacío.
- La tabla de la sección 2 lleva, por cada cosa nueva, LA COLUMNA o LA
  LLAVE que la hace posible — no solo los nombres de las tablas.
- Si el proyecto corre o va a correr contra más de un motor, la sección
  3 dice qué cambia el dialecto y DÓNDE se resolvió. Si una de esas
  diferencias no falla sino que devuelve un valor equivocado, dígalo
  con esas palabras.
- Respete la plantilla. No copie los comentarios <!-- -->. Reemplace
  TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 5_data_model.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los tres documentos]
```

### Cómo se comprueba

| | |
|---|---|
| **Ni un `CREATE TABLE`** | Buscar `CREATE` en el resultado |
| Dice de entrada si toca el modelo o no | |
| La tercera columna nombra la llave, no la tabla | |
| Las tablas que menciona **existen** en `DISENO_BD.md` | |

### Lo que suele salir mal

- **El `CREATE TABLE` propuesto.** Es el fallo más frecuente de esta fase: una
  IA a la que se le da un modelo de datos tiende a mejorarlo. Por eso la regla
  va de primera en el prompt.
- **Tablas que no existen**, con nombres plausibles. Se atrapa cotejando contra
  `DISENO_BD.md`, y vale la pena hacerlo una por una.

---

## Fase 5 — `6_contracts.md`

El más largo del kit, y el que más se consulta mientras se teclea.

### Qué se le pega

| Documento | De dónde sale | Qué aporta |
|---|---|---|
| `RF_V{n}.md` | `docs/dominio/` | qué operación es cada requisito |
| `CODIGOS_DE_ESTADO.md` + `ERRORES_DEL_MOTOR.md` | `docs/dominio/` | **de aquí salen los códigos de estado** |
| `GLOSARIO.md` | `docs/dominio/` | qué significa cada término |
| `6_contracts.plantilla.md` | `spec_kit_plantillas/` | |

### El prompt

```
Usted escribe los CONTRATOS de la versión {n}: las operaciones que
agrega, con su forma exacta de petición y respuesta.

MATERIAL:
  1. RF_V{n}.md               — qué operación corresponde a cada requisito
  2. CODIGOS_DE_ESTADO.md      — QUÉ CÓDIGO RESPONDE CADA FALLO
  3. GLOSARIO.md               — el significado de cada término
  4. La plantilla 6_contracts.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- LOS CÓDIGOS DE ESTADO SALEN DE POLITICA_DE_ERRORES.md. No los elija
  usted. Si el documento no dice qué responder en un caso, escriba
  [NECESITA ACLARACIÓN: ¿qué responde X cuando Y?] y no ponga un código.
- Una sección por operación, con: la ruta y el verbo, una línea de qué
  hace EN LENGUAJE DEL NEGOCIO, y el JSON de ejemplo.
- EL JSON DE EJEMPLO VA CON VALORES REALES de los datos sembrados, no
  con "string" y 0. Un ejemplo que no se puede copiar y pegar no sirve.
- Los nombres de las claves, EXACTOS. El nombre de cada campo es la
  documentación del contrato.
- Si la forma de la respuesta es distinta a la de las versiones
  anteriores, dedíquele una sección al contraste y diga qué pasa si
  alguien lee la clave equivocada.
- Si una operación tiene una trampa —un campo que parece opcional y no
  lo es, un tipo que trunca— va en su sección, no en una nota al final.
- Respete la plantilla. No copie los comentarios <!-- -->. Reemplace
  TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 6_contracts.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| Están **todas** las operaciones de la versión | Contar contra el `2_spec` |
| Cada código de estado aparece en `CODIGOS_DE_ESTADO.md` + `ERRORES_DEL_MOTOR.md` | **Uno por uno** |
| Los JSON tienen valores reales, copiables | |
| Los nombres de las claves coinciden con lo que va a devolver el código | |

### Lo que suele salir mal

- **Códigos inventados**, y es el fallo serio de esta fase: un `409` donde el
  dominio dice `422` se propaga al código y a las pruebas. Vale la pena
  cotejarlos uno por uno; es la comprobación que más rinde de todo el
  generador.
- **JSON de relleno** con `"string"` y `0`, que obliga a inventar los datos de
  prueba después.
- **Operaciones de menos**: se escriben seis de diez y nadie lo nota hasta
  construir.

---

## Fase 6 — `7_quickstart.md`

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `DATOS_DE_PRUEBA.md` | `docs/dominio/` |
| `CODIGOS_DE_ESTADO.md` | `docs/dominio/` |
| **El `6_contracts.md` de la fase 5** | lo que acaba de generar |
| `7_quickstart.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe el ARRANQUE Y LA VERIFICACIÓN de la versión {n}: cómo se
levanta el sistema y cómo se comprueba cada criterio de aceptación.

MATERIAL:
  1. DATOS_DE_PRUEBA.md     — las cuentas y las filas con las que se prueba
  2. CODIGOS_DE_ESTADO.md   — qué código tiene que responder cada caso
  3. 6_contracts.md         — las operaciones que hay que comprobar
  4. La plantilla 7_quickstart.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- UNA SECCIÓN POR CRITERIO DE ACEPTACIÓN del 2_spec, numerada igual y
  titulada «CRITERIO n — …». Ninguno se queda sin su comprobación.
- TODO COMANDO SE EXPLICA: qué hace, para qué sirve y qué efecto tiene.
  Nada de copiar y pegar a ciegas.
- Los comandos en PowerShell usan Invoke-RestMethod, NO curl: el curl
  de PowerShell es un alias de Invoke-WebRequest y no acepta las mismas
  banderas, así que el comando parece correcto y falla por otra razón.
- LAS CREDENCIALES Y LOS DATOS SALEN DE DATOS_DE_PRUEBA.md. No invente
  un usuario ni una clave: una credencial inventada en un quickstart
  hace perder media hora.
- Incluya cómo provocar los RECHAZOS (401, 403, 422, lo que aplique).
  Comprobar que algo se rechaza es la mitad que se olvida.
- La última sección es LA REGRESIÓN, y es obligatoria.
- Si alguna comprobación puede devolver vacío y eso ES pasar, dígalo.
- Respete la plantilla. No copie los comentarios <!-- -->. Reemplace
  TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 7_quickstart.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| Hay una sección por cada criterio del `2_spec` | Contar: tienen que coincidir |
| **Los comandos corren de verdad** | Ejecute uno. Es la comprobación que vale |
| Las credenciales son las de `DATOS_DE_PRUEBA.md` | |
| Están los rechazos, no solo el camino feliz | |
| La regresión está | |

### Lo que suele salir mal

- **Comandos que no corren**: una bandera que no existe, un puerto equivocado,
  `curl` donde va `Invoke-RestMethod`. Se arregla ejecutando, y no hay otra
  forma.
- **Credenciales inventadas.**
- **Solo el camino feliz**, sin un solo rechazo.

---

## Fase 7 — `8_tasks.md`

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `PLAN_V{n}.md` | `docs/dominio/` |
| `PLAN_DE_TRABAJO.md` | `docs/dominio/` |
| **El `2_spec.md` de la fase 1** | |
| `8_tasks.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe las TAREAS de la versión {n}: el orden de construcción,
por fases, cada una terminando en algo que se puede comprobar.

MATERIAL:
  1. PLAN_V{n}.md       — el alcance que estas tareas reparten
  2. PLAN_DE_TRABAJO.md — quién hace qué, y en qué rama
  3. 2_spec.md          — los criterios que hay que satisfacer
  4. La plantilla 8_tasks.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- CADA FASE TERMINA EN UNA VERIFICACIÓN ESCRITA. Una lista de tareas
  sin verificación es una lista de deseos en orden.
- EL ORDEN VA DE ADENTRO HACIA AFUERA: modelos, acceso a datos,
  servicio, controlador, cableado, interfaz. Cada pieza necesita la
  anterior para poder probarse sola.
- Cada casilla es UNA cosa, para que se pueda marcar. «Hacer los
  modelos» no se marca a medias.
- Las tareas llevan la RUTA DEL ARCHIVO.
- Si el proyecto usa inyección de dependencias, EL REGISTRO DE
  DEPENDENCIAS LLEVA SU PROPIA CASILLA, con las líneas exactas y una
  nota de qué falla sin ellas y cuándo. Es el error más frecuente y el
  único que no da ninguna pista al compilar.
- La última fase es la regresión y el cierre, con el tag.
- «Lo que NO se hace en esta versión» es obligatorio, con enlaces a
  dónde está la razón.
- Respete la plantilla. No copie los comentarios <!-- -->. Reemplace
  TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 8_tasks.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| Cada fase tiene su «Verificación:» | |
| El orden va de adentro hacia afuera | |
| El registro de dependencias tiene su casilla | |
| Las casillas son marcables, una cosa cada una | |
| «Lo que NO se hace» está | |

### Lo que suele salir mal

- **Fases sin verificación**, que es lo que convierte la lista en decorado.
- **El cableado olvidado.** Compila, arranca, y falla el día que alguien usa el
  endpoint.
- **Casillas que son tres tareas.**

---

## Fase 8 — `9_checklist.md`

### Qué se le pega

| Documento | De dónde sale |
|---|---|
| `PLAN_V{n}.md` | `docs/dominio/` |
| `RF_V{n}.md` | `docs/dominio/` |
| **El `2_spec.md` de la fase 1** | |
| `9_checklist.plantilla.md` | `spec_kit_plantillas/` |

### El prompt

```
Usted escribe la LISTA DE VERIFICACIÓN de la versión {n}: lo que tiene
que estar antes de cerrarla.

MATERIAL:
  1. PLAN_V{n}.md              — los criterios de aceptación de la versión
  2. RF_V{n}.md               — lo que hay que poder comprobar
  3. 2_spec.md                 — los criterios, ya redactados
  4. La plantilla 9_checklist.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques, NO lo muestre como texto con
  formato, y NO lo acompañe de comentarios fuera del bloque.
- En castellano. Trato de usted.
- CUATRO SECCIONES, EN ESTE ORDEN: El código · La interfaz gráfica ·
  Lo medido, no lo supuesto · Los documentos.
- LA TERCERA SECCIÓN ES LA QUE IMPORTA: cada casilla es una EJECUCIÓN,
  no una lectura. Marque alguna con «(¿se corrió?)» — no es un chiste:
  es la diferencia entre una lista honesta y una lista llena de buena fe.
- La cuarta sección comprueba que las RAZONES estén escritas, nombrando
  la razón concreta. No «está documentado», sino «está escrito por qué
  el cruce no se hace en el navegador, con las tres razones».
- La última casilla de todas: que lo pendiente esté declarado como
  pendiente y no descrito como si existiera.
- Incluya una casilla de que no queda ningún [NECESITA ACLARACIÓN] en
  los ocho documentos de la versión.
- NO invente comprobaciones que el 2_spec no pida.
- Respete la plantilla. No copie los comentarios <!-- -->. Reemplace
  TODAS las marcas {{ASI}}.

SALIDA: un solo archivo Markdown, el contenido de 9_checklist.md, sin
preámbulo.

--- MATERIAL ---
[pegue aquí los cuatro documentos]
```

### Cómo se comprueba

| | |
|---|---|
| Las cuatro secciones, en orden | |
| «Lo medido» pide ejecuciones, no lecturas | |
| «Los documentos» nombra razones concretas | |
| Está la casilla del `[NECESITA ACLARACIÓN]` | |
| Cada casilla se puede responder sí o no | |

### Lo que suele salir mal

- **Una lista de tareas disfrazada**: repite el `8_tasks` en vez de preguntar
  si se comprobó.
- **«Está documentado»** en vez de la razón concreta, que es lo que se pregunta
  en la sustentación.

---

## Fase 9 — `GUIA_IAn.md` · **ésta no se genera**

> **La guía de IA no la puede escribir una IA por usted.** Lleva los prompts
> que de verdad usó **y lo que hubo que corregirle**. Eso solo lo sabe quien
> estuvo en la conversación.

Es el único documento del kit que es obligatoriamente humano — y no es
casualidad que sea el que más pesa en la sustentación.

### Lo que lleva

| | |
|---|---|
| **El índice** `GUIA_IAn.md` | Quién hace qué, quién redacta cada documento, y lo que todos tienen que saber de esta versión |
| **Una por integrante** `GUIA_IAn_<NOMBRE>.md` | Su parte, su prompt completo, y **lo que hubo que corregirle** |

La plantilla de las dos está en
[`spec_kit_plantillas/GUIA_IA.plantilla.md`](spec_kit_plantillas/GUIA_IA.plantilla.md).

### La sección que más vale

La última de cada guía personal: **qué salió, qué estaba mal, cómo lo
detecté.** No se puede llenar antes de usar la herramienta, y no la puede
llenar nadie más.

> Si no hubo nada que corregir, lo más probable es que no se haya revisado, o
> que se haya pedido muy poco. Las dos cosas se ven en la sustentación, y la
> buena noticia es que son fáciles de arreglar: basta volver a mirar el código
> generado con el spec kit al lado.

### Una IA sí puede ayudar aquí

A **ordenar** la guía una vez escrita: poner los prompts en formato, armar la
tabla de correcciones con lo que usted le dicte. No a inventarla.

---

## 5. Al terminar: las tres comprobaciones de la carpeta

Con los nueve documentos generados, antes de dar la versión por documentada:

```powershell
# 1. Ninguna marca de plantilla sin reemplazar.
#    Busca "{{" en todos los .md de la carpeta de la versión.
Select-String -Path "docs\spec_kit\versiones\v4_aplicativo\*.md" -Pattern "\{\{"

# 2. Ningún comentario de plantilla copiado al resultado.
Select-String -Path "docs\spec_kit\versiones\v4_aplicativo\*.md" -Pattern "OBLIGATORIO|OPCIONAL|RECOMENDADO"

# 3. Ninguna aclaración pendiente.
Select-String -Path "docs\spec_kit\versiones\v4_aplicativo\*.md" -Pattern "NECESITA"
```

**Las tres tienen que no devolver nada.** `Select-String` busca un patrón
dentro de archivos —es el equivalente de `grep`— y cuando no encuentra nada no
imprime nada: eso es lo que se busca aquí.

Y la cuarta, que no se puede automatizar y es la que de verdad decide:

> **¿Los nueve documentos hablan de SU sistema?** Si su proyecto no es de
> facturación y aparecen las palabras «factura», «cliente» o «producto», la IA
> se apoyó en el ejemplo del curso. Buscarlas toma diez segundos:
>
> ```powershell
> Select-String -Path "docs\spec_kit\versiones\v4_aplicativo\*.md" -Pattern "factura|cliente|producto"
> ```

---

## 6. Lo que este generador no hace

| | Por qué |
|---|---|
| **No genera código** | Ese es el otro prompt, el que vive en la `GUIA_IAn.md` |
| **No escribe el dominio** | El dominio sale de la elicitación: preguntarle a quien tiene el problema. Una IA que no estuvo en esa conversación no lo puede inventar |
| **No decide el alcance** | Eso es el `PLAN_Vn`, y lo escribe el equipo |
| **No reemplaza las compuertas** | Siguen siendo del equipo: las aclaraciones pendientes, el cotejo contra la constitución y el `9_checklist` |
| **No cubre todo Spec Kit** | Spec Kit tiene diez comandos; aquí se usan cuatro. Los otros seis están nombrados en [`spec_kit_plantillas/0_COMO_SE_USA.md`](spec_kit_plantillas/0_COMO_SE_USA.md) §4 |

---

## 7. Dónde está cada cosa

| | Dónde |
|---|---|
| Las plantillas | [`spec_kit_plantillas/`](spec_kit_plantillas/) |
| Cómo se usan, y el mapa de nombres oficiales | [`spec_kit_plantillas/0_COMO_SE_USA.md`](spec_kit_plantillas/0_COMO_SE_USA.md) |
| El encabezado común de los ocho | [`spec_kit_plantillas/_ENCABEZADO_COMUN.md`](spec_kit_plantillas/_ENCABEZADO_COMUN.md) |
| La documentación de dominio del ejemplo | [`docs/dominio/`](docs/dominio/) |
| Qué documento alimenta a cuál | [`docs/dominio/MAPA_DE_DOCUMENTOS.md`](docs/dominio/MAPA_DE_DOCUMENTOS.md) |
| **Un spec kit completo y real** | [`docs/spec_kit/versiones/v4_aplicativo/`](docs/spec_kit/versiones/v4_aplicativo/) |
| La metodología del curso | [`ProyectosDeAula/docs/0_METODOLOGIA.md`](ProyectosDeAula/docs/0_METODOLOGIA.md) |
