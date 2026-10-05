# Versión 3 — Luis · camino A (chat web)

> **Su parte:** la pantalla de **entrada**, el **menú que se adapta al rol** y la
> pantalla para **repartir permisos**.
>
> **Su herramienta en esta versión:** un chat web. **Es la última**: desde la v4
> pasa a agente — [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3.
>
> **Usted va de último, y no es casualidad:** necesita el token de Carlos para
> entrar y los permisos de Paco para saber qué esconder.

---

## 1. Lo suyo es la única mitad que el usuario ve

Carlos y Paco construyeron la seguridad. **Nadie la va a notar si usted no la
muestra bien** — y, peor, la van a sufrir si la muestra mal.

| Pantalla | Qué hace |
|---|---|
| **Entrar** | Pide correo y contraseña, guarda el token |
| **El menú** | Muestra **solo** lo que ese rol puede abrir |
| **Permisos** | Reparte rutas a roles, sin tocar la base a mano |

---

## 2. La regla que define su parte: **esconder NO es proteger**

**Léala antes del prompt**, porque de aquí sale el error conceptual más grave que
se puede cometer en esta versión.

> **El menú esconde. La API protege. Son dos cosas, y ninguna reemplaza a la
> otra.**

| | Qué hace | Si falta |
|---|---|---|
| **El menú por rol** | No muestra lo que no se puede abrir | La interfaz es **incómoda**: botones que dan error |
| **`[ExigePermiso]`** (de Paco) | **Rechaza la petición** | El sistema está **abierto** |

> **Si usted esconde un botón y la API no exige el permiso, el sistema NO está
> protegido**: cualquiera que escriba la dirección a mano entra. El menú es
> comodidad, no seguridad.
>
> **Y al revés: si la API protege pero el menú no esconde**, el sistema es
> seguro y **desagradable** — la persona hace clic y recibe un 403 por algo que
> nunca debió ver.
>
> **Las dos cosas se hacen, y por razones distintas.** Si en la sustentación le
> preguntan *«¿para qué esconder el menú si la API ya rechaza?»*, ésta es la
> respuesta.

---

## 3. Las rutas son etiquetas, no direcciones

```
interfaz.facturas    ← el nombre de una PANTALLA, en la tabla `ruta`
/api/factura         ← la dirección de un recurso de la API
```

> **No son lo mismo.** Su menú pregunta *«¿este usuario alcanza
> `interfaz.facturas`?»* y según la respuesta muestra o no el enlace a
> `/facturas`. La etiqueta y la dirección son parecidas **a propósito**, para que
> se lean fácil — pero son tablas distintas.

Las quince que existen:

```
interfaz.inicio      interfaz.usuarios    interfaz.facturas
interfaz.clientes    interfaz.vendedores  interfaz.personas
interfaz.empresas    interfaz.productos   interfaz.roles
interfaz.permisos    interfaz.rutas       permiso.crear
permiso.eliminar     ruta.crear           ruta.eliminar
```

> **Las cinco últimas no empiezan por `interfaz.`**, y es a propósito: protegen
> **acciones**, no pantallas. Repartir permisos es algo que **se hace**, no algo
> que se mira.

---

## 4. Lo que el front NO puede hacer

| Prohibido | Por qué |
|---|---|
| Hablar con la base de datos | Artículo 3. Solo HTTP contra la API |
| **Decidir** si alguien tiene permiso | Eso lo decide la base. El front **pregunta y obedece** |
| Guardar la contraseña | Guarda **el token**, y nada más |

> **El front nunca calcula un permiso.** Le pregunta a la API qué puede abrir
> este usuario, y pinta eso. Si usted arma la lógica de roles en C# del lado del
> front, cuando alguien cambie un permiso en la base el menú va a mentir.

---

## 5. Antes de abrir el chat

```powershell
# 1 · Traiga lo de Carlos Y lo de Paco. Usted va de último.
git switch main ; git pull origin main

# 2 · Compruebe que están los dos.
Get-ChildItem api_facturas\Controllers\SesionController.cs
Get-ChildItem api_facturas\Autorizacion\ExigePermisoAttribute.cs

# 3 · Su rama, y su identidad. Compruébela.
git switch -c rama-luis-v3
git config user.name "ccastro202050"
git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

---

## 6. Qué subirle al chat

| # | Archivo |
|---|---|
| 1 | [`2_spec.md`](2_spec.md) · [`6_contracts.md`](6_contracts.md) |
| 2 | **`SesionController.cs`** — para saber qué devuelve el login |
| 3 | **`ExigePermisoAttribute.cs`** de Paco — para ver cómo se pregunta el permiso |
| 4 | **Una pantalla del front que ya funcione** — el molde |
| 5 | **El archivo del menú / la navegación** — es lo que va a modificar |

---

## 7. El prompt (cópielo tal cual como PRIMER mensaje)

```
Actúa como mi asistente de programación en un proyecto universitario que
hacemos TRES personas. El proyecto YA tiene dos versiones funcionando, y
mis dos compañeros acaban de agregar la seguridad en la API: existe
POST /api/sesion que devuelve un token JWT, y un atributo [ExigePermiso]
que responde 403 cuando falta un permiso.

MI PARTE ES SOLO EL FRONT. No toco la API.

Te adjunto los documentos del spec kit y los archivos que necesito. El
front es Blazor Server y habla con la API SOLO por HTTP.

LO QUE TENGO QUE CONSTRUIR, Y SON TRES PANTALLAS:

  1. ENTRAR: pide correo y contraseña, llama a POST /api/sesion, y guarda
     el token para mandarlo en la cabecera Authorization de todas las
     peticiones siguientes. Si las credenciales fallan, muestra UN SOLO
     mensaje genérico — no digas si el correo existe o si la clave está
     mal.

  2. EL MENÚ QUE SE ADAPTA AL ROL: después de entrar, el menú muestra
     SOLO las pantallas que ese usuario puede abrir. Para saberlo,
     PREGÚNTALE A LA API — no calcules nada en el front.

  3. REPARTIR PERMISOS: una pantalla donde se asignan rutas a roles,
     usando los endpoints que ya existen. Los dos lados en desplegables
     cargados de la API, nunca campos de texto.

TRES COSAS QUE NO SE NEGOCIAN:

  1. EL FRONT NO DECIDE PERMISOS. Pregunta y obedece. Si armas la lógica
     de roles en C# del lado del front, el día que alguien cambie un
     permiso en la base el menú va a mentir.

  2. ESCONDER NO ES PROTEGER. El menú oculta lo que no se puede abrir
     para que la interfaz sea cómoda; quien de verdad rechaza es la API
     con su 403. Las dos cosas se hacen, por razones distintas. Si
     alguien escribe la dirección a mano, tiene que recibir el 403 de la
     API — no un error del front.

  3. LAS RUTAS SON ETIQUETAS, NO DIRECCIONES. La tabla `ruta` guarda
     nombres como "interfaz.facturas", que NO son la dirección
     /api/factura. El menú pregunta por la etiqueta y según la respuesta
     muestra el enlace a la pantalla.

Y TRES DETALLES QUE SE ROMPEN EN SILENCIO:

  · Si el token no se manda en la cabecera, TODO responde 401 y la
    pantalla sale vacía sin decir por qué. Pon un aviso claro.
  · Si la sesión expira, hay que mandar a la persona a entrar otra vez,
    no dejarla mirando una pantalla en blanco.
  · Carga las listas de los desplegables con Task.WhenAll, no una tras
    otra: si la API está caída, varias esperas en fila son casi un minuto
    en blanco.

REGLAS QUE SIGUEN VIGENTES:
  · El front NO habla con la base de datos. Solo HTTP contra la API.
  · TODO EN ESPAÑOL, y la interfaz NO habla en jerga: nada de "401",
    "403", "token" ni "PUT" en la pantalla. La persona lee "su sesión
    terminó" o "no tiene permiso para ver esto".
  · NO toques nada de api_facturas/: es de mis compañeros.

COMENTA TODO, en español, diciendo POR QUÉ está escrito así.

EMPIEZA POR LA PANTALLA DE ENTRAR, que es la que desbloquea todo lo demás.
Entrégame archivo por archivo y NO sigas hasta que yo te diga que puedo
entrar y que el token se está mandando.
```

> **Y ahora la parte que el prompt no puede hacer por usted: LEER esos
> comentarios.** La IA comenta lo que **cree** que hizo, y no siempre coincide
> con lo que hizo. **Un comentario equivocado es peor que ninguno**, porque el
> siguiente que lo lea le va a creer.
>
> | | |
> |---|---|
> | El comentario **como producto** | sirve para quien llegue en seis meses |
> | El comentario **como ejercicio** | **revisarlo lo obliga a entender.** Ahí está el valor para quien aprende |
> | El comentario **como evidencia** | vale poco: se puede generar sin entender nada |
>
> **Por eso la interpretabilidad se califica HABLANDO.** Un comentario se puede
> recitar; una respuesta a *«¿y si cambiamos esto?»* no. Si usted no puede juzgar
> si un comentario es **cierto**, no entendió el código — y ésa es exactamente la
> señal que hay que buscar mientras revisa.


> **Fíjese en la regla del idioma**, que en esta versión es más difícil de lo que
> parece: usted va a manejar 401 y 403 todo el día, y **ninguno de los dos puede
> aparecer en la pantalla**. La persona lee *«su sesión terminó»* o *«no tiene
> permiso para ver esto»* — nunca un número.

---

## 8. Comprobar lo suyo antes del PR

```powershell
Start-Process http://localhost:8099/
```

| # | Qué probar | Qué debe pasar |
|---|---|---|
| **1** | Abrir el sistema sin haber entrado | Manda a la pantalla de entrar |
| **2** | Entrar con `admin@correo.com` / `admin123` | Entra, y el menú muestra **todo** |
| **3** | Entrar con `vendedor1@correo.com` / `vendedor123` | Ve **Facturas y Clientes**; NO ve Usuarios, Personas ni Productos |
| **4** | Entrar con `cliente1@correo.com` / `cliente123` | Ve **Productos**; NO ve Facturas ni Clientes |
| **5** | Credenciales malas | **Un solo mensaje**, que no diga cuál de los dos falló |

> **Los pasos 3 y 4 están escogidos a propósito:** el vendedor y el cliente ven
> **cosas distintas, no una el subconjunto de la otra**. Si uno viera lo mismo
> que el otro más algo, un error de permisos podría pasar desapercibido.

```
6 · LA PRUEBA QUE SEPARA ESCONDER DE PROTEGER:
    entre como cliente1, y escriba A MANO la dirección http://localhost:8099/facturas

    Debe recibir un aviso de que no tiene permiso —porque la API respondió 403—,
    NO la pantalla de facturas vacía ni un error feo del front.
```

> **Ése es el paso que demuestra que entendió la §2.** Si la pantalla se abre
> —aunque salga sin datos—, el menú estaba escondiendo algo que la API no estaba
> protegiendo. Y si sale un error técnico en pantalla, falta el aviso en
> castellano.

```powershell
# 7 · Y que el front no toque la base. Tiene que dar 0.
Select-String -Path front_blazor\**\*.cs,front_blazor\**\*.razor `
  -Pattern 'SqlConnection|Npgsql' | Measure-Object | Select-Object Count
```

---

## 9. Subir y abrir el PR

```powershell
git status                      # SOLO archivos de front_blazor
git add front_blazor/
git commit -m "feat: la pantalla de entrar y el menu que se adapta al rol"
git push -u origin rama-luis-v3
#   y el Pull Request. Carlos revisa y fusiona; usted NO.
```

> **En la descripción del PR cuente el resultado del paso 6.** Es la prueba de
> que el sistema está protegido y no solo escondido — y es lo que Carlos tiene
> que verificar antes de poner el tag.

> **Nadie toca el archivo de otro, ni para arreglárselo.**
