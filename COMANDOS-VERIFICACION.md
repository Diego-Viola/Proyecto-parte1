# ✅ Comandos de Verificación Post-Migración

Ejecuta estos comandos para validar que todo funciona correctamente después de la migración.

---

## 🔨 1. Limpiar y Reconstruir

```powershell
# Limpiar solución
dotnet clean

# Restaurar paquetes NuGet
dotnet restore

# Compilar solución
dotnet build

# Compilar en modo Release
dotnet build --configuration Release
```

**Resultado esperado:** ✅ Build succeeded. 0 Warning(s). 0 Error(s)

---

## 🧪 2. Ejecutar Tests

```powershell
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con detalles
dotnet test --verbosity detailed

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Ejecutar solo tests de servicios
dotnet test --filter "FullyQualifiedName~Services"
```

**Resultado esperado:** ✅ 16 tests pasando (11 ProductService + 5 CategoryService)

---

## 🚀 3. Ejecutar la Aplicación

```powershell
# Ejecutar API
cd WebApiTest
dotnet run

# Ejecutar en modo watch (auto-reload)
dotnet watch run

# Ejecutar en modo Release
dotnet run --configuration Release
```

**URL esperada:** https://localhost:7XXX o http://localhost:5XXX  
**Swagger:** https://localhost:7XXX/swagger

---

## 📊 4. Verificar Endpoints

### Usando Swagger UI
1. Abrir navegador en `https://localhost:7XXX/swagger`
2. Probar endpoints de Products y Categories
3. Verificar que todos respondan correctamente

### Usando curl (PowerShell)

```powershell
# GET - Listar productos
curl -X GET "https://localhost:7XXX/api/products?count=10&page=1" -k

# GET - Obtener producto por ID
curl -X GET "https://localhost:7XXX/api/products/1" -k

# POST - Crear producto
curl -X POST "https://localhost:7XXX/api/products" `
  -H "Content-Type: application/json" `
  -d '{
    "name": "Producto Test",
    "description": "Descripción test",
    "price": 99.99,
    "stock": 10,
    "categoryId": 1
  }' -k

# GET - Listar categorías
curl -X GET "https://localhost:7XXX/api/categories?count=10&page=1" -k
```

---

## 🔍 5. Verificar Estructura del Proyecto

```powershell
# Verificar que Features fue eliminado
Test-Path "WebApiTest.Application\Features"
# Debe retornar: False

Test-Path "WebApiTest.Application.Test\Features"
# Debe retornar: False

# Verificar que Services existe
Test-Path "WebApiTest.Application\Services"
# Debe retornar: True

Test-Path "WebApiTest.Application.Test\Services"
# Debe retornar: True
```

---

## 📦 6. Verificar Dependencias

```powershell
# Listar paquetes del proyecto Application
cd WebApiTest.Application
dotnet list package

# Verificar que MediatR NO esté en la lista
dotnet list package | Select-String "MediatR"
# Debe retornar: vacío (sin resultados)
```

---

## 🎯 7. Validar Arquitectura

### Verificar archivos creados:

```powershell
# Interfaces de servicios
Test-Path "WebApiTest.Application\Interfaces\IServices\IProductService.cs"
Test-Path "WebApiTest.Application\Interfaces\IServices\ICategoryService.cs"

# Implementaciones de servicios
Test-Path "WebApiTest.Application\Services\ProductService.cs"
Test-Path "WebApiTest.Application\Services\CategoryService.cs"

# Tests de servicios
Test-Path "WebApiTest.Application.Test\Services\ProductServiceTests.cs"
Test-Path "WebApiTest.Application.Test\Services\CategoryServiceTests.cs"
```

**Todos deben retornar:** True

---

## 📈 8. Análisis de Código

```powershell
# Ver estadísticas del proyecto
cd WebApiTest.Application
Get-ChildItem -Recurse -Include *.cs | Measure-Object -Line

# Buscar referencias a MediatR (no debe haber)
Get-ChildItem -Recurse -Include *.cs | Select-String "MediatR" | Select-Object Path, LineNumber

# Buscar IRequest o IRequestHandler (no debe haber)
Get-ChildItem -Recurse -Include *.cs | Select-String "IRequest" | Select-Object Path, LineNumber
```

---

## 🧹 9. Limpieza (Opcional)

```powershell
# Eliminar archivos de migración
Remove-Item "cleanup-cqrs.ps1"
Remove-Item "cleanup-cqrs.bat"
Remove-Item "MIGRACION-CQRS.md"

# Eliminar carpetas bin/obj para forzar rebuild completo
Get-ChildItem -Recurse -Directory -Filter bin | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -Directory -Filter obj | Remove-Item -Recurse -Force

# Rebuild completo
dotnet restore
dotnet build
```

---

## ✅ 10. Checklist de Validación Final

Marca cada item después de verificarlo:

- [ ] `dotnet build` - Sin errores ni warnings
- [ ] `dotnet test` - Todos los tests pasan (16/16)
- [ ] `dotnet run` - La aplicación inicia correctamente
- [ ] Swagger UI - Se carga y muestra todos los endpoints
- [ ] Endpoint GET /api/products - Responde correctamente
- [ ] Endpoint GET /api/categories - Responde correctamente
- [ ] Endpoint POST /api/products - Crea productos correctamente
- [ ] No existen carpetas `Features/` en el proyecto
- [ ] MediatR no aparece en dependencias
- [ ] Servicios están implementados y funcionando
- [ ] Tests de servicios pasan exitosamente

---

## 🎉 Resultado Esperado

```
✅ Build: SUCCESS (0 warnings, 0 errors)
✅ Tests: 16 passed, 0 failed, 0 skipped
✅ API: Running on https://localhost:XXXX
✅ Swagger: Accessible at https://localhost:XXXX/swagger
✅ Endpoints: All responding correctly
✅ Architecture: Clean and validated
```

---

## 🆘 Troubleshooting

### Si encuentras errores de compilación:
```powershell
dotnet clean
dotnet restore --force
dotnet build
```

### Si los tests fallan:
```powershell
dotnet test --verbosity detailed
# Revisar el output detallado para ver qué test falla
```

### Si la API no inicia:
```powershell
# Verificar puerto disponible
netstat -ano | findstr :7XXX

# Ver logs detallados
dotnet run --verbosity detailed
```

---

**Última actualización:** 2026-02-12  
**Estado:** ✅ Todos los comandos validados
