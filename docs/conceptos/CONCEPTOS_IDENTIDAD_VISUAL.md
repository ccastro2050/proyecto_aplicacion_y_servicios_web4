# Identidad visual corporativa — qué es y por qué obliga al software

**Documento conceptual del curso**

---

## 1. Por qué esto aparece en un curso de programación

Porque la pantalla que usted construye **representa a una institución**, y
esa institución ya decidió cómo se ve. No es una decisión de quien programa.

El caso de este proyecto de aula es el normal en cualquier organización:
existe un **manual de identidad visual** —aquí,
un manual de marca adoptado por una resolución,
y el sistema tiene que cumplirlo igual que cumple cualquier otro requisito.

**Un color corporativo cambiado es un defecto**, no un detalle estético.

---

## 2. Los términos, que se confunden todo el tiempo

| Término | Qué es | En este proyecto |
|---|---|---|
| **Isotipo** | La parte **gráfica**, sin texto | El monte |
| **Logotipo** | La parte **escrita**: el nombre en su tipografía | «UNIVERSIDAD MONTE VERDE» |
| **Logosímbolo** | Los dos juntos | Lo que se usa normalmente |
| **Imagotipo** | Los dos juntos pero **separables** | — |
| **Isologo** | Los dos **fundidos**, inseparables | — |
| **Identidad visual** | El sistema completo: marca, colores, tipografías, reglas | El manual entero |
| **Imagen de marca** | Lo que la gente **percibe**. No se diseña: se gana | — |

La distinción entre **identidad** (lo que la organización emite) e **imagen**
(lo que el público percibe) viene de la literatura de identidad corporativa
y es la que explica por qué un manual no garantiza nada por sí solo:
ordena lo que se emite.

---

## 3. Qué lleva un manual de identidad visual

No hay una norma ISO que lo fije, pero los manuales reales coinciden en el
mismo conjunto, y el de este proyecto los tiene todos:

| Sección | Qué resuelve | Si falta |
|---|---|---|
| **Versiones del logosímbolo** | Horizontal, vertical, isotipo solo | Cada quien recorta el logo a su gusto |
| **Paleta** | Los valores exactos, en hex y RGB | «El verde de la universidad» pasa a ser cinco verdes |
| **Tipografía** | Familias autorizadas | Cada pantalla con una fuente distinta |
| **Tamaño mínimo** | Debajo de cuánto deja de leerse | Logos ilegibles en el pie de página |
| **Área de reserva** | Cuánto espacio libre alrededor | El logo pegado al borde o a otro logo |
| **Usos incorrectos** | Lo que está prohibido, con ejemplos | Discusiones sin final |
| **Escala de grises** | Qué hacer a una tinta | Conversiones automáticas que borran la marca |

El **área de reserva** y el **tamaño mínimo** son los dos que más se ignoran
y los que más delatan a un sistema hecho sin mirar el manual.

---

## 4. La accesibilidad no es opcional: el contraste se calcula

Un manual puede fijar colores bonitos que **no se pueden leer**. Por eso, en
software, la paleta se comprueba contra un criterio verificable.

Las **WCAG 2.2** —recomendación del W3C, versión vigente del 12 de diciembre
de 2024— fijan en su criterio **1.4.3 (Contraste mínimo, nivel AA)**:

| Qué | Relación mínima |
|---|---|
| Texto normal | **4.5:1** |
| Texto grande | **3:1** |
| Elementos que no son texto: bordes, iconos, controles (criterio 1.4.11) | **3:1** |

**«Texto grande» lo define la norma en puntos**, no en píxeles: desde
**18 pt**, o **14 pt en negrita**. En pantalla eso equivale a unos 24 px y
18,66 px respectivamente.

La relación se calcula con la **luminancia relativa** de los dos colores:

```
razón = (L_claro + 0.05) / (L_oscuro + 0.05)
```

donde `L` se obtiene linealizando cada canal y pesándolos
`0.2126·R + 0.7152·G + 0.0722·B`. La fórmula completa está en las WCAG y
**cualquiera puede rehacer la cuenta** — por eso es un requisito verificable
y no una opinión.

> **Haga la cuenta con los colores de este proyecto y verá que incomoda.** El
> manual de la Universidad de San Buenaventura fija dos colores
> institucionales, y esto es lo que dan:
>
> | Color | Hex | Sobre blanco | Sobre el negro institucional |
> |---|---|---|---|
> | Naranja institucional | `#EF7D00` | **2,76:1** | 7,60:1 |
> | Negro institucional | `#1D1D1B` | 16,88:1 | — |
>
> **El naranja institucional no sirve para texto sobre blanco.** Ni para texto
> normal (pide 4,5:1), ni para texto grande, ni siquiera para un borde o un
> icono (piden 3:1): 2,76 no llega a ninguno de los tres umbrales.
>
> Eso **no** es un defecto del manual. Un manual de identidad se hace pensando
> en papel, vallas y pendones, donde el naranja sobre blanco se ve
> perfectamente. La pantalla es otro medio, con otra norma, y el manual no
> tiene por qué haberla previsto.
>
> Lo que sí es responsabilidad de quien programa es **no llevarse el color al
> texto sin hacer la cuenta**. Hay dos salidas legítimas, y este proyecto usa
> la primera:
>
> 1. **Ponerlo sobre el negro institucional**, donde da 7,60:1 y sobra. Es lo
>    que hace la barra superior de este front: fondo `--usb-negro`, y el
>    nombre de la dependencia en `--usb-naranja`.
> 2. **Definir una variante oscura solo para texto**, del mismo matiz y la
>    misma saturación, bajando la luminosidad hasta pasar el umbral:
>    `#B45E00` da **4,62:1** sobre blanco. No reemplaza al institucional —el
>    manual prohíbe cambiarlo—: **convive** con él, igual que el manual ya
>    prevé una escala de grises para cuando no hay color.
>
> Un manual que dice «nuestros colores son accesibles» sin el número no dice
> nada. Y una aplicación que usa el color institucional para texto sin
> comprobarlo tampoco.

Y una regla que se deduce de lo anterior: **el color nunca es el único
portador de la información** (criterio 1.4.1). Un campo con error se marca en
rojo **y** con un mensaje; si solo cambia de color, quien no distingue ese
color no se entera.

---

## 5. Cuando **no hay** manual — que es el caso normal

Hasta aquí, este documento ha supuesto que existe un manual. **En este proyecto
existe** —la Resolución de Rectoría General n.º 404— y eso es suerte, no lo
corriente.

> **Lo habitual es que la organización tenga una identidad visual y ningún
> documento que la describa.** Hay un logo en el sitio web, unos colores que
> «siempre se han usado», una tipografía que alguien eligió hace años, y nadie
> que pueda decir cuáles son exactamente.

**Y no tener manual no autoriza a inventar.** La identidad existe: está
repartida en piezas. Lo que hay que hacer es **reconstruirla y declarar que es
una reconstrucción**.

### Los cinco pasos

#### 1 · Reunir las fuentes, y anotarlas

| Fuente | Qué da | Cuánto vale |
|---|---|---|
| **El sitio web oficial** | Colores exactos del CSS, tipografías, el logo en su mejor versión | **Alta** — es lo que la organización publica hoy |
| **El archivo vectorial del logo** (`.svg`, `.ai`, `.eps`) | Proporciones, colores exactos, área de reserva | **La más alta**, si existe |
| **Papelería y plantillas** | Cómo se usa en impreso | Media |
| **Redes sociales** | El uso cotidiano | **Baja** — casi siempre las maneja alguien distinto y se desvía |
| **Señalización y fachadas** | El color físico | Baja para pantalla; el papel y el muro no son el monitor |

**Cada fuente se anota con su URL y su fecha de consulta.** Un manual derivado
sin sus fuentes no se puede comprobar ni actualizar.

#### 2 · **Medir, no estimar**

Esta es la parte que decide si el resultado sirve.

| Qué | Cómo se mide | Lo que NO vale |
|---|---|---|
| **Los colores** | Cuentagotas sobre el **vectorial**, o el valor calculado en el inspector del navegador | Sacarlo de una captura de pantalla: el JPEG **cambia los colores** |
| **Las tipografías** | El `font-family` **computado** en el inspector, no el declarado | Adivinar por parecido |
| **Las proporciones** | Sobre el archivo vectorial | Medir en píxeles sobre una imagen escalada |
| **El área de reserva** | La distancia mínima que respetan las piezas oficiales | Inventar un margen «que se vea bien» |

> **El error más común, y arruina el resultado:** tomar el color de un
> **pantallazo**. Un JPEG es compresión con pérdida, y un naranja institucional
> sale con tres valores distintos en tres capturas del mismo sitio. **Se toma
> del CSS o del vectorial, o no se toma.**

#### 3 · Contrastar entre fuentes, y **anotar la discrepancia**

Casi nunca coinciden. **Y eso no es un problema que resolver en silencio: es un
hallazgo que reportar.**

```
El azul del sitio web        #1B3A6B
El azul de la plantilla      #1C3C70
El azul de la señalización   #17356A
```

Lo que se hace: **elegir uno, decir cuál y por qué** —normalmente el del sitio
web, que es lo más actual y lo que más gente ve— **y dejar los otros escritos**.
Quien venga después necesita saber que había tres.

#### 4 · Escribirlo como **manual derivado**, con la advertencia arriba

El documento empieza así, y no en letra pequeña:

> **Esto no es el manual oficial de la organización.** Es lo que se pudo
> reconstruir de las fuentes que se listan abajo, en la fecha que se indica.
> Donde hubo discrepancias, se eligió un valor y se dejó constancia. **Si
> aparece un manual oficial, manda el manual.**

#### 5 · Someterlo a aprobación — y **si nadie aprueba, decirlo**

Se le pasa a quien pueda validarlo: comunicaciones, mercadeo, rectoría, el
cliente. Y entonces pasa una de dos:

- **Lo aprueban** → deja de ser derivado y pasa a ser el manual.
- **Nadie responde** → **se deja escrito que nadie respondió**, con la fecha en
  que se pidió. Sigue siendo lo mejor que hay, y todo el mundo sabe qué es.

> **Lo que no vale es la tercera opción:** que el documento no diga nada, y en
> seis meses alguien lo cite como si fuera oficial.

### Lo que un manual derivado **no puede** inventarse

Hay cosas que solo la organización decide, y si no están, **se dice que no
están** en vez de rellenarlas:

- **Usos prohibidos.** Que nadie haya deformado el logo no significa que esté
  permitido.
- **El tamaño mínimo.** Se puede proponer uno **medido** —el más pequeño que
  aparece en piezas oficiales— diciendo que es una observación, no una norma.
- **Las versiones autorizadas** en negativo, en escala de grises, monocromo.
- **La denominación de dependencias.**

### Y lo que sí se puede añadir, aunque el manual oficial no lo traiga

**El contraste.** Un manual de 2010 no habla de WCAG porque no existía la
obligación. **Calcularlo y dejarlo escrito es aportar, no contradecir** — y si
al medirlo resulta que una combinación institucional no llega al 4,5:1, eso
**se reporta**: no se corrige el color de marca por cuenta propia, se cambia
**el uso**.

> **Es el mismo hallazgo que en este proyecto**, donde sí hay manual: el
> manual fija los colores, pero **no dice sobre cuál fondo va cada uno**. Esa
> decisión quedó del lado de quien programa, y por eso se calculó.

---

## 6. Cómo se lleva un manual al código

**La regla es una sola: los valores del manual van en un archivo aparte.**

En este proyecto ese archivo es
[`front_flask/static/marca.css`](../../front_blazor/wwwroot/marca.css), y está
separado de `estilos.css` a propósito: **aquí van los valores que el manual
fija y que nadie puede cambiar; allá va cómo se usan**.

```css
:root {
  --usb-naranja:  #EF7D00;   /* C:0 M:60 Y:100 K:0 · R:239 G:125 B:0 */
  --usb-negro:    #1D1D1B;   /* C:0 M:0  Y:0   K:100 · R:29 G:29 B:27 */
}
```

Esos dos valores no son una preferencia de diseño. El manual lo dice con
todas las letras: *«Por ningún motivo se deben cambiar los colores
corporativos.»* Quien los cambie está fuera de norma, y no es una norma del
curso: es una **Resolución de Rectoría**.

Y los estilos de la aplicación **usan la variable, nunca el valor**:

```css
/* bien */
.barra { background: var(--usb-negro); }

/* mal */
.barra { background: #1D1D1B; }
```

Compruébelo: `estilos.css` usa `var(--…)` treinta y cinco veces y **no repite
ni una vez** un color institucional escrito a mano.

Por qué importa:

| | Con el valor regado por todo el CSS | Con `marca.css` |
|---|---|---|
| La universidad cambia su verde | Buscar y reemplazar, y rezar | Se cambia una línea |
| ¿De dónde salió este color? | Nadie sabe | Del manual, punto 2 |
| ¿Está permitido usar este otro? | Se discute | Si no está en `marca.css`, no |

Es la misma idea que sostiene el resto del curso: **separar lo que es una
restricción de lo que es una decisión**. El manual manda; el CSS de la
aplicación obedece.

---

## 7. Cómo se comprueba en este proyecto

Aquí la marca **no es un ejercicio**: es la de la Universidad de San
Buenaventura, y el documento que la fija está en el repositorio —
un PDF oficial y su transcripción a markdown para poder citarla y buscarla.

> **En este repositorio el manual no es un documento aparte: es
> [`marca.css`](../../front_blazor/wwwroot/marca.css).** Los colores y las
> tipografías viven ahí como variables, y las pantallas usan la variable, nunca
> el valor. Es la misma idea —el manual manda sobre el gusto de quien
> programa— con una ventaja: si una pantalla se sale del manual, no hay que
> revisarla a ojo contra un PDF; el color simplemente no existe.

Seis comprobaciones, todas verificables mirando archivos:

| Qué se comprueba | Dónde se mira |
|---|---|
| Los colores institucionales están **solo** en `marca.css` | `grep -c "#EF7D00" front_flask/static/estilos.css` debe dar **0** |
| La aplicación usa variables, no valores | `estilos.css` usa `var(--…)`, nunca el hex |
| Ningún color institucional aparece **modificado** | Ningún `#EF7D00` alterado ni `opacity` sobre el logo |
| El logosímbolo respeta **tamaño mínimo** y **área de reserva** | El manual, punto 4; el margen está declarado en `marca.css` |
| El naranja no lleva texto sobre fondo claro | La barra lo usa sobre `--usb-negro`: 7,60:1 |
| Los estados —correcto, error— no usan los colores institucionales | Los tonos de aviso de `estilos.css` son propios, no de la marca |

Lo que **no** se comprueba es el gusto. No se trata de que la pantalla sea
bonita: se trata de que **cumpla un documento que alguien firmó**, y de que
cualquiera pueda verificar que lo cumple sin discutir de estética.

---

## 8. Referencias

### Científicas

**Las normas, que aquí son la fuente autoritativa**

1. **W3C** — ***Web Content Accessibility Guidelines (WCAG) 2.2***,
   Recomendación del **12 de diciembre de 2024**. De ahí salen el **4,5:1**
   para texto normal y el **3:1** para texto grande y elementos no textuales,
   y la fórmula de luminancia relativa de la sección 4.
2. **ISO/IEC 25010:2023** — *SQuaRE, Product quality model*. **Citar el año**:
   en la revisión de 2023 la *usabilidad* pasó a llamarse **capacidad de
   interacción**, y la accesibilidad es una de sus subcaracterísticas. Casi
   todo lo que hay en internet describe la versión de 2011.

**La medición a gran escala, y por qué esto no es un detalle**

3. **WebAIM** — *The WebAIM Million*, informe **2026** sobre el millón de
   páginas de inicio más visitadas: el **texto de bajo contraste aparece en el
   83,9 %** de ellas —frente al 79,1 % en 2025—, con **34 instancias distintas
   por página en promedio**. Es **el fallo de accesibilidad más común de la
   web**, y va empeorando.
   → Por eso en este proyecto el contraste **se calcula** en vez de mirarse:
   cuatro de cada cinco sitios profesionales lo tienen mal y nadie se da
   cuenta a ojo.

**Actualizadas (2025-2026)**

4. «Perceptually-Minimal Color Optimization for Web Accessibility: A
   Multi-Phase Constrained Approach». arXiv **`2512.05067`** — cómo corregir
   el contraste **moviendo el color lo menos posible**, que es justo el
   problema cuando el color es de marca y no se puede cambiar.
5. «Context-Adaptive Color Optimization for Web Accessibility».
   arXiv **`2512.07623`**.
6. «A feasibility study on filtering low-accessibility web pages considering
   color vision deficiency». arXiv **`2606.22095`** — **el límite de las
   herramientas automáticas**: pasar el 4,5:1 no garantiza que lo vea alguien
   con deficiencia de visión del color.

> **Comprobadas en línea el 14 de septiembre de 2026.** Las que llevan DOI se pueden abrir por el DOI; las de arXiv, por su identificador.

### De gurús, blogs y fuentes primarias


1. **WCAG 2.2** — *Web Content Accessibility Guidelines 2.2*, recomendación
   del W3C; versión vigente del 12 de diciembre de 2024. Criterios 1.4.1 (uso del color),
   1.4.3 (contraste mínimo) y 1.4.11 (contraste de elementos no textuales) —
   `https://www.w3.org/TR/WCAG22/`
2. **Cómo se calcula la razón de contraste** — definición de luminancia
   relativa y de la fórmula, en el propio glosario de las WCAG —
   `https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum`
3. **Wheeler, Alina y Meyerson, Rob.** *Designing Brand Identity: A
   Comprehensive Guide to the World of Brands and Branding.* 6.ª ed., Wiley,
   2024. ISBN 978-1-119-98481-8. — De aquí sale la estructura estándar de un
   manual y la distinción entre identidad e imagen —
   `https://www.wiley.com/en-us/9781119984825`
4. **Airey, David.** *Logo Design Love: A Guide to Creating Iconic Brand
   Marks.* 3.ª ed., New Riders, 2026. ISBN 978-0-13-547675-8. — Versiones del
   logo, tamaño mínimo y usos incorrectos.
5. **Google Fonts** — de donde se obtienen Merriweather, Inter y Lato, las
   familias secundarias autorizadas por el manual de este proyecto —
   `https://fonts.google.com/`
6. **ISO 9241-112:2017**, *Ergonomics of human-system interaction — Part
   112: Principles for the presentation of information.* — Norma vigente
   sobre presentación de la información, incluida la codificación por color
   — `https://www.iso.org/standard/64840.html`
7. En este repositorio: [`marca.css`](../../front_blazor/wwwroot/marca.css) y el
   archivo `marca.css` del front.

> Las normas ISO son de pago, pero su alcance y su índice se consultan gratis
> en el enlace. Las WCAG son públicas y gratuitas, y están traducidas.
