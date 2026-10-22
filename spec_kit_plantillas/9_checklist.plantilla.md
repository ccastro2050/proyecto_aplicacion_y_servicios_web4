<!--
PLANTILLA — 9_checklist.md
En Spec Kit oficial: checklists/<dominio>.md — VARIOS, uno por dominio
(ux.md, api.md, security.md…). Comando /speckit.checklist
Aquí es uno solo: un proyecto de aula no sostiene cinco listas.

Sale de: PLAN_V{{N}}.md + RF_V{{N}}.md  (+ el 2_spec ya generado)

================================================================
CUIDADO: ESTE DOCUMENTO NO AUDITA EL CÓDIGO. AUDITA EL SPEC KIT.
================================================================

Se firma **ANTES de escribir la primera línea de código**, no después. Es la
compuerta 3 del método: si una casilla no se puede marcar, la versión **no
arranca** — y lo que falta se arregla en los documentos, que es barato.

Y coincide con Spec Kit oficial, cuyo /speckit.checklist revisa la CALIDAD DE
LOS REQUISITOS, no la implementación.

ESTRUCTURA CANÓNICA, tomada de las v1, v2 y v3:

   A. La especificación está completa
   B. El alcance está cerrado
   C. El contrato es exacto
   D. El plan no anticipa
   E. Las tareas son verificables
   F. El entorno está listo
   + Firma

Ejemplos llenos: ../docs/spec_kit/versiones/v2_con_fk/9_checklist.md (3.728 car)
                 ../docs/spec_kit/versiones/v3_control_acceso/9_checklist.md (4.365 car)
-->

# Lista de chequeo — Versión {{N}}: antes de la primera línea de código

> **Cuándo se firma esto:** **antes** de escribir código, no después. Es la
> compuerta 3 del método: si una casilla no se puede marcar, la versión no
> arranca — y lo que falta se arregla en los documentos, que es barato.

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 9_checklist.md va en negrita y sin enlace. -->

---

## A. La especificación está completa

<!-- Lo que se audita es el 2_spec: que no quede nada abierto y que lo que
     dice sea verificable. Con CANTIDADES, para que la casilla se pueda
     responder sí o no. -->

- [ ] **No queda ningún `[NECESITA ACLARACIÓN: …]`** en [2_spec.md](2_spec.md).
- [ ] Los **{{CUANTOS}}** requisitos funcionales dicen **qué** tiene que
      pasar, no cómo programarlo.
- [ ] Los **{{CUANTOS}}** criterios de aceptación son **verificables**: cada
      uno dice qué hacer y qué tiene que pasar.
- [ ] Los criterios están **separados** en los de la API y los de las
      interfaces gráficas. {{CUANTOS}} y {{CUANTOS}}.
- [ ] Las **clarificaciones** tienen su **razón**, no solo la decisión.

## B. El alcance está cerrado

<!-- Que no se haya colado nada de otra versión, y que esté ESCRITO a qué
     versión pertenece lo que no entra. -->

- [ ] La versión es **{{EL_CRITERIO_QUE_LA_DEFINE}}**, y {{LO_DE_LA_VERSION_ANTERIOR}} **no se toca**.
- [ ] Está escrito que **{{LO_QUE_SE_CONFUNDE}}** es de la v{{M}} y no de ésta.
- [ ] Está escrito qué **NO se expone**, y por qué.
- [ ] {{LO_QUE_FALLA_A_PROPOSITO}} está **declarado como deliberado**, no omitido.

## C. El contrato es exacto

<!-- Que el 6_contracts no tenga ambigüedades: las rutas con su forma, los
     códigos con su causa, y lo que NO se manda. -->

- [ ] [6_contracts.md](6_contracts.md) nombra **las {{CUANTAS}} rutas**, con su
      forma exacta.
- [ ] Está dicho **cuál lleva guion y cuál no**, y por qué.
- [ ] Está dicho qué **NO se envía** en cada operación.
- [ ] Los códigos de estado **salen de la política de errores**, no se
      eligieron aquí.
- [ ] {{EL_CODIGO_NUEVO_DE_ESTA_VERSION}} aparece con **sus causas**.

## D. El plan no anticipa

<!-- La compuerta de YAGNI. Una fila por cosa que pertenece a una versión
     posterior y que el plan NO debe traer. -->

- [ ] [3_plan.md](3_plan.md) no mete **ningún paquete nuevo** que la
      especificación no pida.
- [ ] No hay **{{LO_DE_LA_VERSION_SIGUIENTE}}**: eso es la **v{{M}}**.
- [ ] No hay **{{LO_DE_OTRA_VERSION}}**: eso es la **v{{M}}**.
- [ ] Las **capas** siguen estrictas: el servicio **no** devuelve códigos HTTP.
- [ ] **El chequeo de constitución del plan se hizo contra la constitución**,
      artículo por artículo — y ningún ✅ se puso sobre un artículo que el
      propio plan viola.

## E. Las tareas son verificables

- [ ] Las **{{CUANTAS}}** fases de [8_tasks.md](8_tasks.md) tienen cada una
      **su** verificación escrita.
- [ ] Cada fase es **un commit**.
- [ ] Las fases del **front** están en la lista. Una versión sin interfaz
      gráfica no está cerrada.
- [ ] {{LA_CASILLA_QUE_MAS_SE_OLVIDA}} tiene su propia casilla.

## F. El entorno está listo

- [ ] `{{EL_COMANDO}}` levanta {{LO_ANTERIOR}} **y su prueba de humo pasa**.
- [ ] Los **puertos** del proyecto no chocan con nada — se revisó el registro.
- [ ] La base de datos trae {{LO_QUE_TIENE_QUE_TRAER}}: **se comprueba, no se
      supone**.

---

## Firma

| | |
|---|---|
| **Quién** | |
| **Fecha** | |
| **Casillas sin marcar, y por qué** | |

> **Si queda una casilla sin marcar, se escribe aquí por qué.** Una lista con
> huecos sin explicar es peor que no tenerla: da la impresión de que se revisó.

<!-- Y LA FIRMA LA PONE UNA PERSONA, no una IA. Una IA puede decir que un
     criterio se cumple; solo una persona puede haberlo visto cumplirse. -->
