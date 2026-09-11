# Contratos HTTP — Versión 3: identificación, 401 y 403

> | | |
> |---|---|
> | **La ruta** | [0_mapa_versiones.md](../0_mapa_versiones.md) |
> | **Lo anterior** | [v1](../v1_sin_fk/6_contracts.md) · [v2](../v2_con_fk/6_contracts.md) — **las mismas rutas, los mismos cuerpos** |
> | **La API** | `http://localhost:8035` · Swagger en `/swagger` |
> | **La interfaz gráfica** | `http://localhost:8099` |

---

## Lo que la v3 le hace a los contratos que ya existían

**NADA, en su forma.** Las rutas son las mismas, los cuerpos son los mismos y
las respuestas correctas son las mismas.

**Lo único que se agrega es una exigencia:** todas piden el token, y dos
respuestas nuevas aparecen cuando falta.

| | Significa | Cuándo |
|---|---|---|
| **401** `Unauthorized` | **«No sé quién es usted»** | No llegó el token, o es inválido, o venció |
| **403** `Forbidden` | **«Sé quién es, y no puede»** | El token es válido, pero su rol no tiene ese permiso |

> **El 401 está mal nombrado, y de ahí la confusión eterna:** se llama
> *Unauthorized* y significa *no autenticado*. El **RFC 9110 §15.5.2** lo
> define sin ambigüedad: *«the request has not been applied because it lacks
> valid authentication credentials for the target resource»*. El nombre quedó
> del estándar viejo.
>
> El **403**, en cambio, es exactamente lo que dice: prohibido, y el servidor
> **ya sabe quién pregunta**.

---

## A. `POST /api/sesion` — el único endpoint nuevo

```
body { "email": "admin@correo.com", "contrasena": "admin123" }

-> 200 {
     "estado": 200,
     "mensaje": "Sesion iniciada.",
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "email": "admin@correo.com",
     "roles": ["Administrador"],
     "expira": "2026-10-01T18:32:27Z"
   }

-> 401 { "estado": 401, "mensaje": "El correo o la contrasena no son correctos." }
-> 422 si falta el email o la contrasena
```

**Tres cosas del contrato, y las tres son decisiones:**

| | |
|---|---|
| **Las credenciales van en el CUERPO** | Y no en la URL. Una contraseña en la URL queda en el historial del navegador y en los logs de cualquier proxy del camino |
| **El 401 dice LO MISMO en los dos casos** | Correo inexistente y contraseña equivocada: **el mismo mensaje**. Decir «ese correo no existe» le confirma a un desconocido cuáles sí existen |
| **No hay 404** | Por lo mismo. Un 404 aquí sería un buscador de correos válidos |

> **Los `roles` vienen en la respuesta para que la interfaz arme su menú.** Y
> ojo con la conclusión fácil: **que los roles viajen en la respuesta no
> significa que los permisos viajen en el token** — se consultan al usar.

### El token, que se puede leer sin ninguna clave

```
eyJhbGciOiJIUzI1NiJ9 . eyJzdWIiOiJhZG1pbkBjb3JyZW8uY29tIn0 . 4f2c9a...
└──── cabecera ────┘   └──────── contenido ────────┘   └ firma ┘
```

| Parte | Qué lleva |
|---|---|
| **Cabecera** | Con qué algoritmo está firmado: `HS256` |
| **Contenido** | `sub` (el correo), `role` (los roles), `exp`, `iss`, `aud` |
| **Firma** | Lo que impide alterarlo |

| | |
|---|---|
| **Está FIRMADO** | Cambiar una letra del contenido y la firma deja de cuadrar: **401** |
| **NO está cifrado** | **El contenido se lee sin ninguna clave.** Es base64, no un secreto |

> **Es el error más común con JWT: meter algo privado en el contenido.** La
> cédula, el salario, el diagnóstico. Cualquiera que vea el token lo lee
> pegándolo en una página web. Lo que el token garantiza es que **nadie lo
> alteró**, no que nadie lo vea.
>
> **Y qué NO lleva este token: los permisos.** Solo el correo y los nombres de
> los roles. Ver el criterio 7.

---

## B. Cómo se manda el token

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**La palabra `Bearer`, un espacio, y el token.** Sin la palabra, 401 — y es un
401 que cuesta encontrar, porque el token está bien.

---

## C. Los dos endpoints ABIERTOS, y son solo dos

| | Por qué |
|---|---|
| `GET /` | El diagnóstico. Sirve para saber si la API está viva, y para eso no hace falta identificarse |
| `POST /api/sesion` | **No puede exigir lo que todavía no existe** |

**Todo lo demás exige token.** Las 12 rutas de la v1 y la v2, sin excepción.

---

## D. `GET /api/permisos/mios` — para armar el menú

```
Authorization: Bearer <token>

-> 200 { "email": "cliente1@correo.com", "total": 2,
         "datos": ["interfaz.inicio", "interfaz.productos"] }
-> 401 sin token
```

| | |
|---|---|
| **El correo sale DEL TOKEN, no de la URL** | Si viniera por parámetro, cualquiera podría preguntar por los permisos de otro — y de paso averiguar qué correos existen |
| **Exige token pero NO permiso** | Pedirle permiso para saber sus permisos sería un círculo |

> **Y lo que este endpoint NO es: el control de acceso.** Es una lista para
> dibujar un menú. Quien esconda un botón con esta lista y no proteja el
> controlador, **no protegió nada**.

---

## E. Lo que responde cada rol — la matriz, verificada

**Esto no es un ejemplo: es la salida real de correr los tres tokens contra los
diez recursos.**

| Recurso | `admin` | `vendedor1` | `cliente1` |
|---|---|---|---|
| `/api/producto` | **200** | **403** | **200** |
| `/api/cliente` | **200** | **200** | **403** |
| `/api/factura` | **200** | **200** | **403** |
| `/api/usuario` | **200** | **403** | **403** |
| `/api/rutarol` | **200** | **403** | **403** |
| `/api/persona` | **200** | **403** | **403** |
| `/api/empresa` | **200** | **403** | **403** |
| `/api/rol` | **200** | **403** | **403** |
| `/api/ruta` | **200** | **403** | **403** |
| `/api/vendedor` | **200** | **403** | **403** |

**Y calza exactamente con lo que la base dice:**

```
Administrador  las 15 rutas
Vendedor       inicio, facturas, clientes
Cliente        inicio, productos
```

> **Fíjese en el contraste entre `vendedor1` y `cliente1`:** uno entra a
> clientes y no a productos; el otro al revés. **No es un orden de
> privilegios**, es una matriz — y eso es lo que hace RBAC distinto de «niveles
> de usuario».

### El cuerpo del 403

```json
{
  "estado": 403,
  "mensaje": "Su rol no tiene permiso para esta operacion.",
  "ruta": "interfaz.usuarios"
}
```

> **La `ruta` va en la respuesta a propósito:** le dice a quien programa **qué
> permiso** le hizo falta. Sin eso, un 403 es una pared sin cartel.

### El cuerpo del 401

```json
{ "estado": 401, "mensaje": "No hay una sesion valida. Inicie sesion." }
```

> **Esto hay que escribirlo a mano** (`OnChallenge`). Por defecto ASP.NET
> responde el 401 con el cuerpo **vacío**, y la interfaz no tiene nada que
> mostrarle a la persona.

---

## F. El diagnóstico cambia UNA clave

```
GET /  ->  200 { "mensaje": "API Facturas funcionando", "version": "v3",
                 "contratos": "docs/spec_kit/versiones/v3_control_acceso/6_contracts.md" }
```

---

## G. La traducción de errores, acumulada

| Situación | HTTP |
|---|---|
| El cuerpo no cumple las anotaciones | **422** + `errores[]` |
| Regla de negocio | **400** |
| No existe | **404** |
| Clave foránea, pareja repetida, factura anulada | **409** |
| **Sin token, token alterado o vencido** | **401** ← *nuevo en la v3* |
| **Token válido, rol sin permiso** | **403** ← *nuevo en la v3* |
| Lo imprevisto | **500** + el mensaje del motor |

---

## H. Estabilidad

Estos contratos **se congelan** al cerrar la v3 (commit + tag `v3`).

| | |
|---|---|
| **La v4** | Agrega consultas, tablero, marca y publicación. **Todo eso también va detrás del token** |
| **La v5** | Cambia el motor. El control de acceso **no debería notarlo** — `verificar_acceso_ruta` existe en los dos motores |
