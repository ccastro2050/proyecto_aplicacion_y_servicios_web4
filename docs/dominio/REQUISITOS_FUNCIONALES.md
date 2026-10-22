# Requisitos funcionales — Facturación (`bdfacturas`)

> **Qué es este documento.** El **catálogo completo** y la **matriz de
> trazabilidad**: lo que es cierto de todas las versiones a la vez. Si algo
> funciona y no está aquí, el catálogo está incompleto; si algo está aquí y no
> funciona, el sistema está incompleto. Las dos cosas son errores.
>
> **Los requisitos de cada versión están en su propio archivo**, porque es así
> como se le entregan a la IA:
>
> | Versión | Archivo | Qué trae |
> |---|---|---|
> | **v1** | [`RF_V1.md`](RF_V1.md) | las seis tablas sin clave foránea |
> | **v2** | [`RF_V2.md`](RF_V2.md) | las seis tablas CON clave foránea |
> | **v3** | [`RF_V3.md`](RF_V3.md) | quién entra, y a qué |
> | **v4** | [`RF_V4.md`](RF_V4.md) | el aplicativo |
>
> **Por qué están partidos.** La conversación que genera el spec kit de una
> versión **solo necesita los requisitos de esa versión**: los de las otras
> compiten por la atención con el único que importa. Ver
> [`DOMINIO_PARA_GENERAR.md`](../../DOMINIO_PARA_GENERAR.md).
>
> **Qué NO es.** No reemplaza los `2_spec.md` de cada versión, que son los que
> **mandan**.
>
> **Material académico simulado**, pero los endpoints **están medidos contra la
> API corriendo**, no copiados del código.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 0. Cómo leer este catálogo

| Columna | Qué dice |
|---|---|
| **RF** | `RF-v<versión>-<número>`. El número no se reusa: si un requisito se retira, su número queda vacío |
| **Requisito** | Qué hace el sistema, en una frase |
| **Dónde se comprueba** | El endpoint, la pantalla o el comando |

> **La versión importa**, y no es decoración: este proyecto es **acumulativo**.
> Un RF de la v2 sigue vivo en la v4, y tiene que seguir funcionando — eso es la
> **prueba de regresión** que cada versión obliga a correr.

### Aviso sobre los números

Las cuentas de este documento salen de `swagger.json` de la API encendida:
**80 operaciones publicadas** y **19 escritas y apagadas** (§2). Si alguien
agrega un endpoint y no actualiza esta tabla, la comprobación de §3 lo delata.

---

## 1. Versión 5 — el otro motor

**No tiene archivo propio**: la v5 **se proyecta**, está fuera de las cuatro
versiones del curso, y su naturaleza es distinta — no agrega funcionalidad,
**demuestra** que la interfaz del repositorio servía.

| RF | Requisito | Dónde se comprueba |
|---|---|---|
| **RF-v5-1** | Una **fábrica** decide qué repositorio se usa | `IFabricaRepositorios` |
| **RF-v5-2** | El motor se elige por **configuración**, no recompilando | La variable `MOTOR_BD` |
| **RF-v5-3** | PostgreSQL completo: las mismas operaciones, el mismo contrato | `/api/producto` con el otro motor |
| **RF-v5-4** | El diagnóstico dice **qué motor** está puesto | `GET /` |
| **RF-v5-5** | Cambiar de motor **no toca** controladores ni servicios | `git diff` sobre `Controllers/`: vacío |

> **RF-v5-5 es el que da sentido a los otros cuatro.** Si para cambiar de motor
> hubiera que tocar un controlador, las capas estarían mal hechas y las cuatro
> versiones anteriores habrían enseñado una arquitectura que no aguanta. La v5 es
> la factura que se le pasa a la v1.

---

## 2. Los cinco verbos, recurso por recurso

Esto es lo que de verdad publica la API, contado de `swagger.json`:

| Recurso | GET | POST | PUT | PATCH | DELETE |
|---|---|---|---|---|---|
| `producto` · `persona` · `empresa` · `cliente` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `vendedor` · `usuario` · `rol` · `ruta` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `factura` | ✅ | ✅ | 🔸 | 🔸 | 🔸 |
| `rol-usuario` · `rutarol` | ✅ | ✅ | 🔸 | 🔸 | ✅ |
| `usuario-con-roles` | ✅ | ✅ | ✅ | 🔸 | ✅ |
| `sesion` | 🔸 | ✅ | 🔸 | 🔸 | 🔸 |
| `permisos` | 🔸 | 🔸 | 🔸 | 🔸 | ✅ |
| `consultas` | ✅ | — | — | — | — |

**✅ activo · 🔸 escrito y apagado · — no aplica**

> **Las 🔸 NO son huecos: son código escrito, comentado y explicado.** Ocho
> recursos tienen los cinco verbos vivos; los demás tienen el verbo **escrito
> en el controlador, apagado con un comentario que dice por qué**, y con el
> servicio y el repositorio **activos** debajo. Encenderlo es quitar las barras.
>
> Son **19 métodos**, y están así a propósito: esto es material de clase, y un
> estudiante tiene que poder ver **cómo se programa cada verbo**, incluso el que
> esta versión del spec no pide. Borrarlos enseñaría solo a obedecer un spec;
> dejarlos apagados enseña a obedecerlo **y** a programar lo que no pide.

| Y por qué están apagados | Cuáles |
|---|---|
| El recurso **no se modifica, se anula** | `PUT`/`PATCH`/`DELETE` de `factura` |
| En una tabla puente, «actualizar» es **mover la fila**: borrar y crear | `PUT`/`PATCH` de `rol-usuario` y `rutarol` |
| Una sesión **no se edita**: se abre y se deja vencer | todo lo demás de `sesion` |
| El reparto de permisos lo hace la pantalla con `GET` y `DELETE` | el resto de `permisos` |
| El login llama al **servicio**, no a la API | `POST /api/usuario/verificar-contrasena` |

---

## 3. Matriz de trazabilidad, y cómo se comprueba

| Versión | RF | Qué publica | Cuántas | Regresión que hereda |
|---|---|---|---|---|
| **v1** | 8 | los 6 recursos sin FK (6 cada uno) + diagnóstico | **37** | — |
| **v2** | 10 | `cliente` 6 · `vendedor` 6 · `factura` 4 · las 2 puentes 5+5 · `usuario-con-roles` 5 | **31** | los 8 de la v1 |
| **v3** | 9 | `sesion` 1 · `permisos` 1 | **2** | los 18 anteriores |
| **v4** | 5 | `consultas` | **10** | los 27 anteriores |
| **v5** | 5 | nada nuevo — **los mismos, otro motor** | **0** | los 32 anteriores |
| | **37 RF** | | **80** | |

> **La v3 publica DOS operaciones y es la versión que más cambia el sistema.**
> No agrega tablas ni recursos: le pone una puerta a los 68 endpoints que ya
> existían. Medir una versión por cuántos endpoints suma habría dicho que la v3
> casi no es trabajo, y es exactamente al revés.

> **Que la v5 agregue CERO operaciones es el dato más importante de la tabla.**
> Una versión entera de trabajo que no cambia el contrato: eso es exactamente lo
> que significa «abierto a la extensión, cerrado a la modificación».

```powershell
# 1 · Lo que la API publica contra lo que el contrato declara
python auditar_aysw4_por_version.py

# 2 · Cuántas operaciones hay de verdad — debe decir 80
(Invoke-RestMethod http://localhost:8035/swagger/v1/swagger.json).paths.PSObject.Properties |
  ForEach-Object { $_.Value.PSObject.Properties.Name } | Measure-Object | Select-Object Count
```

| Qué | Dónde |
|---|---|
| Los requisitos **de cada versión** | [`RF_V1.md`](RF_V1.md) · [`RF_V2.md`](RF_V2.md) · [`RF_V3.md`](RF_V3.md) · [`RF_V4.md`](RF_V4.md) |
| El requisito que manda, por versión | el `2_spec.md` de cada una |
| El contrato endpoint por endpoint | el `6_contracts.md` de cada una |
| Las reglas que estos RF dan por ciertas | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| Lo que el sistema tiene que **ser** | [`REQUISITOS_NO_FUNCIONALES.md`](REQUISITOS_NO_FUNCIONALES.md) |
