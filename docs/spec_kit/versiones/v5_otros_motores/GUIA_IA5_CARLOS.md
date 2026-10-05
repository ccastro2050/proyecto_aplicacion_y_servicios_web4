# Versión 5 — Carlos · camino B (IDE agéntico)

> **Su parte:** la **fábrica**, el **interruptor `MOTOR_BD`**, y **un
> repositorio de PostgreSQL completo** como molde del dialecto.
>
> **Y usted va primero, solo**: sin la fábrica no hay dónde enchufar nada, y sin
> su molde Paco y Luis van a traducir cada uno a su manera.

---

## 1. Lo que construye

| | |
|---|---|
| `IFabricaRepositorios` y las dos fábricas | `FabricaSqlServer` · `FabricaPostgres` |
| El interruptor | la variable `MOTOR_BD`, leída en `Program.cs` |
| **El molde del dialecto** | `RepositorioProductoPostgres` — completo y bien hecho |

---

## 2. Por qué hace falta una fábrica, si hasta ahora bastaba la inyección

Hasta la v4, `Program.cs` decía «para `IRepositorioProducto`, usa
`RepositorioProductoSqlServer`» y listo. **Eso ya no sirve**, porque ahora hay
dos y la decisión es en tiempo de ejecución.

```
Program.cs  →  IFabricaRepositorios  →  FabricaSqlServer
                                     →  FabricaPostgres
```

> **Y la regla que hace que todo esto valga la pena: la fábrica es el ÚNICO
> sitio del sistema que conoce clases concretas.** Arriba se usa siempre la
> interfaz.
>
> Si alguien escribiera `new RepositorioProductoSqlServer()` dentro de un
> servicio, se cae toda la versión — y **el compilador no diría nada**, porque
> compilar es lo único que ese código haría bien.

---

## 3. Su criterio de aceptación es un `diff` vacío

```powershell
git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios
```

> **Tiene que salir vacío.** Si imprime algo, usted —o el agente— tocó una capa
> que no debía, y la versión falló: no por un error de programación, sino porque
> **las cuatro versiones anteriores enseñaron una arquitectura que no aguanta**.
>
> **Córralo antes de abrir el PR**, no después.

---

## 4. El molde del dialecto: las cuatro diferencias

El esquema de PostgreSQL **ya está traducido** en `db/bdfacturas_postgres.sql`.
Lo que hay que traducir es **el código que lo llama**.

| | SQL Server | PostgreSQL |
|---|---|---|
| Cliente de .NET | `Microsoft.Data.SqlClient` | **`Npgsql`** |
| Llamar un procedimiento | `EXEC sp_x @p = …` | **`CALL sp_x(…)`** |
| Llave autogenerada | `IDENTITY(1,1)` | **`SERIAL`** |
| Parámetros | `@nombre` | `@nombre` **también** — Dapper lo resuelve |

### Y la que de verdad cuesta: los errores

```
SQL Server    THROW 50001, 'Stock insuficiente…', 1   → tiene NÚMERO
PostgreSQL    RAISE EXCEPTION 'Stock insuficiente…'   → solo MENSAJE
```

> **Con SQL Server se pregunta por el número. Con PostgreSQL hay que mirar el
> texto.** Es una decisión incómoda y se toma a sabiendas: **un mensaje puede
> cambiar de redacción y romper la traducción**; un número no.
>
> **Ésta es la parte del molde que Paco y Luis van a calcar**, así que hágala
> bien y deje el patrón en un solo sitio — no repetido en catorce repositorios.

---

## 5. Antes de abrir el agente

```powershell
git switch main ; git pull origin main
git switch -c rama-carlos-v5
git config user.name "ccastro2050" ; git config user.email "su-correo-de-github"
git config user.name ; git config user.email

# Compruebe que la v4 funciona, en SQL Server, antes de tocar nada.
docker compose up -d --build
```

---

## 6. El prompt (cópielo tal cual)

```
Agrega SOPORTE PARA UN SEGUNDO MOTOR de base de datos a este proyecto,
que ya tiene cuatro versiones funcionando sobre SQL Server. Trabajo en
equipo: mis dos compañeros van a escribir la mayoría de los repositorios
de PostgreSQL. YO HAGO LA FÁBRICA, EL INTERRUPTOR Y UN REPOSITORIO COMO
MOLDE.

PRIMERO lee los documentos bajo docs/spec_kit/ (1_constitution.md y los
de versiones/v5_otros_motores/), el código que ya existe y el script
db/bdfacturas_postgres.sql. Después resume en máximo 10 líneas qué vas a
construir y ESPERA MI CONFIRMACIÓN antes de escribir un solo archivo.

PUEDES ESCRIBIR EN: api_facturas/Fabricas/, api_facturas/Repositorios/
                    y api_facturas/Program.cs
NO TOQUES: Controllers/, Servicios/, Modelos/, Peticiones/, docs/, db/

ESA ÚLTIMA LÍNEA ES EL CRITERIO DE ACEPTACIÓN DE ESTA VERSIÓN, no una
preferencia: al terminar, `git diff v4..v5` sobre Controllers y Servicios
TIENE QUE SALIR VACÍO. Si para que PostgreSQL funcione hay que cambiar un
controlador o un servicio, algo está mal diseñado — PÁRATE Y PREGÚNTAME
antes de tocarlos.

LO QUE CONSTRUYO YO:

  1. La interfaz IFabricaRepositorios, con un método Crear<X>() por cada
     repositorio que exista.

  2. Dos implementaciones: FabricaSqlServer y FabricaPostgres. LA FÁBRICA
     ES EL ÚNICO SITIO DEL SISTEMA QUE PUEDE NOMBRAR CLASES CONCRETAS.
     En ningún otro lado puede aparecer un `new RepositorioXSqlServer()`.

  3. El interruptor en Program.cs: lee la variable de entorno MOTOR_BD
     ("sqlserver" por defecto) y registra la fábrica que corresponda. Y
     el endpoint / de diagnóstico pasa a decir QUÉ MOTOR está puesto.

  4. UN SOLO repositorio de PostgreSQL, el de `producto`, COMPLETO y bien
     hecho: es el MOLDE que mis compañeros van a calcar para los otros
     trece. NO escribas los demás.

LAS DIFERENCIAS DEL DIALECTO, que el molde tiene que resolver:

  · El cliente es Npgsql, no Microsoft.Data.SqlClient.
  · Los procedimientos se llaman con CALL, no con EXEC.
  · LOS ERRORES NO TIENEN NÚMERO. SQL Server lanza THROW 50001 y se puede
    preguntar por el número; PostgreSQL lanza RAISE EXCEPTION y solo
    llega el MENSAJE. La traducción a códigos HTTP hay que hacerla
    mirando el patrón del texto.
    PONLO EN UN SOLO SITIO —una clase de traducción de errores—, no
    repetido en cada repositorio: mis compañeros van a escribir trece
    más y no quiero el mismo `if` catorce veces.

  · El esquema YA ESTÁ TRADUCIDO en db/bdfacturas_postgres.sql. NO
    escribas SQL de creación: la base viene dada.

Y LO MÁS IMPORTANTE DE TODO: las respuestas de la API tienen que ser
IDÉNTICAS con los dos motores. El mismo endpoint, el mismo cuerpo, el
mismo código de estado. Si con SQL Server algo responde 409, con
PostgreSQL también.

REGLAS QUE SIGUEN VIGENTES:
  · Las TRES CAPAS con interfaces. El repositorio no decide códigos de
    estado; el servicio NO nombra nada de HTTP.
  · SIN ORM. SQL a mano con Dapper, parametrizado.
  · TODO EN ESPAÑOL.

COMENTA TODO, en español, diciendo POR QUÉ. En el molde de PostgreSQL el
comentario más útil es QUÉ CAMBIÓ respecto al de SQL Server y por qué —
mis compañeros lo van a leer para traducir los suyos.

ORDEN: primero la interfaz de la fábrica y las dos implementaciones con
SQL Server —comprobando que todo sigue funcionando igual—, luego el
interruptor, y de último el repositorio de producto en PostgreSQL. Dime
qué archivos creaste en cada paso.
```

> **Fíjese en el orden del prompt:** la fábrica se monta **primero con SQL Server
> solo**, y todo tiene que seguir funcionando igual. Si algo se rompe ahí, se
> rompió por la fábrica y no por PostgreSQL — y eso es mucho más fácil de
> arreglar que las dos cosas juntas.

---

## 7. Comprobar, y avisar

```powershell
# 1 · EL CRITERIO DE ACEPTACIÓN. Tiene que salir vacío.
git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios

# 2 · Nadie fuera de la fábrica nombra una clase concreta. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\*.cs,api_facturas\Controllers\*.cs `
  -Pattern 'new Repositorio' | Measure-Object | Select-Object Count

# 3 · Con SQL Server, todo sigue igual.
docker compose up -d --build
Invoke-RestMethod http://localhost:8035/

# 4 · Y el interruptor, SIN RECOMPILAR:
$env:MOTOR_BD="postgres"
docker compose up -d api-facturas
Invoke-RestMethod http://localhost:8035/
#    el diagnóstico debe decir ahora "postgres"

# 5 · Su molde responde igual en los dos motores:
Invoke-RestMethod http://localhost:8035/api/producto
```

> **El paso 4 es el que hay que saber explicar.** No se recompiló nada: solo
> cambió una variable de entorno y se levantó el contenedor. **Eso es elegir el
> motor por configuración**, y es lo que la fábrica compró.

**Y después avísele a Paco y a Luis**, porque sin su molde van a traducir cada
uno a su manera. Dígales en concreto: **dónde quedó la clase de traducción de
errores** y **qué repositorio es el molde**.

---

## 8. Subir, y la integración final del curso

```powershell
git status ; git add api_facturas/
git commit -m "feat: la fabrica de repositorios, el unico sitio que conoce clases concretas"
git push -u origin rama-carlos-v5
```

**Al integrar esta versión, la regresión es DOBLE:**

```powershell
# Con SQL Server
$env:MOTOR_BD="sqlserver" ; docker compose up -d api-facturas
#   ... las operaciones de v1 a v4, con token

# Y con PostgreSQL, EXACTAMENTE LAS MISMAS
$env:MOTOR_BD="postgres" ; docker compose up -d api-facturas
#   ... otra vez, y tienen que responder LO MISMO
```

> **No basta con que PostgreSQL funcione: tiene que responder IGUAL.** Si un 409
> se volvió un 500 al cambiar de motor, la traducción de errores quedó a medias —
> y eso es justo lo que esta versión existe para comprobar.

```powershell
git tag -a v5 -m "Version 5: el segundo motor"
git push origin v5
```
