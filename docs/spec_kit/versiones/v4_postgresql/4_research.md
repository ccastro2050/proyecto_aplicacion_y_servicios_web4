# Research — Versión 4: decisiones y alternativas

> Lectura opcional: el PORQUÉ de cada decisión del [plan](3_plan.md).

---

## D1 — ¿Fábrica abstracta, o un `switch` en cada registro?

**Alternativas:** (a) un `switch (motor)` dentro de cada uno de los 11
`AddScoped` · (b) inyección con llaves de .NET (keyed services) · (c) una
**fábrica abstracta**: interfaz con 11 métodos, una implementación por
motor.

**Decisión: (c).** Con (a), la decisión del motor se repite 11 veces (y en
la v5 serían 33 ramas). Con (b), la magia del contenedor esconde el patrón
que el curso quiere ENSEÑAR. La fábrica es el patrón clásico GoF visible
en dos archivos leíbles: agregarle un motor = una clase + un case. El
costo (11 métodos "aburridos" en la interfaz) ES la lección: la fábrica
promete la familia COMPLETA de repositorios, no uno suelto.

## D2 — Npgsql como proveedor

Es el proveedor ADO.NET oficial y estándar de PostgreSQL para .NET, con
las MISMAS clases conceptuales (`NpgsqlConnection`/`Command`/`DataReader`)
que ya se dominan de `Microsoft.Data.SqlClient`. La traducción de 10 de
los 11 repositorios queda mecánica — eso también es didáctico: el
conocimiento de ADO.NET se transfiere entre motores.

## D3 — SPs como `PROCEDURE` + `INOUT json` + `CALL`

**Alternativas:** (a) funciones `RETURNS json` con `SELECT fn(...)` ·
(b) `PROCEDURE` con `INOUT p_resultado json` invocados con `CALL`.

**Decisión: (b),** por paridad con el proyecto gemelo del curso
(Python + FastAPI + PostgreSQL usa exactamente estos procedimientos) y
porque conserva la forma mental de la v2: "un SP que deja su resultado en
un parámetro de salida". Matiz técnico: en PostgreSQL el `CALL` devuelve
los INOUT como UNA fila de resultado → `ExecuteScalarAsync()` la lee (en
SQL Server era un parámetro OUTPUT).

## D4 — Traducir errores por SQLSTATE + patrón de mensaje

SQL Server permite `THROW 50003` con número inventariable; PostgreSQL no:
todo `RAISE EXCEPTION` sale con SQLSTATE **`P0001`** (raise_exception).
La traducción del repositorio filtra por `P0001` + patrón del texto
("no existe" → 404, "anulada" → 409). Es MENOS elegante que el número — y
es la lección de dialectos: cada motor da señales distintas y el
repositorio las normaliza para que arriba nadie se entere.

## D5 — ¿Los dos motores arriba a la vez, o perfiles de compose?

**Decisión: ambos siempre arriba.** El interruptor solo recrea la API —
comparar motores toma segundos y el smoke test §4 lo aprovecha. El costo
es RAM (~2 GB de SQL Server + ~50 MB de Postgres — la asimetría también
enseña). Los `profiles` de compose ahorrarían RAM pero convertirían el
interruptor en "bajar un motor y subir otro", más lento y con más estados
posibles a mitad de camino.

## D6 — Motor por defecto: `postgres`

La versión existe para mostrar el motor nuevo: que `up -d` lo muestre
funcionando de una. El default vive en el compose
(`${MOTOR_BD:-postgres}`), no en el código; `appsettings.json` (para
correr la API sin Docker) queda en `sqlserver`, conservando el
comportamiento local de siempre.

## D7 — Puerto 15462

Mapa de puertos del curso: front 8030 · API facturas
8035 · MariaDB 13336 · **PostgreSQL 15462** · SQL Server 11466. La
reconstrucción del estudiante suma 100 (15562).

## D8 — Semillas idénticas, ids idénticos

`db/bdfacturas_postgres.sql` inserta los MISMOS datos con los MISMOS ids
(con `setval` para alinear las secuencias, el equivalente de
`IDENTITY_INSERT`). Consecuencia valiosa: el smoke test de v1-v3 corre
IGUAL en ambos motores — hasta la nota del quickstart v3 sobre el
identity consumido por inserts fallidos aplica igual (las secuencias de
PostgreSQL también avanzan cuando el INSERT falla).
