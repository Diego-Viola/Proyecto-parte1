# Script para limpiar archivos antiguos de CQRS/MediatR
# Ejecutar este script desde PowerShell en la raíz del proyecto

Write-Host "Eliminando archivos antiguos de CQRS/MediatR..." -ForegroundColor Yellow

# Eliminar carpeta Features de Application
$featuresAppPath = "WebApiTest.Application\Features"
if (Test-Path $featuresAppPath) {
    Remove-Item -Path $featuresAppPath -Recurse -Force
    Write-Host "✓ Eliminado: $featuresAppPath" -ForegroundColor Green
} else {
    Write-Host "✗ No encontrado: $featuresAppPath" -ForegroundColor Red
}

# Eliminar carpeta Features de Application.Test
$featuresTestPath = "WebApiTest.Application.Test\Features"
if (Test-Path $featuresTestPath) {
    Remove-Item -Path $featuresTestPath -Recurse -Force
    Write-Host "✓ Eliminado: $featuresTestPath" -ForegroundColor Green
} else {
    Write-Host "✗ No encontrado: $featuresTestPath" -ForegroundColor Red
}

Write-Host ""
Write-Host "Limpieza completada. Ejecute 'dotnet clean' y 'dotnet build' para reconstruir." -ForegroundColor Cyan
