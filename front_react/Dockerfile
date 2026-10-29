# ============================================================
# El front en React, para DESARROLLO.
#
# Ojo con lo que este Dockerfile NO es: no es como se publica. Publicado,
# este front no corre en ningun contenedor —ni siquiera necesita Node—:
# `npm run build` produce HTML, CSS y JavaScript, y cualquier servidor los
# entrega como entrega una imagen.
#
# Este contenedor existe solo para que `docker compose up` levante tambien
# esta interfaz, con recarga al guardar, sin pedirle a nadie que instale
# Node en su maquina.
# ============================================================
FROM node:22-alpine

WORKDIR /app

# Las dependencias PRIMERO y el codigo despues: la misma leccion del
# Dockerfile de Flask. Mientras package.json no cambie, Docker reusa la capa
# de los paquetes y reconstruir tarda segundos.
COPY package.json ./
RUN npm install

COPY . .

ENV PUERTO=8119
EXPOSE 8119

# `npm run dev` y no `build`: en desarrollo queremos el servidor de Vite,
# que recarga al guardar.
CMD ["npm", "run", "dev"]
