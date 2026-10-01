# Las fuentes — de dónde salió cada cosa, y de dónde NO

> **Qué es este documento.** Qué material fue **normativo** —lo que manda— y qué
> material fue de apoyo. Y, sobre todo, **lo que aquí no hubo**, porque eso
> explica la forma del proyecto entero.
>
> **Material académico simulado.** La empresa es ficticia y no hubo ningún
> cliente. Lo que sí es real es el reparto de fuentes que este documento
> describe, y se puede comprobar abriendo las carpetas.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. La fuente normativa de este proyecto **es un esquema de base de datos**

Y conviene decirlo de entrada, porque es lo que lo distingue de cualquier
proyecto de ingeniería normal:

> **Artículo 5 de la constitución — «La base de datos viene DADA».**
>
> *«La BD `bdfacturas` se crea COMPLETA (12 tablas, triggers, SPs, datos de
> ejemplo) desde la v1, con los scripts provistos en `db/` — se copian, no se
> generan. Lo que crece por versiones es la API.»*

Es decir: **el modelo de datos llegó hecho**, 1 540 líneas de SQL, con sus tres
disparadores y sus dieciséis procedimientos. Nadie lo elicitó, nadie lo negoció
con un usuario. Estaba, y el trabajo fue construirle una API y una interfaz
encima.

### Y eso invierte el orden normal de la ingeniería

| Lo normal | Lo que pasó aquí |
|---|---|
| Se habla con el usuario | No hubo usuario |
| Salen requisitos | Salieron del **esquema** y del curso |
| De los requisitos sale el modelo de datos | El modelo **ya estaba** |
| Del modelo sale el código | Del modelo salió el código |

> **Por qué está bien así, en ESTE proyecto, y por qué no lo estaría en otro.**
>
> Esto es material de clase, y el objeto de la clase es **aprender a construir
> una API en capas sobre una base que no se diseñó uno**. Darle a un estudiante
> un esquema hecho es realista —es lo que le va a pasar en su primer trabajo— y
> le quita del camino una discusión que no es la de este curso.
>
> **Pero en el proyecto de aula NO se vale**, y por eso ahí el orden es el
> correcto: primero se elicita, después se modela. Si alguien copia el orden de
> este repositorio en su proyecto, está copiando una decisión pedagógica como si
> fuera una buena práctica, y no lo es.

---

## 1. Lo normativo — lo que manda, en orden de autoridad

| # | Fuente | Qué fija | Dónde |
|---|---|---|---|
| **1** | **La constitución** | Las reglas de **todas** las versiones. Ante conflicto, **gana ella** | [`1_constitution.md`](../spec_kit/1_constitution.md) |
| **2** | **El mapa de versiones** | Qué va en cada versión y qué no | [`0_mapa_versiones.md`](../spec_kit/versiones/0_mapa_versiones.md) |
| **3** | **El `2_spec.md` de la versión** | Los requisitos y los criterios de aceptación de ese tramo | `versiones/vN/2_spec.md` |
| **4** | **El esquema SQL** | Las doce tablas, los disparadores, los procedimientos | [`db/bdfacturas.sql`](../../db/bdfacturas.sql) |

> **El orden importa.** Si el spec de una versión contradice la constitución,
> manda la constitución y el spec está mal escrito. Si el spec contradice al
> esquema, **manda el esquema**: él ya existe y ya tiene datos.

---

## 2. Lo de apoyo — lo que ayuda y no manda

| Fuente | Para qué sirve | Y qué NO decide |
|---|---|---|
| `docs/conceptos/` (21 documentos) | Explicar los conceptos que el código usa | No es requisito: si un concepto y un spec no coinciden, manda el spec |
| `docs/dominio/` (esta carpeta) | Describir el dominio y el sistema | **Describe, no decide.** Un documento de aquí nunca es la razón de un cambio de código |
| `postman/coleccion_v4.postman_collection.json` | Probar la API sin escribir `curl` | — |
| `backupdb/*.bak` | Volver a un estado de trabajo propio | No es el estado inicial: eso lo da `db/bdfacturas.sql` |
| Los repositorios gemelos de Python y PHP | Comparar la misma idea en otro stack | Ninguno manda sobre el otro: el **contrato** es el mismo, el código no |

> **La distinción entre describir y decidir es la que más se rompe.** Si alguien
> cambia el código porque [`ARQUITECTURA.md`](ARQUITECTURA.md) dice otra cosa, lo
> hizo al revés: ese documento debía actualizarse. Lo que decide es el spec; lo
> que describe es esto.

---

## 3. Lo que está en la carpeta y **NO es de este proyecto**

| Carpeta | Qué es | Por qué está aquí |
|---|---|---|
| `ProyectosDeAula/` | La **metodología, las rúbricas y los módulos** del proyecto que hacen los estudiantes | Es el trabajo *de ellos*, no de este sistema. Vive aquí para que lo encuentren |
| `ProyectosDeAula/Mapa_conocimiento/` | Formularios y modelo de **otro** dominio | Es el insumo de uno de los módulos del aula |
| `ProyectosDeAula/db_scripts/` | Scripts para los proyectos de aula | Igual: son de ellos |

> **No confundir los dos proyectos es importante.** Este repositorio tiene **dos
> cosas distintas adentro**: el sistema de facturación —que es el ejemplo— y la
> documentación del proyecto de aula —que es la tarea—. Las reglas de uno no
> aplican al otro, y hay al menos tres sitios donde difieren a propósito:

| | En este ejemplo | En el proyecto de aula |
|---|---|---|
| El motor | SQL Server, dado | **lo escoge el equipo** |
| El front | Blazor | **libre elección** |
| El borrado en catálogos | físico | **lógico** |
| El modelo de datos | **viene dado** | lo construye el equipo, después de elicitar |

---

## 4. Lo que aquí NO hubo, dicho con nombre propio

| No hubo | Y en su lugar |
|---|---|
| **Reunión con un usuario experto** | El curso y el esquema |
| **Transcripción, grabación, acta** | Nada: no hay fuente oral que citar |
| **Un cliente que aprobara nada** | Las compuertas del spec kit, firmadas por una persona |
| **Historias de usuario firmadas** | Requisitos funcionales derivados del esquema y del spec |

> **Y entonces, ¿qué es `elicitacion/`?** Es **simulada, y lo dice en su primera
> línea**. Se escribió **al final**, no al principio, con un propósito concreto:
> hacer que las preguntas de un usuario imaginario **expliquen las decisiones que
> el sistema de verdad tomó** — por qué `cliente` y `vendedor` son tablas
> aparte, por qué una factura se anula y no se corrige, por qué el stock lo
> defiende un disparador.
>
> **Para qué sirve escrita así:** un estudiante puede leer la pregunta y el
> esquema al lado, y ver **cómo una frase de un usuario se convierte en una
> columna**. Ése es el ejercicio.
>
> **Para qué NO sirve:** como ejemplo de *cómo se hace* una elicitación, porque
> una de verdad se hace **antes** y no sabe cómo va a terminar el sistema. El
> ejemplo de eso está en `proyecto_catedras2`, donde la reunión fue real y la
> transcripción existe. Ver
> [`CONCEPTOS_ELICITACION.md`](../conceptos/CONCEPTOS_ELICITACION.md).

---

## 5. Una advertencia sobre el esquema que no se pudo comprobar

El esquema llegó con decisiones que **no tienen autor consultable**: por qué
`usuario` se identifica por el correo y no por un `id`, por qué el `estado` es
un texto de diez caracteres y no un booleano, por qué hay dos procedimientos
—`sp_actualizar_factura_…` y `sp_borrar_…`— que la API nunca expone.

> **Están razonados en [`DISENO_BD.md`](DISENO_BD.md), y ese razonamiento es
> reconstruido, no heredado.** Es decir: son las razones que **sostienen** la
> decisión, no necesariamente las que la tomaron. Donde esa diferencia importa,
> el documento lo dice.
>
> **Es el límite honesto de este documento**, y vale decirlo: cuando la fuente
> es un archivo y no una persona, hay preguntas que ya no se le pueden hacer a
> nadie.

---

| Qué | Dónde |
|---|---|
| Las reglas que mandan | [`1_constitution.md`](../spec_kit/1_constitution.md) |
| Qué se hizo y cuándo | [`CRONOGRAMA.md`](CRONOGRAMA.md) |
| Qué significa cada palabra | [`GLOSARIO.md`](GLOSARIO.md) |
| La elicitación simulada | [`elicitacion/1_PREGUNTAS.md`](elicitacion/1_PREGUNTAS.md) |
