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

#### El archivo

**Ya quedó creado** en el proyecto, en dos versiones:

| Archivo | Qué es | ¿Va a GitHub? |
|---|---|---|
| `api_facturas/appsettings.Production.json`**`.ejemplo`** | **La plantilla**, con `SU_CONTRASENA` | **Sí** |
| `api_facturas/appsettings.Production.json` | **El de verdad**, con la contraseña puesta | **No** — está en `.gitignore` |

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

#### Y por qué ese archivo no va a GitHub

**Porque trae una contraseña de verdad.** La plantilla sí va — así cualquiera
sabe qué archivo hace falta y qué forma tiene— pero el que tiene el valor
puesto se queda en su disco. En `.gitignore` quedó esto:

```
appsettings.Production.json
```

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


<!-- Se escriben uno a uno, con sus capturas, a medida que se validan. -->
