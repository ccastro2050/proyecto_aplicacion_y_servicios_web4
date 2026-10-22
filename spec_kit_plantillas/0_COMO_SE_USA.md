# Cómo se usan estas plantillas

> **Qué hay en esta carpeta.** Los diez documentos del spec kit **vacíos de
> dominio y llenos de estructura**: sus secciones, sus tablas con los
> encabezados puestos, y marcas `{{ASI}}` donde va lo que cambia de un
> proyecto a otro.
>
> **No se llenan a mano.** Las llena una IA, un documento por conversación,
> siguiendo [`../generador_spec_kit.md`](../generador_spec_kit.md).
>
> Versión 1.0 · 22 de octubre de 2026.

---

## 1. Lo primero: estas plantillas no traen dominio

Están deliberadamente sin ejemplos del sistema de facturación del curso. Si
los trajeran, lo que pasa es esto: alguien genera su spec kit, la IA se apoya
en el ejemplo que tiene a la vista, y el documento termina hablando de
clientes y productos **aunque el proyecto sea de gestión profesoral**. Se nota
en la sustentación, y es incómodo de arreglar tarde.

**Cuando necesite ver uno lleno de verdad**, el ejemplo completo del curso
está en
[`../docs/spec_kit/versiones/v4_aplicativo/`](../docs/spec_kit/versiones/v4_aplicativo/),
con sus cuatro versiones. Ábralo al lado de la plantilla: eso es exactamente
para lo que está.

---

## 2. Las marcas, y qué va en cada una

| Marca | Qué se pone | Ejemplo del curso |
|---|---|---|
| `{{N}}` | El número de la versión | `4` |
| `{{TITULO_VERSION}}` | Cómo se llama esa versión, en palabras | `el aplicativo completo` |
| `{{SISTEMA}}` | Qué sistema es | `gestión profesoral` |
| `{{BD}}` | El nombre de la base de datos | `bdfacturas` |
| `{{MOTOR}}` | El motor | `SQL Server` |
| `{{TABLA}}` · `{{RECURSO}}` | Una tabla o un recurso de la API | `factura` · `producto` |
| `{{PUERTO_API}}` · `{{PUERTO_FRONT}}` | Los puertos de su proyecto | `8035` · `8099` |
| `{{OPERACIONES_ANTERIORES}}` | Cuántas operaciones traía la versión pasada | `70` |
| `{{OPERACIONES_NUEVAS}}` | Cuántas suma esta | `10` |
| `{{INTEGRANTE}}` | El nombre de quien escribe su guía | `Carlos` |
| `{{...}}` suelto | Cualquier otra cosa que la plantilla pida | — |

> **Si al terminar queda una marca sin reemplazar, el documento no está
> listo.** Es la comprobación más fácil de todas: buscar `{{` en el archivo.

---

## 3. La regla que sostiene todo esto

> **La IA no inventa requisitos.** Si su documentación de dominio no dice
> cuánto dura una sesión, el documento sale con
> `[NECESITA ACLARACIÓN: duración de la sesión]` — **no con «una hora»**.

Un `[NECESITA ACLARACIÓN]` es una pregunta que usted le lleva al usuario
experto en la próxima reunión: es trabajo útil. Un dato inventado es una
afirmación que nadie va a revisar hasta que falle.

Y esto no es una ocurrencia del curso. Spec Kit trae el marcador
`NEEDS CLARIFICATION` en su propia plantilla de especificación, con un comando
dedicado a resolverlos (`/speckit.clarify`) y con su Fase 0 obligada a
cerrarlos todos antes de diseñar.

**La primera compuerta del método, entonces:** ningún spec kit se cierra con un
`[NECESITA ACLARACIÓN]` pendiente.

---

## 4. Cómo se llama cada documento en Spec Kit oficial

Nuestros documentos llevan prefijo numérico; **los de Spec Kit no.** Conviene
saberlo antes de buscar en la documentación oficial y no encontrar nada.

| El nuestro | El oficial | Comando | Fase |
|---|---|---|---|
| `1_constitution.md` | `.specify/memory/constitution.md` | `/speckit.constitution` | fuera de las fases: rige todas |
| `2_spec.md` | `spec.md` | `/speckit.specify` | — |
| `3_plan.md` | `plan.md` | `/speckit.plan` | — |
| `4_research.md` | `research.md` | `/speckit.plan` | **Fase 0** |
| `5_data_model.md` | `data-model.md` | `/speckit.plan` | **Fase 1** |
| `6_contracts.md` | `contracts/` — una **carpeta** | `/speckit.plan` | **Fase 1** |
| `7_quickstart.md` | `quickstart.md` | `/speckit.plan` | **Fase 1** |
| `8_tasks.md` | `tasks.md` | `/speckit.tasks` | **Fase 2** |
| `9_checklist.md` | `checklists/<dominio>.md` — **varios** | `/speckit.checklist` | — |
| `GUIA_IA{{N}}.md` | *no tiene equivalente* | — | — |

**Lo que Spec Kit sí numera** es la carpeta de la funcionalidad
(`specs/001-nombre/`), las tres fases del plan (0, 1 y 2) y los artículos de la
constitución (I a IX). Los archivos, no.

**El prefijo lo usamos nosotros por dos razones concretas:** ordena la carpeta
en el explorador, y le dice a quien llega por dónde empezar. Lo que no se puede
es creer que Spec Kit numera así.

### Y tres cosas nuestras que no coinciden, dichas de frente

| | Lo nuestro | Lo oficial |
|---|---|---|
| **Contratos** | un archivo, `6_contracts.md` | una carpeta, un archivo por contrato |
| **Listas de verificación** | una, `9_checklist.md` | varias: `ux.md`, `api.md`, `security.md`… |
| **La guía de IA** | `GUIA_IA{{N}}.md`, y las personales | no existe |

Las dos primeras son simplificaciones a propósito: con un solo servicio HTTP,
un archivo de contratos se lee mejor que una carpeta con uno dentro. La tercera
es **nuestra de verdad**, y es justamente la pieza que el curso evalúa: los
prompts que se usaron y lo que hubo que corregirle a la IA.

Spec Kit tiene además **diez comandos**; aquí se usan cuatro. Los otros seis
—`clarify`, `analyze`, `implement`, `converge`, `taskstoissues` y el de
plantillas— existen y vale la pena mirarlos cuando este método ya les quede
cómodo.

---

## 5. El bloque de encabezado, que es idéntico en ocho documentos

Los ocho documentos de una versión (`2_spec` a `9_checklist`) empiezan **con el
mismo bloque**: la cinta de la versión, el aviso de que es acumulativa, la
tabla con los nueve documentos de la carpeta, y de dónde sale ese documento en
particular.

Ese bloque está en
[`_ENCABEZADO_COMUN.md`](_ENCABEZADO_COMUN.md), una sola vez. Cada plantilla
lo referencia en vez de repetirlo.

> **Por qué importa que sea idéntico.** Quien abre cualquiera de los ocho ve de
> entrada en qué versión está, qué más hay en la carpeta y de qué documento de
> dominio salió lo que está leyendo. Un spec kit en el que cada archivo arranca
> distinto obliga a reorientarse ocho veces.

---

## 6. El orden en que se llenan, y por qué no es el numérico puro

| Cuándo | Qué | Por qué en ese momento |
|---|---|---|
| **Primero** | `2_spec` | Es el alcance. Todo lo demás lo obedece, así que si sale mal, los siete siguientes salen mal |
| Después | `3_plan` → `4_research` | `4_research` es la **Fase 0** del plan: sirve para resolver lo que no se sabe **antes** de diseñar, no para levantar el acta al final |
| Luego | `5_data_model` → `6_contracts` → `7_quickstart` | La Fase 1: el diseño |
| Y | `8_tasks` → `9_checklist` | La Fase 2 y el cierre |
| **Al final** | `GUIA_IA{{N}}` | **Ésta no se genera: se arma** |

> **La guía de IA no la puede escribir una IA por usted.** Lleva los prompts
> que de verdad usó **y lo que hubo que corregirle** — y eso solo lo sabe quien
> estuvo en la conversación. Es el único documento del kit que es
> obligatoriamente humano, y no es casualidad que sea el que más pesa en la
> sustentación.

La `1_constitution` se escribe **una vez para todo el proyecto**, no por
versión — igual que en Spec Kit oficial. Va en `docs/spec_kit/`, fuera de
`versiones/`.

---

## 7. Las tres comprobaciones de cada documento

Antes de dar un documento por bueno:

| | Cómo |
|---|---|
| **Ninguna marca sin reemplazar** | Buscar `{{` en el archivo |
| **Ningún `[NECESITA ACLARACIÓN]` sin resolver** | Buscar `NECESITA` — y los que queden, a la reunión |
| **Habla de SU sistema** | Buscar «factura», «cliente», «producto»: si su proyecto no es de facturación y aparecen, la IA copió del ejemplo |

La tercera es la que más vale la pena correr. Las otras dos las ve cualquiera;
ésa es la que atrapa un spec kit que parece correcto y no es del proyecto.
