# Versión 5 — Luis · camino B (IDE agéntico)

> **Su parte:** **seis repositorios** en dialecto PostgreSQL — los del control de
> acceso, que son los que usted viene construyendo desde la v1.
>
> **No empiece hasta que el PR de Carlos esté fusionado:** necesita la fábrica y
> su molde.

---

## 1. Lo que construye

```
RepositorioRolPostgres            RepositorioRolUsuarioPostgres
RepositorioRutaPostgres           RepositorioRutaRolPostgres
RepositorioUsuarioConRolesPostgres   RepositorioPermisosPostgres
```

> **Es toda la mitad del acceso, y es suya desde el principio:** `rol` y `ruta`
> en la v1, las dos tablas puente y `usuario-con-roles` en la v2, el menú y los
> permisos en la v3. Ahora las traduce.

---

## 2. Lo que NO cambia, que es casi todo

| | |
|---|---|
| **La interfaz** | `IRepositorioRutaRol` es exactamente la misma |
| El servicio y el controlador | no se enteran |
| **La respuesta de la API** | **idéntica**, campo por campo, código por código |

> **Y ese «idéntica» es su criterio de aceptación.** Si con SQL Server una pareja
> repetida responde **409**, con PostgreSQL tiene que responder **409**.

---

## 3. Lo que su parte tiene de particular

### Sus llaves compuestas, otra vez

Sus tablas puente siguen sin `id`. En PostgreSQL eso no cambia — pero **sí cambia
cómo se detecta el duplicado**:

| | SQL Server | PostgreSQL |
|---|---|---|
| Pareja repetida | violación de PK → se detecta por el **número** de error | `RAISE EXCEPTION` / error de PK → **por el texto** |

### Y `usuario-con-roles`, que llama procedimientos

Es el otro recurso del sistema —además de `factura`— que **no escribe SQL de
tablas**. Los seis procedimientos existen en los dos motores con el mismo
nombre; lo que cambia es `EXEC` → **`CALL`**.

### `verificar_acceso_ruta`, el que decide todos los permisos

> **Éste es el más delicado de los seis**, porque si se traduce mal **el sistema
> no falla: deja pasar a quien no debe, o rechaza a quien sí puede**. Y ninguna
> de las dos cosas lanza un error.
>
> **Pruébelo con los tres usuarios**, como en la v3, y compare contra SQL Server.
> Un permiso mal traducido es un agujero de seguridad silencioso.

---

## 4. Las cuatro diferencias del dialecto

| | SQL Server | PostgreSQL |
|---|---|---|
| Cliente de .NET | `Microsoft.Data.SqlClient` | **`Npgsql`** |
| Llamar un procedimiento | `EXEC sp_x @p = …` | **`CALL sp_x(…)`** |
| Llave autogenerada | `IDENTITY(1,1)` | **`SERIAL`** |
| Lanzar un error | `THROW 50001, msg, 1` | **`RAISE EXCEPTION 'msg'`** |

> **Y la clase de traducción de errores ya existe: Carlos la dejó.** Úsela, no la
> reescriba. Si cada repositorio trae su propio `if` del mensaje, el día que un
> texto cambie de redacción habrá que corregirlo en catorce sitios.

---

## 5. Antes de abrir el agente

```powershell
git switch main ; git pull origin main

# Compruebe que el molde de Carlos llegó.
Get-ChildItem api_facturas\Repositorios\RepositorioProductoPostgres.cs
Get-ChildItem api_facturas\Fabricas\

git switch -c rama-luis-v5
git config user.name "ccastro202050" ; git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

---

## 6. El prompt (cópielo tal cual)

```
Traduce SEIS REPOSITORIOS de este proyecto al dialecto de PostgreSQL. El
proyecto ya tiene cuatro versiones funcionando sobre SQL Server, y un
compañero acaba de agregar la fábrica de repositorios y UN repositorio de
PostgreSQL —el de producto— que es EL MOLDE. Otro compañero traduce los
demás.

PRIMERO lee los documentos bajo docs/spec_kit/versiones/v5_otros_motores/,
el repositorio RepositorioProductoPostgres.cs que es el molde, la clase de
traducción de errores que dejó mi compañero, y el script
db/bdfacturas_postgres.sql. Después resume en máximo 10 líneas qué vas a
construir y ESPERA MI CONFIRMACIÓN antes de escribir un solo archivo.

PUEDES ESCRIBIR EN: api_facturas/Repositorios/ (solo archivos nuevos que
                    terminan en Postgres.cs) y la fábrica, para
                    registrarlos.
NO TOQUES: Controllers/, Servicios/, Modelos/, Peticiones/, docs/, db/,
           ni los repositorios de SQL Server que ya existen.

Eso no es una preferencia: el criterio de aceptación de esta versión es
que `git diff v4..v5` sobre Controllers y Servicios SALGA VACÍO. Si para
que algo funcione en PostgreSQL hay que cambiar un controlador o un
servicio, PÁRATE Y PREGÚNTAME.

LOS SEIS QUE ME TOCAN, que son los del control de acceso:

   RepositorioRolPostgres              RepositorioRolUsuarioPostgres
   RepositorioRutaPostgres             RepositorioRutaRolPostgres
   RepositorioUsuarioConRolesPostgres  RepositorioPermisosPostgres

CADA UNO IMPLEMENTA LA MISMA INTERFAZ que su equivalente de SQL Server.
No cambies las interfaces: son el contrato.

LAS DIFERENCIAS DEL DIALECTO:
  · El cliente es Npgsql, no Microsoft.Data.SqlClient.
  · Los procedimientos se llaman con CALL, no con EXEC.
  · USA LA CLASE DE TRADUCCIÓN DE ERRORES QUE YA EXISTE. No escribas tu
    propio `if` del mensaje en cada repositorio.
  · El esquema YA ESTÁ TRADUCIDO. NO escribas SQL de creación.

TRES COSAS PROPIAS DE MI PARTE:

  1. MIS DOS TABLAS PUENTE NO TIENEN COLUMNA id: su llave primaria son
     las dos columnas juntas. Eso no cambia en PostgreSQL. Lo que sí
     cambia es cómo se detecta una pareja repetida: en SQL Server se
     reconoce por el número del error y aquí hay que mirar el texto.
     Tiene que seguir respondiendo 409, igual que antes.

  2. `usuario-con-roles` NO escribe SQL de tablas: llama los seis
     procedimientos que ya existen, con CALL en vez de EXEC.

  3. verificar_acceso_ruta ES EL MÁS DELICADO DE TODOS, porque si se
     traduce mal EL SISTEMA NO FALLA: deja pasar a quien no debe o
     rechaza a quien sí puede, y ninguna de las dos cosas lanza un error.
     Tradúcelo con cuidado y respeta exactamente lo que devuelve el
     procedimiento.

Y LA REGLA QUE MANDA SOBRE TODAS: LAS RESPUESTAS DE LA API TIENEN QUE SER
IDÉNTICAS CON LOS DOS MOTORES. Mismo endpoint, mismo cuerpo, mismo código
de estado.

REGLAS QUE SIGUEN VIGENTES:
  · SIN ORM. SQL a mano con Dapper, parametrizado.
  · El repositorio NO decide códigos de estado: lanza la excepción de
    dominio que corresponda, igual que su gemelo de SQL Server.
  · TODO EN ESPAÑOL.

COMENTA TODO, en español. En estos archivos el comentario más útil es QUÉ
CAMBIÓ respecto al de SQL Server y POR QUÉ.

ORDEN: empieza por rol y ruta, que son los más parecidos al molde; luego
las dos tablas puente; y deja usuario-con-roles y los permisos de ÚLTIMO.
Al terminar cada uno dime qué archivos creaste y espera.
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

## 7. Comprobar lo suyo

```powershell
# 1 · EL CRITERIO DE ACEPTACIÓN. Tiene que salir vacío.
git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios

# 2 · La misma petición en los dos motores, comparada.
$env:MOTOR_BD="sqlserver" ; docker compose up -d api-facturas
$a = Invoke-RestMethod http://localhost:8035/api/rol
$env:MOTOR_BD="postgres"  ; docker compose up -d api-facturas
$b = Invoke-RestMethod http://localhost:8035/api/rol
Compare-Object ($a | ConvertTo-Json) ($b | ConvertTo-Json)

# 3 · LA PAREJA REPETIDA: 409 EN LOS DOS MOTORES.
Invoke-RestMethod http://localhost:8035/api/rutarol -Method Post `
  -ContentType 'application/json' -Body '{"fkidruta":1,"fkidrol":2}'
#    dos veces: la segunda debe dar 409

# 4 · LA PRUEBA QUE SOLO USTED PUEDE HACER — los permisos, con los tres
#     usuarios de la v3, EN POSTGRES:
#       admin@correo.com      → entra a todo
#       vendedor1@correo.com  → Facturas y Clientes; NO Usuarios
#       cliente1@correo.com   → Productos; NO Facturas
#     Si alguno entra donde no debe, verificar_acceso_ruta quedó mal
#     traducido — y NO va a dar ningún error.

# 5 · Que no duplicó la traducción de errores. Debe dar 1, no 6.
Select-String -Path api_facturas\Repositorios\*Postgres.cs `
  -Pattern 'duplicate key|llave duplicada' | Measure-Object | Select-Object Count
```

> **El paso 4 es el más importante de toda la versión, y es suyo.** Un permiso
> mal traducido **no se ve**: el sistema responde 200 y deja pasar. Es la única
> comprobación del curso donde el fallo es **silencioso y peligroso** a la vez.

---

## 8. Subir, y el cierre del curso

```powershell
git status ; git add api_facturas/
git commit -m "feat: rol y ruta en PostgreSQL, calcados del molde"
#   ... uno por uno
git push -u origin rama-luis-v5
```

> **En la descripción del PR ponga el resultado del paso 4**, usuario por
> usuario. Es lo último que Carlos revisa antes del tag final — y lo que no se
> puede dar por bueno «a ojo».

**Y con su PR fusionado, el curso cierra.** Carlos corre la regresión doble —las
mismas operaciones en los dos motores— y pone el último tag:

```powershell
git tag -a v5 -m "Version 5: el segundo motor"
git push origin v5
```

> **Mire el grafo entero una vez, al terminar.** Quince ramas, quince fusiones,
> cinco tags y tres nombres. Eso es lo que usted va a sustentar — y está escrito
> ahí, commit por commit, desde el 8 de agosto.
>
> ```powershell
> git log --oneline --graph --decorate --all
> ```
