# Cómo se conecta cada front con su API

**Una sola hoja, para consultar rápido.** Los tres fronts del proyecto —Blazor,
Flask y React— son la misma aplicación y le hablan a la misma API. Ninguno
tiene la dirección de la API escrita en el código: **los tres la leen de
afuera.** Esta hoja dice de dónde la lee cada uno y qué hay que cambiar para
apuntarlo a otra.

> **Por qué la dirección va afuera y no en el código.** Si estuviera adentro,
> cambiar de API sería **modificar el programa**: editar, volver a compilar,
> volver a probar. Al estar afuera, el mismo programa sirve para todas las
> APIs — y ésa es la razón de que una sola compilación de la API pueda estar
> publicada dos veces, una contra cada motor de base de datos.

---

## La tabla, de un vistazo

| Front | Qué nombre busca | Dónde lo encuentra | **Cuándo lo lee** |
|---|---|---|---|
| **Blazor** (`8099`) | `UrlApi` | `front_blazor/appsettings.json` | **al arrancar** |
| **Flask** (`8118`) | `API_FACTURAS_URL` | una variable de entorno | **al arrancar** |
| **React** (`8119`) | `VITE_URL_API` | `front_react/.env.*` | **al compilar** |

**Esa última columna es la única diferencia que importa**, y de ella sale todo
lo demás:

| | Blazor y Flask | React |
|---|---|---|
| Para cambiar de API | **cambiar el valor y reiniciar** | **volver a compilar y volver a subir** |
| Cuánto tarda | segundos | un par de minutos |
| ¿El programa publicado cambia? | **no**, es el mismo | **sí**, es otro archivo |

**Por qué.** Blazor y Flask corren **en un servidor**: cuando arrancan, hay
alguien ahí que puede leer un archivo o una variable del sistema. React corre
**en el navegador de la persona**, y ahí no hay nadie que lea nada — así que
Vite, el compilador, escribe la dirección **dentro** del JavaScript. Lo que
llega al navegador no es un programa que consulta su configuración: es un
programa con la dirección ya puesta adentro.

> **No es un defecto de React ni algo que se pueda arreglar.** Es el precio de
> que el front sean archivos estáticos: por eso no necesita memoria, ni Node en
> el servidor, ni arranca en frío. Se paga en flexibilidad lo que se ahorra en
> recursos.

---

## Front en Blazor — `UrlApi`

Lo lee [`front_blazor/Program.cs`](front_blazor/Program.cs):

```csharp
var urlApi = builder.Configuration["UrlApi"] ?? "http://localhost:8035";
```

**Tres lugares distintos, según dónde esté corriendo.** No se pisan por
casualidad: ASP.NET Core los consulta **en este orden**, y el último que
encuentra un valor es el que gana.

| Dónde corre | Quién manda | Valor |
|---|---|---|
| En su máquina, fuera de Docker | `front_blazor/appsettings.json` | `http://localhost:8035` |
| **Dentro de Docker** | la variable `UrlApi` del [`docker-compose.yml`](docker-compose.yml) | `http://api-facturas:8035` |
| **Publicado** | `front_blazor/appsettings.Production.json` | su API en internet |

**Para apuntarlo a otra API publicada**, edite `appsettings.Production.json`:

```json
{ "UrlApi": "https://su-otra-api.runasp.net" }
```

y **reinicie el sitio** en el panel del hospedaje. El `.dll` no se toca.

> **Por qué gana el archivo con `.Production`.** ASP.NET Core lee
> `appsettings.json` y **encima** `appsettings.{Entorno}.json`. En el hospedaje
> nadie fija `ASPNETCORE_ENVIRONMENT`, y cuando no se fija, el entorno es
> **Production**. Los archivos **se superponen**: el segundo no reemplaza al
> primero, solo le gana en las claves que repite.
>
> **Y por qué dentro de Docker no es `localhost`:** `localhost` dentro de un
> contenedor es *ese* contenedor, no su máquina. El front tiene que llamar al
> servicio **por su nombre**, `api-facturas`, que es el que Docker sabe
> resolver.

El repositorio trae también `appsettings.Production.postgres.json`, que es el
mismo archivo apuntando a la API contra PostgreSQL. Se usa copiándolo encima
del otro al publicar — el procedimiento está en el
[tutorial 3](TUTORIAL_3_POSTGRES_BLAZOR.md).

---

## Front en Flask — `API_FACTURAS_URL`

Lo lee [`front_flask/cliente_api.py`](front_flask/cliente_api.py):

```python
URL_API = os.environ.get("API_FACTURAS_URL", "http://localhost:8035")
```

**Es una variable de entorno, no un archivo**, y eso es a propósito: así se
hace en Python y así lo espera cualquier hospedaje moderno.

| Dónde corre | Quién manda | Valor |
|---|---|---|
| En su máquina, sin la variable | el valor por defecto del código | `http://localhost:8035` |
| **Dentro de Docker** | `API_FACTURAS_URL` del [`docker-compose.yml`](docker-compose.yml) | `http://api-facturas:8035` |
| **Publicado en Render** | **Environment → Environment Variables** del panel | su API en internet |

**Para apuntarlo a otra API**, cambie el valor en el panel de Render y déle
**Restart**. El código no se toca y no hay que volver a desplegar.

**Dos errores que cuestan un rato:**

| | |
|---|---|
| **Sin `https://` adelante** | `requests` no sabe qué hacer con un texto que no empieza por un esquema |
| **Con barra al final** | El front arma `...runasp.net//api/sesion` —dos barras— y la API responde `404` |

---

## Front en React — `VITE_URL_API`

Lo lee [`front_react/src/api.js`](front_react/src/api.js):

```javascript
export const URL_API =
  import.meta.env.VITE_URL_API || 'http://localhost:8035'
```

**Aquí el archivo lo escoge el comando con que usted compila**, no el lugar
donde corre:

| El comando | Lee | Y produce un front que apunta a |
|---|---|---|
| `npm run dev` | `.env.development` | `http://localhost:8035` |
| `npm run build` | `.env.production` | la API contra **SQL Server** |
| `npm run build -- --mode postgres` | `.env.postgres` | la API contra **PostgreSQL** |

> **El prefijo `VITE_` es obligatorio.** Vite solo deja llegar al navegador las
> variables que empiezan así; las demás se quedan en su máquina. Es una
> protección real: evita que una clave del sistema termine publicada dentro
> del JavaScript por descuido.
>
> **Y los dos guiones sueltos de `-- --mode` no sobran.** Le dicen a `npm` que
> lo que viene después no es para él, sino para el programa que va a llamar.
> Sin ellos, `npm` se queda el argumento y Vite nunca se entera.

**Para apuntarlo a otra API: tres pasos, y ninguno se puede saltar.**

**1. Ponga la dirección** en `.env.production` —o cree su propio archivo, por
ejemplo `.env.pruebas`:

```
VITE_URL_API=https://su-otra-api.runasp.net
```

**2. Vuelva a compilar:**

```powershell
cd front_react
npm run build          # o: npm run build -- --mode pruebas
```

**3. Vuelva a subir** la carpeta `dist/` al hospedaje.

> **El paso 3 no es opcional.** La dirección vieja sigue dentro del
> JavaScript que está publicado hasta que usted lo reemplace.

---

## Y lo que hay que cambiar **en la API**: solo por el de React

Los tres fronts consumen la misma API, pero **uno de los tres necesita que la
API lo autorice por nombre.**

| Front | ¿La API tiene que conocerlo? |
|---|---|
| Blazor | **No** |
| Flask | **No** |
| **React** | **Sí** |

**Por qué.** Blazor y Flask llaman a la API **desde el servidor**, y entre
servidores no hay ningún guardia que pregunte nada. React llama **desde el
navegador de la persona**, y ahí sí hay guardia: el navegador no deja que una
página de un origen llame a otro origen a menos que la API **diga por escrito
que acepta ese origen**. Eso es **CORS**.

La lista está en `api_facturas/appsettings.json` y, publicada, en
`appsettings.Production.json`:

```json
"OrigenesPermitidos": [
  "http://localhost:8119",
  "https://su-front-react.runasp.net"
]
```

**Tres cosas de esa lista que cuestan depuración si se pasan por alto:**

| | |
|---|---|
| **Va el origen, no la página** | `https://mi-front.ejemplo`, **sin** barra al final y **sin** `/login`. Un origen es esquema + nombre + puerto, nada más |
| **`http` y `https` son origenes distintos** | Si el sitio responde por los dos y usted puso uno, el otro queda bloqueado |
| **`localhost:8119` se queda** | Es el front cuando corre en Docker. Quitarlo rompe el desarrollo sin arreglar nada |

> **Cuando falla CORS, en la API no queda ni rastro.** No hay un error en el
> registro, no hay un 403, no hay nada — porque la petición **nunca salió**. El
> navegador la bloqueó antes. El mensaje está en la consola del navegador
> (**F12**), y en ningún otro lado. Si el front carga pero al iniciar sesión no
> pasa nada, mire ahí primero.

---

## Cómo comprobar, en un renglón

**¿A qué API le está hablando un front?** Entre e inicie sesión: la pantalla
de inicio trae la tarjeta **«Contra qué está hablando»**, con la dirección, la
versión y el **motor**. Ese dato no lo escribe el front — se lo pregunta a la
API.

**¿Y qué contesta la API por sí misma?**

```powershell
Invoke-RestMethod -Uri 'https://su-api.runasp.net/'
```

```
mensaje : API Facturas funcionando
version : v4
motor   : postgres
```

**¿Qué dirección quedó dentro del front de React compilado?** Esto es exclusivo
de React, porque es el único donde la dirección va incrustada:

```powershell
Select-String -Path dist\assets\*.js -Pattern 'https://[a-z0-9-]+\.runasp\.net' -AllMatches |
    ForEach-Object { $_.Matches.Value } | Sort-Object -Unique
```

**¿La API acepta el origen de mi front de React?**

```powershell
Invoke-WebRequest -Uri 'https://su-api.runasp.net/api/sesion' -Method Options -Headers @{
    'Origin'                         = 'https://su-front-react.runasp.net'
    'Access-Control-Request-Method'  = 'POST'
    'Access-Control-Request-Headers' = 'content-type'
} -UseBasicParsing
```

| Lo que sale | Qué significa |
|---|---|
| `204` **con** la cabecera `Access-Control-Allow-Origin` | Todo bien |
| `204` **sin** esa cabecera | La API tiene CORS, pero **este origen no está en la lista** |
| `405` | La API **no tiene CORS**: el programa publicado es anterior a que se le agregara |

> **El `204` sin cabecera es el más engañoso de los tres:** la respuesta se ve
> perfecta —código de éxito, sin errores, sin demora—. Lo que falla es **lo que
> no está ahí**.

---

## Si algo no conecta

| Lo que ve | Dónde mirar |
|---|---|
| El front carga pero al iniciar sesión no pasa nada | **F12 → consola.** Si dice `Access-Control-Allow-Origin`, es CORS |
| `404` en todas las llamadas | La dirección tiene **barra al final**, y quedan dos |
| El front de React sigue apuntando a la API vieja | No se volvió a compilar, o se compiló y no se volvió a subir |
| Dentro de Docker no encuentra la API | Puso `localhost` donde va el nombre del servicio, `api-facturas` |
| Publicado dice `motor: sqlserver` y esperaba `postgres` | El front apunta a la otra API, o el `appsettings.Production.json` del servidor es el equivocado |
| Todo tarda muchísimo la primera vez | **Arranque en frío**: el plan gratuito no mantiene el sitio encendido, y Neon duerme la base de datos cuando nadie la usa. El segundo intento es inmediato |

---

**Para publicar desde cero**, los tutoriales van paso por paso:
[1 — SQL Server y Blazor](TUTORIAL_1_SQLSERVER_BLAZOR.md) ·
[2 — PostgreSQL y Flask](TUTORIAL_2_POSTGRES_FLASK.md) ·
[3 — PostgreSQL y Blazor](TUTORIAL_3_POSTGRES_BLAZOR.md) ·
[4 — PostgreSQL y React](TUTORIAL_4_POSTGRES_REACT.md)
