# La procedencia — de dónde salió la base de datos, y de dónde NO

> **Qué es este documento.** Los **dos orígenes** de la base de datos, medidos:
> qué llegó hecho de otro curso y qué se escribió en este proyecto. Y por qué
> **ninguna IA genera el esquema**.
>
> **A qué plantilla alimenta:** el `1_constitution`.
>
> **Por qué está separado de la jerarquía de autoridad.** Porque son dos
> lectores: la constitución necesita saber de dónde viene el esquema; el
> `4_research` necesita saber **qué manda sobre qué**, y eso está en
> [`FUENTES_AUTORIDAD.md`](FUENTES_AUTORIDAD.md). Ver
> [`DOMINIO_PARA_GENERAR.md`](../../DOMINIO_PARA_GENERAR.md).
>
> **Material académico simulado.** La empresa es ficticia y no hubo ningún
> cliente. Lo que sí es real es el reparto que este documento describe, y se
> puede comprobar abriendo las carpetas.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. Dos orígenes, no uno

**Las doce tablas vienen de otro curso.** El equipo ya había modelado
`bdfacturas` en Bases de Datos: las entidades, las relaciones, las llaves y las
restricciones. Ese trabajo **se reusa tal cual** y no se vuelve a hacer.

**Pero ahí no había un sistema de facturación: había un modelo.** Todo lo que
convierte ese modelo en un sistema que defiende sus propias reglas **se escribió
en ESTE proyecto, en la fase 0**, antes de la versión 1.

### Y la diferencia se puede medir

```powershell
# Las tablas y sus restricciones, contra todo lo demás
Select-String -Path db\bdfacturas.sql -Pattern 'CREATE (TABLE|TRIGGER|PROCEDURE)' |
  Group-Object { $_.Matches[0].Groups[1].Value } | Select-Object Count, Name
```

| | | Líneas de **código** | De dónde viene |
|---|---|---|---|
| **12 tablas** con sus restricciones | | **89** | **del curso de Bases de Datos** |
| **3 disparadores** | `trg_prodfact_insert` · `_update` · `_delete` | **760** | **de este proyecto** |
| **16 procedimientos** | los 6 de `factura`, los de usuario y roles, `verificar_acceso_ruta` | | |

> **El 11 % del script llegó hecho. El 89 % se escribió aquí.** Y no es un
> detalle de contabilidad: **las tablas no deciden nada**. Que el stock no quede
> negativo, que el total cuadre con sus renglones, que una factura se anule en
> vez de corregirse y que no se pueda anular dos veces —**todas las reglas del
> negocio**— viven en los disparadores y los procedimientos, y **ninguno existía
> antes de este proyecto**.

---

## 2. De dónde salieron esas reglas: de la fase 0

```
elicitación  →  reglas de negocio  →  disparadores y procedimientos
```

| Paso | Dónde quedó |
|---|---|
| **1 · Se habla con el usuario experto** | [`elicitacion/`](elicitacion/1_PREGUNTAS.md) |
| **2 · Salen las reglas** | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) — las 22, con quién defiende cada una |
| **3 · Cada regla se implementa donde protege a más gente** | el disparador, el procedimiento, o el servicio |
| **4 · Y recién entonces, la API** | las cinco versiones |

> **Ejemplo, y se puede seguir completo:** el jefe de ventas dice *«no se puede
> vender lo que no tengo, eso es sagrado»* → nace la regla **RN-10** → y se
> implementa en `trg_prodfact_insert` con un `THROW 50001`. **No en C#**, porque
> la regla tiene que valer también para quien entre por SSMS.
>
> Las tres cosas se pueden abrir y comparar. Eso es lo que significa que una
> decisión **tenga autor**.

---

## 3. Por qué el modelo no se rehace en cada versión

> **La base de datos se diseña UNA VEZ, en la fase 0.** Desde la v1 en adelante
> **viene dada al código**: se copia, no se genera.

| | |
|---|---|
| **Por qué una sola vez** | Un modelo que cambia en cada entrega obliga a migrar datos, rehacer disparadores y reescribir procedimientos — y nada de eso es lo que el curso enseña |
| **Por qué ninguna IA lo genera** | Porque ya está hecho, y la IA **no estuvo en la elicitación**. Si propone un `CREATE TABLE`, está rehaciendo a ciegas un trabajo que tiene autores |

---

| Qué | Dónde |
|---|---|
| Qué manda sobre qué | [`FUENTES_AUTORIDAD.md`](FUENTES_AUTORIDAD.md) |
| El modelo, tabla por tabla | [`DISENO_BD.md`](DISENO_BD.md) |
| Las 22 reglas, con quién las defiende | [`REGLAS_DE_NEGOCIO.md`](REGLAS_DE_NEGOCIO.md) |
| La elicitación | [`elicitacion/1_PREGUNTAS.md`](elicitacion/1_PREGUNTAS.md) |
