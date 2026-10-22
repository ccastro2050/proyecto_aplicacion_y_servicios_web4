# Requisitos funcionales — Versión 4: el aplicativo, que es lo que el CRUD no puede

> **Qué es este documento.** Los requisitos funcionales **de la v4**, con su
> regla y **cómo se comprueba cada uno**.
>
> **A qué plantillas alimenta:** el `2_spec`, el `6_contracts` y el
> `9_checklist` de la v4.
>
> **Por qué está partido por versión.** Porque la conversación que genera el
> spec kit de la v4 **solo necesita los requisitos de la v4**. Ver
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

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v4-1** | Diez consultas de negocio que **cruzan** tablas y **agrupan** | `/api/consultas/*` (10 operaciones) |
| **RF-v4-2** | Un tablero con indicadores, no un listado | `http://localhost:8099/` |
| **RF-v4-3** | La interfaz sigue el **manual de marca**: ni un color a mano | [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) §6 |
| **RF-v4-4** | La interfaz **no habla en jerga**: ni «PUT», ni «422», ni «FK» | Recorrer las pantallas |
| **RF-v4-5** | Las v1, v2 y v3 siguen funcionando | Regresión completa |

**Las diez, tal como las publica la API:**

```
ventas-por-vendedor      ventas-por-cliente      ventas-por-producto
ventas-por-empresa       ticket-por-vendedor     anulaciones-por-cliente
credito-contra-consumo   productos-sin-vender    alcance-de-usuarios
interfaces-sin-usuarios
```

> **Por qué estas diez no las puede dar el CRUD.** Un CRUD responde *«dame las
> filas de `factura`»*. Ninguna de las diez es eso: son *«cuánto vendió cada
> vendedor»*, *«qué productos no se han vendido nunca»*, *«qué interfaces no
> alcanza ningún usuario»*. Eso es un `JOIN` con un `GROUP BY`, y no cabe en
> `GET /api/{tabla}` — por eso la v4 existe y no es «la v2 con más pantallas».

> **Y las dos últimas no son de ventas, son del propio sistema.**
> `alcance-de-usuarios` e `interfaces-sin-usuarios` responden *«¿quién puede
> entrar a qué?»* y *«¿qué pantalla no alcanza nadie?»*. La segunda es la más
> útil de las diez para administrar: una interfaz que ningún rol alcanza está
> construida y nadie la ve.

---

| Qué | Dónde |
|---|---|
| El requisito que manda en esta versión | [`v4_aplicativo/2_spec.md`](../spec_kit/versiones/v4_aplicativo/2_spec.md) |
| Las diez consultas, tabla por tabla | [`v4_aplicativo/6_contracts.md`](../spec_kit/versiones/v4_aplicativo/6_contracts.md) |
| Los colores y las tipografías | [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) |
| La versión anterior | [`RF_V3.md`](RF_V3.md) |
