# Sustentación del código — diez preguntas, respondidas

> **Qué es este documento.** Diez preguntas sobre **este** código, con su
> respuesta y **con la forma de comprobarla**. No son preguntas de examen
> teórico: todas se responden señalando un archivo, corriendo un comando o
> mandando una petición.
>
> **Para qué existe.** Desde la **versión 2** se califica la
> **interpretabilidad**: el profesor abre un archivo y quien lo entregó cuenta
> qué hace y por qué está así. Es **verbal y presencial** — remoto se vale, en
> vivo es obligatorio. Este documento es el nivel de respuesta que se espera.
>
> **Material académico simulado** en el dominio. Las mediciones son reales y
> están fechadas; §11 dice cómo repetirlas.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## Antes de las diez: para qué sirve sustentar

> **Porque una IA puede generar código que funciona y que nadie entiende.** Y
> código que nadie entiende no se puede arreglar el día que falle, ni cambiar
> cuando el negocio cambie. Que corra es el requisito más bajo, no el más alto.
>
> **De ahí la regla del curso:** la IA **tiene que comentar lo que genera**, y la
> persona **tiene que poder explicarlo sin leer los comentarios**. Si solo puede
> leerlos en voz alta, no lo entendió — lo recibió.

---

## 1 · ¿Quién hizo qué, y dónde está el rastro?

**En el historial, y se cuenta en un comando.** Son **84 commits** de tres
personas:

```powershell
git shortlog -sne main
#     40  Carlos    ← 25 de contenido + 15 fusiones
#     24  Paco
#     20  Luis
```

> **Y la respuesta de sustentación completa no se queda en esa tabla**, porque
> esa tabla engaña: **las 15 fusiones del repositorio son de Carlos**, por ser el
> integrador. Descontándolas, el reparto real es **25 / 24 / 20** — casi parejo.
>
> ```powershell
> git log --no-merges --format='%an' | Group-Object | Select-Object Count, Name
> ```

**Para saber quién escribió un archivo concreto**, que es lo que de verdad se
califica:

```powershell
git log -1 --format='%an' -- docs/spec_kit/versiones/v1_sin_fk/2_spec.md
# responde: Paco
```

> **Y una advertencia sobre contar commits:** mide **actividad**, no dificultad.
> Luis tiene 20 y Carlos 40, y de ahí no se sigue que hiciera la mitad: a Luis le
> tocaron los recursos de llave `IDENTITY`, que son los únicos que **no se
> calcan** del molde. Ver [`CRONOGRAMA.md`](CRONOGRAMA.md) §3.

---

## 2 · ¿Dónde quedó una ambigüedad, y cómo se resolvió?

**En el `3_plan.md` de la v2, y la resolvió la API, no una discusión.**

El plan decía que mandar un desplegable opcional vacío —`""`— daría **409**,
porque *«la cadena vacía es un código que no existe»*. Suena bien. Pero:

```
POST /api/cliente  {"fkcodpersona":"PZZZ","fkcodempresa":"","credito":100}
  →  422  ·  "El campo fkcodempresa debe tener entre 1 y 10 caracteres."
```

> **El 409 nunca llega, porque la anotación atrapa el `""` antes.**
> `[StringLength(10, MinimumLength = 1)]` lo rechaza **en la petición**, y la base
> no se entera. Las dos respuestas son defendibles, y la que da es la mejor: se
> rechaza sin consultar nada.
>
> **Se corrigió el documento, no el código.** Y el spec kit también, que lo
> predecía mal.

> **La lección, que es la de todo el curso:** un documento puede predecir un
> camino que el código no toma, y eso **no se descubre leyendo** — se descubre
> mandando la petición. Un spec kit que nadie contrasta contra la API corriendo
> acumula predicciones que ya no son ciertas.

---

## 3 · ¿Qué es una compuerta, y qué pasa si se salta la segunda?

Una **compuerta** es un punto donde uno se detiene y **no sigue** hasta que esté
en verde. Hay tres, y viven dentro de los documentos:

| | Vive en | La pregunta que hace |
|---|---|---|
| **1 · Clarificaciones** | `2_spec.md` | ¿Hay algo que dos personas leerían distinto? |
| **2 · Chequeo de constitución** | el final de `3_plan.md` | ¿El plan respeta los artículos, uno por uno? ¿Los cinco documentos dicen lo mismo? |
| **3 · Lista de requisitos** | `9_checklist.md` | ¿Se cumple cada criterio? **La marca una persona** |

**¿Y si se salta la segunda?** Pasa exactamente lo que pasó aquí:

> El plan de la v2 predijo un 409 y el código hizo un 422. La compuerta 2
> pregunta *«¿los cinco documentos dicen lo mismo, y lo mismo que el código?»*.
> Saltarla deja el `3_plan.md` y el `6_contracts.md` diciendo cosas distintas, y
> el que las lea después **no sabe cuál creer**.
>
> **El daño no es inmediato, y eso es lo peligroso.** El sistema sigue
> funcionando perfecto. Lo que se rompe es la confianza en los documentos — y
> cuando un documento no es confiable, deja de leerse, y entonces ya no sirve de
> nada tenerlo.

> **La tercera NO la puede marcar una IA.** Una IA puede decir que un criterio se
> cumple; solo una persona puede **haberlo visto cumplirse**.

---

## 4 · ¿Qué no puede aparecer nunca en la capa de servicio, y quién decide el 404?

**En el servicio no puede aparecer nada de HTTP.** Ni un `StatusCode`, ni un
`NotFound()`, ni un `IActionResult`, ni el `HttpContext`.

```powershell
# Cuente cuántas veces el servicio nombra algo de HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\*.cs `
  -Pattern 'StatusCode|NotFound|BadRequest|IActionResult|HttpContext' |
  Measure-Object | Select-Object Count
```

**Medido el 4 de octubre de 2026: `0`.** Y su espejo: `0` `StatusCode` en
`Repositorios/`, `0` `SqlConnection` en `Controllers/`, `0` en el front.

**¿Quién decide el 404, entonces?** El **controlador**, y la cadena es ésta:

| Capa | Qué hace |
|---|---|
| **Repositorio** | Busca. No encuentra. Devuelve `null`. **No decide nada** |
| **Servicio** | Ve el `null` y lanza `NoEncontradoExcepcion` — *«esto no existe»*, en lenguaje del **negocio** |
| **Controlador** | Atrapa esa excepción y responde `StatusCode(404, …)` |

> **Y por qué importa tanto esta separación:** porque *«no existe»* es un hecho
> del negocio y *«404»* es una convención de un protocolo. El día que este
> servicio se llame desde una cola de mensajes, desde una tarea programada o
> desde una prueba, **el 404 no significa nada** — pero *«no existe»* sigue
> significando lo mismo.
>
> Un servicio que devuelve `NotFound()` solo sirve dentro de una petición HTTP. Y
> entonces no es una capa: es un pedazo del controlador que se mudó de archivo.

---

## 5 · Si mañana la base fuera PostgreSQL, ¿qué cambiaría?

**Nada de lo que está arriba. Y no es una promesa: ya pasó, y está medido.**

El segundo motor se construyó el 19 de agosto de 2026 y el salto completo fue:

| Se escribió | No se tocó |
|---|---|
| **14 archivos nuevos**, 12 de ellos repositorios `…Postgres` | **ni un controlador** |
| `FabricaPostgres` y su interruptor `MOTOR_BD` | **ni un servicio** |
| 1 373 líneas | ni una petición, ni un modelo |

```powershell
# Lo que el segundo motor cambió ARRIBA. Sale vacío.
git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios

# Y lo que cambió abajo: 18 archivos, 1373 inserciones
git diff --stat v4..v5 -- api_facturas/
```

> **Ojo con los tags, que son del mapa viejo.** `v4` es el tag de cuando
> PostgreSQL **era** la versión 4; hoy ese trabajo es la **v5** del mapa nuevo y
> todavía no tiene tag. El commit del tag `v4` lo dice en su propio mensaje:
> *«regresión doble 77/77 en cada motor sin recompilar»*. Ver
> [`CRONOGRAMA.md`](CRONOGRAMA.md) §2.

> **La cuenta que lo resume:** `Repositorios/` tiene **29 implementaciones** para
> **14 interfaces**. Dos por recurso, y arriba **nadie sabe cuál está puesta** —
> porque arriba se usa siempre la interfaz.

> **Y la respuesta de sustentación completa agrega la fábrica.** La fábrica es
> **el único sitio que decide cuál implementación de repositorio se usa** — no
> «el único que nombra clases concretas», que sería falso: `ServicioProducto` y
> `ProductoController` lo son y se nombran sin problema, porque de cada uno hay
> **uno solo**. Si alguien hiciera
> `new RepositorioProductoSqlServer()` dentro de un servicio, todo lo anterior se
> caería — y el compilador no diría nada, porque compilar es lo único que ese
> código haría bien.

---

## 6 · `Producto` y `ProductoCrear` son clases distintas. ¿Qué tiene una que la otra no?

```csharp
// Modelos/Producto.cs — la ENTIDAD: el molde de los objetos que viajan
public class Producto {
    public required string Codigo { get; set; }
    public required string Nombre { get; set; }
    public int Stock { get; set; }
    public decimal Valorunitario { get; set; }
}

// Peticiones/ProductoCrear.cs — la FRONTERA: lo que se acepta de afuera
public class ProductoCrear {
    [Required] [StringLength(10, MinimumLength = 1)] public string?  Codigo { get; set; }
    [Required] [MinLength(1)]                        public string?  Nombre { get; set; }
    [Required] [Range(0, int.MaxValue)]              public int?     Stock { get; set; }
    [Required] [Range(0, double.MaxValue)]           public decimal? Valorunitario { get; set; }
}
```

| | `Producto` | `ProductoCrear` |
|---|---|---|
| Para qué | Lo que **viaja** entre capas | Lo que **se acepta** de afuera |
| Los tipos | **no admiten nulo** (`required`, `int`) | **todos admiten nulo** (`string?`, `int?`) |
| Anotaciones | ninguna | `[Required]`, `[Range]`, `[StringLength]` |

> **Y la pregunta buena es: ¿por qué `int?` y no `int`, si `[Required]` ya lo
> exige?**
>
> Porque un `int` **no puede estar vacío**: si no viene en el cuerpo, llega en
> `0`. Y entonces *«mandé stock 0»* y *«no mandé stock»* son **el mismo valor**,
> y `[Required]` no tendría nada que detectar. Con `int?`, lo que no viene llega
> `null`, y el framework puede decir *«falta el campo stock»*.
>
> **Es la respuesta que separa a quien copió el patrón de quien lo entiende.**

> **Y por qué no usar `Producto` como cuerpo de entrada, que es lo cómodo:**
> porque entonces la validación viviría en la clase que **viaja**, y cada verbo
> exige cosas distintas. En `POST` el nombre es obligatorio; en `PATCH` no, porque
> lo que no se manda no se toca. Una sola clase no puede decir las dos cosas — y
> de ahí salen las tres peticiones por recurso.

---

## 7 · El mismo cuerpo, dos respuestas: 422 en PUT y 200 en PATCH

**Medido el 4 de octubre de 2026**, con el mismo cuerpo y sobre una fila creada
para esto:

```
PUT   /api/producto/PRZZZ   {"nombre":"Cambiado"}
  →  422  ·  ["El campo stock es obligatorio.",
              "El campo valorunitario es obligatorio."]

PATCH /api/producto/PRZZZ   {"nombre":"Cambiado"}
  →  200  ·  "Producto actualizado exitosamente."

GET   /api/producto/PRZZZ
  →  {"codigo":"PRZZZ","nombre":"Cambiado","stock":5,"valorunitario":1000.00}
```

**¿Por qué?** Porque son **dos clases distintas** las que validan:

| | Clase | Tiene `[Required]` | Y entonces |
|---|---|---|---|
| **PUT** | `ProductoReemplazo` | **sí**, en todos | Un cuerpo incompleto es un **error de forma** → 422 |
| **PATCH** | `ProductoActualizar` | **no**, en ninguno | Un cuerpo incompleto es **lo normal** → 200 |

> **Y `ProductoActualizar` no tiene `Codigo`.** Eso no es un olvido: significa
> que **un PATCH no puede cambiar la clave**. La clave va en la URL, y la URL
> identifica — no se negocia en el cuerpo.

> **La frase que hay que poder decir en voz alta:** el PUT dice *«así queda el
> recurso»* y el PATCH dice *«cámbiame esto»*. Por eso el PUT exige todo: lo que
> no se mande, se pierde. Quien no haya **visto** los dos 422 de arriba no sabe
> la diferencia; se la cree.

---

## 8 · Una línea con `await`: ¿qué hace el hilo en ese instante?

```csharp
var producto = await _repositorio.ObtenerPorCodigoAsync(codigo);
```

**El hilo se va. No espera.**

| Lo que mucha gente cree | Lo que pasa |
|---|---|
| El hilo se queda esperando la respuesta de la base | El hilo **se devuelve al grupo** y atiende **otra petición** |
| `async` hace el código más rápido | **No.** Esa consulta tarda lo mismo |
| `async` sirve para hacer dos cosas a la vez | Sirve para **no bloquear** mientras se espera algo de afuera |

> **Qué gana entonces, si no gana velocidad:** gana **capacidad**. Con código
> bloqueante, 100 personas pidiendo a la vez son 100 hilos dormidos esperando a
> SQL Server. Con `await`, unos pocos hilos atienden a todos, porque ninguno se
> queda quieto.
>
> **La imagen justa:** el mesero que toma el pedido y se va a atender otra mesa
> mientras la cocina trabaja, en vez de quedarse parado mirando la cocina.

> **Y de aquí sale el error de los desplegables**, que es el tropiezo 5 de la v2:
> cinco listas pedidas **en fila** con `await` una tras otra son cinco esperas
> **sumadas** — 50 segundos con la API apagada. `Task.WhenAll` las pide a la vez:
> 10. El `await` no paraleliza por sí solo; **pedir en fila es pedir en fila**.
> Ver [`PROGRAMACION_ASINCRONICA.md`](../conceptos/PROGRAMACION_ASINCRONICA.md).

---

## 9 · Sin una sola transacción escrita en C#, ¿qué garantiza ya la base?

**Casi todo, y es la respuesta que más sorprende.** En `api_facturas` no hay un
`BeginTransaction()`. Y aun así:

| Lo que no puede pasar | Quién lo impide |
|---|---|
| Una factura con la mitad de sus renglones | La **transacción dentro del procedimiento** |
| Un stock negativo | Un **disparador**: `THROW 50001` |
| Un total que no cuadre con sus renglones | Un **disparador**, que lo recalcula |
| Un renglón sin factura | La **clave foránea** `NOT NULL` |
| Una factura anulada dos veces | El **procedimiento**, que mira el estado |
| Un renglón que sobreviva a su factura | `ON DELETE CASCADE` |

> **Por qué está en la base y no en C#: porque la regla tiene que valer también
> para quien entre por SSMS.** Una validación que solo vive en la aplicación
> protege a quien pasa por la aplicación — y en la vida real siempre hay alguien
> que no pasa por ahí.

> **Y la prueba que de verdad lo demuestra** es la que más se olvida: emita una
> factura con **tres** productos donde el **segundo** no tenga stock. Si al final
> quedó una factura con un renglón, la transacción **no existe** aunque el
> procedimiento diga `BEGIN TRANSACTION`.
>
> **Lo que el curso pide decir en voz alta:** *«atómica»* no significa *«rápida»*
> ni *«pequeña»*. Significa **indivisible**: toda o ninguna. Ver
> [`PRINCIPIOS_ACID.md`](../conceptos/PRINCIPIOS_ACID.md).

---

## 10 · La prueba hueca

```csharp
[Fact]
public async Task CrearProducto_Funciona()
{
    var resultado = await _servicio.CrearAsync(new ProductoCrear { Codigo = "X" });
    Assert.NotNull(resultado);
}
```

**Esa prueba pasa siempre, y no comprueba nada.**

| Lo que parece | Lo que hace |
|---|---|
| «Comprueba que crear un producto funciona» | Comprueba que **el método devolvió algo** |
| Si se rompe la regla, falla | **No.** `NotNull` pasa con un objeto vacío, con un error envuelto, con cualquier cosa |

**Cómo se arregla:** afirmando **lo que la regla promete**, no que hubo
respuesta.

```csharp
[Fact]
public async Task CrearProducto_ConCodigoRepetido_Responde409()
{
    await _servicio.CrearAsync(new ProductoCrear { Codigo = "PR001", /* … */ });
    await Assert.ThrowsAsync<ConflictoExcepcion>(() =>
        _servicio.CrearAsync(new ProductoCrear { Codigo = "PR001", /* … */ }));
}
```

> **Cómo se reconoce una prueba hueca sin ser experto: tápele el cuerpo al
> método que prueba y pregúntese si la prueba se daría cuenta.** Si con un
> `return new object()` la prueba sigue pasando, la prueba no estaba probando el
> método: estaba probando que el método existe.

> **Y la versión incómoda de la misma idea:** una suite verde no dice que el
> sistema funciona. Dice que **ninguna de las pruebas escritas falló** — que es
> otra cosa, y depende enteramente de qué se escribió. Ver
> [`PRUEBAS_Y_CALIDAD_DE_PRUEBAS.md`](../conceptos/PRUEBAS_Y_CALIDAD_DE_PRUEBAS.md).

---

## 11 · Cómo se usa este documento, y cómo se repiten las mediciones

**Para quien sustenta:** no lo memorice. Abra el archivo del que habla y
señale la línea. Una respuesta que no puede apuntar a un archivo no cuenta,
aunque sea correcta.

**Para quien califica:** las diez preguntas se pueden hacer con el código en
pantalla. Y la pregunta de seguimiento es siempre la misma: **«¿y si lo
cambiamos?»** — porque entender algo es saber qué se rompe si se mueve.

```powershell
# 1 · El servicio no nombra HTTP (debe dar 0)
Select-String api_facturas\Servicios\*.cs -Pattern 'StatusCode|NotFound|IActionResult' |
  Measure-Object | Select-Object Count

# 2 · El controlador no abre conexiones (debe dar 0)
Select-String api_facturas\Controllers\*.cs -Pattern 'SqlConnection' |
  Measure-Object | Select-Object Count

# 3 · El 422 contra el 200, con el MISMO cuerpo
$t = (Invoke-RestMethod http://localhost:8035/api/sesion -Method Post `
      -ContentType 'application/json' `
      -Body '{"email":"admin@correo.com","contrasena":"admin123"}').token
$h = @{ Authorization = "Bearer $t" }
# (cree primero su propia fila, pruebe, y bórrela)
```

> **Y una regla de laboratorio que vale más que las diez preguntas: pruebe solo
> sobre filas que usted creó, y bórrelas al terminar.** Las seis facturas y los
> ocho productos sembrados son material de clase; si alguien los modifica, el
> siguiente que levante el proyecto encuentra otra cosa de la que dice
> [`DATOS_DE_PRUEBA.md`](DATOS_DE_PRUEBA.md).

| Qué | Dónde |
|---|---|
| Las capas y sus prohibiciones | [`ARQUITECTURA.md`](ARQUITECTURA.md) |
| Qué código devuelve cada error | [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) |
| Las reglas y quién las defiende | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Qué se hizo y cuándo | [`CRONOGRAMA.md`](CRONOGRAMA.md) |
