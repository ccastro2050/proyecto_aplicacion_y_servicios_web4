# Versión 2 — Carlos · camino B (IDE agéntico)

> **Su parte:** el recurso **`factura`** — el único del sistema que **no escribe
> SQL de tablas**.
>
> **Su herramienta:** un IDE agéntico. El agente lee el repositorio y escribe los
> archivos él mismo.
>
> **Y usted va primero, solo.** No porque construya un molde —eso fue la v1—,
> sino porque construye **el recurso que rompe el molde**, y conviene que esa
> diferencia esté en el repositorio antes de que los otros dos empiecen.

---

## 1. Por qué `factura` no se parece a nada de lo que hay

Los ocho recursos que ya existen tienen `SELECT`, `INSERT`, `UPDATE` y `DELETE`
escritos en su repositorio. **El suyo no.**

| | Los otros ocho | `factura` |
|---|---|---|
| El repositorio | escribe SQL de tablas | **llama procedimientos** |
| Una operación | una escritura | **cuatro o más**, y tienen que ser **una sola** |
| Los verbos | los cinco | **GET, POST y un POST de anular** |

```
factura                numero (IDENTITY) · fecha · total · estado
                       fkidcliente · fkidvendedor

productosporfactura    fknumfactura + fkcodproducto  (llave COMPUESTA)
                       cantidad · subtotal
```

> **Insertar una factura con tres renglones son cuatro escrituras.** Desde C#
> serían cuatro viajes a la base y **cuatro oportunidades de quedar a medias**.
> Dentro del procedimiento hay **una transacción**: o entran las cuatro, o no
> entra ninguna.

### Los seis procedimientos YA EXISTEN

```
sp_insertar_factura_y_productosporfactura
sp_consultar_factura_y_productosporfactura
sp_listar_facturas_y_productosporfactura
sp_actualizar_factura_y_productosporfactura     ← existe, NO se expone
sp_borrar_factura_y_productosporfactura         ← existe, NO se expone
sp_anular_factura
```

> **Están en la base desde la v1** (Artículo 5). El agente **no los escribe**:
> los llama. Si propone un `CREATE PROCEDURE`, párelo.

> **Y dos de ellos existen y NO se exponen, a propósito.** La operación del
> negocio es **anular**, no corregir: una factura emitida no se edita. El `PUT`,
> el `PATCH` y el `DELETE` de `factura` quedan **escritos en el controlador y
> comentados**, con su razón al lado — para que se vea cómo se programa cada
> verbo aunque el negocio no lo quiera. Está en
> [`REQUISITOS_FUNCIONALES.md`](../../../dominio/REQUISITOS_FUNCIONALES.md) §6.

---

## 2. Tres reglas del negocio que NO se programan en C#

Y saber **quién las defiende** es la pregunta de sustentación de esta versión:

| La regla | Quién la defiende |
|---|---|
| El stock nunca queda negativo | un **disparador** — `THROW 50001` |
| El total es la suma de los subtotales | un **disparador**, que lo recalcula |
| Una factura no se anula dos veces | el **procedimiento**, que mira el estado |

> **Por qué en la base y no en la aplicación: porque la regla tiene que valer
> también para quien entre por SSMS.** Una validación que solo vive en C#
> protege a quien pasa por la API — y en la vida real siempre hay alguien que no
> pasa por ahí.

### Y el total NO lo manda el front

Tentador: el formulario ya tiene los renglones, ya sabe sumar.

> **Se descarta, y la razón es de seguridad.** Si el total llegara en el cuerpo,
> cualquiera podría mandar una factura de tres millones con total `1000`. Lo
> calcula un disparador, y la API **ni lo acepta si se lo mandan**.
>
> **La regla general:** del cliente se acepta lo que el cliente **sabe** —qué
> productos y cuántos—, nunca lo que se **deduce** de eso.

---

## 3. Antes de abrir el agente

```powershell
# 1 · Traiga lo último y ramifique desde ahí.
git switch main
git pull origin main
git switch -c rama-carlos-v2

# 2 · SU IDENTIDAD, parado en la carpeta del proyecto. Compruébela.
git config user.name "ccastro2050"
git config user.email "su-correo-de-github"
git config user.name ; git config user.email

# 3 · Compruebe que la v1 sigue funcionando ANTES de tocar nada.
#     Si ya estaba rota, no quiere descubrirlo al final.
docker compose up -d --build
Invoke-RestMethod http://localhost:8035/api/producto
```

---

## 4. Qué puede tocar el agente

| Puede escribir | NO puede tocar |
|---|---|
| `api_facturas/**` — lo de `factura` | `docs/` — **solo lectura** |
| `front_blazor/**` — las dos pantallas de factura | `db/bdfacturas.sql` — **viene dado** |
| | Los recursos de Paco y Luis (§5) |

---

## 5. Lo que usted construye, y lo que NO

**Usted construye `factura`, y nada más.**

**NO construya** `cliente`, `vendedor`, las tablas puente ni `usuario-con-roles`
— son de Paco y de Luis.

> **Si el agente los construye «de una vez, que están en el spec», bórrelos antes
> del commit.** La spec describe la versión **completa**; su rama entrega **su
> tercio**, y la sustentación es individual.

---

## 6. El prompt (cópielo tal cual)

```
Construye UN SOLO RECURSO de la VERSIÓN 2 de este proyecto: `factura`.
Trabajo en equipo: otros dos compañeros construyen los demás recursos de
esta versión, así que NO toques nada que no sea de factura.

PRIMERO lee, en este orden, los documentos que están bajo docs/spec_kit/
(1_constitution.md en la raíz; los demás en versiones/v2_con_fk/):
1_constitution, 2_spec, 3_plan, 4_research, 5_data_model, 6_contracts,
7_quickstart y 8_tasks. Lee también el código de la versión 1 que ya está
en el repositorio, para seguir su misma estructura de capas. Después
resume en máximo 10 líneas qué vas a construir y ESPERA MI CONFIRMACIÓN
antes de escribir un solo archivo.

LO QUE TIENES QUE CONSTRUIR:

   factura               numero (INT IDENTITY, PK) · fecha · total ·
                         estado · fkidcliente · fkidvendedor
   productosporfactura   (fknumfactura, fkcodproducto) llave COMPUESTA ·
                         cantidad · subtotal

Con CUATRO operaciones, y solo cuatro:
   GET  /api/factura              listar
   GET  /api/factura/{numero}     consultar una, con sus renglones
   POST /api/factura              crear el encabezado CON sus renglones
   POST /api/factura/{numero}/anular

ESTO ES LO QUE HACE DISTINTO A ESTE RECURSO, Y NO SE NEGOCIA:

  1. EL REPOSITORIO DE FACTURA NO ESCRIBE SQL DE TABLAS. Llama a los
     procedimientos que YA EXISTEN en la base:
        sp_listar_facturas_y_productosporfactura
        sp_consultar_factura_y_productosporfactura
        sp_insertar_factura_y_productosporfactura
        sp_anular_factura
     NO escribas CREATE PROCEDURE ni CREATE TABLE: la base viene dada
     (Artículo 5). Si crees que falta un procedimiento, PREGÚNTAME.

  2. NO EXPONGAS PUT, PATCH NI DELETE de factura. Existen dos
     procedimientos para eso en la base y NO se usan: la operación del
     negocio es ANULAR, no corregir. Escribe los tres métodos en el
     controlador pero DÉJALOS COMENTADOS, con un comentario que explique
     por qué están apagados.

  3. EL TOTAL Y EL SUBTOTAL NO VIAJAN EN LA PETICIÓN. Los calcula un
     disparador en la base. Si llegan en el body, se ignoran. La petición
     de crear lleva: fkidcliente, fkidvendedor, y una lista de productos
     con `codigo` y `cantidad`. NADA MÁS.

  4. NO VALIDES EL STOCK EN C#. Lo hace un disparador, que lanza
     THROW 50001 con el mensaje exacto. Ese error llega al catch final y
     se responde 500 con el mensaje del motor en `detalle` — así está
     declarado en 2_spec.md. NO lo traduzcas a 400 ni a 409.

  5. SÍ traduce estos: clave foránea inexistente y factura ya anulada
     responden 409; factura inexistente responde 404.

REGLAS QUE SIGUEN VIGENTES DE LA V1:

  · Las TRES CAPAS con interfaces. El controlador no escribe SQL ni llama
    procedimientos directamente; el servicio NO nombra nada de HTTP —ni
    StatusCode, ni NotFound, ni IActionResult—; el repositorio no decide
    códigos de estado.
  · SIN ORM de entidades. Dapper, y siempre parametrizado.
  · TODO EN ESPAÑOL.
  · El front NO habla con la base: solo con la API por HTTP.

LA INTERFAZ GRÁFICA, que son DOS pantallas:
  · El listado de facturas, con su estado (activa / anulada) y el botón
    de anular.
  · El formulario de emisión, que es MAESTRO-DETALLE: se escogen cliente
    y vendedor en DESPLEGABLES cargados de la API, se agregan renglones
    uno por uno a una lista en pantalla, y SOLO al final se manda la
    factura completa en UNA petición. No guardes el encabezado primero y
    los renglones después: tiene que ser una sola llamada.

Y UN DETALLE QUE SE VA A ROMPER SI NO LO CUIDAS: los procedimientos
devuelven columnas como `nombre_cliente` e `idrol`, y las propiedades de
C# se llaman NombreCliente e IdRol. Sin [JsonPropertyName] llegan `null`
y 0, SIN ERROR. Ponlos.

COMENTA TODO LO QUE ESCRIBAS, en español. Cada archivo empieza diciendo
QUÉ ES y QUÉ PAPEL cumple. Los comentarios dicen POR QUÉ está escrito
así, no QUÉ hace la línea.

ORDEN DE TRABAJO: primero el GET de listar —para ver que el procedimiento
responde—, luego el GET de uno, luego el POST, y de último anular. Al
terminar cada uno dime qué archivos creaste y espera.
```

---

## 7. Cómo supervisar al agente

| Momento | Qué mirar |
|---|---|
| **El resumen inicial** | ¿Entendió que son CUATRO operaciones y no cinco verbos? ¿Dijo que llama procedimientos? |
| **Cuando entrega el repositorio** | Que **no haya ni un `SELECT ... FROM factura`**. Si lo hay, calcó el molde de la v1 donde no iba |
| **El POST** | Que la petición **no acepte** total ni subtotal |
| **Antes del commit** | Que no haya construido lo de Paco y Luis |

```powershell
# El repositorio de factura NO escribe SQL de tablas. Esto tiene que dar 0.
Select-String -Path api_facturas\Repositorios\RepositorioFactura*.cs `
  -Pattern 'FROM factura|INSERT INTO|UPDATE factura' | Measure-Object | Select-Object Count

# Y sí llama procedimientos. Esto NO puede dar 0.
Select-String -Path api_facturas\Repositorios\RepositorioFactura*.cs `
  -Pattern 'sp_' | Measure-Object | Select-Object Count
```

---

## 8. La comprobación que de verdad importa

```powershell
# 1 · Anote el stock de PR005 (tiene 14).
Invoke-RestMethod http://localhost:8035/api/producto/PR005

# 2 · LA PRUEBA DE LA TRANSACCIÓN: tres renglones, y el SEGUNDO no alcanza.
$f = @{ fkidcliente=1; fkidvendedor=1; productos=@(
        @{ codigo='PR003'; cantidad=1 },
        @{ codigo='PR005'; cantidad=99 },
        @{ codigo='PR004'; cantidad=1 }) }
Invoke-RestMethod http://localhost:8035/api/factura -Method Post `
  -ContentType 'application/json' -Body ($f | ConvertTo-Json -Depth 4)

# 3 · ¿Quedó algo a medias? NO debe haber factura nueva, y el stock de
#     PR003 tiene que estar INTACTO.
Invoke-RestMethod http://localhost:8035/api/producto/PR003
```

> **Si PR003 bajó, la transacción no existe** aunque el procedimiento diga
> `BEGIN TRANSACTION`. Es el criterio que más se olvida y el único que prueba lo
> que esta versión promete.

> **Y la respuesta esperada del paso 2 es un 500**, con el mensaje del
> disparador en `detalle`: *«Stock insuficiente para producto PR005. Stock
> disponible: 14, cantidad solicitada: 99»*. **No es un error de su código.**

```powershell
# 4 · La devolución de stock al anular.
#     Anote el stock, emita una factura PEQUEÑA, anúlela, y compare: igual.

# 5 · Anular dos veces la misma: la segunda debe dar 409.
```

> **Pruebe solo sobre filas que usted creó, y bórrelas.** Las seis facturas
> sembradas son material de clase.

---

## 9. Subir, y después integrar

```powershell
git status                      # SOLO sus archivos
git add api_facturas/Modelos/Factura.cs api_facturas/Peticiones/Factura*.cs
git commit -m "feat: factura, el modelo y la peticion de emision"
#   ... capa por capa
git push -u origin rama-carlos-v2
#   y el Pull Request hacia main
```

**Cuando los dos PR estén arriba**, usted integra:

```powershell
# 1 · Revise cada PR: ¿compila? ¿cumple la spec? ¿respeta las capas?
# 2 · Fusione desde GitHub. Solo usted.
# 3 · Traiga main y corra LA REGRESIÓN DE LA V1 ENTERA.
git switch main ; git pull origin main ; docker compose up -d --build

# 4 · Con los criterios en verde y el 9_checklist marcado POR UNA PERSONA:
git tag -a v2 -m "Version 2: las seis tablas con clave foranea"
git push origin v2
```

> **El paso 3 no es opcional.** Este proyecto es acumulativo: la v2 incluye la
> v1. Una versión que rompe la anterior no está terminada, está cambiada — y la
> única forma de saberlo es volver a probar lo viejo.
