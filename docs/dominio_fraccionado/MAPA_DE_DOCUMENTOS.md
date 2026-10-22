# Mapa de documentos — de la elicitación al código

> **Qué es este documento.** El recorrido completo: qué se escribe primero,
> qué sale de qué, y dónde mirar cuando algo no cuadra.
>
> **Para qué existe.** Este repositorio tiene **18 documentos de dominio, 41
> de spec kit y una constitución**. Sin un mapa, un estudiante abre uno y no
> sabe si está leyendo la fuente o la consecuencia.
>
> Versión 1.0 · 22 de octubre de 2026.

---

## 1. El recorrido, en una línea

```
  elicitación  ──▶  DOMINIO  ──▶  SPEC KIT de la versión  ──▶  CÓDIGO
   (preguntar)    (qué y por qué)     (qué construir ahora)      (lo construido)
```

Cada flecha va en un solo sentido, y eso es lo que hace el método útil:

| | |
|---|---|
| **Hacia adelante** | Lo de la izquierda se escribe **antes** y decide lo de la derecha |
| **Hacia atrás** | Si el código hace algo que el spec kit no dice, **uno de los dos está mal** |

---

## 2. Los cuatro escalones, con sus documentos

### Escalón 1 — La elicitación: de dónde salen los requisitos

| Documento | Qué contiene |
|---|---|
| [`elicitacion/1_PREGUNTAS.md`](elicitacion/1_PREGUNTAS.md) | Qué se le preguntó al usuario experto, en cuatro rondas |
| [`elicitacion/2_RESPUESTAS.md`](elicitacion/2_RESPUESTAS.md) | Qué contestó, atribuido |
| [`elicitacion/3_HISTORIAS_PROPUESTAS.md`](elicitacion/3_HISTORIAS_PROPUESTAS.md) | Las historias que salen de esas respuestas |

> **Nada de lo que viene después se inventa.** Si una regla no tiene origen
> aquí, tiene que decir de dónde salió — y eso es lo que registra
> [`FUENTES.md`](FUENTES.md).

### Escalón 2 — El dominio: qué es verdad en este negocio

Se escribe **una vez** y vale para las cinco versiones.

| Documento | Responde a |
|---|---|
| [`REQUISITOS_FUNCIONALES.md`](REQUISITOS_FUNCIONALES.md) | ¿Qué **hace** el sistema? |
| [`REQUISITOS_NO_FUNCIONALES.md`](REQUISITOS_NO_FUNCIONALES.md) | ¿Con qué rapidez, seguridad y portabilidad? |
| [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) | ¿Qué **no puede dejar de ser cierto**, y quién lo defiende? |
| [`DISENO_BD.md`](DISENO_BD.md) | ¿Cómo están las tablas, y por qué así? |
| [`ARQUITECTURA.md`](ARQUITECTURA.md) | ¿Cómo está partido el sistema? |
| [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) | ¿Qué responde ante cada fallo, y quién lo decide? |
| [`GLOSARIO.md`](GLOSARIO.md) | ¿Qué significa cada palabra **aquí**? |
| [`DATOS_DE_PRUEBA.md`](DATOS_DE_PRUEBA.md) | ¿Qué trae la base al levantarse, y de dónde salió? |
| [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) | ¿Cómo se ve, y qué color puede ir dónde? |
| [`FUENTES.md`](FUENTES.md) | ¿De dónde salió cada cosa, y **qué manda sobre qué**? |
| [`CRONOGRAMA.md`](CRONOGRAMA.md) | ¿Cuándo se hizo cada versión? |
| [`PLAN_DE_TRABAJO.md`](PLAN_DE_TRABAJO.md) | ¿Quién hace qué, y en qué rama? |
| [`PLAN_V1.md`](PLAN_V1.md) … [`PLAN_V4.md`](PLAN_V4.md) | ¿Qué entra en **cada** versión, y qué se deja? |
| [`SUSTENTACION_DEL_CODIGO.md`](SUSTENTACION_DEL_CODIGO.md) | Las preguntas de la sustentación, respondidas |
| [`PENDIENTES.md`](PENDIENTES.md) | Lo que **no** se hizo, con su razón |

### Escalón 3 — El spec kit: qué construir en esta versión

Se escribe **por versión**, y describe **solo el delta**.

| Documento | Sale de |
|---|---|
| `2_spec.md` | `REQUISITOS_FUNCIONALES` + `REGLAS_DE_NEGOCIO` + el `PLAN_Vn` |
| `3_plan.md` | `ARQUITECTURA` + `REQUISITOS_NO_FUNCIONALES` |
| `4_research.md` | `FUENTES` + `PENDIENTES` |
| `5_data_model.md` | `DISENO_BD` + `DATOS_DE_PRUEBA` |
| `6_contracts.md` | `REQUISITOS_FUNCIONALES` + **`POLITICA_DE_ERRORES`** + `GLOSARIO` |
| `7_quickstart.md` | `DATOS_DE_PRUEBA` + `POLITICA_DE_ERRORES` |
| `8_tasks.md` | el `PLAN_Vn` + `PLAN_DE_TRABAJO` |
| `9_checklist.md` | los criterios del `PLAN_Vn` |
| `GUIA_IAn.md` | el prompt con el que se generó el código, y lo que hubo que corregirle |

**Y desde hoy cada uno lo dice en su cabecera**, con el enlace: hasta el 22 de
octubre de 2026 los 40 documentos del spec kit **no citaban ni uno** del
dominio. La relación existía en los dos sentidos en la cabeza de quien los
escribió, y en el papel solo en uno.

### Escalón 4 — El código

| | |
|---|---|
| `api_facturas/` | La API |
| `front_blazor/` | El front |
| `db/` | El esquema y la siembra |
| `postman/` | Las peticiones listas para correr |

---

## 3. Qué manda sobre qué, cuando dos se contradicen

En orden de autoridad:

| # | Fuente | Alcance |
|---|---|---|
| 1 | La **constitución** | Todas las versiones. Ante conflicto, gana ella |
| 2 | El **mapa de versiones** | Qué va en cada versión |
| 3 | El **spec kit** de la versión | Qué construir ahora |
| 4 | El **dominio** | Qué es verdad en el negocio |
| 5 | El **código** | Lo que de verdad pasa |

> **Y la regla que lo hace honesto:** entre el documento y el código **no
> siempre gana el documento**. El documento se escribió antes, y a veces el
> código aprendió algo que el documento no sabía — una diferencia de dialecto,
> un error del motor, una medición. Lo que no se vale es dejarlos distintos:
> se corrige uno de los dos y se dice cuál y por qué.

---

## 4. Por dónde empezar, según lo que necesite

| Si usted quiere… | Abra |
|---|---|
| Entender **qué hace** el sistema | [`REQUISITOS_FUNCIONALES.md`](REQUISITOS_FUNCIONALES.md) |
| Saber **por qué** algo está así | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) y el `4_research.md` de la versión |
| **Construir** la versión que sigue | el `2_spec.md` y el `8_tasks.md` de esa versión |
| **Probar** que algo funciona | el `7_quickstart.md` de la versión, y `postman/` |
| Preparar la **sustentación** | [`SUSTENTACION_DEL_CODIGO.md`](SUSTENTACION_DEL_CODIGO.md) |
| Saber **qué falta** | [`PENDIENTES.md`](PENDIENTES.md) |
| Entender una palabra | [`GLOSARIO.md`](GLOSARIO.md) |

---

## 5. Lo que este mapa NO dice

| | |
|---|---|
| El orden de lectura «correcto» | No hay uno. Este mapa existe para que cada quien entre por donde necesita |
| Qué documento es más importante | Ninguno lo es: el dominio sin spec kit no se construye, y el spec kit sin dominio se inventa |
| Cuántas versiones hay | Eso lo fija el [mapa de versiones](../spec_kit/versiones/0_mapa_versiones.md) |
