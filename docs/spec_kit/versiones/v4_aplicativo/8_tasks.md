# Tareas — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la capa de consultas y el tablero |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas: cero tablas nuevas |
> | [6_contracts.md](6_contracts.md) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | **8_tasks.md** (este) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

> **Cada fase termina en algo que se puede comprobar.** No se pasa a la
> siguiente sin eso.

## Fase 1 — Los modelos

- [ ] `Modelos/Consultas.cs`: **diez `record`**, uno por consulta, con el
      nombre de cada propiedad diciendo qué trae.
- [ ] **No** un `Dictionary<string, object>`: ver [3_plan.md](3_plan.md) §2.

## Fase 2 — La interfaz del repositorio

- [ ] `Repositorios/IRepositorioConsultas.cs`: diez firmas `Task<List<T>>`.

## Fase 3 — El SQL

- [ ] `Repositorios/RepositorioConsultasSqlServer.cs`: las diez consultas.
- [ ] **Cada una cruza 4 tablas o más.** Las 6 y 9 con `LEFT JOIN`.
- [ ] **`CAST(… AS INT)`** en todas las columnas de conteo: ver
      [4_research.md](4_research.md) §D4.
- [ ] Los **alias** de columna coincidiendo con las propiedades del `record` —
      Dapper mapea **por nombre**, y sin alias el campo llega vacío **sin
      error**.

**Verificación:** las diez consultas corren en un cliente SQL y devuelven filas.

## Fase 4 — El servicio, el controlador y el cableado

- [ ] `Servicios/IServicioConsultas.cs` + `ServicioConsultas.cs` (sin reglas, y
      existe igual).
- [ ] `Controllers/ConsultasController.cs`: `[Route("api/consultas")]`,
      `[Authorize]`, `[ExigePermiso("interfaz.inicio")]` y diez acciones.
- [ ] **El registro en `Program.cs`** — y esta casilla es la que más se olvida:

```csharp
builder.Services.AddScoped<IRepositorioConsultas>(_ => fabrica.CrearRepositorioConsultas());
builder.Services.AddScoped<IServicioConsultas, ServicioConsultas>();
```

> **Sin esa línea el proyecto COMPILA**, arranca, y el endpoint responde
> **500 «Unable to resolve service»** el día que alguien lo pide. El
> compilador no puede atraparlo.

- [ ] Y si el proyecto tiene **fábrica**: `CrearRepositorioConsultas()` en la
      interfaz **y en las dos implementaciones**.

**Verificación:** las diez responden 200 con token. [7_quickstart.md](7_quickstart.md) §2.

## Fase 5 — El servicio del front

- [ ] `Servicios/ServicioConsultas.cs`: `private const string Ruta = "api/consultas"`
      y diez métodos, uno por consulta.
- [ ] Registrarlo en `Program.cs` con su `HttpClient` — mismo patrón que los demás.

**Verificación:** el proyecto compila y el servicio se resuelve.

## Fase 6 — El tablero

- [ ] `Components/Pages/Tablero.razor` con `@page "/tablero"`.
- [ ] Las diez con **`Task.WhenAll`**, no una tras otra.
- [ ] Si una falla, **las otras nueve se dibujan**, y el aviso dice **cuál**.

## Fase 7 — El menú

- [ ] La entrada **de primera** en el `MenuApp` de `Program.cs`, con permiso
      `interfaz.inicio`.

## Fase 8 — La regresión y el cierre

- [ ] El smoke test de la v1, el de la v2 y el de la v3, **completos**.
- [ ] Los 8 criterios de [2_spec.md](2_spec.md) §3.
- [ ] Commit + tag `v4` + push.

## Lo que NO se hace en esta versión

| | Por qué |
|---|---|
| **Caché de resultados** | Ver [3_plan.md](3_plan.md) §4 |
| **Una librería de gráficos** | Ver [3_plan.md](3_plan.md) §3.3 |
| **La marca, las páginas corporativas, la PWA y la publicación** | **Pendientes**, declaradas en [2_spec.md](2_spec.md) §5 |
