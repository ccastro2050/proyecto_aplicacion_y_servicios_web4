# Versión 4 — Carlos · camino B (IDE agéntico)

> **Su parte:** las **cuatro consultas de ventas** y el **tablero**.
>
> **Y usted va primero**, como siempre: su tablero es el que consume lo que los
> otros dos van a producir, así que la forma del sobre la fija usted.

---

## 1. Lo que construye

| | |
|---|---|
| **4 consultas** | `ventas-por-vendedor` · `ventas-por-cliente` · `ventas-por-producto` · `ventas-por-empresa` |
| **El tablero** | La pantalla de inicio, con indicadores |

**No construye** las otras seis consultas ni la marca ni las pantallas de
consulta — son de Paco y de Luis.

---

## 2. Un tablero no es un listado, y ésa es toda la diferencia

> **Si su pantalla de inicio muestra una tabla con las seis facturas, no hizo un
> tablero: hizo otro listado.** Un tablero responde preguntas **sin que nadie
> tenga que leer filas**: cuánto se vendió, quién vende más, qué se está
> acabando.

| Esto NO es tablero | Esto sí |
|---|---|
| «Las últimas 10 facturas» | «Vendido este mes: **$18 450 000**» |
| Una tabla con todo | «El que más vende: **Pedro Castillo**, 2 facturas» |

---

## 3. El sobre de las consultas, que lo define usted

```json
{ "consulta": "ventas_por_vendedor", "total": 3, "datos": [ … ] }
```

> **No es el del CRUD.** No tiene `limite`, y el primer campo se llama
> `consulta`, no `tabla`. Una consulta no se pagina: ya viene agrupada.
>
> **Usted lo fija y los otros dos lo calcan.** Si Paco y Luis devuelven otra
> forma, el front va a tener que tratar cada consulta distinto — y eso es lo que
> el sobre existe para evitar. **Avíseles cuál quedó.**

---

## 4. Las consultas NO llevan lógica de negocio en C#

```csharp
// MAL: traer todo y sumar en memoria
var facturas = await _repo.ListarTodasAsync();
var porVendedor = facturas.GroupBy(f => f.Vendedor).Select(...);
```

> **Eso trae la tabla entera a la aplicación para sumarla.** Con seis facturas
> funciona; con sesenta mil, tumba el proceso.
>
> **El `GROUP BY` va en SQL**, que es donde están los datos y donde hay índices.
> El repositorio devuelve **ya agrupado**, y el servicio no suma nada.

---

## 5. Antes de abrir el agente

```powershell
git switch main ; git pull origin main
git switch -c rama-carlos-v4
git config user.name "ccastro2050" ; git config user.email "su-correo-de-github"
git config user.name ; git config user.email

# Y compruebe que la v3 funciona: con token.
docker compose up -d --build
```

---

## 6. El prompt (cópielo tal cual)

```
Agrega CONSULTAS DE NEGOCIO y un TABLERO a este proyecto, que ya tiene
tres versiones funcionando con autenticación y permisos. Trabajo en
equipo: mis dos compañeros harán otras seis consultas, la marca y las
pantallas. YO HAGO CUATRO CONSULTAS Y EL TABLERO.

PRIMERO lee los documentos bajo docs/spec_kit/ (1_constitution.md y los
de versiones/v4_aplicativo/) y el código que ya existe. Después resume en
máximo 10 líneas qué vas a construir y ESPERA MI CONFIRMACIÓN antes de
escribir un solo archivo.

PUEDES ESCRIBIR EN: api_facturas/ y front_blazor/
NO TOQUES: docs/ (solo lectura) ni db/ (la base viene dada)

LAS CUATRO CONSULTAS QUE ME TOCAN:

  GET /api/consultas/ventas-por-vendedor
      cuánto vendió cada vendedor y en cuántas facturas
  GET /api/consultas/ventas-por-cliente
      cuánto compró cada cliente
  GET /api/consultas/ventas-por-producto
      cuánto se vendió de cada producto, en unidades y en dinero
  GET /api/consultas/ventas-por-empresa
      ventas agrupadas por la empresa del cliente

EL SOBRE DE RESPUESTA, que es distinto al del CRUD:

  { "consulta": "ventas_por_vendedor", "total": 3, "datos": [ ... ] }

  NO lleva "limite" y el primer campo se llama "consulta", no "tabla".
  Una consulta no se pagina: ya viene agrupada.

TRES REGLAS QUE NO SE NEGOCIAN:

  1. EL GROUP BY VA EN SQL, no en C#. NO traigas las tablas enteras para
     agrupar en memoria con LINQ: con seis facturas funciona y con
     sesenta mil tumba el proceso. El repositorio devuelve YA AGRUPADO y
     el servicio no suma nada.

  2. LAS FACTURAS ANULADAS NO CUENTAN como ventas. Filtra por
     estado = 'activa' en el SQL, no después.

  3. CADA CONSULTA EXIGE PERMISO con [ExigePermiso], igual que el resto
     del sistema. Usa un nombre de ruta QUE YA EXISTA en la tabla `ruta`
     — no inventes etiquetas nuevas: si no está en la tabla, responde 403
     a todo el mundo siempre y sin error.

EL TABLERO es la pantalla de inicio, y NO es un listado. Muestra
INDICADORES —totales, el que más vende, cuántas facturas activas y
anuladas—, no una tabla con las últimas facturas. Si muestra filas para
que alguien las lea, no es un tablero.

Carga los datos del tablero con Task.WhenAll, no una consulta tras otra.

REGLAS QUE SIGUEN VIGENTES:
  · Las TRES CAPAS con interfaces. El servicio NO nombra nada de HTTP.
  · SIN ORM. SQL a mano con Dapper, parametrizado.
  · TODO EN ESPAÑOL, y la interfaz no habla en jerga.
  · El front NO habla con la base: solo HTTP contra la API.

COMENTA TODO, en español, diciendo POR QUÉ. En una consulta el comentario
más útil es QUÉ PREGUNTA DEL NEGOCIO responde — el SQL ya dice el cómo.

ORDEN: primero ventas-por-vendedor completa —de la consulta al endpoint—,
y cuando responda bien, las otras tres calcando. El tablero de último,
cuando haya datos que mostrar. Dime qué archivos creaste en cada paso.
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

## 7. Supervisar, y comprobar

```powershell
# 1 · El GROUP BY está en SQL. Esto NO puede dar 0.
Select-String -Path api_facturas\Repositorios\RepositorioConsultas*.cs `
  -Pattern 'GROUP BY' | Measure-Object | Select-Object Count

# 2 · Y NO se agrupa en memoria. Esto tiene que dar 0.
Select-String -Path api_facturas\Servicios\ServicioConsultas*.cs `
  -Pattern '\.GroupBy\(|\.Sum\(' | Measure-Object | Select-Object Count

# 3 · Las cuatro responden, con el sobre correcto.
$t = (Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
      -ContentType 'application/json' `
      -Body '{"email":"admin@correo.com","contrasena":"admin123"}').token
$h = @{ Authorization = "Bearer $t" }
Invoke-RestMethod http://localhost:8035/api/consultas/ventas-por-vendedor -Headers $h
#    debe traer: consulta, total, datos — NO "tabla", NO "limite"

# 4 · LA PRUEBA DE LAS ANULADAS, que es la que se olvida:
#     anote el total de un vendedor, anule una de sus facturas,
#     y vuelva a consultar. EL TOTAL TIENE QUE BAJAR.
```

> **El paso 4 es la comprobación de su parte.** Si el total no cambia, la
> consulta está contando facturas anuladas como ventas — y eso no se nota
> mirando la pantalla, porque los números **parecen** correctos.

---

## 8. Subir, y después integrar

```powershell
git status ; git add api_facturas/ front_blazor/
git commit -m "feat: ventas-por-vendedor, agrupada en SQL y sin contar anuladas"
git push -u origin rama-carlos-v4
#   y el PR. AVÍSELES EL SOBRE a Paco y Luis antes de que empiecen.
```

**Al integrar**, además de lo de siempre:

| Qué revisar en los PR de ellos | |
|---|---|
| ¿El sobre es el mismo? | `consulta`, `total`, `datos` |
| ¿El `GROUP BY` está en SQL? | no en LINQ |
| ¿Las anuladas quedan fuera? | donde corresponda |
| **¿Probaron las dos consultas que dan vacío?** | Ver el índice, §4 |

```powershell
git tag -a v4 -m "Version 4: el aplicativo"
git push origin v4
```
