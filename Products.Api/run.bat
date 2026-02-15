@echo off
REM ============================================
REM Products.Api - Script de Ejecución (CMD)
REM ============================================
REM Uso: run.bat
REM ============================================

echo.
echo ========================================
echo       Products.Api - Docker Run
echo ========================================
echo.

REM Verificar Docker
docker --version >nul 2>&1
if errorlevel 1 (
    echo Error: Docker no esta instalado
    echo Instala Docker desde: https://docs.docker.com/get-docker/
    exit /b 1
)

set IMAGE_NAME=products-api
set CONTAINER_NAME=products-api
set PORT=5000

REM Detener contenedor existente
echo Deteniendo contenedor existente si existe...
docker stop %CONTAINER_NAME% >nul 2>&1
docker rm %CONTAINER_NAME% >nul 2>&1

REM Construir imagen
echo Construyendo imagen Docker...
docker build -t %IMAGE_NAME%:latest -f Dockerfile ..
if errorlevel 1 (
    echo Error al construir la imagen
    exit /b 1
)

echo Imagen construida exitosamente

REM Ejecutar contenedor
echo Iniciando contenedor...
docker run -d -p %PORT%:8080 -e ASPNETCORE_ENVIRONMENT=Development --name %CONTAINER_NAME% %IMAGE_NAME%:latest

REM Esperar
echo Esperando a que la API este lista...
timeout /t 3 /nobreak >nul

echo.
echo ========================================
echo          API LISTA
echo ========================================
echo.
echo   Swagger UI:    http://localhost:%PORT%
echo   Health Check:  http://localhost:%PORT%/api/v1/health
echo   Products:      http://localhost:%PORT%/api/v1/products
echo.
echo Comandos utiles:
echo   docker logs -f %CONTAINER_NAME%    - Ver logs
echo   docker stop %CONTAINER_NAME%       - Detener
echo   docker rm %CONTAINER_NAME%         - Eliminar
echo.
