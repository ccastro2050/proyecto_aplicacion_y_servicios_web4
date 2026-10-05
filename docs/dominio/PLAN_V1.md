# Plan de la versión 1 — Facturación (`bdfacturas`)

> **Qué es este documento.** Cómo se construyó la v1: con qué insumos, qué se
> decidió **antes** de escribir código, en qué orden, y **con qué se tropezó**.
> Es el relato; el requisito que manda está en
> [`2_spec.md`](../spec_kit/versiones/v1_sin_fk/2_spec.md).
>
> **Para qué sirve leerlo.** Para ver que un plan no es una lista de tareas: es
> una **cadena de decisiones**, y cada una cierra puertas. Las decisiones están
> en §3 con su razón, no solo con su resultado.
>
> **Material académico simulado** en el dominio; el plan y los tropiezos son
> reales y están en el historial.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Lo que queda al terminar la v1

| Queda | Comprobable con |
|---|---|
| **Seis recursos** con los cinco verbos — 37 operaciones | `GET /swagger` |
| **Seis pantallas**, una por recurso, con dirección propia | `http://localhost:8099/productos` |
| Tres capas con interfaces, desde el primer día | `Controllers/` · `Servicios/` · `Repositorios/` |
| SQL escrito a mano y parametrizado | cualquier repositorio |
| **Un solo comando** que levanta todo | `docker compose up -d --build` |
| Swagger funcionando | `http://localhost:8035/swagger` |

> **Media versión no es una versión.** La v1 entrega **API y pantallas**. Si solo
> hay API, la versión no está terminada — está la mitad, y el curso lo dice en el
> spec.

---

## 1. Los insumos

| Insumo | Qué aportó |
|---|---|
| **La constitución** | El stack, las capas, el idioma, el «un solo comando» |
| **El esquema `bdfacturas.sql`** | Las doce tablas **ya hechas**: 1 540 líneas con disparadores y procedimientos |
| **El mapa de versiones** | Qué va en la v1 y qué no |

> **Y aquí está la particularidad del proyecto, que conviene tener presente desde
> el principio:** el modelo de datos **no se diseñó**, llegó dado (Artículo 5).
> Lo que crece por versiones es la API. Ver [`FUENTES.md`](FUENTES.md) §0.

---

## 2. El primer hallazgo: **la v1 no es «una tabla»**

El criterio obvio para partir un sistema por versiones es por **tamaño**:
primero una tabla, luego dos, luego el resto. Ese criterio se descartó, y la
razón es la que da forma a todo el curso:

> **La segunda tabla no enseña nada que no enseñe la primera.** Si la v1 es
> `producto` y la v2 es `producto` + `persona`, la v2 es trabajo sin lección: los
> mismos cinco verbos, otros campos.

El corte que quedó es **por concepto**:

| Versión | El concepto que trae |
|---|---|
| **v1** | los cinco verbos, las capas, y **ninguna clave foránea** |
| **v2** | **la clave foránea**, con todo lo que arrastra: el 409, el desplegable, la transacción |
| v3 | **la puerta**: quién entra y a qué |
| v4 | **el aplicativo**: lo que un CRUD no puede responder |
| v5 | **el otro motor**: la prueba de que las capas servían |

Y de ahí sale la definición de la v1: **las tablas que no tienen clave foránea**.
En `bdfacturas` son seis — `producto`, `empresa`, `persona`, `rol`, `ruta` y
`usuario` — y son las primeras porque **se pueden llenar sin que exista nada
más**.

> **Que los seis recursos sean idénticos salvo los campos ES el punto, no un
> descuido.** Con uno solo nunca aparece la pregunta que la versión quiere
> provocar: *«¿y si hago un `Repositorio<T>` genérico y me ahorro cinco?»*. Hay
> que **sentir** la repetición para que la respuesta —el Artículo 10— signifique
> algo. Un estudiante al que le dicen «no uses un genérico» sin haber copiado y
> pegado seis veces, obedece sin entender.

---

## 3. Las decisiones antes de programar

Las ocho están razonadas en
[`4_research.md`](../spec_kit/versiones/v1_sin_fk/4_research.md). Aquí van las
cuatro que más cerraron puertas.

### 3.1 · El SQL a la vista: **Dapper, no Entity Framework**

| Opción | Por qué no / sí |
|---|---|
| Entity Framework | Genera el SQL. **Y el curso es de bases de datos**: si el SQL lo escribe el ORM, el estudiante no lo aprende a leer |
| ADO.NET puro | Enseña el SQL pero cuesta veinte líneas por consulta de plomería que no enseña nada |
| **Dapper** ✅ | **Mapea fila→objeto y JAMÁS genera SQL.** Si una consulta existe, está escrita en un repositorio |

> **La consecuencia que importa:** cada `SELECT` del sistema se puede leer. Y se
> puede copiar a SSMS y correrlo. Con un ORM habría que preguntarle al ORM qué
> escribió.

### 3.2 · **Las tres capas desde el día 1**, y no un MVP en un archivo

Era tentador: la v1 es un CRUD, cabe en un controlador con el SQL adentro.

> **Se descartó por una razón concreta y comprobable:** si la v1 se hace en un
> archivo, la v2 es **reescribirla**. Y lo que el estudiante aprendería es que
> «primero se hace rápido y luego se arregla» — que es justo el hábito que
> produce los sistemas que nadie quiere tocar.
>
> **El costo de hacerlo bien en la v1 es un día. El de arreglarlo en la v5 es el
> proyecto.**

### 3.3 · La validación vive en las **peticiones**, una por verbo

`ProductoCrear`, `ProductoReemplazo`, `ProductoActualizar`: tres clases para un
recurso, y no una.

| Por qué tres | |
|---|---|
| Porque **cada verbo exige cosas distintas** | En `POST` el nombre es obligatorio; en `PATCH` no, porque lo que no se manda no se toca |
| Porque ASP.NET valida **antes** del controlador | Las anotaciones producen un **422 con la lista de errores** sin que el controlador se entere |
| Porque la entidad no es un formulario | de `Producto` se arman los objetos que **viajan**; `ProductoCrear` es lo que **se acepta** |

> **Y de aquí sale una pregunta de sustentación que casi nadie responde bien:**
> con el **mismo cuerpo**, el `PUT` da 422 y el `PATCH` da 200. No es un error:
> es que `ProductoReemplazo` tiene `[Required]` y `ProductoActualizar` no. Ver
> [`SUSTENTACION_DEL_CODIGO.md`](SUSTENTACION_DEL_CODIGO.md) §7.

### 3.4 · **Sin fábrica**, y sin selección de motor

La v5 va a traer dos motores y una fábrica. Y en la v1 **no se escribió nada de
eso**.

> **Es YAGNI aplicado a conciencia** — *You Aren't Gonna Need It*, Artículo 1. El
> ensamblador de la v1 es la sección de inyección de dependencias de
> `Program.cs`, y punto. Escribir la fábrica «porque en la v5 va a hacer falta»
> es construir para un futuro que todavía puede cambiar: de hecho **cambió**, y
> la versión del motor pasó de ser la v4 a ser la v5 (ver
> [`CRONOGRAMA.md`](CRONOGRAMA.md) §2).

---

## 4. El plan, en diez pasos

El orden no es casual: cada paso deja algo **comprobable** antes de seguir.

| # | Paso | Y qué se comprueba antes de pasar al siguiente |
|---|---|---|
| **1** | `docker-compose.yml` con SQL Server y la API | El contenedor del motor acepta conexiones |
| **2** | El esquema se siembra solo al levantar | `SELECT name FROM sys.tables` da 12 |
| **3** | El proyecto de API, con Swagger | `/swagger` abre |
| **4** | El endpoint `/` de diagnóstico | Responde la versión |
| **5** | `Producto`, y sus tres peticiones por verbo | Un `POST` sin nombre da **422 con la lista** |
| **6** | `IRepositorioProducto` → `RepositorioProductoSqlServer` | Un `GET` trae las 8 filas sembradas |
| **7** | `IServicioProducto` → `ServicioProducto` | El servicio no nombra nada de HTTP |
| **8** | `ProductoController`, los cinco verbos | Los cinco responden lo que el contrato dice |
| **9** | **Los otros cinco recursos**, calcando el molde | Las 37 operaciones en Swagger |
| **10** | **Las seis pantallas**, una por recurso | Se crea un producto desde el navegador |

> **Los pasos 5 a 8 son UNA rebanada vertical, y en ese orden.** Primero el
> contrato de entrada, luego el SQL, luego la regla, y el controlador **al
> final**. Hacerlo al revés —empezar por el controlador— obliga a inventar qué
> va a devolver el repositorio antes de haberlo escrito.

> **Y el paso 9 es donde se siente la repetición.** Cinco veces el mismo molde.
> Si en ese momento no da ganas de hacer un genérico, el estudiante no está
> prestando atención; y si las da, ya puede entender por qué el Artículo 10 dice
> que no.

---

## 5. Los tropiezos, y a dónde fue cada corrección

| Tropiezo | Cómo se vio | Dónde quedó la corrección |
|---|---|---|
| **El certificado autofirmado de SQL Server** | `self signed certificate in certificate chain` al conectar desde VS Code | `TUTORIAL_VSCODE_SQLTOOLS.md`, con **cuatro** salidas distintas |
| **El contenedor no volvía tras reiniciar el equipo** | Las bases quedaban apagadas después de reiniciar | `restart: unless-stopped` **también** para las bases |
| **Contenedores huérfanos de otro proyecto** | Puertos ocupados por un repositorio clonado antes | Una advertencia en el README **antes** del `git clone` |
| **Los puertos chocaban con los otros cursos** | Dos proyectos peleando por el 1433 | Puertos propios: API **8035**, SQL Server **11466**, y el registro en `PUERTOS.md` |

> **Ninguno de los cuatro es un error de programación**, y vale notarlo: los
> cuatro son del **entorno**. En un proyecto que se entrega con «un solo
> comando», el entorno **es** parte del producto — y por eso las correcciones
> fueron a la documentación y al `.yml`, no al código.

### Y un tropiezo del propio material

El tutorial de SSMS se escribió primero **de memoria**, y la lección salió mal.
Se rehizo con **capturas reales**, abriendo el programa y haciendo los pasos.

> **Es la misma lección que este curso repite en todos los niveles:** lo que se
> escribe sin correrlo sale mal, y sale mal de una forma que solo se nota cuando
> alguien lo intenta seguir.

---

## 6. Cómo se construye con ayuda de una IA

La v1 se construyó con una IA, y el reparto fue éste:

| Lo hace la persona | Lo hace la IA |
|---|---|
| Escribir la **constitución** y el **spec** | Generar el código que el spec describe |
| Decidir el stack, el corte de versiones, los puertos | Calcar el molde a los otros cinco recursos |
| **Correr el sistema y mirar qué devuelve** | Explicar y **comentar** lo que generó |
| **Firmar las compuertas** | — |

> **La compuerta no la pasa la IA.** `9_checklist.md` lo marca **una persona**,
> y lo marca después de haber **corrido** el sistema. Una IA puede decir que un
> criterio se cumple; solo una persona puede haberlo visto cumplirse.

> **Y desde la v2 la IA tiene una obligación más: comentar el código que
> genera** — y esa **interpretabilidad se califica**, de forma verbal y en
> persona. El profesor abre un archivo y quien lo entregó cuenta qué hace y por
> qué está así. Código que nadie puede explicar no está terminado, aunque corra.

---

## 7. Dónde vamos

La v1 deja seis recursos que **no dependen de nada**. Lo siguiente es
exactamente lo contrario: las seis tablas que **sí** dependen.

> **Y eso no es «más de lo mismo con un campo extra».** Una clave foránea trae
> consigo un código HTTP nuevo —el **409**—, un control nuevo en la interfaz —el
> **desplegable**—, y una operación que **no puede quedar a medias** —la factura
> con sus renglones, en una transacción—. Sigue en [`PLAN_V2.md`](PLAN_V2.md).
