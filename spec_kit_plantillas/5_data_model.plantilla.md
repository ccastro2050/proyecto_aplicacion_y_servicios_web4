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
