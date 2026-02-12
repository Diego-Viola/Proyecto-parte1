﻿﻿# Products API - Sistema de Gestión de Productos y Categorías

## 📋 Descripción General

**Products API** es una API RESTful desarrollada en .NET 8 que proporciona un sistema completo de gestión de productos y categorías. La solución implementa una arquitectura limpia multicapa con persistencia en archivos JSON, logging avanzado, y documentación interactiva con Swagger.

## 🏗️ Arquitectura de la Solución

La aplicación sigue los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**, separando las responsabilidades en capas bien definidas:

```
┌─────────────────────────────────────────────────────────────────┐
│                        Products.Api (Capa de Presentación)      │
│  Controllers | Middlewares | Swagger | Health Checks           │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              Products.Api.Application (Capa de Aplicación)      │
│  Services | DTOs | Interfaces | Mappers | Business Logic       │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                Products.Api.Domain (Capa de Dominio)            │
│  Entities | Models | Domain Exceptions | Business Rules         │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│           Products.Api.Persistence (Capa de Infraestructura)    │
│  Repositories | Adapters | CustomContext | Data Access          │
└─────────────────────────────────────────────────────────────────┘
```

### Proyectos de la Solución

1. **Products.Api** - API Web principal
2. **Products.Api.Application** - Lógica de aplicación y casos de uso
3. **Products.Api.Domain** - Modelos de dominio y reglas de negocio
4. **Products.Api.Persistence** - Acceso a datos y persistencia
5. **Products.Api.Application.Test** - Tests unitarios de aplicación
6. **Products.Api.Persistence.Test** - Tests unitarios de persistencia

## 🔄 Flujo de Funcionamiento del Sistema

### Flujo de una Petición HTTP

```
┌─────────┐
│ Cliente │
└────┬────┘
     │ 1. HTTP Request
     ▼
┌─────────────────────────────────────────────┐
│         CorrelationIdMiddleware             │
│  - Genera/Valida X-Correlation-ID           │
│  - Asigna TraceIdentifier                   │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│    RequestResponseLoggingMiddleware         │
│  - Log de Request (método, URL, body)       │
│  - Log de Response (status, body)           │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│       ExceptionHandlerMiddleware            │
│  - Captura excepciones globales             │
│  - Transforma a ErrorResponse               │
│  - InputException → 400                     │
│  - NotFoundException → 404                  │
│  - BusinessException → 422                  │
│  - DataIntegrationException → 500           │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│           Controller Layer                  │
│  - ProductsController                       │
│  - CategoriesController                     │
│  - Validación de ModelState                 │
│  - InvalidModelStateHandler                 │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│          Application Layer                  │
│  - ProductService / CategoryService         │
│  - Lógica de negocio                        │
│  - Validaciones de dominio                  │
│  - Transformación DTOs                      │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│         Repository Layer                    │
│  - ProductRepository                        │
│  - CategoryRepository                       │
│  - Adapters (Entity ↔ Domain)              │
└────────────────┬────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────┐
│          CustomContext                      │
│  - Persistencia en JSON                     │
│  - Thread-safe operations (lock)            │
│  - Data/data.json                           │
└─────────────────────────────────────────────┘
```

## 🎯 Componentes Principales y Funcionamiento

### 1. **Middlewares (Pipeline de Peticiones)**

#### CorrelationIdMiddleware
- **Función**: Genera o valida el header `X-Correlation-ID` para trazabilidad
- **Funcionamiento**:
  ```csharp
  1. Lee el header X-Correlation-ID de la petición
  2. Si no existe, genera un nuevo GUID
  3. Lo almacena en context.Items
  4. Lo agrega a los headers de respuesta
  5. Lo asigna como TraceIdentifier para logging
  ```

#### RequestResponseLoggingMiddleware
- **Función**: Registra todas las peticiones y respuestas HTTP
- **Funcionamiento**:
  ```csharp
  1. Captura el body de la petición
  2. Log: método HTTP, URL, body
  3. Intercepta el stream de respuesta
  4. Captura el body de la respuesta
  5. Log: status code, body de respuesta
  ```

#### ExceptionHandlerMiddleware
- **Función**: Manejo centralizado de excepciones
- **Funcionamiento**:
  ```csharp
  1. Try-Catch alrededor del pipeline
  2. Captura excepciones según su tipo:
     - InputException → 400 Bad Request
     - BadRequestException → 400
     - NotFoundException → 404 Not Found
     - BusinessException → 422 Unprocessable Entity
     - DataIntegrationException → 500
     - TimeoutException → 503 Service Unavailable
     - Exception → 500 Internal Server Error
  3. Construye ErrorResponse estandarizado
  4. Retorna JSON con detalles del error
  ```

### 2. **Controllers (Capa de Presentación)**

#### ProductsController
**Endpoints:**
- `GET /api/v1/products` - Lista paginada con filtros (count=20, page=1 por defecto)
  - Query params: count, page, name (opcional), categoryId (opcional)
- `GET /api/v1/products/{id}` - Detalle básico de un producto
- `GET /api/v1/products/{id}/detail` - **🆕 Detalle completo estilo marketplace**
  - Incluye: imágenes, vendedor, envío, variantes, atributos, ratings, productos relacionados
- `GET /api/v1/products/{id}/related` - **🆕 Productos relacionados**
  - Query params: limit (default: 6)
- `POST /api/v1/products` - Crear producto
- `PUT /api/v1/products/{id}` - Actualizar producto
- `DELETE /api/v1/products/{id}` - Eliminar producto

#### CategoriesController
**Endpoints:**
- `GET /api/v1/categories` - Lista paginada de categorías (count=20, page=1 por defecto)
- `GET /api/v1/categories/{id}` - Detalle de categoría
- `POST /api/v1/categories` - Crear categoría

**Características:**
- Rutas en minúsculas (LowercaseControllerModelConvention)
- Versionado de API (v1)
- Validación automática de ModelState
- Respuestas estandarizadas (200, 201, 204, 400, 401, 422, 500)
- Documentación Swagger integrada

### 3. **Application Layer (Servicios)**

#### ProductService
**Responsabilidades:**
- Implementa lógica de negocio de productos
- Validaciones:
  - Nombre único al crear
  - Categoría existente
  - Producto existe para operaciones
  - Precio y stock válidos
- Transformación: Domain Models ↔ DTOs

**Flujo CreateAsync:**
```csharp
1. Recibe CreateProductInput (DTO)
2. Valida que no exista producto con mismo nombre
3. Verifica que la categoría exista
4. Valida precio > 0 y stock >= 0
5. Crea modelo de dominio (Product)
6. Llama al repositorio para persistir
7. Transforma resultado a ProductDetailOutput
8. Retorna DTO de salida
```

#### CategoryService
**Responsabilidades:**
- Gestión de categorías
- Validación de nombres únicos
- Paginación de resultados

### 4. **Domain Layer (Modelos de Dominio)**

**Product Model:**
```csharp
- Id: long
- Name: string
- Description: string
- Price: decimal
- Stock: int
- CategoryId: long
```

**Category Model:**
```csharp
- Id: long
- Name: string
```

**Excepciones de Dominio:**
- `BadRequestException` - Datos inválidos
- `NotFoundException` - Recurso no encontrado
- `BusinessException` - Reglas de negocio violadas
- `DataIntegrationException` - Errores de persistencia

### 5. **Persistence Layer (Infraestructura)**

#### CustomContext
- **Función**: Contexto de datos personalizado con persistencia en JSON
- **Funcionamiento**:
  ```csharp
  1. Constructor inicializa ruta del archivo JSON
  2. Si no existe, crea datos por defecto:
     - 3 categorías: Electrónica, Hogar, Deportes
     - 5 productos de ejemplo
  3. Carga datos existentes desde JSON
  4. SaveChanges() serializa en memoria a JSON
  ```

#### Repositories
- **ProductRepository / CategoryRepository**
- **Patrón**: Repository Pattern
- **Thread-Safety**: Uso de `lock` para operaciones concurrentes
- **Operaciones CRUD completas**
- **Paginación implementada**: Skip/Take

#### Adapters
- **Función**: Conversión Entity ↔ Domain Model
- **Patrón**: Adapter Pattern
- **Implementa**: IAdapter<TEntity, TDomain>

### 6. **Configuración y Servicios**

#### Dependency Injection
```csharp
Program.cs:
- AddApplicationServices() → Registra services y mappers
- AddInfrastructureService() → Registra repositories y context
- Singleton: CustomContext, Adapters
- Scoped: Services, Repositories
```

#### Logging con Serilog
```csharp
- Configuración desde appsettings.json
- Sinks: Console + File (rollingInterval: Day)
- MinimumLevel: Information
- Logs en: Logs/appYYYYMMDD.log
```

#### API Versioning
```csharp
- DefaultApiVersion: v1.0
- AssumeDefaultVersionWhenUnspecified: true
- ReportApiVersions: true
- URL: /api/v1/[controller]
```

#### Swagger/OpenAPI
```csharp
- Solo en Development
- SwaggerUI en raíz (/)
- Muestra todas las versiones
- SwaggerDefaultValues para descripciones
- SwaggerOperation attributes en controllers
```

### 7. **Health Checks**

#### AppInfoHealthCheck
- **Endpoint**: `/health`
- **Respuesta JSON**:
  ```json
  {
    "status": "Healthy",
    "checks": [
      {
        "name": "app_info",
        "status": "Healthy",
        "description": "Application Information"
      }
    ]
  }
  ```

## 🛠️ Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | Web API |
| Serilog | 9.0.0 | Logging estructurado |
| Mapster | 7.4.0 | Object mapping |
| Swashbuckle | 9.0.3 | Documentación OpenAPI |
| Asp.Versioning | 8.1.0 | Versionado de API |
| xUnit | - | Testing framework |
| FluentAssertions | - | Assertions para tests |
| Moq | - | Mocking framework |

## 🚀 Instalación y Ejecución

### Requisitos Previos
- .NET 8.0 SDK o superior
- IDE: Visual Studio 2022, Rider, o VS Code

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd Products.Api
   ```

2. **Restaurar dependencias**
   ```powershell
   dotnet restore
   ```

3. **Compilar la solución**
   ```powershell
   dotnet build
   ```

4. **Ejecutar la aplicación**
   ```powershell
   dotnet run --project Products.Api.csproj
   ```

5. **Acceder a Swagger UI**
   ```
   http://localhost:5000
   o
   https://localhost:5001
   ```

### Ejecutar Tests

```powershell
# Todos los tests
dotnet test

# Tests de un proyecto específico
dotnet test Products.Api.Application.Test
dotnet test Products.Api.Persistence.Test

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Estructura de Archivos

```
Products.Api/
├── Controllers/
│   ├── BaseApiController.cs           # Controlador base con [ApiVersion]
│   ├── ProductsController.cs          # Endpoints de productos
│   ├── CategoriesController.cs        # Endpoints de categorías
│   ├── Requests/                      # DTOs de entrada
│   │   ├── CreateProductRequest.cs
│   │   ├── UpdateProductRequest.cs
│   │   └── CreateCategoryRequest.cs
│   └── Responses/                     # 🆕 DTOs de salida enriquecidos
│       └── ProductDetailEnrichedResponse.cs  # Modelo completo de marketplace
├── Middlewares/
│   ├── CorrelationIdMiddleware.cs     # Trazabilidad de requests
│   ├── RequestResponseLoggingMiddleware.cs  # Logging HTTP
│   └── ExceptionHandlerMiddleware.cs  # Manejo de errores
├── Configs/
│   └── LowercaseControllerModelConvention.cs  # Rutas lowercase
├── Handlers/
│   └── InvalidModelStateHandler.cs    # Validación de modelos
├── Helpers/
│   └── ProductEnricherHelper.cs       # 🆕 Enriquecedor de productos para marketplace
├── HealthChecks/
│   └── AppInfoHealthCheck.cs          # Health check personalizado
├── Swagger/
│   ├── ConfigureSwaggerOptions.cs     # Configuración Swagger
│   └── SwaggerDefaultValues.cs        # Valores por defecto
├── Common/
│   └── ErrorResponse.cs               # Modelo de respuesta de error
├── Exceptions/
│   └── InputException.cs              # Excepciones de entrada
├── docs/
│   └── DECISIONS.md                   # 🆕 Registro de decisiones arquitectónicas (ADR)
├── Logs/                              # Archivos de log de Serilog
├── Data/                              # Base de datos JSON
│   └── data.json                      # Persistencia de datos
├── Program.cs                         # Configuración y bootstrap
├── appsettings.json                   # Configuración general
└── appsettings.Development.json       # Configuración desarrollo
```

## 📊 Modelo de Datos

### Estructura del archivo data.json

```json
{
  "Categories": [
    {
      "Id": 1,
      "Name": "Electrónica"
    }
  ],
  "Products": [
    {
      "Id": 1,
      "Name": "Smartphone",
      "Description": "Teléfono inteligente de última generación",
      "Price": 999.99,
      "Stock": 10,
      "CategoryId": 1
    }
  ]
}
```

## 🔐 Formato de Respuestas

### Respuesta Exitosa (200/201)
```json
{
  "id": 1,
  "name": "Smartphone",
  "description": "Teléfono inteligente",
  "price": 999.99,
  "stock": 10,
  "categoryId": 1
}
```

### Respuesta Paginada (200)
```json
{
  "items": [...],
  "total": 100
}
```

### Respuesta de Error (4xx/5xx)
```json
{
  "status": 400,
  "code": "400",
  "detail": "El producto ya existe",
  "instance": "/api/v1/products",
  "traceId": "00-abc123-def456-00"
}
```

## 📝 Ejemplos de Uso

### 🆕 Obtener Detalle Completo de Producto (Marketplace)

```http
GET /api/v1/products/1/detail
```

**Respuesta:**
```json
{
  "id": 1,
  "name": "Smartphone",
  "description": "Teléfono inteligente de última generación",
  "sku": "SKU-001-000001",
  "condition": "new",
  "price": {
    "amount": 999.99,
    "currency": "ARS",
    "originalAmount": 1299.99,
    "discountPercentage": 23,
    "paymentMethods": [
      {
        "type": "credit_card",
        "name": "Visa, Mastercard",
        "installments": 12,
        "installmentAmount": 83.33,
        "interestFree": true
      }
    ]
  },
  "stock": {
    "availableQuantity": 10,
    "status": "available",
    "maxPurchaseQuantity": 6
  },
  "images": [
    {
      "id": "img-1-1",
      "url": "https://cdn.marketplace.com/products/1/image-1.jpg",
      "thumbnailUrl": "https://cdn.marketplace.com/products/1/thumb-1.jpg",
      "order": 1,
      "isPrimary": true
    }
  ],
  "category": { "id": 1, "name": "Electrónica" },
  "breadcrumbs": [
    { "id": 1, "name": "Inicio", "level": 0 },
    { "id": 100, "name": "Categorías", "level": 1 },
    { "id": 1, "name": "Electrónica", "level": 2 }
  ],
  "seller": {
    "id": 1,
    "name": "TechStore Oficial",
    "reputation": {
      "level": "gold",
      "totalSales": 15000,
      "positiveRating": 98.5
    },
    "location": { "city": "Buenos Aires", "country": "Argentina" }
  },
  "attributes": [
    { "id": "brand", "name": "Marca", "value": "Generic Brand" },
    { "id": "model", "name": "Modelo", "value": "Model-1" }
  ],
  "shipping": {
    "freeShipping": true,
    "options": [
      {
        "id": "standard",
        "name": "Envío estándar",
        "cost": 0,
        "estimatedDeliveryDays": 5
      }
    ]
  },
  "rating": {
    "average": 4.5,
    "totalReviews": 150,
    "distribution": { "5": 82, "4": 37, "3": 18, "2": 8, "1": 5 }
  },
  "relatedProducts": [
    { "id": 2, "name": "Producto Relacionado", "price": 500.00 }
  ]
}
```

### 🆕 Obtener Productos Relacionados

```http
GET /api/v1/products/1/related?limit=6
```

### Crear un Producto

```http
POST /api/v1/products
Content-Type: application/json

{
  "name": "Laptop Gaming",
  "description": "Laptop de alta gama para gaming",
  "price": 1500.00,
  "stock": 5,
  "categoryId": 1
}
```

### Obtener Productos con Filtros

```http
GET /api/v1/products?count=10&page=1&name=laptop&categoryId=1
```

### Actualizar un Producto

```http
PUT /api/v1/products/1
Content-Type: application/json

{
  "name": "Laptop Gaming Pro",
  "description": "Versión mejorada",
  "price": 1800.00,
  "stock": 3,
  "categoryId": 1
}
```

## 🧪 Testing

La solución incluye tests unitarios completos:

### Products.Api.Application.Test
- **ProductServiceTests**: Tests de lógica de negocio
- **CategoryServiceTests**: Tests de servicios de categorías
- Cobertura: CreateAsync, UpdateAsync, DeleteAsync, GetAllAsync, GetByIdAsync

### Products.Api.Persistence.Test
- **ProductRepositoryTests**: Tests de repositorio de productos
- **CategoryRepositoryTests**: Tests de repositorio de categorías
- **CustomContextTests**: Tests de contexto de datos
- **AdapterTests**: Tests de adaptadores

**Frameworks de Testing:**
- xUnit para estructura de tests
- FluentAssertions para assertions expresivas
- Moq para mocking de dependencias

## 📈 Características Avanzadas

### 1. **Thread Safety**
- Operaciones de repositorio protegidas con `lock`
- Acceso concurrente seguro al archivo JSON

### 2. **Correlation ID**
- Trazabilidad completa de peticiones
- Header `X-Correlation-ID` en request/response
- Integrado con logging

### 3. **Logging Estructurado**
- Serilog con múltiples sinks
- Logs rotativos por día
- Context information en cada log

### 4. **Validación Robusta**
- Data Annotations en DTOs
- Validación de ModelState automática
- Validaciones de negocio en Services
- Respuestas de error detalladas

### 5. **Documentación Auto-generada**
- Swagger UI interactivo
- Documentación de todos los endpoints
- Ejemplos de peticiones/respuestas
- Soporte para múltiples versiones

## 🔧 Configuración

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "UseSerilog": true,
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/app.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## 🤝 Buenas Prácticas Implementadas

1. ✅ **Clean Architecture**: Separación clara de responsabilidades
2. ✅ **Repository Pattern**: Abstracción de acceso a datos
3. ✅ **Dependency Injection**: Inversión de control completa
4. ✅ **DTO Pattern**: Separación entre modelos internos y externos
5. ✅ **Exception Handling**: Manejo centralizado de errores
6. ✅ **Logging**: Trazabilidad completa de operaciones
7. ✅ **API Versioning**: Soporte para evolución de API
8. ✅ **Unit Testing**: Tests exhaustivos con alta cobertura
9. ✅ **Documentation**: Swagger/OpenAPI completo
10. ✅ **Thread Safety**: Operaciones concurrentes seguras

## 📞 Soporte y Contacto

Para preguntas, problemas o sugerencias, por favor contacta al equipo de desarrollo.

---

**Versión de la API**: 1.0  
**Última actualización**: 2026-02-12  
**Estado**: ✅ Producción
