# ============================================
# Products.Api - Script SOLO con Docker (PowerShell)
# ============================================
# Requisito: SOLO Docker instalado + acceso a internet
# NO requiere .NET SDK en la máquina host
# ============================================

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║   Products.Api - Docker Only Build     ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""

# Verificar Docker
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Docker no está instalado" -ForegroundColor Red
    exit 1
}

# Variables
$IMAGE_NAME = "products-api"
$CONTAINER_NAME = "products-api"
$PORT = "5000"
$ROOT_DIR = "..\..\\"

# Detener contenedor existente
$existing = docker ps -aq -f "name=$CONTAINER_NAME" 2>$null
if ($existing) {
    Write-Host "→ Deteniendo contenedor existente..." -ForegroundColor Yellow
    docker stop $CONTAINER_NAME 2>$null | Out-Null
    docker rm $CONTAINER_NAME 2>$null | Out-Null
}

# Construir imagen (todo dentro de Docker)
Write-Host ""
Write-Host "→ Construyendo imagen (compilación dentro de Docker)..." -ForegroundColor Yellow
Write-Host "   Esto puede tomar varios minutos la primera vez" -ForegroundColor Yellow
Write-Host ""

docker build -t "${IMAGE_NAME}:latest" -f Dockerfile $ROOT_DIR

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error al construir la imagen" -ForegroundColor Red
    Write-Host ""
    Write-Host "Posibles soluciones:" -ForegroundColor Yellow
    Write-Host "1. Verifica tu conexión a internet"
    Write-Host "2. Configura DNS en Docker:" -ForegroundColor Yellow
    Write-Host '   Docker Desktop > Settings > Docker Engine' -ForegroundColor Cyan
    Write-Host '   Agregar: "dns": ["8.8.8.8", "8.8.4.4"]' -ForegroundColor Cyan
    exit 1
}

Write-Host "✓ Imagen construida exitosamente" -ForegroundColor Green

# Ejecutar contenedor
Write-Host ""
Write-Host "→ Iniciando contenedor..." -ForegroundColor Yellow
docker run -d `
    -p "${PORT}:8080" `
    -e "ASPNETCORE_ENVIRONMENT=Development" `
    --name $CONTAINER_NAME `
    "${IMAGE_NAME}:latest"

Start-Sleep -Seconds 3

# Verificar health
try {
    $response = Invoke-WebRequest -Uri "http://localhost:$PORT/api/v1/health" -UseBasicParsing -TimeoutSec 5
    Write-Host "✓ API iniciada correctamente" -ForegroundColor Green
} catch {
    Write-Host "⚠ La API está iniciando..." -ForegroundColor Yellow
}

# Mostrar información
Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║         ✓ API LISTA                    ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "  Swagger UI:    " -NoNewline; Write-Host "http://localhost:$PORT" -ForegroundColor Cyan
Write-Host "  Health Check:  " -NoNewline; Write-Host "http://localhost:$PORT/api/v1/health" -ForegroundColor Cyan
Write-Host ""
