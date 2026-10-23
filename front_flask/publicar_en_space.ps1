# ============================================================
# Copia este front a un Space de Hugging Face y lo deja listo para empujar.
#
# POR QUE EXISTE: Hugging Face no publica desde GitHub como Render. El Space
# ES un repositorio git propio, y hay que llevarle el contenido. Esto es lo
# unico que cambia respecto a los otros hospedajes.
#
# QUE HACE, en orden:
#   1. Comprueba que la carpeta del Space exista y sea un repositorio git.
#   2. Copia el contenido de front_flask a la RAIZ del Space -el Dockerfile
#      tiene que quedar arriba, no dentro de una subcarpeta-.
#   3. Escribe el README.md con la cabecera que Hugging Face exige.
#
# LO QUE NO HACE: el commit y el push. Esos se hacen a mano a proposito, para
# ver que se esta subiendo.
#
# USO:
#   .\publicar_en_space.ps1 -Space C:\ruta\al\clon\del\space `
#                           -UrlApi https://apifacturas-carloscastro.runasp.net
# ============================================================

param(
    # La carpeta donde clono el Space. Es lo unico obligatorio.
    [Parameter(Mandatory = $true)]  [string] $Space,

    # El puerto en que escucha SU aplicacion. Tiene que ser el mismo que
    # el contenedor expone: Hugging Face lo escribe en app_port y manda ahi
    # todo el trafico. Si no coinciden, el Space compila bien y queda
    # mostrando «Application starting» para siempre.
    [Parameter(Mandatory = $false)] [int]    $Puerto = 8118,

    # La direccion de la API que consume el front. Si su aplicacion no
    # consume ninguna, pase cadena vacia: solo sale en el README.
    [Parameter(Mandatory = $false)] [string] $UrlApi = "https://apifacturas-carloscastro.runasp.net",

    [Parameter(Mandatory = $false)] [string] $Titulo = "Facturas - front en Flask"
)

$origen = $PSScriptRoot

if (-not (Test-Path $Space)) {
    Write-Error "No existe la carpeta del Space: $Space"
    exit 1
}
if (-not (Test-Path (Join-Path $Space ".git"))) {
    Write-Error "Esa carpeta no es un repositorio git. Clone primero el Space."
    exit 1
}

# --- 1. Copiar el codigo a la raiz del Space -----------------------------
# __pycache__ no se copia: son compilados del interprete, y en el contenedor
# los crea otro usuario.
Write-Host "Copiando el front al Space..."
Get-ChildItem -Path $origen -Exclude "__pycache__", "publicar_en_space.ps1" |
    Copy-Item -Destination $Space -Recurse -Force

# --- 2. El README.md que Hugging Face exige -----------------------------
# La cabecera entre --- no es decoracion: es lo que le dice al Space que
# construya un Dockerfile y en que puerto escucha la aplicacion. Sin ella,
# el Space no arranca y el mensaje no explica por que.
$readme = @"
---
title: $Titulo
emoji: 🧾
colorFrom: blue
colorTo: gray
sdk: docker
app_port: $Puerto
pinned: false
---

# Facturas — la interfaz en Flask

La **misma aplicación** que el front en Blazor del proyecto, escrita en Python
y hablando con **la misma API** por HTTP.

| | |
|---|---|
| **La API que consume** | ``$UrlApi`` |
| **Entrar** | ``admin@correo.com`` / ``admin123`` |

Este front **no tiene base de datos**: todo lo pide por HTTP. Si la API está
dormida, la primera carga tarda unos segundos.

Material del curso **Aplicación y Servicios Web** — ITM, 2026-2.
"@
Set-Content -Path (Join-Path $Space "README.md") -Value $readme -Encoding utf8
Write-Host "README.md escrito (sdk: docker, app_port: $Puerto)"

Write-Host ""
Write-Host "LISTO. Ahora, dentro de la carpeta del Space:"
Write-Host "  git add ."
Write-Host "  git commit -m `"El front en Flask`""
Write-Host "  git push"
Write-Host ""
Write-Host "Y en el Space, en Settings > Variables and secrets, agregue:"
Write-Host "  API_FACTURAS_URL = $UrlApi"
Write-Host "  CLAVE_SESION     = (una cadena larga cualquiera)"
