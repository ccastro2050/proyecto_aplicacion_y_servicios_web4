# Versión 1 — Paco · camino A (chat web)

> **Su parte:** los recursos **`persona`** y **`empresa`**.
>
> **Su herramienta:** un chat web — DeepSeek, Gemini, ChatGPT. Usted le sube
> archivos, él le devuelve código, y **usted lo pega en su proyecto**.
>
> **No empiece hasta que el PR de Carlos esté fusionado.** Usted no construye de
> cero: **calca un molde que todavía no existe**. Ver [`GUIA_IA1.md`](GUIA_IA1.md) §2.

---

## 1. Qué significa «calcar el molde», y por qué es lo que hay que aprender

Sus dos recursos son **idénticos** al `producto` de Carlos salvo los campos:

```
producto   codigo · nombre · stock · valorunitario     ← el molde de Carlos
persona    codigo · nombre · email · telefono          ← suyo
empresa    codigo · nombre                             ← suyo
```

Los tres tienen **llave primaria de texto**, los tres se operan igual, los tres
pasan por las mismas seis piezas: modelo, tres peticiones por verbo, interfaz +
repositorio, interfaz + servicio, controlador, pantalla.

> **Y que se repitan ES el punto del ejercicio, no un desperdicio.** En algún
> momento mientras calca el segundo le va a dar rabia y va a pensar: *«¿y si
> hago una clase genérica y me ahorro esto?»*. **Esa pregunta es el objetivo de
> la versión 1.** La respuesta está en el Artículo 10 de la constitución — y
> solo significa algo si antes sintió las ganas de hacer el genérico.

> **Lo que se aprende aquí no es escribir CRUD: es reconocer un patrón y
> repetirlo sin desviarse.** Si su `persona` queda con los nombres en otro
> orden, o con una capa saltada «porque era más corto», el proyecto deja de
> tener una arquitectura y pasa a tener tres.

---

## 2. Antes de abrir el chat

```powershell
# 1 · TRAIGA EL MOLDE. Esto es lo que lo diferencia de Carlos: usted necesita
#     que el trabajo de él YA esté en main.
#     QUÉ HACE: baja a su computador lo que Carlos fusionó.
git switch main
git pull origin main

# 2 · COMPRUEBE que el molde llegó. Si esta carpeta está vacía, PARE:
#     Carlos todavía no ha fusionado y usted no tiene qué calcar.
Get-ChildItem api_facturas\Controllers\

# 3 · SU RAMA.
git switch -c rama-paco-v1

# 4 · SU IDENTIDAD. Parado en LA CARPETA DEL PROYECTO —la del git clone—,
#     porque ahi es donde Git guarda con que nombre firma (.git\config).
git config user.name "ccastro2050-50"
git config user.email "su-correo-de-github"

# 5 · COMPRUEBELO SIEMPRE, aunque crea que ya estaba. Si sale el nombre de
#     Carlos, no siga: sus commits se le acreditarian a el.
git config user.name
git config user.email
```

> **El paso 5 es el más importante de esta página.** Si su chat le entrega un
> código perfecto y usted lo sube firmado con la cuenta de Carlos, para la
> calificación **usted no hizo nada** — el historial va a decir que esos commits
> son de él. Y no se arregla después sin reescribir el historial entero.
>
**¿Y si usted trabaja siempre en su propio computador?** Entonces
> `git config --global` le serviría igual y es más cómodo. **No está mal.** Lo
> que está mal es **confiar en ella sin mirarla**: el día que use el PC de la
> universidad, la configuración global es la del que se sentó antes, y sus
> commits salen firmados con el nombre de otro **sin un solo aviso**.
>
> **Lo que protege no es dónde esté la configuración: es comprobarla** antes del
> primer commit de cada carpeta. Los tres casos, en
> [`GUIA_IA1.md`](GUIA_IA1.md) §4.

---

## 3. Qué subirle al chat

Usted sube **dos cosas**: los documentos que mandan, y el molde que va a calcar.

### Los documentos del spec kit

| # | Archivo | Para qué le sirve al chat |
|---|---|---|
| 1 | `docs/spec_kit/1_constitution.md` | Las reglas permanentes: C#, capas, español |
| 2 | `.../v1_sin_fk/2_spec.md` | QUÉ construir y los criterios |
| 3 | `.../v1_sin_fk/3_plan.md` | Las carpetas y las capas |
| 4 | `.../v1_sin_fk/5_data_model.md` | Los campos exactos de sus dos tablas |
| 5 | `.../v1_sin_fk/6_contracts.md` | Los endpoints exactos |

### Y el molde: los seis archivos de `producto` que hizo Carlos

| # | Archivo |
|---|---|
| 6 | `api_facturas/Modelos/Producto.cs` |
| 7 | `api_facturas/Peticiones/ProductoCrear.cs`, `ProductoReemplazo.cs`, `ProductoActualizar.cs` |
| 8 | `api_facturas/Repositorios/IRepositorioProducto.cs` y `RepositorioProductoSqlServer.cs` |
| 9 | `api_facturas/Servicios/IServicioProducto.cs` y `ServicioProducto.cs` |
| 10 | `api_facturas/Controllers/ProductoController.cs` |
| 11 | La pantalla de productos del front |

> **Los del molde son los más importantes de los once**, y es contraintuitivo.
> Sin ellos el chat se inventa una estructura razonable — y razonable no basta:
> tiene que ser **la misma** que la de sus compañeros. Con el molde delante, el
> chat ya no diseña, **calca**, que es exactamente lo que usted necesita.

> **No suba `9_checklist.md`** —es su compuerta, no la de la IA— ni
> `0_mapa_versiones.md` —le revelaría lo que viene, y la v1 no anticipa—.

---

## 4. Antes de enviar el primer mensaje, tres chequeos

1. **Los adjuntos están todos.** Deslice el carrusel: si falta el molde, el chat
   va a inventar.
2. **Active el modo de razonamiento** si el chat lo tiene — en DeepSeek se llama
   *Pensamiento Profundo*. Sigue mucho mejor reglas estrictas como éstas.
3. **Apague la búsqueda web.** No hace falta y puede traerle código de internet
   que no sigue el molde.

---

## 5. El prompt (cópielo tal cual como PRIMER mensaje)

```
Actúa como mi asistente de programación en un proyecto universitario que
estamos haciendo TRES personas. El proyecto YA ESTÁ MONTADO por un
compañero: la estructura, Docker, Swagger y un recurso completo que
funciona. Mi trabajo NO es diseñar nada: es CALCAR ese recurso para dos
tablas más.

Te adjunto 5 documentos del spec kit y los archivos del recurso `producto`,
que es el MOLDE. El proyecto es C# sobre ASP.NET Core (.NET 10) con SQL
Server, y usa Dapper — nunca un ORM de entidades.

LO QUE TIENES QUE ESCRIBIR, Y SON DOS RECURSOS:

   persona    codigo (texto, PK) · nombre · email · telefono
   empresa    codigo (texto, PK) · nombre

Cada uno con las SEIS piezas que tiene `producto`, en el mismo orden y con
los mismos nombres de archivo cambiando solo el del recurso:

   1. El modelo (la entidad).
   2. Las TRES peticiones por verbo: Crear, Reemplazo y Actualizar.
      Crear y Reemplazo llevan [Required] en todos los campos.
      Actualizar NO lleva [Required] en ninguno: lo que no se manda, no
      se toca.
   3. La interfaz del repositorio y su implementación para SQL Server,
      con el SQL escrito a mano y SIEMPRE parametrizado.
   4. La interfaz del servicio y su implementación.
   5. El controlador, con los CINCO verbos: GET (listar), GET por clave,
      POST, PUT, PATCH y DELETE.
   6. La pantalla del front para cada recurso, con su propia dirección
      (/personas y /empresas), calcada de la de productos.

CALCA, NO REDISEÑES. Usa exactamente la misma estructura de capas, los
mismos nombres de métodos, el mismo formato de respuesta y el mismo manejo
de errores que el molde. Si ves algo que harías distinto, NO lo cambies:
dímelo en un comentario aparte al final, pero entrégame el código calcado.

REGLAS QUE NO SE NEGOCIAN:

  · Las TRES CAPAS: controlador → servicio → repositorio, cada una
    dependiendo de una INTERFAZ y no de una clase.
  · El controlador NO escribe SQL. El servicio NO nombra nada de HTTP:
    ni StatusCode, ni NotFound, ni IActionResult. El repositorio no
    decide códigos de estado.
  · SIN ORM de entidades. SQL a mano con Dapper, siempre parametrizado.
  · TODO EN ESPAÑOL: nombres, comentarios y mensajes.
  · El front NO habla con la base de datos, solo con la API por HTTP.
  · NO toques la base de datos: el script ya existe y las tablas ya están
    creadas. No escribas CREATE TABLE.

COMENTA TODO LO QUE ESCRIBAS, en español. Cada archivo empieza con un
bloque que dice QUÉ ES y QUÉ PAPEL cumple en la arquitectura. Cada método
no evidente lleva su comentario. Y los comentarios dicen POR QUÉ está
escrito así, no QUÉ hace la línea.

EMPIEZA POR `persona`, entrégamelo archivo por archivo, y NO sigas con
`empresa` hasta que yo te diga que persona ya me funcionó.
```

---

## 6. El método de la conversación

**La última línea del prompt es la regla más importante de todas:** un archivo a
la vez, y `empresa` solo cuando `persona` funcione.

| Haga esto | No haga esto |
|---|---|
| Pedir un archivo, pegarlo, **compilar**, seguir | Pedir los doce archivos de una y pegarlos todos |
| Cuando algo falle, **pegarle el error literal** | Decirle «no funciona» |
| Leer los comentarios y corregir los que estén mal | Aceptarlos porque suenan bien |

> **Por qué no pedirlo todo de una vez.** Porque si el archivo 3 está mal, los
> archivos 4 a 12 están construidos encima de ese error — y usted no va a saber
> dónde empezó. Pegando uno, compilando y siguiendo, el error aparece **en el
> archivo que lo causó**.

> **Y sobre los comentarios que la IA escribe:** a veces comenta lo que *cree*
> que hace el código, no lo que hace. Léalos uno por uno. Un comentario
> equivocado es peor que ninguno, porque el que lo lea después le va a creer. Y
> desde la v2 **la interpretabilidad se califica hablando**: le van a pedir que
> explique, en voz alta, por qué su código está así.

---

## 7. Comprobar lo suyo antes del PR

```powershell
# 1 · Levantar el sistema.
docker compose up -d --build

# 2 · ¿Sus dos recursos aparecen en Swagger? Deben salir /api/persona y
#     /api/empresa con sus seis operaciones cada uno.
Start-Process http://localhost:8035/swagger

# 3 · Cree SU propia fila de prueba —nunca toque las sembradas—.
$nueva = @{ codigo='PTEST'; nombre='Prueba Paco'; email='p@p.com'; telefono='300' }
Invoke-RestMethod http://localhost:8035/api/persona -Method Post `
  -ContentType 'application/json' -Body ($nueva | ConvertTo-Json)

# 4 · LA COMPROBACIÓN QUE HAY QUE SABER EXPLICAR: el mismo cuerpo, dos verbos.
#     El PUT debe dar 422 —le faltan campos— y el PATCH debe dar 200.
Invoke-RestMethod http://localhost:8035/api/persona/PTEST -Method Put `
  -ContentType 'application/json' -Body '{"nombre":"Cambiado"}'

Invoke-RestMethod http://localhost:8035/api/persona/PTEST -Method Patch `
  -ContentType 'application/json' -Body '{"nombre":"Cambiado"}'

# 5 · Borre su fila.
Invoke-RestMethod http://localhost:8035/api/persona/PTEST -Method Delete

# 6 · Y las pantallas: abra /personas y /empresas y cree un registro desde ahí.
Start-Process http://localhost:8099/personas
```

> **El paso 4 es la pregunta de sustentación más probable de esta versión.**
> Si el PUT y el PATCH se comportan igual, su chat le entregó **una sola clase
> de petición para los dos verbos**, y eso está mal: son `PersonaReemplazo` —con
> `[Required]`— y `PersonaActualizar` —sin ninguno—. La diferencia entre las dos
> ES la diferencia entre reemplazar y actualizar.

### Y lo que nunca debe aparecer en su código

```powershell
# El servicio no nombra HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\ServicioPersona.cs,api_facturas\Servicios\ServicioEmpresa.cs `
  -Pattern 'StatusCode|NotFound|IActionResult' | Measure-Object | Select-Object Count
```

---

## 8. Subir y abrir el PR

```powershell
# 1 · Mire QUÉ va a subir. SOLO deben aparecer SUS archivos.
#     Si aparece algo de Carlos o de Luis, pare: tocó lo que no era.
git status

# 2 · Commits pequeños, uno por pieza, con mensaje de verdad.
git add api_facturas/Modelos/Persona.cs api_facturas/Peticiones/Persona*.cs
git commit -m "feat: persona, el modelo y sus tres peticiones por verbo"
#    ... capa por capa, y luego empresa.

# 3 · Suba su rama.
git push -u origin rama-paco-v1

# 4 · Abra el Pull Request hacia main y describa QUÉ construyó.
#     Carlos lo revisa y lo fusiona. Usted NO fusiona.
```

> **El paso 1 es el que evita el único conflicto posible de esta versión.**
> Si en `git status` aparece un archivo que no es suyo, es que usted lo editó
> —o se lo pidió al chat— y al fusionar va a chocar con el de su dueño.
> **Nadie toca el archivo de otro, ni para arreglárselo.** Si vio un error en el
> código de Luis, dígaselo: no lo corrija en su rama.
