<!--
PLANTILLA — 6_contracts.md
En Spec Kit oficial: contracts/ — una CARPETA con un archivo por contrato.
Comando /speckit.plan  ·  FASE 1
Aquí es un solo archivo: con un único servicio HTTP se lee mejor que una
carpeta con un archivo dentro. Es una simplificación nuestra, y se declara.

Sale de: RF_V{{N}}.md + CODIGOS_DE_ESTADO.md + ERRORES_DEL_MOTOR.md + GLOSARIO.md
         ** LOS CÓDIGOS DE ESTADO SALEN DE CODIGOS_DE_ESTADO, no se eligen aquí **

ESTRUCTURA CANÓNICA, tomada de las v1, v2 y v3:

   0. Convenciones globales  —o, de la v2 en adelante, «lo que esta versión
      le hace a los contratos que ya existían»
   A, B, C… una sección POR RECURSO, con subsecciones si tiene varias
      operaciones (C1, C2, C3…)
   + La traducción de errores, ACUMULADA
   + Estabilidad: qué de las versiones anteriores NO cambió

Es el documento más largo del kit, y el que más se consulta tecleando.

Ejemplos llenos: ../docs/spec_kit/versiones/v1_sin_fk/6_contracts.md (5.761 car)
                 ../docs/spec_kit/versiones/v2_con_fk/6_contracts.md (13.557 car)
-->

# Contratos — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 6_contracts.md va en negrita y sin enlace. -->

---

## 0. Convenciones globales

<!-- En la v1 esta sección fija las convenciones; de la v2 en adelante dice
     qué le hace esta versión a los contratos que YA existían. -->

| | |
|---|---|
| **Base** | `{{RUTA_BASE}}` |
| **Formato** | JSON siempre, **también en el error** |
| **Sobre de lectura** | `{{FORMA}}` |
| **Sobre de error** | `{{FORMA}}` |
| **{{AUTENTICACION_SI_APLICA}}** | {{...}} |

{{LO_QUE_ESTA_VERSION_LE_HACE_A_LOS_CONTRATOS_ANTERIORES}}

> **{{LA_TRAMPA_DEL_SOBRE}}**

<!-- OBLIGATORIO si el sobre cambia respecto de las versiones anteriores. Y la
     nota que vale: leer la clave equivocada devuelve una lista vacía SIN
     ningún error — un front que espera «tabla» y recibe «consulta» muestra una
     pantalla en blanco y no da error. -->

---

<!-- Y AHORA UNA SECCIÓN POR RECURSO. El formato, repetido:

       ## A. `api/{{recurso}}` — {{qué es}} ({{cuántos}} endpoints)

       y dentro, si tiene varias operaciones, una subsección por cada una:

       ### A1. `VERBO /ruta` — {{qué hace}}

     CUATRO REGLAS:
       1. El JSON de ejemplo va con valores REALES de los datos sembrados, no
          con "string" y 0. Un ejemplo que no se puede copiar y pegar no sirve.
       2. Los nombres de las claves EXACTOS. El nombre de cada campo ES la
          documentación del contrato.
       3. Cada operación dice QUÉ NO SE ENVÍA, si hay campos que el servidor
          calcula o ignora.
       4. Si la operación tiene una trampa —un campo que parece opcional y no
          lo es, un tipo que trunca— va en su sección, no en una nota al final.
-->

## A. `{{RUTA_DEL_RECURSO}}` — {{QUE_ES}} ({{CUANTOS}} endpoints)

### A1. `{{VERBO}} {{RUTA}}` — {{QUE_HACE}}

{{QUE_HACE_EN_LENGUAJE_DEL_NEGOCIO}}

**Recibe:**

```json
{{EJEMPLO_CON_VALORES_REALES}}
```

**Responde {{CODIGO}}:**

```json
{{EJEMPLO_CON_VALORES_REALES}}
```

| Si… | Responde |
|---|---|
| {{EL_CASO}} | **{{CODIGO}}** |
| {{EL_CASO}} | **{{CODIGO}}** |

> {{LO_QUE_NO_SE_ENVIA_O_LA_TRAMPA}}

### A2. `{{VERBO}} {{RUTA}}` — {{QUE_HACE}}

{{...}}

## B. `{{EL_SIGUIENTE_RECURSO}}`

{{...}}

<!-- … hasta completar los {{CUANTOS}} recursos de esta versión.

     SI EL CURSO PIDE TODOS LOS VERBOS en cada recurso —GET, POST, PUT, PATCH
     y DELETE—, van los cinco, cada uno con su sección. Un verbo que falta se
     agrega; no se omite porque «no se usa».

     Y EL CONTRASTE PUT vs PATCH se escribe COMPLETO en los dos:
       PUT   = reemplazo completo → 422 si falta un campo
       PATCH = parcial           → 200, y lo que no se manda no se toca
     Escribir solo uno de los dos pierde la lección. -->

---

## {{n}}. La traducción de errores, acumulada

<!-- OBLIGATORIO, y es ACUMULADA: trae también lo de las versiones anteriores,
     porque un cliente necesita UNA tabla, no cuatro. -->

| Qué pasó | Quién lo decide | Responde | Desde |
|---|---|---|---|
| {{...}} | {{...}} | **{{CODIGO}}** | v{{N}} |

> {{LA_DISTINCION_QUE_SE_CONFUNDE}}

<!-- La confusión más común y la que más vale escribir: una respuesta vacía NO
     es un 404. El 404 dice «eso que pide no existe»; vacío dice «la pregunta
     se pudo responder, y la respuesta es: ninguno». Son dos cosas distintas y
     el front las trata distinto. -->

## {{n}}. Estabilidad

<!-- OBLIGATORIO de la v2 en adelante. Es lo que convierte «acumulativa» en
     algo verificable: qué de las versiones anteriores NO cambió. -->

| Lo de las versiones anteriores | ¿Cambió? |
|---|---|
| Las {{CUANTAS}} operaciones de la v{{M}} | **No.** Mismas rutas, mismos códigos, mismos sobres |
| {{...}} | {{...}} |

> **{{LO_UNICO_QUE_CAMBIA_Y_POR_QUE}}**

<!-- CUANDO EL DOMINIO NO DICE QUÉ CÓDIGO RESPONDER, NO SE ELIGE UNO:
     [NECESITA ACLARACIÓN: ¿qué responde {{la operación}} cuando {{el caso}}?]
     CODIGOS_DE_ESTADO.md es quien decide eso, y si no lo dice, se pregunta. -->
