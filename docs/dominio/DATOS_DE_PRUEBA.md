# Datos de prueba — `bdfacturas`

> **Qué es este archivo.** Qué trae la base **recién sembrada**: cuántas filas,
> cuáles, y **para qué sirve cada una**. Porque varias están puestas a propósito
> para que algo se pueda probar.
>
> **Qué NO es.** No es el `INSERT`. Ése está en
> [`db/bdfacturas.sql`](../../db/bdfacturas.sql), al final. Aquí se dice **por
> qué** esos datos y no otros.
>
> **Material académico simulado**: los nombres son inventados, las cifras son
> verosímiles, y el sistema funciona con ellos.
>
> **Medido contra la base recién levantada**, no copiado del script.
>
> Versión 1.0 · 4 de octubre de 2026.

---

## 1. Las cuentas

| Tabla | Filas | Para qué esa cantidad |
|---|---|---|
| `empresa` | 3 | Suficientes para un desplegable con opciones de verdad |
| `persona` | 6 | **Una de ellas cumple los dos papeles** — ver §2 |
| `producto` | 8 | Con stocks distintos, para que «stock insuficiente» se pueda provocar |
| `cliente` | 4 | Uno de ellos **sin empresa**, para probar la llave foránea opcional |
| `vendedor` | 3 | |
| `factura` | 6 | Con 1, 2 y 3 renglones: los tres casos del maestro-detalle |
| `productosporfactura` | 12 | |
| `rol` | 5 | |
| `ruta` | 15 | Las interfaces protegibles |
| `usuario` | 8 | Con repartos de roles distintos — ver §4 |
| `rol_usuario` | 21 | |
| `rutarol` | 25 | Ni todos los roles a todas las rutas: así el 403 se puede provocar |

> **Ninguna factura nace anulada.** Las 6 están `activa`, para que la primera
> anulación que se haga sea la que se está probando.

---

## 2. Las personas, y la que importa

| Código | Nombre | ¿cliente? | ¿vendedor? |
|---|---|---|---|
| P001 | Ana Torres | sí | — |
| P002 | Carlos Pérez | — | sí |
| P003 | María Gómez | sí | — |
| P004 | Juan Díaz | — | sí |
| P005 | Laura Rojas | sí | — |
| **P006** | **Pedro Castillo** | **sí** | **sí** |

> **Pedro Castillo está ahí a propósito.** Es la prueba viva de por qué
> `cliente` y `vendedor` son tablas aparte y no una columna `tipo` en `persona`:
> con un `tipo` habría dos filas con su nombre, su correo y su teléfono, y el
> día que cambie de número una de las dos se queda vieja. Ver
> [`DISENO_BD.md`](DISENO_BD.md) §1.

---

## 3. Los productos y sus stocks

| Código | Producto | Stock sembrado |
|---|---|---|
| PR001 | Laptop Lenovo IdeaPad | 17 |
| PR002 | Monitor Samsung 24" | 27 |
| PR003 | Teclado Logitech K380 | 42 |
| PR004 | Mouse HP | 56 |
| PR005 | Impresora Epson EcoTank1 | **14** |
| PR006 | Auriculares Sony WH-CH510 | 23 |
| PR007 | Tablet Samsung Tab A9 | 15 |
| PR008 | Disco Duro Seagate 1TB | 32 |

> **Estos son los stocks DESPUÉS de las seis facturas sembradas**, no los
> iniciales: el disparador ya descontó lo vendido. Es decir, la base llega en un
> estado coherente, no con unos números sueltos.

> **PR005 con 14 es el más cómodo para provocar el `THROW 50001`:** pida 50 y la
> base responderá *«Stock disponible: 14, cantidad solicitada: 50»*.

---

## 4. Los usuarios y sus roles

| Correo | Contraseña | Roles |
|---|---|---|
| `admin@correo.com` | `admin123` | Administrador |
| `vendedor1@correo.com` | `vendedor123` | Vendedor, Cajero |
| `cliente1@correo.com` | `cliente123` | Cliente |
| `jefe@correo.com` | — | Administrador, Cajero, Contador |
| `nuevo@correo.com` | — | Administrador, Vendedor, Cajero |
| `test_encript@correo.com` | — | Administrador |
| `carlos.castro@usbmed.edu.co` | — | los cinco |
| `carloscastro5033@correo.itm.edu.co` | — | los cinco |

> **Las contraseñas están en la base CIFRADAS.** Las tres de arriba se conocen
> porque el curso las publica para poder entrar; las demás se sembraron con un
> hash y **nadie sabe su clave** — y eso también es correcto: así debe ser.

> **Los tres primeros usuarios son los que importan para probar**, y están
> escogidos para que el 403 se pueda ver sin preparar nada:
>
> | Entre como | Y verá | Pero NO verá |
> |---|---|---|
> | `admin@correo.com` | todo | — |
> | `vendedor1@correo.com` | Facturas, Clientes | Usuarios, Personas, Productos |
> | `cliente1@correo.com` | Productos | Facturas, Clientes — **al revés que el vendedor** |
>
> Que el cliente vea lo que el vendedor no, y al revés, es deliberado: si uno
> viera un subconjunto del otro, un error de permisos podría pasar
> desapercibido.

---

## 5. Las 15 rutas protegibles

```
interfaz.inicio      interfaz.usuarios    interfaz.facturas
interfaz.clientes    interfaz.vendedores  interfaz.personas
interfaz.empresas    interfaz.productos   interfaz.roles
interfaz.permisos    interfaz.rutas       permiso.crear
permiso.eliminar     ruta.crear           ruta.eliminar
```

> **Fíjese en que no son direcciones de la API.** `interfaz.facturas` no es
> `/api/factura`: es el **nombre de una pantalla**. Una fila de `ruta` es una
> etiqueta que el control de acceso usa para decidir. Buscar
> `/api/interfaz.facturas` no lleva a ninguna parte.

> Y hay cinco que no empiezan por `interfaz.`: `permiso.*` y `ruta.*` protegen
> **acciones**, no pantallas. Repartir permisos es algo que se hace, no algo que
> se mira.

---

## 6. Cómo volver a este estado

Las pruebas ensucian: una factura emitida se queda, y el stock baja.

```powershell
docker compose down          # apaga y CONSERVA los datos
docker compose down -v       # ⚠ borra el volumen: la base vuelve a sembrarse
docker compose up -d
```

> **`down -v` borra lo que haya.** Si alguien estaba trabajando con datos
> propios, los pierde. Para una prueba puntual es mejor crear la fila, probar y
> borrarla — y si se tocó una fila sembrada, reponerla.

---

## 7. Comprobar las cuentas

```powershell
docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa `
  -P "Paradigmas123!" -C -d bdfacturas_sqlserver_local -Q `
  "SELECT 'factura', COUNT(*) FROM factura UNION ALL SELECT 'renglones', COUNT(*) FROM productosporfactura;"
```

Recién sembrada debe decir **6 y 12**. Si dice más, alguien dejó datos de una
prueba — y conviene saber quién antes de borrarlos.
