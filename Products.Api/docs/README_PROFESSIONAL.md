# Products API - Prueba Técnica Backend

## 📋 Descripción del Problema

**Objetivo:** Construir una API backend que provea toda la información necesaria para soportar una página de detalle de ítem, inspirada en una tienda/marketplace (estilo MercadoLibre).

La API debe entregar de manera eficiente los detalles requeridos del producto y la información relacionada, alineándose con las mejores prácticas de desarrollo backend.

### Requerimientos Funcionales
- Endpoint principal que obtiene los detalles completos del producto
- Información enriquecida para renderizar una página de detalle completa
- Datos simulados de: imágenes, vendedor, envío, variantes, atributos, ratings, productos relacionados

### Requerimientos No Funcionales
- ✅ Buen manejo de errores
- ✅ Documentación (Swagger/OpenAPI)
- ✅ Testing (unitarios e integración)
- ✅ Logging y trazabilidad
- ✅ Validación robusta

---

## 🏗️ Arquitectura

El proyecto implementa **Clean Architecture** con separación en 4 capas:

```
┌─────────────────────────────────────────────────────────────┐
│                    Products.Api                              │
│            (Presentación: Controllers, Middlewares)          │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                Products.Api.Application                      │
│          (Servicios, DTOs, Interfaces, Lógica de negocio)    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  Products.Api.Domain                         │
│            (Entidades, Excepciones de dominio)               │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                Products.Api.Persistence                      │
│            (Repositorios, Contexto JSON, Adapters)           │
└──────��──────────────────────────────────────────────────────┘
```

### Decisiones Arquitectónicas Clave

1. **Clean Architecture**: Permite testabilidad, mantenibilidad y flexibilidad para cambiar implementaciones
2. **Repository Pattern**: Abstracción del acceso a datos, preparado para migrar a base de datos real
3. **Persistencia JSON**: Cumple con el requisito de simular persistencia sin base de datos real
4. **Middleware Pipeline**: Manejo centralizado de errores, logging y correlation ID

---

## 🚀 Cómo Correr el Proyecto

### Requisitos Previos
- .NET 8.0 SDK o superior
- IDE: Visual Studio 2022, JetBrains Rider, o VS Code

### Instalación y Ejecución

```powershell
# 1. Clonar el repositorio
git clone <repository-url>
cd Products.Api

# 2. Restaurar dependencias
dotnet restore

# 3. Compilar la solución
dotnet build

# 4. Ejecutar la aplicación
dotnet run --project Products.Api.csproj

# 5. Acceder a Swagger UI
# Abrir en navegador: http://localhost:5000 o https://localhost:5001
```

### Endpoints Principales

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/v1/products/{id}/detail` | **Detalle completo para marketplace** |
| GET | `/api/v1/products/{id}` | Detalle básico del producto |
| GET | `/api/v1/products` | Lista paginada de productos |
| GET | `/api/v1/products/{id}/related` | Productos relacionados |
| GET | `/api/v1/categories` | Lista de categorías |
| GET | `/health` | Health check |

---

## 🧪 Cómo Ejecutar Tests

### Ejecutar Todos los Tests

```powershell
# Desde la raíz de la solución
dotnet test
```

### Ejecutar Tests por Proyecto

```powershell
# Tests unitarios y de integración de la API
dotnet test Products.Api.Test

# Tests de la capa de aplicación
dotnet test Products.Api.Application.Test

# Tests de la capa de persistencia
dotnet test Products.Api.Persistence.Test
```

### Ejecutar Tests con Filtros

```powershell
# Solo tests unitarios
dotnet test --filter "FullyQualifiedName~Unit"

# Solo tests de integración
dotnet test --filter "FullyQualifiedName~Integration"

# Tests de un controlador específico
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"
```

---

## 📊 Cómo Generar Reporte de Cobertura

### Opción 1: Usando Coverlet (Recomendado)

```powershell
# 1. Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# 2. Instalar ReportGenerator (una vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# 3. Generar reporte HTML
reportgenerator `
  -reports:"**/coverage.cobertura.xml" `
  -targetdir:"coveragereport" `
  -reporttypes:Html

# 4. Abrir el reporte
start coveragereport/index.html
```

### Opción 2: Script Completo

```powershell
# Script todo-en-uno
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

reportgenerator `
  -reports:"./coverage/**/coverage.cobertura.xml" `
  -targetdir:"./coveragereport" `
  -reporttypes:"Html;Badges;TextSummary"

# Ver resumen en consola
Get-Content ./coveragereport/Summary.txt
```

### Estructura del Reporte

```
coveragereport/
├── index.html          # Reporte principal (abrir en navegador)
├── Summary.txt         # Resumen en texto
└── badge_*.svg         # Badges de cobertura
```

---

## 📈 Estado de Cobertura de Tests

### Resumen de Cobertura (Estimado)

| Capa | Cobertura Estimada | Estado |
|------|-------------------|--------|
| Controllers | ~85% | ✅ Cubierto |
| Middlewares | ~90% | ✅ Cubierto |
| Helpers | ~95% | ✅ Cubierto |
| Validators | ~100% | ✅ Cubierto |
| Integración | ~80% | ✅ Cubierto |

### Tests Implementados

#### Tests Unitarios (Products.Api.Test/Unit)
- **ProductsControllerTests** (12 tests)
  - GetAll con/sin productos
  - GetById existente/no existente
  - GetDetailById (endpoint principal)
  - GetRelatedProducts
  - Delete

- **CategoriesControllerTests** (6 tests)
  - GetAll con/sin categorías
  - GetById existente/no existente
  - Create válido/duplicado

- **ProductEnricherHelperTests** (11 tests)
  - Enriquecimiento completo
  - Consistencia de datos por ID
  - Campos requeridos
  - Estados de stock
  - SKU y Permalink

- **CorrelationIdMiddlewareTests** (6 tests)
  - Generación de nuevo ID
  - Uso de ID existente
  - Headers de respuesta

- **ExceptionHandlerMiddlewareTests** (8 tests)
  - 404 NotFoundException
  - 400 BadRequest/InputException
  - 422 BusinessException
  - 500 Generic Exception
  - 503 TimeoutException

- **CreateProductRequestValidatorTests** (15 tests)
  - Validación de Name, Description, Price, Stock, CategoryId
  - Casos de borde

#### Tests de Integración (Products.Api.Test/Integration)
- **ProductsEndpointsTests** (13 tests)
  - GET /products (paginación, estructura)
  - GET /products/{id}
  - GET /products/{id}/detail (endpoint principal)
  - GET /products/{id}/related
  - POST /products
  - DELETE /products/{id}
  - Correlation ID

- **CategoriesEndpointsTests** (7 tests)
  - GET /categories
  - GET /categories/{id}
  - POST /categories

- **HealthEndpointsTests** (7 tests)
  - Estado healthy
  - Estructura de respuesta
  - Información de app

### Áreas No Cubiertas / Limitaciones

- **RequestResponseLoggingMiddleware**: Requiere configuración adicional de streams
- **Swagger Configuration**: Código de configuración, no crítico
- **Mapster Mappings**: Cubiertos implícitamente por tests de integración
- **Excepciones de infraestructura**: DataIntegrationException (difícil de simular)

---

## 🔧 Supuestos y Limitaciones

### Supuestos

1. **Datos Simulados**: Los datos de vendedor, envío, ratings son generados algorítmicamente basándose en el ID del producto (seed consistente)
2. **Productos Pre-cargados**: El sistema inicia con datos de ejemplo en el archivo JSON
3. **Single-tenant**: No hay autenticación ni autorización (fuera del alcance)
4. **Moneda ARS**: Precios en pesos argentinos por defecto

### Limitaciones Técnicas

1. **Persistencia JSON**: 
   - No soporta transacciones ACID
   - Thread-safe con locks básicos
   - No escalable a producción

2. **Enriquecimiento de Productos**:
   - Datos simulados, no vienen de servicios reales
   - En producción requeriría: servicio de sellers, servicio de shipping, servicio de ratings

3. **Testing**:
   - Tests de integración usan la misma persistencia JSON
   - No hay contenedores Docker para aislamiento

### Mejoras Futuras (Fuera del Alcance)

- [ ] Autenticación JWT
- [ ] Rate limiting
- [ ] Caché distribuido (Redis)
- [ ] Base de datos real (PostgreSQL/MongoDB)
- [ ] Containerización (Docker)
- [ ] CI/CD pipeline

---

## 🛠️ Stack Tecnológico

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | Web API |
| FluentValidation | 11.3.0 | Validación de requests |
| Serilog | 9.0.0 | Logging estructurado |
| Mapster | 7.4.0 | Object mapping |
| Swashbuckle | 9.0.3 | Documentación OpenAPI |
| xUnit | 2.9.3 | Framework de testing |
| FluentAssertions | 8.0.0 | Assertions expresivas |
| Moq | 4.20.72 | Mocking |
| Coverlet | 6.0.4 | Cobertura de código |

---

## 📞 Contacto

Desarrollado como prueba técnica para demostrar competencias en desarrollo backend con .NET.

**Fecha**: Febrero 2026  
**Versión**: 1.0
