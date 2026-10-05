# Versión 4 — Luis · camino B (IDE agéntico)

> **Su parte:** tres consultas **del propio sistema** y las **pantallas de
> consulta**.
>
> **Y aquí usted cambia de herramienta.** Las tres versiones anteriores con
> chat; ésta y la v5 con **agente**. La §2 dice qué cambia.

---

## 1. Lo que construye

| | |
|---|---|
| **3 consultas** | `productos-sin-vender` · `alcance-de-usuarios` · `interfaces-sin-usuarios` |
| **Las pantallas** | Donde se ven las diez consultas |

> **Las suyas no son de ventas: son del sistema mirándose a sí mismo.** Y le
> tocan a usted porque en la v1 construyó `rol` y `ruta`, y en la v2 las tablas
> puente. Son las mismas tablas, ahora preguntándoles algo útil.

---

## 2. Su primer agente: qué cambia

| Con chat (v1–v3) | Con agente (v4–v5) |
|---|---|
| Usted pegaba cada archivo | **Él los escribe, sin preguntar** |
| Veía cada línea antes de que entrara | Ve **el resultado** |

> **1. Que lea, resuma y ESPERE antes de tocar nada.** Si el resumen está mal,
> el código va a estar mal, y descubrirlo ahí cuesta un minuto.
>
> **2. Acote qué carpetas puede escribir.** Va en el prompt.
>
> **3. Y lo que usted trae de las tres versiones con chat —leer antes de
> aceptar— es justo lo que ahora le permite juzgar al agente.** No lo pierda.

---

## 3. LA TRAMPA DE SU PARTE: dos de sus tres consultas devuelven VACÍO

**Y es el resultado correcto.** Con los datos sembrados:

| Consulta | Devuelve | Porque |
|---|---|---|
| `productos-sin-vender` | `total: 0` | **todos** los productos se han vendido alguna vez |
| `interfaces-sin-usuarios` | `total: 0` | **todas** las pantallas las alcanza algún rol |

> **«Vacío» y «roto» se ven exactamente igual en una pantalla**, y ahí está el
> problema: usted puede entregar una consulta con el `JOIN` al revés, verla en
> cero, y pensar que funciona.

### Cómo se prueban de verdad: provocando el caso

```powershell
# productos-sin-vender
# 1 · Cree un producto nuevo y NO lo venda.
# 2 · Consulte: AHORA tiene que aparecer, con total: 1.
# 3 · Bórrelo y vuelva a consultar: otra vez en cero.

# interfaces-sin-usuarios
# 1 · Cree una ruta nueva y NO se la asigne a ningún rol.
# 2 · Consulte: tiene que aparecer.
# 3 · Bórrela.
```

> **Una consulta que nunca se vio devolver algo no está probada.** Es la lección
> de su parte, y vale para toda la carrera: **una prueba que solo confirma el
> caso vacío no distingue entre «no hay» y «no busca».**

> **Y `interfaces-sin-usuarios` es la más útil de las diez para administrar**,
> aunque hoy dé cero: una pantalla que ningún rol alcanza **está construida y
> nadie la ve**. Es trabajo pagado que no existe para el usuario.

---

## 4. Antes de abrir el agente

```powershell
git switch main ; git pull origin main
git switch -c rama-luis-v4
git config user.name "ccastro202050" ; git config user.email "su-correo-de-github"
git config user.name ; git config user.email
```

> **Pregúnteles a Carlos y a Paco** cuál quedó el sobre de las consultas y cómo
> se llaman las suyas: sus pantallas las van a mostrar todas.

---

## 5. El prompt (cópielo tal cual)

```
Agrega TRES CONSULTAS DE NEGOCIO y LAS PANTALLAS DE CONSULTA a este
proyecto, que ya tiene tres versiones funcionando con autenticación y
permisos. Trabajo en equipo: mis compañeros hacen las otras siete
consultas, el tablero y el manual de marca.

PRIMERO lee los documentos bajo docs/spec_kit/ (1_constitution.md y los
de versiones/v4_aplicativo/) y el código que ya existe, incluidas las
consultas que mis compañeros acaban de agregar. Después resume en máximo
10 líneas qué vas a construir y ESPERA MI CONFIRMACIÓN antes de escribir
un solo archivo.

PUEDES ESCRIBIR EN: api_facturas/ y front_blazor/
NO TOQUES: docs/ (solo lectura) ni db/ (la base viene dada)

LAS TRES CONSULTAS QUE ME TOCAN, y no son de ventas:

  GET /api/consultas/productos-sin-vender
      qué productos no aparecen en NINGUNA factura
  GET /api/consultas/alcance-de-usuarios
      a cuántas pantallas alcanza cada usuario, por sus roles
  GET /api/consultas/interfaces-sin-usuarios
      qué rutas NO están asignadas a ningún rol

EL SOBRE es el mismo que ya usan las otras consultas:
  { "consulta": "...", "total": N, "datos": [ ... ] }
  CALCA EXACTAMENTE la forma que ya está en el código.

ATENCIÓN — DOS DE MIS TRES CONSULTAS VAN A DEVOLVER CERO FILAS con los
datos sembrados, Y ESO ES CORRECTO: hoy todos los productos se han
vendido y todas las pantallas las alcanza algún rol. NO "arregles" la
consulta para que devuelva algo, y NO cambies los datos sembrados.
Lo que sí tienes que hacer es ESCRIBIRLAS BIEN:
  · productos-sin-vender y interfaces-sin-usuarios son LEFT JOIN con
    WHERE ... IS NULL, o NOT EXISTS. No un INNER JOIN.
  · El GROUP BY va EN SQL, no en C# con LINQ.
  · Cada consulta exige permiso con [ExigePermiso], con un nombre de ruta
    QUE YA EXISTA en la tabla `ruta`. No inventes etiquetas.

LAS PANTALLAS DE CONSULTA, que muestran LAS DIEZ —las mías y las de mis
compañeros—:

  · Una pantalla por consulta, o una con un selector: tú propones, pero
    cada una con su DIRECCIÓN PROPIA, no una ruta con el nombre de la
    consulta como parámetro.
  · CUANDO UNA CONSULTA DEVUELVE CERO FILAS, la pantalla NO puede quedar
    en blanco: tiene que decir, en castellano, que no hay resultados —y
    si se puede, por qué no los hay—. Una pantalla vacía y una pantalla
    rota se ven igual, y el usuario no tiene cómo distinguirlas.
  · Usa las variables de color del manual de marca que está haciendo mi
    compañero. NO escribas colores a mano.
  · Carga los datos con Task.WhenAll si la pantalla pide varias cosas.

REGLAS QUE SIGUEN VIGENTES:
  · Las TRES CAPAS. El servicio NO nombra nada de HTTP.
  · SIN ORM. SQL a mano con Dapper, parametrizado.
  · TODO EN ESPAÑOL, y la interfaz NO habla en jerga.
  · El front NO habla con la base: solo HTTP contra la API.
  · NO toques las consultas de mis compañeros ni el tablero.

COMENTA TODO, en español, diciendo POR QUÉ. En una consulta el comentario
útil dice QUÉ PREGUNTA DEL NEGOCIO responde — y en las mías, además, POR
QUÉ hoy devuelve cero.

ORDEN: primero alcance-de-usuarios, que sí trae datos y se puede ver
funcionando. Después las dos que dan vacío. Las pantallas de último.
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


> **Fíjese en el orden del prompt: `alcance-de-usuarios` primero.** Es la única
> de las tres que devuelve filas, así que es la que le permite comprobar que el
> sobre y la capa están bien **antes** de meterse con las dos que no se pueden
> distinguir de un error.

---

## 6. Comprobar lo suyo

```powershell
$t = (Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
      -ContentType 'application/json' `
      -Body '{"email":"admin@correo.com","contrasena":"admin123"}').token
$h = @{ Authorization = "Bearer $t" }

# 1 · La que sí trae datos: compruebe el sobre y las cifras.
Invoke-RestMethod http://localhost:8035/api/consultas/alcance-de-usuarios -Headers $h

# 2 · LA PRUEBA DE LAS DOS QUE DAN VACÍO — provocando el caso.
#     Cree un producto y NO lo venda:
$p = @{ codigo='PRLUIS'; nombre='Sin vender'; stock=5; valorunitario=1000 }
Invoke-RestMethod http://localhost:8035/api/producto -Method Post -Headers $h `
  -ContentType 'application/json' -Body ($p | ConvertTo-Json)

#     Ahora TIENE que aparecer:
Invoke-RestMethod http://localhost:8035/api/consultas/productos-sin-vender -Headers $h
#     espera: total 1, y PRLUIS adentro

#     Bórrelo, y vuelve a cero:
Invoke-RestMethod http://localhost:8035/api/producto/PRLUIS -Method Delete -Headers $h

# 3 · Y la pantalla con cero filas: NO puede quedar en blanco.
Start-Process http://localhost:8099/consultas
```

> **El paso 2 es el que separa una consulta escrita de una consulta probada.**
> Si con PRLUIS creado la consulta sigue en cero, tiene un `INNER JOIN` donde
> debía ir un `LEFT JOIN` — y sin provocar el caso, eso **no se ve nunca**.

```powershell
# 4 · El GROUP BY / el LEFT JOIN están en SQL. NO puede dar 0.
Select-String -Path api_facturas\Repositorios\RepositorioConsultas*.cs `
  -Pattern 'LEFT JOIN|NOT EXISTS' | Measure-Object | Select-Object Count

# 5 · Y ni un color a mano en sus pantallas. Tiene que dar 0.
Select-String -Path front_blazor\**\*.razor -Pattern '#[0-9a-fA-F]{6}' |
  Measure-Object | Select-Object Count
```

---

## 7. Subir y abrir el PR

```powershell
git status ; git add api_facturas/ front_blazor/
git commit -m "feat: productos-sin-vender, con LEFT JOIN (y probada provocando el caso)"
git push -u origin rama-luis-v4
```

> **En la descripción del PR cuente el paso 2**: que creó un producto sin vender,
> que apareció, y que al borrarlo volvió a cero. **Decir «devuelve vacío» no
> prueba nada** — y Carlos lo va a preguntar antes de fusionar.

> **Nadie toca el archivo de otro, ni para arreglárselo.**
