# Tutorial 3 — La misma API contra PostgreSQL, con el front en Blazor

**La misma interfaz que ya conoce, leyendo de otro motor de base de datos.**

| | |
|---|---|
| **Una base PostgreSQL** | En **Neon**, gratis, en Frankfurt |
| **La MISMA API** | El mismo `ApiFacturas.dll`, **byte por byte**, publicado otra vez |
| **El MISMO front** | El mismo `FrontFacturas.dll`, publicado otra vez |

> **Antes de empezar: los nombres de las capturas no van a ser los suyos — ni
> son los del sistema que está vivo hoy.**
>
> Las capturas de esta serie se tomaron en la **primera vuelta** de la cuenta,
> cuando el usuario era `carloscastro` y la base de datos era `db75266`. Esa
> cuenta se reconstruyó después, así que **las direcciones que aparecen en las
> capturas ya no responden**. Comprobado el 29 de octubre de 2026:
>
> | Lo que está vivo hoy | Qué es |
> |---|---|
> | `https://apifacturas-carrloscastro.runasp.net` | La API contra **SQL Server** |
> | `https://apifacturas-pg-carrloscastro.runasp.net` | La **misma** API contra **PostgreSQL** en Neon |
> | `https://front-facturas-react-carrloscastro.runasp.net` | El front de **React** |
>
> La dirección del front de **Blazor publicado no está registrada en ningún
> archivo del repositorio**: hay que leerla del panel — y en este tutorial
> hace falta, porque el cuarto sitio se compara **contra el front que ya está
> arriba**.

> **Al terminar va a tener dos sistemas idénticos, abiertos en dos pestañas.**
> Las mismas pantallas, las mismas facturas, los mismos totales — **y abajo,
> dos motores de base de datos distintos.**
>
> **Lo único que los separa son dos archivos de texto.** Uno le dice a la API
> contra qué motor hablar; el otro le dice al front a cuál API pedirle los
> datos. **Ni una línea de código cambia, y no se compila nada distinto.**

### Antes de empezar

**Este tutorial da por hecho el [tutorial 1](TUTORIAL_1_SQLSERVER_BLAZOR.md).**
De él se reutilizan la cuenta de MonsterASP, la API ya publicada y el hábito
de `dotnet publish` + `msdeploy`.

### Esta serie son cuatro documentos

| | Motor | Interfaz | Dónde |
|---|---|---|---|
| [**1**](TUTORIAL_1_SQLSERVER_BLAZOR.md) | SQL Server | Blazor | MonsterASP |
| [**2**](TUTORIAL_2_POSTGRES_FLASK.md) | PostgreSQL | **Flask** | Neon · MonsterASP · Render |
| **3 — éste** | PostgreSQL | Blazor | Neon · MonsterASP |
| [**completo**](TUTORIAL_PUBLICAR.md) | los dos | las dos | los cuatro |

> **El 2 y el 3 comparten toda la primera mitad** — la base en Neon y la API
> contra PostgreSQL son las mismas. **Se separan al final**, cuando cada uno
> publica su interfaz.

> **Sobre la numeración:** los pasos conservan el número del
> [tutorial completo](TUTORIAL_PUBLICAR.md). **No empieza en 1 porque no es un
> documento aparte: es un recorte**, y así las referencias cruzadas siguen
> llevando al mismo sitio en los cuatro.

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


### 8.14 · El front Blazor contra PostgreSQL

**La API de PostgreSQL ya responde, pero el front le sigue hablando a la de SQL
Server.** Falta publicar una segunda interfaz que apunte a la otra.

#### Dos caminos, y por qué escogemos el segundo

| | Editar la configuración **en el servidor** | **Un cuarto sitio** |
|---|---|---|
| **Tiempo** | 30 segundos | 5 minutos |
| **¿Sobrevive a publicar?** | **No** — `sync` lo borra | Sí |
| **¿Se ven los dos a la vez?** | **No** | **Sí** |
| **Para qué sirve** | Comprobar ahora | **Mostrar en clase** |

> **El primero funciona** —`Files` → `wwwroot` → editar `UrlApi` → `Restart`—
> y sirve para salir del paso. **Pero lo que cambie en el servidor lo borra la
> siguiente publicación**, y además deja un solo front: para comparar hay que
> estar cambiando el archivo de un lado al otro.
>
> **La demostración que vale es tener los dos abiertos al mismo tiempo.**

#### 1. El archivo de la variante

**En `front_blazor`, al lado del que ya existe**, el repositorio trae:

```
front_blazor/appsettings.Production.json            -> la API de SQL Server
front_blazor/appsettings.Production.postgres.json   -> la API de PostgreSQL
```

**Su contenido útil es una sola línea:**

```json
{
  "UrlApi": "https://apifacturas-pg-SUNOMBRE.runasp.net"
}
```

| | |
|---|---|
| **Cambie la dirección** | Por la del sitio de la API de PostgreSQL que creó en el paso 8.8 |
| **Éste sí se versiona** | **No tiene un solo secreto**: solo una dirección pública |
| **Con `https://` y sin barra final** | Lo mismo de siempre |

> **Y el error que no avisa**, escrito en el propio archivo: si pone por
> descuido la dirección de la API de SQL Server, **el front arranca igual y
> muestra datos iguales** — porque los dos motores tienen las mismas filas.
> **La demostración queda en nada sin que nada falle.**

#### 2. El cuarto sitio

**`Websites` → `Add website`**, el mismo asistente del paso 5:

| Paso | Qué escoger |
|---|---|
| **Plan** | `Select FREE website` |
| **Domain** | **`front-facturas-pg-SUNOMBRE`** → `.runasp.net` |
| **Location** | **Europa** |
| **Example** | `My own application` |

**Y después:** `Deploy` → `Visual Studio access` → **`Enabled`**, y anótese el
`siteXXXXX` y la contraseña nuevos.

> **Van cuatro sitios de los cinco del plan gratuito.** Queda uno libre.

#### 3. Los tres comandos

**Son los del paso 7, con la carpeta y el sitio cambiados** — y con el `copy`
del medio, que es el que hace la diferencia:

```powershell
cd ...\proyecto_aplicacion_y_servicios_web4\front_blazor

dotnet publish -c Release -o ..\publicados\front_pg

copy appsettings.Production.postgres.json ..\publicados\front_pg\appsettings.Production.json

& "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -source:contentPath="C:\...\publicados\front_pg" -dest:contentPath="siteXXXXX",computerName="https://siteXXXXX.siteasp.net:8172/msdeploy.axd?site=siteXXXXX",userName="siteXXXXX",password="SU_CONTRASENA",authType="Basic" -enableRule:AppOffline -allowUntrusted
```

**Lea el segundo con cuidado, porque es el mismo truco de la API:**

```
copy  appsettings.Production.postgres.json   ...\front_pg\appsettings.Production.json
      |                                      |
      el de PostgreSQL                       se llama como el de SIEMPRE
```

> **.NET busca `appsettings.Production.json` y nada más.** No sabe que existe
> una variante: **le llega un archivo con el nombre de siempre y un contenido
> distinto.**

> **Y fíjese en la carpeta de salida:** `..\publicados\front_pg`, **fuera de
> `front_blazor`**. Publicar dentro de la carpeta del proyecto es lo que nos
> metió en el problema del paso 8.10 — y en el front tendría el mismo efecto.

#### 4. El certificado

**`HTTPS` → `Enable HTTPS` → esperar a `Secured 1 of 1` → `HTTPS Redirect`
→ `Enabled`**, igual que en los pasos 6.7 y 7.7.

> **Y acuérdese de lo del paso 8.13:** el panel dice `Secured` **antes de que
> el enlace funcione**. Si `https` no abre, espere dos minutos y recargue con
> `Ctrl+F5`. **Si el `http` ya responde `307`, la configuración está bien y lo
> único que falta es tiempo.**

### 8.15 · La prueba: los dos, lado a lado

**Abra las dos interfaces en dos pestañas** y entre en las dos con
`admin@correo.com` / `admin123`:

| Pestaña | Lee de |
|---|---|
| `front-facturas-SUNOMBRE.runasp.net` | **SQL Server** |
| `front-facturas-pg-SUNOMBRE.runasp.net` | **PostgreSQL** |

**Vaya a `Facturas` en las dos.** Mismas seis facturas, mismos clientes, mismos
vendedores, mismos totales.

#### Y cómo saber cuál es cuál, sin fiarse de la pestaña

**Abra `/` —la página de diagnóstico— en cada API:**

```json
{"mensaje":"API Facturas funcionando","version":"v4","motor":"sqlserver", ...}
{"mensaje":"API Facturas funcionando","version":"v4","motor":"postgres",  ...}
```

**Esa palabra —`motor`— es lo único que los distingue desde afuera.**

> **Y ahí está todo el argumento del curso, en una pantalla:** si dos sistemas
> hechos con el mismo código, sobre dos motores distintos, responden **lo
> mismo**, entonces **el código no depende del motor**. Eso no se demuestra con
> un diagrama: se demuestra abriendo dos pestañas.
>
> **Y si alguna pantalla difiere**, no es una curiosidad: **es un sitio donde el
> dialecto de un motor se coló hasta arriba.** El tutorial completo cuenta uno
> de esos — las tildes del paso 7.6.

---

## Lo que quedó demostrado

| | |
|---|---|
| **El motor es configuración, no código** | Un archivo de ocho renglones separa SQL Server de PostgreSQL |
| **La interfaz tampoco sabe** | El mismo `FrontFacturas.dll`, publicado dos veces, contra dos APIs |
| **Y se puede ver, no solo contar** | Dos pestañas abiertas, idénticas, con motores distintos debajo |

**Si quiere además una interfaz en otro lenguaje** —Python, en otro
hospedaje— ese es el
[**tutorial 2**](TUTORIAL_2_POSTGRES_FLASK.md).
