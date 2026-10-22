# Arquitectura — Facturación (`bdfacturas`)

> **Este documento está partido en dos, y aquí está cuál es cuál.**
>
> | Documento | Qué trae | A qué plantilla alimenta |
> |---|---|---|
> | [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) | Las tres capas y **qué tiene prohibido cada una** · dónde vive cada regla · la fábrica · lo que esta arquitectura **no tiene** | el `1_constitution` |
> | [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md) | Los tres procesos y sus puertos · **por qué esto no es un monolito** ni microservicios · three-tier · Blazor Server y lo que implica | el `3_plan` |
>
> **Por qué está partido.** Porque tiene **dos lectores que necesitan cosas
> distintas**: la constitución pide las capas y las prohibiciones; el plan pide
> los procesos y el despliegue. Mandarle a cada conversación el documento
> completo es mandarle la mitad que no usa — y esa mitad compite por la
> atención con la que sí. Ver
> [`DOMINIO_PARA_GENERAR.md`](../../DOMINIO_PARA_GENERAR.md).
>
> Versión 2.0 · 22 de octubre de 2026.

---

## En una línea

> Dos unidades desplegables —la API y el front—, tres capas con interfaces
> dentro de la API, y **las reglas del negocio en la base de datos**.

| Si busca… | Vaya a |
|---|---|
| Qué puede y qué no puede hacer cada capa | [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §1 |
| Dónde va una validación | [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §3 |
| Por qué no hay un `Repositorio<T>` genérico | [`ARQUITECTURA_CAPAS.md`](ARQUITECTURA_CAPAS.md) §6 |
| Si esto es un monolito | [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md) §2 |
| Por qué el front pierde el borrador con F5 | [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md) §3 |
| Los puertos | [`ARQUITECTURA_PROCESOS.md`](ARQUITECTURA_PROCESOS.md) §1 |

**Los conceptos detrás:**
[`SOLID_CAPAS_PATRONES.md`](../conceptos/SOLID_CAPAS_PATRONES.md) y
[`FLUJO_DE_UNA_PETICION.md`](../conceptos/FLUJO_DE_UNA_PETICION.md).
