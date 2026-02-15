﻿@echo off
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

REM Cambiar al directorio padre (Proyecto-parte1)
cd ..

REM Detener contenedor existente
echo Deteniendo contenedor existente si existe...
docker stop %CONTAINER_NAME% >nul 2>&1
docker rm %CONTAINER_NAME% >nul 2>&1

REM Estrategia: Pre-compilar localmente para evitar problemas de red en Docker
echo.
echo Paso 1: Compilando aplicacion localmente...
dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish
if errorlevel 1 (
    echo Error al compilar. Verifica que .NET SDK este instalado.
    echo Alternativa: dotnet run --project Products.Api/Products.Api.csproj
    cd Products.Api
    exit /b 1
)

echo.
echo Paso 2: Construyendo imagen Docker con binarios pre-compilados...
docker build -t %IMAGE_NAME%:latest -f Products.Api/Dockerfile.prebuilt .
if errorlevel 1 (
    echo Error al construir imagen Docker.
    echo Ejecutando sin Docker...
    cd Products.Api
    dotnet run --project Products.Api.csproj
    exit /b 0
)

echo Imagen construida exitosamente

REM Ejecutar contenedor
echo.
echo Paso 3: Iniciando contenedor...
docker run -d -p %PORT%:8080 -e ASPNETCORE_ENVIRONMENT=Development --name %CONTAINER_NAME% %IMAGE_NAME%:latest

REM Esperar
echo Esperando a que la API este lista...
timeout /t 3 /nobreak >nul

REM Volver al directorio original
cd Products.Api

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
