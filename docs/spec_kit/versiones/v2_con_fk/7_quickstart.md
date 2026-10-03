# Quickstart — Versión 2: arranque, regresión y prueba de humo

> | | |
> |---|---|
> | **Qué se verifica** | Los **18** criterios de [2_spec.md](2_spec.md) §5 |
> | **Los formatos** | [6_contracts.md](6_contracts.md) |
> | **La API** | `http://localhost:8035` · Swagger en `/swagger` |
> | **La interfaz gráfica** | `http://localhost:8099` |

---

## 1. Arranque — un solo comando

```powershell
docker compose up -d --build
```

Levanta **tres** contenedores: SQL Server (que se siembra solo la primera
vez), la API y la interfaz gráfica.

```powershell
docker compose ps
```

> **Espere a que la API compile.** La primera vez `dotnet watch` tarda cerca de
> un minuto, y mientras tanto el puerto no responde. Se ve con
> `docker compose logs -f api-facturas`.

> **`curl.exe` con la extensión, y no es un capricho.** En PowerShell, `curl`
> pelado es un alias de `Invoke-WebRequest`, que **no entiende** `-X` ni `-d`.
> `curl.exe` es el curl de verdad, y Windows ya lo trae.

---

## 2. La REGRESIÓN primero — la v1 no se rompió

**Antes de probar nada nuevo.** Si la v2 rompió la v1, eso es lo que hay que
saber, y hay que saberlo antes de celebrar lo nuevo.

```powershell
# El diagnostico: lo UNICO que cambia es la version
curl.exe http://localhost:8035/
# -> {"mensaje":"API Facturas funcionando","version":"v2", ...}
```

Y la prueba de humo completa de la v1
([7_quickstart de la v1](../v1_sin_fk/7_quickstart.md) §2): los seis recursos
sin clave foránea, con sus cinco verbos. **Tiene que pasar sin un solo cambio**
—criterio **1**—.

---

## 3. La prueba de humo de la v2 — la API

```powershell
# ============================================================
# CRITERIO 2 — `cliente`, los cinco verbos
# ============================================================
curl.exe http://localhost:8035/api/cliente

# crear: el `id` NO se envia, lo pone la base (SERIAL)
curl.exe -X POST http://localhost:8035/api/cliente -H "Content-Type: application/json" -d "{\"credito\":500000,\"fkcodpersona\":\"P001\",\"fkcodempresa\":\"E001\"}"

# la pareja didactica: el MISMO cuerpo, dos verbos, dos respuestas
curl.exe -i -X PUT   http://localhost:8035/api/cliente/1 -H "Content-Type: application/json" -d "{\"credito\":900000}"   # -> 422
curl.exe -i -X PATCH http://localhost:8035/api/cliente/1 -H "Content-Type: application/json" -d "{\"credito\":900000}"   # -> 200

curl.exe -i -X DELETE http://localhost:8035/api/cliente/99   # -> 404

# ============================================================
# CRITERIO 3 — el 409 de la clave foranea, y el nulo que SI vale
# ============================================================
# una persona que no existe: NO es 422 -el texto esta bien formado-
curl.exe -i -X POST http://localhost:8035/api/cliente -H "Content-Type: application/json" -d "{\"credito\":1000,\"fkcodpersona\":\"NOEXISTE\"}"   # -> 409

# un cliente SIN empresa: se crea, con fkcodempresa en null
curl.exe -X POST http://localhost:8035/api/cliente -H "Content-Type: application/json" -d "{\"credito\":200000,\"fkcodpersona\":\"P002\",\"fkcodempresa\":null}"

# ============================================================
# CRITERIO 4 — `vendedor`, y la integridad referencial en accion
# ============================================================
curl.exe http://localhost:8035/api/vendedor
curl.exe -X POST http://localhost:8035/api/vendedor -H "Content-Type: application/json" -d "{\"carnet\":9001,\"direccion\":\"Calle 10 # 20-30\",\"fkcodpersona\":\"P003\"}"

# borrar la PERSONA de un vendedor: la base lo impide y dice como se llama
# la restriccion. Esto en la v1 era imposible de provocar.
curl.exe -i -X DELETE http://localhost:8035/api/persona/P003

# ============================================================
# CRITERIO 5 — lectura maestro-detalle
# ============================================================
# los nombres vienen RESUELTOS y los renglones ANIDADOS: el JOIN lo hizo el SP
curl.exe http://localhost:8035/api/factura
curl.exe http://localhost:8035/api/factura/1
curl.exe -i http://localhost:8035/api/factura/999     # -> 404

# ============================================================
# CRITERIO 6 — el disparador trabaja
# ============================================================
# 1) anote el stock de DOS productos ANTES
curl.exe http://localhost:8035/api/producto/PR001
curl.exe http://localhost:8035/api/producto/PR003

# 2) emita una factura con los dos
curl.exe -X POST http://localhost:8035/api/factura -H "Content-Type: application/json" -d "{\"fkidcliente\":1,\"fkidvendedor\":1,\"productos\":[{\"codigo\":\"PR001\",\"cantidad\":2},{\"codigo\":\"PR003\",\"cantidad\":1}]}"

# 3) la respuesta trae subtotales y total CALCULADOS, y el stock bajo
curl.exe http://localhost:8035/api/producto/PR001
curl.exe http://localhost:8035/api/producto/PR003
```

> **Lo que hay que comprobar en el criterio 6:** que `total` sea la suma de los
> subtotales **y que el cuerpo que se envió no los llevara**. La API no
> multiplicó nada.

```powershell
# ============================================================
# CRITERIO 7 — los errores del negocio
# ============================================================
# lista vacia: lo para la PETICION, ni llega a la base
curl.exe -i -X POST http://localhost:8035/api/factura -H "Content-Type: application/json" -d "{\"fkidcliente\":1,\"fkidvendedor\":1,\"productos\":[]}"   # -> 422

# mas cantidad que stock: lo para el DISPARADOR, y su mensaje viaja en `detalle`
curl.exe -i -X POST http://localhost:8035/api/factura -H "Content-Type: application/json" -d "{\"fkidcliente\":1,\"fkidvendedor\":1,\"productos\":[{\"codigo\":\"PR001\",\"cantidad\":999999}]}"

# anular: NO es DELETE, es una accion. Y el stock vuelve.
curl.exe -X POST http://localhost:8035/api/factura/7/anular
curl.exe http://localhost:8035/api/producto/PR001           # el stock subio

curl.exe -i -X POST http://localhost:8035/api/factura/7/anular     # -> 409 (ya estaba)
curl.exe -i -X POST http://localhost:8035/api/factura/999/anular   # -> 404

# ============================================================
# CRITERIO 8 — las dos tablas puente
# ============================================================
curl.exe http://localhost:8035/api/rol-usuario
curl.exe http://localhost:8035/api/rol-usuario/usuario/admin@correo.com
curl.exe http://localhost:8035/api/rol-usuario/rol/1

curl.exe -X POST http://localhost:8035/api/rol-usuario -H "Content-Type: application/json" -d "{\"fkemail\":\"admin@correo.com\",\"fkidrol\":2}"
curl.exe -i -X POST http://localhost:8035/api/rol-usuario -H "Content-Type: application/json" -d "{\"fkemail\":\"admin@correo.com\",\"fkidrol\":2}"   # -> 409 (la pareja ya existe)

# el DELETE lleva LAS DOS claves: la PK es compuesta
curl.exe -X DELETE http://localhost:8035/api/rol-usuario/admin@correo.com/2

# la otra puente, y OJO: esta ruta NO lleva guion
curl.exe http://localhost:8035/api/rutarol
curl.exe -X POST http://localhost:8035/api/rutarol -H "Content-Type: application/json" -d "{\"fkidruta\":1,\"fkidrol\":2}"
curl.exe -X DELETE http://localhost:8035/api/rutarol/1/2

# ============================================================
# CRITERIO 9 — `usuario-con-roles`: el usuario Y sus roles
# ============================================================
# los roles vienen PEGADOS a cada usuario: el agrupamiento lo hizo el SP
curl.exe http://localhost:8035/api/usuario-con-roles

# crear con DOS roles en UN envio
curl.exe -X POST http://localhost:8035/api/usuario-con-roles -H "Content-Type: application/json" -d "{\"email\":\"nuevo@correo.com\",\"contrasena\":\"clave123\",\"roles\":[1,2]}"
curl.exe http://localhost:8035/api/usuario-con-roles/nuevo@correo.com

# editar con la contrasena VACIA: cambia los roles y NO toca la contrasena.
# Y los roles REEMPLAZAN: manda uno, queda con uno.
curl.exe -X PUT http://localhost:8035/api/usuario-con-roles/nuevo@correo.com -H "Content-Type: application/json" -d "{\"contrasena\":\"\",\"roles\":[3]}"
curl.exe http://localhost:8035/api/usuario-con-roles/nuevo@correo.com

# sin ningun rol: 422. Un usuario sin rol no puede hacer nada.
curl.exe -i -X POST http://localhost:8035/api/usuario-con-roles -H "Content-Type: application/json" -d "{\"email\":\"x@correo.com\",\"contrasena\":\"x\",\"roles\":[]}"

# borra el detalle y el maestro juntos
curl.exe -X DELETE http://localhost:8035/api/usuario-con-roles/nuevo@correo.com

# ============================================================
# CRITERIO 10 — la prueba de capas, SIN SQL Server
# ============================================================
docker compose exec api-facturas dotnet run --project pruebas
```

> **La contraseña vacía del criterio 9 hay que comprobarla de verdad**, y la
> forma de hacerlo es la de la v3: identificarse. En la v2 todavía no hay
> login, así que se mira la columna:
>
> ```powershell
> docker compose exec postgres sqlcmd -U postgres -d bdfacturas_sqlserver_local -c "SELECT email, contrasena FROM usuario WHERE email='nuevo@correo.com';"
> ```
>
> Tiene que seguir siendo la misma de antes del `PUT`.

---

## 4. Las interfaces gráficas — criterios 11 a 18

**La API sola no cierra la versión.** Estos ocho se comprueban **en el
navegador**, en `http://localhost:8099`.

| | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **11** | Abrir `/clientes` | El desplegable de **persona** está **lleno**, con nombres. Si está vacío, la interfaz no pidió el catálogo |
| **12** | Crear un cliente y mirar lo que viaja | Se ve **«Ana Torres»** y en el JSON va **`P001`**. Nombre para la persona, código para la base |
| **13** | En `/facturas/nueva`: agregar **tres** renglones, quitar **uno**, emitir | **Llegan DOS** |
| **14** | Mirar el cuerpo de esa petición | **No lleva `total`** ni `subtotal`. Los puso la base |
| **15** | Buscar el botón de **eliminar** una factura | **No existe.** Hay **anular** — y después de anular, el stock del producto **subió** |
| **16** | En `/usuario-con-roles`: marcar **dos** casillas y crear | **Un solo envío**, y la lista lo muestra con sus dos roles |
| **17** | Editarlo dejando la contraseña **vacía** | Los roles cambian y la contraseña **sigue siendo la de antes** |
| **18** | **Apagar la API** y recargar cualquier interfaz | **Sigue en pie**: su aviso, el menú intacto, cero filas |

### El criterio 13, que es el único que no se puede aparentar

```
1. Abra /facturas y pulse «Nueva factura»   -> queda en /facturas/nueva
2. Elija cliente y vendedor
3. «Agregar al detalle» el producto A       -> aparece en la tabla de abajo
4. «Agregar al detalle» el producto B       -> aparecen dos
5. «Agregar al detalle» el producto C       -> aparecen tres
6. QUITE el producto B                      -> quedan dos
7. «Emitir la factura»                      -> vuelve a /facturas
8. Pulse «Ver» en la factura nueva
```

**Tienen que ser DOS renglones: A y C.** Si son tres —o si B sigue ahí— el
detalle se está enviando **a medida que se agrega**, y eso no es un
maestro-detalle: son tres operaciones que pueden fallar a mitad de camino y
dejar una factura incompleta.

### El criterio 18, que es el otro

```powershell
docker compose stop api-facturas
```

Recargue `/productos`, `/clientes` y `/facturas`.

| Tiene que pasar | NO puede pasar |
|---|---|
| Un **aviso** que la persona entienda | La página de error del navegador |
| El **menú** sigue ahí, y se puede navegar | Una excepción en pantalla |
| **Cero** filas | Una tabla con filas viejas, como si nada |

```powershell
docker compose start api-facturas
```

Recargue: las filas volvieron, sin reiniciar nada.

---

## 5. Si algo falla

| Síntoma | Qué pasa |
|---|---|
| `curl.exe` no conecta al **8035** | La primera compilación de `dotnet watch` no terminó. Espere ~1 min · `docker compose logs api-facturas` |
| La API responde **500 en todo** | La base no se sembró, o la cadena no apunta a `postgres:5432`. Reset: `docker compose down -v` y `up -d` |
| **Una interfaz sale vacía sin ningún error** | El front deserializó el sobre a `List<T>`. La API devuelve `{tabla, limite, total, datos[]}`: hay que entrar a `datos` |
| **Una columna sale en blanco, con HTTP 200** | Dapper mapea **por nombre**. Falta un alias: `SELECT ruta AS RutaTexto` |
| **`nombre_cliente` llega `null`** | Falta el `[JsonPropertyName("nombre_cliente")]`. El procedimiento devuelve snake_case |
| **Un desplegable opcional da 409** | Está mandando `""` en vez de `null`. La cadena vacía es un código que no existe |
| **El `id` llega en 0 al crear** | Lo genera la base (`IDENTITY`): no se envía, y la respuesta trae el asignado |
| **404 en `api/rol_usuario`** | Es `api/rol-usuario`, **con guion**. Y `api/rutarol` **sin** guion. Se lee el `[Route]` |
| Guardo un `.cs` y no pasa nada | Espere la recompilación; si no, `docker compose restart api-facturas` |
| **Una interfaz tarda 30 o 50 segundos en mostrar el aviso** | Está cargando los desplegables **en fila**: cada uno espera sus 10 s de *timeout*. Van con `Task.WhenAll`, a la vez |
| Reset total de la base | `docker compose down -v` y `docker compose up -d` — el script se vuelve a ejecutar |

> **Los cuatro del medio fallan EN SILENCIO**, y es lo que los hace caros: no
> hay excepción ni error en el log, hay un dato equivocado. Por eso el cierre
> de la versión se hace **mirando la interfaz**, no solo leyendo respuestas.
