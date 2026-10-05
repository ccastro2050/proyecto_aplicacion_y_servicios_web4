# Versión 1 — Carlos · camino B (IDE agéntico)

> **Su parte:** el **montaje completo** del proyecto, más los recursos
> **`producto`** y **`usuario`**.
>
> **Su herramienta:** un IDE agéntico — Antigravity, Cursor, Claude Code. El
> agente lee los documentos del repositorio y escribe los archivos él mismo.
>
> **Y usted va primero, solo.** Paco y Luis no pueden empezar hasta que su
> molde exista. Ver [`GUIA_IA1.md`](GUIA_IA1.md) §3.

---

## 1. Por qué le tocó a usted, y qué significa

Usted es el **integrador**. Eso no es un permiso técnico: es un acuerdo del
equipo, y trae dos obligaciones.

| | |
|---|---|
| **Construye el esqueleto** | Docker, `Program.cs`, las carpetas de capas, Swagger, el diagnóstico. Nada de eso es «su recurso»: es **de todos** |
| **Construye el MOLDE** | `producto` completo — modelo, tres peticiones, repositorio, servicio, controlador y pantalla. Paco y Luis van a **calcarlo** |
| **Y después integra** | Revisa los dos PR, fusiona, y pone el tag |

> **Su rebanada de `producto` no es «su tarea»: es la plantilla del proyecto.**
> Si queda con una capa saltada o un nombre inconsistente, los otros dos van a
> copiar ese error dos veces, y el equipo va a entregar la arquitectura mal
> hecha tres veces en vez de una.
>
> **Por eso usted revisa su propio código antes que nadie**, y con más cuidado
> del que le pondría a su parte si fuera solo suya.

---

## 2. Antes de abrir el agente

```powershell
# 1 · PÁRESE EN main Y TRAIGA LO ÚLTIMO.
#     QUÉ HACE: pone su carpeta en el estado que tiene el repositorio remoto.
#     PARA QUÉ: para ramificar desde ahí y no desde algo viejo.
git switch main
git pull origin main

# 2 · SU RAMA PARA ESTA VERSIÓN.
#     QUÉ HACE: crea el puntero y lo deja parado en él.
#     EFECTO: sus archivos NO cambian — la rama nueva apunta al mismo commit.
git switch -c rama-carlos-v1

# 3 · SU IDENTIDAD. Parado en LA CARPETA DEL PROYECTO —la del git clone—,
#     porque ahi es donde Git guarda con que nombre firma (.git\config).
git config user.name "ccastro2050"
git config user.email "su-correo-de-github"

# 4 · COMPRUEBELO SIEMPRE, aunque crea que ya estaba. Si sale vacio o el
#     nombre de un compañero, pare: despues no se arregla sin reescribir
#     el historial.
git config user.name
git config user.email
```

> **El paso 4 no es paranoia.** Un commit firmado con el correo equivocado es
> trabajo que usted hizo y que no se le va a contar, y eso **no se arregla
> después** sin reescribir el historial.
>
**¿Y si usted trabaja siempre en su propio computador?** Entonces
> `git config --global` le serviría igual y es más cómodo. **No está mal.** Lo
> que está mal es **confiar en ella sin mirarla**: el día que use el PC de la
> universidad, la configuración global es la del que se sentó antes, y sus
> commits salen firmados con el nombre de otro **sin un solo aviso**.
>
> **Lo que protege no es dónde esté la configuración: es comprobarla** antes del
> primer commit de cada carpeta. Los tres casos, en
> [`GUIA_IA1.md`](GUIA_IA1.md) §5.

---

## 3. Qué puede tocar el agente, y qué no

Esto se lo dice en el prompt, y conviene que usted lo tenga claro primero:

| Puede escribir | NO puede tocar |
|---|---|
| `api_facturas/` — toda la API | `docs/` — **es solo lectura** |
| `front_blazor/` — las pantallas | `db/bdfacturas.sql` — **viene dado** |
| `docker-compose.yml` | Los recursos de Paco y Luis (§4) |
| `.gitignore`, `README.md` | |

> **`db/bdfacturas.sql` viene dado y se usa tal cual** — Artículo 5 de la
> constitución. El agente **no escribe SQL de creación de tablas**: SQL Server lo
> ejecuta solo la primera vez que levanta. Si el agente propone generar el
> esquema, está contradiciendo la constitución y hay que pararlo.

> **Y una advertencia que vale oro con un agente:** tiene permiso para escribir
> archivos **sin preguntar**. Un agente al que no se le acota el alcance es
> capaz de reorganizarle el proyecto entero con la mejor intención. Por eso el
> prompt empieza pidiéndole que **lea y resuma antes de tocar nada**.

---

## 4. Lo que usted construye, y lo que deja para los otros

**Usted construye DOS recursos:**

```
producto   codigo (texto, PK) · nombre · stock · valorunitario
usuario    email  (texto, PK) · contrasena
```

**Y NO construye estos cuatro** — son de Paco y de Luis:

```
persona · empresa        ← Paco
rol     · ruta           ← Luis
```

> **Si el agente los construye «de una vez, que ya está en el spec», bórrelos
> antes de hacer commit.** No es desperdicio: es que si usted los sube, Paco y
> Luis se quedan sin su parte de la entrega — y la sustentación es individual.
> La spec describe la versión **completa**; su rama entrega **su tercio**.

### Dos cosas de `usuario` que hay que saber

| | |
|---|---|
| La contraseña va **en texto plano** | En la v1, sí. El cifrado es la **v3**, y adelantarlo viola YAGNI |
| `email` es la llave primaria | No hay un `id` aparte: el correo ya identifica a la persona |

---

## 5. El prompt (cópielo tal cual)

```
Construye el MONTAJE y DOS RECURSOS de la VERSIÓN 1 de este proyecto,
partiendo de cero. Trabajo en equipo: otros dos compañeros van a construir
los otros cuatro recursos calcando lo que tú hagas, así que lo tuyo es
también la plantilla del proyecto.

PRIMERO lee, en este orden, los 7 documentos que están bajo docs/spec_kit/
(1_constitution.md en la raíz; los demás en versiones/v1_sin_fk/):
1_constitution, 2_spec, 3_plan, 4_research, 5_data_model, 6_contracts,
7_quickstart y 8_tasks. Después resume en máximo 10 líneas qué vas a
construir y ESPERA MI CONFIRMACIÓN antes de escribir un solo archivo.

ALCANCE — esto es lo único que construyes:

  1. El MONTAJE completo:
     · docker-compose.yml con SQL Server + la API, de modo que
       `docker compose up -d --build` deje todo funcionando (Artículo 4).
     · El proyecto de API con la estructura de carpetas de 3_plan.md.
     · Swagger en /swagger y el endpoint / de diagnóstico.
     · El proyecto del front, con su menú.

  2. El recurso `producto`, COMPLETO y como MOLDE:
     producto = codigo (texto, PK) · nombre · stock · valorunitario
     Con sus seis capas: modelo, las TRES peticiones por verbo
     (Crear, Reemplazo, Actualizar), interfaz + repositorio,
     interfaz + servicio, controlador, y su pantalla.

  3. El recurso `usuario`, calcado del mismo molde:
     usuario = email (texto, PK) · contrasena
     La contraseña va EN TEXTO PLANO en esta versión. No la cifres:
     el cifrado es la v3 y adelantarlo viola el Artículo 1 (YAGNI).

NO CONSTRUYAS persona, empresa, rol ni ruta. Los hacen mis compañeros.
Aunque 2_spec.md los mencione, mi parte son los dos de arriba. Si los
construyes, los voy a borrar.

REGLAS QUE NO SE NEGOCIAN:

  · docs/ es SOLO LECTURA. No modifiques ningún documento.
  · La base de datos YA VIENE DADA en db/bdfacturas.sql. Úsala tal cual
    para montar SQL Server. NO escribas ni modifiques SQL de creación de
    tablas (Artículo 5).
  · Las TRES CAPAS desde el primer archivo: controlador → servicio →
    repositorio, y cada una depende de una INTERFAZ, no de una clase.
    El controlador no escribe SQL. El servicio no nombra nada de HTTP:
    ni StatusCode, ni NotFound, ni IActionResult. El repositorio no
    decide códigos de estado.
  · SIN ORM de entidades. El SQL se escribe a mano y SIEMPRE
    parametrizado. El ejecutor es Dapper.
  · TODO EN ESPAÑOL: nombres, comentarios y mensajes.
  · El front NO habla con la base de datos. Solo con la API por HTTP.

COMENTA TODO LO QUE ESCRIBAS, en español. Cada archivo empieza con un
bloque que dice QUÉ ES y QUÉ PAPEL cumple en la arquitectura. Cada método
no evidente lleva su comentario. Y los comentarios dicen POR QUÉ está
escrito así, no QUÉ hace la línea: "// suma uno al contador" no le sirve a
nadie; "// el límite es 1000 porque es el ancho de la columna" sí.

ORDEN DE TRABAJO: monta primero, luego `producto` entero —de la petición
al controlador—, y solo cuando `producto` responda bien, calca `usuario`.
No empieces los dos a la vez.

Al terminar cada paso dime qué archivos creaste y espera antes de seguir.
```

> **Y ahora la parte que el prompt no puede hacer por usted: LEER esos
> comentarios.** La IA comenta lo que **cree** que hizo, y no siempre coincide
> con lo que hizo. **Un comentario equivocado es peor que ninguno**, porque el
> siguiente que lo lea le va a creer.
>
> | | |
> |---|---|
> | El comentario **como producto** | sirve para quien llegue en seis meses |
> | El comentario **como ejercicio** | **revisarlo lo obliga a entender.** Ahí está el valor para quien aprende |
> | El comentario **como evidencia** | vale poco: se puede generar sin entender nada |
>
> **Por eso la interpretabilidad se califica HABLANDO.** Un comentario se puede
> recitar; una respuesta a *«¿y si cambiamos esto?»* no. Si usted no puede juzgar
> si un comentario es **cierto**, no entendió el código — y ésa es exactamente la
> señal que hay que buscar mientras revisa.


---

## 6. Cómo supervisar al agente

**Un agente no se lanza y se vuelve en una hora.** Estos son los tres momentos
donde hay que estar:

| Momento | Qué mirar |
|---|---|
| **El resumen inicial** | ¿Entendió que son DOS recursos y no seis? ¿Nombró las tres capas? Si el resumen está mal, el código va a estar mal |
| **Cuando termina `producto`** | **Pare ahí y revise.** Es el molde: lo que esté torcido aquí se va a copiar cinco veces |
| **Antes del commit** | Que no haya construido lo de Paco y Luis |

### La revisión del molde, en cuatro comprobaciones

```powershell
# 1 · El servicio NO nombra HTTP. Tiene que dar 0.
Select-String -Path api_facturas\Servicios\*.cs `
  -Pattern 'StatusCode|NotFound|BadRequest|IActionResult' |
  Measure-Object | Select-Object Count

# 2 · El controlador NO abre conexiones. Tiene que dar 0.
Select-String -Path api_facturas\Controllers\*.cs -Pattern 'SqlConnection' |
  Measure-Object | Select-Object Count

# 3 · El front NO toca la base. Tiene que dar 0.
Select-String -Path front_blazor\*.cs,front_blazor\**\*.razor `
  -Pattern 'SqlConnection' | Measure-Object | Select-Object Count

# 4 · Las tres peticiones por verbo EXISTEN.
Get-ChildItem api_facturas\Peticiones\Producto*.cs
```

> **Si la 1 o la 2 dan algo distinto de 0, el molde está mal y hay que
> arreglarlo ANTES de que Paco y Luis lo copien.** Ese es el momento más barato
> de todo el proyecto para corregir la arquitectura.

---

## 7. Comprobar que funciona, antes del PR

```powershell
# 1 · LEVANTAR EL SISTEMA ENTERO con un solo comando (Artículo 4).
#     La primera vez tarda: descarga SQL Server y siembra la base.
docker compose up -d --build

# 2 · ¿La API responde? Debe decir la versión.
Invoke-RestMethod http://localhost:8035/

# 3 · ¿Swagger abre? Si da error, hay un controlador mal declarado.
Start-Process http://localhost:8035/swagger

# 4 · El CRUD completo de producto. Cree SU propia fila y bórrela al final:
#     nunca pruebe sobre las filas sembradas.
$nuevo = @{ codigo='PRTEST'; nombre='Prueba'; stock=5; valorunitario=1000 }
Invoke-RestMethod http://localhost:8035/api/producto -Method Post `
  -ContentType 'application/json' -Body ($nuevo | ConvertTo-Json)

# 5 · LA COMPROBACIÓN QUE DE VERDAD ENSEÑA: el mismo cuerpo, dos verbos.
#     El PUT debe dar 422 —faltan campos— y el PATCH 200.
Invoke-RestMethod http://localhost:8035/api/producto/PRTEST -Method Put `
  -ContentType 'application/json' -Body '{"nombre":"Cambiado"}'

Invoke-RestMethod http://localhost:8035/api/producto/PRTEST -Method Patch `
  -ContentType 'application/json' -Body '{"nombre":"Cambiado"}'

# 6 · Borre su fila de prueba.
Invoke-RestMethod http://localhost:8035/api/producto/PRTEST -Method Delete
```

> **El paso 5 es el que hay que poder explicar en la sustentación.** Con el
> **mismo cuerpo**, el PUT falla y el PATCH funciona — porque son **dos clases
> distintas** las que validan: `ProductoReemplazo` tiene `[Required]` en todos
> los campos y `ProductoActualizar` no tiene ninguno. Si el agente le entregó una
> sola clase para los dos verbos, el molde está mal.

> **Y una advertencia de laboratorio:** pruebe **solo sobre filas que usted creó,
> y bórrelas**. Las filas sembradas son material de clase; si alguien las
> modifica, el siguiente que levante el proyecto encuentra otra cosa.

---

## 8. Subir y abrir el PR

```powershell
# 1 · Revise QUÉ va a subir. Si aparecen bin/ u obj/, falta el .gitignore.
git status

# 2 · Commits PEQUEÑOS y con mensaje de verdad. Uno por pieza, no uno al final.
git add docker-compose.yml .gitignore
git commit -m "chore: el sistema levanta con un solo comando"

git add api_facturas/Modelos/Producto.cs api_facturas/Peticiones/Producto*.cs
git commit -m "feat: producto, el modelo y sus tres peticiones por verbo"
#    ... y así, capa por capa.

# 3 · Suba SU rama. El -u solo la primera vez.
git push -u origin rama-carlos-v1

# 4 · Abra el Pull Request en GitHub, de su rama hacia main.
#     Describa QUÉ construyó y contra qué criterio del 2_spec.
```

> **«avances» no es un mensaje de commit.** El formato es `tipo: descripción` —
> `feat`, `fix`, `docs`, `chore`, `refactor`— y la descripción dice qué cambió.
> Su rama con un solo commit gigante es una sustentación sin evidencia.

**Y ahora avísele a Paco y a Luis**, porque hasta que su PR esté fusionado ellos
no tienen molde que calcar.

---

## 9. Su segundo turno: integrar

Cuando los dos PR estén arriba:

```powershell
# 1 · Revise cada PR en GitHub: ¿compila? ¿cumple la spec? ¿respeta las capas?
#     Las cuatro comprobaciones de §6, ahora sobre lo que ellos entregaron.

# 2 · Apruebe y fusione desde GitHub. SOLO usted.

# 3 · Traiga el main ya fusionado y compruebe que TODO corre junto.
git switch main
git pull origin main
docker compose up -d --build

# 4 · Con los criterios cumplidos y el 9_checklist marcado POR UNA PERSONA:
git tag -a v1 -m "Version 1: los seis recursos sin clave foranea"
git push origin v1
```

> **Revisar un PR no es mirar si compila.** Es preguntarse si el código **cumple
> la spec** y si **respeta las capas**. Compilar es el requisito más bajo, no el
> más alto — y si usted aprueba algo que no revisó, el tag que ponga después es
> una firma en falso.
