"""
rutas_facturas.py — La facturación en el navegador (v2).

LA FACTURA NO ES UN CRUD, y por eso no entra al registro de entidades:

  * se EMITE con sus renglones en un solo envío —maestro-detalle—,
  * se ANULA, no se borra ni se edita. Una factura emitida es un documento:
    no desaparece, cambia de estado. Y no se corrige: se anula y se hace otra.

DOS PÁGINAS, las mismas que en los otros dos cursos:

    /facturas        la tabla, con el botón «Nueva factura»
    /facturas/nueva  el formulario de emisión
    /facturas/N      el detalle de una factura

LOS RENGLONES SE AGREGAN DE A UNO —elegir producto, cantidad, «Agregar al
detalle»— y aparecen en una tabla de la que se pueden quitar. No hay un número
fijo de casillas, porque nadie sabe de antemano cuántas cosas va a vender.

EL BORRADOR VIVE EN LA SESIÓN, y conviene decir por qué: en Flask cada clic es
una petición, así que si el borrador no se guardara en algún lado se perdería
en cada viaje. En un front de Blazor vive en el circuito; aquí, en la sesión.

  * Lo que se gana: sobrevive a un F5.
  * Lo que cuesta: un viaje al servidor por renglón.

EL TOTAL QUE SE VE ES UN CÁLCULO PARA MIRAR. Lo que queda guardado lo pone el
disparador `trg_actualizar_totales_y_stock`. Si el front lo enviara habría dos
fuentes de verdad, y el día que no coincidan gana la que nadie revisó.
"""

from flask import (Blueprint, flash, redirect, render_template, request,
                   session, url_for)

import cliente_api

bp = Blueprint("facturas", __name__)


def _borrador():
    """El maestro y los renglones a medio armar."""
    return session.setdefault("borrador_factura",
                              {"cliente": "", "vendedor": "", "renglones": []})


def _guardar(b):
    session["borrador_factura"] = b
    session.modified = True


def _opciones():
    """Clientes, vendedores y productos, traídos de la API.

    El nombre del cliente sale de `persona`: `cliente` solo guarda la clave
    foránea. Es la misma razón por la que la consulta 2 del tablero cruza
    cuatro tablas.
    """
    ok_c, clientes, _ = cliente_api.listar("/api/cliente")
    ok_v, vendedores, _ = cliente_api.listar("/api/vendedor")
    ok_p, personas, _ = cliente_api.listar("/api/persona")
    ok_pr, productos, _ = cliente_api.listar("/api/producto")
    if not (ok_c and ok_v and ok_p and ok_pr):
        flash("No se pudieron cargar todas las listas. Algunos desplegables "
              "van a estar vacíos.", "error")
    # La llave de `persona` es `codigo` —"P001"—, NO `id`: eso es lo que trae
    # `fkcodpersona` del cliente y del vendedor. Con `id` el diccionario queda
    # lleno de `None` y el desplegable muestra "?" en vez del nombre. Se midió:
    # GET /api/persona devuelve {codigo, nombre, email, telefono}.
    nombre = {p.get("codigo"): p.get("nombre", "") for p in personas}
    return (
        [(str(c["id"]), "%s (cliente %s)"
          % (nombre.get(c.get("fkcodpersona"), "?"), c["id"])) for c in clientes],
        [(str(v["id"]), "%s (carnet %s)"
          % (nombre.get(v.get("fkcodpersona"), "?"), v.get("carnet", "")))
         for v in vendedores],
        productos,
    )


@bp.route("/facturas")
def lista():
    """La TABLA de facturas. El formulario vive en /facturas/nueva."""
    if "usuario" not in session:
        return redirect(url_for("login"))
    ok, facturas, errores = cliente_api.listar_facturas()
    for e in errores:
        flash(e, "error")
    return render_template("facturas/lista.html", facturas=facturas)


@bp.route("/facturas/nueva", methods=["GET", "POST"])
def crear():
    """El formulario de emisión.

    Cuatro acciones en el mismo formulario, y se distinguen por el `name` del
    botón que se pulsó — que es como se hacía en HTML mucho antes de que
    hubiera componentes.
    """
    if "usuario" not in session:
        return redirect(url_for("login"))

    clientes, vendedores, productos = _opciones()
    precios = {p["codigo"]: p for p in productos}
    b = _borrador()

    if request.method == "POST":
        # el maestro se guarda SIEMPRE: no se puede perder al agregar un renglón
        b["cliente"] = request.form.get("fkidcliente", b.get("cliente", ""))
        b["vendedor"] = request.form.get("fkidvendedor", b.get("vendedor", ""))
        accion = request.form.get("accion", "")

        if accion == "agregar":
            codigo = request.form.get("codigo", "").strip()
            cantidad = request.form.get("cantidad", "").strip()
            if codigo not in precios:
                flash("Elija un producto.", "error")
            elif not cantidad.isdigit() or int(cantidad) < 1:
                flash("La cantidad tiene que ser 1 o más.", "error")
            else:
                # el mismo producto dos veces SUMA, no repite el renglón
                for r in b["renglones"]:
                    if r["codigo"] == codigo:
                        r["cantidad"] += int(cantidad)
                        break
                else:
                    b["renglones"].append({
                        "codigo": codigo,
                        "cantidad": int(cantidad),
                        "nombre": precios[codigo].get("nombre", codigo),
                        "precio": float(precios[codigo].get("valorunitario", 0) or 0),
                    })
            _guardar(b)
            return redirect(url_for("facturas.crear"))

        if accion == "quitar":
            fuera = request.form.get("quitar", "")
            b["renglones"] = [r for r in b["renglones"] if r["codigo"] != fuera]
            _guardar(b)
            return redirect(url_for("facturas.crear"))

        if accion == "limpiar":
            session.pop("borrador_factura", None)
            return redirect(url_for("facturas.crear"))

        if accion == "emitir":
            if not b["cliente"] or not b["vendedor"]:
                flash("Elija el cliente y el vendedor.", "error")
            elif not b["renglones"]:
                flash("Agregue al menos un renglón.", "error")
            else:
                # VIAJAN el cliente, el vendedor y los renglones. NO viajan el
                # subtotal ni el total: los calcula el disparador.
                ok, factura, errores = cliente_api.crear_factura({
                    "fkidcliente": b["cliente"], "fkidvendedor": b["vendedor"],
                    "productos": [{"codigo": r["codigo"], "cantidad": r["cantidad"]}
                                  for r in b["renglones"]],
                })
                if ok:
                    session.pop("borrador_factura", None)
                    flash("Factura %s emitida — los subtotales y el total los "
                          "calculó la base de datos." % factura.get("numero", ""),
                          "exito")
                    return redirect(url_for("facturas.lista"))
                for e in errores:
                    flash(e, "error")
            _guardar(b)

    total = sum(r["cantidad"] * r["precio"] for r in b["renglones"])
    return render_template("facturas/formulario.html", clientes=clientes,
                           vendedores=vendedores, productos=productos,
                           borrador=b, total_estimado=total)


@bp.route("/facturas/<int:numero>")
def detalle(numero):
    if "usuario" not in session:
        return redirect(url_for("login"))
    ok, factura, errores = cliente_api.obtener_factura(numero)
    if not ok:
        for e in errores:
            flash(e, "error")
        return redirect(url_for("facturas.lista"))
    return render_template("facturas/detalle.html", f=factura)


@bp.route("/facturas/<int:numero>/anular", methods=["POST"])
def anular(numero):
    """ANULAR, que no es borrar.

    Una factura emitida es un hecho que ocurrió: se anula dejando constancia, y
    el procedimiento DEVUELVE EL STOCK de cada renglón. Un DELETE haría
    desaparecer el documento y dejaría el inventario descuadrado.
    """
    if "usuario" not in session:
        return redirect(url_for("login"))
    ok, errores = cliente_api.anular_factura(numero)
    flash("Factura %s anulada — el stock se restauró." % numero if ok
          else " ".join(errores), "exito" if ok else "error")
    return redirect(url_for("facturas.lista"))
