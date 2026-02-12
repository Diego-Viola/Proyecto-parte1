# Script de diagnóstico de tests
# Ejecuta este script para identificar los tests que fallan

Write-Host "=== DIAGNÓSTICO DE TESTS ===" -ForegroundColor Cyan
Write-Host ""

# Ejecutar tests con más detalle
Write-Host "Ejecutando tests con detalles..." -ForegroundColor Yellow
dotnet test --verbosity detailed --logger "console;verbosity=detailed"

Write-Host ""
Write-Host "=== EJECUTAR TESTS POR PROYECTO ===" -ForegroundColor Cyan
Write-Host ""

# Tests de Application
Write-Host "1. Tests de Application (Services):" -ForegroundColor Yellow
dotnet test WebApiTest.Application.Test\WebApiTest.Application.Test.csproj --verbosity normal
Write-Host ""

# Tests de Persistence
Write-Host "2. Tests de Persistence (Repositories):" -ForegroundColor Yellow
dotnet test WebApiTest.Persistence.Test\WebApiTest.Persistence.Test.csproj --verbosity normal
Write-Host ""

# Tests de Integration
Write-Host "3. Tests de Integration (Controllers):" -ForegroundColor Yellow
dotnet test WebApiTest.Integration.Test\WebApiTest.Integration.Test.csproj --verbosity normal
Write-Host ""

Write-Host "=== FIN DEL DIAGNÓSTICO ===" -ForegroundColor Cyan
