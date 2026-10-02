# Especificación — Versión 3: el control de acceso

> | | |
> |---|---|
> | **La ruta** | [0_mapa_versiones.md](../0_mapa_versiones.md) |
> | **Lo anterior** | [v1](../v1_sin_fk/2_spec.md) las seis sin FK · [v2](../v2_con_fk/2_spec.md) las seis con FK — **las 12 tablas ya están** |
> | **Lo que rige** | [1_constitution.md](../../1_constitution.md) |

## 1. Propósito de la v3

**Ponerle la puerta a lo que ya existe.**

Con la v2 cerrada, las **12 tablas** de `bdfacturas` son operables por la API
y tienen su interfaz gráfica. Cualquiera que llegue a la dirección puede
hacer cualquier cosa. **Eso es lo que la v3 arregla.**

> **Lo que la v3 NO hace es CRUD.** `usuario`, `rol`, `rol_usuario`, `ruta` y
> `rutarol` **ya tienen su CRUD** —`usuario`, `rol` y `ruta` desde la v1;
> `rol_usuario` y `rutarol` desde la v2—. Administrar la tabla de permisos y
> **hacerlos valer** son dos cosas distintas, y están en dos versiones
> distintas.
>
> **La v3 no agrega una sola tabla.** Agrega el momento en que el sistema
> pregunta «¿y usted quién es?».

### Las tres cosas que llegan, y en este orden

| | Qué | Por qué en ese orden |
|---|---|---|
| **1** | **La contraseña deja de viajar en claro**: se guarda con un hash y se verifica contra el hash | Sin esto, lo demás es decoración: da igual quién entre si la clave está a la vista |
| **2** | **La sesión**: quien se identifica recibe un token (JWT) que acompaña cada petición | Es lo que permite que la segunda petición sepa quién hizo la primera |
| **3** | **El permiso**: cada operación comprueba si el rol de ese usuario puede entrar a esa interfaz | Es `verificar_acceso_ruta`, y ya está en la base |

## 2. Alcance

> **Qué define la v3:** **no se agregan tablas ni recursos.** Se le pone
> autenticación y autorización a los que ya están.

**Incluye:**

- **Hash de la contraseña** al crear y al cambiar un `usuario`, y verificación
  al identificarse. La columna `contrasena` deja de guardar texto plano.
- **Un endpoint de identificación** que devuelve el token si las credenciales
  son correctas, y el mismo error —sin decir cuál de los dos campos falló—
  si no lo son.
- **El token en cada petición**, y el rechazo de las que no lo traigan.
- **La comprobación de permiso** con `verificar_acceso_ruta`, que cruza
  `usuario → rol_usuario → rutarol`: ese procedimiento **ya existe**, y la
  v3 lo usa en vez de armar el `JOIN` en C#.
- **La interfaz gráfica de identificación**, y el menú que **solo muestra lo
  que ese rol puede abrir**.

**No incluye (y es deliberado):**

- **El CRUD de ninguna tabla**: todos están, de la v1 y la v2. Si algo falta,
  es que una de esas dos quedó incompleta.
- **Consultas multitabla, tablero, manual de marca y publicación**: es la
  **v4**.
- **Recuperación de contraseña, correo de confirmación, segundo factor**: no
  los pide el curso.

## 3. Requisitos funcionales

### RF1 — La contraseña no se guarda en claro

| | |
|---|---|
| **Al crear un usuario** | La contraseña se guarda **con hash**. La columna nunca vuelve a tener texto legible |
| **Al identificarse** | Se compara el hash, no la cadena |
| **Al editar** | Si el campo llega **vacío**, la contraseña **no se cambia**. Vacío significa «déjela como está», no «bórrela» |
| **Qué nunca sale** | El hash tampoco se devuelve en el JSON. Un `GET /api/usuario` no trae la columna |

> **La semilla de la base tiene contraseñas en claro.** Cerrar la v3 implica
> volver a sembrarlas con hash — y eso se hace **en el script**, no a mano.

### RF2 — Identificarse devuelve un token

| | |
|---|---|
| **Qué recibe** | El correo y la contraseña |
| **Si son correctos** | Un token con el correo y los roles de ese usuario |
| **Si no** | **El mismo mensaje**, sin decir si falló el correo o la contraseña. Decir «ese correo no existe» le confirma a un desconocido qué correos sí existen |

### RF3 — Sin token no se entra

Toda operación de la API exige el token, **menos dos**: el diagnóstico y el
propio endpoint de identificación — que no puede exigir lo que todavía no
tiene.

| Caso | Respuesta |
|---|---|
| Sin token | **401** |
| Token vencido o alterado | **401** |
| Token válido pero el rol no puede | **403** |

> **El 401 y el 403 no son lo mismo**, y confundirlos es el error clásico:
> **401 es «no sé quién es usted»; 403 es «sé quién es, y no puede»**.

### RF4 — El permiso lo decide la base

La comprobación usa **`verificar_acceso_ruta`**, que ya existe y cruza
`usuario → rol_usuario → rutarol`.

```
CALL verificar_acceso_ruta(p_email, p_fkidruta, p_resultado)
  -> { "tiene_acceso": true/false, "email": …, "fkidruta": … }
```

**Dos cosas que se van a querer hacer y no se deben:**

| | Por qué no |
|---|---|
| **Armar el `JOIN` de permisos en C#** | El procedimiento ya lo hace. Repetirlo deja la regla en dos sitios, y el día que cambie, cambia en uno |
| **Guardar los permisos en el token** | Entonces quitarle un permiso a alguien no surte efecto hasta que el token venza. El permiso se consulta **cuando se usa** |

### RF5 — El menú solo muestra lo que ese rol puede abrir

La interfaz gráfica pregunta por los permisos del usuario y **arma el menú con
eso**.

> **Y esto es lo importante:** **esconder una entrada del menú NO protege
> nada.** Quien escriba la dirección a mano entra igual, si el servicio no
> comprueba. El menú es comodidad; la protección está en la API.
>
> **Se comprueba así:** identifíquese con un rol que no tenga permiso de
> `interfaz.usuarios`, y escriba la dirección a mano. Tiene que responder
> **403**, no mostrar los datos.

## 4. Criterios de aceptación

| | |
|---|---|
| **1** | La columna `contrasena` **no tiene texto legible** en ninguna fila |
| **2** | Identificarse con credenciales correctas devuelve un token; con incorrectas, el **mismo** mensaje en los dos casos |
| **3** | Una operación **sin token** responde **401** |
| **4** | Un token **alterado** responde **401** |
| **5** | Un usuario con rol sin permiso responde **403** — y la diferencia con el 401 se puede explicar |
| **6** | El permiso lo resuelve **`verificar_acceso_ruta`**: no hay un `JOIN` de permisos escrito en C# |
| **7** | **Quitarle un permiso a un rol surte efecto sin volver a identificarse** — porque el permiso no está en el token |
| **8** | El menú de la interfaz gráfica cambia según el rol |
| **9** | **Escribir la dirección a mano, sin permiso, responde 403** — el menú escondido no era la protección |
| **10** | **Regresión:** los criterios de la v1 y la v2 siguen pasando, **con token** |

> **El criterio 9 es el que no se puede simular.** Los otros ocho se pueden
> cumplir con una interfaz gráfica que esconde botones. Ese no.

## 5. Definición de TERMINADA

1. Los **diez** criterios en verde, verificados con el smoke test de
   [7_quickstart.md](7_quickstart.md).
2. La **regresión** de la v1 y la v2 pasa.
3. `9_checklist.md` firmado **antes** de la primera línea de código.
4. Commit y **tag `v3`** en `main`.
