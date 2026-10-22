# Plan de trabajo del equipo — acta de la reunión de arranque

> # ⚠ REUNIÓN SIMULADA
>
> **No ocurrió.** Carlos, Paco y Luis son tres estudiantes inventados para que
> este repositorio demuestre cómo trabaja un equipo de tres. Las citas son un
> recurso de redacción, no una fuente.
>
> **Pero los acuerdos SÍ son reales en un sentido comprobable:** el historial de
> este repositorio los obedece. Lo que este documento dice que hace cada quien,
> `git log` lo confirma — y §7 dice cómo comprobarlo.
>
> **8 de agosto de 2026** · Versión 1.0

---

## 0. Por qué esta reunión existe antes de escribir una línea de código

Porque **un equipo que no acuerda quién hace qué, lo descubre fusionando** — y
para entonces ya hay dos arquitecturas en el mismo proyecto y una tarde perdida.

> **Y porque la nota es individual.** La rúbrica dice que *«cada estudiante
> responde por SU rama: qué hizo, por qué, y sus commits lo respaldan»*. Si el
> reparto no está escrito antes, al final nadie puede demostrar qué parte era
> suya.

---

## 1. Quiénes son, y qué cuenta tiene cada uno

| | Cuenta de GitHub | Papel |
|---|---|---|
| **Carlos** | `ccastro2050` | **Integrador** — y además su parte de código |
| **Paco** | `ccastro2050-50` | Desarrollador |
| **Luis** | `ccastro202050` | Desarrollador |

---

## 2. Acuerdo 1 — Carlos integra, y eso no es un privilegio

> 💬 **Paco:** *«¿Y por qué él? Los tres sabemos hacer merge.»*
>
> 💬 **Carlos:** *«No es que yo sepa más. Es que si los tres fusionamos, nadie
> revisa a nadie — cada uno aprueba lo suyo y entra directo. Que haya UNO
> significa que todo lo que llega a `main` lo miró alguien que no lo escribió.»*

**Queda acordado:**

| | |
|---|---|
| **Nadie trabaja en `main`.** Ni un commit directo | los tres |
| Cada quien tiene **su rama por versión**: `rama-<nombre>-v<N>` | los tres |
| Todo entra por **Pull Request** | los tres |
| **Solo Carlos fusiona**, y revisa antes: ¿compila? ¿cumple la spec? ¿respeta las capas? | Carlos |
| El **tag `vN`** lo pone Carlos, **cuando los criterios pasan** — no antes | Carlos |

> **Y el costo de ser integrador, dicho de frente:** Carlos va a acumular
> commits de fusión que **no son trabajo de contenido**. Al final su `shortlog`
> va a parecer más grande de lo que es. Para repartir mérito hay que descontarlas
> — ver [`CRONOGRAMA.md`](CRONOGRAMA.md) §1.

---

## 3. Acuerdo 2 — Chat primero, agente después: los dos, por todos

> 💬 **Luis:** *«¿Y si repartimos las herramientas? Uno con agente y los otros
> dos con chat.»*
>
> 💬 **Paco:** *«Entonces los dos terminamos el curso sin haber usado nunca un
> agente. Yo no voy a salir de aquí sabiendo la mitad.»*
>
> 💬 **Carlos:** *«Y hay algo más: en la v1 un agente casi no sirve. El agente
> gana cuando hay código que leer, y el primer día no hay nada. Entonces que
> empecemos todos con chat y cambiemos cuando el proyecto ya sea grande.»*

**Queda acordado:**

| | **v1 · v2 · v3** | **v4 · v5** |
|---|---|---|
| **Carlos** | IDE agéntico | IDE agéntico |
| **Paco** | **chat web** | **IDE agéntico** |
| **Luis** | **chat web** | **IDE agéntico** |

> **Por qué el corte va en la v4, y no antes ni después.** Un agente lee el
> repositorio y escribe solo; eso vale cuando **ya hay un repositorio que leer**.
> En la v1 el proyecto está vacío: el agente no tiene contexto que aprovechar y
> lo único que aporta es escribir sin que uno mire.
>
> **De la v4 en adelante es al revés:** hay cuatro capas, quince controladores y
> un front entero. Pedirle a un chat que entienda eso significa subirle treinta
> archivos en cada conversación. Ahí el agente deja de ser comodidad y pasa a ser
> la herramienta correcta.

> **Y lo que el equipo gana haciéndolo en este orden**, que es lo que de verdad
> justifica el acuerdo:
>
> | | |
> |---|---|
> | **Las tres primeras versiones, a mano** | Con chat hay que leer cada archivo antes de pegarlo. Es lento, y por eso se aprende |
> | **Las dos últimas, con agente** | Cuando ya saben qué debe salir, pueden juzgar lo que el agente escribe |
>
> **Un agente en manos de quien no sabe qué esperar no acelera: esconde.**
> Produce código que compila y que nadie revisó, y el error se descubre en la
> sustentación. Por eso el agente llega **después** del chat, no antes.

> **Carlos empieza con agente desde la v1 por su papel, no por privilegio:** es
> el que monta el esqueleto, y es el único que va a tener que leer el código de
> los otros dos en cada revisión de PR.

---

## 4. Acuerdo 3 — El reparto del código: por ARCHIVOS, no por tareas

> 💬 **Carlos:** *«Si los tres editamos el mismo archivo, cada fusión es un
> conflicto. Si cada uno tiene los suyos, Git une las ramas solo.»*

| | **Carlos** | **Paco** | **Luis** |
|---|---|---|---|
| **v1** | el montaje · `producto` · `usuario` | `persona` · `empresa` | `rol` · `ruta` |
| **v2** | `factura` | `cliente` · `vendedor` | las dos puentes · `usuario-con-roles` |
| **v3** | el token · la sesión | BCrypt · `[ExigePermiso]` | el menú por rol · la pantalla de permisos |
| **v4** | el tablero · 4 consultas | 3 consultas · la marca | 3 consultas · las pantallas |
| **v5** | la fábrica · el interruptor | 7 repositorios Postgres | 7 repositorios Postgres |

**Y tres reglas que salen de ahí:**

> **1. Nadie toca el archivo de otro. Ni para arreglárselo.** Si usted ve un
> error en el código de un compañero, se lo dice. No lo corrige en su rama.

> **2. Carlos va primero en cada versión, y solo.** No construye «su parte»:
> construye **el esqueleto y el molde** que los otros dos van a calcar. Si Paco
> empieza antes, su chat se inventa una estructura y al fusionar hay dos
> arquitecturas en el mismo proyecto — y eso no es un conflicto que Git resuelva.

> **3. Paco y Luis sí trabajan a la vez**, porque sus archivos no se tocan.

### Por qué el reparto quedó así, y no repartido por igual

| | |
|---|---|
| **Carlos lleva el montaje** | El que va a fusionar necesita conocer el esqueleto mejor que nadie |
| **Carlos lleva `factura`** | Es lo único que se opera por procedimientos y en una transacción |
| **Paco lleva lo que más se parece al molde** | Calcar bien es lo primero que hay que aprender |
| **Luis lleva `rol` y `ruta`** | Son las de llave `IDENTITY`: **las únicas que no se calcan igual**. Es la parte que exige entender, no copiar |

> **Que Luis termine con menos commits no significa que hiciera menos.** Contar
> commits mide actividad, no dificultad.

---

## 5. Acuerdo 4 — Quién escribe cada documento del spec kit

**Ésta fue la discusión más larga de la reunión**, y la que más valor tiene
escrita.

> 💬 **Paco:** *«¿El spec kit no lo escribe el que integra?»*
>
> 💬 **Luis:** *«Si lo escribe Carlos solo, Paco y yo vamos a construir contra un
> documento que no leímos. Y en la sustentación nos van a preguntar por qué el
> contrato dice lo que dice.»*
>
> 💬 **Carlos:** *«Entonces lo escribimos los tres. Yo me quedo con lo que no se
> puede repartir: la constitución y la lista de chequeo.»*

| Documento | Quién lo redacta | Por qué |
|---|---|---|
| `1_constitution.md` | **Carlos** | Rige **todas** las versiones. No es de una versión, es del proyecto |
| `9_checklist.md` | **Carlos** | Es **la compuerta**: la marca quien pone el tag, y la marca **una persona**, no una IA |
| `GUIA_IA<N>.md` (índice) | **Carlos** | Dice el reparto y el orden: es trabajo de integración |
| `2_spec.md` · `4_research.md` · `7_quickstart.md` | **Paco** | El QUÉ, las decisiones descartadas, y la prueba de humo |
| `3_plan.md` · `5_data_model.md` · `6_contracts.md` · `8_tasks.md` | **Luis** | El CÓMO, el modelo, el contrato exacto y el orden de construcción |
| `GUIA_IA<N>_CARLOS.md` | **Carlos** | |
| `GUIA_IA<N>_PACO.md` | **Paco** | **Cada quien escribe la suya.** Nadie sabe mejor que uno mismo qué necesita su herramienta |
| `GUIA_IA<N>_LUIS.md` | **Luis** | |

> **El criterio que ordena la tabla:** lo que **vale para todas las versiones** o
> es **una compuerta** se queda con el integrador. Lo demás **se reparte**, para
> que los tres hayan leído el spec kit **escribiéndolo**, que es la única forma de
> leerlo de verdad.

> **Y las guías de IA son el caso más claro.** La guía de Paco la escribe Paco:
> él es el que sabe qué archivos le toca subir al chat y dónde se le equivoca.
> Carlos no podría escribirla sin inventar.

---

## 6. Acuerdo 5 — Qué pasa cuando algo sale mal

| Situación | Qué se hace |
|---|---|
| **Un conflicto al fusionar** | Lo resuelve **el autor de la rama**, no el integrador. Él sabe qué quiso decir su código |
| **El PR no compila** | Carlos lo devuelve sin fusionar. No lo arregla él |
| **Alguien tocó un archivo ajeno** | Se revierte y se avisa. No se discute: es la regla que sostiene todo lo demás |
| **Una versión no pasa sus criterios** | **No se pone el tag.** Un tag sobre código que no cumple es una firma en falso |

---

## 7. Cómo se comprueba que este plan se cumplió

**Un plan que no se puede contrastar con el repositorio es una intención.** Éste
sí:

```powershell
# 1 · ¿Existen las ramas acordadas? Deben ser 15: tres por versión.
git branch -a

# 2 · ¿Quién fusionó? Las 15 fusiones deben ser de Carlos (§2).
git log --merges --format='%an' | Group-Object | Select-Object Count, Name

# 3 · ¿Se cumplió el reparto del spec kit (§5)?
git log -1 --format='%an' -- docs/spec_kit/1_constitution.md          # Carlos
git log -1 --format='%an' -- docs/spec_kit/versiones/v1_sin_fk/2_spec.md   # Paco
git log -1 --format='%an' -- docs/spec_kit/versiones/v1_sin_fk/6_contracts.md  # Luis

# 4 · ¿Cada quien escribió SU guía?
git log -1 --format='%an' -- docs/spec_kit/versiones/v1_sin_fk/GUIA_IA1_PACO.md  # Paco

# 5 · ¿El reparto del código (§4) se respetó?
git log -1 --format='%an' -- api_facturas/Controllers/FacturaController.cs  # Carlos
```

> **Si alguno de estos comandos responde otro nombre, el plan se incumplió —o
> este documento está viejo.** Las dos cosas son errores, y la forma de
> distinguirlas es preguntarle al equipo, no adivinar.

---

## 8. Firmas

| | Cuenta | Fecha |
|---|---|---|
| **Carlos** | `ccastro2050` | 8 de agosto de 2026 |
| **Paco** | `ccastro2050-50` | 8 de agosto de 2026 |
| **Luis** | `ccastro202050` | 8 de agosto de 2026 |

> **Este documento se modifica solo en reunión**, y cada cambio deja su fila en
> §9. Un reparto que cambia sin que los tres se enteren no es un reparto: es una
> sorpresa en la fusión.

---

## 9. Historial

| Versión | Fecha | Qué cambió |
|---|---|---|
| **1.1** | **1.º de octubre de 2026** | **Acuerdo 2 modificado en reunión.** La versión 1.0 repartía las herramientas de forma fija —Carlos agéntico, Paco y Luis chat— para las cinco versiones. Paco objetó que así él y Luis terminarían el curso sin haber usado nunca un agente. Queda: **chat en la v1, v2 y v3; agente de la v4 en adelante, los tres** |
| 1.0 | 8 de agosto de 2026 | Reunión de arranque. Los cinco acuerdos |

---

| Qué | Dónde |
|---|---|
| La norma que este plan obedece | [`0_METODOLOGIA.md`](../../ProyectosDeAula/docs/0_METODOLOGIA.md) §4.1 |
| Por qué se trabaja con ramas | [`CONCEPTOS_RAMAS_Y_COLABORACION.md`](../conceptos/CONCEPTOS_RAMAS_Y_COLABORACION.md) |
| Lo que de verdad pasó, contado de `git log` | [`CRONOGRAMA.md`](CRONOGRAMA.md) |
| La guía de cada quien, por versión | el `GUIA_IA<N>.md` de cada carpeta |
