# Mapa de versiones — Aplicación y Servicios Web

> La ruta completa del proyecto. Cada versión se especifica **solo cuando la
> anterior está cerrada** (commit + tag). Este mapa da la dirección; el spec
> kit de cada versión da el detalle.
>
> **Y cada versión entrega su API Y SU INTERFAZ GRÁFICA.** No hay una versión
> «de back» y otra «de front»: se construyen en paralelo, y una versión no está
> cerrada si la API responde y la interfaz gráfica no.
>
> La ruta es la que define
> [0_METODOLOGIA.md](../../../ProyectosDeAula/docs/0_METODOLOGIA.md) §2; aquí
> no se inventa nada, se ordena.

## La ruta — **cuatro versiones**

> **Cada versión INCLUYE la anterior.** La v2 incluye la v1, la v3 incluye la
> v2 y la v4 incluye la v3. No se reinicia nada y no se empieza de cero: lo
> construido sigue en pie, con su código, su interfaz gráfica y sus criterios de
> aceptación —y esos criterios se vuelven a correr, que es lo que se llama **la
> regresión**.
>
> De ahí que el repositorio de la v3 tenga las doce tablas operables: no porque
> la v3 las agregue, sino porque **trae la v1 y la v2 adentro**.

| Versión | Qué agrega (acumulativo) | Estado |
|---|---|---|
| v1 | CRUD completo de **las seis tablas sin clave foránea** — **API e interfaces gráficas** | **Cerrada** · tag `v1` |
| v2 | CRUD de **TODAS las tablas** — con la v2 están las 12: las FK como **listas desplegables cargadas desde la API**, las puente, y la facturación **maestro-detalle** por procedimientos — **API e interfaces gráficas** | **Cerrada** · tag `v2` |
| v3 | **El control de acceso**: la contraseña con hash, la sesión con token, y el permiso resuelto por `verificar_acceso_ruta`. **No agrega tablas**: le pone la puerta a lo que ya existe | **Cerrada** · tag `v3` |
| **v4** | **10 consultas multitabla** (4+ tablas cada una), tablero con gráficos, **imagen corporativa con su manual de marca**, páginas corporativas, responsive/PWA y **publicación** en un servidor | **En curso** ([spec](v4_aplicativo/2_spec.md)) |

> **Las cuatro del curso son estas.** Si aparece una quinta *dentro* de ellas,
> es que algo se dejó a medias y se está aplazando. La v5 que sí existe está
> **después**, y es de otra naturaleza — ver abajo.

### Y una v5, que está FUERA de las cuatro del curso

| Versión | Qué agrega | Estado |
|---|---|---|
| **v5** | **Otros motores de base de datos**: una segunda y una tercera implementación del repositorio —PostgreSQL, MariaDB— y la **fábrica** que elige cuál usar por configuración | Futura |

**Por qué está fuera de las cuatro, y no es un desprecio:**

| | |
|---|---|
| **El curso son cuatro** | `0_METODOLOGIA.md` §2 fija cuatro, y el calendario del semestre está armado sobre esas cuatro |
| **No agrega funcionalidad al producto** | Cambiar de motor agrega **una implementación de la misma interfaz**. Quien usa el sistema no nota nada |
| **Y aun así vale la pena** | Es **la prueba** de que la interfaz del repositorio servía: se agrega un motor **sin tocar el servicio ni el controlador**. La inversión de dependencias, comprobada en vez de prometida |

> **Es la única versión cuyo criterio de éxito es que NO haya que cambiar
> nada.** En las otras cuatro, terminar significa que algo nuevo funciona; en
> la v5, terminar significa que lo viejo **siguió** funcionando con otro motor
> debajo.

## La v5 ya tiene su adelanto en este repositorio

Este repositorio trae una implementación completa de los repositorios contra
**PostgreSQL**, con su servicio en el `docker-compose.yml` y el interruptor
`MOTOR_BD`.

**No es la v4** —la v4 es el aplicativo completo— sino **el adelanto de la
v5**, y está aquí porque ya estaba construido.

| | |
|---|---|
| **Qué demuestra** | Que la interfaz del repositorio servía: se agregó un segundo motor **sin tocar el servicio ni el controlador** |
| **Qué YA está construido** | Los repositorios de **los dos motores**, la **fábrica** (`Fabricas/IFabricaRepositorios.cs` y sus dos implementaciones) y el interruptor `MOTOR_BD`, que elige cuál atiende **sin recompilar** |
| **Qué falta para cerrar la v5** | El **tercer motor (MariaDB)**. Con dos, la fábrica ya evita el `if` repartido por el código; con tres se ve por qué hacía falta |

> **Se conservó a propósito.** Era código que funcionaba y que enseña algo
> real; tirarlo por un cambio de mapa habría sido peor que reubicarlo.


## El reparto de las 12 tablas

Las 12 tablas de `bdfacturas`, repartidas:

| Versión | Tablas | Criterio |
|---|---|---|
| **v1** | `producto` · `empresa` · `persona` · `rol` · `ruta` · `usuario` | **Las SEIS sin clave foránea.** Se pueden llenar sin que exista nada más |
| **v2** | `cliente` · `vendedor` · `factura` · `productosporfactura` · `rol_usuario` · `rutarol` · `usuario_con_roles` | **Las SEIS con clave foránea**, incluidas las puente. Con la v2, las **12** están |
| **v3** | `sesion` · `permisos` — **y ninguna de las dos es una tabla** | **No agrega tablas.** El CRUD de `usuario`, `rol` y `ruta` es de la v1; el de `rol_usuario` y `rutarol`, de la v2. La v3 agrega **la puerta** |
| **v4** | `consultas` — **y no es una tabla**, igual que las dos de la v3 | **No agrega tablas.** Agrega `/api/consultas`, que CRUZA las doce: diez preguntas que ninguna tabla responde sola |

> **`usuario_con_roles` no es una tabla**, y por eso aparece en la lista con una
> advertencia: es un **recurso** —`api/usuario-con-roles`— que opera `usuario` y
> `rol_usuario` **juntas**, a través de los cinco procedimientos almacenados que
> la base ya trae. Está en el reparto porque tiene controlador, servicio,
> repositorio e interfaz gráfica propios, y lo que no se reparte no se audita.

> **`sesion` y `permisos` tampoco son tablas.** Son los dos recursos que la v3
> agrega: `api/sesion` recibe las credenciales y devuelve el token —**la
> puerta**—, y `api/permisos` responde «¿a qué puedo entrar yo?» para que el
> menú se arme. Este último **no decide nada**: la decisión la toma
> `verificar_acceso_ruta` en cada operación.

> **Ojo:** las 12 tablas **existen en la base desde la v1**. Lo que reparte esta
> tabla es qué puede **nombrar el código** de cada versión, no qué existe en el
> motor.
>
> **`usuario` y `rol` SÍ entran en la v1**, aunque sean del control de acceso:
> el criterio de la v1 es **no tener clave foránea**, y no la tienen. Lo que
> llega en la v3 **no es su CRUD** —ese ya está— sino **el token, la sesión y
> que solo quien tenga el permiso pueda usarlo**.

## Lo que este mapa dejó por fuera, y por qué

Una versión anterior de este mapa repartía el trabajo **por motor y por
entidad**: la v1 era `producto` contra SQL Server, la v2 `persona` y `factura`,
la v3 «el resto de las entidades», la v4 **PostgreSQL**, la v5 MariaDB — y el
frontend quedaba en una **v6, «sin especificar»**.

**Se cambió, y conviene saber qué se perdió y qué se ganó:**

| | |
|---|---|
| **Qué se perdió** | Nada del contenido: las mismas 12 tablas, los mismos procedimientos, los mismos motores. Solo cambió **cómo se reparte** |
| **Qué se ganó: el criterio es una propiedad del MODELO** | «Sin clave foránea» y «con clave foránea» se leen en el `CREATE TABLE`. «El resto de las entidades» no se lee en ninguna parte: hay que acordarse |
| **Qué se ganó: cada versión se le puede mostrar a alguien** | Una versión que solo trae endpoints se sustenta con Swagger. Una que trae interfaces gráficas se le muestra a quien la pidió |
| **Qué se ganó: el contrato se ejercita de inmediato** | Uno descubre que el JSON es incómodo **cuando le toca pintarlo**. Con el front tres versiones después, el contrato lleva tres versiones equivocado |
| **Qué se perdió del mapa viejo, y hay que decirlo** | El segundo motor llegaba antes. Ahora llega después — y llega **mejor**, porque para entonces la interfaz del repositorio ya está ejercitada por doce recursos y no por tres |

> **Y el error de fondo del mapa viejo:** aplazar el front a una «v6 sin
> especificar» es la forma educada de no hacerlo. El front de golpe al final es
> el error que se paga caro: doce entidades de API esperando una interfaz que
> nace con una sola.
>
> `0_METODOLOGIA.md` §2 lo dice textual desde la v1: *«API REST **+ Frontend
> funcionando**»*.

## La estrategia: back y front EN PARALELO

**Cada versión entrega su parte de la API *y* su parte del front.**

| | |
|---|---|
| **Lo terminado se le puede mostrar a alguien** | Swagger demuestra que la API responde; la interfaz gráfica demuestra que el sistema sirve |
| **El contrato se ejercita de inmediato** | Pintar un JSON es la mejor forma de descubrir que está mal pensado |
| **No hay front de golpe al final** | Que es el error que el mapa viejo garantizaba |
| **Es lo que pide el curso** | `0_METODOLOGIA.md` §2, textual: *«v1 — CRUD de las tablas sin FK del módulo — **API REST + Frontend funcionando**»* |

> **La regla operativa:** una versión **no está cerrada** si la API responde y
> la interfaz gráfica no.
