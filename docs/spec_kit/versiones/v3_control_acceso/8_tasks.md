# Tareas — Versión 3: el control de acceso, por fases

> | | |
> |---|---|
> | **Qué** | [2_spec.md](2_spec.md) |
> | **Cómo** | [3_plan.md](3_plan.md) · [6_contracts.md](6_contracts.md) |
> | **La verificación** | [7_quickstart.md](7_quickstart.md) |

**El orden de esta versión no se puede cambiar**, y es lo primero que hay que
entender:

```
   1. la CONTRASEÑA     sin esto, lo demás es decoración
         ↓
   2. la SESIÓN         sin esto, no hay a quién preguntarle nada
         ↓
   3. el PERMISO        sin esto, el sistema sabe quién entra y le deja todo
         ↓
   4. la INTERFAZ       sin esto, nadie puede usarlo
```

> **Y cada fase es un COMMIT.**

---

## Fase 0 — El punto de partida, verificado

| | |
|---|---|
| **Qué** | Que la v2 esté **funcionando**: los 12 recursos y sus 12 interfaces |
| **Verificación** | `docker compose up -d --build` y la prueba de humo de la v2 pasa completa |

## Fase 1 — La semilla: las contraseñas con hash

| | |
|---|---|
| **Archivos** | `db/bdfacturas.sql` |
| **Lo que importa** | Las **ocho** filas con hash de bcrypt costo 12, y **las contraseñas en claro escritas en el quickstart** — del hash no se vuelve a la clave, y sin saberlas no hay forma de probar nada |
| **Verificación** | `docker compose down -v` · `up -d` · `SELECT email, contrasena FROM usuario;` → las ocho empiezan por `$2a$12$` |

> **Va EN EL SCRIPT, no a mano.** El siguiente `down -v` vuelve a sembrar.

> **El hash ya funcionaba desde antes** —`BCrypt.Net-Next` estaba y el
> repositorio lo usaba—. Lo que faltaba era que la semilla lo usara también.

## Fase 2 — La configuración del token

| | |
|---|---|
| **Archivos** | `ApiFacturas.csproj` (+`JwtBearer 9.0.10`) · `Modelos/ConfiguracionJwt.cs` · `appsettings.json` · `docker-compose.yml` |
| **Lo que importa** | La clave con **32 caracteres o más**, y que el arranque **falle** si no la hay. Fallar al arrancar es mejor que fallar al firmar el primer token |
| **Verificación** | `docker compose up` arranca. Borre la clave del compose a propósito: **tiene que morir con el mensaje**, no con un 500 |

> **La versión del paquete NO se adivina:** la 9.0.10 es la que ya corre sobre
> `net10.0` en `proyecto_construccion1/api_generica_csharp`.

## Fase 3 — Iniciar sesión

| | |
|---|---|
| **Archivos** | `Modelos/Sesion.cs` · `Peticiones/SesionCrear.cs` · `Servicios/IServicioSesion.cs` · `ServicioSesion.cs` · `Controllers/SesionController.cs` |
| **Lo que importa** | Las credenciales **en el cuerpo**, no en la URL. Y **el mismo error** para el correo inexistente y la contraseña equivocada |
| **Verificación** | Las correctas → **200** con token · las dos formas de fallar → **401 con el MISMO mensaje** |

## Fase 4 — El token exigido: el 401

| | |
|---|---|
| **Archivos** | `Program.cs` — `AddAuthentication` + `AddJwtBearer`, y `UseAuthentication` **antes** de `UseAuthorization`. `[Authorize]` en los 12 controladores |
| **Lo que importa** | `ClockSkew = TimeSpan.Zero` · el `OnChallenge` que le pone cuerpo al 401 · el `/` y el `POST /api/sesion` **abiertos** |
| **Verificación** | Sin token **401** · con token **200** · token con **una letra cambiada** → **401** · el `/` → **200** |

> **Si todo responde 200 sin token, `UseAuthentication` no está o está después
> de `UseAuthorization`.** Compila, arranca y deja pasar todo.

## Fase 5 — El permiso: el 403

| | |
|---|---|
| **Archivos** | `Repositorios/IRepositorioAcceso.cs` · `RepositorioAccesoSqlServer.cs` · `Autorizacion/ExigePermisoAttribute.cs` · `[ExigePermiso("interfaz.x")]` en los 12 controladores |
| **Lo que importa** | **`verificar_acceso_ruta`**, que ya existía: el `JOIN` de permisos **no se escribe en C#**. Y una ruta que no esté en la tabla tiene que **fallar cerrado** |
| **Verificación** | La matriz de los tres roles: `admin` todo 200 · `vendedor1` **403** en usuarios y **200** en clientes · `cliente1` al revés |

> **Y el que decide si la versión está bien hecha:** quítele el permiso a un rol
> en la base y pida otra vez **con el mismo token**. Tiene que responder
> **403** sin que nadie vuelva a identificarse. Si responde 200, los permisos
> están en el token.

## Fase 6 — Los permisos propios, para el menú

| | |
|---|---|
| **Archivos** | `Controllers/PermisosController.cs` |
| **Lo que importa** | El correo sale **del token**, no de la URL. Exige token pero **no** permiso |
| **Verificación** | `admin` → 15 rutas · `cliente1` → `["interfaz.inicio","interfaz.productos"]` |

> **Hasta aquí la API está completa. Y la versión NO está cerrada.**

## Fase 7 — La interfaz de identificación

| | |
|---|---|
| **Archivos** | `Servicios/EstadoSesion.cs` · `Servicios/ServicioSesion.cs` · `Modelos/RespuestaSesion.cs` · `Components/Pages/Sesion.razor` · `Components/Layout/SesionActual.razor` · `Program.cs` del front |
| **Lo que importa** | `EstadoSesion` **`scoped`**, no `singleton`: uno por circuito. Con `singleton` habría **un token para todos** |
| **Verificación** | Entrar como `admin` y ver su correo y sus roles arriba. Salir y que desaparezcan |

## Fase 8 — El token en cada petición

| | |
|---|---|
| **Archivos** | Los **doce** servicios del front suman `Autorizar()` |
| **Lo que importa** | Se llama en **todos** los métodos. Un método al que se le olvide responde 401 y el que lo lea va a creer que la sesión venció |
| **Verificación** | Identificarse como `admin` y abrir las 12 interfaces: **todas traen datos** |

> **No se usa un `DelegatingHandler`**, y la razón está en
> [3_plan.md](3_plan.md) §4.2: puede recibir el scope equivocado, y entonces
> manda **el token de otra sesión**. Silenciosamente.

## Fase 9 — El menú por rol, y el modo de renderizado

| | |
|---|---|
| **Archivos** | `Components/Layout/NavMenu.razor` · `Components/App.razor` |
| **Lo que importa** | El `@rendermode` va en **`App.razor`**, no en cada interfaz: si no, el layout se queda estático y **el menú nunca ve la sesión**. Y el dibujo previo se **apaga** |
| **Verificación** | Entrar como `vendedor1`: el menú muestra Facturas y Clientes, y **no** Usuarios ni Productos. Como `cliente1`: al revés |

## Fase 10 — El cierre

| | |
|---|---|
| **1** | La **regresión**: la prueba de humo de la v1 **y** la de la v2, **con token** |
| **2** | Los **diez** criterios de [2_spec.md](2_spec.md) §4 |
| **3** | **El criterio 9, en el navegador:** como `vendedor1`, **escribir `/usuarios` a mano**. Tiene que mostrar el aviso de permiso y cero filas |
| **4** | Commit y **tag `v3`** |

> **El criterio 9 va al final y se hace de verdad**, abriendo el navegador y
> escribiendo la dirección. Es el único que no se puede aparentar con una
> interfaz bien dibujada.
