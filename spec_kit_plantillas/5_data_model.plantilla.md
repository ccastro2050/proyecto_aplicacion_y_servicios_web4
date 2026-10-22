<!--
PLANTILLA — 5_data_model.md
En Spec Kit oficial: data-model.md  ·  comando /speckit.plan  ·  **FASE 1**
Sale de: DISENO_BD.md + DATOS_DE_PRUEBA.md
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/5_data_model.md

LO QUE LA IA NO DEBE HACER AQUÍ: proponer un CREATE TABLE.
El modelo de datos YA ESTÁ en DISENO_BD.md, y salió de la elicitación con el
usuario experto. Este documento DESCRIBE qué parte del modelo usa esta versión
y por qué — no lo rediseña. Una IA que propone el esquema está rehaciendo un
trabajo hecho, sin haber estado en esa conversación.
-->

# Modelo de datos — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 5_data_model.md va en negrita y sin enlace. -->

---

> {{QUE_LE_PASA_AL_MODELO_EN_ESTA_VERSION}}

<!-- La primera línea responde una sola pregunta: ¿esta versión toca la base de
     datos o no? Las dos respuestas son buenas, y las dos se escriben:

     · «La v4 no agrega ni una tabla, ni una columna, ni un índice. Agrega
        preguntas sobre lo que ya hay. Este documento existe para decir eso.»
     · «La v2 enciende las claves foráneas que la v1 declaró y no usaba.»

     Un 5_data_model que no dice nada porque «no cambió nada» es una
     oportunidad perdida: justamente eso es lo que hay que afirmar. -->

## 1. Lo que ya existía, y basta

{{QUE_TRAIAN_LAS_VERSIONES_ANTERIORES}}

{{POR_QUE_ESO_ALCANZA_PARA_ESTA_VERSION}}

> **{{QUE_DECISION_VIEJA_SE_COBRA_AHORA}}**

<!-- RECOMENDADO: señalar qué decisión de una versión anterior hace posible
     ésta. En el ejemplo: «Es el pago de la v2. Allá las claves foráneas se
     veían como una molestia —desplegables, 409 cuando no existe el padre—.
     Aquí se cobran.»
     Eso es lo que hace que las versiones se sientan acumulativas de verdad y
     no como cuatro proyectos seguidos. -->

## 2. Qué toca {{CADA_PARTE_DE_LO_NUEVO}}

<!-- La tabla que conecta lo nuevo con el modelo: por cada endpoint, pantalla o
     consulta, qué tablas usa y cuál es la columna que lo hace posible.
     La tercera columna es la valiosa: no «las tablas», sino LA LLAVE. -->

| # | {{TABLAS_O_ENTIDADES}} | {{LA_COLUMNA_QUE_LO_HACE_POSIBLE}} |
|---|---|---|
| 1 | {{...}} | {{...}} |
| 2 | {{...}} | {{...}} |

> {{LA_FILA_MAS_INTERESANTE_Y_POR_QUE}}

## 3. Lo que el {{MOTOR_O_DIALECTO}} cambia

<!-- OBLIGATORIO si el proyecto corre contra más de un motor, o si va a correr.
     Las diferencias de dialecto que importan, y DÓNDE se resolvieron.

     Y la pregunta que esta sección debe contestar: ¿la diferencia se resolvió
     en el SQL, o se dejó que subiera hasta los modelos? Lo primero mantiene el
     contrato idéntico entre motores; lo segundo ata el contrato al motor. -->

| | {{MOTOR_DE_ESTA_VERSION}} | {{EL_OTRO_MOTOR}} |
|---|---|---|
| **{{DIFERENCIA_1}}** | {{...}} | {{...}} |
| **{{DIFERENCIA_2}}** | {{...}} | {{...}} |

> {{DONDE_SE_RESOLVIERON_Y_QUE_PERMITE_ESO}}

<!-- Si una de estas diferencias NO falla sino que devuelve un número
     equivocado, dígalo con esas palabras. Es la clase de dato que nadie
     encuentra leyendo documentación del motor. -->

<!-- SI EL MODELO TIENE ALGO QUE AVERIGUAR, NO SE INVENTA:
     [NECESITA ACLARACIÓN: ¿{{la pregunta}}?]
     y va a la próxima reunión con el usuario experto. -->
