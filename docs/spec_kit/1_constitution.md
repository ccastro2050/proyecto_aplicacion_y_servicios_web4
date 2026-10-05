# Constitución del proyecto

> **Documento permanente.** Estas reglas rigen TODAS las versiones del
> curso. Cada versión tiene además su propia especificación en
> [versiones/](versiones/0_mapa_versiones.md); ante conflicto, la
> constitución gana.

---

## Artículo 1 — El curso es POR VERSIONES y la especificación manda

- El sistema se construye por **versiones incrementales** (v1, v2, …), cada
  una con su spec kit propio (documentos 2 a 8). Una versión está TERMINADA
  solo cuando pasa sus criterios de aceptación; entonces se hace commit,
  **tag** (`v1`, `v2`, …) y solo después se escribe la spec siguiente.
- **No se anticipa** (**YAGNI**, *You Aren't Gonna Need It* — "no lo vas a
  necesitar"): nada de fábricas multi-motor, capas "por si acaso" ni tablas
  de más antes de la versión que las pida. El código de cada versión solo
  puede nombrar lo que su spec nombra.
- El repositorio siempre contiene la **versión en curso, funcionando**.

## Artículo 2 — Stack: C# y ASP.NET Core, con el SQL a la vista

- Lenguaje **C#** sobre **ASP.NET Core** (.NET 10): controladores con
  atributos, inyección de dependencias del framework, `async/await` en todo
  el acceso a datos.
- **SIN ORM de entidades** (sin Entity Framework): el SQL se escribe
  A MANO, visible y SIEMPRE parametrizado (`@parametro` — nunca
  concatenar valores). El ejecutor es **Dapper** (micro-ejecutor sobre
  ADO.NET): mapea fila→objeto pero JAMÁS genera SQL por nosotros — si
  una consulta existe, está escrita en el repositorio.
- Paquetes externos permitidos en la v1 (y ninguno más sin que una spec
  lo pida): `Microsoft.Data.SqlClient` (el cliente oficial del motor),
  `Dapper` (el micro-ejecutor) y `Swashbuckle.AspNetCore` (Swagger).

## Artículo 3 — Arquitectura en capas con interfaces, desde el día 1

```
HTTP → Controller (valida el body contra la PETICIÓN del verbo → 422)
     → IServicioProducto      (interfaz — reglas de negocio)
     → IRepositorioProducto   (interfaz — el servicio no sabe qué motor hay)
     → RepositorioProducto<Motor>  (Dapper, SQL a mano parametrizado)
     → la base de datos
```

- El controlador no toca SQL; el servicio no conoce HTTP ni el motor; el
  repositorio no conoce HTTP. Los contratos son `interface` de C#.
- **Solo el ensamblador** (la sección de registro de dependencias en
  `Program.cs`) decide **qué implementación** se usa. Todo lo demás
  **recibe interfaces por constructor**, nunca instancia lo que necesita.

  > **Cuidado con leer esto como «está prohibido nombrar clases
  > concretas»: no lo es.** `ProductoController` y `ServicioProducto` son
  > clases concretas y se nombran sin problema — de cada una hay **una
  > sola**. Lo que la regla prohíbe es que una clase **se fabrique sola**
  > lo que necesita: ningún servicio escribe `new RepositorioX…()`, porque
  > ahí sí hay **dos alternativas** y elegir una lo casaría con un motor.
- El negocio comunica problemas con excepciones
  (`ArgumentException` → 400 · `NoEncontradoExcepcion` → 404) y el
  controlador las traduce a HTTP.

## Artículo 4 — Un solo comando

`docker compose up -d --build` deja TODO el sistema de la versión
funcionando, desde la primera versión. El código va montado como volumen y
corre con `dotnet watch`: guardar un `.cs` recompila y reinicia solo.

## Artículo 5 — La base de datos se diseña UNA VEZ, en la fase 0

La BD `bdfacturas` tiene **dos orígenes**, y conviene no confundirlos:

- **Las 12 tablas vienen del curso de Bases de Datos** — el modelo ya estaba
  hecho y se reusa tal cual. Son **89 líneas de código** del script.
- **Los 3 disparadores y los 16 procedimientos se escriben en ESTE proyecto**,
  en la fase 0, antes de la v1: son **las reglas del negocio**, y salen de la
  elicitación. Son **760 líneas de código** — el **89 %** del script. (El
  conteo excluye comentarios: así no cambia cuando se comenta mejor.)

Ese trabajo está en
[`docs/dominio/elicitacion/`](../dominio/elicitacion/1_PREGUNTAS.md),
[`REGLAS_DE_NEGOCIO.md`](../dominio/REGLAS_DE_NEGOCIO.md) y
[`DISENO_BD.md`](../dominio/DISENO_BD.md).

> **Las tablas no deciden nada.** Que el stock no quede negativo o que una
> factura no se anule dos veces vive en los disparadores y los procedimientos —
> y ninguno existía antes de este proyecto.

**Desde la v1 en adelante, la base VIENE DADA al código.** Se crea COMPLETA
—12 tablas, disparadores, procedimientos y datos de ejemplo— con los scripts
de `db/`: **se copian, no se generan**. Lo que crece por versiones es la API.
El código de cada versión solo puede nombrar las tablas que su spec le
permite.

> **Por qué se diseña una vez y no por versiones.** Porque un modelo de datos
> que cambia en cada entrega obliga a migrar los datos, a rehacer los
> disparadores y a reescribir los procedimientos — y nada de eso es lo que el
> curso enseña. **El modelo se piensa entero al principio, que es cuando se
> piensa un modelo**, y después se construye la API por tramos.
>
> **Y por eso ninguna IA genera el esquema.** Si una propone un `CREATE TABLE`,
> está rehaciendo un trabajo que ya se hizo — y lo va a hacer sin haber estado
> en la elicitación.

## Artículo 6 — Todo en español, comentado para principiantes

- Nombres, rutas, mensajes, comentarios y documentación: **en español**.
- El código lleva **comentarios línea a línea**: qué significa cada
  construcción del lenguaje y para qué sirve aquí. El repositorio es
  material de estudio, no solo software.

## Artículo 7 — Contratos exactos

Los endpoints, formatos y códigos de estado de cada versión están en su
`6_contracts.md` y se cumplen **al pie de la letra** — incluido el
contraste didáctico PUT (reemplazo completo → 422 si falta un campo) vs
PATCH (parcial → 200 con el mismo body).

## Artículo 8 — Convenciones fijas

| Cosa | Convención |
|---|---|
| Puertos del proyecto | API facturas **8035** · interfaz gráfica **8099** · PostgreSQL **15462** · SQL Server **11466** — ninguno se repite en 2026_2, y el registro es [`PUERTOS.md`](../../../PUERTOS.md) |
| Rutas | `/` (diagnóstico) · `/swagger` (documentación interactiva) · `/api/producto` (v1) |
| Nombres | PascalCase en español; interfaces con prefijo `I`; carpetas `Controllers/ Modelos/ Peticiones/ Servicios/ Repositorios/ Excepciones/ pruebas/` (`Modelos/` = clases entidad; `Peticiones/` = el body de cada verbo) |
| Sobre de respuesta | Lecturas: `{tabla, limite, total, datos}` · Errores: `{estado, mensaje, detalle}` (+ `errores:[…]` en el 422) |
| Errores | Body inválido (la petición) → **422** · `ArgumentException` → **400** · `NoEncontradoExcepcion` → **404** · `SqlException` y demás → **500** |
| Credenciales (didácticas) | BD: `sa` / `Paradigmas123!` · base `bdfacturas_sqlserver_local` |
