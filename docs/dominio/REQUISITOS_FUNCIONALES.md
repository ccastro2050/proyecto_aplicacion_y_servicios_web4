# Requisitos funcionales — Facturación (`bdfacturas`)

> **Qué es este documento.** Todo lo que el sistema **hace**, con su versión, su
> regla y **cómo se comprueba**. Es el catálogo: si algo funciona y no está aquí,
> el catálogo está incompleto; si algo está aquí y no funciona, el sistema está
> incompleto. Las dos cosas son errores.
>
> **Qué NO es.** No reemplaza los `2_spec.md` de cada versión, que son los que
> **mandan**. Esto los junta en una sola tabla para poder mirar el sistema
> entero de un tirón.
>
> **Material académico simulado**, pero los endpoints de la última columna
> **están medidos contra la API corriendo**, no copiados del código.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 0. Cómo leer este catálogo

| Columna | Qué dice |
|---|---|
| **RF** | `RF-v<versión>-<número>`. El número no se reusa: si un requisito se retira, su número queda vacío |
| **Requisito** | Qué hace el sistema, en una frase |
| **Dónde se comprueba** | El endpoint, la pantalla o el comando |

> **La versión importa**, y no es decoración: este proyecto es **acumulativo**.
> Un RF de la v2 sigue vivo en la v4, y tiene que seguir funcionando — eso es la
> **prueba de regresión** que cada versión obliga a correr.

### Aviso sobre los números

Las cuentas de este documento salen de `swagger.json` de la API encendida:
**80 operaciones publicadas** y **19 escritas y apagadas** (§6). Si alguien
agrega un endpoint y no actualiza esta tabla, la comprobación de §7 lo delata.

---

## 1. Versión 1 — las seis tablas sin clave foránea

> **Lo que define la v1 no es «una tabla»: son las tablas que NO tienen clave
> foránea.** En `bdfacturas` son seis — `producto`, `empresa`, `persona`, `rol`,
> `ruta` y `usuario` — y son las primeras porque **se pueden llenar sin que
> exista nada más**.

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v1-1** | Listar, con `?limite=` | `GET /api/producto`, y los otros cinco |
| **RF-v1-2** | Consultar uno por su clave | `GET /api/producto/PR001` |
| **RF-v1-3** | Crear | `POST /api/producto` |
| **RF-v1-4** | **Reemplazar** uno completo — los campos que no se mandan quedan vacíos | `PUT /api/producto/PR001` |
| **RF-v1-5** | **Actualizar** solo lo que se manda | `PATCH /api/producto/PR001` |
| **RF-v1-6** | Eliminar | `DELETE /api/producto/PR001` |
| **RF-v1-7** | Decir si la API está viva, y en qué versión | `GET /` |
| **RF-v1-8** | **Una pantalla por recurso**, con dirección propia | `/productos`, `/personas`, `/empresas`, … |

> **Que los seis recursos sean idénticos salvo los campos ES el punto.** Con uno
> solo nunca aparece la pregunta que la v1 quiere provocar: *«¿y si hago un
> `Repositorio<T>` genérico y me ahorro cinco?»*. La respuesta está en el
> Artículo 10 de la constitución, y hay que haber sentido la repetición para que
> la respuesta signifique algo.

> **RF-v1-4 y RF-v1-5 son el corazón de la versión**, y la diferencia entre los
> dos es la lección: con el **mismo cuerpo** `{"nombre":"X"}`, el PUT deja el
> resto de los campos vacíos y el PATCH no los toca. Quien no haya visto eso
> pasar, no sabe la diferencia — se la cree.

> **Y RF-v1-3 falla a propósito con un código duplicado: devuelve 500.** No es
> un descuido: el 409 es la lección de la v2, y verlo mal primero es lo que hace
> que la corrección signifique algo. Está declarado en el spec de la v1.

> **RF-v1-8 dice «dirección propia» y eso excluye `/tabla/{nombre}`.** Una ruta
> con el nombre de la tabla como parámetro parece ahorro y es lo contrario:
> ninguna pantalla se puede enlazar, ningún permiso se puede dar por recurso.

---

## 2. Versión 2 — las seis tablas CON clave foránea

> **Y aquí está la confusión que el mapa de versiones avisa:** `rol_usuario` y
> `rutarol` **son de esta versión**, porque tienen clave foránea. Lo que llega
> en la v3 no es su CRUD — es **la puerta**. Administrar permisos y *hacerlos
> valer* son dos cosas distintas.

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v2-1** | CRUD de `cliente`, con **dos** claves foráneas: una obligatoria (`persona`) y una opcional (`empresa`) | `/api/cliente` |
| **RF-v2-2** | CRUD de `vendedor`, con una clave foránea obligatoria | `/api/vendedor` |
| **RF-v2-3** | `factura` se opera **siempre por procedimiento**: maestro y detalle en una transacción | `/api/factura` |
| **RF-v2-4** | Anular una factura — borrado **lógico**, con devolución de stock | `POST /api/factura/{n}/anular` |
| **RF-v2-5** | Las dos tablas puente, con llave compuesta y **borrado por sus dos claves** | `/api/rol-usuario` · `/api/rutarol` |
| **RF-v2-6** | `usuario-con-roles`: el usuario **y** sus roles en una sola operación | `/api/usuario-con-roles` |
| **RF-v2-7** | Un código HTTP nuevo: el **409** — clave foránea inexistente, pareja repetida, factura ya anulada | `POST /api/cliente` con `P999` |
| **RF-v2-8** | Una clave foránea en la interfaz es un **desplegable**, nunca un campo de texto | Pantalla «Nuevo cliente» |
| **RF-v2-9** | El formulario de factura es **maestro-detalle**: se agregan renglones antes de guardar | Pantalla «Nueva factura» |
| **RF-v2-10** | La v1 **sigue funcionando igual** | Regresión: los 8 RF de la v1 |

> **`productosporfactura` no tiene endpoint propio, y no es un olvido.** Un
> renglón no se crea solo: nace y muere con su factura, dentro del
> procedimiento. Darle un `POST /api/productosporfactura` sería permitir un
> renglón huérfano — justo lo que la transacción existe para impedir.

> **RF-v2-8 es más que estética.** Un campo de texto para `fkcodpersona` deja
> que alguien escriba `P999`, y lo que llega de vuelta es un 409. Un desplegable
> hace que **el error no se pueda cometer** — y ésa es la diferencia entre
> validar y prevenir.

> **RF-v2-9 es la razón de ser de la versión.** Una factura con tres renglones
> son cuatro escrituras que tienen que ser **una sola operación**. Si el
> formulario guardara el encabezado y luego los renglones uno por uno, una
> factura podría quedar con dos de tres. Ver
> [`PRINCIPIOS_ACID.md`](../conceptos/PRINCIPIOS_ACID.md).

> **Dos procedimientos existen en la base de datos y la API NO los expone**:
> `sp_actualizar_factura_y_productosporfactura` y `sp_borrar_…`. Está declarado
> en el spec: la operación del negocio es **anular**, no corregir. Es el mismo
> motivo por el que el `PUT` de factura está escrito y apagado (§6).

---

## 3. Versión 3 — quién entra, y a qué

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v3-1** | La contraseña se guarda **cifrada** (BCrypt), nunca en claro | `SELECT contrasena FROM usuario` |
| **RF-v3-2** | Identificarse devuelve un **token** | `POST /api/sesion` |
| **RF-v3-3** | Sin token no se entra: **401** | Cualquier endpoint sin cabecera |
| **RF-v3-4** | Con token pero sin permiso: **403** | Entre como `cliente1@correo.com` y pida `/api/factura` |
| **RF-v3-5** | El permiso **lo decide la base de datos**, en cada petición | `verificar_acceso_ruta` |
| **RF-v3-6** | El menú muestra **solo** lo que ese rol puede abrir | Entre con los tres usuarios y compare |
| **RF-v3-7** | El login responde **lo mismo** si el correo no existe y si la clave está mal | Dos peticiones, misma respuesta |
| **RF-v3-8** | Repartir permisos desde la interfaz | Pantalla «Permisos» |
| **RF-v3-9** | Las v1 y v2 siguen funcionando — **ahora con token** | Regresión |

> **El 401 y el 403 son dos preguntas distintas**, y confundirlas es el error
> más común: *«¿quién eres?»* contra *«¿puedes?»*. El primero lo responde
> `[Authorize]`; el segundo, `[ExigePermiso]`, **después**. Ver
> [`CONCEPTOS_CONTROL_DE_ACCESO.md`](../conceptos/CONCEPTOS_CONTROL_DE_ACCESO.md) y la imagen de
> `401_contra_403`.

> **RF-v3-5 tiene una consecuencia que vale la pena notar:** como el permiso se
> consulta en cada petición y no viaja en el token, quitarle un permiso a un rol
> **surte efecto de inmediato**, sin que la persona tenga que volver a entrar.
> Si el permiso viviera en el token, habría que esperar a que expire.

---

## 4. Versión 4 — el aplicativo, que es lo que el CRUD no puede

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v4-1** | Diez consultas de negocio que **cruzan** tablas y **agrupan** | `/api/consultas/*` (10 operaciones) |
| **RF-v4-2** | Un tablero con indicadores, no un listado | `http://localhost:8099/` |
| **RF-v4-3** | La interfaz sigue el **manual de marca**: ni un color a mano | [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) §6 |
| **RF-v4-4** | La interfaz **no habla en jerga**: ni «PUT», ni «422», ni «FK» | Recorrer las pantallas |
| **RF-v4-5** | Las v1, v2 y v3 siguen funcionando | Regresión completa |

**Las diez, tal como las publica la API:**

```
ventas-por-vendedor      ventas-por-cliente      ventas-por-producto
ventas-por-empresa       ticket-por-vendedor     anulaciones-por-cliente
credito-contra-consumo   productos-sin-vender    alcance-de-usuarios
interfaces-sin-usuarios
```

> **Por qué estas diez no las puede dar el CRUD.** Un CRUD responde *«dame las
> filas de `factura`»*. Ninguna de las diez es eso: son *«cuánto vendió cada
> vendedor»*, *«qué productos no se han vendido nunca»*, *«qué interfaces no
> alcanza ningún usuario»*. Eso es un `JOIN` con un `GROUP BY`, y no cabe en
> `GET /api/{tabla}` — por eso la v4 existe y no es «la v2 con más pantallas».

> **Y las dos últimas no son de ventas, son del propio sistema.**
> `alcance-de-usuarios` e `interfaces-sin-usuarios` responden *«¿quién puede
> entrar a qué?»* y *«¿qué pantalla no alcanza nadie?»*. La segunda es la más
> útil de las diez para administrar: una interfaz que ningún rol alcanza está
> construida y nadie la ve.

---

## 5. Versión 5 — el otro motor

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v5-1** | Una **fábrica** decide qué repositorio se usa | `IFabricaRepositorios` |
| **RF-v5-2** | El motor se elige por **configuración**, no recompilando | La variable `MOTOR_BD` |
| **RF-v5-3** | PostgreSQL completo: las mismas operaciones, el mismo contrato | `/api/producto` con el otro motor |
| **RF-v5-4** | El diagnóstico dice **qué motor** está puesto | `GET /` |
| **RF-v5-5** | Cambiar de motor **no toca** controladores ni servicios | `git diff` sobre `Controllers/`: vacío |

> **RF-v5-5 es el que da sentido a los otros cuatro.** Si para cambiar de motor
> hubiera que tocar un controlador, las capas estarían mal hechas y las cuatro
> versiones anteriores habrían enseñado una arquitectura que no aguanta. La v5 es
> la factura que se le pasa a la v1.

---

## 6. Los cinco verbos, recurso por recurso

Esto es lo que de verdad publica la API, contado de `swagger.json`:

| Recurso | GET | POST | PUT | PATCH | DELETE |
|---|---|---|---|---|---|
| `producto` · `persona` · `empresa` · `cliente` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `vendedor` · `usuario` · `rol` · `ruta` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `factura` | ✅ | ✅ | 🔸 | 🔸 | 🔸 |
| `rol-usuario` · `rutarol` | ✅ | ✅ | 🔸 | 🔸 | ✅ |
| `usuario-con-roles` | ✅ | ✅ | ✅ | 🔸 | ✅ |
| `sesion` | 🔸 | ✅ | 🔸 | 🔸 | 🔸 |
| `permisos` | 🔸 | 🔸 | 🔸 | 🔸 | ✅ |
| `consultas` | ✅ | — | — | — | — |

**✅ activo · 🔸 escrito y apagado · — no aplica**

> **Las 🔸 NO son huecos: son código escrito, comentado y explicado.** Ocho
> recursos tienen los cinco verbos vivos; los demás tienen el verbo **escrito
> en el controlador, apagado con un comentario que dice por qué**, y con el
> servicio y el repositorio **activos** debajo. Encenderlo es quitar las barras.
>
> Son **19 métodos**, y están así a propósito: esto es material de clase, y un
> estudiante tiene que poder ver **cómo se programa cada verbo**, incluso el que
> esta versión del spec no pide. Borrarlos enseñaría solo a obedecer un spec;
> dejarlos apagados enseña a obedecerlo **y** a programar lo que no pide.

| Y por qué están apagados | Cuáles |
|---|---|
| El recurso **no se modifica, se anula** | `PUT`/`PATCH`/`DELETE` de `factura` |
| En una tabla puente, «actualizar» es **mover la fila**: borrar y crear | `PUT`/`PATCH` de `rol-usuario` y `rutarol` |
| Una sesión **no se edita**: se abre y se deja vencer | todo lo demás de `sesion` |
| El reparto de permisos lo hace la pantalla con `GET` y `DELETE` | el resto de `permisos` |
| El login llama al **servicio**, no a la API | `POST /api/usuario/verificar-contrasena` |

---

## 7. Matriz de trazabilidad, y cómo se comprueba

| Versión | RF | Qué publica | Cuántas | Regresión que hereda |
|---|---|---|---|---|
| **v1** | 8 | los 6 recursos sin FK (6 cada uno) + diagnóstico | **37** | — |
| **v2** | 10 | `cliente` 6 · `vendedor` 6 · `factura` 4 · las 2 puentes 5+5 · `usuario-con-roles` 5 | **31** | los 8 de la v1 |
| **v3** | 9 | `sesion` 1 · `permisos` 1 | **2** | los 18 anteriores |
| **v4** | 5 | `consultas` | **10** | los 27 anteriores |
| **v5** | 5 | nada nuevo — **los mismos, otro motor** | **0** | los 32 anteriores |
| | **37 RF** | | **80** | |

> **La v3 publica DOS operaciones y es la versión que más cambia el sistema.**
> No agrega tablas ni recursos: le pone una puerta a los 68 endpoints que ya
> existían. Medir una versión por cuántos endpoints suma habría dicho que la v3
> casi no es trabajo, y es exactamente al revés.

> **Que la v5 agregue CERO operaciones es el dato más importante de la tabla.**
> Una versión entera de trabajo que no cambia el contrato: eso es exactamente lo
> que significa «abierto a la extensión, cerrado a la modificación».

```powershell
# 1 · Lo que la API publica contra lo que el contrato declara
python auditar_aysw4_por_version.py

# 2 · Cuántas operaciones hay de verdad — debe decir 80
(Invoke-RestMethod http://localhost:8035/swagger/v1/swagger.json).paths.PSObject.Properties |
  ForEach-Object { $_.Value.PSObject.Properties.Name } | Measure-Object | Select-Object Count
```

| Qué | Dónde |
|---|---|
| El requisito que manda, por versión | el `2_spec.md` de cada una |
| El contrato endpoint por endpoint | el `6_contracts.md` de cada una |
| Las reglas que estos RF dan por ciertas | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Lo que el sistema tiene que **ser** | [`REQUISITOS_NO_FUNCIONALES.md`](REQUISITOS_NO_FUNCIONALES.md) |
