# Política de errores — Facturación (`bdfacturas`)

> **Este documento está partido en dos, y aquí está cuál es cuál.**
>
> | Documento | Qué trae | A qué plantillas alimenta |
> |---|---|---|
> | [`CODIGOS_DE_ESTADO.md`](CODIGOS_DE_ESTADO.md) | La **tabla completa** de códigos con quién decide cada uno · las tres parejas que se confunden · la forma del cuerpo · lo que el `detalle` no puede traer | el `1_constitution`, el `6_contracts` y el `7_quickstart` |
> | [`ERRORES_DEL_MOTOR.md`](ERRORES_DEL_MOTOR.md) | El caso del **login** · cómo se traduce un `THROW` a HTTP · por qué la traducción vive en el repositorio | el `6_contracts` de la v2 en adelante, y el `7_quickstart` |
>
> **Por qué está partido.** Porque tiene **tres lectores**, y no todos necesitan
> lo mismo: la constitución necesita la tabla de códigos y nada más; la
> traducción de los errores del motor solo aparece cuando llegan las llaves
> foráneas. Ver
> [`METODOLOGIA_CONSTRUCCION_DOMINIOS.md`](METODOLOGIA_CONSTRUCCION_DOMINIOS.md).
>
> **Sigue siendo una política:** rige en todas las versiones y no se renegocia en
> cada `6_contracts.md`.
>
> Versión 2.0 · 22 de octubre de 2026.

---

## En una línea

> Diez códigos, y cada uno con **quién lo decide**: el enrutador, ASP.NET, el
> servicio, el filtro de permiso o el `catch` final.

| Si busca… | Vaya a |
|---|---|
| Qué código responde cada fallo | [`CODIGOS_DE_ESTADO.md`](CODIGOS_DE_ESTADO.md) §1 |
| La diferencia entre 401 y 403, 422 y 400, 409 y 500 | [`CODIGOS_DE_ESTADO.md`](CODIGOS_DE_ESTADO.md) §2 |
| La forma del cuerpo de un error | [`CODIGOS_DE_ESTADO.md`](CODIGOS_DE_ESTADO.md) §3 |
| Por qué el login no dice si el correo existe | [`ERRORES_DEL_MOTOR.md`](ERRORES_DEL_MOTOR.md) §1 |
| Cómo se traduce un `THROW 50010` | [`ERRORES_DEL_MOTOR.md`](ERRORES_DEL_MOTOR.md) §2 |
