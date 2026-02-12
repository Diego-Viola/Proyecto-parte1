# Script para compilar y verificar el proyecto
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Compilando Products.Api..." -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Limpiar build anterior
Write-Host "Limpiando build anterior..." -ForegroundColor Yellow
dotnet clean Products.Api.sln --nologo --verbosity quiet

# Compilar
Write-Host ""
Write-Host "Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build Products.Api.sln --nologo --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "================================" -ForegroundColor Green
    Write-Host "✅ COMPILACIÓN EXITOSA" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "El endpoint /health ahora estará visible en Swagger UI" -ForegroundColor Green
    Write-Host ""
    Write-Host "Para ejecutar la aplicación:" -ForegroundColor Cyan
    Write-Host "  dotnet run" -ForegroundColor White
    Write-Host ""
    Write-Host "Luego abre tu navegador en:" -ForegroundColor Cyan
    Write-Host "  https://localhost:{puerto}/" -ForegroundColor White
    Write-Host ""
    Write-Host "Busca el grupo 'Health' en Swagger UI" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "================================" -ForegroundColor Red
    Write-Host "❌ ERROR EN COMPILACIÓN" -ForegroundColor Red
    Write-Host "================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor revisa los errores arriba" -ForegroundColor Yellow
    Write-Host ""
}

exit $LASTEXITCODE
