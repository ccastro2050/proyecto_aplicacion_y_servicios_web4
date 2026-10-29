# Tutorial 2 — La misma API contra PostgreSQL, y el front en Flask

**Aquí se pone a prueba lo que el curso viene afirmando:** que la API no sabe
contra qué motor habla, y que la interfaz y el servicio son dos programas
distintos que se hablan por HTTP.

| | |
|---|---|
| **Una base PostgreSQL** | En **Neon**, gratis, en Frankfurt |
| **La MISMA API** | El mismo `ApiFacturas.dll`, **byte por byte**, publicado otra vez |
| **Una segunda interfaz** | **Flask + Jinja2**, en Python, hospedada en **Render** |

> **Lo único que cambia entre la API de SQL Server y la de PostgreSQL es un
> archivo de texto de ocho renglones.** Si eso funciona, las capas están bien
> puestas. Si no, el sitio donde falle es exactamente la capa que se coló.

### Antes de empezar

**Este tutorial da por hecho el [tutorial 1](TUTORIAL_1_SQLSERVER_BLAZOR.md).**
De él se reutilizan:

| | |
|---|---|
| **La cuenta de MonsterASP** | Con sitios creados y WebDeploy encendido |
| **La API publicada** | Y el hábito de `dotnet publish` + `msdeploy` |
| **El `appsettings.Production.json`** | Aquí se crea **una variante** de ese archivo |

**Lo que NO hace falta:** saber Python. El front en Flask **ya está escrito** en
el repositorio — lo que se aprende aquí es a **publicarlo**.

### Los tres tutoriales de esta serie

| | Qué publica | Dónde |
|---|---|---|
| [1](TUTORIAL_1_SQLSERVER_BLAZOR.md) | SQL Server + API en C# + front en Blazor | MonsterASP |
| **2 — éste** | **PostgreSQL + la misma API + front en Flask** | Neon · MonsterASP · Render |
| [3](TUTORIAL_PUBLICAR.md) | **Todo junto**, de punta a punta | los cuatro |

> **Sobre la numeración:** los pasos conservan el número que tienen en el
> [tutorial completo](TUTORIAL_PUBLICAR.md). **No empieza en 1 porque no es un
> documento aparte: es un recorte**, y así las referencias cruzadas —«como en
> el paso 6.7»— siguen llevando al mismo sitio en los tres.

---
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


### 8.14 · El front contra PostgreSQL — sin publicar nada

**La API de PostgreSQL ya responde, pero el front le sigue hablando a la de SQL
Server.** Para verlo contra el otro motor hay dos caminos:

| | Editar la configuración **en el servidor** | **Un cuarto sitio** |
|---|---|---|
| **Tiempo** | 30 segundos | 5 minutos |
| **¿Sobrevive a publicar?** | **No** | Sí |
| **¿Se ven los dos a la vez?** | **No** | Sí |
| **Para qué sirve** | **Comprobar ahora** | **Mostrar en clase** |

**Aquí hacemos el primero.** El segundo es el mismo procedimiento del Paso 7,
con otro sitio y otro `UrlApi`.

#### Los cinco pasos

| | |
|---|---|
| **1** | `Websites` → **`facturas-carloscastro`** → `Manage` → **`Files`** |
| **2** | Entre a `wwwroot` y escriba **`appsettings`** en el `Search` |
| **3** | En **`appsettings.Production.json`**, oprima **`Edit`** |
| **4** | Cambie la dirección por la de la API de PostgreSQL → **`Save file`** |
| **5** | `Overview` → **`Restart`** |

![El editor del panel con el UrlApi cambiado](docs/tutorial_publicar/img/64_editar_urlapi.png)

```json
"UrlApi": "https://apifacturas-pg-carloscastro.runasp.net"
```

> **El `Restart` no es opcional.** El front lee esa configuración **al
> arrancar**: sin reiniciar, sigue hablando con SQL Server aunque el archivo ya
> diga otra cosa.

#### Y un detalle del plan gratuito que confunde

**Después del `Restart` el sitio no queda reiniciado: queda apagado.** Sin
`AlwaysRunning`, **el proceso arranca con la primera petición**.

| Lo que parece | Lo que es |
|---|---|
| «Reinicié y el `AppPool uptime` sigue igual» | **La página del panel está vieja.** Recargue |
| «Tarda mucho en cargar» | **Es el arranque en frío.** La segunda vez vuela |
| «La pestaña que tenía abierta muestra datos viejos» | **Blazor Server mantiene un circuito abierto**, y el reinicio lo rompió. **Cierre esa pestaña** o recargue con `Ctrl+F5` |

#### Y así se ve el resultado

![El front publicado leyendo de PostgreSQL](docs/tutorial_publicar/img/65_front_postgres.png)

```
1   Ana Torres     Carlos Pérez     5,000,000   activa
2   María Gómez    Juan Díaz        1,250,000   activa
3   Laura Rojas    Pedro Castillo   2,030,000   activa
```

**Las mismas seis facturas, los mismos totales, la misma interfaz** — y **los
nombres con tilde**.

> **Hace un rato, esa misma pantalla en esa misma dirección decía `Carlos
> PA(c)rez` y `MarA-a GA3mez`.** El defecto que dejamos sin arreglar del lado de
> SQL Server **sirvió de marcador**: es la prueba, a simple vista, de que el
> front está leyendo de la otra base de datos.
>
> **Un defecto convertido en instrumento de medición.** Vale la pena decirlo en
> clase: cuando dos sistemas deberían ser idénticos, **cualquier diferencia
> sirve para distinguirlos** — incluso una que uno no quería tener.

#### Lo que acaba de quedar demostrado

```
front   (sin tocar el codigo)
   |  un archivo de texto
API     (el mismo ApiFacturas.dll, byte por byte)
   |  un archivo de texto
base    SQL Server  <->  PostgreSQL
```

| Lo que ve el usuario | Lo que cambió debajo |
|---|---|
| La misma interfaz, las mismas seis facturas, los mismos totales | **Toda la base de datos** |

> **Dos archivos de configuración separan un sistema sobre SQL Server de uno
> sobre PostgreSQL.** Si el código estuviera mal hecho —con SQL escrito en los
> controladores, o el motor decidido al compilar— **esto exigiría mantener dos
> proyectos.**

#### Para devolverlo

**El mismo `Edit`**, la dirección anterior
—`https://apifacturas-carloscastro.runasp.net`— y **`Restart`** otra vez.

> **Y recuerde lo que ya sabemos:** eso que editó **lo borra la siguiente
> publicación**. En su disco, `front_blazor/appsettings.Production.json` sigue
> diciendo SQL Server — así que **publicar el front lo devuelve solo.**


## Paso 9 — La segunda interfaz: Flask, y otro hospedaje

**El proyecto tiene dos interfaces contra la misma API:**

| | Lenguaje | Puerto local |
|---|---|---|
| `front_blazor` | **C#** · Blazor Server | `8099` |
| **`front_flask`** | **Python** · Flask + Jinja2 | **`8118`** |

> **Por qué existen las dos, y no es por gusto:** si la API tuviera algo de la
> interfaz adentro —HTML, sesiones, reglas de pantalla— **la segunda no podría
> existir**. Que dos programas en dos lenguajes consuman lo mismo sin tocar una
> línea de la API **es la demostración de que las capas están bien puestas.**

### 9.1 · El problema: MonsterASP no corre Python

**Y lo dice su propio panel.** En la vista general de cualquier sitio:

```
.NET   .NET 10.x/9.x/8.x/7.x/6.x/5.x/Core [x86]
PHP    --- [NONE]
```

| | |
|---|---|
| **MonsterASP es IIS para .NET** | Tiene Node.js para el ejemplo de Next.js, pero **Python no aparece por ninguna parte** |
| **Entonces el front Flask no cabe ahí** | No es cuestión de configurarlo: **el intérprete no está** |

> **Y eso es una lección del curso, no un contratiempo.** «Publicar en la nube»
> no es un solo sitio: **cada tecnología necesita un servidor que la sepa
> ejecutar.** Hasta aquí todo cabía en un hospedaje porque todo era .NET. El
> primer programa en otro lenguaje **obliga a salir a buscar.**

### 9.2 · Dónde podría ir: el panorama de 2026

**Esto cambió mucho este año**, así que las opciones se revisaron en octubre de
2026 — no de memoria:

| Plataforma | Estado | ¿Sirve? |
|---|---|---|
| **Render** | 750 horas/mes, corre un `Dockerfile`, publica desde GitHub. Se duerme a los 15 min | Sí, **con cuenta nueva** |
| **Hugging Face Spaces** | **En 2026 los Spaces de Docker pasaron a plan de pago.** *«Gradio and Docker Spaces require a paid plan»* | **No** |
| **Koyeb** | Docker gratis y sin dormirse… pero **pide tarjeta desde febrero de 2026** | No, para un curso |
| **Northflank** | Siempre encendido, pero **pide tarjeta** | No |
| **Fly.io** | **Ya no tiene plan gratuito** | No |
| **Back4App** | Sin tarjeta y con Docker, **pero el contenedor gratuito vive 60 minutos por despliegue** | Solo para una demostración en vivo |
| **PythonAnywhere** | Gratis, **pero bloquea las salidas HTTP** a sitios fuera de su lista blanca | **No** — no podría llamar a nuestra API |

> **El de PythonAnywhere es el error que más caro sale**, y por eso vale
> nombrarlo: el sitio *arranca*, la página *abre*, el login *falla* — porque la
> plataforma no deja salir hacia la API. **Un hospedaje puede correr su lenguaje
> y aun así no servirle.**

#### La conclusión, que es la lección

> **En octubre de 2026 casi no queda hospedaje Docker gratuito sin tarjeta.** Lo
> que en 2023 era normal —Heroku, Fly.io, Railway, Hugging Face— se fue
> cerrando uno por uno.
>
> **Esto no es un contratiempo del tutorial: es información del oficio.** Un
> proyecto que *depende* de un plan gratuito ajeno **tiene una fecha de
> vencimiento que no controla.**
>
> **Y de ahí el valor de que el front corra en Docker:** mudarlo de plataforma
> es **cambiar de hospedaje, no reescribir la aplicación**. El `Dockerfile` es
> el que vuelve esa mudanza barata.

### 9.3 · Un alto: ¿en MonsterASP usamos Docker?

**No. En ningún momento.** Y conviene detenerse aquí, porque es la confusión
que aparece apenas entran dos hospedajes distintos al mismo proyecto.

| Dónde | ¿Docker? | Qué pasa de verdad |
|---|---|---|
| **Su máquina** | **Sí** | `docker compose up` levanta cinco contenedores: las dos bases, la API y los dos fronts |
| **MonsterASP** | **No** | Usted compiló con `dotnet publish` y **subió los archivos** con `msdeploy`. **IIS los ejecuta directamente** |
| **Un hospedaje de contenedores** | **Sí** | La plataforma **lee el `Dockerfile`, construye la imagen** y corre el contenedor |

#### La prueba está en la lista de archivos que subió

**Vuelva al Paso 6.5**, a los 50 archivos de `wwwroot`: hay `.dll`, `web.config`
y los `appsettings`. **No hay ningún `Dockerfile`.**

> **MonsterASP no sabe que ese archivo existe.** El `Dockerfile` de
> `api_facturas` sirve únicamente para levantar la API **en su máquina**.

#### Por qué cada uno hace lo suyo

| | |
|---|---|
| **MonsterASP es un hospedaje de .NET** | **Ya tiene .NET 10 instalado.** No necesita que usted le diga cómo correr su aplicación: le basta el `web.config` que genera `publish` |
| **Un hospedaje de contenedores no sabe qué es su aplicación** | Puede ser Python, Go o Rust. **El `Dockerfile` es la receta**: «parta de `python:3.12-slim`, instale esto, arranque así» |

> **Y de ahí sale, en una frase, por qué el front Flask no cabía en
> MonsterASP:** ese hospedaje corre **lo que él tiene instalado**; uno de
> contenedores corre **lo que usted le describa**.
>
> **Dos filosofías distintas**, y el curso termina con un ejemplo vivo de cada
> una.

#### La tabla completa, para la clase

| | La API (C#) | El front Blazor (C#) | **El front Flask (Python)** |
|---|---|---|---|
| **En su máquina** | Contenedor | Contenedor | **Contenedor** |
| **Publicado en** | MonsterASP | MonsterASP | **pendiente** — ver 9.4 |
| **Cómo llegaría** | `publish` + `msdeploy` | `publish` + `msdeploy` | **la imagen del `Dockerfile`** |
| **Quién lo ejecuta** | IIS | IIS | **Docker** (local, hoy) |
| **¿Viaja el `Dockerfile`?** | **No** | **No** | **Sí — es lo único que viaja** |

### 9.4 · La cuenta en Render

```
https://render.com
```

**`Get started`**, arriba a la derecha.

![La pantalla de registro de Render](docs/tutorial_publicar/img/66_render_registro.png)

Ofrece **GitHub**, GitLab, Bitbucket, Google o correo.

> **Hágalo en una ventana de incógnito.** No es paranoía: la primera vez que
> intentamos esto, el navegador tenía sesión abierta con **tres cuentas de
> GitHub distintas** —`ccastro2050`, `carloscastroest` y `ccastro2050-50`— y
> Render se conectó con la que no era. **En incógnito no hay sesión previa que
> lo meta donde no quiere.**

> **Y una advertencia que costó una madrugada:** si ya tuvo una cuenta de Render
> con una factura sin pagar, **el espacio de trabajo queda suspendido y no deja
> ni crear uno nuevo**:
>
> ```
> Workspace suspended — Render has suspended your services due to non-payment.
> ```
>
> **La salida es una cuenta nueva, con otro correo.** Y de paso es lo correcto
> para escribir un tutorial: **la única forma de saber qué va a ver el
> estudiante es empezar como él.**

### 9.5 · El servicio

**`Deploy a Web Service`** → y ahí aparecen tres pestañas.

![La pestana de repositorio publico con la direccion pegada](docs/tutorial_publicar/img/67_render_repo_publico.png)

| Pestaña | Qué hace | |
|---|---|---|
| `Git Provider` | Render **se conecta a su cuenta** y republica con cada `git push` | Lo ideal |
| **`Public Git Repository`** | Se le pega la dirección y ya. **Sin conexión de cuenta** | **La que usamos** |
| `Existing Image` | Para una imagen Docker ya construida en un registro | No |

**Pegue la dirección del repositorio y oprima `Connect`:**

```
https://github.com/ccastro2050/proyecto_aplicacion_y_servicios_web4
```

> **Por qué la del medio:** entrando con Google, GitHub no queda conectado — y
> conectarlo abre otra vez el enredo de las cuentas. **El repositorio es
> público, así que Render lo clona sin pedirle permiso a nadie.**
>
> **Lo único que se pierde** es que republique solo: para subir una versión
> nueva hay que oprimir un botón. **Se puede conectar después, con calma.**

#### El formulario

![El formulario con el nombre y el lenguaje detectado](docs/tutorial_publicar/img/68_render_formulario.png)

**Render clona, mira el repositorio y detecta `Docker` solo** — porque
encuentra el `Dockerfile`. Eso no hay que escogerlo.

**Pero no oprima `Deploy` todavía:** abajo a la izquierda dice **`$7 / month`**.

#### Los cuatro campos que sí hay que tocar

![Region, Root Directory y el plan gratuito](docs/tutorial_publicar/img/69_render_opciones.png)

| Campo | Qué poner | Por qué |
|---|---|---|
| **`Name`** | **`facturas-flask`** | Queda en la dirección pública: `facturas-flask.onrender.com`. El que trae por defecto es el del repositorio entero |
| **`Region`** | **`Frankfurt (EU Central)`** | Viene en `Oregon (US West)`. **Su API está en Europa** — desde Oregón cada llamada cruza Estados Unidos **y** el Atlántico |
| **`Root Directory`** | **`front_flask`** | **El que más se olvida.** Sin él, Render busca el `Dockerfile` en la raíz del repositorio, y ahí no está |
| **`Compute`** | **`$0 / month · Free`** | Viene en el de 7 dólares |

> **`Root Directory` es un buscador, no una caja de texto.** Al escribir
> `front_flask` le va a ofrecer las carpetas de adentro —`front_flask/static/`,
> `front_flask/templates/`—. **No escoja ninguna**: la que sirve es
> `front_flask/`, que ya queda escrita arriba. Cierre la lista haciendo clic
> afuera.

> **Y compruebe el renglón de abajo:** debe decir **`front_flask/ .`** en
> `Dockerfile Path`. Es Render armando la ruta solo. **Si dice otra cosa, el
> `Root Directory` no quedó.**

#### Lo que dice el aviso del plan gratuito

> *«Free instances **spin down after periods of inactivity**. They do not support
> SSH access, scaling, one-off jobs, or persistent disks»*

| | |
|---|---|
| **Se duerme** | Igual que MonsterASP. **Pero despierta sola con la primera petición** |
| `0.1 CPU` · `512 MB RAM` | De sobra para un front que solo arma HTML |
| **Sin disco persistente** | **No importa aquí**: este front no guarda nada. Todo lo pide por HTTP |

> **Ese último punto no es casualidad: es la arquitectura pagándose sola.** Un
> front que guardara archivos o sesiones en disco **no cabría en un plan
> gratuito sin disco**. Éste cabe porque **no tiene estado que perder.**

**Y ahora sí: `Deploy web service`.**


### 9.6 · La compilación, leída

![El registro de la compilacion en Render](docs/tutorial_publicar/img/70_render_compilacion.png)

**Render clona, construye la imagen y la arranca.** El registro va saliendo en
vivo, y hay media docena de renglones que vale la pena entender — porque son
los que confirman que todo lo que preparamos sirvió.

#### 1. Construye el `Dockerfile`, paso por paso

```
#6 [1/5] FROM docker.io/library/python:3.12-slim
#8 [3/5] COPY requirements.txt .
#9 [4/5] RUN pip install --no-cache-dir -r requirements.txt
#10 [5/5] COPY . .
```

**Son exactamente las instrucciones del `front_flask/Dockerfile`**, en orden.
Render no sabe que esto es Flask: **sigue la receta.**

#### 2. Instala lo que pide `requirements.txt`

```
Successfully installed blinker-1.9.0 certifi-2026.10.23 ... flask-3.1.3
gunicorn-23.0.0 ... requests-2.34.2 ... werkzeug-3.1.9
```

> **Catorce paquetes, y nosotros solo pedimos tres** — `flask`, `requests` y
> `gunicorn`. Los otros once son **las dependencias de esos tres**: Jinja2 para
> las plantillas, Werkzeug para el servidor, `urllib3` y `certifi` para las
> peticiones HTTPS. **Eso es lo que significa «instalar un paquete».**

#### 3. Arranca con **gunicorn**, sin que nadie lo configure

```
==> Setting WEB_CONCURRENCY=1 by default, based on available CPUs in the instance
[INFO] Starting gunicorn 23.0.0
[INFO] Listening at: http://0.0.0.0:10000 (1)
[INFO] Booting worker with pid: 7
[INFO] Booting worker with pid: 8
```

**Aquí se cobran dos decisiones que tomamos antes de publicar nada:**

| Lo que dice el registro | Por qué |
|---|---|
| **`Starting gunicorn`**, no el servidor de Flask | La imagen trae **`DEPURAR=0`** por defecto. **Nadie puso esa variable en Render** |
| **`Listening at: http://0.0.0.0:10000`** | **Render impone el puerto con la variable `PORT`**, y `app.py` la lee **antes** que `PUERTO`. Por eso el mismo código corre en Docker local (8118) y aquí (10000) |

> **Si el `app.py` solo leyera `PUERTO`**, la aplicación arrancaría en el 8118,
> Render no encontraría nada en el 10000 y **reportaría el servicio como
> caído** — con la aplicación viva adentro. **Es el fallo más común al
> publicar en una plataforma de contenedores**, y el más desconcertante.

> **Y fíjese en la discusión silenciosa:** Render anuncia
> `WEB_CONCURRENCY=1`, pero arrancaron **dos** trabajadores (`pid: 7` y
> `pid: 8`). Es porque nuestro `Dockerfile` dice `--workers 2`, **y lo
> explícito gana sobre lo sugerido.**

#### 4. Está viva

```
==> Your service is live 🎉
==> Available at your primary URL https://facturas-flask.onrender.com
```

### 9.7 · Las variables de entorno

**El servicio ya corre, pero todavía no sirve** — y conviene entender por qué
antes de intentar entrar.

> **El front está buscando la API en `http://localhost:8035`**, que es el valor
> por defecto del código. **Dentro del contenedor de Render, `localhost` es el
> contenedor mismo**: ahí no hay ninguna API. Es el mismo malentendido que
> explica por qué el `docker-compose` usa `api-facturas` y no `localhost`.

**`Environment` → `Add Environment Variable`, dos veces:**

![Las dos variables de entorno](docs/tutorial_publicar/img/71_render_variables.png)

| KEY | VALUE |
|---|---|
| **`API_FACTURAS_URL`** | `https://apifacturas-carrloscastro.runasp.net` |
| **`CLAVE_SESION`** | Una cadena larga cualquiera |

> **`CLAVE_SESION` firma la cookie de sesión.** Sin ella, el front usa la clave
> de desarrollo **que está escrita en el código** — y cualquiera que lea el
> repositorio podría falsificar una sesión.

**Tres cosas que se equivocan al escribir la dirección:**

| | |
|---|---|
| **Con `https://` adelante** | Sin el esquema, `requests` no sabe qué hacer con eso |
| **Sin barra al final** | Con ella, el front arma `...runasp.net//api/sesion` — **dos barras**— y la API responde `404` |
| **Copiada, no escrita** | La de este ejemplo es `apifacturas-ca`**`rr`**`loscastro`, con **dos erres**. Una letra de más o de menos y el front no encuentra nada — **y el error no dice que sea un nombre mal escrito** |

**`Save, rebuild, and deploy`**, y Render reinicia solo.

#### Y la segunda compilación dura 3 segundos

```
#8 [3/5] COPY requirements.txt .      CACHED
#9 [4/5] RUN pip install ...          CACHED
#10 [2/5] WORKDIR /app                CACHED
#11 [5/5] COPY . .                    CACHED
```

**La primera tardó 31 segundos; ésta, 3.** Todo salió de la caché.

> **Y esto es el `Dockerfile` bien escrito, cobrándose solo.** Recuerde el
> comentario que trae adentro:
>
> *«Los requisitos PRIMERO y el código después: así Docker reusa la capa de los
> paquetes mientras `requirements.txt` no cambie»*
>
> **Como no cambió ningún archivo —solo una variable de entorno— ni siquiera
> hubo que reinstalar Flask.** Si el `Dockerfile` copiara todo el código antes
> de instalar, **cada cambio de una coma volvería a bajar los catorce
> paquetes.**

### 9.8 · La prueba: el sistema completo, en cuatro proveedores

```
https://facturas-flask.onrender.com
```

**Entre con `admin@correo.com` / `admin123` y abra `Personas`.**

**Comprobado desde la terminal, contra el sitio publicado:**

```
GET https://facturas-flask.onrender.com/e/persona   (con la sesion abierta)

Carlos Pérez
María Gómez
```

> **La primera carga tarda**, y esta vez hay **dos cosas dormidas en fila**: el
> servicio de Render y el sitio de MonsterASP. El propio panel lo advierte:
> *«Your free instance will spin down with inactivity, which can delay requests
> by **50 seconds or more**»*.

#### Lo que acaba de recorrer esa petición

```
su navegador
   |  https
front en Flask      Render, Frankfurt           (Python, en Docker)
   |  https
API                 MonsterASP, Europa          (C#, sobre IIS)
   |  red interna del centro de datos
base de datos       SQL Server                  (db75266)
```

| | |
|---|---|
| **Cuatro proveedores** | Render, MonsterASP, GitHub y —en la otra variante— Neon |
| **Dos lenguajes** | Python y C# |
| **Dos formas de publicar** | **Docker** en Render, **archivos compilados** en MonsterASP |
| **Un solo sistema** | Y ninguna de las piezas sabe cómo está hecha la otra |

> **Eso es lo que hace posible que las piezas se hablen por contratos y no por
> dentro.** El front en Flask no sabe que la API es de C#; la API no sabe que
> hay dos fronts; la base de datos no sabe que existe internet.
>
> **Y la prueba de que no es un discurso:** este mismo front, cambiando **una
> variable de entorno**, le habla a la API de PostgreSQL en vez de a la de SQL
> Server. **Sin tocar una línea de código ni volver a compilar.**


---

## Lo que quedó demostrado

| | |
|---|---|
| **El motor es configuración, no código** | Una copia de archivo cambia de SQL Server a PostgreSQL |
| **El contrato aguanta** | Las dos APIs responden lo mismo, campo por campo |
| **La interfaz es intercambiable** | Dos fronts en dos lenguajes, contra la misma API |
| **Y el hospedaje también** | Lo que corre en Docker se muda de plataforma sin reescribir nada |
