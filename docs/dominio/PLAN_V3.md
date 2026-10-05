# Plan de la versión 3 — Facturación (`bdfacturas`)

> **Qué es este documento.** Qué agrega la v3, qué se decidió antes de
> programarla, y **los cinco tropiezos que tiene preparados** — tres de los
> cuales dejan el sistema **abierto sin que nada falle**, que es lo que los
> hace caros.
>
> El requisito que manda está en
> [`2_spec.md`](../spec_kit/versiones/v3_control_acceso/2_spec.md). Esto es el
> relato.
>
> **Material académico simulado** en el dominio; las decisiones y los tropiezos
> son reales, y uno de ellos **solo se ve escribiendo la dirección a mano**.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Lo que queda al terminar la v3

| Queda | Comprobable con |
|---|---|
| La contraseña **no existe en claro** en ninguna fila | `SELECT contrasena FROM usuario` |
| Identificarse devuelve un **token** | `POST /api/sesion` |
| **Sin token, nada**: 401 en las 70 operaciones | `GET /api/producto` sin cabecera |
| **Con token y sin permiso**: 403, que es otra cosa | Un rol sin `interfaz.usuarios` pidiendo `/api/usuario` |
| El permiso lo resuelve **la base de datos**, no C# | `verificar_acceso_ruta` |
| Quitarle un permiso a un rol **surte efecto ya** | Sin volver a identificarse |
| El menú **cambia según el rol** | Entrar como `cliente1@correo.com` |
| Escribir la dirección a mano **tampoco entra** | `/usuarios` en la barra, sin permiso |
| **DOS operaciones nuevas** — y las 68 anteriores intactas | Swagger: 70 en total |

> **Dos operaciones y es la versión que más cambia el sistema.** No agrega una
> tabla ni una pantalla de datos: le pone **la puerta** a todo lo que ya
> existía. Por eso el número de endpoints engaña — lo que cambió es que ahora
> **todos** preguntan quién llama.

---

## 1. De dónde se partió

De una v2 que funcionaba **y estaba abierta de par en par**. Cualquiera que
llegara al puerto 8035 podía emitir una factura, borrar un cliente o
repartirse permisos.

Y de una incomodidad que la v2 dejó escrita en su propio plan:

> **Los permisos ya se podían administrar desde la v2** — `rutarol` tenía su
> CRUD, porque es una tabla con claves foráneas. Lo que **no** existía era que
> sirvieran de algo.

**Administrar permisos y hacerlos valer son dos cosas distintas.** La v2 hizo
la primera. La v3 es la segunda, y esa frase es todo el alcance de la versión.

---

## 2. La decisión que define la versión: **el permiso NO va en el token**

Es la decisión que más se discute y la que más consecuencias tiene. El token
podría llevar los permisos adentro: se calculan una vez al identificarse, se
firman, y cada petición ya trae la respuesta puesta. Sería **más rápido**.

Se decidió lo contrario: **el permiso se consulta en cada petición**.

| | Permiso EN el token | Permiso consultado **cada vez** |
|---|---|---|
| **Costo** | Cero consultas por operación | **Una** consulta por operación |
| **Quitarle un permiso a un rol** | No surte efecto **hasta que el token venza** | Surte efecto **en la siguiente petición** |
| **Quién manda** | Lo que se firmó hace una hora | Lo que dice la base de datos **ahora** |

> **Y de ahí sale el criterio 7 de la versión**, que está escrito justamente
> para forzar esta decisión: *quitarle un permiso a un rol surte efecto sin
> volver a identificarse*. Un equipo que mete los permisos en el token no puede
> pasar ese criterio — y no porque se le haya olvidado, sino porque **eligió la
> otra arquitectura**. Por eso el criterio existe: hace visible la elección.

**El token lleva solo quién es**, y su duración está en la configuración —
sesenta minutos si nadie la cambia ([`ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs)).
Lo que puede hacer esa persona se pregunta cada vez.

---

## 3. La segunda decisión: **el permiso lo resuelve la BASE DE DATOS**

«La base de datos», en este documento y en los demás, es **la base de datos** — el
motor, no una capa del código. Y la frase hay que tomarla literal: el permiso
no es un `JOIN` escrito en C#. Es **`verificar_acceso_ruta`**, un
**procedimiento almacenado** que ya estaba en el esquema desde el primer día.

**La cadena completa, archivo por archivo:**

| # | Quién | Qué hace |
|---|---|---|
| 1 | [`ExigePermisoAttribute.cs`](../../api_facturas/Autorizacion/ExigePermisoAttribute.cs) | Saca el correo del token y **pregunta** |
| 2 | [`IRepositorioAcceso`](../../api_facturas/Repositorios/IRepositorioAcceso.cs) | `Task<bool> TieneAccesoAsync(email, ruta)` — una interfaz, sin SQL |
| 3 | `RepositorioAccesoSqlServer` / `…Postgres` | Traduce el nombre de la ruta a su `id` y **ejecuta el procedimiento** |
| 4 | **`verificar_acceso_ruta`**, en el motor | **Aquí está el `JOIN`** de tres tablas, y aquí se decide |

**El `JOIN` que decide, tal como está en el motor:**

```sql
IF EXISTS (
    SELECT 1
    FROM usuario u
    INNER JOIN rol_usuario ur ON u.email = ur.fkemail
    INNER JOIN rutarol rr    ON ur.fkidrol = rr.fkidrol
    WHERE u.email = @p_email AND rr.fkidruta = @p_fkidruta
)
    SET @v_tiene_acceso = 1;
```

> **Y se puede comprobar sin la API encendida**, que es la prueba de que la
> regla no vive en C#. Contra el motor, directo:
>
> ```sql
> CALL verificar_acceso_ruta('admin@correo.com',    2, NULL);
> -- {"tiene_acceso" : true,  "email" : "admin@correo.com",    "fkidruta" : 2}
> CALL verificar_acceso_ruta('cliente1@correo.com', 2, NULL);
> -- {"tiene_acceso" : false, "email" : "cliente1@correo.com", "fkidruta" : 2}
> ```
>
> La ruta 2 es `interfaz.usuarios`. **Nadie levantó la API para obtener esas
> dos respuestas.**

| | |
|---|---|
| **Dónde está la regla** | En la base de datos: `usuario → rol_usuario → rutarol` |
| **Qué hace la API** | Pregunta y **obedece**. No reconstruye el cruce |
| **Por qué ahí** | La regla vale **también** para quien entre por SSMS, por `psql` o por otro cliente — no solo para quien pase por la API |

> **Una cosa que el repositorio sí decide, y conviene verla:** si el nombre de
> la ruta **no está declarado** en la tabla `ruta`, responde `false` sin
> preguntarle a nadie. **Falla cerrado, no abierto** — una ruta que nadie
> declaró no es una ruta que todos pueden usar.

> **Es el mismo argumento de la v2 con el stock**, y conviene que se repita:
> una regla que solo existe en C# protege **una** puerta. La que está en la
> base protege **todas**. Y en este sistema hay otra API que ya se asoma — la
> del motor alterno —, así que «todas» no es una figura retórica.

### Y una tercera decisión, más pequeña y más fácil de arruinar: **es un filtro**

El permiso se exige con un atributo, no con una línea al principio de cada
método:

```csharp
[ExigePermiso("interfaz.usuarios")]
```

> **Si fuera una línea**, el día que alguien escriba un endpoint nuevo y se le
> olvide ponerla, ese endpoint **queda abierto — y nadie lo nota, porque
> funciona**. El filtro no se olvida: está en el atributo, a la vista, y se lee
> en la misma línea donde se lee el nombre del controlador.
>
> Está en
> [`ExigePermisoAttribute.cs`](../../api_facturas/Autorizacion/ExigePermisoAttribute.cs),
> y hoy lo usan **los catorce controladores**.

---

## 4. El orden en que se construyó

**El hash primero, y no es un capricho de orden.**

| # | Paso | Por qué va aquí |
|---|---|---|
| **1** | La contraseña con hash (BCrypt), en el **repositorio** | Sin esto, lo demás es decoración: da igual qué tan bueno sea el token si la contraseña está en claro al lado |
| **2** | `POST /api/sesion` → el token | Es lo que permite que la **segunda** petición sepa quién pregunta |
| **3** | `[Authorize]` en todo → el **401** | Primero cerrar, después afinar. Un sistema a medio cerrar es un sistema abierto |
| **4** | `[ExigePermiso]` → el **403** | Ya hay identidad; ahora la pregunta es qué puede hacer |
| **5** | `GET /api/permisos/mios` | Para que el front pueda **armar el menú** sin adivinar |
| **6** | El menú por rol, en el front | Lo último, y lo que **menos** protege (§5) |

> **El hash vive en el repositorio, no en el servicio.** Es un detalle de cómo
> se guarda, no una regla de negocio: `ServicioUsuario` **no sabe que BCrypt
> existe**. Si mañana se cambia de algoritmo, se cambia en dos archivos —uno
> por motor— y nada más se entera.

---

## 5. Los cinco tropiezos que esta versión tiene preparados

| | Qué pasa | Cómo se nota |
|---|---|---|
| **1 · El menú escondido como «protección»** | Se oculta el enlace y se da por cerrado. **Escribir `/usuarios` en la barra llega igual** | Es el **criterio 9**, y se comprueba a mano. El menú es comodidad; quien rechaza es la API |
| **2 · El permiso en el token** | Funciona, es más rápido, y **rompe el criterio 7** | Se le quita el permiso a un rol y la persona sigue entrando hasta que el token vence |
| **3 · El 403 disfrazado de 401** | Se responde «no autorizado» a todo | El cliente no puede distinguir «vuelva a identificarse» de «usted no puede». Son dos arreglos distintos |
| **4 · El mensaje que delata** | «Ese correo no existe» vs «contraseña incorrecta» | Dos mensajes distintos **confirman qué correos existen**. El criterio 2 exige **el mismo** en los dos casos |
| **5 · El endpoint nuevo sin atributo** | Se agrega un controlador y se olvida `[ExigePermiso]` | **No falla nada**: responde 200 a quien no debía. De ahí el §3 — filtro, no línea |

> **Tres de los cinco no rompen nada.** El 1, el 2 y el 5 dejan un sistema que
> funciona, que pasa las pruebas de la v1 y de la v2, y que está abierto. Por
> eso esta versión se verifica **intentando entrar**, no comprobando que lo
> permitido funcione.

---

## 6. Cómo se verificó

**Lo permitido funciona** no es la prueba de esta versión. La prueba es que
**lo prohibido no pasa**.

| Intento | Respuesta esperada |
|---|---|
| `GET /api/producto` **sin** cabecera | **401** |
| Token con un carácter cambiado | **401** — la firma no cuadra |
| `admin@correo.com` / `admin123` → token | **200**, y el token empieza por `eyJ` |
| Con ese token, las doce tablas | **200** |
| Como `cliente1@correo.com`, pedir `/api/usuario` | **403**, con el nombre de la ruta que faltó |
| Quitarle `interfaz.usuarios` al rol y repetir **sin volver a entrar** | **403** de inmediato |
| Escribir `/usuarios` en la barra sin permiso | La pantalla se abre y **la API responde 403** |

> **El último merece el párrafo.** La pantalla se abre: eso está bien. El front
> no es la autoridad y no tiene que fingir que lo es. Lo que no pasa es el
> dato — y el aviso que la persona ve sale de un 403 que vino de la base de datos.

---

## 7. Lo que queda para la v4

La v3 deja el sistema **con puerta y sin ojos**. Las doce tablas se operan, se
sabe quién entra y qué puede hacer, y aun así el sistema **no responde ninguna
pregunta del negocio**: «¿cuánto vendió cada vendedor?» no es una fila de
ninguna tabla — es un cruce.

> **Y hay algo que la v3 habilita sin que se note:** ahora que el permiso se
> resuelve por ruta, el tablero de la v4 **también** queda detrás del mismo
> filtro. Las diez consultas no son un anexo abierto al público: exigen token y
> permiso como todo lo demás.

Sigue en [`PLAN_V4.md`](PLAN_V4.md).

---

| Qué | Dónde |
|---|---|
| El requisito que manda | [`v3_control_acceso/2_spec.md`](../spec_kit/versiones/v3_control_acceso/2_spec.md) |
| Los contratos de las dos operaciones | [`6_contracts.md`](../spec_kit/versiones/v3_control_acceso/6_contracts.md) |
| Por qué 401 y 403 son distintos | [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) |
| El concepto, explicado aparte | [`CONCEPTOS_CONTROL_DE_ACCESO.md`](../conceptos/CONCEPTOS_CONTROL_DE_ACCESO.md) |
| La versión anterior | [`PLAN_V2.md`](PLAN_V2.md) |
