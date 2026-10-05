# Control de acceso — conceptos, ejemplos y referencias

> Lo que hay que entender **antes** de escribir la v3. Son tres ideas, y el
> orden importa: cada una sin la anterior es decoración.
>
> Los ejemplos salen de **este** repositorio: `bdfacturas`, sus tablas
> `usuario`, `rol`, `rol_usuario`, `ruta`, `rutarol`, y su procedimiento
> `verificar_acceso_ruta`.


![Las dos puertas: el 401 no sabe quién es usted; el 403 sabe quién es y no lo deja pasar](img/401_contra_403.svg)

---

## 1. Dos palabras que se confunden siempre

| | Pregunta que responde | En inglés |
|---|---|---|
| **Autenticación** | **¿Quién es usted?** | *authentication* |
| **Autorización** | **¿Qué puede hacer?** | *authorization* |

Se confunden porque en inglés las dos empiezan igual —y de ahí que se escriban
`authn` y `authz` para distinguirlas—. Pero son dos momentos distintos:

```
   credenciales        token           permiso
        │                │                │
   AUTENTICA  ──────►  SESIÓN  ──────►  AUTORIZA
   ¿quién es?        (lo recuerda)     ¿puede esto?
```

**Y de ahí salen los dos códigos HTTP que todo el mundo intercambia:**

| | Significa | Cuándo |
|---|---|---|
| **401** `Unauthorized` | **«No sé quién es usted»** | No llegó el token, o es inválido o venció |
| **403** `Forbidden` | **«Sé quién es, y no puede»** | El token es válido, pero su rol no tiene ese permiso |

> **El `401` está mal nombrado, y de ahí la confusión:** se llama
> *Unauthorized* pero significa *no autenticado*. El RFC 9110 §15.5.2 lo
> define sin ambigüedad —*«la petición no se aplicó porque **le faltan
> credenciales de autenticación válidas**»*— pero el nombre quedó del
> estándar viejo y nadie lo cambió.
>
> El `403`, en cambio, es exactamente lo que dice: prohibido, y el servidor
> **ya sabe quién pregunta**.

**El ejemplo concreto en este proyecto:**

```
GET /api/usuario   sin token               → 401
GET /api/usuario   con token de un vendedor → 403
```

El segundo es un usuario **perfectamente identificado**. Lo que no tiene es
permiso sobre `interfaz.usuarios`.

---

## 2. La contraseña no se guarda: se guarda su huella

**Una contraseña en la base de datos es una contraseña perdida.** Quien lea la tabla
—por una copia de seguridad mal guardada, por una inyección SQL, por un
empleado— se las lleva todas. Y como la gente reutiliza contraseñas, se lleva
también las de otros sistemas.

**Lo que se guarda es un *hash*:** el resultado de una función que va en una
sola dirección.

| | |
|---|---|
| **Fácil** | De `"clave123"` sacar el hash |
| **Imposible en la práctica** | Del hash volver a `"clave123"` |
| **Cómo se verifica entonces** | Se vuelve a calcular el hash de lo que la persona escribió y se comparan **los hashes** |

### Pero no cualquier hash

`SHA-256` y `MD5` **no sirven para contraseñas**, y la razón es contraintuitiva:
**son demasiado rápidos**. Una tarjeta gráfica calcula miles de millones por
segundo, así que probar todas las contraseñas de ocho caracteres es cuestión de
horas.

Las funciones para contraseñas se diseñaron **para ser lentas a propósito**:

| | Qué aporta |
|---|---|
| **bcrypt** (1999) | Lenta y con **costo ajustable**: el día que las máquinas sean más rápidas, se sube el costo sin cambiar de función |
| **Argon2** (2015) | Ganadora de la *Password Hashing Competition*. Además de lenta, **gasta memoria** — y la memoria es lo que una GPU no tiene de sobra |

**Y las dos traen *salt* incorporado.** El *salt* es un valor aleatorio que se
mezcla con la contraseña: sirve para que **dos personas con la misma clave
tengan hashes distintos**. Sin él, ver dos hashes iguales en la tabla delata
que esos dos usuarios comparten contraseña.

```
sin salt:  "clave123" -> siempre el mismo hash
con salt:  "clave123" -> $2a$11$Xq…   (Ana)
           "clave123" -> $2a$11$9kP…  (Luis)
```

> **En este proyecto:** la columna `usuario.contrasena` es `VARCHAR(200)` —y no
> 20— precisamente porque un hash de bcrypt ocupa 60 caracteres. El tamaño de
> la columna ya anticipaba esta versión.
>
> **La semilla de `db/bdfacturas.sql` tiene contraseñas en claro.**
> Cerrar la v3 implica volver a sembrarlas con hash, **en el script** — no a
> mano, o el siguiente `docker compose down -v` las devuelve a texto plano.

---

## 3. La sesión: el token que la petición lleva encima

HTTP **no tiene memoria**: cada petición llega como si fuera la primera. Si la
segunda tiene que saber quién hizo la primera, hay que llevar la prueba encima.

**Un JWT** (*JSON Web Token*, RFC 7519) es esa prueba: un texto en tres partes
separadas por puntos.

```
eyJhbGciOiJIUzI1NiJ9 . eyJlbWFpbCI6ImFkbWluQGNvcnJlby5jb20ifQ . 4f2c9a…
└──── cabecera ────┘   └──────────── contenido ────────────┘   └ firma ┘
```

| Parte | Qué lleva |
|---|---|
| **Cabecera** | Con qué algoritmo está firmado |
| **Contenido** | Los datos: el correo, los roles, cuándo vence |
| **Firma** | Lo que impide alterarlo |

### La propiedad que importa, y la que engaña

| | |
|---|---|
| **Está FIRMADO** | Si alguien cambia una letra del contenido, la firma deja de cuadrar y el servidor lo rechaza |
| **NO está cifrado** | **El contenido se lee sin ninguna clave.** Es `base64`, no un secreto |

> **Es el error más común con JWT: meter algo privado en el contenido.** La
> cédula, el salario, el diagnóstico médico. Cualquiera que vea el token lo
> lee pegándolo en una página web. Lo que el token garantiza es que **nadie lo
> alteró**, no que nadie lo vea.

### Y el otro error: los permisos dentro del token

Parece eficiente guardar los permisos en el token y no volver a consultarlos.
**El problema aparece el día que hay que quitar uno:**

```
09:00  Ana recibe un token que dice: roles = [administrador]
09:30  se le quita el rol de administrador en la base de datos
09:31  Ana sigue entrando: su token todavía dice que lo es
       …hasta que venza, dos horas después
```

**Por eso el permiso se consulta cuando se usa, no cuando se inicia sesión.**
Es el criterio 7 de la v3, y está escrito justamente para forzar esta
decisión.

---

## 4. El permiso: RBAC, y en este proyecto ya está hecho

**RBAC** (*Role-Based Access Control*) es la idea de no darle permisos a las
personas sino **a los roles**, y poner a las personas en roles.

```
   PERSONA  ──►  ROL  ──►  PERMISO
    Ana      administrador   interfaz.usuarios
    Luis     vendedor        interfaz.facturas
```

**Por qué no permiso directo a la persona:** porque con cincuenta empleados y
veinte interfaces hay mil decisiones que mantener. Con roles hay veinte, y
entra alguien nuevo asignándole uno.

### Las tres tablas de este proyecto son exactamente eso

| Tabla | Qué guarda |
|---|---|
| `usuario` | Las personas |
| `rol` | Los roles |
| `rol_usuario` | **Quién tiene qué rol** (la tabla puente persona ↔ rol) |
| `ruta` | Las interfaces y acciones protegibles: `interfaz.productos`, `permiso.crear`… |
| `rutarol` | **Qué rol entra a qué interfaz** (la puente rol ↔ permiso) |

**Y la pregunta se responde con un procedimiento que ya existe:**

```sql
-- verificar_acceso_ruta, en db/bdfacturas.sql
SELECT EXISTS(
    SELECT 1
    FROM usuario u
    INNER JOIN rol_usuario ur ON u.email  = ur.fkemail
    INNER JOIN rutarol     rr ON ur.fkidrol = rr.fkidrol
    WHERE u.email = p_email AND rr.fkidruta = p_fkidruta
)
```

Tres `JOIN` que recorren `persona → rol → permiso`. **La API no los escribe**:
llama al procedimiento. Repetir ese `JOIN` en C# dejaría la regla en dos
sitios, y el día que cambie, cambia en uno.

---

## 5. El error que invalida todo lo anterior

> **Esconder una entrada del menú NO es control de acceso.**

Es la trampa en la que cae casi todo el mundo, porque *parece* que funciona: el
vendedor entra, no ve «Usuarios», y se va tranquilo.

**Pero el menú es HTML que ya está en su navegador.** Quien escriba la
dirección a mano —o abra las herramientas del navegador, o use `curl`— entra
igual, si el servicio no comprueba.

```
el menú no muestra /usuarios   ·   pero esto responde 200:
    curl -H "Authorization: Bearer <token de vendedor>" .../api/usuario
```

| | |
|---|---|
| **El menú** | **Comodidad.** No le muestro a alguien lo que no va a poder usar |
| **El servicio** | **La protección.** Comprueba en cada operación, sin excepción |

> **Cómo se comprueba, y es el criterio 9 de la v3:** identifíquese con un rol
> sin permiso y **escriba la dirección a mano**. Si responde `403`, el control
> está en el servicio. Si muestra los datos, estaba en el menú — y no era
> control de acceso: era decoración.

---

## 6. El resumen, en seis líneas

| | |
|---|---|
| **1** | **Autenticar** es «quién es»; **autorizar** es «qué puede». `401` y `403` |
| **2** | La contraseña se guarda **con hash lento y con salt** — bcrypt o Argon2, nunca SHA-256 |
| **3** | El **token** le da memoria a HTTP. Está **firmado**, no cifrado: no se le mete nada privado |
| **4** | Los **permisos NO van en el token**: se consultan al usar, o quitarlos no surte efecto |
| **5** | **RBAC**: los permisos se le dan al rol, no a la persona |
| **6** | **La protección está en el servicio.** El menú escondido no protege nada |

---

## 7. Referencias

1. **RFC 9110** — *HTTP Semantics*, §15.5.2 (401) y §15.5.4 (403): la fuente
   normativa de la diferencia entre los dos códigos, y donde se admite que el
   nombre de 401 es engañoso.
   <https://www.rfc-editor.org/rfc/rfc9110>
2. **RFC 7519** — *JSON Web Token (JWT)*: la estructura de las tres partes y
   qué garantiza la firma.
   <https://www.rfc-editor.org/rfc/rfc7519>
3. **Provos, N. & Mazières, D.** — *A Future-Adaptable Password Scheme*
   (USENIX, 1999): el artículo que introdujo **bcrypt**, con el argumento del
   costo ajustable.
4. **Biryukov, A., Dinu, D. & Khovratovich, D.** — *Argon2: New Generation of
   Memory-Hard Functions for Password Hashing and Proof-of-Work Applications*
   (IEEE European Symposium on Security and Privacy, 2016).
   DOI **`10.1109/eurosp.2016.31`**: por qué gastar memoria frena a una GPU.
   Estandarizado después como **RFC 9106** (DOI `10.17487/rfc9106`).
5. **Sandhu, R., Coyne, E., Feinstein, H. & Youman, C.** — *Role-Based Access
   Control Models* (IEEE Computer, 1996). DOI **`10.1109/2.485845`**: el
   artículo que formalizó RBAC y la razón de poner un rol en el medio.
6. **NIST SP 800-63B** — *Digital Identity Guidelines*, §5.1.1: las
   recomendaciones vigentes sobre almacenamiento de contraseñas — y por qué ya
   **no** se recomienda forzar cambios periódicos.
   <https://pages.nist.gov/800-63-3/sp800-63b.html>
7. **OWASP** — *Broken Access Control*, primer puesto del Top Ten 2021: el
   fallo más frecuente en aplicaciones reales es justamente el de §5.
   <https://owasp.org/Top10/A01_2021-Broken_Access_Control/>
8. **En este repositorio:** `verificar_acceso_ruta` y las tablas del acceso en
   [`db/bdfacturas.sql`](../../db/bdfacturas.sql); los diez
   criterios en
   [la spec de la v3](../spec_kit/versiones/v3_control_acceso/2_spec.md).
