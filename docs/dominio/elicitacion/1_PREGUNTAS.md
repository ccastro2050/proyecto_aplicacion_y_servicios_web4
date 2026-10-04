# Preguntas al usuario experto — Comercial Los Andes S.A.

> # ⚠ ESTA ELICITACIÓN ES SIMULADA
>
> **No hubo reunión. No hubo usuario experto. No hay transcripción.** La empresa
> es ficticia y la persona que responde en
> [`2_RESPUESTAS.md`](2_RESPUESTAS.md) **no existe**.
>
> La fuente normativa de este proyecto es **un esquema de base de datos que llegó
> hecho** — Artículo 5 de la constitución — y el curso. Ver
> [`FUENTES.md`](../FUENTES.md) §0 y §4.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Para qué existe este documento

**Es la fase 0 del proyecto**, y es de donde salen las reglas del negocio.

El equipo recibió las **doce tablas** del curso de Bases de Datos — el modelo ya
estaba hecho. Lo que **no** había era un sistema: no había una sola regla que
impidiera vender sin stock, ni que un total cuadrara, ni que una factura no se
anulara dos veces.

> **Esas reglas no se inventan en una reunión de programadores: se preguntan.**
> De aquí salen las 22 de [`REGLAS_DE_NEGOCIO.md`](../REGLAS_DE_NEGOCIO.md), y de
> ahí los **3 disparadores** y los **16 procedimientos** que son el 89 % del
> script — todo lo escrito en este proyecto.

```
estas preguntas  →  las 22 reglas  →  los disparadores y procedimientos  →  la API
```

### Y se puede seguir la cadena completa

| | |
|---|---|
| **La pregunta 3.3** | *«¿Qué pasa si piden más de lo que hay?»* |
| **La respuesta** | *«No se puede vender lo que no tengo. Eso es sagrado.»* |
| **La regla** | **RN-10** — el stock nunca queda negativo |
| **Dónde vive** | `trg_prodfact_insert`, con un `THROW 50001` |

> **Las cuatro se pueden abrir y comparar.** Eso es lo que significa que una
> decisión del sistema **tenga autor** en vez de haber aparecido sola.

---

## 1. Cómo están ordenadas

Cuatro rondas, de lo general a lo concreto. El orden no es decorativo:

| Ronda | Qué busca | Por qué va ahí |
|---|---|---|
| **1 · Por qué existe esto** | El problema, no la solución | Si se pregunta primero por pantallas, se termina construyendo lo que el usuario **imagina**, no lo que **necesita** |
| **2 · Quiénes** | Los actores y sus papeles | De aquí sale quién es cliente, quién vendedor y quién entra al sistema |
| **3 · El día de trabajo** | La secuencia real, no la ideal | Las excepciones viven aquí |
| **4 · Las reglas y los ejemplos** | Qué siempre, qué nunca, y **un caso de verdad** | Un ejemplo concreto desmiente más reglas inventadas que diez preguntas abstractas |

> **La regla de oro de una pregunta buena: que no se pueda responder con «sí».**
> *«¿Necesita facturar?»* no sirve. *«Cuénteme la última factura que emitió
> ayer»* sí, porque la respuesta trae datos, casos raros y el vocabulario de la
> casa.

---

## Ronda 1 — Por qué existe esto

| | Pregunta | Qué se busca |
|---|---|---|
| **1.1** | ¿Qué le está costando tiempo o plata hoy, con lo que tiene? | El problema real, en sus palabras |
| **1.2** | ¿Cómo lo resuelven ahora, sin sistema? | De aquí sale qué NO se puede empeorar |
| **1.3** | ¿Qué tiene que salir del sistema, exactamente? ¿Un papel, un número, un correo? | La salida concreta |
| **1.4** | ¿Quién recibe eso y qué hace con ello? | Si nadie lo usa, no es un requisito |
| **1.5** | Si el sistema solo hiciera **una** cosa, ¿cuál? | La prioridad, dicha por quien manda |

---

## Ronda 2 — Quiénes

| | Pregunta | Qué se busca |
|---|---|---|
| **2.1** | ¿Quiénes son sus clientes: personas, empresas, o las dos? | → `persona`, `empresa`, y la clave foránea **opcional** |
| **2.2** | **¿Puede alguien ser cliente y vendedor a la vez?** | **La pregunta que define el modelo.** Ver `DISENO_BD.md` §1 |
| **2.3** | ¿Qué datos de una persona necesita guardar, y cuáles cambian seguido? | Qué columnas, y por qué no duplicarlas |
| **2.4** | ¿Quién puede entrar al sistema, y quién no debería ver qué? | → `usuario`, `rol`, `ruta` |
| **2.5** | ¿Alguien hace dos papeles — vende y además cobra, por ejemplo? | → por qué los roles son **varios por usuario** |
| **2.6** | ¿Quién reparte los permisos? ¿Usted, o alguien de sistemas? | → la pantalla de permisos |

---

## Ronda 3 — El día de trabajo

| | Pregunta | Qué se busca |
|---|---|---|
| **3.1** | Cuénteme, paso por paso, cómo se emite una factura hoy | La secuencia real |
| **3.2** | ¿Cuántos productos lleva una factura típica? ¿Y la más larga que recuerde? | → el maestro-detalle, y cuánto tiene que aguantar |
| **3.3** | ¿Qué pasa si piden más de lo que hay en bodega? | → **RN-10**, el stock que no queda negativo |
| **3.4** | ¿Quién sabe cuánto hay en bodega, y cómo lo sabe? | → el stock, y quién lo mueve |
| **3.5** | **¿Qué es lo que más se equivocan?** | Donde el sistema tiene que estorbar |
| **3.6** | ¿Alguna vez hay que deshacer una factura ya emitida? ¿Y qué hacen? | **→ la anulación.** Ver Ronda 4 |

---

## Ronda 4 — Las reglas y los ejemplos

| | Pregunta | Qué se busca |
|---|---|---|
| **4.1** | ¿Qué tiene que pasar **siempre**, sin excepción? | Las reglas duras |
| **4.2** | ¿Qué **no puede pasar nunca**? | Las prohibiciones, que son las que se programan |
| **4.3** | **¿Se puede corregir una factura ya emitida?** | **→ RN-14: la anulación en vez de la edición** |
| **4.4** | Si un producto sube de precio, ¿qué pasa con las facturas viejas? | **→ RN-07: el subtotal congelado** |
| **4.5** | ¿El total lo calcula alguien a mano, o sale solo? | **→ RN-08 y RN-09** |
| **4.6** | ¿Hay descuentos, impuestos, devoluciones parciales? | Lo que queda **fuera de alcance**, y dicho |
| **4.7** | ¿Qué límites tienen los datos? ¿Un nombre de cuántas letras, un crédito de cuánto? | Los tipos y las restricciones |
| **4.8** | Déme un ejemplo **de verdad**: la última factura de ayer, con nombres y cifras | El caso que desmiente lo inventado |

---

## 5. Lo que estas preguntas NO cubren, y hay que decirlo

La fase 0 dejó huecos, y **declararlos es parte del trabajo**:

| No se preguntó | Y eso se nota en que… |
|---|---|
| **Cuántas facturas al día, en el pico** | El sistema no promete ningún tiempo de respuesta (`REQUISITOS_NO_FUNCIONALES.md` §8) |
| **Qué pasa si se cae el sistema a mitad de una factura** | La transacción lo resuelve, pero nadie lo pidió: salió del esquema |
| **Quién responde si un total sale mal** | No hay auditoría: se sabe a qué **vendedor** se le atribuye, no qué **usuario** la escribió |
| **Si hay sedes o bodegas distintas** | El stock es **uno**, global por producto |
| **Cuánto tiempo se guardan las facturas** | Para siempre: no hay archivado |

> **El tercero es el más grave, y conviene mirarlo de frente**, porque la
> distinción es fina. `factura` tiene estas seis columnas, y ni una más:
>
> ```sql
> numero · fecha · total · estado · fkidcliente · fkidvendedor
> ```
>
> **Hay un `fkidvendedor`, sí — pero eso es atribución comercial, no
> auditoría.** Dice a quién se le acredita la venta; **no dice qué usuario
> escribió la factura** en el sistema, ni quién la anuló, ni cuándo. Y son dos
> preguntas distintas: un cajero puede registrar la venta de otro vendedor.
>
> **No existe ninguna columna de auditoría en las doce tablas** — se puede
> comprobar: `grep -i 'creado_por\|modificado' db/bdfacturas.sql` no devuelve
> nada. En un sistema de facturación de verdad eso es inaceptable, y aquí no está
> porque **el esquema llegó así** y nadie lo elicitó.
>
> **Es el mejor argumento a favor de elicitar antes de modelar:** nadie echa de
> menos la pregunta que no se hizo, hasta el día en que hay que responderla. Y
> entonces ya no hay dónde buscar el dato, porque nunca se guardó.
>
> **En el proyecto de aula esto se califica.** Un equipo que modele facturación
> sin saber quién hizo cada movimiento está repitiendo el hueco de este ejemplo
> — y el ejemplo, en esto, **no hay que copiarlo**.

---

| Qué | Dónde |
|---|---|
| Las respuestas simuladas | [`2_RESPUESTAS.md`](2_RESPUESTAS.md) |
| Lo que la fuente pide y no está construido | [`3_HISTORIAS_PROPUESTAS.md`](3_HISTORIAS_PROPUESTAS.md) |
| Qué es elicitar, y por qué | [`CONCEPTOS_ELICITACION.md`](../../conceptos/CONCEPTOS_ELICITACION.md) |
| Por qué aquí no hubo | [`FUENTES.md`](../FUENTES.md) §4 |
