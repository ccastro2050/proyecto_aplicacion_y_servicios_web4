# Respuestas del usuario experto — Comercial Los Andes S.A.

> # ⚠ ESTAS CITAS SON INVENTADAS
>
> **Nadie dijo esto.** No hubo reunión, no hubo grabación, no hay transcripción.
> «Don Hernán», el jefe de ventas que habla aquí, **no existe**.
>
> Las comillas son un recurso de redacción, no una fuente. Si alguien cita este
> documento como evidencia de un requisito, está citando una **ficción
> didáctica**. Ver [`FUENTES.md`](../FUENTES.md) §4.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Cómo leer este documento, que es lo que lo hace útil

Cada respuesta trae **tres cosas**, y la tercera es la que vale:

| | Qué es |
|---|---|
| 💬 **La cita** | Lo que un usuario *podría* haber dicho, en lenguaje de negocio |
| 🔎 **Qué se saca de ahí** | La traducción a requisito o a regla |
| ⚙️ **Dónde quedó** | **La columna, el disparador o el procedimiento REAL** que existe en `bdfacturas` |

> **La tercera columna es verdad verificable; las personas son ficción.** La
> decisión existe —se puede abrir el `.sql` y verla—, y la frase de negocio que
> la justifica está escrita al lado.
>
> **Y ése es el ejercicio: leer la fila completa, de izquierda a derecha.** Una
> frase de alguien que no sabe programar se convierte en una restricción, un
> disparador o un procedimiento. Esa traducción es el trabajo de la fase 0, y es
> lo que separa un sistema de una base de datos con pantallas encima.

---

## Ronda 1 — Por qué existe esto

### 1.1 · ¿Qué le está costando tiempo hoy?

> 💬 *«Facturamos en un cuaderno y en un Excel que cada vendedor tiene en su
> computador. El problema no es facturar: es que al final del mes nadie sabe
> cuánto hay en bodega de verdad. Yo vendo quince impresoras y resulta que había
> doce.»*

🔎 **El problema no es emitir la factura: es que el inventario y la venta están
separados.** Lo que se pide no es un facturador, es que **vender descuente**.

⚙️ Dos disparadores, no código de aplicación:

```sql
trg_prodfact_insert   -- al agregar un renglón: comprueba stock y lo BAJA
trg_prodfact_delete   -- al quitarlo: lo DEVUELVE
```

> **Y por eso están en la base de datos y no en C#** (→ **RN-10**, **RN-11**): si el
> descuento viviera en la aplicación, un `INSERT` desde SSMS vendería sin
> descontar — y el problema del cuaderno estaría de vuelta, con más pasos.

### 1.2 · ¿Cómo lo resuelven ahora?

> 💬 *«A mano. Y el viernes me siento con los tres vendedores a cuadrar. Nos
> toma la tarde, y casi siempre algo no cuadra.»*

🔎 De aquí sale lo que **no se puede empeorar**: el sistema tiene que cuadrar
solo. Un total que haya que revisar a mano no sirve de nada.

⚙️ El total **no lo calcula nadie a mano, y tampoco el front**: lo recalcula un
disparador cada vez que un renglón cambia (→ **RN-08**).

### 1.3 · ¿Qué tiene que salir, exactamente?

> 💬 *«Una factura con su número, la fecha, el cliente, lo que llevó, y el total.
> El número es sagrado: es el que el cliente me nombra cuando llama.»*

🔎 *«El número es sagrado»* es un requisito duro disfrazado de comentario: el
número **lo pone el sistema**, no se repite, y **no se reusa**.

⚙️ `numero INT IDENTITY(1,1)` con `pk_factura` (→ **RN-03**).

> **Lo que ese `IDENTITY` descarta:** que el número lo escriba el vendedor. Si lo
> escribiera, dos vendedores sin internet pondrían el mismo — y el dato que el
> cliente nombra por teléfono dejaría de identificar nada.

---

## Ronda 2 — Quiénes

### 2.2 · ¿Puede alguien ser cliente y vendedor a la vez?

**Ésta es la pregunta que define el modelo entero.**

> 💬 *«Claro. Pedro Castillo me vende y además me compra: tiene un negocio
> aparte y se surte conmigo. Y hay dos o tres más así. Es el mismo Pedro, ¿no?
> El mismo teléfono, el mismo correo.»*

🔎 **«Es el mismo Pedro» es la frase de la que sale toda la estructura.** Si una
persona puede hacer los dos papeles, **el papel no puede ser un atributo de la
persona**.

⚙️ `persona` por un lado, y `cliente` y `vendedor` como **tablas aparte** que
apuntan a ella:

```
persona  ←  cliente
         ←  vendedor
```

> **Lo que se descartó, y por qué:** una columna `tipo` en `persona` con los
> valores `cliente` y `vendedor`. Habría obligado a **duplicar la fila de
> Pedro** — el mismo ser humano, dos veces — y entonces su teléfono vive en dos
> sitios. **El día que cambie de número, una de las dos se queda vieja.**
>
> **Y no es hipotético: está en los datos sembrados.** `persona` tiene 6 filas y
> entre `cliente` y `vendedor` hay 7, porque **P006 Pedro Castillo** está en las
> dos. Se puede comprobar hoy: [`DATOS_DE_PRUEBA.md`](../DATOS_DE_PRUEBA.md) §2.
>
> **Ésa es la respuesta corta a por qué hay doce tablas y no ocho.** Una pregunta
> de la Ronda 2 decidió el modelo.

### 2.1 · ¿Sus clientes son personas o empresas?

> 💬 *«Las dos. Hay empresas que me compran con factura a nombre de la empresa, y
> hay personas que compran para ellas. A la persona natural no le pongo
> empresa.»*

🔎 La empresa es **opcional**; la persona, **obligatoria**. Son dos
participaciones distintas en la misma tabla.

⚙️ En `cliente`:

| Columna | | Y eso significa |
|---|---|---|
| `fkcodpersona` | `NOT NULL` | **Todo cliente es una persona.** Sin excepción |
| `fkcodempresa` | admite nulo | Hay clientes sin empresa |

> **Y de ese «admite nulo» sale un tropiezo entero de la v2.** Un desplegable
> vacío en HTML manda `""`, no `null` — y `""` no es «no tiene empresa», es «el
> código de empresa es la cadena vacía». La API responde **422**. Ver
> [`PLAN_V2.md`](../PLAN_V2.md) §5.
>
> **Una sola palabra del usuario —«opcional»— produjo una columna, un control de
> interfaz y un error que cuesta una tarde encontrar.**

### 2.5 · ¿Alguien hace dos papeles?

> 💬 *«Sí. Marta vende y también cobra, porque es la que está en el mostrador.
> Y yo soy el jefe pero también facturo cuando hay mucha gente.»*

🔎 Un usuario tiene **varios** roles, y no uno.

⚙️ `rol_usuario`, tabla puente con llave compuesta `(fkemail, fkidrol)`
(→ **RN-19**).

> **Y la llave compuesta, sin `id` propio, es una decisión:** un `id` permitiría
> dos filas con la misma pareja y distinto id — que es exactamente lo que la
> tabla existe para impedir. «Marta es cajera» es verdad o no es verdad; no puede
> ser verdad dos veces.

### 2.6 · ¿Quién reparte los permisos?

> 💬 *«Yo. Y no quiero llamar a un ingeniero cada vez que entra alguien nuevo.»*

🔎 Repartir permisos es una **pantalla**, no una tarea de base de datos. Y el
cambio tiene que surtir efecto **ya**.

⚙️ `rutarol` con su CRUD, y el permiso se consulta **en cada petición** con
`verificar_acceso_ruta` (→ **RN-21**).

> **«No quiero llamar a un ingeniero» es el requisito; «en cada petición» es la
> consecuencia técnica.** Si el permiso viajara dentro del token, quitarle un
> permiso a alguien no surtiría efecto hasta que el token expire — y don Hernán
> tendría que pedirle a esa persona que vuelva a entrar. Consultarlo cada vez
> cuesta una llamada a la base de datos, y compra que el cambio sea inmediato.

---

## Ronda 3 — El día de trabajo

### 3.2 · ¿Cuántos productos lleva una factura?

> 💬 *«Casi siempre uno o dos. La más larga que recuerdo tenía como ocho, de una
> empresa que surtió oficina nueva.»*

🔎 **Maestro-detalle**, con el detalle de tamaño variable. Y nunca de tamaño
cero.

⚙️ `productosporfactura` con `fknumfactura NOT NULL`, y el procedimiento
`sp_insertar_factura_y_productosporfactura` que **rechaza una factura sin
renglones** (→ **RN-02**).

> **Los datos sembrados tienen facturas de 1, 2 y 3 renglones a propósito**: son
> los tres casos del maestro-detalle. Si todas tuvieran uno, el formulario podría
> estar roto para dos y nadie se daría cuenta.

### 3.3 · ¿Y si piden más de lo que hay?

> 💬 *«No se puede vender lo que no tengo. Eso es sagrado. Si el sistema me deja
> vender quince impresoras cuando hay doce, no me sirve de nada y volvemos al
> cuaderno.»*

🔎 **Es un rechazo, no una advertencia.** La operación no se completa.

⚙️ Dentro de `trg_prodfact_insert`, que redacta el mensaje con los dos números:

```sql
SET @v_msg_err = CONCAT(N'Stock insuficiente para producto ', @v_codigo_err,
    N'. Stock disponible: ', @v_stock_err, N', cantidad solicitada: ', @v_cantidad_err);
THROW 50001, @v_msg_err, 1;
```

→ **RN-10**. Y lo que la API responde, **medido el 4 de octubre de 2026**:

```
POST /api/factura  {"productos":[{"codigo":"PR005","cantidad":50}]}
  →  500  ·  "Stock insuficiente para producto PR005.
              Stock disponible: 14, cantidad solicitada: 50"
```

> **Ese 500 sorprende, y es deliberado.** Un «no hay stock» parece un 400 — es
> una regla de negocio, no una falla del servidor. Pero aquí la regla **la
> defiende la base de datos**, no la API: el `catch` final recibe una `SqlException` que
> nadie tradujo, y responde 500 **pasando el mensaje del disparador tal cual**.
>
> Está declarado en [`POLITICA_DE_ERRORES.md`](../POLITICA_DE_ERRORES.md) §7 y en
> el `6_contracts.md` de la v2, y es el mismo criterio por el que una llave
> repetida da 500 en la v1: **lo que la base de datos rechaza y nadie tradujo, es un
> 500** — y verlo sin traducir es lo que enseña que alguien tiene que traducirlo.

### Y la prueba que de verdad demuestra la transacción

Es el criterio que más se olvida, y está **medido**: tres renglones donde el
**segundo** no alcanza.

```
POST /api/factura  productos: [ PR003 ×1 ,  PR005 ×99 ,  PR004 ×1 ]
  →  500  ·  "Stock insuficiente para producto PR005…"

GET  /api/factura        →  total: 6     (sembradas: 6)
GET  /api/producto/PR003 →  stock: 42    (sembrado:  42)
```

> **El primer renglón NO se quedó, y el stock de PR003 no se movió.** Eso es la
> atomicidad, vista en vez de creída: la base de datos alcanzó a procesar el primer
> renglón, falló en el segundo, y **deshizo el primero**.
>
> **Si PR003 hubiera quedado en 41, la transacción no existiría** aunque el
> procedimiento dijera `BEGIN TRANSACTION`. Con PR005 —que tiene 14— se provoca
> en diez segundos.

### 3.6 · ¿Hay que deshacer una factura ya emitida?

> 💬 *«Sí, pasa. Se equivocó el vendedor, o el cliente devolvió todo. Pero la
> factura no se borra: se anula. El número queda, y queda anulado. Si yo borro el
> número 143, el contador me pregunta dónde está el 143 y yo no tengo qué
> responder.»*

🔎 **Borrado lógico, y por una razón contable, no técnica.** La serie no puede
tener huecos.

⚙️ `estado NVARCHAR(10) DEFAULT N'activa'`, y `sp_anular_factura`, que cambia el
estado **y devuelve el stock** (→ **RN-05**, **RN-12**, **RN-14**).

> **«El contador me pregunta dónde está el 143»** es el mejor argumento posible
> para un borrado lógico, y es mejor que cualquier explicación técnica: no se
> trata de preservar datos por si acaso, se trata de que **la ausencia de un
> número es una pregunta que alguien va a hacer**.

> **Y una consecuencia que el usuario no dijo y el sistema sí tomó:** una factura
> anulada **no se puede anular otra vez** (→ **RN-15**). El procedimiento mira el
> estado antes y responde `THROW 50010`. Si no lo hiciera, el stock se devolvería
> dos veces y la bodega tendría impresoras que no existen.

---

## Ronda 4 — Las reglas y los ejemplos

### 4.3 · ¿Se puede corregir una factura?

> 💬 *«No. Se anula y se hace otra. Corregir una factura emitida es lo que hacía
> el del cuaderno, con corrector, y por eso nadie le creía al cuaderno.»*

🔎 **No hay `UPDATE` de factura.** El flujo es anular y emitir de nuevo.

⚙️ Y esto es comprobable, y es la decisión más visible del sistema:

| | |
|---|---|
| En la base de datos | `sp_actualizar_factura_…` y `sp_borrar_…` **existen** |
| En la API | **no se exponen** |
| En el controlador | `PUT`, `PATCH` y `DELETE` están **escritos y apagados**, con su razón al lado |

> **Los tres verbos están escritos a propósito y comentados a propósito**, y es
> material de clase: un estudiante tiene que ver **cómo se programa cada verbo**,
> incluso el que el negocio no quiere. Borrarlos enseñaría solo a obedecer un
> spec; dejarlos apagados enseña a obedecerlo **y** a programar lo que no pide.
> Ver [`REQUISITOS_FUNCIONALES.md`](../REQUISITOS_FUNCIONALES.md) §6.

### 4.4 · Si un producto sube de precio, ¿qué pasa con las facturas viejas?

**Ésta es la pregunta que más decisiones de diseño produce por palabra.**

> 💬 *«Nada. La factura de marzo dice lo que costó en marzo. Si yo subo el precio
> hoy y la factura de marzo cambia, eso es… eso no puede pasar. Me cambiarían las
> ventas del año pasado.»*

🔎 El precio del renglón está **congelado**. No es un dato derivado del producto:
es un dato **histórico**.

⚙️ `productosporfactura.subtotal DECIMAL(18,2)` — **se guarda**, aunque se
podría calcular (→ **RN-07**).

> **Y por eso se rompe aquí la regla general de «no guardes datos derivados».**
> Normalmente un dato que se puede calcular no se guarda. Aquí sí, porque
> `cantidad × valorunitario` usaría el precio de **hoy**, y recalcularlo
> **reescribiría las ventas del año pasado**.
>
> **La regla correcta no es «nunca guardes derivados»: es «guárdalos cuando
> congelarlos sea el requisito»** — y *«me cambiarían las ventas del año
> pasado»* es ese requisito, dicho por quien lo sufre.

### 4.5 · ¿El total lo calcula alguien?

> 💬 *«Sale solo. Y no quiero que un vendedor pueda escribirlo.»*

🔎 El total **no viaja en la petición**; si llega, se ignora.

⚙️ Lo recalcula el disparador (→ **RN-08**, **RN-09**).

> **«No quiero que un vendedor pueda escribirlo» es un requisito de seguridad
> dicho en lenguaje de negocio.** Si el total llegara en el cuerpo, cualquiera
> podría mandar una factura de tres millones con total `1000`.
>
> **Y la regla general que de ahí sale, que vale para cualquier sistema:** un
> dato que el servidor puede calcular, lo calcula el servidor. Del cliente se
> acepta lo que el cliente **sabe** —qué productos y cuántos—, nunca lo que se
> **deduce** de eso.

### 4.6 · ¿Hay descuentos, impuestos, devoluciones parciales?

> 💬 *«Descuentos sí, pero los hago a mano bajando el precio. Impuestos los
> maneja el contador aparte. Devoluciones parciales… han pasado dos veces en tres
> años, y las resolvimos anulando y haciendo otra.»*

🔎 **Nada de eso entra**, y el documento lo declara en vez de callarlo.

⚙️ [`DISENO_BD.md`](../DISENO_BD.md) §5, «lo que este modelo NO resuelve»:
impuestos, descuentos, devoluciones parciales, entradas de inventario, histórico
de precios.

> **Y ésta es la respuesta más valiosa de la ronda, aunque parezca la más
> aburrida.** *«Han pasado dos veces en tres años»* es exactamente el dato que
> permite decidir **no construirlo**. Sin ese número, alguien habría modelado
> devoluciones parciales «por si acaso» — y eso es YAGNI al revés.
>
> **Una pregunta que trae una frecuencia vale más que diez que traen un sí.**

### 4.8 · Déme un ejemplo de verdad

> 💬 *«Ayer, Ana Torres: dos teclados Logitech a 150 000 y un mouse HP a
> 90 000. Le vendió Carlos. Total 390 000. Se la mandé por WhatsApp en foto.»*

> **Las cifras de esa cita salen del catálogo real**, no de la imaginación:
> `PR003 Teclado Logitech K380` vale **150 000** y `PR004 Mouse HP` vale
> **90 000** — consultados a la API. 2 × 150 000 + 90 000 = **390 000**.
>
> **Y eso es a propósito.** Una cita inventada con precios inventados se
> contradice con los datos sembrados en cuanto alguien compare, y entonces el
> documento enseña a desconfiar de sí mismo. Si la ficción tiene que convivir con
> datos reales, los números se toman de los datos reales.

🔎 Un caso concreto que trae **cuatro** cosas que ninguna pregunta abstracta
habría dado:

| Lo que trae | Y lo que decide |
|---|---|
| Dos renglones, uno con cantidad 2 | El subtotal es `cantidad × precio`, no el precio |
| «Le vendió Carlos» | `fkidvendedor` — **atribución**, no auditoría |
| Pesos sin centavos, en cientos de miles | `DECIMAL(18,2)`, y **nunca `FLOAT`** |
| «Se la mandé en foto» | **No hay PDF, ni correo, ni impresión.** Y el sistema tampoco los tiene |

> **La última fila es la más honesta del documento.** La salida real del proceso
> de don Hernán es una foto por WhatsApp, y el sistema **no la sustituye**: no
> genera un PDF ni manda un correo. Está en «lo que el sistema no promete».
>
> **Un ejemplo de verdad no solo confirma requisitos: destapa los que no se
> cubrieron.**

---

## 5. Los límites de estas respuestas

| Límite | Por qué importa |
|---|---|
| **Nadie dijo nada de esto** | No es evidencia. Es un puente didáctico |
| **Se leen demasiado ordenadas** | Una elicitación real falla, insiste y vuelve sobre lo mismo |
| **No hay contradicciones** | Y una fuente real **se contradice**: dice una cosa en el minuto 12 y otra en el 58 |
| **No hay nada que el usuario no supiera** | En una real, el usuario reconoce no saber — y eso también es un dato |

> **Si su elicitación se lee así de limpia, sospeche de ella.** Las respuestas de
> verdad traen tramos confusos, frases a medias y cosas que el usuario dice sin
> darse cuenta de que son requisitos. La transcripción de `proyecto_catedras2`
> tiene un tramo entero recuperado y un rol **mal nombrado por la propia
> fuente** — ese tipo de ruido es la señal de que la fuente es real.

---

| Qué | Dónde |
|---|---|
| Las preguntas | [`1_PREGUNTAS.md`](1_PREGUNTAS.md) |
| Lo que esto pide y no está construido | [`3_HISTORIAS_PROPUESTAS.md`](3_HISTORIAS_PROPUESTAS.md) |
| Las 22 reglas, con quién las defiende | [`REGLAS_DE_NEGOCIO.md`](../REGLAS_DE_NEGOCIO.md) |
| El modelo que todo esto produjo | [`DISENO_BD.md`](../DISENO_BD.md) |
