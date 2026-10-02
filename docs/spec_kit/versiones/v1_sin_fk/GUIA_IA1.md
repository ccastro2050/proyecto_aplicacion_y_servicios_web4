# Cómo construir la versión con IA — por chat o con un IDE agéntico

> Guía para trabajar la versión en curso con ayuda de IA por **cualquiera de
> los dos caminos**: un chat web (Gemini, DeepSeek, ChatGPT…) o un IDE
> agéntico (Antigravity, Cursor, Claude Code, Copilot en VS Code…).
> La clave del método es la misma en ambos: la IA no inventa — **sigue el
> spec kit**. Usted verifica; la IA propone (chat) o ejecuta bajo su
> supervisión (IDE).

---

## 0. Los dos caminos, en una tabla

| | **Camino A: chat web** | **Camino B: IDE agéntico** |
|---|---|---|
| Herramientas | Gemini, DeepSeek, ChatGPT, Claude (web) | Antigravity, Cursor, Claude Code, Copilot agente |
| ¿Cómo conoce la spec? | Usted le **sube los 8 archivos** | El agente **lee la carpeta `docs/spec_kit/` de su proyecto** |
| ¿Quién crea la estructura de carpetas? | **USTED la crea a mano** en su proyecto — el chat no puede tocar su disco (los dos comandos, carpetas y archivos vacíos, están en A.2) | El agente crea carpetas y archivos solo |
| ¿Quién escribe los archivos? | Usted copia/pega lo que la IA propone | El agente crea y edita los archivos directamente |
| ¿Quién ejecuta los comandos? | Usted, en la terminal integrada de VS Code, y pega la salida | El agente (pidiéndole permiso); usted revisa la salida |
| Su papel | Operador: pegar, ejecutar y reportar | Supervisor: revisar diffs y aprobar |
| Riesgo típico | La IA "olvida" el contexto en chats largos o repite archivos | El agente avanza demasiado rápido: hace varias fases de un tirón o agrega cosas no pedidas |

> **¿De qué "comandos" habla la tabla?** De los **comandos de verificación
> de cada fase** que pide `8_tasks.md`. Ejemplos reales de la v1:
> `docker compose up -d` (levantar la BD), `dotnet build` (compilar),
> `dotnet run --project pruebas` (la prueba de capas) y los `curl` del
> smoke test. En el chat, la IA se los dicta y USTED los ejecuta; en el
> IDE agéntico, el agente los ejecuta y usted revisa la salida.

En ambos casos, "terminado" significa lo mismo: **los 6 criterios de
aceptación de `2_spec.md` en verde**, verificados con el smoke test de
`7_quickstart.md` — corrido por usted.

---

## Camino A — Chat web (Gemini, DeepSeek, ChatGPT…)

### A.1 Qué subirle (los 8 archivos de la v1)

En el chat (todos aceptan adjuntar archivos; si el suyo no, pegue el
contenido de cada uno en el mismo orden):

| # | Archivo | Papel |
|---|---|---|
| 1 | `docs/spec_kit/1_constitution.md` | Las reglas permanentes (C#, capas, un comando) |
| 2 | `docs/spec_kit/versiones/v1_sin_fk/2_spec.md` | QUÉ construir y los criterios de aceptación |
| 3 | `.../v1_sin_fk/3_plan.md` | CÓMO: stack, carpetas, capas |
| 4 | `.../v1_sin_fk/4_research.md` | Decisiones y alternativas (el porqué del plan) |
| 5 | `.../v1_sin_fk/5_data_model.md` | La BD completa (dada) y la tabla producto |
| 6 | `.../v1_sin_fk/6_contracts.md` | Los 7 endpoints exactos |
| 7 | `.../v1_sin_fk/7_quickstart.md` | El smoke test de validación |
| 8 | `.../v1_sin_fk/8_tasks.md` | Las fases, en orden |

Además de los 8 documentos, la versión trae **dos artefactos que NO se
suben al chat ni los genera la IA**: `db/bdfacturas.sql` (el script
completo de la BD en dialecto SQL Server — SQL Server lo ejecuta
solo, la primera vez).
proyecto (ver A.2, paso 5).

> **¿Qué es un "artefacto"?** En ingeniería de software, cualquier archivo
> que el proceso produce o entrega. Aquí lo usamos para distinguir: los
> **documentos** se LEEN (la IA construye a partir de ellos); los
> **artefactos** de `db/` se USAN tal cual — son insumo dado, como la
> imagen de SQL Server. Analogía: los documentos son el plano de la casa;
> los artefactos son prefabricados que llegan listos a la obra.

**No suba nada más.** El mapa de versiones no hace falta (y le revelaría a
la IA lo que viene — la regla es que la v1 no anticipa).

### A.2 Prepare SU proyecto (ANTES de abrir el chat)

**Ojo: NO se construye dentro de la carpeta clonada.** El repositorio
clonado es el **material de referencia**. Su trabajo de reconstrucción va
en un **proyecto propio, en una carpeta nueva y vacía**:

1. Cree una carpeta para su proyecto (ej.: `mi_v1_sin_fk/`) donde usted
   guarda sus trabajos — fuera de la carpeta clonada.
2. Ábrala en VS Code (*File → Open Folder*).
3. **Cree las CARPETAS** (el chat no puede tocar su disco). En la terminal
   integrada (*Terminal → New Terminal*, PowerShell), parado en su carpeta:

   ```powershell
   mkdir docs\spec_kit\versiones\v1_sin_fk, db, api_facturas\Modelos, api_facturas\Peticiones, api_facturas\Controllers, api_facturas\Servicios, api_facturas\Repositorios, api_facturas\Excepciones, api_facturas\pruebas, front_blazor\Modelos, front_blazor\Servicios, front_blazor\Components\Layout, front_blazor\Components\Pages, front_blazor\wwwroot
   ```

4. **Cree los ARCHIVOS VACÍOS** — **USTED los irá llenando** uno a uno,
   pegando en cada archivo el código que la IA le entregue:

   ```powershell
   # Los de la API — SEIS recursos con sus capas:
   New-Item .gitignore, docker-compose.yml, api_facturas\ApiFacturas.csproj,`
     api_facturas\Program.cs, api_facturas\appsettings.json,`
     api_facturas\Dockerfile, api_facturas\Modelos\Producto.cs,`
     api_facturas\Peticiones\ProductoCrear.cs,`
     api_facturas\Peticiones\ProductoReemplazo.cs,`
     api_facturas\Peticiones\ProductoActualizar.cs,`
     api_facturas\Repositorios\IRepositorioProducto.cs,`
     api_facturas\Repositorios\RepositorioProductoSqlServer.cs,`
     api_facturas\Servicios\IServicioProducto.cs,`
     api_facturas\Servicios\ServicioProducto.cs,`
     api_facturas\Controllers\ProductoController.cs,`
     api_facturas\Modelos\Empresa.cs, api_facturas\Peticiones\EmpresaCrear.cs,`
     api_facturas\Peticiones\EmpresaReemplazo.cs,`
     api_facturas\Peticiones\EmpresaActualizar.cs,`
     api_facturas\Repositorios\IRepositorioEmpresa.cs,`
     api_facturas\Repositorios\RepositorioEmpresaSqlServer.cs,`
     api_facturas\Servicios\IServicioEmpresa.cs,`
     api_facturas\Servicios\ServicioEmpresa.cs,`
     api_facturas\Controllers\EmpresaController.cs,`
     api_facturas\Modelos\Persona.cs, api_facturas\Peticiones\PersonaCrear.cs,`
     api_facturas\Peticiones\PersonaReemplazo.cs,`
     api_facturas\Peticiones\PersonaActualizar.cs,`
     api_facturas\Repositorios\IRepositorioPersona.cs,`
     api_facturas\Repositorios\RepositorioPersonaSqlServer.cs,`
     api_facturas\Servicios\IServicioPersona.cs,`
     api_facturas\Servicios\ServicioPersona.cs,`
     api_facturas\Controllers\PersonaController.cs, api_facturas\Modelos\Rol.cs,`
     api_facturas\Peticiones\RolCrear.cs,`
     api_facturas\Peticiones\RolReemplazo.cs,`
     api_facturas\Peticiones\RolActualizar.cs,`
     api_facturas\Repositorios\IRepositorioRol.cs,`
     api_facturas\Repositorios\RepositorioRolSqlServer.cs,`
     api_facturas\Servicios\IServicioRol.cs,`
     api_facturas\Servicios\ServicioRol.cs,`
     api_facturas\Controllers\RolController.cs, api_facturas\Modelos\Ruta.cs,`
     api_facturas\Peticiones\RutaCrear.cs,`
     api_facturas\Peticiones\RutaReemplazo.cs,`
     api_facturas\Peticiones\RutaActualizar.cs,`
     api_facturas\Repositorios\IRepositorioRuta.cs,`
     api_facturas\Repositorios\RepositorioRutaSqlServer.cs,`
     api_facturas\Servicios\IServicioRuta.cs,`
     api_facturas\Servicios\ServicioRuta.cs,`
     api_facturas\Controllers\RutaController.cs,`
     api_facturas\Modelos\Usuario.cs, api_facturas\Peticiones\UsuarioCrear.cs,`
     api_facturas\Peticiones\UsuarioReemplazo.cs,`
     api_facturas\Peticiones\UsuarioActualizar.cs,`
     api_facturas\Repositorios\IRepositorioUsuario.cs,`
     api_facturas\Repositorios\RepositorioUsuarioSqlServer.cs,`
     api_facturas\Servicios\IServicioUsuario.cs,`
     api_facturas\Servicios\ServicioUsuario.cs,`
     api_facturas\Controllers\UsuarioController.cs,`
     api_facturas\Excepciones\NoEncontradoExcepcion.cs,`
     api_facturas\pruebas\PruebaCapas.csproj, api_facturas\pruebas\Programa.cs

   # Y los de la INTERFAZ GRAFICA, que tambien es de la v1:
   New-Item front_blazor\FrontFacturas.csproj, front_blazor\Program.cs,`
     front_blazor\appsettings.json, front_blazor\Dockerfile,`
     front_blazor\wwwroot\app.css, front_blazor\Components\App.razor,`
     front_blazor\Components\Routes.razor,`
     front_blazor\Components\_Imports.razor,`
     front_blazor\Components\Layout\MainLayout.razor,`
     front_blazor\Components\Layout\NavMenu.razor,`
     front_blazor\Components\Pages\Home.razor, front_blazor\Modelos\Producto.cs,`
     front_blazor\Servicios\ServicioProducto.cs,`
     front_blazor\Modelos\Empresa.cs, front_blazor\Servicios\ServicioEmpresa.cs,`
     front_blazor\Modelos\Persona.cs, front_blazor\Servicios\ServicioPersona.cs,`
     front_blazor\Modelos\Rol.cs, front_blazor\Servicios\ServicioRol.cs,`
     front_blazor\Modelos\Ruta.cs, front_blazor\Servicios\ServicioRuta.cs,`
     front_blazor\Modelos\Usuario.cs, front_blazor\Servicios\ServicioUsuario.cs,`
     front_blazor\Components\Pages\Productos.razor,`
     front_blazor\Components\Pages\Empresas.razor,`
     front_blazor\Components\Pages\Personas.razor,`
     front_blazor\Components\Pages\Roles.razor,`
     front_blazor\Components\Pages\Rutas.razor,`
     front_blazor\Components\Pages\Usuarios.razor
   ```

   (`db/bdfacturas.sql` NO está en la lista a propósito:
   esos no nacen vacíos — se copian del repositorio en el paso 5.)

5. **Copie y pegue los 10 archivos que vienen dados** (con el explorador
   de Windows: Ctrl+C, Ctrl+V), desde la carpeta clonada del curso hacia
   SU proyecto — cada uno a la misma ruta:

   | Del clon del curso | A su proyecto |
   |---|---|
   | `db\bdfacturas.sql` | `db\` |
   | `docs\spec_kit\1_constitution.md` | `docs\spec_kit\` |
   | Los 7 `.md` de `docs\spec_kit\versiones\v1_sin_fk\` | `docs\spec_kit\versiones\v1_sin_fk\` |

   (Estos 10 vienen dados — la IA no los genera: las specs se le SUBEN al
   chat, y los scripts de `db/` son la BD completa ya escrita.)

**Antes de abrir el chat, verifique:** `docs\spec_kit\1_constitution.md`
debe existir, `docs\spec_kit\versiones\v1_sin_fk\` debe tener
**7 archivos** (2_spec a 8_tasks), y `db\` debe tener `bdfacturas.sql`
(con contenido, ~1.060 líneas). Si algo está vacío, falta el
paso 5.

> **Son 63 archivos de API y 29 de interfaz grafica, y la cuenta
> sorprende.** Es la medida honesta de lo que son seis recursos con sus
> capas — y es exactamente el argumento del que nace la idea de generar
> codigo: cuando se repite tanto, hay un patron, y el patron se puede
> decir una vez.

La estructura queda lista ANTES de hablar con la IA (es la de `3_plan.md`
§2); al lado, la fase en la que la IA le entregará el código de cada
archivo para que USTED lo pegue:

```
mi_v1_sin_fk/                   ← SU carpeta
├── docs/
│   └── spec_kit/                 ← las especificaciones, IGUAL que en el repo
│       ├── 1_constitution.md
│       └── versiones/
│           └── v1_sin_fk/  ← los 7 documentos de la v1
├── .gitignore                    ← Fase 6 (excluye bin/, obj/, *.session.sql)
├── docker-compose.yml            ← Fase 0 (postgres) y Fase 6 (api-facturas)
├── db/
│   ├── bdfacturas.sql            ← Fase 0: COPIADO del repo (no lo genera la IA)
└── api_facturas/                 ← TODO el código va aquí adentro
    ├── ApiFacturas.csproj        ← Fase 1
    ├── appsettings.json          ← Fase 1
    ├── Program.cs                ← Fase 5
    ├── Dockerfile                ← Fase 6
    ├── Modelos/
    │   └── Producto.cs           ← Fase 1 (el modelo = la entidad)
    ├── Peticiones/
    │   ├── ProductoCrear.cs      ← Fase 2 (la frontera, una petición por verbo)
    │   ├── ProductoReemplazo.cs  ← Fase 2
    │   └── ProductoActualizar.cs ← Fase 2
    ├── Controllers/
    │   └── ProductoController.cs      ← Fase 5
    ├── Servicios/
    │   ├── IServicioProducto.cs       ← Fase 3
    │   └── ServicioProducto.cs        ← Fase 4
    ├── Repositorios/
    │   ├── IRepositorioProducto.cs    ← Fase 3
    │   └── RepositorioProductoSqlServer.cs  ← Fase 3
    ├── Excepciones/
    │   └── NoEncontradoExcepcion.cs   ← Fase 2
    └── pruebas/
        ├── PruebaCapas.csproj         ← Fase 4 (criterio 6)
        └── Programa.cs                ← Fase 4
```

**Cómo pegar lo que la IA entregue** (VS Code): el archivo **ya existe
vacío** (lo creó el comando del paso 4) — ábralo desde el explorador,
pegue el contenido COMPLETO que entregó la IA y guarde (`Ctrl+S`). Un
bloque de la IA = un archivo completo (reemplaza todo, nunca "agregue al
final").

**Qué le entrega la IA y qué hace usted con eso:**

| La IA le entrega | Usted lo pone en |
|---|---|
| Un bloque de código con su ruta (ej.: "Archivo: `api_facturas/Modelos/Producto.cs`") | Ese archivo, que ya existe vacío en ESA ruta |
| (La BD no la entrega la IA) | `db/bdfacturas.sql` se **copia del repositorio** tal cual — si la IA intenta escribirle un `CREATE TABLE`, recuérdele que la BD ya viene dada |
| Comandos (docker compose, dotnet build, curl) | La terminal integrada del IDE, parado en la carpeta correcta (ver abajo) |

Si un bloque llega **sin ruta**, no adivine: pregúntele "¿en qué archivo va
esto?". Y si le dice "modifica la línea X", pídale mejor el archivo
completo actualizado.

**A la terminal SOLO se le pegan COMANDOS** — lo que viene en las cajitas
de código del chat, uno a la vez. Si pega el texto del mensaje (las
frases), la terminal intentará ejecutar cada palabra y llenará la interfaz gráfica
de errores tipo `'Te' no se reconoce como nombre de un cmdlet` (no daña
nada, pero asusta). Al chat, texto; a la terminal, comandos.

**¿Desde qué carpeta se corre cada comando?** (el otro error común es
correr un comando parado en la carpeta equivocada — "no encuentro el
archivo"):

| Comando | Se corre desde |
|---|---|
| `docker compose ...` | La **raíz** de su proyecto (ahí vive `docker-compose.yml`) |
| `dotnet build` · `dotnet run` · `dotnet run --project pruebas` | `api_facturas\` (ahí vive el `.csproj`) — primero `cd api_facturas` |

> Nota: el SDK de .NET local es **opcional**. Si no lo tiene, salte las
> verificaciones de compilación intermedias (dotnet build) y confíe en el
> método "pegue primero": el build real lo hace Docker en la Fase 6, y los
> errores se corrigen ahí con la IA.

### A.3 El prompt (cópielo tal cual como PRIMER mensaje)

**Antes de enviar el primer mensaje, tres chequeos en el chat:**

1. **Los 8 adjuntos**: verifique en el chat que aparecen los 8 documentos
   (deslice el carrusel de adjuntos si no se ven todos).
2. **Active el modo de razonamiento** si el chat lo tiene (en DeepSeek se
   llama **"Pensamiento Profundo"**; en otros, "Thinking" o "Razonar"):
   sigue mucho mejor las reglas estrictas de este prompt.
3. **Apague la búsqueda web** si el chat la tiene (en DeepSeek,
   **"Búsqueda inteligente"**): no se necesita y puede traer código de
   internet por fuera de la spec — justo lo que la regla 1 prohíbe.

```
Actúa como mi asistente de programación para construir la VERSIÓN 1 de un
proyecto universitario, partiendo de cero. Te adjunto 8 documentos: una
constitución (reglas permanentes) y el spec kit de la versión 1 (spec, plan,
research con las decisiones, modelo de datos, contratos, quickstart y tareas).

El proyecto es C# sobre ASP.NET Core (.NET 10) + SQL Server — así lo fija
3_plan.md.

LO QUE HAY QUE CONSTRUIR, Y SON SEIS RECURSOS — NO UNO:

La v1 es el CRUD de las SEIS tablas que NO tienen clave foranea, cada una
con su interfaz gráfica:

   producto   codigo (texto, PK) · nombre · stock · valorunitario
   empresa    codigo (texto, PK) · nombre
   persona    codigo (texto, PK) · nombre · email · telefono
   rol        id (SERIAL)        · nombre
   ruta       id (SERIAL)        · ruta · descripcion
   usuario    email (texto, PK)  · contrasena

Son SEIS REBANADAS VERTICALES IDENTICAS salvo los campos: modelo, tres
peticiones por verbo, interfaz + repositorio, interfaz + servicio,
controlador, y su interfaz. Que se repitan es el punto del ejercicio.

DOS COSAS QUE CAMBIAN SEGUN LA TABLA, y no se pueden calcar:

  · `rol` y `ruta` tienen llave SERIAL: el POST NO manda el id —lo genera
    la base—, asi que su peticion de creacion no lo lleva, y su modelo NO
    marca el Id como `required`. Si lo exige, el POST tendria que
    inventarle una llave.
  · En `ruta`, la COLUMNA se llama `ruta` y la PROPIEDAD tiene que
    llamarse distinto (RutaTexto), porque C# no permite una propiedad con
    el mismo nombre que su clase. El SELECT necesita el alias
    `ruta AS RutaTexto`: sin el, Dapper mapea por nombre, no encuentra la
    columna, y el campo llega NULL EN SILENCIO —la API responde 200 con el
    campo vacio—.

Y LA VERSION INCLUYE SU INTERFAZ GRÁFICA: seis interfaces gráficas en Blazor Server, una por
recurso, con direccion propia (/productos, /empresas, /personas, /roles,
/rutas, /usuarios) y UN SERVICIO POR RECURSO —nunca un ApiService generico
con la tabla como parametro—. La interfaz gráfica no le habla al usuario en jerga:
ni PUT, ni PATCH, ni 422. Los dos botones de guardar se llaman "Guardar la
ficha completa" y "Guardar solo lo que cambie".
 Si en tu respuesta aparece OTRO lenguaje o framework (Python,
Java, Node, PHP…), significa que no leíste los documentos adjuntos: detente
y dímelo en vez de continuar.

REGLAS DE TRABAJO (no negociables):

1. La especificación manda. No agregues NADA que los documentos no pidan:
   ni paquetes NuGet extra (solo Microsoft.Data.SqlClient y
   Swashbuckle.AspNetCore), ni Entity
   Framework, ni Swagger, ni tablas extra, ni motores extra, ni fábricas
   "por si acaso", ni mejoras de tu cosecha. Si crees que falta algo,
   pregúntame antes.
2. Vamos a seguir 8_tasks.md FASE POR FASE, en orden. En cada fase:
   a. Me explicas en 3-5 líneas qué vamos a hacer y por qué.
   b. Me entregas los archivos de la fase DE A UNO: primero la ruta exacta
      y el contenido COMPLETO de UN solo archivo (listo para copiar y
      pegar, con los comentarios didácticos en español que exige la
      constitución). Esperas mi "listo" y solo entonces me das el
      siguiente archivo de la fase.
   c. Al cerrar la fase me dices su comando de verificación y QUÉ salida
      esperar. Correr esa verificación en el momento es OPCIONAL: puedo
      dejarla para el final — tú sigues con la fase siguiente cuando yo
      diga "listo".
   NOTA: la estructura de carpetas y los archivos vacíos YA EXISTEN en mi
   proyecto — no me des comandos para crearlos; tu trabajo es dictarme el
   CONTENIDO de cada archivo.
3. Los errores NO nos frenan. Si te pego un error, lo diagnosticas y me
   das el archivo completo corregido; si no sale rápido, seguimos con las
   fases y lo retomamos al final. Al terminar todas las fases me guías
   para correr el smoke test de 7_quickstart.md y corregimos juntos todo
   lo que salga.
4. El código debe cumplir los contratos de 6_contracts.md al pie de la
   letra: mismos verbos, mismas rutas, mismos códigos de estado, mismos
   formatos de respuesta (incluido el contraste PUT=reemplazo completo vs
   PATCH=parcial, y el 422 con la lista de errores de validación).
5. Todo en español: nombres, comentarios y mensajes. C# sobre ASP.NET Core
   (.NET 10).
6. Yo trabajo en Windows con un IDE (VS Code, usando su terminal integrada
   de PowerShell) y Docker Desktop. Dame los comandos para ese entorno.
7. La base de datos YA VIENE DADA en db/bdfacturas.sql — se
   montan tal cual en el compose; no escribas ni modifiques SQL de
   creación de tablas.
8. En mi máquina TAMBIÉN corre el proyecto clonado del curso con sus
   puertos originales. Para que ambos convivan, MI proyecto:
   a. Publica los puertos del host con +100: en el docker-compose.yml la
      API va "8132:8032" y SQL Server va "11563:1433" (adentro de los
      contenedores todo queda igual que en los documentos).
   b. El docker-compose.yml empieza con la línea `name: mi_v1_sin_fk`
      (antes de services:) — así Docker lo trata como un proyecto
      distinto al del curso, con sus propios contenedores y volúmenes,
      aunque las carpetas se llamen parecido.
   La cadena de conexión por defecto de appsettings.json (para correr sin
   Docker) apunta a localhost,11563. Cuando me des URLs o comandos de
   prueba, usa localhost:8132 (API) y localhost,11563 (BD).

Al final, la versión 1 está TERMINADA solo cuando pasan los 6 criterios de
aceptación de 2_spec.md, verificados con el smoke test de 7_quickstart.md.

Empieza: resume en máximo 10 líneas qué vamos a construir (para confirmar
que entendiste el alcance) y luego arranca con la Fase 0.
```

### A.4 El método de la conversación

1. **Pegue primero, ejecute cuando quiera.** Lo obligatorio es pegar cada
   archivo en su ruta y responder "listo". Las verificaciones de cada fase
   puede correrlas en el momento (ideal: detecta errores temprano) o
   dejarlas todas para el final — **no son un peaje para avanzar**.
2. **No se quede varado en un error.** Si algo falla en una fase y no sale
   rápido, anótelo, siga copiando las fases siguientes y retómelo al
   final — muchos errores desaparecen cuando el sistema está completo, y
   los que queden se corrigen en el paso siguiente.
3. **El punto de control real es el smoke test final** (7_quickstart.md):
   al terminar las fases, córralo en la terminal integrada del IDE y pegue
   en el chat CADA error tal cual salga (completo). La IA le entrega el
   archivo corregido, usted lo pega y repite hasta que los 6 criterios
   estén en verde. **Ojo con los puertos**: SU proyecto corre con +100
   (regla 8 del prompt) — donde el quickstart diga `localhost:8032` use
   `localhost:8132`, y donde diga `11463` use `11563`.
4. **Si la IA se acelera** y entrega varios archivos de un tirón,
   recuérdele la regla 2b: "de a uno, espera mi listo".
5. **Si la primera respuesta llega en OTRO lenguaje** (Python, Java, Node,
   PHP…), no corrija sobre eso: es la señal inequívoca de que la IA **no
   leyó los adjuntos**. Cierre ese chat, verifique que los 8 documentos
   realmente cargaron (deslice el carrusel de adjuntos) y que son los de
   ESTE proyecto (3_plan.md debe decir C#/ASP.NET Core + SQL Server), y
   empiece de nuevo con el prompt tal cual.
6. **Si el chat pierde el contexto** (conversaciones largas): abra un chat
   nuevo, vuelva a subir los 8 documentos y agregue al prompt: "Ya tengo
   construidas las fases 0 a N; te pego el código actual. Continuemos en
   la fase N+1" (y pegue sus archivos).

---

## Camino B — IDE agéntico (Antigravity, Cursor, Claude Code…)

Un IDE agéntico tiene a la IA **dentro del proyecto**: lee los archivos
por sí misma, crea y edita código directamente, y puede ejecutar comandos
en la terminal (pidiendo permiso). Usted pasa de operador a **supervisor**.

### B.1 Preparación

**Igual que en el chat: NO se trabaja dentro de la carpeta clonada** (esa
es la referencia). El agente construye en SU proyecto:

1. Cree una carpeta nueva y vacía para su proyecto (ej.: `mi_v1_sin_fk/`)
   y copie dentro: los 8 documentos de la tabla A.1 en `docs\spec_kit\`
   replicando la estructura por versiones (`docs\spec_kit\1_constitution.md`
   + `docs\spec_kit\versiones\v1_sin_fk\` con los 7 de la
   versión), y el script `db\bdfacturas.sql` del
   repositorio (la BD viene dada — el agente no debe generarla).
2. Abra SU carpeta en el IDE (en Antigravity: *Open Folder*; el agente verá
   `docs/spec_kit/` — no hay que subirle nada).
3. Tenga Docker Desktop corriendo (el agente necesitará levantar SQL Server).
4. Active el modo agente (en Antigravity, el *Agent Manager*; en otros IDE,
   el chat en modo "agent").

### B.2 El prompt para el agente (cópielo tal cual)

```
Construye la VERSIÓN 1 de este proyecto, partiendo de cero.

Primero lee, en este orden, los 8 documentos que están bajo docs/spec_kit/
(1_constitution.md en la raíz; los demás en versiones/v1_sin_fk/):
1_constitution, 2_spec, 3_plan, 4_research, 5_data_model, 6_contracts,
7_quickstart y 8_tasks. Después resume en máximo 10 líneas qué vas a
construir y espera mi confirmación antes de tocar nada. El código va en la
raíz de este proyecto según la estructura de 3_plan.md (docs/spec_kit/ es
solo lectura: no la modifiques). La base de datos YA VIENE DADA en
db/bdfacturas.sql — úsalo tal cual para montar SQL Server;
no escribas ni modifiques SQL de creación de tablas.

LO QUE HAY QUE CONSTRUIR, Y SON SEIS RECURSOS — NO UNO:

La v1 es el CRUD de las SEIS tablas que NO tienen clave foranea, cada una
con su interfaz gráfica:

   producto   codigo (texto, PK) · nombre · stock · valorunitario
   empresa    codigo (texto, PK) · nombre
   persona    codigo (texto, PK) · nombre · email · telefono
   rol        id (SERIAL)        · nombre
   ruta       id (SERIAL)        · ruta · descripcion
   usuario    email (texto, PK)  · contrasena

Son SEIS REBANADAS VERTICALES IDENTICAS salvo los campos: modelo, tres
peticiones por verbo, interfaz + repositorio, interfaz + servicio,
controlador, y su interfaz. Que se repitan es el punto del ejercicio.

DOS COSAS QUE CAMBIAN SEGUN LA TABLA, y no se pueden calcar:

  · `rol` y `ruta` tienen llave SERIAL: el POST NO manda el id —lo genera
    la base—, asi que su peticion de creacion no lo lleva, y su modelo NO
    marca el Id como `required`. Si lo exige, el POST tendria que
    inventarle una llave.
  · En `ruta`, la COLUMNA se llama `ruta` y la PROPIEDAD tiene que
    llamarse distinto (RutaTexto), porque C# no permite una propiedad con
    el mismo nombre que su clase. El SELECT necesita el alias
    `ruta AS RutaTexto`: sin el, Dapper mapea por nombre, no encuentra la
    columna, y el campo llega NULL EN SILENCIO —la API responde 200 con el
    campo vacio—.

Y LA VERSION INCLUYE SU INTERFAZ GRÁFICA: seis interfaces gráficas en Blazor Server, una por
recurso, con direccion propia (/productos, /empresas, /personas, /roles,
/rutas, /usuarios) y UN SERVICIO POR RECURSO —nunca un ApiService generico
con la tabla como parametro—. La interfaz gráfica no le habla al usuario en jerga:
ni PUT, ni PATCH, ni 422. Los dos botones de guardar se llaman "Guardar la
ficha completa" y "Guardar solo lo que cambie".

REGLAS (no negociables):

1. La especificación manda. No agregues NADA que los documentos no pidan:
   ni paquetes NuGet extra (solo Microsoft.Data.SqlClient y
   Swashbuckle.AspNetCore), ni Entity
   Framework, ni Swagger, ni tablas extra, ni motores extra, ni fábricas
   "por si acaso". Si crees que falta algo, pregúntame antes.
2. Sigue 8_tasks.md fase por fase, en orden. Al terminar cada fase, EJECUTA
   su verificación (la que dice la propia fase), muéstrame el resultado
   real, y espera mi OK antes de pasar a la siguiente.
3. El código debe cumplir 6_contracts.md al pie de la letra: verbos, rutas,
   códigos de estado y formatos de respuesta exactos (incluido el contraste
   PUT=reemplazo completo vs PATCH=parcial y el 422 con la lista de errores
   de validación).
4. Todo en español, C# sobre ASP.NET Core (.NET 10), con los comentarios
   didácticos que exige la constitución.
5. En esta máquina TAMBIÉN corre el proyecto clonado del curso. MI proyecto
   publica los puertos del host con +100 (API "8132:8032", SQL Server
   "11563:1433") y su docker-compose.yml empieza con `name: mi_v1_sin_fk`.
6. Al final, corre el smoke test completo de 7_quickstart.md §2 (con mis
   puertos: localhost:8132) y muéstrame la evidencia de los 6 criterios de
   aceptación de 2_spec.md. La versión no está terminada hasta que los 6
   estén en verde.
```

### B.3 Cómo supervisar al agente

- **Revise cada diff** antes de aprobar: ¿el archivo está donde dice
  3_plan.md §2? ¿Tiene los comentarios en español? ¿No agregó paquetes?
- **Freno de emergencia:** si el agente hace varias fases de un tirón,
  deténgalo y pídale: "vuelve a la fase N y muéstrame su verificación".
- **No le crea "terminado":** pídale la evidencia (la salida real de los
  comandos). El criterio de cierre es el smoke test corrido y en verde.
