# Especificación — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | **2_spec.md** (este) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la capa de consultas y el tablero |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas: cero tablas nuevas |
> | [6_contracts.md](6_contracts.md) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

## 1. Propósito de la v4

Las tres versiones anteriores dejaron **las doce tablas operables**: se puede
crear, leer, modificar y borrar cada una, con su puerta puesta. Y aun así el
sistema **no responde ninguna pregunta del negocio**.

«¿Cuánto vendió cada vendedor?» no es una fila de ninguna tabla. Es un cruce.

> **La v4 no agrega ni una tabla.** Agrega **las preguntas** — diez consultas
> que cruzan cuatro o más tablas cada una — y **el tablero** donde se ven.

### Por qué esto no lo puede hacer el CRUD

Se podría pedir `/api/factura` y `/api/producto` por separado y juntarlos en el
navegador. Daría el mismo número **a veces**, y falla por tres lados:

| | |
|---|---|
| **Trae todo a la máquina de quien mira** | Para sumar diez facturas hay que bajar las diez con sus renglones |
| **Suma mal en cuanto haya paginación** | El `limite` del contrato corta la lista, y el total sale corto **sin avisar** |
| **Pide muchas veces lo que se responde una** | Una factura, un viaje: es el problema N+1, con nombre propio |

> **El cruce se hace donde están los datos.** Eso no es una preferencia de
> estilo: es la única forma de que el número sea correcto.

## 2. Las diez consultas

| # | Endpoint | Qué cruza |
|---|---|---|
| **1** | `GET /api/consultas/ventas-por-producto` | producto · productosporfactura · factura · cliente |
| **2** | `GET /api/consultas/ventas-por-cliente` | persona · cliente · factura · productosporfactura |
| **3** | `GET /api/consultas/ventas-por-vendedor` | persona · vendedor · factura · productosporfactura |
| **4** | `GET /api/consultas/ventas-por-empresa` | empresa · cliente · factura · productosporfactura |
| **5** | `GET /api/consultas/ticket-por-vendedor` | persona · vendedor · factura · productosporfactura |
| **6** | `GET /api/consultas/productos-sin-vender` | producto · productosporfactura · factura · cliente — con `LEFT JOIN` |
| **7** | `GET /api/consultas/anulaciones-por-cliente` | persona · cliente · factura · productosporfactura |
| **8** | `GET /api/consultas/alcance-de-usuarios` | usuario · rol_usuario · rol · rutarol · ruta — **CINCO** tablas |
| **9** | `GET /api/consultas/interfaces-sin-usuarios` | ruta · rutarol · rol · rol_usuario — con `LEFT JOIN` |
| **10** | `GET /api/consultas/credito-contra-consumo` | persona · empresa · cliente · factura |

> **Todas cruzan CUATRO o más tablas, y dos cruzan CINCO.** No es un requisito
> decorativo: una consulta de tres tablas la puede resolver una vista, y una de
> dos la resuelve un `JOIN` que cabe en un endpoint existente.

## 3. Los criterios de aceptación

| | Criterio | Cómo se comprueba |
|---|---|---|
| **1** | Los 70 endpoints de v1–v3 responden igual | La **regresión**: el smoke test de las tres versiones anteriores, completo |
| **2** | Las diez consultas responden **200** con el sobre `{consulta, total, datos}` | [7_quickstart.md](7_quickstart.md) §2, una por una |
| **3** | Cada una cruza **4 tablas o más** | Se lee el SQL del repositorio y se cuentan los `JOIN` |
| **4** | Las diez exigen **token y permiso** | Sin token: 401. Con un rol sin `interfaz.inicio`: 403 |
| **5** | El tablero las muestra **todas**, y las pide **a la vez** | [7_quickstart.md](7_quickstart.md) §3 |
| **6** | Si una consulta falla, **las otras nueve se dibujan** | Se apaga la API a mitad de carga, o se mira el aviso con el nombre de la que falló |
| **7** | **Cero filas se muestra como respuesta**, no como error | Las consultas 6 y 9 pueden venir vacías, y la interfaz lo dice con palabras |
| **8** | El tablero **reacciona** al sistema | Anular una factura llena la consulta 7 y baja el ingreso de la 1 |

> **El criterio 8 es el que distingue un tablero de una lámina.** Un tablero
> que no cambia cuando el sistema cambia está leyendo de otro lado.

## 4. El tropiezo del tipo, que el compilador no ve

`COUNT()` y `SUM(entero)` **no devuelven lo mismo en los dos motores**:

| | `COUNT()` devuelve | Y entonces |
|---|---|---|
| **PostgreSQL** | `bigint` | Un modelo con `int` **revienta al deserializar**. Ruidoso: se nota de una |
| **SQL Server** | `int` | Un `SUM(decimal) / COUNT(*)` **trunca** el promedio. No se queja |

> **El segundo es peor**, y por eso está escrito aquí: el primero se cae y se
> arregla; el segundo entrega un número equivocado que parece bien.
>
> **Se resuelve en el SQL, con `CAST(… AS INT)`**, y no cambiando los modelos a
> `long`: así los dos dialectos devuelven la misma forma y la v5 no tiene que
> tocar ni un modelo.

## 5. Lo que esta versión deja PENDIENTE, y se declara

El mapa le asigna a la v4 cuatro cosas más. **No están construidas**, y este
documento no las describe como si lo estuvieran:

| | Estado |
|---|---|
| **Imagen corporativa con su manual de marca** | Pendiente |
| **Páginas corporativas** (inicio, servicios, soporte, contacto) | Pendiente |
| **Responsive / PWA** | Pendiente — la interfaz **sí** es responsive por Bootstrap, pero no hay manifiesto ni *service worker* |
| **Publicación en un servidor** | Pendiente |

> **Está escrito en vez de omitido a propósito.** Un spec kit que describe lo
> que no existe le enseña a quien lo lee que los documentos no se pueden creer,
> y a partir de ahí deja de leerlos.

## 6. Cuándo se cierra

Los 8 criterios pasan → commit + tag `v4` → **las diez preguntas responden y
el tablero las muestra** → recién entonces se habla de la v5.
