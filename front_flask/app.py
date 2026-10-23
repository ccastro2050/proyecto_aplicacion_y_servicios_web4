"""
app.py — El ensamblador del FRONT (Flask + Jinja2).

El front no tiene negocio ni base de datos: rutas que muestran HTML y un
cliente HTTP que habla con la API. Su UNICO estado es la SESION.

LA SESION GUARDA EL TOKEN, y conviene decir donde vive: en la cookie de sesion
de Flask, que va FIRMADA con `CLAVE_SESION` y que el navegador no puede
alterar sin romper la firma.

Lo que NO se hace es guardarlo en `localStorage`: ahi cualquier script de la
pagina lo puede leer. La cookie de sesion de Flask tampoco es perfecta —viaja
al navegador— pero es el mecanismo que el framework ya trae firmado.
"""

import os
from functools import wraps

from flask import Flask, flash, redirect, render_template, request, session, url_for

import cliente_api
from entidades import ENTIDADES
from rutas_entidades import bp as bp_entidades
# v2 — el usuario CON SUS ROLES: maestro-detalle con casillas,
# que no cabe en el molde de las vistas genericas.
from rutas_usuarios_roles import bp as bp_usuarios_roles
# v4 — el tablero: diez consultas multitabla en una pagina.
from rutas_tablero import bp as bp_tablero
from rutas_facturas import bp as bp_facturas

app = Flask(__name__)
# La clave que FIRMA la cookie viaja por variable de entorno, no escrita en el
# codigo: asi se cambia sin recompilar y no queda en el repositorio de nadie.
app.secret_key = os.environ.get("CLAVE_SESION", "clave-solo-para-desarrollo")
app.register_blueprint(bp_entidades)
app.register_blueprint(bp_usuarios_roles)
app.register_blueprint(bp_tablero)
# v2 — la facturacion maestro-detalle: su propio blueprint, porque no
# es un CRUD. Una factura no se edita: se emite y se anula.
app.register_blueprint(bp_facturas)


# Las tarjetas del inicio que NO salen del registro, porque no son
# un CRUD de campos: la factura se emite y se anula, el usuario con sus
# roles viaja con casillas, y el tablero no tiene tabla.
TARJETAS_SUELTAS = [
    ("/facturas", "Facturas", 2, "Maestro-detalle: la factura y sus renglones, en un solo envío"),
    ("/usuarios-con-roles", "Usuarios y roles", 2, "El usuario y sus roles, con casillas"),
    ("/tablero", "Tablero", 4, "Diez consultas que cruzan cuatro o más tablas"),
]


@app.context_processor
def menu():
    """EL MENU, ARMADO CON LOS PERMISOS DE QUIEN ESTA IDENTIFICADO.

    Y AQUI VA LA ADVERTENCIA MAS IMPORTANTE DE LA v3:

        ESCONDER UNA ENTRADA DEL MENU NO ES CONTROL DE ACCESO.

    Es la trampa en la que cae casi todo el mundo, porque PARECE que funciona:
    el vendedor entra, no ve «Usuarios», y se va tranquilo.

    Pero este menu es HTML que ya esta en su navegador. Quien escriba la
    direccion a mano llega igual. Lo que lo para es que la API responda 403.

    ENTONCES PARA QUE SIRVE: para no mostrarle a alguien lo que no va a poder
    usar. Es comodidad. La proteccion esta en el servicio.
    """
    permitidas = session.get("permisos", [])
    visibles = {c: e for c, e in ENTIDADES.items()
                if e.get("permiso") in permitidas}
    return {"menu_entidades": visibles, "hay_sesion": "usuario" in session,
            "usuario_actual": session.get("usuario"),
            "roles_actuales": session.get("roles", []), "tarjetas_sueltas": TARJETAS_SUELTAS}


def login_requerido(vista):
    """Sin sesion, toda ruta devuelve al login.

    NO es la proteccion —la proteccion es el 401 que responde la API— sino
    cortesia: sin token, la peticion traeria un 401 y la persona veria un error
    en vez de entender que le falta identificarse.
    """

    @wraps(vista)
    def envoltura(*args, **kwargs):
        if "usuario" not in session:
            return redirect(url_for("login"))
        return vista(*args, **kwargs)

    return envoltura


app.jinja_env.globals["login_requerido"] = login_requerido


@app.route("/login", methods=["GET", "POST"])
def login():
    if request.method == "POST":
        ok, sesion, errores = cliente_api.iniciar_sesion(
            request.form.get("email", "").strip(),
            request.form.get("contrasena", ""),
        )
        if ok:
            session["usuario"] = sesion["email"]
            session["token"] = sesion["token"]
            session["roles"] = sesion.get("roles", [])
            # Los permisos se piden EN SEGUIDA, con el token recien recibido:
            # son los que arman el menu.
            session["permisos"] = cliente_api.mis_permisos(sesion["token"])
            return redirect(url_for("inicio"))
        for e in errores:
            flash(e, "error")
    return render_template("login.html")


@app.route("/logout")
def logout():
    session.clear()
    return redirect(url_for("login"))


@app.route("/")
@login_requerido
def inicio():
    return render_template("inicio.html", entidades=ENTIDADES)


# ============================================================
# EL ARRANQUE, y por que lee TRES variables del entorno
#
# En Docker lo arranca el compose; en un hospedaje de nube lo arranca la
# plataforma, y cada una tiene sus costumbres. Leer del entorno es lo que
# permite que el MISMO codigo corra en los dos sitios sin tocar una linea.
#
#   PORT    la ponen casi todas las plataformas de nube -Render, Railway,
#           Koyeb- y esperan que el programa escuche AHI. Si uno escucha en
#           otro puerto, la plataforma reporta el sitio como caido sin decir
#           por que.
#   PUERTO  la del compose de este proyecto: 8118.
#   DEPURAR "1" en desarrollo -recarga al guardar y muestra el error
#           completo en la pagina-, "0" publicado.
#
# El orden importa: PORT gana, porque cuando existe es porque la plataforma
# lo exige.
# ============================================================
if __name__ == "__main__":
    puerto = int(os.environ.get("PORT") or os.environ.get("PUERTO") or 8118)
    depurar = os.environ.get("DEPURAR", "1") == "1"
    app.run(host="0.0.0.0", port=puerto, debug=depurar)
