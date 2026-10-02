# Contratos — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la capa de consultas y el tablero |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas: cero tablas nuevas |
> | **6_contracts.md** (este) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

> Los **70 endpoints** de [v1](../v1_sin_fk/6_contracts.md),
> [v2](../v2_con_fk/6_contracts.md) y
> [v3](../v3_control_acceso/6_contracts.md) **siguen vigentes sin un cambio**.
> La v4 agrega **diez**, todos de lectura.

## El sobre de una consulta NO es el del CRUD

```json
{ "consulta": "ventas_por_producto", "total": 8, "datos": [ … ] }
```

| El CRUD devuelve | Una consulta devuelve |
|---|---|
| `{tabla, limite, total, datos[]}` | `{consulta, total, datos[]}` |

> **No tiene `tabla` ni `limite`, y es a propósito:** una consulta no sale de
> una tabla —sale de cuatro o más— y responde sobre todo lo que hay.
>
> **Leer la clave equivocada devuelve una lista vacía SIN ningún error.** Es el
> mismo tropiezo del sobre del CRUD, y sigue siendo silencioso.

## Lo que las diez tienen en común

| | |
|---|---|
| **Método** | `GET`, siempre. Son preguntas |
| **Token** | **Obligatorio.** Sin él: `401` |
| **Permiso** | `interfaz.inicio`. Sin él: `403` |
| **Sin parámetros** | Responden sobre todo lo que hay |
| **`200` con `datos: []`** | Cuando no hay filas. **No es un 404**: la pregunta se pudo responder, y la respuesta es «ninguno» |
| **`500`** | Solo si el motor falla. Y el cuerpo lleva `detalle` |

> **Cero filas es `200`, no `404`.** El `404` diría «esa consulta no existe», y
> eso es otra cosa: la 6 vacía significa que **todo el catálogo se ha vendido**.

---

### 1. `GET /api/consultas/ventas-por-producto`

Cuánto se ha vendido de cada producto: unidades, ingreso, en cuántas facturas y a cuántos clientes distintos.

**Cruza:** producto · productosporfactura · factura · cliente

```json
{
  "consulta": "ventas_por_producto",
  "total": 3,
  "datos": [
    {"codigo": "P001", "nombre": "…", "unidades": 12, "ingreso": 2500000, "facturas": 3, "clientes": 2}
  ]
}
```

### 2. `GET /api/consultas/ventas-por-cliente`

Cuánto ha comprado cada cliente. El nombre sale de `persona`, la condición de cliente de `cliente`.

**Cruza:** persona · cliente · factura · productosporfactura

```json
{
  "consulta": "ventas_por_cliente",
  "total": 3,
  "datos": [
    {"id": 1, "cliente": "…", "email": "…", "facturas": 2, "comprado": 5000000, "unidades": 7}
  ]
}
```

### 3. `GET /api/consultas/ventas-por-vendedor`

Cuánto ha vendido cada vendedor, con su carné.

**Cruza:** persona · vendedor · factura · productosporfactura

```json
{
  "consulta": "ventas_por_vendedor",
  "total": 3,
  "datos": [
    {"id": 1, "vendedor": "…", "carnet": 1001, "facturas": 2, "vendido": 5000000}
  ]
}
```

### 4. `GET /api/consultas/ventas-por-empresa`

Cuánto se le ha facturado a cada empresa, y a través de cuántos clientes.

**Cruza:** empresa · cliente · factura · productosporfactura

```json
{
  "consulta": "ventas_por_empresa",
  "total": 3,
  "datos": [
    {"codigo": "E01", "empresa": "…", "clientes": 2, "facturas": 3, "facturado": 7500000}
  ]
}
```

### 5. `GET /api/consultas/ticket-por-vendedor`

El valor promedio de una factura, por vendedor. **Ojo con el tipo**: ver §4.

**Cruza:** persona · vendedor · factura · productosporfactura

```json
{
  "consulta": "ticket_por_vendedor",
  "total": 3,
  "datos": [
    {"vendedor": "…", "facturas": 2, "total": 5000000, "ticketPromedio": 2500000}
  ]
}
```

### 6. `GET /api/consultas/productos-sin-vender`

Los del catálogo que no aparecen en ninguna factura. **Cero filas es una respuesta**, no un error: significa que todo se ha vendido alguna vez.

**Cruza:** producto · productosporfactura · factura · cliente — con `LEFT JOIN`

```json
{
  "consulta": "productos_sin_vender",
  "total": 3,
  "datos": [
    {"codigo": "P009", "nombre": "…", "stock": 10, "valorunitario": 150000}
  ]
}
```

### 7. `GET /api/consultas/anulaciones-por-cliente`

Cuánto se ha anulado y de quién. Se llena **en cuanto alguien anula una factura** desde la interfaz de la v2 — y eso es lo que demuestra que el tablero lee del sistema vivo.

**Cruza:** persona · cliente · factura · productosporfactura

```json
{
  "consulta": "anulaciones_por_cliente",
  "total": 3,
  "datos": [
    {"cliente": "…", "anuladas": 1, "valorAnulado": 2500000}
  ]
}
```

### 8. `GET /api/consultas/alcance-de-usuarios`

Hasta dónde llega cada usuario: cuántos roles tiene y a cuántas interfaces alcanza. Es la cadena del control de acceso de la v3, recorrida completa.

**Cruza:** usuario · rol_usuario · rol · rutarol · ruta — **CINCO** tablas

```json
{
  "consulta": "alcance_de_usuarios",
  "total": 3,
  "datos": [
    {"email": "…", "roles": 2, "interfaces": 11, "susRoles": "Administrador, Cajero"}
  ]
}
```

### 9. `GET /api/consultas/interfaces-sin-usuarios`

**Es una consulta de auditoría.** Una interfaz protegida a la que ningún usuario llega, o sobra, o alguien se quedó sin el permiso que necesitaba.

**Cruza:** ruta · rutarol · rol · rol_usuario — con `LEFT JOIN`

```json
{
  "consulta": "interfaces_sin_usuarios",
  "total": 3,
  "datos": [
    {"id": 7, "ruta": "interfaz.rutas", "descripcion": "…", "rolesConAcceso": 1, "usuariosConAcceso": 0}
  ]
}
```

### 10. `GET /api/consultas/credito-contra-consumo`

El crédito de cada cliente contra lo que lleva consumido, y lo que le queda disponible.

**Cruza:** persona · empresa · cliente · factura

```json
{
  "consulta": "credito_contra_consumo",
  "total": 3,
  "datos": [
    {"cliente": "…", "empresa": "…", "credito": 10000000, "consumido": 5000000, "disponible": 5000000}
  ]
}
```
