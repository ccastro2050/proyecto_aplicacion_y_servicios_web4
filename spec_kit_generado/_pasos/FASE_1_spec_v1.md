# V1 · FASE 1 — Generar `2_spec.md`

> Seleccione todo (`Ctrl+A`), cópielo y péguelo en una conversación **nueva**.
>
> **Qué produce:** la especificación de la v1 — **el documento que manda**. Los
> siete siguientes del spec kit lo obedecen, así que si sale mal, salen mal los
> siete. Va en `docs/spec_kit/versiones/v1_sin_fk/2_spec.md`.
>
> **Entrada: ~8.400 tokens.** Salida esperada: ~4.400.
>
> **La plantilla va de primera**, antes del dominio: es lo único que no puede
> faltar, y el final de un pegado largo es lo que se pierde.

---

## EL PROMPT

```
Usted escribe la ESPECIFICACIÓN de la versión 1 de un sistema de
facturación. Es un desarrollo incremental: la v1 es la primera, no
hereda nada, y todo lo que entregue tendrá que seguir funcionando en las
cuatro versiones siguientes.

MATERIAL (después de este prompt, en este orden):
  1. La plantilla 2_spec.plantilla.md — LA ESTRUCTURA QUE DEBE SEGUIR
  2. _ENCABEZADO_COMUN.md — EL BLOQUE QUE VA ARRIBA DE TODO, y que la
                            plantilla le pide: la cinta de la version, la
                            tabla de los nueve documentos de la carpeta y
                            el bloque "De donde sale este documento"
  3. RF_V1.md             — los requisitos funcionales DE LA V1, con su
                            regla y dónde se comprueba cada uno
  4. REGLAS_DE_NEGOCIO.md — lo que no puede dejar de ser cierto, y quién
                            lo defiende
  5. PLAN_V1.md           — EL ALCANCE DE ESTA VERSIÓN: qué entra, qué se
                            deja, en qué orden se construyó y con qué se
                            tropezó

REGLAS:
- ANTES DE ESCRIBIR, REVISE SI LE ALCANZA EL MATERIAL, y dígalo. Son tres
  cosas distintas, y las tres se reportan:
    (a) un documento que el prompt nombra en el MATERIAL y no llegó;
    (b) algo que LA PLANTILLA pide —una sección, una tabla, una marca
        {{ASI}}— y que ningún documento del material responde;
    (c) algo que ESTE PROMPT le exige y que el material no sustenta.
  Pídalo por su nombre antes de empezar: «para la sección X me falta Y».
  NO lo reemplace con lo que usted supondría que dice.
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
  ÚNICAMENTE el documento pedido, para
  poder copiarlo de un clic.
- NADA DE MARCADORES DE CITA. No agregue [cite: 1], [cite: 3, 5] ni
  referencias parecidas a los documentos que le pasé: el documento sale
  limpio, como para publicarlo. Si necesita señalar de dónde salió algo,
  nómbrelo en el texto.
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
- EL ALCANCE LO FIJA PLAN_V1.md. Si un requisito de RF_V1 no está en el
  alcance de esta versión, NO entra — aunque sea importante. Y si algo
  del material pertenece a la v2 o posterior, va en «No incluye, y es
  deliberado» con la versión a la que pertenece.
- LOS CRITERIOS DE ACEPTACIÓN SON EL CORAZÓN DEL DOCUMENTO, y van
  SEPARADOS EN DOS: los de la API y los de LAS INTERFACES GRÁFICAS. No es
  cosmético: una versión cuyos criterios son todos de la API se puede dar
  por terminada sin una sola pantalla. Los de la interfaz van en tres
  columnas: número, qué hacer, qué tiene que pasar.
- CADA CRITERIO DICE CÓMO SE COMPRUEBA, no solo qué debe pasar. Un
  criterio que no se puede comprobar es un deseo: no lo escriba. Y si el
  número importa —un tiempo, una cantidad de filas, un código— póngalo:
  un criterio sin número, cuando el número importa, es opinable.
- LOS REQUISITOS FUNCIONALES VAN UNO POR SUBSECCIÓN —RF1, RF2, RF3…—, no
  en una tabla. Cada uno dice QUÉ tiene que pasar, no cómo programarlo:
  el cómo es el 3_plan.
- LA SECCIÓN DE CLARIFICACIONES es obligatoria: registre cada ambigüedad
  que el material resolvió, con la respuesta Y SU RAZÓN. Una decisión sin
  su razón no se puede revisar después. Si el material deja una
  ambigüedad abierta, la fila lleva [NECESITA ACLARACIÓN: ...].
- SI ALGO FALLA A PROPÓSITO EN ESTA VERSIÓN, declárelo. El material dice
  que crear con un código repetido responde 500 y que eso es deliberado
  —el 409 es la lección de la v2—: eso va escrito, no omitido. Un spec
  kit que esconde un defecto conocido enseña que los documentos no se
  pueden creer.
- NO INVENTE requisitos, criterios ni tropiezos. Lo que no esté en el
  material, se marca.

SALIDA: el contenido de 2_spec.md, dentro del bloque de código, sin
preámbulo ni explicación posterior.

--- MATERIAL ---
```

<!--
PLANTILLA — 2_spec.md
En Spec Kit oficial: spec.md  ·  comando /speckit.specify
Sale de: REQUISITOS_FUNCIONALES.md + REGLAS_DE_NEGOCIO.md + PLAN_V{{N}}.md

ESTRUCTURA CANÓNICA, tomada de las v1, v2 y v3 de este repositorio —no de la
v4, que está comprimida y es la excepción—:

   1. Propósito de la vN
   2. Alcance                    ← Incluye / No incluye, y es deliberado
   3. Requisitos funcionales     ← RF1..RFn, UNO POR SUBSECCIÓN
   4. Requisitos no funcionales  ← RNF1..RNFn
   5. Criterios de aceptación    ← separados: La API / Las interfaces gráficas
   6. Definición de TERMINADA
   7. Clarificaciones            ← C1..Cn, la compuerta 1 del método

ÉSTE ES EL PRIMERO QUE SE GENERA, y el que manda: es el alcance de la versión.
Los otros siete lo obedecen. Si sale mal, salen mal los siete.

Ejemplos llenos: ../docs/spec_kit/versiones/v1_sin_fk/2_spec.md (15.981 car)
                 ../docs/spec_kit/versiones/v2_con_fk/2_spec.md (20.058 car)
-->

# Especificación — Versión {{N}}: {{TITULO_VERSION}}

<!-- Aquí va el ENCABEZADO COMÚN completo. Ver _ENCABEZADO_COMUN.md.
     En este documento, la fila de 2_spec.md va en negrita y sin enlace. -->

---

## 1. Propósito de la v{{N}}

{{QUE_DEJARON_LAS_VERSIONES_ANTERIORES}}

{{Y_QUE_SIGUE_SIN_PODER_HACERSE}}

> **La v{{N}} {{LO_QUE_NO_AGREGA}}.** Agrega **{{LO_QUE_SI_AGREGA}}**.

<!-- El propósito se escribe por CONTRASTE: qué ya se podía, y qué no.
     «La v4 agrega consultas» no dice nada. «Se pueden operar las doce tablas
     y aun así el sistema no responde ninguna pregunta del negocio: cuánto
     vendió cada vendedor no es una fila de ninguna tabla, es un cruce» — eso
     sí. -->

### {{LO_QUE_LLEGA_Y_EN_QUE_ORDEN}}

<!-- OPCIONAL. Cuando la versión trae tres o cuatro cosas distintas, decir en
     qué orden entran y por qué ese orden. -->

## 2. Alcance

<!-- OBLIGATORIO, y es la sección que más discusiones evita. Tres partes
     fijas: qué define la versión, qué incluye, y qué NO incluye con dónde
     está cada cosa. -->

> **Qué define la v{{N}}:** **{{EL_CRITERIO_QUE_LA_DEFINE}}**.
>
> **{{LO_QUE_NO_ES_DE_ESTA_VERSION}} NO es de esta versión: es de la
> v{{OTRA}}**, porque {{LA_RAZON}}. Si alguna aparece aquí como trabajo nuevo,
> es que **la v{{ANTERIOR}} quedó incompleta**.

<!-- Esa segunda frase es la que mantiene las versiones honestas: dice cómo
     saber que algo se colgó de la versión equivocada. -->

**Incluye:**

- **{{LO_PRIMERO}}**, {{CON_SU_DETALLE}}.
- **{{LO_SEGUNDO}}**: {{...}}.
- **{{LAS_INTERFACES_GRAFICAS}}**, {{CON_QUE}}.
- {{EL_CAMBIO_EN_EL_DIAGNOSTICO_O_EQUIVALENTE}}.

**No incluye, y es deliberado:**

| | Dónde está |
|---|---|
| **{{COSA_1}}** | La **v{{M}}**. {{LA_CONFUSION_QUE_HAY_QUE_DESHACER}} |
| **{{COSA_2}}** | **En ninguna.** {{POR_QUE}} |
| **{{COSA_3}}** | La **v{{M}}** |

<!-- La columna «Dónde está» es la que sirve: no basta decir que algo no entra,
     hay que decir en qué versión entra — o que en ninguna, con su razón. -->

## 3. Requisitos funcionales

<!-- OBLIGATORIO. UNA SUBSECCIÓN POR REQUISITO, numeradas RF1, RF2, RF3…
     NO una tabla con todos: cada requisito necesita su espacio para decir qué
     tiene que pasar, qué cuerpo recibe, qué responde y qué regla defiende.

     Y la regla: dicen QUÉ tiene que pasar, no CÓMO programarlo. El cómo es el
     3_plan. -->

### RF1 — {{EL_REQUISITO_EN_UNA_LINEA}}

{{QUE_TIENE_QUE_PASAR}}

{{LO_QUE_RECIBE_Y_LO_QUE_RESPONDE}}

> {{LA_REGLA_DE_NEGOCIO_QUE_DEFIENDE_O_LA_TRAMPA}}

### RF2 — {{...}}

{{...}}

### RF{{n}} — {{LA_VERSION_ANTERIOR_QUEDA_INTACTA}}

<!-- OBLIGATORIO en toda versión menos la primera. La regresión es un
     requisito funcional, no una buena intención. -->

{{QUE_TIENE_QUE_SEGUIR_FUNCIONANDO_IGUAL}}

## 4. Requisitos no funcionales

<!-- Lo que el sistema tiene que SER, no lo que tiene que hacer. Numerados
     RNF1..RNFn. El primero casi siempre es «los de la versión anterior siguen
     todos». -->

- **RNF1 — Los de la v{{N-1}} siguen todos:** {{LA_LISTA_CORTA}}.
- **RNF2 — {{...}}**.
- **RNF{{n}} — Sin anticipación.** {{LO_QUE_NO_SE_ADELANTA_Y_A_QUE_VERSION_PERTENECE}}.

## 5. Criterios de aceptación

<!-- EL CORAZÓN DEL DOCUMENTO, y van SEPARADOS EN DOS: la API y las interfaces
     gráficas. No es cosmético: una versión cuyos criterios son todos de la API
     se puede dar por terminada sin una sola pantalla.

     TRES REGLAS:
       1. Cada criterio dice QUÉ HACER y QUÉ TIENE QUE PASAR.
       2. Los de la interfaz van en tres columnas: número, qué hacer, qué tiene
          que pasar.
       3. El criterio 1 es LA REGRESIÓN.
     Un criterio que no se puede comprobar es un deseo; uno sin número, cuando
     el número importa, es opinable. -->

### La API

| | |
|---|---|
| **1** | **Regresión:** {{EL_COMANDO}} y la prueba de humo **de la v{{N-1}}** pasa completa |
| **2** | {{...}} |
| **{{n}}** | {{...}} |

### Las interfaces gráficas — **la versión no está cerrada sin estos**

| | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **{{n}}** | {{...}} | {{...}} |
| **{{n}}** | {{...}} | {{...}} |

> **{{LOS_CRITERIOS_QUE_NO_SE_PUEDEN_SIMULAR}}** {{POR_QUE}}

<!-- RECOMENDADO: señalar cuáles criterios se podrían aparentar con una
     pantalla bien dibujada y cuáles no. Esos últimos son los que de verdad
     cierran la versión. -->

## 6. Definición de TERMINADA

1. Los **{{CUANTOS}}** criterios en verde, verificados con [7_quickstart.md](7_quickstart.md).
2. La **regresión** de la v{{N-1}} pasa.
3. [9_checklist.md](9_checklist.md) firmado **antes** de la primera línea de código.
4. Commit y **tag `v{{N}}`** en `main`.

## 7. Clarificaciones

<!-- OBLIGATORIO, y es LA COMPUERTA 1 DEL MÉTODO: mientras quede un
     [NECESITA ACLARACIÓN: …] arriba, la versión no pasa a la planeación.

     Aquí se registra cada ambigüedad que se detectó ANTES de planear, con la
     respuesta acordada Y SU RAZÓN. La razón es la mitad del valor: una
     decisión sin razón no se puede revisar después.

     Si el dominio no resuelve una ambigüedad, NO SE INVENTA la respuesta: la
     fila queda con [NECESITA ACLARACIÓN: …] y se lleva a la reunión con el
     usuario experto. -->

> **Qué es esta sección:** el registro de las ambigüedades detectadas **antes**
> de planear, con la respuesta que se acordó **y su razón**. Es la compuerta 1
> del método: mientras quede un `[NECESITA ACLARACIÓN: …]` arriba, esta versión
> no pasa a la planeación.

| # | La pregunta | La respuesta, con su razón | Dónde quedó |
|---|---|---|---|
| C1 | {{LA_PREGUNTA}} | **{{LA_RESPUESTA}}.** {{LA_RAZON}} | {{RF_O_CONTRATO}} |
| C2 | {{...}} | {{...}} | {{...}} |

<!-- NOTA SOBRE EL ORDEN DE LAS SECCIONES 6 Y 7: en este repositorio la v1 pone
     Clarificaciones en §6 y TERMINADA en §7, y la v2 al revés. Las dos
     funcionan; lo que no funciona es que falte una. Elija un orden y manténgalo
     en las cuatro versiones. -->
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
# Requisitos funcionales — Versión 1: las seis tablas sin clave foránea

> **Qué es este documento.** Los requisitos funcionales **de la v1**, con su
> regla y **cómo se comprueba cada uno**.
>
> **A qué plantillas alimenta:** el `2_spec`, el `6_contracts` y el
> `9_checklist` de la v1.
>
> **Por qué está partido por versión.** Porque la conversación que genera el
> spec kit de la v1 **solo necesita los requisitos de la v1**: los de las otras
> cuatro compiten por la atención con el único que importa. Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **El catálogo completo**, los cinco verbos recurso por recurso y la matriz de
> trazabilidad están en
> [`REQUISITOS_FUNCIONALES.md`](REQUISITOS_FUNCIONALES.md).
>
> **Material académico simulado**, pero los endpoints **están medidos contra la
> API corriendo**, no copiados del código.
>
> Versión 1.0 · 4 de octubre de 2026.

---

> **Lo que define la v1 no es «una tabla»: son las tablas que NO tienen clave
> foránea.** En `bdfacturas` son seis — `producto`, `empresa`, `persona`, `rol`,
> `ruta` y `usuario` — y son las primeras porque **se pueden llenar sin que
> exista nada más**.

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v1-1** | Listar, con `?limite=` | `GET /api/producto`, y los otros cinco |
| **RF-v1-2** | Consultar uno por su clave | `GET /api/producto/PR001` |
| **RF-v1-3** | Crear | `POST /api/producto` |
| **RF-v1-4** | **Reemplazar** uno completo — los campos que no se mandan quedan vacíos | `PUT /api/producto/PR001` |
| **RF-v1-5** | **Actualizar** solo lo que se manda | `PATCH /api/producto/PR001` |
| **RF-v1-6** | Eliminar | `DELETE /api/producto/PR001` |
| **RF-v1-7** | Decir si la API está viva, y en qué versión | `GET /` |
| **RF-v1-8** | **Una pantalla por recurso**, con dirección propia | `/productos`, `/personas`, `/empresas`, … |

> **Que los seis recursos sean idénticos salvo los campos ES el punto.** Con uno
> solo nunca aparece la pregunta que la v1 quiere provocar: *«¿y si hago un
> `Repositorio<T>` genérico y me ahorro cinco?»*. La respuesta está en el
> Artículo 10 de la constitución, y hay que haber sentido la repetición para que
> la respuesta signifique algo.

> **RF-v1-4 y RF-v1-5 son el corazón de la versión**, y la diferencia entre los
> dos es la lección: con el **mismo cuerpo** `{"nombre":"X"}`, el PUT deja el
> resto de los campos vacíos y el PATCH no los toca. Quien no haya visto eso
> pasar, no sabe la diferencia — se la cree.

> **Y RF-v1-3 falla a propósito con un código duplicado: devuelve 500.** No es
> un descuido: el 409 es la lección de la v2, y verlo mal primero es lo que hace
> que la corrección signifique algo. Está declarado en el spec de la v1.

> **RF-v1-8 dice «dirección propia» y eso excluye `/tabla/{nombre}`.** Una ruta
> con el nombre de la tabla como parámetro parece ahorro y es lo contrario:
> ninguna pantalla se puede enlazar, ningún permiso se puede dar por recurso.

---

| Qué | Dónde |
|---|---|
| El requisito que manda en esta versión | [`v1_sin_fk/2_spec.md`](../spec_kit/versiones/v1_sin_fk/2_spec.md) |
| El contrato, endpoint por endpoint | [`v1_sin_fk/6_contracts.md`](../spec_kit/versiones/v1_sin_fk/6_contracts.md) |
| Las reglas que estos RF dan por ciertas | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| La versión siguiente | [`RF_V2.md`](RF_V2.md) |
# Reglas de negocio — Facturación (`bdfacturas`)

> **Qué es este archivo.** Lo que es **verdad siempre** en este dominio, sin
> importar qué endpoint se llame. No son requisitos: un requisito es algo que el
> sistema **hace**; una regla es algo que **no puede dejar de ser cierto**.
>
> **La diferencia, explicada:**
> [`CONCEPTOS_REGLAS_DE_NEGOCIO.md`](../conceptos/CONCEPTOS_REGLAS_DE_NEGOCIO.md).
>
> **Cada regla dice quién la defiende**, y esa columna es la mitad del valor del
> documento: una regla que nadie defiende es un deseo.
>
> **Material académico simulado.** El dominio es verosímil y el sistema funciona,
> pero no hay un cliente real detrás: las reglas las fijó el curso. Ver
> [`elicitacion/`](elicitacion/).
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Cómo leer la tabla

| Columna | Qué dice |
|---|---|
| **Quién la defiende** | El mecanismo concreto. Si dice «nadie», la regla **no está defendida** y hay que decirlo |
| **Qué pasa si se viola** | El código que recibe quien lo intenta |

Tres mecanismos, y conviene distinguirlos porque fallan distinto:

- **La base de datos** — una restricción (`PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`,
  `NOT NULL`). No se puede evadir, ni siquiera desde SSMS.
- **Un disparador o procedimiento** — código SQL que rechaza con `THROW`. Tampoco
  se evade, y además explica el motivo.
- **El servicio** — código C#. Solo protege a quien pase por la API.

> **El orden importa.** Una regla defendida por el servicio y no por la base de datos se
> rompe con una línea de SQL. Las reglas que de verdad no pueden fallar están
> abajo del todo, en la base de datos.

---

## 2. La factura

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-01** | Toda factura tiene **un cliente y un vendedor**, y los dos existen | `fk_factura_cliente` y `fk_factura_vendedor` | 500 con el error del motor |
| **RN-02** | Una factura tiene **al menos un renglón**. No existe una factura vacía | `sp_insertar_factura_y_productosporfactura`, con su mínimo y su `THROW` | 500 · *«La factura requiere minimo 1 producto(s).»* |
| **RN-03** | El **número** de la factura lo pone la base de datos y no se repite | `IDENTITY(1,1)` + `pk_factura` | — |
| **RN-04** | La **fecha** es la del momento en que se emitió, y no la manda el cliente | `DEFAULT GETDATE()` | lo que llegue se ignora |
| **RN-05** | Una factura está **activa** o **anulada**. No hay un tercer estado | `DEFAULT N'activa'` y solo `sp_anular_factura` lo cambia | — |
| **RN-06** | El mismo producto **no aparece dos veces** en una factura: se suma la cantidad | `pk_productosporfactura (fknumfactura, fkcodproducto)` | 500 por llave duplicada |

> **RN-02 es la que más se olvida al construir el front**, y por eso la pantalla
> de emitir muestra «Agregue al menos un renglón» en vez de dejar oprimir
> «Emitir» con la lista vacía. La base de datos la rechazaría igual — pero decirlo antes
> es más barato que un 500.

---

## 3. El dinero

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-07** | El **subtotal** de un renglón es `cantidad × valor unitario` **del momento en que se emitió**, y queda congelado | `trg_prodfact_insert` lo calcula y lo guarda | no se puede: el cliente no lo manda |
| **RN-08** | El **total** de la factura es la suma de sus subtotales | `trg_prodfact_insert`, `_update` y `_delete` lo recalculan | no se puede |
| **RN-09** | Ni el subtotal ni el total viajan en la petición: **si llegan, se ignoran** | la lista blanca del controlador | — |

> **RN-07 es una decisión de negocio, no una optimización.** Si el subtotal se
> recalculara con el precio de hoy, subir el precio de un producto **reescribiría
> las ventas del año pasado**. Por eso se guarda aunque sea un dato derivado —
> es el caso que [`CONCEPTOS_DISENO_BD.md`](../conceptos/CONCEPTOS_DISENO_BD.md)
> llama «congelar el derivado porque congelarlo es el requisito».

> **Y RN-08 explica por qué el front muestra un «total estimado».** Lo que se ve
> antes de emitir es un cálculo para que la persona sepa cuánto va. El total que
> queda guardado lo pone el disparador, y es el que manda. Dos fuentes de
> verdad, y gana la de la base de datos.

---

## 4. El inventario

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-10** | No se vende lo que no hay: el stock **nunca queda negativo** | `trg_prodfact_insert` y `_update`, con `THROW 50001` | 500 · *«Stock insuficiente para producto PR003. Stock disponible: 8, cantidad solicitada: 50»* |
| **RN-11** | Emitir una factura **baja** el stock de cada producto | los mismos disparadores | — |
| **RN-12** | **Anular** una factura **devuelve** el stock | `sp_anular_factura` | — |
| **RN-13** | Borrar una factura también devuelve el stock | `trg_prodfact_delete` | — |

> **RN-10 trae el número exacto en el mensaje**, y eso no es cosmético: «stock
> insuficiente» obliga a ir a mirar cuánto hay; «hay 8 y pidió 50» deja a la
> persona corregir sin salir de la pantalla.

---

## 5. Anular, que no es borrar

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-14** | Una factura anulada **se queda en la base de datos**, con su número y su fecha | `sp_anular_factura` hace `UPDATE`, no `DELETE` | — |
| **RN-15** | Una factura **no se anula dos veces** | `sp_anular_factura`, con `THROW 50010` | 409 · *«Factura 3 ya está anulada»* |
| **RN-16** | Anular una factura que no existe no es un conflicto: es que no está | `sp_anular_factura`, con `THROW 50010` | 404 |

> **Por qué una factura no se borra.** Porque la numeración es consecutiva y un
> hueco en ella es una pregunta que alguien va a tener que responder en una
> auditoría. Anular deja el rastro: existió, y se dejó sin efecto.
>
> El `DELETE` existe en la API y **está apagado** por esta razón — vea
> `FacturaController`, que lo trae escrito y comentado.

---

## 6. El control de acceso

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-17** | La contraseña **nunca se guarda en claro** | el repositorio, que cifra con BCrypt antes del `INSERT` | — |
| **RN-18** | La contraseña **nunca sale** en una respuesta | el modelo, que no la tiene, y los `SELECT`, que no la piden | — |
| **RN-19** | Un usuario puede tener **varios roles** | `pk_rol_usuario (fkemail, fkidrol)` | — |
| **RN-20** | Un rol puede tener **varias rutas**, y una ruta puede estar en varios roles | `pk_rutarol (fkidruta, fkidrol)` | — |
| **RN-21** | El **permiso se consulta en cada petición**, no se lee del token | `verificar_acceso_ruta`, llamada por `[ExigePermiso]` | — |
| **RN-22** | La ruta es **única**: no hay dos filas con el mismo nombre de interfaz | `uq_ruta` | 500 por índice único |

> **RN-21 es la que hace que el sistema sea administrable.** Si el permiso
> viajara dentro del token, quitárselo a un rol no surtiría efecto hasta que la
> persona volviera a entrar. Así, surte efecto en la petición siguiente.

> **RN-18 tiene una consecuencia que se ve en el código:** la clase `Usuario` no
> tiene la propiedad `Contrasena`. No es que se filtre al serializar — es que no
> existe el dato que filtrar.

---

## 7. Lo que NINGUNA regla defiende, y hay que decirlo

Un documento de reglas que solo lista lo que está protegido engaña por omisión:

| Lo que NO se valida | Qué pasa |
|---|---|
| Que el **crédito** del cliente alcance para la compra | Se puede facturar por encima del cupo. `credito` es informativo |
| Que el correo de `persona` o de `usuario` **tenga forma de correo** | Se acepta `aaa` |
| Que el **teléfono** tenga forma de teléfono | Igual |
| Que la **cantidad** de un renglón sea razonable | 1.000.000 de unidades se acepta si hay stock |
| Que el **valor unitario** sea mayor que cero | Un producto puede valer 0 |

> **No están validadas a propósito**, para que el proyecto de aula tenga qué
> agregar: cada una es un criterio de aceptación que un equipo puede escribir y
> defender. Lo que no se vale es creer que ya están.

---

## 8. Dónde comprobar cada regla

Ninguna fila de este documento es una opinión:

| Qué | Dónde |
|---|---|
| Las restricciones | [`db/bdfacturas.sql`](../../db/bdfacturas.sql), las `CONSTRAINT` |
| Los disparadores | el mismo archivo, `trg_prodfact_insert`, `_update`, `_delete` |
| Los `THROW` y sus números | el mismo archivo — 50001 es el stock, 50010 el anulado |
| Cómo se traducen a HTTP | [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) |
| El glosario de los términos | [`GLOSARIO.md`](GLOSARIO.md) |
# Plan de la versión 1 — Facturación (`bdfacturas`)

> **Qué es este documento.** Cómo se construyó la v1: con qué insumos, qué se
> decidió **antes** de escribir código, en qué orden, y **con qué se tropezó**.
> Es el relato; el requisito que manda está en
> [`2_spec.md`](../spec_kit/versiones/v1_sin_fk/2_spec.md).
>
> **Para qué sirve leerlo.** Para ver que un plan no es una lista de tareas: es
> una **cadena de decisiones**, y cada una cierra puertas. Las decisiones están
> en §3 con su razón, no solo con su resultado.
>
> **Material académico simulado** en el dominio; el plan y los tropiezos son
> reales y están en el historial.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Lo que queda al terminar la v1

| Queda | Comprobable con |
|---|---|
| **Seis recursos** con los cinco verbos — 37 operaciones | `GET /swagger` |
| **Seis pantallas**, una por recurso, con dirección propia | `http://localhost:8099/productos` |
| Tres capas con interfaces, desde el primer día | `Controllers/` · `Servicios/` · `Repositorios/` |
| SQL escrito a mano y parametrizado | cualquier repositorio |
| **Un solo comando** que levanta todo | `docker compose up -d --build` |
| Swagger funcionando | `http://localhost:8035/swagger` |

> **Media versión no es una versión.** La v1 entrega **API y pantallas**. Si solo
> hay API, la versión no está terminada — está la mitad, y el curso lo dice en el
> spec.

---

## 1. Los insumos

| Insumo | Qué aportó |
|---|---|
| **La constitución** | El stack, las capas, el idioma, el «un solo comando» |
| **El esquema `bdfacturas.sql`** | Las doce tablas **ya hechas** —89 líneas de código— y, con ellas, **760 líneas** de disparadores y procedimientos |
| **El mapa de versiones** | Qué va en la v1 y qué no |

> **Y aquí está la particularidad del proyecto, que conviene tener presente desde
> el principio:** el modelo de datos **no se diseñó**, llegó dado (Artículo 5).
> Lo que crece por versiones es la API. Ver [`FUENTES.md`](FUENTES.md) §0.

---

## 2. El primer hallazgo: **la v1 no es «una tabla»**

El criterio obvio para partir un sistema por versiones es por **tamaño**:
primero una tabla, luego dos, luego el resto. Ese criterio se descartó, y la
razón es la que da forma a todo el curso:

> **La segunda tabla no enseña nada que no enseñe la primera.** Si la v1 es
> `producto` y la v2 es `producto` + `persona`, la v2 es trabajo sin lección: los
> mismos cinco verbos, otros campos.

El corte que quedó es **por concepto**:

| Versión | El concepto que trae |
|---|---|
| **v1** | los cinco verbos, las capas, y **ninguna clave foránea** |
| **v2** | **la clave foránea**, con todo lo que arrastra: el 409, el desplegable, la transacción |
| v3 | **la puerta**: quién entra y a qué |
| v4 | **el aplicativo**: lo que un CRUD no puede responder |
| v5 | **el otro motor**: la prueba de que las capas servían |

Y de ahí sale la definición de la v1: **las tablas que no tienen clave foránea**.
En `bdfacturas` son seis — `producto`, `empresa`, `persona`, `rol`, `ruta` y
`usuario` — y son las primeras porque **se pueden llenar sin que exista nada
más**.

> **Que los seis recursos sean idénticos salvo los campos ES el punto, no un
> descuido.** Con uno solo nunca aparece la pregunta que la versión quiere
> provocar: *«¿y si hago un `Repositorio<T>` genérico y me ahorro cinco?»*. Hay
> que **sentir** la repetición para que la respuesta —el Artículo 10— signifique
> algo. Un estudiante al que le dicen «no uses un genérico» sin haber copiado y
> pegado seis veces, obedece sin entender.

---

## 3. Las decisiones antes de programar

Las ocho están razonadas en
[`4_research.md`](../spec_kit/versiones/v1_sin_fk/4_research.md). Aquí van las
cuatro que más cerraron puertas.

### 3.1 · El SQL a la vista: **Dapper, no Entity Framework**

| Opción | Por qué no / sí |
|---|---|
| Entity Framework | Genera el SQL. **Y el curso es de bases de datos**: si el SQL lo escribe el ORM, el estudiante no lo aprende a leer |
| ADO.NET puro | Enseña el SQL pero cuesta veinte líneas por consulta de plomería que no enseña nada |
| **Dapper** ✅ | **Mapea fila→objeto y JAMÁS genera SQL.** Si una consulta existe, está escrita en un repositorio |

> **La consecuencia que importa:** cada `SELECT` del sistema se puede leer. Y se
> puede copiar a SSMS y correrlo. Con un ORM habría que preguntarle al ORM qué
> escribió.

### 3.2 · **Las tres capas desde el día 1**, y no un MVP en un archivo

Era tentador: la v1 es un CRUD, cabe en un controlador con el SQL adentro.

> **Se descartó por una razón concreta y comprobable:** si la v1 se hace en un
> archivo, la v2 es **reescribirla**. Y lo que el estudiante aprendería es que
> «primero se hace rápido y luego se arregla» — que es justo el hábito que
> produce los sistemas que nadie quiere tocar.
>
> **El costo de hacerlo bien en la v1 es un día. El de arreglarlo en la v5 es el
> proyecto.**

### 3.3 · La validación vive en las **peticiones**, una por verbo

`ProductoCrear`, `ProductoReemplazo`, `ProductoActualizar`: tres clases para un
recurso, y no una.

| Por qué tres | |
|---|---|
| Porque **cada verbo exige cosas distintas** | En `POST` el nombre es obligatorio; en `PATCH` no, porque lo que no se manda no se toca |
| Porque ASP.NET valida **antes** del controlador | Las anotaciones producen un **422 con la lista de errores** sin que el controlador se entere |
| Porque la entidad no es un formulario | de `Producto` se arman los objetos que **viajan**; `ProductoCrear` es lo que **se acepta** |

> **Y de aquí sale una pregunta de sustentación que casi nadie responde bien:**
> con el **mismo cuerpo**, el `PUT` da 422 y el `PATCH` da 200. No es un error:
> es que `ProductoReemplazo` tiene `[Required]` y `ProductoActualizar` no. Ver
> [`SUSTENTACION_DEL_CODIGO.md`](SUSTENTACION_DEL_CODIGO.md) §7.

### 3.4 · **Sin fábrica**, y sin selección de motor

La v5 va a traer dos motores y una fábrica. Y en la v1 **no se escribió nada de
eso**.

> **Es YAGNI aplicado a conciencia** — *You Aren't Gonna Need It*, Artículo 1. El
> ensamblador de la v1 es la sección de inyección de dependencias de
> `Program.cs`, y punto. Escribir la fábrica «porque en la v5 va a hacer falta»
> es construir para un futuro que todavía puede cambiar: de hecho **cambió**, y
> la versión del motor pasó de ser la v4 a ser la v5 (ver
> [`CRONOGRAMA.md`](CRONOGRAMA.md) §2).

---

## 4. El plan, en diez pasos

El orden no es casual: cada paso deja algo **comprobable** antes de seguir.

| # | Paso | Y qué se comprueba antes de pasar al siguiente |
|---|---|---|
| **1** | `docker-compose.yml` con SQL Server y la API | El contenedor del motor acepta conexiones |
| **2** | El esquema se siembra solo al levantar | `SELECT name FROM sys.tables` da 12 |
| **3** | El proyecto de API, con Swagger | `/swagger` abre |
| **4** | El endpoint `/` de diagnóstico | Responde la versión |
| **5** | `Producto`, y sus tres peticiones por verbo | Un `POST` sin nombre da **422 con la lista** |
| **6** | `IRepositorioProducto` → `RepositorioProductoSqlServer` | Un `GET` trae las 8 filas sembradas |
| **7** | `IServicioProducto` → `ServicioProducto` | El servicio no nombra nada de HTTP |
| **8** | `ProductoController`, los cinco verbos | Los cinco responden lo que el contrato dice |
| **9** | **Los otros cinco recursos**, calcando el molde | Las 37 operaciones en Swagger |
| **10** | **Las seis pantallas**, una por recurso | Se crea un producto desde el navegador |

> **Los pasos 5 a 8 son UNA rebanada vertical, y en ese orden.** Primero el
> contrato de entrada, luego el SQL, luego la regla, y el controlador **al
> final**. Hacerlo al revés —empezar por el controlador— obliga a inventar qué
> va a devolver el repositorio antes de haberlo escrito.

> **Y el paso 9 es donde se siente la repetición.** Cinco veces el mismo molde.
> Si en ese momento no da ganas de hacer un genérico, el estudiante no está
> prestando atención; y si las da, ya puede entender por qué el Artículo 10 dice
> que no.

---

## 5. Los tropiezos, y a dónde fue cada corrección

| Tropiezo | Cómo se vio | Dónde quedó la corrección |
|---|---|---|
| **El certificado autofirmado de SQL Server** | `self signed certificate in certificate chain` al conectar desde VS Code | `TUTORIAL_VSCODE_SQLTOOLS.md`, con **cuatro** salidas distintas |
| **El contenedor no volvía tras reiniciar el equipo** | Las bases de datos quedaban apagadas después de reiniciar | `restart: unless-stopped` **también** para las bases de datos |
| **Contenedores huérfanos de otro proyecto** | Puertos ocupados por un repositorio clonado antes | Una advertencia en el README **antes** del `git clone` |
| **Los puertos chocaban con los otros cursos** | Dos proyectos peleando por el 1433 | Puertos propios: API **8035**, SQL Server **11466**, y el registro en `PUERTOS.md` |

> **Ninguno de los cuatro es un error de programación**, y vale notarlo: los
> cuatro son del **entorno**. En un proyecto que se entrega con «un solo
> comando», el entorno **es** parte del producto — y por eso las correcciones
> fueron a la documentación y al `.yml`, no al código.

### Y un tropiezo del propio material

El tutorial de SSMS se escribió primero **de memoria**, y la lección salió mal.
Se rehizo con **capturas reales**, abriendo el programa y haciendo los pasos.

> **Es la misma lección que este curso repite en todos los niveles:** lo que se
> escribe sin correrlo sale mal, y sale mal de una forma que solo se nota cuando
> alguien lo intenta seguir.

---

## 6. Cómo se construye con ayuda de una IA

La v1 se construyó con una IA, y el reparto fue éste:

| Lo hace la persona | Lo hace la IA |
|---|---|
| Escribir la **constitución** y el **spec** | Generar el código que el spec describe |
| Decidir el stack, el corte de versiones, los puertos | Calcar el molde a los otros cinco recursos |
| **Correr el sistema y mirar qué devuelve** | Explicar y **comentar** lo que generó |
| **Firmar las compuertas** | — |

> **La compuerta no la pasa la IA.** `9_checklist.md` lo marca **una persona**,
> y lo marca después de haber **corrido** el sistema. Una IA puede decir que un
> criterio se cumple; solo una persona puede haberlo visto cumplirse.

> **Y desde la v2 la IA tiene una obligación más: comentar el código que
> genera** — y esa **interpretabilidad se califica**, de forma verbal y en
> persona. El profesor abre un archivo y quien lo entregó cuenta qué hace y por
> qué está así. Código que nadie puede explicar no está terminado, aunque corra.

---

## 7. Dónde vamos

La v1 deja seis recursos que **no dependen de nada**. Lo siguiente es
exactamente lo contrario: las seis tablas que **sí** dependen.

> **Y eso no es «más de lo mismo con un campo extra».** Una clave foránea trae
> consigo un código HTTP nuevo —el **409**—, un control nuevo en la interfaz —el
> **desplegable**—, y una operación que **no puede quedar a medias** —la factura
> con sus renglones, en una transacción—. Sigue en [`PLAN_V2.md`](PLAN_V2.md).
