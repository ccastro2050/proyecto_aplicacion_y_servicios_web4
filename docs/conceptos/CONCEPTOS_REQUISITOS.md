# El requisito funcional — qué es, cómo se escribe y cómo se comprueba

> **Qué es este archivo.** El conceptual: qué es un requisito funcional, en qué
> se diferencia de las cuatro cosas con que se confunde, cómo se escribe uno
> que sirva y cómo se sabe que el catálogo está completo.
>
> Su instancia para este proyecto es
> [`../dominio/REQUISITOS_FUNCIONALES.md`](../spec_kit/versiones/v1_sin_fk/2_spec.md).

---

## 1. La definición, y por qué la palabra es «requisito»

> Un **requisito funcional** dice **qué hace el sistema**: qué entra, qué sale,
> y bajo qué condición.

Se dice **requisito**, no *requerimiento*. `requirement` es **una cosa** —una
condición necesaria—, y eso en español es «requisito»; «requerimiento» es la
**acción** de requerir, y en primera acepción un acto judicial. La norma se
llama *Requirements engineering* y se traduce **ingeniería de requisitos**.
Aquí está en el
[`5_data_model.md`](../spec_kit/versiones/v1_sin_fk/5_data_model.md) de la v1,
que es donde se define cada tabla y cada columna.

---

## 2. Las cuatro confusiones, con un ejemplo de cada una

Esta sección es la que más trabajo ahorra después.

### 2.1 · No es una **historia de usuario**

| | |
|---|---|
| **Historia** | *«Yo, como gerente comercial, quiero ver cuánto vendió cada vendedor, para repartir las zos el mismo día sin equivocarme de campus.»* |
| **Requisito** | `RF-01` — El sistema **lista** las fichas activas de cada catálogo, con las columnas de esa tabla, y acepta un `limite` opcional. |

La historia dice **quién y para qué**. El requisito dice **qué hace el
sistema**. De una historia salen varios requisitos, y un requisito puede servir
a varias historias.

> **Y no se sustituyen.** La historia sin el requisito no se puede construir; el
> requisito sin la historia no se puede priorizar, porque nadie sabe para qué
> era.

### 2.2 · No es una **regla de negocio**

| | |
|---|---|
| **Regla** | `RN-05` — *una factura puede tener varios renglones, pero **nunca ninguno***. |
| **Requisito** | `RF-03` — El sistema **agrega** una ficha con los campos obligatorios de esa tabla. |

La regla es **verdad siempre**, sin importar qué función se ejecute: restringe
al alta, a la modificación, al listado y al modelo de datos a la vez. El
requisito es **una función**.

> **La prueba para distinguirlos:** si al escribirlo tiene que repetirlo en
> cuatro requisitos distintos, **no es un requisito: es una regla**. Sáquelo, y
> haga que los cuatro la citen.

### 2.3 · No es una **pantalla** ni un **endpoint**

`POST /api/producto` es **cómo** se realiza `RF-03`; la pantalla «Agregar» es
**dónde**. Los dos pueden cambiar sin que el requisito cambie — y de hecho
cambian: el mismo `RF-03` vive en la API y en el formulario.

> **Por eso el catálogo no nombra rutas.** Las rutas están en `6_contracts`, que
> es el documento que sí cambia por versión.

### 2.4 · No es una **tarea**

«Crear la clase `ProductoCrear`» es una tarea: va en `8_tasks`. El requisito
sobrevive a la tarea.

---

## 3. Cómo se escribe uno que sirva

### La forma

```
El sistema <VERBO> <OBJETO> [cuando <CONDICIÓN>].
```

**Sujeto siempre el sistema.** En presente, en afirmativo, y **una sola cosa**.

### Las siete características que exige la norma

ISO/IEC/IEEE 29148:2018 las lista, y se pueden revisar una por una:

| | Qué quiere decir | Cómo se ve que falla |
|---|---|---|
| **Necesario** | Si se quita, falta algo | Nadie sabe de qué historia salió |
| **No ambiguo** | Una sola lectura | «apropiado», «si aplica», «etc.» |
| **Completo** | No manda a buscar afuera | «según lo definido» — ¿dónde? |
| **Singular** | **Una** cosa | Tiene un **«y además»** |
| **Verificable** | Se puede probar | No se puede escribir el comando |
| **Trazable** | Se sabe de dónde vino | Sin historia y sin regla de origen |
| **Realizable** | Se puede construir ya | Depende de algo que no existe |

### Las cuatro formas de arruinarlo

1. **El «gestionar».** *«El sistema gestionará los productos.»* No se puede probar,
   porque no significa nada. Se descompone: listar, consultar, agregar,
   reemplazar, modificar, retirar — **seis requisitos**.
2. **El «debería».** Un requisito no es un deseo: o es o no es.
3. **El «y además».** Dos requisitos disfrazados de uno. El día que uno se
   cumpla y el otro no, no se sabe si el requisito pasó.
4. **El adjetivo sin número.** «Rápido», «amigable», «robusto». **Eso no es un
   requisito funcional mal escrito: es un requisito NO funcional**, y va en el
   otro catálogo — con su número y su unidad.

---

## 4. Qué lleva una entrada del catálogo

| Campo | Para qué |
|---|---|
| **ID** `RF-nn` | Estable. **No lleva el número de versión**: el requisito no se renombra al pasar de la v1 a la v2 |
| **Enunciado** | La forma de §3 |
| **Historia de origen** | Sin ella no se puede priorizar |
| **Reglas que lo restringen** | Los `RN-nn` que le aplican |
| **Versión** | En cuál se realiza, o «fuera de alcance» |
| **Cómo se verifica** | El comando o la acción concreta |
| **Estado** | propuesto · **acordado** · construido · verificado · derogado |

> **Dos reglas duras sobre el identificador:**
>
> 1. **No se reutiliza nunca**, aunque el requisito se derogue.
> 2. **Un requisito derogado no se borra**: se marca «derogado por `RF-nn`».
>
> Las dos salen de un error que este proyecto ya cometió: al renumerar unas
> versiones, la referencia `D-v2-9` pasó de estar **rota** —que se ve— a
> apuntar a **otra decisión** —que no se ve—.

---

## 5. Cómo se sabe que el catálogo está completo

Tres comprobaciones, y las tres se pueden ejecutar:

1. **Cobertura hacia arriba.** Toda historia **firmada** tiene al menos un
   `RF-nn`. Si una historia no generó ninguno, o sobra la historia o falta el
   requisito.
2. **Cobertura hacia abajo.** Todo `RF-nn` tiene historia de origen. Un
   requisito sin origen es un requisito que alguien se inventó — y suele ser
   una función que el equipo quería programar.
3. **Sin huérfanos de versión.** Todo `RF-nn` está asignado a una versión o
   marcado «fuera de alcance». **No se permite el silencio.**

> **La tercera es la que más encuentra.** Un requisito sin versión no es un
> requisito pendiente: es uno que **nadie está construyendo y nadie sabe que
> nadie está construyendo**.

---

## 6. La matriz de trazabilidad

Es el documento que hace que «esto quedó cubierto» deje de ser una opinión:

```
respuesta de la entrevista → historia → regla → requisito → versión → criterio → prueba
```

Se lee en los dos sentidos, y cada sentido contesta una pregunta distinta:

- **Hacia adelante:** *¿lo que el cliente pidió está construido?*
- **Hacia atrás:** *¿lo que construimos lo pidió alguien?*

La segunda es la que nadie hace, y es la que descubre el código que sobra.

---

## 7. En el camino del prompt, esto no es opcional

Si el catálogo no existe, la IA **deduce los requisitos de las historias** — y
ahí pierde exactamente dos clases:

1. **Los que ninguna historia menciona** porque son obvios para quien conoce el
   dominio. En este proyecto: que tres catálogos **no tengan baja** no está en
   ninguna historia; está en el esquema.
2. **Los que están repartidos entre varias historias** y solo se ven al
   juntarlas.

Y no avisa: **escribe una sección de requisitos igual de convincente, más
corta.**

---

## 8. Referencias

### Normas

1. **ISO/IEC/IEEE 29148:2018** — *Requirements engineering*. §5 define las
   características de un requisito bien formado —las siete de §3— y §9 el
   contenido de la especificación de requisitos del software (SRS).
2. **IEEE Computer Society** (2024). ***SWEBOK Guide v4.0***, área
   *Software Requirements*, reescrita por completo frente a la v3.
3. **ISO/IEC/IEEE 15289:2019** — qué contenido lleva cada ítem de información
   del ciclo de vida.

### Científicas

4. **Lucassen, G.; Dalpiaz, F.; van der Werf, J. M. E. M.; Brinkkemper, S.**
   (2016). «Improving agile requirements: the Quality User Story framework and
   tool». *Requirements Engineering* **21**(3), 383-403.
   DOI **`10.1007/s00766-016-0250-x`** — el marco **QUS** y sus 13 criterios,
   evaluados sobre **1.023 historias de 18 organizaciones**. Es el respaldo
   empírico de la frontera historia/requisito de §2.1.
5. «Requirements Ambiguity Detection and Explanation with LLMs: An Industrial
   Study». **ICSME 2025**, *Industry Track* — cuatro tipos de ambigüedad:
   **léxica, sintáctica, semántica y vaguedad**. Es la clasificación que está
   detrás de «las cuatro formas de arruinarlo».
6. «Assessing the Impact of Requirement Ambiguity on LLM-based Function-Level
   Code Generation». arXiv **`2604.21505`**, 2026 — **mide** cuánto empeora el
   código generado cuando el requisito es ambiguo.
7. «LLM Hallucinations in Practical Code Generation: Phenomena, Mechanism, and
   Mitigation». *Proceedings of the ACM on Software Engineering*, 2025.
   DOI **`10.1145/3728894`** · arXiv **`2409.20550`** — el **conflicto con el
   requisito** como alucinación por conflicto con la entrada. Es el respaldo
   de §7.
8. «ReqInOne: A Large Language Model-Based Agent for Software Requirements
   Specification Generation». arXiv **`2508.09648`**.
9. «Automated Alignment between Elicitation Interviews and Requirements».
   arXiv **`2510.08622`** — mide cuánto del requisito está de verdad en la
   entrevista.

### De gurús y fuentes primarias

10. **Karl Wiegers y Joy Beatty** — *Software Requirements*. El manual de
    oficio: de ahí salen la forma de §3 y la idea del atributo por requisito.
11. **Mike Cohn** (2004). *User Stories Applied* — la frontera con la historia.
12. **Bill Wake** (2003). «INVEST in Good Stories, and SMART Tasks». XP123.
13. **Michael Nygard** (2011). «Documenting Architecture Decisions» —
    `adr.github.io`. De ahí sale la regla de no reutilizar identificadores.

> **Comprobadas en línea el 14 de septiembre de 2026.** Las que llevan DOI se
> abren por el DOI; las de arXiv, por su identificador.
