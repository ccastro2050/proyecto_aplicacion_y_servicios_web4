# Especificación — Versión 4: el segundo motor (PostgreSQL) y la fábrica

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1, v2 y v3 — los 51 endpoints
> existentes no se tocan y sus contratos siguen vigentes tal cual.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | **2_spec.md** (este) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la fábrica, los 11 repositorios Postgres y el interruptor |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas, ahora en dialecto PostgreSQL |
> | [6_contracts.md](6_contracts.md) | CERO endpoints nuevos — esa es la gracia |
> | [7_quickstart.md](7_quickstart.md) | La regresión DOBLE: todo pasa en ambos motores |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

## 1. Propósito de la v4

**Demostrar que las capas eran verdad.** Desde la v1 el proyecto repite
que controlador y servicio "no saben qué motor hay debajo". La v4 lo
somete a la prueba definitiva: aparece un **segundo motor (PostgreSQL)**
con la MISMA bdfacturas, y la API entera — 51 endpoints, validaciones,
errores de negocio, BCrypt, triggers y SPs — funciona idéntica contra
cualquiera de los dos. **Sin tocar UNA línea por encima de los
repositorios.**

Y de paso cobra la deuda anunciada: la lista de registros del ensamblador
que en la v3 "ya dolía" se cura con la **fábrica de repositorios** (patrón
fábrica abstracta): UN punto del código decide el motor; el resto pide
interfaces.

## 2. Alcance

**Incluye:** servicio `postgres` en el compose (misma BD semilla) · los 11
repositorios en dialecto Npgsql · `IFabricaRepositorios` +
`FabricaSqlServer` + `FabricaPostgres` · el **interruptor** `MOTOR_BD`
(configuración, no código) · diagnóstico pasa a `"version": "v4"` y
estrena `"motor"` · la prueba de capas crece con la fábrica.

**No incluye (deliberado — [mapa](../0_mapa_versiones.md)):**
- **MariaDB** (v5): el tercer motor esperará — con la fábrica puesta,
  costará una clase.
- **Selección de motor por petición** o endpoints multi-motor:
  descartados del curso. En v4 el motor se elige UNA vez, al arrancar.
- Cambios de contrato: ningún endpoint nuevo, ningún campo nuevo (salvo
  `motor` en el diagnóstico).

## 3. Requisitos funcionales

### RF1 — La fábrica de repositorios
- `IFabricaRepositorios`: una interfaz con 11 métodos `CrearRepositorioX()`
  (una por rebanada). Quien la implementa decide el motor de TODAS.
- `FabricaSqlServer` y `FabricaPostgres`: cada una entrega los 11
  repositorios de su dialecto, con su cadena de conexión.
- El ensamblador (`Program.cs`) elige la fábrica UNA vez según la
  configuración y registra los 11 repositorios pidiéndoselos a ella.
  **Los servicios no cambian ni una letra.**

### RF2 — El motor por configuración (el interruptor)
- Clave de configuración `Motor`: `sqlserver` | `postgres` (cualquier otro
  valor → la API no arranca, con mensaje claro).
- En Docker la fija la variable de entorno `Motor` del compose, que lee
  `${MOTOR_BD:-postgres}`: **por defecto la v4 habla con PostgreSQL** (el
  motor nuevo a la vista), y `MOTOR_BD=sqlserver` vuelve al primero — sin
  tocar código, sin recompilar.
- Dos cadenas de conexión conviven en la configuración (`SqlServer` y
  `Postgres`); cada fábrica usa la suya.

### RF3 — El segundo motor completo
- Servicio `postgres` (PostgreSQL 16) en el compose, puerto publicado
  **15462**, con `db/bdfacturas_postgres.sql`: las MISMAS 12 tablas, las
  MISMAS semillas (mismos ids), el MISMO trigger de totales/stock y los
  MISMOS SPs de factura — en dialecto PostgreSQL
  ([5_data_model](5_data_model.md)).
- Los 11 `RepositorioXPostgres` (Npgsql): mismos contratos de interfaz,
  mismo SQL parametrizado (dialecto aparte: `LIMIT` en vez de `TOP`),
  BCrypt sigue viviendo SOLO en el repositorio de usuario.
- El de factura llama los SPs con `CALL` y traduce los `RAISE EXCEPTION`
  (SQLSTATE `P0001`) a las MISMAS excepciones de negocio: "no existe" →
  404 · "ya está anulada" → 409 · el resto (stock, mínimo) → 500.

### RF4 — Diagnóstico
`GET /` → `{mensaje, version: "v4", motor: "postgres"|"sqlserver",
contratos}`. El campo `motor` es la única adición visible del contrato.

## 4. Requisitos no funcionales

- **RNF1 — Los de v1 a v3 siguen todos** (capas, sin ORM, SQL
  parametrizado, async, errores uniformes, 422 con `errores[]`, el secreto
  nunca viaja).
- **RNF2 — La frontera es el repositorio:** el diff de la v4 NO toca
  Controllers/, Servicios/, Peticiones/, Modelos/ ni Excepciones/. Si algo
  de ahí "necesitara" cambiar, la v4 está mal planteada.
- **RNF3 — Paridad de semillas:** ambos motores arrancan con datos
  idénticos (mismos ids, mismos stocks) — el smoke test es EL MISMO.
- **RNF4 — Sin anticipación:** nada de MariaDB (v5) ni selección dinámica
  de motor por petición (v6).

## 5. Criterios de aceptación

1. **Regresión total contra PostgreSQL (motor por defecto):** `docker
   compose up -d --build` y los smoke tests COMPLETOS de
   [v1](../v1_producto_sqlserver/7_quickstart.md) §2,
   [v2](../v2_persona_factura/7_quickstart.md) §3 y
   [v3](../v3_resto_entidades/7_quickstart.md) §3 pasan tal cual (solo
   cambia el diagnóstico: `"version":"v4"`, `"motor":"postgres"`).
2. **El interruptor:** `MOTOR_BD=sqlserver` + recrear SOLO la API → el
   diagnóstico dice `"motor":"sqlserver"` y la MISMA regresión total pasa
   contra SQL Server. Ni una recompilación del código fuente.
3. **Los errores de negocio son idénticos en ambos motores:** factura 999
   → 404 · doble anulación → 409 · stock insuficiente → 500 con el mensaje
   del motor · FK/UNIQUE/PK violadas → 500. (El `detalle` puede variar en
   redacción — el `estado` y el `mensaje` no.)
4. **El diff respeta la frontera:** `git diff v3 --stat` solo toca
   `Fabricas/`, `Repositorios/*Postgres.cs`, `Program.cs`,
   `ApiFacturas.csproj`, `docker-compose.yml`, `appsettings.json`,
   `db/bdfacturas_postgres.sql`, `pruebas/`, `postman/` y `docs/`.
5. **Prueba de capas ampliada:** `dotnet run --project pruebas` verifica
   además que cada fábrica entrega los repositorios de SU motor — sin
   abrir una sola conexión (las fábricas construyen, no conectan).

## 6. Definición de TERMINADA

Los 5 criterios pasan → commit + tag `v4` → la API es bi-motor → recién
entonces se especifica la v5 (MariaDB: la fábrica pagará su promesa).
