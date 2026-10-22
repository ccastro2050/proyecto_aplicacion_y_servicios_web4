<!--
PLANTILLA — 4_research.md
En Spec Kit oficial: research.md  ·  comando /speckit.plan  ·  **FASE 0**
Sale de: FUENTES.md + PENDIENTES.md  (+ el 3_plan ya generado)
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/4_research.md

CUIDADO CON EL ORDEN. En Spec Kit oficial este documento es la FASE 0 del plan,
y su salida es «research.md with all NEEDS CLARIFICATION resolved»: sirve para
AVERIGUAR LO QUE NO SE SABE ANTES DE DISEÑAR, no para levantar el acta al final.
Por eso se genera después del 3_plan y ANTES del 5_data_model.
-->

# Investigación — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 4_research.md va en negrita y sin enlace.
     En la tabla de documentos, éste se marca «(lectura opcional)» — y lo es:
     el sistema se construye sin leerlo. Se lee para entender POR QUÉ. -->

---

<!-- EL FORMATO: una decisión por sección, numeradas D1, D2, D3…
     Cada una con su tabla de opciones y la elegida marcada ✅.

     LA REGLA QUE HACE ÚTIL ESTE DOCUMENTO: las opciones descartadas se
     escriben con su MEJOR argumento, no como hombres de paja. Un research
     donde las alternativas son obviamente malas no demuestra que se pensó;
     demuestra que se justificó lo que ya se había hecho. -->

## D1 — ¿{{LA_PRIMERA_PREGUNTA}}?

| Opción | Argumento |
|---|---|
| **{{OPCION_A}}** | {{SU_MEJOR_ARGUMENTO_A_FAVOR}}. Y {{LO_QUE_CUESTA}} |
| **{{OPCION_B}}** | {{SU_MEJOR_ARGUMENTO}}. Y {{LO_QUE_CUESTA}} |
| **{{LA_ELEGIDA}}** ✅ | {{POR_QUE}} |

> {{QUE_REGLA_DEL_PROYECTO_O_DE_LA_CONSTITUCION_DECIDE_ESTO}}

<!-- Cuando la decisión la fija la constitución, decirlo: ahí se ve que la
     constitución sirve para algo y no es un documento de adorno. -->

## D2 — ¿{{LA_SEGUNDA_PREGUNTA}}?

{{LO_QUE_SERIA_MAS_CORTO_O_MAS_OBVIO}}

**Decisión: {{LA_ELEGIDA}}.** {{POR_QUE}}

> {{LA_CONSECUENCIA_QUE_SE_VE_DESPUES}}

## D3 — ¿Por qué {{LA_TERCERA_PREGUNTA}}?

{{LA_EXPLICACION}}

## D{{...}} — {{LA_DECISION_QUE_COSTO_UNA_TARDE}}

<!-- RECOMENDADO: la decisión que salió de un tropiezo real, con el mensaje de
     error textual si lo hubo. Es la sección que más se lee de todo el spec kit,
     porque es la que le ahorra la tarde a quien viene después.

     Y el formato que funciona: qué se creía / qué respondió el sistema. -->

| | {{LO_QUE_PASA}} | La consecuencia |
|---|---|---|
| **{{CASO_A}}** | {{...}} | {{...}} |
| **{{CASO_B}}** | {{...}} | {{...}} |

**Decisión: {{LA_ELEGIDA}}.**

| Alternativa | Por qué no |
|---|---|
| {{ALTERNATIVA_1}} | {{...}} |
| {{ALTERNATIVA_2}} | {{...}} |
| **{{LA_ELEGIDA}}** ✅ | {{...}} |

## D{{...}} — Lo que NO se investigó, y por qué

<!-- OBLIGATORIO. Cierra la puerta a la pregunta «¿y por qué no usaron X?»
     antes de que se haga en la sustentación.
     La razón más honesta y la más frecuente: «resuelve el problema entero y
     esconde exactamente lo que esta versión existe para enseñar». -->

| | |
|---|---|
| **{{TEMA_1}}** | {{POR_QUE_NO_SE_MIRO}} |
| **{{TEMA_2}}** | {{POR_QUE_NO_SE_MIRO}} |

---

<!-- SI QUEDAN PREGUNTAS SIN RESPONDER, VAN AQUÍ Y NO SE INVENTAN.
     Esto es lo que Spec Kit llama NEEDS CLARIFICATION, y es la razón por la que
     este documento es la Fase 0: lo que quede abierto aquí es lo que hay que
     preguntarle al usuario experto antes de seguir.

## Lo que falta aclarar

| | A quién se le pregunta |
|---|---|
| [NECESITA ACLARACIÓN: {{LA_PREGUNTA}}] | {{QUIEN}} |

     Y la compuerta: la versión no se cierra con una fila en esta tabla.
-->
