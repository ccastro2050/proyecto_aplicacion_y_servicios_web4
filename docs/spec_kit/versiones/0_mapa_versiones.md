# Mapa de versiones del curso

> La ruta completa del proyecto. Cada versión se especifica SOLO cuando la
> anterior está cerrada (commit + tag). Este mapa da la dirección; las
> specs de cada versión dan el detalle.

| Versión | Qué agrega | Estado |
|---|---|---|
| **v1** | `api_facturas` (C#/ASP.NET Core): CRUD completo de `producto` contra **SQL Server** — capas + interfaces + peticiones por verbo | **Cerrada** — tag `v1` ([spec](v1_producto_sqlserver/2_spec.md)) |
| **v2** | `persona` (el molde de la v1 replicado) y `factura` **maestro-detalle** aprovechando los triggers y SPs de la BD (crear, consultar, listar, anular) | **Cerrada** — tag `v2` ([spec](v2_persona_factura/2_spec.md)) |
| **v3** | **El resto de las entidades** contra SQL Server: empresa, cliente, vendedor, usuario (contraseña con **BCrypt** + verificar-contrasena), rol, ruta y las tablas puente rol_usuario y rutarol — TODA la bdfacturas cubierta con UN motor antes de cambiar de motor | **Cerrada** — tag `v3` ([spec](v3_resto_entidades/2_spec.md)) |
| **v4** | Segundo motor (**PostgreSQL**): la MISMA bdfacturas en otro dialecto, los 11 repositorios Npgsql, la **fábrica de repositorios** (el motor se decide en UN punto) y el interruptor `MOTOR_BD` — cero endpoints nuevos: la demostración de que las capas eran verdad | **Cerrada** — tag `v4` ([spec](v4_postgresql/2_spec.md)) |
| v5 | Tercer motor (**MariaDB**) + compose completo | Sin especificar |
| v6 | **API genérica de plataforma**: CRUD sobre CUALQUIER tabla (`/api/{tabla}` con filas como diccionario columna→valor descubierto en runtime), selección de motor por configuración, **autenticación JWT**, encriptación BCrypt de campos, consultas SELECT parametrizadas, ejecución de **procedimientos almacenados** y exploración de estructuras de la BD | Sin especificar |
| v7 | Frontend **Blazor Server**: CRUD de las 12 entidades (una página por tabla), **login y control de acceso con JWT**, selects de llaves foráneas, y la **facturación maestro-detalle** usando los procedimientos almacenados | Sin especificar |

> **El destino del curso:** la v6 es el punto de llegada de la API — de la
> API específica de la v1 (un endpoint por entidad, peticiones con
> validación) se evoluciona hasta una API **genérica** que opera cualquier
> tabla sin conocerla de antemano. Y la v7 le pone encima un front Blazor
> completo. Cada versión intermedia es un paso deliberado de ese camino.

**Reglas del mapa** (constitución, Artículo 1): no se anticipa nada de una
versión futura; una versión cerrada no se reabre (los ajustes van en la
siguiente); el repositorio siempre muestra la versión en curso funcionando.
