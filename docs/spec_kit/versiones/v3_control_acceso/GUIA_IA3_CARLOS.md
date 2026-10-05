# Versión 3 — Carlos · camino B (IDE agéntico)

> **Su parte:** el **token** y la **sesión** — la primera de las dos puertas:
> *«¿quién es usted?»*.
>
> **Su herramienta:** un IDE agéntico.
>
> **Y usted va primero, obligatoriamente.** Esta versión es una cadena: Paco no
> puede probar un 403 sin un token que identifique a alguien.

---

## 1. Lo que usted construye

| | |
|---|---|
| **El endpoint de identificación** | `POST /api/sesion` — devuelve el token si las credenciales son correctas |
| **La puerta** | `[Authorize]` en todo lo que ya existe: sin token, **401** |
| **La configuración del token** | La firma, el emisor, la vigencia |

**No construye** el hash de la contraseña ni el permiso —son de Paco— ni el menú
—es de Luis—.

---

## 2. El error que esta versión tiene preparado, y es silencioso

**Léalo ahora**, porque el agente lo va a cometer y el compilador **solo avisa
con un *warning*** que es fácil pasar por alto.

`SesionController` necesita que **su POST sea público** —si no, nadie podría
identificarse nunca—. Lo natural es escribir:

```csharp
[AllowAnonymous]            // ← en la CLASE
[ApiController]
public class SesionController : ControllerBase
{
    [HttpPost]              public IActionResult Entrar(...)      { }
    [Authorize] [HttpGet]   public IActionResult VerSesion(...)   { }   // ← creía estar protegido
}
```

> **Y no lo está.** `[AllowAnonymous]` **en la clase anula los `[Authorize]` de
> los métodos**. Ese `GET` queda abierto a cualquiera, sin token, sin aviso en
> tiempo de ejecución y **con todo funcionando**.
>
> ASP.NET sí lo dice, pero como advertencia: **ASP0026**. Entre cincuenta líneas
> de salida de compilación, nadie la ve.

**Así se escribe bien** — al revés:

```csharp
[Authorize]                 // ← la clase protegida por defecto
[ApiController]
public class SesionController : ControllerBase
{
    [AllowAnonymous] [HttpPost]  public IActionResult Entrar(...)  { }  // ← la excepción, explícita
    [HttpGet]                    public IActionResult VerSesion(...) { }
}
```

> **La regla general, y vale para todo el sistema: cerrado por defecto, abierto
> por excepción.** Si se olvida un atributo, el peor caso es que algo quede
> protegido de más — se nota en diez segundos y nadie sale lastimado. Al revés,
> el peor caso es un endpoint abierto que nadie descubre.

---

## 3. Dos decisiones del login que parecen detalles

### No se delata cuál de los dos campos falló

| El usuario escribe | Lo que la API responde |
|---|---|
| Un correo que **no existe** | **401** · *«Credenciales inválidas»* |
| Un correo que existe, con **la clave mala** | **401** · *«Credenciales inválidas»* — **idéntico** |

> **Por qué, y no es paranoia:** si las respuestas fueran distintas, cualquiera
> podría **averiguar qué correos están registrados** probándolos uno por uno. El
> mensaje tiene que ser el mismo, y el tiempo de respuesta también.

### El token dice QUIÉN es, no QUÉ puede

> **En el token va el correo. Los permisos NO.** Se consultan en cada petición
> contra la base de datos — eso es de Paco, pero usted tiene que dejarle el correo
> disponible para que él lo lea.
>
> **Si usted mete los roles en el token**, Paco va a construir sobre eso y el
> sistema entero va a quedar con permisos que tardan en surtir efecto. Es la
> decisión suya que más afecta a los otros dos.

---

## 4. Antes de abrir el agente

```powershell
git switch main ; git pull origin main
git switch -c rama-carlos-v3

# SU IDENTIDAD, parado en la carpeta del proyecto. Compruébela.
git config user.name "ccastro2050"
git config user.email "su-correo-de-github"
git config user.name ; git config user.email

# Y compruebe que la v2 funciona ANTES de ponerle la puerta.
docker compose up -d --build
Invoke-RestMethod http://localhost:8035/api/factura
```

---

## 5. El prompt (cópielo tal cual)

```
Agrega AUTENTICACIÓN a este proyecto, que ya tiene dos versiones
funcionando. Trabajo en equipo: un compañero hará el hash de la
contraseña y los permisos, y otro el menú. YO HAGO SOLO EL TOKEN Y LA
SESIÓN.

PRIMERO lee los documentos bajo docs/spec_kit/ (1_constitution.md y los
de versiones/v3_control_acceso/), y el código que ya existe. Después
resume en máximo 10 líneas qué vas a construir y ESPERA MI CONFIRMACIÓN.

LO QUE CONSTRUYO YO, Y NADA MÁS:

  1. POST /api/sesion — recibe email y contraseña, y si son correctas
     devuelve un token JWT. El paquete ya está en el .csproj:
     Microsoft.AspNetCore.Authentication.JwtBearer.

  2. La configuración del token en Program.cs: firma, emisor, vigencia.
     El secreto se lee de una VARIABLE DE ENTORNO, nunca escrito en el
     código.

  3. [Authorize] en TODOS los controladores que ya existen, para que sin
     token respondan 401.

LO QUE NO CONSTRUYO, y si lo haces lo voy a borrar:
  · El hash de la contraseña (BCrypt) — lo hace un compañero.
  · El atributo de permisos ni la llamada a verificar_acceso_ruta.
  · El menú del front.

DOS COSAS QUE SE HACEN MAL CASI SIEMPRE:

  1. SesionController necesita que su POST sea público. NO pongas
     [AllowAnonymous] en la CLASE: eso ANULA los [Authorize] de los
     métodos y los deja abiertos sin avisar (warning ASP0026).
     Hazlo al revés: [Authorize] en la clase, y [AllowAnonymous] SOLO
     en el método del POST. Cerrado por defecto, abierto por excepción.

  2. EN EL TOKEN VA EL CORREO, Y NADA MÁS. No metas los roles ni los
     permisos. Se consultan en cada petición contra la base de datos — si
     viajaran en el token, quitarle un permiso a alguien no surtiría
     efecto hasta que el token expire.

EL LOGIN NO DEBE DELATAR si el correo existe. "Correo que no existe" y
"contraseña incorrecta" responden EXACTAMENTE lo mismo: 401 con el mismo
mensaje. Si fueran distintos, cualquiera podría averiguar qué correos
están registrados probándolos uno por uno.

Para verificar la contraseña, llama al servicio de usuario que ya existe
—NO por HTTP, sino inyectando la interfaz—. Mi compañero va a poner el
hash ahí dentro; yo solo lo llamo.

REGLAS QUE SIGUEN VIGENTES:
  · Las TRES CAPAS con interfaces. El servicio NO nombra nada de HTTP.
  · TODO EN ESPAÑOL.
  · Cero secretos en el código: variable de entorno.

COMENTA TODO, en español, diciendo POR QUÉ está escrito así. En este
código la razón importa más que en ningún otro: un [Authorize] sin
comentario no dice qué protege ni de quién.

ORDEN: primero la configuración del token, luego el POST de sesión, y de
último el [Authorize] en los controladores que ya existen. Al terminar
cada paso dime qué tocaste y espera.
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


---

## 6. Lo que hay que mirarle al agente

```powershell
# 1 · NINGUNA clase con [AllowAnonymous] a nivel de clase. Tiene que dar 0.
Select-String -Path api_facturas\Controllers\*.cs -Pattern '^\[AllowAnonymous\]' |
  Measure-Object | Select-Object Count

# 2 · El secreto NO está en el código. Tiene que dar 0.
Select-String -Path api_facturas\*.cs,api_facturas\appsettings.json `
  -Pattern 'SecretKey|clave_secreta|"Key"\s*:\s*"[A-Za-z0-9]{16,}' |
  Measure-Object | Select-Object Count

# 3 · Y el warning ASP0026 NO debe aparecer al compilar.
docker compose logs api-facturas | Select-String 'ASP0026'
```

> **El 1 es el que importa.** Si da algo distinto de 0, hay endpoints abiertos
> que nadie va a descubrir hasta que alguien entre sin token.

---

## 7. Comprobar antes del PR

```powershell
# 1 · Sin token: 401 en todo.
Invoke-RestMethod http://localhost:8035/api/producto
#    espera: 401

# 2 · Identificarse.
$r = Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
  -ContentType 'application/json' `
  -Body '{"email":"admin@correo.com","contrasena":"admin123"}'
$h = @{ Authorization = "Bearer $($r.token)" }

# 3 · Con token: pasa.
Invoke-RestMethod http://localhost:8035/api/producto -Headers $h

# 4 · EL LOGIN NO DELATA. Las dos respuestas deben ser IDÉNTICAS.
Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
  -ContentType 'application/json' -Body '{"email":"noexiste@x.com","contrasena":"x"}'
Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
  -ContentType 'application/json' -Body '{"email":"admin@correo.com","contrasena":"malisima"}'

# 5 · Y el token NO lleva permisos: péguelo en jwt.io y mire el payload.
#     Debe tener el correo. Si tiene una lista de roles, está mal.
```

> **El paso 4 es la comprobación que más se salta**, y es de las pocas de este
> curso donde **dos respuestas iguales son el resultado correcto**.

---

## 8. Subir, y después integrar

```powershell
git status ; git add api_facturas/Controllers/SesionController.cs
git commit -m "feat: el endpoint de sesion, con el POST como unica excepcion publica"
git push -u origin rama-carlos-v3
#   y el Pull Request. Avísele a Paco: sin su token él no puede empezar.
```

**Al integrar, la regresión de esta versión es especial:**

> **Todo lo que antes funcionaba ahora exige token.** Vuelva a correr las 68
> operaciones de v1 y v2 **con la cabecera puesta** — y pruebe al menos una
> **sin** ella, para confirmar que responde 401. Si alguna sigue respondiendo sin
> token, ahí quedó un agujero.

```powershell
git tag -a v3 -m "Version 3: el control de acceso"
git push origin v3
```
