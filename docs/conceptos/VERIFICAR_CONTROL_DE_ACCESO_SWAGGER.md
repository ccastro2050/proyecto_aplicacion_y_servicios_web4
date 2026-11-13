# Verificar el control de acceso con Swagger

> Aquí **no entra el front**. Swagger hace las peticiones y se sigue, método por
> método, lo que hace la **API** con cada una.
>
> | Usuario | Contraseña | `GET /api/usuario` debe dar |
> |---|---|---|
> | `admin@correo.com` | `admin123` | `200` |
> | `vendedor1@correo.com` | `vendedor123` | `403` |
> | (sin token) | | `401` |

---

# Parte 1. Cómo se genera el token

## Paso 1. Swagger: `POST /api/sesion`

Abrir **http://localhost:8035/swagger**, sección **Sesion** → `POST /api/sesion`
→ **Try it out**, y en el cuerpo:

```json
{
  "email": "admin@correo.com",
  "contrasena": "admin123"
}
```

![POST /api/sesion con el administrador](img_swagger_acceso/01_post_sesion_admin.png)

**Qué muestra esta pantalla.** La operación `POST /api/sesion` abierta en Swagger, con el cuerpo de la petición escrito: el correo y la contraseña del administrador. Todavía no se ha enviado; se envía con **Execute**.

Al oprimir **Execute**, la petición llega a la API.

**→ Sigue en:** `api_facturas/Controllers/SesionController.cs`, método `Iniciar`.

## Paso 2. `SesionController.cs` → método `Iniciar`

Archivo: [`api_facturas/Controllers/SesionController.cs`](../../api_facturas/Controllers/SesionController.cs), línea 98

![SesionController.Iniciar](img_control_acceso/04_sesion_controller_iniciar.png)

**Qué hace este código.** El algoritmo de `Iniciar`: (1) recibe el correo y la contraseña en `body`, sin pedir token (`[AllowAnonymous]`), porque quien inicia sesión todavía no tiene uno; (2) le pasa los dos datos al servicio, sin revisar nada él mismo; (3) si el servicio devuelve `null`, responde `401` con un mensaje que no dice si falló el correo o la contraseña; (4) si devuelve una sesión, responde `200` con el token, el correo, los roles y la fecha de vencimiento.

Recibe el JSON de Swagger en `body`. `[AllowAnonymous]`: no pide token, porque
todavía no hay uno.

La línea que sigue el flujo es la **102**:

```csharp
var sesion = await _servicio.IniciarAsync(body.Email!, body.Contrasena!);
```

**→ Sigue en:** `api_facturas/Servicios/ServicioSesion.cs`, método `IniciarAsync`.

## Paso 3. `ServicioSesion.cs` → método `IniciarAsync`

Archivo: [`api_facturas/Servicios/ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs), línea 48

![ServicioSesion.IniciarAsync](img_control_acceso/05_servicio_sesion_api_iniciar.png)

**Qué hace este código.** El algoritmo de `IniciarAsync` de la API: (1) si falta el correo o la contraseña, devuelve `null`; (2) le pide al repositorio que compare la contraseña; (3) si no coincide o el correo no existe, devuelve `null` (los dos casos iguales, para no revelar cuál fue); (4) busca los nombres de los roles; (5) calcula el vencimiento (los minutos configurados, o 60); (6) arma el token; (7) devuelve token, correo, roles y vencimiento en un objeto `Sesion`.

Hace tres llamados, en este orden:

| Línea | Llamado | Para qué |
|---|---|---|
| **59** | `_usuarios.VerificarContrasenaAsync(...)` | ¿La contraseña es correcta? |
| **68** | `NombresDeRolesAsync(...)` | ¿Qué roles tiene? |
| **74** | `ArmarToken(...)` | **Crear el token** |

**→ Sigue en:** `api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs`, método `VerificarContrasenaAsync`.

## Paso 4. `RepositorioUsuarioSqlServer.cs` → método `VerificarContrasenaAsync`

Archivo: [`api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs`](../../api_facturas/Repositorios/RepositorioUsuarioSqlServer.cs), línea 72

![RepositorioUsuarioSqlServer.VerificarContrasenaAsync](img_control_acceso/06_repositorio_verificar_contrasena.png)

**Qué hace este código.** El algoritmo de `VerificarContrasenaAsync`: (1) consulta en la tabla `usuario` el hash guardado para ese correo; (2) si no hay fila, el correo no existe y devuelve `null`; (3) si la hay, `BC.Verify` calcula el hash de la contraseña escrita con el mismo salt y costo del hash guardado, y compara: iguales → `true`, distintos → `false`; (4) si lo guardado no es un hash de bcrypt, devuelve `false`.

1. `SELECT contrasena FROM usuario WHERE email = @email` → el **hash** guardado.
2. Si no hay → `null` (el correo no existe).
3. `BC.Verify("admin123", hash)` → bcrypt saca el salt del hash guardado, calcula
   el hash de `admin123` con ese salt y compara → `true`.

**→ Vuelve a:** `IniciarAsync`, línea 68 → `NombresDeRolesAsync`.

## Paso 5. `ServicioSesion.cs` → método `NombresDeRolesAsync`

Archivo: [`api_facturas/Servicios/ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs), línea 111

![NombresDeRolesAsync](img_control_acceso/08_metodo_nombres_de_roles.png)

**Qué hace este código.** El algoritmo de `NombresDeRolesAsync`: (1) trae los roles asignados a ese correo, que vienen solo como números (`fkidrol`); (2) trae todos los roles con su número y su nombre; (3) a cada número asignado le busca su nombre; (4) descarta los que no encontró; (5) devuelve la lista de nombres.

| Llamado | Tabla | Para el administrador |
|---|---|---|
| `_rolesDeUsuario.ObtenerPorUsuarioAsync(email)` | `rol_usuario` | id `1` |
| `_roles.ObtenerTodosAsync(1000)` | `rol` | `1` = Administrador |

Resultado: `["Administrador"]`.

**→ Vuelve a:** `IniciarAsync`, línea 74 → `ArmarToken`.

## Paso 6. `ServicioSesion.cs` → método `ArmarToken` — aquí se crea el token

Archivo: [`api_facturas/Servicios/ServicioSesion.cs`](../../api_facturas/Servicios/ServicioSesion.cs), línea 122

![ArmarToken](img_control_acceso/11_armar_token.png)

**Qué hace este código.** El algoritmo de `ArmarToken`: (1) arma los *claims*, es decir lo que va dentro del token: el correo dos veces (`sub` y `name`) y un `role` por cada rol; (2) crea la llave a partir de la clave secreta de la API; (3) con esa llave prepara la firma `HmacSha256`; (4) construye el token con emisor, audiencia, claims, vencimiento y firma; (5) lo convierte en el texto `eyJ...` y lo devuelve.

| Parte | Código | Para el administrador |
|---|---|---|
| **Contenido** (claims) | `Sub`, `Name`, `Role` | `admin@correo.com`, `Administrador` |
| **Firma** | `_jwt.Key` + `HmacSha256` | con la clave secreta de la API |
| **Vencimiento** | `expires: expira` | una hora después |
| **Texto final** | `WriteToken(token)` | `eyJhbGciOiJIUzI1NiIs...` |

**→ Vuelve a:** `IniciarAsync` → arma la `Sesion` → `SesionController.Iniciar` → `return Ok(...)`.

## Paso 7. Swagger: la respuesta `200` con el token

![Respuesta 200 con el token del administrador](img_swagger_acceso/02_respuesta_token_admin.png)

**Qué muestra esta pantalla.** La respuesta de la API después de **Execute**. Arriba están el `curl` equivalente y la URL que se llamó; abajo, el código `200` y el cuerpo con «Sesion iniciada.», el token, el correo, los roles y la fecha de vencimiento.

| Campo | Valor | Viene de |
|---|---|---|
| Code | `200` | `SesionController.Iniciar`, `return Ok(...)` |
| `token` | `eyJhbGciOiJIUzI1NiIs...` | `ArmarToken` (paso 6) |
| `roles` | `["Administrador"]` | `NombresDeRolesAsync` (paso 5) |
| `expira` | una hora después | `IniciarAsync` (paso 3) |

Copiar el valor de `token`, **sin las comillas**: se usa en el paso siguiente.

---

# Parte 2. Usar el token

## Paso 8. Swagger: darle el token con **Authorize**

1. Seleccionar y copiar el token, **sin las comillas**:

   ![Copiar el token](img_swagger_acceso/03_copiar_token.png)

   **Qué muestra esta pantalla.** En el cuerpo de la respuesta se selecciona el valor del token, solo el texto que está entre comillas, para copiarlo.

2. Subir al comienzo de la página → botón **Authorize** (candado) → pegarlo en
   **Value**. Solo el token: la palabra `Bearer` la pone Swagger.

   ![Pegar el token en Authorize](img_swagger_acceso/04_pegar_token_authorize.png)

   **Qué muestra esta pantalla.** La ventana que abre el botón **Authorize**. En **Value** se pega el token copiado. La misma ventana recuerda que no hay que escribir la palabra `Bearer`.

3. **Authorize** → queda **Authorized**. **Close**.

   ![Authorized](img_swagger_acceso/05_authorized.png)

   **Qué muestra esta pantalla.** Después de oprimir **Authorize**, la ventana dice **Authorized** y oculta el token con asteriscos. Los candados de las operaciones quedan cerrados: desde ahora Swagger envía el token en cada petición.

Desde ahora, cada petición que haga Swagger lleva la cabecera:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## Paso 9. Swagger: `GET /api/usuario` como administrador

Sección **Usuario** → `GET /api/usuario` → **Try it out** → **Execute**.

En **Curl** se ve que Swagger ya manda el token:
`-H 'Authorization: Bearer eyJhbGciOi...'`

**→ Sigue en:** `api_facturas/Controllers/UsuarioController.cs`.

## Paso 10. `UsuarioController.cs` → los atributos de la clase

Archivo: [`api_facturas/Controllers/UsuarioController.cs`](../../api_facturas/Controllers/UsuarioController.cs)

![UsuarioController](img_control_acceso/15_usuario_controller.png)

**Qué hace este código.** Lo que hace este controlador: sobre la clase están `[Authorize]`, que exige un token válido, y `[ExigePermiso("interfaz.usuarios")]`, que exige que el rol tenga esa ruta. Los dos corren antes que cualquier método. `Listar` solo pide la lista al servicio y la devuelve (`200`, o `204` si está vacía); no tiene ninguna línea de control de acceso.

Antes de entrar a `Listar` (línea 45) corren los dos atributos:

| Línea | Atributo | Administrador |
|---|---|---|
| **33** | `[Authorize]`: ¿token válido? | Sí: firma correcta y no ha vencido. Saca `admin@correo.com` del token. |
| **34** | `[ExigePermiso("interfaz.usuarios")]` | Se revisa en el paso 11. |

**→ Sigue en:** `api_facturas/Autorizacion/ExigePermisoAttribute.cs`, método `OnAuthorizationAsync`.

## Paso 11. `ExigePermisoAttribute.cs` → método `OnAuthorizationAsync`

Archivo: [`api_facturas/Autorizacion/ExigePermisoAttribute.cs`](../../api_facturas/Autorizacion/ExigePermisoAttribute.cs), línea 46

![ExigePermisoAttribute.OnAuthorizationAsync](img_control_acceso/16_exige_permiso.png)

**Qué hace este código.** El algoritmo de `OnAuthorizationAsync`: (1) toma el correo del token; (2) si no hay correo, responde `401` y corta; (3) consigue el repositorio de acceso; (4) le pregunta si ese correo tiene la ruta del atributo; (5) si la respuesta es no, responde `403` con la ruta que faltó y corta, así que el método del controlador no corre; (6) si es sí, no hace nada y la petición sigue.

Línea **67**: `TieneAccesoAsync("admin@correo.com", "interfaz.usuarios")`.
Si responde `false` → `403`. Si responde `true` → deja pasar a `Listar`.

**→ Sigue en:** `api_facturas/Repositorios/RepositorioAccesoSqlServer.cs`, método `TieneAccesoAsync`.

## Paso 12. `RepositorioAccesoSqlServer.cs` → método `TieneAccesoAsync`

Archivo: [`api_facturas/Repositorios/RepositorioAccesoSqlServer.cs`](../../api_facturas/Repositorios/RepositorioAccesoSqlServer.cs), línea 35

![RepositorioAccesoSqlServer.TieneAccesoAsync](img_control_acceso/17_tiene_acceso.png)

**Qué hace este código.** El algoritmo de `TieneAccesoAsync`: (1) busca el número de la ruta por su nombre; (2) si la ruta no existe, devuelve `false`, y nadie entra; (3) ejecuta el procedimiento `verificar_acceso_ruta` con el correo y ese número; (4) lee el JSON que el procedimiento deja en `p_resultado`; (5) si `tiene_acceso` vale `1` o `true`, devuelve `true`; cualquier otra cosa, `false`.

1. `"interfaz.usuarios"` → id `2`.
2. Ejecuta el procedimiento `verificar_acceso_ruta("admin@correo.com", 2)`.
3. Lee `tiene_acceso` del JSON que devuelve.

**→ Sigue en:** `db/bdfacturas.sql`, procedimiento `verificar_acceso_ruta`.

## Paso 13. `bdfacturas.sql` → procedimiento `verificar_acceso_ruta`

Archivo: [`db/bdfacturas.sql`](../../db/bdfacturas.sql), línea 1790

![verificar_acceso_ruta](img_control_acceso/18_verificar_acceso_ruta.png)

**Qué hace este código.** El algoritmo de `verificar_acceso_ruta`: (1) empieza con `@v_tiene_acceso = 0`, es decir sin acceso; (2) busca si existe al menos una fila que una ese correo, por `rol_usuario`, con un rol que en `rutarol` tenga esa ruta; (3) si existe, cambia a `1`; (4) devuelve `{"tiene_acceso": 0 o 1, "email": ..., "fkidruta": ...}` en `@p_resultado`.

¿Algún rol de `admin@correo.com` tiene la ruta `2`?
Su rol es `1` (Administrador), y `rutarol` tiene `(2, 1)` → **sí** → `tiene_acceso: 1`.

**→ Vuelve:** `TieneAccesoAsync` → `true` → `ExigePermiso` deja pasar →
`UsuarioController.Listar` corre → `200`.

## Paso 14. Swagger: la respuesta `200`

![GET /api/usuario como administrador: 200](img_swagger_acceso/06_get_usuario_admin_200.png)

**Qué muestra esta pantalla.** `GET /api/usuario` ejecutado con el token del administrador. En el `curl` se ve la cabecera `Authorization: Bearer` con el token. La respuesta es `200` y trae la lista de usuarios.

`200` con la lista de usuarios: el administrador **sí** puede.

---

# Parte 3. El vendedor: mismo pedido, otra respuesta

## Paso 15. Swagger: `POST /api/sesion` como vendedor

```json
{ "email": "vendedor1@correo.com", "contrasena": "vendedor123" }
```

![POST /api/sesion con el vendedor](img_swagger_acceso/07_post_sesion_vendedor.png)

**Qué muestra esta pantalla.** La misma operación `POST /api/sesion`, ahora con el correo y la contraseña del vendedor. La respuesta es `200`, con un token distinto y los roles `Vendedor` y `Cajero`.

`200`, otro token, y `roles: ["Vendedor", "Cajero"]`. En la API pasa lo mismo
que en los pasos 2 a 6; lo único distinto es lo que sale de la base:

| Paso | Administrador | Vendedor |
|---|---|---|
| 4. `VerificarContrasenaAsync` | `true` | `true` |
| 5. `NombresDeRolesAsync` | `["Administrador"]` | `["Vendedor", "Cajero"]` |
| 6. `ArmarToken` | token con `role = Administrador` | token con `role = Vendedor`, `role = Cajero` |

> En **Curl** se ve que Swagger mandó también el token del administrador: todavía
> estaba en **Authorize**. No importa: `POST /api/sesion` tiene `[AllowAnonymous]`
> y no lo mira.

## Paso 16. Swagger: cambiar el token en **Authorize**

**Authorize** → **Logout** → pegar el token **del vendedor** → **Authorize** → **Close**.

![Authorize con el token del vendedor](img_swagger_acceso/08_authorize_vendedor.png)

**Qué muestra esta pantalla.** La ventana de **Authorize** con el token del vendedor pegado, en lugar del token del administrador.

## Paso 17. Swagger: `GET /api/usuario` como vendedor → `403`

![GET /api/usuario como vendedor: 403](img_swagger_acceso/09_get_usuario_vendedor_403.png)

**Qué muestra esta pantalla.** La misma operación `GET /api/usuario`, ahora con el token del vendedor. En el `curl` se ve que el token sí va, pero la respuesta es `403` con «Su rol no tiene permiso para esta operacion.» y la ruta que le faltó: `interfaz.usuarios`.

```json
{
  "estado": 403,
  "mensaje": "Su rol no tiene permiso para esta operacion.",
  "ruta": "interfaz.usuarios"
}
```

En la API recorre **los mismos** pasos 10 a 13. Esta es la diferencia:

| Paso | Administrador | Vendedor |
|---|---|---|
| 10. `[Authorize]` | token válido ✔ | token válido ✔ |
| 11. `ExigePermiso` → `TieneAccesoAsync(email, "interfaz.usuarios")` | | |
| 12. `"interfaz.usuarios"` → id | `2` | `2` |
| 13. `verificar_acceso_ruta` | rol `1` tiene la ruta `2` → **`1`** | roles `2` y `3` no la tienen → **`0`** |
| ← `TieneAccesoAsync` | `true` | `false` |
| ← `ExigePermiso` | deja pasar | **`403`**, línea 70 |
| `UsuarioController.Listar` | corre → `200` | **no corre** |

El mensaje `"Su rol no tiene permiso para esta operacion."` y `"ruta": "interfaz.usuarios"`
salen tal cual de `ExigePermisoAttribute.cs`, líneas 72–74.

---

# Parte 4. Sin token

## Paso 18. Swagger: `GET /api/usuario` sin token → `401`

**Authorize** → **Logout** → **Close** → `GET /api/usuario` → **Execute**.

![GET /api/usuario sin token: 401](img_swagger_acceso/10_get_usuario_sin_token_401.png)

**Qué muestra esta pantalla.** La misma operación `GET /api/usuario` después de **Logout**. El `curl` ya no lleva la cabecera `Authorization`, y la respuesta es `401` con «No hay una sesion valida. Inicie sesion.»

En **Curl** ya **no** aparece la línea `Authorization: Bearer ...`.

```json
{ "estado": 401, "mensaje": "No hay una sesion valida. Inicie sesion." }
```

Aquí la petición **no llega** ni a `ExigePermiso` ni a la base: se detiene en
`[Authorize]` (paso 10). El mensaje lo escribe
[`Program.cs`](../../api_facturas/Program.cs), línea 152 (`OnChallenge`).

---

## Resumen

| Prueba | Token | Se detiene en | Respuesta |
|---|---|---|---|
| Administrador | válido, `role = Administrador` | — llega a `Listar` | **`200`** |
| Vendedor | válido, `role = Vendedor, Cajero` | `ExigePermiso` (la base dice `0`) | **`403`** |
| Sin token | — | `[Authorize]` | **`401`** |

- **`401`**: «no sé quién es usted».
- **`403`**: «sé quién es, y no puede».
