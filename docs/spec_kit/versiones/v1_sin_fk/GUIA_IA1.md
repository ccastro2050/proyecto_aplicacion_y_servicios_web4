# Versión 1 con IA — índice de las tres guías

> **Esta versión la construyen TRES personas, y cada una tiene su propia guía.**
> No busque aquí el prompt: aquí está el reparto y el orden. El prompt suyo está
> en la guía que lleva su nombre.

---

## 1. Quién hace qué, y con qué herramienta

| | Su guía | Su camino | Lo que le toca en la v1 |
|---|---|---|---|
| **Carlos** | [`GUIA_IA1_CARLOS.md`](GUIA_IA1_CARLOS.md) | **B · IDE agéntico** | El montaje completo + `producto` y `usuario` |
| **Paco** | [`GUIA_IA1_PACO.md`](GUIA_IA1_PACO.md) | **A · Chat web** | `persona` y `empresa` |
| **Luis** | [`GUIA_IA1_LUIS.md`](GUIA_IA1_LUIS.md) | **A · Chat web** | `rol` y `ruta` |

> **Esto cambia en la v4: ahí los tres pasan a trabajar con agente.** El acuerdo
> está en [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3, y la
> razón es concreta: **un agente lee el repositorio, y en la v1 no hay nada que
> leer.**
>
> | v1 · v2 · v3 | v4 · v5 |
> |---|---|
> | Paco y Luis con **chat** | los tres con **agente** |
>
> **Las tres primeras a mano, y por eso se aprenden.** Con chat hay que leer cada
> archivo antes de pegarlo. Cuando el proyecto ya tiene cuatro capas y quince
> controladores, subirle treinta archivos a un chat deja de tener sentido — y
> para entonces los tres ya saben qué debe salir, que es lo único que permite
> juzgar lo que un agente escribe.
>
> **Carlos empieza con agente desde la v1 por su papel**, no por privilegio: es
> quien monta el esqueleto y quien va a leer el código de los otros dos en cada
> revisión.

---

## 2. El orden NO es negociable

```
    1 · CARLOS           2 · PACO y LUIS, a la vez        3 · CARLOS
    el montaje     →     sus recursos sobre el molde  →   integra y pone el tag
```

> **Por qué Carlos va primero, y solo.** Él no construye «su parte»: construye
> **el esqueleto del que cuelgan las otras dos**. El `docker-compose.yml`, el
> `Program.cs`, las carpetas de capas, Swagger — y una rebanada vertical
> completa, `producto`, que es **el molde** que Paco y Luis van a calcar.
>
> **Si Paco empieza antes de que exista ese molde**, su chat se va a inventar
> una estructura de carpetas, y al fusionar habrá dos arquitecturas distintas en
> el mismo proyecto. Eso no es un conflicto que Git pueda resolver: es un
> proyecto que hay que rehacer.

**Paco y Luis sí trabajan a la vez**, porque sus archivos no se tocan.

---

## 3. Por qué el reparto quedó así

| | |
|---|---|
| **Carlos lleva el montaje** porque es el integrador | El que va a fusionar necesita conocer el esqueleto mejor que nadie |
| **Carlos lleva `usuario`** | Es la tabla con la contraseña. En la v1 se guarda **en texto plano**, y eso cambia en la v3 — conviene que la toque quien va a hacer ese cambio |
| **Paco lleva `persona` y `empresa`** | Son las dos más parecidas al molde: calcar bien es lo que hay que aprender primero |
| **Luis lleva `rol` y `ruta`** | Son las dos **con llave IDENTITY**: la llave la pone la base. Son las únicas que **no se calcan igual**, y su guía dice en qué |

> **Nadie toca el archivo de otro.** Ni para arreglarle algo. Si usted ve un
> error en el recurso de un compañero, se lo dice — no lo corrige en su rama.
> Repartir por archivos es lo único que hace que Git una las tres ramas solo.

---

## 4. Lo que vale para los tres

### La IA tiene que COMENTAR lo que escribe

Va en los tres prompts, y se exige en los tres:

> Comenta todo el código que generes, en español. Cada archivo empieza con un
> bloque que dice QUÉ ES y QUÉ PAPEL cumple en la arquitectura. Cada método no
> evidente lleva su comentario. Y los comentarios dicen **por qué** está escrito
> así, no **qué** hace la línea.

| | |
|---|---|
| **Usted va a tener que explicarlo** | Desde la v2 la **interpretabilidad se califica**, hablando y en persona |
| **Lo generado se olvida más rápido que lo escrito** | El comentario es lo que queda cuando el recuerdo de la conversación se fue |
| **Y la IA a veces comenta mal** | Comenta lo que *cree* que hace el código. Léalos: un comentario equivocado es peor que ninguno, porque el siguiente le va a creer |

### Su identidad: en QUÉ carpeta, y por qué

**«La carpeta» es la del proyecto**: la que le queda al hacer `git clone`. Ahí
dentro Git guarda un archivo `.git/config`, y de ahí saca el nombre con el que
firma cada commit.

```powershell
# QUÉ HACE: firma SUS commits con su nombre y su correo de GitHub.
# PARA QUÉ: para que el trabajo se le acredite a USTED.
# DÓNDE: párese en la carpeta del proyecto antes de correrlo.
# EFECTO: escribe dos líneas en .git\config de ESA carpeta.
git config user.name "su-usuario-de-github"
git config user.email "su-correo-de-github"
```

> **«¿Y si yo trabajo solo en mi propio computador?»** Entonces `git config
> --global` le funcionaría igual, y es más cómodo: se configura una vez y vale
> para todos sus repositorios. **No está mal usarlo.**
>
> **Lo que sí está mal es confiar en él sin comprobar**, y por tres casos que
> ocurren de verdad:
>
> | Caso | Qué pasa |
> |---|---|
> | **El PC de la universidad** | La configuración global es la del que se sentó antes. Sus commits salen con el nombre de otro, **sin un solo aviso** |
> | **Casa y universidad** | Son dos máquinas. Si solo configuró una, la mitad de sus commits están mal firmados |
> | **Dos cuentas** (la personal y la del curso) | La global las firma todas igual |

**Por eso la línea que no se salta es ésta, y va antes del primer commit:**

```powershell
# COMPRUEBE con qué nombre va a firmar. Si sale vacío o el de un compañero,
# PARE y configúrelo: después no se arregla sin reescribir el historial.
git config user.name
git config user.email
```

> **Lo que protege no es dónde esté la configuración: es mirarla.**

> **Y ojo, que son DOS cosas distintas.** La identidad decide quién **firma**; la
> credencial decide quién **sube**. En un computador compartido, el almacén de
> Windows guarda **una credencial por servidor**, así que el `push` puede irse
> con la cuenta del que estuvo antes **aunque la firma esté bien**. Arreglar una
> no arregla la otra. Ver
> [`CONCEPTOS_RAMAS_Y_COLABORACION.md`](../../../conceptos/CONCEPTOS_RAMAS_Y_COLABORACION.md) §8.

### Nadie trabaja en `main`

Cada uno en su rama — `rama-carlos`, `rama-paco`, `rama-luis` — y todo entra por
**Pull Request**. Solo Carlos fusiona.

---

## 5. Lo que NINGUNO sube a la IA

| No se sube | Por qué |
|---|---|
| `9_checklist.md` | Es **su** compuerta, no la de la IA: revisa la especificación antes de abrirla |
| `0_mapa_versiones.md` | Le revelaría lo que viene, y **la v1 no anticipa** (Artículo 1, YAGNI) |
| `db/bdfacturas.sql` | **Viene dado** y se usa tal cual. La IA no genera SQL de creación |

---

## 6. Cuándo está terminada la versión

Cuando los tres PR están fusionados, **el `9_checklist.md` lo marca una persona**
—no la IA— y el sistema pasa sus criterios de aceptación. Ahí Carlos pone el tag:

```powershell
git tag -a v1 -m "Version 1: los seis recursos sin clave foranea"
git push origin v1
```

> **El tag va DESPUÉS de comprobar, no antes.** Un tag sobre código que no
> cumple es una firma en falso.
