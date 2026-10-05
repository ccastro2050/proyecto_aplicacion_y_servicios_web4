# Cómo se le pregunta al cliente para que salgan las historias

**Elicitación de requisitos en cuatro rondas**

Este documento explica **de dónde salen las historias de usuario**. No de
la cabeza de quien programa: de una conversación con quien tiene el
problema.

---

## 1. El error que se comete siempre

El cliente dice:

> «Necesito un botón que exporte a Excel.»

Y el equipo construye el botón. Seis semanas después nadie lo usa, porque
lo que el cliente necesitaba era **saber cuánto se recogió el mes
pasado** — y para eso el Excel era el camino que él conocía, no el que
servía.

**El cliente conoce su problema; no tiene por qué conocer la solución.**
Esa es toda la técnica: preguntar por el problema y dejar la solución para
después.

De ahí sale la regla práctica: **cuando le pidan una solución, pregunte
para qué.** «¿Y qué haría usted con ese Excel?» Tres o cuatro «para qué»
seguidos y aparece el requisito de verdad. En la literatura eso se llama
**los cinco por qué**.

---

## 2. Las cuatro técnicas que se usan hoy

Ninguna es un formulario. Las cuatro son formas de conversar.

| Técnica | De quién | Qué aporta |
|---|---|---|
| **Impact Mapping** | Gojko Adzic, 2012 | Ordena la conversación en cuatro niveles: **¿por qué? · ¿quién? · ¿cómo? · ¿qué?** Se empieza por el objetivo del negocio, nunca por la funcionalidad |
| **User Story Mapping** | Jeff Patton, 2014 | Recorre el **viaje del usuario** de izquierda a derecha. Las actividades son el espinazo; las historias cuelgan de cada paso |
| **Example Mapping** | Matt Wynne, 2015 | Convierte una historia en **reglas**, cada regla en **ejemplos concretos**, y lo que nadie sabe responder en **preguntas**. Cuatro colores de tarjeta, veinticinco minutos |
| **Three Amigos** | John Ferguson Smart | La conversación es entre **negocio, desarrollo y pruebas**. Quien va a probar el sistema pregunta cosas que a los otros dos no se les ocurren |

**Este documento las junta en un solo recorrido de cuatro rondas**, que es
lo que cabe en una entrevista de una hora.

---

## 3. Las cuatro rondas

### Ronda 1 — Por qué existe esto

Preguntas sobre el **negocio**, no sobre el software:

- ¿Qué problema le está costando plata, tiempo o disgustos hoy?
- ¿Cómo lo resuelven ahora, sin sistema?
- Si esto funciona, **¿qué número cambia?** ¿Cuál es hoy y cuál quisiera?
- ¿Qué pasa si no hacemos nada?

> De aquí **no sale ninguna historia todavía**. Sale el **para qué** de
> todas: el «…para lograr X» que cierra cada una. Sin esta ronda, ese
> pedazo de la historia se inventa, y se nota.

### Ronda 2 — Quiénes

- ¿Quién va a usar esto, día a día? **Nómbrelos por su oficio.**
- ¿Quién decide, quién ejecuta, quién solo consulta?
- **¿Quién sale perjudicado si esto se hace?** — la pregunta que nadie
  hace, y la que más problemas evita.
- De todos esos, ¿cuál lo usaría más?

> De aquí sale el **«como…»** de cada historia. Si no hay roles, todas
> empiezan con «como usuario», que no dice nada y no se puede verificar.

### Ronda 3 — El día de trabajo

Aquí se recorre el trabajo **en el orden en que pasa**:

- Cuénteme un día normal, desde que llega hasta que se va.
- ¿Qué hace primero, qué sigue, con qué termina?
- ¿En qué paso pierde más tiempo?
- **¿Qué es lo que más se equivoca**, y qué pasa cuando se equivoca?
- ¿Qué hace cuando algo sale mal? ¿A quién llama?

> De aquí sale el **«quiero…»**: cada paso del viaje es candidato a
> historia. Y de regalo, **el orden del viaje sugiere el orden de las
> versiones**: lo primero que hace el usuario suele ser lo primero que se
> construye.

### Ronda 4 — Las reglas y los ejemplos

Por cada historia que ya se ve venir:

- ¿Qué tiene que pasar **siempre**? ¿Qué no puede pasar **nunca**?
- **Deme un ejemplo**, con nombres y números de verdad.
- ¿Y si llega vacío? ¿Y si llega dos veces? ¿Y si quien lo hizo se
  equivocó?
- ¿Cómo sabemos que quedó bien hecho?

> De aquí salen los **criterios de aceptación**. Y las preguntas que nadie
> supo responder **no se rellenan a ojo**: se anotan, y en el spec kit se
> escriben como `[NECESITA ACLARACIÓN]`.

---

## 4. De la respuesta a la historia

Esta es la traducción, y es mecánica una vez se tienen las respuestas:

| Lo que respondió el cliente | En qué se convierte |
|---|---|
| «Perdemos media hora cada mañana cuadrando el inventario» | El **objetivo** (ronda 1) |
| «Eso lo hace el bodeguero» | El **rol**: `Yo, Ana, como bodeguera…` |
| «Cuenta lo que llegó y lo anota en un cuaderno» | La **acción**: `…quiero registrar lo que llegó…` |
| «Para que a mediodía sepamos qué hay» | El **beneficio**: `…para saber a mediodía qué hay disponible` |
| «Si anota mal la cantidad, el pedido sale corto» | Un **criterio de aceptación**: la cantidad no puede quedar vacía ni en cero |
| «No sé qué pasa si el mismo lote llega dos veces» | Un **`[NECESITA ACLARACIÓN]`** |

Y la historia queda así:

> **Yo, Ana Restrepo, como bodeguera, quiero registrar los kilos que
> llegan de cada producto, para que a mediodía la cocina sepa con qué
> cuenta.**
>
> **Criterios de aceptación:**
> 1. La cantidad es obligatoria y mayor que cero; si llega vacía, el
>    sistema responde 422 diciendo cuál campo falta.
> 2. …

---

## 5. Cómo saber si la historia quedó bien: INVEST

Seis letras, de Bill Wake (2003): *Independent, Negotiable, Valuable,
Estimable, Small, **Testable***. Se revisa la historia terminada contra
ellas:

| Letra | Qué pregunta |
|---|---|
| **I**ndependiente | ¿Se puede construir sin esperar a otra? |
| **N**egociable | ¿Es una conversación, o ya viene con la solución impuesta? |
| **V**aliosa | ¿A alguien le sirve? ¿A quién, y para qué? |
| **E**stimable | ¿El equipo puede decir cuánto le costaría? |
| **S**mall (pequeña) | ¿Cabe en una iteración? |
| **T**estable (probable) | **¿Se puede probar que quedó hecha?** |

> **La última es la que amarra con el resto del curso.** «El sistema debe
> ser rápido» no es verificable; «el listado responde en menos de un
> segundo con diez mil registros» sí. Un criterio de aceptación sin un
> número o un código de estado no es un criterio: es un deseo.

---

## 6. Tres avisos

**No pregunte por la solución.** Si el cliente pide un botón, pregunte
para qué lo quiere. Puede que necesite un informe, o puede que necesite no
tener que exportar nada.

**No pregunte «¿quiere que sea fácil de usar?»** Nadie responde que no.
Pregunte qué le molesta hoy, cuánto tiempo le cuesta, y qué hace cuando se
equivoca.

**No cierre la entrevista con todo resuelto.** Salir con tres preguntas
abiertas y anotadas es mejor que salir con tres huecos rellenados a ojo.
Ese es el sentido del `[NECESITA ACLARACIÓN]`: no es una falla, es lo que
evita construir sobre una suposición.

---

## 7. En este proyecto

**En este repositorio la elicitación es la fase 0, y está SIMULADA.** No hubo un
cliente real: «Comercial Los Andes S.A.» es ficticia y don Hernán no existe. Lo
que no está simulado es **el orden**: primero se preguntó, después salieron las
reglas, y de las reglas los disparadores y los procedimientos.

> **Y hay una distinción que este proyecto hace y conviene copiar:** las **12
> tablas** vienen del curso de Bases de Datos —el modelo ya estaba—, pero **los 3
> disparadores y los 16 procedimientos se escribieron aquí**. Son **760 de las
> 849 líneas de código** del script — el 89 %.
>
> **Las tablas no deciden nada.** Todas las reglas del negocio viven en lo que se
> escribió en este proyecto — y todas salen de la elicitación.

Lo que sí hay es el lugar donde esos insumos se convierten en decisiones, que es
lo que este documento enseña a mirar:

| Dónde | Qué papel cumple |
|---|---|
| [`2_spec.md`](../spec_kit/versiones/v1_sin_fk/2_spec.md) de cada versión | El QUÉ, con sus criterios de aceptación. Es lo que en otro proyecto habrían producido las historias |
| [`4_research.md`](../spec_kit/versiones/v1_sin_fk/4_research.md) | **Las decisiones y las alternativas que se descartaron.** Es lo más cercano a un acta: dice qué se preguntó y por qué se resolvió así |
| [`5_data_model.md`](../spec_kit/versiones/v1_sin_fk/5_data_model.md) | El esquema tal como llegó, que aquí es el insumo dado |
| [`3_plan.md`](../spec_kit/versiones/v1_sin_fk/3_plan.md) | El CÓMO, y los hallazgos que lo cambiaron |

> **Y eso no vuelve inútil este documento, al contrario.** Cuando el equipo haga
> su proyecto de aula sí va a tener que elicitar — con un cliente de verdad o
> con el profesor haciendo de uno—, y entonces estas cuatro rondas de preguntas
> son el método. Lo que no se puede es inventar un acta de una reunión que no
> ocurrió.

### Y entonces, ¿qué es `docs/dominio/elicitacion/`?

Hay una carpeta con tres documentos que **parecen** una elicitación de este
proyecto. **Lea la advertencia de su primera línea: es SIMULADA.** La empresa es
ficticia, «don Hernán» no existe y las comillas son un recurso de redacción, no
una fuente.

| | |
|---|---|
| [`1_PREGUNTAS.md`](../dominio/elicitacion/1_PREGUNTAS.md) | Las cuatro rondas, aplicadas a este dominio |
| [`2_RESPUESTAS.md`](../dominio/elicitacion/2_RESPUESTAS.md) | Citas inventadas, **y al lado la columna REAL que cada una produjo** |
| [`3_HISTORIAS_PROPUESTAS.md`](../dominio/elicitacion/3_HISTORIAS_PROPUESTAS.md) | Cinco huecos del sistema, escritos como historias |

**Lo que sirve de ahí, y lo que no:**

| Sirve para | NO sirve para |
|---|---|
| Ver el **puente**: una frase de negocio → una regla → un disparador | Ejemplo de **cómo se conduce** una reunión real |
| Seguir la cadena completa y comprobarla en el `.sql` | Evidencia de que alguien lo dijo: **las personas son ficción** |

> **Lo que no se puede simular es la incomodidad.** Una elicitación real tiene
> tramos confusos, contradicciones entre el minuto 12 y el 58, y cosas que el
> usuario reconoce no saber. Para eso está `proyecto_catedras2`, donde la reunión
> ocurrió y la transcripción existe con todo su ruido.
>
> **Y de ahí la señal de alarma para el proyecto de aula: si su elicitación se
> lee demasiado limpia, sospeche de ella.** Ese ruido es la prueba de que la
> fuente existió.

**Lea la sección 2 del [`PLAN_V1.md`](../dominio/PLAN_V1.md).** Ahí está lo que este documento describe
en abstracto: alguien leyó el insumo, lo contrastó con los datos reales, y **lo
que encontró cambió el plan**. Eso es elicitación — no transcribir lo que
piden, sino entenderlo lo suficiente como para descubrir lo que no dijeron.

## 8. Referencias

### Científicas

**Las normas y el cuerpo de conocimiento**

1. **ISO/IEC/IEEE 29148:2018** — *Systems and software engineering — Life cycle
   processes — Requirements engineering*. Define qué es un requisito bien
   formado: necesario, no ambiguo, singular, verificable, trazable.
2. **IEEE Computer Society** (2024). ***SWEBOK Guide v4.0***, publicada el
   **15 de octubre de 2024**. El área *Software Requirements* fue **reescrita
   por completo** frente a la v3.

**Fundacionales sobre la entrevista**

3. **Bano, M.; Zowghi, D.; Ferrari, A.; Spoletini, P.; Donati, B.** (2019).
   «Teaching requirements elicitation interviews: an empirical study of learning
   from mistakes». *Requirements Engineering* **24**(3).
   DOI **`10.1007/s00766-019-00313-0`**
   — 110 estudiantes en 28 grupos. Es de donde sale la afirmación de la
   sección 1: los novatos fallan sobre todo en **formulación de la pregunta,
   omisión de la pregunta y orden de la entrevista**, y **no mejoran solos**
   entre una entrevista y la siguiente. Por eso aquí la entrevista se
   **diseña antes**, no se improvisa.
4. **Ferrari, A.** *et al.* (2018). «Learning from Mistakes: An Empirical Study
   of Elicitation Interviews Performed by Novices». **IEEE RE 2018**.

**Actualizadas — la IA en la elicitación (2024-2026)**

5. «Generative AI for Requirements Engineering: A Systematic Literature
   Review». arXiv **`2409.06741`** — la revisión sistemática del área.
6. «From Chat to Interview: Agentic Requirements Elicitation with an Experience
   Ontology» (*OntoAgent*). arXiv **`2605.05828`**, 2026 — un agente que
   **entrevista** guiado por una ontología, en vez de conversar libremente.
7. «LLM-Based Discovery of Latent Requirements from Stakeholder
   Conversations» (*LENS*). arXiv **`2606.25867`**, 2026 — extrae los
   requisitos explícitos **y infiere los latentes** de una transcripción.
8. «Automated Alignment between Elicitation Interviews and Requirements»
   (*Inter2US*). arXiv **`2510.08622`** — **mide cuánto de la historia está de
   verdad en la entrevista**, con 0,86 de macro-F1. Es exactamente la
   comprobación que a este proyecto le falta hacer sobre sus 15 historias.

> **Comprobadas en línea el 14 de septiembre de 2026.** Las que llevan DOI se pueden abrir por el DOI; las de arXiv, por su identificador.

### De gurús, blogs y fuentes primarias


1. Adzic, Gojko. *Impact Mapping: Making a Big Impact with Software
   Products and Projects*. Provoking Thoughts, 2012.
2. Patton, Jeff. *User Story Mapping: Discover the Whole Story, Build the
   Right Product*. O'Reilly, 2014.
3. Wynne, Matt. «Introducing Example Mapping». Cucumber, 2015 —
   `https://cucumber.io/blog/bdd/example-mapping-introduction/`
4. Ferguson Smart, John. «The anatomy of a Three Amigos requirements
   discovery workshop» —
   `https://johnfergusonsmart.com/three-amigos-requirements-discovery/`
5. Cohn, Mike. *User Stories Applied: For Agile Software Development*.
   Addison-Wesley, 2004.
6. Wake, Bill. «INVEST in Good Stories, and SMART Tasks». XP123,
   17 de agosto de 2003 —
   `https://xp123.com/articles/invest-in-good-stories-and-smart-tasks/`
