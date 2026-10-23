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

### En este tutorial se ven, en el proyecto de aula NO

**Aquí las credenciales aparecen en las capturas a propósito**: sin verlas no
se entiende de dónde sale cada dato, y el tutorial dejaría de enseñar. Es
material de clase, y las de este ejemplo se cambian al terminar.

> **En el proyecto de aula eso no se vale, y la rúbrica lo califica.** Ahí los
> secretos van en **variables de entorno**: la cadena de conexión, la clave con
> que se firman los tokens y la clave del correo **no se escriben en ningún
> archivo del repositorio**.

| | En este tutorial | En el proyecto de aula |
|---|---|---|
| La cadena de conexión | Se ve, para poder explicarla | **Variable de entorno** |
| La clave de los tokens | Se ve | **Variable de entorno** |
| La clave del correo | Se ve | **Variable de entorno** |
| El `appsettings.json` | Se muestra lleno | **Va al repositorio sin secretos** |

> **Y la regla que hace que esto no sea solo una advertencia:** una contraseña
> escrita en el repositorio **queda en el historial de git para siempre**.
> Borrarla en un commit posterior no la saca del historial — solo la esconde de
> la vista. La única salida real es **cambiar la contraseña**.

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

### 3.6 · Encendido: qué aparece y qué significa

Al oprimir **`Enabled`**, los campos tachados se llenan.

![El panel de Remote access encendido](docs/tutorial_publicar/img/25_remote_encendido.png)

| Campo | Qué es |
|---|---|
| **Enabled / Disabled** | El interruptor. **Verde a la izquierda = encendido**, y la base queda alcanzable desde internet |
| **Server** · `db75266.public.databaseasp.net` | **El nombre público.** Es el de siempre con **`.public`** metido en medio — y ésa es toda la diferencia con el interno |
| *Never use the IP address, it can change* | El aviso del panel: **siempre el nombre, nunca la IP.** La dirección numérica del servidor puede cambiar sin avisar y dejar su configuración muerta |
| **Port** · `1433` | El puerto estándar de SQL Server. El mismo en las dos pestañas |
| **Login** · `db75266` | **El mismo usuario.** No hay una cuenta aparte para el acceso remoto |
| **Password** | **La misma contraseña**, con **`Show`** para verla y **`Change`** para cambiarla |

> **Lo único que cambia entre las dos pestañas es el nombre del servidor.**
> Usuario, contraseña, puerto y base son idénticos — por eso el error es tan
> fácil: se copia la cadena de `Local access`, se pega en SSMS, y no conecta.
> Y uno revisa la contraseña veinte veces antes de mirar el servidor.

#### Y una segunda diferencia, que no está a la vista

Compare las dos cadenas de conexión:

```
Local access:
Server=db75266.databaseasp.net; ... Encrypt=False; MultipleActiveResultSets=True;

Remote access:
Server=db75266.public.databaseasp.net; ... Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;
```

| | Local | Remota |
|---|---|---|
| `Encrypt` | **False** | **True** |
| `TrustServerCertificate` | — | **True** |

> **No es un capricho: es coherente.** La conexión local **no sale del centro
> de datos**, así que cifrarla no agrega nada. La remota **viaja por internet
> abierto**, así que se cifra — pero `TrustServerCertificate=True` significa
> que **se acepta el certificado sin validar quién lo emitió**.
>
> Para un proyecto de clase está bien. En un sistema de verdad, eso deja la
> puerta abierta a que alguien se interponga en la conexión: se cifra, sí, pero
> contra quien sea que conteste.

#### Apáguelo cuando no lo necesite

**Mientras está encendido, la base es alcanzable desde cualquier parte del
mundo.** Si solo lo encendió para cargar algo con SSMS, vuelva a `Disabled` al
terminar. La API publicada **no lo necesita**: ella usa `Local access`.

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

## Paso 5 — Crear el sitio de la API

**Websites** → **Add website**.

### 5.1 · El plan

![El asistente de sitio: seis pasos, PREMIUM contra FREE](docs/tutorial_publicar/img/26_crear_sitio_plan.png)

#### Qué hay en la pantalla

| Dónde | Qué es |
|---|---|
| Arriba | **Seis pasos**, no cuatro como la base: **Plan · Domain · DNS · Subscription · Location · Example** |
| **Summary** | El resumen que se va llenando, con una línea por paso |
| Las dos tarjetas | **PREMIUM** y **FREE**, cada una con su botón |

**Oprima `Select FREE website`.**

#### Las cuatro casillas del plan gratuito

| Casilla | Dice | Qué significa |
|---|---|---|
| **DISK SPACE** | `5 GB per website · fixed` | Para los archivos publicados. **Sobra**: una API compilada pesa unas decenas de megabytes |
| **RAM** | `256 MB dedicated · fixed` | **La memoria del proceso.** Es poco pero alcanza para una API de clase. Y es **por sitio**: la API y el front tendrán 256 MB cada uno |
| **PERFORMANCE** | `LOW · shared servers` | El servidor es compartido con otros clientes |
| **DATACENTERS** | `EU only` | **Europa**, igual que la base. Que coincidan no es casualidad: el gratuito no ofrece otra cosa, y así quedan emparejados |

#### Lo incluido, y las cinco limitaciones

**INCLUDED:** *Run ONLY on our FREE subdomains `runasp.net` / `tryasp.net`*

> **O sea: la dirección la pone el hospedaje.** Usted escoge el nombre, pero el
> dominio es de ellos. Un dominio propio es del plan de pago.

**LIMITATIONS**, y dos de las cinco importan de verdad:

| Limitación | Qué implica |
|---|---|
| **No sending e-mails** | **Ojo con esto si su aplicación manda correo.** Habrá que comprobar si bloquea también el envío por un servidor externo como Gmail — se prueba, no se supone |
| *Limited features/capabilities* | Faltan funciones del panel. **La que más se nota es `AlwaysRunning`**, que es de pago — ver abajo |
| *Limited transfers/traffic* | Hay un tope de datos transferidos al mes |
| *Limited quantity per customer* | Cinco sitios como máximo |
| *No guarantees* | Si se cae, no hay a quién reclamarle |

#### `AlwaysRunning` no está, y eso se va a notar

En la tarjeta de pago aparece **`AlwaysRunning`** entre lo incluido. En la
gratuita, no.

> **Qué significa que falte:** cuando el sitio lleva un rato sin recibir
> peticiones, **el servidor apaga el proceso** para liberar memoria. La
> siguiente petición lo despierta — y esa primera petición **tarda varios
> segundos** mientras el proceso arranca y vuelve a conectarse a la base.
>
> **Es el «arranque en frío», y confunde muchísimo:** alguien abre la
> aplicación publicada, tarda ocho segundos, y concluye que el código está mal.
> Vuelve a probar y responde al instante. **No es el código: es que estaba
> dormida.**

> **Y por eso el primer intento después de publicar siempre parece lento.** Si
> va a mostrarle el sistema a alguien, ábralo una vez antes para despertarlo.

### 5.2 · La dirección del sitio

![El segundo paso: escoger la dirección](docs/tutorial_publicar/img/27_crear_sitio_direccion.png)

#### Las tres opciones de dirección

| Opción | Qué es | ¿Disponible? |
|---|---|---|
| **Free subdomain** | Una dirección **instantánea** en `runasp.net` o `tryasp.net`. Sin configurar nada | ✅ **Ésta** |
| **Your own domain** | Un dominio propio —`midominio.com`— que usted ya tenga. Exige configurar el DNS | `PREMIUM` |
| **Your own subdomain** | Un subdominio de un dominio suyo —`api.midominio.com`—. También exige DNS | `PREMIUM` |

> **Las dos de pago no son solo un cobro**: piden **configurar el DNS**, que es
> decirle a internet que ese nombre apunta a este servidor. El subdominio
> gratuito se salta todo eso porque el dominio ya es del hospedaje.

#### Los dos campos

| Campo | Qué poner |
|---|---|
| **Website address** | El nombre. **Mínimo 3 caracteres, solo letras `a–z`, números `0–9` y guion** — sin mayúsculas, sin puntos, sin tildes |
| **Website free subdomain** | El dominio. Déjelo en **`.runasp.net`** |

**Debajo, el renglón `Your website address:` se completa mientras escribe** y
muestra cómo va a quedar. Úselo para confirmar antes de seguir.

#### Qué nombre poner, y por qué importa

**Esto no se puede cambiar después sin borrar el sitio**, así que vale pensarlo
diez segundos.

| | |
|---|---|
| **Tiene que ser único en todo el hospedaje** | No solo en su cuenta. Si alguien ya lo tomó, se lo rechaza |
| **Queda a la vista de todos** | Es la dirección pública que va a escribir cualquiera |
| **Y queda dentro del front** | El `appsettings.json` de la interfaz apunta a esta dirección. Si cambia, hay que volver a publicar el front |

**Para este ejemplo:** `apifacturas-carloscastro` → la dirección queda
`apifacturas-carloscastro.runasp.net`.

| Por qué así | |
|---|---|
| **`apifacturas`** | Dice **qué es**. Un nombre como `csharpapigenerica` describe la tecnología, no el sistema — y dentro de un mes nadie recuerda cuál era |
| **`-carloscastro`** | Es lo que garantiza que esté libre |

> **Para el proyecto de aula, un formato fijo:** `apifacturas-<equipo>` para la
> API y `facturas-<equipo>` para la interfaz. Así, mirando la lista de
> direcciones se sabe de quién es cada una sin abrir ninguna.

### 5.3 · DNS y Subscription se saltan solos

Al oprimir `Continue`, el asistente pasa del paso 2 al **5**: los pasos **3
(DNS)** y **4 (Subscription)** quedan con su visto verde sin preguntar nada.

| Paso | Por qué no pregunta |
|---|---|
| **DNS** | Con subdominio gratuito **no hay DNS que configurar**: el dominio ya es del hospedaje y apunta solo |
| **Subscription** | En el plan gratuito no hay paquete que escoger |

### 5.4 · El centro de datos: el mismo de la base

![El quinto paso: Europa, con Estados Unidos bloqueado](docs/tutorial_publicar/img/28_crear_sitio_datacenter.png)

Es la misma pantalla de la base: **Europa seleccionada, Estados Unidos
bloqueado** por ser plan gratuito.

> **Y esta vez eso es justo lo que queremos.** La base quedó en Europa, el
> sitio queda en Europa, y **los dos viven en la misma red del centro de
> datos**. Por eso la API va a poder usar `Local access`, que es la conexión
> rápida y la que no sale a internet.

**Oprima `Continue`.**

### 5.5 · La aplicación de ejemplo: ninguna

![El sexto paso: My own application, ya seleccionada](docs/tutorial_publicar/img/29_crear_sitio_ejemplo.png)

El hospedaje ofrece instalar una aplicación de demostración —una API .NET, un
Blazor, un chat con SignalR, un Angular, un Next.js— para que uno vea algo
funcionando de una vez.

**No queremos ninguna.** La primera tarjeta, **`My own application`**, ya viene
**seleccionada por defecto**, y en el `Summary` se lee
`Application: None (empty website)`.

> **Y el propio panel explica por qué no conviene escoger un ejemplo:** *«The
> example is installed into the **root of your website** and you will need to
> **delete its files** before you upload your own application. Pick **My own
> application** if you are deploying your own code right away»*.
>
> O sea: el ejemplo se instala **en la misma carpeta** donde va a ir su
> aplicación. Si lo deja, después hay que borrar sus archivos a mano — y
> mientras tanto conviven dos aplicaciones en el mismo sitio.

#### Pero la lista dice algo que sí importa: qué corre en este servidor

**No vamos a instalar ninguno, y aun así vale leerlos.** Cada tarjeta es una
demostración de algo que el hospedaje **sabe ejecutar** — y eso contesta, sin
tener que probarlo, si lo que usted va a subir va a funcionar aquí.

| Tarjeta | Qué trae | Qué demuestra |
|---|---|---|
| **My own application** `DEFAULT` | Sitio vacío. Etiquetas: **`FTP`**, **`WebDeploy`**, **`Git`**, **`Any framework`** | **Tres formas de subir** su código. Nosotros vamos a usar **WebDeploy** |
| **.NET 10 Web API** | `ASP.NET Core Minimal API` con un documento **OpenAPI**. *No database needed* | **.NET 10 ya está instalado.** No hay que pedirlo ni instalarlo |
| **Blazor** | `Blazor Web App (.NET 10)`, etiqueta **`Blazor Server`**. *«Sample interactive UI written in C# running on the server — no JavaScript required»* | **Blazor Server corre aquí** |
| **SignalR Chat** | `ASP.NET Core SignalR` sobre **WebSockets**, con presencia en vivo | **El servidor permite conexiones persistentes.** Sin esto, Blazor Server no funcionaría |
| **Angular SPA** | `.NET 10 API + Angular`, el front servido por el mismo back | **Angular corre**, servido como archivos estáticos desde ASP.NET Core |
| **Next.js** | `Node.js / Next.js / React` bajo **`iisnode`** | **Aquí también corre Node.js**, no solo .NET |

#### Entonces sí: Blazor, Angular y React

| | |
|---|---|
| **Blazor** | Sí, y **es exactamente lo que vamos a publicar en el Paso 6** |
| **Angular** | Sí, compilado y servido como archivos estáticos |
| **React / Next.js** | Sí, porque el servidor corre **Node.js bajo IIS** |

> **Y la tarjeta de Blazor es la respuesta a la pregunta de este proyecto.**
> Nuestro front —`front_blazor`— es **Blazor Server**: en `Program.cs` dice
> `AddInteractiveServerComponents()` y en `App.razor`,
> `InteractiveServerRenderMode`. Es **la misma clase de aplicación** que la
> tarjeta `Blazor`. Si el hospedaje la ofrece como ejemplo, la soporta.

> **Pero hay una advertencia que viene del plan gratuito.** Blazor Server
> mantiene **una conexión abierta con cada navegador** —el *circuito*— y el
> estado de la pantalla **vive en la memoria del servidor**. Con los 256 MB y
> sin `AlwaysRunning`, pasan dos cosas:
>
> | Qué pasa | Qué ve el estudiante |
> |---|---|
> | El sitio se duerme y el proceso se apaga | *«Attempting to reconnect to the server…»* en una franja arriba. Se recupera recargando |
> | Muchos usuarios a la vez | Cada uno ocupa memoria. Con 256 MB **no es para una demostración masiva** |
>
> **Para el proyecto de aula alcanza de sobra.** Pero si va a mostrarlo,
> ábralo antes — igual que la API.

#### Dos avisos más de la pantalla

| Aviso | Qué dice |
|---|---|
| El recuadro amarillo, arriba | *«These are **small demo projects, not finished products**»*. Son de arranque, no un producto |
| El recuadro verde, abajo | *«Looking for a real application instead of a demo? Install **WordPress, Umbraco, Oqtane** and others later from the full catalogue in **Website Management → App Installer**»* |

> **O sea que el catálogo no se cierra aquí.** Lo que no instale ahora se puede
> instalar después desde `Website Management → App Installer`, y ahí mismo se
> sigue la instalación mientras corre.

**Oprima `Create Website`.**

> **Para qué sirven los ejemplos, entonces.** Para comprobar que el hospedaje
> funciona cuando uno todavía no tiene nada que subir. Si un estudiante duda de
> si el problema es su código o el servidor, instalar el ejemplo de `.NET 10
> Web API` **en un sitio aparte** lo resuelve en dos minutos: si el ejemplo
> responde, el hospedaje está bien y el problema está en su código. Y como
> *«none of the examples needs a database»*, la prueba no depende de que la
> conexión a la base esté bien.



### 5.6 · El sitio se crea en dos tiempos

#### Primero: `Creating`

![La lista de sitios con el estado Creating](docs/tutorial_publicar/img/30_sitio_creando.png)

Al oprimir `Create Website` el asistente se cierra y vuelve a la lista. **El
sitio ya aparece, pero todavía no sirve.**

| Lo que se ve | Qué significa |
|---|---|
| **`Creating`**, en naranja | El servidor está **armando el sitio**: creando la carpeta, el *application pool* y las credenciales |
| *1 website is being prepared · the list refreshes automatically* | **No recargue la página.** La lista se actualiza sola |
| **`Tasks 3`**, arriba | Las tareas pendientes del servidor. **Van a bajar a `0`** cuando termine |
| El nombre **sin el ícono del globo** | Todavía no es alcanzable desde internet |

> **Por qué tarda.** Un sitio no es una carpeta: el hospedaje registra el
> dominio en su servidor web, le asigna un *application pool* propio con sus
> 256 MB, y le reserva **su identificador —`site101125`— y su cuenta de FTP**.
>
> **La cuenta de WebDeploy no**, y eso sorprende: esa se crea después, cuando
> usted la pida. Lo vemos en el paso 5.9.

#### Después: `Enabled`

![La lista de sitios con el estado Enabled](docs/tutorial_publicar/img/31_sitio_listo.png)

| Lo que cambió | Qué significa |
|---|---|
| **`Enabled`**, en verde | **El sitio está vivo.** Ya responde en su dirección |
| Apareció el **ícono del globo** junto al nombre | Es alcanzable desde internet |
| **`Tasks 0`** | El servidor terminó |
| Apareció el filtro **`Enabled 1`** | En la barra de `STATUS`. Antes no estaba porque no había ninguno en ese estado |

#### Lo que quedo creado, renglon por renglon

| Columna | Valor | Qué es |
|---|---|---|
| **STATUS** | `Enabled` | Encendido |
| **NAME** | `apifacturas-carloscastro.runasp.net` | **La dirección pública.** El ícono de la flecha la abre en otra pestaña |
| La etiqueta | `Free` | El plan |
| La etiqueta | `EU` | El centro de datos. **El mismo de la base de datos** |
| La etiqueta | **`site101125`** | **El identificador del sitio.** Anótelo: es el usuario de WebDeploy |
| **DOMAIN ALIASES** | `—` | No hay dominios propios apuntando aquí. Es de pago |
| **SUBSCRIPTION** | `—` | No hay paquete contratado |
| **MEMORY** | `256 MB` | Lo que anunciaba el plan, ahora asignado de verdad |
| **DISK USAGE** | `--- MB` de `5120 MB` | **Cero usado** de 5 GB. Todavía no hemos subido nada |

> **`site101125` es el dato que importa de esta pantalla.** Ese número lo asigna
> el hospedaje, no usted, y es **el nombre de usuario con el que se publica**.
> Fuera de aquí casi no se ve: la dirección pública es
> `apifacturas-carloscastro.runasp.net`, pero el usuario de publicación es
> `site101125`. **Son dos cosas distintas y se confunden todo el tiempo.**

> **Y ojo con una cosa que ya habíamos anunciado:** este identificador es
> **nuevo**. El sitio anterior, el que borramos en el Paso 1, tenía otro. Si
> alguien guardó un comando de publicación de antes, **ya no sirve** — ni el
> usuario ni la clave.

#### Compruébelo antes de seguir

**Abra la dirección en el navegador**, con el ícono de la flecha o escribiéndola:

```
https://apifacturas-carloscastro.runasp.net
```

| Qué sale | Qué quiere decir |
|---|---|
| Una página del hospedaje, o un error **403** / **404** | **Bien.** El sitio existe y responde; sencillamente **está vacío**, que es justo como lo pedimos |
| *No se puede acceder a este sitio* | **Mal.** Espere un momento: el nombre recién creado puede tardar en propagarse |

> **Que responda vacío es la señal de que todo va bien.** Lo que falta no es el
> sitio: es **nuestra aplicación dentro de él**.


### 5.7 · `Rename`: el botón que no hace lo que su nombre dice

![El diálogo Rename website](docs/tutorial_publicar/img/32_renombrar_sitio.png)

En el renglón del sitio, al lado de `Manage`, está **`Rename`**. Vale la pena
abrirlo **aunque no vayamos a renombrar nada**, porque lo que **no** deja hacer
es justo lo que hay que entender antes de publicar.

#### Los cuatro campos

| Campo | Estado | Qué es |
|---|---|---|
| **Site ID** · `site101125` | **Gris, bloqueado.** *«Site ID cannot be modified»* | **El identificador no se cambia nunca.** Lo asignó el hospedaje y es el usuario de WebDeploy |
| **Main domain** | **Gris**, con la etiqueta `current` y el `Save` pálido | El dominio principal. **Solo se puede escoger entre los dominios ya conectados al sitio** — y hay uno solo |
| **Description** | **Activo**, con su `Save` azul | **Lo único que sí se puede cambiar.** Una nota suya, de hasta 200 caracteres |
| **PREVIEW IN LISTS** | Solo lectura | Cómo va a verse el sitio en la lista: el dominio arriba y **`site101125`** debajo |

#### Lo que dice el panel, y por qué importa

> *«**No other domain is connected to this website yet.** Add a domain first,
> then you can set it as the main domain»*
>
> *«Changes the **main domain** of this website — the primary name shown
> throughout our Control panel (site lists, breadcrumbs and top header). **Pick
> from any domain currently connected to this site**»*

**O sea: `Rename` no inventa una dirección nueva — escoge entre las que ya
tiene.** Y nuestro sitio tiene una sola, el subdominio gratuito. Por eso el
campo está gris: no hay entre qué escoger.

> **Esto confirma lo que dijimos al bautizar el sitio:** la dirección
> `apifacturas-carloscastro.runasp.net` **no se cambia**. Para tener otra hay
> que **conectar otro dominio** — que es de pago — o **borrar el sitio y crear
> otro**. Por eso valía la pena pensar el nombre diez segundos.

#### Cuidado: `Rename` aquí y `Rename` en la base de datos no son lo mismo

**Es la misma palabra para dos cosas distintas**, y están a dos clics una de
otra en el mismo panel:

| | En **Databases** | En **Websites** |
|---|---|---|
| Qué cambia | El **alias**: el nombre amable con que usted ve la base | El **dominio principal**, entre los ya conectados |
| ¿Sirvió? | **Sí** — le pusimos `bdfacturas` a `db75266` | **No** — no hay otro dominio |
| Lo de abajo | `db75266` sigue siendo el nombre real | `site101125` sigue siendo el identificador |

> **Y en los dos casos pasa lo mismo por debajo:** lo que usted cambia es **la
> etiqueta**, no el nombre con el que el servidor conoce la cosa. El nombre
> real —`db75266`, `site101125`— **no se toca nunca**, y es el que va en las
> conexiones y en los comandos.

#### Sí use la `Description`

Es un texto libre que **solo se ve en este panel** — no sale a internet. El
propio campo sugiere `production, client XY, staging`.

**Para el proyecto de aula sirve de verdad:** con cinco sitios permitidos, la
lista se llena rápido de nombres parecidos. **Escriba tres cosas: qué es, de
quién es y de qué curso es.**

| | Qué escribir |
|---|---|
| **En este tutorial** | `API facturas v4 - demostracion de clase - AySW 2026-2` |
| **Un equipo, en su API** | `API facturas v4 - equipo 3 - AySW 2026-2` |
| **Un equipo, en su front** | `Front facturas v4 - equipo 3 - AySW 2026-2` |

> **Por qué esas tres cosas.** Dentro de un mes, la lista va a tener la API y el
> front de varios equipos, con direcciones parecidas. `API` o `Front` dice
> **cuál de los dos es**, el equipo dice **de quién**, y el semestre dice **si
> todavía sirve o se puede borrar**. Sin eso toca abrir cada uno para saberlo.

![El diálogo con la descripción escrita y Show more abierto](docs/tutorial_publicar/img/33_renombrar_detalle.png)

**Mientras escribe, dos cosas se mueven solas:** el contador —`53/200`— y el
recuadro **`PREVIEW IN LISTS`**, que ahora muestra la descripción debajo del
dominio, junto al identificador.

> **Y la vista previa delata algo:** la descripción aparece **cortada**. Se lee
> `API facturas v4 - demostracion de clase - AySW 20` — el `26-2` se perdió. En
> la lista de sitios el espacio es el que es, y se corta por donde toque.
>
> **De ahí una regla práctica:** caben unos **45 caracteres** a la vista. Ponga
> **lo que lo distingue al principio**. Los 200 no son para escribir un párrafo:
> son para que quepa lo que usted quiera, sabiendo que en la lista solo se ve el
> comienzo.

**Oprima el `Save` azul** del renglón de `Description`, y después `Close`.

> **Ojo con los dos `Save`.** Hay uno por campo, no uno para todo el diálogo. El
> de arriba guarda el dominio —y está pálido porque no hay nada que guardar—;
> el de abajo guarda la descripción.

#### `Show more`: la letra menuda, que esta vez sí importa

Debajo de `About website renaming` hay un **`Show more`**. Ábralo: son cuatro
renglones y **uno de ellos tranquiliza de verdad**.

| Lo que dice el panel | Qué significa |
|---|---|
| *«Only domains already pointed to this website are selectable — **the Site ID itself never changes**»* | Confirma lo de arriba: `site101125` **es para siempre** |
| *«Purely a Control panel / primary-domain change — **does not affect FTP/WebDeploy, DNS records, SSL certificates or any technical configuration**»* | **Éste es el importante** — ver abajo |
| *«Need a domain that is not listed? **Add it first in the website Domains section**, then it becomes selectable here»* | El camino para tener otra dirección: primero conectar el dominio en `Domains` |
| *«The Description field is for your own notes or tags — **also shown in the Control panel only**»* | La descripción **no sale a internet** |

> **Por qué el segundo renglón importa tanto.** Renombrar **no toca las
> credenciales de WebDeploy**, ni el certificado, ni nada técnico. Es solo la
> etiqueta del panel.
>
> **Y eso marca la diferencia con borrar.** Borrar el sitio **sí** cambia el
> identificador y la clave — por eso el sitio nuevo quedó con `site101125` y no
> con el número del anterior. Renombrar es cosmético; borrar es empezar de
> cero.
>
> **En la práctica:** si un equipo quiere «cambiarle el nombre» a su sitio,
> puede hacerlo sin miedo — pero sepa que lo único que cambia es cómo lo ve
> él mismo en el panel. **La dirección pública sigue siendo la misma.**

#### Guardado, así queda en la lista

<!-- PENDIENTE DE CAPTURA: la lista de sitios con la descripción ya guardada -->

Al cerrar el diálogo, el renglón del sitio trae ahora **cuatro etiquetas**
debajo del dominio:

```
apifacturas-carloscastro.runasp.net
API facturas v4 - demostracion de clase - AySW 20   Free   EU   site101125
```

**Y se cortó donde la vista previa anunciaba:** `AySW 20`, sin el `26-2`. No fue
sorpresa — la pantalla anterior ya lo había mostrado. **Por eso vale mirar la
vista previa antes de guardar**, en vez de descubrirlo en la lista.

> **La descripción también se busca.** El campo `Search` de arriba dice *«Search
> by name, description or domain alias»*. Con cinco sitios descritos, escribir
> `equipo 3` los encuentra. Es la razón práctica para llenar ese campo.

#### De paso: el menú del sitio

Al entrar a un sitio, el menú de la izquierda **cambia**: ya no son los
servicios de la cuenta, son **las secciones de este sitio**. Dos de ellas son
las que vamos a usar en seguida:

| Sección | Para qué |
|---|---|
| **`Deploy`** · *FTP / Visual Studio* | **Aquí están las credenciales de publicación.** Es la siguiente parada |
| **`Logs`** · *Error and access logs* | **Cuando la API publicada falle, la respuesta está aquí.** No en su máquina |
| `Overview` | La vista general del sitio |
| `Files` | El administrador de archivos: ver qué quedó subido |
| `Domains` · *Aliases / Bindings* | Conectar otro dominio — lo que `Rename` pedía |
| `HTTPS` · *SSL certificate* | El certificado. Con el subdominio gratuito viene puesto |
| `Git deploy` | Publicar desde GitHub, en vez de WebDeploy |
| `AppInstaller` | El catálogo de aplicaciones del que hablaba el paso anterior |
| `Process` · *Process list* | Ver si el proceso está vivo — útil con los arranques en frío |
| `Resource` | Cuánta memoria de los 256 MB se está usando |


### 5.8 · `Overview`: el tablero del sitio

![La vista general del sitio](docs/tutorial_publicar/img/35_sitio_overview.png)

Es la pantalla más cargada del panel. **Se lee en tres columnas**, y casi todo
es información — muy poco hay que tocar.

#### Columna izquierda: qué es este sitio

| Renglón | Valor | Qué dice |
|---|---|---|
| **ID** | `site101125` | El identificador, otra vez. **Es el usuario de publicación** |
| **Status** | `Enabled` | Encendido |
| **Server** | `site101125.siteasp.net` · `EU` · `W14` | **El servidor físico** donde vive. `W14` es la máquina. *«Always use this server name instead of IP address»* |
| **.NET** | `.NET 10.x/9.x/8.x/7.x/6.x/5.x/Core` **`[x86]`** | **Todas las versiones están disponibles**, nuestra v10 incluida. Y corre en **32 bits** — ver abajo |
| **PHP** | `--- [NONE]` | Apagado. **Aquí solo corre .NET**, y está bien así |
| **Memory limit** | `256 MB RAM` | Lo del plan gratuito |
| **File count** | `0 files` de `100000 files` | **Hay un tope de archivos**, no solo de megas |
| **DISK QUOTA** | `0 MB` · `0% used` de `5 GB` | Vacío, como se esperaba |

> **Lo de `[x86]` vale anotarlo.** El sitio corre en **32 bits**. Para una API
> normal no cambia nada — .NET la compila a código intermedio y el servidor la
> ejecuta igual. **Importa solo si alguien publica con `win-x64`** o usa una
> biblioteca nativa de 64 bits: ahí falla, y el mensaje no dice por qué.
> Nosotros vamos a publicar **sin fijar plataforma**, que es lo que evita el
> problema.

> **Y el tope de 100.000 archivos tampoco es decorativo.** Una API publicada
> tiene unas pocas decenas de archivos; un `node_modules` tiene decenas de
> miles. Si alguna vez sube un front de Angular o React, **suba lo compilado**,
> no la carpeta de trabajo.

#### Columna del medio: las credenciales

**Dos pestañas, y son dos formas distintas de subir lo mismo:**

| Pestaña | Para qué |
|---|---|
| **`FTP / SFTP access`** | Copiar archivos a mano, con un programa como FileZilla |
| **`Visual Studio access`** | **WebDeploy.** Es la que vamos a usar — publica desde la línea de comandos o desde Visual Studio |

De la pestaña de FTP, que es la que está abierta, lo que importa:

| Dato | Valor |
|---|---|
| **Server** | `site101125.siteasp.net` |
| **Port** | `21` FTP · `22` SFTP/SCP |
| **Login** | `site101125` |
| **Password** | Oculta, con **`Show`** para verla y **`Change`** para cambiarla |
| **Website root** | **`\wwwroot`** — **la carpeta donde va la aplicación** |
| **WebFTP** | Un administrador de archivos por navegador, **sin instalar nada** |
| **`Test connection`** | Comprueba que el servidor responde y que la clave sirve, **sin salir del panel** |

> **`\wwwroot` es el destino.** Cuando el comando de publicar diga a dónde va
> el código, va a ser ahí. Y es la misma carpeta que el ejemplo habría ocupado
> si lo hubiéramos instalado.

#### Columna derecha: cómo está el sitio ahora

| Bloque | Qué muestra |
|---|---|
| **La miniatura** | La página que responde hoy: *«Your website is ready…»*. **Es la página de sitio vacío** que anunciamos — la prueba de que el sitio funciona y lo que falta es nuestro código |
| **DNS status** | **`Domain pointing correctly`**, en verde: `apifacturas-carloscastro.runasp.net → 5.9.179.193`. El nombre ya apunta al servidor |
| **QUICK ACTIONS** | **`Start`** · **`Stop`** · **`Restart`** |
| **QUICK STATISTICS** | `Site uptime 7m`, `AppPool uptime 7m`, peticiones `GET` / `POST` en `0`, `404 errors 0`, `Transferred 0 MB` |

> **El `DNS status` es el que estaba en naranja hace un rato.** Mientras el
> sitio se creaba, el nombre todavía no apuntaba a ninguna parte y el panel lo
> marcaba como pendiente. **Ya se resolvió solo**, y por eso ahora está verde.
> No había nada que hacer: solo esperar.

> **`Restart` va a hacer falta.** Si publica una versión nueva y el sitio sigue
> respondiendo como antes, es que el proceso viejo sigue vivo. `Restart` lo
> reinicia sin borrar nada.

> **Y `QUICK STATISTICS` es la respuesta a «¿llegó mi petición?».** Si un
> estudiante dice que la API no responde, aquí se ve si el servidor recibió
> algo:
>
> | Lo que se ve | Qué quiere decir |
> |---|---|
> | `GET requests` sigue en `0` | **La petición nunca llegó.** El problema está en la dirección o en el cliente |
> | Suben las peticiones y suben los `404 errors` | **Llegó, pero a una ruta que no existe** |
> | `AppPool uptime` se reinicia solo | **Es el arranque en frío**, no un error |
> | `AppPool crashed` mayor que `0` | **La aplicación se está cayendo.** A `Logs` |

#### Abajo: los atajos

La fila de íconos —`Files`, `Domains`, `HTTPS`, `Deploy`, `Git`, `Logs`…— es
**el mismo menú de la izquierda, en grande**. No hay nada nuevo ahí.

**Siga a la pestaña `Visual Studio access`**, que es donde están los datos de
WebDeploy.


### 5.9 · `Visual Studio access`: WebDeploy viene apagado

![La pestaña de WebDeploy, con el interruptor en Disabled](docs/tutorial_publicar/img/36_webdeploy_apagado.png)

**Y aquí pasa lo mismo que con la base de datos:** la pestaña existe, pero
**está en `Disabled`**. Donde debería estar el servidor se lee
`---- Disabled ----`, el `Show` de la contraseña está gris y
`Test connection` también.

#### Lo que dice el panel, que es exacto

> *«WebDeploy is switched off — **no publishing account exists on the server**.
> Switch it to **Enabled** to get the server address, password and publish
> profile»*

**Léalo con cuidado, porque no dice lo que uno supone.** No es que la clave
esté escondida: **es que todavía no existe.** La cuenta de publicación **se
crea en el momento de encender el interruptor**.

> **Por eso no servía buscarla antes.** En el Paso 5.6 dijimos que al crear el
> sitio se generaban las credenciales de WebDeploy. Esta pantalla lo corrige:
> el sitio se creó, sí, pero **la cuenta de publicación no — esa la crea
> usted**, aquí, cuando la pida.

#### Lo único que sí se ve estando apagado

| Dato | Valor | Qué es |
|---|---|---|
| **Port** | **`8172`** | **El puerto de WebDeploy**, no el 21 del FTP ni el 443 de la web. Es fijo y no cambia |
| **Site** | `site101125` | **El nombre del sitio dentro de IIS.** El comando de publicar lo pide aparte |
| **Login** | `site101125` | El usuario |
| **Server** | `---- Disabled ----` | **Lo que falta.** Aparece al encender |
| **Password** | `*****`, con `Show` **gris** | Tampoco existe todavía |

> **`Site` y `Login` se parecen tanto que confunden**, y son dos cosas: `Site`
> es **cuál aplicación** del servidor se está actualizando, y `Login` es
> **quién** la actualiza. Que tengan el mismo valor es casualidad del
> hospedaje, no una regla.

#### Qué es WebDeploy, en una frase

> *«Best deployment solution for **IIS** — built by Microsoft specifically for
> publishing .NET applications to Windows hosting»*

| | FTP | **WebDeploy** |
|---|---|---|
| Qué hace | **Copia archivos**, uno por uno | **Sincroniza la aplicación**: compara lo que hay con lo que usted manda |
| Archivos que no cambiaron | Se vuelven a subir | **Se saltan** |
| Archivos que sobran | Quedan ahí | **Los borra**, si se le pide |
| El proceso en marcha | Hay que pararlo a mano | **Lo detiene y lo reanuda solo** |

> **Por eso usamos WebDeploy y no FTP.** Publicar la segunda versión de la API
> por FTP es volver a subirlo todo y rezar para que no quede un archivo viejo
> suelto. Con WebDeploy, **lo que queda arriba es exactamente lo que usted
> publicó.**

#### Enciéndalo

**Oprima `Enabled`.**

Al hacerlo aparecen **los dos datos que faltan** — el servidor y la
contraseña— y se habilitan dos botones:

| Botón | Para qué |
|---|---|
| **`Test connection`** | Comprueba que el servidor de WebDeploy responde **y que esas credenciales entran**. Útil antes de pelear con el comando |
| **`Download publish profile`** | Baja un archivo **`.PublishSettings`** con todo lo anterior adentro. Visual Studio lo importa y queda configurado |

> **Y una diferencia con el acceso remoto de la base de datos, que conviene no
> mezclar:**
>
> | | `Remote access` de la base | **WebDeploy** |
> |---|---|---|
> | Qué expone | **Los datos** | **La publicación** |
> | ¿La aplicación lo necesita? | **No** — ella usa `Local access` | **No** — solo se usa al publicar |
> | Cuándo se usa | Cuando usted entra con SSMS | Cada vez que sube una versión nueva |
>
> **Los dos se pueden apagar cuando no se estén usando**, y ninguno de los dos
> le hace falta al sitio para responder. La diferencia práctica es que a
> WebDeploy se vuelve seguido — cada publicación—, así que en clase se deja
> encendido mientras dure el trabajo.


### 5.10 · Encendido: los tres datos para publicar

![La pestaña de WebDeploy encendida, con el servidor y la contraseña](docs/tutorial_publicar/img/37_webdeploy_encendido.png)

Al oprimir `Enabled`, el `---- Disabled ----` se convirtió en un nombre de
servidor, apareció la contraseña y **se encendieron los dos botones** que
estaban grises.

#### La ficha completa

| Dato | Valor | De dónde sale |
|---|---|---|
| **Server** | `site101125.siteasp.net` | Lo acaba de crear el hospedaje |
| **Port** | `8172` | Fijo, el de WebDeploy |
| **Site** | `site101125` | **Cuál** aplicación se actualiza |
| **Login** | `site101125` | **Quién** la actualiza |
| **Password** | `c+3LK6h#-iM4` | **Recién generada.** Se ve con `Show` y se cambia con `Change` |

> **Anótelos los cinco.** El comando de publicar los pide todos, y si uno sale
> mal el mensaje de error **no dice cuál**.

> **Y fíjese en el nombre del servidor:** `site101125.siteasp.net`, con
> **`siteasp.net`**. No es `runasp.net`, que es donde la gente visita el sitio,
> ni `databaseasp.net`, que es la base de datos. **Tres dominios parecidos para
> tres cosas distintas**, y el panel usa los tres en la misma pantalla:
>
> | Dominio | Para qué |
> |---|---|
> | `apifacturas-carloscastro.`**`runasp.net`** | **Visitar** el sitio |
> | `site101125.`**`siteasp.net`** | **Publicar** en el sitio |
> | `db75266.`**`databaseasp.net`** | **La base de datos** |

#### Compruébelo aquí mismo

**Oprima `Test connection`** en el recuadro `WEBDEPLOY CHECK`: *«Checks that the
WebDeploy server is reachable and that these credentials can sign in»*.

> **Hacer esta prueba ahora ahorra media hora después.** Si el comando de
> publicar falla, lo primero que uno sospecha es la clave. Con esta prueba hecha
> **ya sabe que la clave sirve**, y puede buscar el error donde sí está: en el
> comando.

#### `Download publish profile`

Baja un archivo **`.PublishSettings`** — un XML con el servidor, el sitio, el
usuario y la contraseña adentro.

| Si usa | Haga |
|---|---|
| **Visual Studio** | Bájelo. `Publish` → `Import Profile`, y queda configurado sin escribir nada |
| **La línea de comandos** | **No lo necesita.** Vamos a pasar los datos en el comando |

> **Y ese archivo es un secreto completo.** Trae la contraseña en texto plano.
> **No va al repositorio.** Si lo baja, déjelo fuera de la carpeta del proyecto
> — o agréguelo al `.gitignore` antes de que se le cuele.

#### Sobre las contraseñas de aquí en adelante

En este tutorial **las contraseñas van completas**, en las capturas y en el
texto. Es deliberado: un tutorial con `SU_CONTRASENA` en los comandos obliga a
adivinar dónde va cada cosa, **y es justo ahí donde la gente se equivoca** —
como vamos a ver en el paso 6.1, donde confundimos dos claves.

| | |
|---|---|
| **Aquí se ven** | Para que se entienda **cuál valor va en cuál hueco** |
| **La suya va a ser otra** | Cada cuenta genera las propias. **No copie éstas** — copie el **lugar** de donde se sacan |
| **Y éstas se cambian al final** | Con el `Change` de cada pantalla. Son de una demostración, no de un sistema en uso |

> **Y al terminar la clase, estas dos se cambian:** la de WebDeploy, con el
> `Change` de esta pantalla, y la de la base de datos, con el suyo. **Quedaron
> en capturas de un repositorio público.**


### 5.11 · La prueba de conexión, antes de salir de aquí

![WEBDEPLOY CHECK en verde: OK, Connected in 79 ms](docs/tutorial_publicar/img/38_webdeploy_prueba.png)

```
OK   Connected in 79 ms
```

| Lo que cambió | Qué significa |
|---|---|
| El recuadro se puso **verde** | El servidor de WebDeploy **responde** |
| **`OK`** | Y estas credenciales **entran** |
| `Connected in 79 ms` | Lo que tardó el saludo |
| El botón dice ahora **`Test again`** | Se puede repetir cuando quiera |

> **Esto no es un adorno: es una línea de corte.** De aquí en adelante, si algo
> falla al publicar, **ya sabemos que no son las credenciales ni el servidor**.
> El problema va a estar del lado de acá — en el proyecto, en el comando o en
> la red de su máquina— y se busca ahí.

> **Los 79 ms también dicen algo:** el servidor está en Europa y contestó en
> menos de una décima de segundo. Si un día esa prueba tarda mucho o falla
> intermitente, es la red, no el hospedaje.

#### Y el `Show more` confirma por qué WebDeploy y no FTP

<!-- PENDIENTE DE CAPTURA: esta misma pantalla con el Show more abierto -->

Debajo de `When to use Visual Studio WebDeploy` hay un **`Show more`**. Abierto,
dice **en dos renglones** lo mismo que la tabla del paso anterior — con las
palabras del hospedaje:

> *«**Incremental upload** — only changed files are transferred, so
> subsequent deploys are quick»*
>
> *«Site is **briefly stopped** during publish and **restarted automatically**
> — no manual AppPool recycle needed»*

| Lo que dice | Qué significa para usted |
|---|---|
| **Sube solo lo que cambió** | La primera publicación sube todo; **las siguientes son cuestión de segundos**. Por FTP, cada una es como la primera |
| **Para y reanuda el sitio solo** | **No hay que oprimir `Restart`** después de publicar. Lo hace él |

> **Y eso último corrige a medias algo que dijimos:** en el paso 5.8 anotamos
> `Restart` por si el sitio sigue respondiendo como antes. Con WebDeploy **no
> debería hacer falta** — él reinicia. `Restart` queda como recurso para
> cuando algo se queda pegado, no como parte del procedimiento.

#### Los tres pasos que sugiere el panel — y por qué no son los nuestros

El recuadro `GET STARTED` propone:

| | El panel dice | Nosotros |
|---|---|---|
| 1 | *Click **Download publish profile*** | **No** |
| 2 | *In Visual Studio: right-click your project → **Publish** → Import profile* | **No** |
| 3 | *Hit **Publish*** | **No** |

**Ese camino es para Visual Studio**, con clic derecho y ventanas. **Nosotros
vamos por la línea de comandos**, con los cinco datos que ya anotamos.

> **No es por purismo.** Es por tres razones que importan en un curso:
>
> | | |
> |---|---|
> | **Se ve lo que pasa** | Un comando dice **qué** se manda, **a dónde** y **con qué usuario**. Un botón esconde las tres cosas |
> | **Funciona igual en cualquier parte** | Visual Studio Code, Rider, una terminal pelada o un servidor de integración continua |
> | **Se puede repetir y automatizar** | Un comando se guarda, se comparte y se corre otra vez. Un clic derecho no |

**Ya tenemos todo lo del hospedaje.** Lo que sigue ya no es el panel: es el
proyecto.

## Paso 6 — Publicar la API

**Tres cosas, en este orden:**

| | Qué | Dónde |
|---|---|---|
| **6.1** | Decirle a la API **a cuál base de datos conectarse al publicarse** | Un archivo del proyecto |
| **6.2** | **Compilar** para producción | `dotnet publish` |
| **6.3** | **Subirla** | `msdeploy` |

### 6.1 · La configuración de producción

#### El problema

La API sabe conectarse a la base de datos **de su máquina**:

```json
"SqlServer": "Server=localhost,11466; Database=bdfacturas_sqlserver_local; User Id=sa; ..."
```

Eso está en `api_facturas/appsettings.json`, apunta al contenedor de Docker, y
**está bien que siga así** — es lo que usa cuando trabaja en su computador.

**Publicada necesita otra cadena.** Pero no vamos a cambiar ese archivo: si lo
cambia, deja de poder trabajar local, y tarde o temprano sube a GitHub una
contraseña del hospedaje.

#### La solución: un archivo que solo existe publicado

ASP.NET Core lee **dos archivos de configuración, uno encima del otro**:

| Orden | Archivo | Cuándo manda |
|---|---|---|
| 1º | `appsettings.json` | **Siempre.** Es la base |
| 2º | `appsettings.<Entorno>.json` | **Si el entorno coincide.** Lo que repita, **gana** |

**¿Y cuál es el entorno?** Lo dice la variable `ASPNETCORE_ENVIRONMENT`. En su
máquina vale `Development`; **en el hospedaje no está puesta, y cuando no está
puesta el valor por defecto es `Production`**.

> **De ahí sale el truco, y no es un truco:** un archivo llamado
> **`appsettings.Production.json`** **se ignora en su computador y manda en el
> hospedaje**, sin tocar nada más y sin un solo `if` en el código.

#### De dónde sale ese nombre tan raro

**No es un nombre: es una fórmula que el framework arma y busca solo.**

```
appsettings  .  Production  .  json
     |              |
  fijo: lo que   EL ENTORNO: tiene que coincidir
  busca .NET     con ASPNETCORE_ENVIRONMENT
```

| Pieza | De dónde sale |
|---|---|
| **`appsettings`** | **Fijo.** Es el nombre que ASP.NET Core busca al arrancar |
| **`Production`** | **El valor de la variable `ASPNETCORE_ENVIRONMENT`.** El framework arma el nombre con ella: `appsettings.{lo-que-diga-la-variable}.json` |
| **`.json`** | El formato |

**Y `Production` es el valor por defecto**, que es justo lo que lo hace útil:

| Dónde | `ASPNETCORE_ENVIRONMENT` | Archivo que gana |
|---|---|---|
| Su máquina, con `dotnet run` | `Development` | `appsettings.Development.json` |
| **El hospedaje** | **Nadie la fija** → **`Production`** | **`appsettings.Production.json`** |

> **No configuramos nada para que esto funcione.** Aprovechamos que **cuando la
> variable no está, .NET asume `Production`** — y el hospedaje no la pone.

**Los tres nombres de la convención son `Development`, `Staging` y
`Production`.** No son palabras mágicas del lenguaje: son los tres valores que
Microsoft estandarizó, y el framework trae ayudas para ellos — como
`app.Environment.IsDevelopment()`.

> **Podría inventarse otro.** Un archivo `appsettings.Hospedaje.json`
> funcionaría **si** usted pusiera `ASPNETCORE_ENVIRONMENT=Hospedaje` en el
> servidor. Con `Production` no hay que configurar nada — **un paso menos que
> puede fallar.**

**Dos trampas del nombre:**

| | |
|---|---|
| **La `P` mayúscula** | Tiene que coincidir **exactamente** con el valor de la variable. En Windows da igual; **en Linux, `appsettings.production.json` no se carga**, y el error es mudo |
| **No lo menciona ningún código** | Búsquelo en `Program.cs`: no está. **Lo carga `WebApplication.CreateBuilder`**, que registra los dos archivos antes de que su código corra. Por eso funciona «solo» |

#### El archivo: créelo usted

**En `api_facturas`, al lado de `appsettings.json`, cree un archivo nuevo
llamado:**

```
appsettings.Production.json
```

**Con VS Code:** clic derecho sobre la carpeta `api_facturas` → *New File* →
ese nombre, **exactamente así** — con la `P` mayúscula y los dos puntos del
`.json`. Si lo escribe `appsettings.production.json`, en Windows funciona y
**en el servidor también**, pero deja de funcionar el día que el proyecto corra
en Linux. **Escríbalo bien desde el principio.**

**En este repositorio hay dos archivos, y conviene entender para qué sirve cada
uno:**

| Archivo | Qué es | ¿Va a GitHub? |
|---|---|---|
| `api_facturas/appsettings.Production.json`**`.ejemplo`** | **La plantilla.** Viene en el repositorio, con `SU_CONTRASENA` donde va la clave | **Sí** |
| `api_facturas/appsettings.Production.json` | **El que usted crea**, con su contraseña | **No** |

> **El atajo:** copie el `.ejemplo`, péguelo al lado y quítele el `.ejemplo` del
> nombre. Así no tiene que escribir nada — solo reemplazar la contraseña.

#### Cómo queda el archivo, completo

**Éste es el contenido entero**, tal como está en el proyecto — con los
comentarios y todo:

```json
{
  "_comentario": "ESTE ARCHIVO NO SE VERSIONA -esta en .gitignore- porque trae la contrasena real de la base de datos del hospedaje. La plantilla que si se versiona es appsettings.Production.json.ejemplo, al lado. Si clona el repositorio en otra maquina, copie la plantilla y vuelva a poner la contrasena.",

  "_por_que_Production": "ASP.NET Core lee appsettings.json y encima appsettings.<Entorno>.json. En el hospedaje no se fija ASPNETCORE_ENVIRONMENT, y cuando no se fija el entorno es Production: por eso este archivo gana sobre el otro, SOLO publicado. En su maquina sigue mandando appsettings.json, que apunta a Docker.",

  "Motor": "sqlserver",

  "ConnectionStrings": {
    "_comentario": "Es la cadena de Local access, no la de Remote access. La API vive en el mismo centro de datos que la base de datos, asi que no sale a internet: por eso el servidor NO lleva .public y por eso Encrypt=False.",
    "SqlServer": "Server=db75266.databaseasp.net; Database=db75266; User Id=db75266; Password=Cd3?2F#iYm8_; Encrypt=False; MultipleActiveResultSets=True;"
  }
}
```

#### Renglón por renglón

| Clave | ¿La usa la API? | Para qué está |
|---|---|---|
| **`_comentario`** | **No** | Explica **qué es este archivo** y por qué no se versiona |
| **`_por_que_Production`** | **No** | Explica **por qué se llama así**, que es lo que nadie recuerda después |
| **`Motor`** | **Sí** | La v4 habla con **dos motores** —SQL Server y PostgreSQL— y esta clave escoge. En el hospedaje solo tenemos SQL Server |
| **`ConnectionStrings._comentario`** | **No** | Recuerda **cuál de las dos cadenas** es ésta |
| **`ConnectionStrings.SqlServer`** | **Sí** | La cadena de **`Local access`**, copiada del panel. **No la de `Remote access`** |

> **¿Por qué hay claves que empiezan por guión bajo?** **Porque JSON no tiene
> comentarios.** No existe el `//` ni el `/* */`: si los escribe, el archivo
> deja de ser JSON válido.
>
> **La salida es una clave de mentiras.** `_comentario` es una clave como
> cualquier otra, pero **ninguna parte del código la busca** — la API pide
> `Motor` y `ConnectionStrings:SqlServer`, y lo que sobre lo ignora. El guión
> bajo al principio es la señal, para quien lea, de que **eso es para leer, no
> para funcionar**.
>
> **Y es la convención de todo este proyecto**, no un invento de esta página:
> abra `api_facturas/appsettings.json` y va a ver los mismos `_comentario`.

> **Que un archivo de configuración se explique a sí mismo no es adorno.** Este
> archivo **no está en GitHub**: no tiene historial, no tiene mensaje de commit
> y nadie va a poder preguntarle a `git log` por qué existe. **Lo único que va
> a quedar es lo que diga adentro.**

#### ¿Por qué la del `Local access` y no la del `Remote access`?

**Porque la API no es un cliente de afuera: vive adentro.** Y esta vez no hay
que deducirlo — **el panel lo dice con todas las letras**, en la pestaña
`Local access`:

> *«Use these details in the connection string of **a website hosted with us**.
> The connection **stays inside our datacenter network** — fast, and **not
> reachable from outside**»*
>
> *«Direct connection **from your hosted websites** to the MSSQL server — both
> run inside our datacenter network. **Only usable from applications running on
> your hosted websites**, not from your local computer or external services»*

**Leído al revés, eso también explica otra cosa:** por eso, cuando usted quiso
entrar con SSMS **desde su computador**, tuvo que encender el `Remote access`.
La cadena local **no le habría servido** — no porque la clave estuviera mal,
sino porque esa puerta **no existe desde afuera**.

La base de datos está en Europa, el sitio está en Europa, y los dos cuelgan de
la misma red del centro de datos. Para hablarse **no necesitan salir a
internet** — y si no salen, no tiene sentido tratarlos como si salieran.

| | `Local access` **← la API** | `Remote access` |
|---|---|---|
| **Quién la usa** | **Lo que corre dentro del hospedaje**: su API publicada | **Lo que corre fuera**: SSMS, Azure Data Studio, Visual Studio en su casa |
| **Por dónde va** | Por la red interna del centro de datos | **Por internet abierto** |
| **Velocidad** | **La más rápida.** Cada consulta cuenta | Depende de su conexión |
| **Cifrado** | `Encrypt=False` — no sale a ninguna parte, no hay nada que proteger en el camino | `Encrypt=True`, obligatorio |
| **¿Se puede apagar?** | **No existe el interruptor**: siempre está | **Sí, y viene apagado** |

**Y de ahí salen las tres razones de peso:**

| | |
|---|---|
| **1. No depende de un interruptor** | Si un día apaga el `Remote access` —que es lo recomendable cuando no lo esté usando— **la API publicada sigue funcionando**. Con la cadena remota, se le caería el sistema sin saber por qué |
| **2. Es más rápida** | Una pantalla de facturas hace varias consultas. Por dentro son milisegundos; por internet, cada una paga el viaje |
| **3. No expone nada de más** | La cadena remota es un **punto público**. Si esa cadena se filtra, cualquiera en el mundo puede intentar entrar a sus datos. La interna **no sirve desde afuera** aunque la publiquen |

> **Y hay una trampa concreta con la remota**, que el propio panel advierte:
> *«Repeated failed logins get an IP **blocked automatically**»*. Una API con la
> clave mal puesta reintenta sola, muchas veces — y puede terminar **bloqueando
> la IP del servidor**.

> **La regla, en una frase:** `Remote access` es **para usted**, cuando se
> conecta con SSMS desde su computador. `Local access` es **para la aplicación
> publicada**. Y por eso el `Remote access` se puede volver a apagar cuando
> termine — **la API no lo necesita**.

#### De dónde sale esa contraseña — y cuál NO es

![La pestaña de la base de datos con la contraseña a la vista](docs/tutorial_publicar/img/39_clave_base_datos.png)

En el panel: **`Databases` → `bdfacturas` → `Password` → `Show`**, y hay un
botón de copiar al lado.

> **Aquí nos equivocamos en vivo, y vale la pena dejarlo escrito.** Al llenar
> este archivo pegé **la contraseña de WebDeploy** — la de la pantalla
> anterior. **No es ésa.** A esa altura hay **dos contraseñas distintas
> abiertas en el mismo navegador**, y se parecen hasta en la forma:
>
> | Contraseña | Dónde vive | Para qué sirve |
> |---|---|---|
> | `c+3LK6h#-iM4` | `Websites` → `Visual Studio access` | **Subir el código** |
> | `Cd3?2F#iYm8_` | `Databases` → `bdfacturas` | **Leer y escribir los datos** |
>
> **Y el error no avisa a tiempo.** Con la clave equivocada, `msdeploy` publica
> perfecto — porque esa clave no se usa al publicar— y la API arranca bien.
> Falla **en la primera petición que toque la base de datos**, con un error de
> inicio de sesión. Uno busca el problema en el comando de publicar, que es
> donde no está.

#### Y una que confunde todavía más: `Local` y `Remote` comparten contraseña

**La misma clave sirve en las dos pestañas.** Si abre `Local access` o
`Remote access`, el campo `Password` muestra **el mismo valor** — como ya
dijimos en el paso 3.6: lo único que cambia entre las dos es **el nombre del
servidor**.

| | `Local access` | `Remote access` |
|---|---|---|
| **Server** | `db75266.databaseasp.net` | `db75266.`**`public`**`.databaseasp.net` |
| **Login** | `db75266` | `db75266` |
| **Password** | `Cd3?2F#iYm8_` | **la misma** |
| **Encrypt** | `False` | `True` + `TrustServerCertificate=True` |

> **Por eso la pregunta «¿no es la del remoto?» tiene una respuesta tranquila:
> sí, y también la del local — es una sola.** La contraseña es **del usuario
> `db75266`**, no de la puerta por donde entra. Lo que usted escoge al armar la
> cadena no es qué clave usar, sino **por dónde entrar**: y nosotros entramos
> por dentro, sin `.public`.

#### Cómo queda el renglón, entonces

```
"SqlServer": "Server=db75266.databaseasp.net; Database=db75266; User Id=db75266; Password=Cd3?2F#iYm8_; Encrypt=False; MultipleActiveResultSets=True;"
```

**Compruébelo contra el panel antes de seguir.** En la pestaña `Local access`,
el recuadro `CONNECTION STRING` tiene un botón **`Show with password`**: ahí
sale la cadena entera, armada por el hospedaje. **Si la suya no coincide, la
que manda es la del panel.**

> **Tres descuidos que cuestan media hora cada uno:**
>
> | | |
> |---|---|
> | **Copiar con el ratón** | Use el botón de copiar. Un espacio de más al final **no se ve**, pero el servidor lo cuenta |
> | **Borrar el `;` del final** | Separa los parámetros de la cadena |
> | **Ponerle comillas a la contraseña** | Va dentro de la cadena, no entre comillas propias |

#### Y póngalo en el `.gitignore`, antes de hacer el primer commit

**Porque trae una contraseña de verdad.** La plantilla sí va — así cualquiera
sabe qué archivo hace falta y qué forma tiene— pero el que tiene el valor
puesto **se queda en su disco**.

**Abra el `.gitignore` de la raíz del proyecto y agregue al final:**

```
# La configuracion de PRODUCCION trae la contrasena real de la base de
# datos del hospedaje. Se versiona la plantilla, nunca el archivo con el
# valor puesto.
appsettings.Production.json
```

**Compruébelo**, que es un comando y no una creencia:

```powershell
git status
```

**El archivo NO debe aparecer en la lista.** Si aparece, el `.gitignore` está
mal escrito o el archivo ya se había agregado antes — y entonces hay que
sacarlo con `git rm --cached api_facturas/appsettings.Production.json`.

> **Es el mismo patrón de toda la industria**, y vale la pena nombrarlo: **se
> versiona la forma, no el secreto.** Un compañero que clone el repositorio ve
> la plantilla, sabe que necesita su propia contraseña, y la pone. Lo que nunca
> estuvo en el historial **no se puede filtrar**.

> **Y para el proyecto de aula, el escalón siguiente:** en vez de un archivo,
> **variables de entorno**. ASP.NET Core las lee con el mismo nombre de la
> clave, cambiando los dos puntos por dos guiones bajos —
> `ConnectionStrings__SqlServer`—, que es exactamente lo que hace el
> `docker-compose.yml` de este proyecto. **No hay archivo que se pueda colar.**


### 6.2 · Compilar para producción: `dotnet publish`

#### Qué vamos a hacer, y por qué no basta con `dotnet run`

Lo que usted corre todos los días —`dotnet run`, o el `docker compose up`—
**compila y arranca en el mismo gesto**, y deja los resultados regados en
`bin/` y `obj/` junto con cosas que solo sirven para depurar.

**El hospedaje no compila nada.** Lo que hay que subirle es **la aplicación ya
armada**: los `.dll`, la configuración, los archivos estáticos y el `web.config`
que le dice a IIS cómo arrancarla. Eso es lo que produce `dotnet publish`.

#### El comando

**Desde la carpeta del proyecto de la API:**

```powershell
cd C:\Users\fcl\OneDrive\Desktop\proyectos_usbmed\2026_2\proyecto_aplicacion_y_servicios_web4\api_facturas
dotnet publish -c Release -o publicado
```

**Pieza por pieza:**

| Parte | Qué hace | Qué pasa si no la pone |
|---|---|---|
| **`dotnet publish`** | Compila **y arma el paquete listo para desplegar**: resuelve los paquetes NuGet, copia los archivos de configuración y **genera el `web.config`** | — |
| **`-c Release`** | Compila en modo **publicación**: optimizado, sin la información de depuración | Compila en `Debug`: **más lento y más pesado**, y expone detalles internos en los errores |
| **`-o publicado`** | Deja el resultado en una carpeta llamada **`publicado`** | Lo deja enterrado en `bin\Release\net10.0\publish\`, que es largo de escribir en el comando siguiente |

> **`-c` es de *configuration* y `-o` de *output*.** Las dos tienen forma larga
> —`--configuration`, `--output`— y hacen exactamente lo mismo.

#### Qué debe salir

Al final, una línea así:

```
api_facturas -> C:\...\api_facturas\publicado\
```

| Lo que se ve | Qué significa |
|---|---|
| **Terminado con la ruta de `publicado`** | **Bien.** Ahí quedó todo |
| `error CS...` | **No compiló.** Arregle el código primero — esto no tiene nada que ver con el hospedaje |
| `warning` | Avisos. **No impiden publicar** |

#### Qué quedó adentro

Vale la pena abrir la carpeta una vez, porque explica qué es «una aplicación
publicada»:

| Archivo | Qué es |
|---|---|
| **`ApiFacturas.dll`** | **Su código compilado.** Lo que de verdad corre |
| `ApiFacturas.exe` | El lanzador, para correrla sin IIS |
| **`web.config`** | **El que hace que IIS la sepa arrancar.** Lo genera `publish`, no está en el repositorio |
| `appsettings.json` | La configuración base — la de Docker |
| **`appsettings.Production.json`** | **La del hospedaje**, con la cadena de la base de datos |
| `*.deps.json` · `*.runtimeconfig.json` | Qué paquetes y qué versión de .NET necesita |
| Un montón de `.dll` más | Las bibliotecas de las que depende |

> **Sí: el `appsettings.Production.json` con la contraseña viaja adentro.** Es
> lo que queremos — allá la necesita— y es exactamente por eso que **la
> carpeta `publicado/` también quedó en el `.gitignore`**. Lo que no puede
> estar en GitHub no puede colarse por la puerta de atrás.

> **Y fíjese en lo que NO quedó:** no hay archivos `.cs`. **El código fuente no
> se sube.** Lo que corre en el hospedaje es el compilado — que es la
> diferencia entre publicar una aplicación .NET y publicar un PHP, donde lo que
> se sube **es** el código.

#### Así se ve cuando sale bien

```
Restauracion completada (0,7s)
  ApiFacturas net10.0 realizado correctamente (8,3s) -> publicado\

Compilacion realizado correctamente en 10,7s
```

| Renglón | Qué pasó |
|---|---|
| `Restauracion completada` | Resolvió los paquetes NuGet. **Menos de un segundo** porque ya estaban bajados |
| `ApiFacturas net10.0 ... -> publicado\` | **Compiló y dejó el resultado ahí.** El `net10.0` confirma la versión |
| `Compilacion realizado correctamente` | **Cero errores.** Si hubiera uno, aquí diría cuántos |

**Y esto es lo que quedó**, medido en la carpeta:

| | |
|---|---|
| **50 archivos** | 18 MB en total |
| **`0` archivos `.cs`** | **El código fuente no viaja** |
| **`web.config`** | Generado por `publish` — no estaba en el proyecto |
| **`appsettings.Production.json`** | Con la cadena de la base de datos adentro |

#### El `web.config`, que nadie escribió

Ábralo: son diez renglones, y **es la pieza que hace que IIS sepa arrancar una
aplicación .NET**.

```xml
<aspNetCore processPath="dotnet"
            arguments=".\ApiFacturas.dll"
            stdoutLogEnabled="false"
            stdoutLogFile=".\logs\stdout"
            hostingModel="inprocess" />
```

| Atributo | Qué dice |
|---|---|
| `processPath="dotnet"` · `arguments=".\ApiFacturas.dll"` | **Cómo arrancarla**: literalmente, `dotnet ApiFacturas.dll` |
| `hostingModel="inprocess"` | La aplicación corre **dentro del proceso de IIS**. Es lo más rápido |
| **`stdoutLogEnabled="false"`** | **Los mensajes de arranque NO se guardan** |
| `stdoutLogFile=".\logs\stdout"` | Dónde se guardarían si se encendiera |

> **Guarde ese `stdoutLogEnabled`, que vale oro cuando algo falle.** Si la API
> publicada responde **error 500 desde la primera petición** y los `Logs` del
> panel no dicen nada útil, es porque **ni siquiera llegó a arrancar** — y lo
> que explica por qué está en esos mensajes de arranque que están apagados.
>
> **La solución:** cambiar ese `false` por `true` en el `web.config` **de la
> carpeta `publicado`**, volver a subir, provocar el error y mirar
> `logs\stdout_*.log` con el administrador de archivos del panel. **Y apagarlo
> otra vez después**: encendido, escribe en cada arranque y llena el disco.

#### Si lo vuelve a correr

**No hay problema: `publish` sobreescribe.** Pero si antes había archivos que
ya no existen en el proyecto, **se quedan ahí** — `publish` no limpia. Para
una carpeta limpia de verdad, bórrela antes:

```powershell
Remove-Item -Recurse -Force publicado
dotnet publish -c Release -o publicado
```


### 6.3 · Subirla: `msdeploy`

#### El comando

**Desde la misma carpeta `api_facturas`:**

```powershell
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" `
  -verb:sync `
  -source:contentPath="C:\Users\fcl\OneDrive\Desktop\proyectos_usbmed\2026_2\proyecto_aplicacion_y_servicios_web4\api_facturas\publicado" `
  -dest:contentPath="site101125",computerName="https://site101125.siteasp.net:8172/msdeploy.axd?site=site101125",userName="site101125",password="c+3LK6h#-iM4",authType="Basic" `
  -enableRule:AppOffline `
  -allowUntrusted
```

> **El acento grave `` ` `` al final de cada renglón es el continuador de
> PowerShell**: dice «el comando sigue abajo». Si prefiere, bórrelos y escriba
> todo en una sola línea — es exactamente lo mismo.

#### Pieza por pieza

| Parte | Qué dice |
|---|---|
| **`& "C:\Program Files\...\msdeploy.exe"`** | El programa. **El `&` hace falta** porque la ruta tiene espacios: sin él, PowerShell cree que es un texto |
| **`-verb:sync`** | **Sincronizar**, no copiar. Compara las dos carpetas y manda **solo la diferencia** |
| **`-source:contentPath=`** | **Qué mando**: la carpeta `publicado`, **con la ruta completa** |
| **`-dest:contentPath="site101125"`** | **A cuál sitio.** Es el `Site` del panel |
| **`computerName="https://site101125.siteasp.net:8172/msdeploy.axd?site=site101125"`** | **A qué servidor.** Fíjese que el identificador va **dos veces**: en el `contentPath` y en el `?site=` |
| **`userName`** · **`password`** | El `Login` y el `Password` de `Visual Studio access` |
| **`authType="Basic"`** | Usuario y contraseña, **no** la cuenta de Windows de su máquina |
| **`-enableRule:AppOffline`** | **Pausa el sitio mientras sube** y lo levanta al terminar |
| **`-allowUntrusted`** | Acepta el certificado del puerto 8172 sin validar quién lo emitió |

> **`-enableRule:AppOffline` no es opcional en la práctica.** Si el sitio está
> corriendo, el `ApiFacturas.dll` **está en uso** y el servidor no deja
> reemplazarlo. Con esta regla, WebDeploy pone un `app_offline.htm`, el proceso
> suelta los archivos, sube todo y lo quita. **Sin ella, el error típico es
> «file in use» a mitad de camino**, con el sitio a medio actualizar.

#### Lo que sale, y cómo leerlo

**Son muchos renglones. Hay tres que importan:**

```
Info: Usando el id. '8f6c13c3-...' para las conexiones al servidor remoto.
Info: Adding file (site101125\ApiFacturas.dll).
Info: Deleting file (site101125\iisstart.htm).
...
Numero total de cambios: 81 (80 agregados, 1 eliminados, 0 actualizados, 0 parametros cambiados, 18647083 bytes copiados)
```

| Renglón | Qué significa |
|---|---|
| `Usando el id. ...` | Se conectó y el servidor le dio una sesión. **Si las credenciales estuvieran mal, el comando moriría aquí** |
| `Adding file (...)` | Cada archivo que sube. La primera vez son todos |
| **`Deleting file (site101125\iisstart.htm)`** | **El único borrado** — ver abajo |
| **`Numero total de cambios: 81`** | **El resumen, y la línea que hay que leer** |

**El resumen, desglosado:**

| | |
|---|---|
| **80 agregados** | Los 50 archivos más las carpetas: los idiomas de `Microsoft.Data.SqlClient` y los `runtimes` por plataforma |
| **1 eliminado** | `iisstart.htm` |
| **0 actualizados** | No había nada que actualizar: estaba vacío |
| **18.647.083 bytes** | Los 18 MB |

#### Ese `iisstart.htm` borrado es la demostración de qué es «sincronizar»

**`iisstart.htm` era la página de «Your website is ready…»** — la que vimos
en la miniatura del panel, la que el hospedaje pone en todo sitio nuevo.

**Nadie le dijo a `msdeploy` que la borrara.** La borró porque
**`-verb:sync` deja el destino igual al origen**, y en `publicado` no hay
ningún `iisstart.htm`.

> **Eso es lo que separa `sync` de «subir por FTP»**, y conviene entenderlo en
> las dos direcciones:
>
> | | |
> |---|---|
> | **A favor** | **Lo que queda arriba es exactamente lo que usted publicó.** No sobrevive un archivo viejo de una versión anterior causando un error imposible de explicar |
> | **En contra** | **Lo que esté en el servidor y no en su carpeta, se borra.** Si alguien subió algo a mano —una imagen, un archivo de configuración— **desaparece en la siguiente publicación** |
>
> **Para una API no es problema**: todo lo que necesita viene de `publicado`.
> Pero vale saberlo antes de que pase.

#### Y la segunda vez va a ser mucho más corta

Esta fue la primera: **subió todo**. La próxima, **solo lo que cambie** — si
tocó un controlador, un archivo; si cambió la configuración, otro. Es lo que
prometía el panel: *«only changed files are transferred, so subsequent deploys
are quick»*.

**El ciclo completo de ahí en adelante son dos comandos:**

```powershell
dotnet publish -c Release -o publicado
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -source:contentPath="...\publicado" -dest:contentPath="site101125",computerName="https://site101125.siteasp.net:8172/msdeploy.axd?site=site101125",userName="site101125",password="c+3LK6h#-iM4",authType="Basic" -enableRule:AppOffline -allowUntrusted
```


### 6.4 · Comprobar que quedó viva — en tres golpes

**No basta con que el comando no diera error.** Hay tres cosas distintas que
pueden estar mal, y **cada prueba descarta una**:

| Prueba | Qué demuestra |
|---|---|
| **1. La raíz** | Que el sitio **arranca** |
| **2. Swagger** | Que los **controladores** se cargaron |
| **3. Un inicio de sesión fallido** | Que **la base de datos contesta** |

#### 1. La raíz: ¿arranca?

```
http://apifacturas-carloscastro.runasp.net/
```

```json
{"mensaje":"API Facturas funcionando","version":"v4","motor":"sqlserver",
 "contratos":"docs/spec_kit/versiones/v4_aplicativo/6_contracts.md"}
```

| Lo que dice | Por qué importa |
|---|---|
| `API Facturas funcionando` | **El proceso arrancó.** Ya no está el `iisstart.htm` |
| `"version":"v4"` | Es la versión que creíamos |
| **`"motor":"sqlserver"`** | **El `appsettings.Production.json` sí se está leyendo.** En su máquina esto también dice `sqlserver`, pero aquí lo dice **porque ganó el archivo de producción** |

> **Si esta primera petición tarda varios segundos, es el arranque en frío**
> del plan gratuito. La segunda vuela.

#### 2. Swagger: ¿se cargaron los controladores?

```
http://apifacturas-carloscastro.runasp.net/swagger
```

**Debe abrir la página interactiva de siempre**, con los catorce controladores
y el botón `Authorize`.

> **Que esto funcione publicado no era obvio.** En muchos proyectos Swagger
> está envuelto en un `if (app.Environment.IsDevelopment())`, y entonces
> **desaparece justo donde más se necesita**. En esta API no lo está — por eso
> se puede probar el sistema publicado sin instalar nada.

#### 3. La prueba que de verdad importa: ¿contesta la base de datos?

**Las dos anteriores pasarían igual con la cadena de conexión equivocada.** La
API arranca sin tocar la base de datos — solo la toca en la primera petición
que necesite datos.

**El truco: intentar entrar con una contraseña que uno sabe que está mal.**

```powershell
Invoke-RestMethod -Uri "http://apifacturas-carloscastro.runasp.net/api/sesion" `
  -Method POST -ContentType "application/json" `
  -Body '{"email":"admin@correo.com","contrasena":"estaNoEs"}'
```

La respuesta esperada:

```json
{"estado":401,"mensaje":"El correo o la contrasena no son correctos."}
```

| La respuesta | Qué quiere decir |
|---|---|
| **`401` con ese mensaje** | **Todo bien.** Para poder decir que la contraseña no sirve, la API **tuvo que ir a la base de datos**, buscar el correo y comparar. **La conexión funciona** |
| **`500`** | **La conexión NO funciona.** Revise el `appsettings.Production.json`: servidor, usuario, contraseña |
| **`404`** | La ruta está mal escrita, no es un problema de datos |

> **Ese 401 es la mejor noticia de todo el tutorial**, y es contraintuitivo:
> **un error es la prueba de que todo funciona.** Un rechazo correcto exige que
> la cadena entera esté bien — red, credenciales del servidor, base de datos,
> tabla, consulta y comparación de la clave. **Lo único que falla es lo que
> queríamos que fallara.**

> **Y por eso es la prueba que se le pide a un estudiante**: no necesita saber
> ninguna contraseña de verdad para demostrar que su sistema quedó bien
> publicado.


### 6.5 · Ver con los propios ojos lo que llegó

**`Files`**, en el menú del sitio. Es el equivalente a lo que hicimos con la
base de datos cuando contamos las doce tablas: **comprobar que lo que uno mandó
es lo que llegó**, en vez de creerle al mensaje de «éxito».

#### La raíz: dos carpetas y ningún archivo

![El administrador de archivos en Home](docs/tutorial_publicar/img/40_archivos_raiz.png)

```
2 folders  ·  0 files
```

| Carpeta | `SOURCE` | Qué es |
|---|---|---|
| **`wwwroot`** | `System` | **La raíz del sitio.** Lo que esté aquí es lo que se sirve por internet |
| **`private`** | `System` | **Fuera del alcance de la web.** Nada de lo que esté aquí se puede pedir desde un navegador |

> **Las dos las creó el hospedaje**, no nosotros — eso dice el `SOURCE:
> System`. Y `private` existe por una razón: **hay cosas que una aplicación
> necesita guardar pero nadie debe poder descargar.** Si un día guarda archivos
> subidos por usuarios, documentos o respaldos, **van ahí, no en `wwwroot`**.

> **Ojo con el `Disk usage` de arriba: dice `0 B used`.** Y acabamos de subir 18
> MB. **No es un error: es un número viejo** — el panel lo calcula cada tanto,
> no en cada cambio. El botón **`Recalculate`** lo pone al día.

#### Dentro de `wwwroot`

![El contenido de wwwroot](docs/tutorial_publicar/img/41_archivos_wwwroot.png)

```
14 folders  ·  36 files  ·  8.9 MB
```

**Y ese renglón se puede comprobar contra su propia carpeta.** En
`api_facturas\publicado` hay, medido:

| | En su máquina | En el servidor |
|---|---|---|
| Carpetas en la raíz | **14** | **14** |
| Archivos en la raíz | **36** | **36** |

**Coinciden exactamente.** Lo que mandó es lo que llegó.

#### ¿Por qué dice 8.9 MB si `msdeploy` dijo 18 MB?

**Porque no están midiendo lo mismo**, y vale la pena saberlo para no salir a
buscar un problema que no existe:

| Número | Qué cuenta |
|---|---|
| **`18.647.083 bytes`** de `msdeploy` | **Todo**: los 36 archivos de la raíz **más** los de las 14 carpetas |
| **`8.9 MB`** del administrador | **Solo los 36 archivos de este nivel.** Las carpetas aparecen con `—` en la columna `SIZE` |

> **Comprobado:** sumando solo los 36 archivos de la raíz en el disco local dan
> **9.321.867 bytes = 8,9 MB**. El total con subcarpetas da **18.647.083
> bytes** — **el mismo número exacto que reportó `msdeploy`**.
>
> **La diferencia son los `runtimes`**: la misma biblioteca de SQL Server
> compilada para `win-x64`, `win-x86`, `win-arm64` y `unix`. **Casi la mitad
> del peso de la aplicación son copias de una sola biblioteca** para
> plataformas que este servidor no usa.

#### La columna `SOURCE` dice quién puso cada cosa

| Valor | Significa |
|---|---|
| **`WebDeploy`** | **Llegó con nuestro comando.** Lo tienen las catorce carpetas |
| `System` | Lo puso el hospedaje al crear el sitio |

> **Es más útil de lo que parece.** Si un día aparece un archivo con otro
> origen —subido a mano, por FTP— **esa columna lo delata**. Y recuerde que en
> la siguiente publicación `sync` lo va a borrar.

#### Las carpetas de idiomas, explicadas

`cs`, `de`, `es`, `fr`, `it`, `ja`, `ko`, `pl`, `pt-BR`, `ru`, `tr`, `zh-Hans`,
`zh-Hant` — trece.

**No son suyas, y no son el idioma de la aplicación.** Cada una trae un
`Microsoft.Data.SqlClient.resources.dll`: **los mensajes de error del conector
de SQL Server traducidos**. Si el servidor estuviera en alemán, el error de
conexión saldría en alemán.

> **Y `es` está entre ellas**, así que un error de base de datos puede salirle
> **en español** — lo cual confunde, porque uno cree que el mensaje lo escribió
> alguien del proyecto.

#### ¿Y los archivos? En la página siguiente

**La primera página solo tiene carpetas, y confunde:** el listado muestra **15
renglones** y pone **las carpetas primero**. Las catorce más la fila `..`
llenan la página justa. **Los 36 archivos están en las páginas 2, 3 y 4** — la
paginación está abajo a la derecha.

| Para verlos todos de una vez | |
|---|---|
| **Suba `Records`** | El desplegable de arriba dice `15`. Con `50` caben todos |
| **O use el `Search`** | Filtra por nombre, ruta u origen |

![La segunda pagina: los archivos](docs/tutorial_publicar/img/42_archivos_pagina2.png)

#### Lo que enseña esta lista

**1. Su código es una porción minúscula de lo que se publica.**

| Archivo | Tamaño |
|---|---|
| **`ApiFacturas.dll`** — **todo su código** | **439 KB** |
| El total subido | **18 MB** |

> **Su trabajo es el 2 % del paquete.** El resto son las bibliotecas de las que
> depende: el conector de SQL Server, Dapper, el manejo de tokens, Swagger.
> **Eso es lo que significa «depender de un paquete»** — se ve en la balanza.

**2. La columna `UPDATED` no dice cuándo se subió.**

| Archivo | `CREATED` | `UPDATED` |
|---|---|---|
| `appsettings.Production.json` | hace 10 min | **hace 23 min** |
| `appsettings.json` | hace 10 min | **hace 18 días** |
| `Azure.Core.dll` | hace 10 min | **26.02.2024** |

> **`CREATED` es cuándo llegó; `UPDATED` es la fecha que el archivo traía.**
> WebDeploy **conserva la fecha de modificación original** — por eso el
> `Azure.Core.dll` dice 2024: es cuando Microsoft lo compiló.
>
> **Y eso no es un detalle estético: es el mecanismo del `sync`.** Así decide
> qué subir la próxima vez — **compara fechas y tamaños**. Un archivo cuya
> fecha no cambió ni se vuelve a mandar. De ahí sale el *«only changed files
> are transferred»*.
>
> **El `appsettings.json` con 18 días lo confirma**: nadie lo tocó desde
> entonces, y el servidor lo sabe.

**3. El botón `Edit` solo aparece en unos renglones.**

| Tipo | Botones |
|---|---|
| `.json`, `.config` y demás texto | **`Download`** y **`Edit`** |
| `.dll`, `.exe`, `.pdb` | **Solo `Download`** |

> **No es un permiso: es sentido común.** Un `.dll` es binario — abrirlo en un
> editor de texto no sirve de nada.

#### Ver el `appsettings.Production.json` que llegó

**Está en esa misma página 2**, y tiene botón `Edit`. Si prefiere ir directo,
**escriba `appsettings` en el `Search`**.

**Ábralo con el menú `...` del renglón**, con la opción de ver o editar.

**Debe decir exactamente lo mismo que el de su máquina**, con la contraseña de
la base de datos adentro.

> **Y ahí, viendo el archivo en el servidor, se entiende la regla del proyecto
> de aula.** Esa contraseña **está en texto plano en un archivo del
> hospedaje**, y cualquiera que entre a este panel puede leerla. Para una
> demostración de clase está bien. **Para el proyecto calificado, por eso se
> exigen variables de entorno**: no quedan en ningún archivo que alguien pueda
> abrir.

> **No lo edite desde aquí**, aunque el panel lo permita. **Lo que cambie en el
> servidor lo borra la siguiente publicación**, porque `sync` deja el destino
> igual al origen. Lo que haya que cambiar se cambia en su máquina y se vuelve
> a publicar. **La única excepción razonable** es encender el
> `stdoutLogEnabled` del `web.config` para diagnosticar un arranque fallido — y
> sabiendo que se pierde en la próxima subida.


### 6.6 · Probar el sistema completo desde el Swagger publicado

![El Swagger de la API publicada](docs/tutorial_publicar/img/43_swagger_publicado.png)

```
http://apifacturas-carloscastro.runasp.net/swagger
```

**Es el mismo Swagger de siempre**, con los catorce controladores y los cinco
verbos de cada uno — pero ahora **la dirección es pública**: cualquiera con ese
enlace lo abre, sin instalar nada y sin Docker.

> **Fíjese en el enlace del documento OpenAPI**, debajo del título:
> `http://apifacturas-carloscastro.runasp.net/swagger/v1/swagger.json`. **Ese
> enlace es el contrato de la API**, y se le puede pasar a alguien para que
> genere un cliente sin ver el código.

#### Las credenciales de prueba

Son las mismas de siempre — **las que siembra `bdfacturas.sql`**, y por eso
funcionan igual publicadas:

| Usuario | Contraseña | Quién es |
|---|---|---|
| **`admin@correo.com`** | **`admin123`** | **Administrador** — ve y hace todo |
| `vendedor1@correo.com` | `admin123` | Vendedor |
| `jefe@correo.com` | `admin123` | Jefe |
| `cliente1@correo.com` | `admin123` | Cliente |

> **Y esto sí es un secreto de mentiras:** son datos sembrados de un ejercicio
> de clase, iguales en los cinco repositorios del curso. **No se parecen en
> nada a una contraseña de verdad** — que es, justamente, lo que las hace
> servibles para enseñar.
>
> **Pero ojo con una consecuencia real:** la API está **publicada en internet**
> y estas claves son públicas. Cualquiera que vea este tutorial puede entrar.
> **Mientras sea una demostración, está bien.** Si alguna vez pone datos que
> importen, lo primero es cambiarlas.

#### Los cuatro pasos en Swagger

| | Qué hacer |
|---|---|
| **1** | Despliegue **`POST /api/sesion`** → **`Try it out`** |
| **2** | En el cuerpo: `{ "email": "admin@correo.com", "contrasena": "admin123" }` → **`Execute`** |
| **3** | De la respuesta, **copie el valor de `token`** — sin las comillas |
| **4** | Arriba, **`Authorize`** → pegue el token → `Authorize` → `Close` |

> **En el `Authorize` va el token pelado**, sin escribir `Bearer` adelante: **esa
> palabra la pone Swagger**. Es el error más común, y se manifiesta como un 401
> después de haberse autenticado bien.

**De ahí en adelante, el candado de cada ruta queda cerrado** y cualquier
`Try it out` lleva el token.

#### Comprobado contra la API publicada

**No es teoría: esto se corrió contra `apifacturas-carloscastro.runasp.net`.**

| Petición | Respuesta |
|---|---|
| `POST /api/sesion` con `admin123` | **`token`, `email`, `roles`, `expira`** — la sesión se abrió |
| `GET /api/cliente` con el token | **4 clientes** |
| `GET /api/producto` con el token | La lista de productos |
| `GET /api/consultas/ventas-por-producto` | La consulta multitabla de la v4 |

Y el primer cliente llegó así:

```json
{"id":1,"credito":520000.00,"fkcodpersona":"P001","fkcodempresa":"E001"}
```

> **Ese `P001` es Ana Torres**, la primera persona que sembramos con el
> `bdfacturas_publicar.sql` en el Paso 4. **El círculo se cerró:** el archivo
> `.sql` que importó en el panel, la base de datos `db75266`, la cadena de
> conexión del `appsettings.Production.json`, el `ApiFacturas.dll` que subió
> con `msdeploy` — **y un dato que sale por internet**.

#### Y el sobre de la respuesta, de paso

```json
{ "datos": [...], "total": 4, "limite": 50, "tabla": "cliente" }
```

**La lista no viene suelta: viene envuelta.** Es el contrato del curso — el
front no tiene que adivinar cuántos hay ni de dónde salieron.

#### Así se ve el inicio de sesión, hecho

![El POST a api/sesion respondiendo 200 con el token](docs/tutorial_publicar/img/44_swagger_sesion.png)

```json
{
  "estado": 200,
  "mensaje": "Sesion iniciada.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "admin@correo.com",
  "roles": [ "Administrador" ],
  "expira": "2026-10-23T04:15:01.6051958Z"
}
```

**Tres cosas de esta respuesta vale la pena mirar:**

| | |
|---|---|
| **`roles: ["Administrador"]`** | **No lo dice el token nada más: lo averiguó en la base de datos.** Esa consulta cruza `usuario`, `rol_usuario` y `rol` — tres tablas de las doce que importó |
| **`expira`** | Una hora exacta después. La cabecera `date` dice `03:15:00 GMT` y el token expira a las `04:15:01`: **es el `DuracionMinutos: 60`** del `appsettings.json`, confirmado con reloj |
| **El recuadro `Curl`** | Swagger arma **el comando equivalente**. Cópielo y tiene la misma petición para una terminal, un script o Postman |

**Y las cabeceras de respuesta dicen dónde está corriendo:**

```
server: Microsoft-IIS/10.0
x-powered-by: ASP.NET
```

> **Eso no sale cuando usted corre en su máquina.** Ahí contesta Kestrel
> directamente; aquí contesta **IIS**, que recibe la petición y se la pasa a su
> aplicación por el `AspNetCoreModuleV2` — el del `web.config`.

#### Y con el token puesto, los datos

![El GET a api/usuario con el token, respondiendo ocho usuarios](docs/tutorial_publicar/img/45_swagger_usuarios.png)

```json
{ "tabla": "usuario", "limite": 1000, "total": 8, "datos": [ ... ] }
```

**`total: 8`**, y los ocho correos son **exactamente** los del `INSERT INTO
usuario` del `bdfacturas_publicar.sql`:

| | |
|---|---|
| Los cuatro de rol | `admin@` · `vendedor1@` · `jefe@` · `cliente1@correo.com` |
| Los dos de prueba | `test_encript@` · `nuevo@correo.com` |
| Los dos reales | `carlos.castro@usbmed.edu.co` · `carloscastro5033@correo.itm.edu.co` |

> **Ocho en el archivo, ocho en la respuesta.** Es la misma comprobación que
> hicimos con las doce tablas, pero ahora **atravesando todo el sistema**: del
> `.sql` a la base de datos, de la base de datos a la API, de la API a
> internet.

> **Y fíjese en lo que NO viene:** la respuesta trae `email` y nada más.
> **La contraseña no sale** — ni cifrada. Un `SELECT *` habría mandado los
> hashes a cualquiera con un token. **Que no estén ahí es una decisión del
> código**, no un regalo del framework.

#### Una cosa pendiente: `Not secure` en la barra

La barra del navegador dice **`Not secure`**, porque entramos por **`http://`**.

> **No significa que el sitio no tenga certificado.** El panel tiene una
> sección **`HTTPS` · SSL certificates**, y los subdominios `runasp.net` suelen
> traer el suyo puesto. **Pruebe la misma dirección con `https://`** — si
> abre, úsela siempre.
>
> **Y por qué importa más aquí que en otras páginas:** por `http://`,
> **la contraseña y el token viajan legibles**. Acabamos de mandar
> `admin123` en el cuerpo de una petición. En una red compartida —la
> universidad, un café— eso se puede leer.

### 6.7 · El certificado: de `http` a `https`

**`https://apifacturas-carloscastro.runasp.net/swagger` no abre.** Comprobado
también desde la terminal:

```
schannel: failed to receive handshake, SSL/TLS connection failed
```

**Y no es la red:** `http://` responde perfecto por el mismo camino. **El sitio
no tiene certificado**, sencillamente.

#### La pantalla lo dice sin rodeos

![La seccion HTTPS con el dominio sin certificado](docs/tutorial_publicar/img/46_https_apagado.png)

**`HTTPS` · SSL certificates**, en el menú del sitio.

| Lo que se ve | Qué significa |
|---|---|
| **`Secured 0 of 1 domain`** | Un dominio, ninguno asegurado |
| **`HTTP only`** · *«No certificate — visitors get an insecure connection»* | El estado de la única fila |
| **`HTTPS redirect` · `Disabled`** | Nadie es enviado a la versión segura |

#### Lo importante: es gratis y automático

> *«Each domain/binding connected to your website can have its own HTTPS
> certificate. For a **Free certificate** we use **Let's Encrypt** —
> certificates are **issued and renewed automatically**»*

| | |
|---|---|
| **No cuesta** | También en el plan gratuito |
| **No se renueva a mano** | Let's Encrypt dura 90 días y **el hospedaje lo renueva solo**. Sin esto, cada tres meses el sitio amanecería roto |
| **Es por dominio** | Cada dominio conectado al sitio lleva el suyo |

#### El orden, y el propio panel lo obliga

**Oprima `Enable HTTPS`** en la fila del dominio — no el botón verde de arriba,
que hace lo mismo pero para escoger cuál.

**Y solo después, encienda `HTTPS Redirect`.** El panel no deja hacerlo al
revés, y lo explica:

> *«Enable HTTPS on at least one domain first — **there is nothing to redirect
> to yet**»*

> **Ese candado está puesto a propósito, y evita un desastre clásico:** forzar
> la redirección **antes** de tener certificado deja el sitio **completamente
> muerto** — toda petición `http` se manda a `https`, y `https` no existe. No
> hay página de error que lo explique: simplemente no carga.

#### Qué hace la redirección, y por qué encenderla

> *«When enabled, every HTTP request is automatically redirected to its secure
> HTTPS version. **Modern devices work only over `https://`** and search
> engines also prefer `https://` URLs»*

**Sin la redirección, las dos direcciones funcionan** — y la insegura sigue
abierta para el que la escriba. Con ella, **no hay forma de entrar por la
puerta mala.**

#### Por qué esto no es opcional en este proyecto

| | |
|---|---|
| **La contraseña viaja legible** | Acabamos de mandar `admin123` en el cuerpo de una petición |
| **El token también** | Quien lo lea entra como administrador **durante una hora** |
| **Y el front lo va a exigir** | Si la interfaz queda en `https` y llama a una API en `http`, **el navegador bloquea la llamada** — *mixed content*— y el error no dice que sea por eso |

> **Ese último es el que vuelve esto urgente.** Resolverlo ahora cuesta cinco
> minutos; descubrirlo depurando el front cuesta una tarde buscando en el lado
> equivocado.

#### Después de encenderlo

| Compruebe | Dónde |
|---|---|
| `Secured **1** of 1 domain` | En esta misma pantalla |
| Que **`https://apifacturas-carloscastro.runasp.net/swagger`** abra | En el navegador |
| Que la barra **ya no diga `Not secure`** | Arriba |
| Que `http://` **lo mande solo** a `https://` | Con la redirección encendida |

**Y de ahí en adelante, `https` en todas partes:** en lo que se le pase al
estudiante, en Postman y —sobre todo— **en el `UrlApi` del front**, que es lo
que sigue.

#### Hecho: así quedó

![La seccion HTTPS con el certificado emitido y la redireccion encendida](docs/tutorial_publicar/img/47_https_encendido.png)

```
Done - HTTPS redirect was enabled
Secured 1 of 1 domain      HTTPS redirect on
apifacturas-carloscastro.runasp.net -> HTTPS
Let's Encrypt - renews automatically - expires in 60 days (22.12.2026)
```

**Y la redirección se comprobó desde la terminal**, que es lo único que no
miente:

```
http://apifacturas-carloscastro.runasp.net/
-> 307, redirige a: https://apifacturas-carloscastro.runasp.net/
```

| Antes | Ahora |
|---|---|
| `http` respondía `200` **directo** | `http` responde **`307`** y manda a `https` |

> **El `307` es un «muévase, pero sin cambiar nada».** A diferencia del `301`
> o el `302`, **conserva el método y el cuerpo**: un `POST` sigue siendo un
> `POST` después del salto. Si fuera un `302`, un inicio de sesión se
> convertiría en `GET` y **perdería el cuerpo con las credenciales** — un error
> precioso de diagnosticar.

#### Comprobado en el navegador

**El Swagger abrió por `https` y la barra ya no dice `Not secure`.** Y dentro
de la página, **todo cambió solo**:

| Antes | Ahora |
|---|---|
| `Request URL: http://apifacturas-carloscastro.runasp.net/api/sesion` | **`https://`**`apifacturas-carloscastro.runasp.net/api/sesion` |
| El recuadro `Curl` decía `http` | Dice **`https`** |

> **Nadie reconfiguró Swagger.** Esas direcciones las arma **con la dirección
> por la que usted entró**: si entra por `https`, prueba por `https`. Por eso
> basta con cambiar la letra en la barra.

**Y las dos pruebas del paso anterior se repitieron por el canal cifrado:**

| Petición | Resultado |
|---|---|
| `POST /api/sesion` con `admin123` | **`200`** · *«Sesion iniciada»* con su token |
| `GET /api/usuario?limite=1000` | **`200`** · `total: 8` |

**Mismo sistema, mismos datos, ahora cifrado.**

#### Una consecuencia inmediata: ya no hay `http`

**Todo lo que estaba apuntando a `http://` ahora paga un salto de más**, y hay
que cambiarlo:

| Dónde | A qué |
|---|---|
| El enlace que se le pasa al estudiante | `https://apifacturas-carloscastro.runasp.net/swagger` |
| Las peticiones de Postman o de la terminal | `https://` |
| **El `UrlApi` del front** | **`https://`** — lo configuramos en el Paso 7 |

> **Y aquí una trampa que descubrimos midiendo, no leyendo:** la redirección
> **no sustituye configurar bien el front** — y no por lentitud, sino porque
> **rompe las llamadas autenticadas**.
>
> ```
> GET http://...  con la cabecera  Authorization: Bearer <token>
>   -> 307 hacia https://...
>   -> el cliente repite la peticion SIN la cabecera
>   -> 401 No autorizado
> ```
>
> **El cliente descarta la cabecera `Authorization` al seguir un redirect**, y
> lo hace a propósito: el redirect podría llevar a otro servidor, y mandarle el
> token a un desconocido sería peor. **Comprobado contra esta misma API**: la
> petición sin token pasa, la petición con token vuelve `401`.
>
> **Moraleja:** `http` **no funciona a medias — no funciona.** El `UrlApi` va
> directo a `https`.

#### Las cuatro pestañas del diálogo, por si aparecen dudas

| Pestaña | Para qué |
|---|---|
| **`Let's Encrypt`** | **La nuestra.** Gratis, automática, renovada sola |
| `Certificate` | Ver el que ya está puesto |
| `Custom PEM/KEY` · `Custom PFX` | Subir uno comprado. Dos formatos del mismo archivo |
| `Disable` | Quitarlo |

> **Una inconsistencia del panel que conviene no perseguir:** el diálogo dice
> que el certificado *«renews itself automatically every 90 days»* y la fila
> dice `expires in 60 days`. **La palabra que importa es `automatically`** —
> nadie tiene que acordarse en diciembre.

#### Lo que falta: la interfaz

**La API ya sirve, y ahora cifrada. Lo que no hay todavía es la pantalla**, y
es el Paso 7.


## Paso 7 — Publicar la interfaz

**La API ya sirve, pero nadie va a usarla escribiendo JSON en Swagger.** Falta
la pantalla.

**Es el mismo procedimiento del Paso 6, con tres diferencias:**

| | Qué cambia |
|---|---|
| **Otro sitio** | Un segundo sitio en el hospedaje, con **su propia dirección y sus propias credenciales** |
| **Otra configuración** | El front no habla con la base de datos: **habla con la API**. Lo único que necesita saber es su dirección |
| **Otra carpeta** | `front_blazor` en vez de `api_facturas` |

### 7.1 · Decirle al front dónde está la API

#### El problema, que es el mismo de antes con otra cara

Hoy el front busca la API en **tres sitios distintos**, según dónde esté
corriendo:

| Dónde corre | A qué dirección le habla | Quién se la dice |
|---|---|---|
| En su máquina, sin Docker | `http://localhost:8035` | `front_blazor/appsettings.json` |
| Dentro de Docker | `http://api-facturas:8035` | La variable `UrlApi` del `docker-compose.yml` |
| **Publicado** | **la dirección pública de su API** | **El archivo que va a crear ahora** |

> **Sin ese archivo, el front publicado buscaría la API en `localhost:8035`** —
> o sea, **dentro del servidor del hospedaje**, donde no hay ninguna API. No
> encontraría nada: todas las pantallas saldrían vacías o con error, y el
> mensaje no diría que el problema es la dirección.

#### Cuándo se crea: **ahora, antes de compilar**

**El orden importa y no es negociable:**

| | |
|---|---|
| **1. Ahora** | Se crea `front_blazor/appsettings.Production.json` |
| **2. Después** | `dotnet publish` — que **lo copia** a la carpeta `publicado` |
| **3. Por último** | `msdeploy` — que **sube esa copia** al servidor |

> **Si lo crea después de compilar, no viaja.** `publish` copia lo que
> encuentra **en el momento en que corre**; un archivo creado después se queda
> en su disco, el front publicado sigue buscando `localhost:8035` y las
> pantallas salen vacías. **Y nada en la salida de `msdeploy` lo advierte.**

> **Dos archivos con el mismo nombre, y uno de ellos no se toca:**
>
> | Ruta | Qué es |
> |---|---|
> | **`front_blazor/appsettings.Production.json`** | **El suyo.** Aquí se edita |
> | `front_blazor/publicado/appsettings.Production.json` | **Una copia**, que `publish` vuelve a generar cada vez |
>
> **Editar el de `publicado` no sirve de nada:** el siguiente `dotnet publish`
> lo sobreescribe.

#### Créelo

**En `front_blazor`, al lado de `appsettings.json`, cree un archivo llamado:**

```
appsettings.Production.json
```

**Con este contenido completo** — cambiando la dirección por la de **su** API:

```json
{
  "_comentario": "La configuracion del front PUBLICADO. A diferencia de la de la API, esta SI se versiona: no tiene un solo secreto. Lo unico que dice es a que direccion le habla, y esa direccion es publica.",

  "_por_que_Production": "ASP.NET Core lee appsettings.json y encima appsettings.<Entorno>.json. En el hospedaje no se fija ASPNETCORE_ENVIRONMENT y el entorno por defecto es Production: por eso este archivo gana SOLO publicado. En su maquina sigue mandando appsettings.json, que apunta a localhost:8035; dentro de Docker manda la variable UrlApi del compose.",

  "_por_que_https": "Con la redireccion forzada encendida, la API responde 307 a toda peticion http. Apuntar directo a https evita ese salto en cada llamada -y evita que el navegador bloquee la mezcla de http con https cuando el front si esta cifrado-.",

  "UrlApi": "https://apifacturas-carloscastro.runasp.net"
}
```

#### Qué hace cada clave

| Clave | ¿La usa el front? | Para qué está |
|---|---|---|
| `_comentario` | **No** | Dice **qué es este archivo** y por qué éste sí se versiona |
| `_por_que_Production` | **No** | Deja escrito **por qué se llama así** — y las tres direcciones posibles |
| `_por_que_https` | **No** | Deja escrito **por qué `https` y no `http`** |
| **`UrlApi`** | **Sí** | **La única clave que lee el programa.** Lo demás es para quien lea |

> **Tres de cuatro claves son comentarios**, y no sobra ninguna. Es un archivo
> de una línea útil que **nadie va a volver a mirar en seis meses** — y cuando
> lo mire, la pregunta no va a ser *«qué dice»* sino *«por qué existe»*.

**Tres cosas que se equivocan y cuestan una tarde:**

| | |
|---|---|
| **`https`, no `http`** | Con `http` **las llamadas autenticadas fallan con `401`**: el redirect `307` hace que el cliente descarte la cabecera `Authorization`. No es lentitud — es que no funciona |
| **Sin la barra al final** | `...runasp.net`, **no** `...runasp.net/` — el front le pega las rutas detrás, y dos barras seguidas dan un 404 |
| **Sin `/api` al final** | Va **la dirección del sitio**, no la de un recurso. El `/api/...` lo pone el código |

#### Éste sí va a GitHub

| | El de la API | **El del front** |
|---|---|---|
| Qué trae | La cadena de conexión, **con contraseña** | **Solo una dirección pública** |
| ¿Se versiona? | **No** | **Sí** |

> **Y como el `.gitignore` ya tiene la línea `appsettings.Production.json`, esa
> regla también tapa al del front.** Hay que destaparlo — agregue debajo:
>
> ```
> # EXCEPCION: la del front no tiene secretos -solo la direccion publica
> # de la API-, asi que esa si se versiona.
> !front_blazor/appsettings.Production.json
> ```
>
> **El `!` significa «esto NO se ignora»**, y tiene que ir **después** de la
> regla que lo ignoraba: en `.gitignore` gana **la última** que coincide.

**Compruébelo:**

```powershell
git status
```

| Debe aparecer | No debe aparecer |
|---|---|
| `front_blazor/appsettings.Production.json` | `api_facturas/appsettings.Production.json` |

> **Y ahí está la regla de verdad, en una frase:** **lo que no se versiona no
> son los archivos de producción — son los secretos.** Confundir las dos cosas
> lleva a esconder configuración que a todo el equipo le serviría ver.


### 7.2 · El segundo sitio

**Es el asistente del Paso 5, otra vez.** Nada nuevo que explicar — solo lo que
cambia:

| Paso | Qué escoger |
|---|---|
| **Plan** | `Select FREE website` |
| **Domain** | `Free subdomain` → **`facturas-carloscastro`** → `.runasp.net` |
| **DNS · Subscription** | Se saltan solos |
| **Location** | **Europa** — el mismo de la API y de la base de datos |
| **Example** | `My own application` |

> **El nombre, sin el `api` adelante.** Así, en la lista, se distinguen de un
> vistazo: `apifacturas-…` es la API, `facturas-…` es la interfaz. Con cinco
> sitios permitidos y dos por equipo, esa diferencia de cuatro letras es lo que
> evita publicar el front encima de la API.

**Quedó creado como `site101138`** — distinto del `site101125` de la API, como
era de esperarse.

#### Y la descripción, esta vez que quepa

```
Front facturas v4 - demo clase - AySW 2026-2
```

**44 caracteres**, a propósito. Al de la API le pusimos
`API facturas v4 - demostracion de clase - AySW 2026-2` y **en la lista se cortó
en `AySW 20`**. Las dos filas juntas son el ejemplo:

```
facturas-carloscastro.runasp.net
Front facturas v4 - demo clase - AySW 2026-2      site101138

apifacturas-carloscastro.runasp.net
API facturas v4 - demostracion de clase - AySW 20  site101125
```

#### Las credenciales, nuevas otra vez

`Deploy` → **`Visual Studio access`** → **`Enabled`**, igual que en el Paso 5.9
— **la cuenta de WebDeploy tampoco existe hasta encenderla.**

| | La API | **El front** |
|---|---|---|
| **Site / Login** | `site101125` | **`site101138`** |
| **Server** | `site101125.siteasp.net` | **`site101138.siteasp.net`** |
| **Password** | `c+3LK6h#-iM4` | **`Na4_e#T36Bg@`** |

**Y `Test connection` antes de seguir:** `OK - Connected in 102 ms`.

### 7.3 · Compilar y subir el front

**Son los dos comandos del Paso 6**, cambiando la carpeta y el sitio.

#### Compilar

```powershell
cd ...\proyecto_aplicacion_y_servicios_web4\front_blazor
dotnet publish -c Release -o publicado
```

#### Y una sorpresa en la balanza

| | La API | **El front** |
|---|---|---|
| Archivos | 54 | **30** |
| Tamaño | **17,8 MB** | **1,6 MB** |

**El front pesa once veces menos**, y uno esperaría lo contrario — al fin y al
cabo trae pantallas, CSS e iconos, y la API solo manda JSON.

> **La explicación está en el `FrontFacturas.csproj`**, que lo dice en un
> comentario: *«Sin paquetes: Blazor Server viene en el SDK web, y el front no
> necesita NADA de acceso a datos»*.
>
> **Los 18 MB de la API eran las bibliotecas que el front no tiene:**
> `Microsoft.Data.SqlClient` con sus `runtimes` para cuatro plataformas,
> `Npgsql`, `Dapper`, `Azure.Identity` y los trece idiomas de los mensajes de
> error. **El front no habla con ninguna base de datos — habla con la API por
> HTTP**, y eso no necesita un solo paquete.

> **Es la arquitectura, hecha peso.** Que el front sea incapaz de llegar a la
> base de datos no es una promesa del diseño: **se comprueba mirando la
> carpeta.** Si mañana alguien le agregara un conector de base de datos, los
> archivos publicados lo delatarían.

#### Subir

```powershell
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" `
  -verb:sync `
  -source:contentPath="C:\...\front_blazor\publicado" `
  -dest:contentPath="site101138",computerName="https://site101138.siteasp.net:8172/msdeploy.axd?site=site101138",userName="site101138",password="Na4_e#T36Bg@",authType="Basic" `
  -enableRule:AppOffline `
  -allowUntrusted
```

**Cuatro cambios respecto al de la API, y todos son el mismo:** otro sitio.

| Qué cambió | De | A |
|---|---|---|
| La carpeta de origen | `api_facturas\publicado` | **`front_blazor\publicado`** |
| El `contentPath` del destino | `site101125` | **`site101138`** |
| El `computerName` — **dos veces** | `site101125` | **`site101138`** |
| `userName` y `password` | los de la API | **los del front** |

> **Y aquí está el único error de este paso que cuesta caro:** si deja el
> `site101125` por descuido, **no falla — publica el front encima de la API**,
> y se la borra, porque `sync` deja el destino igual al origen. Se arregla
> volviendo a publicar la API, pero mientras tanto el sistema está caído.
>
> **Revise los números antes de oprimir Enter.** Son cinco apariciones del
> identificador en un solo comando.

#### Y el certificado de este sitio también

**`HTTPS` → `Enable HTTPS` → y después `HTTPS Redirect` → `Enabled`**, igual
que en el Paso 6.7.

> **Aquí importa doble.** El front va a quedar en `https` y llama a una API en
> `https`: **los dos cifrados, y el navegador no tiene nada que objetar.** Si el
> front se quedara en `http`, el navegador vería una página insegura llamando a
> un servicio seguro — funciona, pero es la mitad de la protección y la barra
> va a seguir diciendo `Not secure` a sus estudiantes.


### 7.4 · Lo que subió, leído

```
Numero total de cambios: 39 (38 agregados, 1 eliminados, 0 actualizados,
                             0 parametros cambiados, 1682908 bytes copiados)
```

| | |
|---|---|
| **1.682.908 bytes** | **El mismo número exacto** que tenía la carpeta `publicado` |
| **1 eliminado** | `iisstart.htm` otra vez — el «Your website is ready…» de este sitio |
| **38 agregados** | Nueve archivos en la raíz y el resto dentro de `wwwroot` |

#### LOS DOS `appsettings`: cuál manda, y por qué viajan los dos

**En la salida se ven subiendo uno detrás del otro:**

```
Info: Adding file (site101138\appsettings.json).
Info: Adding file (site101138\appsettings.Production.json).
```

**Y eso es exactamente lo que queremos.** No sobra ninguno:

| Archivo | Qué dice | Cuándo lo usa |
|---|---|---|
| **`appsettings.json`** | `UrlApi: http://localhost:8035` | **Siempre se lee.** Es la base |
| **`appsettings.Production.json`** | `UrlApi: https://apifacturas-carloscastro.runasp.net` | **Encima del anterior**, porque allá el entorno es `Production` |

**La regla, en una frase:** ASP.NET Core **lee los dos, en ese orden**, y
**la clave repetida gana en el último**. No es que uno reemplace al otro: es
que se **superponen**.

```
appsettings.json              UrlApi = http://localhost:8035
appsettings.Production.json   UrlApi = https://apifacturas-carloscastro.runasp.net   <- gana
                              ------------------------------------------------------
resultado en el hospedaje     UrlApi = https://apifacturas-carloscastro.runasp.net
resultado en su maquina       UrlApi = http://localhost:8035
```

> **Por qué importa que se superpongan y no que se reemplacen:** el
> `appsettings.json` del front trae además `Logging` y `AllowedHosts`, que el
> de producción **no repite**. Esas siguen valiendo. **Solo cambia lo que el
> segundo archivo menciona** — por eso el de producción puede tener una sola
> línea útil.

> **Y por eso el de desarrollo se sube sin problema.** Alguien podría pensar
> que apuntar a `localhost` en el servidor es peligroso. No lo es: **esa línea
> nunca gana allá**. Y si borrara el archivo base, se llevaría por delante el
> `Logging` y el `AllowedHosts`.

#### `.br` y `.gz`: cada archivo estático subió tres veces

Fíjese en el patrón:

```
wwwroot\app.css
wwwroot\app.css.br
wwwroot\app.css.gz
```

**Y lo mismo con `marca.css`, `formularios.js`, `bootstrap.min.css`,
`bootstrap.bundle.min.js`, `blazor.server.js` y `blazor.web.js`.**

| Extensión | Qué es |
|---|---|
| sin extensión extra | El archivo **tal cual** |
| **`.gz`** | Comprimido con **gzip** — lo entiende cualquier navegador |
| **`.br`** | Comprimido con **Brotli** — más pequeño, lo entienden los navegadores modernos |

> **No los creó nadie a mano: los genera `dotnet publish`.** Y el servidor
> escoge: el navegador dice en cada petición qué compresiones acepta, y IIS le
> manda **la mejor que ambos entiendan**. Si no entiende ninguna, recibe el
> archivo normal.
>
> **Es por lo que la página carga rápido aunque el servidor esté en Europa:**
> lo que cruza el Atlántico es la versión comprimida.

#### Y los dos `blazor` que delatan la tecnología

```
wwwroot\_framework\blazor.server.js
wwwroot\_framework\blazor.web.js
```

**Ahí está el circuito de Blazor Server**, en forma de archivo. Ese JavaScript
es lo único que corre en el navegador: **abre la conexión permanente con el
servidor, manda los clics y recibe los pedacitos de HTML que cambian.**

> **Toda la lógica de las pantallas sigue en C#, en el servidor.** Por eso el
> front pesa 1,6 MB: **no hay una aplicación JavaScript aquí**, solo el cable.


### 7.5 · Los archivos del front en el servidor

![El administrador de archivos del front](docs/tutorial_publicar/img/48_archivos_front.png)

```
1 folder  ·  9 files  ·  668.5 KB
```

**Cabe todo en una página**, sin paginación — a diferencia de la API, que
necesitaba cuatro.

#### Las dos `wwwroot`, que no son un error

Fíjese en la ruta: está en `Home > wwwroot`, y **adentro hay otra `wwwroot`**.

| Cuál | De quién es | Qué contiene |
|---|---|---|
| **La de afuera** | **Del hospedaje** | La raíz del sitio: donde IIS busca la aplicación |
| **La de adentro** | **De su proyecto** | Los archivos estáticos de ASP.NET Core: CSS, JavaScript, iconos |

> **La API no tenía la de adentro**, y esa ausencia dice qué es cada cosa:
> **una API no sirve archivos — solo manda JSON.** Un front sí: el navegador
> le pide la hoja de estilos, los iconos y el JavaScript.

#### Los nueve archivos de la raíz

| Archivo | Tamaño | Qué es |
|---|---|---|
| **`FrontFacturas.dll`** | **304,5 KB** | **Todo el código del front**: las catorce pantallas, el menú, los servicios |
| `FrontFacturas.exe` | 158,5 KB | El lanzador |
| `FrontFacturas.pdb` | 155,4 KB | Símbolos de depuración |
| `FrontFacturas.staticwebassets.endpoints.json` | **47,6 KB** | **El mapa de los archivos estáticos** — ver abajo |
| `FrontFacturas.deps.json` · `runtimeconfig.json` | menos de 1 KB | De qué depende y con qué .NET corre |
| **`web.config`** | 499 B | El que hace que IIS lo sepa arrancar |
| `appsettings.json` | 331 B | La configuración base — `localhost:8035` |
| **`appsettings.Production.json`** | **892 B** | **La que manda aquí** — apunta a la API publicada |

> **Y no hay ni un solo `.dll` de terceros.** Compárelo con la API, que traía
> cuarenta y tantos: `Microsoft.Data.SqlClient`, `Npgsql`, `Dapper`,
> `Swashbuckle`, `Azure.Identity`… **El front no depende de nada que no venga
> con .NET.** Eso es lo que explica los 1,6 MB contra 18.

#### El archivo raro: `staticwebassets.endpoints.json`

**47,6 KB — pesa más que todos los demás archivos de configuración juntos.**

Es **el catálogo de los archivos estáticos**: por cada uno dice su tamaño, su
huella y **qué versiones comprimidas existen**. Es lo que le permite al
servidor contestar «¿acepta Brotli? tome el `.br`» sin salir a mirar el disco.

> **Por eso la compresión funciona sin configurar nada**: no es magia de IIS,
> es este archivo.

#### Y otra vez la columna `UPDATED`

| Archivo | `CREATED` | `UPDATED` |
|---|---|---|
| `appsettings.Production.json` | hace 5 min | **hace 27 min** |
| `appsettings.json` | hace 5 min | **hace 18 días** |

**Las mismas fechas que tenían en su disco.** El de producción dice 27 minutos
— cuando se escribió— y el base, 18 días, que es desde cuando nadie lo toca.

#### Compruébelo y siga

**Abra `appsettings.Production.json` con `Edit`.** Debe decir su dirección
`https://`. **Si dice `localhost:8035`, el archivo no viajó** — lo creó
después de compilar, y hay que volver a hacer `dotnet publish` y `msdeploy`.

> **Y el `Disk usage` de arriba sigue diciendo `0 B used`.** Igual que con la
> API: es un número viejo. `Recalculate` lo pone al día.


### 7.6 · El sistema completo, y un defecto que solo aparece publicado

![El front publicado con la sesion iniciada y las facturas](docs/tutorial_publicar/img/50_front_facturas.png)

**Entró con `admin@correo.com`, y ahí está todo funcionando:** el menú con las
trece interfaces, `admin@correo.com [Administrador]`, y las seis facturas con
su cliente, su vendedor y su total.

**Esa pantalla recorrió la cadena entera:**

```
navegador -> front (site101138) -> API (site101125) -> base de datos (db75266)
```

#### Pero mire la columna del vendedor

```
Carlos PA(c)rez        MarA-a GA3mez        Juan DA-az
```

**Las tildes están rotas.** Y no es el front: **son los datos, mal guardados en
la base de datos.**

#### De dónde salió

| | |
|---|---|
| El archivo `bdfacturas_publicar.sql` estaba en **UTF-8** | Es lo correcto |
| Pero **sin BOM** | La marca de tres bytes que anuncia «esto es UTF-8» |
| El hospedaje importa con **`sqlcmd`** | Y `sqlcmd`, **sin BOM, lee el archivo como Windows-1252** |

**Entonces cada letra acentuada se partió en dos.** En UTF-8, la `é` ocupa dos
bytes (`C3 A9`); leídos como Windows-1252, esos dos bytes son **dos
caracteres**: `Ã` y `©`. Así quedaron guardados.

> **Y esto es lo que lo hace peligroso: el import dijo `Success` y `Errors 0`.**
> Para SQL Server no hubo ningún error — recibió texto perfectamente válido,
> solo que el texto equivocado. **La comprobación de «cero errores» no alcanza
> para esto.**
>
> **Ni se nota en su máquina.** En Docker la base se carga por otro camino, que
> sí respeta el UTF-8. **Es un defecto que solo existe publicado** — de los que
> aparecen el día de la sustentación.

#### El arreglo: tres bytes

**Al archivo `db/bdfacturas_publicar.sql` se le agregó el BOM de UTF-8** — los
bytes `EF BB BF` al principio, invisibles en el editor. Con eso, `sqlcmd`
reconoce la codificación y los acentos entran bien.

| | |
|---|---|
| **En VS Code** | Abajo a la derecha dice la codificación. Debe decir **`UTF-8 with BOM`** |
| **Si dice `UTF-8` a secas** | Haga clic ahí → `Save with Encoding` → `UTF-8 with BOM` |

#### Y hay que volver a importar

**El archivo arreglado no arregla los datos ya cargados.** Hay que repetir el
Paso 4 con el archivo nuevo:

**`Databases` → `bdfacturas` → `Import SQL`** → el mismo archivo → importar.

> **Y se puede hacer sin miedo**, porque el script **empieza borrando todo**:
> los disparadores, los procedimientos y las doce tablas, en orden. Está hecho
> para correrse encima de sí mismo.
>
> **Lo que sí se pierde son los datos que haya agregado a mano** desde la
> interfaz. En una demostración no importa; el día que importe, primero un
> respaldo.

**Después de reimportar, recargue el front.** Donde decía `Carlos PA(c)rez` debe
decir **`Carlos Pérez`**.

> **Por qué no hay que volver a publicar nada:** el defecto estaba **en los
> datos**, no en el código. La API y el front siguen siendo los mismos
> archivos.


### 7.7 · El certificado del front, y el sistema cerrado

![La seccion HTTPS del front, asegurada y redirigiendo](docs/tutorial_publicar/img/51_https_front.png)

**Es el mismo procedimiento del Paso 6.7**, en el otro sitio:

| | |
|---|---|
| **1** | `HTTPS` → **`Enable HTTPS`** en la fila del dominio → `Let's Encrypt` → `Enable HTTPS` |
| **2** | Esperar a **`Secured 1 of 1 domain`** |
| **3** | `Force redirect to HTTPS` → **`Enabled`** |

```
Secured 1 of 1 domain      HTTPS redirect on
facturas-carloscastro.runasp.net -> HTTPS
Let's Encrypt - renews automatically - expires in 60 days (22.12.2026)
```

#### Comprobado desde la terminal

```
front  http -> 307 -> https://facturas-carloscastro.runasp.net/
api    http -> 307 -> https://apifacturas-carloscastro.runasp.net/
```

**Los dos sitios cifrados y los dos redirigiendo.**

#### Y aquí se cobra la decisión del Paso 7.1

**Entre otra vez al front y abra cualquier pantalla con datos.** Funciona igual
que antes — y eso **no era inevitable**.

| Si el `UrlApi` dijera | Qué pasaría hoy |
|---|---|
| `http://apifacturas-...` | **El inicio de sesión seguiría funcionando** — el `307` conserva el cuerpo |
| | **Pero toda pantalla con datos daría `401`** — el `307` descarta la cabecera `Authorization` |
| **`https://apifacturas-...`** | **Todo funciona.** El front habla directo, sin redirect |

> **Y ese es el peor tipo de defecto que existe:** el que deja entrar y falla
> *después*. Un estudiante vería la pantalla de inicio de sesión aceptar su
> clave y, un segundo más tarde, una tabla vacía o un error. **Buscaría el
> problema en los datos, en el token, en los permisos** — en todo menos en una
> letra `s` de un archivo de configuración.

#### El sistema, completo

```
navegador
   |  https
front  facturas-carloscastro.runasp.net     site101138
   |  https
API    apifacturas-carloscastro.runasp.net  site101125
   |  red interna del centro de datos
base   db75266.databaseasp.net              db75266
```

| Tramo | Cómo va | Por qué |
|---|---|---|
| Navegador → front | **`https`** | Sale a internet |
| Front → API | **`https`** | Sale a internet — son dos sitios distintos |
| **API → base de datos** | **Sin cifrar** | **No sale a internet**: viajan por la red del centro de datos |

> **Que el último tramo no vaya cifrado no es un descuido** — es el
> `Local access` del Paso 3, y es más rápido justamente porque no sale. Los
> dos primeros sí salen, y por eso los dos llevan certificado.


## Paso 8 — El mismo sistema, contra PostgreSQL

**Hasta aquí el sistema corre contra SQL Server.** Pero la v4 habla con **dos
motores**, y hasta ahora eso ha sido una promesa del diseño. **Este paso la
pone a prueba.**

| Lo que vamos a hacer | |
|---|---|
| Crear una base PostgreSQL en **Neon**, gratis | |
| Cargarle **el mismo esquema**, en su dialecto | |
| Publicar **la misma API** en un tercer sitio, cambiando **un archivo** | |

> **Y aquí está la apuesta:** los dos sitios van a correr el **mismo
> `ApiFacturas.dll`, byte por byte**. Mismas rutas, mismo contrato, mismas
> respuestas. Lo único distinto será un archivo de texto de ocho renglones.
>
> **Si funciona, las capas están bien hechas:** la API no sabe contra qué motor
> habla. **Si falla, tampoco se pierde el viaje** — el sitio donde falle es
> exactamente la capa que se coló.

### 8.1 · Por qué Neon y no otro

![La pagina de Neon](docs/tutorial_publicar/img/52_neon_inicio.png)

**MonsterASP ofrece MySQL y MSSQL, pero no PostgreSQL.** Hay que buscarlo
afuera, y Neon es la opción razonable para un curso:

| | |
|---|---|
| **Gratis de verdad** | No pide tarjeta |
| **Se apaga solo** | *Serverless*: cuando nadie la consulta, **no hay ningún proceso encendido**. Por eso puede ser gratis |
| **Se crea en menos de un segundo** | Literal — lo vamos a medir |
| **Entra con GitHub** | Los estudiantes ya tienen cuenta. Una contraseña menos |

**El precio de que se apague sola:** la primera consulta después de un rato
**tarda segundos** mientras la base despierta. Es el mismo arranque en frío del
sitio — pero ahora hay **dos siestas encadenadas**: la del sitio y la de la
base.

> **Y una advertencia que conviene decir en voz alta:** arriba, en su propia
> página, Neon anuncia que **fue comprada** — *NEON IS PART OF THE DATABRICKS
> PLATFORM*. **El plan gratuito de hoy puede no existir el año entrante.** Por
> eso el proyecto **no depende de Neon**: el motor se cambia por configuración,
> y volver a SQL Server no cuesta una línea de código.

**`Sign up`** → entrar con **GitHub**.

### 8.2 · El proyecto, y la única decisión que importa

![La consola de Neon con los proyectos](docs/tutorial_publicar/img/53_neon_proyectos.png)

La consola abre en **`Projects`**. Arriba, el consumo del mes — `Compute`,
`Storage`, `History`, `Network transfer`— y abajo, los proyectos que haya.

**`New project`.**

![El dialogo de crear proyecto](docs/tutorial_publicar/img/54_neon_crear_opciones.png)

#### Los `Services`: encéndase solo uno

| Servicio | Qué es | ¿Se usa? |
|---|---|---|
| **`Postgres database`** | **La base de datos** | **Sí** — viene encendido |
| `Object storage` | Baldes para archivos e imágenes | No |
| `Functions` | Código que corre al lado de los datos | No — la lógica está en la API |
| `AI gateway` | Un punto de entrada para modelos de IA | No |
| `Realtime` | Consultas que se actualizan solas | No — el front pregunta cuando necesita |
| **`Better Auth`** | Autenticación empaquetada | **No, y es importante**: el proyecto ya tiene la suya, con JWT y roles. **Eso es lo que el curso enseña** |

> **Encender servicios que no se usan no es gratis**: consumen del mismo plan y
> ensucian la consola. **Un proyecto de clase necesita una base de datos y nada
> más.**

#### La región: **esto es lo que hay que cambiar**

**El desplegable viene en una región de Estados Unidos** — `AWS US East 2
(Ohio)`, o la que le toque. **Cámbielo a `AWS Europe Central 1 (Frankfurt)`.**

![El dialogo con Frankfurt seleccionado](docs/tutorial_publicar/img/55_neon_frankfurt.png)

| | Ohio | **Frankfurt** |
|---|---|---|
| Distancia a la API (Europa) | **Cruza el Atlántico** | Misma región |
| Ida y vuelta por consulta | **~90–100 ms** | **~1–5 ms** |
| Una pantalla con cinco consultas | **medio segundo solo en viajes** | imperceptible |

> **Es la misma decisión del Paso 5.4**, cuando escogimos Europa para el sitio
> *porque la base de datos estaba en Europa*. Lo que cambia es que ahora **la
> base no es del hospedaje**, y por primera vez usted escoge dónde ponerla.

> **Y la región NO se puede cambiar después.** La primera vez que hicimos esto
> se quedó en Ohio: hubo que borrar el proyecto y empezar otra vez.
>
> **El orden que lo evita:** **primero la región, después el nombre.** Al revés
> uno escribe el nombre, da por hecho que ya está, y oprime el botón sin volver
> a mirar arriba.

**Tres indicadores tienen que coincidir antes de crear:** el desplegable, **el
recuadro negro de la derecha** y **el globo** — que gira hasta mostrar la
región escogida.

![El proyecto creado](docs/tutorial_publicar/img/56_neon_creado.png)

```
Your new project was created in 672 ms
```

> **Esos 672 ms explican por qué rehacerlo no era drama.** Y también por qué
> este modelo existe: **no se aprovisiona un servidor**, se registra una base en
> una infraestructura que ya está corriendo.

**De esa pantalla, ignore el `Agent prompt`** — es para que una IA cablee una
aplicación de Node con `npm`. **Nosotros ya tenemos la aplicación hecha, y en
.NET.** El enlace útil es `Prefer to wire it up yourself?`.

**`Go to project`.**

### 8.3 · El tablero del proyecto

![La vista general de la rama](docs/tutorial_publicar/img/57_neon_rama.png)

#### Lo primero: no está viendo «la base de datos» — está viendo **una rama**

El título dice **`Branch overview`** y la ruta es
`All projects / facturas_aysw4 / production`.

**Ésa es la idea central de Neon:** no hay «la base» y ya — hay **ramas, como
en git**. La de por defecto se llama **`production`**. De ahí se sacan copias
instantáneas para probar algo destructivo y botarlas después.

> **Es lo que uno quisiera tener la noche anterior a una migración.** Hoy no la
> vamos a usar, pero vale que los estudiantes sepan que existe.

#### El gráfico vacío dice algo

```
ENDPOINT INACTIVE
```

**La base está dormida.** Nadie la ha consultado, así que **no hay ningún
proceso encendido**. No es un error: es el *scale to zero* a la vista.

#### La ficha de la derecha

| Dato | Valor | Qué significa |
|---|---|---|
| **Region** | `AWS Europe Central 1 (Frankfurt)` | **Lo que costó rehacer el proyecto** |
| **Project ID** | `dawn-sky-10056593` | Aparece en la cadena de conexión |
| **Branch** | `production` · `Expires Never` | La rama por defecto no caduca |
| **`IP restrictions`** | **`None set`** | **Cualquiera con la contraseña entra, desde donde sea** |
| **Default compute** | `0.25 ↔ 2 CU` | Arranca con un cuarto de unidad y crece hasta 2. Un cuarto es ~0,25 de CPU y 1 GB de RAM: **sobra** |
| **History retention** | `6 horas` | Puede devolver la base a cualquier instante de las últimas seis horas |
| **Version** | **`18`** | **PostgreSQL 18** |

> **Lo de `IP restrictions: None set` marca una diferencia de fondo con
> MonsterASP.** Allá el acceso remoto **viene apagado** y hay que encenderlo;
> aquí **el acceso desde afuera es lo normal**, porque Neon está pensada
> justamente para eso. **La contraseña es toda la puerta**, y por eso importa
> más que la otra.

> **Y `Version 18` contra el `postgres:16-alpine` del `docker-compose.yml`:**
> el esquema del proyecto no usa nada exclusivo de una versión, así que
> **debería cargar sin problema** — pero eso es un supuesto, no un hecho. **Se
> comprueba al importar**, y si algo falla el mensaje lo dirá.

### 8.4 · La cadena de conexión, y dónde va

![El dialogo de conexion](docs/tutorial_publicar/img/58_neon_conectar.png)

**El botón verde `Connect`**, arriba a la izquierda.

#### Lo que hay que mirar en el diálogo

| Campo | Valor | Qué es |
|---|---|---|
| **Las cuatro pestañas** | `Postgres database` · `Storage` · `Data API` · `Auth` | Formas distintas de llegar a los datos. **La nuestra es la primera**: conexión SQL de toda la vida |
| **Compute** | `Primary` · **`Active`** | **Se despertó.** Hace un minuto el gráfico decía `ENDPOINT INACTIVE`; **abrir esta pantalla la encendió** |
| **Database** | **`neondb`** | El nombre que Neon le pone. No vale la pena cambiarlo |
| **Role** | **`neondb_owner`** | El usuario. En PostgreSQL los usuarios se llaman *roles* |
| **`Reset password`** | | Cambia la contraseña. Es lo que se usa cuando una clave se filtra |

#### El interruptor `Connection pooling`: déjelo encendido

Mire lo que le hace al nombre del servidor:

```
ep-calm-heart-b9e1zu5w-pooler.c-7.eu-central-1.aws.neon.tech
                      ^^^^^^^
                      esto lo agrega el pooling
```

| Sin pooling | **Con pooling** |
|---|---|
| Cada conexión abre un proceso en la base | **Un intermediario reparte un puñado de conexiones** entre todos |
| Se agota rápido | Aguanta muchos más clientes |

> **Y para esta API es justo lo que hace falta:** abre y cierra conexión **en
> cada petición**. Sin el *pooler*, veinte estudiantes probando a la vez la
> tumban.

#### La cadena, y lo que dice el final

```
postgresql://neondb_owner:CLAVE@ep-calm-heart-...-pooler.c-7.eu-central-1.aws.neon.tech/neondb
             ?sslmode=require&channel_binding=require
```

| Parte | Qué dice |
|---|---|
| `postgresql://` | El formato estándar de PostgreSQL — **no es el de .NET** |
| `...eu-central-1...` | **Frankfurt.** Ahí queda confirmada la región |
| **`sslmode=require`** | **Cifrado obligatorio.** Es lo contrario de la conexión a MonsterASP: **ésta sí sale a internet** |
| **`channel_binding=require`** | Amarra la autenticación al canal cifrado, para que nadie se interponga |

#### Traducirla al formato de .NET

**Npgsql no entiende ese formato.** Hay que pasarla a pares `Clave=Valor`:

```
Host=ep-calm-heart-b9e1zu5w-pooler.c-7.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=SU_CLAVE;SSL Mode=Require
```

| El formato de Neon | **El de .NET** |
|---|---|
| `postgresql://usuario:clave@servidor/base` | Pares `Clave=Valor` separados por `;` |
| `?sslmode=require` | **`SSL Mode=Require`** — con espacio y mayúsculas |
| `&channel_binding=require` | **Se omite.** Npgsql no tiene esa opción: **ya usa amarre de canal por su cuenta** cuando la conexión va cifrada |

> **Cópiela con `Copy snippet`, nunca a mano.** Esa contraseña mezcla
> mayúsculas, minúsculas y dígitos que se confunden entre sí — `l` con `1`,
> `O` con `0`— y un carácter mal leído se manifiesta como un error de
> autenticación que no dice que el problema sea un carácter.

#### Y ahora sí: **dónde va**

**En un archivo de configuración nuevo**, al lado del que ya existe:

```
api_facturas/
   appsettings.Production.json            -> SQL Server  (el sitio que ya publicamos)
   appsettings.Production.postgres.json   -> Neon        (el sitio que falta)
```

**Su contenido:**

```json
{
  "Motor": "postgres",

  "ConnectionStrings": {
    "Postgres": "Host=ep-calm-heart-b9e1zu5w-pooler.c-7.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=SU_CLAVE;SSL Mode=Require"
  }
}
```

**Dos claves, y las dos tienen que coincidir:**

| Clave | Por qué |
|---|---|
| **`"Motor": "postgres"`** | Es la que escoge el dialecto. **Sin ella la API sigue hablando T-SQL** |
| **`ConnectionStrings:`**`Postgres` | **Ojo con el nombre: `Postgres`, no `SqlServer`.** El `Program.cs` lee las dos por separado, y **la que no corresponda al motor ni se mira** |

> **El error que viene, y cómo se ve:** si pone la cadena de Neon pero deja
> `"Motor": "sqlserver"`, **la API arranca bien**, lee la cadena de SQL Server
> —que en ese archivo no existe— y falla con un error de conexión **que no
> menciona PostgreSQL por ninguna parte**. Uno revisa la cadena de Neon veinte
> veces antes de mirar la otra línea.

**Y este archivo tampoco se versiona:** trae la contraseña de Neon. Al
`.gitignore`:

```
appsettings.Production.postgres.json
```

con su `.ejemplo` al lado, igual que hicimos con el de SQL Server.


### 8.5 · Cargar el esquema en Neon

**Neon no tiene un botón de «importar archivo» como MonsterASP.** Hay dos
caminos, y los dos sirven:

| Camino | Cuándo conviene |
|---|---|
| **El editor SQL de la consola** | Si no quiere instalar ni correr nada. Se pega el script y `Run` |
| **`psql` desde la terminal** | **El que vamos a usar.** Más control, mejores mensajes de error |

#### El editor SQL, por si lo prefiere

![El editor SQL de Neon](docs/tutorial_publicar/img/59_neon_editor.png)

`Postgres database` → **`SQL Editor`**. Trae un ejemplo cargado — una tabla
`playing_with_neon`— que **hay que borrar**: clic dentro, `Ctrl+A`, `Supr`.

Luego se abre `db/bdfacturas_postgres.sql` en VS Code, `Ctrl+A`, `Ctrl+C`, se
pega y **`Run`**. Antes de oprimir, compruebe arriba que diga **`neondb`** y
**`Primary`**.

> **Sirve, pero son 55 KB en un editor de navegador.** Y si algo falla a mitad
> de camino, el mensaje es más difícil de ubicar.

#### El camino que usamos: `psql`, sin instalarlo

**No hace falta instalar PostgreSQL.** El cliente `psql` vive dentro de la
imagen de Docker que el proyecto **ya usa** para la base local.

```powershell
cd ...\proyecto_aplicacion_y_servicios_web4

docker run --rm -e PGCLIENTENCODING=UTF8 -v "${PWD}\db:/db:ro" postgres:16-alpine psql "postgresql://neondb_owner:SU_CLAVE@ep-calm-heart-b9e1zu5w-pooler.c-7.eu-central-1.aws.neon.tech/neondb?sslmode=require" -v ON_ERROR_STOP=1 -f /db/bdfacturas_postgres.sql
```

#### La idea de fondo: **no instalamos PostgreSQL — lo prestamos**

`psql` es el cliente de línea de comandos de PostgreSQL, y **viene dentro de la
imagen `postgres`**. El proyecto ya la usa para la base local, así que **ya está
descargada en su máquina**.

**Entonces en vez de instalar PostgreSQL entero para usar una herramienta,
levantamos un contenedor de un solo uso, le pasamos el archivo, y lo botamos.**

> **Y eso es lo que lo hace servible en clase:** ningún estudiante tiene que
> instalar nada que no tenga ya. El único requisito es Docker, que el curso usa
> desde la primera versión.

#### El comando, partido en dos mitades

```
docker run --rm -e ... -v ...   postgres:16-alpine   psql "..." -v ... -f ...
|_________ la mitad de Docker _________|             |____ la mitad de psql ____|
```

**Todo lo que va ANTES del nombre de la imagen es para Docker.** Todo lo que va
DESPUÉS es **el programa que corre adentro** y sus propios argumentos. Leerlo
así desarma la confusión.

#### La mitad de Docker

| Parte | Qué hace | Qué pasa si falta |
|---|---|---|
| **`docker run`** | Levanta un contenedor nuevo a partir de una imagen | — |
| **`--rm`** | **Lo borra apenas termina** | Queda un contenedor muerto ocupando disco. A la décima vez, diez |
| **`-e PGCLIENTENCODING=UTF8`** | Define una **variable de entorno** dentro del contenedor. `psql` la lee para saber **en qué codificación viene el archivo** | `psql` adivina por la configuración regional del contenedor, y **las tildes pueden partirse** — exactamente lo que pasó con `sqlcmd` |
| **`-v "${PWD}\db:/db:ro"`** | **Monta una carpeta suya dentro del contenedor** | El contenedor no ve el archivo. `psql` responde que no existe |
| **`postgres:16-alpine`** | **La imagen.** Aquí termina la mitad de Docker | — |

**El `-v` de Docker se lee en tres partes, separadas por dos puntos:**

```
-v  "${PWD}\db  :  /db  :  ro"
     |              |      |
     |              |      solo lectura
     |              como se ve ADENTRO
     la carpeta de SU maquina
```

| | |
|---|---|
| **`${PWD}`** | PowerShell lo reemplaza por **la carpeta donde usted está parado**. Por eso el `cd` de antes importa |
| **`/db`** | El nombre que esa carpeta tiene **dentro** del contenedor. Por eso el archivo se llama `/db/bdfacturas_postgres.sql` y no `C:\...\db\...` |
| **`:ro`** | *read only*. **El contenedor puede leer sus archivos, no escribirlos.** Cuesta cuatro letras y evita un accidente |

> **Las comillas no son decoración:** la ruta puede tener espacios —
> `OneDrive\Desktop\proyectos usbmed`— y sin comillas PowerShell la parte en
> pedazos.

#### La mitad de `psql`

| Parte | Qué hace | Qué pasa si falta |
|---|---|---|
| **`psql "postgresql://..."`** | El programa y **a dónde conectarse** | Intenta conectarse a una base local que no existe |
| **`-v ON_ERROR_STOP=1`** | **Para en el primer error** | **Sigue de largo.** Uno termina con media base cargada **creyendo que todo salió bien** |
| **`-f /db/bdfacturas_postgres.sql`** | El archivo a ejecutar, **con la ruta de adentro** | Se queda esperando que usted escriba SQL a mano |

#### ¡OJO! El `-v` significa **dos cosas distintas** en el mismo comando

**No es un error del tutorial — es así, y confunde a cualquiera:**

| Dónde aparece | De quién es | Qué significa |
|---|---|---|
| `-v "${PWD}\db:/db:ro"` | **De Docker** | *volume* — **montar una carpeta** |
| `-v ON_ERROR_STOP=1` | **De `psql`** | *variable* — **fijar una opción** |

> **Lo que los distingue es la posición**, no la letra: el primero está **antes**
> del nombre de la imagen; el segundo, **después**. Cambiarlos de lugar no da un
> error claro — da uno confuso.

#### Dos detalles que parecen contradicciones

| | |
|---|---|
| **La imagen es `postgres:16` y Neon corre PostgreSQL 18** | **No importa:** lo único que usamos de la imagen es **el cliente**, y un `psql` 16 habla con un servidor 18 sin problema. Si quisiera el cliente exacto, cambie el `16` por `18` |
| **La cadena va en formato `postgresql://`** | **Aquí sí es el formato bueno:** es la que entiende `psql`. **La de .NET —`Host=...;Database=...`— es la del paso anterior**, y solo sirve para la API |

> **Esa es la confusión que viene:** la misma base, la misma contraseña, **dos
> formatos de cadena**, y cada herramienta acepta solo el suyo. Copiar la de .NET
> aquí produce un error de sintaxis que no explica nada.

#### Y por qué montar la carpeta en vez de mandar el archivo por la tubería

Sería más corto escribir `Get-Content archivo.sql | docker run -i ...`. **Y
sería un error.**

> **PowerShell reescribe el texto** al pasárselo a un programa externo: lo
> convierte a la codificación de la consola, que en Windows no es UTF-8. **Las
> tildes se partirían igual que con `sqlcmd`** — y esta vez por culpa de la
> tubería, no del archivo.
>
> **Montando la carpeta, el archivo nunca pasa por la consola:** `psql` lo abre
> él mismo, desde el disco, byte por byte.

#### La salida, cuando sale bien

```
CREATE TABLE        (x12)
INSERT 0 3 / 0 6 / 0 8 / 0 5 ...
 setval
--------
      5
CREATE FUNCTION
CREATE TRIGGER
CREATE PROCEDURE    (x16)
```

| Renglón | Qué es |
|---|---|
| **12 `CREATE TABLE`** | Las mismas doce de SQL Server |
| **`INSERT 0 n`** | El `0` es el OID — en desuso— y **`n` es cuántas filas entraron** |
| **`setval`** | **Alinea los contadores** de las columnas autonuméricas con los ids sembrados. Sin esto, el primer `INSERT` del sistema chocaría con una clave que ya existe |
| **1 `CREATE FUNCTION` + 1 `CREATE TRIGGER`** | El disparador de totales y stock |
| **16 `CREATE PROCEDURE`** | Los mismos dieciséis |

**Y ningún `ERROR`**, con `ON_ERROR_STOP=1` encendido: **el script escrito para
PostgreSQL 16 corrió tal cual en el 18.** El supuesto del paso anterior quedó
comprobado.

#### La comprobación que de verdad importa

```powershell
docker run --rm -e PGCLIENTENCODING=UTF8 postgres:16-alpine psql "postgresql://neondb_owner:SU_CLAVE@...neon.tech/neondb?sslmode=require" -t -c "SELECT count(*) FROM information_schema.tables WHERE table_schema='public'; SELECT codigo, nombre FROM persona ORDER BY codigo LIMIT 4;"
```

```
    12

 P001   | Ana Torres
 P002   | Carlos Pérez
 P003   | María Gómez
 P004   | Juan Díaz
```

**Doce tablas — y las tildes bien.**

> **Compare con lo que quedó en SQL Server:** `Carlos PA(c)rez`, `MarA-a
> GA3mez`. **El mismo contenido, el mismo origen, dos resultados distintos.**
>
> | | SQL Server, por el panel | **PostgreSQL, por `psql`** |
> |---|---|---|
> | Quién leyó el archivo | `sqlcmd`, **adivinando** la codificación | `psql`, **con `PGCLIENTENCODING=UTF8`** |
> | Resultado | Tildes partidas en dos | **Tildes correctas** |
> | ¿Avisó alguien? | **No** — dijo `Success` y `Errors 0` | No hizo falta |
>
> **La diferencia no fue el motor ni la herramienta: fue decirle la
> codificación antes, en vez de descubrirla después en una pantalla.**

**En la consola de Neon, `Tables` debe mostrar ahora las doce.**


### 8.6 · Las doce tablas, vistas en la consola

![La vista de tablas en Neon](docs/tutorial_publicar/img/60_neon_tablas.png)

`Postgres database` → **`Tables`**.

**La misma lista de SQL Server:** `cliente`, `empresa`, `factura`, `persona`,
`producto`, `productosporfactura`, `rol`, `rol_usuario`, `ruta`, `rutarol`,
`usuario`, `vendedor`.

**Y los datos adentro.** La tabla `cliente` muestra cuatro filas, con sus
columnas tipadas: `id serial`, `credito numeric`, `fkcodpersona varchar(10)`.

> **Un detalle que confirma que las semillas entraron idénticas:** los ids de
> `cliente` son **`1, 2, 3, 5`** — **falta el `4`**, igual que en SQL Server.
> **No es un error: es el mismo salto del script original.**
>
> **Y es justo lo que tenía que pasar.** Si los ids fueran distintos entre los
> dos motores, las dos APIs darían respuestas distintas a la misma petición
> —`GET /api/cliente/5` existiría en una y no en la otra— y la demostración se
> caería. Por eso el script trae `setval`: **alinea los contadores para que el
> próximo id también coincida.**

**Las tres últimas columnas —`empresa`, `persona`, `facturas`— no son datos:**
son los enlaces que Neon dibuja siguiendo las claves foráneas. Un clic lleva a
la fila relacionada.

### 8.7 · El archivo de configuración de la variante

#### Cuándo se crea: **ahora, antes de compilar**

Igual que con los otros dos:

| | |
|---|---|
| **1. Ahora** | Se crea `api_facturas/appsettings.Production.postgres.json` |
| **2. Después** | `dotnet publish` — que **no lo usa todavía** |
| **3. Y en seguida** | **Se copia encima** del `appsettings.Production.json` de la carpeta publicada |

> **Esa tercera línea es la que hace distinta a esta variante**, y se explica
> en el paso siguiente. Por ahora basta con que el archivo exista.

#### Dónde y cómo se crea

**En `api_facturas`**, al lado de los otros `appsettings`. **El atajo:** en el
repositorio ya está la plantilla

```
api_facturas/appsettings.Production.postgres.json.ejemplo
```

**Cópiela, péguela al lado, quítele el `.ejemplo` del nombre** y reemplace
`SU_CLAVE` por la contraseña que le dio Neon.

#### Cómo queda el archivo, completo

```json
{
  "_comentario": "ESTE ARCHIVO NO SE VERSIONA -esta en .gitignore- porque trae la contrasena real de Neon. La plantilla que si se versiona es appsettings.Production.postgres.json.ejemplo, al lado.",

  "_para_que_sirve": "Es la MISMA API publicada en otro sitio, hablandole a PostgreSQL en vez de SQL Server. No se compila nada distinto: al publicar, este archivo reemplaza al appsettings.Production.json dentro de la carpeta publicada. Mismo ApiFacturas.dll, mismo contrato, otro motor.",

  "_las_dos_claves": "Motor y ConnectionStrings:Postgres tienen que coincidir. Si deja Motor en sqlserver, la API lee la cadena de SQL Server -que aqui no existe- y falla con un error de conexion que no menciona PostgreSQL por ninguna parte.",

  "Motor": "postgres",

  "ConnectionStrings": {
    "_comentario": "Es la cadena de Neon, traducida al formato de Npgsql: pares Clave=Valor separados por punto y coma. El formato postgresql://... que muestra Neon es el de psql, y Npgsql NO lo entiende. El servidor lleva -pooler porque el Connection pooling esta encendido: la API abre y cierra conexion en cada peticion, y sin el pooler se agotan.",
    "Postgres": "Host=ep-calm-heart-b9e1zu5w-pooler.c-7.eu-central-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=SU_CLAVE;SSL Mode=Require"
  }
}
```

#### Renglón por renglón

| Clave | ¿La usa la API? | Para qué está |
|---|---|---|
| `_comentario` | **No** | Qué es el archivo y por qué no se versiona |
| `_para_que_sirve` | **No** | **La idea entera en tres renglones**: mismo `.dll`, otro motor |
| `_las_dos_claves` | **No** | El error que viene, escrito **antes** de cometerlo |
| **`Motor`** | **Sí** | Escoge el dialecto |
| `ConnectionStrings._comentario` | **No** | Por qué la cadena está en ese formato y no en el de Neon |
| **`ConnectionStrings.Postgres`** | **Sí** | La cadena |

#### Las dos claves que tienen que coincidir

| | |
|---|---|
| **`"Motor": "postgres"`** | **Sin ella la API sigue hablando T-SQL**, aunque la cadena sea de Neon |
| **`ConnectionStrings:`**`Postgres` | **El nombre importa: `Postgres`, no `SqlServer`.** El `Program.cs` lee las dos por separado, y **la que no corresponda al motor ni se mira** |

> **Cómo se ve el error si se equivoca:** con `"Motor": "sqlserver"` y la cadena
> de Neon, **la API arranca perfecto**, busca `ConnectionStrings:SqlServer`
> —que en este archivo no existe— y falla en la primera consulta con un error
> de conexión **que no menciona PostgreSQL por ninguna parte**. Uno revisa la
> cadena de Neon veinte veces antes de mirar la otra línea.

#### Al `.gitignore`

```
appsettings.Production.postgres.json
```

**Compruébelo:**

```powershell
git status
```

| Debe aparecer | No debe aparecer |
|---|---|
| `appsettings.Production.postgres.json`**`.ejemplo`** | `appsettings.Production.postgres.json` |

#### Ahora hay tres archivos de producción, y conviene no confundirlos

| Archivo | Apunta a | ¿Se versiona? |
|---|---|---|
| `api_facturas/appsettings.Production.json` | **SQL Server** en MonsterASP | **No** |
| `api_facturas/appsettings.Production.postgres.json` | **PostgreSQL** en Neon | **No** |
| `front_blazor/appsettings.Production.json` | **La API**, por su dirección pública | **Sí** — no tiene secretos |

#### Lo que NO hay que hacer

> **No cambie el `appsettings.Production.json` que ya funciona.** La tentación
> es editarlo, publicar, y ver si PostgreSQL responde. **Pero eso tumba la API
> de SQL Server** — y la gracia del paso es tener las dos vivas al mismo
> tiempo.

> **Y no ponga las dos cadenas en el mismo archivo** «por si acaso».
> Funcionaría —la API solo lee la que corresponda al `Motor`— **pero entonces
> el secreto de Neon viaja al sitio de SQL Server y al revés.** Cada sitio debe
> llevar **únicamente la credencial que necesita**: si uno se compromete, el
> otro no.


### 8.8 · El tercer sitio

**`Websites` → `Add website`**, el mismo asistente del Paso 5:

| Paso | Qué escoger |
|---|---|
| **Plan** | `Select FREE website` |
| **Domain** | **`apifacturas-pg-carloscastro`** → `.runasp.net` |
| **Location** | **Europa** — el mismo de todo lo demás |
| **Example** | `My own application` |

**El `-pg-` en el nombre es lo que las distingue.** Con tres sitios y
direcciones parecidas, esas tres letras son lo que evita publicar una encima de
la otra.

**La descripción:**

```
API facturas v4 - PostgreSQL/Neon - AySW 2026-2
```

**Y las credenciales:** `Deploy` → `Visual Studio access` → **`Enabled`**.

| | La de SQL Server | **La de PostgreSQL** |
|---|---|---|
| **Site / Login** | `site101125` | **`site101144`** |
| **Password** | `c+3LK6h#-iM4` | **`6La_Xy=93Nt?`** |

> **Tres sitios, tres identificadores, tres contraseñas.** Ninguna sirve en el
> otro sitio. Esa separación es a propósito: **si una se compromete, los otros
> dos siguen cerrados.**

### 8.9 · Publicar: tres comandos, y uno es el que importa

#### 1. Compilar — **exactamente lo mismo de siempre**

```powershell
cd ...\proyecto_aplicacion_y_servicios_web4\api_facturas
dotnet publish -c Release -o publicado_pg
```

```
Restauracion completada (0,6s)
  ApiFacturas net10.0 realizado correctamente (6,5s) -> publicado_pg\

Compilacion realizado correctamente en 8,3s
```

> **Fíjese en lo que NO tiene este comando:** ni una palabra sobre PostgreSQL.
> **Es el mismo `dotnet publish` del Paso 6.2**, cambiando únicamente la carpeta
> de salida —`publicado_pg` en vez de `publicado`— **para no pisar la otra**.
>
> **El `ApiFacturas.dll` que acaba de salir es idéntico al que ya está
> publicado.** Mismo código, misma compilación, mismos bytes.

#### 2. Cambiarle la configuración — **éste es el paso**

```powershell
copy appsettings.Production.postgres.json publicado_pg\appsettings.Production.json
```

**Lea los dos nombres con cuidado, porque son distintos a propósito:**

```
copy  appsettings.Production.postgres.json     publicado_pg\appsettings.Production.json
      |                                        |
      el archivo de PostgreSQL                 se llama como el de SIEMPRE
      -el que usted creó en el paso 8.7-       -el nombre que .NET va a buscar-
```

| | |
|---|---|
| **Por qué se renombra** | Porque **.NET busca `appsettings.Production.json` y nada más**. No sabe que existe una variante: **le llega un archivo con el nombre de siempre y un contenido distinto** |
| **Por qué después de compilar** | Porque `publish` **acaba de copiar ahí el de SQL Server**. Este comando lo reemplaza |
| **Por qué no se compila distinto** | **Porque no hace falta.** El motor no está en el código: está en una clave de configuración |

**El comando no responde nada**, y eso está bien: **en PowerShell el silencio
es éxito.** Si el archivo de origen no existiera, ahí sí habría un error en
rojo.

**Compruébelo**, que es más seguro que confiar:

```powershell
type publicado_pg\appsettings.Production.json
```

Debe decir **`"Motor": "postgres"`** y la cadena de Neon. **Si dice
`sqlserver`**, el `copy` no corrió — o corrió desde la carpeta equivocada.

> **Este comando de una línea es el curso entero.** Si la aplicación estuviera
> mal hecha —con SQL escrito dentro de los controladores, o con el motor
> decidido en tiempo de compilación— **esto sería imposible** y habría que
> mantener dos versiones del proyecto.

#### 3. Subir — al sitio nuevo

```powershell
& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -source:contentPath="...\api_facturas\publicado_pg" -dest:contentPath="site101144",computerName="https://site101144.siteasp.net:8172/msdeploy.axd?site=site101144",userName="site101144",password="6La_Xy=93Nt?",authType="Basic" -enableRule:AppOffline -allowUntrusted
```

**Antes de oprimir Enter, dos comprobaciones:**

| | |
|---|---|
| **`site101144` aparece cinco veces** | Una en `contentPath`, dos en `computerName`, una en `userName`, y el `site=` de la dirección |
| **La carpeta dice `publicado_pg`** | **Si se le cuela `publicado` a secas, sube la API de SQL Server** y los dos sitios quedan iguales — sin ningún error que lo avise |

> **Y un detalle de esta contraseña en particular:** `6La_Xy=93Nt?` **tiene un
> `=`**, que es justo el carácter que `msdeploy` usa para separar
> `nombre=valor`. **Va entre comillas y pasa bien** — pero si alguna vez
> `msdeploy` se queja de un parámetro desconocido, mire ahí primero.


### 8.10 · La trampa que nos cayó encima, y por qué usted no la va a sufrir

**La primera vez que publicamos esta variante, la salida traía esto:**

```
Info: Adding directory (site101144\publicado).
Info: Adding file (site101144\publicado\appsettings.Production.json).
Info: Adding file (site101144\publicado\web.config).
```

**Se subió una carpeta `publicado` dentro del sitio nuevo** — la salida del
Paso 6, **la de SQL Server**. Y adentro iba su `appsettings.Production.json`,
**con la contraseña de la base de datos de MonsterASP**.

> **O sea: el secreto de un sitio viajó al otro.** Exactamente lo que el paso
> 8.7 advertiía que no debía pasar — y pasó **sin que nadie lo escribiera en
> ninguna parte.**

#### Por qué ocurre

| | |
|---|---|
| **El SDK web de .NET** | Incluye en el paquete **todos los `.json` y el `web.config` de la carpeta del proyecto** |
| **Y `publicado` está dentro de `api_facturas`** | Porque así lo pidió el comando: `-o publicado` |
| **Entonces** | La segunda publicación **se lleva la primera adentro** |

> **Y se repite sola.** La tercera vez tendría `publicado\publicado`, y así.
> **Es una trampa clásica de .NET**, y no da ningún aviso: el paquete sale más
> grande y nadie lo nota.

#### El arreglo: cuatro renglones en el `.csproj`

**Ya están en el repositorio**, en `api_facturas/ApiFacturas.csproj`:

```xml
<ItemGroup>
  <Content Remove="publicado*/**" />
  <None Remove="publicado*/**" />

  <Content Remove="appsettings.Production.postgres.json*" />
  <None Remove="appsettings.Production.postgres.json*" />
</ItemGroup>
```

| Las dos primeras | **Las carpetas de salida no entran al paquete.** Da igual cómo las llame — `publicado`, `publicado_pg`, `publicado2` |
| Las dos últimas | **La variante de PostgreSQL no viaja con el paquete.** Se copia a mano después, y así **cada sitio lleva únicamente la credencial que usa** |

#### El estudiante no tiene que hacer nada de esto

**Ese arreglo se versiona.** Cuando alguien clone el repositorio, **ya viene
adentro** — publique una vez o publique diez, no se le anida nada.

| | Qué hace |
|---|---|
| **El estudiante** | **Nada.** Corre los tres comandos del paso 8.9 |
| **Quien publicó antes del arreglo** | Borrar a mano la carpeta que ya se le coló |

**Si le tocó lo segundo** —porque publicó antes de traer este cambio—:

```powershell
Remove-Item -Recurse -Force publicado_pg\publicado
copy appsettings.Production.postgres.json publicado_pg\appsettings.Production.json
```

> **Y después hay que volver a subir.** El servidor todavía tiene la carpeta
> intrusa — pero como `msdeploy` usa **`-verb:sync`**, al volver a publicar
> **la borra sola**: lo que no esté en el origen no sobrevive en el destino.
>
> **Es la misma propiedad que borró el `iisstart.htm` del Paso 6.3**, ahora
> trabajando a favor nuestro para limpiar un error.

#### La lección, que vale más que el arreglo

> **El error no fue de código ni de configuración: fue de «dónde dejo los
> archivos».** Publicar dentro de la carpeta del proyecto parecía cómodo — y
> **convirtió la salida de una publicación en la entrada de la siguiente**.
>
> **Y lo peor: ningún paso falló.** `dotnet publish` dijo que todo bien,
> `msdeploy` dijo `89 cambios`, el sitio arrancó. **Lo único que delataba el
> problema eran tres renglones en medio de ochenta y nueve.**


### 8.11 · Probarla — y la prueba que vale por todo el paso

**Tres comprobaciones, de menos a más exigente.**

#### 1. ¿Arranca, y con qué motor?

```
http://apifacturas-pg-carloscastro.runasp.net
```

```json
{
  "mensaje": "API Facturas funcionando",
  "version": "v4",
  "motor": "postgres",
  "contratos": "docs/spec_kit/versiones/v4_aplicativo/6_contracts.md"
}
```

| Si dice | |
|---|---|
| **`"motor": "postgres"`** | **El archivo de configuración llegó y manda** |
| `"motor": "sqlserver"` | El `copy` no se hizo — o se hizo **antes** de publicar, y `publish` lo pisó |

> **Esa línea es la única señal visible de todo el paso.** Lo demás de la
> respuesta es idéntico a la otra API.

> **La primera carga tarda**, y esta vez se despiertan **dos cosas dormidas**:
> el sitio en MonsterASP y la base en Neon. Son dos siestas encadenadas.

#### 2. ¿Contesta la base de datos? — el 401 a propósito

Igual que en el Paso 6.4: **intentar entrar con una clave que uno sabe que está
mal.**

```
POST /api/sesion   { "email": "admin@correo.com", "contrasena": "estaNoEs" }
```

```json
{"estado":401,"mensaje":"El correo o la contrasena no son correctos."}
```

> **Para poder decir eso, la API tuvo que ir hasta Frankfurt, buscar el correo y
> comparar la contraseña.** Un `500` habría significado que no conecta.

#### 3. La prueba del paso: **las dos APIs, lado a lado**

**Abra las dos en dos pestañas**, entre con `admin@correo.com` / `admin123` en
cada una, y pida lo mismo.

| | SQL Server · `site101125` | **PostgreSQL / Neon · `site101144`** |
|---|---|---|
| Sesión con `admin123` | token | **token** |
| `GET /api/cliente` → `total` | **4** | **4** |
| Los ids | **1, 2, 3, 5** | **1, 2, 3, 5** |
| El primer cliente | `{"id":1,"credito":520000.00,"fkcodpersona":"P001","fkcodempresa":"E001"}` | **idéntico** |
| El Swagger | `ApiFacturas 1.0 · OAS 3.0`, catorce controladores | **idéntico** |

> **Y eso es exactamente lo que se quería demostrar: no se nota la
> diferencia.** Mismo contrato, mismas rutas, mismo documento OpenAPI, mismas
> respuestas. **Las dos corren el mismo `ApiFacturas.dll`, byte por byte**, y lo
> único distinto entre ellas es **un archivo de texto de ocho renglones**.
>
> **Si la aplicación estuviera mal hecha, esto sería imposible.** Con SQL
> escrito dentro de los controladores, o con el motor decidido al compilar,
> habría que mantener dos versiones del proyecto. **El hecho de que una copia de
> archivo baste es la demostración de que las capas sirven para algo.**

#### Una diferencia que sí se ve, y no es un error

**Pida `GET /api/persona` en las dos:**

![La respuesta de persona en la API de PostgreSQL](docs/tutorial_publicar/img/62_persona_postgres.png)

```json
{
  "tabla": "persona",
  "total": 6,
  "datos": [
    { "codigo": "P001", "nombre": "Ana Torres",    "email": "ana.torres@correo.com" },
    { "codigo": "P002", "nombre": "Carlos Pérez",  "email": "carlos.perez@correo.com" },
    { "codigo": "P003", "nombre": "María Gómez",   "email": "maria.gomez@correo.com" },
    { "codigo": "P004", "nombre": "Juan Díaz",     "email": "juan.diaz@correo.com" }
  ]
}
```

| | |
|---|---|
| **PostgreSQL** | **`Carlos Pérez`, `María Gómez`, `Juan Díaz`** |
| **SQL Server** | `Carlos PA(c)rez`, `MarA-a GA3mez`, `Juan DA-az` |

**Es el defecto del Paso 7.6**, pendiente de reimportar. Y de paso, **la prueba
de que las dos bases se cargaron por caminos distintos**: una por el panel con
`sqlcmd` adivinando la codificación, la otra por `psql` con
`PGCLIENTENCODING=UTF8` declarado.

> **El mismo archivo de origen, dos resultados.** No fue el motor: fue avisar la
> codificación, o no avisarla.

### 8.12 · Lo que queda, y lo que esto deja enseñado

**Publicado:**

| | |
|---|---|
| **API SQL Server** | `https://apifacturas-carloscastro.runasp.net` |
| **API PostgreSQL** | `http://apifacturas-pg-carloscastro.runasp.net` |
| **Front** | `https://facturas-carloscastro.runasp.net` |
| **Las bases** | `db75266` en MonsterASP · `neondb` en Neon / Frankfurt |

**Pendiente, todo de panel y nada de código:**

| | |
|---|---|
| **1** | Reimportar `bdfacturas_publicar.sql` — arregla las tildes de SQL Server |
| **2** | `HTTPS` en el sitio de PostgreSQL: `Enable HTTPS`, y después el redirect |
| **3** | Al terminar la clase, **cambiar las contraseñas** que quedaron en las capturas |

#### Y las tres cosas que este paso deja probadas

| | |
|---|---|
| **El motor es configuración, no código** | Una copia de archivo cambia de SQL Server a PostgreSQL |
| **El contrato aguanta** | Las dos APIs responden lo mismo, campo por campo |
| **Los errores aparecen donde uno no mira** | Las tildes, el `401` por el redirect, la carpeta anidada — **ninguno dio un mensaje que dijera lo que pasaba** |


### 8.13 · El certificado del tercer sitio — y por qué no funciona en seguida

**Es el mismo procedimiento de los pasos 6.7 y 7.7**, en el tercer sitio:
`HTTPS` → `Enable HTTPS` → esperar a `Secured 1 of 1` → `HTTPS Redirect` →
`Enabled`.

![La seccion HTTPS del sitio de PostgreSQL, asegurado y redirigiendo](docs/tutorial_publicar/img/63_https_postgres.png)

```
Secured 1 of 1 domain      HTTPS redirect on
apifacturas-pg-carloscastro.runasp.net -> HTTPS
Let's Encrypt - renews automatically - expires in 60 days (22.12.2026)
```

#### Y aquí pasa algo que confunde: **la dirección no abre todavía**

Con el panel diciendo `Secured`, `https://apifacturas-pg-carloscastro.runasp.net`
**no responde**. Parece que algo quedó mal.

**No quedó mal.** Medido desde la misma máquina, en el mismo minuto:

```
http  -> 307 redirige a https://apifacturas-pg-carloscastro.runasp.net/   (bien)
https -> sin respuesta                                                     (todavia no)
```

**Y la comparación que lo prueba:** el sitio de SQL Server, encendido una hora
antes, **sí responde `200` por `https`** desde ese mismo sitio y en ese mismo
momento.

| Sitio | `https` | Cuándo se encendió |
|---|---|---|
| `apifacturas-carloscastro` | **`200`** | hace una hora |
| `apifacturas-pg-carloscastro` | **sin respuesta** | hace un momento |

#### Qué está pasando

| | |
|---|---|
| **El `307` prueba que la configuración ya se aplicó** | El servidor **sabe** que debe mandar a `https` |
| **Lo que falta es el enlace cifrado** | El servidor web tarda en **empezar a atender en el puerto 443 con el certificado nuevo** |
| **El panel se adelanta** | Dice `Secured 1 of 1` cuando el certificado **se emitió y se registró**, no cuando ya está sirviendo |

> **Son unos minutos.** Al volver a medir, la misma dirección respondió:
>
> ```
> https -> 200
> {"mensaje":"API Facturas funcionando","version":"v4","motor":"postgres", ...}
> ```

#### Cómo no perder media hora con esto

| Haga | En vez de |
|---|---|
| **Espere dos o tres minutos y recargue** | Volver a emitir el certificado |
| **Recargue con `Ctrl+F5`**, o en una ventana de incógnito | Creer que sigue roto — **el navegador recuerda los fallos de conexión un rato** |
| **Compruebe que el `http` dé `307`** | Mirar la configuración otra vez |

> **Ese `307` es el diagnóstico barato:** si el sitio **redirige** a `https`,
> la configuración está bien y **lo único que falta es tiempo**. Si respondiera
> `200` por `http`, entonces sí: la redirección no se aplicó.

> **Y vale la pena decirlo en voz alta en clase**, porque es un patrón que se
> repite con todo lo que se publica: **un panel que dice «listo» antes de que
> la cosa funcione no está mintiendo** — está informando de otro paso. Lo que
> hay que aprender es **cuál señal mirar mientras tanto.**


<!-- Se escriben uno a uno, con sus capturas, a medida que se validan. -->
