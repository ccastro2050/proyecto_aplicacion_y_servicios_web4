"""
rutas_usuarios_roles.py — El usuario CON SUS ROLES (v2).

No es un CRUD de campos, y por eso no entra al registro de `entidades.py`: el
«campo» roles es una LISTA, y se dibuja con casillas.

LA LECCION DE ESTA PANTALLA, y es la que la v2 quiere mostrar: junto a ella
esta `/e/rol_usuario`, que es la MISMA relacion vista como tabla puente cruda
—una pareja a la vez—. Las dos operan sobre `rol_usuario`. La diferencia es
quien decide: ahi la persona arma la relacion pareja por pareja; aqui manda el
juego completo y la API lo reemplaza.
"""

from flask import (Blueprint, flash, redirect, render_template, request,
                   url_for, session)

import cliente_api
from entidades import ENTIDADES

bp = Blueprint("usuarios_roles", __name__)


def _roles():
    """Los roles disponibles, traidos de la API — no escritos aqui."""
    ok, datos, _ = cliente_api.listar("/api/rol")
    return datos if ok else []


def _exigir_sesion():
    """Sin sesion, al login. Cortesia, no proteccion: la
    proteccion es el 401 de la API."""
    if "usuario" not in session:
        return redirect(url_for("login"))
    return None


@bp.route("/usuarios-con-roles")
def lista():
    ida = _exigir_sesion()
    if ida:
        return ida
    ok, datos, errores = cliente_api.listar_usuarios_con_roles()
    for e in errores:
        flash(e, "error")
    return render_template("usuarios_roles/lista.html", usuarios=datos,
                           roles=_roles())


@bp.route("/usuarios-con-roles/nuevo", methods=["GET", "POST"])
def crear():
    ida = _exigir_sesion()
    if ida:
        return ida
    if request.method == "POST":
        # `getlist` porque son CASILLAS: pueden venir varias con el mismo
        # nombre. Con `get` llegaria una sola y el resto se perderia en
        # silencio — sin error, con el usuario a medio permiso.
        roles = request.form.getlist("roles")
        ok, errores = cliente_api.crear_usuario_con_roles(
            request.form.get("email", "").strip(),
            request.form.get("contrasena", ""),
            roles)
        if ok:
            flash("Usuario creado con sus %d rol(es)." % len(roles), "exito")
            return redirect(url_for("usuarios_roles.lista"))
        for e in errores:
            flash(e, "error")
    return render_template("usuarios_roles/formulario.html", roles=_roles(),
                           usuario=None)


@bp.route("/usuarios-con-roles/<email>/roles", methods=["POST"])
def reemplazar(email):
    ida = _exigir_sesion()
    if ida:
        return ida
    roles = request.form.getlist("roles")
    ok, errores = cliente_api.reemplazar_roles(email, roles)
    flash("Roles actualizados: ahora son %d." % len(roles) if ok
          else " ".join(errores), "exito" if ok else "error")
    return redirect(url_for("usuarios_roles.lista"))


@bp.route("/usuarios-con-roles/<email>/eliminar", methods=["POST"])
def eliminar(email):
    ida = _exigir_sesion()
    if ida:
        return ida
    ok, errores = cliente_api.eliminar_usuario_con_roles(email)
    flash("Usuario eliminado." if ok else " ".join(errores),
          "exito" if ok else "error")
    return redirect(url_for("usuarios_roles.lista"))
