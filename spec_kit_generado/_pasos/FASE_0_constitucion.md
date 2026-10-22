# V1 · FASE 0 — Generar `1_constitution.md`

> Seleccione todo (`Ctrl+A`), cópielo y péguelo en una conversación **nueva**.
> Ya trae el prompt y los cuatro documentos que necesita.
>
> **Qué produce:** la constitución — **una sola para las cuatro versiones**,
> generada desde la v1. Va en `docs/spec_kit/1_constitution.md`, **fuera** de
> `versiones/`.
>
> **Entrada: 8.442 tokens.** Salida esperada: ~2.000.
>
> Ya no hay extractos: los dos pedazos que antes hubo que recortar
> —la tabla de códigos y la procedencia del esquema— **ahora son documentos de
> dominio propios**. Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](../../docs/dominio/METODOLOGIA_CONSTRUCCION_DOMINIOS.md).

---

## EL PROMPT

```
Usted escribe la CONSTITUCIÓN de un proyecto de software: el documento
de reglas permanentes que rigen TODAS sus versiones. Se escribe una sola
vez, junto con la versión 1, y ante un conflicto con la especificación
de cualquier versión, la constitución gana.

MATERIAL (al final de este mensaje):
  1. ARQUITECTURA_CAPAS.md        — las tres capas, qué tiene PROHIBIDO
                                    cada una, dónde vive cada regla, y lo
                                    que esta arquitectura NO tiene
  2. REQUISITOS_NO_FUNCIONALES.md — lo que el sistema tiene que SER,
                                    con su forma de comprobarlo
  3. CODIGOS_DE_ESTADO.md         — qué código responde cada fallo y
                                    quién lo decide
  4. FUENTES_PROCEDENCIA.md       — de dónde salió la base de datos y
                                    por qué ninguna IA genera el esquema
  5. CONVENCIONES_DEL_PROYECTO.md   — el stack, los paquetes por
                                    version, los nombres, los puertos y
                                    las credenciales didacticas

LA PLANTILLA 1_constitution.plantilla.md va de PRIMERA en el material,
justo despues de este prompt.

REGLAS:
- ANTES DE ESCRIBIR, REVISE SI LE ALCANZA EL MATERIAL, y dígalo. Son tres
  cosas distintas, y las tres se reportan:
    (a) un documento que el prompt nombra en el MATERIAL y no llegó;
    (b) algo que LA PLANTILLA pide —una sección, una tabla, una marca
        {{ASI}}— y que ningún documento del material responde;
    (c) algo que ESTE PROMPT le exige y que el material no sustenta.
  Pídalo por su nombre antes de empezar: «para la sección X me falta Y».
  NO lo reemplace con lo que usted supondría que dice.
- Y SI ESO NO EXISTE O NO SE LE PUEDE ENTREGAR, no lo invente: deje
  [NECESITA ACLARACIÓN: ...] en esa sección, y DESPUÉS del bloque de
  código escriba, COMO TEXTO CORRIENTE y NO en otro bloque, un apartado
  titulado «SUGERENCIAS PARA EL INGENIERO(A) DE SOFTWARE» con, por cada
  hueco: cuál de los tres casos es, qué documento de dominio haría falta
  —o qué tendría que agregarse a uno que ya existe—, qué debería
  responder, y qué parte del spec kit queda sin sustento mientras no
  exista.
- ESCRIBA UN SOLO DOCUMENTO .md, EN UN SOLO BLOQUE DE CÓDIGO, Y EL BLOQUE
  DE AFUERA LLEVA CUATRO COMILLAS INVERTIDAS, no tres: abre con
  ````markdown y cierra con ````. Es obligatorio porque el documento
  puede llevar adentro sus propios bloques de tres comillas —XML, SQL,
  C#, PowerShell, diagramas— y con tres afuera el primero de adentro
  CIERRA el de afuera: a partir de ahí todo sale crudo y las tablas se
  rompen. No genere dos archivos .md, no parta el documento en varios
  bloques, y no ponga las sugerencias en un bloque aparte. Adentro va
  ÚNICAMENTE el documento pedido, para
  poder copiarlo de un clic.
- NADA DE MARCADORES DE CITA. No agregue [cite: 1], [cite: 3, 5] ni
  referencias parecidas a los documentos que le pasé: el documento sale
  limpio, como para publicarlo. Si necesita señalar de dónde salió algo,
  nómbrelo en el texto.
- LOS DIAGRAMAS VAN DENTRO DE UN BLOQUE DE CÓDIGO, para que las flechas
  y la sangría no se descuadren al renderizar.
- En castellano. Trato de usted.
- NO invente reglas que el material no respalde. Si algo falta, escriba
  [NECESITA ACLARACIÓN: la pregunta concreta] y siga. No lo rellene.
- CADA ARTÍCULO LLEVA SU RAZÓN, en una línea. Un artículo que solo manda
  se incumple en cuanto estorba; uno que explica por qué, se discute.
- Ocho artículos o menos. Una constitución que no se lee entera no rige
  nada.
- Respete la estructura de la plantilla. Los comentarios <!-- --> de la
  plantilla son instrucciones para usted: NO los copie al resultado.
- Reemplace TODAS las marcas {{ASI}}. Si no sabe qué va en una, es un
  [NECESITA ACLARACIÓN], no una suposición.
- NO agregue tecnologías ni paquetes que el material no nombre, aunque
  sean los que normalmente acompañan a este stack. Y los paquetes
  permitidos son LOS DE LA VERSIÓN 1: no adelante los de versiones
  posteriores — eso es justamente lo que YAGNI prohíbe.
- Las RUTAS BASE van completas: diagnóstico, documentación interactiva y
  recursos.
- EL ARTÍCULO DE ERRORES DICE QUÉ CAUSA CADA CÓDIGO, no solo la lista.
  Una lista de códigos HTTP no es una convención; la convención es qué
  situación produce cada uno.
- NO AFIRME NADA SOBRE EL MODELO DE CONCURRENCIA que el material no diga.
  Que «dentro de una petición todo es síncrono» significa que no hay
  colas ni eventos — NO que el acceso a datos sea bloqueante. Si no está
  escrito, marque [NECESITA ACLARACIÓN: ¿el acceso a datos es
  asincrónico?].
- EL ARTÍCULO DE ARQUITECTURA INCLUYE LAS PROHIBICIONES que el material
  liste como «lo que NO tiene esta arquitectura». Una arquitectura se
  define tanto por lo que permite como por lo que impide.
- EL ARTÍCULO DE LA BASE DE DATOS tiene que decir que el esquema VIENE
  DADO al código —se copia, no se genera— y por qué ninguna IA lo
  genera.

SALIDA: el contenido de 1_constitution.md, dentro del bloque de código,
sin preámbulo ni explicación posterior.

--- MATERIAL ---
```

<!--
PLANTILLA — 1_constitution.md
En Spec Kit oficial: .specify/memory/constitution.md  ·  comando /speckit.constitution
Fase: ninguna. Rige TODAS las versiones y se escribe UNA VEZ.
Dónde va: docs/spec_kit/1_constitution.md  — FUERA de versiones/
Sale de: ARQUITECTURA.md + REQUISITOS_NO_FUNCIONALES.md
Un ejemplo lleno: ../docs/spec_kit/1_constitution.md
-->

# Constitución del proyecto

> **Documento permanente.** Estas reglas rigen TODAS las versiones del
> curso. Cada versión tiene además su propia especificación en
> [versiones/](versiones/0_mapa_versiones.md); ante conflicto, la
> constitución gana.

---

<!-- OBLIGATORIO. Son los artículos. Ocho es lo que cabe sin que nadie deje de
     leerlos; si le salen quince, casi siempre dos se pueden juntar.
     CADA ARTÍCULO LLEVA SU RAZÓN. Un artículo que solo manda se incumple en
     cuanto estorba; uno que explica por qué, se discute — y eso es mejor. -->

## Artículo 1 — {{COMO_SE_CONSTRUYE_EL_SISTEMA}}

- {{LA_REGLA}}
- {{LA_SEGUNDA_REGLA}}
- {{QUE_CONTIENE_SIEMPRE_EL_REPOSITORIO}}

<!-- En el ejemplo del curso este artículo fija tres cosas: que se construye por
     versiones incrementales con su tag, que NO se anticipa nada que la versión
     en curso no pida (YAGNI), y que el repositorio siempre tiene la versión en
     curso funcionando. -->

## Artículo 2 — Stack: {{LENGUAJE}} y {{FRAMEWORK}}

- Lenguaje **{{LENGUAJE}}** sobre **{{FRAMEWORK}}**: {{LO_QUE_SE_USA_DEL_FRAMEWORK}}.
- {{LA_DECISION_SOBRE_EL_ACCESO_A_DATOS}} — y **por qué**: {{LA_RAZON}}.
- Paquetes externos permitidos: {{LISTA}} — y **ninguno más** sin que una
  especificación lo pida.

<!-- La última viñeta parece burocracia y no lo es: es lo que evita que al final
     del semestre nadie sepa por qué hay catorce dependencias. -->

## Artículo 3 — Arquitectura en capas, desde el día 1

```
{{DIAGRAMA_DE_CAPAS_CON_FLECHAS}}
```

- {{QUIEN_NO_PUEDE_SABER_QUE}}
- **Solo el ensamblador** decide **qué implementación** se usa. Todo lo demás
  **recibe sus dependencias**, nunca instancia lo que necesita.
- {{COMO_VIAJAN_LOS_ERRORES_DEL_NEGOCIO_HASTA_HTTP}}

<!-- OPCIONAL pero recomendado: un aviso contra la lectura exagerada de la regla
     de arriba. En el ejemplo del curso aclara que nombrar clases concretas NO
     está prohibido —de ProductoController hay una sola—; lo prohibido es que
     una clase se fabrique sola aquello de lo que hay DOS alternativas. Sin esa
     aclaración, alguien termina poniendo una interfaz por clase. -->

## Artículo 4 — Un solo comando

`{{EL_COMANDO}}` deja TODO el sistema de la versión funcionando, desde la
primera versión.

<!-- Y si hay algo que saber sobre ese comando —que la primera vez tarda, que
     recompila al encender y el puerto no contesta hasta que termine— va aquí.
     Es lo que separa «no funciona» de «todavía no terminó de arrancar». -->

## Artículo 5 — {{CUANDO_SE_DISENA_LA_BASE_DE_DATOS}}

{{QUE_VIENE_DADO_Y_QUE_SE_ESCRIBE_EN_ESTE_PROYECTO}}

<!-- OBLIGATORIO si la base de datos viene de otro curso o de otro equipo:
     decir qué parte se heredó y qué parte se escribió aquí, con números si se
     pueden contar. En el ejemplo son 12 tablas heredadas (89 líneas) contra 3
     disparadores y 16 procedimientos escritos en el proyecto (760 líneas, el
     89 % del script) — y la conclusión es que las reglas del negocio NO están
     en las tablas. -->

> {{POR_QUE_SE_DISENA_ASI_Y_NO_POR_VERSIONES}}

## Artículo 6 — Todo en español, comentado para principiantes

- Nombres, rutas, mensajes, comentarios y documentación: **en español**.
- El código lleva **comentarios línea a línea**: qué significa cada
  construcción del lenguaje y para qué sirve aquí. El repositorio es
  material de estudio, no solo software.

## Artículo 7 — Contratos exactos

Los endpoints, formatos y códigos de estado de cada versión están en su
`6_contracts.md` y se cumplen **al pie de la letra**{{MAS_EL_CONTRASTE_DIDACTICO_SI_HAY}}.

## Artículo 8 — Convenciones fijas

| Cosa | Convención |
|---|---|
| Puertos del proyecto | {{LISTA_DE_PUERTOS}} — ninguno se repite, y el registro es [`PUERTOS.md`]({{RUTA}}) |
| Rutas | {{LAS_RUTAS_BASE}} |
| Nombres | {{LA_CONVENCION_DE_NOMBRES_Y_LAS_CARPETAS}} |
| Sobre de respuesta | Lecturas: `{{FORMA_DE_LECTURA}}` · Errores: `{{FORMA_DE_ERROR}}` |
| Errores | {{QUE_CODIGO_RESPONDE_CADA_FALLO}} |
| Credenciales (didácticas) | {{USUARIO_Y_CLAVE_DE_EJEMPLO}} |

<!-- Esta tabla es la que más se consulta de todo el spec kit, porque responde
     las preguntas que uno se hace tecleando. Vale la pena que esté completa. -->
# Arquitectura — las capas y sus prohibiciones

> **Qué es este documento.** Las tres capas de la API, **qué tiene prohibido
> hacer cada una**, dónde vive cada regla, y lo que esta arquitectura **no
> tiene**. Las prohibiciones son la mitad del documento: una arquitectura se
> define tanto por lo que permite como por lo que impide.
>
> **A qué plantilla alimenta:** el `1_constitution`.
>
> **Por qué está separado de los procesos.** Porque son **dos lectores**: la
> constitución necesita las capas y las prohibiciones; el `3_plan` necesita los
> procesos y el despliegue, que están en
> [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md). Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Los conceptos detrás:**
> [`SOLID_CAPAS_PATRONES.md`](../conceptos/SOLID_CAPAS_PATRONES.md) y
> [`FLUJO_DE_UNA_PETICION.md`](../conceptos/FLUJO_DE_UNA_PETICION.md).
>
> **Material académico simulado**, pero el reparto es real y se puede comprobar
> abriendo las carpetas.
>
> Versión 2.0 · 22 de octubre de 2026.

![Las tres capas y sus interfaces](../conceptos/img/las_tres_capas.svg)

---

## 1. Las tres capas de la API

| Capa | Qué hace | Qué tiene PROHIBIDO |
|---|---|---|
| **Controlador** | Lee la petición, valida la **forma**, traduce excepciones a códigos HTTP | Escribir SQL · tomar decisiones de negocio |
| **Servicio** | Las reglas: qué se puede y qué no | Saber qué es un 404 · saber qué motor hay debajo |
| **Repositorio** | El SQL y la llamada a los procedimientos | Decidir nada |

Y entre cada par, **una interfaz**:

```
FacturaController  →  IServicioFactura   →  IRepositorioFactura
                          ↑                        ↑
                   ServicioFactura        RepositorioFacturaSqlServer
                                          RepositorioFacturaPostgres
```

> **Lo que se usa es siempre la interfaz, nunca la clase.** Por eso el servicio
> no sabe si detrás hay SQL Server, PostgreSQL o un falso en memoria — y por eso
> se puede probar sin levantar una base.

### El inventario, por recurso

Nueve recursos con sus tres capas, más los dos puentes y las vistas agregadas.
Se puede contar:

| Carpeta | Cuántos | |
|---|---|---|
| `Controllers/` | **15** | uno por recurso, más consultas, permisos y sesión |
| `Servicios/` | **14** + sus 14 interfaces | |
| `Repositorios/` | **28** implementaciones + 14 interfaces | **dos por recurso**: SqlServer y Postgres |

Esos 28 contra 14 son la arquitectura en un número: cada contrato tiene dos
implementaciones, y arriba nadie sabe cuál está puesta.

---

## 2. La regla que no se negocia

> **En el front no puede haber un solo `SqlConnection`.** Si el front puede
> llegar a la base de datos, la separación es un dibujo y no una arquitectura.
>
> **Y se comprueba, no se promete:** apague la API con la base de datos
> encendida y abra el front. Tiene que seguir en pie, con su menú y un aviso de
> que el servicio no está disponible, **y sin una sola fila**. Si sigue
> mostrando datos, alguien abrió una conexión que no debía.

---

## 3. Dónde vive cada regla, y por qué ahí

| Regla | Dónde | Por qué no más arriba |
|---|---|---|
| El stock no queda negativo | **disparador** | Porque también vale para quien entre por SSMS |
| El total es la suma de subtotales | **disparador** | Igual, y además así no puede desactualizarse |
| Una factura tiene al menos un renglón | **procedimiento** | Va en la misma transacción que la inserta |
| Una factura no se anula dos veces | **procedimiento** | Es una condición sobre el estado, y el estado está en la base de datos |
| El `{}` del PATCH no actualiza nada | **servicio** | Es una decisión de negocio, no de la base de datos |
| Falta el campo `nombre` | **la petición** (anotaciones) | Es forma, y la forma se rechaza antes de entrar |
| ¿Tiene permiso? | **`[ExigePermiso]`**, antes del controlador | Para que ningún método pueda olvidarse de preguntarlo |

> **El criterio es simple:** cuanto más abajo vive una regla, a más gente
> protege. Una validación que solo está en C# protege a quien pasa por la API.

---

## 4. La base de datos es parte del sistema, y la que más código propio tiene

De las **849 líneas de código** de `db/bdfacturas.sql` —sin contar
comentarios—, **760 se escribieron en este proyecto**: los 3 disparadores y los
16 procedimientos. Las 12 tablas que vinieron del curso de Bases de Datos son
**89**.

> **El conteo es de código a propósito.** El archivo entero tiene más de dos
> mil líneas porque está comentado; contar comentarios haría que la cifra
> cambiara cada vez que alguien explica mejor algo, y entonces no mediría nada.

Y no es un dato de contabilidad: **las tablas no deciden nada.** Que el stock
no quede negativo, que el total cuadre con sus renglones y que una factura no
se anule dos veces vive **ahí**, no en C#. Ver
[`FUENTES.md`](FUENTES.md) §0.

---

## 5. La fábrica, que es donde se decide el motor

```
Program.cs  →  IFabricaRepositorios  →  FabricaSqlServer
                                     →  FabricaPostgres
```

Un interruptor —la variable `MOTOR_BD`— elige cuál. **Y es el único sitio del
sistema que decide CUÁL IMPLEMENTACIÓN DE REPOSITORIO se usa.**

> **No confundirlo con «no se nombran clases concretas».** `ProductoController` y
> `ServicioProducto` lo son, y se nombran sin problema: de cada uno hay **uno
> solo**. La regla aplica donde hay **dos alternativas** — los repositorios — y
> por eso son los únicos que pasan por la fábrica.

> **Ésa es la prueba del principio abierto/cerrado, y está MEDIDA con un
> `diff`:** agregar PostgreSQL fue **18 archivos y 1 373 líneas**, de los cuales
> 12 son repositorios nuevos. Y sobre `Controllers/` y `Servicios/` el `diff`
> sale **vacío**.
>
> ```powershell
> git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios
> ```
>
> **Si hubiera que tocarlos, las capas estaban mal hechas.** Nótese que el tag
> es `v4` y no `v5`: los tags son del mapa de versiones viejo, donde el motor era
> la v4. Hoy ese trabajo es la v5.

---

## 6. Lo que NO tiene esta arquitectura

Decirlo evita que alguien lo busque:

| No hay | Y en su lugar |
|---|---|
| Entity Framework ni ningún ORM | SQL escrito a mano y **Dapper** como micro-ejecutor |
| Caché | Cada petición va a la base de datos |
| Colas, eventos, mensajería | Todo es síncrono dentro de la petición |
| Microservicios | Tres procesos, y ya — ver [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md) §2 |
| Repositorio genérico `Repositorio<T>` | Uno por recurso, a propósito — ver abajo |

> **Por qué no un `Repositorio<T>` genérico ni un `/api/{tabla}`.** Porque el
> contrato quedaría en blanco: Swagger no diría qué recursos hay, los permisos
> no se podrían dar por recurso, y cada cambio tocaría las doce tablas.

> **Ojo con «todo es síncrono dentro de la petición»:** significa que **no hay
> colas, eventos ni mensajería** — no que el acceso a datos sea bloqueante. El
> acceso a datos es **asincrónico** (`async`/`await`), y eso es otra cosa.

---

## 7. Comprobarlo

| Qué se afirma | Cómo se comprueba |
|---|---|
| El front no toca la base de datos | Apague `api-facturas` y abra el front: en pie y sin filas |
| Las capas no se saltan | En `Controllers/` no hay ni un `SqlConnection` —**0 archivos**—, y en `Repositorios/` no hay ni un `StatusCode` —**0**—. La palabra `Sql` sí aparece dos veces en los controladores, pero **en comentarios**, explicando que `SqlException` se traduce a 500 |
| Cambiar de motor no toca arriba | `git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios`: **vacío** |
| Las reglas están abajo | Intente dejar el stock negativo con un `INSERT` directo en SSMS |
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
# Requisitos no funcionales — Facturación (`bdfacturas`)

> **Qué es este documento.** Lo que el sistema tiene que **ser**, no lo que tiene
> que hacer. Cada uno con un **número y una forma de comprobarlo** — porque un
> requisito no funcional que no se puede medir no es un requisito, es un deseo.
>
> **El concepto detrás:**
> [`CONCEPTOS_REQUISITOS_NO_FUNCIONALES.md`](../conceptos/CONCEPTOS_REQUISITOS_NO_FUNCIONALES.md).
>
> **De dónde salen.** Casi todos están en
> [`1_constitution.md`](../spec_kit/1_constitution.md), que es donde rigen: la
> constitución es el documento de los requisitos no funcionales de este
> proyecto, y aquí se los recoge con su forma de medirlos.
>
> **Material académico simulado.**
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Cómo leer este documento

| Columna | Qué dice |
|---|---|
| **Requisito** | Qué tiene que ser cierto, **con un número** cuando se puede |
| **Cómo se comprueba** | El comando o la observación. Si no hay ninguna, el requisito está mal escrito |
| **Dónde está escrito** | El artículo de la constitución o el documento que lo fija |

> **La prueba de que un RNF está bien escrito es que alguien pueda decir «no se
> cumple» y tener razón.** «El sistema será rápido» no admite esa frase; «la
> mediana de `GET /api/producto` responde por debajo de 200 ms» sí.

---

## 2. Arranque y entorno

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-01** | El sistema entero se levanta con **un solo comando** | `docker compose up -d --build` en una máquina limpia | Art. 4 |
| **RNF-02** | No hace falta instalar .NET, SQL Server ni nada más: **solo Docker** | Una máquina sin SDK levanta el sistema igual | Art. 4 |
| **RNF-03** | El mismo comando en otro computador da **el mismo resultado** | Dos máquinas distintas, mismo `docker compose up` | Art. 4 |
| **RNF-04** | Los puertos son fijos y **no se repiten** con otros proyectos del semestre | API 8035 · front 8099 · SQL Server 11466 · PostgreSQL 15462, contra `PUERTOS.md` | Art. 8 |

---

## 3. Separación y arquitectura

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-05** | El front **no toca la base de datos** | Apague `api-facturas` con la base de datos encendida: el front sigue en pie, con su menú y **sin una sola fila**. Y `grep SqlConnection front_blazor` da **0** | Art. 3 |
| **RNF-06** | Cada capa depende de una **interfaz**, no de una clase | Los servicios reciben `IRepositorio*` por constructor | Art. 3 |
| **RNF-07** | Cambiar de motor **no toca** controladores ni servicios | `git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios`: **vacío** | Art. 3 |
| **RNF-08** | El SQL está **a la vista**, sin ORM que lo genere | No hay Entity Framework en el `.csproj`; hay Dapper | Art. 2 |

---

## 4. Contratos y respuestas

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-09** | **Toda** respuesta es JSON, también el error | Ningún error devuelve HTML | Art. 7 |
| **RNF-10** | El sobre de lectura es `{tabla, limite, total, datos}` | `GET /api/producto` | Art. 8 |
| **RNF-11** | El sobre de error es `{estado, mensaje, detalle}`, más `errores[]` en el 422 | `POST /api/producto` con `{}` | Art. 8 |
| **RNF-12** | La API está **documentada e interactiva** | `http://localhost:8035/swagger` responde 200 | Art. 8 |
| **RNF-13** | Lo que `6_contracts.md` declara es **exactamente** lo que la API publica | `auditar_aysw4_por_version.py`, que compara el contrato contra `swagger.json` | Art. 7 |

---

## 5. Seguridad

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-14** | Las contraseñas se guardan **cifradas**, nunca en claro | `SELECT contrasena FROM usuario`: hashes BCrypt | v3 |
| **RNF-15** | Ninguna respuesta devuelve una contraseña ni su hash | `GET /api/usuario` no trae el campo. La clase `Usuario` **no lo tiene** | v3 |
| **RNF-16** | El login responde **lo mismo** para «el correo no existe» y «la clave está mal» | Dos peticiones, misma respuesta 401 y mismo texto | v3, criterio 2 |
| **RNF-17** | El permiso se consulta **en cada petición**, no se lee del token | Quite un permiso a un rol: surte efecto sin volver a entrar | v3 |
| **RNF-18** | **Cero secretos en el código** | La cadena de conexión llega por variable de entorno; `.env` en `.gitignore` | Art. 4 |

> **RNF-18 tiene una excepción declarada**, y conviene no imitarla: en este
> repositorio las credenciales **están quemadas en el `docker-compose.yml`**, y
> es a propósito — es material de clase y tiene que levantar sin configurar
> nada. En el proyecto de aula **no se vale**, y la rúbrica lo califica.

---

## 6. Idioma y legibilidad

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-19** | Todo en castellano: nombres, comentarios y mensajes | Leer cualquier archivo | Art. 6 |
| **RNF-20** | El código está **comentado** para que un principiante lo siga | Cada archivo abre diciendo qué es y qué papel cumple | Art. 6 |
| **RNF-21** | La interfaz **no habla en jerga**: ni «PUT», ni «422», ni «FK» | Recorrer las pantallas | v1 |

> **RNF-20 se califica desde la v2**, y se evalúa **hablando**: el profesor abre
> un archivo y quien lo entregó cuenta qué hace y por qué está así. Ver la
> rúbrica en `ProyectosDeAula/docs/0_METODOLOGIA.md`.

---

## 7. Accesibilidad e imagen

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-22** | Los colores de la interfaz salen del **manual de marca**, no del gusto de quien programa | Buscar `#rrggbb` fuera de `marca.css`: no debe haber | v4 |
| **RNF-23** | El texto cumple **WCAG AA** (4.5:1) | Calculado en [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) §2 | v4 |
| **RNF-24** | La interfaz es **responsive** | Abrirla a ancho de teléfono | v4 |

> **RNF-22 y RNF-23 NO se cumplían cuando se escribió este documento**, y
> escribirlo fue lo que lo destapó: había **cuatro colores de Bootstrap** en la
> hoja del proyecto, y el borde del campo enfocado daba **2.09** de contraste
> cuando WCAG pide 3.0 para un indicador.
>
> Los dos quedaron corregidos — el foco en azul cordillera (8.76) y los cuatro
> colores saliendo de la paleta. Hoy se cumplen, y §6 del manual dice cómo
> volver a contarlos.
>
> Un documento de requisitos que solo lista lo que ya se cumple no sirve para
> nada: el valor está en que la comprobación se pueda correr y falle.

---

## 8. Lo que este sistema NO promete

| No se promete | Por qué |
|---|---|
| Un tiempo de respuesta | No hay medición de carga, y prometerlo sin medirlo sería inventarlo |
| Alta disponibilidad | Un contenedor por servicio, sin réplicas |
| Respaldo automático | Hay un `backupdb/` para hacerlo a mano |
| Escalar a muchos usuarios a la vez | Blazor Server mantiene un circuito por persona |
| Funcionar sin Docker | Se puede, pero no está soportado ni probado |

> **Esta tabla es tan requisito como las otras.** Un sistema que no dice lo que
> no hace deja que cada quien suponga lo que quiera — y el día que falle, la
> culpa será de la suposición de alguien.
# Códigos de estado — qué responde el sistema ante cada fallo

> **Qué es este documento.** Qué código de estado devuelve el sistema ante cada
> clase de fallo, **quién decide cada uno**, y qué forma tiene el cuerpo. Es una
> **política**: rige en todas las versiones y no se renegocia en cada
> `6_contracts.md`.
>
> **A qué plantillas alimenta:** el `1_constitution`, el `6_contracts` y el
> `7_quickstart`.
>
> **Por qué está separado de los errores del motor.** Porque son lectores
> distintos: esto lo necesitan tres plantillas; la traducción de los errores del
> motor solo la necesitan los contratos de la v2 en adelante, y está en
> [`ERRORES_DEL_MOTOR.md`](ERRORES_DEL_MOTOR.md). Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Por qué existe.** Sin esta política, cada versión inventa sus códigos, y la
> v1 y la v2 del mismo sistema acaban respondiendo distinto al mismo error.
> Entonces el cliente no puede escribir una sola rutina de manejo.
>
> **Material académico simulado**, pero los códigos son los que la API devuelve
> de verdad: cada fila se puede comprobar con una petición.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. La tabla completa

| Código | Qué significa | Quién lo decide | Ejemplo real |
|---|---|---|---|
| **200** | Salió bien | el controlador | `GET /api/producto` |
| **204** | Salió bien y **no hay contenido**: la tabla está vacía | el controlador | `GET /api/empresa` sin empresas |
| **400** | Los **parámetros** no sirven — una regla de negocio sobre la forma | el **servicio**, con `ArgumentException` | `PATCH /api/producto/PR001` con body `{}` |
| **401** | **No sé quién es usted** | `[Authorize]`, antes del controlador | sin token, o con uno vencido |
| **403** | **Sé quién es, y no puede** | `[ExigePermiso]`, antes del controlador | token bueno, permiso ausente |
| **404** | **Eso no existe** | el servicio, con `NoEncontradoExcepcion` | `GET /api/factura/9999` |
| **405** | La ruta existe, pero **no con ese verbo** | el enrutador | `PUT /api/factura/3` |
| **409** | La petición está bien y **choca con el estado** | el servicio, con `ConflictoExcepcion` | anular una factura ya anulada |
| **422** | El **body no tiene la forma** pedida | ASP.NET, antes del controlador | `POST /api/producto` sin `nombre` |
| **500** | Algo se rompió, o la base de datos rechazó | el `catch` final | stock insuficiente |

---

## 2. Las tres parejas que se confunden, y cómo distinguirlas

### 401 contra 403

| | 401 | 403 |
|---|---|---|
| Qué dice | «No sé quién es usted» | «Sé quién es, y no puede» |
| Causa | Sin token, alterado o vencido | Token válido, permiso ausente |
| **Qué hace la persona** | Identificarse | Pedírselo al administrador |

> Confundirlos manda a alguien a arreglar lo que no está roto: si responde 401
> cuando lo que falta es un permiso, la persona va a volver a escribir su
> contraseña —y va a entrar— y seguirá sin poder.

### 422 contra 400

| | 422 | 400 |
|---|---|---|
| Qué falló | La **forma**: falta un campo, o es del tipo equivocado | El **sentido**: la forma está bien y aun así no se puede |
| Quién lo decide | ASP.NET, con las anotaciones de la petición | El servicio |
| Ejemplo | `POST /api/producto` sin `nombre` | `PATCH` con `{}` — es un JSON válido, pero no manda nada que cambiar |

> **El `{}` del PATCH es el caso que lo enseña.** Es un body perfectamente
> formado. Lo que pasa es que no pide ningún cambio, y eso es una regla de
> negocio: «actualizar nada» no es actualizar.

### 409 contra 500

| | 409 | 500 |
|---|---|---|
| Qué pasó | Choca con el **estado** del recurso, y el sistema lo **esperaba** | La base de datos rechazó y nadie lo tradujo |
| Ejemplo | Anular una factura ya anulada | Stock insuficiente · llave duplicada |

> **Y aquí hay una decisión deliberada que conviene no «arreglar»:** en la v1 una
> llave repetida responde **500**, no 409. Está escrito en su `2_spec.md`: *«la
> llave la defiende la BD, no la API. Convertirlo en 409 sería lógica de negocio
> que esta versión no pide.»* El 409 llega en la v2, cuando aparecen las llaves
> foráneas y los rechazos se vuelven cotidianos.

---

## 3. La forma del cuerpo

**Siempre JSON**, también en el error. Nunca una página de error de ASP.NET.

### El error corriente

```json
{ "estado": 404,
  "mensaje": "Factura no encontrada.",
  "detalle": "Factura 9999 no existe" }
```

| Campo | Para quién |
|---|---|
| `estado` | Redundante con el código HTTP, **a propósito**: quien lee un registro de texto no tiene el código a la vista |
| `mensaje` | Para la persona. En castellano, sin jerga |
| `detalle` | Para quien programa. Puede traer el mensaje del motor |

### El 422, que trae la lista

```json
{ "estado": 422,
  "mensaje": "Datos inválidos.",
  "errores": [ "El campo stock es obligatorio.",
               "El campo codigo es obligatorio.",
               "El campo nombre es obligatorio.",
               "El campo valorunitario es obligatorio." ] }
```

> **Los errores de forma llegan TODOS a la vez.** Devolver el primero y callar
> los otros obliga a la persona a corregir de a uno y volver a enviar: tres
> viajes para tres errores que se sabían desde el principio.

### El 403, que dice qué permiso faltó

```json
{ "estado": 403,
  "mensaje": "Su rol no tiene permiso para esta operacion.",
  "ruta": "interfaz.facturas" }
```

---

## 4. Lo que el `detalle` NO puede traer

| Nunca | Por qué |
|---|---|
| Una contraseña o un hash | Ver [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) RN-18 |
| La cadena de conexión | Trae la clave de la base de datos |
| El *stack trace* completo | Le dice a un desconocido qué biblioteca y qué versión corren |
| Si un correo **existe** | El login responde lo mismo para «no existe» y «clave mala» — ver [`ERRORES_DEL_MOTOR.md`](ERRORES_DEL_MOTOR.md) §1 |

---

## 5. Comprobarlo

Cada fila de §1 se puede verificar con una petición. Las que no necesitan
sesión:

```powershell
# 405 — la ruta existe, el verbo no
Invoke-RestMethod -Method Put -Uri http://localhost:8035/api/factura/3

# 401 — sin token
Invoke-RestMethod -Uri http://localhost:8035/api/producto
```

El resto están en el `7_quickstart.md` de cada versión, que es donde se corren
de verdad.
# La procedencia — de dónde salió la base de datos, y de dónde NO

> **Qué es este documento.** Los **dos orígenes** de la base de datos, medidos:
> qué llegó hecho de otro curso y qué se escribió en este proyecto. Y por qué
> **ninguna IA genera el esquema**.
>
> **A qué plantilla alimenta:** el `1_constitution`.
>
> **Por qué está separado de la jerarquía de autoridad.** Porque son dos
> lectores: la constitución necesita saber de dónde viene el esquema; el
> `4_research` necesita saber **qué manda sobre qué**, y eso está en
> [`FUENTES_AUTORIDAD.md`](FUENTES_AUTORIDAD.md). Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Material académico simulado.** La empresa es ficticia y no hubo ningún
> cliente. Lo que sí es real es el reparto que este documento describe, y se
> puede comprobar abriendo las carpetas.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. Dos orígenes, no uno

**Las doce tablas vienen de otro curso.** El equipo ya había modelado
`bdfacturas` en Bases de Datos: las entidades, las relaciones, las llaves y las
restricciones. Ese trabajo **se reusa tal cual** y no se vuelve a hacer.

**Pero ahí no había un sistema de facturación: había un modelo.** Todo lo que
convierte ese modelo en un sistema que defiende sus propias reglas **se escribió
en ESTE proyecto, en la fase 0**, antes de la versión 1.

### Y la diferencia se puede medir

```powershell
# Las tablas y sus restricciones, contra todo lo demás
Select-String -Path db\bdfacturas.sql -Pattern 'CREATE (TABLE|TRIGGER|PROCEDURE)' |
  Group-Object { $_.Matches[0].Groups[1].Value } | Select-Object Count, Name
```

| | | Líneas de **código** | De dónde viene |
|---|---|---|---|
| **12 tablas** con sus restricciones | | **89** | **del curso de Bases de Datos** |
| **3 disparadores** | `trg_prodfact_insert` · `_update` · `_delete` | **760** | **de este proyecto** |
| **16 procedimientos** | los 6 de `factura`, los de usuario y roles, `verificar_acceso_ruta` | | |

> **El 11 % del script llegó hecho. El 89 % se escribió aquí.** Y no es un
> detalle de contabilidad: **las tablas no deciden nada**. Que el stock no quede
> negativo, que el total cuadre con sus renglones, que una factura se anule en
> vez de corregirse y que no se pueda anular dos veces —**todas las reglas del
> negocio**— viven en los disparadores y los procedimientos, y **ninguno existía
> antes de este proyecto**.

---

## 2. De dónde salieron esas reglas: de la fase 0

```
elicitación  →  reglas de negocio  →  disparadores y procedimientos
```

| Paso | Dónde quedó |
|---|---|
| **1 · Se habla con el usuario experto** | [`elicitacion/`](elicitacion/1_PREGUNTAS.md) |
| **2 · Salen las reglas** | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) — las 22, con quién defiende cada una |
| **3 · Cada regla se implementa donde protege a más gente** | el disparador, el procedimiento, o el servicio |
| **4 · Y recién entonces, la API** | las cinco versiones |

> **Ejemplo, y se puede seguir completo:** el jefe de ventas dice *«no se puede
> vender lo que no tengo, eso es sagrado»* → nace la regla **RN-10** → y se
> implementa en `trg_prodfact_insert` con un `THROW 50001`. **No en C#**, porque
> la regla tiene que valer también para quien entre por SSMS.
>
> Las tres cosas se pueden abrir y comparar. Eso es lo que significa que una
> decisión **tenga autor**.

---

## 3. Por qué el modelo no se rehace en cada versión

> **La base de datos se diseña UNA VEZ, en la fase 0.** Desde la v1 en adelante
> **viene dada al código**: se copia, no se genera.

| | |
|---|---|
| **Por qué una sola vez** | Un modelo que cambia en cada entrega obliga a migrar datos, rehacer disparadores y reescribir procedimientos — y nada de eso es lo que el curso enseña |
| **Por qué ninguna IA lo genera** | Porque ya está hecho, y la IA **no estuvo en la elicitación**. Si propone un `CREATE TABLE`, está rehaciendo a ciegas un trabajo que tiene autores |

---

| Qué | Dónde |
|---|---|
| Qué manda sobre qué | [`FUENTES_AUTORIDAD.md`](FUENTES_AUTORIDAD.md) |
| El modelo, tabla por tabla | [`DISENO_BD.md`](DISENO_BD.md) |
| Las 22 reglas, con quién las defiende | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| La elicitación | [`elicitacion/1_PREGUNTAS.md`](elicitacion/1_PREGUNTAS.md) |
