# Versión 4 con IA — índice de las tres guías

> **La versión del aplicativo**, y la primera en la que **los tres trabajan con
> agente**.
>
> Hasta aquí el sistema sabía *guardar* y *proteger*. Lo que no sabía era
> **responder preguntas del negocio**.

---

## 1. Quién hace qué — y todos con la misma herramienta

| | Su guía | Su camino | Lo que le toca |
|---|---|---|---|
| **Carlos** | [`GUIA_IA4_CARLOS.md`](GUIA_IA4_CARLOS.md) | **B · agéntico** | El tablero + las 4 consultas de ventas |
| **Paco** | [`GUIA_IA4_PACO.md`](GUIA_IA4_PACO.md) | **B · agéntico** ← cambia | 3 consultas + **el manual de marca** |
| **Luis** | [`GUIA_IA4_LUIS.md`](GUIA_IA4_LUIS.md) | **B · agéntico** ← cambia | 3 consultas + las pantallas |

> **Paco y Luis cambian de herramienta aquí**, y está acordado en
> [`PLAN_DE_TRABAJO.md`](../../../dominio/PLAN_DE_TRABAJO.md) §3. La razón es
> concreta: **un agente lee el repositorio**, y a estas alturas hay cuatro
> capas, quince controladores y un front entero. Subirle treinta archivos a un
> chat en cada conversación dejó de tener sentido.

> **Y el orden importa:** llegan al agente **después** de tres versiones a mano.
> Con chat hay que leer cada archivo antes de pegarlo — es lento, y por eso se
> aprende. Ahora ya saben qué debe salir, **que es lo único que permite juzgar
> lo que un agente escribe** en vez de aceptarlo.

---

## 2. Quién escribe cada documento de ESTA carpeta

| Documento | Lo redacta |
|---|---|
| [`2_spec.md`](2_spec.md) · [`4_research.md`](4_research.md) · [`7_quickstart.md`](7_quickstart.md) | **Paco** |
| [`3_plan.md`](3_plan.md) · [`5_data_model.md`](5_data_model.md) · [`6_contracts.md`](6_contracts.md) · [`8_tasks.md`](8_tasks.md) | **Luis** |
| [`9_checklist.md`](9_checklist.md) · este índice | **Carlos** |
| `GUIA_IA4_<NOMBRE>` | **cada quien la suya** |

---

## 3. Qué define esta versión: lo que un CRUD no puede responder

Un CRUD responde *«dame las filas de `factura`»*. **Ninguna de las diez
consultas es eso:**

```
ventas-por-vendedor      ventas-por-cliente       ventas-por-producto
ventas-por-empresa       ticket-por-vendedor      anulaciones-por-cliente
credito-contra-consumo   productos-sin-vender     alcance-de-usuarios
interfaces-sin-usuarios
```

> Son `JOIN` con `GROUP BY`, y **no caben en `GET /api/{tabla}`**. Por eso la v4
> existe y no es «la v2 con más pantallas».

| El reparto | |
|---|---|
| **Carlos** | `ventas-por-vendedor` · `ventas-por-cliente` · `ventas-por-producto` · `ventas-por-empresa` |
| **Paco** | `ticket-por-vendedor` · `anulaciones-por-cliente` · `credito-contra-consumo` |
| **Luis** | `productos-sin-vender` · `alcance-de-usuarios` · `interfaces-sin-usuarios` |

> **Las tres de Luis no son de ventas: son del propio sistema.** Preguntan
> *«¿qué producto no se ha vendido nunca?»*, *«¿quién alcanza qué?»* y *«¿qué
> pantalla no alcanza nadie?»*. La última es la más útil para administrar: **una
> interfaz que ningún rol alcanza está construida y nadie la ve**.

---

## 4. Lo que los tres tienen que saber

### El sobre de las consultas NO es el del CRUD

```json
CRUD       { "tabla": "...", "limite": 50, "total": 8, "datos": [...] }
Consultas  { "consulta": "...",            "total": 3, "datos": [...] }
```

> **No tiene `limite`, y el nombre del primer campo cambia.** Una consulta no se
> pagina: devuelve el resultado del agrupamiento, que ya viene resumido. Si el
> front espera `tabla`, la pantalla sale vacía **sin un solo error**.

### Una consulta que devuelve CERO filas no está rota

Con los datos sembrados, **dos de las diez responden vacío**:

| | Y eso significa |
|---|---|
| `productos-sin-vender` → `total: 0` | **todos** los productos se han vendido alguna vez |
| `interfaces-sin-usuarios` → `total: 0` | **todas** las pantallas las alcanza algún rol |

> **Son el resultado correcto, y es la lección de la versión.** «Vacío» y «roto»
> se ven **exactamente igual** en una pantalla. La única forma de distinguirlos
> es **provocar el caso**: cree un producto nuevo, no lo venda, y vuelva a
> consultar. Si sigue en cero, **ahora sí** está rota.
>
> **Una consulta que nunca se vio devolver algo no está probada.**

### Las consultas también exigen permiso

No porque sean peligrosas de escribir, sino porque **contestan preguntas del
negocio**: cuánto vendió cada quien no lo puede ver cualquiera.

---

## 5. Lo que cambia al trabajar con agente

**Para Paco y Luis esto es nuevo.** Lo esencial, y está en sus guías:

| Con chat | Con agente |
|---|---|
| Usted pega cada archivo | **Él los escribe, sin preguntar** |
| Usted ve cada línea antes de que entre | Usted ve **el resultado** |
| El error aparece donde usted pegó | El error puede estar en un archivo que usted no abrió |

> **La regla que reemplaza a «pegar de a un archivo»:** el prompt empieza
> pidiéndole al agente que **lea, resuma y espere confirmación** antes de tocar
> nada. Si el resumen está mal, el código va a estar mal — y es mucho más barato
> descubrirlo ahí.
>
> **Y la segunda: acote qué carpetas puede escribir.** Un agente sin alcance
> definido es capaz de reorganizarle el proyecto entero con la mejor intención.

---

## 6. Lo que vale para los tres

**Comentar, identidad, ramas** — igual que siempre:

```powershell
git switch main ; git pull origin main
git switch -c rama-<nombre>-v4
git config user.name "su-usuario" ; git config user.email "su-correo"
git config user.name ; git config user.email
```

**La interpretabilidad se califica** hablando y en persona. Con agente eso pesa
más: es código que usted no tecleó.

---

## 7. Cuándo está terminada

| Qué | Cómo |
|---|---|
| Las diez consultas responden | `/api/consultas/*` |
| Las dos que dan vacío **se probaron provocando el caso** | §4 |
| El tablero muestra indicadores, no listados | `http://localhost:8099/` |
| **Ni un color escrito a mano** fuera de `marca.css` | Buscar `#rrggbb` |
| La interfaz no habla en jerga | Recorrer las pantallas |
| **La regresión de v1, v2 y v3** | Todo lo anterior, con token |

```powershell
git tag -a v4 -m "Version 4: el aplicativo"
git push origin v4
```
