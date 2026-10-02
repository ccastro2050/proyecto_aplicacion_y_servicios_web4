# ==============================================================
# Front Blazor — Facturas · Blazor Server / .NET 10 (puerto 8099)
#
# Usa la imagen del SDK (no la de runtime) a propósito: el contenedor corre
# `dotnet watch`, que RECOMPILA Y REINICIA solo al guardar un .razor o un .cs.
# Es el mismo ciclo que la API.
# ==============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

# Primero solo el .csproj: si no cambian las dependencias, Docker reutiliza la
# capa del restore y los arranques siguientes son más rápidos.
COPY FrontFacturas.csproj ./
RUN dotnet restore FrontFacturas.csproj

COPY . .

# DOTNET_USE_POLLING_FILE_WATCHER: en Docker sobre Windows los avisos de
#   "archivo cambiado" no siempre llegan al contenedor. El sondeo revisa cada
#   tanto y el watch funciona de verdad.
ENV DOTNET_USE_POLLING_FILE_WATCHER=true \
    ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://0.0.0.0:8099

EXPOSE 8099

# --no-launch-profile ignora launchSettings.json y respeta ASPNETCORE_URLS.
# --non-interactive: cuando un cambio NO se puede aplicar en caliente
# -agregar una interfaz a un componente, por ejemplo- dotnet watch
# REINICIA en vez de preguntar. Sin este argumento se queda esperando
# una respuesta en una consola que nadie esta mirando, el puerto deja
# de responder, y parece que el contenedor se cayo. El Dockerfile de la
# API ya lo tenia; este no, y costo encontrarlo.
CMD ["dotnet", "watch", "--project", "FrontFacturas.csproj", "run", "--non-interactive", "--no-launch-profile"]
