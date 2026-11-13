# El flujo del control de acceso, en el código

> La teoría está en [CONCEPTOS_CONTROL_DE_ACCESO.md](CONCEPTOS_CONTROL_DE_ACCESO.md).
> Aquí se sigue el código **método por método**, con dos usuarios de
> [DATOS_DE_PRUEBA.md](../dominio/DATOS_DE_PRUEBA.md):
>
> | Usuario | Contraseña | Rol |
> |---|---|---|
> | `admin@correo.com` | `admin123` | Administrador |
> | `vendedor1@correo.com` | `vendedor123` | Vendedor, Cajero |

---

## Paso 1. `Sesion.razor` → método `Entrar`

Archivo: [`front_blazor/Components/Pages/Sesion.razor`](../../front_blazor/Components/Pages/Sesion.razor)

El usuario digita correo y contraseña y oprime el botón (línea 83):

![Formulario de inicio de sesión](img_control_acceso/01_formulario_sesion.png)

**Qué hace este código.** Es el formulario de inicio de sesión. Lo que el usuario escribe en «Correo» queda en la variable `email` y lo que escribe en «Contraseña» queda en `contrasena`. Al oprimir **Entrar**, Blazor ejecuta el método `Entrar`.

El botón llama al método `Entrar` (línea 157):

![Método Entrar](img_control_acceso/02_metodo_entrar.png)

**Qué hace este código.** El algoritmo de `Entrar`: (1) lee las dos cajas; (2) si alguna está vacía, muestra un aviso y termina; (3) le pide al servicio que inicie sesión con ese correo y esa contraseña; (4) si la API dice que no, borra la contraseña, muestra el error y termina; (5) si dice que sí, pide las rutas permitidas con el token recibido, guarda token, correo, roles y rutas, y lleva al usuario a la página de inicio.

La línea que sigue el flujo es la **178**:

```csharp
var r = await Servicio.IniciarAsync(email.Trim(), contrasena);
```

**→ Sigue en:** `front_blazor/Servicios/ServicioSesion.cs`, método `IniciarAsync`.

## Paso 2. `ServicioSesion.cs` (front) → método `IniciarAsync`

Archivo: [`front_blazor/Servicios/ServicioSesion.cs`](../../front_blazor/Servicios/ServicioSesion.cs)

![IniciarAsync del front](img_control_acceso/03_front_servicio_sesion_post.png)

**Qué hace este código.** El algoritmo de `IniciarAsync` del front: (1) envía el correo y la contraseña en un `POST` a `api/sesion`; (2) si la API responde `401`, devuelve «El correo o la contraseña no son correctos»; (3) si responde otro error, devuelve «No se pudo iniciar sesión»; (4) si responde `200`, lee el cuerpo y, si trae token, lo devuelve; (5) si la API no contesta, devuelve «No se pudo conectar con el servicio».

La línea que sigue el flujo es la **29**:

```csharp
var r = await cliente.PostAsJsonAsync("api/sesion", new { email, contrasena });
```

Manda un `POST` a `api/sesion` con el correo y la contraseña en JSON. Aquí
**termina el front**: la petición sale por la red hacia la API.

Lo demás del método es lo que hace con la respuesta: `401` → «correo o
contraseña no son correctos»; `200` → lee el token y se lo devuelve a `Entrar`.

**→ Sigue en:** `api_facturas/Controllers/SesionController.cs`, método `Iniciar`
(la clase con `[Route("api/sesion")]` y `[HttpPost]`).

## Paso 3. `SesionController.cs` (API) → método `Iniciar`

Archivo: [`api_facturas/Controllers/SesionController.cs`](../../api_facturas/Controllers/SesionController.cs)

![SesionController.Iniciar](img_control_acceso/04_sesion_controller_iniciar.png)

**Qué hace este código.** El algoritmo de `Iniciar`: (1) recibe el correo y la contraseña en `body`, sin pedir token (`[AllowAnonymous]`), porque quien inicia sesión todavía no tiene uno; (2) le pasa los dos datos al servicio, sin revisar nada él mismo; (3) si el servicio devuelve `null`, responde `401` con un mensaje que no dice si falló el correo o la contraseña; (4) si devuelve una sesión, responde `200` con el token, el correo, los roles y la fecha de vencimiento.

El `POST` a `api/sesion` llega aquí. `[AllowAnonymous]`: este método no pide
token, porque quien está entrando todavía no tiene uno.

La línea que sigue el flujo es la **102**:

```csharp
var sesion = await _servicio.IniciarAsync(body.Email!, body.Contrasena!);
```

Con lo que vuelva: si es `null` → `401`; si no → `200` con el **token** y los **roles**.

**→ Sigue en:** `api_facturas/Servicios/ServicioSesion.cs`, método `IniciarAsync`.

## Paso 4. `ServicioSesion.cs` (API) → método `IniciarAsync`

Archivo: [`api_facturas/Servicios/ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs)

![ServicioSesion.IniciarAsync de la API](img_control_acceso/05_servicio_sesion_api_iniciar.png)

**Qué hace este código.** El algoritmo de `IniciarAsync` de la API: (1) si falta el correo o la contraseña, devuelve `null`; (2) le pide al repositorio que compare la contraseña; (3) si no coincide o el correo no existe, devuelve `null` (los dos casos iguales, para no revelar cuál fue); (4) busca los nombres de los roles; (5) calcula el vencimiento (los minutos configurados, o 60); (6) arma el token; (7) devuelve token, correo, roles y vencimiento en un objeto `Sesion`.

Este método hace tres llamados, en este orden:

| Línea | Llamado | Para qué |
|---|---|---|
| **59** | `_usuarios.VerificarContrasenaAsync(...)` | ¿La contraseña es correcta? |
| **68** | `NombresDeRolesAsync(...)` | ¿Qué roles tiene? |
| **74** | `ArmarToken(...)` | Armar el token |

Si la contraseña no es correcta (línea 63), devuelve `null` y no sigue.

**→ Sigue en:** `api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs`, método `VerificarContrasenaAsync`.

## Paso 5. `RepositorioUsuarioSqlServer.cs` → método `VerificarContrasenaAsync`

Archivo: [`api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs`](../../api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs)

![RepositorioUsuarioSqlServer.VerificarContrasenaAsync](img_control_acceso/06_repositorio_verificar_contrasena.png)

**Qué hace este código.** El algoritmo de `VerificarContrasenaAsync`: (1) consulta en la tabla `usuario` el hash guardado para ese correo; (2) si no hay fila, el correo no existe y devuelve `null`; (3) si la hay, `BC.Verify` calcula el hash de la contraseña escrita con el mismo salt y costo del hash guardado, y compara: iguales → `true`, distintos → `false`; (4) si lo guardado no es un hash de bcrypt, devuelve `false`.

1. `SELECT contrasena FROM usuario WHERE email = @email`: trae de la base el **hash** guardado.
2. Si no trae nada, el correo no existe → `null`.
3. `BC.Verify(contrasena, hash)`: compara lo digitado con el hash → `true` o `false`.

### Qué hace bcrypt aquí

`BC` es **BCrypt**, declarado arriba en el mismo archivo (línea 14):

```csharp
using BC = BCrypt.Net.BCrypt;   // el paquete BCrypt.Net-Next
```

La base **no** guarda `admin123`. Guarda esto:

```
$2a$12$PJf6LIuW8uL9q9hK0LsG0ebvUll.eLcJgg6lmTIPVk84p0fwD0T5u
 │   │  └──────── salt ───────┘└────────── hash ───────────┘
 │   └─ costo (12)
 └─ algoritmo (bcrypt)
```

El hash **no se puede devolver** a `admin123`. Por eso `BC.Verify` no "descifra":
hace lo mismo otra vez y compara.

| | `BC.Verify("admin123", "$2a$12$PJf6...")` |
|---|---|
| **1** | Del hash guardado saca el **costo** (`12`) y el **salt** (`PJf6LIuW8uL9q9hK0LsG0e`). |
| **2** | Calcula el hash de lo que se digitó (`admin123`) con **ese mismo** costo y salt. |
| **3** | Compara el resultado con el hash guardado. Iguales → `true`. Distintos → `false`. |

Si alguien digita `vendedor1@correo.com` con la contraseña `admin123`, el
`SELECT` trae el hash **del vendedor**; `admin123` con ese salt no da ese hash
→ `false` → `401`.

Aquí el flujo llega a la base de datos y **se devuelve**.

**→ Vuelve a:** `ServicioSesion.cs` (API), método `IniciarAsync`, línea 68 → `NombresDeRolesAsync`.

## Paso 6. `ServicioSesion.cs` (API) → método `NombresDeRolesAsync`

Archivo: [`api_facturas/Servicios/ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs)

De vuelta en `IniciarAsync`: si la contraseña fue correcta, la línea **68** pide los roles:

![Llamado a NombresDeRolesAsync](img_control_acceso/07_llamado_nombres_de_roles.png)

**Qué hace este código.** De vuelta en `IniciarAsync`, ya comprobada la contraseña. La línea `var roles = await NombresDeRolesAsync(...)` pide los nombres de los roles; la siguiente calcula el vencimiento; y el `return new Sesion` arma el token con `ArmarToken`.

Y el método, en la línea **111**:

![Método NombresDeRolesAsync](img_control_acceso/08_metodo_nombres_de_roles.png)

**Qué hace este código.** El algoritmo de `NombresDeRolesAsync`: (1) trae los roles asignados a ese correo, que vienen solo como números (`fkidrol`); (2) trae todos los roles con su número y su nombre; (3) a cada número asignado le busca su nombre; (4) descarta los que no encontró; (5) devuelve la lista de nombres.

| Línea | Qué hace |
|---|---|
| `_rolesDeUsuario.ObtenerPorUsuarioAsync(email)` | Los **ids** de los roles de ese correo (tabla `rol_usuario`). |
| `_roles.ObtenerTodosAsync(1000)` | Todos los roles con su **nombre** (tabla `rol`). |
| `.Select(... r.Id == a.Fkidrol ...)` | Cambia cada id por su nombre. |

**Aquí los dos usuarios se separan:**

| Usuario | `rol_usuario` (ids) | `roles` (nombres) |
|---|---|---|
| `admin@correo.com` | `1` | `["Administrador"]` |
| `vendedor1@correo.com` | `2`, `3` | `["Vendedor", "Cajero"]` |

**→ Sigue en:** `api_facturas/Repositorios/RepositorioRolUsuarioSqlServer.cs`, método `ObtenerPorUsuarioAsync`.

## Paso 7. `RepositorioRolUsuarioSqlServer.cs` → método `ObtenerPorUsuarioAsync`

Archivo: [`api_facturas/Repositorios/RepositorioRolUsuarioSqlServer.cs`](../../api_facturas/Repositorios/RepositorioRolUsuarioSqlServer.cs)

![RepositorioRolUsuarioSqlServer.ObtenerPorUsuarioAsync](img_control_acceso/09_rol_usuario_por_usuario.png)

**Qué hace este código.** El algoritmo de `ObtenerPorUsuarioAsync`: consulta en la tabla `rol_usuario` todas las filas de ese correo y las devuelve. Cada fila es un rol que el usuario tiene, identificado por su número.

```sql
SELECT fkemail, fkidrol FROM rol_usuario WHERE fkemail = @fkemail
```

| Usuario | Devuelve |
|---|---|
| `admin@correo.com` | `(admin@correo.com, 1)` |
| `vendedor1@correo.com` | `(vendedor1@correo.com, 2)`, `(vendedor1@correo.com, 3)` |

**→ Sigue en:** `api_facturas/Repositorios/RepositorioRolSqlServer.cs`, método `ObtenerTodosAsync`.

## Paso 8. `RepositorioRolSqlServer.cs` → método `ObtenerTodosAsync`

Archivo: [`api_facturas/Repositorios/RepositorioRolSqlServer.cs`](../../api_facturas/Repositorios/RepositorioRolSqlServer.cs)

![RepositorioRolSqlServer.ObtenerTodosAsync](img_control_acceso/10_rol_obtener_todos.png)

**Qué hace este código.** El algoritmo de `ObtenerTodosAsync`: consulta la tabla `rol` completa, ordenada por número, y devuelve el número y el nombre de cada rol. Con esa lista se traducen a nombres los números del paso anterior.

```sql
SELECT TOP (@limite) id, nombre FROM rol ORDER BY id
```

Devuelve todos los roles: `1 Administrador`, `2 Vendedor`, `3 Cajero`,
`4 Contador`, `5 Cliente`. Con esta lista, `NombresDeRolesAsync` (paso 6)
cambia los ids del paso 7 por nombres.

**→ Vuelve a:** `ServicioSesion.cs` (API), método `IniciarAsync`, línea 74 → `ArmarToken`.

## Paso 9. `ServicioSesion.cs` (API) → método `ArmarToken`

Archivo: [`api_facturas/Servicios/ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs), línea 122

![ArmarToken](img_control_acceso/11_armar_token.png)

**Qué hace este código.** El algoritmo de `ArmarToken`: (1) arma los *claims*, es decir lo que va dentro del token: el correo dos veces (`sub` y `name`) y un `role` por cada rol; (2) crea la llave a partir de la clave secreta de la API; (3) con esa llave prepara la firma `HmacSha256`; (4) construye el token con emisor, audiencia, claims, vencimiento y firma; (5) lo convierte en el texto `eyJ...` y lo devuelve.

1. **Claims**: lo que va *dentro* del token → el correo y los roles.
2. **Firma**: con la clave secreta `_jwt.Key` y `HmacSha256`. Sin esa clave nadie puede fabricar ni alterar un token.
3. **Expira**: la fecha calculada en `IniciarAsync`.
4. `WriteToken`: lo convierte en el texto `eyJhbGciOi...` que se le entrega al front.

El token **se puede leer sin clave** (está firmado, no cifrado). Por eso no lleva nada privado.

| Usuario | Lo que va dentro del token |
|---|---|
| `admin@correo.com` | `sub = admin@correo.com`, `role = Administrador` |
| `vendedor1@correo.com` | `sub = vendedor1@correo.com`, `role = Vendedor`, `role = Cajero` |

Los **permisos** (qué pantallas puede abrir) **no** van en el token.

---

## El regreso: el token vuelve al front

Nada nuevo que abrir: el flujo deshace el camino.

| | Dónde | Qué pasa |
|---|---|---|
| ← | `ServicioSesion.cs` (API) `IniciarAsync` | devuelve la `Sesion` (token, correo, roles, expira) |
| ← | `SesionController.cs` `Iniciar` (paso 3) | responde `200` con esos datos |
| ← | `ServicioSesion.cs` (front) `IniciarAsync` (paso 2) | lee la respuesta |
| ← | `Sesion.razor` `Entrar` (paso 1) | recibe el token |

En `Entrar`, la línea **193** usa el token por primera vez:

```csharp
var rutas = await Servicio.MisRutasAsync(r.Dato!.Token);
```

**→ Sigue en:** `front_blazor/Servicios/ServicioSesion.cs`, método `MisRutasAsync`.

---

# Con el token: ¿a qué puede entrar?

## Paso 10. `ServicioSesion.cs` (front) → método `MisRutasAsync`

Archivo: [`front_blazor/Servicios/ServicioSesion.cs`](../../front_blazor/Servicios/ServicioSesion.cs), línea 61

![MisRutasAsync del front](img_control_acceso/12_mis_rutas_front.png)

**Qué hace este código.** El algoritmo de `MisRutasAsync`: (1) prepara un `GET` a `api/permisos/mios`; (2) le pone el token en la cabecera `Authorization: Bearer`; (3) lo envía; (4) si la respuesta no es correcta, devuelve una lista vacía; (5) si lo es, saca la lista de rutas del campo `datos` y la devuelve. No envía el correo: la API lo saca del token.

```csharp
var peticion = new HttpRequestMessage(HttpMethod.Get, "api/permisos/mios");
peticion.Headers.Authorization = new("Bearer", token);
```

Es la **primera petición con token**: `GET api/permisos/mios`, con el token en
la cabecera `Authorization: Bearer eyJ...`.

### Qué es `new("Bearer", token)`

`new(...)` aquí es un `AuthenticationHeaderValue` (C# deduce el tipo). Arma
esta cabecera de la petición:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
               └─┬──┘ └──────── token ────────┘
              esquema
```

**Bearer** significa **«portador»**: *quien porta este token, entra*. La API no
pide nada más, ni usuario ni contraseña; solo revisa que el token sea válido
(firma y vencimiento). Por eso el token se cuida como una contraseña: quien lo
tenga, entra como usted hasta que venza.

El correo **no** se manda: la API lo saca del token. Así nadie puede preguntar
por los permisos de otro.

**→ Sigue en:** `api_facturas/Controllers/PermisosController.cs`, método `Mios`
(la clase con `[Route("api/permisos")]` y `[HttpGet("mios")]`).

## Paso 11. `PermisosController.cs` → método `Mios`

Archivo: [`api_facturas/Controllers/PermisosController.cs`](../../api_facturas/Controllers/PermisosController.cs), línea 54

![PermisosController.Mios](img_control_acceso/13_permisos_mios.png)

**Qué hace este código.** El algoritmo de `Mios`: (1) antes de entrar, `[Authorize]` ya validó el token; (2) toma el correo del token (`User.Identity.Name`), no de la petición; (3) si no hay correo, responde `401`; (4) pide al repositorio las rutas permitidas de ese correo; (5) responde `200` con el correo, el total y la lista en `datos`.

La clase tiene `[Authorize]` (línea 35). Antes de entrar a `Mios`, ASP.NET
revisa el token de la cabecera `Bearer`:

- firma inválida o token vencido → `401`, y `Mios` **no corre**;
- token válido → saca el correo del token y lo deja en `User.Identity.Name`.

Por eso la primera línea del método es:

```csharp
var email = User.Identity?.Name;   // el correo, sacado del token — no de la petición
```

La línea que sigue el flujo es la **64**:

```csharp
var rutas = await _acceso.RutasPermitidasAsync(email);
```

**→ Sigue en:** `api_facturas/Repositorios/RepositorioAccesoSqlServer.cs`, método `RutasPermitidasAsync`.

## Paso 12. `RepositorioAccesoSqlServer.cs` → método `RutasPermitidasAsync`

Archivo: [`api_facturas/Repositorios/RepositorioAccesoSqlServer.cs`](../../api_facturas/Repositorios/RepositorioAccesoSqlServer.cs), línea 82

![RepositorioAccesoSqlServer.RutasPermitidasAsync](img_control_acceso/14_rutas_permitidas.png)

**Qué hace este código.** El algoritmo de `RutasPermitidasAsync`: una sola consulta que parte del correo en `usuario`, pasa por `rol_usuario` a sus roles, por `rutarol` a las rutas de esos roles, y por `ruta` a sus nombres. `DISTINCT` quita las repetidas. El resultado solo sirve para armar el menú.

Un `SELECT` que recorre las cuatro tablas del control de acceso:

```
usuario ──► rol_usuario ──► rutarol ──► ruta
 (correo)    (sus roles)    (rutas de    (nombre de
                             cada rol)    la ruta)
```

**El resultado, para cada usuario:**

| Usuario | Roles | Rutas |
|---|---|---|
| `admin@correo.com` | Administrador | **las 15**: `interfaz.inicio`, `interfaz.usuarios`, `interfaz.facturas`, `interfaz.clientes`, `interfaz.productos`, … |
| `vendedor1@correo.com` | Vendedor + Cajero | **3**: `interfaz.clientes`, `interfaz.facturas`, `interfaz.inicio` |

El vendedor tiene dos roles; `DISTINCT` evita que `interfaz.facturas` (que la
tienen los dos roles) salga repetida.

> Esta lista **solo sirve para armar el menú**. No protege nada: el control de
> verdad se hace en cada petición (eso viene después).

**→ Vuelve a:** `PermisosController.Mios` → responde `200` con la lista →
`MisRutasAsync` (front) → `Entrar`, que guarda token, roles y rutas y va al inicio.

---

# El control de verdad: cada petición

Ya con sesión, el usuario abre **Usuarios**. El front manda:

```http
GET /api/usuario
Authorization: Bearer eyJ...
```

Mismo pedido para los dos. La respuesta **no**:

| Usuario | Respuesta |
|---|---|
| `admin@correo.com` | `200` con la lista de usuarios |
| `vendedor1@correo.com` | `403` |

## Paso 13. `UsuarioController.cs` → método `Listar`

Archivo: [`api_facturas/Controllers/UsuarioController.cs`](../../api_facturas/Controllers/UsuarioController.cs)

![UsuarioController](img_control_acceso/15_usuario_controller.png)

**Qué hace este código.** Lo que hace este controlador: sobre la clase están `[Authorize]`, que exige un token válido, y `[ExigePermiso("interfaz.usuarios")]`, que exige que el rol tenga esa ruta. Los dos corren antes que cualquier método. `Listar` solo pide la lista al servicio y la devuelve (`200`, o `204` si está vacía); no tiene ninguna línea de control de acceso.

`Listar` (línea 45) no tiene **ni una línea** de control de acceso. El control
está en los dos atributos de la clase, que corren **antes** del método:

| Línea | Atributo | Pregunta | Si falla |
|---|---|---|---|
| **33** | `[Authorize]` | ¿El token es válido? | `401` |
| **34** | `[ExigePermiso("interfaz.usuarios")]` | ¿Su rol tiene esa ruta? | `403` |

Los dos usuarios pasan la línea 33 (los dos tienen token válido).
En la línea 34 se separan.

**→ Sigue en:** `api_facturas/Autorizacion/ExigePermisoAttribute.cs`, método `OnAuthorizationAsync`.

## Paso 14. `ExigePermisoAttribute.cs` → método `OnAuthorizationAsync`

Archivo: [`api_facturas/Autorizacion/ExigePermisoAttribute.cs`](../../api_facturas/Autorizacion/ExigePermisoAttribute.cs), línea 46

![ExigePermisoAttribute.OnAuthorizationAsync](img_control_acceso/16_exige_permiso.png)

**Qué hace este código.** El algoritmo de `OnAuthorizationAsync`: (1) toma el correo del token; (2) si no hay correo, responde `401` y corta; (3) consigue el repositorio de acceso; (4) le pregunta si ese correo tiene la ruta del atributo; (5) si la respuesta es no, responde `403` con la ruta que faltó y corta, así que el método del controlador no corre; (6) si es sí, no hace nada y la petición sigue.

`_ruta` es lo que se escribió en el atributo: `"interfaz.usuarios"`.

| Línea | Qué hace |
|---|---|
| **52** | Saca el correo del token (`User.Identity.Name`). |
| **53–61** | Sin correo → `401`. |
| **67** | `TieneAccesoAsync(email, "interfaz.usuarios")`: le pregunta a la base. |
| **68–77** | Si la base dice que no → `403` y `Listar` **no corre**. |
| **78** | Si dice que sí → no hace nada, y ASP.NET entra a `Listar`. |

| Usuario | `TieneAccesoAsync(..., "interfaz.usuarios")` | Resultado |
|---|---|---|
| `admin@correo.com` | `true` | entra a `Listar` → `200` |
| `vendedor1@correo.com` | `false` | `403` «Su rol no tiene permiso para esta operacion.» |

**→ Sigue en:** `api_facturas/Repositorios/RepositorioAccesoSqlServer.cs`, método `TieneAccesoAsync`.

## Paso 15. `RepositorioAccesoSqlServer.cs` → método `TieneAccesoAsync`

Archivo: [`api_facturas/Repositorios/RepositorioAccesoSqlServer.cs`](../../api_facturas/Repositorios/RepositorioAccesoSqlServer.cs), línea 35

![RepositorioAccesoSqlServer.TieneAccesoAsync](img_control_acceso/17_tiene_acceso.png)

**Qué hace este código.** El algoritmo de `TieneAccesoAsync`: (1) busca el número de la ruta por su nombre; (2) si la ruta no existe, devuelve `false`, y nadie entra; (3) ejecuta el procedimiento `verificar_acceso_ruta` con el correo y ese número; (4) lee el JSON que el procedimiento deja en `p_resultado`; (5) si `tiene_acceso` vale `1` o `true`, devuelve `true`; cualquier otra cosa, `false`.

| Línea | Qué hace |
|---|---|
| **41–46** | Cambia el nombre `"interfaz.usuarios"` por su id (`2`). Si la ruta no existe → `false`: **nadie** entra. |
| **54–55** | Ejecuta el procedimiento almacenado **`verificar_acceso_ruta`** con el correo y el id de la ruta. |
| **57** | Lee lo que el procedimiento dejó en `p_resultado`: un JSON como `{"tiene_acceso":1, ...}`. |
| **73–79** | `tiene_acceso` = `1` → `true`; cualquier otra cosa → `false`. |

La API **no** decide: la decisión la toma la base de datos.

**→ Sigue en:** `db/bdfacturas.sql`, procedimiento `verificar_acceso_ruta`.

## Paso 16. `bdfacturas.sql` → procedimiento `verificar_acceso_ruta`

Archivo: [`db/bdfacturas.sql`](../../db/bdfacturas.sql), línea 1790

![Procedimiento verificar_acceso_ruta](img_control_acceso/18_verificar_acceso_ruta.png)

**Qué hace este código.** El algoritmo de `verificar_acceso_ruta`: (1) empieza con `@v_tiene_acceso = 0`, es decir sin acceso; (2) busca si existe al menos una fila que una ese correo, por `rol_usuario`, con un rol que en `rutarol` tenga esa ruta; (3) si existe, cambia a `1`; (4) devuelve `{"tiene_acceso": 0 o 1, "email": ..., "fkidruta": ...}` en `@p_resultado`.

1. Empieza en `0`: **sin acceso**, hasta que se demuestre lo contrario.
2. `IF EXISTS`: ¿hay **algún** rol de ese correo que tenga esa ruta?
   `usuario → rol_usuario → rutarol`.
3. Si existe → `1`.
4. Devuelve el JSON `{"tiene_acceso": 0 o 1, "email": ..., "fkidruta": ...}`.

**Aquí se decide.** Con la ruta `interfaz.usuarios` (id `2`):

| Usuario | Sus roles | ¿Algún rol tiene la ruta 2? | `tiene_acceso` |
|---|---|---|---|
| `admin@correo.com` | 1 Administrador | Sí: `rutarol (2, 1)` | **1** |
| `vendedor1@correo.com` | 2 Vendedor, 3 Cajero | No: Vendedor tiene 1, 3, 4; Cajero tiene 1, 3 | **0** |

---

## El regreso: la respuesta

| | Dónde | Administrador | Vendedor |
|---|---|---|---|
| ← | `verificar_acceso_ruta` (paso 16) | `tiene_acceso: 1` | `tiene_acceso: 0` |
| ← | `TieneAccesoAsync` (paso 15) | `true` | `false` |
| ← | `ExigePermiso` (paso 14) | deja pasar | `403` y se detiene |
| ← | `UsuarioController.Listar` (paso 13) | corre → `200` con la lista | **no corre** |

---

## Resumen: el flujo completo

| Paso | Archivo | Método |
|---|---|---|
| | **Iniciar sesión** | |
| 1 | `front_blazor/Components/Pages/Sesion.razor` | `Entrar` |
| 2 | `front_blazor/Servicios/ServicioSesion.cs` | `IniciarAsync` → `POST api/sesion` |
| 3 | `api_facturas/Controllers/SesionController.cs` | `Iniciar` |
| 4 | `api_facturas/Servicios/ServicioSesion.cs` | `IniciarAsync` |
| 5 | `api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs` | `VerificarContrasenaAsync` (bcrypt) |
| 6 | `api_facturas/Servicios/ServicioSesion.cs` | `NombresDeRolesAsync` |
| 7 | `api_facturas/Repositorios/RepositorioRolUsuarioSqlServer.cs` | `ObtenerPorUsuarioAsync` |
| 8 | `api_facturas/Repositorios/RepositorioRolSqlServer.cs` | `ObtenerTodosAsync` |
| 9 | `api_facturas/Servicios/ServicioSesion.cs` | `ArmarToken` |
| | **Armar el menú** | |
| 10 | `front_blazor/Servicios/ServicioSesion.cs` | `MisRutasAsync` → `GET api/permisos/mios` |
| 11 | `api_facturas/Controllers/PermisosController.cs` | `Mios` |
| 12 | `api_facturas/Repositorios/RepositorioAccesoSqlServer.cs` | `RutasPermitidasAsync` |
| | **El control, en cada petición** | |
| 13 | `api_facturas/Controllers/UsuarioController.cs` | `Listar` (`[Authorize]` + `[ExigePermiso]`) |
| 14 | `api_facturas/Autorizacion/ExigePermisoAttribute.cs` | `OnAuthorizationAsync` |
| 15 | `api_facturas/Repositorios/RepositorioAccesoSqlServer.cs` | `TieneAccesoAsync` |
| 16 | `db/bdfacturas.sql` | `verificar_acceso_ruta` |
