<!--
PLANTILLA — 8_tasks.md
En Spec Kit oficial: tasks.md  ·  comando /speckit.tasks  ·  **FASE 2**
Sale de: PLAN_V{{N}}.md + PLAN_DE_TRABAJO.md  (+ el 2_spec ya generado)
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/8_tasks.md

LA REGLA: cada fase termina en algo que se puede COMPROBAR, y la comprobación
está escrita. Una lista de tareas sin verificación es una lista de deseos en
orden.

Y EL ORDEN DE LAS FASES NO ES EL DE LAS CARPETAS: va de adentro hacia afuera
—modelos, repositorio, servicio, controlador, cableado, interfaz— porque cada
pieza necesita la anterior para poder probarse sola.
-->

# Tareas — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 8_tasks.md va en negrita y sin enlace. -->

---

> **Cada fase termina en algo que se puede comprobar.** No se pasa a la
> siguiente sin eso.

## Fase 1 — {{LO_PRIMERO}}

- [ ] {{LA_TAREA}}, {{CON_EL_ARCHIVO_Y_SU_RUTA}}.
- [ ] **{{LO_QUE_NO_SE_DEBE_HACER}}**: ver [3_plan.md](3_plan.md) §{{...}}.

<!-- Las casillas se marcan, así que cada una es UNA cosa. «Hacer los modelos»
     no se puede marcar a medias; «diez record, uno por consulta» sí. -->

## Fase 2 — {{LO_SEGUNDO}}

- [ ] {{...}}

**Verificación:** {{COMO_SE_COMPRUEBA_ESTA_FASE}}

## Fase 3 — {{LO_TERCERO}}

- [ ] {{...}}
- [ ] {{LA_REGLA_QUE_SE_INCUMPLE_SIN_QUE_NADA_FALLE}}.

<!-- RECOMENDADO en la fase del acceso a datos: las trampas que no fallan.
     En el ejemplo son dos, y las dos son silenciosas:
       · los alias de columna tienen que coincidir con las propiedades, porque
         el mapeo es POR NOMBRE: sin alias el campo llega vacío sin error;
       · el CAST de los conteos, que sin él trunca un promedio en silencio. -->

**Verificación:** {{...}}

## Fase 4 — {{EL_CABLEADO}}

- [ ] {{...}}
- [ ] **{{EL_REGISTRO_DE_DEPENDENCIAS}}** — y esta casilla es la que más se olvida:

```{{LENGUAJE}}
{{LAS_LINEAS_EXACTAS}}
```

> **Sin esa línea el proyecto COMPILA**, arranca, y {{QUE_FALLA_Y_CUANDO}}.
> El compilador no puede atraparlo.

<!-- OBLIGATORIO si el proyecto usa inyección de dependencias. Es el error más
     frecuente de todos y el único que no da ni una pista en tiempo de
     compilación. -->

**Verificación:** {{...}} — [7_quickstart.md](7_quickstart.md) §{{...}}

## Fase 5 — {{LA_INTERFAZ_GRAFICA}}

- [ ] {{...}}
- [ ] {{EL_COMPORTAMIENTO_ANTE_UN_FALLO_PARCIAL}}.

## Fase {{n}} — {{EL_MENU_O_LA_NAVEGACION}}

- [ ] {{DONDE_VA_LA_ENTRADA_Y_CON_QUE_PERMISO}}.

<!-- Se olvida siempre: la pantalla queda construida y no hay cómo llegar a
     ella. Una ruta sin entrada en el menú existe y nadie la ve. -->

## Fase {{n}} — La regresión y el cierre

- [ ] El smoke test de {{LAS_VERSIONES_ANTERIORES}}, **completos**.
- [ ] Los {{CUANTOS}} criterios de [2_spec.md](2_spec.md) §3.
- [ ] Commit + tag `v{{N}}` + push.

## Lo que NO se hace en esta versión

<!-- OBLIGATORIO, y cada fila con el enlace a dónde está la razón. Evita que
     alguien lo agregue «de paso» y se lleve el alcance por delante. -->

| | Por qué |
|---|---|
| **{{COSA_1}}** | Ver [3_plan.md](3_plan.md) §{{...}} |
| **{{COSA_2}}** | Ver [3_plan.md](3_plan.md) §{{...}} |
| **{{LO_PENDIENTE}}** | **Pendientes**, declaradas en [2_spec.md](2_spec.md) §5 |
