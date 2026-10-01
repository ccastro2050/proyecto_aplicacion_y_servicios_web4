# La regla de negocio — lo que es verdad siempre

> **Qué es este archivo.** El conceptual: qué es una regla de negocio, en qué
> se diferencia de un requisito y de una restricción de integridad, cómo se
> hacen salir cuando **no** vienen dadas, y cómo se decide **dónde vive** cada
> una.
>
> Su instancia para este proyecto es
> [`../dominio/REGLAS_DE_NEGOCIO.md`](../spec_kit/versiones/v2_con_fk/2_spec.md).

---

## 1. La definición, y la prueba para reconocerla

> Una **regla de negocio** es algo que tiene que ser **verdad siempre** en el
> dominio, sin importar qué función se esté ejecutando.

No dice qué hace el sistema. Dice **qué no puede dejar de ser cierto**.

### La prueba, en una pregunta

> **¿Tengo que repetirlo en varios requisitos?**
>
> Si al escribirlo aparece en el alta, en la modificación, en el listado y en
> el informe — **no es un requisito: es una regla**. Sáquelo, póngale número, y
> haga que los cuatro requisitos la citen.

Es la prueba más barata que existe y resuelve el noventa por ciento de los
casos.

---

## 2. Las tres cosas con que se confunde

| | Qué es | Ejemplo |
|---|---|---|
| **Requisito funcional** | Algo que el sistema **hace** | *El sistema agrega una ficha con sus campos obligatorios* |
| **Regla de negocio** | Algo que es **verdad siempre** | *Una factura puede tener varios renglones, pero **nunca ninguno*** |
| **Restricción de integridad** | El **mecanismo** con que se defiende una regla | El `THROW` del procedimiento cuando el detalle llega vacío |

**La confusión que cuesta cara es la tercera**, y es sutil: la restricción **no
es** la regla, es **una forma de hacerla cumplir**. La misma regla se puede
defender con un índice, con un disparador o con una revisión humana — y esa
elección se toma y se escribe.

> **Por qué importa la distinción:** si la regla solo existe como
> `CREATE UNIQUE INDEX`, el día que alguien migre a otro motor —o cargue datos
> por otro camino— **la regla desaparece sin que nadie la haya derogado**.

---

## 3. Cómo se hacen salir cuando NO vienen dadas

Este es el caso normal, y es el que casi nunca se enseña. Las reglas **no se
preguntan directamente**: nadie contesta «¿cuáles son sus reglas de negocio?».

### Las cuatro preguntas que sí las sacan

| Pregunta | Qué regla destapa |
|---|---|
| **«¿Qué pasa si…?»** — un caso raro, dicho en concreto | Las condicionales: *«¿y si el producto no tiene programa?»* |
| **«¿Puede haber dos…?»** | Las de unicidad y cardinalidad |
| **«¿Esto se puede borrar?»** — y si dicen que sí, **¿y lo que colgaba de ahí?** | Las de conservación e histórico |
| **«¿Siempre?»** — dicho después de cualquier afirmación | Las excepciones, que son donde vive el trabajo |

> **La cuarta es la más productiva y la que más incomoda.** El cliente dice
> «el programa es obligatorio»; uno pregunta «¿siempre?»; y ahí sale que hay un
> grupo que entra sin programa. Esa excepción **es** una regla, y sin ella el
> sistema rechaza a gente que debería aceptar.

### Las tres fuentes donde ya están escritas y nadie mira

1. **La normativa** — leyes, reglamentos, resoluciones. Producen reglas que
   **nadie va a pedir** porque se dan por sabidas, y son obligatorias igual.
2. **El sistema que ya existe**, si lo hay. Sus validaciones **son** reglas,
   puestas por alguien que sí sabía. Leerlas es más rápido que deducirlas.
3. **Los datos reales.** Una columna que en la práctica nunca se repite es una
   regla de unicidad que nadie declaró. **Contar es una técnica de elicitación.**

---

## 4. Dónde vive cada regla: la decisión que hay que tomar

Toda regla se puede defender en tres sitios, y elegir mal se paga:

| Dónde | Ventaja | Costo |
|---|---|---|
| **La base de datos** | **Nadie la puede saltar**, ni por error ni a propósito, ni por otro camino | El mensaje de error es ilegible para una persona |
| **El servicio** | El error se puede explicar en el idioma del usuario | Quien entre por otra puerta —una carga masiva, otro cliente— se la salta |
| **La pantalla** | El usuario lo ve antes de enviar, y es la mejor experiencia | **No protege nada.** Es comodidad, no control |

**La regla práctica:** lo que protege la **integridad** va en la base de datos; lo que
**explica** va en el servicio; la pantalla **ayuda pero nunca es la única
defensa**.

> **Y las tres a la vez no es redundancia: es lo correcto.** La pantalla avisa,
> el servicio explica, la base de datos garantiza. Lo que sí es un error es tener **solo
> la pantalla** y creer que está protegido.

---

## 5. Las que no caben en una restricción

No todas las reglas son declarativas, y saber cuáles no lo son es la mitad del
diseño. Tres formas típicas:

| Forma | Por qué no cabe | Qué se necesita |
|---|---|---|
| **Depende del conjunto**, no de la fila | Ninguna fila sola la viola | Un procedimiento **de cierre** |
| **Depende de contar** lo que ya hay | Entre contar e insertar cabe otra transacción | Un disparador **con bloqueo** |
| **Depende de un valor que cambia** en otra tabla | Una restricción no puede leer otra tabla | Un disparador, o mover la regla al servicio |

> **Y esta es la consecuencia que hay que escribir:** una regla que necesita
> disparador es una regla **que alguien tiene que acordarse de programar**. Si
> no está en el catálogo, nadie se acuerda — y el sistema funciona mal sin que
> falle nada.

---

## 6. Qué lleva una entrada del catálogo

| Campo | Para qué |
|---|---|
| **ID** `RN-nn` | Estable, **no se reutiliza** aunque la regla se derogue |
| **Enunciado** | En presente, afirmativo, y sin decir cómo se implementa |
| **Origen** | La decisión, la historia, la marca de tiempo o la ley |
| **Mecanismo** | **Dónde se defiende**: restricción, disparador, procedimiento o revisión |
| **Estado** | propuesta · **acordada** · implementada · derogada |

> **Marcar cuáles salen textualmente de la fuente** —y no de una deducción de
> quien modeló— es de lo más útil que se puede hacer. Son las que **no se
> pueden negociar sin volver a preguntar**; las deducidas sí.

---

## 7. En el camino del prompt

Sin catálogo de reglas, una IA que redacta el spec kit hace dos cosas, las dos
malas:

1. **Convierte cada regla en un requisito funcional**, y la repite en cuatro
   sitios. A la tercera, dos versiones se contradicen.
2. **Pierde las que no están en ninguna historia** — que son justo las
   normativas y las excepciones, o sea las caras.

Y como siempre: **no avisa**. Escribe un `5_data_model` verosímil con las
reglas que dedujo.

---

## 8. Referencias

### Normas

1. **ISO/IEC/IEEE 29148:2018** — trata las reglas de negocio como
   **restricciones** sobre la solución, y exige que sean trazables y no
   ambiguas igual que un requisito.
2. **IEEE Computer Society** (2024). ***SWEBOK Guide v4.0***, área
   *Software Requirements*.
3. **ISO/IEC/IEEE 15289:2019** — el catálogo de reglas es una
   **especificación**, no un registro: se versiona y se firma.

### Científicas

4. **Chen, P. P.-S.** (1976). «The Entity-Relationship Model — Toward a Unified
   View of Data». *ACM TODS* **1**(1), 9-36. DOI **`10.1145/320434.320440`**
   — la **cardinalidad** y la **participación** son las dos primeras reglas de
   negocio que cualquier modelo escribe, aunque nadie las llame así.
5. **Codd, E. F.** (1970). «A Relational Model of Data for Large Shared Data
   Banks». *CACM* **13**(6), 377-387. DOI **`10.1145/362384.362685`**
   — de aquí salen las restricciones de integridad como concepto.
6. **Härder, T.; Reuter, A.** (1983). «Principles of Transaction-Oriented
   Database Recovery». *ACM Computing Surveys* **15**(4), 287-317.
   DOI **`10.1145/289.291`** — la **C** de ACID *es* «la base de datos deja los datos
   cumpliendo sus reglas». §5 depende de esto: una regla que necesita contar
   antes de insertar necesita aislamiento, no buena voluntad.
7. «Requirements Ambiguity Detection and Explanation with LLMs: An Industrial
   Study». **ICSME 2025**, *Industry Track* — el **conocimiento del dominio**
   ayuda a separar lo genuinamente ambiguo de lo que solo lo parece. Un
   catálogo de reglas **es** conocimiento del dominio.
8. «LLM Hallucinations in Practical Code Generation: Phenomena, Mechanism, and
   Mitigation». *Proceedings of the ACM on Software Engineering*, 2025.
   DOI **`10.1145/3728894`** — el respaldo de §7.

### De gurús y fuentes primarias

9. **Business Rules Group** — *The Business Rules Manifesto* (2003). La fuente
   de la idea de §1: **las reglas son un activo del negocio, no un detalle de
   implementación**, y por eso se escriben aparte del código.
10. **Ronald G. Ross** — *Principles of the Business Rule Approach*.
11. **Eric Evans** (2003). *Domain-Driven Design* — las reglas viven en el
    dominio, no repartidas por los servicios.
12. **Karl Wiegers y Joy Beatty** — *Software Requirements*, capítulo de reglas
    de negocio: de ahí sale la separación regla / requisito de §2.

> **Comprobadas en línea el 14 de septiembre de 2026**, salvo las 9 a 12, que
> se arrastran de la bibliografía de oficio y **hay que comprobar edición y
> enlace** al citarlas en un trabajo.
