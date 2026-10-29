# Los secretos — conceptos, ejemplos y referencias

> Lo que hay que entender **antes** de publicar. Son cuatro prácticas, y cada
> una tapa una fuga distinta: la que tapa git no tapa el disco, y la que tapa
> el disco no tapa el servidor.
>
> Los ejemplos salen de **este** repositorio: `appsettings.Production.json`,
> `appsettings.Production.postgres.json`, el `docker-compose.yml`, y los dos
> hospedajes que usan los tutoriales — **MonsterASP** y **Render**.

```
          EL PROYECTO                    EL SERVIDOR
   ┌───────────────────────┐        ┌───────────────────────┐
   │  appsettings.json     │        │  variable de entorno  │
   │  .env                 │  ───►  │  o archivo del sitio  │
   └───────────┬───────────┘        └───────────────────────┘
               │
       ┌───────┴────────┬──────────────┐
       ▼                ▼              ▼
     git            el disco        copiar /
   (historial)     (OneDrive)      comprimir
       │                │              │
       └──── las tres fugas del mismo archivo ────┘
```

---

## 1. Qué es un secreto, y qué no

**Un secreto es un dato que, en manos de otro, le permite hacerse pasar por
usted o por su sistema.** No es «cualquier cosa de configuración».

| | Ejemplo en este proyecto | ¿Secreto? |
|---|---|---|
| Una **dirección pública** | `UrlApi`, `VITE_URL_API`, `API_FACTURAS_URL` | **No.** Cualquiera la ve en la barra del navegador |
| Un **nombre de usuario** de base de datos | `neondb_owner`, `db75640` | **No** por sí solo. Sin la clave no abre nada |
| Una **contraseña** | la de Neon, la de SQL Server del hospedaje | **Sí** |
| Una **clave de firma** | `Jwt__Key` | **Sí**, y es peor que una contraseña: con ella se **falsifican sesiones** sin necesidad de entrar a nada |
| Una **clave de despliegue** | las de WebDeploy | **Sí.** Permite reemplazar el programa publicado |
| Una contraseña **de desarrollo local** | `Paradigmas123!` del `docker-compose.yml` | **No.** Esa base de datos solo existe en su máquina, y el valor está a la vista a propósito |

> **Esta distinción importa, y no es un tecnicismo:** si todo se trata como
> secreto, nada se trata como secreto. En este repositorio la clave de
> desarrollo del compose se llama literalmente
> `ClaveDeDesarrolloDelCursoAplicacionYServiciosWeb_2026_2_NoEsUnSecreto!`
> para que nadie dude de cuál es cuál.

### El inventario de este proyecto

| Secreto | Dónde vive hoy |
|---|---|
| Contraseña de **SQL Server del hospedaje** | `api_facturas/appsettings.Production.json` — en `.gitignore` |
| Contraseña de **PostgreSQL en Neon** | `api_facturas/appsettings.Production.postgres.json` — en `.gitignore` |
| `Jwt__Key` **publicada** | los mismos archivos |
| Contraseñas de **WebDeploy** | solo en el panel de MonsterASP |
| `CLAVE_SESION` del front de Flask | el panel de **Render** |

---

## 2. Las tres fugas, y por qué hacen falta cuatro prácticas

Un secreto escrito en un archivo del proyecto se escapa por **tres caminos
distintos**, y cada uno necesita su propia defensa:

| La fuga | Cómo ocurre | Qué la tapa |
|---|---|---|
| **git** | `git add .` y queda **en el historial para siempre** | el `.gitignore` (práctica 1) |
| **el disco** | la carpeta se copia, se comprime, se manda por chat — o **la sincroniza OneDrive** | sacar el secreto del proyecto (práctica 3) |
| **el servidor** | el archivo publicado se abre desde el panel del hospedaje | la variable de entorno (práctica 4) |

> **Por eso `.gitignore` no es suficiente, aunque sea lo primero.** Evita la
> fuga **irreversible** —lo que entra al historial de git no sale— pero el
> archivo sigue en su disco. En este repositorio eso no es teórico: el
> proyecto vive en `OneDrive\Desktop\...`, así que **cada archivo con una
> contraseña se está sincronizando a la nube de Microsoft**.

---

## 3. Las cuatro prácticas

| # | Práctica | Cómo se hace | Qué fuga tapa |
|---|---|---|---|
| **1** | El archivo del secreto, en `.gitignore` | Agregue `appsettings.Production.json` al `.gitignore` y compruebe con `git check-ignore -v api_facturas\appsettings.Production.json` | **git** |
| **2** | Versionar una plantilla `.ejemplo` | `copy appsettings.Production.json appsettings.Production.json.ejemplo`, y en la **copia** reemplace la clave por `SU_CONTRASENA`. Esa copia **sí** entra a git | la que se abre cuando alguien **pide el archivo real** |
| **3** | La clave en `dotnet user-secrets` | `dotnet user-secrets init` y luego `dotnet user-secrets set "ConnectionStrings:Postgres" "...;Password=LA_CLAVE;..."`. Después **borre la contraseña del `.json`** | **el disco** |
| **4** | En el servidor, variable de entorno | `ConnectionStrings__Postgres`, según el hospedaje (sección 5) | **el servidor** |

### 3.1 La 1 y la 2 son opuestas, no lo mismo

| Archivo | Tiene la clave real | ¿Entra a git? |
|---|---|---|
| `appsettings.Production.json` | **sí** | **no** |
| `appsettings.Production.json.ejemplo` | no, dice `SU_CONTRASENA` | **sí** |

La práctica 1 dice **qué no sube**. La 2 dice **qué sube en su lugar**.

> **Y la 2 tapa una fuga que no es técnica, sino humana:** sin plantilla, el
> que clona no sabe qué archivo crear ni qué va adentro. Lo más fácil entonces
> es pedirle el archivo real a un compañero o mandar el propio por chat — y
> así es como los secretos se filtran de verdad en los trabajos en equipo.

### 3.2 Qué hace `user-secrets`, exactamente

| Comando | Qué hace |
|---|---|
| `dotnet user-secrets init` | Agrega al `.csproj` un `UserSecretsId` —un número largo—. **Eso es lo único que queda en el proyecto**, y no es un secreto |
| `dotnet user-secrets set "clave" "valor"` | Guarda el valor en `C:\Users\<usted>\AppData\Roaming\Microsoft\UserSecrets\<ese-número>\secrets.json` |

**Cómo lo lee la aplicación: sola.** Al arrancar ve el `UserSecretsId`, busca
ese archivo y lo superpone a los `appsettings`. **No se cambia una línea de
código.**

| Qué resuelve | Qué NO resuelve |
|---|---|
| No se puede subir por error · no viaja al copiar o comprimir · **OneDrive no lo sincroniza** | Quien entre con su usuario de Windows lo lee. Eso es seguridad de la máquina, y no lo arregla ningún archivo de configuración |

> **Límite importante: `user-secrets` funciona solo en desarrollo.** El
> proveedor se registra únicamente cuando el entorno es `Development`. En el
> servidor publicado no existe — ahí manda la práctica 4.

---

## 4. Cómo se consulta cada secreto, con ejemplos

**Un secreto no se busca en «el proyecto»: se busca donde lo haya puesto la
práctica que se aplicó.** Esta es la tabla de dónde mirar, y abajo cada caso
con su comando.

| Si el secreto está en… | Se consulta con |
|---|---|
| Un archivo del proyecto | `Select-String` o `Get-Content` |
| `user-secrets` | `dotnet user-secrets list` |
| Una variable de su sesión | `$env:NOMBRE` |
| Un contenedor de Docker | `docker compose exec <servicio> printenv NOMBRE` |
| El panel de **Render** | Environment → **Reveal** |
| El panel de **MonsterASP** | File Manager, o Databases → Local access |
| El panel de **Neon** | Connect, o Roles |

### 4.1 En un archivo del proyecto

```powershell
Select-String -Path api_facturas\appsettings.Production.postgres.json -Pattern "Password="
```

`Select-String` es el equivalente de `grep` en PowerShell: busca un patrón
dentro de un archivo y muestra el renglón completo. Sale algo así:

```
"Postgres": "Host=ep-...neon.tech;Database=neondb;Username=neondb_owner;Password=npg_•••••;SSL Mode=Require"
```

Para ver el archivo entero: `Get-Content api_facturas\appsettings.Production.postgres.json`.

### 4.2 En `user-secrets`

```powershell
cd api_facturas
dotnet user-secrets list
```

```
ConnectionStrings:Postgres = Host=...;Password=npg_•••••;SSL Mode=Require
Jwt:Key = ...
```

| | |
|---|---|
| Hay que estar **en la carpeta del proyecto** | El comando busca el `UserSecretsId` en el `.csproj` de ahí |
| `list` muestra **todos** los que ese proyecto tenga | Para uno solo: `dotnet user-secrets list \| Select-String "Postgres"` |

### 4.3 En una variable de entorno de su sesión

```powershell
$env:ConnectionStrings__Postgres
```

Y para ver **todas** las que parezcan tener una clave:

```powershell
Get-ChildItem Env: | Where-Object Name -Match 'PASS|CLAVE|PWD|KEY'
```

`Get-ChildItem Env:` lista el entorno de **esta** sesión de PowerShell;
`Where-Object` filtra por nombre. Si no sale nada, en su sesión no hay
ninguna — que es lo normal en este proyecto, porque las variables las pone
Docker dentro de los contenedores, no su terminal.

### 4.4 Dentro de un contenedor de Docker

```powershell
docker compose exec api-facturas printenv ConnectionStrings__Postgres
docker compose exec sqlserver    printenv MSSQL_SA_PASSWORD
docker compose exec front-flask  printenv API_FACTURAS_URL
```

| | |
|---|---|
| `docker compose exec <servicio>` | Ejecuta una orden **dentro** de ese contenedor |
| `printenv NOMBRE` | Imprime esa variable del entorno |

> **Esto es distinto de leer el `docker-compose.yml`**, y la diferencia
> importa: el archivo dice **qué se pidió**, el comando dice **qué llegó**. Si
> `printenv` no contesta nada, la variable no llegó y la aplicación está usando
> el valor de respaldo que trae escrito el código — que es la explicación de
> por qué a veces «funciona» apuntando a donde uno no quería.
>
> **Con el front de React este comando no sirve:** ahí la dirección no es una
> variable en tiempo de ejecución, va incrustada al compilar. Se consulta así:
> ```powershell
> Select-String -Path front_react\dist\assets\*.js -Pattern 'https://[a-z0-9-]+\.runasp\.net' -AllMatches |
>     ForEach-Object { $_.Matches.Value } | Sort-Object -Unique
> ```

### 4.5 En los paneles, publicado

| Dónde | Ruta exacta |
|---|---|
| **Render** — `CLAVE_SESION` del front de Flask | el servicio → **Environment** → *Environment Variables* → **Reveal** |
| **MonsterASP** — lo que lee la API publicada | **Websites** → el sitio → **File Manager** → abrir `appsettings.Production.json` o `web.config` |
| **MonsterASP** — la contraseña de la base de datos | **Databases** → la base de datos → **Local access** → `Password` → **Show** |
| **MonsterASP** — la clave de despliegue | **Websites** → el sitio → **WebDeploy** |
| **Neon** — la contraseña de PostgreSQL | el proyecto → **Connect** (la muestra dentro de la cadena), o **Roles** para generarla de nuevo |

> **En los dos paneles, quien entra los ve.** Ni Render ni MonsterASP se los
> esconden al dueño de la cuenta: tiene que poder recuperarlos. Lo que
> distingue a uno de otro es si quedan escritos en un archivo (sección 6).

### 4.6 Una advertencia sobre consultar secretos

**Imprimir una contraseña en la terminal la deja en dos sitios más:** el
*scrollback* de la ventana y el historial de PowerShell —el archivo
`ConsoleHost_history.txt`, que `Get-PSReadLineOption` le dice dónde está—.

En una máquina de clase no pasa nada. Pero el hábito que conviene llevarse es
el contrario: **los secretos se consultan en el panel o en el gestor, no se
imprimen.** Y si los imprimió, sepa que acaba de dejar una copia.

---

## 5. Por qué la variable de entorno gana: el orden de la configuración

ASP.NET Core no lee **un** archivo de configuración: lee **varias fuentes, una
encima de otra**, y la última que traiga un valor es la que manda.

```
  1. appsettings.json
  2. appsettings.{Entorno}.json        ← Production, publicado
  3. user-secrets                      ← SOLO si el entorno es Development
  4. VARIABLES DE ENTORNO              ← ganan sobre todo lo anterior
  5. argumentos de la línea de comandos
```

**De ahí salen dos cosas que en este proyecto se usan todos los días:**

| | |
|---|---|
| Por qué `appsettings.Production.json` manda publicado | El hospedaje no fija `ASPNETCORE_ENVIRONMENT`, y **sin fijar, el entorno es `Production`**. El archivo se superpone al general: no lo reemplaza, solo le gana en las claves que repite |
| Por qué la variable de entorno no necesita código | Está en el nivel 4. Poner `ConnectionStrings__Postgres` **sustituye** lo que diga el archivo, sin tocar el programa |

**El nombre cambia de forma.** En un archivo es `ConnectionStrings:Postgres`
(dos puntos); como variable de entorno los dos puntos no se permiten, así que
van **dos guiones bajos**: `ConnectionStrings__Postgres`.

> **Ya hay un ejemplo real en el repositorio.** El
> [`docker-compose.yml`](../../docker-compose.yml) le pasa a la API
> `ConnectionStrings__SqlServer`, `ConnectionStrings__Postgres` y `Jwt__Key`
> con esa misma notación. No es un truco de Docker: es el nivel 4 de la lista
> de arriba.

---

## 6. El límite real: depende del hospedaje, y no siempre se puede

| Hospedaje | Dónde va la variable | ¿Queda en un archivo? |
|---|---|---|
| **Render** (el front de Flask) | Panel → **Environment** → *Environment Variables* | **No.** Render guarda el valor en su propio almacén y lo inyecta al proceso al arrancar |
| **MonsterASP** (la API) | No tiene panel de variables. Toca ponerlas en el `web.config` del sitio | **Sí** |

```xml
<aspNetCore processPath="dotnet" arguments=".\ApiFacturas.dll">
  <environmentVariables>
    <environmentVariable name="ConnectionStrings__Postgres" value="Host=...;Password=..." />
  </environmentVariables>
</aspNetCore>
```

### 6.1 En MonsterASP la práctica 4 cumple a medias

**Un `web.config` se puede abrir desde el panel.** Entonces poner la clave ahí
o en el `appsettings.json` es **casi lo mismo**: las dos son archivos legibles
en el servidor. Lo único que se gana es que la contraseña **no esté en un
archivo del proyecto** — el que se sube, se copia y se sincroniza.

**Y no hay forma de evitarlo.** Una variable de entorno tiene que declararse en
algún lado, y hay dos:

| Dónde se declara | ¿Se puede en el plan gratuito? |
|---|---|
| En el `web.config` del sitio | **Sí** — pero es un archivo del sitio |
| En el sistema operativo o en el *app pool* de IIS | **No.** Es un servidor compartido y no hay acceso a ese nivel |

> **En resumen: con un hospedaje compartido gratuito, el secreto vale lo que
> vale la contraseña de su cuenta del hospedaje.** Es un límite de la
> infraestructura, no del método — y saber **dónde está el límite** es parte
> de entender el método.

**Qué se hace cuando el hospedaje no da más:**

| | |
|---|---|
| **Privilegio mínimo** | Que ese usuario de base de datos pueda hacer solo lo que la aplicación necesita, no ser dueño de todo |
| **No reutilizar** | Esa contraseña en ningún otro sitio: si se filtra, se pierde una sola cosa |
| **Rotar al cerrar** | Fin del proyecto, claves nuevas |

> **Un detalle práctico que muerde:** `msdeploy -verb:sync` deja el destino
> idéntico al origen, así que **sobrescribe el `web.config` en cada
> publicación**. O se vuelve a poner cada vez, o se genera desde el `.csproj`
> para que salga ya con la variable.

### 6.2 Y en Render, que sí tiene variables de verdad

El valor vive en el almacén de Render; al arrancar el contenedor se lo inyecta
al proceso, y el código lo lee con `os.environ.get(...)` sin saber de dónde
salió. Para verlo o cambiarlo: panel → **Environment**, con un **Reveal** al
lado de cada valor. Al guardar, Render **reinicia el servicio**.

| | MonsterASP | Render |
|---|---|---|
| Dónde vive el valor | en un archivo del sitio | en el almacén de Render |
| ¿Se puede abrir el archivo? | **sí** | no hay archivo |
| Quién lo puede ver | quien entre al panel | quien entre al panel |

**Fíjese en la última fila: en los dos casos, quien entra al panel lo ve.**
Render no se lo esconde a usted — tiene que poder recuperarlo. Lo que cambia es
que **no queda escrito en ningún archivo**, así que no se filtra por copiar la
carpeta, por subirla a git ni por un respaldo olvidado.

---

## 7. ¿Y un gestor de secretos? Reduce el problema, no lo elimina

**Un gestor de secretos no es una función del hospedaje: es código en su
aplicación.** Al arrancar, le pide la contraseña a un servicio externo —Azure
Key Vault, AWS Secrets Manager, HashiCorp Vault, Infisical— y la recibe por la
red. Nada de eso depende del hospedaje, así que **funcionaría igual en
MonsterASP**.

**El problema que queda:** para pedirle un secreto al gestor hay que
**autenticarse ante el gestor**, y esa credencial tiene que estar en algún
lado — otra vez, en un archivo.

| | Secretos guardados en archivos |
|---|---|
| Sin gestor | la clave de la base de datos, la de firma, y cuantas haya |
| Con gestor | **una sola**: la del gestor |

Eso se llama **el secreto de arranque** (*bootstrap secret*), y no tiene
solución por software. Se resuelve con **identidad administrada** —cuando la
máquina misma prueba quién es, sin credencial escrita— pero eso solo lo dan
Azure, AWS y Google **a sus propias máquinas**. En un hospedaje compartido
ajeno, no existe.

**Lo que sí se gana, y no es poco:** una sola credencial que vigilar, rotación
centralizada, y registro de quién pidió qué secreto y cuándo.

> **Para un proyecto de aula no hace falta.** Agrega una cuenta más que
> administrar, y lo que enseña —que **la cadena de confianza termina en algo
> que hay que guardar en alguna parte**— cabe en una frase.

---

## 8. Si el secreto ya se expuso: rotar

**Esto no es una práctica: es una reparación.** Solo aplica si la clave ya
salió — se subió a git, se mandó por chat, se sincronizó a una nube, se
proyectó en una clase.

| | Qué es |
|---|---|
| **Mover** | la misma contraseña, en otro lugar. Si alguien ya la vio, **sigue entrando** |
| **Rotar** | generar una **nueva** y dejar la vieja inservible |

**Se rota porque una contraseña expuesta no se puede «desexponer»:** borrarla
de un archivo no la borra de la memoria de quien la leyó, ni del historial de
git, ni de la nube donde se sincronizó.

| Paso | Cómo se hace |
|---|---|
| **Rotar** | Neon: panel → el proyecto → **Roles** → el rol → *Reset password*. MonsterASP: **Databases → la base de datos → Local access → Password**. Luego poner la nueva en las prácticas 3 y 4 |
| **Buscar rastros** | `Select-String -Path . -Pattern "npg_" -Recurse` y revisar `git status`. Si aparece en un archivo versionado, sacarlo y volver a rotar |

> **El orden importa:** las prácticas 3 y 4 **antes** de rotar. Si rota
> primero, la aplicación queda caída hasta que ponga la nueva en los dos lados.

> **Y lo que no funciona:** borrar el archivo en un commit nuevo. El secreto
> sigue en los commits anteriores. Reescribir el historial —con
> `git filter-repo`— es posible, pero si el repositorio ya se publicó, hay que
> suponer que alguien lo clonó. **Rotar es la única defensa que de verdad
> cierra la puerta.**

---

## 9. El resumen, en seis líneas

1. Un secreto es lo que permite **hacerse pasar por usted o por su sistema**. Una dirección pública no lo es; una clave de firma lo es dos veces.
2. El mismo archivo se fuga por **tres caminos**: git, el disco y el servidor. Cada uno necesita su propia defensa.
3. `.gitignore` es **lo primero**, porque la fuga por git es la irreversible — pero ahí no termina.
4. `user-secrets` saca el secreto de la carpeta; **variable de entorno** lo saca del archivo publicado. La variable gana sobre los `appsettings` sin tocar el código.
5. En un hospedaje compartido gratuito el secreto **termina en un archivo del servidor**, y vale lo que vale la contraseña de su cuenta. Un gestor de secretos reduce el problema a una sola credencial, no a ninguna.
6. Si ya se expuso, **rotar** — no mover, no borrar. Y después de rotar, buscar si quedó alguna copia olvidada.

---

## 10. Referencias

| Tema | Fuente |
|---|---|
| `user-secrets` y almacenamiento seguro en desarrollo | [Safe storage of app secrets in development — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) |
| El orden de las fuentes de configuración | [Configuration in ASP.NET Core — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/) |
| Variables de entorno en IIS y el `web.config` | [Host ASP.NET Core on Windows with IIS — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/) |
| La configuración va en el entorno, no en el código | [The Twelve-Factor App — III. Config](https://12factor.net/config) |
| Gestión de secretos, con el problema del secreto de arranque | [Secrets Management Cheat Sheet — OWASP](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html) |
| Variables de entorno en Render | [Configure environment variables — Render](https://render.com/docs/configure-environment-variables) |
| Rotar la contraseña de un rol | [Manage roles — Neon](https://neon.com/docs/manage/roles) |
| Sacar un secreto del historial de git (y por qué igual hay que rotar) | [Removing sensitive data from a repository — GitHub Docs](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository) |

**En este repositorio:** [CONEXIONES.md](../../CONEXIONES.md) —
de dónde lee cada front la dirección de su API · los tutoriales de publicación
[1](../../TUTORIAL_1_SQLSERVER_BLAZOR.md) ·
[2](../../TUTORIAL_2_POSTGRES_FLASK.md) ·
[3](../../TUTORIAL_3_POSTGRES_BLAZOR.md) ·
[4](../../TUTORIAL_4_POSTGRES_REACT.md)
