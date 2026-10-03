# Cómo construir la VERSIÓN 5 con IA — sobre su proyecto de la v4

> Guía de la **v5** (acumulativa: se construye encima de su proyecto con
> v1, v2, v3 y v4 terminadas). El método general es el de la
> [guía de la v1](../v1_sin_fk/GUIA_IA1.md) y los ajustes de
> trabajo acumulativo son los de la
> [guía de la v2](../v2_con_fk/GUIA_IA2.md): aquí está SOLO lo
> propio de la v5.

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

## 0. Punto de partida

Su proyecto con la **v3 funcionando** (los smoke tests de v1 a v3 pasan
con sus puertos +100). Novedad de esta versión: por primera vez cambia
la INFRAESTRUCTURA (un motor nuevo en el compose, con inicializador) y
NO el contrato (cero endpoints nuevos).

**Sus puertos:** API 8145 · SQL Server 11566 · **PostgreSQL 15562**
(la regla +100 de siempre).

## A.1 Qué subirle al chat: los 8 archivos de la v5

`docs/spec_kit/1_constitution.md` + los **siete** documentos de
`docs/spec_kit/versiones/v5_otros_motores/` (2_spec a 8_tasks).

> **Y el `9_checklist.md` no se sube, a propósito.** Es la compuerta que usted
> pasa ANTES de abrir la IA —revisa la especificación, no el código—, así que
> la IA no tiene nada que hacer con él. Por eso son siete documentos y no ocho.

Además esta vez la IA necesita ver DOS archivos suyos completos: `Program.cs` (lo va
a reescribir alrededor de la fábrica) y un repositorio SqlServer
cualquiera (el molde del calco — por ejemplo
`RepositorioEmpresaSqlServer.cs`).

## A.2 Prepare su proyecto (comandos PowerShell)

1. **Carpeta nueva de specs** y copia desde el clon del curso (ajuste la
   primera ruta):

   ```powershell
   mkdir docs\spec_kit\versiones\v5_otros_motores
   Copy-Item ..\proyecto_aplicacion_y_servicios_web4\docs\spec_kit\versiones\v5_otros_motores\* docs\spec_kit\versiones\v5_otros_motores\
   ```

2. **La BD PostgreSQL y su inicializador** — cópielos tal cual del clon
   del curso (son dato, no código a generar: mismas semillas o la
   regresión no será comparable):

   ```powershell
   Copy-Item ..\proyecto_aplicacion_y_servicios_web4\db\bdfacturas_postgres.sql db\
   Copy-Item ..\proyecto_aplicacion_y_servicios_web4\db\init_postgres.sh db\
   ```

3. **Cree los ARCHIVOS VACÍOS nuevos** — los 14 que la IA irá llenando
   (1 carpeta + 3 de fábrica + 14 repositorios Postgres):

   ```powershell
   mkdir api_facturas\Fabricas
   New-Item api_facturas\Fabricas\IFabricaRepositorios.cs, api_facturas\Fabricas\FabricaSqlServer.cs, api_facturas\Fabricas\FabricaPostgres.cs, api_facturas\Repositorios\RepositorioProductoPostgres.cs, api_facturas\Repositorios\RepositorioPersonaPostgres.cs, api_facturas\Repositorios\RepositorioFacturaPostgres.cs, api_facturas\Repositorios\RepositorioEmpresaPostgres.cs, api_facturas\Repositorios\RepositorioClientePostgres.cs, api_facturas\Repositorios\RepositorioVendedorPostgres.cs, api_facturas\Repositorios\RepositorioUsuarioPostgres.cs, api_facturas\Repositorios\RepositorioRolPostgres.cs, api_facturas\Repositorios\RepositorioRutaPostgres.cs, api_facturas\Repositorios\RepositorioRolUsuarioPostgres.cs, api_facturas\Repositorios\RepositorioRutaRolPostgres.cs
   ```

4. Archivos de la v3 que **CRECEN** (la IA le entrega la versión completa
   actualizada): `Program.cs` (la fábrica + el switch + diagnóstico v5
   con `motor`), `ApiFacturas.csproj` (paquete Npgsql),
   `docker-compose.yml` (postgres + postgres-init + variables de la
   API), `appsettings.json` (cadena Postgres + clave Motor) y
   `pruebas/Programa.cs` (criterio de la fábrica).

## A.3 El prompt (los cambios sobre el de la v3)

Use el prompt de la [guía v2](../v2_con_fk/GUIA_IA2.md) A.3
cambiando:

- "VERSIÓN 2" → "VERSIÓN 4", y el CONTEXTO CLAVE: *"Mi proyecto YA TIENE
  v1, v2 y v3 construidas y funcionando (las 12 tablas cubiertas contra
  SQL Server); NO toques Controllers/, Servicios/, Peticiones/, Modelos/
  ni Excepciones/ — la v5 vive de los repositorios hacia abajo. Solo
  crecen Program.cs, ApiFacturas.csproj, docker-compose.yml,
  appsettings.json y pruebas/Programa.cs."*
- Regla de alcance: *"nada de MariaDB (versión futura); el motor se
  elige UNA vez al arrancar (nada de motor por petición); los
  repositorios Postgres son un CALCO de los SqlServer
  (Npgsql, TOP (@limite) al principio en vez de LIMIT
  al final); el de factura usa CommandType.StoredProcedure con
  @p_resultado OUTPUT y traduce los THROW por número + patrón (50003 y
  50010); BCrypt se queda en el repositorio de usuario; PostgreSQL NO se
  siembra solo: el contenedor postgres-init corre el script una vez."*
- El ancla de stack queda igual; a los puertos +100 agregue:
  *"PostgreSQL publica en 15562 (no 15462)"*.

## A.4 Método: igual que la v3, con dos alarmas extra

1. Si la IA "aprovecha" para tocar un servicio o un controller
   ("mejoré el manejo de errores…"): recháselo — el criterio 4 de
   [2_spec.md](2_spec.md) exige que el diff no cruce la frontera de los
   repositorios.
2. Si la IA propone un ORM, EF Core o "un provider genérico" para no
   escribir los 14 repositorios: recháselo — la constitución prohíbe
   ORM, y escribir el calco ES la lección (el conocimiento ADO.NET se
   transfiere entre motores).

## Cierre

El cierre de esta versión es DOBLE de verdad: la regresión completa
(v1+v2+v3) debe pasar **contra SQL Server** y, con el interruptor
`MOTOR_BD=postgres`, **contra PostgreSQL** —
[7_quickstart.md](7_quickstart.md) §2, con sus puertos +100 → tag `v5`.
