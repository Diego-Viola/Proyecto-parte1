﻿# ============================================
# Script de Implementación de Cambios de Testing
# ============================================
# Este script implementa las recomendaciones del análisis
# de duplicación de tests (ANALISIS_DUPLICACION_TESTS.md)
#
# Uso: .\implement-test-changes.ps1
# Ejecutar desde: Products.Api/
# ============================================

$ErrorActionPreference = "Continue"

Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║  Implementación de Cambios - Optimización de Tests     ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""

$rootPath = ".."
$apiTestPath = "$rootPath\Products.Api.Test"
$appTestPath = "$rootPath\Products.Api.Application.Test"
$apiPath = "$rootPath\Products.Api"
$appPath = "$rootPath\Products.Api.Application"

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "Products.Api.csproj")) {
    Write-Host "Error: Ejecuta este script desde la carpeta Products.Api/" -ForegroundColor Red
    exit 1
}

Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "PASO 1: Eliminar tests duplicados de Controllers" -ForegroundColor Yellow
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Eliminar ProductsControllerTests.cs
$productsControllerTests = "$apiTestPath\Controllers\ProductsControllerTests.cs"
if (Test-Path $productsControllerTests) {
    Remove-Item $productsControllerTests -Force
    Write-Host "✓ Eliminado: ProductsControllerTests.cs" -ForegroundColor Green
} else {
    Write-Host "- Ya eliminado o no encontrado: ProductsControllerTests.cs" -ForegroundColor Gray
}

# Eliminar CategoriesControllerTests.cs
$categoriesControllerTests = "$apiTestPath\Controllers\CategoriesControllerTests.cs"
if (Test-Path $categoriesControllerTests) {
    Remove-Item $categoriesControllerTests -Force
    Write-Host "✓ Eliminado: CategoriesControllerTests.cs" -ForegroundColor Green
} else {
    Write-Host "- Ya eliminado o no encontrado: CategoriesControllerTests.cs" -ForegroundColor Gray
}

# Eliminar carpeta Controllers si está vacía
$controllersDir = "$apiTestPath\Controllers"
if (Test-Path $controllersDir) {
    $files = Get-ChildItem $controllersDir -File -ErrorAction SilentlyContinue
    if ($null -eq $files -or $files.Count -eq 0) {
        Remove-Item $controllersDir -Force -Recurse
        Write-Host "✓ Eliminada carpeta vacía: Controllers/" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "PASO 2: Mover ProductEnricherHelper a Application" -ForegroundColor Yellow
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Crear carpeta Helpers en Application si no existe
$appHelpersDir = "$appPath\Helpers"
if (-not (Test-Path $appHelpersDir)) {
    New-Item -ItemType Directory -Path $appHelpersDir -Force | Out-Null
    Write-Host "✓ Creada carpeta: Products.Api.Application/Helpers/" -ForegroundColor Green
} else {
    Write-Host "- Carpeta ya existe: Products.Api.Application/Helpers/" -ForegroundColor Gray
}

# Mover ProductEnricherHelper.cs (ya tiene namespace actualizado)
$helperSource = "$apiPath\Helpers\ProductEnricherHelper.cs"
$helperDest = "$appHelpersDir\ProductEnricherHelper.cs"
if (Test-Path $helperSource) {
    Move-Item -Path $helperSource -Destination $helperDest -Force
    Write-Host "✓ Movido: ProductEnricherHelper.cs -> Application/Helpers/" -ForegroundColor Green
} elseif (Test-Path $helperDest) {
    Write-Host "- Ya movido: ProductEnricherHelper.cs" -ForegroundColor Gray
} else {
    Write-Host "⚠ No encontrado: ProductEnricherHelper.cs" -ForegroundColor Yellow
}

# Eliminar carpeta Helpers de Api si está vacía
$apiHelpersDir = "$apiPath\Helpers"
if (Test-Path $apiHelpersDir) {
    $files = Get-ChildItem $apiHelpersDir -File -ErrorAction SilentlyContinue
    if ($null -eq $files -or $files.Count -eq 0) {
        Remove-Item $apiHelpersDir -Force -Recurse
        Write-Host "✓ Eliminada carpeta vacía: Products.Api/Helpers/" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "PASO 3: Mover ProductEnricherHelperTests a Application.Test" -ForegroundColor Yellow
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Crear carpeta Helpers en Application.Test si no existe
$appTestHelpersDir = "$appTestPath\Helpers"
if (-not (Test-Path $appTestHelpersDir)) {
    New-Item -ItemType Directory -Path $appTestHelpersDir -Force | Out-Null
    Write-Host "✓ Creada carpeta: Products.Api.Application.Test/Helpers/" -ForegroundColor Green
}

# Mover ProductEnricherHelperTests.cs
$helperTestSource = "$apiTestPath\Helpers\ProductEnricherHelperTests.cs"
$helperTestDest = "$appTestHelpersDir\ProductEnricherHelperTests.cs"
if (Test-Path $helperTestSource) {
    # Leer contenido y actualizar namespaces
    $content = Get-Content $helperTestSource -Raw
    $content = $content -replace "namespace Products\.Api\.Test\.Helpers", "namespace Products.Api.Application.Test.Helpers"
    $content = $content -replace "using Products\.Api\.Helpers;", "using Products.Api.Application.Helpers;"
    Set-Content -Path $helperTestDest -Value $content -Encoding UTF8
    Remove-Item $helperTestSource -Force
    Write-Host "✓ Movido: ProductEnricherHelperTests.cs -> Application.Test/Helpers/" -ForegroundColor Green
} else {
    Write-Host "- No encontrado: ProductEnricherHelperTests.cs" -ForegroundColor Gray
}

# Eliminar carpeta Helpers de Api.Test si está vacía
$apiTestHelpersDir = "$apiTestPath\Helpers"
if (Test-Path $apiTestHelpersDir) {
    $files = Get-ChildItem $apiTestHelpersDir -File
    if ($files.Count -eq 0) {
        Remove-Item $apiTestHelpersDir -Force -Recurse
        Write-Host "✓ Eliminada carpeta vacía: Products.Api.Test/Helpers/" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "PASO 4: Actualizar referencias en código principal" -ForegroundColor Yellow
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Actualizar referencias en ProductsController.cs
$productsController = "$apiPath\Controllers\ProductsController.cs"
if (Test-Path $productsController) {
    $content = Get-Content $productsController -Raw
    if ($content -match "using Products\.Api\.Helpers;") {
        $content = $content -replace "using Products\.Api\.Helpers;", "using Products.Api.Application.Helpers;"
        Set-Content -Path $productsController -Value $content -Encoding UTF8
        Write-Host "✓ Actualizado: ProductsController.cs (using statement)" -ForegroundColor Green
    } else {
        Write-Host "- ProductsController.cs ya tiene referencia correcta o no usa Helper" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "PASO 5: Actualizar archivos .csproj" -ForegroundColor Yellow
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Agregar referencia a Application en Api.csproj si usa el Helper
$apiCsproj = "$apiPath\Products.Api.csproj"
if (Test-Path $apiCsproj) {
    $content = Get-Content $apiCsproj -Raw
    # Verificar si ya tiene la referencia
    if ($content -notmatch "Products\.Api\.Application") {
        Write-Host "⚠ Verifica que Products.Api.csproj tenga referencia a Products.Api.Application" -ForegroundColor Yellow
    } else {
        Write-Host "✓ Products.Api.csproj ya tiene referencia a Application" -ForegroundColor Green
    }
}

# Verificar que Application.Test tiene referencia a Application
$appTestCsproj = "$appTestPath\Products.Api.Application.Test.csproj"
if (Test-Path $appTestCsproj) {
    Write-Host "✓ Application.Test.csproj existe" -ForegroundColor Green
} else {
    Write-Host "⚠ No encontrado: Products.Api.Application.Test.csproj" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "RESUMEN DE CAMBIOS" -ForegroundColor Green
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

Write-Host "Archivos eliminados:" -ForegroundColor Yellow
Write-Host "  - Products.Api.Test/Controllers/ProductsControllerTests.cs"
Write-Host "  - Products.Api.Test/Controllers/CategoriesControllerTests.cs"
Write-Host ""

Write-Host "Archivos movidos:" -ForegroundColor Yellow
Write-Host "  - ProductEnricherHelper.cs -> Products.Api.Application/Helpers/"
Write-Host "  - ProductEnricherHelperTests.cs -> Products.Api.Application.Test/Helpers/"
Write-Host ""

Write-Host "Referencias actualizadas:" -ForegroundColor Yellow
Write-Host "  - ProductsController.cs (using statement)"
Write-Host ""

Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║         ✓ CAMBIOS IMPLEMENTADOS                        ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

Write-Host "Próximos pasos:" -ForegroundColor Yellow
Write-Host "1. Ejecutar: dotnet build Products.Api.sln" -ForegroundColor Cyan
Write-Host "2. Ejecutar: dotnet test Products.Api.sln" -ForegroundColor Cyan
Write-Host "3. Verificar que todos los tests pasen" -ForegroundColor Cyan
Write-Host ""
