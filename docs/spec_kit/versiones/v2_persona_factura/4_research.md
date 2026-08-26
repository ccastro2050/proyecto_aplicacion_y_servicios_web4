# Investigación y decisiones — Versión 2

> **Versión 2** · **Lectura opcional**: el porqué del [plan](3_plan.md),
> con las alternativas que se descartaron.

---

## D1 — ¿Por qué persona Y factura en la misma versión?
Porque enseñan lecciones opuestas y se refuerzan: persona demuestra que el
molde de la v1 **se replica sin pensar** (si costó trabajo, la arquitectura
estaba mal); factura demuestra que no todo es CRUD — hay lógica que
**pertenece a la BD**. Una versión solo-persona sería trivial; una
solo-factura dejaría sin practicar la replicación.
**Alternativa descartada:** agregar también cliente/vendedor/empresa — puro
volumen sin lección nueva; su gestión llega en la v3 (el resto de las entidades).

## D2 — Factura SOLO por procedimientos almacenados
**Decisión:** el repositorio de factura no escribe SQL de tablas: llama 4
SPs (`CommandType.StoredProcedure`) y deserializa su JSON.
**Por qué:** la BD ya resuelve la transacción completa (encabezado +
renglones + trigger) — reimplementarla en C# duplicaría la lógica y abriría
la puerta a que API y BD calculen distinto (la pesadilla de consistencia).
Además es la lección ACID del curso en acción: la factura se crea ENTERA o
no se crea ([PRINCIPIOS_ACID.md](../../../PRINCIPIOS_ACID.md)).
**Alternativa descartada:** INSERTs desde C# con transacción ADO.NET —
funcionaría, pero repite lo que el trigger ya hace y no enseña SPs.

## D3 — ¿Por qué NO exponer sp_actualizar ni sp_borrar en v2?
Anular ES la operación de negocio real (las facturas no se editan ni se
borran en contabilidad; se anulan). Exponer el borrado físico invitaría a
usarlo. El SP existe para el administrador de BD; si una versión futura lo
necesita, será una decisión registrada en SU spec.

## D4 — Los renglones de la petición viajan al SP como JSON
El SP recibe `@p_productos NVARCHAR(MAX)` y lo abre con `OPENJSON`.
**Decisión:** el servicio serializa la lista (ya validada por la petición)
a ese JSON — un solo viaje a la BD, una sola transacción.
**Alternativa descartada:** un INSERT por renglón desde C# — N viajes y la
transacción quedaría del lado equivocado.

## D5 — Validación por capas: la petición primero, el SP de respaldo
`[MinLength(1)]` en la lista y `[Range(1, …)]` en cantidad matan en **422**
los errores de forma ANTES de tocar la BD. El `THROW 50002` del SP (mínimo
de renglones) queda como **defensa en profundidad**: protege a la BD de
CUALQUIER cliente, no solo de esta API. No es redundancia — son dos
fronteras distintas.

## D6 — Los THROW del SP → excepciones de negocio (en el repositorio)
`sp_consultar` y `sp_anular` lanzan error 50010 con mensajes distintos
("no existe" / "ya está anulada"). **Decisión:** el repositorio los captura
con `catch … when` y lanza `NoEncontradoExcepcion` (→404) o la nueva
`ConflictoExcepcion` (→409).
**Por qué en el repositorio:** los números de error del motor son un
detalle del proveedor de datos; servicio y controller no deben conocer
`SqlException`. **Por qué 409:** anular dos veces no es "no existe" (404)
ni "petición mal formada" (422) — es un conflicto con el ESTADO actual del
recurso: exactamente la semántica de 409 Conflict.

## D7 — Stock insuficiente → 500, no 400
El trigger lanza su error con el mensaje «Stock insuficiente…» y la API lo
reporta como 500 con ese mensaje en `detalle`. ¿No debería ser 400/409?
Defendible — pero exigiría reconocer los mensajes del trigger uno a uno.
**La v2 elige la regla simple de la v1:** lo que la BD rechaza es 500 con
el detalle visible. Refinar ese mapeo puede ser decisión de una versión
futura (quedaría registrado en su spec).

## D8 — Lecturas de factura como modelos tipados, no JSON crudo
El SP ya devuelve JSON: ¿por qué deserializar a `Factura`/`ProductoDeFactura`
en vez de re-emitirlo tal cual? Porque el contrato de la API es de la API:
tiparlo (a) congela el formato aunque el SP cambie, (b) da IntelliSense y
errores de compilación, y (c) enseña `System.Text.Json` con
`[JsonPropertyName]` para el snake_case. El costo (2 clases) es bajo.

## D9 — La prueba de capas de factura NO entra al criterio 6
El repositorio falso de persona es idéntico al de producto (diccionario) y
prueba la MISMA lección. Falsificar el de factura exigiría simular SPs,
triggers y JSON — mucho andamiaje para ninguna lección nueva: la lógica de
factura vive en la BD, así que su verificación honesta ES el smoke test
contra la BD real (criterios 3–5). La prueba de capas se amplía solo con
persona.
