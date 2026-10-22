# Reglas de negocio — Facturación (`bdfacturas`)

> **Qué es este archivo.** Lo que es **verdad siempre** en este dominio, sin
> importar qué endpoint se llame. No son requisitos: un requisito es algo que el
> sistema **hace**; una regla es algo que **no puede dejar de ser cierto**.
>
> **La diferencia, explicada:**
> [`CONCEPTOS_REGLAS_DE_NEGOCIO.md`](../conceptos/CONCEPTOS_REGLAS_DE_NEGOCIO.md).
>
> **Cada regla dice quién la defiende**, y esa columna es la mitad del valor del
> documento: una regla que nadie defiende es un deseo.
>
> **Material académico simulado.** El dominio es verosímil y el sistema funciona,
> pero no hay un cliente real detrás: las reglas las fijó el curso. Ver
> [`elicitacion/`](elicitacion/).
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Cómo leer la tabla

| Columna | Qué dice |
|---|---|
| **Quién la defiende** | El mecanismo concreto. Si dice «nadie», la regla **no está defendida** y hay que decirlo |
| **Qué pasa si se viola** | El código que recibe quien lo intenta |

Tres mecanismos, y conviene distinguirlos porque fallan distinto:

- **La base de datos** — una restricción (`PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`,
  `NOT NULL`). No se puede evadir, ni siquiera desde SSMS.
- **Un disparador o procedimiento** — código SQL que rechaza con `THROW`. Tampoco
  se evade, y además explica el motivo.
- **El servicio** — código C#. Solo protege a quien pase por la API.

> **El orden importa.** Una regla defendida por el servicio y no por la base de datos se
> rompe con una línea de SQL. Las reglas que de verdad no pueden fallar están
> abajo del todo, en la base de datos.

---

## 2. La factura

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-01** | Toda factura tiene **un cliente y un vendedor**, y los dos existen | `fk_factura_cliente` y `fk_factura_vendedor` | 500 con el error del motor |
| **RN-02** | Una factura tiene **al menos un renglón**. No existe una factura vacía | `sp_insertar_factura_y_productosporfactura`, con su mínimo y su `THROW` | 500 · *«La factura requiere minimo 1 producto(s).»* |
| **RN-03** | El **número** de la factura lo pone la base de datos y no se repite | `IDENTITY(1,1)` + `pk_factura` | — |
| **RN-04** | La **fecha** es la del momento en que se emitió, y no la manda el cliente | `DEFAULT GETDATE()` | lo que llegue se ignora |
| **RN-05** | Una factura está **activa** o **anulada**. No hay un tercer estado | `DEFAULT N'activa'` y solo `sp_anular_factura` lo cambia | — |
| **RN-06** | El mismo producto **no aparece dos veces** en una factura: se suma la cantidad | `pk_productosporfactura (fknumfactura, fkcodproducto)` | 500 por llave duplicada |

> **RN-02 es la que más se olvida al construir el front**, y por eso la pantalla
> de emitir muestra «Agregue al menos un renglón» en vez de dejar oprimir
> «Emitir» con la lista vacía. La base de datos la rechazaría igual — pero decirlo antes
> es más barato que un 500.

---

## 3. El dinero

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-07** | El **subtotal** de un renglón es `cantidad × valor unitario` **del momento en que se emitió**, y queda congelado | `trg_prodfact_insert` lo calcula y lo guarda | no se puede: el cliente no lo manda |
| **RN-08** | El **total** de la factura es la suma de sus subtotales | `trg_prodfact_insert`, `_update` y `_delete` lo recalculan | no se puede |
| **RN-09** | Ni el subtotal ni el total viajan en la petición: **si llegan, se ignoran** | la lista blanca del controlador | — |

> **RN-07 es una decisión de negocio, no una optimización.** Si el subtotal se
> recalculara con el precio de hoy, subir el precio de un producto **reescribiría
> las ventas del año pasado**. Por eso se guarda aunque sea un dato derivado —
> es el caso que [`CONCEPTOS_DISENO_BD.md`](../conceptos/CONCEPTOS_DISENO_BD.md)
> llama «congelar el derivado porque congelarlo es el requisito».

> **Y RN-08 explica por qué el front muestra un «total estimado».** Lo que se ve
> antes de emitir es un cálculo para que la persona sepa cuánto va. El total que
> queda guardado lo pone el disparador, y es el que manda. Dos fuentes de
> verdad, y gana la de la base de datos.

---

## 4. El inventario

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-10** | No se vende lo que no hay: el stock **nunca queda negativo** | `trg_prodfact_insert` y `_update`, con `THROW 50001` | 500 · *«Stock insuficiente para producto PR003. Stock disponible: 8, cantidad solicitada: 50»* |
| **RN-11** | Emitir una factura **baja** el stock de cada producto | los mismos disparadores | — |
| **RN-12** | **Anular** una factura **devuelve** el stock | `sp_anular_factura` | — |
| **RN-13** | Borrar una factura también devuelve el stock | `trg_prodfact_delete` | — |

> **RN-10 trae el número exacto en el mensaje**, y eso no es cosmético: «stock
> insuficiente» obliga a ir a mirar cuánto hay; «hay 8 y pidió 50» deja a la
> persona corregir sin salir de la pantalla.

---

## 5. Anular, que no es borrar

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-14** | Una factura anulada **se queda en la base de datos**, con su número y su fecha | `sp_anular_factura` hace `UPDATE`, no `DELETE` | — |
| **RN-15** | Una factura **no se anula dos veces** | `sp_anular_factura`, con `THROW 50010` | 409 · *«Factura 3 ya está anulada»* |
| **RN-16** | Anular una factura que no existe no es un conflicto: es que no está | `sp_anular_factura`, con `THROW 50010` | 404 |

> **Por qué una factura no se borra.** Porque la numeración es consecutiva y un
> hueco en ella es una pregunta que alguien va a tener que responder en una
> auditoría. Anular deja el rastro: existió, y se dejó sin efecto.
>
> El `DELETE` existe en la API y **está apagado** por esta razón — vea
> `FacturaController`, que lo trae escrito y comentado.

---

## 6. El control de acceso

| | Regla | Quién la defiende | Si se viola |
|---|---|---|---|
| **RN-17** | La contraseña **nunca se guarda en claro** | el repositorio, que cifra con BCrypt antes del `INSERT` | — |
| **RN-18** | La contraseña **nunca sale** en una respuesta | el modelo, que no la tiene, y los `SELECT`, que no la piden | — |
| **RN-19** | Un usuario puede tener **varios roles** | `pk_rol_usuario (fkemail, fkidrol)` | — |
| **RN-20** | Un rol puede tener **varias rutas**, y una ruta puede estar en varios roles | `pk_rutarol (fkidruta, fkidrol)` | — |
| **RN-21** | El **permiso se consulta en cada petición**, no se lee del token | `verificar_acceso_ruta`, llamada por `[ExigePermiso]` | — |
| **RN-22** | La ruta es **única**: no hay dos filas con el mismo nombre de interfaz | `uq_ruta` | 500 por índice único |

> **RN-21 es la que hace que el sistema sea administrable.** Si el permiso
> viajara dentro del token, quitárselo a un rol no surtiría efecto hasta que la
> persona volviera a entrar. Así, surte efecto en la petición siguiente.

> **RN-18 tiene una consecuencia que se ve en el código:** la clase `Usuario` no
> tiene la propiedad `Contrasena`. No es que se filtre al serializar — es que no
> existe el dato que filtrar.

---

## 7. Lo que NINGUNA regla defiende, y hay que decirlo

Un documento de reglas que solo lista lo que está protegido engaña por omisión:

| Lo que NO se valida | Qué pasa |
|---|---|
| Que el **crédito** del cliente alcance para la compra | Se puede facturar por encima del cupo. `credito` es informativo |
| Que el correo de `persona` o de `usuario` **tenga forma de correo** | Se acepta `aaa` |
| Que el **teléfono** tenga forma de teléfono | Igual |
| Que la **cantidad** de un renglón sea razonable | 1.000.000 de unidades se acepta si hay stock |
| Que el **valor unitario** sea mayor que cero | Un producto puede valer 0 |

> **No están validadas a propósito**, para que el proyecto de aula tenga qué
> agregar: cada una es un criterio de aceptación que un equipo puede escribir y
> defender. Lo que no se vale es creer que ya están.

---

## 8. Dónde comprobar cada regla

Ninguna fila de este documento es una opinión:

| Qué | Dónde |
|---|---|
| Las restricciones | [`db/bdfacturas.sql`](../../db/bdfacturas.sql), las `CONSTRAINT` |
| Los disparadores | el mismo archivo, `trg_prodfact_insert`, `_update`, `_delete` |
| Los `THROW` y sus números | el mismo archivo — 50001 es el stock, 50010 el anulado |
| Cómo se traducen a HTTP | [`POLITICA_DE_ERRORES.md`](POLITICA_DE_ERRORES.md) |
| El glosario de los términos | [`GLOSARIO.md`](GLOSARIO.md) |
