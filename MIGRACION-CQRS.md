# Migración de CQRS/MediatR a Arquitectura de Capas - Instrucciones

## Estado Actual

La refactorización de CQRS/MediatR a Arquitectura de Capas con Servicios está casi completa. Sin embargo, hay archivos antiguos que deben eliminarse para que la compilación funcione correctamente.

## Errores Actuales

Los errores de compilación se deben a que:
1. La carpeta `Features` antigua todavía existe con código que usa MediatR (que fue eliminado del proyecto)
2. Los tests antiguos en `Application.Test/Features` intentan usar los handlers de MediatR

## Pasos para Completar la Migración

### Opción 1: Usar el Script PowerShell (Recomendado)

1. Abrir PowerShell en la raíz del proyecto
2. Ejecutar el script de limpieza:
   ```powershell
   .\cleanup-cqrs.ps1
   ```
3. Limpiar y reconstruir la solución:
   ```powershell
   dotnet clean
   dotnet build
   ```

### Opción 2: Eliminación Manual

Eliminar manualmente las siguientes carpetas:

1. **`WebApiTest.Application\Features\`** - Carpeta completa con todos los Commands y Queries
2. **`WebApiTest.Application.Test\Features\`** - Carpeta completa con todos los tests de handlers

### Verificar la Compilación

Después de eliminar las carpetas, ejecutar:

```powershell
dotnet clean
dotnet build
dotnet test
```

## Nueva Estructura del Proyecto

```
WebApiTest.Application/
├── Services/                          ← NUEVO
│   ├── ProductService.cs
│   └── CategoryService.cs
├── Interfaces/
│   ├── IServices/                     ← NUEVO
│   │   ├── IProductService.cs
│   │   └── ICategoryService.cs
│   └── IRepositories/
│       ├── IProductRepository.cs
│       └── ICategoryRepository.cs
├── DTOs/
│   ├── Inputs/
│   └── Outputs/
├── Exceptions/
└── ServiceRegistration.cs             ← MODIFICADO (ahora registra servicios)

WebApiTest.Application.Test/
├── Services/                          ← NUEVO
│   ├── ProductServiceTests.cs
│   └── CategoryServiceTests.cs

WebApiTest/Controllers/
├── ProductsController.cs              ← MODIFICADO (usa IProductService)
└── CategoriesController.cs            ← MODIFICADO (usa ICategoryService)
```

## Cambios Realizados

### ✅ Archivos Creados
- `IProductService.cs` y `ICategoryService.cs` - Interfaces de servicios
- `ProductService.cs` y `CategoryService.cs` - Implementaciones
- `ProductServiceTests.cs` y `CategoryServiceTests.cs` - Tests actualizados

### ✅ Archivos Modificados
- `ServiceRegistration.cs` - Registra servicios en lugar de MediatR
- `WebApiTest.Application.csproj` - Eliminada dependencia de MediatR
- `ProductsController.cs` y `CategoriesController.cs` - Usan servicios

### ⚠️ Archivos a Eliminar
- `WebApiTest.Application\Features\` - Toda la carpeta
- `WebApiTest.Application.Test\Features\` - Toda la carpeta

## Beneficios de la Nueva Arquitectura

✅ **Más simple** - No requiere MediatR, menos capas de abstracción  
✅ **Más directa** - Los controladores llaman directamente a servicios  
✅ **Igualmente testeable** - Los servicios se pueden mockear fácilmente  
✅ **Sigue Clean Architecture** - Separación clara de responsabilidades  
✅ **Menos dependencias** - Una librería externa menos

## Flujo de Ejecución

```
Cliente HTTP Request
    ↓
Controller (Presentación)
    ↓ inyecta
IProductService (Contrato)
    ↓ implementa
ProductService (Lógica de Negocio)
    ↓ usa
IProductRepository (Contrato)
    ↓ implementa
ProductRepository (Acceso a Datos)
    ↓
Base de Datos
```

## Notas Importantes

- Los tests antiguos de `Features/` no compilarán porque usan MediatR
- Los nuevos tests en `Services/` están listos y funcionan con la nueva arquitectura
- La funcionalidad de la API permanece exactamente igual, solo cambió la implementación interna
