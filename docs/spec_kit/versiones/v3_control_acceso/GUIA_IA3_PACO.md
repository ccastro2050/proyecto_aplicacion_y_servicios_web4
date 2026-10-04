# Versión 3 — Paco · camino A (chat web)

> **Su parte:** el **hash de la contraseña** y el **permiso** — la segunda
> puerta: *«y usted, ¿puede?»*.
>
> **Su herramienta en esta versión:** un chat web. **Es la última**: desde la v4
> pasa a agente — [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3.
>
> **No empiece hasta que el PR de Carlos esté fusionado:** usted necesita que ya
> exista un token que diga **quién** es alguien, para poder preguntar **si
> puede**.

---

## 1. Sus dos mitades, y son independientes

| | Qué hace | Responde |
|---|---|---|
| **El hash** | La contraseña deja de guardarse en claro | — |
| **El permiso** | `[ExigePermiso]` pregunta a la base antes de dejar pasar | **403** |

---

## 2. La mitad del hash

### Lo que ya existe, y lo que usted pone

| Ya está | Lo pone usted |
|---|---|
| La columna `contrasena NVARCHAR(200)` | El **cifrado** al crear y al cambiar |
| El paquete `BCrypt.Net-Next 4.0.3` en el `.csproj` | La **verificación** al identificarse |

> **La columna es de 200 caracteres a propósito**, aunque un hash de BCrypt mida
> 60: deja espacio para un algoritmo futuro más largo. Es una decisión del modelo
> que se tomó en la fase 0.

### Y la decisión de diseño que ya está tomada, y conviene que la entienda

Abra `api_facturas/Modelos/Usuario.cs`. Son tres líneas:

```csharp
public class Usuario
{
    public required string Email { get; set; }
}
```

> **No tiene la contraseña. Ni cifrada.** Y por eso **ninguna respuesta de la API
> puede devolverla jamás** — no porque alguien se acuerde de quitarla en cada
> endpoint, sino **porque la clase que viaja no la tiene**.
>
> **Ésa es la diferencia entre una regla y un diseño.** Una regla hay que
> recordarla en veinte sitios; un diseño la hace imposible de romper. Si usted
> le agrega un campo `Contrasena` a esa clase «para que el servicio lo use», rompe
> la garantía entera.

> **Lo que sí lleva la contraseña es la PETICIÓN de entrada** —`UsuarioCrear`,
> `SesionCrear`—, que es lo que llega de afuera. Lo que **sale** no la tiene.
> Frontera de entrada y modelo que viaja son dos cosas, y aquí se ve por qué.

---

## 3. La mitad del permiso

```sql
verificar_acceso_ruta(@p_email, @p_fkidruta)
   -- cruza:  usuario → rol_usuario → rutarol
   -- devuelve: {"tiene_acceso": 0|1, ...}
```

**El procedimiento ya existe.** Usted no escribe el `JOIN` en C#: lo llama.

> **Por qué la consulta está en la base y no armada en C#.** Porque la pregunta
> *«¿este correo alcanza esta ruta?»* es un cruce de tres tablas, y si se arma en
> la aplicación, cada quien lo arma a su manera. Con el procedimiento hay **una
> sola respuesta posible**, y vale también para quien consulte por SSMS.

### El permiso se pregunta EN CADA PETICIÓN

> Y **no** se lee del token. Carlos dejó en el token solo el correo, a propósito.
>
> **Qué compra:** quitarle un permiso a un rol surte efecto **de inmediato**. Si
> viviera en el token, el jefe que quita un acceso tendría que pedirle a esa
> persona que vuelva a entrar.
>
> **Qué cuesta:** una consulta a la base por petición. Se paga a sabiendas, y
> está declarado en **RN-21**.

### Y las rutas no son direcciones de la API

```
interfaz.facturas    ← es el nombre de una PANTALLA
/api/factura         ← es la dirección de un recurso
```

> **No son lo mismo y no se mezclan.** Una fila de `ruta` es **una etiqueta** que
> el control de acceso usa para decidir. Buscar `/api/interfaz.facturas` no lleva
> a ninguna parte.
>
> Si usted inventa un nombre de ruta que no está en la tabla, `[ExigePermiso]` va
> a responder 403 **siempre**, para todo el mundo, sin error — porque esa
> etiqueta no existe y nadie la tiene asignada.

---

## 4. Antes de abrir el chat

```powershell
# 1 · Traiga lo de Carlos. Sin su token usted no puede probar nada.
git switch main ; git pull origin main

# 2 · Compruebe que llegó.
Get-ChildItem api_facturas\Controllers\SesionController.cs

# 3 · Su rama, y su identidad. Compruébela.
git switch -c rama-paco-v3
git config user.name "ccastro2050-50"
git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

---

## 5. Qué subirle al chat

| # | Archivo |
|---|---|
| 1 | [`1_constitution.md`](../../1_constitution.md) |
| 2 | [`2_spec.md`](2_spec.md) — lo escribió usted |
| 3 | [`4_research.md`](4_research.md) — **por qué el permiso se consulta cada vez** |
| 4 | [`6_contracts.md`](6_contracts.md) |
| 5 | **`Modelos/Usuario.cs`** — para que vea que **no tiene contraseña** |
| 6 | **`Servicios/ServicioUsuario.cs`** — donde va el hash |
| 7 | **`SesionController.cs`** de Carlos — para ver cómo se lee el correo del token |
| 8 | Un controlador cualquiera — para calcar dónde se pone el atributo |

**No suba** `9_checklist.md` ni `db/bdfacturas.sql`.

---

## 6. El prompt (cópielo tal cual como PRIMER mensaje)

```
Actúa como mi asistente de programación en un proyecto universitario que
hacemos TRES personas. El proyecto YA tiene dos versiones funcionando, y
un compañero acaba de agregar el token: existe POST /api/sesion y los
controladores ya exigen [Authorize]. MI PARTE son dos cosas: el cifrado
de la contraseña y el permiso.

Te adjunto los documentos del spec kit y los archivos del proyecto. Es C#
sobre ASP.NET Core (.NET 10) con SQL Server y Dapper.

PARTE 1 — EL CIFRADO DE LA CONTRASEÑA

  El paquete BCrypt.Net-Next 4.0.3 YA ESTÁ en el .csproj.

  · Al CREAR un usuario y al CAMBIARLE la contraseña, se guarda el HASH,
    nunca el texto.
  · Al identificarse, se VERIFICA contra el hash.
  · Esto va EN EL SERVICIO de usuario, no en el controlador ni en el
    repositorio.

  Y NO TOQUES la clase Modelos/Usuario.cs. Solo tiene Email, y es A
  PROPÓSITO: así ninguna respuesta de la API puede devolver la contraseña
  aunque alguien se olvide de quitarla. La contraseña viaja en las
  PETICIONES de entrada, no en el modelo que sale. Si le agregas un campo
  Contrasena, rompes esa garantía.

PARTE 2 — EL PERMISO

  Escribe un atributo [ExigePermiso("nombre.de.la.ruta")] que, ANTES de
  que el controlador se ejecute:
    1. Lee el correo del token (ya está ahí, lo puso mi compañero).
    2. Busca el id de esa ruta en la tabla `ruta`.
    3. Llama al procedimiento verificar_acceso_ruta(@p_email, @p_fkidruta)
       — YA EXISTE en la base, NO escribas el JOIN en C#.
    4. Si tiene_acceso es 0, responde 403. Si es 1, deja pasar.

  Y pónselo a los controladores que lo necesiten, con el nombre de ruta
  que YA EXISTE en la tabla: interfaz.facturas, interfaz.clientes,
  interfaz.productos, etc. NO INVENTES nombres de ruta: si la etiqueta no
  está en la tabla, el atributo va a responder 403 a todo el mundo
  siempre, sin error.

DOS COSAS QUE NO SE NEGOCIAN:

  1. EL PERMISO SE CONSULTA EN CADA PETICIÓN, no se lee del token. El
     token solo trae el correo, a propósito: así quitarle un permiso a un
     rol surte efecto de inmediato, sin esperar a que el token expire.
     NO metas los permisos en el token ni los guardes en memoria.

  2. 401 Y 403 SON DISTINTOS. 401 es "no sé quién eres" y lo responde
     [Authorize]. 403 es "sé quién eres y no puedes" y lo responde mi
     atributo, DESPUÉS. No los mezcles ni respondas 401 cuando falta un
     permiso.

REGLAS QUE SIGUEN VIGENTES:
  · Las TRES CAPAS. El servicio NO nombra nada de HTTP —ni StatusCode, ni
    NotFound, ni IActionResult—. El atributo SÍ puede: vive en la capa
    HTTP, no en el servicio.
  · SIN ORM. Dapper, parametrizado.
  · TODO EN ESPAÑOL.
  · NO toques la base: el procedimiento ya existe.
  · NO toques el token ni el menú del front: son de mis compañeros.

COMENTA TODO, en español, diciendo POR QUÉ. En este código la razón
importa más que en ningún otro: un [ExigePermiso("interfaz.facturas")]
sin comentario no dice de dónde sale esa cadena ni quién la reparte.

EMPIEZA POR EL CIFRADO, que es independiente y se puede probar solo.
Entrégame archivo por archivo y NO sigas con el permiso hasta que yo te
diga que el hash funciona.
```

---

## 7. Comprobar lo suyo antes del PR

```powershell
# --- EL HASH ---
# 1 · Cree un usuario y mire la base: NO debe verse la contraseña.
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa `
  -P "Paradigmas123!" -C -d bdfacturas_sqlserver_local `
  -Q "SELECT TOP 3 email, contrasena FROM usuario;"
#    espera: cadenas que empiezan por $2a$ o $2b$ — nunca texto legible

# 2 · Y que la API no la devuelva NUNCA.
$r = Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
  -ContentType 'application/json' -Body '{"email":"admin@correo.com","contrasena":"admin123"}'
$h = @{ Authorization = "Bearer $($r.token)" }
Invoke-RestMethod http://localhost:8035/api/usuario -Headers $h
#    espera: ni rastro de contrasena

# --- EL PERMISO ---
# 3 · EL 403, que es lo suyo. Entre como cliente y pida facturas.
$c = Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
  -ContentType 'application/json' -Body '{"email":"cliente1@correo.com","contrasena":"cliente123"}'
Invoke-RestMethod http://localhost:8035/api/factura `
  -Headers @{ Authorization = "Bearer $($c.token)" }
#    espera: 403 — NO 401. El sistema sabe quién es; lo que no tiene es permiso

# 4 · Y el mismo endpoint SIN token: 401, no 403.
Invoke-RestMethod http://localhost:8035/api/factura
#    espera: 401

# 5 · QUE SURTE EFECTO YA: quítele un permiso a un rol desde la base o la
#     pantalla, y repita el paso 3 CON EL MISMO TOKEN. Debe cambiar.
```

> **Los pasos 3 y 4 juntos son la demostración de la versión.** El mismo
> endpoint, dos fallos distintos: uno dice *«no sé quién eres»* y el otro *«sé
> quién eres y no puedes»*. Si los dos responden lo mismo, una de las dos puertas
> no existe.

> **Y el paso 5 es el que prueba RN-21.** Si hay que volver a entrar para que el
> cambio surta efecto, el permiso se está leyendo del token — y eso es justo lo
> que se decidió no hacer.

```powershell
# El servicio no nombra HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\ServicioUsuario.cs `
  -Pattern 'StatusCode|NotFound|IActionResult' | Measure-Object | Select-Object Count

# Y el modelo Usuario NO tiene contraseña. Tiene que dar 0.
Select-String -Path api_facturas\Modelos\Usuario.cs -Pattern 'ontrasena' |
  Measure-Object | Select-Object Count
```

---

## 8. Subir y abrir el PR

```powershell
git status                      # SOLO sus archivos
git add api_facturas/Autorizacion/ExigePermisoAttribute.cs
git commit -m "feat: ExigePermiso, que pregunta a la base antes de dejar pasar"
git push -u origin rama-paco-v3
#   y el PR. Avísele a Luis: sin sus permisos él no puede armar el menú.
```

> **En la descripción del PR diga qué nombres de ruta usó y de dónde los sacó.**
> Es lo que Carlos va a revisar primero: una etiqueta inventada responde 403 a
> todo el mundo y **no da ningún error**.

> **Nadie toca el archivo de otro, ni para arreglárselo.**
