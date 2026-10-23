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

![La lista de bases vacía: 0 databases found](docs/tutorial_publicar/img/07_cuenta_limpia.png)

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

## Pasos 2 a 5 — pendientes

<!-- Se escriben uno a uno, con sus capturas, a medida que se validan. -->
