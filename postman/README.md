# postman — la colección de la API, lista para importar

Esta API ya trae documentación interactiva propia: **Swagger en
http://localhost:8032/swagger** (ASP.NET Core la genera con Swashbuckle).
Esta colección es el **camino alternativo**: los mismos endpoints como un
recorrido guiado y numerado, útil para quien prefiere Postman/Thunder
Client, para presentar la API sin abrir el código, y para comparar con los
proyectos gemelos del curso (Python y PHP tienen la misma colección).

## Cómo usarla (3 pasos)

1. Instale **Postman** (postman.com/downloads). Si le pide cuenta, puede
   usar la opción de cliente ligero sin registrarse.
2. **Import** (botón arriba a la izquierda) → arrastre el archivo
   `coleccion_v4.postman_collection.json` de esta carpeta (acumulativa:
   trae v1, v2 y v3 — la v4 no agrega peticiones: agrega un MOTOR, y
   las 41 corren idénticas contra PostgreSQL o SQL Server).
3. Con el proyecto corriendo (`docker compose up -d`), abra cualquier
   petición y dele **Send**.

## El orden cuenta una historia

Las 41 peticiones están numeradas para recorrerlas de arriba a abajo.
**1–13 (v1, producto):** diagnóstico → lecturas → el ciclo de escritura →
**la pareja didáctica** (9 y 10: el mismo body da 422 en PUT y 200 en
PATCH) → los errores (404, el 422 con `errores[]`, el 500 del duplicado).
**14–25 (v2):** persona repite el molde (con la pareja en 17 y 18 y el
**error de llave foránea** en la 20) y factura estrena la lógica en la BD:
lecturas por SP con nombres resueltos (21–22), la creación maestro-detalle
donde **el trigger calcula subtotales, total y stock** (23), y la anulación
con su **409** al repetirla (24–25).
**26–41 (v3):** los moldes que completan la BD (empresa y cliente con sus
**opcionales** en la 30, la FK como última defensa en la 31, el **UNIQUE**
en la 32), usuario donde **el secreto nunca viaja** (33–37: se crea con
hash BCrypt y el trío 200/401/404 de verificar-contrasena), y las tablas
puente con su **PK compuesta** (38–41: parejas exactas, sin PUT ni PATCH).
Cada petición trae su explicación en la pestaña de descripción.

## La variable {{base}}

La colección usa la variable `base` = `http://localhost:8032` (el proyecto
del curso). Si está probando **SU reconstrucción** (la de la
[GUIA_IA](../docs/spec_kit/versiones/v4_postgresql/GUIA_IA4.md), que corre en el puerto 8132): clic en la
colección → pestaña **Variables** → cambie `base` a
`http://localhost:8132`. Una sola edición y las 41 peticiones apuntan a su
proyecto.
