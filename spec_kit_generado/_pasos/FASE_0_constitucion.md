# FASE 0 — Generar `1_constitution.md`

> Seleccione todo (`Ctrl+A`), cópielo y péguelo en una conversación **nueva**.
> Ya trae el prompt y lo que necesita.
>
> **Qué produce:** la constitución — **una sola para las cuatro versiones**,
> generada desde la v1. Va en `docs/spec_kit/1_constitution.md`, **fuera** de
> `versiones/`.
>
> Entrada aprox.: **7.800 tokens** · Salida esperada: ~2.000.

---

## EL PROMPT

```
Usted escribe la CONSTITUCIÓN de un proyecto de software: el documento
de reglas permanentes que rigen TODAS sus versiones. Se escribe una sola
vez, junto con la versión 1, y ante un conflicto con la especificación
de cualquier versión, la constitución gana.

MATERIAL (al final de este mensaje):
  1. ARQUITECTURA.md              — las capas, los procesos y las
                                    prohibiciones de cada capa
  2. REQUISITOS_NO_FUNCIONALES.md — lo que el sistema tiene que SER,
                                    con su forma de comprobarlo
  3. DOS EXTRACTOS: la tabla de códigos de estado y la procedencia de la
                    base de datos
  4. La plantilla 1_constitution.plantilla.md

REGLAS:
- DEVUELVA TODO EL RESULTADO DENTRO DE UN ÚNICO BLOQUE DE CÓDIGO
  MARKDOWN, es decir entre ```markdown y ```, para poder copiarlo de un
  clic. NO lo parta en varios bloques y NO escriba nada fuera del bloque.
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

SALIDA: el contenido de 1_constitution.md, dentro del bloque de código,
sin preámbulo ni explicación posterior.

--- MATERIAL ---
```

---

## EXTRACTO 1 — Qué causa cada código de estado

| Código | Qué significa | Quién lo decide |
|---|---|---|
| **400** | Los **parámetros** no sirven — una regla de negocio sobre la forma | el **servicio**, con `ArgumentException` |
| **401** | **No sé quién es usted** | `[Authorize]`, antes del controlador |
| **403** | **Sé quién es, y no puede** | `[ExigePermiso]`, antes del controlador |
| **404** | **Eso no existe** | el servicio, con `NoEncontradoExcepcion` |
| **405** | La ruta existe, pero **no con ese verbo** | el enrutador |
| **409** | La petición está bien y **choca con el estado** | el servicio, con `ConflictoExcepcion` |
| **422** | El **body no tiene la forma** pedida | ASP.NET, antes del controlador |
| **500** | Algo se rompió, o la base de datos rechazó | el `catch` final |

## EXTRACTO 2 — De dónde viene la base de datos

> De la v1 en adelante **viene dada al código: se copia, no se genera.**
>
> **Y por qué ninguna IA lo genera:** porque ya está hecho, y la IA **no estuvo
> en la elicitación**. Si propone un `CREATE TABLE`, está rehaciendo a ciegas un
> trabajo que tiene autores.

---

# Arquitectura — Facturación (`bdfacturas`)

> **Qué es este documento.** Cómo está partido el sistema, qué hace cada parte y
> **qué tiene prohibido hacer**. Las prohibiciones son la mitad del documento:
> una arquitectura se define tanto por lo que permite como por lo que impide.
>
> **Los conceptos detrás:**
> [`SOLID_CAPAS_PATRONES.md`](../conceptos/SOLID_CAPAS_PATRONES.md) y
> [`FLUJO_DE_UNA_PETICION.md`](../conceptos/FLUJO_DE_UNA_PETICION.md).
>
> **Material académico simulado**, pero el reparto es real y se puede comprobar
> abriendo las carpetas.
>
> Versión 1.0 · 4 de octubre de 2026.

![Las tres capas y sus interfaces](../conceptos/img/las_tres_capas.svg)

---

## 1. Tres procesos

| Proceso | Qué es | Puerto |
|---|---|---|
| **`front-blazor`** | Blazor Server. El único que le habla a la persona | 8099 |
| **`api-facturas`** | ASP.NET Core. El único que habla con la base de datos | 8035 |
| **`sqlserver`** | El motor, con **las reglas del negocio adentro** | 11466 |

En la v5 se le suma **`postgres`** (15462), y el interruptor `MOTOR_BD` decide
cuál de los dos atiende.

> **La base de datos NO es un tercero ajeno: es parte del sistema, y de las tres la que
> más código propio tiene.** De las **849 líneas de código** de
> `db/bdfacturas.sql` —sin contar comentarios—, **760 se escribieron en este
> proyecto**: los 3 disparadores y los 16 procedimientos. Las 12 tablas que
> vinieron del curso de Bases de Datos son **89**.
>
> **El conteo es de código a propósito.** El archivo entero tiene más de dos
> mil líneas porque está comentado; contar comentarios haría que la cifra
> cambiara cada vez que alguien explica mejor algo, y entonces no mediría
> nada.
>
> Y no es un dato de contabilidad: **las tablas no deciden nada.** Que el stock
> no quede negativo, que el total cuadre con sus renglones y que una factura no
> se anule dos veces vive **ahí**, no en C#. Ver
> [`FUENTES.md`](FUENTES.md) §0.

> **La regla que no se negocia: en el front no puede haber un solo
> `SqlConnection`.** Si el front puede llegar a la base de datos, la separación es un
> dibujo y no una arquitectura.
>
> **Y se comprueba, no se promete:** apague la API con la base de datos encendida y abra
> el front. Tiene que seguir en pie, con su menú y un aviso de que el servicio
> no está disponible, **y sin una sola fila**. Si sigue mostrando datos, alguien
> abrió una conexión que no debía.

---

## 2. ¿Esto es un monolito?

**Es la pregunta que más se responde mal, y casi siempre por el mismo motivo:
porque todo está en un repositorio.**

> **Un repositorio no es una unidad de despliegue.** El repositorio dice **cómo
> se guarda** el código; la arquitectura dice **cómo se ejecuta**. Son dos cosas
> distintas, y tener todo junto en GitHub se llama **monorepo** — no monolito.

### Las tres palabras, separadas

| | Qué significa | ¿Aplica aquí? |
|---|---|---|
| **Monolito** | **UNA** sola unidad desplegable: todo se compila, se despliega y se cae junto | **Sí** a la API · **Sí** al front · **No** al sistema |
| **Monorepo** | **UN** solo repositorio, con varias unidades adentro | **Sí** |
| **Microservicios** | Muchos servicios pequeños, cada uno con **su propia base**, descubrimiento y despliegue independiente | **No** |

### La respuesta, en una línea

> **La API es un monolito. El front TAMBIÉN. El sistema no.**
>
> Las tres cosas son ciertas **a la vez**, y confundir el nivel es de donde sale
> el error. «Monolito» se predica de **una unidad desplegable** — no de un
> repositorio, no de un sistema entero.

**Por qué la API es un monolito:** es **una** unidad. Sus tres capas y sus
quince controladores viven en el mismo proceso, se compilan juntos y se
despliegan juntos. Si hay que cambiar una línea de `ProductoController`, se
vuelve a desplegar **toda** la API.

**Por qué el front también:** exactamente lo mismo. Sus **15 pantallas** son un
solo proyecto, un solo contenedor y un solo despliegue. Cambiar un color obliga a
volver a publicarlo entero.

**Y por qué el sistema no lo es:** porque son **DOS monolitos**, no uno. Se
compilan por separado, se despliegan por separado, **se caen por separado** —
apague la API y el front sigue en pie— y se hablan **solo por HTTP**.

> **Y aquí está lo que de verdad hay que llevarse: «monolito» no es un insulto.**
> Casi todo software empieza siendo uno, y para este tamaño es **la decisión
> correcta**. Lo que importa no es si algo es monolito, sino **cuántas unidades
> desplegables hay y cómo se hablan**.
>
> Un monolito bien ordenado por dentro —en capas, con interfaces— se llama
> **monolito modular**, y es lo que son estos dos. Partirlos en servicios
> pequeños antes de necesitarlo solo agrega problemas de red a un sistema que
> todavía no los tenía.

**Y por qué no son microservicios:** dos monolitos no son microservicios. Faltan
todas las señas: no hay una base por servicio —hay **una** base compartida—, no
hay descubrimiento, no hay colas, y no hay despliegue independiente de partes de
la API ni del front.

### El nombre que sí le queda

**Arquitectura de tres niveles** *(three-tier)*:

| Nivel | Proceso | Qué decide |
|---|---|---|
| **Presentación** | `front-blazor` | **nada**: pregunta y obedece |
| **Aplicación** | `api-facturas` | la forma de la petición y el flujo |
| **Datos** | `sqlserver` / `postgres` | **el stock, el total, la anulación** |

> **Y aquí hay una diferencia con el three-tier de manual que vale la pena
> notar:** en el libro, el nivel de datos **guarda y ya**. En este sistema
> **defiende las reglas** — por eso la pregunta *«¿dónde va esta validación?»*
> tiene **tres** respuestas posibles y no dos. Ver §4.

### Cómo se comprueba, que es lo que lo vuelve un hecho

```powershell
# 1 · DOS unidades desplegables propias (y un proyecto de pruebas).
Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch 'obj' }

# 2 · Se despliegan por separado: apague UNA y la otra sigue.
docker compose stop api-facturas
Start-Process http://localhost:8099/facturas
#    El front sigue en pie, con su menú y sin una sola fila.
#    En un monolito eso no se puede: se cae todo junto.

# 3 · Se hablan SOLO por HTTP. Esto tiene que dar 0.
Select-String -Path front_blazor\**\*.cs,front_blazor\**\*.razor `
  -Pattern 'SqlConnection|Npgsql' | Measure-Object | Select-Object Count
```

> **El paso 2 es la prueba.** Si apagar la API tumbara también el front, serían
> una sola unidad — y entonces sí sería un monolito, por más carpetas separadas
> que tuviera.

---

## 3. Las tres capas de la API

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

## 4. La fábrica, que es donde se decide el motor

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

## 5. Dónde vive cada regla, y por qué ahí

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

## 6. El front: por qué Blazor Server y qué implica

| | |
|---|---|
| **Qué es** | El componente vive **en el servidor**; el navegador recibe HTML y mantiene un **circuito** abierto |
| **Qué gana** | No hay que escribir JavaScript para la interacción, y el estado de la pantalla vive en C# |
| **Qué cuesta** | Si el circuito se corta —se recarga la página, se pierde la red— **la pantalla pierde su estado** |

> **Eso último se nota al emitir una factura:** los renglones que se van
> agregando viven **en el circuito**, no en la base de datos. Oprimir F5 a mitad **los
> pierde**.
>
> **No es un defecto: es la consecuencia de haber escogido Blazor Server.** Si
> hiciera falta que el borrador sobreviviera, habría que guardarlo en otra parte
> —la sesión, el navegador o la base de datos—, y eso es trabajo que esta versión no
> hizo. Está declarado, sin construir, en
> [`3_HISTORIAS_PROPUESTAS.md`](elicitacion/3_HISTORIAS_PROPUESTAS.md).

---

## 7. Lo que NO tiene esta arquitectura

Decirlo evita que alguien lo busque:

| No hay | Y en su lugar |
|---|---|
| Entity Framework ni ningún ORM | SQL escrito a mano y **Dapper** como micro-ejecutor |
| Caché | Cada petición va a la base de datos |
| Colas, eventos, mensajería | Todo es síncrono dentro de la petición |
| Microservicios | Tres procesos, y ya — ver §2 |
| Repositorio genérico `Repositorio<T>` | Uno por recurso, a propósito — ver abajo |

> **Por qué no un `Repositorio<T>` genérico ni un `/api/{tabla}`.** Porque el
> contrato quedaría en blanco: Swagger no diría qué recursos hay, los permisos
> no se podrían dar por recurso, y cada cambio tocaría las doce tablas. Es el
> Artículo 10 de la constitución, y la razón está escrita ahí.

---

## 8. Comprobarlo

| Qué se afirma | Cómo se comprueba |
|---|---|
| El front no toca la base de datos | Apague `api-facturas` y abra el front: en pie y sin filas |
| Las capas no se saltan | En `Controllers/` no hay ni un `SqlConnection` —**0 archivos**—, y en `Repositorios/` no hay ni un `StatusCode` —**0**—. La palabra `Sql` sí aparece dos veces en los controladores, pero **en comentarios**, explicando que `SqlException` se traduce a 500 |
| Cambiar de motor no toca arriba | `git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios`: **vacío** |
| Las reglas están abajo | Intente dejar el stock negativo con un `INSERT` directo en SSMS |
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
