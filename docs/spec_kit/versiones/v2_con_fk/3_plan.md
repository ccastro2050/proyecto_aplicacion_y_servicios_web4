# Plan técnico — Versión 2: las tablas con clave foránea

> | | |
> |---|---|
> | **Lo que hay que construir** | [2_spec.md](2_spec.md) |
> | **Los formatos exactos** | [6_contracts.md](6_contracts.md) |
> | **El orden** | [8_tasks.md](8_tasks.md) |

---

## 1. La pila, que no cambia

| | |
|---|---|
| **Lenguaje** | C# sobre ASP.NET Core (**.NET 10**) |
| **Base** | SQL Server 2022 |
| **Acceso a datos** | **Dapper** — SQL a la vista, sin ORM |
| **Interfaz gráfica** | **Blazor Server**, en su propio proyecto y su propio contenedor |
| **Todo** | Docker Compose, con un solo `up` |

> **No se cambia nada de la pila en la v2.** Si en el plan apareciera un
> paquete nuevo, un ORM o un framework de front, sería anticipación — y la
> constitución la prohíbe.

## 2. Las capas, que tampoco cambian

```
   HTTP  →  CONTROLADOR  →  SERVICIO  →  REPOSITORIO  →  SQL Server
            sabe de HTTP    sabe de      sabe de SQL
            y de nada más   reglas       y de nada más
```

| Capa | Qué sabe | Qué NO sabe |
|---|---|---|
| **Controlador** | Verbos, códigos de estado, el sobre de la respuesta | Nada de SQL |
| **Servicio** | Las reglas del dominio | **Nada de HTTP.** No conoce `IActionResult` ni los códigos |
| **Repositorio** | El SQL, o el `CALL` del procedimiento | Nada de reglas |

**Y la regla que la v2 pone a prueba:** el servicio **no puede** devolver un
código HTTP. Lanza una excepción del dominio —`NoEncontradoExcepcion`,
`ConflictoExcepcion`— y **el controlador** la traduce. Si el servicio
devolviera un 409, la capa de reglas sabría de HTTP y la frontera estaría
rota.

## 3. Lo que la v2 agrega, archivo por archivo

### 3.1 Los dos recursos calcados — `cliente` y `vendedor`

Rebanada vertical completa, idéntica en forma a la de la v1:

```
Modelos/Cliente.cs                    Modelos/Vendedor.cs
Peticiones/ClienteCrear.cs            Peticiones/VendedorCrear.cs
Peticiones/ClienteReemplazo.cs        Peticiones/VendedorReemplazo.cs
Peticiones/ClienteActualizar.cs       Peticiones/VendedorActualizar.cs
Repositorios/IRepositorioCliente.cs   Repositorios/IRepositorioVendedor.cs
Repositorios/RepositorioClienteSqlServer.cs   …VendedorSqlServer.cs
Servicios/IServicioCliente.cs         Servicios/IServicioVendedor.cs
Servicios/ServicioCliente.cs          Servicios/ServicioVendedor.cs
Controllers/ClienteController.cs      Controllers/VendedorController.cs
```

> **Tres peticiones por recurso y no una, y es la lección que la v1 ya dejó:**
> `Crear` tiene todo obligatorio, `Reemplazo` también —de ahí el **422** del
> PUT— y `Actualizar` todo opcional —de ahí el **200** del PATCH con el mismo
> cuerpo—. **Una sola clase para los tres verbos haría imposible esa
> diferencia.**

**Lo único nuevo de verdad:** traducir el error de clave foránea del motor.

```
SqlException con el error 547  (foreign_key_violation)
SqlException con los errores 2627 y 2601  (unique_violation)
        ↓  el repositorio
ConflictoExcepcion
        ↓  el controlador
409
```

### 3.2 `factura` — el repositorio que no escribe SQL de tablas

```
Modelos/Factura.cs                    el maestro, con la lista adentro
Modelos/ProductoDeFactura.cs          un renglón del detalle
Peticiones/FacturaCrear.cs            { fkidcliente, fkidvendedor, productos[] }
Repositorios/IRepositorioFactura.cs
Repositorios/RepositorioFacturaSqlServer.cs   CUATRO `CALL`, cero SELECT de tablas
Servicios/IServicioFactura.cs
Servicios/ServicioFactura.cs
Controllers/FacturaController.cs      4 endpoints, no 6
```

**Cómo se llama un procedimiento con INOUT, que es lo que hay que aprender:**

```csharp
// El CALL devuelve UNA fila cuya unica columna es el INOUT p_resultado.
// ExecuteScalarAsync la lee como texto JSON:
var resultado = await conexion.ExecuteScalarAsync<string?>(
    "CALL sp_consultar_factura_y_productosporfactura(@p_numero, NULL)",
    new { p_numero = numero });
```

| | |
|---|---|
| **El último `NULL`** | Es el parámetro `INOUT`. Hay que pasarlo, aunque no se use |
| **El `::json`** | Al crear, el detalle viaja como texto y hay que **tiparlo**: `@p_productos::json`. Sin eso, el procedimiento recibe una cadena |
| **La traducción del error** | Los `RAISE EXCEPTION` de plpgsql **no traen número**: todos llegan con `el número de error P0001`. Se distinguen **por el patrón del mensaje** |

> **Por qué por patrón del mensaje, que suena frágil:** porque es lo que el
> motor da. La alternativa sería que cada procedimiento devolviera un código
> propio en su JSON — mejor, y es una mejora que **no** es de esta versión.
> Queda escrito en [4_research.md](4_research.md) para que la decisión esté a
> la vista y no parezca un descuido.

### 3.3 Las dos puentes — `rol_usuario` y `rutarol`

```
Modelos/RolUsuario.cs                 dos claves, ningún campo
Modelos/RutaRol.cs
Peticiones/RolUsuarioCrear.cs         sin Reemplazo ni Actualizar: NO HAY PUT NI PATCH
Peticiones/RutaRolCrear.cs
… repositorio, servicio y controlador de cada una
```

**Y las rutas, que hay que mirar y no adivinar:**

| Tabla | Ruta | |
|---|---|---|
| `rol_usuario` | `api/rol-usuario` | **Con guion** |
| `rutarol` | `api/rutarol` | **Sin guion**, porque la tabla no lleva subrayado |

> Son dos convenciones distintas —**URL** y **SQL**— y asumir una por la otra
> da **404**. Se lee el `[Route]` del controlador.

### 3.4 `usuario-con-roles` — el recurso que no es una tabla

```
Modelos/UsuarioConRoles.cs            el usuario con sus roles anidados
Peticiones/UsuarioConRolesCrear.cs       { email, contrasena, roles:[2,3] }
Peticiones/UsuarioConRolesActualizar.cs  { contrasena?, roles:[1] }
Repositorios/…UsuarioConRolesSqlServer.cs  CINCO `CALL`
Servicios/ServicioUsuarioConRoles.cs      ← aquí está su único trabajo propio
Controllers/UsuarioConRolesController.cs
```

**El trabajo del servicio, que vale la pena ver:** traducir la lista de
enteros del formulario al JSON que el procedimiento espera.

```csharp
// [1, 3]  ->  [{"fkidrol":1},{"fkidrol":3}]
JsonSerializer.Serialize(idsRol.Select(id => new { fkidrol = id }));
```

> **La clave es `fkidrol`**, que es la que el procedimiento abre con
> `json_array_elements`. Si se escribe `idrol`, el procedimiento **no
> encuentra nada** y el usuario queda sin roles — **sin un solo error**. Se
> mira el plpgsql, no se adivina.
>
> Y va en el servicio, no en el controlador, porque **la forma de ese JSON es
> una regla del dominio, no del transporte**. Y no en el repositorio, porque el
> repositorio ejecuta, no decide.

### 3.5 El ensamblador — lo único de la v1 que crece

`Program.cs` suma seis recursos. **Agrupado por versión y sin un solo registro
repetido:** registrar dos veces la misma interfaz no rompe nada —gana el
último— y por eso nadie lo nota, pero el que lo lee no sabe cuál manda.

### 3.6 Las interfaces gráficas

| Archivo | Qué enseña |
|---|---|
| `Clientes.razor` | **Dos** desplegables, uno **opcional** con «(ninguna)» → `null` |
| `Vendedores.razor` | Uno obligatorio: el mismo patrón, sin la opción vacía |
| `Facturas.razor` | **El recurso entero en un archivo**: la tabla, el maestro-detalle y el ver. Tres `@if` sobre un campo `vista`, y botones para alternar — ver **D9** |
| `UsuariosYRoles.razor` | El mismo patrón con **casillas** |
| `RolesPorUsuario.razor` | La puente cruda: **sin editar**, y el quitar con dos claves |
| `PermisosPorRol.razor` | La otra puente, **con el aviso de que todavía no aplica nada** |

**Un servicio del front POR RECURSO**, igual que en la v1 — doce líneas en el
`Program.cs` del front. Un `ApiService` genérico sería más corto y es
justamente lo que no se hace: con doce recursos, el que lee ya no sabe qué
rutas existen.

> **Y el nombre del archivo `.razor` ES el nombre de una clase.** Un
> `RolUsuario.razor` tapa el modelo `RolUsuario` y **no compila**. De ahí que
> las interfaces se llamen como la persona las llamaría —«Roles por
> usuario»— y no como la tabla.

## 4. Los cuatro tropiezos que esta versión tiene preparados

| | Qué pasa | Cómo se ve |
|---|---|---|
| **1** | **El front deserializa el sobre a `List<T>`** | La interfaz sale **vacía sin un solo error**. La API devuelve `{tabla, limite, total, datos[]}` — hay que entrar a `datos` |
| **2** | **Dapper mapea por NOMBRE de columna** | Una propiedad que no se llama igual que su columna llega `null` **en silencio**, con HTTP 200 y la celda en blanco. Se arregla con un alias: `SELECT ruta AS RutaTexto` |
| **3** | **El `JsonPropertyName` de los procedimientos** | Los procedimientos devuelven `nombre_cliente` y `idrol`; las propiedades son `NombreCliente` e `IdRol`. Sin el atributo, llegan `null` y **0** |
| **4** | **La cadena vacía de un desplegable opcional** | `""` no es `null`. La anotación `[StringLength(10, MinimumLength = 1)]` lo atrapa **en la petición** y responde **422** — sin ella llegaría a la base y sería un **409** |
| **5** | **Los desplegables cargados EN FILA** | Con la API apagada, cada petición espera sus 10 segundos de *timeout*. `/facturas` pide **cinco** listas: en fila son **50 segundos** en blanco antes de mostrar el aviso. Con `Task.WhenAll` son 10 |

> **Este quinto no se encuentra leyendo: se encuentra MIDIENDO.** Apareció
> cronometrando el criterio 18 —«apague la API y la interfaz sigue en pie»—. La
> interfaz *sí* quedaba en pie y *sí* mostraba su aviso: lo hacía casi un
> minuto después, que para quien la usa es lo mismo que estar roto. Un criterio
> que se comprueba «a ojo» lo habría dado por bueno.

> **Los cuatro primeros fallan EN SILENCIO**, y es lo que los hace caros. No hay
> excepción, no hay log: hay un dato equivocado. Por eso el cierre de la
> versión se hace **mirando la interfaz**, no solo leyendo respuestas de la
> API.

## 5. Lo que este plan deja FUERA a propósito

| | Dónde va |
|---|---|
| Token, 401, 403, hash de contraseña | **v3** |
| Consultas multitabla, tablero, marca, publicación | **v4** |
| Segundo motor y la fábrica | **v5** |
| Editar o borrar físicamente una factura | En ninguna: la operación es **anular** |
