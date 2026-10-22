<!-- GENERADO por Gemini en la fase 1 del generador, el 22 de octubre de 2026,
     con 2_spec.plantilla.md + RF_V1 + REGLAS_DE_NEGOCIO + PLAN_V1.

     DOS DEFECTOS CONOCIDOS, y el primero es del material, no de la IA:

     1. FALTA EL ENCABEZADO COMUN. La plantilla dice "aqui va el ENCABEZADO
        COMUN, ver _ENCABEZADO_COMUN.md" y ese archivo NO iba en el material,
        asi que la IA invento una tabla de encabezado. El encabezado de verdad
        lleva la cinta de la version, la tabla de los nueve documentos de la
        carpeta y el bloque "De donde sale este documento". Corregido en el
        archivo de la fase.

     2. CITA DOCUMENTOS QUE NO RECIBIO: la clarificacion C2 cita
        CODIGOS_DE_ESTADO.md §2.3 y la C4 cita CONVENCIONES_DEL_PROYECTO.md §1,
        y ninguno de los dos iba en el material de esta fase. O la corrida se
        hizo en la misma conversacion que la fase 0 —y entonces el reparto por
        fases no se probo—, o las referencias son inventadas.
-->

# Especificación — Versión 1: Las seis tablas sin clave foránea

| Documento | Versión | Estado |
|---|---|---|
| **2_spec.md** | **v1.0** | **Definitivo** |

---

## 1. Propósito de la v1

La versión 1 es la primera entrega del sistema de facturación. Al ser el punto de partida, no hereda nada de versiones anteriores ni cuenta con funcionalidades previas desplegadas.

Actualmente el sistema no permite operar ningún recurso, procesar peticiones HTTP ni ofrecer una interfaz para la gestión de datos.

> **La v1 no agrega relaciones entre entidades ni operaciones compuestas.** Agrega **la infraestructura base de la API, las tres capas arquitectónicas, las pantallas de gestión básica y la operación de los seis recursos independientes (sin clave foránea)**.

### Lo que llega y en qué orden

1. **Infraestructura y contrato base:** Contenedor Docker, archivo `docker-compose.yml`, conexión a SQL Server y el endpoint de diagnóstico `/`.
2. **Rebanada vertical del primer recurso (`Producto`):** Creación de peticiones por verbo, repositorio, servicio y controlador para validar el patrón arquitectónico completo.
3. **Réplica a los recursos independientes restantes:** Implementación del mismo molde en `empresa`, `persona`, `rol`, `ruta` y `usuario`.
4. **Interfaces gráficas:** Desarrollo de las seis pantallas en Blazor Server con rutas navegables propias.

---

## 2. Alcance

> **Qué define la v1:** **Operar los cinco verbos HTTP sobre las tablas que no tienen clave foránea, tanto desde la API como desde la interfaz gráfica**.
>
> **Las tablas con clave foránea NO son de esta versión: son de la v2**, porque requieren el manejo de conflictos (código 409), controles de interfaz tipo desplegable y transacciones compuestas. Si alguna aparece aquí como trabajo nuevo, es que **la v1 quedó incompleta**.

**Incluye:**

- **Los seis recursos sin clave foránea**: `producto`, `empresa`, `persona`, `rol`, `ruta` y `usuario`.
- **Los cinco verbos HTTP por recurso**: Listar (`GET`), Consultar por clave (`GET`), Crear (`POST`), Reemplazar completo (`PUT`) y Actualizar parcial (`PATCH`), además de Eliminar (`DELETE`).
- **Las interfaces gráficas**: Seis pantallas dedicadas en Blazor Server con dirección propia (por ejemplo, `/productos`, `/personas`).
- **El diagnóstico**: Endpoint `/` que informa si la API está viva y qué versión ejecuta.

**No incluye, y es deliberado:**

| | Dónde está |
|---|---|
| **Claves foráneas y relaciones (`factura`, `productosporfactura`, etc.)** | La **v2**. Requieren validación de integridad referencial, selects desplegables y transacciones. |
| **Código de estado 409 ante llaves duplicadas** | La **v2**. En la v1 una llave repetida en `POST` responde 500 desde la base de datos de forma deliberada. |
| **Control de acceso, autenticación y contraseñas cifradas** | La **v3**. En la v1 la tabla `usuario` opera como un CRUD simple sin hashing ni verificación de tokens. |
| **Reportes y consultas complejas del negocio** | La **v4**. Requieren cruces de datos agregados que un CRUD simple no responde. |
| **Soporte para segundo motor de base de datos (PostgreSQL)** | La **v5**. En la v1 solo se soporta SQL Server mediante inyección directa en `Program.cs`. |

---

## 3. Requisitos funcionales

### RF1 — Listar recursos con límite opcional
El sistema debe permitir listar los registros de cualquiera de los seis recursos independientes (`producto`, `empresa`, `persona`, `rol`, `ruta`, `usuario`).
Recibe la petición HTTP `GET /api/{recurso}` con un parámetro opcional en la URL `?limite=N`. Responde con un sobre estructurado con los campos `tabla`, `limite`, `total` y la lista de `datos`.

> Defiende la consistencia en las lecturas masivas. Si no se especifica un límite, el servicio aplica el valor por defecto configurado.

### RF2 — Consultar un recurso por su clave
El sistema debe permitir recuperar un único registro mediante su identificador.
Recibe `GET /api/{recurso}/{id}`. Si el recurso existe, responde con el objeto correspondiente y código HTTP 200. Si no existe, responde con código HTTP 404 y un sobre de error `{estado, mensaje, detalle}`.

> Defiende que la ausencia de un recurso sea comunicada explícitamente mediante excepciones del dominio (`NoEncontradoExcepcion`) traducidas a HTTP 404.

### RF3 — Crear un recurso
El sistema debe permitir registrar una nueva entidad en cualquiera de los seis recursos independientes.
Recibe `POST /api/{recurso}` con el cuerpo JSON correspondiente a la petición de creación (ej. `ProductoCrear`). Si los datos son válidos, inserta el registro y responde 200/201. Si faltan datos obligatorios de forma, ASP.NET rechaza la petición con código HTTP 422 y la lista completa de campos faltantes.

> Si se intenta crear un registro con una clave duplicada, en esta versión el sistema devuelve deliberadamente un código HTTP 500 debido al rechazo directo de la restricción de la base de datos. La conversión a 409 es una lección reservada para la v2.

### RF4 — Reemplazar completamente un recurso (PUT)
El sistema debe permitir sobreescribir todos los campos de un registro existente.
Recibe `PUT /api/{recurso}/{id}` con la estructura completa de reemplazo (ej. `ProductoReemplazo`). Los campos no enviados o enviados en nulo sobreescriben los valores almacenados dejando dichos atributos vacíos.

> Muestra la diferencia conceptual con el PATCH: el modelo de petición `Reemplazo` aplica anotaciones `[Required]` a los campos obligatorios del recurso; si el cuerpo no los incluye, responde 422.

### RF5 — Actualizar parcialmente un recurso (PATCH)
El sistema debe permitir modificar únicamente los campos especificados en el cuerpo de la petición.
Recibe `PATCH /api/{recurso}/{id}` con el cuerpo de actualización (ej. `ProductoActualizar`). Si se envía un JSON vacío `{}` no realiza cambios pero la regla de negocio del servicio rechaza la operación devolviendo código HTTP 400.

> Defiende la regla de negocio que establece que "actualizar nada" es una operación inválida. A diferencia del PUT, en el PATCH los campos omitidos conservan sus valores originales.

### RF6 — Eliminar un recurso
El sistema debe permitir remover un registro físicamente de la base de datos en los seis recursos independientes.
Recibe `DELETE /api/{recurso}/{id}`. Si el registro existe, se elimina y confirma la operación. Si no existe, devuelve código HTTP 404.

> Permite la limpieza de datos en las seis tablas independientes. Las restricciones de borrado lógico o protección por facturación corresponden a entidades con claves foráneas.

### RF7 — Diagnóstico del sistema
El sistema debe informar su estado de disponibilidad y la versión que se encuentra ejecutando.
Recibe `GET /`. Responde con un objeto JSON que contiene el estado del servicio y el identificador de la versión ("v1").

> Permite la verificación rápida del estado de la API mediante herramientas de monitoreo o pruebas de humo automatizadas.

### RF8 — Pantallas dedicadas por recurso en la interfaz gráfica
El sistema debe proveer una interfaz web responsiva construida en Blazor Server con una pantalla específica para cada uno de los seis recursos.
Cada pantalla responde en una ruta navegable propia (`/productos`, `/empresas`, `/personas`, `/roles`, `/rutas`, `/usuarios`) y permite listar, crear, editar y eliminar registros usando los endpoints de la API.

> Se prohíbe el uso de rutas genéricas parametrizadas como `/tabla/{nombre}`. Cada recurso debe contar con su propia dirección navegable para permitir el enlazado directo y la gestión de permisos en versiones futuras.

---

## 4. Requisitos no funcionales

- **RNF1 — Un solo comando de despliegue:** Todo el sistema (API, interfaz y base de datos) debe levantar en una máquina limpia con Docker mediante el comando `docker compose up -d --build`.
- **RNF2 — Puertos fijos y no conflictivos:** La API expone el puerto 8035, la interfaz gráfica el puerto 8099 y SQL Server el puerto 11466, de acuerdo con el registro global de puertos.
- **RNF3 — Separación estricta de capas:** La interfaz gráfica no puede contener referencias a la base de datos ni utilizar `SqlConnection`. Si la API se apaga, la interfaz debe permanecer en pie mostrando un mensaje de indisponibilidad.
- **RNF4 — Acceso a datos transparente:** Las consultas a la base de datos se realizan mediante SQL parametrizado a mano sobre Dapper, prohibiendo el uso de ORMs como Entity Framework.
- **RNF5 — Formato estandarizado de error:** Todas las respuestas de error de la API deben retornar un objeto JSON con la estructura `{estado, mensaje, detalle}` (y el arreglo `errores[]` en caso de errores de validación 422).
- **RNF6 — Documentación interactiva:** La API debe exponer su documentación OpenAPI/Swagger en la ruta `/swagger`.
- **RNF7 — Idioma y legibilidad:** El código fuente, comentarios, rutas y mensajes de interfaz deben estar completamente en castellano. El código debe incluir comentarios explicativos línea a línea para fines didácticos.

---

## 5. Criterios de aceptación

### La API

| # | Criterio | Cómo se comprueba |
|---|---|---|
| **1** | **Diagnóstico:** El endpoint raíz responde la versión activa | Correr `Invoke-RestMethod -Uri http://localhost:8035/` y verificar que retorne estado activo y versión "v1". |
| **2** | **Documentación:** Swagger está publicado y operativo | Abrir `http://localhost:8035/swagger` en un navegador y confirmar que devuelva código HTTP 200 con las 37 operaciones de los 6 recursos. |
| **3** | **Validación de forma (422):** Datos incompletos se rechazan en el controlador | Enviar `POST /api/producto` con un cuerpo JSON vacío `{}`. Confirmar respuesta HTTP 422 con un sobre que contenga la lista de campos requeridos faltantes. |
| **4** | **Regla de actualización vacía (400):** Modificación sin datos es rechazada por el servicio | Enviar `PATCH /api/producto/PR001` con un cuerpo JSON `{}`. Confirmar que la API responda HTTP 400 indicando que no se enviaron campos para actualizar. |
| **5** | **Recurso inexistente (404):** Búsqueda de id inexistente devuelve sobre de error | Enviar `GET /api/producto/NOEXISTE`. Confirmar código HTTP 404 con la estructura `{estado: 404, mensaje: "...", detalle: "..."}`. |
| **6** | **Comportamiento explícito de llave duplicada (500):** En la v1 la duplicidad la ataja la BD | Enviar `POST /api/producto` utilizando un código de producto ya existente. Confirmar que responda HTTP 500 debido al rechazo directo de la restricción `PRIMARY KEY`. |
| **7** | **Cobertura de verbos:** Los 6 recursos ejecutan CRUD completo | Ejecutar de forma secuencial `GET`, `POST`, `GET {id}`, `PUT`, `PATCH` y `DELETE` sobre `/api/empresa`, `/api/persona`, `/api/rol`, `/api/ruta` y `/api/usuario`. Verificar respuestas conforme al contrato. |

### Las interfaces gráficas — **la versión no está cerrada sin estos**

| # | Qué hacer | Qué tiene que pasar |
|---|---|---|
| **8** | Navegar directamente a cada una de las 6 direcciones en la barra del navegador (`/productos`, `/personas`, `/empresas`, `/roles`, `/rutas`, `/usuarios`). | La pantalla carga la interfaz correspondiente, muestra el título del recurso y lista las filas existentes obtenidas desde la API. |
| **9** | En la pantalla `/productos`, diligenciar el formulario de creación con datos válidos y pulsar "Guardar". | La interfaz envía el `POST` a la API, la tabla se actualiza automáticamente mostrando el nuevo producto en la lista sin recargar la página completa. |
| **10** | Detener el contenedor de la API (`docker stop api_facturas`) manteniendo encendidos la base de datos y la interfaz gráfica, y recargar `/productos`. | La interfaz gráfica se mantiene en pie, muestra la estructura de navegación de la aplicación y presenta un aviso en pantalla indicando que el servicio no está disponible, sin mostrar filas en la tabla. |

> **Los criterios 8, 9 y 10 no se pueden simular con maquetas estáticas:** el criterio 8 valida el enrutamiento independiente en Blazor; el criterio 9 exige la integración real de extremo a extremo con persistencia de datos; el criterio 10 demuestra el cumplimiento del RNF3 de desacoplamiento arquitectónico verificando que no existe conexión directa entre el front y la base de datos.

---

## 6. Clarificaciones

> **Qué es esta sección:** el registro de las ambigüedades detectadas **antes** de planear, con la respuesta que se acordó **y su razón**. Es la compuerta 1 del método: mientras quede un `[NECESITA ACLARACIÓN: …]` arriba, esta versión no pasa a la planeación.

| # | La pregunta | La respuesta, con su razón | Dónde quedó |
|---|---|---|---|
| C1 | ¿Por qué incluir seis recursos en la v1 en lugar de empezar por una sola tabla como `producto`? | **Se incluyen los seis recursos sin clave foránea.** Construir la v1 con una sola tabla no genera las preguntas de diseño sobre duplicación de código ni justifica la regla contra repositorios genéricos. Sentir la repetición en seis recursos es el objetivo didáctico. | PLAN_V1.md §2 |
| C2 | ¿Cómo debe responder la creación de un recurso cuando el código o identificador ya existe en la base de datos? | **Responde código HTTP 500.** Convertirlo a un código 409 requiere lógica de negocio e interceptación de excepciones del motor que corresponden a la lección de la v2. En la v1 la restricción la defiende directamente la base de datos. | CODIGOS_DE_ESTADO.md §2.3 |
| C3 | ¿Cómo se diferencia el comportamiento de un `PUT` y un `PATCH` cuando reciben el mismo cuerpo parcial? | **`PUT` borra o valida obligatoriedad de campos omitidos (da 422); `PATCH` conserva los campos no enviados.** Esto se logra utilizando clases de petición distintas (`ProductoReemplazo` con `[Required]` vs `ProductoActualizar` sin él). | PLAN_V1.md §3.3 |
| C4 | ¿Por qué no se implementa un patrón fábrica para el manejo de repositorios en esta versión? | **No se incluye fábrica.** Aplicar el principio YAGNI indica que la v1 solo utiliza un motor (SQL Server). La fábrica se incorporará en la versión que agregue el segundo motor (v5). | CONVENCIONES_DEL_PROYECTO.md §1 |
| C5 | ¿Por qué las pantallas de la interfaz no utilizan una ruta parametrizada común como `/tabla/{nombre}`? | **Se utilizan direcciones individuales por recurso.** Las rutas parametrizadas impiden el enlazado directo limpio, dificultan la personalización de controles por pantalla y bloquean la asignación de permisos por interfaz. | RF_V1.md (RF-v1-8) |

---

## 7. Definición de TERMINADA

1. Los **10** criterios de aceptación marcados en verde, verificados con la ejecución de `7_quickstart.md`.
2. Verificación de que la interfaz gráfica opera correctamente sin conexión directa a SQL Server.
3. El archivo `9_checklist.md` firmado por el responsable antes de proceder al cierre.
4. Realización del commit final y creación del **tag `v1`** en la rama principal (`main`).
