# Plan de la versión 4 — Facturación (`bdfacturas`)

> **Qué es este documento.** Qué agrega la v4, qué se decidió antes de
> programarla, **los cinco tropiezos que tiene preparados** —uno de ellos
> entrega un número equivocado que parece bien— y **qué falta todavía**, que en
> esta versión es parte del relato.
>
> El requisito que manda está en
> [`2_spec.md`](../spec_kit/versiones/v4_aplicativo/2_spec.md). Esto es el
> relato.
>
> **Material académico simulado** en el dominio; las consultas, el tablero y los
> tropiezos son reales — y los tres pendientes del final también.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Lo que queda al terminar la v4

| Queda | Comprobable con |
|---|---|
| **Diez consultas**, cada una de **4 tablas o más** | `/api/consultas/ventas-por-vendedor` y sus nueve hermanas |
| Un **sobre distinto**: `{consulta, total, datos}` | Sin `limite` — y el §2 dice por qué |
| El **tablero** con las diez, pedidas **a la vez** | Pantalla «Tablero» · `Task.WhenAll` de diez tareas |
| Si **una** consulta falla, **las otras nueve se dibujan** | Se apaga la API a mitad de carga |
| **Cero filas es una respuesta**, no un error | Las consultas 6 y 9 pueden venir vacías, y lo dicen con palabras |
| El tablero **reacciona** al sistema | Anular una factura mueve la consulta 1 y llena la 7 |
| La **imagen corporativa**: `marca.css` sale del manual | Los cinco colores del [manual](MANUAL_DE_MARCA.md) son los del CSS |
| **10 operaciones nuevas** — y las 70 anteriores intactas | Swagger: 80 en total |

> **Y tres cosas que NO están, dichas aquí y no escondidas al final:** las
> páginas corporativas, el manifiesto de PWA y la publicación en un servidor.
> Están en el §7.

---

## 1. De dónde se partió

De un sistema completo y con puerta: doce tablas operables, token, permisos
por ruta, menú por rol. **Y que no responde ninguna pregunta del negocio.**

«¿Cuánto vendió cada vendedor?» no es una fila de ninguna tabla. Es un cruce —
y hasta la v3 el sistema sabía guardar y sabía proteger, pero no sabía
**mirar**.

> **La v4 no agrega ni una tabla.** Agrega **las preguntas** y el sitio donde
> se ven. Por eso es la versión que más se parece a un producto y la que menos
> se parece a las anteriores: no hay un CRUD nuevo en toda la versión.

---

## 2. La decisión que define la versión: **el cruce se hace donde están los datos**

Se podría pedir `/api/factura` y `/api/producto` por separado y juntarlos en el
navegador. Daría el mismo número **a veces**, y falla por tres lados:

| | |
|---|---|
| **Trae todo a la máquina de quien mira** | Para sumar diez facturas hay que bajar las diez con sus renglones |
| **Suma mal en cuanto haya paginación** | El `limite` del contrato corta la lista y el total sale corto **sin avisar** |
| **Pide muchas veces lo que se responde una** | Una factura, un viaje: es el problema **N+1**, con nombre propio |

> **El segundo es el que decide.** No es que sea lento: es que **da un número
> equivocado y no se queja**. Una suma hecha sobre una lista paginada es una
> suma de la primera página — y se ve perfectamente bien.

**Y de ahí sale el sobre nuevo.** Las consultas responden
`{consulta, total, datos}` y **no llevan `limite`**: una consulta no se pagina,
porque paginarla sería volver a romper el total. El CRUD sigue con su
`{tabla, limite, total, datos}` — son dos contratos distintos **a propósito**.

### Una ruta por consulta, con nombre propio

No hay un `/api/consultas?tipo=ventas-por-vendedor`. Hay diez rutas:

```
/api/consultas/ventas-por-producto        /api/consultas/productos-sin-vender
/api/consultas/ventas-por-cliente         /api/consultas/anulaciones-por-cliente
/api/consultas/ventas-por-vendedor        /api/consultas/alcance-de-usuarios
/api/consultas/ventas-por-empresa         /api/consultas/interfaces-sin-usuarios
/api/consultas/ticket-por-vendedor        /api/consultas/credito-contra-consumo
```

> **Porque una ruta con nombre se puede documentar, probar y permisar; un
> parámetro mágico, no.** En Swagger se leen las diez preguntas del negocio sin
> abrir el código, y cada una se puede romper por su cuenta sin tumbar a las
> otras nueve.

---

## 3. La segunda decisión: **el tablero dibuja con CSS, no con una librería**

Las barras del tablero son `div`s con un `width` en porcentaje. No hay
Chart.js, ni D3, ni un paquete de gráficos.

| | |
|---|---|
| **Qué se gana** | Una dependencia menos, nada que actualizar, y **se entiende**: quien lo lea ve una regla de tres y un `style="width:…"` |
| **Qué se pierde** | Gráficos de torta, animaciones, tooltips de la librería |
| **Cuándo cambiar** | El día que el gráfico que se necesita no se pueda dibujar con una barra. El sitio es `wwwroot/lib/`, y el tablero no tendría que cambiar de forma |

> **No es una postura contra las librerías.** Es que para diez barras
> horizontales la librería es más código del que ahorra — y en un curso donde
> hay que **sustentar** lo que se entregó, una barra que uno mismo calculó se
> explica; una que vino configurada, no siempre.

### Y las diez se piden a la vez

```csharp
await Task.WhenAll(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
```

> **En fila serían diez viajes esperándose uno a otro.** A la vez es **un
> viaje de ida y vuelta** medido por el más lento de los diez. Es el criterio 5
> de la versión, y es la única parte del proyecto donde la programación
> asincrónica se ve en el reloj y no en la teoría.

---

## 4. El orden en que se construyó

| # | Paso | Por qué va aquí |
|---|---|---|
| **1** | El **SQL** de las diez, en el repositorio | Primero que el número sea correcto. Lo demás es presentación |
| **2** | Contar los `JOIN` de cada una | El criterio exige **4 tablas o más**: se cuenta antes de seguir, no al final |
| **3** | El servicio y el controlador, con su `[ExigePermiso]` | Las consultas no son un anexo público: exigen token y permiso como todo |
| **4** | El sobre `{consulta, total, datos}` | Antes del front, porque el front lee **este** sobre y no el del CRUD |
| **5** | El tablero: las diez con `Task.WhenAll` | Y cada una con su propio aviso de error (§5) |
| **6** | `marca.css` sobre Bootstrap | **Lo último que se carga**, para poder ajustar lo que Bootstrap ya puso |

---

## 5. Los cinco tropiezos que esta versión tiene preparados

| | Qué pasa | Cómo se nota |
|---|---|---|
| **1 · El tipo que el compilador no ve** | `COUNT()` devuelve `bigint` en PostgreSQL e `int` en SQL Server. Un modelo con `int` **revienta al deserializar** en uno de los dos motores | Se cae, y se arregla. **Este es el bueno** |
| **2 · El promedio truncado** | `SUM(decimal) / COUNT(*)` con enteros **trunca** y no se queja | Un ticket promedio de 1 250 000 que sale 1 250 000**0** menos. **Este es el caro**: entrega un número equivocado que parece bien |
| **3 · Las diez en fila** | Se piden una tras otra. Funciona | El tablero tarda diez veces lo necesario, y nadie lo llama error |
| **4 · Cero filas tratado como falla** | `productos-sin-vender` puede venir vacía — y eso **es** la respuesta | Un «error al cargar» donde debía decir «todos los productos se han vendido» |
| **5 · El tablero lámina** | Los números se calculan una vez y quedan quietos | **Criterio 8**: se anula una factura y el tablero no se mueve. Está leyendo de otro lado |

> **El 2 es el que hay que mirar dos veces.** El 1 se cae y se arregla en cinco
> minutos; el 2 pasa las pruebas, se ve bien en la pantalla, y lleva un número
> equivocado a una decisión. En la sustentación es la pregunta más justa que se
> puede hacer sobre esta versión: *¿por qué su promedio es `decimal` y no `int`?*

---

## 6. Cómo se verificó

| Prueba | Resultado |
|---|---|
| Las diez consultas con token de administrador | **200** las diez, con el sobre `{consulta, total, datos}` |
| Las diez **sin** token | **401** — la v3 sigue en pie |
| Contar los `JOIN` de cada SQL | 4 tablas o más; `alcance-de-usuarios` cruza **cinco** |
| Apagar la API con el tablero a medio cargar | Nueve barras dibujadas y **un** aviso con el nombre de la que falló |
| Anular una factura y recargar | La consulta 1 baja su ingreso y la 7 gana una fila |
| Comparar `marca.css` con el manual | Los cinco colores coinciden: `#17495B`, `#2F7D5C`, `#E39B2D`, `#B4402F`, `#4A4A4A` |
| La regresión de v1, v2 y v3 | Completa, el día de la entrega |

---

## 7. Lo que falta para cerrar la v4

Esto no es un apéndice: **la v4 está en curso**, y estas tres cosas son parte
de sus criterios.

| | Estado | Qué falta exactamente |
|---|---|---|
| **Imagen corporativa con su manual** | **Hecha** | `marca.css` está enchufada en `App.razor` y sale del [manual](MANUAL_DE_MARCA.md) |
| **Páginas corporativas** | **Pendiente** | Hay `Home`; faltan servicios, soporte y contacto |
| **Responsive / PWA** | **A medias** | La interfaz **sí** es responsive por Bootstrap. No hay manifiesto ni *service worker*: sin eso no es una PWA, es un sitio que se ve bien en el teléfono |
| **Publicación en un servidor** | **Pendiente** | Con los secretos en variables de entorno del servidor, no en el repositorio |

> **La distinción de la tercera fila es la que más se confunde.** «Responsive»
> es que la interfaz se acomode al ancho; **PWA** es que el navegador la pueda
> instalar y abrir sin red. Lo primero está; lo segundo son dos archivos que
> todavía no existen.

### Y después de la v4

La v5 **se proyecta**: otros motores de base de datos y la fábrica que elige
cuál usar. Está **fuera de las cuatro del curso** y su naturaleza es distinta —
no agrega funcionalidad, **demuestra** que la interfaz del repositorio servía.
El mapa lo explica: [`0_mapa_versiones.md`](../spec_kit/versiones/0_mapa_versiones.md).

---

| Qué | Dónde |
|---|---|
| El requisito que manda | [`v4_aplicativo/2_spec.md`](../spec_kit/versiones/v4_aplicativo/2_spec.md) |
| Las diez consultas, tabla por tabla | [`6_contracts.md`](../spec_kit/versiones/v4_aplicativo/6_contracts.md) |
| Los colores y las tipografías | [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) |
| El sobre de las consultas, campo por campo | [`6_contracts.md`](../spec_kit/versiones/v4_aplicativo/6_contracts.md) |
| Qué código responde cada capa | [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) |
| La versión anterior | [`PLAN_V3.md`](PLAN_V3.md) |
