# Arquitectura — las capas y sus prohibiciones

> **Qué es este documento.** Las tres capas de la API, **qué tiene prohibido
> hacer cada una**, dónde vive cada regla, y lo que esta arquitectura **no
> tiene**. Las prohibiciones son la mitad del documento: una arquitectura se
> define tanto por lo que permite como por lo que impide.
>
> **A qué plantilla alimenta:** el `1_constitution`.
>
> **Por qué está separado de los procesos.** Porque son **dos lectores**: la
> constitución necesita las capas y las prohibiciones; el `3_plan` necesita los
> procesos y el despliegue, que están en
> [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md). Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Los conceptos detrás:**
> [`SOLID_CAPAS_PATRONES.md`](../conceptos/SOLID_CAPAS_PATRONES.md) y
> [`FLUJO_DE_UNA_PETICION.md`](../conceptos/FLUJO_DE_UNA_PETICION.md).
>
> **Material académico simulado**, pero el reparto es real y se puede comprobar
> abriendo las carpetas.
>
> Versión 2.0 · 22 de octubre de 2026.

![Las tres capas y sus interfaces](../conceptos/img/las_tres_capas.svg)

---

## 1. Las tres capas de la API

| Capa | Qué hace | Qué tiene PROHIBIDO |
|---|---|---|
| **Controlador** | Lee la petición, valida la **forma**, traduce excepciones a códigos HTTP | Escribir SQL · tomar decisiones de negocio |
| **Servicio** | Las reglas: qué se puede y qué no | Saber qué es un 404 · saber qué motor hay debajo |
| **Repositorio** | El SQL y la llamada a los procedimientos | Decidir nada |

Y entre cada par, **una interfaz**:

```
FacturaController  →  IServicioFactura   →  IRepositorioFactura
                          ↑                        ↑
                   ServicioFactura        RepositorioFacturaSqlServer
                                          RepositorioFacturaPostgres
```

> **Lo que se usa es siempre la interfaz, nunca la clase.** Por eso el servicio
> no sabe si detrás hay SQL Server, PostgreSQL o un falso en memoria — y por eso
> se puede probar sin levantar una base.

### El inventario, por recurso

Nueve recursos con sus tres capas, más los dos puentes y las vistas agregadas.
Se puede contar:

| Carpeta | Cuántos | |
|---|---|---|
| `Controllers/` | **15** | uno por recurso, más consultas, permisos y sesión |
| `Servicios/` | **14** + sus 14 interfaces | |
| `Repositorios/` | **28** implementaciones + 14 interfaces | **dos por recurso**: SqlServer y Postgres |

Esos 28 contra 14 son la arquitectura en un número: cada contrato tiene dos
implementaciones, y arriba nadie sabe cuál está puesta.

---

## 2. La regla que no se negocia

> **En el front no puede haber un solo `SqlConnection`.** Si el front puede
> llegar a la base de datos, la separación es un dibujo y no una arquitectura.
>
> **Y se comprueba, no se promete:** apague la API con la base de datos
> encendida y abra el front. Tiene que seguir en pie, con su menú y un aviso de
> que el servicio no está disponible, **y sin una sola fila**. Si sigue
> mostrando datos, alguien abrió una conexión que no debía.

---

## 3. Dónde vive cada regla, y por qué ahí

| Regla | Dónde | Por qué no más arriba |
|---|---|---|
| El stock no queda negativo | **disparador** | Porque también vale para quien entre por SSMS |
| El total es la suma de subtotales | **disparador** | Igual, y además así no puede desactualizarse |
| Una factura tiene al menos un renglón | **procedimiento** | Va en la misma transacción que la inserta |
| Una factura no se anula dos veces | **procedimiento** | Es una condición sobre el estado, y el estado está en la base de datos |
| El `{}` del PATCH no actualiza nada | **servicio** | Es una decisión de negocio, no de la base de datos |
| Falta el campo `nombre` | **la petición** (anotaciones) | Es forma, y la forma se rechaza antes de entrar |
| ¿Tiene permiso? | **`[ExigePermiso]`**, antes del controlador | Para que ningún método pueda olvidarse de preguntarlo |

> **El criterio es simple:** cuanto más abajo vive una regla, a más gente
> protege. Una validación que solo está en C# protege a quien pasa por la API.

---

## 4. La base de datos es parte del sistema, y la que más código propio tiene

De las **849 líneas de código** de `db/bdfacturas.sql` —sin contar
comentarios—, **760 se escribieron en este proyecto**: los 3 disparadores y los
16 procedimientos. Las 12 tablas que vinieron del curso de Bases de Datos son
**89**.

> **El conteo es de código a propósito.** El archivo entero tiene más de dos
> mil líneas porque está comentado; contar comentarios haría que la cifra
> cambiara cada vez que alguien explica mejor algo, y entonces no mediría nada.

Y no es un dato de contabilidad: **las tablas no deciden nada.** Que el stock
no quede negativo, que el total cuadre con sus renglones y que una factura no
se anule dos veces vive **ahí**, no en C#. Ver
[`FUENTES.md`](FUENTES.md) §0.

---

## 5. La fábrica, que es donde se decide el motor

```
Program.cs  →  IFabricaRepositorios  →  FabricaSqlServer
                                     →  FabricaPostgres
```

Un interruptor —la variable `MOTOR_BD`— elige cuál. **Y es el único sitio del
sistema que decide CUÁL IMPLEMENTACIÓN DE REPOSITORIO se usa.**

> **No confundirlo con «no se nombran clases concretas».** `ProductoController` y
> `ServicioProducto` lo son, y se nombran sin problema: de cada uno hay **uno
> solo**. La regla aplica donde hay **dos alternativas** — los repositorios — y
> por eso son los únicos que pasan por la fábrica.

> **Ésa es la prueba del principio abierto/cerrado, y está MEDIDA con un
> `diff`:** agregar PostgreSQL fue **18 archivos y 1 373 líneas**, de los cuales
> 12 son repositorios nuevos. Y sobre `Controllers/` y `Servicios/` el `diff`
> sale **vacío**.
>
> ```powershell
> git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios
> ```
>
> **Si hubiera que tocarlos, las capas estaban mal hechas.** Nótese que el tag
> es `v4` y no `v5`: los tags son del mapa de versiones viejo, donde el motor era
> la v4. Hoy ese trabajo es la v5.

---

## 6. Lo que NO tiene esta arquitectura

Decirlo evita que alguien lo busque:

| No hay | Y en su lugar |
|---|---|
| Entity Framework ni ningún ORM | SQL escrito a mano y **Dapper** como micro-ejecutor |
| Caché | Cada petición va a la base de datos |
| Colas, eventos, mensajería | Todo es síncrono dentro de la petición |
| Microservicios | Tres procesos, y ya — ver [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md) §2 |
| Repositorio genérico `Repositorio<T>` | Uno por recurso, a propósito — ver abajo |

> **Por qué no un `Repositorio<T>` genérico ni un `/api/{tabla}`.** Porque el
> contrato quedaría en blanco: Swagger no diría qué recursos hay, los permisos
> no se podrían dar por recurso, y cada cambio tocaría las doce tablas.

> **Ojo con «todo es síncrono dentro de la petición»:** significa que **no hay
> colas, eventos ni mensajería** — no que el acceso a datos sea bloqueante. El
> acceso a datos es **asincrónico** (`async`/`await`), y eso es otra cosa.

---

## 7. Comprobarlo

| Qué se afirma | Cómo se comprueba |
|---|---|
| El front no toca la base de datos | Apague `api-facturas` y abra el front: en pie y sin filas |
| Las capas no se saltan | En `Controllers/` no hay ni un `SqlConnection` —**0 archivos**—, y en `Repositorios/` no hay ni un `StatusCode` —**0**—. La palabra `Sql` sí aparece dos veces en los controladores, pero **en comentarios**, explicando que `SqlException` se traduce a 500 |
| Cambiar de motor no toca arriba | `git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios`: **vacío** |
| Las reglas están abajo | Intente dejar el stock negativo con un `INSERT` directo en SSMS |
