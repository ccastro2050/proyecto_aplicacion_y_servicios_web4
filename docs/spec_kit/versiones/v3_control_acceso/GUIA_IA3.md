# Cómo construir la VERSIÓN 3 con IA — por chat o con un IDE agéntico

> **La v3 es la única versión del curso en la que un error NO se ve.** Un
> sistema con el control de acceso mal hecho **funciona**: entra, muestra
> datos, no da errores. Lo que no hace es proteger.
>
> Por eso aquí la supervisión importa más que en las otras, y por eso el
> prompt lleva los cinco tropiezos escritos: para que la IA no los cometa, y
> para que usted sepa qué buscar si los comete.
>
> | | |
> |---|---|
> | **Qué construir** | [2_spec.md](2_spec.md) |
> | **El orden** | [8_tasks.md](8_tasks.md) — **once** fases |
> | **Los formatos** | [6_contracts.md](6_contracts.md) |
> | **Cómo se verifica** | [7_quickstart.md](7_quickstart.md) |
> | **Los conceptos, para USTED** | [CONCEPTOS_CONTROL_DE_ACCESO.md](../../../conceptos/CONCEPTOS_CONTROL_DE_ACCESO.md) |

---

## LO PRIMERO: la IA tiene que COMENTAR lo que escribe

Esto va antes que todo lo demás porque aplica a los dos caminos —el chat y el
IDE agéntico— y a cada archivo que la IA entregue.

**Exíjaselo en el prompt y recháceselo cuando no lo cumpla:**

> Comenta todo el código que generes, en español. Cada archivo empieza con un
> bloque que dice QUÉ ES y QUÉ PAPEL cumple en la arquitectura. Cada método no
> evidente lleva su comentario. Y los comentarios dicen **por qué** está escrito
> así, no **qué** hace la línea: `// suma uno al contador` no le sirve a nadie;
> `// el límite es 1000 porque es el ancho de la columna en la base` sí.

**Por qué esto no es un capricho de estilo:**

| | |
|---|---|
| **Usted va a tener que explicarlo** | Desde la v2, la **interpretabilidad se califica**: el profesor le va a pedir que cuente, en voz alta, qué hace el código que entregó y por qué está así. Un archivo sin comentarios es un archivo que usted va a tener que reconstruir de memoria enfrente de él |
| **El código generado se olvida más rápido que el escrito** | Lo que uno escribe a mano deja rastro en la cabeza. Lo que aceptó de un chat, no. El comentario es lo que queda cuando el recuerdo de la conversación se fue |
| **Es la prueba de que usted entendió, no de que la IA produjo** | Un comentario que explica el porqué solo lo puede revisar quien entendió la decisión. Si usted no puede juzgar si el comentario es cierto, no entendió el código |

> **Y una advertencia sobre los comentarios que la IA inventa.** A veces
> comenta lo que *cree* que hace el código, no lo que hace. Léalos: un
> comentario equivocado es peor que ninguno, porque el siguiente que lo lea le
> va a creer. Corregirlos es trabajo suyo, y de los buenos: es exactamente el
> tipo de corrección que la `GUIA_IA` le pide guardar.

---

## 0. Antes de abrir la IA: entienda los seis conceptos

**Esta vez no es opcional.** Lea
[CONCEPTOS_CONTROL_DE_ACCESO.md](../../../conceptos/CONCEPTOS_CONTROL_DE_ACCESO.md) y
firme la sección B de [9_checklist.md](9_checklist.md).

**Por qué:** en las otras versiones, si la IA se equivoca, algo no compila o no
responde. Aquí, si se equivoca, **todo responde 200** — y usted no tiene cómo
notarlo si no sabe qué debería fallar.

> **El conceptual NO se le sube a la IA.** Es para usted.

## 1. El punto de partida, verificado

```powershell
docker compose up -d --build
```

Y la prueba de humo de la **v2** pasa completa.

---

## Camino A — Chat web

### A.1 Qué subirle: los 8 archivos de la v3

| Archivo | Qué le dice |
|---|---|
| `docs/spec_kit/1_constitution.md` | Las reglas permanentes |
| `.../v3_control_acceso/2_spec.md` | **Qué** construir y sus 10 criterios |
| `.../v3_control_acceso/3_plan.md` | **Cómo**, con los cinco tropiezos |
| `.../v3_control_acceso/4_research.md` | Las decisiones y por qué |
| `.../v3_control_acceso/5_data_model.md` | Las cinco tablas y `verificar_acceso_ruta` |
| `.../v3_control_acceso/6_contracts.md` | Los formatos, el 401 y el 403 |
| `.../v3_control_acceso/7_quickstart.md` | Cómo se verifica |
| `.../v3_control_acceso/8_tasks.md` | El orden, en 11 fases |

> **Y el `9_checklist.md` no se sube, a propósito.** Es la compuerta que usted
> pasa ANTES de abrir la IA —revisa la especificación, no el código—, así que
> la IA no tiene nada que hacer con él. Por eso son siete documentos y no ocho.


### A.2 Prepare SU proyecto

**1. Copie la carpeta de specs** `v3_control_acceso` a su proyecto.

**2. Cree las carpetas y los archivos vacíos:**

```powershell
mkdir api_facturas\Autorizacion
```

```powershell
New-Item api_facturas\Modelos\ConfiguracionJwt.cs, api_facturas\Modelos\Sesion.cs, `
  api_facturas\Peticiones\SesionCrear.cs, api_facturas\Servicios\IServicioSesion.cs, `
  api_facturas\Servicios\ServicioSesion.cs, api_facturas\Controllers\SesionController.cs, `
  api_facturas\Repositorios\IRepositorioAcceso.cs, `
  api_facturas\Repositorios\RepositorioAccesoSqlServer.cs, `
  api_facturas\Autorizacion\ExigePermisoAttribute.cs, `
  api_facturas\Controllers\PermisosController.cs, `
  front_blazor\Servicios\EstadoSesion.cs, front_blazor\Servicios\ServicioSesion.cs, `
  front_blazor\Modelos\RespuestaSesion.cs, `
  front_blazor\Components\Pages\Sesion.razor, `
  front_blazor\Components\Layout\SesionActual.razor
```

**Son 15 archivos nuevos.** Y **muchos que crecen**, que es lo propio de esta
versión:

| | Qué le pasa |
|---|---|
| `api_facturas\ApiFacturas.csproj` | +`JwtBearer 9.0.10` |
| `api_facturas\Program.cs` | La configuración del token y el middleware |
| `api_facturas\appsettings.json` | La sección `Jwt` |
| `docker-compose.yml` | Las variables `Jwt__*` |
| `db\bdfacturas.sql` | Las contraseñas con hash |
| **Los 12 controladores** | `[Authorize]` y `[ExigePermiso]` |
| **Los 12 servicios del front** | El método `Autorizar()` |
| `front_blazor\Components\App.razor` | El `@rendermode` |
| `front_blazor\Components\Layout\NavMenu.razor` | El menú por permisos |

### A.3 El prompt de la v3 — cópielo tal cual

```
Actúa como mi asistente de programación para construir la VERSIÓN 3 de un
proyecto universitario. Te adjunto 8 documentos: la constitución y el spec kit
de la versión 3.

El proyecto es C# sobre ASP.NET Core (.NET 10) + SQL Server, con la interfaz
gráfica en Blazor Server. Si en tu respuesta aparece otro lenguaje o framework,
no leíste los documentos: detente y dímelo.

CONTEXTO — LAS VERSIONES SON ACUMULATIVAS:

Mi proyecto YA TIENE la v1 y la v2 construidas y funcionando: el CRUD de las 12
tablas de bdfacturas, con su API y sus 12 interfaces gráficas. Esas versiones
están CERRADAS. La v3 se construye ENCIMA.

QUÉ ES LA VERSIÓN 3 — EL CONTROL DE ACCESO, y lo primero es lo que NO es:

  NO agrega ni modifica NINGUNA tabla. El CRUD de usuario, rol, ruta,
  rol_usuario y rutarol YA ESTÁ (usuario, rol y ruta desde la v1;
  rol_usuario y rutarol desde la v2). Administrar la tabla de permisos y
  HACERLOS VALER son dos cosas distintas.

  La v3 le pone LA PUERTA a lo que ya existe. Tres cosas, en este orden:

  1. LA CONTRASEÑA deja de estar en claro en la semilla de la base: las ocho
     filas con hash de bcrypt costo 12. Y las contraseñas en claro quedan
     escritas en el quickstart, porque del hash no se vuelve a la clave y sin
     saberlas no se puede probar nada. El hash ya funcionaba desde antes
     (BCrypt.Net-Next ya está): lo que falta es que la semilla lo use.

  2. LA SESIÓN: POST /api/sesion recibe el correo y la contraseña EN EL CUERPO
     —no en la URL— y devuelve un JWT. Si fallan, responde 401 con EL MISMO
     MENSAJE para el correo inexistente y para la contraseña equivocada.

  3. EL PERMISO: cada operación comprueba si el rol de quien pide puede entrar
     a esa interfaz, y responde 403 si no. Lo resuelve el procedimiento
     almacenado verificar_acceso_ruta, que YA EXISTE en la base y cruza
     usuario -> rol_usuario -> rutarol. NO armes ese JOIN en C#.

REGLAS DE TRABAJO (no negociables):

1. La especificación manda. No agregues nada que los documentos no pidan: ni
   refresh token, ni recuperar contraseña, ni segundo factor, ni OAuth, ni
   ASP.NET Identity, ni permisos por operación. Si crees que falta algo,
   pregúntame antes.

2. Sigue 8_tasks.md FASE POR FASE, las once, en orden. En cada fase:
   a. Me explicas en 3-5 líneas qué vamos a hacer y por qué.
   b. Me entregas los archivos DE A UNO: ruta exacta y contenido COMPLETO de
      UN archivo, con comentarios didácticos en español. Esperas mi "listo".
   c. Al cerrar la fase me das su comando de verificación y qué esperar.
   Los archivos nuevos YA EXISTEN VACÍOS: no me des comandos para crearlos.

3. EL ORDEN NO SE PUEDE CAMBIAR: contraseña, sesión, permiso, interfaz. Sin la
   primera lo demás es decoración; sin la segunda no hay a quién preguntarle
   nada; sin la tercera el sistema sabe quién entra y le deja hacer todo.

4. LOS PERMISOS NO VAN DENTRO DEL TOKEN. Se consultan contra la base EN CADA
   PETICIÓN, llamando a verificar_acceso_ruta. Si fueran en el token, quitarle
   un permiso a un rol no surtiría efecto hasta que el token venciera — y el
   criterio 7 dice justamente que sí tiene que surtir efecto. El token lleva
   el correo y los nombres de los roles, y nada más: está FIRMADO, no cifrado,
   así que su contenido se lee sin ninguna clave.

5. CINCO TROPIEZOS QUE TIENES QUE EVITAR, Y LOS CINCO COMPILAN:
   - ClockSkew: ponlo en TimeSpan.Zero. Por defecto ASP.NET perdona 5 minutos
     y un token vencido responde 200 durante ese rato.
   - El 401 por defecto llega con el cuerpo VACÍO. Ponle cuerpo con
     JwtBearerEvents.OnChallenge, con el mismo sobre {estado, mensaje}.
   - UseAuthentication() va ANTES de UseAuthorization(). Al revés deja pasar
     todo, y arranca igual.
   - Si el nombre de la ruta no está en la tabla `ruta`, NADIE entra: falla
     cerrado, no abierto.
   - EstadoSesion en el front es SCOPED, no singleton. Con singleton habría un
     token para todos los que entren.

6. EL PERMISO SE EXIGE CON UN ATRIBUTO, [ExigePermiso("interfaz.x")], no con
   una línea al principio de cada método: así no se puede olvidar en un
   endpoint nuevo. Los nombres de las rutas son los que la tabla `ruta` ya
   trae sembrados: interfaz.usuarios, interfaz.facturas, interfaz.clientes,
   interfaz.productos, interfaz.personas, interfaz.empresas, interfaz.roles,
   interfaz.rutas, interfaz.vendedores, interfaz.permisos. No los inventes.

7. SOLO DOS ENDPOINTS QUEDAN ABIERTOS: GET / (el diagnóstico) y
   POST /api/sesion (que no puede exigir lo que todavía no existe). Los 12
   recursos de la v1 y la v2 exigen token, sin excepción.

8. CADA VERSIÓN ES API + INTERFAZ GRÁFICA. Las fases 7, 8 y 9 son del front y
   la versión no cierra sin ellas:
   - El token vive en EstadoSesion (scoped al circuito), NO en localStorage.
   - El token se manda a mano en cada servicio, NO con un DelegatingHandler:
     en Blazor Server la cadena de handlers se reutiliza y puede recibir el
     scope equivocado — o sea, el token de otra sesión.
   - El @rendermode va en App.razor, no en cada interfaz: si no, el layout se
     queda estático y el menú nunca ve la sesión del circuito. Y el prerender
     se APAGA, porque corre antes de que el circuito exista.
   - El menú se arma con GET /api/permisos/mios. Y ESO NO PROTEGE NADA:
     esconder una entrada del menú no es control de acceso. La protección es
     el 403 de la API.

9. Todo en español: nombres, comentarios y mensajes.

10. Trabajo en Windows con VS Code (PowerShell) y Docker Desktop. Usa curl.exe
    con la extensión: en PowerShell, curl pelado es otra cosa.

La versión 3 está TERMINADA solo cuando: (a) la prueba de humo de la v1 y la de
la v2 pasan completas CON TOKEN —la regresión— y (b) los 10 criterios de
2_spec.md §4 pasan con 7_quickstart.md, incluidos el criterio 7 (quitar un
permiso surte efecto sin volver a identificarse) y el criterio 9 (escribir la
dirección a mano sin permiso responde 403).

Empieza: resume en máximo 10 líneas qué vamos a construir y sobre qué base
—para confirmar que entendiste que la v3 NO agrega tablas— y arranca con la
Fase 0.
```

### A.4 El método de la conversación

1. **Pegue y diga "listo".** Un archivo por turno.
2. **Proteja la v1 y la v2.** Lo único que crece está en la lista de A.2.
3. **Cuando la IA diga «listo» en la fase 5, no le crea: pruebe el criterio
   7.** Quítele un permiso a un rol en la base y pida otra vez con el mismo
   token. Es la prueba de que los permisos no quedaron en el token.
4. **Si responde en otro lenguaje o pierde el hilo, reinicie el chat.**
5. **El cierre es triple:** regresión de la v1, regresión de la v2, y los diez
   criterios.

---

## Camino B — IDE agéntico

### B.1 Preparación

Abra el IDE sobre su proyecto de la v2 y copie antes la carpeta
`v3_control_acceso`. **No hay que crear archivos vacíos.**

### B.2 El prompt para el agente

```
Construye la VERSIÓN 3 de este proyecto: EL CONTROL DE ACCESO.

LAS VERSIONES SON ACUMULATIVAS: este proyecto YA TIENE la v1 y la v2
funcionando —el CRUD de las 12 tablas con su API y sus 12 interfaces
gráficas—. NO las modifiques más allá de lo que esta versión exige.

Primero LEE, en este orden: docs/spec_kit/1_constitution.md y los 8 documentos
de docs/spec_kit/versiones/v3_control_acceso/ (2_spec a 8_tasks). Lee también
el código existente para calcar el estilo. docs/spec_kit/ es SOLO LECTURA.

Después resume en máximo 10 líneas qué vas a construir y ESPERA MI
CONFIRMACIÓN antes de tocar un archivo.

LA v3 NO AGREGA NI MODIFICA NINGUNA TABLA. El CRUD de usuario, rol, ruta,
rol_usuario y rutarol ya está. Lo único que cambia en db/ es la SEMILLA de
usuario: las ocho contraseñas con hash de bcrypt costo 12, y las contraseñas en
claro escritas en 7_quickstart.md.

QUÉ CONSTRUYE: (1) POST /api/sesion que devuelve un JWT, con las credenciales
en el cuerpo y el MISMO 401 para el correo inexistente y la contraseña
equivocada; (2) el token exigido en los 12 recursos, con 401 si falta;
(3) el permiso por operación con el procedimiento verificar_acceso_ruta —que
YA EXISTE en la base— y 403 si el rol no puede; (4) la interfaz de
identificación y el menú armado por permisos.

REGLAS (no negociables):

1. La especificación manda: ni refresh token, ni recuperar contraseña, ni
   segundo factor, ni OAuth, ni ASP.NET Identity. Ante la duda, pregunta.

2. Sigue 8_tasks.md FASE POR FASE, las once. Al cerrar cada fase EJECUTA su
   verificación, muéstrame el resultado REAL —no el que esperabas— y espera mi
   OK. Un commit por fase.

3. LOS PERMISOS NO VAN EN EL TOKEN: se consultan con verificar_acceso_ruta en
   cada petición. No armes el JOIN de permisos en C#. El criterio 7 —quitar un
   permiso surte efecto sin volver a identificarse— no se puede cumplir de
   otra forma, y quiero que lo EJECUTES: borra una fila de rutarol, vuelve a
   pedir con el mismo token, y muéstrame el 403.

4. Evita estos cinco, que compilan todos: ClockSkew en TimeSpan.Zero; el 401
   con cuerpo (OnChallenge); UseAuthentication ANTES de UseAuthorization; una
   ruta que no esté en la tabla falla CERRADA; EstadoSesion scoped y no
   singleton.

5. El permiso se exige con [ExigePermiso("interfaz.x")] sobre el controlador,
   con los nombres que la tabla `ruta` ya trae. No los inventes: léelos de
   db/.

6. Solo GET / y POST /api/sesion quedan abiertos.

7. El front: el token en EstadoSesion (scoped), mandado a mano en cada
   servicio —no con un DelegatingHandler—, el @rendermode en App.razor con el
   prerender apagado, y el menú por GET /api/permisos/mios. Y deja escrito en
   el propio NavMenu que esconder una entrada del menú NO protege nada.

8. Todo en español, con comentarios didácticos.

9. Cierre triple: la regresión de la v1, la de la v2 —las dos con token— y los
   10 criterios de 7_quickstart.md. Con evidencia.
```

### B.3 Cómo supervisar al agente

| Alarma | Qué hacer |
|---|---|
| **Instala `Microsoft.AspNetCore.Identity`** | Recházelo. Trae su propio modelo de usuarios y tapa todo lo que esta versión enseña |
| **Mete los permisos en los `claims` del token** | Recházelo. Es el criterio 7 |
| **Escribe el `JOIN` de permisos en C#** | Recházelo. `verificar_acceso_ruta` ya existe |
| **Pone `[AllowAnonymous]` en algo que no sea `/` o la sesión** | Pregúntele dónde lo pide la spec |
| **Guarda el token en `localStorage`** | Recházelo. En Blazor Server no hace falta que baje al navegador |
| **Dice «listo» sin ejecutar la verificación** | Pídale la salida real. En esta versión «debería funcionar» es peligroso: todo responde 200 |
| **Pone el `@rendermode` en cada interfaz** | El menú no va a ver la sesión. Va en `App.razor` |

---

## Por qué así — la lección de la v3

En la v1 la lección era **dirigir con una especificación**. En la v2, **agregar
sobre un sistema vivo sin romperlo**. En la v3 es otra, y es incómoda:

> **Un sistema mal protegido no se ve mal.** Funciona, muestra datos, no da
> errores. La diferencia entre uno protegido y uno que lo aparenta **no está en
> lo que hace, sino en lo que NO deja hacer** — y eso solo se comprueba
> intentándolo.

| | |
|---|---|
| **Por eso hay que entender los conceptos ANTES** | Si no se sabe qué debería fallar, no se nota que no falla |
| **Por eso el criterio 7 está escrito así** | Es el único que distingue «consulta el permiso» de «se lo cree del token», y las dos cosas funcionan |
| **Por eso el criterio 9 se hace en el navegador, a mano** | Escribir la dirección es la única forma de saber si el menú era la protección |
| **Por eso el prompt lleva los cinco tropiezos** | Los cinco compilan. Tres dejan el sistema **menos seguro de lo que parece** |
