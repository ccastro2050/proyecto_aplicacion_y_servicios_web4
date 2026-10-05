# Versión 2 con IA — índice de las tres guías

> **La versión de la clave foránea.** Y con ella llegan tres cosas que la v1 no
> tenía: un código HTTP nuevo —el **409**—, un control nuevo en la interfaz —el
> **desplegable**— y una operación que **no puede quedar a medias** —la factura
> con sus renglones—.
>
> El prompt suyo está en la guía que lleva su nombre.

---

## 1. Quién hace qué, y con qué herramienta

| | Su guía | Su camino | Lo que le toca en la v2 |
|---|---|---|---|
| **Carlos** | [`GUIA_IA2_CARLOS.md`](GUIA_IA2_CARLOS.md) | **B · IDE agéntico** | `factura` — procedimientos y transacción |
| **Paco** | [`GUIA_IA2_PACO.md`](GUIA_IA2_PACO.md) | **A · Chat web** | `cliente` y `vendedor` |
| **Luis** | [`GUIA_IA2_LUIS.md`](GUIA_IA2_LUIS.md) | **A · Chat web** | las dos tablas puente y `usuario-con-roles` |

> **Paco y Luis siguen con chat, y es la última versión en que lo hacen a solas**
> antes de la v3. Desde la **v4** los tres pasan a agente, porque para entonces
> habrá un repositorio grande que leer. Acuerdo en
> [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3.

---

## 2. Quién escribe cada documento de ESTA carpeta

| Documento | Lo redacta |
|---|---|
| [`2_spec.md`](2_spec.md) · [`4_research.md`](4_research.md) · [`7_quickstart.md`](7_quickstart.md) | **Paco** |
| [`3_plan.md`](3_plan.md) · [`5_data_model.md`](5_data_model.md) · [`6_contracts.md`](6_contracts.md) · [`8_tasks.md`](8_tasks.md) | **Luis** |
| [`9_checklist.md`](9_checklist.md) · este índice | **Carlos** |
| `GUIA_IA2_<NOMBRE>` | **cada quien la suya** |

> **El criterio es el mismo de siempre:** la compuerta se queda con el
> integrador; lo demás se reparte, **para que los tres hayan leído el spec kit
> escribiéndolo**. Manda [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §5.

> **Y en esta versión el `4_research.md` importa más que en la v1.** Ahí está
> por qué una clave foránea inexistente responde **409 y no 422** — y esa es una
> pregunta de sustentación casi segura. Lo escribe Paco, pero lo leen los tres.

---

## 3. El orden

```
   1 · CARLOS            2 · PACO y LUIS, a la vez        3 · CARLOS
   factura          →    sus recursos                →    integra y pone el tag
```

> **Carlos va primero otra vez, y por una razón distinta a la de la v1.** Aquí no
> construye un molde: construye **el recurso que rompe el molde**. `factura` es
> el único que **no escribe SQL de tablas** —llama procedimientos—, y conviene
> que esa diferencia esté en el repositorio antes de que los otros dos empiecen,
> para que nadie la calque donde no va.

---

## 4. La decisión que define la versión, y la tienen que saber los tres

Alguien manda `{"fkcodpersona": "NOEXISTE"}`. ¿Qué responde la API?

| | El argumento |
|---|---|
| **422** | «El dato está mal, y el 422 es para datos malos» |
| **409** ✅ | **El dato NO está mal: tiene la forma correcta.** Lo que se rompe es el **estado** de la base de datos: esa fila no está |

> **Lo que decide:** el 422 se reserva para lo que la petición puede rechazar
> **sin consultar nada** — un campo que falta, un número negativo, un tipo
> equivocado. Saber si `P001` existe **exige ir a la base de datos**, y eso ya es estado.
>
> **Y de ahí sale por qué el 409 aparece en la v2 y no antes:** es la primera
> versión en la que una fila **depende de otra**. En la v1 no había nada que
> violar.

### La deuda de la v1 que se paga aquí

La v1 devolvía **500** al insertar un código repetido, **y estaba declarado en su
spec**. No era un descuido: era una deuda con fecha.

> **Por qué se dejó mal a propósito.** Un 500 es el motor gritando sin
> traducción, y verlo una vez enseña más que leer que «hay que capturar la
> excepción». En la v2 ese mismo caso responde **409** — y la diferencia entre
> las dos respuestas es la lección: **quién traduce el error del motor, y en qué
> capa**.

---

## 5. Los cinco tropiezos de esta versión

**Léanlos los tres antes de empezar.** Cuatro de los cinco fallan **en
silencio**, y eso es lo que los hace caros.

| | Qué pasa | Cómo se ve |
|---|---|---|
| **1** | El front deserializa el sobre a `List<T>` | La pantalla sale **vacía, sin un solo error**. La API devuelve `{tabla, limite, total, datos[]}`: hay que entrar a `datos` |
| **2** | Dapper mapea por **nombre de columna** | Una propiedad que no se llama igual que su columna llega `null` **en silencio**, con HTTP 200 |
| **3** | Los nombres que devuelven los procedimientos | Vienen `nombre_cliente` e `idrol`; las propiedades son `NombreCliente` e `IdRol`. Sin `[JsonPropertyName]` llegan `null` y **0** |
| **4** | La cadena vacía de un desplegable opcional | `""` **no es** `null`. Aquí lo atrapa la anotación: **422** |
| **5** | Los desplegables cargados **en fila** | Con la API apagada, cinco listas en fila son **50 segundos** en blanco. Con `Task.WhenAll`, 10 |

> **Los cuatro primeros no lanzan excepción, no dejan log y responden 200.** Hay
> un dato equivocado y nada más. Por eso el cierre de esta versión se hace
> **mirando la interfaz**, no solo leyendo respuestas de la API.

> **Y el quinto no se encuentra leyendo: se encuentra MIDIENDO.** Apareció
> cronometrando el criterio «apague la API y la interfaz sigue en pie». La
> interfaz *sí* quedaba en pie: lo hacía **casi un minuto después**, que para
> quien la usa es lo mismo que estar roto.
>
> **Un criterio de aceptación sin un número no es comprobable, es opinable.**

---

## 6. Lo que vale para los tres

### La IA tiene que COMENTAR lo que escribe

> Comenta todo el código que generes, en español. Cada archivo empieza con un
> bloque que dice QUÉ ES y QUÉ PAPEL cumple. Cada método no evidente lleva su
> comentario, y dice **por qué** está escrito así, no **qué** hace la línea.

> **Y desde ESTA versión la interpretabilidad se califica**, hablando y en
> persona: el profesor abre un archivo y usted cuenta qué hace y por qué está
> así. **Un archivo sin comentarios es un archivo que va a tener que reconstruir
> de memoria enfrente de él.**

### Su identidad, antes del primer commit

```powershell
# Parado en LA CARPETA DEL PROYECTO —la del git clone—, porque ahí es donde Git
# guarda con qué nombre firma (.git\config).
git config user.name "su-usuario-de-github"
git config user.email "su-correo-de-github"

# COMPRUÉBELO SIEMPRE, aunque crea que ya estaba.
git config user.name
git config user.email
```

### Nadie trabaja en `main`

Cada uno en su rama —`rama-carlos-v2`, `rama-paco-v2`, `rama-luis-v2`— y todo
entra por **Pull Request**. Solo Carlos fusiona.

---

## 7. Lo que NINGUNO sube a la IA

| No se sube | Por qué |
|---|---|
| `9_checklist.md` | Es **su** compuerta, no la de la IA |
| `0_mapa_versiones.md` | Le revelaría lo que viene; **la v2 no anticipa** |
| `db/bdfacturas.sql` | **Viene dado.** Las tablas y los procedimientos **ya existen** |

> **Esto último es nuevo y conviene subrayarlo:** los seis procedimientos de
> `factura` **ya están en la base de datos** desde la v1. La IA **no los escribe**: los
> llama. Si propone crear un `CREATE PROCEDURE`, está contradiciendo el Artículo 5.

---

## 8. Cuándo está terminada la versión

| Qué se comprueba | Cómo |
|---|---|
| El 409 de clave foránea | `POST /api/cliente` con `fkcodpersona: "P999"` |
| El 409 de pareja repetida | `POST /api/rol-usuario` dos veces con lo mismo |
| El 409 de factura ya anulada | Anular dos veces la misma |
| **La transacción** | Una factura con **tres** productos donde el **segundo** no tenga stock: no debe quedar **ninguno** |
| La devolución de stock | Anotar el stock, emitir, anular, comparar: igual |
| El desplegable opcional | Crear un cliente **sin** empresa |
| **La regresión de la v1** | Las 37 operaciones de la v1, otra vez |

> **La prueba de la transacción es la única que importa de verdad**, y es la que
> más se olvida. Si al final quedó una factura con un renglón, **la transacción
> no existe** aunque el procedimiento diga `BEGIN TRANSACTION`.

> **Y la regresión no es burocracia:** este proyecto es **acumulativo**. Una
> versión que rompe la anterior no está terminada, está cambiada.

Con todo eso en verde y el `9_checklist.md` marcado **por una persona**:

```powershell
git tag -a v2 -m "Version 2: las seis tablas con clave foranea"
git push origin v2
```
