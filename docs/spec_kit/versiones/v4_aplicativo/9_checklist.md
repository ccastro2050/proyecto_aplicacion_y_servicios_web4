# Lista de verificación — Versión 4: el aplicativo completo

> **Versión 4** del desarrollo incremental ([mapa de versiones](../0_mapa_versiones.md)).
> Rige la constitución: [../../1_constitution.md](../../1_constitution.md).
> **Acumulativa:** contiene TODO lo de v1 a v3 — los 70 endpoints existentes no
> se tocan y sus contratos siguen vigentes tal cual. La v4 **suma 10**.
>
> | Documento de esta versión | Contenido |
> |---|---|
> | [2_spec.md](2_spec.md) | QUÉ agrega la v4 y sus criterios de aceptación |
> | [3_plan.md](3_plan.md) | CÓMO: la capa de consultas y el tablero |
> | [4_research.md](4_research.md) | Decisiones y alternativas *(lectura opcional)* |
> | [5_data_model.md](5_data_model.md) | La MISMA bdfacturas: cero tablas nuevas |
> | [6_contracts.md](6_contracts.md) | Los 10 endpoints de `/api/consultas` |
> | [7_quickstart.md](7_quickstart.md) | Arranque y el smoke test de las diez |
> | [8_tasks.md](8_tasks.md) | Orden de construcción por fases verificables |
> | **9_checklist.md** (este) | Lo que tiene que estar antes de cerrar |
> | [GUIA_IA4.md](GUIA_IA4.md) | Construirla con IA, sobre su proyecto v3 |

---

## El código

- [ ] Diez `record` en `Modelos/Consultas.cs`, con nombres que dicen qué traen.
- [ ] Diez firmas en `IRepositorioConsultas`.
- [ ] **Cada consulta cruza 4 tablas o más**, y dos cruzan cinco.
- [ ] Las consultas 6 y 9 usan `LEFT JOIN` — preguntan por **ausencias**.
- [ ] `CAST(… AS INT)` en todas las columnas de conteo.
- [ ] Cada columna del `SELECT` tiene **alias** que coincide con la propiedad.
- [ ] El servicio existe **aunque no valide nada**, y está escrito por qué.
- [ ] El controlador lleva `[Authorize]` y `[ExigePermiso("interfaz.inicio")]`.
- [ ] **El registro en `Program.cs` está**, y la fábrica —si la hay— tiene su
      `CrearRepositorioConsultas()` en las **dos** implementaciones.

## La interfaz gráfica

- [ ] Las diez se piden **a la vez**, no una tras otra.
- [ ] Si una falla, las otras nueve se dibujan, y el aviso dice **cuál**.
- [ ] Cero filas se muestra **con palabras**, no como una tabla vacía.
- [ ] Las barras son CSS: **sin librería y sin CDN**.
- [ ] La entrada del tablero está en el menú, con su permiso.

## Lo medido, no lo supuesto

- [ ] Las diez responden **200** con token. *(¿se corrió?)*
- [ ] Sin token: **401**. Con un rol sin `interfaz.inicio`: **403**.
- [ ] Quitar el permiso **sin volver a identificarse** cambia el 200 por 403.
- [ ] Anular una factura **mueve** el tablero: la consulta 7 se llena y el
      ingreso del producto baja.
- [ ] **La regresión de v1, v2 y v3 pasa completa.**

## Los documentos

- [ ] Está escrito **por qué el cruce no se hace en el navegador** (las tres
      razones: trae todo, suma mal con paginación, N+1).
- [ ] Está escrito por qué son **diez endpoints con nombre** y no uno con
      parámetro.
- [ ] Está escrito el **tropiezo del tipo** de `COUNT()`, y por qué se resuelve
      con `CAST` y no cambiando los modelos.
- [ ] Está escrito por qué **cero filas es 200 y no 404**.
- [ ] Está escrito por qué los gráficos van **sin librería y sin CDN**.
- [ ] **Lo pendiente está declarado como pendiente** —marca, páginas
      corporativas, PWA, publicación— y no descrito como si existiera.

> **La última casilla es la que sostiene a las demás.** Un spec kit que
> describe lo que no existe le enseña a quien lo lee que los documentos no se
> pueden creer — y a partir de ahí deja de leerlos todos.
