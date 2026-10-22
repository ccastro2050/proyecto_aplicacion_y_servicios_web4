# Plan del generador de spec kits — y qué quedó construido

> **Qué es este documento.** El plan del generador, con **lo medido en este
> repositorio** y el registro de qué se construyó, dónde y qué falta decidir.
>
> Lo construido está en [`generador_spec_kit.md`](generador_spec_kit.md) y
> [`spec_kit_plantillas/`](spec_kit_plantillas/).
>
> Versión 2.0 · 22 de octubre de 2026.

---

## 1. Lo medido aquí, que es lo que decide el diseño

Contado el 22 de octubre de 2026 en este repositorio:

| | Documentos | Caracteres | Tokens aprox. |
|---|---|---|---|
| **El dominio** (`docs/dominio/`) | 19 | 186.821 | **~51.900** |
| **Un spec kit de una versión** (v4, con la guía) | 10 | 58.666 | **~16.300** |
| El más grande de entrada (`SUSTENTACION_DEL_CODIGO`) | — | 17.959 | ~5.000 |
| El más grande de salida (`2_spec`) | — | 7.440 | ~2.100 |

**Y aquí está la diferencia con los otros repositorios del semestre:** en este
el problema es **la entrada**, no la salida. El dominio pesa 51.900 tokens y
los documentos del spec kit son compactos —el más grande no llega a 2.100—.
En `proyecto_php4` es al revés: el dominio es más chico (37.800) y un spec kit
sale en 41.400.

> **Por eso el generador se diseñó aquí.** Este repositorio es el que tiene las
> cuatro versiones completas, la constitución **afuera** de `versiones/` —como
> manda Spec Kit oficial— y las guías por integrante. Es la base, y las
> plantillas salen de su v4.

---

## 2. La partición, y por qué no es solo ahorro

Pegar los 19 documentos y pedir los nueve de salida es **una conversación de
unos 68.000 tokens**. Cabe. Y sale mal, por dos razones distintas:

| | |
|---|---|
| **El noveno sale peor que el primero** | Con cincuenta mil tokens de material encima, el `9_checklist` termina resumiendo la conversación en vez de comprobar el sistema |
| **Ningún documento necesita todo el dominio** | El `5_data_model` no necesita la política de errores. Mandar lo que no se usa es ruido que compite con lo que sí importa |

**La segunda es la que importa**, y es la que hace que esto funcione: el mapa
de qué documento alimenta a cuál **ya estaba escrito** en cada documento del
spec kit, en su bloque «De dónde sale este documento», y completo en
[`docs/dominio/MAPA_DE_DOCUMENTOS.md`](docs/dominio/MAPA_DE_DOCUMENTOS.md).

Con ese mapa, **diez conversaciones**: la más cargada recibe ~11.400 tokens, la
más liviana ~5.700.

---

## 3. La numeración real de Spec Kit

Comprobado contra el repositorio oficial (`github/spec-kit`, rama `main`):
**los documentos de Spec Kit no llevan número.**

| Qué numera Spec Kit de verdad | Cómo |
|---|---|
| La carpeta de la funcionalidad | `specs/001-nombre/` |
| Las fases del plan | Fase 0, 1 y 2 — **dentro** de `plan.md` |
| Los artículos de la constitución | I a IX |
| Los archivos | **nada** |

**Nuestro 1 al 9 está en el orden oficial exacto**, así que no hay nada que
renombrar; el prefijo agrega algo que el nombre oficial no da —ordena la
carpeta y dice por dónde empezar—. El mapa completo de nombres, comandos y
fases quedó en
[`spec_kit_plantillas/0_COMO_SE_USA.md`](spec_kit_plantillas/0_COMO_SE_USA.md) §4.

### La corrección que salió de comprobarlo

La primera versión de este plan mandaba `4_research` a la última fase, con el
argumento de que «es el registro de las decisiones». **Es lo contrario de lo
que hace Spec Kit**, cuya Fase 0 produce *«research.md with all NEEDS
CLARIFICATION resolved»*: existe para **averiguar lo que no se sabe antes de
diseñar**. Ya está en su sitio: después del plan, antes del modelo de datos.

### Y una que juega a favor

**`/speckit.clarify` existe justamente para resolver los `NEEDS
CLARIFICATION`** que la plantilla oficial deja marcados. La regla de «no
inventar requisitos, marcar lo que falta» **es de Spec Kit**, no una invención
del curso.

---

## 4. Qué quedó construido

| Pieza | Qué es |
|---|---|
| [`generador_spec_kit.md`](generador_spec_kit.md) | **Diez fases.** Cada una: qué documentos de dominio pegar, qué plantilla, el prompt exacto para copiar, cómo comprobar lo que salió, y lo que suele salir mal |
| [`spec_kit_plantillas/0_COMO_SE_USA.md`](spec_kit_plantillas/0_COMO_SE_USA.md) | Las reglas, el glosario de marcas, el mapa de nombres oficiales y las divergencias declaradas |
| [`spec_kit_plantillas/_ENCABEZADO_COMUN.md`](spec_kit_plantillas/_ENCABEZADO_COMUN.md) | El bloque de encabezado, **idéntico en los ocho documentos de una versión**, escrito una sola vez |
| `spec_kit_plantillas/1_constitution` … `9_checklist` | Las nueve plantillas, derivadas de la v4 de este repositorio |
| `spec_kit_plantillas/GUIA_IA.plantilla.md` | El índice y la guía por integrante — la pieza que el curso evalúa |

### Las decisiones que se tomaron al construirlo

| | Qué se decidió | Por qué |
|---|---|---|
| **Las plantillas no traen dominio** | Ni un ejemplo de facturación | Si lo trajeran, un equipo de otro módulo entregaría un spec kit hablando de clientes y productos sin darse cuenta. El ejemplo se mira **al lado**, en `docs/spec_kit/versiones/v4_aplicativo/` |
| **Conservan los comentarios didácticos** | Cada plantilla explica *por qué* existe cada sección | Es un curso. Una plantilla que solo lista secciones produce documentos correctos que nadie puede sustentar |
| **El encabezado común, una vez** | En `_ENCABEZADO_COMUN.md` | Es idéntico en ocho documentos; repetirlo ocho veces en las plantillas garantiza que se desincronicen |
| **La `GUIA_IA` no se genera** | Fase 9: se arma | Lleva los prompts que de verdad se usaron **y lo que hubo que corregirle a la IA**. Eso solo lo sabe quien estuvo en la conversación |
| **Tres reglas en todos los prompts** | No inventar · no rediseñar el modelo · todo rastreable | La segunda nació de un fallo concreto: a una IA que recibe un modelo de datos le da por mejorarlo, y propone `CREATE TABLE` sobre un esquema que salió de la elicitación |

---

## 5. Lo que falta, y es suyo

| # | Qué | Opciones |
|---|---|---|
| **1** | **Probarlo con un dominio que no sea facturación** | Es el paso que de verdad lo valida. Los módulos de [`ProyectosDeAula/docs/`](ProyectosDeAula/docs/) sirven: mapa de conocimiento, gestión profesoral, investigación o innovación curricular |
| **2** | **Dónde queda para el estudiante** | Hoy está en la raíz de este repositorio. Si va también en `ProyectosDeAula/docs/`, se copia allá |
| **3** | **Llevarlo a los otros cuatro repositorios** | Las plantillas cambian de stack (C#/.NET aquí, PHP y Python allá); los prompts y las fases, no |
| **4** | **Si entra en la rúbrica** | El generador produce el spec kit, pero la `GUIA_IAn` sigue siendo humana — y es la que se sustenta |

> **El punto 1 es el que vale.** Un generador probado solo con el dominio del
> que salió no demostró nada todavía: lo que hay que ver es si funciona cuando
> el sistema no se parece al ejemplo.

---

## 6. Las tres comprobaciones de un spec kit generado

Están en [`generador_spec_kit.md`](generador_spec_kit.md) §5 con los comandos,
y son éstas:

| | Qué se busca |
|---|---|
| **1** | Ninguna marca `{{` sin reemplazar |
| **2** | Ningún comentario de plantilla copiado al resultado |
| **3** | Ningún `[NECESITA ACLARACIÓN]` pendiente |

Y la cuarta, que no se automatiza y es la que decide: **¿los nueve documentos
hablan del sistema de ese equipo?** Si el proyecto no es de facturación y
aparecen «factura», «cliente» o «producto», la IA se apoyó en el ejemplo.
