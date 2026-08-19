# Tareas — Versión 4: orden de construcción por fases verificables

> Cada fase termina en un estado COMPROBABLE. No avance con una fase en
> rojo. El detalle de diseño está en [3_plan.md](3_plan.md).

---

## Fase 0 — Punto de partida

- [ ] La v3 corre y pasa su smoke test (tag `v3` presente).
- [ ] `git diff v3` limpio (se parte de la versión cerrada).

**Verificar:** diagnóstico responde `"version":"v3"`.

## Fase 1 — El motor nuevo en el compose (sin tocar la API)

- [ ] `db/bdfacturas_postgres.sql`: la MISMA bdfacturas en dialecto
      PostgreSQL ([5_data_model.md](5_data_model.md)) — 12 tablas,
      trigger, SPs, semillas con `setval`.
- [ ] `docker-compose.yml`: servicio `postgres` (16-alpine, puerto
      15462, volumen `pgdata`, script montado en
      `/docker-entrypoint-initdb.d/`, healthcheck `pg_isready`).
- [ ] `docker compose up -d` — la API sigue en v3 y sigue hablando con
      SQL Server: **nada se rompe por agregar un contenedor**.

**Verificar:**
```powershell
docker compose exec postgres psql -U postgres -d bdfacturas_postgres_local -c "\dt"           # 12 tablas
docker compose exec postgres psql -U postgres -d bdfacturas_postgres_local -c "SELECT COUNT(*) FROM producto"   # 8
```

## Fase 2 — Los repositorios Postgres (el calco mecánico)

- [ ] Paquete **Npgsql** en `ApiFacturas.csproj` (+ recrear el contenedor
      para que restaure).
- [ ] Los 10 repositorios calcados (todos menos factura): tabla de
      traducción del [plan §2](3_plan.md) — `Npgsql*`, `LIMIT` al final,
      resto idéntico. BCrypt intacto en el de usuario.
- [ ] `RepositorioFacturaPostgres`: `CALL` + `ExecuteScalarAsync()` +
      traducción por `P0001` y patrón ([plan §3](3_plan.md)).

**Verificar:** compila (`docker compose logs api-facturas` sin errores).
Nada los usa todavía — el ensamblador sigue en SQL Server.

## Fase 3 — La fábrica

- [ ] `Fabricas/IFabricaRepositorios.cs` (11 métodos `CrearRepositorioX`).
- [ ] `Fabricas/FabricaSqlServer.cs` y `Fabricas/FabricaPostgres.cs`.
- [ ] `pruebas/Programa.cs`: criterio 5 (cada fábrica entrega SU dialecto,
      con cadenas de mentira — construir no conecta).

**Verificar:** `docker compose exec api-facturas dotnet run --project
pruebas` → todos los criterios OK.

## Fase 4 — El ensamblador con interruptor

- [ ] `appsettings.json`: cadena `Postgres` + clave `Motor`
      (default local `sqlserver`).
- [ ] `docker-compose.yml`: variables `Motor: ${MOTOR_BD:-postgres}` y
      `ConnectionStrings__Postgres` en la API.
- [ ] `Program.cs`: el switch de fábricas + los 11 registros vía fábrica
      + diagnóstico `"version":"v4"` con `"motor"`.

**Verificar:** `GET /` → `"motor":"postgres"` · con
`$env:MOTOR_BD="sqlserver"` y recrear la API → `"motor":"sqlserver"`.

## Fase 5 — Verificación total y cierre

- [ ] **Regresión doble completa** ([7_quickstart.md](7_quickstart.md)
      §2): v1+v2+v3 contra postgres, interruptor, v1+v2+v3 contra
      sqlserver.
- [ ] Errores de negocio idénticos en ambos motores (criterio 3).
- [ ] `git diff v3 --stat` respeta la frontera (criterio 4).
- [ ] Colección Postman: nota de la v4 (mismos endpoints, campo `motor`).
- [ ] Commit + tag `v4` + push.

**Verificar:** los 5 criterios de [2_spec.md](2_spec.md) §5 en verde.
