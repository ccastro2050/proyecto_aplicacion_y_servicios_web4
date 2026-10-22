<!--
PLANTILLA — GUIA_IA{{N}}.md  y  GUIA_IA{{N}}_<NOMBRE>.md
En Spec Kit oficial: NO EXISTE. Ésta es nuestra.

Y ES LA PIEZA QUE EL CURSO EVALÚA: los prompts que de verdad se usaron, y lo
que hubo que corregirle a la IA.

**ÉSTA NO LA GENERA UNA IA.** Lleva lo que pasó en la conversación: qué se
pidió, qué salió mal, qué se corrigió. Eso solo lo sabe quien estuvo ahí. Una
IA puede ayudar a ORDENARLA una vez escrita, no a inventarla.

Son dos documentos:
  · GUIA_IA{{N}}.md          — el índice: quién hace qué, y lo que los tres
                                tienen que saber
  · GUIA_IA{{N}}_<NOMBRE>.md — una por integrante, con SU prompt
-->

# Versión {{N}} con IA — índice de las {{CUANTAS}} guías

> **{{QUE_DEFINE_ESTA_VERSION_EN_UNA_LINEA}}**
>
> {{EL_CONTRASTE_CON_LO_QUE_YA_SABIA_HACER_EL_SISTEMA}}

---

## 1. Quién hace qué

| | Su guía | Su camino | Lo que le toca |
|---|---|---|---|
| **{{INTEGRANTE_1}}** | [`GUIA_IA{{N}}_{{NOMBRE_1}}.md`](GUIA_IA{{N}}_{{NOMBRE_1}}.md) | {{A_chat_o_B_agentico}} | {{SU_PARTE}} |
| **{{INTEGRANTE_2}}** | [`GUIA_IA{{N}}_{{NOMBRE_2}}.md`](GUIA_IA{{N}}_{{NOMBRE_2}}.md) | {{...}} | {{SU_PARTE}} |
| **{{INTEGRANTE_3}}** | [`GUIA_IA{{N}}_{{NOMBRE_3}}.md`](GUIA_IA{{N}}_{{NOMBRE_3}}.md) | {{...}} | {{SU_PARTE}} |

> {{SI_ALGUIEN_CAMBIA_DE_HERRAMIENTA_EN_ESTA_VERSION_POR_QUE}}

<!-- El reparto sale de PLAN_DE_TRABAJO.md, con su sección. No se decide aquí. -->

---

## 2. Quién escribe cada documento de ESTA carpeta

<!-- OBLIGATORIO. Los ocho documentos del spec kit repartidos con nombre.
     Es lo que evita que los escriba uno solo la noche anterior. -->

| Documento | Lo redacta |
|---|---|
| [`2_spec.md`](2_spec.md) · [`4_research.md`](4_research.md) · [`7_quickstart.md`](7_quickstart.md) | **{{QUIEN}}** |
| [`3_plan.md`](3_plan.md) · [`5_data_model.md`](5_data_model.md) · [`6_contracts.md`](6_contracts.md) · [`8_tasks.md`](8_tasks.md) | **{{QUIEN}}** |
| [`9_checklist.md`](9_checklist.md) · este índice | **{{QUIEN}}** |
| `GUIA_IA{{N}}_<NOMBRE>` | **cada quien la suya** |

---

## 3. Qué define esta versión

{{LO_QUE_NO_PODIA_HACERSE_ANTES}}

```
{{LA_LISTA_DE_LO_NUEVO}}
```

> {{POR_QUE_ESTO_NO_ES_LA_VERSION_ANTERIOR_CON_MAS_PANTALLAS}}

| El reparto | |
|---|---|
| **{{INTEGRANTE_1}}** | {{...}} |
| **{{INTEGRANTE_2}}** | {{...}} |
| **{{INTEGRANTE_3}}** | {{...}} |

---

## 4. Lo que {{TODOS}} tienen que saber

<!-- Las dos o tres cosas que se equivocan TODOS si nadie las dice.
     Con el formato que funciona: el contraste lado a lado. -->

### {{LA_PRIMERA}}

```json
{{EL_CONTRASTE}}
```

> **{{POR_QUE_IMPORTA_Y_COMO_SE_NOTA_CUANDO_SALE_MAL}}**

### {{LA_SEGUNDA}}

| | Y eso significa |
|---|---|
| {{CASO}} | {{...}} |

> **{{LA_LECCION_DE_LA_VERSION}}**
>
> **{{COMO_SE_COMPRUEBA_PROVOCANDO_EL_CASO}}**

<!-- RECOMENDADO: «vacío» y «roto» se ven exactamente igual en una pantalla. La
     única forma de distinguirlos es PROVOCAR el caso. Una respuesta que nunca
     se vio llegar con datos no está probada. -->

---

## 5. Lo que cambia al trabajar con {{CHAT_O_AGENTE}}

<!-- OBLIGATORIO la primera vez que el equipo cambia de herramienta. -->

| Con chat | Con agente |
|---|---|
| Usted pega cada archivo | **Él los escribe, sin preguntar** |
| Usted ve cada línea antes de que entre | Usted ve **el resultado** |
| El error aparece donde usted pegó | El error puede estar en un archivo que usted no abrió |

> **La regla que reemplaza a «pegar de a un archivo»:** el prompt empieza
> pidiéndole al agente que **lea, resuma y espere confirmación** antes de tocar
> nada. Si el resumen está mal, el código va a estar mal — y es mucho más
> barato descubrirlo ahí.
>
> **Y la segunda: acote qué carpetas puede escribir.** Un agente sin alcance
> definido es capaz de reorganizar el proyecto entero con la mejor intención.

---

## 6. Lo que vale para {{TODOS}}

```powershell
git switch main ; git pull origin main
git switch -c rama-{{nombre}}-v{{N}}
git config user.name "{{su-usuario}}" ; git config user.email "{{su-correo}}"
git config user.name ; git config user.email
```

<!-- Las dos últimas líneas no sobran: imprimen lo que quedó configurado. Un
     commit con el nombre de otra persona se arregla, pero después. -->

**La interpretabilidad se califica** hablando y en persona. Con agente eso pesa
más: es código que usted no tecleó.

---

## 7. Cuándo está terminada

| Qué | Cómo |
|---|---|
| {{...}} | {{...}} |
| {{LO_QUE_SE_PRUEBA_PROVOCANDO_EL_CASO}} | §4 |
| **La regresión de {{LAS_VERSIONES_ANTERIORES}}** | Todo lo anterior |

```powershell
git tag -a v{{N}} -m "Version {{N}}: {{TITULO_VERSION}}"
git push origin v{{N}}
```

---
---

<!-- =====================================================================
     SEGUNDA PARTE: GUIA_IA{{N}}_<NOMBRE>.md — una por integrante
     ===================================================================== -->

# Versión {{N}} — {{INTEGRANTE}} · camino {{A_o_B}}

> **Su parte:** {{QUE_LE_TOCA}}.
>
> {{POR_QUE_VA_PRIMERO_O_DESPUES_DE_LOS_OTROS}}

---

## 1. Lo que construye

| | |
|---|---|
| **{{PIEZA_1}}** | {{...}} |
| **{{PIEZA_2}}** | {{...}} |

**No construye** {{LO_QUE_ES_DE_LOS_OTROS}} — son de {{QUIENES}}.

<!-- Decir qué NO le toca evita el trabajo duplicado y el conflicto de merge. -->

## 2. {{LA_CONFUSION_PROPIA_DE_SU_PARTE}}

> **{{EL_AVISO_CONCRETO}}**

| Esto NO es {{LO_QUE_DEBE_SER}} | Esto sí |
|---|---|
| {{EJEMPLO_MALO}} | {{EJEMPLO_BUENO}} |

## 3. {{LO_QUE_EL_FIJA_Y_LOS_DEMAS_CALCAN}}

<!-- Si su parte define algo que los otros consumen —la forma de una respuesta,
     el nombre de un campo—, va aquí, y con el encargo de avisarles. -->

```json
{{...}}
```

> **Usted lo fija y los otros lo calcan. Avíseles cuál quedó.**

## 4. {{EL_ERROR_QUE_SU_PARTE_INVITA_A_COMETER}}

```{{LENGUAJE}}
// MAL: {{QUE}}
{{EL_CODIGO_MALO}}
```

> **{{POR_QUE_ESTA_MAL_CON_NUMEROS}}**
>
> **{{DONDE_VA_DE_VERDAD}}**

<!-- Con números: «con seis filas funciona; con sesenta mil, tumba el proceso»
     explica más que «no es escalable». -->

## 5. Antes de abrir {{LA_HERRAMIENTA}}

```powershell
git switch main ; git pull origin main
git switch -c rama-{{nombre}}-v{{N}}
git config user.name "{{su-usuario}}" ; git config user.email "{{su-correo}}"
git config user.name ; git config user.email

# Y compruebe que la versión anterior funciona:
{{EL_COMANDO}}
```

## 6. El prompt (cópielo tal cual)

```
{{EL_PROMPT_COMPLETO}}
```

<!-- EL PROMPT ES LO QUE SE EVALÚA. Las cuatro partes que tiene que tener:

     1. EL PAPEL Y EL ALCANCE — qué se construye, y qué NO (lo de los
        compañeros), para que el agente no se lo lleve por delante.
     2. LEER ANTES DE ESCRIBIR — «PRIMERO lee los documentos bajo
        docs/spec_kit/ y el código que ya existe. Después resume lo que vas a
        hacer y ESPERA mi confirmación antes de tocar nada.»
     3. LAS REGLAS DEL PROYECTO — en español, las capas, sin inventar
        requisitos, lo que no esté en el dominio se marca
        [NECESITA ACLARACIÓN] en vez de rellenarse.
     4. QUÉ CARPETAS PUEDE ESCRIBIR — la lista. Nada más.
-->

## 7. Lo que hubo que corregirle

<!-- ESTA SECCIÓN SE ESCRIBE DESPUÉS, Y ES LA QUE MÁS VALE.
     No se puede llenar antes de usar la herramienta, y no la puede llenar
     nadie más. Es la prueba de que usted entendió lo que la IA escribió:
     si no hubo nada que corregir, o no se revisó, o se pidió muy poco.

     El formato: qué pidió, qué salió, qué estaba mal, cómo lo detectó. -->

| Qué salió | Qué estaba mal | Cómo lo detecté |
|---|---|---|
| {{...}} | {{...}} | {{...}} |

> {{LO_QUE_APRENDIO_PIDIENDOLE_ESTO_A_UNA_IA}}
