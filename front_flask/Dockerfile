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
CMD ["python", "app.py"]
