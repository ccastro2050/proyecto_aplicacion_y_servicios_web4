<!-- GENERADO por Gemini en la fase 0 del generador_spec_kit.md, el 22 de
     octubre de 2026, con ARQUITECTURA.md + REQUISITOS_NO_FUNCIONALES.md +
     1_constitution.plantilla.md.
     El contenido es el que devolvió Gemini; solo se restauró el formato
     Markdown (tablas y listas), que se perdió al copiar desde la pantalla.
     La comparación contra el real está en ../spec_kit_generado/REPORTE.md -->

# Constitución del proyecto

> **Documento permanente.** Estas reglas rigen TODAS las versiones del
> curso. Cada versión tiene además su propia especificación en
> [versiones/](versiones/0_mapa_versiones.md); ante conflicto, la
> constitución gana.

---

## Artículo 1 — Construcción por versiones incrementales

- El sistema se construye por **versiones incrementales** etiquetadas con su
  correspondiente versión.
- **No se anticipa** ninguna funcionalidad ni regla que la versión en curso no
  pida (**YAGNI**).
- El repositorio siempre tiene la **versión en curso funcionando**.

> **Razón:** desarrollar de forma incremental garantiza la estabilidad
> funcional entregable y evita sobreingeniería no requerida.

## Artículo 2 — Stack: C# / ASP.NET Core y Blazor Server

- Lenguaje **C#** sobre **ASP.NET Core / Blazor Server**: API REST síncrona en
  el backend (`api-facturas`) e interfaz interactiva en el servidor
  (`front-blazor`).
- **SQL escrito a mano** y **Dapper** como micro-ejecutor — y por qué: se evita
  Entity Framework y los ORM pesados para mantener el SQL explícito, el acceso
  a datos liviano y la lógica centrada en la base de datos.
- Paquetes externos permitidos: **Dapper** y **BCrypt** — y ninguno más sin que
  una especificación lo pida.

> **Razón:** limitar las dependencias minimiza la complejidad operativa, evita
> sobrecostos de mantenimiento y mantiene visible el comportamiento real del
> sistema.

## Artículo 3 — Arquitectura en capas, desde el día 1

```
Controlador  →  IServicio   →  IRepositorio
                   ↑                ↑
                Servicio        RepositorioSqlServer
                                RepositorioPostgres
```

- El front (`front-blazor`) **no puede conocer ni conectar a la base de datos**
  (0 `SqlConnection`), y el servicio **no conoce el motor de base de datos ni
  los códigos HTTP**.
- **Solo el ensamblador** —la fábrica de repositorios accionada por `MOTOR_BD`
  en `Program.cs`— decide **qué implementación** se usa. Todo lo demás
  **recibe sus dependencias**, nunca instancia lo que necesita.
- Las excepciones de la base de datos o del dominio son capturadas por los
  controladores y traducidas a códigos de estado HTTP estandarizados.

> **Razón:** la separación estricta por interfaces permite alternar motores de
> persistencia y probar componentes sin afectar las reglas de negocio ni la
> interfaz.

## Artículo 4 — Un solo comando

- `docker compose up -d --build` deja TODO el sistema de la versión
  funcionando, desde la primera versión.
- No se requiere instalar .NET ni motores de bases de datos localmente: **solo
  Docker**.

> **Razón:** garantizar que el entorno de despliegue sea idéntico y
> reproducible en cualquier equipo con un único paso.

## Artículo 5 — Diseño e integración de la base de datos

- Las **12 tablas** base son heredadas del curso de Bases de Datos (**89
  líneas**), mientras que la lógica operativa crítica —**760 líneas**
  correspondientes a **3 disparadores** y **16 procedimientos almacenados**— se
  escribe en este proyecto.
- Las reglas clave de negocio —stock no negativo, cálculo de totales, control
  de anulaciones— **residen en la base de datos** y no en la capa de C#.

> **Razón:** colocar las defensas y restricciones en el motor de base de datos
> garantiza la integridad de los datos independientemente de qué cliente o
> proceso acceda a ellos.

## Artículo 6 — Todo en español, comentado para principiantes

- Nombres, rutas, mensajes, comentarios y documentación: **en español**.
- El código lleva **comentarios línea a línea**: qué significa cada
  construcción del lenguaje y para qué sirve aquí. El repositorio es material
  de estudio, no solo software.

> **Razón:** mantener la claridad cognitiva y pedagógica para que cualquier
> estudiante o desarrollador pueda auditar y comprender la intención del
> código.

## Artículo 7 — Contratos exactos

- Los endpoints, formatos y códigos de estado de cada versión están en su
  `6_contracts.md` y se cumplen **al pie de la letra**.
- **Toda** respuesta de la API es JSON, incluyendo los errores.

> **Razón:** la previsibilidad técnica en las respuestas HTTP previene
> desalineaciones entre la interfaz de usuario y los servicios web.

## Artículo 8 — Convenciones fijas

| Cosa | Convención |
|---|---|
| Puertos del proyecto | `front-blazor` **8099** · `api-facturas` **8035** · SQL Server **11466** · PostgreSQL **15462** — ninguno se repite, y el registro es [`PUERTOS.md`](../../../PUERTOS.md) |
| Rutas | `/api/{recurso}` (p. ej. `/api/producto`, `/api/factura`, `/api/usuario`) |
| Nombres | PascalCase en C#, minúsculas/snake_case en base de datos, y rutas en español |
| Sobre de respuesta | Lecturas: `{tabla, limite, total, datos}` · Errores: `{estado, mensaje, detalle}` (con `errores[]` en la validación 422) |
| Errores | 400 Bad Request · 401 Unauthorized · 403 Forbidden · 404 Not Found · 422 Unprocessable Entity · 500 Internal Server Error |
| Credenciales (didácticas) | Quemadas en `docker-compose.yml` para uso en clase, y variables de entorno para producción |

> **Razón:** estandarizar rutas, contratos de respuesta y puertos elimina la
> ambigüedad en la comunicación entre capas y facilita la consulta directa del
> equipo.
