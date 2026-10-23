# ============================================================
# El FRONT en Flask + Jinja2.
#
# Es un CONTENEDOR APARTE de la API, y eso es lo que se quiere demostrar: la
# interfaz y la API son dos procesos distintos que se hablan por HTTP. Apague
# la API y la interfaz sigue en pie, con su aviso y sin una sola fila.
#
# `slim` y no la imagen completa: son ~50 MB contra ~350 MB, y lo unico que
# hace falta es Python y dos paquetes.
# ============================================================
FROM python:3.12-slim

WORKDIR /app

# Los requisitos PRIMERO y el codigo despues: asi Docker reusa la capa de los
# paquetes mientras `requirements.txt` no cambie, y reconstruir tras editar un
# .py tarda segundos en vez de minutos.
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

ENV PUERTO=8118
EXPOSE 8118

# El defecto es PUBLICADO, no desarrollo. Asi, un hospedaje que solo haga
# `docker build` + `docker run` -Hugging Face, Render, Back4App- arranca con
# gunicorn sin que nadie configure nada. El modo de desarrollo se pide a
# proposito: el docker-compose de este repositorio pone DEPURAR=1.
#
# Ponerlo al reves -depurar por defecto- es como queda abierto el depurador
# de Werkzeug en internet. Y nadie se entera hasta que alguien lo encuentra.
ENV DEPURAR=0

# ============================================================
# UN SOLO ARRANQUE PARA LOS DOS MUNDOS
#
# En desarrollo queremos el servidor de Flask: recarga al guardar y muestra
# el error completo en la pagina. Publicado NO lo queremos: atiende una
# peticion a la vez, y su depurador en internet es una puerta abierta.
#
# La variable DEPURAR decide. Vale "1" por defecto -asi el compose no tiene
# que ponerla- y se fija en "0" en el hospedaje.
#
# El ${PORT:-${PUERTO:-8118}} es la misma cadena de respaldo del app.py:
# manda el puerto que imponga la plataforma, y si no hay, el del proyecto.
# ============================================================
CMD ["sh", "-c", "if [ \"$DEPURAR\" = \"0\" ]; then exec gunicorn --bind 0.0.0.0:${PORT:-${PUERTO:-8118}} --workers 2 app:app; else exec python app.py; fi"]
