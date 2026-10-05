# Versión 2 — Paco · camino A (chat web)

> **Su parte:** los recursos **`cliente`** y **`vendedor`**.
>
> **Su herramienta en esta versión:** un chat web. Desde la **v4** usted pasa a
> agente, como los otros dos — [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3.
>
> **No empiece hasta que el PR de Carlos esté fusionado.**

---

## 1. Lo suyo es el molde de la v1 **más una cosa nueva**

```
producto   codigo (texto, PK)  · nombre · stock · valorunitario     ← v1
cliente    id (IDENTITY, PK)   · credito · fkcodpersona · fkcodempresa
vendedor   id (IDENTITY, PK)   · carnet · direccion · fkcodpersona
```

Las seis piezas siguen siendo las mismas —modelo, tres peticiones por verbo,
repositorio, servicio, controlador, pantalla—. Lo nuevo es **la clave foránea**,
y trae consigo dos cosas que la v1 no tenía: **el 409** y **el desplegable**.

> **Y hay una herencia de la v1 que ya conoce:** sus dos tablas tienen llave
> **`IDENTITY`**, como las de Luis en la v1. La petición de crear **no lleva
> `id`** — lo genera la base. Si el chat se lo pone, quítelo.

---

## 2. `cliente` tiene DOS claves foráneas, y no son iguales

**Ésta es la pieza que define su parte.**

| Columna | | Qué significa |
|---|---|---|
| `fkcodpersona` | `NOT NULL` | **Todo cliente es una persona.** Sin excepción |
| `fkcodempresa` | **admite nulo** | **Hay clientes sin empresa** |

`vendedor`, en cambio, tiene **una sola** y es obligatoria.

### Y de ese «admite nulo» sale el tropiezo más caro de la versión

Un desplegable vacío en HTML **no manda `null`: manda `""`**. Y no son lo mismo:

| Lo que el front manda | Qué responde la API |
|---|---|
| `""` | **422** · *«El campo fkcodempresa debe tener entre 1 y 10 caracteres»* |
| `null` | **200** · *«Cliente creado exitosamente»* |

> **La corrección es una línea en el front —convertir `""` a `null` antes de
> mandar—, y encontrarla cuesta una tarde** si uno no sabe que ese 422 puede
> venir de ahí.

> **Y una advertencia sobre el `3_plan.md` de esta versión**, que escribió Luis:
> dice que la cadena vacía daría **409**. **La API responde 422**, porque la
> anotación `[StringLength(10, MinimumLength = 1)]` la atrapa **antes** de que la
> base se entere. Las dos respuestas son defendibles y la que da es la mejor: se
> rechaza sin consultar nada.
>
> **No "arregle" el código para que dé 409.** Si algo hay que corregir es el
> documento — y eso se habla en la reunión, no en su rama.

---

## 3. El 409, que es el código nuevo de la versión

Alguien manda `{"fkcodpersona": "P999"}`, y esa persona no existe.

> **Responde 409, no 422.** El dato **no está mal**: `"P999"` es un texto de la
> longitud permitida. Lo que se rompe es el **estado** de la base: esa fila no
> está. Y saberlo **exige ir a la base**, así que ya no es un problema de forma.

| | Lo rechaza | Sin consultar la base |
|---|---|---|
| **422** | la petición, por las anotaciones | **sí** |
| **409** | la base, y el controlador lo traduce | **no** |

> **Es la pregunta de sustentación más probable de su parte**, y está razonada
> en el [`4_research.md`](4_research.md) de esta versión — que lo escribió usted.

---

## 4. Antes de abrir el chat

```powershell
# 1 · TRAIGA LO DE CARLOS. Usted necesita ver cómo quedó factura, aunque no
#     la toque: es el recurso que rompe el molde y conviene saber en qué.
git switch main
git pull origin main

# 2 · Compruebe que llegó. Si no está, PARE: Carlos no ha fusionado.
Get-ChildItem api_facturas\Controllers\FacturaController.cs

# 3 · Su rama.
git switch -c rama-paco-v2

# 4 · SU IDENTIDAD, parado en la carpeta del proyecto. Y compruébela.
git config user.name "ccastro2050-50"
git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

---

## 5. Qué subirle al chat

| # | Archivo | Para qué |
|---|---|---|
| 1 | [`1_constitution.md`](../../1_constitution.md) | Las reglas permanentes |
| 2 | [`2_spec.md`](2_spec.md) | QUÉ construir — lo escribió usted |
| 3 | [`5_data_model.md`](5_data_model.md) | Los campos y **cuál clave foránea admite nulo** |
| 4 | [`6_contracts.md`](6_contracts.md) | Los endpoints exactos |
| 5 | [`4_research.md`](4_research.md) | **Por qué el 409 y no el 422** |
| 6–11 | **Los archivos de un recurso de la v1** — `persona`, el suyo | El molde de las seis capas |
| 12 | **Un archivo donde ya se traduzca el 409** | Para que calque la traducción en vez de inventarla |

> **El 12 es el que marca la diferencia.** Si el chat no ve cómo se traduce el
> error del motor, se lo va a inventar — y probablemente lo capture en el
> servicio, que es justo donde no va.

**No suba** `9_checklist.md`, `0_mapa_versiones.md` ni `db/bdfacturas.sql`.

**Tres chequeos antes del primer mensaje:** adjuntos completos, **modo de
razonamiento encendido**, **búsqueda web apagada**.

---

## 6. El prompt (cópielo tal cual como PRIMER mensaje)

```
Actúa como mi asistente de programación en un proyecto universitario que
hacemos TRES personas. El proyecto YA EXISTE: tiene una versión 1 completa
con seis recursos funcionando, y un compañero acaba de agregar `factura`.
Mi trabajo es agregar DOS recursos más, calcando la estructura que ya está.

Te adjunto los documentos del spec kit y los archivos de un recurso que ya
funciona, que es el MOLDE. Es C# sobre ASP.NET Core (.NET 10) con SQL
Server y Dapper — nunca un ORM de entidades.

LO QUE TIENES QUE ESCRIBIR, Y SON DOS RECURSOS:

   cliente    id (INT IDENTITY, PK) · credito (decimal) ·
              fkcodpersona (texto, OBLIGATORIO) ·
              fkcodempresa (texto, OPCIONAL - admite nulo)

   vendedor   id (INT IDENTITY, PK) · carnet (entero) ·
              direccion (texto) · fkcodpersona (texto, OBLIGATORIO)

Cada uno con las SEIS piezas del molde: modelo, las TRES peticiones por
verbo, interfaz + repositorio, interfaz + servicio, controlador con los
CINCO verbos, y su pantalla.

TRES COSAS QUE NO SE CALCAN DEL MOLDE DE LA V1:

  1. LA LLAVE ES IDENTITY: la genera la base. La petición de CREAR NO
     lleva `id`, y el modelo NO lo marca como `required`.

  2. LAS CLAVES FORÁNEAS. En cliente son DOS y NO son iguales:
     fkcodpersona es OBLIGATORIA y fkcodempresa ADMITE NULO. En la
     petición, fkcodempresa es opcional; si llega, tiene entre 1 y 10
     caracteres. En vendedor hay una sola y es obligatoria.

  3. UN CÓDIGO HTTP NUEVO: el 409. Si la clave foránea apunta a una fila
     que no existe, la base rechaza la operación y hay que traducir ese
     error a 409 — NO a 422. Razón: el dato tiene la FORMA correcta, lo
     que falla es el ESTADO de la base, y saberlo exige consultarla.
     La traducción va EN EL CONTROLADOR, no en el servicio: el servicio
     no puede nombrar nada de HTTP.

LA INTERFAZ GRÁFICA, y aquí hay algo nuevo:

  Las claves foráneas se escogen en un DESPLEGABLE cargado de la API,
  NUNCA en un campo de texto. El de empresa, además, lleva una opción
  vacía porque es opcional.

  Y CUIDADO CON ESTO: un desplegable vacío en HTML manda "" (cadena
  vacía), NO null. Son cosas distintas: "" es un código que no cumple la
  longitud mínima y da 422. Convierte "" a null ANTES de mandar la
  petición.

  Los desplegables se cargan con Task.WhenAll, NO uno tras otro: si la
  API está caída, cinco esperas en fila son cincuenta segundos en blanco.

REGLAS QUE SIGUEN VIGENTES:

  · Las TRES CAPAS con interfaces. El controlador no escribe SQL; el
    servicio NO nombra nada de HTTP —ni StatusCode, ni NotFound, ni
    IActionResult—; el repositorio no decide códigos de estado.
  · SIN ORM. SQL a mano con Dapper, siempre parametrizado.
  · TODO EN ESPAÑOL.
  · El front NO habla con la base: solo con la API por HTTP.
  · NO toques la base de datos: las tablas ya existen. Nada de CREATE
    TABLE.
  · NO toques `factura` ni las tablas puente: son de mis compañeros.

COMENTA TODO LO QUE ESCRIBAS, en español. Cada archivo empieza diciendo
QUÉ ES y QUÉ PAPEL cumple. Los comentarios dicen POR QUÉ está escrito
así, no QUÉ hace la línea.

EMPIEZA POR `vendedor`, que tiene UNA SOLA clave foránea y es el más
simple. Entrégamelo archivo por archivo, y NO sigas con `cliente` hasta
que yo te diga que vendedor ya me funcionó.
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


> **Y fíjese en el orden que pide el prompt: `vendedor` primero.** Tiene una
> clave foránea obligatoria y nada más. `cliente` trae la opcional, que es donde
> está el problema del `""`. Aprenda el caso simple antes de pelear con el raro.

---

## 7. El método, y lo que hay que vigilar

| Haga esto | No haga esto |
|---|---|
| Un archivo, pegarlo, **compilar**, seguir | Pedir los doce de una |
| Cuando falle, **pegarle el error literal** | Decirle «no funciona» |
| Leer los comentarios y corregir los falsos | Aceptarlos porque suenan bien |

> **Vigile dos archivos en concreto:**
>
> | Archivo | Qué mirar |
> |---|---|
> | `ClienteCrear.cs` | Que **no tenga `id`**, y que `fkcodempresa` **no sea `[Required]`** |
> | `ServicioCliente.cs` | Que **no nombre HTTP**. Si tiene un `409` ahí, está en la capa equivocada |

---

## 8. Comprobar lo suyo antes del PR

```powershell
# 1 · El 409 de clave foránea inexistente.
Invoke-RestMethod http://localhost:8035/api/cliente -Method Post `
  -ContentType 'application/json' -Body '{"fkcodpersona":"P999","credito":100}'
#    espera: 409

# 2 · Un cliente SIN empresa — la clave foránea opcional.
Invoke-RestMethod http://localhost:8035/api/cliente -Method Post `
  -ContentType 'application/json' `
  -Body '{"fkcodpersona":"P001","fkcodempresa":null,"credito":100}'
#    espera: 200

# 3 · Y con cadena vacía, que es el tropiezo.
Invoke-RestMethod http://localhost:8035/api/cliente -Method Post `
  -ContentType 'application/json' `
  -Body '{"fkcodpersona":"P001","fkcodempresa":"","credito":100}'
#    espera: 422 — y por eso el front convierte "" a null

# 4 · Crear SIN mandar id: la base lo genera.
Invoke-RestMethod http://localhost:8035/api/vendedor -Method Post `
  -ContentType 'application/json' `
  -Body '{"carnet":999,"direccion":"Prueba Paco","fkcodpersona":"P001"}'

# 5 · El mismo cuerpo, dos verbos: PUT 422, PATCH 200.

# 6 · Borre SUS filas de prueba. Nunca toque las sembradas.
```

```powershell
# 7 · Y la pantalla, que es donde fallan en silencio los tropiezos 1 y 2:
#     abra /clientes, cree uno CON empresa y otro SIN empresa.
Start-Process http://localhost:8099/clientes
```

> **El paso 7 no se puede saltar.** Los tropiezos del sobre y del mapeo de
> Dapper **no lanzan excepción**: responden 200 con un dato equivocado. Una
> prueba que solo mira el código de estado los da por buenos.

```powershell
# El servicio no nombra HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\ServicioCliente.cs,api_facturas\Servicios\ServicioVendedor.cs `
  -Pattern 'StatusCode|NotFound|IActionResult' | Measure-Object | Select-Object Count
```

---

## 9. Subir y abrir el PR

```powershell
git status                      # SOLO sus archivos
git add api_facturas/Modelos/Vendedor.cs api_facturas/Peticiones/Vendedor*.cs
git commit -m "feat: vendedor, el modelo y sus peticiones (clave foranea obligatoria)"
#   ... capa por capa, y luego cliente
git push -u origin rama-paco-v2
#   y el Pull Request hacia main. Carlos revisa y fusiona; usted NO.
```

> **En la descripción del PR diga cómo resolvió el `""` del desplegable.** Es lo
> que Carlos va a mirar primero, y es lo que más se equivoca.

> **Nadie toca el archivo de otro, ni para arreglárselo.**
