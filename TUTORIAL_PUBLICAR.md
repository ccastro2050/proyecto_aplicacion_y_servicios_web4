# Tutorial — publicar la API y el front en un hospedaje

> **Qué es este documento.** Cómo se publica el sistema en un servidor de
> verdad: la base de datos, la API y la interfaz gráfica, cada una con su
> comando y con cómo se comprueba que quedó arriba.
>
> **Qué NO es.** No es un tutorial de Docker. Aquí el sistema **no corre en
> contenedores**: corre en un hospedaje de Windows con IIS, que es otra cosa y
> tiene otros problemas.
>
> Versión 0.1 · en construcción · 22 de octubre de 2026.

---

## Antes de empezar: las credenciales NO van en este documento

> **Este repositorio es público.** Una contraseña escrita aquí queda en el
> historial de git para siempre, y rotarla después no la borra del historial.

En el tutorial todo lo sensible va como **marcador**:

| Marcador | Qué es |
|---|---|
| `<SERVIDOR_BD>` · `<BASE>` · `<USUARIO_BD>` · `<CLAVE_BD>` | La base de datos del hospedaje |
| `<SITIO_API>` · `<CLAVE_API>` | El sitio y la clave de WebDeploy de la API |
| `<SITIO_FRONT>` · `<CLAVE_FRONT>` | Los de la interfaz gráfica |
| `<CLAVE_JWT>` | La clave con que se firman los tokens |
| `<CLAVE_CORREO>` | La clave de aplicación del correo |

**Los valores reales van en un archivo local que NO se commitea.** Cree
`credenciales.local.txt` en la raíz y agréguelo al `.gitignore`:

```powershell
# Crea el archivo y lo deja fuera de git, en una sola orden.
# "Add-Content" agrega una línea al final de un archivo sin borrar lo que ya
# tenía; si el archivo no existe, lo crea.
Add-Content -Path .gitignore -Value "credenciales.local.txt"
```

> **Y una advertencia que vale decir ahora:** si una de esas claves ya salió de
> su máquina —se pegó en un chat, se mandó por correo, se mostró en una
> pantalla compartida—, trátela como quemada y cámbiela en el panel del
> hospedaje. Cambiarla cuesta dos minutos; no cambiarla cuesta el sitio.

---

## Las palabras que van a aparecer, en una línea cada una

**Del panel del hospedaje:**

| Palabra | Qué es |
|---|---|
| **Website** | Un **sitio**: un espacio en el servidor con su propia dirección, donde se copian los archivos de un programa web. Aquí hacen falta dos: uno para la API y otro para la interfaz |
| **Database** | La **base de datos**: donde viven las tablas y los datos. Es un servicio **aparte** del sitio |
| **MSSQL** | **SQL Server**, el motor de Microsoft. Es el que usa este proyecto |
| **MySQL** | Otro motor, más común en hospedajes baratos. **No sirve aquí** sin reescribir el acceso a datos |
| **E-mails** · **DNS zones** | Correo propio y nombres de dominio propios. **No se usan en este tutorial** |
| **Plan Free / Premium** | El gratuito alcanza: cinco sitios y cinco bases. El de pago da más memoria y dominio propio |
| **Status: Enabled** | El sitio está **encendido** y responde |
| **Memory** · **Disk usage** | Lo que el sitio tiene asignado: 256 MB de memoria y 5 GB de disco en el plan gratuito |
| **Domain aliases** | Otros nombres que apuntan al mismo sitio. Vacío aquí: se usa el nombre que da el hospedaje |
| **Manage** · **Rename** · **Delete** | Administrar el sitio · cambiarle el nombre · borrarlo |

**Del despliegue:**

| Palabra | Qué es |
|---|---|
| **Identificador del sitio** (`site70966`) | El nombre **interno** con el que el servidor conoce al sitio. Es el que va en el comando de publicar — no la dirección pública |
| **Dirección pública** (`…runasp.net`) | La que se escribe en el navegador |
| **WebDeploy** | La herramienta de Microsoft que **copia los archivos** de su máquina al servidor por internet. Es el reemplazo moderno del FTP |
| **Publicar** (`dotnet publish`) | Compilar el proyecto y dejar en una carpeta **solo lo que el servidor necesita** — sin el código fuente |
| **Cadena de conexión** | La línea que le dice a la API **a qué base de datos conectarse y con qué cuenta** |
| **`appsettings.json`** | El archivo de configuración de la API y del front. Ahí van la cadena de conexión y la dirección de la API |
| **Certificado HTTPS** | Lo que hace que la dirección sea `https://` y no `http://`. El hospedaje lo da gratis, y **se borra con el sitio** |

---

## Los cinco pasos, y qué deja cada uno

| # | Paso | Qué queda funcionando |
|---|---|---|
| **1** | Entrar y **dejar la cuenta limpia** | Cero sitios y cero bases: se empieza desde nada |
| **2** | Crear la base de datos | Una base vacía, en la nube, a la que se puede conectar |
| **3** | El esquema y los datos | Las tablas, los disparadores, los procedimientos y las filas de ejemplo |
| **4** | Publicar la API | La API respondiendo en su dirección pública |
| **5** | Publicar la interfaz gráfica | Las pantallas, apuntando a esa API |

---

## Lo que hace falta antes de empezar

> **Este tutorial supone que usted YA tiene cuenta en el hospedaje.** Si no la
> tiene: **https://www.monsterasp.net/**, plan **FREE**, y **verifique el
> correo** — hasta que no se verifique, los botones de crear aparecen y fallan,
> sin decir por qué.

| | |
|---|---|
| **Qué da el plan gratuito** | **Cinco sitios web y cinco bases de datos**. Aquí se usan dos sitios —la API y la interfaz— y una base |
| **Qué más hace falta** | El **SDK de .NET** y **Web Deploy** instalados en su máquina, y usage: git [-v | --version] [-h | --help] [-C <path>] [-c <name>=<value>]
           [--exec-path[=<path>]] [--html-path] [--man-path] [--info-path]
           [-p | --paginate | -P | --no-pager] [--no-replace-objects] [--no-lazy-fetch]
           [--no-optional-locks] [--no-advice] [--bare] [--git-dir=<path>]
           [--work-tree=<path>] [--namespace=<name>] [--config-env=<name>=<envvar>]
           <command> [<args>]

These are common Git commands used in various situations:

start a working area (see also: git help tutorial)
   clone      Clone a repository into a new directory
   init       Create an empty Git repository or reinitialize an existing one

work on the current change (see also: git help everyday)
   add        Add file contents to the index
   mv         Move or rename a file, a directory, or a symlink
   restore    Restore working tree files
   rm         Remove files from the working tree and from the index

examine the history and state (see also: git help revisions)
   bisect     Use binary search to find the commit that introduced a bug
   diff       Show changes between commits, commit and working tree, etc
   grep       Print lines matching a pattern
   log        Show commit logs
   show       Show various types of objects
   status     Show the working tree status

grow, mark and tweak your common history
   backfill   Download missing objects in a partial clone
   branch     List, create, or delete branches
   commit     Record changes to the repository
   history    EXPERIMENTAL: Rewrite history
   merge      Join two or more development histories together
   rebase     Reapply commits on top of another base tip
   reset      Set `HEAD` or the index to a known state
   switch     Switch branches
   tag        Create, list, delete or verify tags

collaborate (see also: git help workflows)
   fetch      Download objects and refs from another repository
   pull       Fetch from and integrate with another repository or a local branch
   push       Update remote refs along with associated objects

'git help -a' and 'git help -g' list available subcommands and some
concept guides. See 'git help <command>' or 'git help <concept>'
to read about a specific subcommand or concept.
See 'git help git' for an overview of the system. para clonar los dos repositorios |

---

## Paso 1 — Entrar al panel y dejar la cuenta LIMPIA

> **De aquí arranca todo: la cuenta existe y hay que dejarla en cero.** Es el
> caso real — la cuenta ya trae lo de intentos anteriores, y publicar encima de
> eso es lo que produce las sorpresas.
>
> Si su panel ya está vacío, mire §1.1 y pase al paso 2.

### 1.1 · Entrar

Abra **https://admin.monsterasp.net/** e identifíquese.

![El panel de MonsterASP.NET, con los servicios de la cuenta](docs/tutorial_publicar/img/01_panel_inicio.png)

**Lo que hay que mirar de esta pantalla**, que es lo que va a importar después:

| Dónde | Qué dice | Por qué importa |
|---|---|---|
| Arriba, junto al nombre | **FREE** | El plan gratuito: alcanza para este tutorial, y tiene límites |
| **Websites** | *2 of 5 used* | Caben cinco sitios. Aquí hacen falta **dos**: la API y la interfaz |
| **Databases** | *1 of 5 used · 0 MySQL · 1 MSSQL* | Caben cinco bases. Hace falta **una**, de **MSSQL** — que es SQL Server |

> **Por qué un hospedaje de Windows y no uno cualquiera.** Porque la API es
> ASP.NET Core y la base de datos es SQL Server. Un hospedaje de Linux con
> MySQL —que es lo más común y lo más barato— no sirve para este proyecto sin
> cambiarle el motor, y cambiarle el motor es otra versión del sistema.

### 1.2 · Borrar los sitios web

En la columna de la izquierda, **Websites** — o **Manage websites** en la
tarjeta del panel. Las dos llevan al mismo sitio.

![La lista de sitios, con los botones Manage, Rename y Delete en cada fila](docs/tutorial_publicar/img/02_lista_sitios.png)

**Lo que muestra esta pantalla**, y conviene leerlo antes de borrar:

| Columna | Qué dice |
|---|---|
| **NAME** | La dirección pública del sitio, y debajo su **identificador interno** — `site70979`, `site70966` |
| **STATUS** | `Enabled`: está encendido y respondiendo |
| **MEMORY** · **DISK USAGE** | Lo que consume. En el plan gratuito: 256 MB de memoria y 5 GB de disco por sitio |
| A la derecha | **Manage** · **Rename** · **Delete** |

> **El identificador interno es el que importa para publicar.** La dirección
> `frontblazorfacturas-carloscastro.runasp.net` es la que ve el usuario;
> `site70979` es el nombre con el que WebDeploy se conecta, y es el que va en el
> comando del paso 5. Son dos nombres de la misma cosa, y confundirlos es el
> error más común al desplegar.

**El botón `Delete` está en cada fila**, en rojo. No hay menú escondido.

**Borre primero el front y después la API.** El front le habla a la API: al
revés queda, por un momento, una pantalla viva apuntando a una API que ya no
existe — que es exactamente la pantalla que carga y no muestra nada.

#### El diálogo de confirmación, y los dos avisos que trae

![El diálogo de confirmación, con la casilla que hay que marcar](docs/tutorial_publicar/img/03_confirmar_borrado_sitio.png)

| Lo que dice | Qué significa |
|---|---|
| *All files, domains, certificates and settings… **This cannot be undone**.* | **No hay papelera.** Se van los archivos publicados y también el certificado HTTPS |
| *Databases and e-mail hostings are **separate services** — they are not deleted with the website.* | **Borrar el sitio NO borra la base de datos.** Por eso la base se borra aparte, en §1.3 |
| *A newly created website can be deleted after at least one minute.* | Si acaba de crear un sitio y lo quiere borrar, **espere un minuto**. Antes de eso el botón falla, y el mensaje no explica por qué |

**Para que el botón rojo funcione hay que marcar la casilla** *«I understand
that the website and all its data will be deleted»*. Está así a propósito: es
la última oportunidad de notar que está borrando el que no era.

Después: **Delete website**.

> **Y una consecuencia que hay que tener presente antes de oprimir:** al borrar
> el sitio **se liberan su nombre y sus credenciales de WebDeploy**. Los sitios
> que cree después van a tener **otro identificador y otra clave**, y la
> dirección pública anterior deja de existir. Si tenía el `appsettings.json` del
> front apuntando a la API vieja, hay que rehacerlo con la dirección nueva — eso
> va en el paso 5.



#### Cómo se sabe que los dos se fueron

![La lista de sitios vacía, con el aviso Site was deleted](docs/tutorial_publicar/img/04_sitios_borrados.png)

| Dónde | Qué tiene que decir |
|---|---|
| Arriba a la derecha | **Done · Site was deleted** |
| La lista | **0 websites found** · *You don't have any websites yet* |
| **Websites**, en la columna izquierda | **0** |
| **Databases**, en la columna izquierda | **sigue en 1** |

> **Ese 1 en Databases no es un error: es la prueba de lo que avisaba el
> diálogo.** La base de datos es un servicio aparte y **sobrevivió al borrado de
> los sitios**. Se borra en §1.3.

### 1.3 · Borrar la base de datos

En la columna de la izquierda, **Databases**.

![La lista de bases de datos, con una base MSSQL y sus botones](docs/tutorial_publicar/img/05_lista_bases.png)

**Lo que muestra, y de aquí salen los datos que el paso 2 va a necesitar:**

| Columna | Qué dice |
|---|---|
| **TYPE** | **MSSQL** — es SQL Server. Si dijera MySQL, sería la base equivocada para este proyecto |
| **DATABASE** | `db53638.databaseasp.net` es **el servidor**; el `db53638` de abajo es **la base**, y también **el usuario** |
| **DISK USAGE** | `8 MB used of 1024 MB`. El plan gratuito da **1 GB por base** |
| A la derecha | **Manage** · **Rename** · **Delete** |

> **El mismo número en tres sitios.** Servidor, base y usuario comparten el
> número —`db53638`—, y eso confunde al armar la cadena de conexión: parece que
> uno se repitió. No: los tres llevan el mismo, y cuando cambie de base hay que
> cambiarlo **en los tres**.

> **Esos 8 MB son datos de verdad.** No es una base vacía: tiene el esquema y
> las filas de un intento anterior. **Al borrarla se van, y no hay papelera.**

#### El diálogo de la base, que avisa dos cosas más

![El diálogo de confirmación de la base de datos](docs/tutorial_publicar/img/06_confirmar_borrado_base.png)

| Lo que dice | Qué significa |
|---|---|
| *Websites and e-mail hostings are separate services… **Applications that use it will stop working** until you point them elsewhere.* | Es el aviso espejo del de los sitios: **borrar la base no borra el sitio**, pero el sitio deja de funcionar — se queda sin a dónde conectarse |
| *Need the data later? **Download a backup first** in Database Management → Backup.* | **Ahí está la salida si quiere conservar los datos**: la copia se baja desde **Backups**, en el menú de la izquierda, ANTES de borrar |
| *A newly created database can be deleted after at least one minute.* | Igual que con los sitios: recién creada, no se deja borrar de inmediato |

Marque la casilla y oprima **Delete database**.

> **Y de paso, fíjese en el menú de la izquierda de esta pantalla**, porque el
> paso 3 lo va a usar: **Tables**, **Users**, **Backups**, **Import** —cargar un
> archivo `.sql`—, **Run T-SQL** —ejecutar un script— y **WebMSSQL**, que es un
> cliente de base de datos dentro del navegador.
>
> **Eso significa que el esquema se puede cargar sin instalar nada**: ni SSMS,
> ni Azure Data Studio, ni el módulo de PowerShell. El paso 3 lo hace desde ahí.

### 1.4 · Comprobar que quedó limpia

<!-- CAPTURA pendiente · 07_cuenta_limpia.png : la lista de bases vacia -->

En la columna de la izquierda tienen que estar los dos en **0**:

| | |
|---|---|
| **Websites** | **0** · *You don't have any websites yet* |
| **Databases** | **0** · *You don't have any databases yet* |

> **Si alguno sigue en 1**, el borrado no se completó. A veces el panel tarda en
> refrescar: recargue con `Ctrl+F5` antes de volver a intentarlo.

**Con esto el paso 1 está terminado**, y lo que queda es una cuenta en el mismo
estado en que la recibe un estudiante que nunca ha publicado nada. De ahí para
adelante, todo lo que aparezca lo puso usted.

---

## Lo que el paso 1 dejó anotado

Tres cosas de las pantallas de este paso se van a necesitar más adelante, y
conviene tenerlas a la vista:

| | |
|---|---|
| **El plan gratuito da 5 sitios y 5 bases** | Aquí se usan **dos sitios** —API e interfaz— y **una base** |
| **El identificador del sitio no es la dirección** | `site70966` es con lo que se publica; `csharpapigenerica-carloscastro.runasp.net` es lo que ve el usuario |
| **La base trae sus propias herramientas** | **Import**, **Run T-SQL**, **WebMSSQL** y **Backups**, en el menú de la base. El esquema se carga desde ahí, sin instalar nada |

## Paso 2 — Crear la base de datos

> **Primero la base, después los sitios.** Cuando se publique la API va a
> necesitar la cadena de conexión, y la cadena no existe hasta que la base
> exista.

### 2.1 · Escoger el plan

**Databases** → **Add database**.

![El primer paso del asistente: PREMIUM contra FREE](docs/tutorial_publicar/img/08_crear_base_plan.png)

#### Qué hay en esta pantalla

| Dónde | Qué es |
|---|---|
| **Arriba, los números 1 a 4** | El asistente tiene **cuatro pasos**: **Plan** (el que está abierto), **Database** —tipo y nombre—, **Subscription** —el paquete— y **Location** —el centro de datos—. El número en azul es dónde está |
| **A la derecha, Summary** | Un resumen que **se va llenando** con lo que escoja en cada paso. Ahora está en guiones porque todavía no ha escogido nada. Sirve para revisar antes de confirmar |
| **Las dos tarjetas** | Las dos opciones de plan: **PREMIUM** y **FREE**. Cada una con su botón abajo |
| **DESIGNED FOR**, en cada tarjeta | Para qué dice el hospedaje que sirve cada plan. El gratuito dice *Hobby projects, Development, Testing, Proof of concept* — un trabajo de clase cabe ahí |
| **RECOMMENDED**, sobre PREMIUM | Es la recomendación **del vendedor**, no la del curso. Y abajo dice *Premium plan required*: si lo escoge, le pide pagar |

#### Las cuatro casillas de cada tarjeta, una por una

| Casilla | Qué dice el plan gratuito | Qué significa |
|---|---|---|
| **DISK SPACE** | `1 GB per database · fixed, not expandable` | Cuánto pesan **sus datos**: las tablas, los índices y el registro de transacciones. **`fixed`** quiere decir que **no se puede ampliar** — si se llena, hay que borrar datos o pasar al plan de pago. Un GB es muchísimo aquí: la base de este ejemplo pesaba **8 MB** |
| **ENGINES** | `MSSQL + MySQL` · *SQL Server 2025 · MySQL 8.0* | Los **motores** disponibles. **MSSQL es SQL Server**, el de Microsoft, y es el que usa este proyecto; **MySQL** es otro, y no sirve aquí sin reescribir el acceso a datos. **La versión importa**: SQL Server 2025 entiende todo lo que se escriba para versiones anteriores |
| **PERFORMANCE** | `LOW · shared servers` | **Compartido**: su base vive en una máquina junto con las de otros clientes, y la potencia se reparte. Si otro proyecto está trabajando duro, el suyo responde más lento. Para un trabajo de clase no se nota; para un sistema con usuarios, sí |
| **DATACENTERS** | `EU only · no USA option` | **Dónde está físicamente el servidor.** En el plan gratuito solo hay Europa, así que desde Colombia cada consulta cruza el Atlántico. El de pago deja escoger Estados Unidos, que está más cerca |

#### Y las dos listas de abajo

| Lista | Qué trae |
|---|---|
| **INCLUDED** | *SQL Server 2025 and MySQL 8.0* — **los mismos motores del plan de pago**, solo que en servidores compartidos. Y *Simple backup and restore*: se puede bajar y subir una copia **a mano**, desde el menú de la base |
| **LIMITATIONS** | *No automatic daily backups* — **nadie hace la copia por usted** · *Limited features* — faltan funciones del panel · *Limited transfers/traffic* — hay un tope de datos transferidos al mes · *Limited quantity per customer* — cinco bases como máximo · *No guarantees* — si se cae, no hay a quién reclamarle |

> **De todas esas, la que de verdad importa en un curso es la primera de
> LIMITATIONS.** Las demás son molestias; esa es la que hace perder el trabajo.
> **Baje una copia desde Backups antes de cada entrega**, y guárdela fuera del
> hospedaje.

#### Qué escoger, y qué implica

**Oprima `Select FREE database`.**

| | FREE | PREMIUM |
|---|---|---|
| **Espacio** | **1 GB** por base, **fijo** | 2 GB, ampliable |
| **Motores** | SQL Server 2025 · MySQL 8.0 — **los mismos** | los mismos |
| **Rendimiento** | **LOW**, en servidores compartidos | alto, con SSD |
| **Centro de datos** | **solo Europa** | Europa o Estados Unidos |
| **Copias automáticas** | **no hay** | diarias, guardadas 14 días |
| **Acceso desde sus herramientas** | por la interfaz web del panel | también con SQL Management Studio y otras |

> **Los motores son los mismos en los dos planes.** Lo que cambia es el
> rendimiento y las garantías, no lo que SQL Server sabe hacer: un
> procedimiento almacenado o un disparador funcionan igual en el gratuito. Un
> gigabyte es muchísimo para un proyecto de clase — la base de este ejemplo pesa
> 8 MB.

> **Dos límites del gratuito que sí se notan, y conviene saberlos antes:**
>
> **No hay copias automáticas.** La única copia es la que uno baje a mano desde
> **Backups**. Si borra la base por error, no hay a qué volver — acabamos de
> verlo en el paso 1.
>
> **El servidor está en Europa.** Desde Colombia cada consulta cruza el
> Atlántico, así que lo publicado responde **más lento que en su máquina**. No
> es un defecto del código: es la latencia, que aparece en cuanto el sistema
> sale del `localhost`. Es de las cosas que un proyecto de aula solo enseña
> cuando se publica de verdad.

> **Y el aviso del panel es honesto:** *«Not recommended for any production
> use. No guarantees, support or automatic daily backups»*. Para un trabajo de
> clase está bien; para un sistema con usuarios de verdad, no.

### 2.2 · Escoger el motor

![El segundo paso: SQL Server, MySQL o PostgreSQL](docs/tutorial_publicar/img/09_crear_base_motor.png)

#### Qué cambió en la pantalla

| Dónde | Qué pasó |
|---|---|
| **Paso 1** | Ya no es un número: es un **visto verde**. Está hecho |
| **Summary** | La primera línea se llenó: **Hosting plan → Free plan**. Así se va confirmando lo escogido |
| **Back** | Apareció un botón para volver atrás sin perder lo andado |

#### Las tres tarjetas, y cuál va

| Motor | Para qué dice que sirve | Las etiquetas de abajo |
|---|---|---|
| **SQL Server 2025** *(MSSQL)* | ASP.NET y .NET, Entity Framework | **T-SQL** es el dialecto de SQL de Microsoft · **SSMS** es SQL Server Management Studio, el cliente de escritorio · **Compatibility 2008–2025** quiere decir que un script escrito para versiones viejas funciona igual |
| **MySQL 8.0** | PHP: WordPress, Laravel | **InnoDB** es su motor de almacenamiento · **Workbench** y **HeidiSQL** son clientes. Nótese el subtítulo: *MariaDB 10.10 compatible* — lo que corre por debajo es **MariaDB**, no MySQL de Oracle |
| **PostgreSQL 18** · **BETA** | ASP.NET Core, Node.js, Next.js | **psql** es su cliente de consola · **pgAdmin**, el gráfico. **BETA** significa que el hospedaje todavía lo está probando |

**Escoja `SQL Server 2025`.**

> **Por qué ése y no los otros dos**, que es la pregunta que un estudiante debe
> poder responder: los repositorios de este proyecto **escriben T-SQL a mano** y
> se conectan con el cliente de SQL Server. Cambiar de motor no es escoger otra
> tarjeta aquí: es reescribir la capa de acceso a datos.

> **Y fíjese en la tarjeta de PostgreSQL**, porque dice algo del curso: también
> sirve para ASP.NET Core. Es exactamente el segundo motor del proyecto, el de
> la v5 — así que el día que exista esa versión, **también se puede publicar
> aquí**. Que esté en **BETA** quiere decir que todavía no hay que confiarle un
> trabajo con fecha de entrega.

#### Al escoger SQL Server aparecen dos ajustes más

![SQL Server escogido, con los ajustes de colación e idioma](docs/tutorial_publicar/img/10_crear_base_ajustes.png)

| Dónde | Qué pasó |
|---|---|
| La tarjeta de SQL Server | Quedó **enmarcada en verde con un visto** |
| **Summary** | Se llenó la segunda línea: **Database type → SQL Server (MSSQL)** |
| Abajo | Apareció **MSSQL database settings**, que no estaba |
| A la derecha | Apareció el botón **Continue** |

**Deje los dos ajustes como están** y oprima **Continue**. Pero vale la pena
entender qué son, porque uno de los dos no se puede corregir después del todo.

#### QUÉ SE SELECCIONA, y por qué

| Ajuste | Qué seleccionar |
|---|---|
| **Collation** | **`SQL_Latin1_General_CP1_CI_AS`** — la que ya viene puesta |
| **User language** | **`English`** — el que ya viene puesto |

**No se cambia ninguno de los dos.** Y las razones, en orden de peso:

| # | Por qué esa colación |
|---|---|
| **1** | **Es la que espera el script de este proyecto.** La base se creó contra un SQL Server con esa colación. Con otra, al cargar el script las comparaciones de texto entre tablas pueden fallar — ver más abajo |
| **2** | **`CI` es lo correcto para buscar.** No distingue mayúsculas: quien escriba `pedro` encuentra a `PEDRO`. Con `CS` no lo encontraría, y creería que el dato no existe |
| **3** | **`AS` es lo correcto para el castellano.** Sí distingue tildes: `papá` y `papa` siguen siendo dos palabras. Con `AI` se volverían la misma |
| **4** | **No se puede corregir del todo después.** Cambiarla más adelante solo afecta a las tablas que nazcan desde ese momento |

**Y el idioma en inglés** porque cambia **los mensajes de error del servidor**:
un error en inglés se busca y aparece resuelto mil veces; el mismo traducido,
casi nunca. No toca sus datos — solo los mensajes y el formato de fecha. Ése sí
se cambia cuando quiera, en **Database Management → Users**.

#### Qué significa cada parte del nombre

`SQL_Latin1_General_CP1_CI_AS` se lee por partes:

| Parte | Qué significa |
|---|---|
| `Latin1_General` | El alfabeto: el latino, con sus reglas de orden |
| `CP1` | La página de códigos para los datos que no son Unicode |
| **`CI`** | **Case Insensitive**: no distingue mayúsculas. `PEDRO` = `pedro` |
| **`AS`** | **Accent Sensitive**: sí distingue tildes. `José` ≠ `Jose` |

**Las otras terminaciones que ofrece la lista, y por qué no:**

| Terminación | Qué haría | ¿Sirve aquí? |
|---|---|---|
| `_CI_AI` | No distinguiría mayúsculas **ni** tildes | **No**: `José` y `Jose` serían la misma persona |
| `_CS_AS` | Distinguiría ambas | **No**: buscar `pedro` no encontraría a `PEDRO` |
| `Modern_Spanish_CI_AS` | Reglas de ordenamiento del español | Funciona, pero **no es la que esperan .NET y el script del proyecto** |

#### El error que esto evita: *collation conflict*

Es lo que responde SQL Server cuando tiene que **comparar dos textos guardados
con reglas distintas** y no sabe cuáles aplicar. Entonces no escoge: se niega.

```
Cannot resolve the collation conflict between
"SQL_Latin1_General_CP1_CI_AS" and "Modern_Spanish_CI_AS"
in the equal to operation.
```

| | |
|---|---|
| **Cuándo aparece** | Cuando una consulta junta texto de dos sitios con colación distinta: un `JOIN` por un campo de texto, un `WHERE a.nombre = b.nombre`, un `UNION`, una concatenación |
| **De dónde sale** | Cada columna **guarda la colación con la que nació**. Si la base se creó con una y las tablas vinieron de un script hecho contra otra, quedan columnas con reglas distintas en la misma base |
| **Por qué es traicionero** | **Aparece al consultar, no al crear.** Las tablas se cargan bien y los datos entran bien; el fallo salta semanas después, en el `JOIN` de una pantalla. Y el mensaje habla de colaciones — no dice que las tablas vengan de bases distintas, que es la causa |
| **Cómo se arregla** | En la consulta, forzando una con `COLLATE` — un parche, y hay que repetirlo en cada consulta que compare. De verdad: recreando la columna con la colación correcta, lo que implica mover los datos |

> **Por eso se escoge bien ahora.** Es de los errores que se evitan en treinta
> segundos al crear la base y cuestan horas después.

> **Y el aviso verde del panel dice lo mismo en una línea:** *«Restoring a backup
> from another server? Pick the **same collation** as the original»*.

> **El panel también avisa lo otro:** *«The collation is applied when the
> database is created… that only changes the default for new objects — columns
> in existing tables keep the collation they were created with»*. O sea: se
> escoge ahora, o se arrastra.

#### El idioma del usuario

`English`, y **déjelo así**. Cambia el idioma de los **mensajes del servidor** y
el **formato de fecha** con que entra ese usuario — **no traduce sus datos**.

> **Déjelo en inglés por una razón práctica:** cuando salga un error y lo vaya a
> buscar, el mensaje en inglés tiene miles de resultados; el mismo mensaje
> traducido, casi ninguno.

Se puede cambiar después en **Database Management → Users**.

### 2.3 · El paquete, que se salta solo

Al oprimir **Continue** el asistente pasa del paso 2 al **4**. El paso 3,
*Subscription*, **se resuelve solo**: en el plan gratuito no hay paquete que
escoger, así que en el **Summary** esa línea se queda en guion. No es un error.

### 2.4 · Escoger el centro de datos

![El cuarto paso: Europa seleccionada, Estados Unidos bloqueado](docs/tutorial_publicar/img/11_crear_base_datacenter.png)

#### Qué hay en la pantalla

| Dónde | Qué dice |
|---|---|
| Los cuatro pasos | **Los cuatro con visto verde**: el asistente ya tiene todo |
| **Summary** | `Free plan` · `SQL Server (MSSQL)` con su colación debajo · `Subscription —` · `Datacenter EU` |
| **Europe Datacenter** | Ya aparece **Selected**, en verde |
| **USA Datacenter** | **Gris, no se puede escoger** |
| El aviso naranja | *USA datacenter is not available for the Free plan* |
| **GREAT SPEED FOR VISITORS FROM** | Desde dónde responde rápido cada centro |
| **TYPICAL LATENCY FROM MAJOR CITIES** | Cuánto tarda **un viaje de ida y vuelta** desde esa ciudad, en milisegundos |

**No hay nada que escoger: oprima `Create Database`.**

#### Y aquí hay una lección que conviene no pasar por alto

**Mire la tarjeta de Estados Unidos: entre las ciudades que lista está
Colombia.** Y está **bloqueada** porque el plan es gratuito.

| | |
|---|---|
| Lo que convendría desde Colombia | El centro de **Estados Unidos** |
| Lo que el plan gratuito permite | **Solo Europa** |
| Lo que eso significa | Cada consulta de su aplicación **cruza el Atlántico y vuelve** |

> **Por eso lo publicado va a responder más lento que en su máquina**, y no es
> culpa del código. En `localhost` la base está a un milímetro; publicada, está
> a un océano. Los números de la tarjeta lo dicen: desde Frankfurt son **5 ms**;
> desde Barcelona, **42 ms**. Desde Bogotá, más.

> **La regla que sale de esta pantalla, y que el panel escribe arriba:**
> *«ideally the same datacenter as your website»*. **La base y el sitio van en
> el mismo centro de datos.** Si la base queda en Europa y el sitio en Estados
> Unidos, cada consulta cruza el océano **dos veces** — una de ida y otra de
> vuelta— y la aplicación se arrastra.
>
> Como el plan gratuito solo ofrece Europa para las dos cosas, aquí salen
> emparejadas sin que haya que pensarlo. **En el plan de pago sí hay que
> escoger**, y es donde se comete el error.

### 2.5 · La base ya existe

![La lista de bases con la recién creada](docs/tutorial_publicar/img/12_base_creada.png)

El panel vuelve a la lista y ahí está: **1 database found**.

#### Por qué se llama `db75266` y no `bdfacturas`

**El nombre no se escoge: lo pone el hospedaje.** Y hay una razón concreta.

| | |
|---|---|
| **Todas las bases viven en un servidor compartido** | El suyo es `db75266.databaseasp.net`, y ahí están también las de otros clientes |
| **El nombre tiene que ser único entre todos** | Si cada quien pudiera escoger, media docena de personas pondrían `facturas` y chocarían |
| **Por eso es un número de la casa** | `db75266` es el identificador que su sistema le asignó. Lo mismo pasa con los sitios: `site70966` |
| **Y el servidor se arma con él** | `db75266` + `.databaseasp.net`. Por eso el número aparece **tres veces** en la cadena de conexión: servidor, base y usuario |

#### ¿Y se puede cambiar? No — y lo dice el panel

![El diálogo de Rename, con Database ID cannot be modified](docs/tutorial_publicar/img/13_rename_base.png)

El botón **`Rename`** existe también en el plan gratuito, pero al abrirlo se ve
qué deja cambiar y qué no:

| Campo | ¿Se puede cambiar? | Qué es |
|---|---|---|
| **Database ID** · `db75266` | **NO.** *«Database ID cannot be modified»* | **El nombre real en el servidor.** Es el que va en la cadena de conexión |
| **Friendly name** | Sí | Un alias **para la lista del panel**. Viene con el nombre del servidor puesto |
| **Description** | Sí | Sus propias notas — *«production, client XY, staging»*. **Solo se ven en este panel** |

> **Y el aviso azul del diálogo lo cierra:** *«Friendly name and description are
> displayed **only in the Control panel**… They are not visible in SQL
> Management Studio or other database tools — **the real database name on the
> server stays `db75266`**»*.

> **Entonces el alias sirve, pero para otra cosa.** No para que la cadena de
> conexión quede legible —eso no cambia nunca—, sino para **no confundirse en la
> lista** cuando haya cinco bases con números parecidos.

#### Qué ponerle, entonces

| Campo | Qué escribir | Por qué |
|---|---|---|
| **Friendly name** | `bdfacturas` | Es como se llama la base **en su máquina**. Así el panel y el proyecto dicen lo mismo |
| **Description** | `AySW4 · publicación del tutorial` | Para qué es. Cuando haya cinco, esta línea es la que evita borrar la equivocada |

![El alias escrito, con la vista previa debajo](docs/tutorial_publicar/img/14_alias_escrito.png)

**La vista previa —`PREVIEW IN LISTS`— muestra cómo va a quedar antes de
guardar**, y confirma lo importante: el identificador `db75266` **no
desaparece**, queda como etiqueta debajo.

> **Son dos botones `Save` independientes**, uno por campo. Guardar el alias no
> guarda la descripción.

![La lista ya con el alias](docs/tutorial_publicar/img/15_alias_guardado.png)

Cerrando con `Close`, la lista queda así: **`bdfacturas`** en grande, `db75266`
como etiqueta.

> **Para el proyecto de aula conviene un formato fijo**, por ejemplo
> `bdfacturas — <equipo>`. Así, cuando un estudiante comparta su pantalla
> pidiendo ayuda, se sabe de quién es la base sin preguntar.

> **Y una advertencia sobre las capturas que siguen:** de aquí en adelante el
> panel muestra `bdfacturas` donde antes decía `db75266.databaseasp.net`. Es la
> misma base — solo cambió el nombre con el que el panel la llama.



> **Y de ahí una consecuencia que conviene anticipar:** el nombre de la base en
> el servidor **no se parece** al de su máquina. En su computador la base se
> llama `bdfacturas`; publicada se llama `db75266`. Son la misma base de datos
> con dos nombres, y lo único que cambia es la cadena de conexión. **El código
> no se entera.**

#### La fila, columna por columna

| Columna | Qué dice | Qué significa |
|---|---|---|
| **STATUS** | `Enabled` | La base está **encendida** y acepta conexiones |
| **TYPE** | `MSSQL` | SQL Server, que es lo que se escogió |
| **DATABASE** | `db75266.databaseasp.net` | **El servidor.** Debajo, las etiquetas: `Free` el plan, `EU` el centro de datos, y **`db75266`** que es **la base y también el usuario** |
| **SUBSCRIPTION** | `—` | Vacío porque el plan gratuito no tiene paquete |
| **DISK USAGE** | `0 MB` de `1024 MB` | **Está vacía**: ni una tabla todavía. Eso lo pone el paso 3 |

#### Los filtros de arriba, y para qué sirven cuando hay más de una

| Control | Para qué |
|---|---|
| **Status** · `All` | Mostrar todas, o solo las encendidas |
| **Plan** · `All` | Filtrar por gratuitas o de pago |
| **Subscription** | Filtrar por paquete contratado. Sin uso en el plan gratuito |
| **Search** | Buscar por nombre o descripción |
| **Records** · `15` | Cuántas filas se ven por página |
| Las etiquetas **STATUS / PLAN / TYPE** con números | Atajos de filtrado, y de paso un conteo: `MySQL 0`, `MSSQL 1` |

> Con una sola base no hacen falta. Cuando el curso tenga cinco, sí — y el
> contador de `TYPE` es la forma rápida de ver si alguien creó una MySQL por
> error.

#### Los tres botones de la fila

| Botón | Qué hace |
|---|---|
| **Manage** | **Entra a la base**: ahí están las credenciales, la cadena de conexión, `Import`, `Run T-SQL`, `Backups` y `WebMSSQL`. Es a donde vamos ahora |
| **Rename** | Cambia solo la **descripción** con que usted la distingue — no el nombre real |
| **Delete** | Borrarla. Con su diálogo y su casilla, como en el paso 1 |

> **El número de la base cambió, y hay que anotarlo.** La anterior era
> `db53638`; la nueva es **`db75266`**. Ese número va en **tres sitios** de la
> cadena de conexión, así que cópielo a `credenciales.local.txt` antes de
> seguir.

**Oprima `Manage`.** La contraseña no se ve en esta lista: está adentro.

## Paso 3 — Dentro de la base: credenciales y herramientas

**Databases** → **Manage** en la fila de `bdfacturas`.

![La pantalla de la base, con Overview, Local access y las herramientas](docs/tutorial_publicar/img/16_base_overview.png)

Es la pantalla más densa del panel, y vale la pena recorrerla entera: casi todo
lo que falta del tutorial sale de aquí.

### 3.1 · El panel Overview, a la izquierda

| Campo | Qué dice | Qué significa |
|---|---|---|
| **ID** · **Database** | `db75266` | El nombre real en el servidor. El panel aclara: *Database real name on server* |
| **Name** | `bdfacturas` | El alias que le pusimos. Se puede cambiar otra vez con **Change** |
| **Server** | `db75266.databaseasp.net` | Y el aviso: *Always use this server name instead of IP address* — **nunca la IP**, porque cambia |
| **Collation** | `SQL_Latin1_General_CP1_CI_AS` | La que escogimos. El **Change** solo afecta a lo que nazca después |
| **Compatibility** | `SqlServer2025` | **El nivel de compatibilidad.** Decide qué sintaxis acepta el motor. Si un script viejo falla por una función que no reconoce, se baja aquí |
| **Disk limit** | `1024 MB` | Con **Increase** marcado como `PREMIUM`: ampliar cuesta |
| **Contents** | `0 tables · 1 users · 0 backups` | **Cero tablas**: la base está vacía. Ese `1 users` es la cuenta `db75266` |

### 3.2 · Local access y Remote access: DOS cadenas distintas

**Es la pestaña que más confusión causa**, y conviene entenderla antes de
copiar nada.

| Pestaña | Para qué sirve | Desde dónde funciona |
|---|---|---|
| **Local access** | La conexión que usa **la API publicada** | **Solo desde un sitio alojado aquí mismo.** La conexión no sale del centro de datos |
| **Remote access** | La conexión desde **su computador** | Desde afuera: SSMS, Azure Data Studio, un script suyo |

> **El panel lo dice sin rodeos:** *«Only usable from applications running on
> your hosted websites, **not from your local computer or external
> services**»*.
>
> **Por eso una cadena que funciona publicada puede fallar en su máquina, y al
> revés.** No es que las credenciales estén mal: es que son **dos puertas
> distintas al mismo cuarto**.

**Lo que muestra Local access:**

| Campo | Valor |
|---|---|
| **Server** | `db75266.databaseasp.net` — *internal hostname* |
| **Port** | `1433`, el puerto estándar de SQL Server |
| **Login** | `db75266` — **el mismo número otra vez** |
| **Password** | Oculta, con **Show** para verla y **Change** para cambiarla |
| **WebMSSQL** | Un cliente de base de datos **dentro del navegador** |

Y abajo, **CONNECTION STRING**, ya armada:

```
Server=db75266.databaseasp.net; Database=db75266; User Id=db75266; Password=********; Encrypt=False; MultipleActiveResultSets=True;
```

> **No hay que escribirla a mano.** El botón **Show with password** la completa,
> y el icono de copiar la lleva al portapapeles. Esa línea es **exactamente** lo
> que va en el `appsettings.json` de la API — con la contraseña de verdad.

> **Y ahí están las dos piezas que explicamos en el paso 1:** `Encrypt=False`
> porque el hospedaje no presenta un certificado validable, y
> `MultipleActiveResultSets=True` para poder tener dos consultas abiertas a la
> vez en la misma conexión.

### 3.3 · Probar antes de seguir

**CONNECTION TEST → `Test connection`.** Comprueba que la base está viva y que
esas credenciales entran.

> **Hágalo ahora.** Si falla aquí, no tiene sentido cargar el esquema ni
> publicar nada: el problema está antes.

![La prueba de conexión en verde](docs/tutorial_publicar/img/17_test_conexion_ok.png)

Cuando pasa, el recuadro se pone verde:

| | |
|---|---|
| **OK** | La base está encendida y esas credenciales entran |
| **Connected in 195 ms** | Lo que tardó el intento |

> **Si falla**, el mensaje dice qué pasó. Los dos casos comunes: el nombre del
> servidor mal copiado —sobra un espacio al pegar— o la contraseña cambiada
> desde **Change** y no actualizada donde la tenía anotada.

### 3.4 · Las herramientas, que son las que faltan

En **QUICK ACTIONS**, arriba a la derecha, y repetidas en las fichas de abajo:

| Herramienta | Para qué |
|---|---|
| **Import SQL** | **Cargar un archivo `.sql`** — es como va a entrar el esquema |
| **Run T-SQL** | Pegar y ejecutar un script, sin archivo |
| **WebMSSQL** | Un cliente completo en el navegador: ver tablas, consultar, editar |
| **Backups** | Bajar y subir copias. **La única copia del plan gratuito es la que usted baje** |
| **Add user** | Crear otra cuenta de acceso a la base |

> **Esto es lo que evita instalar SQL Server Management Studio.** El esquema, la
> siembra y las consultas de comprobación se pueden hacer desde aquí — lo que en
> un salón de clase ahorra la media hora que se va en instalaciones que fallan.

**QUICK STATISTICS** confirma dónde estamos: **Tables 0**. La base existe y está
vacía. Lo que sigue es llenarla.

### 3.5 · Remote access, que viene APAGADO

![La pestaña Remote access, deshabilitada](docs/tutorial_publicar/img/18_remote_access.png)

Al abrir la segunda pestaña, lo primero que salta: **`Disabled`**. El servidor y
la cadena de conexión aparecen tachados — **no existen hasta que se encienda**.

> **Esto explica la mitad de los «no me conecta desde mi máquina».** No es la
> contraseña ni el nombre del servidor: es que **el acceso remoto está apagado
> de fábrica**, y nadie lo mira porque la pestaña de al lado sí muestra una
> cadena que parece completa.

| | Local access | Remote access |
|---|---|---|
| **Para qué** | La API **publicada** | SSMS, Visual Studio, Azure Data Studio, CI/CD |
| **Desde dónde** | Solo desde un sitio alojado aquí | **Desde cualquier parte de internet** |
| **Estado inicial** | Encendido | **Apagado** |
| **Servidor** | `db75266.databaseasp.net` | **Otro nombre**, que solo aparece al encenderlo |
| **Login y contraseña** | `db75266` | **Los mismos** |

> **Lo único que cambia entre las dos es el nombre del servidor.** Usuario,
> contraseña, puerto y base son idénticos — y por eso el error es tan fácil de
> cometer: la cadena de `Local access` se ve bien, se pega en SSMS, y no
> conecta.

#### ¿Hay que encenderlo?

**Para este tutorial, no.** El esquema se carga desde el propio panel con
**Import SQL**, **Run T-SQL** o **WebMSSQL**, y eso no necesita acceso remoto.

| Déjelo apagado si… | Enciéndalo si… |
|---|---|
| Va a trabajar desde el panel | Quiere usar **SSMS** o **Azure Data Studio** desde su computador |
| Es un proyecto de clase | Necesita que una herramienta externa entre a la base |

> **Y una razón para dejarlo apagado que vale enseñar:** encenderlo expone la
> base de datos **a todo internet**. Mientras está apagada, el único que entra
> es el sitio publicado, que vive en la misma red. Es la diferencia entre una
> puerta interior y una puerta a la calle.

#### Si lo enciende, el aviso que hay que leer

> *«Can't connect from your computer although Remote access is enabled?
> **Repeated failed logins get an IP blocked automatically** — check your IP in
> Automated Security Protection»*.

**Es la trampa más desagradable de esta pantalla.** Alguien se equivoca de
contraseña cuatro o cinco veces, el hospedaje **bloquea su dirección IP**, y a
partir de ahí **ni siquiera la contraseña correcta funciona**. El síntoma es
idéntico al de una credencial mala, y la causa es otra.

> **Si le pasa:** deje de intentar, vaya a **Automated Security Protection** y
> quite el bloqueo de su IP. Seguir probando solo lo confirma.

## Paso 4 — Cargar el esquema y los datos

### 4.1 · Antes de subir nada: adaptar el script

**El script del proyecto NO se puede subir tal cual.** Su línea 7 dice:

```sql
USE bdfacturas_sqlserver_local;
```

| Por qué falla | |
|---|---|
| **Esa base no existe en el hospedaje** | Allá se llama `db75266` |
| **Y aunque existiera, no tendría permiso** | El usuario `db75266` solo manda sobre **su** base |

**Lo que pasa si se sube así:** el `USE` se rechaza y lo que viene después o
falla o se ejecuta donde no es. Y el error habla de la base, no del script.

**La solución es quitar esa línea y su `GO`.** Nada más — los objetos se crean
en la base a la que uno ya está conectado, que es justo lo que se quiere.

En este repositorio ya está hecho:

| Archivo | Para qué |
|---|---|
| `db/bdfacturas.sql` | El original, **para su máquina**. Lleva el `USE` |
| `db/bdfacturas_publicar.sql` | **El de publicar.** El mismo, sin el `USE` |

> **Son dos archivos a propósito, y el de publicar lo dice en su cabecera.** Un
> solo archivo que sirva para los dos sitios no existe: en local hace falta el
> `USE` y en el hospedaje estorba.

### 4.2 · La pantalla de importar

**QUICK ACTIONS → `Import SQL`**, o la ficha **Import** de abajo.

![La pantalla de importar, vacía](docs/tutorial_publicar/img/19_import_vacio.png)

La pantalla explica su propio proceso en tres pasos:

| # | Qué dice | Qué significa |
|---|---|---|
| **1** | *Upload the .sql script* | Subir el archivo, **desde su computador** (hasta 2 GB) o desde un sitio alojado aquí |
| **2** | *Start the import* | **El script corre con `sqlcmd` en el servidor**, en segundo plano |
| **3** | *Watch the progress* | La lista de **Imports** muestra filas importadas, duración y errores **en vivo** |

> **Ese «corre con `sqlcmd`» resuelve una duda importante.** `sqlcmd` es el
> cliente oficial de línea de comandos de SQL Server, y **entiende `GO`** — que
> no es T-SQL, sino una instrucción del cliente para separar lotes. El script de
> este proyecto tiene **27**, y con otro importador podrían haber reventado.

> **Y «en segundo plano» quiere decir que puede cerrar la página.** La
> importación sigue, y al volver la lista muestra en qué va. Con un script de
> noventa mil caracteres eso importa.

**Oprima `Upload SQL file`** y escoja `db/bdfacturas_publicar.sql`.

> **El tamaño no es problema:** el límite es **2 GB** y el archivo pesa **92
> KB**.

### 4.3 · Escoger el archivo

![El selector de archivos, con los tres .sql de la carpeta db](docs/tutorial_publicar/img/20_escoger_archivo.png)

En `db/` hay **tres** archivos, y solo uno sirve aquí:

| Archivo | |
|---|---|
| `bdfacturas.sql` | **No.** Es el de su máquina: lleva el `USE` que falla |
| `bdfacturas_postgres.sql` | **No.** Es del otro motor |
| **`bdfacturas_publicar.sql`** | **Sí.** 91 KB |

### 4.4 · El diálogo de confirmación, y sus tres avisos

![El diálogo de Import SQL file](docs/tutorial_publicar/img/21_dialogo_import.png)

**El archivo se renombró al subir**:
`db75266_bdfacturas_publicar_20261023035734.sql`. El hospedaje le antepone la
base y le pega la fecha y la hora — así, si sube el mismo script tres veces, los
tres quedan distinguibles.

Y trae tres avisos, cada uno con su consecuencia:

| Aviso | Qué significa |
|---|---|
| *Statements are executed with the **owner privileges**… and **cannot be undone**. If the script drops or overwrites data, **create a backup first**.* | **Este script empieza borrando.** Sus primeras líneas hacen `DROP` de disparadores, procedimientos y tablas. Sobre una base vacía da igual; **sobre una con datos, se los lleva** |
| *Errors in individual statements **do not stop the import** — they are collected in the log so you can fix and re-run only the failed parts.* | **El más importante de los tres**, y está abajo |
| *Scripts that switch to or reference **system databases** (master, model, msdb, tempdb) are **rejected**.* | Confirma por qué hubo que quitar el `USE`: el importador vigila a qué base apunta el script |

> **«Success» NO significa «todo salió bien».** Significa que el proceso
> terminó. Como los errores de una instrucción **no detienen** la importación,
> un script puede acabar en verde con la mitad de los procedimientos sin crear.
>
> **Por eso hay que mirar el registro, no el color.**

### 4.5 · El resultado

![La importación terminada: Success, 116 filas](docs/tutorial_publicar/img/22_import_exitoso.png)

| Dónde | Qué dice |
|---|---|
| **STATUS** | `Success` |
| **ROWS** | `116 rows` |
| **DURATION** | `0 seconds` |
| **Tables**, en el menú izquierdo | **12** — y antes decía **0** |

**Ese `12` es la comprobación de verdad**, no el `Success`: las doce tablas del
modelo están creadas.

> **Y las 116 filas son los datos sembrados**, los del `DATOS_DE_PRUEBA.md` del
> proyecto: productos, personas, empresas, roles, rutas, usuarios, clientes,
> vendedores y las facturas de ejemplo.

**Oprima `Detail`** en la fila de la importación: ahí está el registro
instrucción por instrucción, que es donde se ve si alguna falló en silencio.

### 4.6 · El registro, que es donde se comprueba

![El detalle de la importación: Errors 0, Exit code 0](docs/tutorial_publicar/img/23_import_detalle.png)

**Aquí sí está la confirmación**, y son dos números:

| Campo | Valor | Qué significa |
|---|---|---|
| **Errors** | **`0`** | **Ninguna instrucción falló.** Es lo que el `Success` no garantizaba |
| **Exit code** | **`0`** | `sqlcmd` terminó bien. Cualquier otro número sería un fallo |
| **Rows affected** | `116` | Las filas sembradas |
| **Duration** | `0 seconds` | Noventa mil caracteres en menos de un segundo |
| **Log size** | `0.2 KB` | Un registro corto **es buena señal**: si hubiera errores, pesaría mucho más |

**Y el registro de la derecha tiene un detalle que vale la pena notar:**

```
(3 rows affected)    (8 rows affected)    (6 rows affected)
(6 rows affected)    (4 rows affected)    (12 rows affected)
(8 rows affected)    (3 rows affected)    (21 rows affected)
(5 rows affected)                         (25 rows affected)
(15 rows affected)
```

**Son doce líneas, y suman 116.** Doce líneas para doce tablas: cada una es un
bloque de inserciones. Si faltara una tabla por sembrar, aquí habría once.

> **Esto es lo que hay que mirar, y no el color del `Success`.** Un script puede
> terminar en verde con procedimientos sin crear; lo que no miente es
> `Errors: 0` y `Exit code: 0`.

> **Y `Copy log`, arriba a la derecha**, copia el registro completo. Útil para
> pegarlo en una entrega como evidencia de que la carga salió bien — sobre todo
> porque el panel avisa que **los registros se borran solos** pasado un tiempo.

#### La comprobación final del paso 4

| Qué mirar | Qué tiene que decir |
|---|---|
| **Errors** y **Exit code** | **0** los dos |
| **Tables**, en el menú izquierdo | **12** |
| **Overview → Contents** | `12 tables` |

**Con eso la base está lista.** Tiene las doce tablas, los tres disparadores,
los dieciséis procedimientos y los datos de ejemplo. Lo que falta es la API que
la use.

### 4.7 · Ver las tablas: la comprobación que cierra el paso

**Tables**, en el menú de la izquierda.

![La lista de las doce tablas con sus filas](docs/tutorial_publicar/img/24_tablas_cargadas.png)

**Las doce tablas, con las filas que trae cada una:**

| Tabla | Filas | | Tabla | Filas |
|---|---|---|---|---|
| `cliente` | 4 | | `rol` | 5 |
| `empresa` | 3 | | `rol_usuario` | 21 |
| `factura` | 6 | | `ruta` | 15 |
| `persona` | 6 | | `rutarol` | 25 |
| `producto` | 8 | | `usuario` | 8 |
| `productosporfactura` | 12 | | `vendedor` | 3 |

> **Esas doce cifras suman 116**, que es exactamente lo que dijo el registro de
> la importación. **Las dos cuentas cuadran**, y por eso ésta es la
> comprobación que cierra el paso: no es que el proceso haya terminado — es que
> los datos están donde deben.

> **Y de paso se reconoce el modelo:** `rutarol` con 25 filas y `rol_usuario`
> con 21 son las dos tablas puente del control de acceso; `productosporfactura`
> con 12 son los renglones de las 6 facturas.

#### Los archivos físicos, abajo

| Tipo | Archivo | Usa | Límite |
|---|---|---|---|
| **Data** | `db75266.mdf` | 8 MB | **1024 MB** |
| **Log** | `db75266_log.ldf` | 8 MB | **5120 MB** |

> **El registro de transacciones tiene su propio límite, y es cinco veces el de
> los datos.** Conviene saberlo: el `1 GB` del plan gratuito es para los datos;
> el `.ldf` tiene 5 GB aparte. Un proceso que inserte y borre mucho **infla el
> registro sin que crezcan los datos**, y para eso están los botones
> **`Shrink data`** y **`Shrink logs`**, que devuelven el espacio liberado.

> **Cuidado con el botón rojo de arriba, `Delete all content`.** Vacía la base
> entera —las doce tablas— sin borrar la base. Es útil para volver a empezar un
> ejercicio, y es fácil de oprimir por error.

## Paso 5 — pendiente

<!-- Se escriben uno a uno, con sus capturas, a medida que se validan. -->
