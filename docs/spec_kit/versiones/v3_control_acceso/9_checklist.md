# Lista de chequeo — Versión 3: antes de la primera línea de código

> Se firma **antes** de escribir código. Si una casilla no se puede marcar, la
> versión no arranca — y lo que falta se arregla en los documentos, que es
> barato.

---

## A. La especificación está completa

- [ ] No queda ningún `[NECESITA ACLARACIÓN: …]` en [2_spec.md](2_spec.md).
- [ ] Los **diez** criterios de aceptación son **verificables**: cada uno dice
      qué hacer y qué tiene que pasar.
- [ ] Está escrito **cuál de los diez no se puede simular**, y por qué.

## B. Los conceptos están entendidos — no solo leídos

- [ ] Se puede explicar la diferencia entre **401 y 403** sin mirar.
- [ ] Se puede explicar por qué **SHA-256 no sirve** para contraseñas, y la
      razón es que es **demasiado rápido**.
- [ ] Se puede explicar qué es el ***salt*** y qué problema resuelve.
- [ ] Se puede explicar que un JWT está **firmado y NO cifrado**, y qué
      significa eso para lo que se le mete adentro.
- [ ] Se puede explicar por qué **los permisos no van en el token**.
- [ ] Se puede explicar por qué **esconder una entrada del menú no protege
      nada**.

> **Los seis están en
> [CONCEPTOS_CONTROL_DE_ACCESO.md](../../../conceptos/CONCEPTOS_CONTROL_DE_ACCESO.md).**
> Esta versión es la única del curso en la que entender mal un concepto produce
> un sistema **que parece funcionar**.

## C. El alcance está cerrado

- [ ] Está escrito que la v3 **no agrega ni modifica una sola tabla**.
- [ ] Está escrito que el **CRUD** de `usuario`, `rol`, `ruta`, `rol_usuario` y
      `rutarol` **ya está** (v1 y v2), y que lo que llega es **la puerta**.
- [ ] Está escrito que **un token no se puede revocar** — y que de ahí sale que
      la duración sea corta.
- [ ] Está escrito qué queda fuera: refrescar el token, recuperar la
      contraseña, segundo factor, permisos por operación.

## D. El contrato es exacto

- [ ] [6_contracts.md](6_contracts.md) nombra `POST /api/sesion` con su cuerpo.
- [ ] Está dicho que el **401 responde lo MISMO** para el correo inexistente y
      la contraseña equivocada, **y por qué**.
- [ ] Están nombrados **los dos** endpoints abiertos, y solo dos.
- [ ] Está la **matriz de los tres roles** contra los recursos.
- [ ] Está el **cuerpo** del 401 y el del 403.

## E. El plan no anticipa, y nombra los tropiezos

- [ ] El único paquete nuevo es **`JwtBearer`**, y su versión **se copió de un
      proyecto que compila** — no se eligió por intuición.
- [ ] Están escritos los **cinco** tropiezos de [3_plan.md](3_plan.md) §5, y
      los tres que dejan el sistema **menos seguro de lo que parece**.
- [ ] Está escrito por qué **no** se usa un `DelegatingHandler`.
- [ ] Está escrito por qué el `@rendermode` va en `App.razor`.

## F. Las tareas son verificables

- [ ] Las **once** fases de [8_tasks.md](8_tasks.md) tienen su verificación.
- [ ] El orden **contraseña → sesión → permiso → interfaz** está justificado.
- [ ] Cada fase es **un commit**.
- [ ] La fase del **criterio 9 en el navegador** está en la lista.

## G. El entorno está listo

- [ ] La v2 **arranca y su prueba de humo pasa**.
- [ ] Se sabe que esta versión necesita **`docker compose down -v`** una vez, y
      por qué: el script de la base de datos solo corre cuando el volumen nace.
- [ ] `verificar_acceso_ruta` **existe** en la base de datos: se comprobó con `\df`, no
      se supuso.
- [ ] Las **15 rutas** y los **5 roles** están sembrados: se comprobó con un
      `SELECT`.

---

## Firma

| | |
|---|---|
| **Quién** | |
| **Fecha** | |
| **Casillas sin marcar, y por qué** | |

> **Si queda una casilla sin marcar, se escribe aquí por qué.** Una lista con
> huecos sin explicar es peor que no tenerla.
