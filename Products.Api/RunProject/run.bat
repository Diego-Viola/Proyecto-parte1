@echo off
REM ============================================
REM Products.Api - Script de Ejecución (CMD)
REM ============================================
REM Uso: run.bat
REM Ejecutar desde: Products.Api/RunProject/
REM
REM Esta versión compila localmente para evitar
REM problemas de red (NU1301) en Docker
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

REM Verificar .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK no esta instalado
    echo Instala desde: https://dotnet.microsoft.com/download
    echo.
    echo Alternativa: intentar build dentro de Docker ^(puede fallar por red^)
    echo   docker build -t products-api:latest -f Dockerfile ..\..
    exit /b 1
)

set IMAGE_NAME=products-api
set CONTAINER_NAME=products-api
set PORT=5000
set ROOT_DIR=..\..

REM Detener contenedor existente
echo Deteniendo contenedor existente si existe...
docker stop %CONTAINER_NAME% >nul 2>&1
docker rm %CONTAINER_NAME% >nul 2>&1

REM Paso 1: Compilar localmente (evita problemas de red en Docker)
echo.
echo Paso 1: Compilando aplicacion localmente...
echo          ^(esto evita errores NU1301 en Docker^)

pushd %ROOT_DIR%
dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish --nologo
if errorlevel 1 (
    echo Error al compilar.
    echo Verifica que .NET SDK este instalado y las dependencias sean correctas.
    popd
    exit /b 1
)
echo Compilacion exitosa
popd

REM Paso 2: Construir imagen Docker con binarios pre-compilados
echo.
echo Paso 2: Construyendo imagen Docker...
docker build -t %IMAGE_NAME%:latest -f Dockerfile.prebuilt %ROOT_DIR%
if errorlevel 1 (
    echo Error al construir imagen Docker.
    echo.
    echo Alternativa: ejecutar localmente sin Docker
    echo   cd ..\
    echo   dotnet run --project Products.Api.csproj
    exit /b 1
)

echo Imagen construida exitosamente

REM Paso 3: Ejecutar contenedor
echo.
echo Paso 3: Iniciando contenedor...
docker run -d -p %PORT%:8080 -e ASPNETCORE_ENVIRONMENT=Development --name %CONTAINER_NAME% %IMAGE_NAME%:latest

REM Esperar
echo Esperando a que la API este lista...
timeout /t 3 /nobreak >nul

REM Limpiar carpeta publish
echo Limpiando archivos temporales...
rmdir /s /q %ROOT_DIR%\publish >nul 2>&1

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
