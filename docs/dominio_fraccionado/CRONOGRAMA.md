# Cronograma — Facturación (`bdfacturas`)

> **Qué es este documento.** Quién hizo qué y cuándo, **contado de `git log`** y
> no de la memoria. Los números se pueden regenerar con los comandos de §6: si
> alguien los recalcula y no coinciden, este documento está viejo.
>
> **Material académico simulado.** El sistema es un ejemplo de clase, y **los
> tres integrantes también**: Carlos, Paco y Luis. Lo que **no** está simulado
> es la forma del historial — las ramas, las fusiones y los tags existen y se
> pueden abrir.
>
> Versión 2.0 · 4 de octubre de 2026.

---

## 0. Antes del calendario: dos incomodidades que hay que decir

**1. Las fechas son cuándo se guardó, no cuándo se pensó.** Un día con diez
commits no fue un día diez veces más productivo: fue un día en que se guardó
seguido. Y un commit solitario pudo ser el día en que se entendió el problema.

**2. Aquí no hubo sprints, ni daily, ni velocidad de equipo.** Llamar «Scrum» a
esto sería ponerle el nombre de un marco de trabajo a algo que no lo usó. Lo que
sí hubo fue **desarrollo por versiones con compuertas**, que es lo que el curso
enseña y lo que el spec kit sostiene.

---

## 1. Lo real, contado de `git log`

**84 commits**, entre el **8 de agosto** y el **3 de octubre de 2026**.

| Versión | Fecha | Carlos | Paco | Luis | Qué dejó |
|---|---|---|---|---|---|
| **v1** | 8 de agosto | **9** | 4 | 4 | Las seis tablas sin clave foránea, y el montaje |
| **v2** | 19 de agosto | 8 | **8** | **8** | Las seis con clave foránea, y el 409 |
| **v3** | 10 de septiembre | **7** | 4 | 1 | El token, el 401 y el 403 |
| **v4** | 1 de octubre | **10** | 6 | 5 | Las diez consultas, el tablero y la marca |
| **v5** | 3 de octubre | **6** | 2 | 2 | El segundo motor y la fábrica |
| | | **40** | **24** | **20** | **84** |

> **La v2 es la única pareja**, y no por casualidad: es la versión donde el
> reparto por recursos cuadra en tercios —`factura`, los dos recursos con clave
> foránea, y las dos tablas puente—. Las demás se reparten peor, y el que carga
> la diferencia es siempre el integrador.

### Carlos aparece con 40 de 84, y eso no significa que hiciera la mitad

| De sus 40 commits | |
|---|---|
| **15 son FUSIONES** | Integrar los PR de los otros dos: una por persona por versión. **Las 15 del repositorio son suyas** |
| **25 son contenido** | Su parte de verdad |

**Y al quitar las fusiones, el reparto cambia de cara:**

| | Commits totales | **De contenido** |
|---|---|---|
| Carlos | 40 | **25** |
| Paco | 24 | **24** |
| Luis | 20 | **20** |

> **Ahí están los tres casi parejos**, y la diferencia de la primera columna era
> casi toda trabajo de integración. Un `shortlog` crudo habría dicho que Carlos
> hizo **el doble** que Luis; descontando las fusiones, hizo **un 25 % más**.
>
> **Por eso contar commits no reparte mérito por sí solo.** Para saber quién
> escribió qué hay que mirar **los archivos**, no la cuenta:
>
> ```powershell
> git log -1 --format='%an' -- docs/spec_kit/versiones/v1_sin_fk/2_spec.md
> # responde: Paco
> ```

---

## 2. La forma del historial, que es la lección

Cada versión se construyó igual, y el grafo lo muestra:

```
rama-carlos-vN ──●──●──●──╮
rama-paco-vN   ──●──●─────┤   tres ramas, tres fusiones --no-ff
rama-luis-vN   ──●──●──●──┤
                          ▼
main           ─────●──●──●── tag vN
```

| | |
|---|---|
| **Ramas** | **15** — una por persona por versión |
| **Fusiones** | **15**, todas con `--no-ff` |
| **Tags** | `v1` a `v5`, uno por versión cerrada |

> **El `--no-ff` no es estética.** Sin él, una rama que no se separó del tronco
> desaparece del grafo al fusionarse, y ya no se ve dónde empezaba ni terminaba
> el aporte de cada quien. Con él, las tres líneas entran a `main` y se pueden
> señalar. Ver
> [`CONCEPTOS_RAMAS_Y_COLABORACION.md`](../conceptos/CONCEPTOS_RAMAS_Y_COLABORACION.md) §5.

> **Y los tres aparecen como tres contribuyentes en GitHub**, no como uno. Eso
> depende de una sola cosa: que cada quien firmara con **su** correo. Si los tres
> hubieran usado la configuración global de un computador compartido, las 84
> líneas dirían el mismo nombre y no habría forma de repartir la nota.

---

## 3. Lo que este cronograma NO puede decir

Y conviene decirlo, porque es el límite honesto del documento:

| No se puede saber | Por qué |
|---|---|
| **Cuántas horas costó cada versión** | Un commit no registra tiempo. Registra un momento |
| **Quién ayudó a quién** | Las conversaciones entre ellos no dejan rastro en git |
| **Cuánto se rehízo** | El historial muestra lo que quedó, no lo que se descartó |
| **Si el reparto fue justo** | 40 contra 20 commits no dice si el trabajo fue parejo |

> **El último es el que más se malinterpreta.** Luis tiene 20 commits y Carlos
> 40, y de ahí no se sigue que Luis hiciera la mitad: sus dos recursos de la v1
> son los de llave `IDENTITY`, que son los que **no se calcan** y los que más
> cuesta entender. **Contar commits mide actividad, no dificultad.**

---

## 4. Lo que falta, y su tramo

| Qué | Dónde está hoy | Qué falta |
|---|---|---|
| Las **guías de IA por estudiante** | completas en la v1 | las de la v2 a la v5 |
| La **v5** | especificada y con su código | correrla de punta a punta en los dos motores |
| `docs/conceptos` y `docs/dominio` | solo en este repositorio | replicarlos a los otros de la ruta |

---

## 5. Lo que queda abierto y **no tiene fecha**

Decir «sin fecha» es más honesto que poner una que nadie va a cumplir:

- **Las pruebas automatizadas.** Hay un documento que explica qué es una prueba
  y cómo se reconoce una hueca. Pruebas que corran en cada commit, no hay.
- **La publicación fuera de `localhost`.** Todo corre en Docker local.
- **El respaldo automático.** Hay un `backupdb/` para hacerlo a mano.

> Las tres están en «lo que este sistema NO promete» de
> [`REQUISITOS_NO_FUNCIONALES.md`](REQUISITOS_NO_FUNCIONALES.md) §8, y ahí es
> donde cuentan.

---

## 6. Cómo se regenera este cronograma

```powershell
# Quién hizo cuántos commits — lo que GitHub muestra en Contributors
git shortlog -sne main

# Commits por autor DENTRO de una versión
git log --format='%an' v1..v2 | Group-Object | Select-Object Count, Name

# Las ramas y las fusiones: el grafo entero
git log --oneline --graph --decorate --all

# Cuántas de las fusiones son del integrador
git log --merges --format='%an' | Group-Object | Select-Object Count, Name
```

> **Si estos comandos dan otros números, el documento está viejo y el historial
> tiene razón.** Ésa es la única jerarquía posible: el documento describe al
> repositorio, no al contrario.

---

## 7. Historial de este documento

| Versión | Fecha | Qué cambió |
|---|---|---|
| **2.0** | 4 de octubre de 2026 | **Reescrito entero.** El repositorio pasó de un historial de un solo autor a uno de tres, con ramas, fusiones y tags por versión. Todo lo que decía la versión 1.0 —98 commits, 18 días, un autor— dejó de ser cierto el día del cambio |
| 1.0 | 4 de octubre de 2026 | Primera versión, sobre el historial anterior |

> **Y esto es, en sí mismo, la lección más dura del documento:** un documento
> construido sobre `git log` **caduca el día que el historial cambia**, y no
> avisa. Queda ahí, con sus cifras, pareciendo cierto. Por eso §6 existe — para
> que cualquiera pueda comprobarlo en diez segundos en vez de creerle.
