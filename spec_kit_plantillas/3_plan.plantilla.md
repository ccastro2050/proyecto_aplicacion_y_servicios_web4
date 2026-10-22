<!--
PLANTILLA — 3_plan.md
En Spec Kit oficial: plan.md  ·  comando /speckit.plan
Sale de: ARQUITECTURA.md + REQUISITOS_NO_FUNCIONALES.md  (+ el 2_spec ya generado)
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/3_plan.md

Es el CÓMO. El 2_spec dice qué; éste dice con qué piezas, y qué se decidió.
En Spec Kit oficial, este documento es el que contiene las tres fases (0, 1 y 2)
que producen research.md, data-model.md / contracts/ / quickstart.md, y tasks.md.
-->

# Plan técnico — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 3_plan.md va en negrita y sin enlace. -->

---

## 1. La decisión de fondo: {{LA_PREGUNTA_QUE_DEFINE_LA_VERSION}}

<!-- OBLIGATORIO, y va de PRIMERO. Toda versión tiene una decisión que la
     define; si no se escribe, el resto del plan parece arbitrario.
     Se escribe como TABLA DE OPCIONES con la elegida marcada ✅ — porque lo
     que enseña no es la decisión, son las descartadas. -->

| Opción | Qué pasa |
|---|---|
| **{{OPCION_A}}** | {{LA_CONSECUENCIA_CONCRETA}} |
| **{{OPCION_B}}** | {{LA_CONSECUENCIA_CONCRETA}} |
| **{{LA_ELEGIDA}}** ✅ | {{POR_QUE_ESTA}} |

## 2. La capa, que es la misma de siempre

```
{{DIAGRAMA_DE_LA_CADENA_DE_ESTA_VERSION}}
```

| Pieza | Qué hace |
|---|---|
| `{{ARCHIVO_1}}` | {{...}} |
| `{{ARCHIVO_2}}` | {{...}} |

<!-- Los archivos con su ruta real. Una tabla de piezas con nombres genéricos
     («el controlador», «el servicio») no le sirve a quien va a escribirlos. -->

> **{{LA_PREGUNTA_INCOMODA_QUE_ALGUIEN_VA_A_HACER}}** {{LA_RESPUESTA}}

<!-- Esta sección es de las que más se agradecen. En el ejemplo responde «¿para
     qué un servicio que no valida nada?» — porque el día que una consulta
     necesite una regla hay dónde ponerla, y si el controlador le hablara
     directo al repositorio esa regla terminaría en el controlador. La capa no
     se salta porque hoy esté vacía. -->

> {{POR_QUE_ESTE_TIPO_DE_DATO_Y_NO_EL_OTRO}}

## 3. {{LA_SEGUNDA_PARTE_DE_LA_VERSION}}

<!-- Normalmente la interfaz gráfica, o la otra mitad del trabajo.
     Con su estructura de carpetas, y una subsección por decisión. -->

```
{{ESTRUCTURA_DE_ARCHIVOS}}
```

### 3.1 {{DECISION_TECNICA_1}}

{{LA_EXPLICACION_CON_NUMEROS_SI_SE_PUEDEN_MEDIR}}

<!-- Con números cuando se puedan medir. «Las diez se piden a la vez; en fila
     serían diez esperas de 10 segundos: minuto y medio para abrir una página»
     convence; «es más eficiente» no. -->

### 3.2 {{DECISION_TECNICA_2}}

{{...}}

### 3.3 {{DECISION_TECNICA_3}}

| | |
|---|---|
| **{{ARGUMENTO}}** | {{...}} |
| **{{ARGUMENTO}}** | {{...}} |

> {{CUANDO_SI_SE_JUSTIFICARIA_LO_CONTRARIO}}

<!-- OPCIONAL y muy bueno: decir en qué caso la decisión sería la otra.
     Es lo que diferencia una decisión de un dogma. -->

### 3.4 {{EL_COMPORTAMIENTO_ANTE_FALLOS}}

{{QUE_PASA_SI_UNA_PARTE_FALLA}}

## 4. Lo que este plan deja FUERA

<!-- OBLIGATORIO. Cada fila con su razón, y la razón buena es casi siempre
     «el problema todavía no existe» — no «no tuvimos tiempo». -->

| | Por qué |
|---|---|
| **{{COSA_1}}** | {{LA_RAZON}} |
| **{{COSA_2}}** | {{LA_RAZON}} |
| **{{LO_PENDIENTE_DEL_ALCANCE}}** | **Pendientes**, y declaradas como tal en [2_spec.md](2_spec.md) §5 |
