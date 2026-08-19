# Especificación — Versión 2: las tablas CON clave foránea (con la v2 están las 12)

> | | |
> |---|---|
> | **La ruta** | [0_mapa_versiones.md](../0_mapa_versiones.md) |
> | **Lo anterior** | [v1](../v1_sin_fk/2_spec.md) — las seis tablas **sin** clave foránea |
> | **Lo que rige** | [1_constitution.md](../../1_constitution.md) |
>
> **La v2 INCLUYE la v1.** Los seis recursos de la v1 no se tocan, sus
> contratos siguen vigentes y sus criterios **se vuelven a correr** — eso es la
> regresión.
>
> | Documento | Contenido |
> |---|---|
> | **2_spec.md** (este) | QUÉ agrega la v2 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: los archivos nuevos y el diseño |
> | [4_research.md](4_research.md) | Las decisiones y sus alternativas |
> | [5_data_model.md](5_data_model.md) | Las seis tablas, los procedimientos y el disparador |
> | [6_contracts.md](6_contracts.md) | Los endpoints nuevos, con formatos exactos |
> | [7_quickstart.md](7_quickstart.md) | Arranque, regresión y prueba de humo |
> | [8_tasks.md](8_tasks.md) | El orden de construcción, por fases verificables |

---

## 1. Propósito de la v2

**Cerrar el modelo.** Con la v1 están las seis tablas que no dependen de
nadie; con la v2 llegan las seis que **sí** dependen, y entonces están las
**12** de `bdfacturas`.

Y con ellas llegan **tres cosas que la v1 no podía enseñar**, porque sin
claves foráneas no existen:

| | Qué aparece | Por qué la v1 no lo tenía |
|---|---|---|
| **1** | **La dependencia entre filas.** Un cliente no existe sin su persona, y el motor lo hace cumplir | Ninguna de las seis tablas de la v1 tenía clave foránea: no había nada que violar |
| **2** | **La lógica que vive en la base.** `factura` es maestro-detalle, y sus reglas pesadas —subtotales, total, stock— las hacen **procedimientos almacenados y un disparador**, no C# | La v1 era SQL plano: un `SELECT` por operación |
| **3** | **El desplegable en vez del campo de texto.** La clave foránea se **elige** de una lista traída de la API | Sin claves foráneas no había nada que elegir |

```
┌──────────────────────── el sistema, al cerrar la v2 ────────────────────────┐
│ CONTROLADOR │ los 6 de la v1 ███ │ cliente █ vendedor █ factura █ puentes █ │
│ SERVICIO    │ los 6 de la v1 ███ │ ídem                                     │
│ REPOSITORIO │ los 6 de la v1 ███ │ ídem — y factura NO escribe SQL de tablas│
│ BASE        │ las 12 tablas · los procedimientos · el disparador            │
└─────────────┴────────────────────┴──────────────────────────────────────────┘
               intacta, y se le                 lo que la v2 agrega
               vuelve a correr
```

## 2. Alcance

> **Qué define la v2:** **las tablas que SÍ tienen clave foránea.** Son seis:
> `cliente`, `vendedor`, `factura`, `productosporfactura`, `rol_usuario` y
> `rutarol`. Las cuatro primeras son del negocio; las dos últimas son **tablas
> puente**.
>
> **`persona`, `empresa`, `producto`, `rol`, `ruta` y `usuario` NO son de esta
> versión: son de la v1**, porque no tienen clave foránea. Si alguna aparece
> aquí como trabajo nuevo, es que **la v1 quedó incompleta**.

**Incluye:**

- **CRUD de `cliente` y de `vendedor`**, con los cinco verbos, calcados del
  molde de la v1. Lo nuevo es la **clave foránea**: obligatoria en vendedor,
  y en cliente una obligatoria (`fkcodpersona`) y otra opcional
  (`fkcodempresa`).
- **`factura` por procedimientos**: listar, consultar una, **crear** el
  encabezado con sus renglones en **una** transacción, y **anular** —borrado
  lógico que devuelve el stock—. La API **no escribe SQL de tablas** para
  factura: llama procedimientos.
- **Las dos tablas puente**, `rol_usuario` y `rutarol`, con lo que una tabla
  puente necesita y nada más: listar por los dos lados, agregar una pareja y
  quitarla **con sus dos claves**.
- **`api/usuario-con-roles`**: el usuario **y** sus roles como una sola
  operación, con los cinco procedimientos que la base ya trae. No es una tabla
  nueva — es otra forma de operar `usuario` y `rol_usuario`.
- **Un código HTTP nuevo: el 409.** Clave foránea inexistente, pareja
  repetida, factura ya anulada.
- **Las interfaces gráficas de todo lo anterior**, con los desplegables
  cargados de la API y los **dos formularios integrados**.
- El diagnóstico `/` pasa a reportar `"version": "v2"`.

**No incluye, y es deliberado:**

| | Dónde está |
|---|---|
| **Token, sesión, 401 y 403** | La **v3**. Ojo con la confusión: el **CRUD** de `rol_usuario` y `rutarol` **sí es de esta versión** —tienen clave foránea—. Lo que llega en la v3 **no es su CRUD: es la puerta**. Administrar los permisos y *hacerlos valer* son dos cosas distintas |
| **Editar o borrar físicamente una factura** | En ninguna. `sp_actualizar_factura_y_productosporfactura` y `sp_borrar_…` **existen en la base** y la API **no los expone**: la operación del negocio es **anular** |
| **Consultas multitabla, tablero, marca, publicación** | La **v4** |
| **Otros motores y la fábrica** | La **v5**, después del curso |

## 3. Requisitos funcionales

### RF1 — CRUD de `cliente`, con dos claves foráneas

Los seis endpoints del molde sobre `/api/cliente`, con PK **entera generada
por la base** (`IDENTITY`) y dos claves foráneas.

| Campo | Regla |
|---|---|
| `id` | **Lo genera la base.** No se envía al crear, y enviarlo no lo cambia |
| `credito` | Decimal, no negativo |
| `fkcodpersona` | **Obligatoria** → `persona.codigo` |
| `fkcodempresa` | **Opcional y nullable** → `empresa.codigo`. Un cliente puede ser persona natural |

**Y aquí aparece el 409:** mandar un `fkcodpersona` que no existe no es un
error de forma —es un texto válido— sino un **conflicto con el estado** de la
base.

### RF2 — CRUD de `vendedor`, con una clave foránea obligatoria

Idéntico al molde, con `carnet`, `direccion` y `fkcodpersona` —esta última
obligatoria, porque un vendedor siempre es una persona—.

**El requisito que sirve para ver la integridad referencial en acción:**
borrar una `persona` que ya es vendedor tiene que **fallar**, con el nombre de
la restricción del motor en el `detalle`. La base protege sus relaciones, y
eso no se programa: ya está.

### RF3 — `factura`: cuatro operaciones, todas por procedimiento

| Operación | Procedimiento | Qué devuelve |
|---|---|---|
| Listar | `sp_listar_facturas_y_productosporfactura` | Todas, con los **nombres** de cliente y vendedor ya resueltos y el detalle **anidado** |
| Consultar una | `sp_consultar_factura_y_productosporfactura` | Una, con su detalle. Inexistente → **404** |
| Crear | `sp_insertar_factura_y_productosporfactura` | El encabezado y los renglones en **una** transacción |
| Anular | `sp_anular_factura` | Estado `'anulada'` y el stock devuelto. Ya anulada → **409** |

**Tres reglas, y las tres son del mismo tipo:**

| | |
|---|---|
| **La API no hace `JOIN`** | Los nombres vienen resueltos del procedimiento |
| **La API no calcula nada** | Ni subtotales, ni total, ni stock: eso es el disparador `trg_actualizar_totales_y_stock` |
| **La API no inventa el orden** | El procedimiento inserta el encabezado y los renglones; si falla uno, no entra ninguno |

> **Por qué así, y no con tres `INSERT` desde C#:** porque entonces la
> transacción la tendría que manejar la API, y el día que alguien cambie la
> regla del total habría que cambiarla en dos sitios. Si un número sale mal, el
> error se busca **en la base**, no en el servicio.

### RF4 — Las dos tablas puente, con lo que una puente necesita

`rol_usuario` (qué roles tiene cada usuario) y `rutarol` (qué rol entra a qué
interfaz). Las dos tienen **clave primaria compuesta** y ningún campo propio.

| | |
|---|---|
| **Listar por los DOS lados** | `/usuario/{email}` y `/rol/{idrol}`. Una puente se consulta por sus dos extremos |
| **Agregar una pareja** | Repetirla → **409**: la clave primaria son las dos columnas juntas |
| **Quitarla con las DOS claves** | Con una sola no se sabe cuál pareja quitar |
| **Sin PUT ni PATCH** | La fila no tiene campos. **Existe o no existe** |

### RF5 — `api/usuario-con-roles`: el usuario Y sus roles, en una operación

Los cinco procedimientos que la base ya trae:
`listar_usuarios_con_roles`, `consultar_usuario_con_roles`,
`crear_usuario_con_roles`, `actualizar_usuario_con_roles` y
`eliminar_usuario_con_roles`.

| | Por qué |
|---|---|
| **No se crea el usuario y después se le asignan los roles** | Si falla la segunda llamada queda un usuario **sin ningún rol** —que no puede hacer nada y que nadie sabe que está ahí—. `crear_usuario_con_roles` existe para que sea **una** |
| **No se arma el `JOIN` en C#** | `listar_usuarios_con_roles` ya trae los roles pegados. Repetirlo en la API es tener la consulta en dos sitios |
| **La contraseña vacía al editar significa «déjela como está»** | No significa borrarla. Sin esta regla, cambiarle un rol a alguien le borra la clave |
| **Los roles se REEMPLAZAN, no se suman** | La lista que llega es la que queda |

> **Convive con `api/usuario` y con `api/rol-usuario`, y los tres se quedan.**
> El primero administra la tabla sola; el segundo, la puente pareja a pareja;
> este, el conjunto. **La interfaz gráfica usa el tercero**, porque es el único
> que no deja a nadie a medias.

### RF6 — La v1 queda intacta

Los contratos de los seis recursos de la v1 siguen cumpliéndose al pie de la
letra. **Lo único que cambia es `"version": "v2"`** en el diagnóstico.

### RF7 — La clave foránea es un DESPLEGABLE, no un campo de texto

Una interfaz gráfica por recurso, y en cada una la clave foránea se **elige**
de una lista que la interfaz **pide a la API al abrirse**.

| | |
|---|---|
| **Qué hace** | Al cargar, pide el catálogo del recurso referenciado y llena el desplegable |
| **Qué NO hace** | Pedirle a la persona que escriba `P001`. Si hay que digitar la llave, el que la descubre mal es el motor, y la persona ve un error que no entiende |
| **Qué se revisa** | Que muestre **el nombre** y mande **el código**. Es la diferencia entre lo que la persona lee y lo que viaja en el JSON |
| **El campo opcional** | `fkcodempresa` lleva una opción **«(ninguna)»**, y lo que viaja es `null` — **no** cadena vacía, que sería un código de empresa inexistente |

### RF8 — El formulario integrado de factura (maestro-detalle)

**Una sola interfaz gráfica que maneja la factura Y sus renglones.** Es el
corazón de la v2.

| Parte | Qué lleva |
|---|---|
| **El maestro** | Cliente y vendedor, **como desplegables** |
| **El detalle** | Una tabla donde se **agregan y quitan renglones ANTES de guardar**: producto (desplegable) y cantidad |
| **El total** | Se **muestra** calculado, y **no se envía** |
| **El guardado** | **UN solo envío** con la factura y todos sus renglones |

**Tres cosas que se van a querer hacer y no se deben:**

| | Por qué no |
|---|---|
| **Guardar la factura primero y los renglones después** | Si falla la segunda llamada queda una factura sin renglones. El procedimiento existe para que sea **una** operación |
| **Calcular el total en el front y mandarlo** | Lo pone el disparador. Si el front lo manda hay **dos verdades**, y el día que no coincidan gana la que nadie revisó |
| **Poner un botón de «eliminar factura»** | La palabra es **anular**, y además **devuelve el stock** |

> **La pregunta que hay que hacerle a la interfaz, y la que no se puede
> simular:** agregue **tres** renglones, quite **uno**, guarde. **¿Llegaron
> dos?** Si el detalle se envía renglón por renglón a medida que se agrega, van
> a llegar tres.

### RF9 — El formulario integrado de usuario con sus roles

**El mismo patrón, sobre una tabla puente.** Los datos del usuario arriba y
los roles como **casillas** —no un desplegable de uno, porque un usuario tiene
varios—. Un solo envío.

| | |
|---|---|
| **Marcar una casilla NO llama a la API** | Se guarda en memoria, igual que los renglones de la factura |
| **Al editar, las casillas arrancan con lo que el usuario YA tiene** | Editar es ver el estado actual, no empezar de cero |
| **Al editar, la contraseña va VACÍA** | En blanco significa «no la cambie» |

> **Y `rutarol` es el tercer caso del patrón.** Su interfaz **administra** la
> tabla; **no decide permisos todavía** —eso es la v3—. La interfaz lo dice con
> todas las letras, porque tener esa tabla llena no protege nada.

## 4. Requisitos no funcionales

- **RNF1 — Los de la v1 siguen todos:** capas estrictas, sin ORM, SQL
  parametrizado, `async`, errores uniformes.
- **RNF2 — La lógica de facturación NO se duplica en C#.** Ni subtotales, ni
  total, ni stock. Si un número sale mal, el error está en la base.
- **RNF3 — Los errores de la base también son contrato.** El repositorio
  traduce los `RAISE EXCEPTION` de los procedimientos —todos llegan con
  `el número de error P0001`, sin número— **por patrón del mensaje**: «no existe» → 404,
  «anulada» → 409. Las señales de error del motor son parte de la interfaz.
- **RNF4 — Sin anticipación.** Ni fábricas, ni motores nuevos, ni token.

## 5. Criterios de aceptación

### La API

| | |
|---|---|
| **1** | **Regresión:** `docker compose up -d --build` —un comando— y la prueba de humo **de la v1** pasa completa, con lo único cambiado siendo `"version":"v2"` |
| **2** | **`cliente`, los cinco verbos** con su `id` generado por la base, y la pareja didáctica: el **mismo** cuerpo da **422** en PUT y **200** en PATCH |
| **3** | **El 409 de la clave foránea:** crear un cliente con un `fkcodpersona` inexistente responde **409** — y un cliente **sin** empresa se crea con `fkcodempresa: null` |
| **4** | **`vendedor`, los cinco verbos**, y **borrar la persona de un vendedor falla** con el nombre de la restricción del motor en el `detalle` |
| **5** | **Lectura maestro-detalle:** `GET /api/factura` trae las facturas sembradas con los **nombres** de cliente y vendedor y sus renglones **anidados**; una inexistente → **404** |
| **6** | **El disparador trabaja:** anotar el stock de dos productos → crear una factura con ellos → la respuesta trae subtotales y total calculados (total = Σ subtotales) y el stock de los dos **bajó**. La API no multiplicó nada |
| **7** | **Los errores del negocio:** `productos: []` → **422** · cantidad mayor que el stock → **500** con el mensaje del disparador en `detalle` · anular → **200** y el stock **volvió** · anular otra vez → **409** · anular una inexistente → **404** |
| **8** | **Las puentes:** agregar una pareja → **200**; la misma otra vez → **409**; quitarla **con sus dos claves** → **200**; y listar por los dos lados devuelve lo que debe |
| **9** | **`usuario-con-roles`:** crear uno con **dos** roles en **un** envío y que la lista lo muestre con los dos · editarlo con la contraseña **vacía** y que la contraseña **siga sirviendo** · mandarle **un** rol y que quede con **uno** (reemplazan, no se suman) |
| **10** | **Prueba de capas:** `dotnet run --project pruebas` ejercita los servicios con repositorios falsos **sin SQL Server** |

### Las interfaces gráficas — **la versión no está cerrada sin estos**

| | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **11** | Abrir `/clientes` | El desplegable de persona está **lleno**. Si está vacío, la interfaz no pidió el catálogo |
| **12** | Mirar lo que viaja al crear | Se ve «Ana Torres» y en el JSON va `P001` |
| **13** | En `/facturas/nueva`: agregar **tres** renglones, quitar **uno**, emitir | **Llegan dos.** Si llegan tres, el detalle se está enviando a medida que se agrega |
| **14** | Mirar el JSON que sale al emitir | **No lleva `total`** ni subtotales |
| **15** | Buscar el botón de eliminar una factura | **No existe.** Hay **anular**, y después de anular el stock **subió** |
| **16** | En `/usuario-con-roles`: crear uno con dos casillas marcadas | Un solo envío, y la lista lo muestra con sus dos roles |
| **17** | Editarlo dejando la contraseña vacía | Los roles cambian y **la contraseña sigue sirviendo** |
| **18** | **Apagar la API** y recargar cualquier interfaz | **Sigue en pie**, con su aviso, el menú intacto y **cero** filas. No una página de error del navegador |

> **El 13 y el 18 son los que no se pueden simular.** Los otros se pueden
> aparentar con una interfaz bien dibujada. Esos dos, no.

## 6. Definición de TERMINADA

1. Los **18** criterios en verde, verificados con [7_quickstart.md](7_quickstart.md).
2. La **regresión** de la v1 pasa.
3. [9_checklist.md](9_checklist.md) firmado **antes** de la primera línea de código.
4. Commit y **tag `v2`** en `main`.

## 7. Clarificaciones

> **Qué es esta sección:** el registro de las ambigüedades detectadas **antes**
> de planear, con la respuesta que se acordó **y su razón**. Es la compuerta 1
> del método: mientras quede un `[NECESITA ACLARACIÓN: …]` arriba, esta versión
> no pasa a la planeación.

| # | La pregunta | La respuesta, con su razón | Dónde quedó |
|---|---|---|---|
| C1 | La clave foránea inexistente, ¿422 o 409? | **409.** El dato tiene la forma correcta —es un texto válido— y lo que se rompe es el **estado** de la base. El 422 se reserva para el cuerpo mal formado | RF1 · contrato de `cliente` |
| C2 | `fkcodempresa` sin empresa, ¿cadena vacía o `null`? | **`null`.** La columna es nullable; `""` sería un código de empresa que no existe, y daría 409 | RF1 · RF7 |
| C3 | Una factura equivocada, ¿se borra o se anula? | Se **anula**: borrado lógico que devuelve el stock. Es un hecho contable; borrarla perdería la trazabilidad | RF3 · contrato de anular |
| C4 | Anular dos veces la misma factura, ¿qué responde? | **409.** La factura existe (no es 404) y el cuerpo está bien (no es 422): el conflicto es de **estado** | RF3 |
| C5 | El total de la factura, ¿lo manda el front o lo calcula la base? | **La base**, con su disparador. Dos fuentes de verdad para un número es garantía de que un día no coincidan | RF3 · RF8 |
| C6 | El detalle de la factura, ¿se envía renglón por renglón o junto? | **Junto, en un solo envío.** Renglón por renglón, un fallo a mitad deja una factura incompleta — y la transacción del procedimiento no sirve de nada | RF8 · criterio 13 |
| C7 | ¿Por qué un recurso `usuario-con-roles` si ya hay `usuario` y `rol-usuario`? | Porque crear el usuario y asignarle los roles son **dos** operaciones, y si falla la segunda queda un usuario sin rol. Los tres recursos se quedan: administran cosas distintas | RF5 |
| C8 | La contraseña vacía al editar, ¿la borra o la deja? | **La deja.** Vacío significa «no la cambie». Lo contrario haría que cambiar un rol borrara la clave | RF5 · RF9 |
