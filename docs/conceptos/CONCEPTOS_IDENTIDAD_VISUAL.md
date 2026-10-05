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
> manual de **Comercial Los Andes S.A.** —la empresa de este proyecto— fija
> cinco colores, y esto es lo que dan sobre el fondo:
>
> | Color | Hex | Sobre el fondo | Con blanco encima |
> |---|---|---|---|
> | azul cordillera | `#17495B` | **8,76** ✅ | 9,81 ✅ |
> | verde páramo | `#2F7D5C` | **4,46** ⚠ | 4,99 ✅ |
> | rojo anulada | `#B4402F` | **5,05** ✅ | 5,65 ✅ |
> | piedra | `#4A4A4A` | **7,92** ✅ | 8,86 ✅ |
> | **ámbar cosecha** | `#E39B2D` | **2,09** ❌ | 2,34 ❌ |
>
> **El ámbar no sirve para texto, ni para llevar texto encima.** Ni para texto
> normal (pide 4,5), ni para texto grande, **ni siquiera para un borde o un
> icono** (piden 3): 2,09 no llega a ninguno de los tres umbrales.
>
> Eso **no** es un defecto de la paleta. Un ámbar cálido es perfecto como
> **acento** —el triángulo del isotipo, un realce—, que es para lo que se
> escogió. La pantalla solo le exige un número cuando se le pone texto encima o
> se usa para **informar algo**.
>
> **Y en este proyecto eso se incumplió, de verdad.** El borde del campo
> enfocado —que es un **indicador de interfaz** y pide 3— estaba en ámbar:
> **2,09**. Alguien que distinga mal los colores no veía en qué campo estaba
> escribiendo. Hay dos salidas legítimas:
>
> 1. **Llevarlo a un color que sí cumpla.** Es lo que se hizo: el foco pasó a
>    `azul cordillera`, que da **8,76**.
> 2. **Definir una variante oscura del mismo matiz**, bajando la luminosidad
>    hasta pasar el umbral. No reemplaza al color de marca: **convive** con él.
>
> **Y el verde da 4,46, que se queda a cuatro centésimas del 4,5 — y eso no se
> redondea hacia arriba.** Para texto va en negrita o en tamaño grande; para un
> botón, con blanco encima, donde sí llega a 4,99.
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
NO existe**, y por eso §5 no es teoría: es lo que de verdad se hizo.

> **«Comercial Los Andes S.A.» es una empresa ficticia**, así que no hay
> departamento de comunicaciones al que pedirle el PDF ni resolución que citar.
> El manual de [`MANUAL_DE_MARCA.md`](../dominio/MANUAL_DE_MARCA.md) es
> **derivado**: se construyó siguiendo los cinco pasos de abajo, y lo dice en su
> primera línea.
>
> **Y eso es lo normal, no la excepción.** Al equipo le va a pasar lo mismo en su
> proyecto de aula.

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

Se le pasa a quien pueda validarlo: comunicaciones, mercadeo, la gerencia, el
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
[`front_blazor/wwwroot/marca.css`](../../front_blazor/wwwroot/marca.css), y está
separado de `app.css` a propósito: **aquí van los valores que el manual fija;
allá va cómo se usan**.

```css
:root {
  --azul-cordillera: #17495B;   /* el principal: barra, títulos, botón primario */
  --verde-paramo:    #2F7D5C;   /* lo que salió bien */
  --rojo-anulada:    #B4402F;   /* lo que falló o se anuló */
  --piedra:          #4A4A4A;   /* texto corriente */
  --ambar-cosecha:   #E39B2D;   /* SOLO acento: nunca lleva texto */
  --niebla:          #F4F2EC;   /* el fondo */
}
```

Y las pantallas **usan la variable, nunca el valor**:

```css
/* bien */
.barra { background: var(--azul-cordillera); }

/* mal */
.barra { background: #17495B; }
```

Por qué importa:

| | Con el valor regado por el CSS | Con `marca.css` |
|---|---|---|
| La empresa cambia su azul | Buscar y reemplazar, y rezar | Se cambia **una línea** |
| ¿De dónde salió este color? | Nadie sabe | Del manual |
| ¿Está permitido usar este otro? | Se discute | **Si no está en `marca.css`, no** |

> **Y esto no es teoría: aquí se rompió.** Cuando se escribió el manual, `app.css`
> traía **cuatro colores de Bootstrap escritos a mano** —`#86b7fe` en el foco,
> `#0b5ed7` y `#0a58ca` en el botón primario, y `#6f42c1` en una barra del
> tablero—. Ninguno era de esta marca: eran los valores por defecto de Bootstrap,
> de cuando se copió el estilo.
>
> **Peor aún: había DOS estilos de foco distintos** —uno en `marca.css` y otro en
> `app.css`—, así que según el campo en que uno se parara veía uno u otro.
> **Tener dos es tener ninguno.**

Es la misma idea que sostiene el resto del curso: **separar lo que es una
restricción de lo que es una decisión**. El manual manda; el CSS obedece.

---

## 7. Cómo se comprueba en este proyecto

> **Aquí el manual no es un PDF aparte: son dos documentos que se leen juntos.**
> [`MANUAL_DE_MARCA.md`](../dominio/MANUAL_DE_MARCA.md) explica **qué** y **por
> qué**; [`marca.css`](../../front_blazor/wwwroot/marca.css) lo hace **exigible**,
> porque un color que no esté ahí **sencillamente no existe** para las pantallas.

**Y la ventaja de tenerlo en CSS en vez de en un PDF es que se puede CONTAR.**
«La interfaz se ve bien» no se califica; **«hay cero colores fuera de la paleta»
sí**:

```powershell
# 1 · NINGÚN color de marca escrito a mano fuera de marca.css.
#     OJO: hay DOS formas de escribir un color, y la segunda es la que se
#     escapa. Las sombras en rgba(0,0,0,…) NO cuentan: el negro de una sombra
#     no es un color de marca.
Select-String -Path front_blazor\wwwroot\app.css `
  -Pattern '#[0-9a-fA-F]{6}|rgba?\([0-9]' |
  Where-Object { $_.Line -notmatch 'rgba\(0, ?0, ?0|rgba\(255|/\*|^\s*\*' }

# 2 · TODOS los focos del mismo color: compare el valor de cada uno.
Select-String -Path front_blazor\wwwroot\*.css -Pattern ':focus' -Context 0,3

# 3 · El ámbar NO lleva texto encima. Revise a ojo dónde se usa:
Select-String -Path front_blazor\wwwroot\*.css `
  -Pattern 'ambar-cosecha|227, ?155, ?45'
```

> **El `rgba?` del comando 1 no es un adorno, y este proyecto lo aprendió a
> golpes.** Buscando solo `#rrggbb` el repositorio daba **cero colores a mano**
> — y tenía dos, escritos en el otro formato:
>
> | Dónde | Qué era |
> |---|---|
> | `app.css`, el foco del botón | `rgba(13, 110, 253, .25)` — **el azul de Bootstrap**, `#0d6efd` |
> | `marca.css`, el foco del campo | el borde en azul y **la sombra todavía en ámbar** |
>
> El segundo es el peor: una corrección anterior cambió el borde y **se olvidó
> de la sombra**, así que el foco quedó medio azul y medio ámbar. **Nadie lo
> notó, porque la comprobación no lo buscaba.**
>
> **Una comprobación que no puede fallar no está comprobando nada.**

| Qué se comprueba | Cómo |
|---|---|
| Los colores viven **solo** en `marca.css` | el comando 1 da **0** |
| Las pantallas usan variables, no valores | `app.css` usa `var(--…)`, nunca el hex |
| **Un solo** estilo de foco | el comando 2 lo encuentra una vez |
| El ámbar nunca lleva texto | 2,09 no alcanza ningún umbral — §4 |
| El verde no se usa para texto pequeño sobre el fondo | 4,46 se queda a cuatro centésimas |
| El rojo significa **una** cosa: algo falló o se deshizo | No se usa para «destacar» un dato importante |

> **La última fila es la que más se rompe.** Usar el rojo para resaltar un dato
> que no es un error le quita el significado — y el día que haya un error de
> verdad, nadie lo va a distinguir.

Lo que **no** se comprueba es el gusto. No se trata de que la pantalla sea
bonita: se trata de que **cumpla un documento**, y de que cualquiera pueda
verificar que lo cumple **sin discutir de estética**.

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
