# Plan de la versión 2 — Facturación (`bdfacturas`)

> **Qué es este documento.** Qué agrega la v2, qué se decidió antes de
> programarla, y **los cinco tropiezos que tiene preparados** — cuatro de los
> cuales fallan **en silencio**, que es lo que los hace caros.
>
> El requisito que manda está en
> [`2_spec.md`](../spec_kit/versiones/v2_con_fk/2_spec.md). Esto es el relato.
>
> **Material académico simulado** en el dominio; los tropiezos son reales, y uno
> de ellos **apareció midiendo**, no leyendo.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Lo que queda al terminar la v2

| Queda | Comprobable con |
|---|---|
| `cliente` y `vendedor` con sus claves foráneas | `/api/cliente` · `/api/vendedor` |
| `factura` **por procedimientos**, en una transacción | `POST /api/factura` con tres renglones |
| Anular una factura, con **devolución de stock** | `POST /api/factura/{n}/anular` |
| Las dos tablas puente, con llave compuesta | `/api/rol-usuario` · `/api/rutarol` |
| `usuario-con-roles`: el usuario **y** sus roles a la vez | `/api/usuario-con-roles` |
| **Un código HTTP nuevo: el 409** | `POST /api/cliente` con `fkcodpersona: "P999"` |
| Los **desplegables** y el **formulario maestro-detalle** | Pantalla «Nueva factura» |
| **31 operaciones nuevas** — y las 37 de la v1 intactas | Swagger: 68 en total |

---

## 1. De dónde se partió

De una v1 que funciona: seis recursos, tres capas, un comando. **Y de un
esquema que ya tenía las doce tablas** — las seis de la v2 existían en la base
desde el primer día; lo que no existía era el código que las tocara.

> **Eso cambia la naturaleza del trabajo.** La v2 no «crea» las tablas con clave
> foránea: **las descubre**. Los disparadores, los procedimientos y las
> restricciones ya estaban puestos, esperando. Y la primera vez que la API
> intenta insertar un renglón, el disparador que nadie escribió esta semana
> responde — y hay que entender qué dijo.

---

## 2. La decisión que define la versión: **¿422 o 409?**

Es la pregunta que abre la v2, y la respuesta vale más que el código.

Alguien manda `{"fkcodpersona": "NOEXISTE"}`. ¿Qué responde la API?

| Opción | El argumento |
|---|---|
| **422** | «El dato está mal, y el 422 es para datos malos» |
| **409** ✅ | **El dato NO está mal: tiene la forma correcta.** `"NOEXISTE"` es un texto de la longitud permitida. Lo que se rompe es el **estado** de la base: esa fila no está |

> **Lo que decide:** el 422 se reserva para lo que la **petición** puede rechazar
> **sin consultar nada** — un campo que falta, un número negativo, un tipo
> equivocado. Saber si `P001` existe **exige ir a la base**, y eso ya es estado.
>
> **Y de ahí sale por qué el 409 aparece en la v2 y no antes:** es la primera
> versión en la que una fila **depende de otra**. En la v1 no había nada que
> violar.

### La consecuencia en la v1, que es una deuda que se paga aquí

La v1 devolvía **500** al insertar un código repetido, **y estaba declarado en su
spec**. No era un descuido: era una deuda con fecha.

> **Por qué se dejó mal a propósito.** Un 500 es el motor gritando sin
> traducción, y verlo una vez enseña más que leer que «hay que capturar la
> excepción». En la v2 ese mismo caso responde **409**, y la diferencia entre las
> dos respuestas es la lección: **quién traduce el error del motor, y en qué
> capa**. Ver [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md).

---

## 3. La segunda decisión: **la factura NO escribe SQL de tablas**

| Opción | Por qué no / sí |
|---|---|
| SQL en el repositorio, como los otros recursos | Insertar un encabezado y tres renglones son **cuatro escrituras**, y desde C# son cuatro viajes y cuatro oportunidades de quedar a medias |
| **Procedimientos** ✅ | La transacción vive **adentro** del procedimiento: una sola llamada, o todo o nada |

> **Y por eso `factura` es el único recurso que rompe el molde de la v1.** Los
> otros ocho tienen `SELECT`, `INSERT`, `UPDATE` y `DELETE` escritos en su
> repositorio. `RepositorioFactura` **no escribe SQL de tablas**: llama
> procedimientos. No es inconsistencia — es que la factura tiene un requisito que
> los otros no tienen. Ver
> [`PRINCIPIOS_ACID.md`](../conceptos/PRINCIPIOS_ACID.md).

### Y el total **no lo manda el front**

Tentador: el formulario ya tiene los renglones, ya sabe sumar.

> **Se descartó, y la razón es de seguridad, no de elegancia.** Si el total
> llegara en el cuerpo de la petición, cualquiera podría mandar una factura de
> tres millones con total `1000`. El total lo calcula un **disparador**, en la
> base, y la API ni lo acepta si se lo mandan.
>
> **La regla general:** un dato que el servidor puede calcular, el servidor lo
> calcula. Lo que llega del cliente es lo que el cliente **sabe** — qué productos
> y cuántos —, no lo que se **deduce** de eso.

---

## 4. El orden en que se construyó

| # | Paso | Por qué en ese momento |
|---|---|---|
| **1** | `cliente` y `vendedor`, calcando el molde de la v1 | Son lo más parecido a lo conocido: una clave foránea y nada más |
| **2** | La traducción del error del motor al **409** | Hasta que esto no esté, los pasos 1 y 3 no se pueden probar bien |
| **3** | `factura`: listar, consultar, crear, anular | Es lo nuevo de verdad: procedimientos y transacción |
| **4** | Las dos tablas puente | Llave compuesta, y borrado **por sus dos claves** |
| **5** | `usuario-con-roles` | No es una tabla: es otra forma de operar dos |
| **6** | Los **desplegables** en las pantallas | Necesitan que los recursos de arriba respondan |
| **7** | El formulario **maestro-detalle** de factura | Es lo último porque es lo que más depende |
| **8** | **La regresión de la v1** | Antes de firmar: las 37 operaciones de la v1, otra vez |

> **El paso 8 no es burocracia.** Este proyecto es **acumulativo**: la v2 incluye
> la v1. Una versión que rompe la anterior no está terminada, está cambiada — y
> la única forma de saberlo es volver a probar lo viejo.

---

## 5. Los cinco tropiezos que esta versión tiene preparados

| | Qué pasa | Cómo se ve |
|---|---|---|
| **1** | **El front deserializa el sobre a `List<T>`** | La pantalla sale **vacía, sin un solo error**. La API devuelve `{tabla, limite, total, datos[]}`: hay que entrar a `datos` |
| **2** | **Dapper mapea por NOMBRE de columna** | Una propiedad que no se llama igual que su columna llega `null` **en silencio**, con HTTP 200 y la celda en blanco. Se arregla con un alias: `SELECT ruta AS RutaTexto` |
| **3** | **Los nombres que devuelven los procedimientos** | Vienen `nombre_cliente` e `idrol`; las propiedades son `NombreCliente` e `IdRol`. Sin `[JsonPropertyName]` llegan `null` y **0** |
| **4** | **La cadena vacía de un desplegable opcional** | `""` **no es** `null`. Aquí lo atrapa la anotación y responde **422**; sin esa anotación llegaría a la base y sería un **409** |
| **5** | **Los desplegables cargados EN FILA** | Con la API apagada, cada petición espera sus 10 segundos de *timeout*. `/facturas` pide **cinco** listas: en fila son **50 segundos** en blanco antes del aviso. Con `Task.WhenAll`, **10** |

> **Los cuatro primeros fallan EN SILENCIO, y eso es lo que los hace caros.** No
> hay excepción, no hay log, no hay 500: hay **un dato equivocado** y un HTTP
> 200. Por eso el cierre de esta versión se hace **mirando la interfaz**, no solo
> leyendo respuestas de la API. Una API que responde 200 con un `null` adentro
> pasa cualquier prueba que solo mire el código de estado.

> **Y el quinto no se encuentra leyendo: se encuentra MIDIENDO.** Apareció
> cronometrando el criterio «apague la API y la interfaz sigue en pie». La
> interfaz *sí* quedaba en pie y *sí* mostraba su aviso: lo hacía **casi un
> minuto después**, que para quien la usa es lo mismo que estar roto.
>
> **Un criterio que se comprueba «a ojo» lo habría dado por bueno.** De ahí la
> regla que vale para todo el curso: un criterio de aceptación sin un número no
> es comprobable, es opinable.

### El tropiezo 4, con nombre y apellido

`fkcodempresa` es opcional: hay clientes sin empresa. En la pantalla es un
desplegable con una opción vacía. Y un desplegable vacío en HTML manda `""`.

| Lo que el front manda | Qué responde la API **medido** |
|---|---|
| `""` | **422** · *«El campo fkcodempresa debe tener entre 1 y 10 caracteres»* |
| `null` | **200** · *«Cliente creado exitosamente»* |

> **Son dos cosas distintas y se escriben casi igual.** La corrección es una
> línea en el front: convertir `""` a `null` antes de mandar.

> **Y aquí hay una lección que solo aparece MIDIENDO.** El `3_plan.md` de esta
> versión decía que la cadena vacía daría **409**, y la API devuelve **422**. Las
> dos respuestas son defendibles, y la que da es la mejor: `[StringLength(10,
> MinimumLength = 1)]` atrapa el `""` **en la petición**, antes de que la base se
> entere.
>
> **O sea que el documento predijo un camino y el código tomó uno más corto.** Lo
> que se corrige es el documento, no el código — pero solo se sabe cuál corregir
> **después de mandar la petición**. Un spec kit que nadie contrasta contra la API
> corriendo acumula predicciones que ya no son ciertas.

---

## 6. Cómo se verificó

| Qué | Cómo |
|---|---|
| El 409 de clave foránea | `POST /api/cliente` con `fkcodpersona: "P999"` |
| El 409 de pareja repetida | `POST /api/rol-usuario` dos veces con lo mismo |
| El 409 de factura ya anulada | Anular dos veces la misma |
| La transacción | Una factura con un producto **sin stock** entre los tres: no debe quedar **ninguno** |
| La devolución de stock | Anotar el stock, emitir, anular, comparar: igual |
| El desplegable opcional | Crear un cliente **sin** empresa |
| La regresión de la v1 | Las 37 operaciones, otra vez |

> **La prueba de la transacción es la única que importa de verdad**, y es la que
> más se olvida: pedir tres productos donde el segundo no alcanza. Si al final
> quedó una factura con un renglón, la transacción no existe aunque el
> procedimiento diga `BEGIN TRANSACTION`.

---

## 7. Lo que queda para la v3

La v2 deja algo incómodo, y hay que decirlo: **todo esto está abierto**. Cualquiera
que llegue al puerto 8035 puede emitir una factura, borrar un cliente o repartirse
permisos.

> **Y los permisos ya se pueden administrar desde la v2** — `rutarol` tiene su
> CRUD, porque es una tabla con claves foráneas. Lo que **no** existe todavía es
> que sirvan de algo.
>
> **Administrar permisos y hacerlos valer son dos cosas distintas**, y la v2
> hace la primera. La segunda es la v3: el token, el 401, el 403 y un menú que
> solo muestra lo que ese rol puede abrir.
