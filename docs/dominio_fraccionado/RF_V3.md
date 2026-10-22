# Requisitos funcionales — Versión 3: quién entra, y a qué

> **Qué es este documento.** Los requisitos funcionales **de la v3**, con su
> regla y **cómo se comprueba cada uno**.
>
> **A qué plantillas alimenta:** el `2_spec`, el `6_contracts` y el
> `9_checklist` de la v3.
>
> **Por qué está partido por versión.** Porque la conversación que genera el
> spec kit de la v3 **solo necesita los requisitos de la v3**. Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **El catálogo completo**, los cinco verbos recurso por recurso y la matriz de
> trazabilidad están en
> [`REQUISITOS_FUNCIONALES.md`](REQUISITOS_FUNCIONALES.md).
>
> **Material académico simulado**, pero los endpoints **están medidos contra la
> API corriendo**, no copiados del código.
>
> Versión 1.0 · 4 de octubre de 2026.

---

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v3-1** | La contraseña se guarda **cifrada** (BCrypt), nunca en claro | `SELECT contrasena FROM usuario` |
| **RF-v3-2** | Identificarse devuelve un **token** | `POST /api/sesion` |
| **RF-v3-3** | Sin token no se entra: **401** | Cualquier endpoint sin cabecera |
| **RF-v3-4** | Con token pero sin permiso: **403** | Entre como `cliente1@correo.com` y pida `/api/factura` |
| **RF-v3-5** | El permiso **lo decide la base de datos**, en cada petición | `verificar_acceso_ruta` |
| **RF-v3-6** | El menú muestra **solo** lo que ese rol puede abrir | Entre con los tres usuarios y compare |
| **RF-v3-7** | El login responde **lo mismo** si el correo no existe y si la clave está mal | Dos peticiones, misma respuesta |
| **RF-v3-8** | Repartir permisos desde la interfaz | Pantalla «Permisos» |
| **RF-v3-9** | Las v1 y v2 siguen funcionando — **ahora con token** | Regresión |

> **El 401 y el 403 son dos preguntas distintas**, y confundirlas es el error
> más común: *«¿quién eres?»* contra *«¿puedes?»*. El primero lo responde
> `[Authorize]`; el segundo, `[ExigePermiso]`, **después**. Ver
> [`CONCEPTOS_CONTROL_DE_ACCESO.md`](../conceptos/CONCEPTOS_CONTROL_DE_ACCESO.md) y la imagen de
> `401_contra_403`.

> **RF-v3-5 tiene una consecuencia que vale la pena notar:** como el permiso se
> consulta en cada petición y no viaja en el token, quitarle un permiso a un rol
> **surte efecto de inmediato**, sin que la persona tenga que volver a entrar.
> Si el permiso viviera en el token, habría que esperar a que expire.

---

| Qué | Dónde |
|---|---|
| El requisito que manda en esta versión | [`v3_control_acceso/2_spec.md`](../spec_kit/versiones/v3_control_acceso/2_spec.md) |
| El contrato, endpoint por endpoint | [`v3_control_acceso/6_contracts.md`](../spec_kit/versiones/v3_control_acceso/6_contracts.md) |
| Por qué 401 y 403 son distintos | [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) |
| La versión anterior · la siguiente | [`RF_V2.md`](RF_V2.md) · [`RF_V4.md`](RF_V4.md) |
