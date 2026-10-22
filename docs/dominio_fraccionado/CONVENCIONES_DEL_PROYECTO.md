# Las convenciones del proyecto — stack, nombres, puertos y credenciales

> **Qué es este documento.** Las decisiones fijas que no cambian entre
> versiones: **con qué se construye**, **qué paquetes se permiten**, **cómo se
> nombran las cosas**, **qué puertos usa** y **con qué credenciales se prueba**.
>
> **A qué plantilla alimenta:** el `1_constitution`.
>
> **Por qué existe este documento.** Porque este contenido vivía **dentro de la
> constitución**, y la constitución se genera a partir del dominio: el documento
> habría sido su propia fuente. Apareció al correr la fase 0 — la IA pidió los
> paquetes, la convención de nombres y las credenciales, y el material no los
> traía. Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md) §8.
>
> **Material académico simulado**, pero todo lo de aquí se puede comprobar
> abriendo el `.csproj`, el `docker-compose.yml` y las carpetas.
>
> Versión 1.0 · 22 de octubre de 2026.

---

## 1. Cómo se construye el sistema por versiones

| | |
|---|---|
| **Versiones incrementales** | v1, v2, … Cada una con su spec kit propio. Una versión está TERMINADA solo cuando pasa sus criterios de aceptación; entonces se hace commit, **tag** (`v1`, `v2`, …) y **solo después** se escribe la especificación siguiente |
| **Acumulativas** | La v2 contiene la v1. De ahí la **prueba de regresión** obligatoria en cada versión |
| **No se anticipa** | **YAGNI** (*You Aren't Gonna Need It*): nada de fábricas multi-motor, capas «por si acaso» ni tablas de más antes de la versión que las pida. **El código de cada versión solo puede nombrar lo que su especificación nombra** |
| **El repositorio** | Siempre contiene la **versión en curso, funcionando** |

> **YAGNI es la regla que más se incumple con buena intención.** Escribir la
> fábrica en la v1 «porque en la v5 va a hacer falta» es construir para un
> futuro que todavía puede cambiar — y de hecho **cambió**: la versión del motor
> pasó de ser la v4 a ser la v5.

---

## 2. El stack, y los paquetes permitidos

| | |
|---|---|
| **Lenguaje** | **C#** sobre **ASP.NET Core** (.NET 10): controladores con atributos, inyección de dependencias del framework, `async`/`await` en todo el acceso a datos |
| **Interfaz gráfica** | **Blazor Server** |
| **Acceso a datos** | **SIN ORM de entidades.** El SQL se escribe **a mano**, visible y **siempre parametrizado** (`@parametro` — nunca concatenar valores) |
| **El ejecutor** | **Dapper**, un micro-ejecutor sobre ADO.NET: mapea fila→objeto pero **JAMÁS genera SQL**. Si una consulta existe, está escrita en un repositorio |

### Los paquetes externos permitidos en la v1, y ninguno más

| Paquete | Para qué |
|---|---|
| `Microsoft.Data.SqlClient` | El cliente oficial del motor |
| `Dapper` | El micro-ejecutor |
| `Swashbuckle.AspNetCore` | Swagger: la documentación interactiva |

**Y los que entran después, cada uno con la versión que lo pide:**

| Paquete | Desde | Por qué no antes |
|---|---|---|
| `BCrypt.Net-Next` | **v3** | La contraseña cifrada llega con el control de acceso |
| `Npgsql` | **v5** | El segundo motor es el tema de esa versión |

> **La lista no es burocracia: es lo que evita que al final del semestre nadie
> sepa por qué hay catorce dependencias.** Un paquete que una especificación no
> pidió, no entra — y adelantar el de una versión posterior es exactamente lo
> que YAGNI prohíbe.

---

## 3. La convención de nombres y las carpetas

| Cosa | Convención |
|---|---|
| Clases, métodos y propiedades en C# | **PascalCase**, en español |
| Interfaces | prefijo **`I`**: `IServicioProducto`, `IRepositorioFactura` |
| Tablas y columnas en la base de datos | **minúsculas**, sin separadores: `productosporfactura`, `fkcodproducto` |
| Claves foráneas | prefijo **`fk`**: `fkidcliente`, `fknumfactura` |
| Rutas de la API | **minúsculas**, en español, con guion cuando son dos palabras: `/api/rol-usuario` |
| Procedimientos almacenados | prefijo **`sp_`** · disparadores, **`trg_`** |

### Las carpetas de la API

| Carpeta | Qué contiene |
|---|---|
| `Controllers/` | Un controlador por recurso |
| `Modelos/` | Las clases entidad |
| `Peticiones/` | **El cuerpo de cada verbo** — una clase por verbo, no una por recurso |
| `Servicios/` | Las reglas de negocio, y sus interfaces |
| `Repositorios/` | El SQL y las llamadas a procedimientos, y sus interfaces |
| `Excepciones/` | Las excepciones del dominio |
| `pruebas/` | La prueba de capas, con repositorios falsos |

> **`Modelos/` y `Peticiones/` separados es una decisión, no desorden.** De
> `Producto` se arman los objetos que **viajan**; `ProductoCrear` es lo que **se
> acepta**. Y hay tres peticiones por recurso porque **cada verbo exige cosas
> distintas**: en el `POST` el nombre es obligatorio; en el `PATCH` no, porque
> lo que no se manda no se toca.

---

## 4. Las rutas base

| Ruta | Qué responde |
|---|---|
| `/` | **El diagnóstico**: si la API está viva y **en qué versión** |
| `/swagger` | La documentación interactiva |
| `/api/{recurso}` | Los recursos: `/api/producto`, `/api/factura`, `/api/usuario`… |

### El contraste PUT / PATCH, que rige en todas las versiones

Los dos verbos reciben **el mismo cuerpo** y responden distinto, y eso es a
propósito:

| Verbo | Qué significa | Con `{"nombre":"X"}` |
|---|---|---|
| **`PUT`** | **Reemplazo completo** | **422**: faltan los demás campos obligatorios |
| **`PATCH`** | **Actualización parcial** | **200**, y los otros campos **no se tocan** |

> **No es una inconsistencia: son dos peticiones distintas.** La de reemplazo
> lleva `[Required]` en todos sus campos; la de actualización, en ninguno. De
> ahí que haya **una clase de petición por verbo** y no una por recurso.
>
> **Y es la pregunta de sustentación que casi nadie responde bien**, porque hay
> que haberlo visto pasar: quien no lo ha probado se cree que el PUT y el PATCH
> hacen lo mismo con menos letras.


---

## 5. Los puertos

| Servicio | Puerto |
|---|---|
| API de facturas | **8035** |
| Interfaz gráfica | **8099** |
| SQL Server | **11466** |
| PostgreSQL | **15462** |

**Ninguno se repite con otro proyecto del semestre**, y el registro está en
[`PUERTOS.md`](../../../PUERTOS.md) — un nivel arriba del repositorio, porque es
el registro **compartido** entre todos los proyectos.

> **Por qué un registro fuera del repositorio.** Porque el choque de puertos no
> ocurre dentro de un proyecto: ocurre cuando dos proyectos distintos quieren el
> mismo. Un registro que vive dentro de uno de ellos no puede evitarlo.

---

## 6. Las credenciales, que son didácticas

| | |
|---|---|
| Base de datos | usuario `sa` · clave `Paradigmas123!` |
| Base | `bdfacturas` |

> **Están quemadas en el `docker-compose.yml` a propósito, y eso NO se imita.**
> Es material de clase y tiene que levantar sin configurar nada. En el proyecto
> de aula **no se vale**: ahí van en variables de entorno, y la rúbrica lo
> califica. Ver [`REQUISITOS_NO_FUNCIONALES.md`](REQUISITOS_NO_FUNCIONALES.md)
> RNF-18.

---

## 7. El idioma

| | |
|---|---|
| Nombres, rutas, mensajes, comentarios y documentación | **en castellano** |
| Los comentarios del código | **línea a línea**: qué significa cada construcción del lenguaje y para qué sirve aquí |
| La interfaz | **sin jerga**: ni «PUT», ni «422», ni «FK» |

> **El repositorio es material de estudio, no solo software.** Por eso se
> comenta más de lo que se comentaría en producción — y por eso la
> interpretabilidad se califica hablando.

---

## 8. Dónde comprobar cada cosa

| Qué | Dónde |
|---|---|
| Los paquetes de verdad instalados | `api_facturas/*.csproj` |
| Los puertos de verdad publicados | `docker-compose.yml` |
| Las carpetas | `api_facturas/` |
| Que no haya SQL concatenado | Buscar `+ "` dentro de los repositorios: no debe haber |
| Las medidas de todo lo anterior | [`REQUISITOS_NO_FUNCIONALES.md`](REQUISITOS_NO_FUNCIONALES.md) |
