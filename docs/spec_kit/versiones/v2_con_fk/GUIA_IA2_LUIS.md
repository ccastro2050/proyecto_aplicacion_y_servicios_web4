# Versión 2 — Luis · camino A (chat web)

> **Su parte:** las dos **tablas puente** —`rol_usuario` y `rutarol`— y el
> recurso **`usuario-con-roles`**.
>
> **Su herramienta en esta versión:** un chat web. Desde la **v4** usted pasa a
> agente — [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3.
>
> **No empiece hasta que el PR de Carlos esté fusionado.**
>
> **Y lea la §2 antes del prompt:** sus tablas **no tienen llave propia**, y de
> ahí sale casi todo lo raro de su parte.

---

## 1. Qué es una tabla puente, y por qué le tocó a usted

```
usuario  ←── rol_usuario ──→  rol          ¿qué roles tiene esta persona?
ruta     ←── rutarol     ──→  rol          ¿a qué pantallas entra este rol?
```

Una tabla puente **no guarda cosas: guarda parejas.** No existe «un
`rol_usuario`» como existe «un producto». Existe **la afirmación** *«Marta es
cajera»*, y eso es verdad o no es verdad.

| | Columnas | Llave primaria |
|---|---|---|
| `rol_usuario` | `fkemail` · `fkidrol` | **las dos juntas** |
| `rutarol` | `fkidruta` · `fkidrol` | **las dos juntas** |

> **Le tocó a usted porque en la v1 ya construyó `rol` y `ruta`.** Es la
> continuación natural: ahora las conecta.

---

## 2. Lo que NO se calca del molde: no hay `id`

**Sus tablas no tienen llave propia**, y eso cambia **cuatro** cosas respecto a
todo lo que usted ya escribió.

### Por qué no tienen `id`, que es la pregunta de sustentación

> **Un `id` propio permitiría DOS filas con la misma pareja y distinto id** — y
> eso es exactamente lo que la tabla existe para impedir. *«Marta es cajera»* no
> puede ser verdad dos veces.
>
> Con la llave compuesta, la base de datos **rechaza el duplicado sola**. Con un `id`,
> habría que programar la comprobación en algún lado, y alguien se olvidaría.

### Las cuatro consecuencias

| | En un recurso normal | En una tabla puente |
|---|---|---|
| **Consultar uno** | `GET /api/rol/5` | `GET /api/rol-usuario/{email}/{idrol}` — **las dos claves** |
| **Borrar** | `DELETE /api/rol/5` | `DELETE /api/rol-usuario/{email}/{idrol}` — **las dos** |
| **Listar** | una lista | **por los dos lados**: los roles de un usuario, o los usuarios de un rol |
| **Actualizar** | `PUT` / `PATCH` | **no existe** — ver abajo |

### Por qué no hay PUT ni PATCH, y aun así hay que escribirlos

> **En una tabla puente, «actualizar» es MOVER la fila.** Cambiar
> `(marta, cajera)` por `(marta, contadora)` no es modificar un dato: es **borrar
> una afirmación y crear otra**. No hay nada que actualizar — la fila **es** su
> llave.

**Pero los escribe igual, y los deja comentados**, con la razón al lado. Es
material de clase: hay que ver **cómo se programa cada verbo**, incluso el que
el modelo no admite. Está en
[`REQUISITOS_FUNCIONALES.md`](../../../dominio/REQUISITOS_FUNCIONALES.md) §6.

---

## 3. `usuario-con-roles`, que no es una tabla

**No hay ninguna tabla que se llame así.** Es un recurso que opera **dos a la
vez** —`usuario` y `rol_usuario`— con procedimientos que ya existen:

```
crear_usuario_con_roles        consultar_usuario_con_roles
actualizar_usuario_con_roles   listar_usuarios_con_roles
eliminar_usuario_con_roles     actualizar_roles_usuario
```

> **Para qué existe:** crear un usuario y asignarle tres roles son **cuatro
> escrituras**. Si el front las hiciera una por una, un usuario podría quedar
> creado y sin roles — y entonces existe y no puede entrar a nada.
>
> **Es la misma idea de la factura de Carlos**, aplicada a otro sitio: cuando
> una operación del negocio son varias escrituras, **va en un procedimiento**.

> **Y por eso su repositorio de `usuario-con-roles` NO escribe SQL de tablas**:
> llama procedimientos, igual que el de factura. Los otros dos recursos suyos
> —las puentes— **sí** escriben SQL normal.

---

## 4. Antes de abrir el chat

```powershell
# 1 · Traiga lo de Carlos.
git switch main
git pull origin main

# 2 · Compruebe que llegó. Si no está, PARE.
Get-ChildItem api_facturas\Controllers\FacturaController.cs

# 3 · Su rama.
git switch -c rama-luis-v2

# 4 · SU IDENTIDAD, parado en la carpeta del proyecto. Y compruébela.
git config user.name "ccastro202050"
git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

---

## 5. Qué subirle al chat

| # | Archivo | Para qué |
|---|---|---|
| 1 | [`1_constitution.md`](../../1_constitution.md) | Las reglas permanentes |
| 2 | [`5_data_model.md`](5_data_model.md) | **Las llaves compuestas** — lo escribió usted |
| 3 | [`6_contracts.md`](6_contracts.md) | Los endpoints con **dos** parámetros de ruta |
| 4 | [`2_spec.md`](2_spec.md) | QUÉ construir |
| 5–10 | **Los archivos de `rol`**, que hizo usted en la v1 | El molde |
| 11 | **El repositorio de `factura` de Carlos** | Para calcar **cómo se llama un procedimiento** |

> **El 11 es el que le va a ahorrar la tarde.** `usuario-con-roles` llama
> procedimientos, y si el chat no ha visto uno hecho, va a escribir SQL de
> tablas — que es justo lo que no va.

**No suba** `9_checklist.md`, `0_mapa_versiones.md` ni `db/bdfacturas.sql`.

**Tres chequeos:** adjuntos completos, **razonamiento encendido**, **búsqueda web
apagada**.

---

## 6. El prompt (cópielo tal cual como PRIMER mensaje)

```
Actúa como mi asistente de programación en un proyecto universitario que
hacemos TRES personas. El proyecto YA EXISTE: una versión 1 completa con
seis recursos, y un compañero acaba de agregar `factura`. Mi trabajo son
las DOS TABLAS PUENTE y un recurso que opera dos tablas a la vez.

Te adjunto los documentos del spec kit, los archivos de un recurso normal
que ya funciona (el molde) y el repositorio de `factura` (para que veas
cómo se llama un procedimiento). Es C# sobre ASP.NET Core (.NET 10) con
SQL Server y Dapper — nunca un ORM de entidades.

LO QUE TIENES QUE ESCRIBIR, Y SON TRES RECURSOS:

   rol_usuario   (fkemail, fkidrol)      llave COMPUESTA, sin id propio
   rutarol       (fkidruta, fkidrol)     llave COMPUESTA, sin id propio
   usuario-con-roles                     NO es una tabla: ver abajo

LO MÁS IMPORTANTE, Y ES LO QUE NO SE CALCA DEL MOLDE:

  LAS DOS TABLAS PUENTE NO TIENEN COLUMNA `id`. Su llave primaria son las
  DOS columnas juntas. Por lo tanto:

    · El modelo NO lleva un Id. Lleva las dos claves foráneas.
    · La petición de CREAR lleva las dos claves, y nada más.
    · Consultar una es GET /api/rol-usuario/{email}/{idrol} — DOS
      parámetros de ruta, no uno.
    · Borrar es DELETE /api/rol-usuario/{email}/{idrol} — las dos.
    · Listar va POR LOS DOS LADOS: los roles de un usuario
      (GET /api/rol-usuario/usuario/{email}) y los usuarios de un rol
      (GET /api/rol-usuario/rol/{idrol}).
    · Y crear una pareja que YA EXISTE responde 409, no 422: el dato
      tiene la forma correcta, lo que choca es el estado de la base de datos.

  NO HAY PUT NI PATCH, y la razón importa: en una tabla puente
  "actualizar" significa MOVER la fila, o sea borrar una pareja y crear
  otra. La fila ES su llave, así que no hay nada que modificar.
  PERO ESCRÍBELOS IGUAL en el controlador y DÉJALOS COMENTADOS, con un
  comentario que explique por qué están apagados. Es material de clase.

EL TERCER RECURSO, `usuario-con-roles`:

  NO existe una tabla con ese nombre. Es un recurso que opera `usuario` y
  `rol_usuario` A LA VEZ, llamando procedimientos que YA EXISTEN:
     crear_usuario_con_roles        consultar_usuario_con_roles
     actualizar_usuario_con_roles   listar_usuarios_con_roles
     eliminar_usuario_con_roles     actualizar_roles_usuario

  Su repositorio NO escribe SQL de tablas: llama procedimientos, igual
  que el de factura que te adjunté. NO escribas CREATE PROCEDURE: la base de datos
  viene dada.

  Para qué existe: crear un usuario y asignarle tres roles son cuatro
  escrituras. Si se hicieran una por una, el usuario podría quedar creado
  y sin roles.

UN DETALLE QUE SE ROMPE EN SILENCIO SI NO LO CUIDAS: los procedimientos
devuelven columnas como `idrol` y `nombre_rol`, y las propiedades de C# se
llaman IdRol y NombreRol. Sin [JsonPropertyName] llegan `null` y 0, SIN
ERROR y con HTTP 200. Ponlos.

REGLAS QUE SIGUEN VIGENTES:

  · Las TRES CAPAS con interfaces. El controlador no escribe SQL; el
    servicio NO nombra nada de HTTP —ni StatusCode, ni NotFound, ni
    IActionResult—; el repositorio no decide códigos de estado.
  · SIN ORM. SQL a mano con Dapper, siempre parametrizado.
  · TODO EN ESPAÑOL.
  · El front NO habla con la base de datos: solo con la API por HTTP.
  · NO toques la base de datos: las tablas y los procedimientos ya existen.
  · NO toques `factura`, `cliente` ni `vendedor`: son de mis compañeros.

LA INTERFAZ GRÁFICA: una pantalla para asignar roles a un usuario y otra
para asignar rutas a un rol. En las dos, las claves se escogen en
DESPLEGABLES cargados de la API, nunca en un campo de texto. Cárgalos con
Task.WhenAll, no uno tras otro.

COMENTA TODO LO QUE ESCRIBAS, en español. Cada archivo empieza diciendo
QUÉ ES y QUÉ PAPEL cumple. Los comentarios dicen POR QUÉ está escrito
así, no QUÉ hace la línea.

EMPIEZA POR `rutarol`, que es la más simple —dos enteros—, entrégamelo
archivo por archivo, y NO sigas hasta que yo te diga que funciona.
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

## 7. El método, y lo que hay que vigilar

| Haga esto | No haga esto |
|---|---|
| Un archivo, pegarlo, **compilar**, seguir | Pedir los quince de una |
| Cuando falle, **pegarle el error literal** | Decirle «no funciona» |

> **Vigile el modelo en el archivo 1.** Si `RolUsuario.cs` tiene una propiedad
> `Id`, el chat calcó cuando no debía. Dígaselo así: *«rol_usuario no tiene id:
> su llave primaria son las dos columnas juntas. Quítalo.»*
>
> Encontrarlo ahí cuesta diez segundos. Encontrarlo cuando el `DELETE` no sepa
> qué borrar, media hora.

---

## 8. Comprobar lo suyo antes del PR

```powershell
# 1 · Crear una pareja.
Invoke-RestMethod http://localhost:8035/api/rutarol -Method Post `
  -ContentType 'application/json' -Body '{"fkidruta":1,"fkidrol":2}'

# 2 · LA PRUEBA DE LA LLAVE COMPUESTA: la misma pareja otra vez.
#     Debe responder 409 — la base de datos lo impide sola, sin que usted lo programe.
Invoke-RestMethod http://localhost:8035/api/rutarol -Method Post `
  -ContentType 'application/json' -Body '{"fkidruta":1,"fkidrol":2}'

# 3 · Listar por los DOS lados.
Invoke-RestMethod http://localhost:8035/api/rutarol/rol/2
Invoke-RestMethod http://localhost:8035/api/rutarol/ruta/1

# 4 · Borrar con LAS DOS claves.
Invoke-RestMethod http://localhost:8035/api/rutarol/1/2 -Method Delete

# 5 · usuario-con-roles: crear un usuario CON sus roles, en UNA llamada.
#     Luego consúltelo: debe traer el usuario Y la lista de roles.
#     Si los roles llegan vacíos o en 0, falta el [JsonPropertyName].

# 6 · Borre lo que usted creó.
```

> **El paso 2 es el que hay que saber explicar.** Ese 409 **no lo programó
> usted**: lo produce la llave compuesta. Si su diseño tuviera un `id`, esa
> segunda petición habría funcionado y habría dos filas diciendo lo mismo.

```powershell
# 7 · Y las pantallas, donde los tropiezos 1 y 2 fallan EN SILENCIO:
Start-Process http://localhost:8099/permisos
```

```powershell
# El servicio no nombra HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\ServicioRolUsuario.cs,api_facturas\Servicios\ServicioRutaRol.cs `
  -Pattern 'StatusCode|NotFound|IActionResult' | Measure-Object | Select-Object Count

# Y el modelo de una puente NO tiene Id. Tiene que dar 0.
Select-String -Path api_facturas\Modelos\RutaRol.cs -Pattern 'public int Id' |
  Measure-Object | Select-Object Count
```

---

## 9. Subir y abrir el PR

```powershell
git status                      # SOLO sus archivos
git add api_facturas/Modelos/RutaRol.cs api_facturas/Peticiones/RutaRol*.cs
git commit -m "feat: rutarol, el modelo con llave compuesta (sin id propio)"
#   ... capa por capa
git push -u origin rama-luis-v2
#   y el Pull Request hacia main. Carlos revisa y fusiona; usted NO.
```

> **En la descripción del PR explique por qué sus tablas no tienen `id`.** Es la
> decisión de diseño de su parte, y es lo primero que Carlos va a mirar.

> **Nadie toca el archivo de otro, ni para arreglárselo.**
