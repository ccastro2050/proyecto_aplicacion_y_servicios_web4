# Historias propuestas — lo que la fuente pide y NO está construido

> # ⚠ PROPUESTAS, NO REQUISITOS
>
> **Ninguna de estas cinco historias está construida, y ninguna está aprobada.**
> No las pidió nadie: salen de lo que la elicitación **simulada** deja al
> descubierto. Ver [`1_PREGUNTAS.md`](1_PREGUNTAS.md) §0.
>
> **No se pueden usar como criterio de aceptación de nada.** Si una de éstas
> entrara al sistema, tendría que pasar primero por un `2_spec.md`.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## Antes de leerlas: tres advertencias

**1. Una historia propuesta no es una historia.** Le falta lo único que la hace
una historia: que alguien con autoridad sobre el problema diga *«sí, eso
necesito»*. Hasta entonces es una hipótesis bien escrita.

**2. Están aquí porque el hueco se puede señalar, no porque haya que taparlo.**
Al menos dos de las cinco probablemente **no deberían construirse** en un
proyecto de clase — y lo dice cada una.

**3. Y la más importante: este documento existe para que el hueco se vea.** Un
sistema sin lista de lo que le falta parece completo. Y ninguno lo está.

> **Cómo se leen las etiquetas:**
>
> | | Qué significa |
> |---|---|
> | 🔴 **Hueco grave** | En un sistema real esto no es opcional |
> | 🟡 **Hueco declarado** | Falta, está dicho en la documentación, y está bien que falte **aquí** |
> | ⚪ **Mejora** | Nadie se queja hoy; mejoraría el sistema |

---

## HP-01 🔴 · Saber quién emitió y quién anuló cada factura

> **Como** jefe de ventas,
> **quiero** ver qué usuario registró cada factura y qué usuario la anuló, con la
> fecha,
> **para** poder responder cuando alguien pregunta por un movimiento raro.

**De dónde sale:** de la pregunta que **no se hizo** — *«¿quién responde si un
total sale mal?»* — y del hueco que eso dejó.

**Por qué no está:** porque el esquema llegó así. `factura` tiene seis columnas,
y `fkidvendedor` dice **a quién se le acredita la venta**, que es otra cosa: un
cajero puede registrar la venta de otro vendedor.

| Criterio de aceptación | |
|---|---|
| 1 | Cada factura guarda **qué usuario** la creó, y la fecha ya existe |
| 2 | Al anular, se guarda **qué usuario** anuló y **cuándo** |
| 3 | Esos datos **no se pueden editar** desde ninguna pantalla |
| 4 | Una factura sembrada antes de este cambio queda con el autor **en blanco**, y se ve en blanco — no se inventa |

> **El criterio 4 es el que más se olvida en una migración**, y es el que decide
> si el dato sirve: si al agregar la columna se rellena con *«admin»* para que no
> quede vacía, el campo queda **inservible para siempre** — nadie podrá
> distinguir lo que de verdad hizo admin de lo que se rellenó.
>
> **«Sin dato» es un dato. «Inventado» no lo es.**

**Y por qué esto es grave de verdad:** sin auditoría, la pregunta *«¿quién anuló
la factura 143?»* **no tiene respuesta posible**. No es que sea difícil de
consultar: es que el dato nunca se guardó, y no hay forma de recuperarlo.

---

## HP-02 🔴 · Avisar cuando un producto está por agotarse

> **Como** jefe de ventas,
> **quiero** que el sistema me avise cuando un producto baje de un mínimo que yo
> fijo,
> **para** pedir antes de quedarme sin con qué vender.

**De dónde sale:** directamente de la respuesta 1.1 — *«al final del mes nadie
sabe cuánto hay en bodega de verdad»*. El sistema resolvió la **mitad** del
problema: el stock ahora es exacto. Pero sigue siendo **pasivo**: hay que ir a
mirarlo.

**Por qué no está:** las diez consultas de la v4 incluyen
`productos-sin-vender`, que es otra pregunta — *«¿qué nunca se ha vendido?»*, no
*«¿qué se está acabando?»*.

| Criterio de aceptación | |
|---|---|
| 1 | Cada producto tiene un **mínimo** configurable |
| 2 | El tablero muestra los que están **por debajo** de su mínimo |
| 3 | Un producto sin mínimo fijado **no aparece**, y no se le inventa uno |
| 4 | El aviso **no bloquea** vender: avisa, no impide |

> **El criterio 4 marca la diferencia entre un aviso y una regla**, y confundirlos
> rompe el negocio. **RN-10** —el stock no queda negativo— **sí** es una regla y
> **sí** bloquea. «Estás por debajo del mínimo» es información: el usuario puede
> vender hasta el último, y debe poder.
>
> **Si esto bloqueara, habría inventario que no se puede vender** — que es
> exactamente lo contrario de lo que don Hernán pidió.

**Qué haría falta en la base:** una columna `stock_minimo` en `producto`. Es el
cambio más pequeño de las cinco historias, y el que más resuelve del problema
original.

---

## HP-03 🟡 · Que la factura salga en un archivo que se pueda mandar

> **Como** vendedor,
> **quiero** descargar la factura en un archivo,
> **para** mandársela al cliente sin tomarle una foto a la pantalla.

**De dónde sale:** de la última frase de la respuesta 4.8 — *«se la mandé por
WhatsApp en foto»*. Eso **no** es lo que el sistema hace: es lo que el usuario
hace **porque** el sistema no lo hace.

| Criterio de aceptación | |
|---|---|
| 1 | Desde una factura se descarga un archivo con sus datos y sus renglones |
| 2 | Una factura **anulada** sale marcada como anulada, visiblemente |
| 3 | El archivo usa el **manual de marca** |
| 4 | **No se manda por correo**: se descarga. Mandarlo es otra historia |

> **El criterio 4 parte la historia a propósito**, y es una decisión de alcance,
> no pereza: generar el archivo es trabajo de formato; mandarlo por correo es un
> servidor de correo, credenciales, reintentos y qué hacer cuando una dirección
> rebota. **Juntarlos es garantizar que ninguna de las dos se termine.**

**🟡 Y por qué está bien que falte AQUÍ.** Generar un PDF no enseña nada de lo
que este curso enseña — ni capas, ni SQL, ni contratos, ni transacciones: enseña
a usar una biblioteca de PDF. En un sistema real es casi obligatorio; en este
ejemplo sería tiempo gastado lejos del objetivo. Está en «lo que el sistema no
promete», y ahí es donde cuenta.

---

## HP-04 🟡 · Registrar cómo ENTRA el inventario

> **Como** encargado de bodega,
> **quiero** registrar cuando llega mercancía,
> **para** que el stock del sistema sea el de la bodega sin tener que corregirlo
> a mano.

**De dónde sale:** de un hueco que ninguna pregunta tocó y que el modelo deja a
la vista. El stock **baja** al vender (**RN-11**) y **sube** al anular
(**RN-12**). Pero **cómo llegó ahí la primera vez, no se modela**.

**Cómo se resuelve hoy:** con un `PATCH /api/producto/PR001 {"stock": 50}`. Es
decir: **escribiendo el número a mano**.

| Criterio de aceptación | |
|---|---|
| 1 | Una entrada registra producto, cantidad, fecha y quién la registró |
| 2 | Registrar una entrada **sube** el stock, por el mismo camino que una venta lo baja |
| 3 | El stock de un producto se puede **explicar**: entradas − ventas + anulaciones |
| 4 | Ya **no se puede** cambiar el stock con un `PATCH` directo |

> **El criterio 3 es el que de verdad justifica la historia**, y es una idea que
> vale más que esta función: **un número que no se puede explicar no se puede
> defender**. Hoy, si el stock dice 14 y la bodega tiene 12, no hay forma de
> saber dónde se separaron. Con un registro de movimientos, el número **tiene
> historia**.

> **Y el criterio 4 es el precio, dicho de frente.** Esta historia **quita** una
> capacidad que hoy existe. Eso es correcto —el `PATCH` de stock es el agujero
> por donde se descuadra el inventario— pero hay que decirlo, porque alguien lo
> está usando.

**🟡 Por qué está bien que falte aquí:** es un **segundo dominio** —movimientos
de inventario—, con sus tablas, sus reglas y su pantalla. Duplicaría el tamaño
del ejemplo sin agregar un concepto nuevo de los que el curso enseña.

---

## HP-05 ⚪ · Que el borrador de la factura sobreviva a un F5

> **Como** vendedor,
> **quiero** que los renglones que llevo agregados no se pierdan si recargo la
> página,
> **para** no volver a empezar una factura de ocho renglones.

**De dónde sale:** no de la fuente simulada, sino de una **limitación real y
documentada** del front. Es la historia más honesta de las cinco: nace de haber
usado el sistema, no de haber imaginado un usuario.

**Por qué pasa:** Blazor Server mantiene el estado de la pantalla en un
**circuito** en el servidor. Los renglones viven ahí, no en la base. Si el
circuito se corta —un F5, una red que se cae—, **el borrador se pierde**.

| Criterio de aceptación | |
|---|---|
| 1 | Los renglones agregados sobreviven a una recarga |
| 2 | Un borrador abandonado **no** se convierte nunca en factura |
| 3 | Un borrador **no toca el stock**: el stock se mueve al emitir |
| 4 | Dos personas pueden tener su propio borrador a la vez, sin verse |

> **El criterio 3 es el que hay que leer dos veces.** Si el borrador tocara el
> stock, un vendedor que agrega ocho renglones y se va a almorzar dejaría ocho
> productos reservados sin que nadie lo sepa. **El stock se mueve cuando la
> venta existe, no cuando alguien la está pensando.**

> **Y esto es exactamente lo que el curso compara entre los tres fronts.** En el
> front de **Flask** del curso de Diseño el borrador vive en la **sesión** y
> **sobrevive al F5**; en Blazor vive en el circuito y no. Es la única diferencia
> funcional real entre los dos, y está en
> [`ARQUITECTURA.md`](../ARQUITECTURA.md) §5.
>
> **Es la mejor lección disponible sobre «elegir una tecnología es elegir sus
> consecuencias»**: nadie escogió Blazor *para* perder el borrador. Vino en el
> paquete.

---

## Lo que estas cinco NO resuelven, y conviene no esperar que resuelvan

| Sigue faltando | Y por qué no se propone |
|---|---|
| **Devoluciones parciales** | *«Han pasado dos veces en tres años»*. Ese número es lo que permite decidir **no** construirlo |
| **Impuestos y descuentos** | El contador los maneja aparte, y el descuento se hace bajando el precio |
| **Varias bodegas o sedes** | El stock es uno, global. Nadie pidió más |
| **Histórico de precios** | El renglón ya congela el precio (**RN-07**); para lo demás nadie lo pidió |
| **Archivar facturas viejas** | No hay volumen que lo justifique |

> **Las cinco de esa tabla están en
> [`DISENO_BD.md`](../DISENO_BD.md) §5 como «lo que este modelo NO resuelve»** —
> y ahí es donde cuentan, declaradas en vez de calladas.

> **Y la primera fila es la lección de todo este documento.** *«Dos veces en tres
> años»* es el dato que permite **no construir** algo con argumentos. Sin ese
> número, alguien habría modelado devoluciones parciales «por si acaso» — y
> habría tenido razón, porque sin datos la precaución siempre parece razonable.
>
> **Una buena elicitación no sirve solo para decidir qué se construye: sirve
> sobre todo para poder decir «esto no», y sostenerlo.**

---

| Qué | Dónde |
|---|---|
| Las preguntas | [`1_PREGUNTAS.md`](1_PREGUNTAS.md) |
| Las respuestas simuladas | [`2_RESPUESTAS.md`](2_RESPUESTAS.md) |
| Lo que sí está construido | [`REQUISITOS_FUNCIONALES.md`](../REQUISITOS_FUNCIONALES.md) |
| Lo que el modelo no resuelve | [`DISENO_BD.md`](../DISENO_BD.md) §5 |
| Qué es una historia de usuario | [`CONCEPTOS_HISTORIAS_DE_USUARIO.md`](../../conceptos/CONCEPTOS_HISTORIAS_DE_USUARIO.md) |
