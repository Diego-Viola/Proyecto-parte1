# ============================================
# Products.Api - Script de Ejecución (PowerShell)
# ============================================
# Uso: .\run.ps1
# Ejecutar desde: Products.Api/RunProject/
# 
# Esta versión compila localmente para evitar
# problemas de red (NU1301) en Docker
# ============================================

$ErrorActionPreference = "Continue"

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

# Verificar .NET SDK
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: .NET SDK no está instalado" -ForegroundColor Red
    Write-Host "Instala desde: https://dotnet.microsoft.com/download"
    Write-Host ""
    Write-Host "Alternativa: intentar build dentro de Docker (puede fallar por red)" -ForegroundColor Yellow
    Write-Host "  docker build -t products-api:latest -f Dockerfile ..\.." -ForegroundColor Cyan
    exit 1
}

# Variables
$IMAGE_NAME = "products-api"
$CONTAINER_NAME = "products-api"
$PORT = "5000"
$ROOT_DIR = "..\..\\"
$PUBLISH_DIR = "..\..\publish"

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
Write-Host "         (esto evita errores NU1301 en Docker)" -ForegroundColor Gray

Push-Location $ROOT_DIR
try {
    dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish --nologo
    if ($LASTEXITCODE -ne 0) {
        throw "Error en compilación"
    }
    Write-Host "✓ Compilación exitosa" -ForegroundColor Green
} catch {
    Write-Host "Error al compilar." -ForegroundColor Red
    Write-Host "Verifica que .NET SDK esté instalado y las dependencias sean correctas." -ForegroundColor Yellow
    Pop-Location
    exit 1
}
Pop-Location

# Paso 2: Verificar y crear Dockerfile.prebuilt sin BOM
Write-Host ""
Write-Host "Paso 2: Preparando Dockerfile..." -ForegroundColor Yellow

$dockerfileContent = @"
# ============================================
# Products.Api - Dockerfile Pre-built
# ============================================
# Esta versión copia binarios pre-compilados localmente
# para evitar problemas de red (NU1301) en Docker
# ============================================

# Stage 1: Runtime (imagen mínima)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Crear usuario no-root para seguridad
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D appuser

# Copiar artefactos pre-compilados
COPY publish/ .

# Copiar datos JSON
COPY Products.Api.Persistence/Data/ ./Data/

# Establecer permisos
RUN chown -R appuser:appgroup /app

# Cambiar a usuario no-root
USER appuser

# Exponer puerto
EXPOSE 8080

# Variables de entorno
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/api/v1/health || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "Products.Api.dll"]
"@

# Crear archivo sin BOM usando UTF8 sin BOM
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllLines("$PSScriptRoot\Dockerfile.prebuilt", $dockerfileContent, $utf8NoBom)
Write-Host "✓ Dockerfile.prebuilt preparado" -ForegroundColor Green

# Paso 3: Construir imagen Docker con binarios pre-compilados
Write-Host ""
Write-Host "Paso 3: Construyendo imagen Docker..." -ForegroundColor Yellow
docker build -t "${IMAGE_NAME}:latest" -f Dockerfile.prebuilt $ROOT_DIR

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error al construir imagen Docker." -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternativa: ejecutar localmente sin Docker" -ForegroundColor Yellow
    Write-Host "  cd ..\" -ForegroundColor Cyan
    Write-Host "  dotnet run --project Products.Api.csproj" -ForegroundColor Cyan
    exit 1
}

Write-Host "✓ Imagen construida exitosamente" -ForegroundColor Green

# Paso 4: Ejecutar contenedor
Write-Host ""
Write-Host "Paso 4: Iniciando contenedor..." -ForegroundColor Yellow
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

# Limpiar carpeta publish
Write-Host ""
Write-Host "→ Limpiando archivos temporales..." -ForegroundColor Gray
Remove-Item -Path $PUBLISH_DIR -Recurse -Force -ErrorAction SilentlyContinue

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
