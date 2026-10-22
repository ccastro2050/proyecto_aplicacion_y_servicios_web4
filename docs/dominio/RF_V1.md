# Requisitos funcionales — Versión 1: las seis tablas sin clave foránea

> **Qué es este documento.** Los requisitos funcionales **de la v1**, con su
> regla y **cómo se comprueba cada uno**.
>
> **A qué plantillas alimenta:** el `2_spec`, el `6_contracts` y el
> `9_checklist` de la v1.
>
> **Por qué está partido por versión.** Porque la conversación que genera el
> spec kit de la v1 **solo necesita los requisitos de la v1**: los de las otras
> cuatro compiten por la atención con el único que importa. Ver
> [`DOMINIO_PARA_GENERAR.md`](../../DOMINIO_PARA_GENERAR.md).
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

> **Lo que define la v1 no es «una tabla»: son las tablas que NO tienen clave
> foránea.** En `bdfacturas` son seis — `producto`, `empresa`, `persona`, `rol`,
> `ruta` y `usuario` — y son las primeras porque **se pueden llenar sin que
> exista nada más**.

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v1-1** | Listar, con `?limite=` | `GET /api/producto`, y los otros cinco |
| **RF-v1-2** | Consultar uno por su clave | `GET /api/producto/PR001` |
| **RF-v1-3** | Crear | `POST /api/producto` |
| **RF-v1-4** | **Reemplazar** uno completo — los campos que no se mandan quedan vacíos | `PUT /api/producto/PR001` |
| **RF-v1-5** | **Actualizar** solo lo que se manda | `PATCH /api/producto/PR001` |
| **RF-v1-6** | Eliminar | `DELETE /api/producto/PR001` |
| **RF-v1-7** | Decir si la API está viva, y en qué versión | `GET /` |
| **RF-v1-8** | **Una pantalla por recurso**, con dirección propia | `/productos`, `/personas`, `/empresas`, … |

> **Que los seis recursos sean idénticos salvo los campos ES el punto.** Con uno
> solo nunca aparece la pregunta que la v1 quiere provocar: *«¿y si hago un
> `Repositorio<T>` genérico y me ahorro cinco?»*. La respuesta está en el
> Artículo 10 de la constitución, y hay que haber sentido la repetición para que
> la respuesta signifique algo.

> **RF-v1-4 y RF-v1-5 son el corazón de la versión**, y la diferencia entre los
> dos es la lección: con el **mismo cuerpo** `{"nombre":"X"}`, el PUT deja el
> resto de los campos vacíos y el PATCH no los toca. Quien no haya visto eso
> pasar, no sabe la diferencia — se la cree.

> **Y RF-v1-3 falla a propósito con un código duplicado: devuelve 500.** No es
> un descuido: el 409 es la lección de la v2, y verlo mal primero es lo que hace
> que la corrección signifique algo. Está declarado en el spec de la v1.

> **RF-v1-8 dice «dirección propia» y eso excluye `/tabla/{nombre}`.** Una ruta
> con el nombre de la tabla como parámetro parece ahorro y es lo contrario:
> ninguna pantalla se puede enlazar, ningún permiso se puede dar por recurso.

---

| Qué | Dónde |
|---|---|
| El requisito que manda en esta versión | [`v1_sin_fk/2_spec.md`](../spec_kit/versiones/v1_sin_fk/2_spec.md) |
| El contrato, endpoint por endpoint | [`v1_sin_fk/6_contracts.md`](../spec_kit/versiones/v1_sin_fk/6_contracts.md) |
| Las reglas que estos RF dan por ciertas | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| La versión siguiente | [`RF_V2.md`](RF_V2.md) |
