# Quickstart — Versión 3: arranque y prueba de humo del control de acceso

> | | |
> |---|---|
> | **Qué se verifica** | Los **diez** criterios de [2_spec.md](2_spec.md) §4 |
> | **Los formatos** | [6_contracts.md](6_contracts.md) |
> | **La API** | `http://localhost:8035` |
> | **La interfaz gráfica** | `http://localhost:8099` |

---

## 1. Arranque

```powershell
docker compose down -v
docker compose up -d --build
```

> **El `down -v` es necesario esta vez, y solo esta vez.** La v3 vuelve a
> sembrar las contraseñas con hash, y el script de la base **solo se ejecuta
> cuando el volumen nace**. Sin borrarlo, la base sigue con las contraseñas
> viejas y nada funciona.

> **`curl.exe` con la extensión.** En PowerShell, `curl` pelado es un alias de
> `Invoke-WebRequest`, que no entiende `-X` ni `-d`.

---

## 2. Las contraseñas, que ahora se saben

| Correo | Contraseña | Roles | Entra a |
|---|---|---|---|
| `admin@correo.com` | `admin123` | Administrador | **las 15 rutas** |
| `vendedor1@correo.com` | `vendedor123` | Vendedor, Cajero | inicio, facturas, clientes |
| `cliente1@correo.com` | `cliente123` | Cliente | inicio, productos |
| `jefe@correo.com` | `jefe123` | Administrador, Cajero, Contador | las 15 |
| `test_encript@correo.com` | `test123` | Administrador | las 15 |
| `nuevo@correo.com` | `nuevo123` | Administrador, Vendedor, Cajero | las 15 |
| `carlos.castro@usbmed.edu.co` | `carlos123` | todos | las 15 |
| `carloscastro5033@correo.itm.edu.co` | `carlos123` | todos | las 15 |

> **Están escritas aquí porque del hash no se puede volver a la clave** —eso es
> lo que lo hace un hash—. Sin saberlas, no hay forma de iniciar sesión, y sin
> iniciar sesión no se comprueba un solo criterio.
>
> **Los tres primeros son los que importan.** Con ellos se demuestra el 403.

---

## 2bis. Swagger, que ahora pide token

**La v3 le cambia el uso a Swagger**, y hay que saberlo o parece que algo se
rompió: al exigir token, cualquier endpoint que se pruebe sin autorizar
responde **401**.

> **Por eso la API declara su esquema de seguridad en el documento OpenAPI.**
> Swagger **no adivina** cómo se autentica una API: hay que decírselo, y eso es
> parte del contrato — un OpenAPI que no dice cómo se entra está incompleto.

### Los cuatro pasos

```
http://localhost:8035/swagger
```

| | Qué hacer |
|---|---|
| **1** | Despliegue **`POST /api/sesion`** → *Try it out* → en el cuerpo ponga `{ "email": "admin@correo.com", "contrasena": "admin123" }` → *Execute* |
| **2** | En la respuesta, **copie el valor de `token`** — solo el texto, sin las comillas |
| **3** | Botón **`Authorize`**, arriba a la derecha → pegue **solo el token** → *Authorize* → *Close* |
| **4** | Ya puede probar cualquier endpoint: el candado se cierra y Swagger manda la cabecera |

> **No escriba la palabra `Bearer`.** La pone Swagger. Escribirla produce
> `Bearer Bearer eyJ…`, que es un 401 que cuesta encontrar porque el token
> está bien.

### Lo que conviene probar ahí mismo

| | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **Sin autorizar** | Pruebe `GET /api/producto` antes del paso 3 | **401** |
| **Autorizado** | El mismo, después | **200** |
| **El 403** | Cierre sesión (*Authorize* → *Logout*), entre como `vendedor1@correo.com` / `vendedor123`, autorice con **ese** token y pruebe `GET /api/usuario` | **403**, con la `ruta` que le faltó |

> **`POST /api/sesion` muestra el candado como los demás** —el requisito está
> declarado para toda la API— **y funciona sin token igual**, porque el
> controlador es `[AllowAnonymous]`. El candado dice «esta API usa token», no
> «este endpoint lo exige».

## 3. CRITERIO 1 — ninguna contraseña legible en la base

```powershell
docker compose exec postgres sqlcmd -U postgres -d bdfacturas_sqlserver_local -c "SELECT email, contrasena FROM usuario;"
```

**Las ocho tienen que empezar por `$2a$12$`**, y ninguna puede ser una palabra
legible.

```
admin@correo.com          | $2a$12$PJf6LIuW8uL9q9hK0LsG0ebvUll.eLcJgg6lmTIPVk84p0fwD0T5u
vendedor1@correo.com      | $2a$12$MeuuKTqIN3JeEYGUCMtbueU5k8QVy7mmiB.yVDkT9hUp0FIyyrdZ2
...
```

> **Mire los dos usuarios de `carlos.castro`: comparten la contraseña y sus
> hashes no se parecen.** Eso es el *salt*, que bcrypt trae incorporado. Sin
> él, dos hashes iguales delatarían que esas dos personas usan la misma clave.

---

## 4. CRITERIO 2 — iniciar sesión, y el MISMO error en los dos casos

```powershell
# correctas -> 200 con el token
curl.exe -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"admin@correo.com\",\"contrasena\":\"admin123\"}"

# la contrasena mal -> 401
curl.exe -i -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"admin@correo.com\",\"contrasena\":\"NOESLACLAVE\"}"

# un correo que NO existe -> 401, CON EL MISMO MENSAJE
curl.exe -i -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"nadie@correo.com\",\"contrasena\":\"x\"}"
```

**Los dos mensajes tienen que ser IDÉNTICOS:**

```
{"estado":401,"mensaje":"El correo o la contrasena no son correctos."}
{"estado":401,"mensaje":"El correo o la contrasena no son correctos."}
```

> **Si el segundo dijera «ese correo no existe»**, le confirmaría a un
> desconocido cuáles correos sí existen — y con una lista de correos válidos,
> probar contraseñas vale la pena.

---

## 5. CRITERIOS 3 y 4 — sin token 401, token alterado 401

```powershell
# guarde el token en una variable
$t = (curl.exe -s -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"admin@correo.com\",\"contrasena\":\"admin123\"}" | ConvertFrom-Json).token

curl.exe -i http://localhost:8035/api/usuario                                      # -> 401 (sin token)
curl.exe -i -H "Authorization: Bearer $t" http://localhost:8035/api/usuario        # -> 200
curl.exe -i -H "Authorization: Bearer $($t)X" http://localhost:8035/api/usuario    # -> 401 (alterado)
curl.exe -i -H "Authorization: Bearer esto.no.es.un.token" http://localhost:8035/api/usuario  # -> 401

curl.exe http://localhost:8035/                                                     # -> 200 (abierto)
```

**Resultado:**

```
sin token:        401
con token:        200
token ALTERADO:   401
basura por token: 401
el diagnostico /: 200
```

> **El token alterado es el que demuestra la firma:** se le cambió **una
> letra** y el servidor lo rechazó. No tuvo que consultar nada: la firma no
> cuadra.

---

## 6. CRITERIOS 5 y 9 — el 403, que es el corazón de la versión

```powershell
$ta = (curl.exe -s -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"admin@correo.com\",\"contrasena\":\"admin123\"}" | ConvertFrom-Json).token
$tv = (curl.exe -s -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"vendedor1@correo.com\",\"contrasena\":\"vendedor123\"}" | ConvertFrom-Json).token
$tc = (curl.exe -s -X POST http://localhost:8035/api/sesion -H "Content-Type: application/json" -d "{\"email\":\"cliente1@correo.com\",\"contrasena\":\"cliente123\"}" | ConvertFrom-Json).token

# el vendedor pidiendo usuarios: TOKEN VALIDO, rol sin permiso
curl.exe -i -H "Authorization: Bearer $tv" http://localhost:8035/api/usuario     # -> 403
# y pidiendo clientes, que SI puede
curl.exe -i -H "Authorization: Bearer $tv" http://localhost:8035/api/cliente     # -> 200
# el cliente, al CONTRARIO: productos si, clientes no
curl.exe -i -H "Authorization: Bearer $tc" http://localhost:8035/api/producto    # -> 200
curl.exe -i -H "Authorization: Bearer $tc" http://localhost:8035/api/cliente     # -> 403
```

**La matriz completa, que es la salida real:**

| Recurso | `admin` | `vendedor1` | `cliente1` |
|---|---|---|---|
| `/api/producto` | 200 | **403** | 200 |
| `/api/cliente` | 200 | 200 | **403** |
| `/api/factura` | 200 | 200 | **403** |
| `/api/usuario` | 200 | **403** | **403** |
| `/api/rutarol` | 200 | **403** | **403** |
| `/api/persona` | 200 | **403** | **403** |
| `/api/empresa` | 200 | **403** | **403** |
| `/api/rol` | 200 | **403** | **403** |
| `/api/ruta` | 200 | **403** | **403** |
| `/api/vendedor` | 200 | **403** | **403** |

> **El contraste entre `vendedor1` y `cliente1` es lo que hay que ver:** uno
> entra a clientes y no a productos; el otro al revés. **No es un orden de
> privilegios** — es una matriz, y eso es RBAC.

### El 403 dice QUÉ permiso faltó

```json
{"estado":403,"mensaje":"Su rol no tiene permiso para esta operacion.","ruta":"interfaz.usuarios"}
```

---

## 7. CRITERIO 6 — el permiso lo resuelve el procedimiento

```powershell
docker compose exec api-facturas grep -rn "verificar_acceso_ruta" Repositorios/
```

Tiene que aparecer **en `RepositorioAccesoSqlServer.cs`**, y el `JOIN` de
permisos **no puede estar escrito en C# para decidir**.

> **Hay UN `JOIN` de permisos en C#, en `RutasPermitidasAsync`, y no
> contradice esto: no decide nada.** Es una lista para dibujar un menú. La
> decisión la toma el procedimiento, y solo él.

---

## 8. CRITERIO 7 — el que no se puede simular

**Quitarle un permiso a un rol surte efecto SIN volver a identificarse.**

```powershell
# 1. el vendedor pide clientes con su token
curl.exe -i -H "Authorization: Bearer $tv" http://localhost:8035/api/cliente      # -> 200

# 2. se le quita a su rol (Vendedor = 2) el permiso de interfaz.clientes (ruta 4)
docker compose exec postgres sqlcmd -U postgres -d bdfacturas_sqlserver_local -c "DELETE FROM rutarol WHERE fkidruta=4 AND fkidrol=2;"

# 3. CON EL MISMO TOKEN, sin volver a entrar
curl.exe -i -H "Authorization: Bearer $tv" http://localhost:8035/api/cliente      # -> 403

# 4. se le devuelve
docker compose exec postgres sqlcmd -U postgres -d bdfacturas_sqlserver_local -c "INSERT INTO rutarol (fkidruta, fkidrol) VALUES (4, 2);"

# 5. con el mismo token otra vez
curl.exe -i -H "Authorization: Bearer $tv" http://localhost:8035/api/cliente      # -> 200
```

**Resultado:**

```
1. 200
3. 403   <- el token NO cambio
5. 200
```

> **El token no cambió en ningún momento.** Si los permisos viajaran dentro, el
> paso 3 habría respondido **200** hasta que el token venciera — una hora de
> permiso que ya se le había quitado.
>
> **Y se puede ver que no están dentro:** pegue el token en
> <https://jwt.io> —o decodifique el segundo trozo en base64— y busque la
> palabra «interfaz». No está. Solo el correo y los nombres de los roles.

---

## 9. CRITERIO 10 — la REGRESIÓN, con token

**Todo lo de la v1 y la v2 sigue pasando**, agregando la cabecera.

```powershell
$h = @{ Authorization = "Bearer $ta" }

# los 12 recursos
foreach ($r in "producto","empresa","persona","rol","ruta","usuario","cliente","vendedor","factura","rol-usuario","rutarol","usuario-con-roles") {
    curl.exe -s -o NUL -w "$r %{http_code}`n" -H "Authorization: Bearer $ta" "http://localhost:8035/api/$r"
}

# el ciclo de los 5 verbos de la v1
curl.exe -X POST http://localhost:8035/api/producto -H "Authorization: Bearer $ta" -H "Content-Type: application/json" -d "{\"codigo\":\"PRV3\",\"nombre\":\"Prueba\",\"stock\":10,\"valorunitario\":1000}"
curl.exe -i -X PUT http://localhost:8035/api/producto/PRV3 -H "Authorization: Bearer $ta" -H "Content-Type: application/json" -d "{\"stock\":9}"      # -> 422
curl.exe -i -X PATCH http://localhost:8035/api/producto/PRV3 -H "Authorization: Bearer $ta" -H "Content-Type: application/json" -d "{\"stock\":9}"    # -> 200
curl.exe -X DELETE http://localhost:8035/api/producto/PRV3 -H "Authorization: Bearer $ta"

# la v2: el maestro-detalle y el 409
curl.exe -X POST http://localhost:8035/api/factura -H "Authorization: Bearer $ta" -H "Content-Type: application/json" -d "{\"fkidcliente\":1,\"fkidvendedor\":1,\"productos\":[{\"codigo\":\"PR001\",\"cantidad\":1}]}"
curl.exe -i -X POST http://localhost:8035/api/cliente -H "Authorization: Bearer $ta" -H "Content-Type: application/json" -d "{\"credito\":1,\"fkcodpersona\":\"NOEXISTE\"}"   # -> 409
```

**Resultado: los 12 recursos en 200, el 422/200 del PUT/PATCH, y el 409 de la
clave foránea.** La v3 no rompió nada.

---

## 10. CRITERIOS 8 y 9 — en el NAVEGADOR

**Estos dos no se pueden comprobar con `curl`**, y conviene decir por qué: la
interfaz gráfica guarda el token **en el circuito de Blazor Server** —en memoria
del servidor, atado a la conexión del navegador—. Una petición de `curl` no
tiene circuito, así que no tiene sesión.

Abra `http://localhost:8099`.

| | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **8a** | Entrar sin identificarse | El menú tiene **una** entrada: «Iniciar sesión» |
| **8b** | Entrar como `admin@correo.com` / `admin123` | El menú muestra **las 12** interfaces, y arriba sale el correo con la etiqueta «Administrador» |
| **8c** | Salir y entrar como `vendedor1@correo.com` / `vendedor123` | El menú muestra **Facturas y Clientes**, y **NO** Usuarios, Personas, Empresas, Roles, Rutas ni Productos |
| **8d** | Salir y entrar como `cliente1@correo.com` / `cliente123` | El menú muestra **Productos** y **NO** Facturas ni Clientes — al revés que el vendedor |
| **9** | Como `vendedor1`, **escribir `/usuarios` en la barra de direcciones** | La interfaz **se abre** y muestra el aviso **«Su rol no tiene permiso para esta operacion»** y **cero filas** |

### El criterio 9 es el único que no se puede simular

Los otros nueve se pueden cumplir con una interfaz que esconda botones. **Ese
no.**

```
el menu no muestra «Usuarios»  ·  pero esto existe:
    http://localhost:8099/usuarios     <- escrito a mano
```

| Tiene que pasar | NO puede pasar |
|---|---|
| La interfaz se abre, con su menú | Que no se abra |
| El aviso dice que **no tiene permiso** | Que diga «error del servicio» |
| **Cero filas** | **Que muestre los datos** |

> **Si mostrara los datos, el control estaba en el menú — y no era control de
> acceso: era decoración.**

### Y una advertencia práctica

**Recargar con F5 cierra la sesión.** Es el costo de que el token no baje al
navegador: vive en el circuito, y el F5 lo tumba. Está en
[3_plan.md](3_plan.md) §4.1 con su razón.

---

## 11. Si algo falla

| Síntoma | Qué pasa |
|---|---|
| **Iniciar sesión responde 401 con las contraseñas de la tabla** | La base no se re-sembró. `docker compose down -v` y `up -d --build` |
| **Todo responde 401, incluso con token** | Falta la palabra **`Bearer`** y el espacio antes del token |
| **Todo responde 200, incluso sin token** | `app.UseAuthentication()` no está, o está **después** de `UseAuthorization()` |
| **Un token vencido responde 200 durante cinco minutos** | Falta `ClockSkew = TimeSpan.Zero`. ASP.NET perdona 5 minutos de reloj por defecto |
| **El 401 llega con el cuerpo vacío** | Falta el `OnChallenge`. Es lo que ASP.NET hace por defecto |
| **Un rol sin permiso responde 200** | Falta `[ExigePermiso]` en ese controlador. Se revisan los 12 |
| **Todo responde 403, incluso el administrador** | El nombre de la ruta del atributo no está en la tabla `ruta`. Se compara con `SELECT ruta FROM ruta;` |
| **El menú no cambia al identificarse** | El `@rendermode` está en cada interfaz y no en `App.razor`: el layout se quedó estático y no ve el `EstadoSesion` del circuito |
| **La API arranca y muere** | La clave `Jwt:Key` falta o tiene menos de 32 caracteres. El mensaje lo dice |
| **La sesión se cae al recargar** | Es lo esperado. Ver §10 |
