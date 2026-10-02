# Lista de chequeo — Versión 2: antes de la primera línea de código

> **Cuándo se firma esto:** **antes** de escribir código, no después. Es la
> compuerta 3 del método: si una casilla no se puede marcar, la versión no
> arranca — y lo que falta se arregla en los documentos, que es barato.

---

## A. La especificación está completa

- [ ] **No queda ningún `[NECESITA ACLARACIÓN: …]`** en [2_spec.md](2_spec.md).
- [ ] Los **nueve** requisitos funcionales dicen **qué** tiene que pasar, no
      cómo programarlo.
- [ ] Los **18** criterios de aceptación son **verificables**: cada uno dice
      qué hacer y qué tiene que pasar.
- [ ] Las **clarificaciones** (§7) tienen su **razón**, no solo la decisión.

## B. El alcance está cerrado

- [ ] La versión son **las seis tablas con clave foránea**, y los seis
      recursos de la v1 **no se tocan**.
- [ ] Está escrito que `persona`, `empresa`, `producto`, `rol`, `ruta` y
      `usuario` **son de la v1**.
- [ ] Está escrito que el **CRUD** de `rol_usuario` y `rutarol` **es de esta
      versión**, y que lo de la v3 es **la puerta**, no su CRUD.
- [ ] Está escrito que **editar y borrar físicamente una factura no se
      exponen** — y por qué.

## C. El contrato es exacto

- [ ] [6_contracts.md](6_contracts.md) nombra **las seis rutas nuevas**, con
      su forma exacta: `api/cliente`, `api/vendedor`, `api/factura`,
      `api/rol-usuario`, `api/rutarol`, `api/usuario-con-roles`.
- [ ] Está dicho **cuál lleva guion y cuál no**, y por qué.
- [ ] Está dicho que el sobre de `factura` **no trae `limite`**, y que
      consultar una **no viene en sobre**.
- [ ] Está dicho qué **NO se envía** al crear una factura: `total`,
      `subtotal`, `fecha`.
- [ ] Está dicho qué significa la **contraseña vacía** al editar un usuario.
- [ ] El **409** aparece con sus tres causas: clave foránea inexistente,
      pareja repetida, factura ya anulada.

## D. El plan no anticipa

- [ ] [3_plan.md](3_plan.md) no mete **ningún paquete nuevo**.
- [ ] No hay **fábrica**, ni segundo motor: eso es la **v5**.
- [ ] No hay **token** ni middleware de autenticación: eso es la **v3**.
- [ ] Las **capas** siguen estrictas: el servicio **no** devuelve códigos
      HTTP.

## E. Las tareas son verificables

- [ ] Las **12** fases de [8_tasks.md](8_tasks.md) tienen cada una **su**
      verificación escrita.
- [ ] Cada fase es **un commit**.
- [ ] Las fases del **front** están en la lista. Una versión sin interfaz
      gráfica no está cerrada.

## F. El entorno está listo

- [ ] `docker compose up -d --build` levanta la v1 **y la prueba de humo de la
      v1 pasa**.
- [ ] Los **puertos** del proyecto no chocan con nada —se revisó el registro—.
- [ ] La base trae **las 12 tablas**, los **procedimientos** y el
      **disparador**: se comprueba, no se supone.

---

## Firma

| | |
|---|---|
| **Quién** | |
| **Fecha** | |
| **Casillas sin marcar, y por qué** | |

> **Si queda una casilla sin marcar, se escribe aquí por qué.** Una lista con
> huecos sin explicar es peor que no tenerla: da la impresión de que se revisó.
