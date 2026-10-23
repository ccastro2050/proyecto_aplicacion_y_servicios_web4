"""
rutas_entidades.py — Las vistas GENERICAS del front.

Un solo juego de vistas atiende a TODAS las entidades: la entidad llega en la
URL (`/e/<clave>`) y sus metadatos salen de `entidades.py`.

Y conviene decir por que aqui si vale lo generico y en la API no, que es la
pregunta obvia: la API expone un CONTRATO que otros leen, y un `/api/{tabla}`
lo deja en blanco. Esto no expone nada — es la configuracion de UNA
aplicacion, y el contrato que consume sigue siendo especifico.

UNA SOLA PAGINA POR RECURSO, igual que el front de Blazor: el formulario va
ENCIMA de la tabla, y «Editar» lo rellena en el mismo sitio -con `?editar=`-
en vez de llevar a otra pagina. Antes eran tres paginas: lista, crear y
editar.

LAS REGLAS DE NEGOCIO SIGUEN TODAS EN LA API. Aqui solo se dibuja.
"""

from flask import (Blueprint, abort, flash, redirect, render_template,
                   request, session, url_for)

import cliente_api
from entidades import ENTIDADES

bp = Blueprint("entidades", __name__)


def _config(clave):
    """Los metadatos de la entidad, y la cortesia de no abrir lo que no se puede.

    OJO CON LO QUE ESTO NO ES: no es el control de acceso. Si alguien quita
    esta comprobacion, la API sigue respondiendo 403 — porque la proteccion
    esta alla, en [ExigePermiso], y no aqui.

    Esto solo evita que la persona llegue a una pantalla que le va a responder
    403 en todas las operaciones. Y por eso usa EL MISMO permiso que la API:
    el valor de la tabla `ruta`.
    """
    cfg = ENTIDADES.get(clave)
    if cfg is None:
        abort(404)
    if "usuario" not in session:
        abort(redirect(url_for("login")))
    if cfg.get("permiso") and cfg["permiso"] not in session.get("permisos", []):
        flash("Su rol no tiene permiso para esa seccion.", "error")
        abort(redirect(url_for("inicio")))
    return cfg


def _opciones_fk(cfg):
    """Para cada campo que es clave foranea, trae sus opciones DESDE LA API.

    Es la leccion de la v2: la clave foranea se ELIGE de un desplegable, no se
    escribe. Un campo de texto obliga a la persona a adivinar que codigos
    existen; escribe uno que no esta, la API responde 409, y no hay forma de
    saber cual era el bueno.

    El desplegable muestra el NOMBRE y manda el CODIGO: la persona reconoce
    nombres, la base necesita claves.
    """
    opciones = {}
    for nombre, _, fk in cfg["campos"]:
        if not fk:
            continue
        fuente = ENTIDADES[fk]
        ok, datos, _ = cliente_api.listar(fuente["endpoint"])
        pk = fuente["pk"]
        etiqueta = fuente["campos"][1][0] if len(fuente["campos"]) > 1 else pk
        opciones[nombre] = ([(str(d[pk]), "%s - %s" % (d[pk], d.get(etiqueta, "")))
                             for d in datos] if ok else [])
    return opciones


def _del_formulario(cfg, solo_diligenciado=False):
    """Lo que la persona escribio, listo para viajar.

    LO VACIO NO VIAJA cuando se piden solo los cambios -el PATCH-: asi un campo
    que no se toco no se manda, que es lo que PATCH significa.

    EN EL PUT VIAJA TODO, porque PUT dice «la ficha queda asi» — pero lo vacio
    viaja como `null`, NO como cadena vacia. La diferencia se ve en el mensaje
    que recibe la persona:

        con ""    -> «The JSON value could not be converted to
                      System.Nullable`1[System.Decimal]»
        con null  -> «El campo valorunitario es obligatorio.»

    Las dos rechazan la ficha incompleta, que es lo correcto. Solo una lo dice
    en castellano y nombra el campo.
    """
    datos = {}
    for nombre, _, _ in cfg["campos"]:
        valor = request.form.get(nombre, "").strip()
        if solo_diligenciado and valor == "":
            continue
        datos[nombre] = valor if valor != "" else None
    return datos


@bp.route("/e/<clave>", methods=["GET", "POST"])
def lista(clave):
    """La pagina del recurso: el formulario y la tabla, juntos."""
    cfg = _config(clave)

    # --- POST sin pk: CREAR
    if request.method == "POST":
        datos = {k: v for k, v in _del_formulario(cfg).items() if v != ""}
        ok, errores = cliente_api.crear(cfg["endpoint"], datos)
        if ok:
            flash("Registro creado.", "exito")
            return redirect(url_for("entidades.lista", clave=clave))
        for e in errores:
            flash(e, "error")
        return _pintar(clave, cfg, registro=datos, editando=False)

    # --- GET con ?editar=: el MISMO formulario, relleno
    pk = request.args.get("editar")
    registro = {}
    if pk and cfg["editable"]:
        ok, registro, errores = cliente_api.obtener(cfg["endpoint"], pk)
        if not ok:
            for e in errores:
                flash(e, "error")
            return redirect(url_for("entidades.lista", clave=clave))
    return _pintar(clave, cfg, registro=registro or {}, editando=bool(pk and cfg["editable"]))


def _pintar(clave, cfg, registro, editando):
    ok, datos, errores = cliente_api.listar(cfg["endpoint"])
    for e in errores:
        flash(e, "error")
    return render_template("entidades/lista.html", clave=clave, cfg=cfg,
                           datos=datos, registro=registro, editando=editando,
                           opciones=_opciones_fk(cfg))


@bp.route("/e/<clave>/<pk>/guardar", methods=["POST"])
def guardar(clave, pk):
    """LOS DOS BOTONES DE GUARDAR, y sus nombres no son decorativos.

    «Guardar la ficha completa» manda PUT: la ficha queda como diga el
    formulario, y si falta un campo obligatorio la API responde 422.

    «Guardar solo lo que cambie» manda PATCH: viaja unicamente lo diligenciado.

    LA INTERFAZ NO LE DICE «PUT» NI «PATCH» A LA PERSONA. Le dice lo que va a
    pasar con su ficha, que es lo que la persona necesita decidir.
    """
    cfg = _config(clave)
    if not cfg["editable"]:
        abort(404)

    if request.form.get("accion") == "completa":
        ok, errores = cliente_api.reemplazar(cfg["endpoint"], pk,
                                             _del_formulario(cfg))
    else:
        datos = _del_formulario(cfg, solo_diligenciado=True)
        datos.pop(cfg["pk"], None)      # la clave no se cambia
        ok, errores = cliente_api.actualizar(cfg["endpoint"], pk, datos)

    if ok:
        flash("Registro actualizado.", "exito")
        return redirect(url_for("entidades.lista", clave=clave))
    for e in errores:
        flash(e, "error")
    return redirect(url_for("entidades.lista", clave=clave, editar=pk))


@bp.route("/e/<clave>/<pk>/eliminar", methods=["POST"])
def eliminar(clave, pk):
    cfg = _config(clave)
    ok, errores = cliente_api.eliminar(cfg["endpoint"], pk)
    flash("Registro retirado." if ok else " ".join(errores),
          "exito" if ok else "error")
    return redirect(url_for("entidades.lista", clave=clave))


@bp.route("/e/<clave>/<a>/<b>/quitar", methods=["POST"])
def quitar_puente(clave, a, b):
    """El borrado de una tabla PUENTE necesita LAS DOS claves.

    Su clave primaria son las dos columnas juntas, asi que con una sola no se
    sabe cual pareja quitar. De ahi que esta ruta exista aparte.
    """
    cfg = _config(clave)
    if not cfg.get("puente"):
        abort(404)
    ok, errores = cliente_api.eliminar(cfg["endpoint"], "%s/%s" % (a, b))
    flash("Asignacion retirada." if ok else " ".join(errores),
          "exito" if ok else "error")
    return redirect(url_for("entidades.lista", clave=clave))

# ------------------------------------------------------------
# UNA DIRECCION POR RECURSO, apuntando a la MISMA vista generica
# ------------------------------------------------------------
# `/productos`, `/empresas`, `/personas`… — las mismas que en el front de
# Blazor de los otros dos cursos, para que la direccion de un recurso sea la
# misma en los tres.
#
# No contradice que las vistas sean genericas: la implementacion sigue siendo
# UNA, y lo que se registra aqui es por donde se entra. Flask elige la regla
# cuyos `defaults` coinciden, asi que `url_for("entidades.lista",
# clave="producto")` sigue funcionando y devuelve `/productos`.
for _clave, _cfg in ENTIDADES.items():
    _url = _cfg.get("url", _clave)
    bp.add_url_rule("/%s" % _url, "lista", lista,
                    defaults={"clave": _clave}, methods=["GET", "POST"])
    bp.add_url_rule("/%s/<pk>/guardar" % _url, "guardar", guardar,
                    defaults={"clave": _clave}, methods=["POST"])
    bp.add_url_rule("/%s/<pk>/eliminar" % _url, "eliminar", eliminar,
                    defaults={"clave": _clave}, methods=["POST"])
    bp.add_url_rule("/%s/<a>/<b>/quitar" % _url, "quitar_puente",
                    quitar_puente, defaults={"clave": _clave}, methods=["POST"])
