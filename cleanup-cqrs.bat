@echo off
echo Eliminando archivos antiguos de CQRS/MediatR...
echo.

if exist "WebApiTest.Application\Features" (
    rmdir /s /q "WebApiTest.Application\Features"
    echo [OK] Eliminado: WebApiTest.Application\Features
) else (
    echo [X] No encontrado: WebApiTest.Application\Features
)

if exist "WebApiTest.Application.Test\Features" (
    rmdir /s /q "WebApiTest.Application.Test\Features"
    echo [OK] Eliminado: WebApiTest.Application.Test\Features
) else (
    echo [X] No encontrado: WebApiTest.Application.Test\Features
)

echo.
echo Limpieza completada. Ejecute 'dotnet clean' y 'dotnet build' para reconstruir.
pause
