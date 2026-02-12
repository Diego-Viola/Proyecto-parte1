# Script para resolver el problema de archivos bloqueados
# Ejecutar este script para limpiar y resolver el error de deps.json

Write-Host "=== SOLUCIÓN DE ARCHIVOS BLOQUEADOS ===" -ForegroundColor Cyan
Write-Host ""

# 1. Cerrar procesos de .NET
Write-Host "Paso 1: Deteniendo procesos de .NET..." -ForegroundColor Yellow
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "testhost" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "VSTest.Console" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "✓ Procesos detenidos" -ForegroundColor Green
Write-Host ""

# 2. Limpiar carpetas bin y obj
Write-Host "Paso 2: Limpiando carpetas bin y obj..." -ForegroundColor Yellow
dotnet clean
Write-Host "✓ Limpieza completada" -ForegroundColor Green
Write-Host ""

# 3. Eliminar carpetas bin/obj manualmente
Write-Host "Paso 3: Eliminando carpetas bin/obj recursivamente..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | ForEach-Object {
    Write-Host "  Eliminando: $($_.FullName)" -ForegroundColor Gray
    Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}
Write-Host "✓ Carpetas eliminadas" -ForegroundColor Green
Write-Host ""

# 4. Restaurar paquetes
Write-Host "Paso 4: Restaurando paquetes NuGet..." -ForegroundColor Yellow
dotnet restore
Write-Host "✓ Paquetes restaurados" -ForegroundColor Green
Write-Host ""

# 5. Compilar solución
Write-Host "Paso 5: Compilando solución..." -ForegroundColor Yellow
dotnet build --no-restore
Write-Host ""

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ COMPILACIÓN EXITOSA" -ForegroundColor Green
    Write-Host ""
    
    # 6. Ejecutar tests
    Write-Host "Paso 6: Ejecutando tests..." -ForegroundColor Yellow
    dotnet test --no-build --verbosity normal
    Write-Host ""
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ TODOS LOS TESTS PASARON" -ForegroundColor Green
    } else {
        Write-Host "⚠️ ALGUNOS TESTS FALLARON - Revisar detalles arriba" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ ERROR EN LA COMPILACIÓN" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== PROCESO COMPLETADO ===" -ForegroundColor Cyan
