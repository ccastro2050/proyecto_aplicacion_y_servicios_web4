<!--
PLANTILLA — 1_constitution.md
En Spec Kit oficial: .specify/memory/constitution.md  ·  comando /speckit.constitution
Fase: ninguna. Rige TODAS las versiones y se escribe UNA VEZ.
Dónde va: docs/spec_kit/1_constitution.md  — FUERA de versiones/
Sale de: ARQUITECTURA.md + REQUISITOS_NO_FUNCIONALES.md
Un ejemplo lleno: ../docs/spec_kit/1_constitution.md
-->

# Constitución del proyecto

> **Documento permanente.** Estas reglas rigen TODAS las versiones del
> curso. Cada versión tiene además su propia especificación en
> [versiones/](versiones/0_mapa_versiones.md); ante conflicto, la
> constitución gana.

---

<!-- OBLIGATORIO. Son los artículos. Ocho es lo que cabe sin que nadie deje de
     leerlos; si le salen quince, casi siempre dos se pueden juntar.
     CADA ARTÍCULO LLEVA SU RAZÓN. Un artículo que solo manda se incumple en
     cuanto estorba; uno que explica por qué, se discute — y eso es mejor. -->

## Artículo 1 — {{COMO_SE_CONSTRUYE_EL_SISTEMA}}

- {{LA_REGLA}}
- {{LA_SEGUNDA_REGLA}}
- {{QUE_CONTIENE_SIEMPRE_EL_REPOSITORIO}}

<!-- En el ejemplo del curso este artículo fija tres cosas: que se construye por
     versiones incrementales con su tag, que NO se anticipa nada que la versión
     en curso no pida (YAGNI), y que el repositorio siempre tiene la versión en
     curso funcionando. -->

## Artículo 2 — Stack: {{LENGUAJE}} y {{FRAMEWORK}}

- Lenguaje **{{LENGUAJE}}** sobre **{{FRAMEWORK}}**: {{LO_QUE_SE_USA_DEL_FRAMEWORK}}.
- {{LA_DECISION_SOBRE_EL_ACCESO_A_DATOS}} — y **por qué**: {{LA_RAZON}}.
- Paquetes externos permitidos: {{LISTA}} — y **ninguno más** sin que una
  especificación lo pida.

<!-- La última viñeta parece burocracia y no lo es: es lo que evita que al final
     del semestre nadie sepa por qué hay catorce dependencias. -->

## Artículo 3 — Arquitectura en capas, desde el día 1

```
{{DIAGRAMA_DE_CAPAS_CON_FLECHAS}}
```

- {{QUIEN_NO_PUEDE_SABER_QUE}}
- **Solo el ensamblador** decide **qué implementación** se usa. Todo lo demás
  **recibe sus dependencias**, nunca instancia lo que necesita.
- {{COMO_VIAJAN_LOS_ERRORES_DEL_NEGOCIO_HASTA_HTTP}}

<!-- OPCIONAL pero recomendado: un aviso contra la lectura exagerada de la regla
     de arriba. En el ejemplo del curso aclara que nombrar clases concretas NO
     está prohibido —de ProductoController hay una sola—; lo prohibido es que
     una clase se fabrique sola aquello de lo que hay DOS alternativas. Sin esa
     aclaración, alguien termina poniendo una interfaz por clase. -->

## Artículo 4 — Un solo comando

`{{EL_COMANDO}}` deja TODO el sistema de la versión funcionando, desde la
primera versión.

<!-- Y si hay algo que saber sobre ese comando —que la primera vez tarda, que
     recompila al encender y el puerto no contesta hasta que termine— va aquí.
     Es lo que separa «no funciona» de «todavía no terminó de arrancar». -->

## Artículo 5 — {{CUANDO_SE_DISENA_LA_BASE_DE_DATOS}}

{{QUE_VIENE_DADO_Y_QUE_SE_ESCRIBE_EN_ESTE_PROYECTO}}

<!-- OBLIGATORIO si la base de datos viene de otro curso o de otro equipo:
     decir qué parte se heredó y qué parte se escribió aquí, con números si se
     pueden contar. En el ejemplo son 12 tablas heredadas (89 líneas) contra 3
     disparadores y 16 procedimientos escritos en el proyecto (760 líneas, el
     89 % del script) — y la conclusión es que las reglas del negocio NO están
     en las tablas. -->

> {{POR_QUE_SE_DISENA_ASI_Y_NO_POR_VERSIONES}}

## Artículo 6 — Todo en español, comentado para principiantes

- Nombres, rutas, mensajes, comentarios y documentación: **en español**.
- El código lleva **comentarios línea a línea**: qué significa cada
  construcción del lenguaje y para qué sirve aquí. El repositorio es
  material de estudio, no solo software.

## Artículo 7 — Contratos exactos

Los endpoints, formatos y códigos de estado de cada versión están en su
`6_contracts.md` y se cumplen **al pie de la letra**{{MAS_EL_CONTRASTE_DIDACTICO_SI_HAY}}.

## Artículo 8 — Convenciones fijas

| Cosa | Convención |
|---|---|
| Puertos del proyecto | {{LISTA_DE_PUERTOS}} — ninguno se repite, y el registro es [`PUERTOS.md`]({{RUTA}}) |
| Rutas | {{LAS_RUTAS_BASE}} |
| Nombres | {{LA_CONVENCION_DE_NOMBRES_Y_LAS_CARPETAS}} |
| Sobre de respuesta | Lecturas: `{{FORMA_DE_LECTURA}}` · Errores: `{{FORMA_DE_ERROR}}` |
| Errores | {{QUE_CODIGO_RESPONDE_CADA_FALLO}} |
| Credenciales (didácticas) | {{USUARIO_Y_CLAVE_DE_EJEMPLO}} |

<!-- Esta tabla es la que más se consulta de todo el spec kit, porque responde
     las preguntas que uno se hace tecleando. Vale la pena que esté completa. -->
