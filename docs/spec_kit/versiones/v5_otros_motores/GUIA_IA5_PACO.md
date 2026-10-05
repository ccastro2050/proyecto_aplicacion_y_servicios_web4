# Versión 5 — Paco · camino B (IDE agéntico)

> **Su parte:** **siete repositorios** en dialecto PostgreSQL — los de las
> tablas que usted ya construyó, más **`factura`**, que es el único que llama
> procedimientos.
>
> **No empiece hasta que el PR de Carlos esté fusionado:** usted necesita la
> fábrica donde enchufar lo suyo, y su molde para saber cómo se traduce.

---

## 1. Lo que construye

```
RepositorioPersonaPostgres      RepositorioClientePostgres
RepositorioEmpresaPostgres      RepositorioVendedorPostgres
RepositorioUsuarioPostgres      RepositorioConsultasPostgres
RepositorioFacturaPostgres   ← el único que llama procedimientos
```

> **Son los mismos recursos que usted hizo en las versiones anteriores**, ahora
> en otro dialecto. Y le toca `factura` porque es el caso difícil: Carlos dejó el
> molde del caso simple, y alguien tiene que resolver el de los procedimientos.

---

## 2. Lo que NO cambia, que es casi todo

| | |
|---|---|
| **La interfaz** | `IRepositorioPersona` es exactamente la misma. Usted escribe **otra implementación**, no otro contrato |
| El servicio | no se entera |
| El controlador | no se entera |
| **La respuesta de la API** | **idéntica**, campo por campo, código por código |

> **Eso último es el criterio de aceptación de su parte, y no es negociable:** si
> con SQL Server algo responde **409**, con PostgreSQL tiene que responder
> **409**. No «un error parecido»: el mismo.

---

## 3. Las cuatro diferencias del dialecto

| | SQL Server | PostgreSQL |
|---|---|---|
| Cliente de .NET | `Microsoft.Data.SqlClient` | **`Npgsql`** |
| Llamar un procedimiento | `EXEC sp_x @p = …` | **`CALL sp_x(…)`** |
| Llave autogenerada | `IDENTITY(1,1)` | **`SERIAL`** |
| Lanzar un error | `THROW 50001, msg, 1` | **`RAISE EXCEPTION 'msg'`** |

### Y la quinta, que es la que de verdad cuesta

```
SQL Server    THROW 50001         → se pregunta por el NÚMERO
PostgreSQL    RAISE EXCEPTION     → llega el MENSAJE, sin número propio
```

> **Carlos ya dejó una clase que traduce por el patrón del texto. Úsela, no la
> reescriba.** Si cada repositorio trae su propio `if`, el día que un mensaje
> cambie de redacción habrá que corregirlo en catorce sitios.

---

## 4. Su caso difícil: `factura` con `CALL`

Los seis procedimientos existen en los dos motores, con **el mismo nombre**. Lo
que cambia es cómo se llaman y cómo se leen sus errores.

| | SQL Server | PostgreSQL |
|---|---|---|
| La llamada | `EXEC sp_insertar_factura_y_productosporfactura @…` | `CALL sp_insertar_factura_y_productosporfactura(…)` |
| Stock insuficiente | `THROW 50001` → se detecta por el número | `RAISE EXCEPTION 'Stock insuficiente…'` → **por el texto** |
| Ya anulada | `THROW 50010` | igual: **por el texto** |

> **Y la transacción sigue estando adentro del procedimiento**, en los dos
> motores. Usted **no** abre una transacción en C#: la prueba de los tres
> renglones con el segundo sin stock tiene que seguir dejando **cero facturas**.

---

## 5. Antes de abrir el agente

```powershell
git switch main ; git pull origin main

# Compruebe que el molde de Carlos llegó.
Get-ChildItem api_facturas\Repositorios\RepositorioProductoPostgres.cs
Get-ChildItem api_facturas\Fabricas\

git switch -c rama-paco-v5
git config user.name "ccastro2050-50" ; git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

> **Pregúntele a Carlos dónde quedó la clase de traducción de errores.** Si no la
> usa, va a duplicarla.

---

## 6. El prompt (cópielo tal cual)

```
Traduce SIETE REPOSITORIOS de este proyecto al dialecto de PostgreSQL.
El proyecto ya tiene cuatro versiones funcionando sobre SQL Server, y un
compañero acaba de agregar la fábrica de repositorios y UN repositorio de
PostgreSQL —el de producto— que es EL MOLDE. Otro compañero traduce los
demás.

PRIMERO lee los documentos bajo docs/spec_kit/versiones/v5_otros_motores/,
el repositorio RepositorioProductoPostgres.cs que es el molde, la clase de
traducción de errores que dejó mi compañero, y el script
db/bdfacturas_postgres.sql. Después resume en máximo 10 líneas qué vas a
construir y ESPERA MI CONFIRMACIÓN antes de escribir un solo archivo.

PUEDES ESCRIBIR EN: api_facturas/Repositorios/ (solo los archivos nuevos
                    que terminan en Postgres.cs) y la fábrica, para
                    registrarlos.
NO TOQUES: Controllers/, Servicios/, Modelos/, Peticiones/, docs/, db/,
           ni los repositorios de SQL Server que ya existen.

ESO NO ES UNA PREFERENCIA: el criterio de aceptación de esta versión es
que `git diff v4..v5` sobre Controllers y Servicios SALGA VACÍO. Si para
que algo funcione en PostgreSQL hay que cambiar un controlador o un
servicio, PÁRATE Y PREGÚNTAME.

LOS SIETE QUE ME TOCAN:

   RepositorioPersonaPostgres     RepositorioClientePostgres
   RepositorioEmpresaPostgres     RepositorioVendedorPostgres
   RepositorioUsuarioPostgres     RepositorioConsultasPostgres
   RepositorioFacturaPostgres

CADA UNO IMPLEMENTA LA MISMA INTERFAZ que su equivalente de SQL Server.
No cambies las interfaces: son el contrato, y arriba nadie puede
enterarse de qué motor hay debajo.

LAS DIFERENCIAS DEL DIALECTO:
  · El cliente es Npgsql, no Microsoft.Data.SqlClient.
  · Los procedimientos se llaman con CALL, no con EXEC.
  · USA LA CLASE DE TRADUCCIÓN DE ERRORES QUE YA EXISTE. NO escribas
    tu propio `if` del mensaje en cada repositorio: si cada uno trae el
    suyo, el día que un mensaje cambie hay que corregir catorce sitios.
  · El esquema YA ESTÁ TRADUCIDO en db/bdfacturas_postgres.sql. NO
    escribas SQL de creación.

EL CASO DIFÍCIL ES `factura`, y es el único de los siete que no se calca
del molde:
  · Llama SEIS procedimientos, con CALL en vez de EXEC.
  · La transacción sigue estando DENTRO del procedimiento. NO abras una
    transacción en C#.
  · Sus errores no tienen número: "stock insuficiente" y "ya anulada"
    llegan como texto de un RAISE EXCEPTION. Tradúcelos a los MISMOS
    códigos HTTP que devuelve la versión de SQL Server.

Y LA REGLA QUE MANDA SOBRE TODAS: LAS RESPUESTAS DE LA API TIENEN QUE SER
IDÉNTICAS CON LOS DOS MOTORES. Mismo endpoint, mismo cuerpo, mismo código
de estado. Si con SQL Server algo responde 409, con PostgreSQL también —
no "un error parecido".

REGLAS QUE SIGUEN VIGENTES:
  · SIN ORM. SQL a mano con Dapper, parametrizado.
  · El repositorio NO decide códigos de estado: lanza la excepción de
    dominio que corresponda, igual que su gemelo de SQL Server.
  · TODO EN ESPAÑOL.

COMENTA TODO, en español. En estos archivos el comentario más útil es QUÉ
CAMBIÓ respecto al de SQL Server y POR QUÉ — no qué hace la consulta, que
ya se sabe.

ORDEN: empieza por persona y empresa, que son los más parecidos al molde,
y deja `factura` de ÚLTIMO. Al terminar cada uno dime qué archivos
creaste y espera.
```

---

## 7. Comprobar lo suyo

```powershell
# 1 · EL CRITERIO DE ACEPTACIÓN. Tiene que salir vacío.
git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios

# 2 · LA PRUEBA QUE IMPORTA: la MISMA petición en los dos motores,
#     comparando las respuestas.
$env:MOTOR_BD="sqlserver" ; docker compose up -d api-facturas
$a = Invoke-RestMethod http://localhost:8035/api/persona

$env:MOTOR_BD="postgres"  ; docker compose up -d api-facturas
$b = Invoke-RestMethod http://localhost:8035/api/persona

#    Compare: mismo sobre, mismos campos, mismos valores.
Compare-Object ($a | ConvertTo-Json) ($b | ConvertTo-Json)
#    Sin diferencias = bien.

# 3 · Y LOS ERRORES, que es donde se rompe:
#     la clave foránea inexistente debe dar 409 EN LOS DOS.
Invoke-RestMethod http://localhost:8035/api/cliente -Method Post `
  -ContentType 'application/json' -Body '{"fkcodpersona":"P999","credito":100}'

# 4 · LA TRANSACCIÓN, en PostgreSQL: tres renglones, el segundo sin stock.
#     No debe quedar NINGUNA factura, igual que en SQL Server.

# 5 · Y que no duplicó la traducción de errores. Debe dar 1, no 7.
Select-String -Path api_facturas\Repositorios\*Postgres.cs `
  -Pattern 'Stock insuficiente' | Measure-Object | Select-Object Count
```

> **El paso 3 es el que más se falla**, y no se ve: el `GET` funciona
> perfectamente en los dos motores, y el error devuelve 500 en uno y 409 en el
> otro. **Pruebe los caminos de error, no solo los felices.**

> **Y el paso 5 es el que delata si el agente se repitió.** Si el patrón del
> mensaje aparece siete veces, cada repositorio trae su propio `if` — y eso es
> una bomba de tiempo.

---

## 8. Subir y abrir el PR

```powershell
git status ; git add api_facturas/
git commit -m "feat: persona y empresa en PostgreSQL, calcadas del molde"
#   ... uno por uno, y factura de último
git push -u origin rama-paco-v5
```

> **En la descripción del PR ponga el resultado del paso 2 y del 3**: que las
> respuestas son idénticas y que los errores también. Es exactamente lo que
> Carlos tiene que verificar antes del último tag del curso.
