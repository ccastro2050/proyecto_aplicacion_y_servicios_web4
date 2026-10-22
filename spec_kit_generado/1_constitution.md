<!-- GENERADO por Gemini en la fase 0 del generador, el 22 de octubre de 2026,
     con 1_constitution.plantilla.md + ARQUITECTURA_CAPAS + CONVENCIONES_DEL_PROYECTO
     + REQUISITOS_NO_FUNCIONALES + CODIGOS_DE_ESTADO + FUENTES_PROCEDENCIA.
     Es el contenido que devolvio Gemini. Se le quitaron los marcadores
     [cite: n] que inyecto, y se devolvio el diagrama a su bloque de codigo.
     La comparacion contra la constitucion real esta en la conversacion. -->

# Constitución del proyecto

> **Documento permanente.** Estas reglas rigen TODAS las versiones del
> curso. Cada versión tiene además su propia especificación en
> [versiones/](versiones/0_mapa_versiones.md); ante conflicto, la
> constitución gana.

---

## Artículo 1 — Construcción incremental y acumulativa sin anticipación

- **Versiones incrementales:** v1, v2, … Cada una cuenta con su spec kit
  propio. Una versión se considera TERMINADA solo cuando cumple sus criterios
  de aceptación; en ese momento se realiza commit, tag (`v1`, `v2`, …) y
  únicamente después se escribe la especificación siguiente.
- **Versiones acumulativas y YAGNI:** la v2 contiene a la v1, exigiendo prueba
  de regresión en cada entrega. Se prohíbe la anticipación (*You Aren't Gonna
  Need It*): no se deben incluir fábricas multi-motor, capas «por si acaso» ni
  tablas adicionales antes de la versión que las requiera. El código de cada
  versión solo puede nombrar lo que su especificación explicita.
- **Estado del repositorio:** el repositorio contiene siempre la versión en
  curso, funcionando.

## Artículo 2 — Stack: C# y ASP.NET Core

- Lenguaje **C#** sobre **ASP.NET Core (.NET 10)**: controladores con
  atributos, inyección de dependencias del framework, `async`/`await` en todo
  el acceso a datos e interfaz gráfica con Blazor Server.
- **Acceso a datos sin ORM de entidades** — y **por qué**: el SQL se escribe a
  mano, visible y siempre parametrizado (`@parametro`) para mantener el control
  explícito sobre las consultas y evitar abstracciones innecesarias. Se utiliza
  **Dapper** como micro-ejecutor sobre ADO.NET para mapear filas a objetos sin
  generar SQL jamás.
- Paquetes externos permitidos: `Microsoft.Data.SqlClient`, `Dapper` y
  `Swashbuckle.AspNetCore` (Swagger) — y **ninguno más** sin que una
  especificación lo pida.

## Artículo 3 — Arquitectura en capas, desde el día 1

```
Controlador  →  IServicio   →  IRepositorio
                    ↑                ↑
                Servicio        Repositorio
```

- **Incompatibilidades y aislamiento:** el controlador no puede escribir SQL ni
  tomar decisiones de negocio; el servicio no puede saber qué es un código 404
  ni qué motor existe debajo; el repositorio no decide nada y ejecuta el SQL y
  los procedimientos. En el front no puede haber una sola conexión a la base de
  datos (`SqlConnection`).
- **Solo el ensamblador** decide **qué implementación** se usa: la fábrica
  (`Program.cs` / `IFabricaRepositorios`) es el único punto que elige el motor.
  Todo lo demás recibe sus dependencias por constructor e interfaces
  (`IRepositorio*`), nunca instancia implementaciones de las que existan
  alternativas.
- **Traducción de errores:** las validaciones de forma se rechazan en la
  petición y el controlador; las excepciones del negocio y del servicio
  (`ArgumentException`, `NoEncontradoExcepcion`, `ConflictoExcepcion`) viajan
  hasta el controlador, donde se traducen a códigos HTTP (400, 404, 409); las
  violaciones de la base de datos caen en el `catch` final como 500.
- **Lo que NO tiene esta arquitectura:** sin Entity Framework ni ORM, sin
  caché, sin colas, eventos ni mensajería —todo es síncrono dentro de la
  petición, sin perjuicio de que el acceso a datos sea asincrónico con
  `async`/`await`—, sin microservicios y sin repositorio genérico
  `Repositorio<T>`.

## Artículo 4 — Un solo comando

`docker compose up -d --build` deja TODO el sistema de la versión funcionando,
desde la primera versión, sin requerir SDK ni instalaciones locales
adicionales.

## Artículo 5 — Diseño de la base de datos en fase 0

Las 12 tablas y sus restricciones vienen dadas del curso de Bases de Datos
—**89 líneas de código**— y **se copian, no se generan**. Todo lo que convierte
ese modelo en un sistema que defiende sus reglas —los 3 disparadores y 16
procedimientos almacenados, **760 líneas de código, el 89 % del script**— se
escribió en este proyecto en la fase 0.

> **Razón de diseño:** la base de datos se diseña una sola vez en la fase 0
> porque un modelo cambiante por entrega obligaría a migrar datos y reescribir
> procedimientos. **Ninguna IA genera el esquema** porque el modelo ya está
> definido a partir del proceso de elicitación, y la IA no participó en dicha
> etapa.

## Artículo 6 — Todo en español, comentado para principiantes

- Nombres, rutas, mensajes, comentarios y documentación: **en español**.
- El código lleva **comentarios línea a línea**: qué significa cada
  construcción del lenguaje y para qué sirve aquí. El repositorio es material
  de estudio, no solo software.

## Artículo 7 — Contratos exactos

Los endpoints, formatos y códigos de estado de cada versión están en su
`6_contracts.md` y se cumplen **al pie de la letra**, garantizando respuestas
consistentes e impidiendo que cada versión invente sus propios códigos.

## Artículo 8 — Convenciones fijas

| Cosa | Convención |
|---|---|
| Puertos del proyecto | API **8035** · Front **8099** · SQL Server **11466** · PostgreSQL **15462** — ninguno se repite, y el registro es [`PUERTOS.md`](../../../PUERTOS.md) |
| Rutas | Diagnóstico `/` · Documentación interactiva `/swagger` · Recursos `/api/{recurso}` |
| Nombres | C#: PascalCase en español · Interfaces: prefijo `I` · Base de datos: minúsculas sin separadores (`productosporfactura`) · Claves foráneas: prefijo `fk` (`fkidcliente`) · API: minúsculas con guion (`/api/rol-usuario`) · Procedimientos `sp_` · Disparadores `trg_`. Carpetas: `Controllers/`, `Modelos/`, `Peticiones/`, `Servicios/`, `Repositorios/`, `Excepciones/`, `pruebas/` |
| Sobre de respuesta | Lecturas: `{tabla, limite, total, datos}` · Errores: `{estado, mensaje, detalle}` (y `errores[]` en el 422) |
| Errores | **200** éxito · **204** sin contenido · **400** regla de negocio sobre los parámetros, por el servicio · **401** identidad no autenticada · **403** autenticado sin permiso · **404** recurso no encontrado, por el servicio · **405** verbo no permitido en la ruta · **409** conflicto con el estado del recurso · **422** cuerpo mal formado o incompleto, por ASP.NET · **500** fallo no traducido o rechazo de la base de datos |
| Credenciales (didácticas) | Usuario `sa` · Clave `Paradigmas123!` · Base `bdfacturas` |
