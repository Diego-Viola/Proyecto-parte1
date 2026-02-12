# Script de limpieza de archivos obsoletos de WebApiTest
# Este script elimina todos los vestigios del nombre anterior "WebApiTest"
# y regenera los archivos de build con el nombre correcto "Products.Api"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Limpieza de Vestigios de WebApiTest" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Obtener el directorio del proyecto
$projectPath = "C:\Users\DV84056\Desktop\Repos\Proyecto-parte1\Products.Api"
Set-Location $projectPath

Write-Host "Directorio del proyecto: $projectPath" -ForegroundColor Yellow
Write-Host ""

# Función para eliminar archivo/carpeta con manejo de errores
function Remove-ItemSafely {
    param (
        [string]$Path,
        [string]$Description
    )
    
    if (Test-Path $Path) {
        try {
            Remove-Item -Path $Path -Recurse -Force -ErrorAction Stop
            Write-Host "[OK] Eliminado: $Description" -ForegroundColor Green
        }
        catch {
            Write-Host "[ERROR] No se pudo eliminar: $Description - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    else {
        Write-Host "[INFO] No existe: $Description" -ForegroundColor Gray
    }
}

Write-Host "Paso 1: Eliminando archivos y carpetas obsoletos..." -ForegroundColor Cyan
Write-Host ""

# Eliminar archivos específicos de WebApiTest
Remove-ItemSafely -Path "WebApiTest.http" -Description "WebApiTest.http"
Remove-ItemSafely -Path "WebApiTest.sln.DotSettings.user" -Description "WebApiTest.sln.DotSettings.user"

# Eliminar carpetas de caché
Remove-ItemSafely -Path ".vs" -Description "Carpeta .vs (caché de Visual Studio)"
Remove-ItemSafely -Path ".idea\.idea.WebApiTest" -Description "Carpeta .idea\.idea.WebApiTest (caché de Rider)"
Remove-ItemSafely -Path "obj" -Description "Carpeta obj (archivos intermedios)"
Remove-ItemSafely -Path "bin" -Description "Carpeta bin (binarios compilados)"

Write-Host ""
Write-Host "Paso 2: Limpiando la solución con dotnet clean..." -ForegroundColor Cyan
Write-Host ""

try {
    dotnet clean --nologo
    Write-Host "[OK] Limpieza completada con dotnet clean" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] Error al ejecutar dotnet clean: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Paso 3: Restaurando paquetes NuGet..." -ForegroundColor Cyan
Write-Host ""

try {
    dotnet restore --nologo
    Write-Host "[OK] Paquetes restaurados correctamente" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] Error al restaurar paquetes: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Paso 4: Compilando la solución..." -ForegroundColor Cyan
Write-Host ""

try {
    dotnet build --nologo --no-restore
    Write-Host "[OK] Compilación completada exitosamente" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] Error al compilar: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Limpieza completada!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Resumen:" -ForegroundColor Yellow
Write-Host "- Se eliminaron archivos y carpetas obsoletos de WebApiTest" -ForegroundColor White
Write-Host "- Se limpiaron los archivos de build" -ForegroundColor White
Write-Host "- Se restauraron los paquetes NuGet" -ForegroundColor White
Write-Host "- Se recompiló la solución con el nombre correcto (Products.Api)" -ForegroundColor White
Write-Host ""
Write-Host "Nota: Los nuevos archivos de build ahora usan el nombre 'Products.Api'" -ForegroundColor Green
Write-Host ""
Write-Host "Puedes verificar el archivo RENAMING_SUMMARY.md para más detalles." -ForegroundColor Cyan
Write-Host ""
