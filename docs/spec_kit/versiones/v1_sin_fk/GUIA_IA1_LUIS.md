# Versión 1 — Luis · camino A (chat web)

> **Su parte:** los recursos **`rol`** y **`ruta`**.
>
> **Su herramienta en esta versión:** un chat web — DeepSeek, Gemini, ChatGPT.
> Usted le sube archivos, él le devuelve código, y **usted lo pega en su
> proyecto**.
>
> **Chat en la v1, la v2 y la v3; desde la v4 usted pasa a agente**, como los
> otros dos. Está acordado en
> [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3, y el orden no
> es casual: **un agente lee el repositorio, y en la v1 no hay nada que leer**.
> Las tres primeras versiones a mano son las que le van a permitir, en la v4,
> **juzgar lo que el agente escriba** en vez de aceptarlo.
>
> **No empiece hasta que el PR de Carlos esté fusionado:** usted calca un molde
> que todavía no existe. Ver [`GUIA_IA1.md`](GUIA_IA1.md) §3.
>
> **Y lea la §2 antes del prompt.** Sus dos tablas **no se calcan igual** que las
> de sus compañeros, y si no sabe por qué, el chat le va a entregar código que
> compila y no funciona.
>
> **Ojo: no es que las tablas tengan truco.** El `IDENTITY` y el `UNIQUE` son
> decisiones correctas de un esquema que viene dado. Lo que falla es **el calco**:
> la IA copia bien un molde que ahí no aplica. El problema está en el copiado,
> no en las tablas.

---

## 1. Lo suyo es calcar… con dos excepciones

Sus recursos salen del mismo molde que todo lo demás —`producto`, hecho por
Carlos—, con las mismas seis piezas: modelo, tres peticiones por verbo, interfaz
+ repositorio, interfaz + servicio, controlador, pantalla.

```
producto   codigo (texto, PK)  · nombre · stock · valorunitario   ← el molde
rol        id (IDENTITY, PK)   · nombre
ruta       id (IDENTITY, PK)   · ruta · descripcion
```

Mire la diferencia: `producto` tiene una llave **de texto que usted escribe**;
las suyas tienen una llave **numérica que pone la base de datos**. De ahí salen las dos
diferencias de la §2.

---

## 2. Las dos cosas que NO se calcan

**Léalas ahora.** Las dos están puestas en el prompt, pero usted tiene que saber
reconocerlas cuando el chat se equivoque — porque se va a equivocar, y va a
equivocarse **precisamente por calcar bien**.

### 1 · La llave la pone la base de datos, no usted

`id INT IDENTITY(1,1)` significa que **SQL Server genera el número**. Y eso
cambia dos archivos respecto al molde:

| | En `producto` (llave de texto) | En `rol` y `ruta` (llave IDENTITY) |
|---|---|---|
| La petición de **crear** | lleva `codigo` | **NO lleva `id`** |
| El modelo | `required string Codigo` | `int Id`, **sin `required`** |

> **Qué pasa si el chat lo calca tal cual, que es lo que va a hacer.** Le va a
> poner `[Required] public int? Id` en `RolCrear`, porque así está en
> `ProductoCrear`. Y entonces el `POST` **exige que usted le invente una llave a
> la base de datos** — que es justo lo que la base de datos existe para evitar. El resultado es un
> 422 pidiéndole un campo que nadie debería mandar.
>
> **Y si lo deja pasar, el error sobrevive:** compila, Swagger lo muestra, y solo
> se descubre cuando alguien intenta crear un rol.

### 2 · `ruta` no admite nombres repetidos

La tabla `ruta` tiene una restricción que `rol` no tiene:

```sql
CONSTRAINT uq_ruta UNIQUE (ruta)
```

Es decir: **no puede haber dos filas con el mismo nombre de interfaz**. Y tiene
sentido — si `interfaz.facturas` existiera dos veces, el control de acceso no
sabría cuál mirar.

> **Y esto es lo raro, que hay que entender en vez de «arreglar»:** cuando usted
> intente crear una ruta repetida, la API va a responder **500**, no un error
> bonito. **Está bien así, y es a propósito.**
>
> En la v1, **la llave la defiende la base de datos, no la API**. Convertir ese choque en
> un 409 con mensaje claro es **la lección de la v2**, y adelantarla rompe el
> Artículo 1 (YAGNI). Está declarado en el `2_spec.md` de esta versión.
>
> **Si su chat le propone capturar la excepción y devolver 409, dígale que no.**
> Y acuérdese de por qué: ver ese 500 sin traducir una vez es lo que hace que el
> 409 de la v2 signifique algo.

---

## 3. Antes de abrir el chat

```powershell
# 1 · TRAIGA EL MOLDE. Usted necesita que el trabajo de Carlos YA esté en main.
git switch main
git pull origin main

# 2 · COMPRUEBE que llegó. Si esta carpeta está vacía, PARE: Carlos no ha
#     fusionado y usted no tiene qué calcar.
Get-ChildItem api_facturas\Controllers\

# 3 · SU RAMA.
git switch -c rama-luis-v1

# 4 · SU IDENTIDAD. Parado en LA CARPETA DEL PROYECTO —la del git clone—,
#     porque ahi es donde Git guarda con que nombre firma (.git\config).
git config user.name "ccastro202050"
git config user.email "su-correo-de-github"

# 5 · COMPRUEBELO SIEMPRE, aunque crea que ya estaba. Si sale el nombre de
#     otro, no siga: sus commits se le acreditarian a el.
git config user.name
git config user.email
```

> **El paso 5 decide su nota, literalmente.** Si sube un código perfecto firmado
> con la cuenta de un compañero, para la calificación **usted no hizo nada** — y
> no se arregla después sin reescribir el historial.
>
**¿Y si usted trabaja siempre en su propio computador?** Entonces
> `git config --global` le serviría igual y es más cómodo. **No está mal.** Lo
> que está mal es **confiar en ella sin mirarla**: el día que use el PC de la
> universidad, la configuración global es la del que se sentó antes, y sus
> commits salen firmados con el nombre de otro **sin un solo aviso**.
>
> **Lo que protege no es dónde esté la configuración: es comprobarla** antes del
> primer commit de cada carpeta. Los tres casos, en
> [`GUIA_IA1.md`](GUIA_IA1.md) §5.

---

## 4. Qué subirle al chat

Dos cosas: los documentos que mandan, y **el molde que va a calcar**.

| # | Archivo | Para qué |
|---|---|---|
| 1 | `docs/spec_kit/1_constitution.md` | Las reglas permanentes |
| 2 | `.../v1_sin_fk/2_spec.md` | QUÉ construir y los criterios |
| 3 | `.../v1_sin_fk/3_plan.md` | Las carpetas y las capas |
| 4 | `.../v1_sin_fk/5_data_model.md` | **Los campos de sus dos tablas, con el IDENTITY** |
| 5 | `.../v1_sin_fk/6_contracts.md` | Los endpoints exactos |
| 6–11 | **Los archivos de `producto` que hizo Carlos** | El molde: modelo, las 3 peticiones, repositorio, servicio, controlador y pantalla |

> **Los del molde son los más importantes**, aunque no lo parezca. Sin ellos el
> chat se inventa una estructura razonable — y razonable no basta: tiene que ser
> **la misma** que la de sus compañeros, o el proyecto termina con tres
> arquitecturas.

> **No suba `9_checklist.md`** —es su compuerta, no la de la IA— ni
> `0_mapa_versiones.md` —le revelaría lo que viene, y la v1 no anticipa—.

**Tres chequeos antes del primer mensaje:** que los adjuntos estén todos, que el
**modo de razonamiento** esté encendido (en DeepSeek, *Pensamiento Profundo*), y
que la **búsqueda web** esté apagada.

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

   rol     id (INT IDENTITY, PK) · nombre
   ruta    id (INT IDENTITY, PK) · ruta · descripcion

Cada uno con las SEIS piezas que tiene `producto`, en el mismo orden y con
los mismos nombres de archivo cambiando solo el del recurso:

   1. El modelo (la entidad).
   2. Las TRES peticiones por verbo: Crear, Reemplazo y Actualizar.
   3. La interfaz del repositorio y su implementación para SQL Server,
      con el SQL escrito a mano y SIEMPRE parametrizado.
   4. La interfaz del servicio y su implementación.
   5. El controlador, con los CINCO verbos: GET (listar), GET por id,
      POST, PUT, PATCH y DELETE.
   6. La pantalla del front para cada recurso, con su propia dirección
      (/roles y /rutas), calcada de la de productos.

ATENCIÓN — DOS COSAS QUE NO SE CALCAN IGUAL, Y SON LAS IMPORTANTES:

  1. MIS DOS TABLAS TIENEN LLAVE IDENTITY: el id lo genera la base de datos, no el
     cliente. Por lo tanto:
       · La petición de CREAR (RolCrear, RutaCrear) NO lleva el campo id.
         Ni como obligatorio ni como opcional: no lo lleva.
       · El modelo NO marca el Id como `required`.
       · El POST devuelve el id que la base de datos generó.
     El molde `producto` tiene llave de TEXTO que manda el cliente, así que
     en esto NO lo copies: si me pides el id al crear, el POST estaría
     exigiéndome inventarle una llave a la base de datos.

  2. LA TABLA `ruta` TIENE UNA RESTRICCIÓN UNIQUE sobre la columna `ruta`:
     no puede haber dos filas con el mismo nombre de interfaz.
     NO captures esa violación ni la conviertas en un 409 con mensaje
     bonito. En esta versión la llave la defiende la BASE, no la API, y
     que eso salga como error 500 es DELIBERADO: está declarado en
     2_spec.md. Traducirlo a 409 es la versión 2 y adelantarlo viola el
     Artículo 1 de la constitución (YAGNI).

CALCA LO DEMÁS, NO REDISEÑES. Misma estructura de capas, mismos nombres de
métodos, mismo formato de respuesta, mismo manejo de errores. Si ves algo
que harías distinto, NO lo cambies: dímelo en un comentario aparte al
final, pero entrégame el código calcado.

REGLAS QUE NO SE NEGOCIAN:

  · Las TRES CAPAS: controlador → servicio → repositorio, cada una
    dependiendo de una INTERFAZ y no de una clase.
  · El controlador NO escribe SQL. El servicio NO nombra nada de HTTP:
    ni StatusCode, ni NotFound, ni IActionResult.
  · SIN ORM de entidades. SQL a mano con Dapper, siempre parametrizado.
  · TODO EN ESPAÑOL: nombres, comentarios y mensajes.
  · El front NO habla con la base de datos, solo con la API por HTTP.
  · NO toques la base de datos: las tablas ya están creadas. No escribas
    CREATE TABLE.

COMENTA TODO LO QUE ESCRIBAS, en español. Cada archivo empieza con un
bloque que dice QUÉ ES y QUÉ PAPEL cumple en la arquitectura. Cada método
no evidente lleva su comentario. Y los comentarios dicen POR QUÉ está
escrito así, no QUÉ hace la línea.

EMPIEZA POR `rol`, que es el más simple, entrégamelo archivo por archivo, y
NO sigas con `ruta` hasta que yo te diga que rol ya me funcionó.
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

## 6. El método de la conversación

Un archivo a la vez, y `ruta` solo cuando `rol` funcione.

| Haga esto | No haga esto |
|---|---|
| Pedir un archivo, pegarlo, **compilar**, seguir | Pedir los doce de una y pegarlos todos |
| Cuando falle, **pegarle el error literal** | Decirle «no funciona» |
| Leer los comentarios y corregir los que estén mal | Aceptarlos porque suenan bien |

> **Vigile la diferencia 1 en el archivo 2.** Cuando le entregue `RolCrear.cs`,
> **mírelo antes de pegarlo**: si tiene un campo `Id`, el chat calcó cuando no
> debía. Dígaselo así: *«RolCrear no lleva id: la llave es IDENTITY y la genera
> la base de datos. Quítalo.»*
>
> Encontrarlo ahí cuesta diez segundos. Encontrarlo cuando el `POST` falle, media
> hora.

---

## 7. Comprobar lo suyo antes del PR

```powershell
# 1 · Levantar el sistema.
docker compose up -d --build

# 2 · ¿Sus dos recursos salen en Swagger, con sus seis operaciones cada uno?
Start-Process http://localhost:8035/swagger

# 3 · LA PRUEBA DE LA DIFERENCIA 1: crear un rol SIN mandar id.
#     Si responde 422 pidiéndole el id, su RolCrear quedó mal.
#     Y fíjese en el id que le devuelve: lo generó la base de datos, no usted.
Invoke-RestMethod http://localhost:8035/api/rol -Method Post `
  -ContentType 'application/json' -Body '{"nombre":"RolDePruebaLuis"}'

# 4 · El mismo cuerpo, dos verbos. PUT debe dar 422 y PATCH 200.
#     (use el id que le devolvió el paso 3, aquí va 99 de ejemplo)
Invoke-RestMethod http://localhost:8035/api/rol/99 -Method Patch `
  -ContentType 'application/json' -Body '{"nombre":"Cambiado"}'

# 5 · LA PRUEBA DE LA DIFERENCIA 2: una ruta repetida.
#     DEBE responder 500. Si responde 409, su chat "arregló" lo que no debía.
Invoke-RestMethod http://localhost:8035/api/ruta -Method Post `
  -ContentType 'application/json' `
  -Body '{"ruta":"interfaz.inicio","descripcion":"repetida a proposito"}'

# 6 · Borre SU rol de prueba. Nunca toque las filas sembradas.
Invoke-RestMethod http://localhost:8035/api/rol/99 -Method Delete
```

> **El paso 5 es el único de todo el curso donde un 500 es el resultado
> correcto**, y por eso es la pregunta de sustentación más probable de su parte:
> *«¿por qué esto responde 500 y no 409?»*. La respuesta: porque en la v1 **la
> llave la defiende la base de datos y nadie la traduce**, y verlo sin traducir es lo que
> hace que el 409 de la v2 se entienda.
>
> Está escrito dos veces en el `2_spec.md` de esta versión — en el RF3 y en el
> criterio 5—: *«código duplicado → 500 con el error del motor en `detalle`»*.

> **Y un aviso para que no se confunda si mira el repositorio del curso.** Ahí el
> mismo caso responde **409**, no 500 — porque ese repositorio tiene el sistema
> **terminado**, y la traducción del error llegó en la v2.
>
> **O sea que no puede comprobar su 500 comparándolo con el ejemplo del
> profesor:** compárelo con el `2_spec.md` de la v1, que es lo que manda sobre
> su versión. Es la primera vez en el curso en que **el ejemplo terminado y la
> versión que usted entrega deben comportarse distinto a propósito** — y
> entenderlo es más importante que el código.

### Y lo que nunca debe aparecer en su código

```powershell
# El servicio no nombra HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\ServicioRol.cs,api_facturas\Servicios\ServicioRuta.cs `
  -Pattern 'StatusCode|NotFound|IActionResult' | Measure-Object | Select-Object Count

# Y su peticion de crear NO tiene id. Tiene que dar 0.
Select-String -Path api_facturas\Peticiones\RolCrear.cs,api_facturas\Peticiones\RutaCrear.cs `
  -Pattern 'Id' | Measure-Object | Select-Object Count
```

---

## 8. Subir y abrir el PR

```powershell
# 1 · Mire QUÉ va a subir. SOLO deben aparecer SUS archivos.
#     Si aparece algo de Carlos o de Paco, pare: tocó lo que no era.
git status

# 2 · Commits pequeños, uno por pieza.
git add api_facturas/Modelos/Rol.cs api_facturas/Peticiones/Rol*.cs
git commit -m "feat: rol, el modelo y sus peticiones (la llave la pone la base de datos)"
#    ... capa por capa, y luego ruta.

# 3 · Suba su rama.
git push -u origin rama-luis-v1

# 4 · Abra el Pull Request hacia main y describa QUÉ construyó.
#     Mencione las dos diferencias y cómo las resolvió: eso es lo que Carlos
#     va a revisar primero.
#     Carlos fusiona. Usted NO fusiona.
```

> **Nadie toca el archivo de otro, ni para arreglárselo.** Si vio un error en el
> código de Paco, dígaselo — no lo corrija en su rama. Repartir por archivos es
> lo único que hace que Git una las tres ramas solo.
