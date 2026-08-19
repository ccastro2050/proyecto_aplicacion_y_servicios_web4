# Contratos HTTP — Versión 2: los recursos con clave foránea

> | | |
> |---|---|
> | **La ruta** | [0_mapa_versiones.md](../0_mapa_versiones.md) |
> | **Lo anterior** | [v1](../v1_sin_fk/6_contracts.md) — **sigue vigente sin un solo cambio** |
> | **La API** | `http://localhost:8035` · Swagger en `/swagger` |
> | **La interfaz gráfica** | `http://localhost:8099` |

**Aquí está solo lo NUEVO.** Las convenciones son las mismas de la v1 y no se
repiten: el sobre en los listados, los errores como
`{estado, mensaje, detalle}`, el 422 con `errores:[…]`.

> **La v2 INCLUYE la v1.** Los seis recursos sin clave foránea siguen
> respondiendo igual, y sus criterios se vuelven a correr — eso es la
> regresión.

---

## Lo que esta versión agrega al contrato, y no estaba en la v1

| | Por qué aparece ahora |
|---|---|
| **El 409 por clave foránea** | En la v1 ninguna tabla tenía FK, así que no había forma de violarla. Ahora sí: mandar un `fkcodpersona` que no existe es un **conflicto con el estado** de la base, no un error de forma |
| **Un recurso sin PUT ni PATCH ni DELETE** | `api/factura`. Una factura emitida no se corrige: **se anula** |
| **Dos recursos con clave compuesta** | `api/rol-usuario` y `api/rutarol`. Su borrado lleva **dos** valores en la URL |
| **Un recurso que no es una tabla** | `api/usuario-con-roles`. Opera `usuario` y `rol_usuario` juntas, en una transacción |

---

## A. `api/cliente` — CRUD con DOS claves foráneas (6 endpoints)

Mismos verbos y mismos códigos que los de la v1. Lo nuevo son las dos FK.

```
GET    /api/cliente[?limite=N]   → 200 {tabla:"cliente", limite, total, datos:[…]}
                                   · 204 vacia · 400 si limite <= 0
GET    /api/cliente/{id}         → 200 {"id":1,"credito":500000,
                                        "fkcodpersona":"P001","fkcodempresa":"E001"}
                                   · 404 si no existe
POST   /api/cliente              body {credito, fkcodpersona, fkcodempresa?}
                                 → 200 {estado, mensaje} · 422 · 409 si la FK no existe
PUT    /api/cliente/{id}         body {credito, fkcodpersona, fkcodempresa?} — TODO obligatorio
                                 → 200 · 422 si falta cualquier campo · 404
PATCH  /api/cliente/{id}         body parcial
                                 → 200 · 400 si el body va vacio · 404
DELETE /api/cliente/{id}         → 200 · 404
```

| | |
|---|---|
| **`id`** | Lo genera la base (`IDENTITY`). **No se envía al crear**, y mandarlo no lo cambia |
| **`fkcodpersona`** | **Obligatoria.** Un cliente es una persona |
| **`fkcodempresa`** | **Opcional y nullable.** Un cliente puede ser persona natural. Se manda `null`, **no cadena vacía**: `""` sería un código de empresa que no existe, y la base responde 409 |

**La pareja didáctica, en cliente:** `{"credito":900000}` → **422** en PUT
(falta `fkcodpersona`) y **200** en PATCH. El mismo cuerpo, dos respuestas.

---

## B. `api/vendedor` — CRUD con UNA clave foránea (6 endpoints)

```
GET    /api/vendedor[?limite=N]  → 200 {tabla:"vendedor", limite, total, datos:[…]}
GET    /api/vendedor/{id}        → 200 {"id":1,"carnet":1001,
                                        "direccion":"Calle 10 # 20-30","fkcodpersona":"P002"}
POST   /api/vendedor             body {carnet, direccion, fkcodpersona} — los tres obligatorios
PUT    /api/vendedor/{id}        body {carnet, direccion, fkcodpersona} — TODO obligatorio
PATCH  /api/vendedor/{id}        body parcial
DELETE /api/vendedor/{id}        → 200 · 404
```

**La lección de integridad referencial, y conviene provocarla a propósito:**

```
DELETE /api/persona/P002          <- P002 es vendedor (FK desde vendedor.fkcodpersona)
→ 409 {estado:409, mensaje:"…", detalle:"…violates foreign key constraint
       \"fk_vendedor_persona\"…"}
```

La base **protege sus relaciones**, y el mensaje del motor viaja completo en
`detalle`. Esto en la v1 era imposible de provocar.

---

## C. `api/factura` — maestro-detalle por procedimientos (4 endpoints)

**Cuatro, no seis**, y la ausencia es el contrato:

| | |
|---|---|
| **No hay PUT ni PATCH** | Una factura emitida es un documento. No se corrige: se anula y se hace otra |
| **No hay DELETE** | Hay `POST .../anular`. La fila **no desaparece**: cambia de estado y el stock vuelve |

### C1. `GET /api/factura` — listar, con el detalle anidado

```
→ 200 {tabla:"factura", total:N, datos:[
         {numero, fecha, total, estado, fkidcliente, nombre_cliente,
          fkidvendedor, nombre_vendedor,
          productos:[{codigo_producto, nombre_producto, cantidad,
                      valorunitario, subtotal}]} ]}
```

> **Este sobre no trae `limite`**, y es la única excepción: el procedimiento
> `sp_listar_facturas_y_productosporfactura` devuelve todas.
>
> **Y `nombre_cliente` llega resuelto.** El `JOIN` lo hizo el procedimiento: ni
> la API ni la interfaz gráfica hacen una segunda petición para buscar el
> nombre.

### C2. `GET /api/factura/{numero}` — una sola

```
→ 200 {numero, fecha, total, estado, …, productos:[…]}   <- SIN sobre
· 404 si no existe
```

> **Ojo con la asimetría:** el listado viene en sobre y esta no. Es deliberado
> —el procedimiento devuelve el objeto y el controlador lo emite tal cual— y
> está escrito aquí para que nadie lo descubra deserializando a ciegas.

### C3. `POST /api/factura` — crear el maestro y el detalle, UN envío

```
body { "fkidcliente": 1, "fkidvendedor": 1,
       "productos": [ {"codigo":"PR001","cantidad":2},
                      {"codigo":"PR003","cantidad":1} ] }

→ 200 la factura COMPLETA, con fecha, subtotales y total calculados por la base
· 422 si falta un campo o si `productos` llega vacío (mínimo 1)
· 409 si el cliente, el vendedor o un producto no existen
· 500 si no hay stock suficiente — el mensaje del trigger viaja en `detalle`
```

**Tres campos que NO se envían, y mandarlos es un error:**

| | Quién lo pone |
|---|---|
| `total` | El **trigger** `trg_actualizar_totales_y_stock` |
| `subtotal` de cada renglón | El mismo trigger |
| `fecha` | La base, al insertar |

> **Por qué importa:** si el cliente enviara el total habría **dos fuentes de
> verdad**, y el día que no coincidan gana la que nadie revisó. El renglón que
> viaja es `{codigo, cantidad}` y nada más — ni el precio, que lo toma la base
> del producto.

### C4. `POST /api/factura/{numero}/anular` — el borrado lógico

```
→ 200 {mensaje, numero, estado:"anulada", …}
· 404 si la factura no existe
· 409 si YA estaba anulada
```

**Por qué `POST` y no `DELETE`:** porque `DELETE` significa «que deje de
existir». Anular significa «que quede como anulada» — es una **acción del
negocio**, y además devuelve el stock de cada renglón.

**Por qué 409 y no 400 ni 404:** la petición está bien formada y la factura
existe. El conflicto es con **el estado actual del recurso**, y esa es la
semántica exacta de `409 Conflict`.

---

## D. `api/rol-usuario` — una tabla puente (5 endpoints)

```
GET    /api/rol-usuario[?limite=N]        → 200 {tabla:"rol_usuario", limite, total, datos:[…]}
                                            datos: [{"fkemail":"ana@correo.com","fkidrol":2}]
GET    /api/rol-usuario/usuario/{email}   → 200 los roles de ESE usuario
GET    /api/rol-usuario/rol/{idrol}       → 200 los usuarios de ESE rol
POST   /api/rol-usuario                   body {fkemail, fkidrol}
                                          → 200 · 409 si la pareja ya existe o una FK no
DELETE /api/rol-usuario/{email}/{idrol}   → 200 · 404
```

| | |
|---|---|
| **La ruta lleva GUION** | `api/rol-usuario`, aunque la tabla sea `rol_usuario` con subrayado. Son dos convenciones distintas —URL y SQL— y asumir una por la otra da **404** |
| **No hay PUT ni PATCH** | La fila no tiene campos: **existe o no existe** |
| **El DELETE lleva DOS valores** | Porque la clave primaria son las dos columnas juntas. Con una sola no se sabe cuál pareja quitar |
| **Hay DOS listados por lado** | `/usuario/{email}` y `/rol/{idrol}`. Una tabla puente se consulta por los dos extremos |

---

## E. `api/rutarol` — la otra puente (5 endpoints)

```
GET    /api/rutarol[?limite=N]          → 200 {tabla:"rutarol", limite, total, datos:[…]}
GET    /api/rutarol/ruta/{idruta}       → 200 los roles que entran a ESA interfaz
GET    /api/rutarol/rol/{idrol}         → 200 las interfaces de ESE rol
POST   /api/rutarol                     body {fkidruta, fkidrol}
DELETE /api/rutarol/{idruta}/{idrol}    → 200 · 404
```

> **Aquí la ruta NO lleva guion:** `api/rutarol`, porque la tabla se llama
> `rutarol` sin subrayado. No es un descuido: se mira el `[Route]` del
> controlador antes de escribirla.

> **Y lo que esta tabla hace y NO hace en la v2:** se **administra** —se listan
> las parejas, se agregan, se quitan—. **No se aplica**: nadie pregunta todavía
> si quien pide tiene el permiso. Eso es la **v3**, con
> `verificar_acceso_ruta`. Tener esta tabla llena no protege nada.

---

## F. `api/usuario-con-roles` — el recurso que no es una tabla (5 endpoints)

Opera `usuario` y `rol_usuario` **juntas**, con los cinco procedimientos que la
base ya trae.

```
GET    /api/usuario-con-roles           → 200 {tabla:"usuario_con_roles", total, datos:[
                                            {"email":"ana@correo.com",
                                             "roles":[{"idrol":2,"nombre":"vendedor"}]} ]}
GET    /api/usuario-con-roles/{email}   → 200 uno solo, con sus roles · 404
POST   /api/usuario-con-roles           body {email, contrasena, roles:[2,3]}
                                        → 200 el usuario con sus roles
                                        · 422 si falta algo o `roles` va vacío (mínimo 1)
                                        · 409 si el correo ya existe o un rol no
PUT    /api/usuario-con-roles/{email}   body {contrasena?, roles:[1]}
                                        → 200 · 404 · 409
DELETE /api/usuario-con-roles/{email}   → 200 {mensaje, email_eliminado} · 404
```

**Tres reglas que hay que leer dos veces:**

| | |
|---|---|
| **`roles` llega como `[2,3]`** | Enteros pelados. El JSON que el procedimiento espera —`[{"fkidrol":2}]`— lo arma **el servicio**, porque la forma de ese JSON es una regla del dominio, no del transporte |
| **La contraseña vacía al editar = «déjela como está»** | No significa borrarla. Sin esta regla, cambiarle un rol a alguien le borraría la clave |
| **Los roles se REEMPLAZAN, no se suman** | La lista que llega es la que queda. El procedimiento borra los que había y pone los nuevos, en la misma transacción |

> **Por qué este recurso existe además de `api/usuario` y `api/rol-usuario`:**
> porque crear el usuario y después asignarle los roles son **dos** operaciones,
> y si falla la segunda queda un usuario **sin ningún rol** —que no puede hacer
> nada y que nadie sabe que está ahí—. `crear_usuario_con_roles` lo hace en
> **una**.
>
> Los tres recursos conviven a propósito: `api/usuario` administra la tabla
> sola, `api/rol-usuario` la puente pareja a pareja, y este el conjunto. La
> interfaz gráfica usa el tercero.

---

## G. Diagnóstico — cambia UNA clave

```
GET /  → 200 {"mensaje":"API Facturas funcionando","version":"v2",
             "contratos":"docs/spec_kit/versiones/v2_con_fk/6_contracts.md"}
```

---

## H. La traducción de errores, acumulada

| Situación | Excepción interna | HTTP |
|---|---|---|
| El cuerpo no cumple las anotaciones de su petición | (la responde el framework) | **422** + `errores[]` |
| Regla de negocio (`limite <= 0`, PATCH vacío, cero roles) | `ArgumentException` | **400** |
| No existe | `NoEncontradoExcepcion` | **404** |
| Clave foránea inexistente, pareja repetida, correo duplicado | `ConflictoExcepcion` | **409** |
| Ya estaba anulada | `ConflictoExcepcion` | **409** |
| Sin stock, base caída, lo imprevisto | `SqlException` y demás | **500** + el mensaje del motor en `detalle` |

> **El 409 es el código nuevo de la v2**, y aparece por una razón de fondo: es
> la primera versión en la que una fila **depende de otra**.

---

## I. Estabilidad

Estos contratos **se congelan** al cerrar la v2 (commit + tag `v2`).

| | |
|---|---|
| **La v3** | Les agrega la exigencia del token: las mismas rutas, los mismos cuerpos, y **401** si no llega. No cambia una sola forma |
| **La v5** | Cambia el **motor** por configuración. Si estos endpoints respondieran distinto contra SQL Server que contra SQL Server, **esa versión está mal** — es su criterio de éxito |
