# Tutorial 4 — La misma API contra PostgreSQL, con el front en React

**El cuarto y último de la serie, y el que cierra la matriz.** Al terminar va
a tener el mismo sistema que ya conoce, con el mismo código, apuntando al otro
motor de base de datos — y va a haber descubierto, por el camino, que **este
front no se configura como los otros dos**.

| | |
|---|---|
| **Una base de datos PostgreSQL** | En **Neon**, la que ya creó en el tutorial 2 o 3 |
| **La MISMA API** | El mismo `ApiFacturas.dll`, publicado contra PostgreSQL |
| **El MISMO front de React** | El mismo código, **compilado otra vez** — y esa diferencia es el tema de este tutorial |

> **Este documento no tiene capturas de pantalla.** Los tutoriales 1, 2 y 3
> van pantalla por pantalla del panel del hospedaje; este va por los comandos
> y por el razonamiento. Las pantallas que necesita ya las conoce de los otros
> tres: es el mismo panel, los mismos botones.
>
> Versión 0.1 · 29 de octubre de 2026.

> **Antes de empezar: los nombres que aparecen aquí no van a ser los suyos.**
> Comprobado el 29 de octubre de 2026, lo que está vivo es esto:
>
> | Lo que está vivo hoy | Qué es |
> |---|---|
> | `https://apifacturas-carrloscastro.runasp.net` | La API contra **SQL Server** |
> | `https://apifacturas-pg-carrloscastro.runasp.net` | La **misma** API contra **PostgreSQL** en Neon |
> | `https://front-facturas-react-carrloscastro.runasp.net` | El front de **React** contra **SQL Server** |
>
> A cada quien le toca su propio nombre de sitio. Lo que importa es **de dónde
> se lee cada dato**, no cuál nombre salió.

---

## Dónde encaja este tutorial

La serie cubre cuatro combinaciones de motor e interfaz. Con este, la tabla
queda completa:

| | SQL Server | PostgreSQL |
|---|---|---|
| **Blazor** | [Tutorial 1](TUTORIAL_1_SQLSERVER_BLAZOR.md) | [Tutorial 3](TUTORIAL_3_POSTGRES_BLAZOR.md) |
| **Flask** | — | [Tutorial 2](TUTORIAL_2_POSTGRES_FLASK.md) |
| **React** | ya publicado | **este documento** |

> **Qué se da por hecho.** Que ya hizo el **tutorial 1** —existe la cuenta, la
> API publicada y el hospedaje— y el **tutorial 2 o el 3**, porque de ahí sale
> la base de datos PostgreSQL en Neon y la segunda API. Si no los ha hecho,
> este tutorial no tiene de dónde agarrarse.

### Lo que este tutorial enseña y los otros tres no pueden

Los tres fronts del proyecto son la misma aplicación. Pero **cambiar de motor
de base de datos no cuesta lo mismo en los tres**, y la razón no es un detalle
de programación: es una consecuencia de *dónde corre* cada uno.

| Para apuntar el front al otro motor… | Qué hay que hacer | Cuánto tarda |
|---|---|---|
| Front en **Blazor** | Editar `appsettings.Production.json` **en el servidor** y reiniciar | segundos |
| Front en **Flask** | Cambiar una variable **en el panel de Render** y reiniciar | segundos |
| Front en **React** | **Volver a compilar y volver a subir** | un par de minutos |

**Por qué.** Blazor y Flask corren **en un servidor**: cuando arrancan, leen
un archivo o una variable del sistema y ahí se enteran de a qué API le hablan.
Cambie el archivo, reinicie, y el mismo programa publicado sirve para los dos
motores.

React no corre en ningún servidor: **corre en el navegador de la persona**. No
hay nadie ahí que pueda leer una variable de entorno. Entonces Vite —el
compilador— toma el valor **al compilar** y lo escribe *dentro* del JavaScript
que produce. Lo que llega al navegador no es un programa que consulta su
configuración: es un programa con la dirección ya escrita adentro.

> **Esto no es un defecto de React ni algo que se pueda arreglar.** Es el
> precio de que el front sean **archivos estáticos**: por eso no necesita
> memoria, ni Node, ni se duerme, ni arranca en frío. Se paga en flexibilidad
> lo que se ahorra en recursos — y saber qué se está pagando es justamente lo
> que distingue a quien elige una tecnología de quien la copia.

---

## Paso 10.1 — Comprobar antes de tocar: ¿la API de PostgreSQL acepta este front?

**Esto es lo primero, y no es un trámite.** Antes de compilar nada, hay que
saber si la API contra PostgreSQL está en condiciones de recibir llamadas
desde un navegador. Y lo más probable es que **no lo esté** — por una razón
que vale la pena entender.

### Qué es lo que hay que comprobar

Cuando una página de un origen —`https://mi-front.ejemplo`— llama a otro
origen —`https://mi-api.ejemplo`—, **el navegador no manda la llamada de una**.
Primero manda una pregunta: un `OPTIONS` que dice *«vengo de este origen,
quiero hacer un POST, ¿me deja?»*. Eso se llama **preflight** —vuelo de
reconocimiento— y la API tiene que responder **con una cabecera** que diga
explícitamente que sí.

Si esa cabecera no viene, el navegador **bloquea la llamada antes de hacerla**.
Y aquí está lo que confunde a todo el mundo la primera vez: **en la API no
queda ni rastro**. No hay un error en el registro, no hay un 403, no hay nada
— porque la petición nunca salió. El error solo se ve en la consola del
navegador.

### El comando

Abra **PowerShell** y pegue esto, cambiando las dos direcciones por las suyas:

```powershell
$api    = 'https://apifacturas-pg-carrloscastro.runasp.net'
$origen = 'https://front-facturas-react-carrloscastro.runasp.net'

Invoke-WebRequest -Uri "$api/api/sesion" -Method Options -Headers @{
    'Origin'                         = $origen
    'Access-Control-Request-Method'  = 'POST'
    'Access-Control-Request-Headers' = 'content-type'
} -UseBasicParsing
```

**Qué hace, renglón por renglón:**

| | |
|---|---|
| `-Method Options` | Manda **el preflight**, no la llamada de verdad. Es exactamente lo que haría el navegador antes de iniciar sesión |
| `Origin` | Dice de qué origen viene. Es la cabecera que la API examina para decidir |
| `Access-Control-Request-Method` | Anuncia que la llamada real va a ser un `POST` |
| `Access-Control-Request-Headers` | Anuncia que va a mandar `Content-Type`. Si la API no permite esa cabecera, tampoco deja pasar |
| `-UseBasicParsing` | Que no intente interpretar el HTML de la respuesta. Aquí no hay HTML, y sin esto PowerShell puede quejarse |

> **Por qué `Invoke-WebRequest` y no `Invoke-RestMethod`.** El segundo
> devuelve solo el contenido; aquí **el contenido no importa** —está vacío—.
> Lo que importa son **las cabeceras**, y esas solo las entrega
> `Invoke-WebRequest`.

### Las tres respuestas posibles, y qué significa cada una

| Lo que sale | Qué significa | Qué hacer |
|---|---|---|
| **`204`** y una cabecera `Access-Control-Allow-Origin` con su origen | **Todo bien.** La API conoce este front y lo acepta | Siga al paso 10.3 |
| **`204`** pero **sin** esa cabecera | La API **tiene CORS configurado**, pero **este origen no está en la lista**. Le contestó amablemente «no» | Paso 10.2: agregar el origen |
| **`405 Método no permitido`** | La API **no tiene CORS en absoluto**. El `OPTIONS` ni fue atendido: se fue derecho al controlador, que solo acepta `POST`, y respondió «ese verbo aquí no» | Paso 10.2 **completo**: hay que volver a publicar la API |

> **El `204` sin cabecera es el caso más engañoso de los tres**, y conviene
> mirarlo despacio: la respuesta **se ve perfecta**. Código de éxito, sin
> errores, sin demora. Lo que falla es **lo que no está ahí**. Es el mismo
> razonamiento del tutorial 1 con `sqlcmd`: que diga `Success` y `Errors 0` no
> prueba que lo que usted quería haya pasado.

### Lo que salió cuando se escribió este tutorial

Las dos APIs, el mismo día, con el mismo comando:

```
=== API contra SQL SERVER ===
  front-facturas-react-carrloscastro  ->  204   Allow-Origin: https://front-facturas-react-carrloscastro.runasp.net
  site101654.siteasp.net              ->  204   Allow-Origin: (ninguna)

=== API contra POSTGRES ===
  front-facturas-react-carrloscastro  ->  405 Método no permitido
```

**Tres cosas se leen ahí, y las tres enseñan algo:**

1. **La API de SQL Server funciona**, y acepta exactamente un origen: el del
   front de React. Lo demuestra la cabecera con el nombre completo.
2. **La misma API dice «no» al sitio desconocido** con un `204` limpio y sin
   cabecera. No es un error: es una negativa correcta, y el navegador la
   entiende como bloqueo.
3. **La API de PostgreSQL da `405`**, o sea que **no tiene CORS**. Y aquí está
   el detalle que importa:

> **La configuración ya estaba puesta. El programa publicado era de antes.**
>
> En el disco, `api_facturas/appsettings.Production.postgres.json` ya traía la
> lista `OrigenesPermitidos` con el front de React adentro. Pero el
> `ApiFacturas.dll` que está **publicado** en ese sitio se subió **antes** de
> que la API tuviera CORS: antes de que `Program.cs` llamara a `AddCors` y a
> `UseCors`.
>
> Y un programa **no puede obedecer una configuración que no sabe leer**. El
> archivo estaba ahí, correcto y completo, y el programa pasó de largo porque
> no tiene una línea que lo busque.
>
> **Esta es la clase de defecto que solo aparece publicado**, y la razón de
> que este paso vaya primero: en su máquina todo funciona, porque ahí sí está
> compilado el código nuevo.

---

## Paso 10.2 — Poner a punto la API de PostgreSQL

Dos cosas, en este orden: el origen del front nuevo en la lista, y **la API
publicada otra vez** con el programa que sabe leer esa lista.

### 10.2.1 — Decidir la dirección del front nuevo

El front que va a compilar necesita un sitio donde vivir, y **la dirección de
ese sitio es un dato que hace falta antes de compilar** — porque va a ir
escrita en la lista de la API.

En el panel del hospedaje, **Websites**, use un sitio libre o cree uno nuevo
(el procedimiento está en el paso 1 del [tutorial 1](TUTORIAL_1_SQLSERVER_BLAZOR.md)).
Anote dos datos de ese sitio:

| Dato | Dónde sale | Marcador que usa este tutorial |
|---|---|---|
| El nombre del sitio | La lista de **Websites** | `<SITIO_FRONT_PG>` |
| Su contraseña de WebDeploy | **Websites → el sitio → WebDeploy** | `<CLAVE_FRONT_PG>` |
| Su dirección pública | La misma pantalla del sitio | `<URL_FRONT_PG>` |

> **Si el sitio no tiene todavía un nombre bonito**, su dirección por defecto
> —algo como `https://site101654.siteasp.net`— **funciona igual de bien**. CORS
> no distingue entre una dirección bonita y una fea: compara el texto exacto,
> carácter por carácter.

### 10.2.2 — Agregar el origen a la lista de la API

Abra `api_facturas/appsettings.Production.postgres.json` y deje la lista con
**las dos** direcciones:

```json
{
  "_comentario_cors": "Quien puede llamar a esta API DESDE UN NAVEGADOR. Solo hace falta para fronts que corren en el navegador; los que arman el HTML en el servidor -Blazor, Flask- no aparecen aqui.",
  "OrigenesPermitidos": [
    "http://localhost:8119",
    "<URL_FRONT_PG>"
  ],

  "Motor": "postgres",

  "ConnectionStrings": {
    "Postgres": "Host=<SERVIDOR_NEON>; Database=<BASE>; Username=<USUARIO_BD>; Password=<CLAVE_BD>; SSL Mode=Require; Trust Server Certificate=True;"
  }
}
```

**Tres cosas de esa lista que cuestan un rato de depuración si se pasan por
alto:**

| | |
|---|---|
| **Va el origen, no la página** | `https://mi-front.ejemplo`, **sin** barra al final y **sin** `/login`. Un origen es esquema + nombre + puerto, nada más. Con una barra de más, la comparación falla y el navegador bloquea |
| **`https` y `http` son origenes distintos** | Si su sitio responde por los dos, y usted solo puso uno, el otro queda bloqueado |
| **`localhost:8119` se queda** | Es el front cuando usted lo corre en Docker. Quitarlo rompería el desarrollo sin arreglar nada |

> **Y lo de siempre con este archivo: NO se versiona.** Está en `.gitignore`
> porque trae la contraseña de la base de datos de Neon. En este tutorial las
> credenciales van como marcadores por la misma razón.

### 10.2.3 — Volver a publicar la API

Esto es lo que arregla el `405`. Dos comandos, y conviene entender qué hace
cada uno porque hacen cosas muy distintas.

**Primero, compilar:**

```powershell
cd <la carpeta del proyecto>\api_facturas
dotnet publish -c Release -o publicados\api_pg
```

| | |
|---|---|
| `dotnet publish` | Compila **y** deja en una carpeta todo lo que hace falta para correr: el `.dll`, sus dependencias y los `appsettings` |
| `-c Release` | En modo **Release**: optimizado y sin la información de depuración. Es lo que va a un servidor |
| `-o publicados\api_pg` | Dónde dejarlo. **Esta carpeta importa que esté fuera de `api_facturas`** o, por lo menos, que esté excluida del proyecto — si queda adentro, `dotnet watch` entra en un ciclo de recompilación sin fin, y una publicación anterior puede filtrarse dentro de la siguiente |

> **Lo que acaba de pasar y es el punto de todo el tutorial:** este `.dll` es
> **el mismo** que el de la API contra SQL Server. Byte por byte. Lo único que
> los distingue es el `appsettings.Production.postgres.json` que se copia al
> lado. **El programa no sabe contra qué motor habla**, y por eso el mismo
> binario sirve para los dos.

**Ahora, poner el archivo de configuración correcto.** El `publish` copió
`appsettings.Production.json` —el de SQL Server—, así que hay que reemplazarlo
por el de PostgreSQL con **el nombre que la API va a buscar**:

```powershell
copy appsettings.Production.postgres.json publicados\api_pg\appsettings.Production.json
```

> **Por qué se renombra.** ASP.NET Core lee `appsettings.json` y **encima**
> `appsettings.{Entorno}.json`. En el hospedaje nadie fija
> `ASPNETCORE_ENVIRONMENT`, y cuando no se fija, el entorno es **Production**.
> Entonces el único nombre que la API va a leer es
> `appsettings.Production.json` — el sufijo `.postgres` es para que **usted**
> distinga los dos archivos en su disco, no para que el programa los
> distinga.

**Y subir:**

```powershell
cmd.exe /c '"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -source:contentPath="<la carpeta del proyecto>\api_facturas\publicados\api_pg" -dest:contentPath="<SITIO_API_PG>",computerName="https://<SITIO_API_PG>.siteasp.net:8172/msdeploy.axd?site=<SITIO_API_PG>",userName="<SITIO_API_PG>",password="<CLAVE_API_PG>",authType="Basic" -enableRule:AppOffline -allowUntrusted'
```

| | |
|---|---|
| `cmd.exe /c '...'` | **No es adorno.** PowerShell reescribe los argumentos de un programa externo y `msdeploy` los rechaza con *«Todos los argumentos deben comenzar con -»*. Pasándolo por `cmd.exe` el comando llega tal cual se escribió |
| `-verb:sync` | **Sincronizar**: el destino queda **idéntico** al origen. Lo que esté en el servidor y no en su carpeta, **se borra** |
| `-enableRule:AppOffline` | Pone un `app_offline.htm` mientras copia, para que IIS suelte el `.dll` y no falle por archivo en uso |
| `-allowUntrusted` | Acepta el certificado del puerto 8172, que es interno del hospedaje y no está firmado por una autoridad pública |

> **Cuidado con lo que `sync` borra.** Si en ese sitio hay algo que usted puso
> a mano y no está en la carpeta de publicación, desaparece. Es la razón de que
> el tutorial 1 advierta sobre `iisstart.htm` y sobre
> `.well-known/acme-challenge`, la carpeta que Let's Encrypt usa para renovar
> el certificado.

### 10.2.4 — Comprobar que el `405` se fue

**El mismo comando del paso 10.1, otra vez.** Y ahora debe salir distinto:

```powershell
$api    = 'https://<SU_API_PG>.runasp.net'
$origen = '<URL_FRONT_PG>'

$r = Invoke-WebRequest -Uri "$api/api/sesion" -Method Options -Headers @{
    'Origin'                         = $origen
    'Access-Control-Request-Method'  = 'POST'
    'Access-Control-Request-Headers' = 'content-type'
} -UseBasicParsing

"codigo: $($r.StatusCode)"
"Allow-Origin: $($r.Headers['Access-Control-Allow-Origin'])"
```

**Lo que debe salir:**

```
codigo: 204
Allow-Origin: <URL_FRONT_PG>
```

> **Si todavía sale `405`**, la API vieja sigue arriba: o la publicación no
> llegó, o IIS tiene el proceso anterior en memoria. Dele **Restart** al sitio
> en el panel y espere: en el plan gratuito no hay `AlwaysRunning`, y un sitio
> recién reiniciado **arranca en frío** la primera vez que alguien lo llama.
> Treinta segundos es normal.
>
> **Si sale `204` sin cabecera**, el origen de la lista y el que usted mandó
> no son idénticos. Compárelos carácter por carácter: casi siempre es una
> barra al final, o `http` donde debía ir `https`.

---

## Paso 10.3 — Compilar el front apuntando a PostgreSQL

Aquí es donde este tutorial se separa de los otros tres.

### 10.3.1 — El archivo de configuración

El repositorio trae `front_react/.env.postgres`. **Ábralo y ponga la dirección
de su API contra PostgreSQL:**

```
VITE_URL_API=https://<SU_API_PG>.runasp.net
```

**Qué es cada parte del nombre `VITE_URL_API`:**

| | |
|---|---|
| El prefijo `VITE_` | **Obligatorio.** Vite solo deja llegar al navegador las variables que empiezan así. Las demás se quedan en su máquina — y es una protección real: evita que una clave del sistema termine publicada dentro del JavaScript por descuido |
| El resto del nombre | Es el que el código busca, en `src/api.js`: `import.meta.env.VITE_URL_API` |

> **Este archivo sí se versiona**, y a propósito: no tiene un solo secreto. Lo
> único que dice es a qué dirección pública le habla. Es el mismo criterio que
> `front_blazor/appsettings.Production.postgres.json`, que también está en el
> repositorio.

### 10.3.2 — El comando

```powershell
cd <la carpeta del proyecto>\front_react
npm install
npm run build -- --mode postgres
```

| | |
|---|---|
| `npm install` | Descarga las dependencias a `node_modules/`. **Solo la primera vez**, o cuando cambie `package.json` |
| `npm run build` | Llama a `vite build`, que compila todo a la carpeta `dist/` |
| `--` | **Esto no sobra.** Le dice a `npm` que lo que viene después no es para él, sino **para el programa que va a llamar**. Sin los dos guiones, `npm` se queda con `--mode postgres` y Vite nunca se entera |
| `--mode postgres` | Hace que Vite lea `.env` y **`.env.postgres`** — y **no** `.env.production`. Ahí está el cambio de motor |

**Lo que debe salir:**

```
vite v5.4.11 building for production...
✓ 38 modules transformed.
dist/index.html                  1.66 kB │ gzip:  0.97 kB
dist/assets/index-Dp3HsvpT.js  192.29 kB │ gzip: 60.95 kB
✓ built in 1.52s
```

### 10.3.3 — Comprobar que la dirección quedó adentro

**Este es el comando que demuestra de qué habla todo el tutorial.** No se
confíe del `✓`: mire el archivo.

```powershell
Select-String -Path dist\assets\*.js -Pattern 'https://[a-z0-9-]+\.runasp\.net' -AllMatches |
    ForEach-Object { $_.Matches.Value } | Sort-Object -Unique
```

| | |
|---|---|
| `Select-String` | Busca un patrón dentro de archivos. Es el equivalente de `grep` en PowerShell |
| `-Pattern '...'` | La expresión regular: cualquier dirección que termine en `.runasp.net` |
| `-AllMatches` | Que no se detenga en la primera coincidencia de cada renglón — el JavaScript compilado viene en renglones larguísimos |
| `Sort-Object -Unique` | Quita las repetidas, para ver la lista limpia |

**Lo que debe salir es una sola dirección, y es la de PostgreSQL:**

```
https://apifacturas-pg-carrloscastro.runasp.net
```

> **Ahí está, con todas sus letras, dentro de un archivo `.js`.** No en una
> variable, no en un archivo de configuración al lado: **incrustada en el
> programa**. Eso es lo que significa «se lee al compilar».
>
> **Y hay una segunda prueba, más bonita todavía:** compare el nombre del
> archivo con el de la compilación anterior.
>
> | Compilado con | El archivo se llama |
> |---|---|
> | `npm run build` (SQL Server) | `index-D1opATUh.js` |
> | `npm run build -- --mode postgres` | `index-Dp3HsvpT.js` |
>
> **Mismo código fuente, mismo tamaño —192.29 kB los dos—, nombre distinto.**
> Ese nombre es una huella del contenido: si cambia un solo carácter adentro,
> cambia el nombre. Dos nombres distintos prueban que **el contenido es
> distinto**, y lo único que se cambió fue la dirección de la API.
>
> De paso, eso explica por qué no hay que preocuparse por la caché del
> navegador: cada compilación produce un nombre nuevo, así que nadie se queda
> con el archivo viejo.

---

## Paso 10.4 — Subir el front

```powershell
cmd.exe /c '"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -source:contentPath="<la carpeta del proyecto>\front_react\dist" -dest:contentPath="<SITIO_FRONT_PG>",computerName="https://<SITIO_FRONT_PG>.siteasp.net:8172/msdeploy.axd?site=<SITIO_FRONT_PG>",userName="<SITIO_FRONT_PG>",password="<CLAVE_FRONT_PG>",authType="Basic" -enableRule:AppOffline -allowUntrusted'
```

**Es el mismo comando de la API, con dos diferencias que valen la pena:**

| | |
|---|---|
| El origen es `front_react\dist` | **La carpeta compilada, no el código.** Lo que se publica es el resultado de `npm run build`: unos pocos archivos. `node_modules/` **no se sube** — tiene decenas de miles de archivos y el plan gratuito tiene un tope de 100 000 |
| No hace falta `.NET` en el sitio | Esto son **archivos estáticos**: HTML, JavaScript y CSS. IIS los entrega y no ejecuta nada. Por eso este front no se duerme ni arranca en frío |

**Lo que debe salir:**

```
Info: Adding file (<SITIO_FRONT_PG>\index.html).
Info: Adding file (<SITIO_FRONT_PG>\assets\index-Dp3HsvpT.js).
Info: Adding file (<SITIO_FRONT_PG>\web.config).
...
Número total de cambios: N (N agregados, 0 eliminados, 0 actualizados, 0 parámetros cambiados, ... bytes copiados)
```

> **Fíjese en que `web.config` está en la lista, y que eso no es casualidad.**
> Está en `front_react/public/`, y todo lo que vive en `public/` Vite lo copia
> a `dist/` tal cual. Ese archivo es **indispensable**, y el paso 10.5 muestra
> exactamente qué se rompe sin él.

---

## Paso 10.5 — Comprobar, y entender el `web.config`

### 10.5.1 — Las cuatro direcciones

```powershell
$front = '<URL_FRONT_PG>'
foreach ($ruta in @('/', '/login', '/facturas', '/facturas/1')) {
    $r = Invoke-WebRequest -Uri "$front$ruta" -UseBasicParsing
    "{0,-16} {1}" -f $ruta, $r.StatusCode
}
```

**Las cuatro tienen que dar `200`.** Y la tercera y la cuarta son las
interesantes:

> **Por qué `/facturas` es la prueba de verdad.** En el servidor **no existe
> ninguna carpeta llamada `facturas`**. Lo único que hay en `dist/` es
> `index.html`, la carpeta `assets/` y `web.config`. Si usted pide `/facturas`
> a un IIS sin configurar, IIS busca esa carpeta, no la encuentra y responde
> **404** — con toda la razón.
>
> El `web.config` le dice otra cosa: *«si lo que piden no es un archivo ni una
> carpeta que exista, entregue `index.html`»*. Entonces llega el programa de
> React, **él** mira la dirección y decide qué pantalla dibujar. Ese es el
> trabajo que en Blazor y en Flask hace el servidor.
>
> **Y de ahí sale un detalle del que casi nadie se acuerda:** en
> `vite.config.js`, `base` tiene que ser `'/'` —absoluto— y no `'./'`
> —relativo—. Con rutas relativas, el `index.html` que se entrega en
> `/facturas/1` pediría sus archivos a `/facturas/assets/...`, que no existe, y
> **la página saldría en blanco sin un solo mensaje de error**.

### 10.5.2 — Que de verdad esté hablando con PostgreSQL

Entre a `<URL_FRONT_PG>` e inicie sesión con `admin@correo.com` / `admin123`.

En la pantalla de inicio, la tarjeta **«Contra qué está hablando»** muestra
tres renglones que vienen de la API, no del front:

| | |
|---|---|
| **API** | Su dirección de PostgreSQL |
| **Versión** | `v4` |
| **Motor** | **`postgres`** |

**Ese `postgres` es la prueba.** No lo escribió el front: se lo preguntó a la
API, y la API lo sabe porque su `appsettings.Production.json` lo dice.

Y si prefiere comprobarlo sin navegador:

```powershell
Invoke-RestMethod -Uri 'https://<SU_API_PG>.runasp.net/'
```

```
mensaje : API Facturas funcionando
version : v4
motor   : postgres
```

### 10.5.3 — La prueba que cierra el curso: los dos, lado a lado

**Abra las dos pestañas:**

| Pestaña | Dirección | Motor |
|---|---|---|
| 1 | el front de React del tutorial anterior | **SQL Server** |
| 2 | `<URL_FRONT_PG>` | **PostgreSQL** |

Entre a **Facturas** en las dos y compare.

**Lo que tiene que verse igual:** las mismas columnas, los mismos nombres de
cliente con sus tildes, los mismos totales, el mismo maestro-detalle al pulsar
**Ver**, el mismo cálculo de subtotales.

**Lo que puede verse distinto:** el número de facturas. Cuando se escribió
este tutorial había **7 en SQL Server y 6 en PostgreSQL**, porque son **dos
bases de datos de verdad, independientes**: lo que se emitió en una no está en
la otra. Que las cuentas no coincidan no es un error — es la confirmación de
que no hay ningún truco.

> **Y ahora lo que todo esto demuestra, que es el punto del curso completo:**
>
> **El mismo `ApiFacturas.dll`**, byte por byte, está publicado dos veces y
> cada copia habla con un motor distinto. Lo único que las diferencia es un
> archivo de texto.
>
> **El mismo código de React**, sin una línea cambiada, está publicado dos
> veces y cada copia le habla a una API distinta. Lo único que las diferencia
> es un `.env`.
>
> **Si la API tuviera adentro algo de la interfaz** —HTML, sesiones, reglas de
> pantalla— el segundo front no podría existir, y menos el tercero. **Si la
> API tuviera adentro algo del motor de base de datos** —un `CAST` de T-SQL en
> la capa de negocio, por ejemplo— la segunda publicación no funcionaría.
>
> Que las cuatro combinaciones funcionen **sin tocar el código** no es suerte:
> es lo que significa tener las capas bien puestas, y es lo único que esta
> serie de cuatro tutoriales estaba tratando de demostrar.

---

## Lo que puede salir mal, y cómo se reconoce

| Lo que ve | Qué está pasando | Qué hacer |
|---|---|---|
| La página carga, pero al iniciar sesión no pasa nada | Casi siempre es **CORS**. Abra la consola del navegador con **F12**: ahí está el mensaje, porque en la API no hay rastro | Paso 10.1 y 10.2 |
| En la consola: *«No 'Access-Control-Allow-Origin' header»* | El origen no está en la lista, o no es idéntico | Revise la barra final y `http` contra `https` |
| `405` en el preflight | La API publicada es anterior a CORS | Paso 10.2.3: volver a publicar |
| `/` carga pero `/facturas` da **404** | Falta el `web.config`, o no llegó al servidor | Compruebe que esté en `dist/` y vuelva a subir |
| La página sale **en blanco**, sin errores visibles | `base` relativo en `vite.config.js` | Déjelo en `'/'` y vuelva a compilar |
| Dice **motor `sqlserver`** cuando esperaba `postgres` | El front apunta a la API equivocada: compiló sin `--mode postgres`, o `npm` se quedó con el argumento por falta de `--` | Paso 10.3.3: mire qué dirección quedó adentro |
| `500` con *«Named Pipes error 40»* | La API no encuentra la base de datos. La cadena de conexión está mal, o la base de datos se recreó con otro número | Compare la cadena con la que muestra el panel **hoy** |
| Todo tarda muchísimo la primera vez | **Arranque en frío.** El plan gratuito no tiene `AlwaysRunning` y Neon duerme la base de datos cuando nadie la usa | Espere y reintente. El segundo intento es inmediato |

---

## Lo que queda por hacer

**Para ser honestos con lo que este documento es y lo que no es:**

| | |
|---|---|
| **No tiene capturas del panel** | Los tutoriales 1, 2 y 3 van pantalla por pantalla; este va por los comandos. Las pantallas que necesita son las mismas de esos tres |
| **Las salidas son de una sola corrida** | Los comandos y las comprobaciones se midieron de verdad el 29 de octubre de 2026, pero contra las APIs ya publicadas. **La publicación del front a un cuarto sitio no se ejecutó**: los pasos 10.4 y 10.5 están escritos a partir de la publicación que sí se hizo del front de React contra SQL Server, que es el mismo comando con otro sitio |
| **Al front de React le faltan tres pantallas** | Va por nueve de las doce entradas del menú: le faltan el tablero y las dos de usuarios y roles. Las nueve que tiene —incluido el maestro-detalle de facturas— alcanzan de sobra para la comparación de este tutorial |

> **Y lo que hay que hacer cuando el curso termine:** todas las contraseñas que
> aparecieron en esta serie —la de la base de datos, las de WebDeploy, la que
> firma los tokens— **se rotan**. Quedaron a la vista a propósito, para que se
> entendiera de dónde sale cada dato. En un proyecto de verdad van en variables
> de entorno, nunca en un archivo que se versiona.
