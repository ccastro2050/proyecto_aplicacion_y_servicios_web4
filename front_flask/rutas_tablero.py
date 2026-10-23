"""
rutas_tablero.py — EL TABLERO de la v4.

Diez consultas, una sola pagina. Y tres decisiones que vale la pena mirar:

 1. LAS DIEZ SE PIDEN DE UNA, no una tras otra. Cada una tarda lo suyo; en
    serie, con el tiempo maximo de 10 segundos por peticion, abrir el tablero
    podria costar un minuto y medio. Con un grupo de hilos cuesta lo que la
    mas lenta. Es la misma leccion que costo 50 segundos en la v2 con los
    desplegables de la factura.

 2. SI UNA FALLA, LAS OTRAS NUEVE SE DIBUJAN. Un tablero que se cae entero
    porque una consulta tuvo un problema es peor que uno incompleto: no deja
    ver lo que si esta bien, y no dice cual fallo.

 3. EL TOKEN SE LEE AQUI Y SE PASA A LOS HILOS. `session` pertenece al
    contexto de la peticion, y un hilo nuevo no lo tiene.

    EL CAMINO QUE NO SIRVIO, porque es el primero que se intenta: envolver la
    funcion con `copy_current_request_context` y dejar que cada hilo entre al
    contexto copiado. Eso revienta con

        ValueError: <Token ...> was created in a different Context

    porque el envoltorio copia UN contexto y los diez hilos entran y salen del
    mismo — y el token que guarda la variable de contexto al entrar no se puede
    devolver desde otro hilo. Funcionaria con una copia por hilo; no vale la
    pena. Pasar el dato explicitamente es mas corto y no tiene magia: la
    funcion de abajo no depende de Flask para nada.

Y EL SQL NO ESTA AQUI. Las diez consultas viven en el repositorio de la API,
en su dialecto. Este archivo no sabe que hay cuatro tablas cruzadas detras de
cada numero: pide por nombre y dibuja lo que vuelva.
"""

from concurrent.futures import ThreadPoolExecutor

from flask import (Blueprint, flash, redirect, render_template, session,
                   url_for)

import cliente_api

bp = Blueprint("tablero", __name__)

# El nombre de cada consulta tal como lo expone la API, y el titulo con que se
# muestra. El orden es el del tablero.
CONSULTAS = [
    ("ventas-por-producto", "Ventas por producto"),
    ("ventas-por-cliente", "Ventas por cliente"),
    ("ventas-por-vendedor", "Ventas por vendedor"),
    ("ventas-por-empresa", "Ventas por empresa"),
    ("ticket-por-vendedor", "Ticket promedio por vendedor"),
    ("productos-sin-vender", "Productos que nunca se han vendido"),
    ("anulaciones-por-cliente", "Anulaciones por cliente"),
    ("alcance-de-usuarios", "Alcance de cada usuario"),
    ("interfaces-sin-usuarios", "Interfaces a las que no llega nadie"),
    ("credito-contra-consumo", "Credito contra consumo"),
]


@bp.route("/tablero")
def tablero():
    """El tablero. Sin sesion no hay token, y sin token la API responde 401.

    Como en el resto del front, mandar al login es CORTESIA, no proteccion: la
    proteccion es el 401 y el 403 que responde la API.
    """
    if "usuario" not in session:
        return redirect(url_for("login"))

    # Se lee UNA vez, en el hilo de la peticion, y viaja como argumento.
    token = session.get("token")

    def pedir(par):
        nombre, titulo = par
        ok, datos, _ = cliente_api.consulta(nombre, token)
        return nombre, titulo, ok, datos

    with ThreadPoolExecutor(max_workers=len(CONSULTAS)) as grupo:
        resultados = list(grupo.map(pedir, CONSULTAS))

    fallaron = [t for _, t, ok, _ in resultados if not ok]
    if fallaron:
        # Decir CUALES fallaron, no «hubo un error»: con el nombre se sabe
        # donde mirar; sin el, hay que abrir las diez a mano.
        flash("No se pudieron cargar: %s. Las demas si." % ", ".join(fallaron),
              "error")

    return render_template(
        "tablero.html", orden=CONSULTAS,
        resultados={n: (t, ok, d) for n, t, ok, d in resultados})
