<!--
PLANTILLA — 9_checklist.md
En Spec Kit oficial: checklists/<dominio>.md — VARIOS, uno por dominio
(ux.md, api.md, security.md…). Comando /speckit.checklist
Aquí es uno solo: un proyecto de aula no sostiene cinco listas. Es una
simplificación nuestra, y se declara.

Sale de: PLAN_V{{N}}.md + REQUISITOS_FUNCIONALES.md  (+ el 2_spec ya generado)
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/9_checklist.md

LAS CUATRO SECCIONES SON FIJAS, y el orden también:
  El código · La interfaz gráfica · Lo medido, no lo supuesto · Los documentos

LA TERCERA ES LA QUE DISTINGUE ESTA LISTA DE UNA LISTA DE TAREAS: no pregunta
si se escribió, pregunta SI SE CORRIÓ.
-->

# Lista de verificación — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 9_checklist.md va en negrita y sin enlace. -->

---

## El código

<!-- Lo que tiene que existir, con nombre y cantidad. Cada casilla verificable
     mirando el repositorio, sin ejecutar nada. -->

- [ ] {{LAS_PIEZAS_CON_SU_CANTIDAD}}.
- [ ] {{LA_REGLA_DE_LA_VERSION}}.
- [ ] {{LO_QUE_SE_INCUMPLE_SIN_QUE_NADA_FALLE}}.
- [ ] {{LA_CAPA_QUE_EXISTE_AUNQUE_ESTE_VACIA}}, y está escrito por qué.
- [ ] **{{EL_REGISTRO_DE_DEPENDENCIAS_ESTA}}**.

## La interfaz gráfica

- [ ] {{COMO_SE_PIDEN_LOS_DATOS}}.
- [ ] Si {{UNA_PARTE}} falla, {{EL_RESTO}} se dibuja, y el aviso dice **cuál**.
- [ ] {{LA_RESPUESTA_VACIA}} se muestra **con palabras**, no como una tabla vacía.
- [ ] {{LAS_REGLAS_DE_LA_MARCA_O_DEL_ESTILO}}.
- [ ] La entrada está en el menú, con su permiso.

## Lo medido, no lo supuesto

<!-- LA SECCIÓN QUE SOSTIENE LA LISTA. Cada casilla es una EJECUCIÓN, no una
     lectura. El «(¿se corrió?)» entre paréntesis no es un chiste: es la
     diferencia entre una lista honesta y una lista llena de buena fe. -->

- [ ] {{LO_NUEVO}} responde **{{CODIGO_ESPERADO}}**. *(¿se corrió?)*
- [ ] Los rechazos: {{401_403_422_LO_QUE_APLIQUE}}.
- [ ] {{LA_PRUEBA_DE_QUE_EL_PERMISO_NO_ESTA_EN_EL_TOKEN_O_EQUIVALENTE}}.
- [ ] {{LA_PRUEBA_DE_QUE_EL_SISTEMA_ESTA_VIVO}}: {{QUE_SE_MUEVE_CUANDO_ALGO_CAMBIA}}.
- [ ] **La regresión de {{LAS_VERSIONES_ANTERIORES}} pasa completa.**

## Los documentos

<!-- Que las razones estén ESCRITAS. Es lo que se pregunta en la sustentación,
     y es lo que hace que el spec kit valga para el que llega después.
     Cada casilla nombra la razón concreta, no «está documentado». -->

- [ ] Está escrito **por qué {{LA_DECISION_PRINCIPAL}}** — {{LAS_RAZONES_CONCRETAS}}.
- [ ] Está escrito por qué {{LA_SEGUNDA_DECISION}}.
- [ ] Está escrito el **tropiezo de {{LO_QUE_FUE}}**, y por qué se resolvió así
      y no de la otra forma.
- [ ] Está escrito por qué {{LA_DISTINCION_DE_CODIGOS}}.
- [ ] **Lo pendiente está declarado como pendiente** —{{LA_LISTA}}— y no
      descrito como si existiera.

> **La última casilla es la que sostiene a las demás.** Un spec kit que
> describe lo que no existe le enseña a quien lo lee que los documentos no se
> pueden creer — y a partir de ahí deja de leerlos todos.

<!-- Y la compuerta del método, que se comprueba aquí:
     - [ ] No queda ningún [NECESITA ACLARACIÓN] en los ocho documentos.
     Buscar «NECESITA» en la carpeta de la versión. -->
