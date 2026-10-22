# Diseño de base de datos — `bdfacturas`

> **Qué es este documento.** El modelo de datos en las cuatro etapas que
> [`CONCEPTOS_DISENO_BD.md`](../conceptos/CONCEPTOS_DISENO_BD.md) define:
> conceptual → selección del motor → lógico → físico. Y **por qué** quedó así.
>
> **Qué NO es.** No es el script. [`db/bdfacturas.sql`](../../db/bdfacturas.sql)
> es la **salida** de la etapa física, no su sustituto: un script dice *qué*
> quedó, no *por qué*.
>
> **Material académico simulado.** El esquema es verosímil y funciona; el dominio
> lo fijó el curso.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Conceptual — las entidades y cómo se relacionan

Doce tablas en cuatro grupos:

| Grupo | Tablas | Qué resuelve |
|---|---|---|
| **Catálogos** | `producto` · `empresa` · `persona` | Lo que existe antes de cualquier venta |
| **Papeles** | `cliente` · `vendedor` | Lo que una persona *hace* |
| **La venta** | `factura` · `productosporfactura` | El maestro y su detalle |
| **Acceso** | `usuario` · `rol` · `ruta` · `rol_usuario` · `rutarol` | Quién entra y a qué |

### Las relaciones, con su cardinalidad

| Desde | Hacia | Cardinalidad | Participación |
|---|---|---|---|
| `cliente` | `persona` | N:1 | **total** — todo cliente es una persona |
| `cliente` | `empresa` | N:1 | **parcial** — hay clientes sin empresa |
| `vendedor` | `persona` | N:1 | **total** |
| `factura` | `cliente` | N:1 | **total** |
| `factura` | `vendedor` | N:1 | **total** |
| `productosporfactura` | `factura` | N:1 | **total** — no hay renglón sin factura |
| `productosporfactura` | `producto` | N:1 | **total** |
| `usuario` ↔ `rol` | vía `rol_usuario` | **N:M** | parcial por los dos lados |
| `ruta` ↔ `rol` | vía `rutarol` | **N:M** | parcial por los dos lados |

> **La participación total es la que se olvida**, y es la que produce datos
> huérfanos. «Todo renglón pertenece a una factura» no es cardinalidad: es que
> `fknumfactura` es `NOT NULL`. Si fuera opcional, la base de datos aceptaría renglones
> que no son de nadie, y nadie los vería nunca más.

### La decisión que define el modelo: `cliente` y `vendedor` aparte

Lo fácil habría sido una columna `tipo` en `persona` con los valores `cliente` y
`vendedor`. Se descartó, y por una razón concreta:

> **Una persona puede ser las dos cosas.** Con una columna `tipo` habría que
> duplicar la fila —el mismo ser humano, dos veces— y entonces el correo y el
> teléfono quedan en dos sitios que se desincronizan. Con dos tablas que apuntan
> a `persona`, el dato de la persona vive **una sola vez** y los papeles son
> filas aparte.

**Y no es una hipótesis: está en los datos sembrados.** `persona` tiene 6 filas,
y entre `cliente` y `vendedor` hay 7:

| Persona | ¿cliente? | ¿vendedor? |
|---|---|---|
| P001 Ana Torres | sí | — |
| P002 Carlos Pérez | — | sí |
| P003 María Gómez | sí | — |
| P004 Juan Díaz | — | sí |
| P005 Laura Rojas | sí | — |
| **P006 Pedro Castillo** | **sí** | **sí** |

Pedro Castillo compra y vende. Con una columna `tipo` en `persona` habría dos
filas con su nombre, su correo y su teléfono — y el día que cambie de número,
una de las dos se queda vieja.

---

## 2. Selección del motor

**SQL Server** de la v1 a la v4. **PostgreSQL** se suma en la v5.

| Qué pide este dominio | Qué se necesitaba |
|---|---|
| Un total que no puede descuadrarse | **Transacciones** y **disparadores** |
| Un stock que no puede quedar negativo | Un rechazo que la aplicación no pueda evadir |
| Una factura que no se corrige | Integridad referencial con `ON DELETE` explícito |

Un motor sin transacciones queda descartado de entrada: esto es facturación, y
media factura no existe. Ver [`PRINCIPIOS_ACID.md`](../conceptos/PRINCIPIOS_ACID.md).

> **Y por qué DOS motores y no uno.** Porque la v5 existe para demostrar que el
> sistema está **abierto al cambio**: el mismo contrato, la misma interfaz, otro
> motor debajo, y sin tocar un controlador. Eso no se puede probar con un solo
> motor — se afirma.

---

## 3. Lógico — las llaves y los tipos

### Las llaves primarias, y por qué no son todas iguales

| Tabla | Llave | Quién la pone | Por qué |
|---|---|---|---|
| `producto` · `empresa` · `persona` | `codigo` NVARCHAR(10) | **quien crea la fila** | Son códigos del negocio: `PR001`, `E001`. La gente los dice en voz alta |
| `usuario` | `email` | quien crea la fila | El correo ya identifica a la persona; un `id` aparte sería un dato más que mantener |
| `rol` · `ruta` · `cliente` · `vendedor` · `factura` | `id` / `numero` INT **IDENTITY** | **la base de datos** | No hay un código natural. Inventarlo sería pedirle a alguien que lleve la cuenta |
| `productosporfactura` | `(fknumfactura, fkcodproducto)` | — | **Compuesta**: el renglón *es* esa pareja |
| `rol_usuario` · `rutarol` | las dos columnas | — | Igual: una asignación existe o no existe |

> **Las tres llaves compuestas no llevan `id` propio a propósito.** Un `id` en
> una tabla puente permite dos filas con la misma pareja y distinto id — que es
> exactamente lo que la tabla existe para impedir.

### Los tipos que importan

| Columna | Tipo | Por qué ése |
|---|---|---|
| `valorunitario` · `subtotal` · `total` · `credito` | `DECIMAL(18,2)` | **Nunca `FLOAT` para dinero.** `0.1 + 0.2` en coma flotante no da `0.3`, y en una factura eso es un centavo que nadie encuentra |
| `fecha` | `DATETIME2` con `DEFAULT GETDATE()` | La pone la base de datos. Si la mandara el cliente, dependería del reloj de su máquina |
| `estado` | `NVARCHAR(10)` con `DEFAULT N'activa'` | Dos valores: `activa` y `anulada` |
| `contrasena` | `NVARCHAR(200)` | Guarda el **hash**, no la clave. 200 deja espacio para un algoritmo futuro más largo |

### Lo derivado que SÍ se guarda, y por qué

Normalmente un dato derivado no se guarda. Aquí hay dos excepciones, y las dos
son el mismo caso:

| Dato | Se podría calcular | Y aun así se guarda porque… |
|---|---|---|
| `productosporfactura.subtotal` | `cantidad × valorunitario` | …el precio de **hoy** no es el de cuando se emitió. Recalcularlo **reescribiría las ventas del año pasado** |
| `factura.total` | sumando los subtotales | …es el valor del documento. Y lo mantiene un disparador, así que no puede quedar desactualizado |

> **La regla no es «nunca guardes derivados», es «guárdalos cuando congelarlos
> sea el requisito»** — y aquí lo es.

---

## 4. Físico — lo que el script hace

### Los tres disparadores

| Disparador | Cuándo | Qué hace |
|---|---|---|
| `trg_prodfact_insert` | al agregar un renglón | Comprueba stock (`THROW 50001` si no alcanza) · calcula el `subtotal` · **baja** el stock · recalcula el `total` |
| `trg_prodfact_update` | al cambiar un renglón | Lo mismo, ajustando la diferencia |
| `trg_prodfact_delete` | al quitar un renglón | **Devuelve** el stock · recalcula el `total` |

> **Por qué en la base de datos y no en C#.** Porque la regla tiene que valer también para
> quien entre por SSMS. Una validación que solo vive en la aplicación protege a
> quien pasa por la aplicación.

### Los dos `ON DELETE CASCADE`, y los que NO lo llevan

| Relación | Al borrar el padre | Por qué |
|---|---|---|
| `productosporfactura` → `factura` | **CASCADE** | Un renglón sin su factura no significa nada |
| `rutarol` → `ruta` y → `rol` | **CASCADE** | Una asignación a un rol que ya no existe tampoco |
| `factura` → `cliente` / `vendedor` | **sin cascada** | Borrar un cliente **no** puede llevarse sus facturas. La base de datos lo impide, y está bien que lo impida |
| `productosporfactura` → `producto` | **sin cascada** | Igual: un producto vendido no se puede borrar |

> **Esos dos «sin cascada» son una decisión, no un olvido.** Significan que un
> cliente con facturas **no se puede borrar**, y que la API responde el error del
> motor. Es el comportamiento correcto para un sistema donde la factura es un
> documento.

### Los 16 procedimientos

| Grupo | Para qué |
|---|---|
| `sp_*_factura_y_productosporfactura` (5) | Maestro y detalle **en una transacción**: listar, consultar, insertar, actualizar, borrar |
| `sp_anular_factura` | El borrado lógico de la factura, con devolución de stock |
| `*_usuario_con_roles` (5) + `actualizar_roles_usuario` | El usuario y sus roles juntos |
| `verificar_acceso_ruta` | La pregunta del control de acceso, en cada petición |
| `listar/crear/eliminar_rutarol` | Los permisos |

> **La factura se opera SIEMPRE por procedimiento, nunca con SQL suelto desde
> C#.** Y no es gusto: insertar el encabezado y tres renglones son cuatro
> escrituras, y tienen que ser **una sola operación**. Dentro del procedimiento
> hay una transacción; desde C# serían cuatro viajes y cuatro oportunidades de
> quedar a medias.

---

## 5. Lo que este modelo NO resuelve

| No está | Consecuencia |
|---|---|
| Impuestos, descuentos | El total es la suma de subtotales |
| Devoluciones parciales | Una factura se anula entera |
| Entradas de inventario | El stock baja al vender y sube al anular; cómo llegó ahí, no se modela |
| Histórico de precios | Solo el precio de hoy en `producto` y el congelado en cada renglón |
| Borrado lógico en catálogos | `producto`, `empresa` y `persona` se borran de verdad |

> Esa última fila **es distinta en el proyecto de aula**, donde el borrado sí es
> lógico. Copiar el ejemplo tal cual deja mal al equipo.

---

## 6. Cómo comprobar todo esto

```powershell
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa `
  -P "Paradigmas123!" -C -d bdfacturas -Q "SELECT name FROM sys.tables ORDER BY name;"
```

| Qué | Dónde |
|---|---|
| El script completo | [`db/bdfacturas.sql`](../../db/bdfacturas.sql) |
| El mismo esquema en PostgreSQL | [`db/bdfacturas_postgres.sql`](../../db/bdfacturas_postgres.sql) |
| Las reglas que defiende | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Los datos sembrados | [`DATOS_DE_PRUEBA.md`](DATOS_DE_PRUEBA.md) |
| El modelo por versión | el `5_data_model.md` de cada una |
