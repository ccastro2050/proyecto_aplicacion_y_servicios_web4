# Los secretos: dónde van y dónde no

**Una hoja de consulta.** Qué hacer para que una contraseña no quede expuesta,
qué protege cada mecanismo **y qué no**, y qué hacer si ya se escapó.

---

## Las cuatro prácticas — para quien empieza el proyecto

| # | Paso | Cómo se hace | Por qué |
|---|---|---|---|
| **1** | El archivo del secreto, en `.gitignore` | Agregue `appsettings.Production.json` al `.gitignore`. Compruebe con `git check-ignore -v api_facturas\appsettings.Production.json` | Lo que entra a git **queda en el historial para siempre**. Borrarlo después no lo saca |
| **2** | Versionar una plantilla `.ejemplo` | `copy appsettings.Production.json appsettings.Production.json.ejemplo`, y en la **copia** reemplace la clave por `SU_CONTRASENA`. Esa copia **sí** se agrega a git | El que clona necesita saber **qué archivo crear y qué va adentro**. Sin plantilla, acaba pidiéndole el archivo real a un compañero — y así se filtran de verdad |
| **3** | La clave en `dotnet user-secrets` | En `api_facturas`: `dotnet user-secrets init` y luego `dotnet user-secrets set "ConnectionStrings:Postgres" "Host=...;Password=LA_CLAVE;..."`. Después **borre la contraseña del `.json`** | Saca el secreto de la carpeta del proyecto: no se sube por error, no viaja al copiar o comprimir, **y OneDrive no lo sincroniza** |
| **4** | En el servidor, variable de entorno | Según el hospedaje — ver la tabla de más abajo | La aplicación la lee **encima** de los `appsettings`, sin cambiar una línea de código |

**Con esos cuatro, la clave nunca queda expuesta y nunca hay que rotar.**

### El paso 1 y el paso 2 son opuestos, no lo mismo

| Archivo | Tiene la clave real | ¿Sube a git? |
|---|---|---|
| `appsettings.Production.json` | **sí** | **no** |
| `appsettings.Production.json.ejemplo` | no, dice `SU_CONTRASENA` | **sí** |

El paso 1 dice **qué no sube**. El paso 2 dice **qué sube en su lugar**.

### El paso 3, en detalle

| Comando | Qué hace |
|---|---|
| `dotnet user-secrets init` | Le agrega al `.csproj` un identificador —un número largo—. **Eso es lo único que queda en el proyecto**, y no es un secreto |
| `dotnet user-secrets set "clave" "valor"` | Guarda el valor en `C:\Users\<usted>\AppData\Roaming\Microsoft\UserSecrets\<ese-número>\secrets.json` |

**Cómo lo lee la API: sola.** Al arrancar ve el identificador del `.csproj`, busca ese archivo y lo superpone a los `appsettings` — igual que `appsettings.Production.json` se superpone a `appsettings.json`.

> **Dónde queda:** dentro de su PC, **fuera de la carpeta del proyecto** y por lo tanto fuera de OneDrive.
>
> **Qué no resuelve:** quien entre con su usuario de Windows lo puede leer. Eso ya es la seguridad de la máquina, y no lo arregla ningún archivo de configuración.
>
> **Y un límite importante:** `user-secrets` funciona **solo en desarrollo**. En el servidor publicado no existe — ahí manda el paso 4.

### El paso 4, en detalle

**El nombre cambia de forma.** En un archivo es `ConnectionStrings:Postgres` (dos puntos); como variable de entorno los dos puntos no se permiten, así que van **dos guiones bajos**: `ConnectionStrings__Postgres`. Es la misma clave y la aplicación la encuentra igual.

> Ya hay un ejemplo real de esto en el proyecto: el [`docker-compose.yml`](docker-compose.yml) pasa `ConnectionStrings__SqlServer` y `Jwt__Key` así.

**Y dónde se pone depende del hospedaje:**

| Hospedaje | Dónde va | ¿Queda en un archivo? |
|---|---|---|
| **Render** (el front de Flask) | Panel → **Environment** → *Environment Variables* | **No.** Render lo guarda en su propio almacén y lo inyecta al proceso al arrancar |
| **MonsterASP** (la API) | No tiene panel de variables. Hay que ponerlas en el `web.config` del sitio, dentro de `<aspNetCore>` | **Sí**, y eso cambia mucho — ver abajo |

```xml
<aspNetCore processPath="dotnet" arguments=".\ApiFacturas.dll">
  <environmentVariables>
    <environmentVariable name="ConnectionStrings__Postgres" value="Host=...;Password=..." />
  </environmentVariables>
</aspNetCore>
```

---

## El límite honesto: en MonsterASP el paso 4 cumple a medias

**Un `web.config` se puede abrir desde el panel.** Entonces, en MonsterASP, poner la clave ahí o en el `appsettings.json` es **prácticamente lo mismo**: las dos son archivos legibles en el servidor.

**Lo único que de verdad se gana** es que la contraseña no esté en un archivo **de su proyecto** — el que se sube a git, se copia y se sincroniza. En el servidor sigue expuesta a quien tenga el panel.

**Por qué no hay forma de evitarlo.** Una variable de entorno tiene que declararse en algún lado, y hay dos:

| Dónde se declara | ¿Se puede en MonsterASP gratuito? |
|---|---|
| En el `web.config` del sitio | **Sí** — pero es un archivo del sitio |
| En el sistema operativo o en el *app pool* de IIS | **No.** Es un servidor compartido y no hay acceso a ese nivel |

**En resumen: con MonsterASP gratuito, el secreto es tan seguro como la contraseña de su cuenta de MonsterASP.** Es un límite del hospedaje, no del método.

**Qué se hace cuando el hospedaje no da más:**

| | |
|---|---|
| **Privilegio mínimo** | Que ese usuario de base de datos pueda hacer solo lo que la aplicación necesita, no ser dueño de todo |
| **No reutilizar** | Esa contraseña en ningún otro sitio: si se filtra, se pierde una sola cosa |
| **Rotar al cerrar** | Fin del curso, claves nuevas |

> **Un detalle práctico que muerde:** `msdeploy -verb:sync` deja el destino idéntico al origen, así que **sobrescribe el `web.config` en cada publicación**. O lo vuelve a poner cada vez, o lo genera desde el `.csproj` para que salga ya con la variable.

---

## Cómo funciona en Render, que sí tiene variables de verdad

**El valor se guarda en el almacén de Render**, no en el repositorio ni en ningún archivo del servicio. Al arrancar el contenedor, Render se lo inyecta al proceso; el código lo lee con `os.environ.get(...)` y nunca sabe de dónde salió.

**Para verlo o cambiarlo:** panel → su servicio → **Environment** → *Environment Variables*. Los valores aparecen ocultos, con un **Reveal** al lado. Al guardar un cambio, Render **reinicia el servicio** para que el proceso arranque con el valor nuevo.

| | MonsterASP | Render |
|---|---|---|
| Dónde vive el valor | en un archivo del sitio | en el almacén de Render |
| ¿Se puede abrir el archivo? | **sí** | no hay archivo |
| Quién lo puede ver | quien entre al panel | quien entre al panel |

**Fíjese en la última fila: en los dos casos, quien entra al panel lo ve.** Render no se lo esconde a usted — tiene que poder recuperarlo. Lo que cambia es que **no queda escrito en ningún archivo**, así que no se filtra por copiar la carpeta, por subirla a git, ni por un respaldo olvidado.

---

## ¿Y un gestor de secretos? Se puede, pero reduce el problema, no lo elimina

**Un gestor de secretos no es una función del hospedaje: es código en su aplicación.** La API, al arrancar, le pide la contraseña a un servicio externo —Azure Key Vault, AWS Secrets Manager, Infisical, Doppler— y la recibe por la red. Nada de eso depende del hospedaje, así que **sí funcionaría en MonsterASP**.

**El problema que queda:** para pedirle un secreto al gestor hay que **autenticarse ante el gestor**, y esa credencial tiene que estar en algún lado — otra vez, en un archivo del sitio.

| | Secretos en archivos del servidor |
|---|---|
| Sin gestor | la clave de la base de datos, la de los tokens, y cuantas haya |
| Con gestor | **una sola**: la del gestor |

Eso se llama **el secreto de arranque**, y no tiene solución por software. Se resuelve con **identidad administrada** —cuando el servidor mismo prueba quién es, sin credencial— pero eso solo lo dan Azure, AWS y Google **a sus propias máquinas**. En un hospedaje compartido ajeno, no existe.

**Lo que sí se gana, y no es poco:** una sola credencial que vigilar, rotación centralizada, y registro de quién pidió qué secreto y cuándo.

**Para este curso no hace falta.** Agrega una cuenta más que administrar, y lo que enseña —que la cadena de confianza termina en algo que hay que guardar en alguna parte— cabe en una frase.

---

## Si el secreto YA se expuso: la reparación

**Esto no es una práctica, es un arreglo.** Solo aplica si la clave ya salió: se subió a git, se mandó por chat, se sincronizó a una nube, se mostró en una clase.

| # | Paso | Cómo se hace | Por qué |
|---|---|---|---|
| **5** | **Rotar** la contraseña | En Neon: panel → su proyecto → **Roles** → el rol → **Reset password**. En MonsterASP: **Databases → la base de datos → Local access → Password**. Luego ponga la nueva en los pasos 3 y 4 | Una clave vista **no se puede desexponer**. Borrarla de un archivo no la borra de la memoria de quien la leyó, ni del historial de git, ni de la nube donde se sincronizó |
| **6** | Buscar si quedó algún rastro | `Select-String -Path . -Pattern "npg_" -Recurse` y revise `git status`. Si aparece en un archivo versionado, sáquelo y vuelva al paso 5 | Es fácil dejar una copia olvidada: unos apuntes, un `.txt`, una carpeta de publicación, un respaldo |

> **Rotar no es mover.** Mover es la misma contraseña en otro lugar — si alguien ya la vio, **sigue entrando**. Rotar es generar una **nueva** y dejar la vieja inservible.

> **El orden importa:** los pasos 3 y 4 **antes** del 5. Si rota primero, la aplicación queda caída hasta que ponga la nueva en los dos lados.

---

## Los secretos de este proyecto, y dónde vive cada uno

| Secreto | Dónde vive | ¿Es secreto de verdad? |
|---|---|---|
| `MSSQL_SA_PASSWORD`, `POSTGRES_PASSWORD` locales | [`docker-compose.yml`](docker-compose.yml), a la vista | **No.** Son de desarrollo y esas bases de datos solo existen en su máquina |
| `Jwt__Key` local | el mismo compose — dice `...NoEsUnSecreto!` | **No**, por lo mismo |
| Contraseña de **SQL Server del hospedaje** | `api_facturas/appsettings.Production.json` (en `.gitignore`) | **Sí** |
| Contraseña de **PostgreSQL en Neon** | `api_facturas/appsettings.Production.postgres.json` (en `.gitignore`) | **Sí** |
| `Jwt__Key` **publicada** | el mismo archivo. Con ella se pueden **falsificar sesiones** | **Sí** |
| Contraseñas de **WebDeploy** | solo en el panel de MonsterASP | **Sí** |
| `CLAVE_SESION` del front de Flask | el panel de Render. Firma la cookie de sesión | **Sí** |

> **En los tutoriales las credenciales aparecen a propósito**, porque sin verlas no se entiende de dónde sale cada dato. Por eso **todas quedan quemadas** y todas se rotan al cerrar el curso. En el proyecto de aula van por los pasos 1 a 4, nunca a la vista.

---

**Hojas relacionadas:**
[Cómo se conecta cada front con su API](CONEXIONES.md) ·
[README](README.md) ·
los tutoriales de publicación
[1](TUTORIAL_1_SQLSERVER_BLAZOR.md) ·
[2](TUTORIAL_2_POSTGRES_FLASK.md) ·
[3](TUTORIAL_3_POSTGRES_BLAZOR.md) ·
[4](TUTORIAL_4_POSTGRES_REACT.md)
