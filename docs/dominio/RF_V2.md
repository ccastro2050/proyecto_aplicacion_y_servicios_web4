# Requisitos funcionales — Versión 2: las seis tablas CON clave foránea

> **Qué es este documento.** Los requisitos funcionales **de la v2**, con su
> regla y **cómo se comprueba cada uno**.
>
> **A qué plantillas alimenta:** el `2_spec`, el `6_contracts` y el
> `9_checklist` de la v2.
>
> **Por qué está partido por versión.** Porque la conversación que genera el
> spec kit de la v2 **solo necesita los requisitos de la v2**. Ver
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

> **Y aquí está la confusión que el mapa de versiones avisa:** `rol_usuario` y
> `rutarol` **son de esta versión**, porque tienen clave foránea. Lo que llega
> en la v3 no es su CRUD — es **la puerta**. Administrar permisos y *hacerlos
> valer* son dos cosas distintas.

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v2-1** | CRUD de `cliente`, con **dos** claves foráneas: una obligatoria (`persona`) y una opcional (`empresa`) | `/api/cliente` |
| **RF-v2-2** | CRUD de `vendedor`, con una clave foránea obligatoria | `/api/vendedor` |
| **RF-v2-3** | `factura` se opera **siempre por procedimiento**: maestro y detalle en una transacción | `/api/factura` |
| **RF-v2-4** | Anular una factura — borrado **lógico**, con devolución de stock | `POST /api/factura/{n}/anular` |
| **RF-v2-5** | Las dos tablas puente, con llave compuesta y **borrado por sus dos claves** | `/api/rol-usuario` · `/api/rutarol` |
| **RF-v2-6** | `usuario-con-roles`: el usuario **y** sus roles en una sola operación | `/api/usuario-con-roles` |
| **RF-v2-7** | Un código HTTP nuevo: el **409** — clave foránea inexistente, pareja repetida, factura ya anulada | `POST /api/cliente` con `P999` |
| **RF-v2-8** | Una clave foránea en la interfaz es un **desplegable**, nunca un campo de texto | Pantalla «Nuevo cliente» |
| **RF-v2-9** | El formulario de factura es **maestro-detalle**: se agregan renglones antes de guardar | Pantalla «Nueva factura» |
| **RF-v2-10** | La v1 **sigue funcionando igual** | Regresión: los 8 RF de la v1 |

> **`productosporfactura` no tiene endpoint propio, y no es un olvido.** Un
> renglón no se crea solo: nace y muere con su factura, dentro del
> procedimiento. Darle un `POST /api/productosporfactura` sería permitir un
> renglón huérfano — justo lo que la transacción existe para impedir.

> **RF-v2-8 es más que estética.** Un campo de texto para `fkcodpersona` deja
> que alguien escriba `P999`, y lo que llega de vuelta es un 409. Un desplegable
> hace que **el error no se pueda cometer** — y ésa es la diferencia entre
> validar y prevenir.

> **RF-v2-9 es la razón de ser de la versión.** Una factura con tres renglones
> son cuatro escrituras que tienen que ser **una sola operación**. Si el
> formulario guardara el encabezado y luego los renglones uno por uno, una
> factura podría quedar con dos de tres. Ver
> [`PRINCIPIOS_ACID.md`](../conceptos/PRINCIPIOS_ACID.md).

> **Dos procedimientos existen en la base de datos y la API NO los expone**:
> `sp_actualizar_factura_y_productosporfactura` y `sp_borrar_…`. Está declarado
> en el spec: la operación del negocio es **anular**, no corregir. Es el mismo
> motivo por el que el `PUT` de factura está escrito y apagado.

---

| Qué | Dónde |
|---|---|
| El requisito que manda en esta versión | [`v2_con_fk/2_spec.md`](../spec_kit/versiones/v2_con_fk/2_spec.md) |
| El contrato, endpoint por endpoint | [`v2_con_fk/6_contracts.md`](../spec_kit/versiones/v2_con_fk/6_contracts.md) |
| Las reglas que estos RF dan por ciertas | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| La versión anterior · la siguiente | [`RF_V1.md`](RF_V1.md) · [`RF_V3.md`](RF_V3.md) |
