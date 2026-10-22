# Arquitectura — los procesos y el despliegue

> **Qué es este documento.** Cuántos procesos hay, cómo se hablan, por qué
> **esto no es un monolito** —ni microservicios—, y qué implica el front que se
> escogió.
>
> **A qué plantilla alimenta:** el `3_plan`.
>
> **Por qué está separado de las capas.** Porque son **dos lectores**: el plan
> necesita los procesos y el despliegue; la constitución necesita las capas y
> sus prohibiciones, que están en
> [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md). Ver
> [`DOMINIO_PARA_GENERAR.md`](../../DOMINIO_PARA_GENERAR.md).
>
> **Material académico simulado**, pero el reparto es real y se puede comprobar
> abriendo las carpetas.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. Tres procesos

| Proceso | Qué es | Puerto |
|---|---|---|
| **`front-blazor`** | Blazor Server. El único que le habla a la persona | 8099 |
| **`api-facturas`** | ASP.NET Core. El único que habla con la base de datos | 8035 |
| **`sqlserver`** | El motor, con **las reglas del negocio adentro** | 11466 |

En la v5 se le suma **`postgres`** (15462), y el interruptor `MOTOR_BD` decide
cuál de los dos atiende.

> **La base de datos NO es un tercero ajeno: es parte del sistema**, y de las
> tres la que más código propio tiene. El detalle está en
> [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §4.

---

## 2. ¿Esto es un monolito?

**Es la pregunta que más se responde mal, y casi siempre por el mismo motivo:
porque todo está en un repositorio.**

> **Un repositorio no es una unidad de despliegue.** El repositorio dice **cómo
> se guarda** el código; la arquitectura dice **cómo se ejecuta**. Son dos cosas
> distintas, y tener todo junto en GitHub se llama **monorepo** — no monolito.

### Las tres palabras, separadas

| | Qué significa | ¿Aplica aquí? |
|---|---|---|
| **Monolito** | **UNA** sola unidad desplegable: todo se compila, se despliega y se cae junto | **Sí** a la API · **Sí** al front · **No** al sistema |
| **Monorepo** | **UN** solo repositorio, con varias unidades adentro | **Sí** |
| **Microservicios** | Muchos servicios pequeños, cada uno con **su propia base**, descubrimiento y despliegue independiente | **No** |

### La respuesta, en una línea

> **La API es un monolito. El front TAMBIÉN. El sistema no.**
>
> Las tres cosas son ciertas **a la vez**, y confundir el nivel es de donde sale
> el error. «Monolito» se predica de **una unidad desplegable** — no de un
> repositorio, no de un sistema entero.

**Por qué la API es un monolito:** es **una** unidad. Sus tres capas y sus
quince controladores viven en el mismo proceso, se compilan juntos y se
despliegan juntos. Si hay que cambiar una línea de `ProductoController`, se
vuelve a desplegar **toda** la API.

**Por qué el front también:** exactamente lo mismo. Sus **15 pantallas** son un
solo proyecto, un solo contenedor y un solo despliegue. Cambiar un color obliga a
volver a publicarlo entero.

**Y por qué el sistema no lo es:** porque son **DOS monolitos**, no uno. Se
compilan por separado, se despliegan por separado, **se caen por separado** —
apague la API y el front sigue en pie— y se hablan **solo por HTTP**.

> **Y aquí está lo que de verdad hay que llevarse: «monolito» no es un insulto.**
> Casi todo software empieza siendo uno, y para este tamaño es **la decisión
> correcta**. Lo que importa no es si algo es monolito, sino **cuántas unidades
> desplegables hay y cómo se hablan**.
>
> Un monolito bien ordenado por dentro —en capas, con interfaces— se llama
> **monolito modular**, y es lo que son estos dos. Partirlos en servicios
> pequeños antes de necesitarlo solo agrega problemas de red a un sistema que
> todavía no los tenía.

**Y por qué no son microservicios:** dos monolitos no son microservicios. Faltan
todas las señas: no hay una base por servicio —hay **una** base compartida—, no
hay descubrimiento, no hay colas, y no hay despliegue independiente de partes de
la API ni del front.

### El nombre que sí le queda

**Arquitectura de tres niveles** *(three-tier)*:

| Nivel | Proceso | Qué decide |
|---|---|---|
| **Presentación** | `front-blazor` | **nada**: pregunta y obedece |
| **Aplicación** | `api-facturas` | la forma de la petición y el flujo |
| **Datos** | `sqlserver` / `postgres` | **el stock, el total, la anulación** |

> **Y aquí hay una diferencia con el three-tier de manual que vale la pena
> notar:** en el libro, el nivel de datos **guarda y ya**. En este sistema
> **defiende las reglas** — por eso la pregunta *«¿dónde va esta validación?»*
> tiene **tres** respuestas posibles y no dos. Ver
> [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §3.

### Cómo se comprueba, que es lo que lo vuelve un hecho

```powershell
# 1 · DOS unidades desplegables propias (y un proyecto de pruebas).
Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch 'obj' }

# 2 · Se despliegan por separado: apague UNA y la otra sigue.
docker compose stop api-facturas
Start-Process http://localhost:8099/facturas
#    El front sigue en pie, con su menú y sin una sola fila.
#    En un monolito eso no se puede: se cae todo junto.

# 3 · Se hablan SOLO por HTTP. Esto tiene que dar 0.
Select-String -Path front_blazor\**\*.cs,front_blazor\**\*.razor `
  -Pattern 'SqlConnection|Npgsql' | Measure-Object | Select-Object Count
```

> **El paso 2 es la prueba.** Si apagar la API tumbara también el front, serían
> una sola unidad — y entonces sí sería un monolito, por más carpetas separadas
> que tuviera.

---

## 3. El front: por qué Blazor Server y qué implica

| | |
|---|---|
| **Qué es** | El componente vive **en el servidor**; el navegador recibe HTML y mantiene un **circuito** abierto |
| **Qué gana** | No hay que escribir JavaScript para la interacción, y el estado de la pantalla vive en C# |
| **Qué cuesta** | Si el circuito se corta —se recarga la página, se pierde la red— **la pantalla pierde su estado** |

> **Eso último se nota al emitir una factura:** los renglones que se van
> agregando viven **en el circuito**, no en la base de datos. Oprimir F5 a mitad **los
> pierde**.
>
> **No es un defecto: es la consecuencia de haber escogido Blazor Server.** Si
> hiciera falta que el borrador sobreviviera, habría que guardarlo en otra parte
> —la sesión, el navegador o la base de datos—, y eso es trabajo que esta versión no
> hizo. Está declarado, sin construir, en
> [`3_HISTORIAS_PROPUESTAS.md`](elicitacion/3_HISTORIAS_PROPUESTAS.md).
