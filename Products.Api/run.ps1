# ============================================
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

# Detener contenedor existente si existe
$existing = docker ps -aq -f "name=$CONTAINER_NAME" 2>$null
if ($existing) {
    Write-Host "→ Deteniendo contenedor existente..." -ForegroundColor Yellow
    docker stop $CONTAINER_NAME 2>$null | Out-Null
    docker rm $CONTAINER_NAME 2>$null | Out-Null
}

# Construir imagen
Write-Host "→ Construyendo imagen Docker..." -ForegroundColor Yellow
docker build -t "${IMAGE_NAME}:latest" -f Dockerfile ..

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al construir la imagen" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Imagen construida exitosamente" -ForegroundColor Green

# Ejecutar contenedor
Write-Host "→ Iniciando contenedor..." -ForegroundColor Yellow
docker run -d `
    -p "${PORT}:8080" `
    -e "ASPNETCORE_ENVIRONMENT=Development" `
    --name $CONTAINER_NAME `
    "${IMAGE_NAME}:latest"

# Esperar a que la API esté lista
Write-Host "→ Esperando a que la API esté lista..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

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
