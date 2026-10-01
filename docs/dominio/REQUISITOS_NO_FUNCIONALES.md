# Requisitos no funcionales — Facturación (`bdfacturas`)

> **Qué es este documento.** Lo que el sistema tiene que **ser**, no lo que tiene
> que hacer. Cada uno con un **número y una forma de comprobarlo** — porque un
> requisito no funcional que no se puede medir no es un requisito, es un deseo.
>
> **El concepto detrás:**
> [`CONCEPTOS_REQUISITOS_NO_FUNCIONALES.md`](../conceptos/CONCEPTOS_REQUISITOS_NO_FUNCIONALES.md).
>
> **De dónde salen.** Casi todos están en
> [`1_constitution.md`](../spec_kit/1_constitution.md), que es donde rigen: la
> constitución es el documento de los requisitos no funcionales de este
> proyecto, y aquí se los recoge con su forma de medirlos.
>
> **Material académico simulado.**
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Cómo leer este documento

| Columna | Qué dice |
|---|---|
| **Requisito** | Qué tiene que ser cierto, **con un número** cuando se puede |
| **Cómo se comprueba** | El comando o la observación. Si no hay ninguna, el requisito está mal escrito |
| **Dónde está escrito** | El artículo de la constitución o el documento que lo fija |

> **La prueba de que un RNF está bien escrito es que alguien pueda decir «no se
> cumple» y tener razón.** «El sistema será rápido» no admite esa frase; «la
> mediana de `GET /api/producto` responde por debajo de 200 ms» sí.

---

## 2. Arranque y entorno

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-01** | El sistema entero se levanta con **un solo comando** | `docker compose up -d --build` en una máquina limpia | Art. 4 |
| **RNF-02** | No hace falta instalar .NET, SQL Server ni nada más: **solo Docker** | Una máquina sin SDK levanta el sistema igual | Art. 4 |
| **RNF-03** | El mismo comando en otro computador da **el mismo resultado** | Dos máquinas distintas, mismo `docker compose up` | Art. 4 |
| **RNF-04** | Los puertos son fijos y **no se repiten** con otros proyectos del semestre | API 8035 · front 8099 · SQL Server 11466 · PostgreSQL 15462, contra `PUERTOS.md` | Art. 8 |

---

## 3. Separación y arquitectura

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-05** | El front **no toca la base de datos** | Apague `api-facturas` con la base encendida: el front sigue en pie, con su menú y **sin una sola fila**. Y `grep SqlConnection front_blazor` da **0** | Art. 3 |
| **RNF-06** | Cada capa depende de una **interfaz**, no de una clase | Los servicios reciben `IRepositorio*` por constructor | Art. 3 |
| **RNF-07** | Cambiar de motor **no toca** controladores ni servicios | `git diff --stat v4..v5 -- api_facturas/Controllers api_facturas/Servicios`: **vacío** | Art. 3 |
| **RNF-08** | El SQL está **a la vista**, sin ORM que lo genere | No hay Entity Framework en el `.csproj`; hay Dapper | Art. 2 |

---

## 4. Contratos y respuestas

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-09** | **Toda** respuesta es JSON, también el error | Ningún error devuelve HTML | Art. 7 |
| **RNF-10** | El sobre de lectura es `{tabla, limite, total, datos}` | `GET /api/producto` | Art. 8 |
| **RNF-11** | El sobre de error es `{estado, mensaje, detalle}`, más `errores[]` en el 422 | `POST /api/producto` con `{}` | Art. 8 |
| **RNF-12** | La API está **documentada e interactiva** | `http://localhost:8035/swagger` responde 200 | Art. 8 |
| **RNF-13** | Lo que `6_contracts.md` declara es **exactamente** lo que la API publica | `auditar_aysw4_por_version.py`, que compara el contrato contra `swagger.json` | Art. 7 |

---

## 5. Seguridad

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-14** | Las contraseñas se guardan **cifradas**, nunca en claro | `SELECT contrasena FROM usuario`: hashes BCrypt | v3 |
| **RNF-15** | Ninguna respuesta devuelve una contraseña ni su hash | `GET /api/usuario` no trae el campo. La clase `Usuario` **no lo tiene** | v3 |
| **RNF-16** | El login responde **lo mismo** para «el correo no existe» y «la clave está mal» | Dos peticiones, misma respuesta 401 y mismo texto | v3, criterio 2 |
| **RNF-17** | El permiso se consulta **en cada petición**, no se lee del token | Quite un permiso a un rol: surte efecto sin volver a entrar | v3 |
| **RNF-18** | **Cero secretos en el código** | La cadena de conexión llega por variable de entorno; `.env` en `.gitignore` | Art. 4 |

> **RNF-18 tiene una excepción declarada**, y conviene no imitarla: en este
> repositorio las credenciales **están quemadas en el `docker-compose.yml`**, y
> es a propósito — es material de clase y tiene que levantar sin configurar
> nada. En el proyecto de aula **no se vale**, y la rúbrica lo califica.

---

## 6. Idioma y legibilidad

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-19** | Todo en castellano: nombres, comentarios y mensajes | Leer cualquier archivo | Art. 6 |
| **RNF-20** | El código está **comentado** para que un principiante lo siga | Cada archivo abre diciendo qué es y qué papel cumple | Art. 6 |
| **RNF-21** | La interfaz **no habla en jerga**: ni «PUT», ni «422», ni «FK» | Recorrer las pantallas | v1 |

> **RNF-20 se califica desde la v2**, y se evalúa **hablando**: el profesor abre
> un archivo y quien lo entregó cuenta qué hace y por qué está así. Ver la
> rúbrica en `ProyectosDeAula/docs/0_METODOLOGIA.md`.

---

## 7. Accesibilidad e imagen

| | Requisito | Cómo se comprueba | Dónde |
|---|---|---|---|
| **RNF-22** | Los colores de la interfaz salen del **manual de marca**, no del gusto de quien programa | Buscar `#rrggbb` fuera de `marca.css`: no debe haber | v4 |
| **RNF-23** | El texto cumple **WCAG AA** (4.5:1) | Calculado en [`MANUAL_DE_MARCA.md`](MANUAL_DE_MARCA.md) §2 | v4 |
| **RNF-24** | La interfaz es **responsive** | Abrirla a ancho de teléfono | v4 |

> **RNF-22 y RNF-23 NO se cumplían cuando se escribió este documento**, y
> escribirlo fue lo que lo destapó: había **cuatro colores de Bootstrap** en la
> hoja del proyecto, y el borde del campo enfocado daba **2.09** de contraste
> cuando WCAG pide 3.0 para un indicador.
>
> Los dos quedaron corregidos — el foco en azul cordillera (8.76) y los cuatro
> colores saliendo de la paleta. Hoy se cumplen, y §6 del manual dice cómo
> volver a contarlos.
>
> Un documento de requisitos que solo lista lo que ya se cumple no sirve para
> nada: el valor está en que la comprobación se pueda correr y falle.

---

## 8. Lo que este sistema NO promete

| No se promete | Por qué |
|---|---|
| Un tiempo de respuesta | No hay medición de carga, y prometerlo sin medirlo sería inventarlo |
| Alta disponibilidad | Un contenedor por servicio, sin réplicas |
| Respaldo automático | Hay un `backupdb/` para hacerlo a mano |
| Escalar a muchos usuarios a la vez | Blazor Server mantiene un circuito por persona |
| Funcionar sin Docker | Se puede, pero no está soportado ni probado |

> **Esta tabla es tan requisito como las otras.** Un sistema que no dice lo que
> no hace deja que cada quien suponga lo que quiera — y el día que falle, la
> culpa será de la suposición de alguien.
