# Cómo construir la VERSIÓN 2 con IA — por chat o con un IDE agéntico

> **La v2 se construye ENCIMA de la v1.** Esto no es un proyecto nuevo: es un
> sistema vivo al que se le agrega sin romperlo, y es la lección de fondo de
> esta versión.
>
> | | |
> |---|---|
> | **Qué hay que construir** | [2_spec.md](2_spec.md) |
> | **El orden** | [8_tasks.md](8_tasks.md) — **doce** fases |
> | **Los formatos** | [6_contracts.md](6_contracts.md) |
> | **Cómo se verifica** | [7_quickstart.md](7_quickstart.md) |

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

## 0. Los dos caminos

| | **Camino A — chat web** | **Camino B — IDE agéntico** |
|---|---|---|
| **Herramientas** | Gemini, DeepSeek, ChatGPT… | Antigravity, Cursor, Claude Code… |
| **Quién toca el disco** | **Usted.** La IA le entrega texto y usted lo pega | **El agente.** Escribe los archivos |
| **Cómo recibe las specs** | **Se las SUBE** como adjuntos | **Las LEE** del repositorio |
| **El riesgo** | Perder el hilo en una conversación larga | Que toque lo que no debe |
| **Qué se aprende** | A dirigir con una especificación | A **supervisar** con una especificación |

**Son el mismo trabajo y la misma spec.** Se elige uno, no los dos a la vez.

> **Y en los dos el método es el mismo, que es lo que importa:** **fase por
> fase**, los archivos **de a uno**, y no se avanza sin verificar.

---

## 1. El punto de partida — verifíquelo ANTES de abrir la IA

```powershell
docker compose up -d --build
```

Y la prueba de humo de la v1
([7_quickstart de la v1](../v1_sin_fk/7_quickstart.md) §2) **pasa completa**.

> **Si la v1 no arranca, no empiece la v2.** Construir encima de algo que no
> funciona garantiza que después no se sabrá qué rompió qué.

---

## Camino A — Chat web

### A.1 Qué subirle: los 8 archivos de la v2

| Archivo | Qué le dice a la IA |
|---|---|
| `docs/spec_kit/1_constitution.md` | Las reglas permanentes del proyecto |
| `.../v2_con_fk/2_spec.md` | **Qué** construir y sus 18 criterios |
| `.../v2_con_fk/3_plan.md` | **Cómo**: la pila, las capas, los archivos |
| `.../v2_con_fk/4_research.md` | Las decisiones y por qué |
| `.../v2_con_fk/5_data_model.md` | Las tablas, los procedimientos, el disparador |
| `.../v2_con_fk/6_contracts.md` | Los formatos exactos |
| `.../v2_con_fk/7_quickstart.md` | Cómo se verifica |
| `.../v2_con_fk/8_tasks.md` | El orden, en 12 fases |

> **Los conceptuales de `docs/conceptos/` NO se suben.** Son para **usted** —
> para entender qué está pidiendo y poder discutirlo—. La IA no los necesita, y
> ocupan el contexto que hace falta para el trabajo.
>
> **El `9_checklist.md` tampoco se sube.** Se firma antes, y es suyo.

### A.2 Prepare SU proyecto

Usted ya tiene su proyecto de la v1 **funcionando**. La v2 se construye ahí
mismo.

**1. Copie la carpeta de specs de la v2** desde el clon del curso a su
proyecto:

```
docs\spec_kit\versiones\v2_con_fk\     (los 8 .md, menos GUIA_IA2)
```

**2. Cree las carpetas nuevas** (solo si no las tiene):

```powershell
mkdir front_blazor\Modelos, front_blazor\Servicios, front_blazor\Components\Pages
```

**3. Cree los archivos VACÍOS de la API** — la IA no puede tocar su disco, y
usted los va llenando uno por uno:

```powershell
New-Item api_facturas\Modelos\Cliente.cs, api_facturas\Peticiones\ClienteCrear.cs,`
  api_facturas\Peticiones\ClienteReemplazo.cs,`
  api_facturas\Peticiones\ClienteActualizar.cs,`
  api_facturas\Repositorios\IRepositorioCliente.cs,`
  api_facturas\Repositorios\RepositorioClienteSqlServer.cs,`
  api_facturas\Servicios\IServicioCliente.cs,`
  api_facturas\Servicios\ServicioCliente.cs,`
  api_facturas\Controllers\ClienteController.cs,`
  api_facturas\Modelos\Vendedor.cs, api_facturas\Peticiones\VendedorCrear.cs,`
  api_facturas\Peticiones\VendedorReemplazo.cs,`
  api_facturas\Peticiones\VendedorActualizar.cs,`
  api_facturas\Repositorios\IRepositorioVendedor.cs,`
  api_facturas\Repositorios\RepositorioVendedorSqlServer.cs,`
  api_facturas\Servicios\IServicioVendedor.cs,`
  api_facturas\Servicios\ServicioVendedor.cs,`
  api_facturas\Controllers\VendedorController.cs,`
  api_facturas\Modelos\Factura.cs, api_facturas\Modelos\ProductoDeFactura.cs,`
  api_facturas\Peticiones\FacturaCrear.cs,`
  api_facturas\Repositorios\IRepositorioFactura.cs,`
  api_facturas\Repositorios\RepositorioFacturaSqlServer.cs,`
  api_facturas\Servicios\IServicioFactura.cs,`
  api_facturas\Servicios\ServicioFactura.cs,`
  api_facturas\Controllers\FacturaController.cs,`
  api_facturas\Excepciones\ConflictoExcepcion.cs,`
  api_facturas\Modelos\RolUsuario.cs,`
  api_facturas\Peticiones\RolUsuarioCrear.cs,`
  api_facturas\Repositorios\IRepositorioRolUsuario.cs,`
  api_facturas\Repositorios\RepositorioRolUsuarioSqlServer.cs,`
  api_facturas\Servicios\IServicioRolUsuario.cs,`
  api_facturas\Servicios\ServicioRolUsuario.cs,`
  api_facturas\Controllers\RolUsuarioController.cs,`
  api_facturas\Modelos\RutaRol.cs, api_facturas\Peticiones\RutaRolCrear.cs,`
  api_facturas\Repositorios\IRepositorioRutaRol.cs,`
  api_facturas\Repositorios\RepositorioRutaRolSqlServer.cs,`
  api_facturas\Servicios\IServicioRutaRol.cs,`
  api_facturas\Servicios\ServicioRutaRol.cs,`
  api_facturas\Controllers\RutaRolController.cs,`
  api_facturas\Modelos\UsuarioConRoles.cs,`
  api_facturas\Peticiones\UsuarioConRolesCrear.cs,`
  api_facturas\Peticiones\UsuarioConRolesActualizar.cs,`
  api_facturas\Repositorios\IRepositorioUsuarioConRoles.cs,`
  api_facturas\Repositorios\RepositorioUsuarioConRolesSqlServer.cs,`
  api_facturas\Servicios\IServicioUsuarioConRoles.cs,`
  api_facturas\Servicios\ServicioUsuarioConRoles.cs,`
  api_facturas\Controllers\UsuarioConRolesController.cs
```

**4. Y los de la interfaz gráfica:**

```powershell
New-Item front_blazor\Modelos\Cliente.cs, front_blazor\Servicios\ServicioCliente.cs,`
  front_blazor\Modelos\Vendedor.cs,`
  front_blazor\Servicios\ServicioVendedor.cs,`
  front_blazor\Modelos\Factura.cs, front_blazor\Servicios\ServicioFactura.cs,`
  front_blazor\Modelos\RolUsuario.cs,`
  front_blazor\Servicios\ServicioRolUsuario.cs,`
  front_blazor\Modelos\RutaRol.cs, front_blazor\Servicios\ServicioRutaRol.cs,`
  front_blazor\Modelos\UsuarioConRoles.cs,`
  front_blazor\Servicios\ServicioUsuarioConRoles.cs,`
  front_blazor\Components\Pages\Clientes.razor,`
  front_blazor\Components\Pages\Vendedores.razor,`
  front_blazor\Components\Pages\Facturas.razor,`
  front_blazor\Components\Pages\EmitirFactura.razor,`
  front_blazor\Components\Pages\FacturaDetalle.razor,`
  front_blazor\Components\Pages\UsuariosYRoles.razor,`
  front_blazor\Components\Pages\RolesPorUsuario.razor,`
  front_blazor\Components\Pages\PermisosPorRol.razor
```

> **Son 49 archivos de API y 18 de front, y la cuenta sorprende.**
> Es la medida honesta de lo que son seis recursos con sus capas — y es
> exactamente el argumento del que nace la idea de generar código: cuando se
> repite tanto, hay un patrón, y el patrón se puede decir una vez.

**Dos archivos EXISTENTES crecen, y solo dos:**

| | |
|---|---|
| `api_facturas\Program.cs` | Suma el registro de los seis recursos |
| `front_blazor\Program.cs` | Suma los seis servicios del front |

(Y `front_blazor\Components\Layout\NavMenu.razor`, que suma las entradas del
menú.)

**Antes de abrir el chat, verifique:**

- [ ] `docs\spec_kit\versiones\v2_con_fk\` tiene **8** archivos `.md`.
- [ ] Su v1 **arranca y su prueba de humo pasa**.
- [ ] La base tiene las **12** tablas, los **procedimientos** y el
      **disparador** — no solo las seis de la v1.

```powershell
docker compose exec postgres sqlcmd -U postgres -d bdfacturas_sqlserver_local -c "\dt"
docker compose exec postgres sqlcmd -U postgres -d bdfacturas_sqlserver_local -c "\df"
```

### A.3 El prompt de la v2 — cópielo tal cual como PRIMER mensaje

Los tres chequeos previos son los de la v1: **adjuntos completos**, **modo
razonamiento ON**, **búsqueda web OFF**.

```
Actúa como mi asistente de programación para construir la VERSIÓN 2 de un
proyecto universitario. Te adjunto 8 documentos: la constitución (las reglas
permanentes) y el spec kit de la versión 2 (spec, plan, research, modelo de
datos, contratos, quickstart y tareas).

El proyecto es C# sobre ASP.NET Core (.NET 10) + SQL Server, con la interfaz
gráfica en Blazor Server — así lo fija 3_plan.md. Si en tu respuesta aparece
otro lenguaje o framework (Python, Java, Node, PHP, React…), significa que no
leíste los documentos: detente y dímelo en vez de continuar.

CONTEXTO — LAS VERSIONES SON ACUMULATIVAS:

Mi proyecto YA TIENE la versión 1 construida y funcionando: el CRUD completo
de las SEIS tablas SIN clave foránea (producto, empresa, persona, rol, ruta,
usuario), con su API y sus seis interfaces gráficas. Esa versión está CERRADA.

La v2 se construye ENCIMA. No reescribas nada de la v1, no la "mejores" y no
me vuelvas a entregar ninguno de esos seis recursos. Si para algo necesitas
ver mi código actual, pídemelo y te lo pego.

QUÉ CONSTRUYE LA VERSIÓN 2 — las SEIS tablas CON clave foránea:

  cliente              CRUD de 5 verbos. Dos claves foráneas: fkcodpersona
                       obligatoria, fkcodempresa OPCIONAL y nullable
  vendedor             CRUD de 5 verbos. Una clave foránea obligatoria
  factura +            CUATRO operaciones, no seis: listar, consultar, crear
  productosporfactura  y ANULAR. Todas por procedimiento almacenado
  rol_usuario          tabla puente: listar por los dos lados, agregar y
                       quitar con las DOS claves. Sin PUT ni PATCH
  rutarol              la otra tabla puente, igual

Y un recurso más, que no es una tabla nueva:

  usuario_con_roles    el usuario Y sus roles en UNA operación, con los cinco
                       procedimientos que la base ya trae. Ruta:
                       api/usuario-con-roles

Con la v2 están las 12 tablas de bdfacturas.

REGLAS DE TRABAJO (no negociables):

1. La especificación manda. No agregues NADA que los documentos no pidan: ni
   token ni login (eso es la v3), ni endpoints para editar o borrar
   físicamente una factura (no se exponen: la operación es ANULAR), ni
   fábricas, ni otro motor de base de datos (eso es la v5). Si crees que
   falta algo, pregúntame antes de escribirlo.

2. Vamos a seguir 8_tasks.md FASE POR FASE, en orden, las doce. En cada fase:
   a. Me explicas en 3-5 líneas qué vamos a hacer y por qué.
   b. Me entregas los archivos DE A UNO: la ruta exacta y el contenido
      COMPLETO de UN archivo, con los comentarios didácticos en español que
      exige la constitución. Esperas mi "listo" y sigues con el siguiente.
   c. Al cerrar la fase me das su comando de verificación y qué salida
      esperar.
   Los archivos nuevos YA EXISTEN VACÍOS en mi proyecto: no me des comandos
   para crearlos.

3. Los errores no nos frenan: te pego el error, me das el archivo completo
   corregido. Si no sale rápido, seguimos y lo retomamos en el cierre.

4. Cumple 6_contracts.md al pie de la letra. En particular:
   - La API NUNCA calcula subtotales, total ni stock: eso lo hacen los
     procedimientos y el disparador de la base.
   - Al crear una factura, el cuerpo NO lleva total ni subtotales ni fecha.
   - api/rol-usuario lleva GUION; api/rutarol NO lo lleva.
   - El 409 tiene tres causas: clave foránea inexistente, pareja repetida en
     una tabla puente, y factura ya anulada.
   - La contraseña vacía al editar un usuario significa "déjela como está".

5. CADA VERSIÓN ES API + INTERFAZ GRÁFICA. Las fases 9, 10 y 11 son del front
   en Blazor Server, y la versión no está cerrada sin ellas. En particular:
   - Las claves foráneas van como DESPLEGABLES cargados de la API: muestran
     el nombre, mandan el código. No como campos de texto.
   - El formulario de factura es UNO: se agregan y quitan renglones ANTES de
     guardar, y se envía UNA sola vez.
   - El formulario de usuario con roles usa CASILLAS, y también un solo envío.
   - El nombre de un archivo .razor es el nombre de una clase: no puede
     llamarse igual que un modelo, o no compila.

6. Todo en español: nombres, comentarios y mensajes.

7. Trabajo en Windows con VS Code (terminal PowerShell) y Docker Desktop. Dame
   los comandos para ese entorno, y usa curl.exe con la extensión (en
   PowerShell, curl pelado es otra cosa).

La versión 2 está TERMINADA solo cuando: (a) la prueba de humo de la V1 pasa
completa —la regresión, no rompimos nada— y (b) los 18 criterios de
aceptación de 2_spec.md §5 pasan con 7_quickstart.md, incluidos los ocho de
las interfaces gráficas.

Empieza: resume en máximo 10 líneas qué vamos a construir y sobre qué base
—para confirmar que entendiste que la v1 ya existe y no se toca— y arranca
con la Fase 0.
```

### A.4 El método de la conversación

1. **Pegue y diga "listo".** Un archivo por turno. La tentación de pedir
   "dame todos los de la fase" es la que llena el chat de código que no se
   revisó.
2. **Proteja la v1.** Si la IA le entrega un archivo de `producto` o de
   `persona` "mejorado", **no lo pegue**: *«eso es de la v1 cerrada, no se
   toca»*. Las únicas excepciones son los dos `Program.cs` y el menú.
3. **Cuando pida ver código de la v1, péguelo completo.** Típicamente
   `Program.cs`, un controlador y un repositorio: los necesita para calcar el
   estilo.
4. **Si responde en otro lenguaje o pierde el hilo, reinicie el chat** y
   vuelva a subir los 8 documentos. No insista en una conversación perdida.
5. **El cierre es doble y en ese orden:** primero la **regresión** de la v1,
   después la prueba de humo de la v2.

---

## Camino B — IDE agéntico

### B.1 Preparación

Abra el IDE **sobre su proyecto de la v1** —el que ya tiene código— y copie
antes la carpeta `v2_con_fk` de specs (paso A.2.1). **No hay que crear
archivos vacíos:** el agente los crea.

### B.2 El prompt para el agente — cópielo tal cual

```
Construye la VERSIÓN 2 de este proyecto.

LAS VERSIONES SON ACUMULATIVAS: este proyecto YA TIENE la v1 construida y
funcionando —el CRUD de las seis tablas SIN clave foránea (producto, empresa,
persona, rol, ruta, usuario) con su API y sus seis interfaces gráficas—. NO la
modifiques. Los únicos archivos existentes que crecen son
api_facturas/Program.cs, front_blazor/Program.cs y
front_blazor/Components/Layout/NavMenu.razor.

Primero LEE, en este orden: docs/spec_kit/1_constitution.md y los 8
documentos de docs/spec_kit/versiones/v2_con_fk/ (2_spec a 8_tasks). Puedes
leer el código de la v1 para calcar su estilo. docs/spec_kit/ es SOLO
LECTURA. La base de datos ya está completa en db/ desde la v1: no toques SQL.

Después resume en máximo 10 líneas qué vas a construir y sobre qué base, y
ESPERA MI CONFIRMACIÓN antes de tocar un solo archivo.

QUÉ CONSTRUYE LA VERSIÓN 2 — las seis tablas CON clave foránea: cliente,
vendedor, factura (+ productosporfactura), rol_usuario y rutarol. Más el
recurso usuario_con_roles, que opera usuario y rol_usuario juntas con los
cinco procedimientos de la base. Con la v2 están las 12 tablas.

REGLAS (no negociables):

1. La especificación manda: nada que los documentos no pidan. Ni token ni
   login (v3), ni editar/borrar físicamente una factura (no se exponen: la
   operación es ANULAR), ni fábricas ni otro motor (v5). Ante la duda,
   pregunta.

2. Sigue 8_tasks.md FASE POR FASE, las doce. Al cerrar cada fase EJECUTA su
   verificación, muéstrame el resultado REAL —no lo que esperabas— y espera
   mi OK antes de seguir.

3. Cumple 6_contracts.md al pie de la letra: la API nunca calcula
   subtotales/total/stock; el cuerpo de crear factura no los lleva;
   api/rol-usuario con guion y api/rutarol sin guion; el 409 con sus tres
   causas; la contraseña vacía al editar significa "déjela como está".

4. CADA VERSIÓN ES API + INTERFAZ GRÁFICA. Las fases 9, 10 y 11 son del front
   en Blazor Server y la versión no cierra sin ellas: desplegables cargados
   de la API, el formulario de factura con UN solo envío, las casillas de
   roles con UN solo envío. Y el nombre de un .razor no puede chocar con el
   de un modelo.

5. Todo en español, con comentarios didácticos.

6. Un commit por fase, con mensaje en español que diga qué se construyó.

7. Cierre doble: primero la regresión de la v1 (su quickstart completo),
   después los 18 criterios de 7_quickstart.md —incluidos los ocho del
   navegador—. Con evidencia.
```

### B.3 Cómo supervisar al agente

| Alarma | Qué hacer |
|---|---|
| **El diff toca un archivo de la v1** (distinto de los dos `Program.cs` y el menú) | Recházelo. La v1 está cerrada |
| **Aparece un `SELECT` en `RepositorioFacturaSqlServer.cs`** | Recházelo: factura es solo procedimientos |
| **El front manda `total`** | Recházelo. Lo pone el disparador |
| **Dice «listo» sin haber ejecutado la verificación** | Pídale la salida real. «Debería funcionar» no es una verificación |
| **Hace las doce fases de un tirón** | Párelo. El valor del método está en verificar cada fase |
| **Agrega un paquete de NuGet** | Pregúntele dónde lo pide la spec. Si no lo pide, fuera |

---

## Por qué así — la lección de la v2

En la v1 la lección era **dirigir a una IA desde cero** con una
especificación. En la v2 es la de la vida real: **casi nunca se parte de
cero**.

| | |
|---|---|
| **Por eso el prompt protege la v1** | Lo que ya funciona tiene dueño: su spec cerrada |
| **Por eso el cierre empieza por la regresión** | Lo primero que hay que saber es si se rompió algo |
| **Por eso la spec de la v2 solo describe el DELTA** | Lo acumulado está especificado en otra parte, y repetirlo es garantizar que un día las dos copias no coincidan |

> **Y la trampa de esta versión, que vale la pena decir:** los cuatro errores
> más probables de la v2 —el sobre deserializado mal, el alias que falta en
> Dapper, el `JsonPropertyName` olvidado, la cadena vacía en vez de `null`—
> **fallan en silencio**. No hay excepción ni error en el log: hay un dato
> equivocado, con HTTP 200.
>
> Por eso el cierre se hace **mirando la interfaz gráfica**, y no solo leyendo
> respuestas de la API. Una IA que no ejecuta la verificación no puede
> encontrarlos, y usted tampoco si no abre el navegador.
