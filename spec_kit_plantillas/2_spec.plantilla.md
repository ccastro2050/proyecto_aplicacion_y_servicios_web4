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
