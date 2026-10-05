# Modelo de datos — Versión 3: las tablas del acceso (ya existen)

> **La v3 no crea ni modifica una sola tabla.** Las cinco tablas del control de
> acceso están en la base de datos desde la v1, y su CRUD se construyó en la v1 y la v2.
>
> Lo único que cambia en la base de datos es **la semilla de `usuario`**: sus
> contraseñas se vuelven a sembrar con hash.

---

## 1. Las cinco tablas, y qué pregunta responde cada una

| Tabla | Qué guarda | Su CRUD es de |
|---|---|---|
| `usuario` | Las personas que entran, con su contraseña **con hash** | **v1** |
| `rol` | Los roles | **v1** |
| `ruta` | Las interfaces y acciones **protegibles** | **v1** |
| `rol_usuario` | **Quién tiene qué rol** (puente usuario ↔ rol) | **v2** |
| `rutarol` | **Qué rol entra a qué interfaz** (puente rol ↔ ruta) | **v2** |

```
   PERSONA  ────────►  ROL  ────────►  PERMISO
   usuario           rol            ruta
        └ rol_usuario ┘  └ rutarol ──┘
```

**Eso es RBAC** (*Role-Based Access Control*): los permisos no se le dan a las
personas sino **a los roles**, y las personas se ponen en roles.

> **Por qué no permiso directo a la persona:** porque con cincuenta empleados y
> quince interfaces hay **750** decisiones que mantener. Con roles hay quince,
> y entra alguien nuevo asignándole uno.

## 2. Lo que la v3 cambia en la base de datos: la semilla

**Dos problemas, y el segundo es el que impedía cerrar la versión:**

| | |
|---|---|
| **1** | Dos de las ocho filas tenían la contraseña **en texto plano** |
| **2** | Las otras seis tenían hash, **y nadie sabía de qué**. Un hash va en una sola dirección: del hash no se vuelve a la clave. **Sin saberlas no hay forma de iniciar sesión** — y sin iniciar sesión no se comprueba un solo criterio |

Las ocho se vuelven a sembrar con bcrypt costo 12, **de contraseñas escritas en
[7_quickstart.md](7_quickstart.md)**.

> **Y va EN EL SCRIPT, no a mano.** El siguiente `docker compose down -v`
> vuelve a sembrar, y un cambio hecho a mano se perdería.

### La columna, que ya estaba lista

```sql
contrasena VARCHAR(200)
```

**200 y no 20**, precisamente porque un hash de bcrypt ocupa **60** caracteres.
El tamaño de la columna ya anticipaba esta versión.

## 3. `verificar_acceso_ruta` — el procedimiento que por fin se usa

```sql
-- en db/bdfacturas.sql
SELECT EXISTS(
    SELECT 1
    FROM usuario u
    INNER JOIN rol_usuario ur ON u.email    = ur.fkemail
    INNER JOIN rutarol     rr ON ur.fkidrol = rr.fkidrol
    WHERE u.email = p_email AND rr.fkidruta = p_fkidruta
)
```

**Tres `JOIN` que recorren `persona → rol → permiso`**, y devuelve un JSON con
`tiene_acceso`.

| | |
|---|---|
| **Recibe el ID de la ruta, no su nombre** | Así que hay un paso antes: traducir `interfaz.usuarios` al id. Lo hace el repositorio, con un `SELECT id FROM ruta WHERE ruta = @nombre` |
| **Si la ruta no está en la tabla, NADIE entra** | Y es lo correcto: una ruta que no se declaró no se concedió. **Fallar cerrado, no abierto** |

> **Estaba ahí desde el primer día, sin que nadie lo llamara.** Eso es lo que
> cambia en la v3: no se agrega una tabla, **se empieza a preguntar**.

## 4. Los valores sembrados: los permisos, tal como están

### Las 15 rutas

```
interfaz.inicio      interfaz.usuarios    interfaz.facturas    interfaz.clientes
interfaz.vendedores  interfaz.personas    interfaz.empresas    interfaz.productos
interfaz.roles       interfaz.permisos    interfaz.rutas
permiso.crear        permiso.eliminar     ruta.crear           ruta.eliminar
```

> **Los nombres llevan PUNTO y no barra** —`interfaz.productos`, no
> `/productos`— y es deliberado: con barra se confunden con los endpoints de la
> API, que son otra cosa. Estos son **interfaces y acciones protegibles**.

### Los cinco roles, y a qué entra cada uno

| Rol | Entra a |
|---|---|
| **Administrador** | **las 15** |
| **Vendedor** | inicio · facturas · clientes |
| **Cajero** | inicio · facturas |
| **Contador** | inicio · clientes · productos |
| **Cliente** | inicio · productos |

### Los tres usuarios con los que se prueba

| Correo | Rol | Sirve para |
|---|---|---|
| `admin@correo.com` | Administrador | Que todo responda **200** |
| `vendedor1@correo.com` | Vendedor + Cajero | **El 403**: pedir `/api/usuario` con su token |
| `cliente1@correo.com` | Cliente | **El contraste**: entra a productos y no a clientes, al revés que el vendedor |

> **Con esos tres el criterio 9 es demostrable**, y es el que no se puede
> simular: identificarse como `vendedor1` y **escribir la dirección a mano**.
> Tiene que responder **403**.

## 5. La v3 NO agrega tablas, y conviene decirlo tres veces

| Lo que se podría creer que hace falta | Por qué no |
|---|---|
| Una tabla `sesion` | El token **no se guarda**: está firmado y se valida con la clave. Guardarlo sería tener estado que no hace falta |
| Una tabla `intentos_fallidos` | Bloquear tras N intentos es un requisito real que **el curso no plantea** |
| Una tabla `auditoria` | Ídem |
| Una columna `salt` en `usuario` | **bcrypt lo trae dentro del propio hash.** Los 29 primeros caracteres de `$2a$12$…` son la variante, el costo y el salt |
