<!--
PLANTILLA — 3_plan.md
En Spec Kit oficial: plan.md  ·  comando /speckit.plan
Sale de: ARQUITECTURA_PROCESOS.md + REQUISITOS_NO_FUNCIONALES.md
         (+ el 2_spec ya generado, que fija el alcance)

ESTRUCTURA CANÓNICA, tomada de las v1, v2 y v3 de este repositorio —no de la
v4, que está comprimida y es la excepción—:

   1. La pila: qué cambia y qué NO
   2. La estructura y las capas, con el flujo de una petición
   3. Lo que esta versión agrega, ARCHIVO POR ARCHIVO
   4. Las decisiones de diseño, cada una con su razón
   5. Los tropiezos que esta versión tiene preparados
   6. Chequeo de constitución
   7. Lo que este plan deja FUERA, a propósito

Es el CÓMO. El 2_spec dice qué; éste dice con qué piezas y qué se decidió.
En Spec Kit oficial, este documento contiene las tres fases (0, 1 y 2) que
producen research.md, data-model.md / contracts/ / quickstart.md, y tasks.md.

Ejemplos llenos: ../docs/spec_kit/versiones/v1_sin_fk/3_plan.md (13.140 car)
                 ../docs/spec_kit/versiones/v2_con_fk/3_plan.md (10.841 car)
-->

# Plan técnico — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 3_plan.md va en negrita y sin enlace. -->

---

## 1. La pila: qué cambia y qué no

<!-- En la v1 esto es «el stack». De la v2 en adelante lo valioso es decir
     QUÉ NO CAMBIA: una versión que no agrega un paquete es una versión que
     no se desvió. -->

| | |
|---|---|
| **Lo que se agrega a la pila** | {{NADA_O_LO_QUE_SEA}} |
| **Lo que sigue igual** | {{LA_LISTA}} |

> {{POR_QUE_NO_HACE_FALTA_NADA_NUEVO}}

<!-- Si la versión NO agrega ni un paquete, dígalo con esas palabras y explique
     por qué eso es una buena noticia: significa que la decisión de la v1
     alcanzaba. -->

## 2. La estructura y las capas

```
{{ESTRUCTURA_DE_CARPETAS}}
```

### El flujo de una petición, de punta a punta

```
{{HTTP_→_CONTROLADOR_→_SERVICIO_→_REPOSITORIO_→_BASE_DE_DATOS}}
```

| Paso | Quién | Qué hace | Qué tiene PROHIBIDO |
|---|---|---|---|
| 1 | {{...}} | {{...}} | {{...}} |

<!-- OBLIGATORIO en la v1, porque es donde se aprende. De la v2 en adelante
     basta recordarlo y señalar lo que cambia. -->

> {{LA_REGLA_QUE_NO_SE_NEGOCIA}}

## 3. Lo que esta versión agrega, archivo por archivo

<!-- OBLIGATORIO, y es el corazón del documento. UNA SUBSECCIÓN POR PIEZA, con
     sus archivos y sus rutas REALES. Una tabla con nombres genéricos —«el
     controlador», «el servicio»— no le sirve a quien va a escribirlos. -->

### 3.1 {{LA_PRIMERA_PIEZA}}

| Archivo | Qué hace |
|---|---|
| `{{RUTA/ARCHIVO}}` | {{...}} |

{{LO_QUE_HAY_QUE_SABER_DE_ESTA_PIEZA}}

### 3.2 {{LA_SEGUNDA_PIEZA}}

{{...}}

### 3.{{n}} El ensamblador — {{QUE_LE_PASA_EN_ESTA_VERSION}}

<!-- Casi siempre es «lo único de la versión anterior que crece». Y es la
     casilla que más se olvida: sin registrar la dependencia, el proyecto
     COMPILA y falla el día que alguien usa el endpoint. -->

```{{LENGUAJE}}
{{LAS_LINEAS_QUE_SE_AGREGAN}}
```

### 3.{{n}} Las interfaces gráficas

<!-- OBLIGATORIO si la versión tiene pantallas, y va en el plan con el mismo
     detalle que la API. Media versión no es una versión. -->

{{...}}

## 4. Las decisiones de diseño, con su razón

<!-- UNA SUBSECCIÓN POR DECISIÓN. Cada una con TABLA DE OPCIONES y la elegida
     marcada ✅ — porque lo que enseña no es la decisión, son las descartadas,
     y hay que escribirlas con su MEJOR argumento.

     Y cuando se pueda poner un número, póngalo: «diez esperas de 10 segundos
     son minuto y medio para abrir una página» convence; «es más eficiente»
     no. -->

### 4.1 {{LA_DECISION_QUE_DEFINE_LA_VERSION}}

| Opción | Qué pasa |
|---|---|
| **{{OPCION_A}}** | {{SU_MEJOR_ARGUMENTO}}. Y {{LO_QUE_CUESTA}} |
| **{{LA_ELEGIDA}}** ✅ | {{POR_QUE_ESTA}} |

> {{LA_CONSECUENCIA_QUE_SE_VE_DESPUES}}

### 4.2 {{LA_SEGUNDA_DECISION}}

{{...}}

### 4.{{n}} {{LA_PREGUNTA_INCOMODA_QUE_ALGUIEN_VA_A_HACER}}

{{LA_RESPUESTA}}

<!-- RECOMENDADO y muy agradecido. En el ejemplo responde «¿para qué un
     servicio que no valida nada?» — porque el día que haga falta una regla
     hay dónde ponerla, y si el controlador le hablara directo al repositorio
     esa regla terminaría en el controlador. La capa no se salta porque hoy
     esté vacía. -->

## 5. Los {{CUANTOS}} tropiezos que esta versión tiene preparados

<!-- OBLIGATORIO de la v2 en adelante, y es la sección que más tiempo ahorra.
     LO QUE LA HACE VALIOSA: separar los que FALLAN de los que NO FALLAN. Un
     error que revienta se arregla en cinco minutos; uno que responde 200 con
     un dato equivocado cuesta una tarde.

     Si no conoce ninguno todavía, deje la sección y vuelva cuando aparezca. -->

| | Qué pasa | Cómo se ve |
|---|---|---|
| **1** | {{...}} | {{...}} |
| **2** | {{...}} | {{...}} |

> **{{CUANTOS_FALLAN_EN_SILENCIO_Y_POR_QUE_ESO_IMPORTA}}**

<!-- Y si alguno apareció MIDIENDO y no leyendo, dígalo: es la clase de dato
     que no está en ninguna documentación del lenguaje ni del motor. -->

## 6. Chequeo de constitución

<!-- OBLIGATORIO. Es la compuerta 2 del método: el plan se cuadra contra la
     constitución ANTES de construir. Una fila por artículo que esta versión
     toca. -->

| Artículo | Qué exige | ¿Lo cumple este plan? |
|---|---|---|
| {{N}} | {{...}} | ✅ {{COMO}} |
| {{N}} | {{...}} | ✅ {{COMO}} |

> {{SI_ALGUN_ARTICULO_QUEDA_EN_TENSION_DECIRLO_AQUI}}

## 7. Lo que este plan deja FUERA, a propósito

<!-- OBLIGATORIO. Cada fila con su razón, y la buena razón es casi siempre
     «el problema todavía no existe» — no «no tuvimos tiempo». -->

| | Por qué |
|---|---|
| **{{COSA_1}}** | {{LA_RAZON}} |
| **{{COSA_2}}** | {{LA_RAZON}} |
| **{{LO_PENDIENTE_DEL_ALCANCE}}** | **Pendientes**, y declaradas como tal en [2_spec.md](2_spec.md) |
