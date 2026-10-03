# Guía de IA — Versión 4: el aplicativo completo

> Guía de la **v4** (acumulativa: se construye encima de su proyecto con la
> **v3** cerrada). Dos caminos, el mismo resultado: **A** con un chat web,
> **B** con un agente en el IDE. Lea el que vaya a usar.
>
> **Sus puertos:** API 8135 · interfaz gráfica 8199 (los del curso
> +100, para que los dos proyectos convivan).
>
> Novedad de esta versión: por primera vez el sistema **responde preguntas** en
> vez de guardar filas.

---

## LO PRIMERO: la IA tiene que COMENTAR lo que escribe

Esto va antes que todo lo demás porque aplica a los dos caminos —el chat y el
IDE agéntico— y a cada archivo que la IA entregue.

**Exíjaselo en el prompt y recháceselo cuando no lo cumpla:**

> Comenta todo el código que generes, en español. Cada archivo empieza con un
> bloque que dice QUÉ ES y QUÉ PAPEL cumple en la arquitectura. Cada método no
> evidente lleva su comentario. Y los comentarios dicen **por qué** está escrito
> así, no **qué** hace la línea: `// suma uno al contador` no le sirve a nadie;
> `// el límite es 1000 porque es el ancho de la columna en la base` sí.

**Por qué esto no es un capricho de estilo:**

| | |
|---|---|
| **Usted va a tener que explicarlo** | Desde la v2, la **interpretabilidad se califica**: el profesor le va a pedir que cuente, en voz alta, qué hace el código que entregó y por qué está así. Un archivo sin comentarios es un archivo que usted va a tener que reconstruir de memoria enfrente de él |
| **El código generado se olvida más rápido que el escrito** | Lo que uno escribe a mano deja rastro en la cabeza. Lo que aceptó de un chat, no. El comentario es lo que queda cuando el recuerdo de la conversación se fue |
| **Es la prueba de que usted entendió, no de que la IA produjo** | Un comentario que explica el porqué solo lo puede revisar quien entendió la decisión. Si usted no puede juzgar si el comentario es cierto, no entendió el código |

> **Y una advertencia sobre los comentarios que la IA inventa.** A veces
> comenta lo que *cree* que hace el código, no lo que hace. Léalos: un
> comentario equivocado es peor que ninguno, porque el siguiente que lo lea le
> va a creer. Corregirlos es trabajo suyo, y de los buenos: es exactamente el
> tipo de corrección que la `GUIA_IA` le pide guardar.

---

## A.1 Qué subirle al chat (los 9 de la v4)

Los nueve documentos de `docs/spec_kit/versiones/v4_aplicativo/` (2_spec a
8_tasks). Además esta guía y `1_constitution.md`.

**Y los de las versiones anteriores**, porque la v4 las incluye: los
`6_contracts.md` de v1, v2 y v3 — son los 70 endpoints que **no se pueden
tocar**.

## A.2 Cree los archivos vacíos

```powershell
New-Item api_facturas\Modelos\Consultas.cs,`
  api_facturas\Repositorios\IRepositorioConsultas.cs,`
  api_facturas\Repositorios\RepositorioConsultasSqlServer.cs,`
  api_facturas\Servicios\IServicioConsultas.cs,`
  api_facturas\Servicios\ServicioConsultas.cs,`
  api_facturas\Controllers\ConsultasController.cs,`
  front_blazor\Servicios\ServicioConsultas.cs, front_blazor\Components\Pages\Tablero.razor
```

**Dos archivos EXISTENTES crecen:**

| | |
|---|---|
| `api_facturas\Program.cs` | El registro del repositorio y del servicio |
| La fábrica, si la hay | `CrearRepositorioConsultas()` en la interfaz **y en las dos implementaciones** |

---

## A.3 El prompt

```
Vas a construir la VERSION 4 de un proyecto que ya tiene las versiones 1, 2 y
3 cerradas y funcionando. ESTE PROYECTO USA DOS LENGUAJES, uno por proceso: la
API en C# / ASP.NET Core, y la interfaz gráfica en Blazor Server. Si en tu respuesta
la API aparece en otro lenguaje, o la interfaz gráfica en otro framework,
significa que no leíste los documentos adjuntos: detente y dímelo.

LA v4 NO AGREGA NI UNA TABLA. Agrega DIEZ CONSULTAS que cruzan cuatro o más
tablas cada una, y el TABLERO donde se ven. Los 70 endpoints de v1 a v3 no se
tocan: sus contratos siguen vigentes tal cual.

1. LAS DIEZ CONSULTAS, con estos nombres exactos de ruta:
   ventas-por-producto, ventas-por-cliente, ventas-por-vendedor,
   ventas-por-empresa, ticket-por-vendedor, productos-sin-vender,
   anulaciones-por-cliente, alcance-de-usuarios, interfaces-sin-usuarios,
   credito-contra-consumo.
   Todas bajo [Route("api/consultas")], todas GET, todas sin parámetros.

2. DIEZ ENDPOINTS CON NOMBRE, NO UNO CON PARAMETRO. Si propones
   /api/consultas?nombre=x, recházalo tú mismo: cada consulta devuelve una
   forma distinta, y un contrato que dice «depende» no es un contrato.

3. EL SQL VA EN EL REPOSITORIO, no en vistas ni en procedimientos: la
   constitución exige el SQL a la vista.

4. CADA CONSULTA CRUZA 4 TABLAS O MAS. Dos cruzan cinco
   (alcance-de-usuarios). Las de ausencia —productos-sin-vender e
   interfaces-sin-usuarios— necesitan LEFT JOIN: un INNER JOIN no puede
   responder una ausencia.

5. CAST(... AS INT) EN TODAS LAS COLUMNAS DE CONTEO, y esto no es cosmético:
   en PostgreSQL COUNT() devuelve bigint y el modelo con int revienta al
   deserializar; en SQL Server devuelve int y un SUM(decimal)/COUNT(*) TRUNCA
   el promedio sin quejarse. El CAST deja las dos respuestas idénticas.

6. ALIAS EN CADA COLUMNA, coincidiendo con la propiedad del record. Dapper
   mapea POR NOMBRE: sin alias el campo llega vacío y la API responde 200 con
   el dato en blanco.

7. DIEZ RECORD CON NOMBRE en Modelos/Consultas.cs. No un
   Dictionary<string, object>: el nombre de cada propiedad ES la
   documentación.

8. EL SERVICIO EXISTE AUNQUE NO VALIDE NADA. El controlador no le habla al
   repositorio. La capa no se salta porque hoy esté vacía.

9. EL SOBRE ES { consulta, total, datos[] } — no el { tabla, limite, total,
   datos[] } del CRUD. Una consulta no sale de una tabla ni tiene límite.

10. CERO FILAS ES 200, NO 404. La 6 y la 9 pueden venir vacías, y eso es la
    respuesta: el 404 diría que la consulta no existe, que es otra cosa.

11. EL CONTROLADOR LLEVA [Authorize] Y [ExigePermiso("interfaz.inicio")].

12. EL REGISTRO EN Program.cs NO SE PUEDE OLVIDAR:
    builder.Services.AddScoped<IRepositorioConsultas>(_ => fabrica.CrearRepositorioConsultas());
    builder.Services.AddScoped<IServicioConsultas, ServicioConsultas>();
    Sin esas dos líneas el proyecto COMPILA, arranca, y el endpoint responde
    500 «Unable to resolve service» cuando alguien lo pide.

13. CADA VERSION ES API + INTERFAZ GRAFICA, y la v4 no cierra sin el tablero:
   - Las diez se piden A LA VEZ con Task.WhenAll, no en fila.
   - El tablero va PRIMERO en el MenuApp, con permiso interfaz.inicio.
   - Si una consulta falla, las otras nueve se dibujan, y el aviso dice CUAL.
   - Los gráficos van SIN LIBRERIA Y SIN CDN: una barra es un div con su
     width en porcentaje. La regla del proyecto es «sin CDN» y su razón está
     escrita: un front que necesita internet para verse bien no arranca en un
     salón sin red.
   - Cero filas se muestra CON PALABRAS, no como una tabla vacía.

14. NO HAGAS: caché de resultados, filtros por fecha, exportación a Excel, ni
    una librería de gráficos. Y NO toques la marca, las páginas corporativas,
    la PWA ni la publicación: están declaradas PENDIENTES en 2_spec.md §5.

Al final, la versión 4 está TERMINADA solo cuando pasan los 8 criterios de
aceptación de 2_spec.md, verificados con el smoke test de 7_quickstart.md —
incluida la REGRESION de v1, v2 y v3.
```

---

## B. Camino B — el agente en el IDE

Mismo prompt, y además:

| | |
|---|---|
| **Déjelo leer el repositorio** | Los repositorios de v1–v3 ya tienen el patrón: que copie **ese**, no uno inventado |
| **Pídale que ejecute** | Las diez consultas, una por una, con token. «Debería funcionar» no es una verificación |
| **Y la regresión** | Es el criterio 1, y es el que un agente se salta más seguido |

## C. Lo que hay que rechazarle

| Si la IA… | Qué hacer |
|---|---|
| **Propone `/api/consultas?nombre=x`** | Recházelo. Punto 2 del prompt |
| **Crea vistas o procedimientos** | Recházelo. La constitución exige el SQL a la vista |
| **Agrega una tabla** | Recházelo: la v4 no agrega ninguna |
| **Devuelve `Dictionary<string, object>`** | Recházelo. Punto 7 |
| **Olvida el `CAST`** | Pídale que lo ponga **antes** de ejecutar: el síntoma en PostgreSQL es un error de constructor con `System.Int64` |
| **Olvida el registro en `Program.cs`** | Es el que más se olvida, y **compila igual**. Pídale la salida de los diez endpoints |
| **Mete Chart.js por CDN** | Recházelo. Si quiere la librería, va servida desde el repositorio |
| **Dice «listo» sin ejecutar** | Pídale la salida real de las diez, y la de la regresión |
| **Describe la marca o la publicación como hechas** | Recházelo: están **pendientes**, y el spec lo dice |
