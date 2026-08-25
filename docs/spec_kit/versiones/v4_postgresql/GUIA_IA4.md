# Cómo construir la VERSIÓN 4 con IA — sobre su proyecto de la v3

> Guía de la **v4** (acumulativa: se construye encima de su proyecto con
> v1, v2 y v3 terminadas). El método general es el de la
> [guía de la v1](../v1_producto_sqlserver/GUIA_IA1.md) y los ajustes de
> trabajo acumulativo son los de la
> [guía de la v2](../v2_persona_factura/GUIA_IA2.md): aquí está SOLO lo
> propio de la v4.

---

## 0. Punto de partida

Su proyecto con la **v3 funcionando** (los smoke tests de v1 a v3 pasan
con sus puertos +100). Novedad de esta versión: por primera vez cambia
la INFRAESTRUCTURA (un motor nuevo en el compose) y NO el contrato (cero
endpoints nuevos).

**Sus puertos:** API 8135 · SQL Server 11566 · **PostgreSQL 15562**
(la regla +100 de siempre).

## A.1 Qué subirle al chat (los 9 de la v4)

`docs/spec_kit/1_constitution.md` + los 7 documentos de
`docs/spec_kit/versiones/v4_postgresql/` (2_spec a 8_tasks). Además esta
vez la IA necesita ver DOS archivos suyos completos: `Program.cs` (lo va
a reescribir alrededor de la fábrica) y un repositorio SqlServer
cualquiera (el molde del calco — por ejemplo
`RepositorioEmpresaSqlServer.cs`).

## A.2 Prepare su proyecto (comandos PowerShell)

1. **Carpeta nueva de specs** y copia desde el clon del curso (ajuste la
   primera ruta):

   ```powershell
   mkdir docs\spec_kit\versiones\v4_postgresql
   Copy-Item ..\proyecto_aplicacion_y_servicios_web4\docs\spec_kit\versiones\v4_postgresql\* docs\spec_kit\versiones\v4_postgresql\
   ```

2. **El script de la BD PostgreSQL** — cópielo tal cual del clon del
   curso (es dato, no código a generar: mismas semillas o la regresión
   no será comparable):

   ```powershell
   Copy-Item ..\proyecto_aplicacion_y_servicios_web4\db\bdfacturas_postgres.sql db\
   ```

3. **Cree los ARCHIVOS VACÍOS nuevos** — los 14 que la IA irá llenando
   (1 carpeta + 3 de fábrica + 11 repositorios Postgres):

   ```powershell
   mkdir api_facturas\Fabricas
   New-Item api_facturas\Fabricas\IFabricaRepositorios.cs, api_facturas\Fabricas\FabricaSqlServer.cs, api_facturas\Fabricas\FabricaPostgres.cs, api_facturas\Repositorios\RepositorioProductoPostgres.cs, api_facturas\Repositorios\RepositorioPersonaPostgres.cs, api_facturas\Repositorios\RepositorioFacturaPostgres.cs, api_facturas\Repositorios\RepositorioEmpresaPostgres.cs, api_facturas\Repositorios\RepositorioClientePostgres.cs, api_facturas\Repositorios\RepositorioVendedorPostgres.cs, api_facturas\Repositorios\RepositorioUsuarioPostgres.cs, api_facturas\Repositorios\RepositorioRolPostgres.cs, api_facturas\Repositorios\RepositorioRutaPostgres.cs, api_facturas\Repositorios\RepositorioRolUsuarioPostgres.cs, api_facturas\Repositorios\RepositorioRutaRolPostgres.cs
   ```

4. Archivos de la v3 que **CRECEN** (la IA le entrega la versión completa
   actualizada): `Program.cs` (la fábrica + el switch + diagnóstico v4
   con `motor`), `ApiFacturas.csproj` (paquete Npgsql),
   `docker-compose.yml` (servicio postgres + variables de la API),
   `appsettings.json` (cadena Postgres + clave Motor) y
   `pruebas/Programa.cs` (criterio de la fábrica).

## A.3 El prompt (los cambios sobre el de la v3)

Use el prompt de la [guía v2](../v2_persona_factura/GUIA_IA2.md) A.3
cambiando:

- "VERSIÓN 2" → "VERSIÓN 4", y el CONTEXTO CLAVE: *"Mi proyecto YA TIENE
  v1, v2 y v3 construidas y funcionando (las 12 tablas cubiertas contra
  SQL Server); NO toques Controllers/, Servicios/, Peticiones/, Modelos/
  ni Excepciones/ — la v4 vive de los repositorios hacia abajo. Solo
  crecen Program.cs, ApiFacturas.csproj, docker-compose.yml,
  appsettings.json y pruebas/Programa.cs."*
- Regla de alcance: *"nada de MariaDB (versión futura); el motor se elige
  UNA vez al arrancar (nada de motor por petición); los repositorios
  Postgres son un CALCO de los SqlServer (Npgsql, LIMIT en vez de TOP);
  el de factura usa CALL y traduce por SQLSTATE P0001 + patrón del
  mensaje; BCrypt se queda en el repositorio de usuario."*
- El ancla de stack queda igual; a los puertos +100 agregue:
  *"PostgreSQL publica en 15562 (no 15462)"*.

## A.4 Método: igual que la v3, con dos alarmas extra

1. Si la IA "aprovecha" para tocar un servicio o un controller
   ("mejoré el manejo de errores…"): recháselo — el criterio 4 de
   [2_spec.md](2_spec.md) exige que el diff no cruce la frontera de los
   repositorios.
2. Si la IA propone un ORM, EF Core o "un provider genérico" para no
   escribir los 11 repositorios: recháselo — la constitución prohíbe ORM,
   y escribir el calco ES la lección (el conocimiento ADO.NET se
   transfiere entre motores).

## Cierre

El cierre de esta versión es DOBLE de verdad: la regresión completa
(v1+v2+v3) debe pasar **contra PostgreSQL** y, con el interruptor
`MOTOR_BD=sqlserver`, **contra SQL Server** —
[7_quickstart.md](7_quickstart.md) §2, con sus puertos +100 → tag `v4`.
