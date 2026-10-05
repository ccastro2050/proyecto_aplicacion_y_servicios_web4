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

## 0. De dónde sale la base de datos: dos orígenes, no uno

**Las doce tablas vienen de otro curso.** El equipo ya había modelado
`bdfacturas` en Bases de Datos: las entidades, las relaciones, las llaves y las
restricciones. Ese trabajo **se reusa tal cual** y no se vuelve a hacer.

**Pero ahí no había un sistema de facturación: había un modelo.** Todo lo que
convierte ese modelo en un sistema que defiende sus propias reglas **se escribió
en ESTE proyecto, en la fase 0**, antes de la versión 1.

### Y la diferencia se puede medir

```powershell
# Las tablas y sus restricciones, contra todo lo demás
Select-String -Path dbdfacturas.sql -Pattern 'CREATE (TABLE|TRIGGER|PROCEDURE)' |
  Group-Object { $_.Matches[0].Groups[2].Value } | Select-Object Count, Name
```

| | | Líneas de **código** | De dónde viene |
|---|---|---|---|
| **12 tablas** con sus restricciones | | **89** | **del curso de Bases de Datos** |
| **3 disparadores** | `trg_prodfact_insert` · `_update` · `_delete` | | |
| **16 procedimientos** | los 6 de `factura`, los de usuario y roles, `verificar_acceso_ruta` | **760** | **de este proyecto** |
| Los datos de ejemplo | | | |

> **El 11 % del script llegó hecho. El 89 % se escribió aquí.** Y no es un
> detalle de contabilidad: **las tablas no deciden nada**. Que el stock no quede
> negativo, que el total cuadre con sus renglones, que una factura se anule en
> vez de corregirse y que no se pueda anular dos veces —**todas las reglas del
> negocio**— viven en los disparadores y los procedimientos, y **ninguno existía
> antes de este proyecto**.

### De dónde salieron esas reglas: de la fase 0

```
elicitación  →  reglas de negocio  →  disparadores y procedimientos
```

| Paso | Dónde quedó |
|---|---|
| **1 · Se habla con el usuario experto** | [`elicitacion/`](elicitacion/1_PREGUNTAS.md) |
| **2 · Salen las reglas** | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) — las 22, con quién defiende cada una |
| **3 · Cada regla se implementa donde protege a más gente** | el disparador, el procedimiento, o el servicio |
| **4 · Y recién entonces, la API** | las cinco versiones |

> **Ejemplo, y se puede seguir completo:** don Hernán dice *«no se puede vender
> lo que no tengo, eso es sagrado»* → nace la regla **RN-10** → y se implementa
> en `trg_prodfact_insert` con un `THROW 50001`. **No en C#**, porque la regla
> tiene que valer también para quien entre por SSMS.
>
> Las tres cosas se pueden abrir y comparar. Eso es lo que significa que una
> decisión **tenga autor**.

### Por qué el modelo no se rehace en cada versión

> **Artículo 5: «La base de datos se diseña UNA VEZ, en la fase 0.»** Desde la
> v1 en adelante **viene dada al código**: se copia, no se genera.

| | |
|---|---|
| **Por qué una sola vez** | Un modelo que cambia en cada entrega obliga a migrar datos, rehacer disparadores y reescribir procedimientos — y nada de eso es lo que el curso enseña |
| **Por qué ninguna IA lo genera** | Porque ya está hecho, y la IA **no estuvo en la elicitación**. Si propone un `CREATE TABLE`, está rehaciendo a ciegas un trabajo que tiene autores |

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

## 4. La elicitación es SIMULADA, y eso hay que decirlo

**No hubo un cliente de verdad.** «Comercial Los Andes S.A.» es una empresa
ficticia y «don Hernán», el jefe de ventas que responde en
[`2_RESPUESTAS.md`](elicitacion/2_RESPUESTAS.md), **no existe**.

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
> real, con su incomodidad, sus silencios y sus contradicciones. Para eso está
> `proyecto_catedras2`, donde la reunión ocurrió y la transcripción existe.

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
| Las reglas que mandan | [`1_constitution.md`](../spec_kit/1_constitution.md) |
| Qué se hizo y cuándo | [`CRONOGRAMA.md`](CRONOGRAMA.md) |
| Qué significa cada palabra | [`GLOSARIO.md`](GLOSARIO.md) |
| La elicitación simulada | [`elicitacion/1_PREGUNTAS.md`](elicitacion/1_PREGUNTAS.md) |
