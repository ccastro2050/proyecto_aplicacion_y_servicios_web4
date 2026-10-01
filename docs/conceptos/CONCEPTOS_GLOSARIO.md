# El glosario — por qué cada cosa se llama de UNA manera

> **Qué es este archivo.** El documento conceptual: qué es un glosario de
> proyecto, qué lleva, cómo se hace uno que sirva, y cómo se comprueba.
> **Este repositorio no tiene un `GLOSARIO.md` aparte**: los términos se
> definen donde se usan — las tablas en
> [`5_data_model.md`](../spec_kit/versiones/v1_sin_fk/5_data_model.md) y las
> reglas en el `2_spec.md` de su versión. Para un proyecto de aula con un
> dominio ajeno, en cambio, el glosario aparte sí hace falta.

---

## 1. El problema, con un caso real de este proyecto

En la reunión del 18 de junio, el usuario experto corrigió expresamente un
nombre:

> *«si dejamos eso al administrador, **el administrador es la unidad de
> tecnología**, y la unidad de tecnología no debe estar subiendo cátedras ni
> descargando archivos del ASIS, nada de esas cosas»* — `1:44:28`

> **Esta cita es textual y es de OTRO proyecto** —la grabación de una entrevista
> real del sistema de cátedras—, y se deja tal cual a propósito: una cita no se
> reescribe para que encaje. Lo que enseña vale igual aquí: en esa frase, «el
> administrador» significa **dos cosas distintas** en la misma oración, y nadie
> en la reunión lo notó.
>
> *«Creo que es como todo eso, **es como el líder**»* — `1:44:46`

Y sin embargo, cuando se cuenta lo que quedó escrito:

| Dónde | Qué dice |
|---|---|
| Las diez historias de usuario | «administrador», **21 veces**. «líder», **cero** |
| La tabla `rol` de la base de datos | `Administrador`, `Coordinador de dependencia`, `Consulta` |

**El nombre que la fuente corrigió es el que quedó en todo el sistema.**

Y no es una cuestión de etiqueta. En esa universidad «administrador» **ya
significa otra cosa**: la unidad de tecnología. Con ese nombre, el rol se le
va a asignar a quien no debe tenerlo. La reunión lo vio venir, lo dijo, y el
documento no lo recogió — porque **no había dónde recogerlo**.

**Para eso es el glosario:** para que una decisión sobre cómo se llama algo
tenga un sitio, una fecha y una firma, en vez de vivir en la memoria de quien
estuvo en la reunión.

---

## 2. Qué es, en términos de la disciplina que se ocupa de esto

Hay una disciplina entera —la **terminología**— y dos normas que la rigen. Vale
la pena tomar de ahí tres ideas, porque aclaran más que cualquier definición
casera:

### 2.1 · Concepto, designación y definición son **tres cosas**

- El **concepto** es la unidad de conocimiento: *la persona o la empresa a la
  que se le emite una factura*.
- La **designación** es la palabra con que se le llama: `cliente`.
- La **definición** es el enunciado que lo delimita.

**La mayoría de los problemas de glosario son de designación, no de concepto.**
Todo el mundo entiende de qué se habla; el lío es que unos lo llaman
`cliente`, otros `comprador` y la base de datos lo llama `cliente` pero el front
escribe «público».

### 2.2 · Los tres fallos, y hay que saber distinguirlos

| Fallo | Qué pasa | Ejemplo de este proyecto |
|---|---|---|
| **Sinonimia** — un concepto, dos nombres | El lector cree que son dos cosas | `administrador` / `líder` |
| **Homonimia** — un nombre, dos conceptos | El lector cree que son la misma cosa | **`rol`**: en la tabla `rol` es un permiso del sistema (`ADMIN`, `CONSULTA`); en la columna `ponencia.rol` es la función en un evento (`PONENTE`, `MODERADOR`) |
| **Vaguedad** — un nombre sin concepto detrás | No se puede probar nada | «gestionar», «amigable», «robusto» |

**La homonimia es la peor de las tres**, y es la que casi nunca se detecta:
sinonimia se ve leyendo, homonimia solo se ve contando.

### 2.3 · Una definición **delimita**; no explica ni da ejemplos

Una buena definición dice **qué es** y **qué lo distingue** de lo parecido. No
empieza por «es cuando…», no repite el término dentro de sí misma, y no
enumera casos.

```
MAL   Cliente: es cuando alguien compra algo y se le factura.
MAL   Cliente: el que es cliente.                  ← se define con su propia raíz
BIEN  Cliente: persona o empresa a la que se le puede emitir una factura.
      virtualmente. Puede ser estudiante, docente, administrativo o externo.
      Se distingue del ponente, que es quien la dicta.
```

---

## 3. El concepto que le da sentido a todo esto: **lenguaje ubicuo**

De *Domain-Driven Design*, y es la idea más útil de ese libro:

> **El mismo término, en la conversación con el cliente, en la historia de
> usuario, en el requisito, en la tabla, en la clase y en la URL.**

Cuando el nombre **cambia al cruzar una frontera**, la frontera está mal
puesta. Y ese cambio se puede rastrear:

```
El cliente dice     →  «los que nos compran»
La historia dice    →  «comprador»
El requisito dice   →  «cliente»
La tabla se llama   →  cliente
La clase se llama   →  Customer
La URL es           →  /api/buyers
```

Cada salto de esa cadena es una traducción, y **cada traducción es un sitio
donde se pierde algo**. El glosario existe para que la cadena sea una sola
palabra de punta a punta.

> **Y el caso en que sí hay que traducir:** cuando el término del dominio está
> en un idioma y el código en otro. La regla entonces no es «no traduzca», es
> **traduzca una vez, en el glosario, y a partir de ahí nadie improvisa**.

---

## 4. Qué lleva una entrada, y por qué cada campo

| Campo | Para qué |
|---|---|
| **Término** | La designación **única** |
| **Definición** | Qué es y qué lo distingue de lo parecido |
| **Quién lo dijo** | Nombre y marca de tiempo, o el documento. **Sin fuente, es una invención** |
| **No decir** | Los sinónimos **prohibidos**. Este campo es el que hace el trabajo |
| **Dónde vive** | Tabla, clase, ruta. Es lo que permite comprobarlo con un `grep` |
| **Estado** | propuesto · **acordado** · derogado |

> **El campo «no decir» es el que casi nadie pone y es el más útil.** Un
> glosario que solo dice qué significa `cliente` no impide que alguien
> escriba `participante`: hay que **nombrar el error** para poder buscarlo.

---

## 5. Los calcos: cuándo se aceptan y cuándo no

Un glosario de proyecto en español tiene que decidir sobre las palabras que
llegaron del inglés. La regla que usamos:

| Palabra | Decisión | Por qué |
|---|---|---|
| **requisito**, no *requerimiento* | Se corrige | `requirement` es **una cosa** —una condición necesaria—, y eso en español es «requisito». «Requerimiento» es la **acción** de requerir, y en primera acepción un acto judicial. La norma se llama *Requirements engineering* y se traduce **ingeniería de requisitos** |
| **elicitación** | **Se conserva** | Nombra algo que «recolección» no nombra: el requisito **no está** para recogerlo, hay que **hacerlo salir** |
| **spec kit**, *commit*, *push* | Se conservan | Son nombres de herramientas o de operaciones de una herramienta. Traducirlos deja al estudiante sin poder buscar en internet |

**La regla, en una línea:** se traduce lo que tiene traducción exacta; se
conserva lo que al traducirse **pierde una distinción** o **deja al lector sin
poder buscar**. Y en los dos casos **se dice una vez, en el glosario**, para
que el estudiante reconozca la otra forma cuando la encuentre en la
bibliografía.

---

## 6. Cómo se hace uno, en cinco pasos

1. **Vaciar las fuentes.** De la transcripción y de los documentos recibidos,
   sacar **todo sustantivo del dominio** que aparezca más de una vez. Sin
   filtrar todavía.
2. **Agrupar por concepto**, no por palabra. Aquí aparecen las sinonimias.
3. **Contar las apariciones de cada designación.** Aquí aparecen las
   homonimias — y aparece cuál nombre ganó **por inercia**, que casi nunca es
   el que se acordó.
4. **Definir**, con la regla de la sección 2.3, y **anclar cada definición a su
   fuente**.
5. **Decidir lo que tenga dos nombres**, con quien tenga autoridad para
   decidirlo, **y dejar constancia de la fecha**. Lo que no se decida se marca
   **«sin decidir»**, y con **quién firma** — nunca se resuelve en silencio
   por el camino fácil.

> **Por qué «sin decidir» y no «en disputa».** «Disputa» suena a que hay dos
> bandos peleando, y casi nunca los hay: lo que hay es **la palabra de la
> fuente contra la inercia del documento**. Y esconde lo único accionable —
> **quién firma y cuándo**—, que es justo lo que hace que el asunto se cierre
> algún día.

> **El paso 3 es el que descubre cosas.** Contar es lo que revela que
> «administrador» ganó 21 a 0 contra el nombre que la fuente había corregido.
> Leyendo no se ve.

---

## 7. Cómo se comprueba

Un glosario que no se comprueba se desactualiza en una semana. Tres
comprobaciones, todas ejecutables:

1. **Los prohibidos no aparecen.** Ningún término del campo «no decir» está en
   historias, requisitos, modelo, código ni pantallas.
2. **Los acordados sí aparecen.** Cada término con estado «acordado» aparece en
   el sitio que declara su campo «dónde vive».
3. **No quedan huérfanos.** Ningún nombre de tabla, clase o ruta del sistema
   falta en el glosario.

```bash
# La comprobación 1, en una línea. Si devuelve algo, hay trabajo.
grep -rniw "participante\|usuario final\|público" --include=*.md --include=*.cs .
```

---

## 8. En la ruta A —generar el spec kit con una IA— esto **no es opcional**

Y hay evidencia, no solo intuición:

- La ambigüedad en los requisitos **degrada el código que un LLM genera**: hay
  estudios de 2026 que la miden a nivel de función.
- Y lo que es más directo para nosotros: al estudiar requisitos reales, se
  encontró que **incorporar conocimiento del dominio ayuda al modelo a
  distinguir lo genuinamente ambiguo de lo que solo lo parece**. Un glosario
  **es** conocimiento del dominio en la forma más barata de entregar.

Sin glosario, una IA que redacta el spec kit **va a elegir un sinónimo cada
vez**, y lo va a hacer con soltura. El resultado son nueve documentos
coherentes por dentro y desalineados entre sí.

---

## 9. En este proyecto

- Aquí el glosario está repartido: cada término se define en el documento
  donde aparece por primera vez — las tablas en el `5_data_model.md`.
- Alimenta a **todos** los documentos posteriores: es el número 4 de los quince
  de [`DOCUMENTOS_DE_INGENIERIA_DEL_SOFTWARE.md`](DOCUMENTOS_DE_INGENIERIA_DEL_SOFTWARE.md), y el
  primero que se escribe según [`MAPA_DE_DOCUMENTOS.md`](../../README.md).
- Trae **una decisión sin decidir** —cómo se llama el rol principal— que está
  sin resolver a propósito, esperando al usuario experto. Un glosario que
  resuelve solo lo fácil no sirve para nada.

---

## 10. Referencias

### Normas — la terminología es una disciplina, no una costumbre

1. **ISO 704:2022** — *Terminology work — Principles and methods*. Establece
   los principios para **formar designaciones y formular definiciones**. De
   aquí sale la regla de la sección 2.3.
2. **ISO 1087:2019** — *Terminology work and terminology science — Vocabulary*.
   Define los conceptos de la propia terminología: **concepto, designación,
   definición**, y la relación entre ellos.
3. **ISO/IEC/IEEE 29148:2018** — exige terminología **consistente y no
   ambigua** en la especificación de requisitos.
4. **IEEE Computer Society** (2024). ***SWEBOK Guide v4.0***, área
   *Software Requirements*.

### Científicas actualizadas (2025-2026)

5. «Assessing the Impact of Requirement Ambiguity on LLM-based Function-Level
   Code Generation». arXiv **`2604.21505`**, 2026 — **mide** cuánto empeora el
   código generado cuando el requisito es ambiguo.
6. «Requirements Ambiguity Detection and Explanation with LLMs: An Industrial
   Study». **ICSME 2025**, *Industry Track* — distingue cuatro tipos de
   ambigüedad: **léxica, sintáctica, semántica y vaguedad**, y encuentra que
   **el conocimiento del dominio** ayuda al modelo a separar lo genuinamente
   ambiguo de lo que solo lo parece. **Es el respaldo de la sección 8.**
7. «Context-Adaptive Requirements Defect Prediction through Human-LLM
   Collaboration». arXiv **`2601.01952`**.
8. «ReqInOne: A Large Language Model-Based Agent for Software Requirements
   Specification Generation». arXiv **`2508.09648`**.
9. Ferrari, A.; Spoletini, P.; *et al.* — «Interview Review: An Empirical Study
   on Detecting Ambiguities in Requirements Elicitation Interviews» — la
   ambigüedad **se detecta releyendo**, no en vivo.

### De gurús y fuentes primarias

10. **Eric Evans** (2003). *Domain-Driven Design: Tackling Complexity in the
    Heart of Software*. Addison-Wesley — el **lenguaje ubicuo** de la sección 3.
11. **Karl Wiegers y Joy Beatty** — *Software Requirements*, capítulo del
    diccionario de datos y el glosario.
12. **RAE** — *Diccionario de la lengua española*, entradas `requisito` y
    `requerimiento`, para la sección 5.

> **Comprobadas en línea el 14 de septiembre de 2026.** Las que llevan DOI se
> abren por el DOI; las de arXiv, por su identificador.
