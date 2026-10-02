# El requisito no funcional — el que hunde el proyecto en producción

> **Qué es este archivo.** El conceptual: qué es un requisito no funcional, por
> qué el nombre es malo, cómo se escribe uno que se pueda medir, y qué hacer
> cuando dos se pelean.
>
> Su instancia para este proyecto es
> [`../dominio/REQUISITOS_NO_FUNCIONALES.md`](../spec_kit/1_constitution.md).

---

## 1. El nombre es malo, y conviene decirlo primero

«No funcional» suena a **opcional**, a lo que se hace si sobra tiempo. Es al
revés:

> El requisito funcional decide si el sistema **sirve**.
> El no funcional decide si el sistema **se puede usar, mantener y defender**.

Un sistema que cumple los treinta y cuatro requisitos funcionales y tarda
cuarenta segundos en listar, o tiene el texto ilegible, o guarda las
contraseñas en claro, **está terminado y no sirve**.

**Se les llama también *atributos de calidad*, y es mejor nombre** porque dice
lo que son. Aquí se usan los dos: «no funcional» porque es como aparece en la
bibliografía, «calidad» cuando se habla del modelo que los organiza.

---

## 2. La regla que los hace útiles: **número, unidad y forma de medirlo**

Es una sola regla y lo decide todo.

```
MAL    El sistema debe ser rápido.
MAL    El sistema debe ser rápido y responder en poco tiempo.
MAL    El listado debe responder en menos de 1 segundo.        ← ¿cuál listado?
                                                                ¿con cuánta carga?
BIEN   La mediana de GET /api/producto responde por debajo de 200 ms
       con 10 peticiones concurrentes, medida sobre 100 repeticiones.
```

**Tres cosas, y si falta una el requisito no se puede verificar:**

| | | Sin esto pasa que… |
|---|---|---|
| **El número** | `200` | «rápido» lo decide quien mire |
| **La unidad** | `ms` | nadie sabe contra qué comparar |
| **La forma de medirlo** | *mediana, 10 concurrentes, 100 repeticiones* | dos personas miden distinto y las dos tienen razón |

> **La forma de medirlo es la que casi siempre falta**, y es la que más
> discusiones evita. «Responde en 200 ms» ¿es la mediana o el peor caso? ¿Con
> la base de datos vacía o con datos? ¿Desde la misma máquina o por la red?

---

## 3. El modelo que los organiza: ISO/IEC 25010:**2023**

**Citar el año importa**, porque la revisión de 2023 cambió cosas y **casi todo
lo que hay en internet describe la de 2011**:

- *Usabilidad* pasó a llamarse **capacidad de interacción**.
- *Portabilidad* pasó a llamarse **flexibilidad**.
- **Seguridad física** (*safety*) es una característica **nueva**.

Las **nueve** características, con la pregunta que contesta cada una:

| Característica | ¿El sistema…? |
|---|---|
| **Adecuación funcional** | ¿hace lo que se pidió, completo y correcto? |
| **Eficiencia de desempeño** | ¿en cuánto tiempo, con cuántos recursos, hasta qué volumen? |
| **Compatibilidad** | ¿convive con lo demás y se entiende con ello? |
| **Capacidad de interacción** | ¿lo puede usar una persona — **incluida una con una discapacidad**? |
| **Fiabilidad** | ¿sigue funcionando, y se recupera cuando no? |
| **Seguridad** | ¿protege los datos de quien no debe verlos? |
| **Mantenibilidad** | ¿se puede cambiar sin romperlo? |
| **Flexibilidad** | ¿se adapta a otro entorno, otro motor, otra escala? |
| **Seguridad física** | ¿puede causar daño si falla? |

> **Usar el modelo como lista de chequeo es la mitad de su valor.** No es para
> clasificar lo que ya se escribió: es para **recorrer las nueve y preguntarse
> qué falta**. Casi siempre aparecen tres o cuatro que nadie había pensado — y
> suelen ser fiabilidad, recuperación y capacidad.
>
> **Y una característica que no aplica se declara.** «Seguridad física: no
> aplica, este sistema no controla nada que pueda hacer daño» es una respuesta
> correcta. El silencio no.

---

## 4. Dónde viven, y por qué casi nunca en `2_spec`

Un requisito funcional cambia en cada versión; un no funcional **casi nunca**.
De ahí sale el reparto:

| Dónde | Cuáles | Por qué |
|---|---|---|
| **El catálogo** | Todos | Es el índice, y no se repite por versión |
| **La constitución** | Los **transversales** | Rigen siempre y **no se negocian**: son una *política* |
| **`2_spec` §4 de cada versión** | Los que esa versión **estrena o mide** | Con su criterio de aceptación concreto |

> **La constitución es el sitio natural de un RNF transversal.** «El SQL va
> siempre parametrizado» no es algo que se cumpla en la v2 y no en la v3.

---

## 5. Cuándo se pelean dos, que es siempre

Los no funcionales **compiten entre sí**, y esa es su diferencia más incómoda
con los funcionales:

| Tensión | Cómo se manifiesta |
|---|---|
| Seguridad ↔ comodidad | Cada control que protege añade un paso al usuario |
| Desempeño ↔ trazabilidad | Registrar todo cuesta tiempo y espacio |
| Flexibilidad ↔ simplicidad | Lo que sirve para cinco motores es más difícil de leer que lo que sirve para uno |
| Capacidad ↔ costo | Aguantar diez veces más vale diez veces más |

**No se resuelven: se ponderan.** Y lo que hay que dejar escrito no es la
decisión, sino **qué se sacrificó**. Una decisión sin su costo declarado no se
puede revisar: solo se puede obedecer.

---

## 6. Los tres errores que se pagan caro

### 6.1 · El adjetivo sin número

«Rápido», «seguro», «amigable», «robusto», «escalable». **Nadie los verifica
nunca**, porque no se puede. Si el catálogo tiene uno, no está incompleto:
está mintiendo.

### 6.2 · Inventar un RNF para justificar una decisión

Es el más tentador, y deja la decisión peor de lo que estaba.

> **El ejemplo es de este proyecto.** La decisión de tener una API específica
> por recurso se sostenía con «la genérica es más lenta». **Se midió y es
> falso**: emparejar la ruta y leer el cuerpo cuesta lo mismo en las dos. La
> decisión era buena, pero el argumento se cayó al primer contacto con un
> cronómetro, y hubo que rehacerla entera sobre lo que sí se podía medir —el
> contrato— .
>
> **Un requisito no funcional inventado tumba una decisión correcta.**

### 6.3 · El RNF que no existe hasta que duele

El caso clásico es la **capacidad**.

> **El ejemplo, también real.** Una API genérica devolvía `limite ?? 50000` sin
> tope — y **distinto según el motor**: cincuenta mil filas contra PostgreSQL,
> mil contra Oracle, con el mismo endpoint y sin que el contrato lo dijera.
> Nadie lo decidió: **nadie escribió el requisito**, y el valor lo puso quien
> escribió la línea.

---

## 7. En el camino del prompt

Sin catálogo de no funcionales, una IA que redacta `2_spec` **escribe igual la
sección 4**. Y lo que escribe es exactamente lo de §6.1: «el sistema debe ser
rápido, seguro y fácil de usar».

Pasa la lectura, no se puede verificar, y **nadie vuelve a mirarlo**.

---

## 8. Referencias

### Normas

1. **ISO/IEC 25010:2023** — *SQuaRE, Product quality model*. **Las nueve
   características de §3.** Citar el año: la revisión de 2023 renombró
   *usabilidad* → **capacidad de interacción** y *portabilidad* →
   **flexibilidad**, y añadió **seguridad física**.
2. **ISO/IEC 25012** — modelo de calidad **del dato**, que es otra cosa y
   también hace falta.
3. **ISO/IEC/IEEE 29148:2018** — §5: un requisito debe ser **verificable**. Es
   de donde sale la regla de §2.
4. **W3C — WCAG 2.2**, Recomendación del **12 de diciembre de 2024**. Los
   umbrales de contraste: **4,5:1** texto normal, **3:1** texto grande y
   elementos no textuales.
5. **IEEE Computer Society** (2024). ***SWEBOK Guide v4.0***, área
   *Software Quality*.

### Científicas actualizadas

6. **WebAIM** — *The WebAIM Million*, informe **2026**: texto de bajo contraste
   en el **83,9 %** del millón de páginas de inicio más visitadas, frente al
   79,1 % en 2025, con **34 instancias por página**. Es el fallo de
   accesibilidad más común de la web **y va empeorando**. Por eso el contraste
   se calcula y no se mira.
7. «Perceptually-Minimal Color Optimization for Web Accessibility».
   arXiv **`2512.05067`** — cómo corregir el contraste **moviendo el color lo
   menos posible**, que es el problema cuando el color es de marca.
8. «A feasibility study on filtering low-accessibility web pages considering
   color vision deficiency». arXiv **`2606.22095`** — **el límite de las
   herramientas**: pasar el 4,5:1 no garantiza que lo vea alguien con
   deficiencia de visión del color.
9. «LLM Hallucinations in Practical Code Generation: Phenomena, Mechanism, and
   Mitigation». *Proceedings of the ACM on Software Engineering*, 2025.
   DOI **`10.1145/3728894`** — la taxonomía incluye el incumplimiento de
   requisitos **no funcionales** como alucinación por conflicto con la entrada.
   Es el respaldo de §7.

### De gurús y herramientas

10. **arc42 quality model** — `quality.arc42.org`. Los **escenarios de
    calidad**: la forma práctica de volver medible un atributo, escribiéndolo
    como *estímulo → sistema → respuesta medible*.
11. **Karl Wiegers y Joy Beatty** — *Software Requirements*, capítulos de
    atributos de calidad.
12. **Michael Nygard** — *Release It!*, sobre lo que rompe en producción y no
    en la máquina del programador.

> **Comprobadas en línea el 14 de septiembre de 2026.**
