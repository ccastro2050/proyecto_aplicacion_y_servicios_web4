<!--
PLANTILLA — 7_quickstart.md
En Spec Kit oficial: quickstart.md  ·  comando /speckit.plan  ·  **FASE 1**
Sale de: DATOS_DE_PRUEBA.md + POLITICA_DE_ERRORES.md  (+ el 6_contracts generado)
Un ejemplo lleno: ../docs/spec_kit/versiones/v4_aplicativo/7_quickstart.md

LA REGLA DE ESTE DOCUMENTO: hay UNA SECCIÓN POR CRITERIO DE ACEPTACIÓN del
2_spec §3, numerada igual y titulada «CRITERIO n — …». Así cada criterio tiene
su comprobación y ninguna queda huérfana.

Y TODO COMANDO SE EXPLICA: qué hace, para qué sirve y qué efecto tiene.
Nada de copiar y pegar a ciegas.
-->

# Arranque y verificación — Versión {{N}}: {{TITULO_VERSION}}

<!-- ENCABEZADO COMÚN. Ver _ENCABEZADO_COMUN.md.
     Aquí la fila de 7_quickstart.md va en negrita y sin enlace. -->

---

## 1. Levantar

```powershell
{{EL_COMANDO_UNICO}}
```

{{QUE_PASA_AL_CORRERLO_Y_CUANTO_TARDA}}

> **{{LA_TRAMPA_DEL_ARRANQUE}}**

<!-- OBLIGATORIO, y es la nota que más tiempo ahorra. Lo típico: el contenedor
     dice `Up` pero el proceso de dentro todavía está compilando, así que el
     puerto no contesta. Un cliente apurado devuelve «connection refused» y
     parece una falla del código.
     Si su proyecto recompila al encender, dígalo aquí. -->

## 2. CRITERIO {{n}} — {{EL_CRITERIO}}

<!-- Si hace falta identificarse primero, va de primero. Y en PowerShell: -->

> **En PowerShell se usa `Invoke-RestMethod`, no `curl`.** El `curl` de
> PowerShell es un alias de `Invoke-WebRequest` y **no acepta las mismas
> banderas**: el comando parece correcto y falla por otra razón.

```powershell
{{COMO_OBTENER_LA_CREDENCIAL_O_EL_TOKEN}}
```

<!-- Las cuentas y los datos salen de DATOS_DE_PRUEBA.md. No se inventan:
     una clave inventada en un quickstart hace perder media hora. -->

```powershell
{{LAS_COMPROBACIONES_UNA_POR_UNA_CON_SU_LINEA_DE_SALIDA}}
```

{{QUE_TIENE_QUE_RESPONDER}}

> **{{QUE_CASOS_PUEDEN_SALIR_VACIOS_Y_TAMBIEN_PASAN}}**

<!-- Si alguna comprobación puede devolver cero y eso ES pasar, dígalo aquí y
     cite el criterio del 2_spec. Si no, alguien lo va a reportar como falla. -->

> **Si {{EL_SINTOMA}}**, es {{LA_CAUSA}}: ver [4_research.md](4_research.md) §{{D}}.

<!-- RECOMENDADO: el síntoma textual del tropiezo conocido, con el mensaje de
     error tal como sale, y a dónde ir. Es lo que convierte el 4_research en
     algo que se usa y no en algo que se archiva. -->

## 3. CRITERIO {{n}} — {{LA_INTERFAZ_GRAFICA}}

Abra **http://localhost:{{PUERTO_FRONT}}/{{RUTA}}**.

| Lo que hay que ver | Si no se ve |
|---|---|
| {{...}} | {{LA_CAUSA_PROBABLE_Y_A_DONDE_IR}} |
| {{...}} | {{...}} |

<!-- La segunda columna es la que importa: no «revise el código», sino la causa
     concreta y el documento donde está. -->

## 4. CRITERIO {{n}} — {{LOS_RECHAZOS}}

```powershell
{{COMO_PROVOCAR_EL_401_EL_403_O_EL_422}}
```

{{QUE_TIENE_QUE_RESPONDER_Y_POR_QUE}}

<!-- Comprobar que algo se RECHAZA es la mitad que se olvida. Un sistema que
     acepta todo pasa el camino feliz completo. -->

## 5. CRITERIO {{n}} — {{LA_PRUEBA_QUE_DEMUESTRA_QUE_ESTA_VIVO}}

<!-- RECOMENDADO. La comprobación de que el sistema lee del sistema y no de una
     copia: cambie algo por un lado y véalo cambiar por el otro. -->

| Paso | Qué tiene que pasar |
|---|---|
| **1.** {{ANOTE_UN_VALOR}} | — |
| **2.** {{CAMBIELO_DESDE_OTRA_PARTE_DEL_SISTEMA}} | — |
| **3.** {{VUELVA_A_MIRAR}} | {{QUE_SE_MOVIO}} |

> Si el número no se mueve, {{DE_DONDE_ESTA_LEYENDO_EN_REALIDAD}}.

## {{n}}. CRITERIO 1 — la regresión

<!-- OBLIGATORIO, Y VA SIEMPRE. Las versiones son acumulativas: que lo anterior
     siga funcionando HAY QUE COMPROBARLO, no suponerlo. -->

**Y es obligatoria.** El smoke test de {{LAS_VERSIONES_ANTERIORES}}, completos.
La v{{N}} no tocó {{LO_ANTERIOR}}, y **eso hay que comprobarlo**, no suponerlo.
