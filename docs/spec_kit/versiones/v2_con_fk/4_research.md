# Investigación y decisiones — Versión 2: las tablas con clave foránea

> **Qué es este documento:** las decisiones que se tomaron y **las
> alternativas que se descartaron, con su razón**. No es relleno: su utilidad
> aparece el día que alguien pregunte «¿y por qué no hicieron…?» — y la
> respuesta esté escrita en vez de olvidada.

---

## D1 — La clave foránea inexistente: ¿422 o 409?

| Opción | Argumento |
|---|---|
| **422** | «El dato está mal, y el 422 es para datos malos» |
| **409** ✅ | **El dato NO está mal: tiene la forma correcta.** `"NOEXISTE"` es un texto de la longitud permitida. Lo que se rompe es el **estado** de la base: esa fila no está |

**Lo que decide:** el 422 se reserva para lo que la **petición** puede
rechazar sin consultar nada —un campo que falta, un número negativo, un tipo
equivocado—. Saber si `P001` existe **requiere ir a la base**, y eso ya es
estado.

> **Y es la razón por la que el 409 aparece en la v2 y no antes:** es la
> primera versión en la que una fila **depende de otra**. En la v1 no había
> nada que violar.

## D2 — `fkcodempresa` sin empresa: ¿cadena vacía o `null`?

**`null`.** Y conviene decir por qué `""` es tentador y está mal: un
`<select>` de HTML no sabe de `null` —su opción vacía vale `""`— así que lo
más fácil es dejar que viaje tal cual.

```
""     ->  un codigo de empresa de cero caracteres, que NO existe  ->  409
null   ->  «este cliente no tiene empresa»                         ->  200
```

**La conversión se hace en la interfaz gráfica, justo antes de enviar.** Y
queda escrita en el código, porque es el tipo de detalle que se olvida y
produce un 409 que nadie entiende.

## D3 — `factura`: ¿SQL en el repositorio, o procedimientos?

| Opción | Argumento |
|---|---|
| **SQL en C#** | Más fácil de leer para quien no sabe plpgsql. Todo el código en un solo lenguaje |
| **Procedimientos** ✅ | **La base ya los trae** —seis, escritos y probados— y la transacción es suya |

**Lo que decide, y no es la pereza:** el detalle de una factura y su
encabezado tienen que entrar **juntos o no entrar**. Con `INSERT` desde C#
habría que manejar la transacción en la API, y el cálculo del total viviría en
dos sitios —el disparador y el servicio—. **El día que la regla del total
cambie, cambia en uno.**

> **Qué se pierde, para no pintarlo bonito:** el estudiante tiene que leer
> plpgsql, que no se enseñó. Se compensa con el `5_data_model`, que explica
> qué hace cada procedimiento, y con que el repositorio **solo** llame: no hay
> que escribir plpgsql, hay que leerlo.

## D4 — La traducción de los errores de plpgsql: por patrón del mensaje

Los `RAISE EXCEPTION` de plpgsql **no traen número**: todos llegan con
`el número de error P0001`. Así que para distinguir «no existe» de «ya está anulada» hay
que mirar **el texto**.

```csharp
catch (SqlException e) when (e.SqlState == "P0001"
                                  && e.MessageText.Contains("no existe"))
    => throw new NoEncontradoExcepcion(e.MessageText);   // 404
```

| Opción | Argumento |
|---|---|
| **Por patrón del mensaje** ✅ | Es lo que el motor da. Funciona hoy, sin tocar la base |
| **Un código propio en el JSON del procedimiento** | **Mejor**, y está descartado solo por alcance |

> **Está escrito aquí a propósito, porque es frágil y se nota:** si alguien
> traduce el mensaje del procedimiento al inglés, la API deja de responder 404
> y responde 500 — **sin que nada falle al compilar**. La alternativa buena
> —que cada procedimiento devuelva `{"error":"no_encontrado"}`— implica tocar
> la base, y la base **se entrega dada**. Queda como deuda **conocida**, que es
> distinto de un descuido.

## D5 — Un recurso `usuario-con-roles` además de los otros dos

Ya existen `api/usuario` y `api/rol-usuario`. ¿Por qué un tercero?

| Opción | Argumento |
|---|---|
| **Dos llamadas desde el front** | Ningún endpoint nuevo. «Crear el usuario y después asignarle los roles» |
| **Un recurso nuevo** ✅ | **Una** transacción |

**Lo que decide:** con dos llamadas, un fallo en la segunda deja **un usuario
sin ningún rol** — que no puede hacer nada, y que nadie sabe que está ahí
hasta que alguien se queja. La base **ya trae** `crear_usuario_con_roles` para
que sea una.

> **Y los tres recursos se quedan**, que es la parte que suele incomodar:
> administran cosas distintas. El primero la tabla sola, el segundo la puente
> pareja a pareja, el tercero el conjunto. **La interfaz gráfica usa el
> tercero.** Quitar los otros dos no ahorraría nada y le quitaría al
> administrador la operación fina.

## D6 — El total de la factura: ¿lo manda el front?

**No.** Lo calcula el disparador, y el front lo **muestra** sin enviarlo.

| Opción | Argumento |
|---|---|
| **El front lo manda** | Un cálculo menos en la base. Y el front ya lo tiene, porque lo está mostrando |
| **Lo calcula la base** ✅ | **Una sola fuente de verdad** |

**Lo que decide:** si los dos lo calculan, un día no van a coincidir —por un
redondeo, por un precio que cambió entre que se cargó el desplegable y se
emitió— y **va a ganar el número que nadie revisó**. El que manda es el de la
base, así que es el único que se guarda.

> **Lo que el front muestra se llama «total estimado» en la interfaz**, y no es
> un adorno: es decirle a la persona que ese número es una ayuda, no el
> documento.

## D7 — El detalle: ¿renglón por renglón, o todo junto?

**Todo junto, en un solo envío.** Y es el criterio que no se puede simular.

| Opción | Argumento |
|---|---|
| **Un POST por renglón** | Más simple de programar: cada botón hace su llamada |
| **Un solo POST al final** ✅ | La transacción del procedimiento **sirve para algo** |

**Lo que decide:** con un POST por renglón, agregar tres y quitar uno deja
**tres** en la base —el borrado del tercero habría que programarlo también— y
un fallo a mitad deja una factura incompleta que nadie pidió.

> **Cómo se comprueba, y es el criterio 13:** agregue tres renglones, quite
> uno, emita. **Tienen que llegar dos.**

## D8 — Las interfaces de las tablas puente: ¿se llaman como la tabla?

**No**, y por dos razones distintas.

| | |
|---|---|
| **La técnica** | El nombre de un archivo `.razor` **es el nombre de una clase**. `RolUsuario.razor` genera una clase `RolUsuario` que **tapa el modelo** del mismo nombre, y el proyecto **no compila**. Pasó |
| **La humana** | El menú le habla a una persona. «Roles por usuario» y «Permisos por rol» dicen qué hay ahí; `rol_usuario` y `rutarol` son nombres de tabla |

> **Y de ahí salió una corrección en el auditor:** adivinaba el recurso de cada
> interfaz **por el nombre del archivo**, lo que forzaba a llamarlas como la
> tabla. Ahora lee el servicio que la interfaz inyecta
> —`@inject ServicioRolUsuario Servicio`—, que es un dato y no una suposición.

## D9 — Lo que NO se investigó, y por qué

| | |
|---|---|
| **Un ORM** | La constitución lo prohíbe: la idea es **ver el SQL**. No es una decisión de esta versión |
| **Paginación de verdad** (`offset`, cursores) | El `?limite` alcanza para los volúmenes del curso. Agregarla ahora sería anticipación |
| **Bloqueo optimista** (`ETag`, versión de fila) | Dos personas editando la misma fila es un problema real que **el curso no plantea** |
| **Un segundo motor** | Es la **v5**, y su valor depende de que la interfaz del repositorio ya esté probada — que es lo que esta versión hace |
