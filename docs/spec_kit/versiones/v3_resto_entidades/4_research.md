# Investigación y decisiones — Versión 3

> **Versión 3** · **Lectura opcional**: el porqué del [plan](3_plan.md).

---

## D1 — ¿Por qué "toda la BD con un motor" antes del segundo motor?
Regla del curso fijada por el profesor: cambiar de motor con la cobertura
a medias obligaría a la v4 a perseguir dos objetivos a la vez (completar
entidades Y portar dialecto). Con las 12 tablas cubiertas, la v4 será una
sola pregunta limpia: *¿el contrato sobrevive idéntico en otro motor?* —
y la fábrica nacerá con el sistema completo detrás.

## D2 — Cinco moldes en serie: la lección es que NO hay lección
Empresa, cliente, vendedor, rol y ruta no aportan técnica nueva — y eso es
lo que demuestran: el molde de la v1 escala en serie sin fricción. El
costo real aparece en Program.cs (16 registros más, §D6).

## D3 — BCrypt en el repositorio, y el hash JAMÁS sale
**Decisión:** el hash se calcula y se compara en `RepositorioUsuarioSqlServer`
(BCrypt.Net-Next, costo 12), y el modelo de lectura `Usuario` NO tiene
propiedad de contraseña.
**Por qué en el repo:** cómo se persiste un secreto es un detalle de la
capa de datos; servicio y controller ignoran el algoritmo.
**Por qué no exponer ni el hash:** un hash filtrado es material de ataque
offline. Regla simple y verificable: si no está en el modelo, no puede
viajar. (El gemelo Python del curso sí devuelve la fila completa — esta
versión endurece esa decisión a propósito y lo deja escrito.)
**Alternativa descartada:** SHA-256 "a mano" — sin salt ni factor de
costo, no es un hash de contraseñas.

## D4 — verificar-contrasena SIN JWT todavía
La v3 entrega el cimiento (credenciales verificables); el token, el
middleware y el control de acceso llegan con el front. Separarlos
deja ver que **autenticar** (¿eres quien dices?) y **autorizar** (¿puedes
hacer esto?) son problemas distintos.

## D5 — Puentes sin PUT/PATCH y DELETE por AMBAS columnas
Una asignación (usuario↔rol, ruta↔rol) no tiene campos editables: existe o
no existe — editar es quitar y poner. Y el DELETE filtra por las DOS
columnas de la PK compuesta: en el sistema padre del curso se detectó un
gemelo que filtraba solo por la primera y borraba de más; la v3 fija la
regla correcta desde la spec.

## D6 — Se deja crecer Program.cs a propósito
Con 22 `AddScoped`, el ensamblador ya duele. **Decisión:** NO refactorizar
todavía — la constitución prohíbe anticipar, y ese dolor es el argumento
pedagógico con el que la v4 justificará la fábrica real. El plan lo
declara para que nadie lo "arregle" por iniciativa propia.

## D7 — productosporfactura no recibe CRUD directo
Sus renglones nacen y mueren con la factura (SPs + trigger, v2). Un CRUD
directo permitiría desalinear subtotales sin pasar por la lógica de la BD.
Cobertura ≠ un controller por tabla: cobertura es que la tabla sea
operable por el camino correcto.

## D8 — Rutas por `{id:int}` también en `ruta`
La tabla `ruta` guarda paths con barras ("/home") pero su PK es `id INT
IDENTITY` — el CRUD va por id y no hay problema de URLs con barras. (El
gemelo Python usaba el string como clave y necesitó el convertidor
`:path`; aquí el DDL evita el problema.)
