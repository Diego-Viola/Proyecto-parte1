﻿﻿﻿# ============================================
# Products.Api - Script de Ejecución (PowerShell)
# ============================================
# Uso: .\run.ps1
# ============================================

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║       Products.Api - Docker Run        ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""

# Verificar Docker
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Docker no está instalado" -ForegroundColor Red
    Write-Host "Instala Docker desde: https://docs.docker.com/get-docker/"
    exit 1
}

# Variables
$IMAGE_NAME = "products-api"
$CONTAINER_NAME = "products-api"
$PORT = "5000"

# Cambiar al directorio padre (Proyecto-parte1)
Set-Location ..

# Detener contenedor existente si existe
$existing = docker ps -aq -f "name=$CONTAINER_NAME" 2>$null
if ($existing) {
    Write-Host "→ Deteniendo contenedor existente..." -ForegroundColor Yellow
    docker stop $CONTAINER_NAME 2>$null | Out-Null
    docker rm $CONTAINER_NAME 2>$null | Out-Null
}

# Paso 1: Compilar localmente (evita problemas de red en Docker)
Write-Host ""
Write-Host "Paso 1: Compilando aplicación localmente..." -ForegroundColor Yellow
dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al compilar. Verifica que .NET SDK esté instalado." -ForegroundColor Red
    Set-Location Products.Api
    exit 1
}

Write-Host "✓ Compilación exitosa" -ForegroundColor Green

# Paso 2: Construir imagen Docker con binarios pre-compilados
Write-Host ""
Write-Host "Paso 2: Construyendo imagen Docker..." -ForegroundColor Yellow
docker build -t "${IMAGE_NAME}:latest" -f Products.Api/Dockerfile.prebuilt .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al construir imagen Docker. Ejecutando sin Docker..." -ForegroundColor Yellow
    Set-Location Products.Api
    dotnet run --project Products.Api.csproj
    exit 0
}

Write-Host "✓ Imagen construida exitosamente" -ForegroundColor Green

# Paso 3: Ejecutar contenedor
Write-Host ""
Write-Host "Paso 3: Iniciando contenedor..." -ForegroundColor Yellow
docker run -d `
    -p "${PORT}:8080" `
    -e "ASPNETCORE_ENVIRONMENT=Development" `
    --name $CONTAINER_NAME `
    "${IMAGE_NAME}:latest"

# Esperar a que la API esté lista
Write-Host "→ Esperando a que la API esté lista..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# Volver al directorio original
Set-Location Products.Api

# Verificar health
try {
    $response = Invoke-WebRequest -Uri "http://localhost:$PORT/api/v1/health" -UseBasicParsing -TimeoutSec 5
    Write-Host "✓ API iniciada correctamente" -ForegroundColor Green
} catch {
    Write-Host "⚠ La API está iniciando, espera unos segundos..." -ForegroundColor Yellow
}

# Mostrar información
Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║         ✓ API LISTA                    ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "  Swagger UI:    " -NoNewline; Write-Host "http://localhost:$PORT" -ForegroundColor Cyan
Write-Host "  Health Check:  " -NoNewline; Write-Host "http://localhost:$PORT/api/v1/health" -ForegroundColor Cyan
Write-Host "  Products:      " -NoNewline; Write-Host "http://localhost:$PORT/api/v1/products" -ForegroundColor Cyan
Write-Host ""
Write-Host "Comandos útiles:" -ForegroundColor Yellow
Write-Host "  docker logs -f $CONTAINER_NAME    # Ver logs"
Write-Host "  docker stop $CONTAINER_NAME       # Detener"
Write-Host "  docker rm $CONTAINER_NAME         # Eliminar"
Write-Host ""
