# Versión 4 — Paco · camino B (IDE agéntico)

> **Su parte:** tres consultas y **el manual de marca**.
>
> **Y aquí usted cambia de herramienta.** Las tres versiones anteriores las hizo
> con chat; ésta y la v5 son con **agente**. La §2 dice qué cambia y qué se
> vuelve peligroso.

---

## 1. Lo que construye

| | |
|---|---|
| **3 consultas** | `ticket-por-vendedor` · `anulaciones-por-cliente` · `credito-contra-consumo` |
| **El manual de marca** | `marca.css`, la paleta, y que **ningún color quede escrito a mano** |

---

## 2. Su primer agente: qué cambia

Usted ya sabe pedirle código a una IA. Lo que cambia no es el prompt: es **quién
pega los archivos**.

| Con chat (v1–v3) | Con agente (v4–v5) |
|---|---|
| Usted pegaba cada archivo | **Él los escribe, sin preguntar** |
| Veía cada línea antes de que entrara | Ve **el resultado** |
| El error estaba donde usted pegó | El error puede estar en un archivo que no abrió |

**Las dos reglas que reemplazan al «pegar de a uno»:**

> **1. Que lea, resuma y ESPERE antes de tocar nada.** Está en la primera línea
> del prompt. Si el resumen está mal, el código va a estar mal — y descubrirlo
> ahí cuesta un minuto en vez de una tarde.

> **2. Acote qué carpetas puede escribir.** Un agente sin alcance definido es
> capaz de reorganizarle el proyecto entero con la mejor intención. En el prompt
> va dicho: `api_facturas/` y `front_blazor/`, nada más.

> **Y una tercera, que es suya y de nadie más:** usted viene de tres versiones
> leyendo cada archivo antes de pegarlo. **Eso es exactamente lo que ahora le
> permite juzgar lo que el agente escribe.** No lo pierda: revise como si
> todavía tuviera que pegarlo.

---

## 3. El manual de marca, que es la parte seria

No es «ponerle colores bonitos». Es fijar una paleta **y que el código no pueda
salirse de ella**.

### La paleta, con sus contrastes CALCULADOS

| Nombre | Hex | Sobre el fondo | Para qué |
|---|---|---|---|
| **azul cordillera** | `#17495B` | **8.76** ✅ | El principal: títulos, barra, botón primario |
| **verde páramo** | `#2F7D5C` | **4.46** ⚠ | Lo que salió bien. **Solo texto grande** |
| **rojo anulada** | `#B4402F` | **5.05** ✅ | Lo que falló o se anuló |
| **piedra** | `#4A4A4A` | **7.92** ✅ | Texto corriente |
| **ámbar cosecha** | `#E39B2D` | **2.09** ❌ | **Solo acento.** Nunca lleva texto |
| **niebla** | `#F4F2EC` | — | El fondo |

> **Los números son contrastes WCAG**, con la fórmula `(L1+0.05)/(L2+0.05)`. El
> nivel **AA** pide **4.5** para texto normal y **3.0** para texto grande y para
> **indicadores de interfaz** — como el borde de un campo enfocado.

**Y de ahí salen dos reglas duras:**

> **1. El ámbar NUNCA lleva texto, ni es texto.** Con 2.09 no alcanza ni el
> mínimo de texto grande. Es un acento y poco más.

> **2. El verde no se usa para texto pequeño sobre el fondo.** 4.46 se queda a
> **cuatro centésimas** del 4.5, y eso **no se redondea hacia arriba**.

### El defecto que este proyecto ya cometió, para que no lo repita

Cuando se escribió el manual por primera vez, se encontró esto:

| | |
|---|---|
| **Había DOS focos distintos** | `marca.css` ponía el borde del campo enfocado en ámbar; `app.css` lo ponía en el azul de Bootstrap. Según el campo, se veía uno u otro. **Tener dos es tener ninguno** |
| **Y el ámbar no cumplía** | 2.09 cuando WCAG pide 3.0 para un indicador. Alguien que distinga mal los colores no veía dónde estaba escribiendo |
| **Tres colores de Bootstrap colados** | `#0b5ed7`, `#0a58ca` y `#6f42c1`, de cuando se copió el estilo |

> **Los cuatro se arreglaron, y así es como un manual de marca gana el sueldo:**
> no diciendo qué bonito se ve, **sino dejando que se pueda CONTAR cuántos
> colores hay fuera de él**. Eran cuatro. Tienen que ser cero.

---

## 4. Antes de abrir el agente

```powershell
git switch main ; git pull origin main
git switch -c rama-paco-v4
git config user.name "ccastro2050-50" ; git config user.email "su-correo-de-github"
git config user.name ; git config user.email

# Y pregúntele a Carlos cuál quedó el sobre de las consultas.
```

---

## 5. El prompt (cópielo tal cual)

```
Agrega TRES CONSULTAS DE NEGOCIO y EL MANUAL DE MARCA a este proyecto,
que ya tiene tres versiones funcionando con autenticación y permisos.
Trabajo en equipo: mis compañeros hacen las otras siete consultas, el
tablero y las pantallas de consulta.

PRIMERO lee los documentos bajo docs/spec_kit/ (1_constitution.md y los
de versiones/v4_aplicativo/) y el código que ya existe, incluidas las
consultas que mi compañero acaba de agregar. Después resume en máximo 10
líneas qué vas a construir y ESPERA MI CONFIRMACIÓN antes de escribir un
solo archivo.

PUEDES ESCRIBIR EN: api_facturas/ y front_blazor/
NO TOQUES: docs/ (solo lectura) ni db/ (la base de datos viene dada)

LAS TRES CONSULTAS QUE ME TOCAN:

  GET /api/consultas/ticket-por-vendedor
      el ticket promedio: total vendido dividido entre número de facturas
  GET /api/consultas/anulaciones-por-cliente
      cuántas facturas anuladas tiene cada cliente
  GET /api/consultas/credito-contra-consumo
      el crédito de cada cliente contra lo que de verdad consumió

EL SOBRE es el mismo que ya usan las otras consultas:
  { "consulta": "...", "total": N, "datos": [ ... ] }
  NO lleva "limite" y el primer campo se llama "consulta", no "tabla".
  CALCA EXACTAMENTE la forma que ya está en el código.

REGLAS DE LAS CONSULTAS:
  · El GROUP BY va EN SQL, no en C# con LINQ.
  · Las facturas ANULADAS no cuentan como ventas — salvo en
    anulaciones-por-cliente, que es justo lo que cuenta.
  · OJO CON LA DIVISIÓN del ticket promedio: un vendedor sin facturas
    daría división por cero. Resuélvelo en el SQL.
  · Cada consulta exige permiso con [ExigePermiso], usando un nombre de
    ruta QUE YA EXISTA en la tabla `ruta`. No inventes etiquetas.

EL MANUAL DE MARCA:

  Crea front_blazor/wwwroot/marca.css con la paleta como VARIABLES CSS:

     --azul-cordillera:  #17495B    el principal
     --verde-paramo:     #2F7D5C    lo que salió bien
     --rojo-anulada:     #B4402F    lo que falló o se anuló
     --piedra:           #4A4A4A    texto corriente
     --ambar-cosecha:    #E39B2D    SOLO acento
     --niebla:           #F4F2EC    el fondo

  Y LA REGLA QUE IMPORTA: las pantallas usan LA VARIABLE, nunca el valor.
  Revisa TODO el front y reemplaza cualquier color escrito a mano
  —incluidos los que vienen de Bootstrap, como #0b5ed7 o #6f42c1— por su
  variable. Al terminar, buscar #rrggbb fuera de marca.css NO DEBE
  ENCONTRAR NADA.

  DOS RESTRICCIONES DE CONTRASTE, que son accesibilidad y no gusto:
   · El ámbar da 2.09 de contraste. NUNCA lleva texto encima ni se usa
     como texto. Es un acento.
   · El borde del campo enfocado es un INDICADOR DE INTERFAZ y WCAG le
     pide 3.0. Úsalo en azul cordillera (8.76). Y que haya UN SOLO estilo
     de foco en todo el front: si hay dos definiciones distintas, la
     interfaz tiene dos focos y eso es tener ninguno.

REGLAS QUE SIGUEN VIGENTES:
  · Las TRES CAPAS. El servicio NO nombra nada de HTTP.
  · SIN ORM. SQL a mano con Dapper, parametrizado.
  · TODO EN ESPAÑOL, y la interfaz no habla en jerga.
  · NO toques las consultas de mis compañeros ni el tablero.

COMENTA TODO, en español, diciendo POR QUÉ. En una consulta, el
comentario útil dice QUÉ PREGUNTA DEL NEGOCIO responde. En el CSS, dice
por qué ese color y no otro.

ORDEN: primero las tres consultas, y la marca de último —cuando ya haya
pantallas que revisar—. Dime qué archivos tocaste en cada paso.
```

> **Y ahora la parte que el prompt no puede hacer por usted: LEER esos
> comentarios.** La IA comenta lo que **cree** que hizo, y no siempre coincide
> con lo que hizo. **Un comentario equivocado es peor que ninguno**, porque el
> siguiente que lo lea le va a creer.
>
> | | |
> |---|---|
> | El comentario **como producto** | sirve para quien llegue en seis meses |
> | El comentario **como ejercicio** | **revisarlo lo obliga a entender.** Ahí está el valor para quien aprende |
> | El comentario **como evidencia** | vale poco: se puede generar sin entender nada |
>
> **Por eso la interpretabilidad se califica HABLANDO.** Un comentario se puede
> recitar; una respuesta a *«¿y si cambiamos esto?»* no. Si usted no puede juzgar
> si un comentario es **cierto**, no entendió el código — y ésa es exactamente la
> señal que hay que buscar mientras revisa.


---

## 6. Comprobar lo suyo

```powershell
$t = (Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
      -ContentType 'application/json' `
      -Body '{"email":"admin@correo.com","contrasena":"admin123"}').token
$h = @{ Authorization = "Bearer $t" }

# 1 · Las tres responden, con el MISMO sobre que las de Carlos.
Invoke-RestMethod http://localhost:8035/api/consultas/ticket-por-vendedor -Headers $h

# 2 · LA DIVISIÓN POR CERO: cree un vendedor nuevo, SIN facturas,
#     y vuelva a consultar. No debe reventar ni devolver infinito.

# 3 · LA COMPROBACIÓN DEL MANUAL, que es un número y no una opinión:
#     ni un color escrito a mano fuera de marca.css. TIENE QUE DAR 0.
Select-String -Path front_blazor\wwwroot\*.css,front_blazor\**\*.razor `
  -Pattern '#[0-9a-fA-F]{6}' | Where-Object { $_.Path -notmatch 'marca.css' } |
  Measure-Object | Select-Object Count

# 4 · UN SOLO estilo de foco. Debe aparecer UNA vez.
Select-String -Path front_blazor\wwwroot\*.css -Pattern ':focus' |
  Select-Object Path, LineNumber, Line
```

> **El paso 3 es el que convierte el manual en algo exigible.** «La interfaz se
> ve bien» no se puede calificar; «hay cero colores fuera de la paleta» sí.

> **Y el paso 4 es el que este proyecto falló la primera vez.** Si `:focus`
> aparece dos veces con colores distintos, la interfaz tiene dos focos.

---

## 7. Subir y abrir el PR

```powershell
git status ; git add api_facturas/ front_blazor/
git commit -m "feat: ticket-por-vendedor, con la division por cero resuelta en SQL"
git push -u origin rama-paco-v4
```

> **En la descripción del PR ponga el resultado del paso 3.** Si no es cero, el
> manual no se cumplió — y eso Carlos lo va a correr antes de fusionar.
