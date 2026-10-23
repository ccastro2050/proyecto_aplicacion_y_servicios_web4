"""
cliente_api.py — La capa de datos del FRONT.

Es al front lo que el repositorio es al back: la UNICA pieza que sabe donde
viven los datos. Traduce respuestas HTTP a tuplas `(ok, datos, errores)` — y
NUNCA decide negocio.

Si manana la API cambia de direccion, se cambia aqui y en ningun otro sitio.
"""

import os

import requests
from flask import session

# El NOMBRE del servicio del compose, jamas localhost: dentro del contenedor
# del front, localhost seria el front mismo.
URL_API = os.environ.get("API_FACTURAS_URL", "http://localhost:8035")

# Si la API no contesta en diez segundos, el front lo DICE. No se queda
# esperando: una interfaz colgada es peor que una que avisa.
TIEMPO_MAXIMO = 10

def _cabecera(token=None):
    """La cabecera con el token. Si no hay, se manda sin ella — y la API
    responde 401, que es lo correcto: no se simula una sesion que no existe."""
    if token is None:
        token = session.get("token")
    return {"Authorization": f"Bearer {token}"} if token else {}


def _llamar(metodo: str, ruta: str, **kwargs):
    """Ejecuta la peticion y unifica el manejo de «la API no responde»."""
    try:
        return requests.request(metodo, f"{URL_API}{ruta}",
                                timeout=TIEMPO_MAXIMO, **kwargs)
    except requests.RequestException:
        return None


def _mensaje(r):
    """El mensaje del DOMINIO que trae la API, no el codigo HTTP.

    El 422 trae `errores[]` con una entrada por campo; los demas traen
    `mensaje`. Y el 401 y el 403 se dicen con palabras, porque son los dos
    codigos que una persona tiene que poder distinguir:
    401 = «no se quien es usted»; 403 = «se quien es, y no puede».
    """
    try:
        cuerpo = r.json()
    except Exception:
        cuerpo = {}
    if r.status_code == 422:
        errores = cuerpo.get("errores") or []
        if isinstance(errores, list) and errores:
            return [str(e) for e in errores]
        if isinstance(errores, dict):
            return [f"{c}: {'; '.join(m)}" for c, m in errores.items()]
    if r.status_code == 401:
        return ["Su sesion no es valida o ya vencio. Vuelva a iniciar sesion."]
    if r.status_code == 403:
        return ["Su rol no tiene permiso para esta operacion."]
    return [cuerpo.get("mensaje", "El servicio respondio con un problema.")]


def listar(endpoint: str):
    r = _llamar("GET", endpoint, headers=_cabecera())
    if r is None:
        return False, [], ["El servicio no esta disponible."]
    if r.status_code == 204:
        # 204: la tabla esta vacia. NO es un error.
        return True, [], []
    if r.status_code == 200:
        # EL SOBRE DEL CONTRATO: { tabla, limite, total, datos[] }. La API no
        # devuelve un arreglo pelado, y deserializarlo como tal deja la
        # interfaz vacia SIN ningun error.
        return True, r.json().get("datos", []), []
    return False, [], _mensaje(r)


def obtener(endpoint: str, clave):
    r = _llamar("GET", f"{endpoint}/{clave}", headers=_cabecera())
    if r is None:
        return False, None, ["El servicio no esta disponible."]
    if r.status_code == 200:
        return True, r.json(), []
    return False, None, _mensaje(r)


def crear(endpoint: str, datos: dict):
    r = _llamar("POST", endpoint, json=datos, headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code in (200, 201):
        return True, []
    return False, _mensaje(r)


def actualizar(endpoint: str, clave, datos: dict):
    """PATCH: viaja SOLO lo diligenciado. Dejar un campo vacio significa «no lo
    toque», y por eso el formulario de editar no exige volver a escribirlo
    todo. El PUT, con el mismo cuerpo, responderia 422."""
    r = _llamar("PATCH", f"{endpoint}/{clave}", json=datos, headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code == 200:
        return True, []
    return False, _mensaje(r)


def eliminar(endpoint: str, clave):
    r = _llamar("DELETE", f"{endpoint}/{clave}", headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code == 200:
        return True, []
    return False, _mensaje(r)

# ── v3: LA SESION ────────────────────────────────────────────────────

def iniciar_sesion(email: str, contrasena: str):
    """POST /api/sesion — devuelve (ok, sesion, errores).

    Las credenciales van EN EL CUERPO, no en la URL. El endpoint viejo
    `verificar-contrasena` las recibia por la URL, y una contrasena en la URL
    queda en el historial del navegador y en los logs de cualquier proxy del
    camino.
    """
    r = _llamar("POST", "/api/sesion", json={"email": email, "contrasena": contrasena})
    if r is None:
        return False, None, ["El servicio no está disponible."]
    if r.status_code == 200:
        return True, r.json(), []
    # El MISMO mensaje para el correo inexistente y la contrasena equivocada:
    # lo decide la API, y el front no lo "mejora" averiguando cual fue.
    return False, None, [r.json().get("mensaje", "El correo o la contrasena no son correctos.")]


def mis_permisos(token: str):
    """GET /api/permisos/mios — las rutas a las que ESTE usuario puede entrar.

    El correo sale DEL TOKEN, no de la URL: si viniera por parametro,
    cualquiera podria preguntar por los permisos de otro.

    Y OJO: esto NO es el control de acceso. Es una lista para dibujar el menu.
    La proteccion es el 403 que responde la API en cada operacion.
    """
    r = _llamar("GET", "/api/permisos/mios", headers=_cabecera(token))
    if r is None or r.status_code != 200:
        return []
    return r.json().get("datos", [])


def consulta(nombre: str, token: str = None):
    """v4 — una de las diez consultas multitabla.

    AQUI SI VALE LO GENERICO, y conviene decir por que, porque la regla del
    proyecto es la contraria: la API expone una RUTA POR CONSULTA
    -/api/consultas/ventas-por-producto-, con su nombre y su contrato. Esto no
    es un /api/{consulta} en la API: es el cliente de este front, que recibe
    cual pedir. El contrato que consume sigue siendo especifico.

    EL SOBRE ES OTRO: { consulta, total, datos[] } — no el { tabla, limite,
    total, datos[] } del CRUD. Una consulta no tiene tabla ni limite: tiene
    nombre. Leer la clave equivocada devolveria una lista vacia SIN error.
    """
    # El token llega por argumento porque el tablero pide las diez EN HILOS,
    # y `session` no existe en un hilo nuevo. Si no llega, _cabecera lo busca
    # en la sesion como siempre.
    r = _llamar("GET", f"/api/consultas/{nombre}", headers=_cabecera(token))
    if r is None:
        return False, [], ["El servicio no esta disponible."]
    if r.status_code == 200:
        return True, r.json().get("datos", []), []
    return False, [], _mensaje(r)

# ------------------------------------------------------------
# LA FACTURACION (v2) — maestro-detalle, y no es un CRUD
# ------------------------------------------------------------
# Van aparte de las funciones genericas de arriba porque el recurso no se
# comporta como los demas: no tiene PUT ni PATCH ni DELETE. Se emite y se
# anula.


def listar_facturas():
    """Las facturas, con su encabezado. Mismo sobre que el resto del CRUD."""
    r = _llamar("GET", "/api/factura", headers=_cabecera())
    if r is None:
        return False, [], ["El servicio no esta disponible."]
    if r.status_code == 204:
        return True, [], []
    if r.status_code == 200:
        return True, r.json().get("datos", []), []
    return False, [], _mensaje(r)


def obtener_factura(numero):
    """Una factura CON SUS RENGLONES.

    Y aqui el sobre es OTRO: la API devuelve el objeto de la factura, no un
    `{tabla, limite, total, datos[]}`. Una factura no es una lista.
    """
    r = _llamar("GET", f"/api/factura/{numero}", headers=_cabecera())
    if r is None:
        return False, None, ["El servicio no esta disponible."]
    if r.status_code == 200:
        return True, r.json(), []
    return False, None, _mensaje(r)


def crear_factura(datos):
    """Emite la factura con todos sus renglones EN UN SOLO ENVIO.

    POR QUE DE UNA Y NO RENGLON POR RENGLON: una factura con tres renglones no
    son cuatro peticiones. Si la tercera fallara, quedaria media factura en la
    base —y «media factura» no es un estado que el negocio reconozca—. La API
    lo mete todo en un procedimiento almacenado, dentro de UNA transaccion.

    LO QUE NO VIAJA: `subtotal` ni `total`. Los calcula el disparador. Mandarlos
    seria tener la misma regla en dos lugares, y el dia que difieran nadie
    sabria cual manda.
    """
    cuerpo = {
        "fkidcliente": int(datos["fkidcliente"]) if datos.get("fkidcliente") else None,
        "fkidvendedor": int(datos["fkidvendedor"]) if datos.get("fkidvendedor") else None,
        "productos": [{"codigo": p["codigo"], "cantidad": int(p["cantidad"])}
                      for p in datos.get("productos", [])],
    }
    r = _llamar("POST", "/api/factura", json=cuerpo, headers=_cabecera())
    if r is None:
        return False, None, ["El servicio no esta disponible."]
    if r.status_code in (200, 201):
        return True, r.json(), []
    return False, None, _mensaje(r)


def anular_factura(numero):
    """ANULA la factura. No la borra, y la diferencia importa.

    Un DELETE haria desaparecer el documento, y una factura emitida es un hecho
    que ocurrio: se anula dejando constancia. De ahi que sea un POST a
    `/anular` y no un DELETE — y de ahi que el tablero de la v4 pueda contar
    cuanto se ha anulado y por cliente.
    """
    r = _llamar("POST", f"/api/factura/{numero}/anular", headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code in (200, 204):
        return True, []
    return False, _mensaje(r)

# ------------------------------------------------------------
# USUARIO CON SUS ROLES (v2) — el otro maestro-detalle
# ------------------------------------------------------------


def listar_usuarios_con_roles():
    """Los usuarios con la lista de sus roles, en una sola consulta.

    La API los trae con STRING_AGG; pedir los usuarios y despues un viaje por
    cada uno para sus roles serian N+1 peticiones y el mismo resultado.
    """
    r = _llamar("GET", "/api/usuario-con-roles", headers=_cabecera())
    if r is None:
        return False, [], ["El servicio no esta disponible."]
    if r.status_code == 204:
        return True, [], []
    if r.status_code == 200:
        return True, r.json().get("datos", []), []
    return False, [], _mensaje(r)


def crear_usuario_con_roles(email, contrasena, roles):
    """Crea el usuario Y le asigna sus roles EN UN SOLO ENVIO.

    `roles` es una lista de enteros. La API exige minimo uno: un usuario sin
    rol no puede hacer nada, y crearlo asi solo deja basura en la tabla.
    """
    cuerpo = {"email": email, "contrasena": contrasena,
              "roles": [int(x) for x in roles]}
    r = _llamar("POST", "/api/usuario-con-roles", json=cuerpo, headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code in (200, 201):
        return True, []
    return False, _mensaje(r)


def reemplazar_roles(email, roles, contrasena=None):
    """REEMPLAZA los roles del usuario por los que llegan.

    Es un PUT y no un PATCH a proposito: lo que llega ES el juego completo de
    roles. Quitar un rol es volver a enviar la lista sin el — no hay un
    «quitame este». Asi la pantalla de casillas dice la verdad: lo que esta
    marcado es lo que queda.
    """
    cuerpo = {"roles": [int(x) for x in roles]}
    if contrasena:
        cuerpo["contrasena"] = contrasena
    r = _llamar("PUT", f"/api/usuario-con-roles/{email}", json=cuerpo, headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code in (200, 204):
        return True, []
    return False, _mensaje(r)


def eliminar_usuario_con_roles(email):
    """Borra el usuario y sus asignaciones."""
    r = _llamar("DELETE", f"/api/usuario-con-roles/{email}", headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code in (200, 204):
        return True, []
    return False, _mensaje(r)

def reemplazar(endpoint: str, clave, datos: dict):
    """PUT — reemplaza la ficha COMPLETA.

    LA DIFERENCIA CON `actualizar` -que manda PATCH- es la leccion de la v1, y
    se ve con el mismo cuerpo:

        PUT  {"stock": 99}  -> 422, porque faltan los demas campos
        PATCH {"stock": 99} -> 200, porque solo toca lo que llega

    No es un capricho del servidor: PUT dice «la ficha queda ASI», y una ficha
    a la que le faltan campos no es una ficha. PATCH dice «cambiame esto».

    Por eso aqui viaja TODO el formulario, incluso lo que no se toco.
    """
    r = _llamar("PUT", f"{endpoint}/{clave}", json=datos, headers=_cabecera())
    if r is None:
        return False, ["El servicio no esta disponible."]
    if r.status_code in (200, 204):
        return True, []
    return False, _mensaje(r)
