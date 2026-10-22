# La autoridad — qué manda sobre qué, y qué aquí no hubo

> **Qué es este documento.** Qué material es **normativo** —lo que manda— y qué
> material es de apoyo. Y **lo que aquí no hubo**, porque eso explica la forma
> del proyecto entero.
>
> **A qué plantilla alimenta:** el `4_research`.
>
> **Por qué está separado de la procedencia.** Porque son dos lectores: la
> constitución necesita de dónde viene el esquema —y eso está en
> [`FUENTES_PROCEDENCIA.md`](FUENTES_PROCEDENCIA.md)—; el `4_research` necesita
> la jerarquía y los límites. Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Material académico simulado.**
>
> Versión 2.0 · 22 de octubre de 2026.

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
| `docs/conceptos/` | Explicar los conceptos que el código usa | No es requisito: si un concepto y un spec no coinciden, manda el spec |
| `docs/dominio/` (esta carpeta) | Describir el dominio y el sistema | **Describe, no decide.** Un documento de aquí nunca es la razón de un cambio de código |
| `postman/` | Probar la API sin escribir `curl` | — |
| `backupdb/*.bak` | Volver a un estado de trabajo propio | No es el estado inicial: eso lo da `db/bdfacturas.sql` |
| Los repositorios gemelos de Python y PHP | Comparar la misma idea en otro stack | Ninguno manda sobre el otro: el **contrato** es el mismo, el código no |

> **La distinción entre describir y decidir es la que más se rompe.** Si alguien
> cambia el código porque un documento de dominio dice otra cosa, lo hizo al
> revés: ese documento debía actualizarse. Lo que decide es el spec; lo que
> describe es esto.

---

## 3. Lo que está en el repositorio y **NO es de este proyecto**

| Carpeta | Qué es | Por qué está aquí |
|---|---|---|
| `ProyectosDeAula/` | La **metodología, las rúbricas y los módulos** del proyecto que hacen los estudiantes | Es el trabajo *de ellos*, no de este sistema. Vive aquí para que lo encuentren |
| `ProyectosDeAula/Mapa_conocimiento/` | Formularios y modelo de **otro** dominio | Es el insumo de uno de los módulos del aula |
| `ProyectosDeAula/db_scripts/` | Scripts para los proyectos de aula | Igual: son de ellos |

> **No confundir los dos proyectos es importante.** Este repositorio tiene **dos
> cosas distintas adentro**: el sistema de facturación —que es el ejemplo— y la
> documentación del proyecto de aula —que es la tarea—. Las reglas de uno no
> aplican al otro, y hay al menos cuatro sitios donde difieren a propósito:

| | En este ejemplo | En el proyecto de aula |
|---|---|---|
| El motor | SQL Server, dado | **lo escoge el equipo** |
| El front | Blazor | **libre elección** |
| El borrado en catálogos | físico | **lógico** |
| El modelo de datos | **viene dado** | lo construye el equipo, después de elicitar |

---

## 4. La elicitación es SIMULADA, y eso hay que decirlo

**No hubo un cliente de verdad.** «Comercial Los Andes S.A.» es una empresa
ficticia, y el jefe de ventas que responde en
[`2_RESPUESTAS.md`](elicitacion/2_RESPUESTAS.md) **no existe**.

| Lo que sí es verdad | Lo que está simulado |
|---|---|
| El **orden**: primero se preguntó, después se modeló | **Las personas** y la empresa |
| Que cada decisión del modelo **tiene una razón escrita** | Las **citas**, que son un recurso de redacción |
| Que esa razón se puede rastrear | Que alguien las haya dicho en voz alta |

> **Para qué sirve así:** un estudiante puede leer la pregunta y el esquema al
> lado, y ver **cómo una frase de negocio se convierte en una columna**. Ése es
> el ejercicio, y se puede hacer aunque la persona sea inventada.
>
> **Para qué NO sirve:** como ejemplo de **cómo se conduce** una elicitación
> real, con su incomodidad, sus silencios y sus contradicciones. Eso solo se
> aprende en una reunión de verdad — y la del proyecto de aula lo es.

> **Y la señal que vale para el proyecto de aula: si su elicitación se lee
> demasiado limpia, sospeche de ella.** Una de verdad tiene tramos confusos,
> cosas que el usuario dice sin darse cuenta de que son requisitos, y cosas que
> reconoce no saber. **Ese ruido es la prueba de que la fuente existió.**

---

## 5. El límite honesto: lo que la elicitación no alcanzó a preguntar

El modelo salió de la fase 0, **y la fase 0 tuvo huecos**. Están declarados:

| No se preguntó | Y se nota en que… |
|---|---|
| **Quién hace cada movimiento** | `factura` **no guarda qué usuario la emitió ni quién la anuló**. Hay `fkidvendedor`, pero eso es atribución comercial, no auditoría |
| Cuántas facturas al día, en el pico | El sistema no promete ningún tiempo de respuesta |
| Si hay sedes o bodegas distintas | El stock es **uno**, global por producto |
| Cuánto tiempo se guardan las facturas | Para siempre: no hay archivado |

> **El primero es grave, y es el mejor argumento del curso a favor de elicitar
> bien.** La pregunta *«¿quién anuló la factura 143?»* **no tiene respuesta
> posible** — no porque sea difícil de consultar, sino porque **el dato nunca se
> guardó**. Y no se guardó porque nadie preguntó.
>
> **Nadie echa de menos la pregunta que no se hizo**, hasta el día en que hay
> que responderla. Para entonces ya no hay dónde buscar.

Los cuatro están en [`3_HISTORIAS_PROPUESTAS.md`](elicitacion/3_HISTORIAS_PROPUESTAS.md),
escritos como historias que **no están construidas**.

---

| Qué | Dónde |
|---|---|
| De dónde salió la base de datos | [`FUENTES_PROCEDENCIA.md`](FUENTES_PROCEDENCIA.md) |
| Lo que se decidió NO hacer, con su razón | [`PENDIENTES.md`](PENDIENTES.md) |
| Qué se hizo y cuándo | [`CRONOGRAMA.md`](CRONOGRAMA.md) |
| Qué significa cada palabra | [`GLOSARIO.md`](GLOSARIO.md) |
