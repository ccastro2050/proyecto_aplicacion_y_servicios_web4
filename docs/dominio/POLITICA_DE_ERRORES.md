# Política de errores — Facturación (`bdfacturas`)

> **Qué es este archivo.** Qué código de estado devuelve el sistema ante cada
> clase de fallo, qué forma tiene el cuerpo, y **quién decide cada uno**. Es una
> **política**: rige en todas las versiones y no se renegocia en cada
> `6_contracts.md`.
>
> **Por qué existe.** Sin ella, cada versión inventa sus códigos, y la v1 y la v2
> del mismo sistema acaban respondiendo distinto al mismo error. Entonces el
> cliente no puede escribir una sola rutina de manejo.
>
> **Material académico simulado**, pero los códigos son los que la API devuelve
> de verdad: cada fila de este documento se puede comprobar con una petición.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. La tabla completa

| Código | Qué significa | Quién lo decide | Ejemplo real |
|---|---|---|---|
| **200** | Salió bien | el controlador | `GET /api/producto` |
| **204** | Salió bien y **no hay contenido**: la tabla está vacía | el controlador | `GET /api/empresa` sin empresas |
| **400** | Los **parámetros** no sirven — una regla de negocio sobre la forma | el **servicio**, con `ArgumentException` | `PATCH /api/producto/PR001` con body `{}` |
| **401** | **No sé quién es usted** | `[Authorize]`, antes del controlador | sin token, o con uno vencido |
| **403** | **Sé quién es, y no puede** | `[ExigePermiso]`, antes del controlador | token bueno, permiso ausente |
| **404** | **Eso no existe** | el servicio, con `NoEncontradoExcepcion` | `GET /api/factura/9999` |
| **405** | La ruta existe, pero **no con ese verbo** | el enrutador | `PUT /api/factura/3` |
| **409** | La petición está bien y **choca con el estado** | el servicio, con `ConflictoExcepcion` | anular una factura ya anulada |
| **422** | El **body no tiene la forma** pedida | ASP.NET, antes del controlador | `POST /api/producto` sin `nombre` |
| **500** | Algo se rompió, o la base de datos rechazó | el `catch` final | stock insuficiente |

---

## 2. Las tres parejas que se confunden, y cómo distinguirlas

### 401 contra 403

| | 401 | 403 |
|---|---|---|
| Qué dice | «No sé quién es usted» | «Sé quién es, y no puede» |
| Causa | Sin token, alterado o vencido | Token válido, permiso ausente |
| **Qué hace la persona** | Identificarse | Pedírselo al administrador |

> Confundirlos manda a alguien a arreglar lo que no está roto: si responde 401
> cuando lo que falta es un permiso, la persona va a volver a escribir su
> contraseña —y va a entrar— y seguirá sin poder.

### 422 contra 400

| | 422 | 400 |
|---|---|---|
| Qué falló | La **forma**: falta un campo, o es del tipo equivocado | El **sentido**: la forma está bien y aun así no se puede |
| Quién lo decide | ASP.NET, con las anotaciones de la petición | El servicio |
| Ejemplo | `POST /api/producto` sin `nombre` | `PATCH` con `{}` — es un JSON válido, pero no manda nada que cambiar |

> **El `{}` del PATCH es el caso que lo enseña.** Es un body perfectamente
> formado. Lo que pasa es que no pide ningún cambio, y eso es una regla de
> negocio: «actualizar nada» no es actualizar.

### 409 contra 500

| | 409 | 500 |
|---|---|---|
| Qué pasó | Choca con el **estado** del recurso, y el sistema lo **esperaba** | La base de datos rechazó y nadie lo tradujo |
| Ejemplo | Anular una factura ya anulada | Stock insuficiente · llave duplicada |

> **Y aquí hay una decisión deliberada que conviene no «arreglar»:** en la v1 una
> llave repetida responde **500**, no 409. Está escrito en su `2_spec.md`: *«la
> llave la defiende la BD, no la API. Convertirlo en 409 sería lógica de negocio
> que esta versión no pide.»* El 409 llega en la v2, cuando aparecen las llaves
> foráneas y los rechazos se vuelven cotidianos.

---

## 3. La forma del cuerpo

**Siempre JSON**, también en el error. Nunca una página de error de ASP.NET.

### El error corriente

```json
{ "estado": 404,
  "mensaje": "Factura no encontrada.",
  "detalle": "Factura 9999 no existe" }
```

| Campo | Para quién |
|---|---|
| `estado` | Redundante con el código HTTP, **a propósito**: quien lee un registro de texto no tiene el código a la vista |
| `mensaje` | Para la persona. En castellano, sin jerga |
| `detalle` | Para quien programa. Puede traer el mensaje del motor |

### El 422, que trae la lista

```json
{ "estado": 422,
  "mensaje": "Datos inválidos.",
  "errores": [ "El campo stock es obligatorio.",
               "El campo codigo es obligatorio.",
               "El campo nombre es obligatorio.",
               "El campo valorunitario es obligatorio." ] }
```

> **Los errores de forma llegan TODOS a la vez.** Devolver el primero y callar
> los otros obliga a la persona a corregir de a uno y volver a enviar: tres
> viajes para tres errores que se sabían desde el principio.

### El 403, que dice qué permiso faltó

```json
{ "estado": 403,
  "mensaje": "Su rol no tiene permiso para esta operacion.",
  "ruta": "interfaz.facturas" }
```

---

## 4. Lo que el `detalle` NO puede traer

| Nunca | Por qué |
|---|---|
| Una contraseña o un hash | Ver [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) RN-18 |
| La cadena de conexión | Trae la clave de la base de datos |
| El *stack trace* completo | Le dice a un desconocido qué biblioteca y qué versión corren |
| Si un correo **existe** | El login responde lo mismo para «no existe» y «clave mala» — ver §5 |

---

## 5. El caso del login, que parece un error y es una decisión

`POST /api/sesion` responde **401 con el mismo mensaje** en los dos casos:

```
el correo no existe        → 401 "El correo o la contrasena no son correctos."
la contraseña está mal     → 401 "El correo o la contrasena no son correctos."
```

> **No es pereza: es el criterio 2 de la v3.** Si respondiera 404 cuando el
> correo no existe, cualquiera podría averiguar **qué correos están
> registrados** probando uno por uno. El precio es que quien se equivocó de
> contraseña no sabe cuál de las dos cosas falló — y ese precio se paga a
> propósito.
>
> Por la misma razón, `POST /api/usuario/verificar-contrasena` está **escrito y
> apagado** en el código: era un oráculo que deshacía este cuidado.

---

## 6. Cómo se traduce un error del motor

La base de datos habla en números. El repositorio los traduce, y **nadie por encima de él
conoce `SqlException`**.

| El motor dice | Se traduce a | Dónde |
|---|---|---|
| `THROW 50001` — stock insuficiente | **500**, con el mensaje tal cual | el disparador lo redacta con el número exacto |
| `THROW 50003` / `50010` + «no existe» | **404** | `RepositorioFacturaSqlServer` |
| `THROW 50010` + «anulada» | **409** | el mismo |
| Violación de llave primaria | **500** en la v1, **409** desde la v2 | `ErroresSqlServer` |

> **SQL Server numera sus errores y PostgreSQL usa `SQLSTATE`.** Por eso cada
> repositorio tiene su traductor, y por eso el de PostgreSQL filtra por un texto
> distinto. Es la diferencia de dialecto que la v5 existe para mostrar.

---

## 7. Comprobarlo

Cada fila de §1 se puede verificar con una petición. Las que no necesitan sesión:

```powershell
# 405 — la ruta existe, el verbo no
Invoke-RestMethod -Method Put -Uri http://localhost:8035/api/factura/3

# 401 — sin token
Invoke-RestMethod -Uri http://localhost:8035/api/producto
```

El resto están en el `7_quickstart.md` de cada versión, que es donde se corren
de verdad.
