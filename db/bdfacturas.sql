-- ============================================================
-- Script de creación de base de datos: bdfacturas_sqlserver_local
-- Compatible con SQL Server 2016+
-- Incluye: tablas, restricciones, triggers y datos de ejemplo
-- ============================================================

USE bdfacturas_sqlserver_local;
GO

-- ============================================================
-- LIMPIEZA: Eliminar objetos existentes en orden correcto
-- ============================================================

-- Triggers
IF OBJECT_ID('trg_prodfact_insert', 'TR') IS NOT NULL DROP TRIGGER trg_prodfact_insert;
IF OBJECT_ID('trg_prodfact_update', 'TR') IS NOT NULL DROP TRIGGER trg_prodfact_update;
IF OBJECT_ID('trg_prodfact_delete', 'TR') IS NOT NULL DROP TRIGGER trg_prodfact_delete;
GO

-- Procedimientos almacenados
IF OBJECT_ID('sp_insertar_factura_y_productosporfactura', 'P') IS NOT NULL DROP PROCEDURE sp_insertar_factura_y_productosporfactura;
IF OBJECT_ID('sp_consultar_factura_y_productosporfactura', 'P') IS NOT NULL DROP PROCEDURE sp_consultar_factura_y_productosporfactura;
IF OBJECT_ID('sp_listar_facturas_y_productosporfactura', 'P') IS NOT NULL DROP PROCEDURE sp_listar_facturas_y_productosporfactura;
IF OBJECT_ID('sp_actualizar_factura_y_productosporfactura', 'P') IS NOT NULL DROP PROCEDURE sp_actualizar_factura_y_productosporfactura;
IF OBJECT_ID('sp_borrar_factura_y_productosporfactura', 'P') IS NOT NULL DROP PROCEDURE sp_borrar_factura_y_productosporfactura;
IF OBJECT_ID('sp_anular_factura', 'P') IS NOT NULL DROP PROCEDURE sp_anular_factura;
IF OBJECT_ID('crear_usuario_con_roles', 'P') IS NOT NULL DROP PROCEDURE crear_usuario_con_roles;
IF OBJECT_ID('actualizar_usuario_con_roles', 'P') IS NOT NULL DROP PROCEDURE actualizar_usuario_con_roles;
IF OBJECT_ID('eliminar_usuario_con_roles', 'P') IS NOT NULL DROP PROCEDURE eliminar_usuario_con_roles;
IF OBJECT_ID('actualizar_roles_usuario', 'P') IS NOT NULL DROP PROCEDURE actualizar_roles_usuario;
IF OBJECT_ID('consultar_usuario_con_roles', 'P') IS NOT NULL DROP PROCEDURE consultar_usuario_con_roles;
IF OBJECT_ID('listar_usuarios_con_roles', 'P') IS NOT NULL DROP PROCEDURE listar_usuarios_con_roles;
IF OBJECT_ID('verificar_acceso_ruta', 'P') IS NOT NULL DROP PROCEDURE verificar_acceso_ruta;
IF OBJECT_ID('listar_rutarol', 'P') IS NOT NULL DROP PROCEDURE listar_rutarol;
IF OBJECT_ID('crear_rutarol', 'P') IS NOT NULL DROP PROCEDURE crear_rutarol;
IF OBJECT_ID('eliminar_rutarol', 'P') IS NOT NULL DROP PROCEDURE eliminar_rutarol;
GO

-- Tablas dependientes primero
IF OBJECT_ID('rutarol', 'U') IS NOT NULL DROP TABLE rutarol;
IF OBJECT_ID('rol_usuario', 'U') IS NOT NULL DROP TABLE rol_usuario;
IF OBJECT_ID('productosporfactura', 'U') IS NOT NULL DROP TABLE productosporfactura;
IF OBJECT_ID('factura', 'U') IS NOT NULL DROP TABLE factura;
IF OBJECT_ID('cliente', 'U') IS NOT NULL DROP TABLE cliente;
IF OBJECT_ID('vendedor', 'U') IS NOT NULL DROP TABLE vendedor;
IF OBJECT_ID('empresa', 'U') IS NOT NULL DROP TABLE empresa;
IF OBJECT_ID('persona', 'U') IS NOT NULL DROP TABLE persona;
IF OBJECT_ID('producto', 'U') IS NOT NULL DROP TABLE producto;
IF OBJECT_ID('rol', 'U') IS NOT NULL DROP TABLE rol;
IF OBJECT_ID('ruta', 'U') IS NOT NULL DROP TABLE ruta;
IF OBJECT_ID('usuario', 'U') IS NOT NULL DROP TABLE usuario;
GO

-- ============================================================
-- TABLAS INDEPENDIENTES (sin foreign keys)
--
-- Van primero porque NADA de aqui apunta a otra tabla. Y el orden no es
-- estetico: una clave foranea solo se puede crear si la tabla a la que
-- apunta YA existe. Si se intenta crear `cliente` antes que `persona`, el
-- motor rechaza el script.
--
-- SON LAS SEIS DE LA VERSION 1: con estas seis se hace un CRUD completo sin
-- tocar una sola clave foranea.
-- ============================================================

-- ------------------------------------------------------------
-- empresa — las empresas a las que puede pertenecer un cliente.
--
-- PARA QUE: distinguir al cliente que compra por su cuenta del que compra a
-- nombre de una empresa. En `cliente` esa relacion es OPCIONAL.
--
-- LA CLAVE ES EL CODIGO, no un autonumerico: el codigo lo pone el negocio
-- (EM001), existe en el mundo real y se puede dictar por telefono. Un
-- autonumerico solo existe dentro de esta base.
-- ------------------------------------------------------------
CREATE TABLE empresa (
    codigo NVARCHAR(10) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    CONSTRAINT pk_empresa PRIMARY KEY (codigo)
);

-- ------------------------------------------------------------
-- persona — los DATOS de una persona: nombre, correo, telefono.
--
-- PARA QUE: es la tabla raiz de la gente. `cliente` y `vendedor` NO repiten
-- el nombre ni el telefono: apuntan aqui. La misma persona puede ser las dos
-- cosas sin que sus datos existan dos veces y se contradigan.
--
-- OJO CON EL CORREO: aqui NO es unico, es un dato de contacto. El correo que
-- no se puede repetir es el de `usuario`, que si es clave primaria. Son dos
-- correos con dos oficios distintos.
-- ------------------------------------------------------------
CREATE TABLE persona (
    codigo NVARCHAR(10) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    telefono NVARCHAR(20) NOT NULL,
    CONSTRAINT pk_persona PRIMARY KEY (codigo)
);

-- ------------------------------------------------------------
-- producto — el catalogo: que se vende, a como, y cuanto hay.
--
-- PARA QUE: `valorunitario` es el precio con el que se calcula el subtotal
-- de cada renglon de factura, y `stock` es lo que hay en bodega.
--
-- EL STOCK NO LO MUEVE LA API: lo mueven los disparadores de mas abajo, al
-- insertar o borrar un renglon. Si alguien tambien lo baja desde C#, el
-- stock baja DOS veces. Esta escrito aqui porque es el error mas caro del
-- proyecto y no se ve leyendo el C#.
-- ------------------------------------------------------------
CREATE TABLE producto (
    codigo NVARCHAR(10) NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    stock INT NOT NULL,
    valorunitario DECIMAL(18,2) NOT NULL,
    CONSTRAINT pk_producto PRIMARY KEY (codigo)
);

-- ------------------------------------------------------------
-- rol — los perfiles del sistema: Administrador, Vendedor, Cajero, Cliente.
--
-- PARA QUE: el permiso no se le da a una persona, se le da a un ROL, y la
-- persona recibe el rol. Asi, cambiar lo que puede hacer un cargo se hace en
-- un sitio y no usuario por usuario.
--
-- La clave es un autonumerico (IDENTITY) y el nombre NO es la clave: un rol
-- se puede renombrar sin que se caigan los permisos que ya tiene asignados.
-- ------------------------------------------------------------
CREATE TABLE rol (
    id INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(50) NOT NULL,
    CONSTRAINT pk_rol PRIMARY KEY (id)
);

-- ------------------------------------------------------------
-- ruta — LAS INTERFACES DEL SISTEMA: una fila por pantalla protegible
-- (interfaz.usuarios, interfaz.facturas, ...).
--
-- PARA QUE: es el catalogo de lo que se puede permitir. El nombre que esta
-- aqui es exactamente el que el codigo exige en C#:
--
--     [ExigePermiso("interfaz.usuarios")]
--
-- EL UNIQUE SOBRE `ruta` ES LO QUE SOSTIENE ESO: si el nombre se pudiera
-- repetir, habria dos filas distintas respondiendo por la misma pantalla.
--
-- Y UNA RUTA QUE NO ESTE DECLARADA AQUI NO LA PUEDE USAR NADIE: el
-- repositorio responde false sin preguntarle a nadie. Falla cerrado.
-- ------------------------------------------------------------
CREATE TABLE ruta (
    id INT IDENTITY(1,1) NOT NULL,
    ruta NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(200) NOT NULL,
    CONSTRAINT pk_ruta PRIMARY KEY (id),
    CONSTRAINT uq_ruta UNIQUE (ruta)
);

-- ------------------------------------------------------------
-- usuario — quien puede entrar al sistema. El correo ES la clave.
--
-- PARA QUE: identificarse. Ojo con la diferencia: `persona` es quien es
-- alguien; `usuario` es quien tiene llave. Hay personas sin usuario.
--
-- `contrasena` GUARDA EL HASH, NUNCA EL TEXTO. Es un hash BCrypt —unos 60
-- caracteres— y de ahi el NVARCHAR(200): sobra espacio a proposito, para que
-- cambiar de algoritmo no obligue a alterar la tabla.
--
-- Y el hash no se compara con otro hash: se VERIFICA con BCrypt. Dos hashes
-- del mismo texto son distintos, porque cada uno lleva su propia sal dentro.
-- ------------------------------------------------------------
CREATE TABLE usuario (
    email NVARCHAR(100) NOT NULL,
    contrasena NVARCHAR(200) NOT NULL,
    CONSTRAINT pk_usuario PRIMARY KEY (email)
);
GO

-- ============================================================
-- TABLAS DEPENDIENTES (con foreign keys)
--
-- Cada una apunta a alguna de las seis de arriba, y por eso van despues.
-- SON LAS SEIS DE LA VERSION 2, y traen tres cosas que la v1 no tenia:
--
--   1. LA CLAVE FORANEA, que en la interfaz grafica se vuelve un
--      desplegable: se elige un padre que existe, no se digita.
--   2. LA RELACION MAESTRO-DETALLE: factura y productosporfactura.
--   3. LAS TABLAS PUENTE con llave compuesta: rol_usuario y rutarol.
-- ============================================================

-- ------------------------------------------------------------
-- cliente — una persona que COMPRA.
--
-- PARA QUE: guarda lo que es propio de comprar —el credito— sin repetir los
-- datos de la persona.
--
-- `fkcodempresa` ES LA UNICA CLAVE FORANEA OPCIONAL DEL ESQUEMA: fijese que
-- no dice NOT NULL. No es un descuido — es la diferencia entre el cliente que
-- compra por su cuenta y el que compra a nombre de una empresa. En la
-- interfaz es la opcion «(ninguna)» del desplegable, y llega como null.
--
-- `credito DEFAULT 0`: un cliente nuevo no nace con credito. La consulta
-- «credito contra consumo» de la v4 compara este numero con lo que de verdad
-- ha comprado.
-- ------------------------------------------------------------
CREATE TABLE cliente (
    id INT IDENTITY(1,1) NOT NULL,
    credito DECIMAL(18,2) NOT NULL DEFAULT 0,
    fkcodpersona NVARCHAR(10) NOT NULL,
    fkcodempresa NVARCHAR(10),
    CONSTRAINT pk_cliente PRIMARY KEY (id),
    CONSTRAINT fk_cliente_persona FOREIGN KEY (fkcodpersona) REFERENCES persona(codigo),
    CONSTRAINT fk_cliente_empresa FOREIGN KEY (fkcodempresa) REFERENCES empresa(codigo)
);

-- ------------------------------------------------------------
-- vendedor — una persona que VENDE. El otro papel de `persona`.
--
-- PARA QUE: cada factura tiene que saber quien la hizo, y el `carnet` y la
-- `direccion` son datos del empleado, no de la persona.
--
-- La misma persona puede estar en `cliente` y en `vendedor`: son dos papeles,
-- no dos personas. Esa es toda la razon de que `persona` exista aparte.
-- ------------------------------------------------------------
CREATE TABLE vendedor (
    id INT IDENTITY(1,1) NOT NULL,
    carnet INT NOT NULL,
    direccion NVARCHAR(100) NOT NULL,
    fkcodpersona NVARCHAR(10) NOT NULL,
    CONSTRAINT pk_vendedor PRIMARY KEY (id),
    CONSTRAINT fk_vendedor_persona FOREIGN KEY (fkcodpersona) REFERENCES persona(codigo)
);

-- ------------------------------------------------------------
-- factura — EL ENCABEZADO de la venta. El «maestro» del maestro-detalle.
--
-- PARA QUE: quien compro, quien vendio, cuando, y cuanto en total.
--
-- `total DEFAULT 0` Y ESO NO ES UN ERROR: la factura NACE EN CERO. El total
-- lo calcula el disparador cada vez que entra o sale un renglon. Si la API
-- mandara el total, estaria mandando un numero que no calculo — y dos
-- fuentes para el mismo dato es una contradiccion esperando ocurrir.
--
-- `estado DEFAULT activa`: anular una factura NO la borra, le cambia el
-- estado. Es el borrado logico, y es lo que permite que la consulta de
-- anulaciones de la v4 tenga algo que contar. Lo borrado no se audita.
-- ------------------------------------------------------------
CREATE TABLE factura (
    numero INT IDENTITY(1,1) NOT NULL,
    fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
    total DECIMAL(18,2) NOT NULL DEFAULT 0,
    estado NVARCHAR(10) NOT NULL DEFAULT N'activa',
    fkidcliente INT NOT NULL,
    fkidvendedor INT NOT NULL,
    CONSTRAINT pk_factura PRIMARY KEY (numero),
    CONSTRAINT fk_factura_cliente FOREIGN KEY (fkidcliente) REFERENCES cliente(id),
    CONSTRAINT fk_factura_vendedor FOREIGN KEY (fkidvendedor) REFERENCES vendedor(id)
);

-- ------------------------------------------------------------
-- productosporfactura — EL DETALLE: los renglones de cada factura.
--
-- PARA QUE: que producto, cuantos, y por cuanto. Es el «detalle» del
-- maestro-detalle, y la tabla donde los disparadores hacen su trabajo.
--
-- LA CLAVE ES COMPUESTA (factura + producto), y eso decide una regla del
-- negocio sin una linea de codigo: UN PRODUCTO NO PUEDE APARECER DOS VECES
-- EN LA MISMA FACTURA. Si se pide mas, se cambia la cantidad del renglon que
-- ya existe. Intentar meterlo dos veces es el 409 que responde la API.
--
-- `subtotal DEFAULT 0`: igual que el total, lo calcula el disparador
-- (cantidad x valorunitario). La API no multiplica precios.
--
-- ON DELETE CASCADE: borrar la factura se lleva sus renglones. Es el unico
-- sitio del esquema donde la cascada tiene sentido — un renglon sin su
-- factura no significa nada.
-- ------------------------------------------------------------
CREATE TABLE productosporfactura (
    fknumfactura INT NOT NULL,
    fkcodproducto NVARCHAR(10) NOT NULL,
    cantidad INT NOT NULL,
    subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT pk_productosporfactura PRIMARY KEY (fknumfactura, fkcodproducto),
    CONSTRAINT fk_prodfact_factura FOREIGN KEY (fknumfactura) REFERENCES factura(numero) ON DELETE CASCADE,
    CONSTRAINT fk_prodfact_producto FOREIGN KEY (fkcodproducto) REFERENCES producto(codigo)
);

-- ------------------------------------------------------------
-- rol_usuario — TABLA PUENTE: que roles tiene cada usuario.
--
-- PARA QUE: un usuario puede tener varios roles, y un rol lo pueden tener
-- varios usuarios. Eso es «muchos a muchos», y no cabe en ninguna de las dos
-- tablas: necesita una tabla propia.
--
-- LA CLAVE ES LA PAREJA, y por eso esta tabla no tiene boton de editar en la
-- interfaz: una pareja existe o no existe. Se asigna o se retira. Asignar
-- dos veces lo mismo es el 409.
-- ------------------------------------------------------------
CREATE TABLE rol_usuario (
    fkemail NVARCHAR(100) NOT NULL,
    fkidrol INT NOT NULL,
    CONSTRAINT pk_rol_usuario PRIMARY KEY (fkemail, fkidrol),
    CONSTRAINT fk_rolusuario_usuario FOREIGN KEY (fkemail) REFERENCES usuario(email),
    CONSTRAINT fk_rolusuario_rol FOREIGN KEY (fkidrol) REFERENCES rol(id)
);

-- ------------------------------------------------------------
-- rutarol — TABLA PUENTE: a que interfaces entra cada rol.
--
-- ES LA TABLA DE PERMISOS DEL SISTEMA. Aqui vive la respuesta que el
-- procedimiento `verificar_acceso_ruta` viene a buscar en CADA peticion:
--
--     usuario -> rol_usuario -> rutarol -> (la ruta permitida)
--
-- QUITAR UNA FILA DE AQUI LE QUITA EL PERMISO AL ROL DE INMEDIATO, sin que
-- nadie vuelva a identificarse — porque el permiso no esta en el token, se
-- consulta cada vez. Es el criterio 7 de la version 3, y se comprueba
-- borrando una fila de esta tabla.
--
-- ON DELETE CASCADE en las dos claves: si se borra una interfaz o un rol,
-- sus permisos se van con el. Un permiso que apunta a una ruta que ya no
-- existe no es un permiso: es basura que confunde.
-- ------------------------------------------------------------
CREATE TABLE rutarol (
    fkidruta INT NOT NULL,
    fkidrol INT NOT NULL,
    CONSTRAINT pk_rutarol PRIMARY KEY (fkidruta, fkidrol),
    CONSTRAINT fk_rutarol_ruta FOREIGN KEY (fkidruta) REFERENCES ruta(id) ON DELETE CASCADE,
    CONSTRAINT fk_rutarol_rol FOREIGN KEY (fkidrol) REFERENCES rol(id) ON DELETE CASCADE
);
GO

-- ============================================================
-- TRIGGERS: Actualizar totales de factura y stock de producto
-- SQL Server requiere triggers AFTER separados por operación.
-- Se usan las tablas virtuales INSERTED y DELETED.
-- ============================================================

-- ------------------------------------------------------------
-- TRIGGER INSERT — cuando ENTRA un renglon a una factura.
--
-- QUE ES UN DISPARADOR: codigo que el motor ejecuta SOLO, sin que nadie lo
-- llame, cuando alguien toca una tabla. No se invoca: se dispara. Por eso la
-- regla vale igual para la API, para SSMS y para quien llegue manana.
--
-- `AFTER INSERT` = corre DESPUES de que la fila entro. En SQL Server no hay
-- un BEFORE: si hay que corregir la fila, se corrige con un UPDATE (ver
-- abajo). PostgreSQL si tiene BEFORE, y por eso alla el mismo trabajo se
-- escribe distinto.
--
-- `inserted` ES UNA TABLA VIRTUAL con las filas que acaban de entrar. No es
-- una variable: es una tabla, puede traer VARIAS filas, y por eso todo aqui
-- esta escrito con JOIN contra ella en vez de con variables sueltas. Un
-- disparador escrito con `SELECT @x = ...` solo atiende bien UNA fila, y el
-- dia que alguien inserte tres de golpe, calcula mal dos.
-- ------------------------------------------------------------
CREATE TRIGGER trg_prodfact_insert
ON productosporfactura
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- PASO 1 — ¿HAY STOCK? Esta es la regla mas importante del sistema:
    -- «no se vende lo que no hay». Y esta aqui, no en C#.
    --
    -- El IF EXISTS pregunta si EXISTE AL MENOS UN renglon cuyo producto tenga
    -- menos stock del pedido. No cuenta: solo averigua si hay alguno, que es
    -- mas barato — al primero que encuentra, deja de buscar.
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN producto p ON p.codigo = i.fkcodproducto
        WHERE p.stock < i.cantidad
    )
    BEGIN
        -- Hay faltante. Ahora SI hay que saber cual, para poder decirlo: se
        -- toma UNO de los que no alcanzan (TOP 1) y se guardan sus datos en
        -- variables para armar el mensaje.
        --
        -- POR QUE TOP 1 Y NO TODOS: porque la operacion se va a deshacer
        -- completa de todos modos. Nombrar el primero que falla es suficiente
        -- para que quien pidio sepa que corregir.
        DECLARE @v_codigo_err NVARCHAR(10), @v_stock_err INT, @v_cantidad_err INT;
        SELECT TOP 1 @v_codigo_err = i.fkcodproducto, @v_stock_err = p.stock, @v_cantidad_err = i.cantidad
        FROM inserted i
        JOIN producto p ON p.codigo = i.fkcodproducto
        WHERE p.stock < i.cantidad;

        DECLARE @v_msg_err NVARCHAR(500);
        SET @v_msg_err = CONCAT(N'Stock insuficiente para producto ', @v_codigo_err,
            N'. Stock disponible: ', @v_stock_err, N', cantidad solicitada: ', @v_cantidad_err);

        -- THROW: LEVANTA UN ERROR Y SE DETIENE TODO. Los tres argumentos son
        -- (numero, mensaje, estado). El numero propio tiene que ser >= 50000:
        -- los de abajo son del motor.
        --
        -- Y ESTO ES LO QUE HAY QUE SABER PARA SUSTENTARLO: este THROW deshace
        -- la transaccion ENTERA que abrio el procedimiento — la factura y los
        -- renglones que ya habian entrado—, y el mensaje llega INTACTO hasta
        -- la respuesta HTTP. Medido contra la API en marcha:
        --
        --   POST /api/factura con cantidad 9999  ->  HTTP 500
        --   {"estado":500,"mensaje":"Error interno.","detalle":
        --    "Stock insuficiente para producto PR001. Stock disponible: 16,
        --     cantidad solicitada: 9999"}
        --
        -- Que salga 500 y no 400 es una decision tomada y escrita en
        -- docs/dominio/POLITICA_DE_ERRORES.md: el disparador es la ultima
        -- defensa, y si se llego hasta el es porque la validacion de arriba
        -- no vio venir el problema.
        THROW 50001, @v_msg_err, 1;
    END

    -- PASO 2 — EL SUBTOTAL. cantidad x valorunitario.
    --
    -- Fijese que es un UPDATE sobre la fila que ACABA de entrar: como el
    -- disparador es AFTER, la fila ya esta escrita (con subtotal 0) y hay que
    -- corregirla. En PostgreSQL, que si tiene BEFORE, esto es una asignacion
    -- (`NEW.subtotal := ...`) y se ahorra la segunda escritura.
    --
    -- EL PRECIO SE LEE DE `producto`, NO LLEGA EN LA PETICION. Si el precio
    -- viniera de afuera, cualquiera podria facturarse un portatil en $1000.
    UPDATE pf
    SET pf.subtotal = i.cantidad * p.valorunitario
    FROM productosporfactura pf
    JOIN inserted i ON pf.fknumfactura = i.fknumfactura AND pf.fkcodproducto = i.fkcodproducto
    JOIN producto p ON p.codigo = i.fkcodproducto;

    -- PASO 3 — BAJAR EL STOCK. Lo vendido sale de la bodega.
    --
    -- AQUI ESTA EL ERROR MAS CARO QUE SE PUEDE COMETER EN ESTE PROYECTO: si
    -- el codigo C# TAMBIEN descuenta el stock, el stock baja DOS veces por
    -- cada venta. Y no falla nada — simplemente las cifras dejan de cuadrar
    -- y nadie sabe desde cuando. El stock lo mueve ESTE disparador, punto.
    UPDATE p
    SET p.stock = p.stock - i.cantidad
    FROM producto p
    JOIN inserted i ON p.codigo = i.fkcodproducto;

    -- PASO 4 — EL TOTAL DE LA FACTURA. Y se RECALCULA, no se acumula.
    --
    -- Lo importante es que NO dice `total = total + subtotal`. Vuelve a sumar
    -- TODOS los renglones de esa factura desde cero:
    --
    --   acumular  ->  si el disparador corre dos veces por un error, suma dos
    --                 veces, y el numero malo se queda para siempre
    --   recalcular -> corra una vez o diez, el resultado es el mismo
    --
    -- Eso se llama ser IDEMPOTENTE, y es la razon de que la consulta interna
    -- (la que va entre parentesis y se llama `sub`) exista: agrupa por factura
    -- y trae la suma de cada una.
    --
    -- `IN (SELECT DISTINCT fknumfactura FROM inserted)`: solo se recalculan
    -- las facturas tocadas. Si entraron tres renglones de la misma factura,
    -- DISTINCT evita recalcularla tres veces.
    --
    -- ISNULL(sub.suma, 0): si no quedo ningun renglon, SUM devuelve NULL — y
    -- una factura en NULL no es una factura en cero. Se obliga a cero.
    UPDATE f
    SET f.total = ISNULL(sub.suma, 0)
    FROM factura f
    JOIN (
        SELECT pf.fknumfactura, SUM(pf.subtotal) AS suma
        FROM productosporfactura pf
        WHERE pf.fknumfactura IN (SELECT DISTINCT fknumfactura FROM inserted)
        GROUP BY pf.fknumfactura
    ) sub ON f.numero = sub.fknumfactura;
END;
GO

-- TRIGGER UPDATE
-- ------------------------------------------------------------
-- TRIGGER UPDATE — cuando CAMBIA la cantidad de un renglon.
--
-- AQUI HAY DOS TABLAS VIRTUALES, Y ESA ES TODA LA DIFICULTAD:
--
--     `inserted` = como queda la fila   (lo nuevo)
--     `deleted`  = como estaba la fila  (lo viejo)
--
-- En un UPDATE existen las dos, y hay que usar LAS DOS. Un disparador de
-- UPDATE que solo mira `inserted` pierde la cuenta del stock, porque no sabe
-- cuanto habia reservado antes.
--
-- Se emparejan por la clave compuesta (factura + producto): asi cada fila
-- nueva se junta con su propia version vieja y no con la de otro renglon.
-- ------------------------------------------------------------
CREATE TRIGGER trg_prodfact_update
ON productosporfactura
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- ¿HAY STOCK? Pero ojo con la cuenta: NO se compara el stock contra la
    -- cantidad nueva. Se compara contra `stock + cantidad_vieja`.
    --
    -- POR QUE: las unidades viejas estaban reservadas por este mismo renglon,
    -- y al cambiarlo se devuelven. Si un renglon tenia 10 y se sube a 12, no
    -- hacen falta 12 unidades libres: hacen falta 2.
    --
    -- Olvidar el `+ d.cantidad` produce un «stock insuficiente» falso, y es de
    -- los errores que parecen del negocio y son de aritmetica.
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN deleted d ON i.fknumfactura = d.fknumfactura AND i.fkcodproducto = d.fkcodproducto
        JOIN producto p ON p.codigo = i.fkcodproducto
        WHERE p.stock + d.cantidad < i.cantidad
    )
    BEGIN
        DECLARE @v_codigo_err NVARCHAR(10), @v_stock_err INT, @v_cantidad_err INT;
        SELECT TOP 1 @v_codigo_err = i.fkcodproducto, @v_stock_err = p.stock + d.cantidad, @v_cantidad_err = i.cantidad
        FROM inserted i
        JOIN deleted d ON i.fknumfactura = d.fknumfactura AND i.fkcodproducto = d.fkcodproducto
        JOIN producto p ON p.codigo = i.fkcodproducto
        WHERE p.stock + d.cantidad < i.cantidad;

        DECLARE @v_msg_err NVARCHAR(500);
        SET @v_msg_err = CONCAT(N'Stock insuficiente para producto ', @v_codigo_err,
            N'. Stock disponible: ', @v_stock_err, N', cantidad solicitada: ', @v_cantidad_err);
        THROW 50001, @v_msg_err, 1;
    END

    -- Recalcular subtotal
    UPDATE pf
    SET pf.subtotal = i.cantidad * p.valorunitario
    FROM productosporfactura pf
    JOIN inserted i ON pf.fknumfactura = i.fknumfactura AND pf.fkcodproducto = i.fkcodproducto
    JOIN producto p ON p.codigo = i.fkcodproducto;

    -- AJUSTAR EL STOCK EN UN SOLO MOVIMIENTO: se devuelve lo viejo y se
    -- descuenta lo nuevo (`stock + d.cantidad - i.cantidad`).
    --
    -- Se hace en una sola sentencia a proposito. Devolver primero y descontar
    -- despues, en dos pasos, deja un instante con el stock inflado — y si algo
    -- falla en el medio, inflado se queda.
    UPDATE p
    SET p.stock = p.stock + d.cantidad - i.cantidad
    FROM producto p
    JOIN inserted i ON p.codigo = i.fkcodproducto
    JOIN deleted d ON d.fknumfactura = i.fknumfactura AND d.fkcodproducto = i.fkcodproducto;

    -- Recalcular total de la factura
    UPDATE f
    SET f.total = ISNULL(sub.suma, 0)
    FROM factura f
    JOIN (
        SELECT pf.fknumfactura, SUM(pf.subtotal) AS suma
        FROM productosporfactura pf
        WHERE pf.fknumfactura IN (SELECT DISTINCT fknumfactura FROM inserted)
        GROUP BY pf.fknumfactura
    ) sub ON f.numero = sub.fknumfactura;
END;
GO

-- ------------------------------------------------------------
-- TRIGGER DELETE — cuando se QUITA un renglon de una factura.
--
-- Aqui solo existe `deleted`: lo que habia. No hay `inserted`, porque no
-- queda fila nueva. Y el trabajo es el inverso del INSERT:
--
--     INSERT  ->  valida stock, calcula subtotal, BAJA stock,  recalcula total
--     DELETE  ->  (nada que validar),            SUBE stock,  recalcula total
--
-- NO VALIDA NADA, y es correcto: devolver mercancia a la bodega nunca puede
-- fallar por falta de espacio.
-- ------------------------------------------------------------
CREATE TRIGGER trg_prodfact_delete
ON productosporfactura
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- DEVOLVER EL STOCK. Lo que se quito de la factura vuelve a la bodega.
    --
    -- OJO CON QUIEN DISPARA ESTO, PORQUE NO ES ANULAR UNA FACTURA:
    -- `sp_anular_factura` NO borra renglones —los conserva para poder
    -- auditarlos— y devuelve el stock con un UPDATE propio. Este disparador
    -- no interviene ahi.
    --
    -- QUIEN LO DISPARA DE VERDAD:
    --   · borrar un renglon suelto
    --   · borrar la factura entera (la cascada se lleva los renglones)
    --   · y sobre todo `sp_actualizar_factura_y_productosporfactura`, que
    --     borra TODOS los renglones y los vuelve a insertar
    UPDATE p
    SET p.stock = p.stock + d.cantidad
    FROM producto p
    JOIN deleted d ON p.codigo = d.fkcodproducto;

    -- RECALCULAR EL TOTAL con los renglones que QUEDAN.
    --
    -- La subconsulta suma `productosporfactura` (la tabla real, ya sin la fila
    -- borrada), no `deleted`. Y el ISNULL de adentro es el que importa: si se
    -- borro el ULTIMO renglon, no queda nada que sumar y SUM devuelve NULL.
    -- Sin ese ISNULL la factura quedaria con total NULL en vez de 0.
    UPDATE f
    SET f.total = ISNULL(sub.suma, 0)
    FROM factura f
    JOIN (
        SELECT d.fknumfactura,
               ISNULL((SELECT SUM(pf.subtotal) FROM productosporfactura pf WHERE pf.fknumfactura = d.fknumfactura), 0) AS suma
        FROM (SELECT DISTINCT fknumfactura FROM deleted) d
    ) sub ON f.numero = sub.fknumfactura;
END;
GO

-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS - FACTURAS Y PRODUCTOS POR FACTURA
-- Los resultados se retornan via parámetro OUTPUT tipo NVARCHAR(MAX)
-- ============================================================

-- ------------------------------------------------------------
-- 1. SP INSERTAR FACTURA Y PRODUCTOSPORFACTURA
-- Recibe: id cliente, id vendedor, y un JSON array de productos
-- Retorna: JSON con la factura creada y sus productos
-- Nota: El trigger se encarga de calcular subtotal, descontar
--       stock y actualizar total factura.
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "sp_insertar_factura_y_productosporfactura",
--     "p_fkidcliente": 1, "p_fkidvendedor": 1,
--     "p_productos": "[{\"codigo\":\"PR001\",\"cantidad\":2},{\"codigo\":\"PR003\",\"cantidad\":3}]",
--     "p_resultado": null }
-- ------------------------------------------------------------
-- ESTE ES EL PROCEDIMIENTO QUE HAY QUE ENTENDER. Es el unico que escribe en
-- DOS tablas a la vez (el maestro y su detalle), el unico que abre una
-- transaccion a mano, y el unico que recibe una lista. Si se entiende este,
-- los otros quince son variaciones mas simples.
--
-- LO QUE HACE, EN UNA LINEA: crea el encabezado de la factura, le mete sus
-- renglones uno por uno, y devuelve la factura completa en JSON — o no deja
-- nada, si algo falla.
CREATE PROCEDURE sp_insertar_factura_y_productosporfactura
    -- EL MAESTRO: a quien se le factura y quien vende.
    @p_fkidcliente INT,
    @p_fkidvendedor INT,

    -- EL DETALLE, QUE LLEGA COMO TEXTO JSON:
    --     '[{"codigo":"PR001","cantidad":2},{"codigo":"PR003","cantidad":3}]'
    --
    -- ¿Por que texto y no una tabla? Porque un procedimiento recibe tipos
    -- simples: numeros, textos, fechas. Para recibir UNA TABLA habria que
    -- declarar antes un tipo de tabla en el motor, y entonces el
    -- procedimiento solo serviria para quien conozca ese tipo. El JSON viaja
    -- como texto —que cualquier cliente sabe mandar— y se abre aqui.
    @p_productos NVARCHAR(MAX),

    -- Cuantos renglones exige el negocio como minimo. Lleva `= 1`, o sea
    -- DEFAULT: si quien llama no lo manda, vale 1. Asi se puede endurecer la
    -- regla sin cambiar a quienes ya llaman al procedimiento.
    @p_minimo_detalle INT = 1,

    -- LA SALIDA, TAMBIEN JSON, POR UN PARAMETRO `OUTPUT`.
    --
    -- Fijese que NO termina con un SELECT que devuelva filas. Devuelve UN
    -- texto por este parametro, y la API lo deserializa. Por que:
    --   · el contrato es UN valor, no «cuantos result sets haya»
    --   · un JSON puede traer la factura Y sus renglones anidados; un
    --     conjunto de filas plano, no
    --   · si manana se agrega un campo, el cliente viejo lo ignora y sigue
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    -- SET NOCOUNT ON: apaga los mensajes de «(1 row affected)». No es
    -- cosmetica: esos avisos viajan por la red en cada sentencia y algunos
    -- clientes los confunden con resultados.
    SET NOCOUNT ON;

    -- LAS VARIABLES LOCALES. En T-SQL se declaran antes de usarlas, y la
    -- costumbre es ponerlas todas arriba para poder leer de un vistazo con
    -- que trabaja el procedimiento. El prefijo @v_ es de «variable», para no
    -- confundirlas con los parametros @p_.
    DECLARE @v_numero INT;          -- el numero de factura que asigne el motor
    DECLARE @v_codigo NVARCHAR(10); -- el producto del renglon que se este leyendo
    DECLARE @v_cantidad INT;        -- su cantidad
    DECLARE @v_minimo INT;          -- el minimo de renglones ya resuelto
    DECLARE @v_count INT;           -- cuantos renglones trajo el JSON
    DECLARE @v_msg NVARCHAR(500);   -- el mensaje de error, si toca

    -- ============================================================
    -- PRIMERO LO QUE SE PUEDE RECHAZAR SIN TOCAR NADA
    --
    -- Y ESO ES UNA DECISION, NO UN ORDEN CASUAL: validar ANTES de abrir la
    -- transaccion. Abrir una transaccion toma recursos y bloquea filas; si la
    -- peticion viene mal de entrada, no hay por que abrirla para cerrarla.
    -- ============================================================

    -- COALESCE(NULLIF(@p_minimo_detalle, 0), 1) — tres pasos en una linea:
    --   NULLIF(x, 0)  -> si x es 0, devuelve NULL; si no, devuelve x
    --   COALESCE(a,b) -> el primero de los dos que no sea NULL
    --
    -- O SEA: «si me mandaron 0, usa 1». Y hace falta porque la API manda 0
    -- cuando el parametro no viene — y un minimo de 0 renglones significaria
    -- aceptar facturas vacias, que es justo lo que esto evita.
    SET @v_minimo = COALESCE(NULLIF(@p_minimo_detalle, 0), 1);

    IF @p_productos IS NULL
    BEGIN
        SET @v_msg = CONCAT(N'La factura requiere minimo ', @v_minimo, N' producto(s).');
        THROW 50002, @v_msg, 1;
    END

    -- CUANTOS RENGLONES TRAE EL JSON. `OPENJSON` convierte el texto en una
    -- tabla —una fila por elemento del arreglo—, y sobre una tabla ya se
    -- puede hacer COUNT(*). Es la primera de las dos veces que aparece.
    SELECT @v_count = COUNT(*) FROM OPENJSON(@p_productos);
    IF @v_count < @v_minimo
    BEGIN
        SET @v_msg = CONCAT(N'La factura requiere minimo ', @v_minimo, N' producto(s).');

        -- OJO CON ESTE THROW, PORQUE ES EL QUE SE MALINTERPRETA:
        --
        -- Por la API, una factura sin renglones NO llega hasta aqui — el
        -- controlador la rechaza antes con **422** (medido: POST /api/factura
        -- con "productos":[] responde 422). Entonces, ¿para que sirve?
        --
        -- PARA QUIEN NO PASE POR LA API. Quien ejecute el procedimiento desde
        -- SSMS tambien tiene que chocar con la regla. Es la misma idea del
        -- disparador del stock: la regla vive donde estan los datos, y la
        -- validacion de la API es comodidad, no la defensa.
        THROW 50002, @v_msg, 1;
    END

    -- ── TRANSACCION: todo o nada ──
    -- Si un trigger falla (ej: stock insuficiente en el 2do producto),
    -- se revierte la factura y todos los productos insertados previamente.
    BEGIN TRY
        BEGIN TRANSACTION;

        -- PASO 1 — EL ENCABEZADO, CON TOTAL 0.
        --
        -- No es un descuido mandar 0: el total lo calculan los disparadores
        -- cuando entren los renglones. Mandar aqui un total seria mandar un
        -- numero que este procedimiento no calculo.
        INSERT INTO factura (fkidcliente, fkidvendedor, total)
        VALUES (@p_fkidcliente, @p_fkidvendedor, 0);

        -- PASO 2 — ¿QUE NUMERO LE TOCO A ESA FACTURA?
        --
        -- `numero` es IDENTITY: lo asigna el motor, y hasta que la fila no
        -- entra nadie sabe cual fue. Los renglones necesitan ese numero para
        -- saber de quien son, asi que hay que preguntarlo.
        --
        -- POR QUE `SCOPE_IDENTITY()` Y NO `@@IDENTITY`. Y AQUI NO ES TEORIA:
        --
        --   SCOPE_IDENTITY() -> el ultimo id generado EN ESTE AMBITO, o sea
        --                       por este procedimiento
        --   @@IDENTITY       -> el ultimo id generado EN LA SESION, SIN
        --                       IMPORTAR QUIEN LO GENERO
        --
        -- ESTA TABLA TIENE DISPARADORES. Si un disparador insertara en otra
        -- tabla con IDENTITY —una auditoria, por ejemplo—, `@@IDENTITY`
        -- devolveria el id de ESA fila, no el de la factura. Y entonces los
        -- renglones se colgarian de una factura que no existe.
        --
        -- Es el error clasico de este patron: funciona hoy, y el dia que
        -- alguien agrega un disparador de bitacora se rompe sin tocar este
        -- archivo. `IDENT_CURRENT('factura')` tiene el problema contrario:
        -- devuelve el ultimo de la TABLA, incluso si lo inserto otro usuario.
        SET @v_numero = SCOPE_IDENTITY();

        -- ============================================================
        -- PASO 3 — LOS RENGLONES, UNO POR UNO. Esta es la parte larga, y lo
        -- es porque SQL no tiene un `for` como C#: para recorrer filas de a
        -- una se usa un CURSOR.
        --
        -- QUE ES UN CURSOR: un apuntador que se para en la primera fila de un
        -- resultado y se va moviendo. Cuatro tiempos, siempre los mismos:
        --
        --     DECLARE  -> se define QUE consulta va a recorrer
        --     OPEN     -> se ejecuta la consulta y el apuntador se posiciona
        --     FETCH    -> se trae la fila donde esta parado y se avanza
        --     CLOSE    -> se suelta
        --
        -- `LOCAL`        = el cursor muere con este procedimiento. Sin esto
        --                  queda vivo en la sesion, y el proximo que declare
        --                  uno con el mismo nombre choca.
        -- `FAST_FORWARD` = solo lectura y solo hacia adelante. Es el mas
        --                  barato que hay: no permite retroceder ni editar, y
        --                  aqui no hace falta ninguna de las dos cosas.
        -- ============================================================
        DECLARE producto_cursor CURSOR LOCAL FAST_FORWARD FOR
            -- LA CONSULTA QUE EL CURSOR VA A RECORRER: el JSON convertido en
            -- tabla. Dos funciones hacen el trabajo:
            --
            --   OPENJSON(texto)  -> una FILA por elemento del arreglo. Para un
            --     arreglo, cada fila trae las columnas `key` (la posicion),
            --     `value` (el elemento completo, todavia como texto JSON) y
            --     `type`. Aqui solo interesa `value`.
            --
            --   JSON_VALUE(value, '$.codigo') -> saca UN campo de ese texto.
            --     El `$` es la raiz del objeto y `.codigo` la propiedad, o sea
            --     «de este elemento, dame codigo».
            --
            -- Y EL `CAST(... AS INT)` NO ES OPCIONAL: JSON_VALUE SIEMPRE
            -- devuelve texto, aunque en el JSON el numero vaya sin comillas.
            -- Sin el CAST se estaria insertando el texto '2' en una columna
            -- INT: a veces el motor lo convierte solo y a veces falla, que es
            -- la peor de las dos.
            SELECT
                JSON_VALUE(value, '$.codigo'),
                CAST(JSON_VALUE(value, '$.cantidad') AS INT)
            FROM OPENJSON(@p_productos);

        OPEN producto_cursor;

        -- EL PRIMER FETCH VA ANTES DEL BUCLE, y sorprende a todo el mundo.
        -- Es porque `@@FETCH_STATUS` solo tiene valor DESPUES de un FETCH:
        -- preguntar antes del primero seria preguntar por el resultado de
        -- algo que no ha pasado.
        --
        -- `INTO @v_codigo, @v_cantidad` reparte las dos columnas de la fila en
        -- las dos variables, EN ESE ORDEN. Si se invierten, el procedimiento
        -- compila igual y empieza a guardar cantidades en el codigo.
        FETCH NEXT FROM producto_cursor INTO @v_codigo, @v_cantidad;

        -- @@FETCH_STATUS = 0 significa «el ultimo FETCH SI trajo una fila».
        -- Cuando se acaban, pasa a -1 y el bucle termina. Es el equivalente
        -- de preguntar «¿hay siguiente?» en un recorrido de C#.
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- EL RENGLON, CON SUBTOTAL 0 — igual que el total de la factura:
            -- lo calcula el disparador. Y cada uno de estos INSERT dispara
            -- `trg_prodfact_insert`, que valida el stock, calcula el subtotal,
            -- descuenta la bodega y recalcula el total.
            --
            -- POR ESO ESTE BUCLE ES LA PIEZA CLAVE DEL PROYECTO: en estas dos
            -- lineas se ve el maestro-detalle completo. El renglon sabe de que
            -- factura es (@v_numero) y la regla de negocio no esta aqui.
            INSERT INTO productosporfactura (fknumfactura, fkcodproducto, cantidad, subtotal)
            VALUES (@v_numero, @v_codigo, @v_cantidad, 0);

            -- Y EL SEGUNDO FETCH, AL FINAL DEL BUCLE. Si se olvida, el
            -- apuntador no avanza, @@FETCH_STATUS se queda en 0 y el
            -- procedimiento inserta el mismo renglon para siempre. Es el
            -- bucle infinito clasico de los cursores.
            FETCH NEXT FROM producto_cursor INTO @v_codigo, @v_cantidad;
        END

        -- CERRAR Y LIBERAR. CLOSE suelta las filas; DEALLOCATE borra la
        -- definicion del cursor. Son dos cosas distintas: un cursor cerrado
        -- se puede volver a abrir, uno liberado ya no existe.
        CLOSE producto_cursor;
        DEALLOCATE producto_cursor;

        -- Retornar resultado como JSON
        -- ============================================================
        -- PASO 4 — DEVOLVER LA FACTURA RECIEN HECHA, EN JSON.
        --
        -- Y se vuelve a LEER de la base en vez de armarla con lo que se
        -- recibio. Es a proposito: el total y los subtotales los puso el
        -- disparador, no este procedimiento. Devolver lo que se mando seria
        -- devolver ceros.
        -- ============================================================
        DECLARE @v_factura_json NVARCHAR(MAX);
        DECLARE @v_productos_json NVARCHAR(MAX);

        -- EL ENCABEZADO, COMO UN OBJETO SUELTO.
        --
        -- `FOR JSON PATH` convierte el resultado del SELECT en JSON: cada
        -- columna se vuelve una propiedad y cada fila un objeto.
        --
        -- Y `WITHOUT_ARRAY_WRAPPER` SE LEE LITERAL: «sin la envoltura de
        -- arreglo». Sin el, esto devolveria [ { ... } ] — con corchetes —
        -- porque SQL Server piensa en conjuntos y envuelve el resultado
        -- aunque traiga UNA sola fila. La factura es una, y con corchetes el
        -- cliente tendria que escribir resultado[0] para siempre.
        SELECT @v_factura_json = (
            SELECT f.numero, f.fecha, f.total, f.estado, f.fkidcliente, f.fkidvendedor
            FROM factura f WHERE f.numero = @v_numero
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        -- Y LOS RENGLONES, ESTE SI COMO ARREGLO: `FOR JSON PATH` a secas
        -- devuelve `[ {...}, {...} ]`, que es lo que se quiere para una lista.
        --
        -- LA DIFERENCIA CON EL DE ARRIBA ES ESA UNICA PALABRA:
        --
        --     FOR JSON PATH                          ->  [ { ... } ]
        --     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER   ->    { ... }
        --
        -- Se lee LITERAL: «sin la envoltura de arreglo». SQL Server siempre
        -- piensa en conjuntos, asi que por defecto envuelve el resultado en
        -- corchetes aunque traiga una sola fila. Para la factura —que es UNA—
        -- esos corchetes obligarian al cliente a escribir `resultado[0]`, y
        -- entonces el contrato diria «un arreglo» cuando siempre hay uno.
        --
        -- Los `AS` tambien cuentan: el alias es el NOMBRE DE LA PROPIEDAD en
        -- el JSON. `pf.fkcodproducto AS codigo_producto` sale como
        -- "codigo_producto", no como "fkcodproducto" — el nombre interno de la
        -- columna no se le filtra al cliente.
        SELECT @v_productos_json = (
            SELECT pf.fkcodproducto AS codigo_producto, pr.nombre AS nombre_producto,
                   pf.cantidad, pr.valorunitario, pf.subtotal
            FROM productosporfactura pf
            JOIN producto pr ON pr.codigo = pf.fkcodproducto
            WHERE pf.fknumfactura = @v_numero
            FOR JSON PATH
        );

        -- SE ARMA EL SOBRE FINAL pegando los dos pedazos:
        --     {"factura": {...}, "productos": [...]}
        --
        -- ISNULL(@v_productos_json, N'[]') — y aqui hay una trampa real: si
        -- no hubiera renglones, `FOR JSON` devuelve NULL, no '[]'. Y en T-SQL
        -- concatenar texto con NULL da NULL: el JSON ENTERO se volveria NULL
        -- y la API recibiria nada, sin un solo error. Ese ISNULL es lo unico
        -- que lo evita.
        SET @p_resultado = N'{"factura":' + @v_factura_json + N',"productos":' + ISNULL(@v_productos_json, N'[]') + N'}';

        COMMIT TRANSACTION;
    END TRY
    -- ============================================================
    -- SI ALGO FALLO: DESHACER, LIMPIAR Y CONTARLO. En ese orden.
    -- ============================================================
    BEGIN CATCH
        -- @@TRANCOUNT dice cuantas transacciones hay abiertas. Se pregunta
        -- antes de deshacer porque si el error ya la cerro, un ROLLBACK sin
        -- transaccion abierta es OTRO error — y entonces el mensaje que llega
        -- es el del rollback y no el del problema de verdad.
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- EL CURSOR NO LO DESHACE EL ROLLBACK. Si el error salto a mitad del
        -- bucle, el cursor quedo abierto: las filas siguen reservadas y el
        -- nombre sigue tomado en la sesion. CURSOR_STATUS >= 0 significa que
        -- existe, y entonces hay que cerrarlo a mano.
        --
        -- Es la fuga mas comun de este patron, y no se nota en una prueba:
        -- se nota en el segundo intento, que falla por un nombre ocupado.
        IF CURSOR_STATUS('local', 'producto_cursor') >= 0
        BEGIN
            CLOSE producto_cursor;
            DEALLOCATE producto_cursor;
        END;

        -- `THROW` SIN ARGUMENTOS RELANZA EL ERROR ORIGINAL, con su numero y
        -- su mensaje intactos. Es lo que hace que el «Stock insuficiente para
        -- producto PR001...» que escribio el disparador llegue hasta la
        -- respuesta HTTP.
        --
        -- Si en vez de esto se hiciera `THROW 50000, 'Error', 1`, se perderia
        -- la unica informacion util. Y tragarse el error —un CATCH vacio— es
        -- peor todavia: el procedimiento terminaria «bien» sin haber hecho
        -- nada, y quien llamo creeria que su factura existe.
        THROW;
    END CATCH
END;
GO

-- ------------------------------------------------------------
-- 2. SP CONSULTAR FACTURA Y PRODUCTOSPORFACTURA
-- Consulta una factura por número con detalle de productos,
-- nombre del cliente y nombre del vendedor
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "sp_consultar_factura_y_productosporfactura",
--     "p_numero": 1, "p_resultado": "" }
-- ------------------------------------------------------------
CREATE PROCEDURE sp_consultar_factura_y_productosporfactura
    @p_numero INT,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM factura WHERE numero = @p_numero)
    BEGIN
        DECLARE @v_msg NVARCHAR(500);
        SET @v_msg = CONCAT(N'Factura ', @p_numero, N' no existe');
        THROW 50003, @v_msg, 1;
    END

    DECLARE @v_factura_json NVARCHAR(MAX);
    DECLARE @v_productos_json NVARCHAR(MAX);

    SELECT @v_factura_json = (
        SELECT f.numero, f.fecha, f.total, f.estado, f.fkidcliente,
               pc.nombre AS nombre_cliente,
               f.fkidvendedor,
               pv.nombre AS nombre_vendedor
        FROM factura f
        JOIN cliente c ON c.id = f.fkidcliente
        JOIN persona pc ON pc.codigo = c.fkcodpersona
        JOIN vendedor v ON v.id = f.fkidvendedor
        JOIN persona pv ON pv.codigo = v.fkcodpersona
        WHERE f.numero = @p_numero
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    SELECT @v_productos_json = (
        SELECT pr.codigo AS codigo_producto, pr.nombre AS nombre_producto,
               pf.cantidad, pr.valorunitario, pf.subtotal
        FROM productosporfactura pf
        JOIN producto pr ON pr.codigo = pf.fkcodproducto
        WHERE pf.fknumfactura = @p_numero
        FOR JSON PATH
    );

    SET @p_resultado = N'{"factura":' + @v_factura_json + N',"productos":' + ISNULL(@v_productos_json, N'[]') + N'}';
END;
GO

-- ------------------------------------------------------------
-- 3. SP LISTAR FACTURAS Y PRODUCTOSPORFACTURA
-- Lista todas las facturas con sus productos, cliente y vendedor
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "sp_listar_facturas_y_productosporfactura",
--     "p_resultado": "" }
-- ------------------------------------------------------------
CREATE PROCEDURE sp_listar_facturas_y_productosporfactura
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_result NVARCHAR(MAX) = N'[';
    DECLARE @v_numero INT;
    DECLARE @v_factura_json NVARCHAR(MAX);
    DECLARE @v_productos_json NVARCHAR(MAX);
    DECLARE @v_first BIT = 1;

    DECLARE factura_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT f.numero
        FROM factura f
        ORDER BY f.numero;

    OPEN factura_cursor;
    FETCH NEXT FROM factura_cursor INTO @v_numero;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @v_first = 0
            SET @v_result = @v_result + N',';
        SET @v_first = 0;

        SELECT @v_factura_json = (
            SELECT f.numero, f.fecha, f.total, f.fkidcliente,
                   pc.nombre AS nombre_cliente,
                   f.fkidvendedor,
                   pv.nombre AS nombre_vendedor
            FROM factura f
            JOIN cliente c ON c.id = f.fkidcliente
            JOIN persona pc ON pc.codigo = c.fkcodpersona
            JOIN vendedor v ON v.id = f.fkidvendedor
            JOIN persona pv ON pv.codigo = v.fkcodpersona
            WHERE f.numero = @v_numero
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        SELECT @v_productos_json = (
            SELECT pr.codigo AS codigo_producto, pr.nombre AS nombre_producto,
                   pf.cantidad, pr.valorunitario, pf.subtotal
            FROM productosporfactura pf
            JOIN producto pr ON pr.codigo = pf.fkcodproducto
            WHERE pf.fknumfactura = @v_numero
            FOR JSON PATH
        );

        SET @v_result = @v_result + N'{' +
            N'"numero":' + CAST(@v_numero AS NVARCHAR) + N',' +
            N'"fecha":"' + CONVERT(NVARCHAR(30), (SELECT fecha FROM factura WHERE numero = @v_numero), 126) + N'",' +
            N'"total":' + CAST((SELECT total FROM factura WHERE numero = @v_numero) AS NVARCHAR) + N',' +
            N'"estado":"' + (SELECT estado FROM factura WHERE numero = @v_numero) + N'",' +
            N'"fkidcliente":' + CAST((SELECT fkidcliente FROM factura WHERE numero = @v_numero) AS NVARCHAR) + N',' +
            N'"nombre_cliente":"' + (SELECT pc.nombre FROM factura f JOIN cliente c ON c.id = f.fkidcliente JOIN persona pc ON pc.codigo = c.fkcodpersona WHERE f.numero = @v_numero) + N'",' +
            N'"fkidvendedor":' + CAST((SELECT fkidvendedor FROM factura WHERE numero = @v_numero) AS NVARCHAR) + N',' +
            N'"nombre_vendedor":"' + (SELECT pv.nombre FROM factura f JOIN vendedor v ON v.id = f.fkidvendedor JOIN persona pv ON pv.codigo = v.fkcodpersona WHERE f.numero = @v_numero) + N'",' +
            N'"productos":' + ISNULL(@v_productos_json, N'[]') +
            N'}';

        FETCH NEXT FROM factura_cursor INTO @v_numero;
    END

    CLOSE factura_cursor;
    DEALLOCATE factura_cursor;

    SET @v_result = @v_result + N']';

    -- Si no hay facturas, retornar array vacio
    IF @v_first = 1
        SET @v_result = N'[]';

    SET @p_resultado = @v_result;
END;
GO

-- ------------------------------------------------------------
-- 4. SP ACTUALIZAR FACTURA Y PRODUCTOSPORFACTURA
-- Reemplaza los productos de una factura existente.
-- Nota: El trigger se encarga de restaurar stock (DELETE),
--       descontar stock (INSERT) y recalcular subtotales/total.
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "sp_actualizar_factura_y_productosporfactura",
--     "p_numero": 1, "p_fkidcliente": 2, "p_fkidvendedor": 1,
--     "p_productos": "[{\"codigo\":\"PR002\",\"cantidad\":1},{\"codigo\":\"PR004\",\"cantidad\":5}]",
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE sp_actualizar_factura_y_productosporfactura
    @p_numero INT,
    @p_fkidcliente INT,
    @p_fkidvendedor INT,
    @p_productos NVARCHAR(MAX),
    @p_minimo_detalle INT = 1,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_codigo NVARCHAR(10);
    DECLARE @v_cantidad INT;
    DECLARE @v_minimo INT;
    DECLARE @v_count INT;
    DECLARE @v_msg NVARCHAR(500);

    -- Validaciones antes de abrir transaccion
    IF NOT EXISTS (SELECT 1 FROM factura WHERE numero = @p_numero)
    BEGIN
        SET @v_msg = CONCAT(N'Factura ', @p_numero, N' no existe');
        THROW 50004, @v_msg, 1;
    END

    SET @v_minimo = COALESCE(NULLIF(@p_minimo_detalle, 0), 1);

    IF @p_productos IS NULL
    BEGIN
        SET @v_msg = CONCAT(N'La factura requiere minimo ', @v_minimo, N' producto(s).');
        THROW 50004, @v_msg, 1;
    END

    SELECT @v_count = COUNT(*) FROM OPENJSON(@p_productos);
    IF @v_count < @v_minimo
    BEGIN
        SET @v_msg = CONCAT(N'La factura requiere minimo ', @v_minimo, N' producto(s).');
        THROW 50004, @v_msg, 1;
    END

    -- ── TRANSACCION: todo o nada ──
    -- Si un trigger falla al insertar un nuevo producto (ej: stock insuficiente),
    -- se revierten todos los cambios: el DELETE previo, los INSERTs parciales y el UPDATE.
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Eliminar detalle anterior (el trigger restaura stock y recalcula total)
        -- ============================================================
        -- LA DECISION DE ESTE PROCEDIMIENTO: BORRAR TODO EL DETALLE Y
        -- VOLVERLO A INSERTAR. No se comparan renglon por renglon para ver
        -- cual cambio.
        --
        -- POR QUE, Y QUE CUESTA:
        --
        --   · comparar exigiria averiguar que renglon se agrego, cual se
        --     quito y cual cambio de cantidad — tres caminos distintos y
        --     tres formas de equivocarse
        --   · borrar y reinsertar es UN camino, y el estado final es
        --     exactamente el que mando quien llamo
        --
        -- Y LO QUE PASA POR DEBAJO, QUE ES LO BONITO DE VERLO:
        --
        --   1. este DELETE dispara `trg_prodfact_delete` una vez, que
        --      DEVUELVE a la bodega el stock de todos los renglones
        --   2. cada INSERT de abajo dispara `trg_prodfact_insert`, que
        --      vuelve a validar el stock y a descontarlo
        --
        -- O sea que la validacion de «no se vende lo que no hay» se aplica
        -- otra vez, con el stock ya devuelto. Nadie escribio codigo para eso:
        -- sale gratis de haber puesto la regla en los disparadores.
        --
        -- EL EFECTO SECUNDARIO QUE HAY QUE CONOCER: `trg_prodfact_update`
        -- casi nunca se dispara desde la API, porque la API no actualiza
        -- renglones — los borra y los vuelve a crear. Ese disparador esta
        -- ahi para quien entre por SSMS y haga un UPDATE a mano.
        -- ============================================================
        DELETE FROM productosporfactura WHERE fknumfactura = @p_numero;

        -- Insertar nuevos productos (el trigger calcula subtotal, descuenta stock, actualiza total)
        DECLARE producto_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT
                JSON_VALUE(value, '$.codigo'),
                CAST(JSON_VALUE(value, '$.cantidad') AS INT)
            FROM OPENJSON(@p_productos);

        OPEN producto_cursor;
        FETCH NEXT FROM producto_cursor INTO @v_codigo, @v_cantidad;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO productosporfactura (fknumfactura, fkcodproducto, cantidad, subtotal)
            VALUES (@p_numero, @v_codigo, @v_cantidad, 0);

            FETCH NEXT FROM producto_cursor INTO @v_codigo, @v_cantidad;
        END

        CLOSE producto_cursor;
        DEALLOCATE producto_cursor;

        -- Actualizar cliente y vendedor de la factura
        UPDATE factura
        SET fkidcliente = @p_fkidcliente,
            fkidvendedor = @p_fkidvendedor
        WHERE numero = @p_numero;

        -- Retornar resultado como JSON
        DECLARE @v_factura_json NVARCHAR(MAX);
        DECLARE @v_productos_json NVARCHAR(MAX);

        SELECT @v_factura_json = (
            SELECT f.numero, f.fecha, f.total, f.estado, f.fkidcliente, f.fkidvendedor
            FROM factura f WHERE f.numero = @p_numero
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        SELECT @v_productos_json = (
            SELECT pf.fkcodproducto AS codigo_producto, pr.nombre AS nombre_producto,
                   pf.cantidad, pr.valorunitario, pf.subtotal
            FROM productosporfactura pf
            JOIN producto pr ON pr.codigo = pf.fkcodproducto
            WHERE pf.fknumfactura = @p_numero
            FOR JSON PATH
        );

        SET @p_resultado = N'{"factura":' + @v_factura_json + N',"productos":' + ISNULL(@v_productos_json, N'[]') + N'}';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        IF CURSOR_STATUS('local', 'producto_cursor') >= 0
        BEGIN
            CLOSE producto_cursor;
            DEALLOCATE producto_cursor;
        END;

        THROW;
    END CATCH
END;
GO

-- ------------------------------------------------------------
-- 5. SP BORRAR FACTURA Y PRODUCTOSPORFACTURA
-- ON DELETE CASCADE elimina productosporfactura automáticamente.
-- El trigger restaura stock al borrar cada producto de la factura.
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "sp_borrar_factura_y_productosporfactura",
--     "p_numero": 1, "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE sp_borrar_factura_y_productosporfactura
    @p_numero INT,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_total DECIMAL(18,2);
    DECLARE @v_cantidad_productos INT;
    DECLARE @v_msg NVARCHAR(500);

    -- Validacion antes de abrir transaccion
    IF NOT EXISTS (SELECT 1 FROM factura WHERE numero = @p_numero)
    BEGIN
        SET @v_msg = CONCAT(N'Factura ', @p_numero, N' no existe');
        THROW 50005, @v_msg, 1;
    END

    -- ── TRANSACCION: todo o nada ──
    -- El DELETE CASCADE dispara el trigger de delete para cada producto,
    -- restaurando stock. Si algo falla, se revierte todo.
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Guardar info antes de borrar para el JSON de respuesta
        SELECT @v_cantidad_productos = COUNT(*)
        FROM productosporfactura WHERE fknumfactura = @p_numero;

        SELECT @v_total = f.total FROM factura f WHERE f.numero = @p_numero;

        -- Borrar factura (ON DELETE CASCADE borra productosporfactura,
        -- y el trigger restaura stock por cada producto eliminado)
        DELETE FROM factura WHERE numero = @p_numero;

        -- Retornar resultado como JSON
        SET @p_resultado = N'{"mensaje":"Factura eliminada exitosamente",' +
            N'"numero_eliminado":' + CAST(@p_numero AS NVARCHAR) + N',' +
            N'"total_eliminado":' + CAST(@v_total AS NVARCHAR) + N',' +
            N'"productos_eliminados":' + CAST(@v_cantidad_productos AS NVARCHAR) + N'}';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

-- ------------------------------------------------------------
-- 6. SP ANULAR FACTURA (borrado lógico)
-- Cambia el estado de la factura a 'anulada' y restaura el stock
-- de todos los productos. NO elimina la factura de la BD.
-- El borrado físico (DELETE) solo lo puede hacer el admin via
-- sp_borrar_factura_y_productosporfactura.
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "sp_anular_factura",
--     "p_numero": 1, "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE sp_anular_factura
    @p_numero INT,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_total DECIMAL(18,2);
    DECLARE @v_cantidad_productos INT;
    DECLARE @v_estado NVARCHAR(10);
    DECLARE @v_msg NVARCHAR(500);

    -- ============================================================
    -- ANULAR NO ES BORRAR, Y ESA ES TODA LA IDEA DE ESTE PROCEDIMIENTO.
    --
    -- La factura se queda donde esta, con sus renglones intactos, y solo
    -- cambia de `estado` a 'anulada'. Es el borrado logico:
    --
    --   · se puede auditar que se anulo, cuanto valia y que llevaba
    --   · la consulta «anulaciones por cliente» de la v4 tiene algo que
    --     contar — sobre filas borradas no se cuenta nada
    --   · y el numero de factura no se reutiliza
    -- ============================================================

    -- GUARDIA 1 — ¿existe la factura? Si no, no hay nada que anular.
    --
    -- `IF NOT EXISTS (SELECT 1 ...)` es el modismo de «¿hay alguna fila que
    -- cumpla esto?». El `SELECT 1` no trae datos: trae un uno cualquiera, y
    -- al motor le basta encontrar la primera coincidencia para contestar.
    -- Por eso no se escribe `SELECT COUNT(*)`, que recorreria todo para
    -- responder algo que se sabe con la primera fila.
    IF NOT EXISTS (SELECT 1 FROM factura WHERE numero = @p_numero)
    BEGIN
        SET @v_msg = CONCAT(N'Factura ', @p_numero, N' no existe');
        THROW 50010, @v_msg, 1;
    END

    -- Validar que no esté ya anulada
    -- GUARDIA 2 — ¿YA ESTABA ANULADA? Y esta guardia no es cortesia: es lo
    -- unico que protege el inventario.
    --
    -- Sin ella, anular dos veces la misma factura devolveria el stock DOS
    -- veces, y la bodega quedaria con mercancia que no existe. El sistema
    -- seguiria funcionando y las cifras serian mentira.
    --
    -- Es la diferencia entre una operacion idempotente y una que no lo es:
    -- cambiar el estado a 'anulada' dos veces da lo mismo, pero SUMAR stock
    -- dos veces no. Por eso se verifica antes de sumar.
    SELECT @v_estado = estado FROM factura WHERE numero = @p_numero;
    IF @v_estado = N'anulada'
    BEGIN
        SET @v_msg = CONCAT(N'Factura ', @p_numero, N' ya está anulada');
        THROW 50010, @v_msg, 1;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- DEVOLVER EL STOCK, A MANO Y EN UNA SOLA SENTENCIA.
        --
        -- ¿Por que a mano, si hay disparadores que ya saben hacerlo? Porque
        -- los disparadores viven en `productosporfactura`, y aqui NO se toca
        -- esa tabla: los renglones se conservan. Nadie los borra, asi que
        -- nada se dispara, y el stock hay que devolverlo explicitamente.
        --
        -- ES LA EXCEPCION A LA REGLA «EL STOCK LO MUEVEN LOS DISPARADORES»,
        -- y conviene saberla porque es la pregunta natural de la
        -- sustentacion: aqui lo mueve el procedimiento, y es correcto
        -- justamente porque la alternativa —borrar los renglones para que el
        -- disparador actue— destruiria la informacion que se quiere auditar.
        --
        -- El JOIN recorre todos los renglones de esa factura y le suma a cada
        -- producto su cantidad. Una sentencia, todos los productos.
        UPDATE p
        SET p.stock = p.stock + pf.cantidad
        FROM producto p
        JOIN productosporfactura pf ON p.codigo = pf.fkcodproducto
        WHERE pf.fknumfactura = @p_numero;

        -- Guardar info para la respuesta
        SELECT @v_total = total FROM factura WHERE numero = @p_numero;
        SELECT @v_cantidad_productos = COUNT(*) FROM productosporfactura WHERE fknumfactura = @p_numero;

        -- Cambiar estado a 'anulada'
        UPDATE factura SET estado = N'anulada' WHERE numero = @p_numero;

        -- Retornar resultado como JSON
        SET @p_resultado = N'{"mensaje":"Factura anulada exitosamente",' +
            N'"numero_anulado":' + CAST(@p_numero AS NVARCHAR) + N',' +
            N'"total_anulado":' + CAST(@v_total AS NVARCHAR) + N',' +
            N'"productos_afectados":' + CAST(@v_cantidad_productos AS NVARCHAR) + N',' +
            N'"estado":"anulada"}';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS - USUARIOS CON ROLES
-- Nota: El cifrado lo hace la API C# con el parámetro camposEncriptar
-- ============================================================

-- ------------------------------------------------------------
-- 6. SP CREAR USUARIO CON ROLES
-- Recibe: email, contraseña y JSON array de roles [{"fkidrol":1},...]
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "crear_usuario_con_roles",
--     "p_email": "user@correo.com", "p_contrasena": "pass123",
--     "p_roles_json": "[{\"fkidrol\":1},{\"fkidrol\":2}]",
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE crear_usuario_con_roles
    @p_email NVARCHAR(100),
    @p_contrasena NVARCHAR(200),
    @p_roles_json NVARCHAR(MAX),
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_idrol INT;
    DECLARE @v_roles_json NVARCHAR(MAX);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insertar el usuario
        INSERT INTO usuario (email, contrasena) VALUES (@p_email, @p_contrasena);

        -- Insertar los roles del usuario
        DECLARE rol_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT CAST(JSON_VALUE(value, '$.fkidrol') AS INT)
            FROM OPENJSON(@p_roles_json);

        OPEN rol_cursor;
        FETCH NEXT FROM rol_cursor INTO @v_idrol;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO rol_usuario (fkemail, fkidrol) VALUES (@p_email, @v_idrol);
            FETCH NEXT FROM rol_cursor INTO @v_idrol;
        END

        CLOSE rol_cursor;
        DEALLOCATE rol_cursor;

        -- Retornar resultado como JSON
        SELECT @v_roles_json = (
            SELECT r.id AS idrol, r.nombre
            FROM rol_usuario ru
            JOIN rol r ON r.id = ru.fkidrol
            WHERE ru.fkemail = @p_email
            FOR JSON PATH
        );

        SET @p_resultado = N'{"email":"' + @p_email + N'","roles":' + ISNULL(@v_roles_json, N'[]') + N'}';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        IF CURSOR_STATUS('local', 'rol_cursor') >= 0
        BEGIN
            CLOSE rol_cursor;
            DEALLOCATE rol_cursor;
        END;

        THROW;
    END CATCH
END;
GO

-- ------------------------------------------------------------
-- 7. SP ACTUALIZAR USUARIO CON ROLES
-- Actualiza contraseña (si no está vacía) y reemplaza roles
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "actualizar_usuario_con_roles",
--     "p_email": "user@correo.com", "p_contrasena": "newpass",
--     "p_roles": "[{\"fkidrol\":1},{\"fkidrol\":3}]",
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE actualizar_usuario_con_roles
    @p_email NVARCHAR(100),
    @p_contrasena NVARCHAR(200),
    @p_roles NVARCHAR(MAX),
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_idrol INT;
    DECLARE @v_roles_json NVARCHAR(MAX);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Actualizar la contraseña solo si no está vacía
        IF @p_contrasena IS NOT NULL AND @p_contrasena != N''
            UPDATE usuario SET contrasena = @p_contrasena WHERE email = @p_email;

        -- Eliminar los roles anteriores
        DELETE FROM rol_usuario WHERE fkemail = @p_email;

        -- Insertar los nuevos roles
        DECLARE rol_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT CAST(JSON_VALUE(value, '$.fkidrol') AS INT)
            FROM OPENJSON(@p_roles);

        OPEN rol_cursor;
        FETCH NEXT FROM rol_cursor INTO @v_idrol;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO rol_usuario (fkemail, fkidrol) VALUES (@p_email, @v_idrol);
            FETCH NEXT FROM rol_cursor INTO @v_idrol;
        END

        CLOSE rol_cursor;
        DEALLOCATE rol_cursor;

        -- Retornar resultado como JSON
        SELECT @v_roles_json = (
            SELECT r.id AS idrol, r.nombre
            FROM rol_usuario ru
            JOIN rol r ON r.id = ru.fkidrol
            WHERE ru.fkemail = @p_email
            FOR JSON PATH
        );

        SET @p_resultado = N'{"email":"' + @p_email + N'","roles":' + ISNULL(@v_roles_json, N'[]') + N'}';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        IF CURSOR_STATUS('local', 'rol_cursor') >= 0
        BEGIN
            CLOSE rol_cursor;
            DEALLOCATE rol_cursor;
        END;

        THROW;
    END CATCH
END;
GO

-- ------------------------------------------------------------
-- 8. SP ELIMINAR USUARIO CON ROLES
-- Elimina el usuario (ON DELETE CASCADE borra sus roles)
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "eliminar_usuario_con_roles",
--     "p_email": "user@correo.com", "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE eliminar_usuario_con_roles
    @p_email NVARCHAR(100),
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_msg NVARCHAR(500);

    IF NOT EXISTS (SELECT 1 FROM usuario WHERE email = @p_email)
    BEGIN
        SET @v_msg = CONCAT(N'Usuario ', @p_email, N' no existe');
        THROW 50006, @v_msg, 1;
    END

    -- Eliminar roles del usuario primero (FK sin CASCADE)
    DELETE FROM rol_usuario WHERE fkemail = @p_email;
    DELETE FROM usuario WHERE email = @p_email;

    SET @p_resultado = N'{"mensaje":"Usuario eliminado exitosamente","email_eliminado":"' + @p_email + N'"}';
END;
GO

-- ------------------------------------------------------------
-- 9. SP ACTUALIZAR ROLES DE USUARIO
-- Solo reemplaza los roles sin tocar la contraseña
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "actualizar_roles_usuario",
--     "p_email": "user@correo.com",
--     "p_roles_json": "[{\"fkidrol\":1},{\"fkidrol\":2}]",
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE actualizar_roles_usuario
    @p_email NVARCHAR(100),
    @p_roles_json NVARCHAR(MAX),
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_idrol INT;
    DECLARE @v_roles_json NVARCHAR(MAX);
    DECLARE @v_msg NVARCHAR(500);

    IF NOT EXISTS (SELECT 1 FROM usuario WHERE email = @p_email)
    BEGIN
        SET @v_msg = CONCAT(N'Usuario ', @p_email, N' no existe');
        THROW 50007, @v_msg, 1;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Eliminar los roles anteriores
        DELETE FROM rol_usuario WHERE fkemail = @p_email;

        -- Insertar los nuevos roles
        DECLARE rol_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT CAST(JSON_VALUE(value, '$.fkidrol') AS INT)
            FROM OPENJSON(@p_roles_json);

        OPEN rol_cursor;
        FETCH NEXT FROM rol_cursor INTO @v_idrol;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO rol_usuario (fkemail, fkidrol) VALUES (@p_email, @v_idrol);
            FETCH NEXT FROM rol_cursor INTO @v_idrol;
        END

        CLOSE rol_cursor;
        DEALLOCATE rol_cursor;

        -- Retornar resultado como JSON
        SELECT @v_roles_json = (
            SELECT r.id AS idrol, r.nombre
            FROM rol_usuario ru
            JOIN rol r ON r.id = ru.fkidrol
            WHERE ru.fkemail = @p_email
            FOR JSON PATH
        );

        SET @p_resultado = N'{"email":"' + @p_email + N'","roles":' + ISNULL(@v_roles_json, N'[]') + N'}';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        IF CURSOR_STATUS('local', 'rol_cursor') >= 0
        BEGIN
            CLOSE rol_cursor;
            DEALLOCATE rol_cursor;
        END;

        THROW;
    END CATCH
END;
GO

-- ------------------------------------------------------------
-- 10. SP CONSULTAR USUARIO CON ROLES
-- Retorna JSON con email y array de roles
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "consultar_usuario_con_roles",
--     "p_email": "admin@correo.com", "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE consultar_usuario_con_roles
    @p_email NVARCHAR(100),
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_roles_json NVARCHAR(MAX);
    DECLARE @v_msg NVARCHAR(500);

    IF NOT EXISTS (SELECT 1 FROM usuario WHERE email = @p_email)
    BEGIN
        SET @v_msg = CONCAT(N'Usuario ', @p_email, N' no existe');
        THROW 50008, @v_msg, 1;
    END

    SELECT @v_roles_json = (
        SELECT r.id AS idrol, r.nombre
        FROM rol_usuario ru
        JOIN rol r ON r.id = ru.fkidrol
        WHERE ru.fkemail = @p_email
        FOR JSON PATH
    );

    SET @p_resultado = N'{"email":"' + @p_email + N'","roles":' + ISNULL(@v_roles_json, N'[]') + N'}';
END;
GO

-- ------------------------------------------------------------
-- 11. SP LISTAR USUARIOS CON ROLES
-- Retorna JSON array con todos los usuarios y sus roles
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "listar_usuarios_con_roles", "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE listar_usuarios_con_roles
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_result NVARCHAR(MAX) = N'[';
    DECLARE @v_email NVARCHAR(100);
    DECLARE @v_roles_json NVARCHAR(MAX);
    DECLARE @v_first BIT = 1;

    DECLARE usuario_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT email FROM usuario ORDER BY email;

    OPEN usuario_cursor;
    FETCH NEXT FROM usuario_cursor INTO @v_email;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @v_first = 0
            SET @v_result = @v_result + N',';
        SET @v_first = 0;

        SELECT @v_roles_json = (
            SELECT r.id AS idrol, r.nombre
            FROM rol_usuario ru
            JOIN rol r ON r.id = ru.fkidrol
            WHERE ru.fkemail = @v_email
            FOR JSON PATH
        );

        SET @v_result = @v_result + N'{"email":"' + @v_email + N'","roles":' + ISNULL(@v_roles_json, N'[]') + N'}';

        FETCH NEXT FROM usuario_cursor INTO @v_email;
    END

    CLOSE usuario_cursor;
    DEALLOCATE usuario_cursor;

    SET @v_result = @v_result + N']';

    IF @v_first = 1
        SET @v_result = N'[]';

    SET @p_resultado = @v_result;
END;
GO

-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS - PERMISOS (RBAC)
-- ============================================================

-- ------------------------------------------------------------
-- 12. SP VERIFICAR ACCESO A RUTA
-- Verifica si un usuario tiene permiso para acceder a una ruta
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "verificar_acceso_ruta",
--     "p_email": "admin@correo.com", "p_fkidruta": 2,
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE verificar_acceso_ruta
    @p_email NVARCHAR(100),
    @p_fkidruta INT,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_tiene_acceso BIT = 0;

    IF EXISTS (
        SELECT 1
        FROM usuario u
        INNER JOIN rol_usuario ur ON u.email = ur.fkemail
        INNER JOIN rutarol rr ON ur.fkidrol = rr.fkidrol
        WHERE u.email = @p_email AND rr.fkidruta = @p_fkidruta
    )
        SET @v_tiene_acceso = 1;

    SET @p_resultado = N'{"tiene_acceso":' + CAST(@v_tiene_acceso AS NVARCHAR) +
        N',"email":"' + @p_email + N'","fkidruta":' + CAST(@p_fkidruta AS NVARCHAR) + N'}';
END;
GO

-- ------------------------------------------------------------
-- 13. SP LISTAR RUTAROL
-- Lista todos los permisos ruta-rol con nombres
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "listar_rutarol", "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE listar_rutarol
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @p_resultado = (
        SELECT rr.fkidruta, rt.ruta, rr.fkidrol, r.nombre AS rol
        FROM rutarol rr
        JOIN ruta rt ON rt.id = rr.fkidruta
        JOIN rol r ON r.id = rr.fkidrol
        ORDER BY rt.ruta, r.nombre
        FOR JSON PATH
    );

    SET @p_resultado = ISNULL(@p_resultado, N'[]');
END;
GO

-- ------------------------------------------------------------
-- 14. SP CREAR RUTAROL
-- Asigna un rol a una ruta por IDs
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "crear_rutarol",
--     "p_fkidruta": 8, "p_fkidrol": 3,
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE crear_rutarol
    @p_fkidruta INT,
    @p_fkidrol INT,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si la ruta existe
    IF NOT EXISTS (SELECT 1 FROM ruta WHERE id = @p_fkidruta)
    BEGIN
        SET @p_resultado = N'{"success":false,"message":"La ruta especificada no existe"}';
        RETURN;
    END

    -- Verificar si el rol existe
    IF NOT EXISTS (SELECT 1 FROM rol WHERE id = @p_fkidrol)
    BEGIN
        SET @p_resultado = N'{"success":false,"message":"El rol especificado no existe"}';
        RETURN;
    END

    -- Verificar si el permiso ya existe
    IF EXISTS (SELECT 1 FROM rutarol WHERE fkidruta = @p_fkidruta AND fkidrol = @p_fkidrol)
    BEGIN
        SET @p_resultado = N'{"success":false,"message":"El permiso ya existe"}';
        RETURN;
    END

    INSERT INTO rutarol (fkidruta, fkidrol) VALUES (@p_fkidruta, @p_fkidrol);
    SET @p_resultado = N'{"success":true,"message":"Permiso creado exitosamente"}';
END;
GO

-- ------------------------------------------------------------
-- 15. SP ELIMINAR RUTAROL
-- Quita un permiso ruta-rol por IDs
-- Ejemplo via API:
--   POST /api/procedimientos/ejecutarsp
--   { "nombreSP": "eliminar_rutarol",
--     "p_fkidruta": 8, "p_fkidrol": 3,
--     "p_resultado": null }
-- ------------------------------------------------------------
CREATE PROCEDURE eliminar_rutarol
    @p_fkidruta INT,
    @p_fkidrol INT,
    @p_resultado NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el permiso existe
    IF NOT EXISTS (SELECT 1 FROM rutarol WHERE fkidruta = @p_fkidruta AND fkidrol = @p_fkidrol)
    BEGIN
        SET @p_resultado = N'{"success":false,"message":"El permiso no existe"}';
        RETURN;
    END

    DELETE FROM rutarol WHERE fkidruta = @p_fkidruta AND fkidrol = @p_fkidrol;
    SET @p_resultado = N'{"success":true,"message":"Permiso eliminado exitosamente"}';
END;
GO

-- ============================================================
-- DATOS
-- ============================================================

-- Empresas
INSERT INTO empresa (codigo, nombre) VALUES
(N'E001', N'Comercial Los Andes S.A.'),
(N'E002', N'Distribuciones El Centro S.A.'),
(N'E999', N'Empresa Test');

-- Personas
INSERT INTO persona (codigo, nombre, email, telefono) VALUES
(N'P001', N'Ana Torres', N'ana.torres@correo.com', N'3011111111'),
(N'P002', N'Carlos Pérez', N'carlos.perez@correo.com', N'3022222222'),
(N'P003', N'María Gómez', N'maria.gomez@correo.com', N'3033333333'),
(N'P004', N'Juan Díaz', N'juan.diaz@correo.com', N'3044444444'),
(N'P005', N'Laura Rojas', N'laura.rojas@correo.com', N'3055555555'),
(N'P006', N'Pedro Castillo', N'pedro.castillo@correo.com', N'3066666666');

-- Productos
INSERT INTO producto (codigo, nombre, stock, valorunitario) VALUES
(N'PR001', N'Laptop Lenovo IdeaPad', 17, 2500000),
(N'PR002', N'Monitor Samsung 24"', 27, 800000),
(N'PR003', N'Teclado Logitech K380', 42, 150000),
(N'PR004', N'Mouse HP', 55, 90000),
(N'PR005', N'Impresora Epson EcoTank1', 14, 1100000),
(N'PR006', N'Auriculares Sony WH-CH510', 23, 240000),
(N'PR007', N'Tablet Samsung Tab A9', 15, 950000),
(N'PR008', N'Disco Duro Seagate 1TB', 32, 280000);

-- Roles (con IDENTITY_INSERT para IDs explícitos)
--
-- QUE HACE IDENTITY_INSERT: le dice al motor «dejame poner yo el id, no lo
-- generes tu». Normalmente un IDENTITY no se puede escribir a mano.
--
-- PARA QUE SE NECESITA AQUI: porque las semillas de `rutarol` mas abajo dicen
-- (1,1), (2,1), (3,2)... Esos numeros son ids de rol y de ruta. Si el motor
-- los repartiera por su cuenta, los permisos quedarian apuntando a roles
-- distintos de los que se pensaron — y el sistema arrancaria con los permisos
-- cruzados, sin un solo error.
--
-- Se apaga en cuanto termina (IDENTITY_INSERT OFF): de ahi en adelante los
-- ids los vuelve a poner el motor.
SET IDENTITY_INSERT rol ON;
INSERT INTO rol (id, nombre) VALUES
(1, N'Administrador'),
(2, N'Vendedor'),
(3, N'Cajero'),
(4, N'Contador'),
(5, N'Cliente');
SET IDENTITY_INSERT rol OFF;

-- Rutas
-- ============================================================
-- LOS NOMBRES DE `ruta` LLEVAN PUNTO, NO BARRA
--
-- Decian '/producto', '/usuario', '/permiso/crear'... y se confundian con los
-- ENDPOINTS de la API -/api/producto-, que son OTRA COSA.
--
-- Esto no son rutas HTTP: son INTERFACES y ACCIONES PROTEGIBLES. Lo que la
-- tabla guarda es «a que se puede entrar», y quien lo consume es
-- verificar_acceso_ruta, no el enrutador de la API.
--
--   interfaz.productos   una interfaz grafica a la que un rol entra o no
--   permiso.crear        una accion concreta
--
-- La notacion de punto no se puede leer como una URL, que es justamente el
-- punto. El procedimiento usa el ID y no el texto, asi que el cambio no rompe
-- nada.
-- ============================================================
INSERT INTO ruta (ruta, descripcion) VALUES
(N'interfaz.inicio', N'Página principal - Dashboard'),
(N'interfaz.usuarios', N'Gestión de usuarios'),
(N'interfaz.facturas', N'Gestión de facturas'),
(N'interfaz.clientes', N'Gestión de clientes'),
(N'interfaz.vendedores', N'Gestión de vendedores'),
(N'interfaz.personas', N'Gestión de personas'),
(N'interfaz.empresas', N'Gestión de empresas'),
(N'interfaz.productos', N'Gestión de productos'),
(N'interfaz.roles', N'Gestión de roles'),
(N'interfaz.permisos', N'Gestión de permisos (asignación rol-ruta)'),
(N'permiso.crear', N'Crear permiso (POST)'),
(N'permiso.eliminar', N'Eliminar permiso (POST)'),
(N'interfaz.rutas', N'Gestión de rutas del sistema'),
(N'ruta.crear', N'Crear ruta (POST)'),
(N'ruta.eliminar', N'Eliminar ruta (POST)');

-- Usuarios
-- ============================================================
-- LAS CONTRASENAS: CON HASH, Y SE SABEN CUALES SON
--
-- Dos reglas, y la segunda es la que suele faltar:
--
--   1. NINGUNA fila guarda texto legible. La columna es NVARCHAR(200) -y no
--      20- precisamente porque un hash de bcrypt ocupa 60 caracteres.
--
--   2. Las contrasenas en claro estan ESCRITAS EN LA DOCUMENTACION, porque del
--      hash no se puede volver a la clave -eso es lo que lo hace un hash-. Sin
--      saberlas no hay forma de iniciar sesion, y sin iniciar sesion no se
--      comprueba un solo criterio del control de acceso.
--
-- EL HASH ES BCRYPT CON COSTO 12. El `$2a$12$` del principio lo dice: `2a` es
-- la variante y `12` el costo. Subir el costo a 13 duplica el tiempo de
-- calculo — y es para lo que se diseno bcrypt: para encarecerlo cuando las
-- maquinas sean mas rapidas, sin cambiar de funcion.
--
-- Y CADA HASH ES DISTINTO AUNQUE LA CLAVE SEA LA MISMA. Los dos usuarios de
-- carlos.castro comparten contrasena y sus hash no se parecen: bcrypt trae
-- SALT incorporado. Sin el, dos hash iguales delatarian que esas dos personas
-- usan la misma clave.
--
-- Las contrasenas en claro, para las pruebas (7_quickstart.md):
--
--   admin@correo.com                      admin123       Administrador
--   vendedor1@correo.com                  vendedor123    Vendedor + Cajero
--   jefe@correo.com                       jefe123        Administrador + Cajero + Contador
--   cliente1@correo.com                   cliente123     Cliente
--   test_encript@correo.com               test123        Administrador
--   nuevo@correo.com                      nuevo123       Administrador + Vendedor + Cajero
--   carlos.castro@usbmed.edu.co           carlos123      todos los roles
--   carloscastro5033@correo.itm.edu.co    carlos123      todos los roles
--
-- LOS TRES QUE IMPORTAN PARA PROBAR EL CONTROL DE ACCESO:
--
--   admin@correo.com       Administrador: entra a las 15 rutas
--   vendedor1@correo.com   Vendedor: SOLO inicio, facturas y clientes
--   cliente1@correo.com    Cliente: SOLO inicio y productos
--
-- Con esos tres se comprueba el 403: identificarse como vendedor1 y pedir
-- /api/usuario tiene que responder 403, no 401. Y NO porque la interfaz
-- esconda el boton: escribiendo la direccion a mano.
-- ============================================================
INSERT INTO usuario (email, contrasena) VALUES
(N'admin@correo.com', N'$2a$12$PJf6LIuW8uL9q9hK0LsG0ebvUll.eLcJgg6lmTIPVk84p0fwD0T5u'),
(N'vendedor1@correo.com', N'$2a$12$MeuuKTqIN3JeEYGUCMtbueU5k8QVy7mmiB.yVDkT9hUp0FIyyrdZ2'),
(N'jefe@correo.com', N'$2a$12$Ymj52uGk70gKEzBTbRuJZe951H0y1dMXWe6C92k0iEqI/ztDiEoI2'),
(N'cliente1@correo.com', N'$2a$12$6jj6g3NiJU9QJ/DmPifIc.z4LP/csDIdbmZKlRgLAYfFcMz4S.Y9a'),
(N'test_encript@correo.com', N'$2a$12$FfoTc6rfT1N8jnjZtT5f0OzEC.36IgR2yHQmPgURMfh5lNrw6W7ky'),
(N'nuevo@correo.com', N'$2a$12$ug9KzUG5hN77MpwVzy/vyuSwo.bFxFIA80xwkr4//R3lkwgudaKOy'),
(N'carlos.castro@usbmed.edu.co', N'$2a$12$f1UjnYhuaQUrCS8w/EARw.BtSSqCyPh3lA82/tTEgeZ.kQcbZMFzi'),
(N'carloscastro5033@correo.itm.edu.co', N'$2a$12$F7CLooKrzi/ec4U0iI9.leBhPod38EMnViwRD6ER.6IkSaha8kF3K');

-- Clientes (con IDENTITY_INSERT para IDs explícitos)
SET IDENTITY_INSERT cliente ON;
INSERT INTO cliente (id, credito, fkcodpersona, fkcodempresa) VALUES
(1, 520000, N'P001', N'E001'),
(2, 250000, N'P003', N'E002'),
(3, 400000, N'P005', N'E001'),
(5, 700000, N'P006', N'E001');
SET IDENTITY_INSERT cliente OFF;

-- Vendedores (con IDENTITY_INSERT para IDs explícitos)
SET IDENTITY_INSERT vendedor ON;
INSERT INTO vendedor (id, carnet, direccion, fkcodpersona) VALUES
(1, 1001, N'Calle 10 #5-33', N'P002'),
(2, 1002, N'Carrera 15 #7-20', N'P004'),
(3, 1003, N'Avenida 30 #18-09', N'P006');
SET IDENTITY_INSERT vendedor OFF;

-- Facturas (con IDENTITY_INSERT para IDs explícitos)
-- Nota: los totales se insertan como 0, pero como los triggers están
-- deshabilitados para la carga de datos semilla, insertamos los totales directamente.
-- Primero deshabilitamos los triggers para la carga de datos semilla.
DISABLE TRIGGER trg_prodfact_insert ON productosporfactura;
DISABLE TRIGGER trg_prodfact_update ON productosporfactura;
DISABLE TRIGGER trg_prodfact_delete ON productosporfactura;
GO

SET IDENTITY_INSERT factura ON;
INSERT INTO factura (numero, fecha, total, fkidcliente, fkidvendedor) VALUES
(1, N'2025-12-03 12:57:19.2759200', 5000000, 1, 1),
(2, N'2025-12-03 12:57:19.2759200', 1250000, 2, 2),
(3, N'2025-12-03 12:57:19.2759200', 2030000, 3, 3),
(4, N'2025-12-03 13:04:59.0286130', 950000, 1, 1),
(5, N'2025-12-03 13:05:17.8743850', 2740000, 2, 2),
(6, N'2025-12-03 13:05:35.0284600', 4850000, 3, 3);
SET IDENTITY_INSERT factura OFF;

-- Productos por factura (triggers deshabilitados, insertamos subtotales directamente)
INSERT INTO productosporfactura (fknumfactura, fkcodproducto, cantidad, subtotal) VALUES
(1, N'PR001', 2, 5000000),
(2, N'PR002', 1, 800000),
(2, N'PR003', 3, 450000),
(3, N'PR004', 5, 450000),
(3, N'PR005', 1, 1100000),
(3, N'PR006', 2, 480000),
(4, N'PR007', 1, 950000),
(5, N'PR007', 2, 1900000),
(5, N'PR008', 3, 840000),
(6, N'PR001', 1, 2500000),
(6, N'PR002', 2, 1600000),
(6, N'PR003', 5, 750000);

-- Rehabilitar triggers
ENABLE TRIGGER trg_prodfact_insert ON productosporfactura;
ENABLE TRIGGER trg_prodfact_update ON productosporfactura;
ENABLE TRIGGER trg_prodfact_delete ON productosporfactura;
GO

-- Roles por usuario
INSERT INTO rol_usuario (fkemail, fkidrol) VALUES
(N'admin@correo.com', 1),
(N'vendedor1@correo.com', 2),
(N'vendedor1@correo.com', 3),
(N'jefe@correo.com', 1),
(N'jefe@correo.com', 3),
(N'jefe@correo.com', 4),
(N'cliente1@correo.com', 5),
(N'test_encript@correo.com', 1),
(N'nuevo@correo.com', 1),
(N'nuevo@correo.com', 2),
(N'nuevo@correo.com', 3),
(N'carlos.castro@usbmed.edu.co', 1),
(N'carlos.castro@usbmed.edu.co', 2),
(N'carlos.castro@usbmed.edu.co', 3),
(N'carlos.castro@usbmed.edu.co', 4),
(N'carlos.castro@usbmed.edu.co', 5),
(N'carloscastro5033@correo.itm.edu.co', 1),
(N'carloscastro5033@correo.itm.edu.co', 2),
(N'carloscastro5033@correo.itm.edu.co', 3),
(N'carloscastro5033@correo.itm.edu.co', 4),
(N'carloscastro5033@correo.itm.edu.co', 5);

-- Rutas por rol
-- Rutas por rol (fkidruta, fkidrol)
-- Rutas: 1=/home,2=/usuarios,3=/facturas,4=/clientes,5=/vendedores,6=/personas,7=/empresas,8=/productos,9=/roles,10=/permisos,11=/permisos/crear,12=/permisos/eliminar,13=/rutas,14=/rutas/crear,15=/rutas/eliminar
-- Roles: 1=Administrador,2=Vendedor,3=Cajero,4=Contador,5=Cliente
INSERT INTO rutarol (fkidruta, fkidrol) VALUES
(1, 1), (2, 1), (3, 1), (4, 1), (5, 1), (6, 1), (7, 1), (8, 1), (9, 1), (10, 1), (11, 1), (12, 1), (13, 1), (14, 1), (15, 1),
(1, 2), (3, 2), (4, 2),
(1, 3), (3, 3),
(1, 4), (4, 4), (8, 4),
(1, 5), (8, 5);
GO
