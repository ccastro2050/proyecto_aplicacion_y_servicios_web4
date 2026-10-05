# Investigación y decisiones — Versión 3: el control de acceso

> Las decisiones que se tomaron **y las alternativas que se descartaron, con su
> razón**. En esta versión importa más que en las otras, porque casi todas las
> alternativas «también funcionan» — y fallan de formas que no se ven.

---

## D1 — El hash: ¿SHA-256 o bcrypt?

**bcrypt**, y la razón es contraintuitiva.

| Opción | Argumento |
|---|---|
| **SHA-256** | Está en la librería estándar, no hace falta ningún paquete, y es criptográficamente sólido |
| **bcrypt** ✅ | **Es LENTO a propósito** |

**Lo que decide:** SHA-256 es **demasiado rápido**. Una tarjeta gráfica calcula
miles de millones por segundo, así que probar todas las contraseñas de ocho
caracteres es cuestión de horas. bcrypt se diseñó para ser lento **y con costo
ajustable**: el día que las máquinas sean más rápidas se sube el costo, sin
cambiar de función.

**Y bcrypt trae *salt* incorporado**, que es la otra mitad. El *salt* es un
valor aleatorio que se mezcla con la contraseña, y hace que **dos personas con
la misma clave tengan hashes distintos**:

```
los dos usuarios de carlos.castro comparten la contrasena, y sus hash no se parecen:
  $2a$12$f1UjnYhuaQUrCS8w/EARw...
  $2a$12$F7CLooKrzi/ec4U0iI9.le...
```

Sin *salt*, ver dos hashes iguales en la tabla delataría que esas dos personas
usan la misma contraseña.

> **¿Y Argon2, que ganó la competencia de 2015?** Es mejor —además de lenta,
> **gasta memoria**, y la memoria es lo que una GPU no tiene de sobra—. No se
> usa aquí porque `BCrypt.Net-Next` **ya estaba en el proyecto** desde la v1,
> funcionando. Cambiar de función de hash obligaría a que todos vuelvan a
> poner su contraseña. Queda escrito como la mejora que es.

## D2 — El costo 12: ¿por qué ese número?

| | |
|---|---|
| **Qué significa** | `$2a$12$` — el `12` es el **costo**, y es un exponente: cada punto **duplica** el tiempo |
| **Por qué 12** | Tarda del orden de **un cuarto de segundo** en una máquina de escritorio. Imperceptible para quien inicia sesión una vez; carísimo para quien quiera probar millones |
| **Por qué no 4** | Sería rápido y **gratis de atacar** |
| **Por qué no 20** | Cada inicio de sesión tardaría **minutos** |

> **Y es ajustable sin cambiar de función, que es la gracia de bcrypt:** el día
> que 12 sea poco, se sube a 13 y los hashes viejos **siguen verificando** —el
> costo va escrito dentro del hash—.

## D3 — La sesión: ¿cookie de servidor o JWT?

| Opción | Argumento |
|---|---|
| **Cookie con sesión en el servidor** | **Se puede revocar**: se borra la sesión y listo. Es más seguro |
| **JWT** ✅ | No necesita que el servidor recuerde nada. **Y es lo que el curso enseña** |

**Lo que decide, y conviene ser honesto:** la cookie con estado en el servidor
es **mejor** para una aplicación como esta. El JWT se elige porque es el
mecanismo que el estudiante va a encontrar en todas partes, y porque **no poder
revocarlo es en sí una lección**: obliga a entender por qué la duración es
corta.

> **Lo que se pierde está escrito en el plan:** un token no se puede apagar. Si
> alguien se lo roba, sirve hasta que venza.

## D4 — Los permisos: ¿dentro del token, o se consultan?

**Se consultan.** Es la decisión más importante de la versión.

| Opción | Argumento |
|---|---|
| **En el token** | **Una consulta menos por petición.** El token ya trae todo: se lee y se decide |
| **Consultados** ✅ | **Quitar un permiso surte efecto de inmediato** |

**Lo que decide:**

```
09:00  Ana recibe un token que dice: puede entrar a interfaz.usuarios
09:30  se le quita ese permiso a su rol, en la base de datos
09:31  Ana sigue entrando: su token todavia dice que puede
       ...hasta que venza, una hora despues
```

**Una hora de permiso que ya se le quitó.** Y el caso que importa no es Ana
olvidadiza: es alguien a quien se le retiró el acceso por una razón.

> **Es el criterio 7, y está escrito para forzar esta decisión:** *«quitarle un
> permiso a un rol surte efecto sin volver a identificarse»*. Con los permisos
> en el token, ese criterio **no se puede cumplir**.
>
> **Lo que cuesta:** una consulta a la base de datos por operación. Es el precio, y es
> barato: el procedimiento son tres `JOIN` sobre tablas con índice.

## D5 — El permiso: ¿el `JOIN` en C#, o el procedimiento?

**El procedimiento**, `verificar_acceso_ruta`, que **ya existía en la base de datos
desde el primer día** sin que nadie lo llamara.

| Opción | Argumento |
|---|---|
| **El `JOIN` en C#** | Se lee sin saber plpgsql, y está en el mismo lenguaje que el resto |
| **El procedimiento** ✅ | **La regla vive en un solo sitio** |

**Lo que decide:** repetir el `JOIN` en C# deja la regla del acceso en dos
lugares, y el día que cambie, cambia en uno.

> **Y hay un `JOIN` de permisos escrito en C#, en `RutasPermitidasAsync`.**
> Conviene decir por qué no contradice esto: **no decide nada**. Es una lista
> para dibujar un menú. La **decisión** —si una operación entra o no— la toma
> el procedimiento, y solo él.

## D6 — El 403: ¿un filtro, o una línea en cada método?

**Un filtro**, `[ExigePermiso]`.

| Opción | Argumento |
|---|---|
| **Una línea al principio de cada método** | Se ve dónde está. Nada de «magia» |
| **Un atributo** ✅ | **No se puede olvidar** |

**Lo que decide:** con la línea a mano, el día que alguien escriba un endpoint
nuevo y se le olvide, **ese endpoint queda abierto** — y nadie lo nota, porque
funciona. El atributo está en la declaración del controlador, a la vista de
cualquiera que lo abra.

## D7 — `ClockSkew`: ¿por qué tocarlo?

Por defecto ASP.NET perdona **cinco minutos** de desajuste de reloj al validar
que un token no haya vencido. Es razonable en producción —los relojes de dos
servidores no coinciden al segundo— y **arruina la demostración**: un token
vencido responde **200** durante cinco minutos, y el estudiante concluye que su
código está mal.

**Se pone en cero**, y queda escrito aquí para que la decisión se vea.

## D8 — El token en el navegador: ¿`localStorage` o el circuito?

**El circuito.** En Blazor Server el token puede quedarse **en memoria del
servidor** y no bajar nunca al navegador.

| Opción | Argumento |
|---|---|
| **`localStorage`** | Sobrevive al F5. Es lo que se hace en React, Angular, Blazor WebAssembly |
| **El circuito** ✅ | **El token no baja al navegador.** Ningún script de la página lo puede leer |

> **Lo que se pierde:** recargar con F5 cierra la sesión. Para el curso es el
> cambio correcto, y es una ventaja real de Blazor Server que vale la pena
> nombrar — en un front que corre en el navegador **no hay** esta opción.

## D9 — Lo que NO se investigó, y por qué

| | |
|---|---|
| **Refrescar el token** | Trae su propio problema sin resolver: cómo se revoca el *refresh token*. Con una hora, volver a identificarse alcanza |
| **OAuth / OpenID Connect** | Delegar la identidad a Google o Microsoft es lo que se hace en producción, y **esconde exactamente lo que esta versión existe para enseñar** |
| **Permisos por operación** | La tabla `ruta` trae `permiso.crear` y `permiso.eliminar`, así que la base de datos lo soportaría. Los diez criterios piden protección **por interfaz** |
| **Auditoría** (quién hizo qué) | Es un requisito real que el curso no plantea |
