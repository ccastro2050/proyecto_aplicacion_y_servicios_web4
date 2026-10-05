# Lo que falta — registro de pendientes

> **Qué es este documento.** Lo que este repositorio **todavía no tiene**,
> dicho antes de que alguien lo descubra en la sustentación. Cada punto dice
> **qué falta**, **dónde** y **cómo se comprueba** que ya se hizo.
>
> No es una lista de deseos: es lo que se midió y resultó faltando el
> **4 de octubre de 2026**, con el repositorio en la v4.
>
> **La regla de este archivo:** un pendiente que se cierra se **tacha con su
> commit**, no se borra. Lo que se borró nunca estuvo.

---

## 1. La v4, que está EN CURSO

Lo que falta para cerrarla. Está también en
[`PLAN_V4.md`](PLAN_V4.md) §7 y en
[`v4_aplicativo/2_spec.md`](../spec_kit/versiones/v4_aplicativo/2_spec.md).

| | Estado | Qué falta exactamente | Cómo se comprueba |
|---|---|---|---|
| **Las 10 consultas y el tablero** | **Hecho** | — | Las diez responden 200 y el tablero las dibuja |
| **Imagen corporativa** | **Hecho** | — | `marca.css` enchufada en `App.razor`, con los cinco colores del [manual](MANUAL_DE_MARCA.md) |
| **Páginas corporativas** | **Pendiente** | Hay `Home.razor`. Faltan **servicios, soporte y contacto** | Que existan las tres páginas y el menú las alcance |
| **Responsive / PWA** | **A medias** | Responsive **sí** (Bootstrap). **No hay `manifest.json` ni *service worker*** en `front_blazor/wwwroot/` | Que el navegador ofrezca instalar el sitio |
| **Publicación** | **Pendiente** | Publicar con los **secretos en variables de entorno del servidor**, no en el repositorio | Una URL que responda, y `.env.example` sin valores reales |

> **La distinción que más se confunde:** «responsive» es que la interfaz se
> acomode al ancho; **PWA** es que el navegador la pueda instalar y abrir sin
> red. Lo primero está; lo segundo son dos archivos que no existen.

---

## 2. Los comentarios del código — la interpretabilidad

Desde la v2, el 20 % de la nota de cada versión es interpretabilidad
([`0_METODOLOGIA.md`](../../ProyectosDeAula/docs/0_METODOLOGIA.md) §7), y el
comentario es lo que queda cuando el recuerdo de la conversación se fue.

| | Estado | Qué falta |
|---|---|---|
| **`db/bdfacturas.sql`** (SQL Server) | **Hecho** | Las 12 tablas, los 3 disparadores y, por dentro, `sp_insertar_factura`, `sp_anular_factura` y `sp_actualizar` |
| **`db/bdfacturas_postgres.sql`** | **A medias** | Tiene las 12 tablas, la función disparadora y la **tabla de equivalencias de dialecto** en la cabecera. **Falta comentar por dentro** su gemelo de `sp_insertar_factura` —`RETURNING … INTO`, `FOR … LOOP`, `json_array_elements`, `json_agg`— y los de anular y actualizar |
| **Los procedimientos de usuarios y permisos** | **Pendiente** | `crear_usuario_con_roles`, `actualizar_usuario_con_roles`, `actualizar_roles_usuario`, `crear_rutarol`, `eliminar_rutarol`: tienen **encabezado** pero no explican sus decisiones por dentro. En los **dos** motores |

> **Y ya se midió que ahí no hay sintaxis nueva.** Comparando el vocabulario
> SQL de cada procedimiento contra el de `sp_insertar_factura`, lo único que
> aparece de más es SQL corriente: `EXISTS`, `NOT EXISTS`, `DELETE FROM`,
> `ORDER BY`, `GROUP BY`. **Lo que falta son las decisiones, no las palabras**
> — por eso el pendiente es corto.

---

## 3. El material del curso

| | Estado | Qué falta |
|---|---|---|
| [`VERSION_2.md`](../../ProyectosDeAula/docs/VERSION_2.md) | **Hecho** | Incluye el 20 % de interpretabilidad en su §8.1 |
| **`VERSION_3.md`** | **No existe** | El equivalente para la v3: el control de acceso, con sus criterios y sus trampas |
| **`VERSION_4.md`** | **No existe** | El equivalente para la v4: las 10 consultas, el tablero, la marca y la publicación |
| **Los planes de versión** | **Hecho** | [`PLAN_V1`](PLAN_V1.md) · [`PLAN_V2`](PLAN_V2.md) · [`PLAN_V3`](PLAN_V3.md) · [`PLAN_V4`](PLAN_V4.md) |

---

## 4. Lo que GitHub todavía muestra

| | Qué pasa | Qué habría que decidir |
|---|---|---|
| **El tag `v5`** | `main` ya **no** publica `docs/spec_kit/versiones/v5_otros_motores/` —salió con `git rm --cached` y entró al `.gitignore`—, pero **el tag `v5` sí está empujado y contiene sus 9 documentos**. Quien elija ese tag en el desplegable de GitHub los ve | Borrar el tag del remoto y conservarlo local, o dejarlo. **Es decisión del profesor**, no se tocó |

> La carpeta sigue en el disco: es trabajo en curso, y la historia no se
> reescribió. La razón está en el `.gitignore`, escrita al lado de la regla.

---

## 5. Lo que es de otros repositorios, no de este

| | Qué falta |
|---|---|
| **Replicar los conceptos y el dominio** | `docs/conceptos/` (23 documentos) y `docs/dominio/` (19) están adaptados a **este** código. Llevarlos a los demás repositorios de la ruta exige **adaptarlos** al codigo de cada uno, no copiarlos: un documento que cita un archivo que alla no existe es peor que no tenerlo |
| **`proyecto_construccion_de_software4`** | Recibió el mismo cierre el 4 de octubre: el README dejó de apuntar a la v5 y su spec salió de GitHub. Lo que **no** se replicó allá son los planes de versión ni los comentarios de la base de datos |

---

## Cómo se midió todo esto

Para que el próximo que lea esto no tenga que creer:

| Qué se comprobó | Con qué |
|---|---|
| API y front en pie | `GET /api/producto` 200 con token, **401 sin token**; las diez consultas 200; cinco pantallas 200 |
| Los dos scripts de base de datos **ejecutan completos** | Se corrieron contra **bases de datos temporales** (SQL Server: 12 tablas, 16 procedimientos, 3 disparadores · PostgreSQL: 12 tablas, 17 rutinas) y se borraron. Las reales no se tocaron |
| Los disparadores **hacen lo que dicen** | Factura de 4 unidades → stock 17→13 · anular → stock 17 y renglones conservados · anular otra vez → error 50010 y el stock **no** sube dos veces · actualizar de 4 a 2 → stock 13→15 |
| Las referencias a archivos | Se buscaron las rutas citadas en el código y en los documentos que **no existen**. Se encontraron cuatro y se arreglaron |

---

| Qué | Dónde |
|---|---|
| Lo que falta de la v4, con su razón | [`PLAN_V4.md`](PLAN_V4.md) §7 |
| La rúbrica y el 20 % de interpretabilidad | [`0_METODOLOGIA.md`](../../ProyectosDeAula/docs/0_METODOLOGIA.md) §7 |
| El mapa de versiones, y por qué la v5 está fuera | [`0_mapa_versiones.md`](../spec_kit/versiones/0_mapa_versiones.md) |
| Lo que ya se hizo, contado de `git log` | [`CRONOGRAMA.md`](CRONOGRAMA.md) |
