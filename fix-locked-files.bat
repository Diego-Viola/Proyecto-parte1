@echo off
echo === SOLUCION DE ARCHIVOS BLOQUEADOS ===
echo.

echo Paso 1: Deteniendo procesos de .NET...
taskkill /F /IM dotnet.exe 2>nul
taskkill /F /IM testhost.exe 2>nul
taskkill /F /IM VSTest.Console.exe 2>nul
timeout /t 2 /nobreak >nul
echo [OK] Procesos detenidos
echo.

echo Paso 2: Limpiando con dotnet clean...
dotnet clean
echo [OK] Limpieza completada
echo.

echo Paso 3: Eliminando carpetas bin/obj...
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s /q "%%d" 2>nul
echo [OK] Carpetas eliminadas
echo.

echo Paso 4: Restaurando paquetes NuGet...
dotnet restore
echo [OK] Paquetes restaurados
echo.

echo Paso 5: Compilando solucion...
dotnet build --no-restore
echo.

if %ERRORLEVEL% EQU 0 (
    echo [OK] COMPILACION EXITOSA
    echo.
    echo Paso 6: Ejecutando tests...
    dotnet test --no-build --verbosity normal
    echo.
    if %ERRORLEVEL% EQU 0 (
        echo [OK] TODOS LOS TESTS PASARON
    ) else (
        echo [!] ALGUNOS TESTS FALLARON - Revisar detalles arriba
    )
) else (
    echo [X] ERROR EN LA COMPILACION
)

echo.
echo === PROCESO COMPLETADO ===
pause
