# Manual de marca — Comercial Los Andes S.A.

> **Qué es este documento.** La identidad visual del sistema: el logosímbolo, la
> paleta con sus valores exactos, las tipografías y **dónde se puede usar cada
> color**. Es una restricción, no una sugerencia.
>
> **El concepto detrás:**
> [`CONCEPTOS_IDENTIDAD_VISUAL.md`](../conceptos/CONCEPTOS_IDENTIDAD_VISUAL.md).
>
> **Dónde vive de verdad.** En
> [`front_blazor/wwwroot/marca.css`](../../front_blazor/wwwroot/marca.css), como
> variables. Las pantallas usan la variable, **nunca el valor** — así, un color
> que no esté en el manual sencillamente no existe en la interfaz.
>
> **Material académico simulado:** la empresa es ficticia. Los contrastes, en
> cambio, están **calculados**, no estimados.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. El logosímbolo y sus tres piezas

```
Los Andes ▲ comercial s.a.
└───┬───┘ │  └─────┬─────┘
 logotipo │     descriptor
       isotipo
```

| Pieza | Qué es | Cómo se compone |
|---|---|---|
| **Logotipo** | La palabra | `Los Andes` en Georgia, **versalitas**, interletraje +2px |
| **Isotipo** | El símbolo | El triángulo `▲` en ámbar — la cordillera |
| **Descriptor** | Qué es la empresa | `comercial s.a.` en minúscula, tamaño menor |

> **Los tres van juntos y en ese orden.** El isotipo solo se usa suelto cuando no
> cabe el conjunto —un favicon, una pestaña—, nunca por gusto.

---

## 2. La paleta, con sus contrastes calculados

| Nombre | Hex | Para qué | Sobre `niebla` | Con blanco encima |
|---|---|---|---|---|
| **azul cordillera** | `#17495B` | Color principal. Títulos, barra, botón primario | **8.76** ✅ AA | **9.81** ✅ AA |
| **verde páramo** | `#2F7D5C` | Lo que salió bien. Confirmaciones, factura activa | **4.46** ⚠ solo texto grande | **4.99** ✅ AA |
| **rojo anulada** | `#B4402F` | Lo que falló o se anuló | **5.05** ✅ AA | **5.65** ✅ AA |
| **piedra** | `#4A4A4A` | Texto corriente | **7.92** ✅ AA | **8.86** ✅ AA |
| **ámbar cosecha** | `#E39B2D` | **Solo acento.** El isotipo y el foco | **2.09** ❌ | **2.34** ❌ |
| **niebla** | `#F4F2EC` | El fondo | — | — |

> **Los números son contrastes WCAG**, calculados con la fórmula
> `(L1+0.05)/(L2+0.05)`. El nivel **AA** pide **4.5** para texto normal, **3.0**
> para texto grande (18pt, o 14pt en negrita) y **3.0** para componentes de
> interfaz.

### Las dos reglas que salen de esa tabla

> **1. El ámbar NUNCA lleva texto, ni es texto.** Con 2.09 sobre el fondo y 2.34
> con blanco encima, no alcanza ni el mínimo de texto grande. Es un acento: el
> triángulo del isotipo y poco más. Si alguna vez hace falta ámbar con una
> palabra encima, el texto va en `piedra` o en `azul cordillera`, nunca en
> blanco.

> **2. El verde páramo no se usa para texto pequeño sobre el fondo.** 4.46 se
> queda a cuatro centésimas del 4.5, y eso no se redondea hacia arriba. Para
> texto va en negrita o en tamaño grande; para un botón, con **blanco encima**,
> donde sí llega a 4.99.

---

## 3. El foco, y un defecto que este documento destapó

Escribir este manual destapó dos cosas en la interfaz, y las dos se corrigieron:

**1. Había DOS focos distintos.** `marca.css` ponía el borde del campo enfocado
en ámbar y `app.css` lo ponía en el azul de Bootstrap (`#86b7fe`). Según en qué
campo se parara uno, se veía uno u otro. Tener dos es tener ninguno.

**2. Y el ámbar no cumplía.** Un borde de foco es un **indicador de interfaz**, y
WCAG le pide **3:1**. El ámbar da **2.09** contra el fondo: alguien que distinga
mal los colores podía no ver en qué campo estaba escribiendo.

> **Los dos quedaron en `azul cordillera`, que da 8.76.** Un solo foco, y que se
> ve.

El isotipo `▲` **sí se queda en ámbar**: los logotipos están exentos de la
exigencia de contraste (WCAG 1.4.3 y 1.4.11), porque una marca es una marca.

### Y tres colores de Bootstrap que se habían colado

`app.css` —la hoja del proyecto— traía `#0b5ed7` y `#0a58ca` para el botón
primario al pasar el mouse, y `#6f42c1` para una barra del tablero. Ninguno es
de esta marca: son los valores por defecto de Bootstrap, que quedaron ahí de
cuando se copió el estilo.

Los tres salen ahora de la paleta. Y el tono oscuro del azul dejó de estar
escrito a mano en tres sitios: tiene su propia variable,
`--azul-cordillera-oscuro`.

> **Así es como un manual de marca gana el sueldo:** no diciendo qué bonito se
> ve, sino dejando que se pueda contar cuántos colores hay fuera de él. Eran
> cuatro. Ahora son cero — y §6 dice cómo contarlos.

---

## 4. Las tipografías

| Dónde | Familia | Por qué |
|---|---|---|
| Logotipo, títulos `h1` `h2`, títulos de tarjeta | **Georgia**, serif | Da el aire institucional de la marca |
| Todo lo demás | la del sistema | No se carga ninguna fuente de internet |
| Números de dinero, códigos, identificadores | **monoespaciada** | Para que las cifras se alineen y se puedan comparar de un vistazo |

> **No se trae ninguna tipografía de un CDN**, y es la misma razón por la que
> Bootstrap está guardado en el repositorio: la interfaz tiene que verse igual
> sin internet, y nadie tiene que enterarse de quién la abrió.

---

## 5. Dónde manda cada color en la interfaz

| Elemento | Color | Por qué ése |
|---|---|---|
| Barra superior, títulos | azul cordillera | Es el principal |
| Botón «Emitir», «Agregar», «Guardar» | azul cordillera, texto blanco | Acción principal |
| Factura **activa**, confirmaciones | verde páramo | Salió bien |
| Factura **anulada**, errores, «Eliminar» | rojo anulada | Algo se deshizo o falló |
| Fondo de toda la aplicación | niebla | Menos agresivo que el blanco puro |
| Texto corriente | piedra | |

> **El rojo significa UNA cosa: que algo se deshizo o falló.** Usarlo para
> «destacar» un dato importante que no es un error le quita el significado — y
> el día que haya un error de verdad, nadie lo va a distinguir.

---

## 6. Cómo comprobar que la interfaz cumple el manual

```powershell
# Ningún color escrito a mano: todo sale de una variable
Select-String -Path front_blazor\wwwroot\*.css,front_blazor\Components\**\*.razor `
  -Pattern '#[0-9a-fA-F]{6}' | Where-Object { $_.Line -notmatch '--' }
```

Lo que aparezca ahí es un color que alguien puso a ojo, y eso es exactamente lo
que el manual existe para impedir.

| Qué | Dónde |
|---|---|
| Los valores, como variables | [`marca.css`](../../front_blazor/wwwroot/marca.css) |
| Qué pide la versión 4 sobre la marca | el `2_spec.md` de `v4_aplicativo` |
| Por qué el contraste se calcula y no se opina | [`CONCEPTOS_IDENTIDAD_VISUAL.md`](../conceptos/CONCEPTOS_IDENTIDAD_VISUAL.md) |
