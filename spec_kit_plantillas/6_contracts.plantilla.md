<!--
PLANTILLA — 6_contracts.md
En Spec Kit oficial: contracts/ — una CARPETA con un archivo por contrato.
Comando /speckit.plan  ·  **FASE 1**
Aquí es un solo archivo: con un único servicio HTTP se lee mejor que una
carpeta con un archivo dentro. Es una simplificación nuestra, y se declara.

Sale de: REQUISITOS_FUNCIONALES.md + POLITICA_DE_ERRORES.md + GLOSARIO.md
         ** LOS CÓDIGOS DE ESTADO SALEN DE POLITICA_DE_ERRORES, no se eligen aquí **
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/6_contracts.md

Es el documento más largo del kit, y el que más se consulta tecleando.
-->

# Contratos — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 6_contracts.md va en negrita y sin enlace. -->

---

> Las **{{OPERACIONES_ANTERIORES}} operaciones** de las versiones anteriores
> **siguen vigentes sin un cambio**. La v{{N}} agrega **{{OPERACIONES_NUEVAS}}**.

<!-- Con enlace a los 6_contracts de cada versión anterior. Es lo que convierte
     «acumulativa» en algo que se puede verificar. -->

## {{EL_SOBRE_DE_RESPUESTA}}

```json
{{EJEMPLO_DEL_SOBRE}}
```

<!-- OBLIGATORIO si el sobre de esta versión es DISTINTO al de las anteriores.
     Y la tabla comparativa, porque es donde se equivoca todo el mundo: -->

| {{LO_ANTERIOR}} devuelve | {{LO_NUEVO}} devuelve |
|---|---|
| `{{FORMA_A}}` | `{{FORMA_B}}` |

> **{{POR_QUE_CAMBIA_Y_ES_A_PROPOSITO}}**
>
> **Leer la clave equivocada devuelve {{QUE}} SIN ningún error.** {{Y_POR_QUE_ESO_IMPORTA}}

<!-- Ese segundo aviso es oro: un front que espera «tabla» y recibe «consulta»
     muestra una pantalla vacía y NO da error. Es la clase de fallo que cuesta
     una tarde y se evita con dos líneas de documentación. -->

## Lo que {{TODAS_LAS_OPERACIONES_NUEVAS}} tienen en común

<!-- Esta sección evita repetir diez veces lo mismo más abajo. -->

| | |
|---|---|
| **Método** | {{...}} |
| **{{AUTENTICACION}}** | {{...}}. Sin ella: `401` |
| **{{PERMISO}}** | {{...}}. Sin él: `403` |
| **Parámetros** | {{...}} |
| **`200` con {{RESPUESTA_VACIA}}** | {{CUANDO}}. **No es un 404**: {{POR_QUE}} |
| **`500`** | Solo si {{...}}. Y el cuerpo lleva `detalle` |

> **{{LA_DISTINCION_DE_CODIGOS_QUE_SE_CONFUNDE}}**

<!-- La confusión más común y más vale la pena escribirla: vacío NO es 404.
     El 404 dice «eso que pide no existe»; una respuesta vacía dice «la pregunta
     se pudo responder, y la respuesta es: ninguno». Son dos cosas distintas y
     el front las trata distinto. -->

---

<!-- Y AHORA UNA SECCIÓN POR OPERACIÓN. El formato, repetido:
       ### n. `VERBO /ruta`
       Una línea de qué hace, en lenguaje del negocio.
       **{{Lo que cruza / lo que exige / lo que devuelve}}:** …
       El JSON de ejemplo.

     TRES REGLAS:
       1. El JSON de ejemplo va con valores REALES de los datos sembrados, no
          con "string" y 0. Un ejemplo que no se puede copiar y pegar no sirve.
       2. Los nombres de las claves EXACTOS. El nombre de cada propiedad ES la
          documentación del contrato.
       3. Si la operación tiene una trampa —un tipo que trunca, un campo que
          parece opcional y no lo es— va en su propia sección, no en una nota
          al final.
-->

### 1. `{{VERBO}} {{RUTA}}`

{{QUE_HACE_EN_LENGUAJE_DEL_NEGOCIO}}

**{{LO_QUE_CRUZA_O_EXIGE}}:** {{...}}

```json
{{EJEMPLO_CON_VALORES_REALES}}
```

### 2. `{{VERBO}} {{RUTA}}`

{{...}}

```json
{{...}}
```

<!-- … hasta completar las {{OPERACIONES_NUEVAS}}.

     SI EL CURSO PIDE TODOS LOS VERBOS en cada recurso —GET, POST, PUT, PATCH y
     DELETE—, van los cinco, cada uno con su sección. Un verbo que falta se
     agrega; no se omite porque «no se usa».

     Y EL CONTRASTE DIDÁCTICO PUT vs PATCH, si la constitución lo exige:
       PUT   = reemplazo completo → 422 si falta un campo
       PATCH = parcial           → 200 con el mismo cuerpo
     Las dos secciones se escriben completas: es lo que hace ver la diferencia.
-->

<!-- CUANDO EL DOMINIO NO DICE QUÉ CÓDIGO RESPONDER, NO SE ELIGE UNO:
     [NECESITA ACLARACIÓN: ¿qué responde {{la operación}} cuando {{el caso}}?]
     POLITICA_DE_ERRORES.md es quien decide eso, y si no lo dice, se pregunta. -->
