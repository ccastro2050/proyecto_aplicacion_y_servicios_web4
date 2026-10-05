# Tareas — Versión 2: las tablas con clave foránea, por fases

> | | |
> |---|---|
> | **Qué** | [2_spec.md](2_spec.md) |
> | **Cómo** | [3_plan.md](3_plan.md) · [6_contracts.md](6_contracts.md) |
> | **La verificación final** | [7_quickstart.md](7_quickstart.md) |

**Cada fase termina con algo que se puede VERIFICAR**, y la verificación está
escrita. Una fase que no se puede comprobar no es una fase: es una esperanza.

> **Y cada fase es un COMMIT.** No se acumulan tres fases para un commit
> grande: si algo se rompe, el commit chico dice dónde.

---

## Fase 0 — El punto de partida, verificado

| | |
|---|---|
| **Qué** | Que la v1 esté **funcionando**, no que exista |
| **Verificación** | `docker compose up -d --build` y la prueba de humo de la v1 pasa completa |

> **No se empieza la v2 sin esto.** Construir encima de una v1 que no arranca
> es garantizar que no se sabrá qué rompió qué.

## Fase 1 — `cliente`: el modelo y las tres peticiones

| | |
|---|---|
| **Archivos** | `Modelos/Cliente.cs` · `Peticiones/ClienteCrear.cs` · `ClienteReemplazo.cs` · `ClienteActualizar.cs` |
| **Lo que importa** | `Crear` y `Reemplazo` con **todo obligatorio**; `Actualizar` con **todo opcional**. `Fkcodempresa` es `string?` en las tres |
| **Verificación** | `dotnet build` compila. Todavía no hay endpoint |

## Fase 2 — `cliente`: repositorio, servicio y controlador

| | |
|---|---|
| **Archivos** | La interfaz y la implementación del repositorio, la interfaz y el servicio, el controlador, y el registro en `Program.cs` |
| **Lo que importa** | La traducción de `el error 547` y `23505` a `ConflictoExcepcion` → **409** |
| **Verificación** | `GET /api/cliente` responde con el sobre; crear con un `fkcodpersona` inexistente responde **409**; el mismo cuerpo da **422** en PUT y **200** en PATCH |

## Fase 3 — `vendedor`: la rebanada completa, calcada

| | |
|---|---|
| **Archivos** | Los nueve de la rebanada, idénticos en forma a los de cliente |
| **Lo que importa** | Que **no haya ninguna decisión nueva**. Si aparece una, la v1 dejó algo sin resolver |
| **Verificación** | Los cinco verbos; y `DELETE /api/persona/{codigo}` de una persona que es vendedor **falla**, con el nombre de la restricción en el `detalle` |

## Fase 4 — `factura`: los modelos de lectura y la petición de creación

| | |
|---|---|
| **Archivos** | `Modelos/Factura.cs` · `Modelos/ProductoDeFactura.cs` · `Peticiones/FacturaCrear.cs` |
| **Lo que importa** | Los `[JsonPropertyName]` de `nombre_cliente`, `nombre_vendedor`, `codigo_producto`, `nombre_producto`. Y que `FacturaCrear` **no tenga** `total` |
| **Verificación** | Compila. Y se puede explicar por qué el total no está en la petición |

## Fase 5 — `factura`: el repositorio de procedimientos

| | |
|---|---|
| **Archivos** | `IRepositorioFactura.cs` · `RepositorioFacturaSqlServer.cs` |
| **Lo que importa** | **Cuatro `CALL`, cero `SELECT` de tablas.** El `INOUT` leído con `ExecuteScalarAsync`, el `::json` del detalle, y los `P0001` traducidos por patrón |
| **Verificación** | `grep -c "SELECT" RepositorioFacturaSqlServer.cs` → **0**. Si hay uno, la lógica se está saliendo de la base de datos |

## Fase 6 — `factura`: servicio, controlador y ensamblador

| | |
|---|---|
| **Archivos** | El servicio, el controlador de **4 endpoints** y el registro |
| **Lo que importa** | Que **no haya PUT, PATCH ni DELETE**. Anular es `POST .../anular` |
| **Verificación** | Listar trae el detalle anidado y los nombres · crear baja el stock y calcula el total · anular devuelve el stock · anular otra vez → **409** |

## Fase 7 — Las dos tablas puente

| | |
|---|---|
| **Archivos** | Las rebanadas de `rol_usuario` y `rutarol` — **sin** peticiones de Reemplazo ni Actualizar |
| **Lo que importa** | Las rutas: `api/rol-usuario` **con** guion, `api/rutarol` **sin** guion. Y el `DELETE` con **dos** valores |
| **Verificación** | Agregar una pareja → 200 · la misma otra vez → **409** · quitarla con sus dos claves → 200 · listar por los dos lados |

## Fase 8 — `usuario-con-roles`: el recurso de los cinco procedimientos

| | |
|---|---|
| **Archivos** | Modelo, dos peticiones, repositorio, servicio y controlador |
| **Lo que importa** | La traducción `[1,3]` → `[{"fkidrol":1},{"fkidrol":3}]` **en el servicio**. Y que la contraseña vacía al editar **no** la cambie |
| **Verificación** | Crear con dos roles en un envío → la lista lo muestra con los dos · editar con la contraseña vacía → sigue sirviendo · mandar un rol → queda con uno |

> **Hasta aquí la API está completa: los 12 recursos.** Y la versión **no
> está cerrada**, porque falta la mitad.

## Fase 9 — Las interfaces gráficas de los recursos con clave foránea

| | |
|---|---|
| **Archivos** | Los modelos y servicios del front de los seis recursos · `Clientes.razor` · `Vendedores.razor` · `RolesPorUsuario.razor` · `PermisosPorRol.razor` · el menú · el `Program.cs` del front |
| **Lo que importa** | **El desplegable cargado de la API**: muestra el nombre, manda el código. El opcional con «(ninguna)» → `null`. Y el nombre del `.razor` **no puede** chocar con el de un modelo |
| **Verificación** | Abrir `/clientes`: el desplegable está **lleno**. Crear un cliente **sin** empresa: se crea, con `fkcodempresa: null` |

## Fase 10 — El formulario integrado de factura

| | |
|---|---|
| **Archivos** | `Facturas.razor` — **uno solo**: la tabla, el formulario maestro-detalle y el ver, con un campo `vista` que decide (D9) · `ServicioFactura.cs` del front |
| **Lo que importa** | **Agregar un renglón NO llama a la API.** El total se muestra y **no se envía**. El botón dice **anular** |
| **Verificación** | **Agregar tres renglones, quitar uno, emitir → llegan DOS.** Y el JSON que sale **no lleva** `total` |

> **Esta es la fase que no se puede simular**, y si algo va a salir mal, sale
> mal aquí. Conviene mirar el cuerpo de la petición en el navegador.

## Fase 11 — El formulario integrado de usuario con sus roles

| | |
|---|---|
| **Archivos** | `UsuariosYRoles.razor` · `ServicioUsuarioConRoles.cs` del front |
| **Lo que importa** | **Casillas**, no un desplegable. Marcar **no** llama a la API. Al editar, las casillas arrancan con lo que el usuario ya tiene y la contraseña va **vacía** |
| **Verificación** | Crear con dos casillas en un envío · editar dejando la contraseña vacía y comprobar que **sigue sirviendo** |

## Fase 12 — El cierre

| | |
|---|---|
| **1** | La **regresión**: la prueba de humo de la v1, completa |
| **2** | Los **18** criterios de [2_spec.md](2_spec.md) §5, con [7_quickstart.md](7_quickstart.md) |
| **3** | **Apagar la API** y recargar una interfaz: sigue en pie, con su aviso y el menú intacto |
| **4** | Commit y **tag `v2`** |

> **El criterio 18 va al final y se hace de verdad**, porque es el único que
> prueba que la interfaz no supone que la API siempre responde.
