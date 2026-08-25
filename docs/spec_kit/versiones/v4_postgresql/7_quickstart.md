# Quickstart — Versión 4: arranque y la regresión DOBLE

> **Versión 4** · Validación rápida de la versión ya construida. Si aún no
> hay nada construido, empiece por [8_tasks.md](8_tasks.md).

---

## 1. Arranque (igual que siempre — ahora con 4 servicios)

```powershell
docker compose up -d --build
```

Al final: `sqlserver` (healthy), `sqlserver-init` (Exited 0), **`postgres`
(healthy — se inicializa SOLO: PostgreSQL sí ejecuta los scripts
montados)** y `api-facturas` arriba. Primera compilación: ~30-60 s.

## 2. La regresión doble (criterios 1 y 2 — el corazón de la v4)

### 2a. TODO contra PostgreSQL (el motor por defecto)

```powershell
curl.exe http://localhost:8035/     # → "version":"v4", "motor":"postgres"
```

Correr COMPLETOS los smoke tests de la
[v1](../v1_producto_sqlserver/7_quickstart.md) §2, la
[v2](../v2_persona_factura/7_quickstart.md) §3 y la
[v3](../v3_resto_entidades/7_quickstart.md) §3. **Pasan tal cual** —
mismos ids, mismos stocks, mismos 404/409/422/500 (los ids de la nota de
la v3 incluidos: las secuencias de PostgreSQL también se consumen en los
inserts fallidos).

### 2b. El interruptor: los MISMOS tests contra SQL Server

```powershell
$env:MOTOR_BD = "sqlserver"
docker compose up -d api-facturas       # recrea SOLO la API (segundos)
curl.exe http://localhost:8035/         # → "motor":"sqlserver"
```

Correr la MISMA regresión completa. Pasa igual. **Eso** — ninguna línea
de código cambió entre 2a y 2b — es la demostración de que las capas
eran verdad.

> ⚠️ Ambos motores parten de la MISMA semilla pero cada uno guarda lo
> suyo: lo que usted creó en 2a vive solo en PostgreSQL. Si repitió los
> POST en ambos motores y quiere el estado semilla exacto:
> `docker compose down -v && docker compose up -d`.

Para volver al default (PostgreSQL):

```powershell
Remove-Item Env:MOTOR_BD
docker compose up -d api-facturas
```

## 3. Los errores de negocio en el motor nuevo (criterio 3)

Con `motor=postgres` (ya cubiertos por la regresión — aquí los tres
emblemáticos, para verlos de cerca):

```powershell
curl.exe -i http://localhost:8035/api/factura/999                 # → 404 "Factura 999 no existe"
curl.exe -i -X POST http://localhost:8035/api/factura -H "Content-Type: application/json" -d "{\"fkidcliente\":1,\"fkidvendedor\":1,\"productos\":[{\"codigo\":\"PR001\",\"cantidad\":9999}]}"   # → 500 "Stock insuficiente…"
# (anule dos veces cualquier factura creada por usted: la segunda → 409)
```

## 4. La frontera del diff (criterio 4)

```powershell
git diff v3 --stat
```

NADA de `Controllers/`, `Servicios/`, `Peticiones/`, `Modelos/` ni
`Excepciones/` aparece en la lista. La v4 vive de repositorios hacia
abajo (+ el ensamblador, que para eso existe).

## 5. La prueba de capas (criterio 5)

```powershell
docker compose exec api-facturas dotnet run --project pruebas
# → … CRITERIO 5 OK: cada fábrica entrega los repositorios de su motor, sin abrir conexiones
```

## 6. Si algo falla

| Síntoma | Causa probable |
|---|---|
| Los de v1/v2/v3 | Aplican todos igual (sus quickstarts) |
| `postgres` no queda healthy | Puerto 15462 ocupado, o el volumen quedó de un intento fallido: `docker compose down -v` y de nuevo |
| La API arranca pero todo da 500 con `motor=postgres` | ¿El script `db/bdfacturas_postgres.sql` corrió? Solo se auto-ejecuta si el volumen estaba VACÍO — `docker compose down -v && up -d` |
| `GET /` dice un motor y usted esperaba el otro | La variable `MOTOR_BD` quedó fija en su sesión de PowerShell: `Remove-Item Env:MOTOR_BD` y recree la API |
| "Motor desconocido: …" en los logs | Valor inválido en `MOTOR_BD` (solo `sqlserver` o `postgres`) |
| Factura da 500 en vez de 404/409 en postgres | El repositorio no está filtrando por SQLSTATE `P0001` + patrón — ver [3_plan.md](3_plan.md) §3 |
