# Glosario — Facturación (`bdfacturas`)

> **Qué es este archivo.** El vocabulario del dominio: cada término con **una
> sola** definición, y el nombre exacto que lleva en la base de datos, en la API y en la
> interfaz. Cuando los tres no coinciden, aquí se dice cuál manda.
>
> **Qué es un glosario y por qué no es una lista de palabras bonitas:**
> [`CONCEPTOS_GLOSARIO.md`](../conceptos/CONCEPTOS_GLOSARIO.md).
>
> **Qué NO es.** No es el modelo de datos —ése es
> [`DISENO_BD.md`](DISENO_BD.md)— ni las reglas —
> [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md)—. Aquí solo se dice **qué
> significa cada palabra**.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Cómo leer la tabla

Cada término trae cuatro cosas, y las cuatro hacen falta:

| Columna | Para qué |
|---|---|
| **Término** | La palabra en castellano, como la dice el negocio |
| **Definición** | Qué es, sin usar la propia palabra y sin decir cómo se implementa |
| **En la base de datos** | El nombre exacto de la tabla o la columna |
| **En la API** | La ruta o el campo JSON |

> **La cuarta columna es la que evita el 404.** La tabla se llama `rol_usuario`
> con subrayado y la ruta es `/api/rol-usuario` con guion. Son dos convenciones
> distintas —SQL y URL— y suponer una por la otra es media hora perdida.

---

## 2. Las personas y las empresas

| Término | Definición | En la base de datos | En la API |
|---|---|---|---|
| **Persona** | Ser humano identificado por un código, con nombre, correo y teléfono. **No** es todavía ni cliente ni vendedor: es el dato de la persona | `persona` | `/api/persona` |
| **Empresa** | Compañía a la que puede pertenecer un cliente. Solo tiene código y nombre | `empresa` | `/api/empresa` |
| **Cliente** | Persona —opcionalmente vinculada a una empresa— **a la que se le puede emitir una factura**, con un cupo de crédito | `cliente` | `/api/cliente` |
| **Vendedor** | Persona que emite facturas, con su carnet y su dirección | `vendedor` | `/api/vendedor` |
| **Crédito** | Cupo en dinero asignado a un cliente. Es un dato informativo: **el sistema no lo descuenta ni lo valida al facturar** | `cliente.credito` | `credito` |

> **Por qué `cliente` y `vendedor` son tablas aparte y no una columna «tipo» en
> `persona`.** Porque no son clases de persona: son **papeles** que una persona
> cumple, y la misma persona puede cumplir los dos. Un `tipo` obligaría a
> duplicar la fila para quien vende y compra; dos tablas con su propia llave
> foránea a `persona` no.

> **Y por qué `fkcodempresa` admite nulo.** Porque hay clientes que compran a
> título personal. Es la única llave foránea opcional del esquema, y por eso su
> desplegable en la interfaz lleva una opción «(ninguna)».

---

## 3. La factura y su detalle

| Término | Definición | En la base de datos | En la API |
|---|---|---|---|
| **Factura** | Documento que registra una venta: a quién, quién vendió, cuándo y por cuánto. Es el **maestro** | `factura` | `/api/factura` |
| **Renglón** | Una línea de la factura: qué producto y cuántas unidades. Es el **detalle** | `productosporfactura` | el arreglo `productos` |
| **Producto** | Artículo que se vende, con su precio y sus unidades disponibles | `producto` | `/api/producto` |
| **Stock** | Unidades disponibles de un producto. **Lo mueve la base de datos, no la API** | `producto.stock` | `stock` |
| **Valor unitario** | Precio de UNA unidad del producto **hoy** | `producto.valorunitario` | `valorunitario` |
| **Subtotal** | `cantidad × valor unitario` **en el momento en que se emitió la factura**. Se guarda, no se recalcula | `productosporfactura.subtotal` | `subtotal` |
| **Total** | Suma de los subtotales de la factura. Lo calcula un disparador | `factura.total` | `total` |
| **Estado** | `activa` o `anulada`. No hay otros | `factura.estado` | `estado` |
| **Anular** | Dejar la factura sin efecto **conservando la fila**, con su número y su fecha, y devolviendo el stock a los productos | — | `POST /api/factura/{n}/anular` |

> **«Renglón» no existe como palabra en la base de datos, y es deliberado.** La tabla se
> llama `productosporfactura` porque así llegó el esquema. En la conversación,
> en la interfaz y en este glosario se dice **renglón**, que es como lo llama
> quien factura. El glosario existe justamente para que esa diferencia esté
> escrita en algún lado.

> **Subtotal y total NO son lo mismo que `cantidad × valorunitario` de hoy.** El
> `subtotal` quedó congelado al emitir. Si mañana sube el precio del producto,
> las facturas viejas **no cambian** — y eso es un requisito, no un descuido.
> Ver [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) RN-07.

---

## 4. El control de acceso

| Término | Definición | En la base de datos | En la API |
|---|---|---|---|
| **Usuario** | Quien puede entrar al sistema, identificado por su correo | `usuario` | `/api/usuario` |
| **Contraseña** | La clave de un usuario. **En la base de datos vive cifrada**, nunca en claro, y no sale en ninguna respuesta | `usuario.contrasena` | solo de entrada |
| **Rol** | Papel que agrupa permisos: `Administrador`, `Vendedor`, `Cajero`, `Contador`, `Cliente` | `rol` | `/api/rol` |
| **Ruta** | Una interfaz o acción que se puede proteger. Es un dato, no una dirección de la API | `ruta` | `/api/ruta` |
| **Permiso** | Que un rol tenga concedida una ruta | `rutarol` | `/api/rutarol` |
| **Asignación** | Que un usuario tenga un rol | `rol_usuario` | `/api/rol-usuario` |
| **Token** | Credencial firmada que la API entrega al identificarse y que el cliente manda en cada petición | — | `POST /api/sesion` |
| **Sesión** | Que alguien esté identificado. **No hay tabla**: la sesión es el token, y vive en el cliente | — | `/api/sesion` |

> **Cuidado con `rol`, que significa dos cosas en este proyecto.** En la tabla
> `rol` es un **conjunto de permisos**. En la frase «el rol de una persona en la
> venta» es **lo que esa persona hace** —comprar o vender—, que aquí se resuelve
> con las tablas `cliente` y `vendedor`. Son dos conceptos con un solo nombre:
> es el caso de **homonimia** que
> [`CONCEPTOS_GLOSARIO.md`](../conceptos/CONCEPTOS_GLOSARIO.md) §3 describe.

> **Y `ruta` también.** La tabla `ruta` guarda **nombres de interfaces**
> —`interfaz.facturas`, `interfaz.productos`—, no direcciones HTTP. Una fila de
> `ruta` no es un endpoint: es una etiqueta que el control de acceso usa para
> decidir. Confundirlas lleva a buscar `/api/interfaz.facturas`, que no existe.

---

## 5. Lo que NO está en este dominio

Decirlo evita que alguien lo busque, y evita que alguien lo invente:

| No existe | Por qué |
|---|---|
| **Impuestos, IVA, descuentos** | El total es la suma de los subtotales, sin más. Agregarlos sería cambiar el dominio |
| **Devoluciones parciales** | Una factura se anula entera o no se anula |
| **Proveedores, compras, inventario de entrada** | El stock baja al vender y sube al anular. Cómo llegó ahí está fuera del alcance |
| **Borrado lógico en los catálogos** | `producto`, `empresa` y `persona` se eliminan de verdad. La única baja lógica del sistema es **anular** una factura |
| **Sucursales, bodegas, zonas** | Hay un solo punto de venta |

> **Esa última fila importa para el proyecto de aula.** Allá el borrado **sí** es
> lógico —se marca `activo = 0` y los listados filtran—, y aquí no. Es de las
> pocas cosas donde copiar el ejemplo tal cual deja mal al equipo.

---

## 6. Las palabras que NO se usan

Un glosario sirve tanto por lo que define como por lo que prohíbe:

| No se dice | Se dice | Por qué |
|---|---|---|
| comprador, adquiriente | **cliente** | La tabla se llama `cliente` y la interfaz también |
| ítem, línea, detalle *(en singular)* | **renglón** | «Detalle» se reserva para el conjunto: *el detalle de la factura* |
| borrar una factura | **anular** una factura | Son dos cosas distintas y las dos existen. Ver §3 |
| usuario *(para quien compra)* | **cliente** | `usuario` es quien entra al sistema. Un cliente puede no tener usuario |
| precio | **valor unitario** | Es el nombre de la columna, y evita confundirlo con el subtotal |

---

## 7. Dónde se comprueba cada término

Ninguna definición de este archivo es una opinión: todas salen del esquema o
del código, y se pueden verificar.

| Qué | Dónde |
|---|---|
| Las tablas y sus columnas | [`db/bdfacturas.sql`](../../db/bdfacturas.sql) |
| El modelo, con su razón | [`DISENO_BD.md`](DISENO_BD.md) |
| Las reglas | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Las rutas y sus formatos | el `6_contracts.md` de cada versión |
| Los nombres en la interfaz | `front_blazor/Components/Pages/` |
