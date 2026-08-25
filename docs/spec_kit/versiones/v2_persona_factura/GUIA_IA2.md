# Cómo construir la VERSIÓN 2 con IA — sobre su proyecto de la v1

> Guía de la **v2** (las versiones son acumulativas: la v2 se construye
> **encima de su proyecto de la v1 terminado** — no se parte de cero ni se
> reescribe nada de producto). El método general — los dos caminos (chat
> web / IDE agéntico), cómo pegar archivos, a qué carpeta va cada comando,
> los hábitos de la conversación — es el MISMO de la
> [guía de la v1](../v1_producto_sqlserver/GUIA_IA1.md): aquí está lo que cambia para la v2.

---

## 0. Punto de partida (verifíquelo ANTES de abrir la IA)

Su proyecto `mi_v1_producto/` con la **v1 funcionando**: el smoke test de
la v1 pasa ([7_quickstart de v1](../v1_producto_sqlserver/7_quickstart.md) §2,
con sus puertos +100). Si la v1 no pasa, primero ciérrela — la v2 hereda
sus cimientos, incluidos sus bugs.

La BD **no cambia**: los SPs y triggers que la v2 estrena están en su
`db/bdfacturas.sql` desde la v1. No hay artefactos nuevos que copiar de
`db/`.

## A. Camino del chat web

### A.1 Qué subirle (los 8 archivos de la v2)

| # | Archivo | Papel |
|---|---|---|
| 1 | `docs/spec_kit/1_constitution.md` | Las reglas permanentes (las mismas de la v1) |
| 2 | `docs/spec_kit/versiones/v2_persona_factura/2_spec.md` | QUÉ agrega la v2 y sus criterios |
| 3 | `.../v2_persona_factura/3_plan.md` | CÓMO: los archivos nuevos y el diseño |
| 4 | `.../v2_persona_factura/4_research.md` | Decisiones y alternativas |
| 5 | `.../v2_persona_factura/5_data_model.md` | Las tablas nuevas + los SPs y triggers |
| 6 | `.../v2_persona_factura/6_contracts.md` | Los 10 endpoints nuevos exactos |
| 7 | `.../v2_persona_factura/7_quickstart.md` | Regresión v1 + smoke test v2 |
| 8 | `.../v2_persona_factura/8_tasks.md` | Las fases, en orden |

**No suba los documentos de la v1** (la IA no los necesita: lo que importa
del pasado es su CÓDIGO, y ese se lo pega usted cuando lo pida). Tampoco el
mapa de versiones (la v2 no anticipa la v3).

### A.2 Prepare su proyecto (5 minutos)

Todos los comandos van en la terminal integrada de VS Code (PowerShell),
**parado en la raíz de SU proyecto**.

1. **Cree la CARPETA nueva** (la única de la v2: la de sus specs — las
   carpetas de código ya existen todas desde la v1):

   ```powershell
   mkdir docs\spec_kit\versiones\v2_persona_factura
   ```

2. **Copie las specs de la v2** desde el clon del curso (los 7 `.md` +
   esta guía) — ajuste la primera ruta a donde tenga el clon:

   ```powershell
   Copy-Item ..\proyecto_aplicacion_y_servicios_web2\docs\spec_kit\versiones\v2_persona_factura\* docs\spec_kit\versiones\v2_persona_factura\
   ```

   (También sirve el explorador de Windows: Ctrl+C, Ctrl+V de la carpeta
   completa, como en la v1.)

3. **Cree los ARCHIVOS VACÍOS nuevos** — los 18 que la IA irá llenando:

   ```powershell
   New-Item api_facturas\Modelos\Persona.cs, api_facturas\Modelos\Factura.cs, api_facturas\Modelos\ProductoDeFactura.cs, api_facturas\Peticiones\PersonaCrear.cs, api_facturas\Peticiones\PersonaReemplazo.cs, api_facturas\Peticiones\PersonaActualizar.cs, api_facturas\Peticiones\FacturaCrear.cs, api_facturas\Controllers\PersonaController.cs, api_facturas\Controllers\FacturaController.cs, api_facturas\Servicios\IServicioPersona.cs, api_facturas\Servicios\ServicioPersona.cs, api_facturas\Servicios\IServicioFactura.cs, api_facturas\Servicios\ServicioFactura.cs, api_facturas\Repositorios\IRepositorioPersona.cs, api_facturas\Repositorios\RepositorioPersonaSqlServer.cs, api_facturas\Repositorios\IRepositorioFactura.cs, api_facturas\Repositorios\RepositorioFacturaSqlServer.cs, api_facturas\Excepciones\ConflictoExcepcion.cs
   ```

4. Sepa desde ya qué archivos de la v1 **CRECEN** (la IA le entregará la
   versión completa actualizada y usted REEMPLAZA el contenido):
   `Program.cs` (4 AddScoped nuevos + version "v2") y
   `pruebas/Programa.cs` (el repo falso de persona). **Ningún otro archivo
   de la v1 se toca.**

Los 18 archivos nuevos, con su fase (estructura de
[3_plan.md](3_plan.md) §1):

```
api_facturas/
├── Program.cs                        ★ CRECE en Fase 3 y Fase 6
├── Modelos/         Persona.cs ← F1 · Factura.cs, ProductoDeFactura.cs ← F4
├── Peticiones/      PersonaCrear/Reemplazo/Actualizar.cs ← F1 · FacturaCrear.cs ← F4
├── Controllers/     PersonaController.cs ← F3 · FacturaController.cs ← F6
├── Servicios/       IServicioPersona/ServicioPersona.cs ← F2 · IServicioFactura/ServicioFactura.cs ← F6
├── Repositorios/    IRepositorioPersona/RepositorioPersonaSqlServer.cs ← F2
│                    IRepositorioFactura/RepositorioFacturaSqlServer.cs ← F5
├── Excepciones/     ConflictoExcepcion.cs ← F4
└── pruebas/         Programa.cs ★ CRECE en Fase 2
```

### A.3 El prompt de la v2 (cópielo tal cual como PRIMER mensaje)

Los tres chequeos previos son los de siempre (adjuntos completos, modo
razonamiento ON, búsqueda web OFF — [guía v1](../v1_producto_sqlserver/GUIA_IA1.md) A.3).

```
Actúa como mi asistente de programación para construir la VERSIÓN 2 de un
proyecto universitario. Te adjunto 8 documentos: la constitución (reglas
permanentes) y el spec kit de la versión 2 (spec, plan, research, modelo de
datos, contratos, quickstart y tareas).

El proyecto es C# sobre ASP.NET Core (.NET 10) + SQL Server — así lo fija
3_plan.md. Si en tu respuesta aparece OTRO lenguaje o framework (Python,
Java, Node, PHP…), significa que no leíste los documentos adjuntos: detente
y dímelo en vez de continuar.

CONTEXTO CLAVE — las versiones son acumulativas:
Mi proyecto YA TIENE la versión 1 construida y funcionando según estos
mismos documentos (CRUD de producto con capas e interfaces; su spec está
cerrada). La v2 se construye ENCIMA: NO reescribas, NO "mejores" y NO me
vuelvas a entregar nada de producto. Solo dos archivos existentes crecen
(Program.cs y pruebas/Programa.cs) — de esos me entregarás la versión
completa actualizada. Si para algo necesitas ver mi código actual de la v1,
pídemelo y te lo pego.

REGLAS DE TRABAJO (no negociables):

1. La especificación manda. No agregues NADA que los documentos no pidan:
   ni CRUD de cliente/vendedor, ni endpoints de editar/borrar factura, ni
   frameworks, ni fábricas "por si acaso". Si crees que falta algo,
   pregúntame antes.
2. Vamos a seguir 8_tasks.md FASE POR FASE, en orden. En cada fase:
   a. Me explicas en 3-5 líneas qué vamos a hacer y por qué.
   b. Me entregas los archivos DE A UNO: la ruta exacta y el contenido
      COMPLETO de UN archivo (con los comentarios didácticos en español
      que exige la constitución). Esperas mi "listo" y sigues con el
      siguiente.
   c. Al cerrar la fase me dices su comando de verificación y qué salida
      esperar (correrla en el momento es opcional).
   NOTA: los archivos nuevos YA EXISTEN VACÍOS en mi proyecto — no me des
   comandos para crearlos.
3. Los errores NO nos frenan: te pego el error, me das el archivo completo
   corregido; si no sale rápido, seguimos y lo retomamos al final con el
   smoke test de 7_quickstart.md.
4. El código debe cumplir 6_contracts.md al pie de la letra — incluidos el
   409 de anular dos veces y la regla de que la API NUNCA calcula
   subtotales, totales ni stock (eso lo hacen los SPs y triggers de la BD).
5. Todo en español: nombres, comentarios y mensajes. C# sobre ASP.NET Core
   (.NET 10).
6. Yo trabajo en Windows con VS Code (terminal PowerShell) y Docker
   Desktop. Dame los comandos para ese entorno.
7. Mi proyecto corre con los puertos de la v1 desplazados +100 (convivo
   con el clon del curso): API en 8135 y SQL Server en 11566, con
   name: mi_v1_producto en el compose. Cuando me des URLs o comandos de
   prueba, usa localhost:8135 y localhost,11566.

Al final, la versión 2 está TERMINADA solo cuando: (a) el smoke test de la
V1 sigue pasando completo (regresión — no rompimos nada), y (b) los 6
criterios de aceptación de 2_spec.md de la v2 pasan con el smoke test de
7_quickstart.md.

Empieza: resume en máximo 10 líneas qué vamos a construir y sobre qué base
(para confirmar que entendiste que la v1 ya existe), y arranca con la Fase 0.
```

### A.4 El método (lo nuevo respecto a la v1)

Los hábitos son los de la [guía v1](../v1_producto_sqlserver/GUIA_IA1.md) A.4 (pegar y "listo",
no atascarse en errores, reiniciar el chat si responde en otro lenguaje o
pierde contexto). Además, para la v2:

1. **Proteja la v1.** Si la IA le entrega un archivo de producto
   "mejorado", no lo pegue: "eso es de la v1 cerrada, no se toca" (regla
   del CONTEXTO CLAVE). Las únicas excepciones: `Program.cs` y
   `pruebas/Programa.cs`.
2. **Cuando pida ver código v1, péguelo completo** (típicamente
   `Program.cs`, `ProductoController.cs` o el `Programa.cs` de pruebas —
   los necesita para calcar el estilo y para entregar los que crecen).
3. **El cierre es doble**: primero la REGRESIÓN de la v1 (su smoke test
   completo) y luego el smoke test de la v2 — en ese orden. Con sus
   puertos: donde el quickstart diga `8035` use `8135`.

## B. Camino del IDE agéntico

Preparación como en la [guía v1](../v1_producto_sqlserver/GUIA_IA1.md) B.1, con una diferencia:
abra el IDE **sobre su proyecto de la v1** (que ya tiene código) y copie
antes la carpeta `v2_persona_factura` de specs (paso A.2.1).

```
Construye la VERSIÓN 2 de este proyecto. Las versiones son acumulativas:
este proyecto YA TIENE la v1 construida y funcionando (CRUD de producto);
NO la modifiques — solo Program.cs y pruebas/Programa.cs crecen.

Primero lee, en este orden: docs/spec_kit/1_constitution.md y los 7
documentos de docs/spec_kit/versiones/v2_persona_factura/ (2_spec a
8_tasks). Puedes leer el código v1 existente para calcar su estilo.
Después resume en máximo 10 líneas qué vas a construir y sobre qué base, y
espera mi confirmación antes de tocar nada. docs/spec_kit/ es solo lectura.
La BD ya está en db/ desde la v1 — no toques SQL.

REGLAS (no negociables):
1. La especificación manda: nada que los documentos no pidan (ni CRUD de
   cliente/vendedor, ni editar/borrar factura). Ante la duda, pregunta.
2. Sigue 8_tasks.md FASE POR FASE; al cerrar cada fase EJECUTA su
   verificación, muéstrame el resultado real y espera mi OK.
3. Cumple 6_contracts.md al pie de la letra (el 409 del doble anular
   incluido); la API NUNCA calcula subtotales/total/stock — eso es de los
   SPs y triggers.
4. Todo en español, C# (.NET 10), comentarios didácticos.
5. Cierre doble: la regresión de la v1 (su quickstart completo) y el smoke
   test de la v2 (7_quickstart.md §3), con evidencia de los 6 criterios.
```

La supervisión es la de la [guía v1](../v1_producto_sqlserver/GUIA_IA1.md) B.3, más una alarma
nueva: **si el diff toca archivos de producto** (distintos de `Program.cs`
y `pruebas/Programa.cs`), recháselo — la v1 está cerrada.

## Por qué así (la lección de la v2)

En la v1 la lección era dirigir a la IA **desde cero** con una spec. En la
v2 la lección es la de la vida real: casi nunca se parte de cero — se
agrega sobre un sistema vivo SIN romperlo. Por eso el prompt protege la v1,
por eso el cierre empieza por la regresión, y por eso la spec de la v2 solo
describe el DELTA: lo acumulado ya tiene dueño (las specs cerradas de las
versiones anteriores).
