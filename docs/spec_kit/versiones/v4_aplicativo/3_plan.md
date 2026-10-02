# Plan técnico — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v4 y sus criterios de aceptación |
> | **3_plan.md** (este) | CÓMO: la capa de consultas y el tablero |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas: cero tablas nuevas |
> | [6_contracts.md](6_contracts.md) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | [9_checklist.md](9_checklist.md) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

## 1. La decisión de fondo: ¿dónde vive el cruce?

| Opción | Qué pasa |
|---|---|
| **En el navegador** | Trae todas las filas a la máquina de quien mira, suma mal con paginación, y pide N+1 veces |
| **En el servicio, en C#** | Trae las filas igual, solo que al servidor. Y vuelve a escribir en C# lo que el motor ya sabe hacer |
| **En el motor, con SQL** ✅ | El cruce ocurre donde están los datos. Vuelve **una** fila por grupo |

## 2. La capa, que es la misma de siempre

```
ConsultasController  →  IServicioConsultas  →  IRepositorioConsultas
                                                       ↓
                                          RepositorioConsultasSqlServer
```

| Pieza | Qué hace |
|---|---|
| `Controllers/ConsultasController.cs` | 10 acciones `[HttpGet("nombre-de-la-consulta")]`. Traduce a HTTP y nada más |
| `Servicios/ServicioConsultas.cs` | **Vacío de reglas, y existe igual** |
| `Repositorios/IRepositorioConsultas.cs` | Las diez firmas |
| `Repositorios/RepositorioConsultasSqlServer.cs` | El SQL, con su dialecto |
| `Modelos/Consultas.cs` | Diez `record`, uno por consulta |

> **¿Para qué un servicio que no valida nada?** Porque el día que una consulta
> necesite una regla —un rango de fechas obligatorio, un tope de filas— hay
> dónde ponerla. Si el controlador le hablara directo al repositorio, esa regla
> terminaría en el controlador, que es donde no va. **La capa no se salta
> porque hoy esté vacía.**

> **Son `record` y no clases** porque no tienen comportamiento ni identidad:
> son el resultado de una pregunta, no una entidad del dominio.

> **Y son diez modelos con nombre, no un `Dictionary<string, object>`.** La API
> responderia lo mismo, y nadie sabría qué esperar sin ejecutarla. **El nombre
> de cada propiedad ES la documentación.**

## 3. El tablero

```
front_blazor/
├── Servicios/ServicioConsultas.cs    el único que sabe de HTTP, con sus 10 métodos
└── Components/Pages/Tablero.razor    las diez dibujadas, con barras de CSS
```

### 3.1 Las diez se piden A LA VEZ

Con `Task.WhenAll`. En fila serían diez viajes encadenados, y con la API lenta
—o apagada— diez esperas de 10 segundos cada una: **minuto y medio para abrir
una página**. Es la misma lección que en la v2 costó 50 segundos en la pantalla
de facturas.

### 3.2 El tablero va PRIMERO en el menú

Es lo que se mira al entrar. Y exige `interfaz.inicio`, que es el permiso que
todos los roles tienen — porque un tablero al que nadie puede entrar no es un
tablero.

### 3.3 Los gráficos van SIN librería y SIN CDN

| | |
|---|---|
| **La regla del proyecto es «sin CDN»** | Y su razón está escrita: un front que necesita internet para verse bien **no arranca en un salón sin red** |
| **Una barra es geometría** | Un `div` con su `width` en porcentaje **es** un gráfico de barras |
| **Se ve lo que el navegador va a pintar** | Con una librería uno lee una configuración y tiene que imaginarse el resultado |

> Una librería se justifica cuando hacen falta interacción, ejes, escalas
> logarítmicas o series temporales. Nada de eso hay aquí. Si el equipo la
> quiere, el sitio es la carpeta de `lib/` **del repositorio**, nunca un CDN.

### 3.4 Si una falla, las otras nueve se dibujan

Y el aviso dice **cuál** falló, por su nombre. «Hubo un error» obliga a abrir
las diez a mano para encontrarla.

## 4. Lo que este plan deja FUERA

| | Por qué |
|---|---|
| **Caché de los resultados** | Un tablero que muestra números viejos es peor que uno lento, y la v4 todavía no tiene el problema |
| **Filtros por fecha o por rango** | Las diez responden sobre **todo** lo que hay. El filtro es una conversación aparte |
| **Exportar a Excel o PDF** | Es un requisito real que el curso no plantea |
| **La marca, las páginas corporativas y la publicación** | **Pendientes**, y declaradas como tal en [2_spec.md](2_spec.md) §5 |
