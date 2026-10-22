# Los errores del motor — cómo se traducen a HTTP

> **Qué es este documento.** Cómo se convierte lo que dice la base de datos en
> un código de estado, y el caso del login, que parece un error y es una
> decisión.
>
> **A qué plantillas alimenta:** el `6_contracts` de la v2 en adelante, y el
> `7_quickstart`.
>
> **Por qué está separado de la tabla de códigos.** Porque son lectores
> distintos: la tabla de códigos la necesitan tres plantillas, incluida la
> constitución; esto solo lo necesitan los contratos desde que aparecen las
> llaves foráneas. La tabla está en
> [`CODIGOS_DE_ESTADO.md`](CODIGOS_DE_ESTADO.md). Ver
> [`DOMINIO_PARA_GENERAR.md`](../../DOMINIO_PARA_GENERAR.md).
>
> **Material académico simulado**, pero las traducciones son las que el código
> hace de verdad.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. El caso del login, que parece un error y es una decisión

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

## 2. Cómo se traduce un error del motor

La base de datos habla en números. El repositorio los traduce, y **nadie por
encima de él conoce `SqlException`**.

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

## 3. Por qué la traducción vive en el repositorio y no más arriba

| Si viviera en… | Qué pasaría |
|---|---|
| el **controlador** | Tendría que conocer `SqlException` y `NpgsqlException`: la capa de HTTP sabiendo de motores |
| el **servicio** | Igual, y además el servicio dejaría de poder probarse con un repositorio falso |
| el **repositorio** ✅ | Es el único que ya es específico del motor. Arriba suben excepciones del dominio, no del motor |

> **La prueba de que está bien puesta:** en `Repositorios/` no hay ni un
> `StatusCode`, y en `Controllers/` no hay ni un `SqlConnection`. Las dos cosas
> se cuentan, y las dos dan cero.

---

| Qué | Dónde |
|---|---|
| La tabla de códigos y la forma del cuerpo | [`CODIGOS_DE_ESTADO.md`](CODIGOS_DE_ESTADO.md) |
| Las reglas que los disparadores defienden | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Dónde vive cada regla, y por qué ahí | [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §3 |
