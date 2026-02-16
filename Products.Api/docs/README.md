## 📋 Descripción General

**Products API** es una API RESTful desarrollada en .NET 8 que proporciona toda la información necesaria para soportar una página de detalle de ítem estilo marketplace. La solución implementa una arquitectura limpia multicapa con persistencia en archivos JSON, logging avanzado, y documentación interactiva con Swagger.

---

## 📜 Consigna Original

> **Prueba Técnica**
>
> **Objetivo:**
> Construir una API backend que provea toda la información necesaria para soportar una página de detalle de ítem, inspirada en Mercado Libre.
> Tu API debe entregar de manera eficiente los detalles requeridos del producto y la información relacionada, alineándose con las mejores prácticas de desarrollo backend.
>
> Este ejercicio se enfoca exclusivamente en el diseño e implementación del backend.
>
> **Requisitos:**
> - Backend: Desarrollo de API
> - Implementar una API que soporte al frontend proporcionando los detalles necesarios del producto.
> - El endpoint principal debe obtener los detalles del producto.
>
> **Stack:**
> - Puedes utilizar cualquier tecnología o framework backend de tu elección.
> - Simular la persistencia de datos utilizando archivos locales JSON/CSV o una base de datos en memoria (por ejemplo, SQLite, H2 Database) para representar el inventario.
> - No se requiere una base de datos real.
>
> **Requisitos no funcionales:**
> - Se dará especial consideración a las buenas prácticas en el manejo de errores, documentación, testing y cualquier otro aspecto no funcional relevante que elijas demostrar.
>
> **Uso de herramientas:**
> - Herramientas permitidas: Puedes usar y se recomienda utilizar herramientas de GenAI, IDEs con capacidades generativas y otras herramientas de desarrollo.

---

## ✅ Cumplimiento de la Consigna

| Requisito | Estado | Implementación |
|-----------|--------|----------------|
| **Endpoint principal de detalle** | ✅ | `GET /api/v1/products/{id}/detail` - Respuesta completa estilo marketplace |
| **API para frontend** | ✅ | Controllers REST con respuestas JSON estructuradas |
| **Persistencia JSON/CSV** | ✅ | `CustomContext` con archivo `data.json` |
| **Manejo de errores** | ✅ | `ExceptionHandlerMiddleware` + excepciones tipadas (400, 404, 422, 500) |
| **Documentación** | ✅ | Swagger/OpenAPI + README + ADR (Architecture Decision Records) |
| **Testing** | ✅ | Tests unitarios + integración con xUnit, Moq, FluentAssertions |
| **Uso de GenAI** | ✅ | Documentado en `prompts.md` |

---

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
---


## 🏗️ Proyectos de la Solución

| Proyecto | Responsabilidad |
|----------|-----------------|
| `Products.Api` | Capa de presentación: controllers, middlewares, validators |
| `Products.Api.Application` | Lógica de aplicación: services, DTOs, interfaces |
| `Products.Api.Domain` | Modelos de dominio y excepciones de negocio |
| `Products.Api.Persistence` | Acceso a datos: repositories, context JSON |
| `Products.Api.Test` | Tests unitarios de API |
| `Products.Api.Application.Test` | Tests unitarios de servicios |
| `Products.Api.Persistence.Test` | Tests unitarios de repositorios |
| `Products.Api.Integration.Test` | Tests de integración end-to-end |
---

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
---

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
---

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

---

## 🛠️ Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | Web API |
| Serilog | 9.0.0 | Logging estructurado |
| Mapster | 7.4.0 | Object mapping |
| Swashbuckle | 9.0.3 | Documentación OpenAPI |
| Asp.Versioning | 8.1.0 | Versionado de API |
| xUnit | 2.9.3 | Testing framework |
| FluentAssertions | 8.6.0 | Assertions para tests |
| Moq | 4.20.72 | Mocking framework |
---

## 🚀 Ejecución de proyecto

### Ejecución Local

### Requisitos
- .NET 8.0 SDK

### Comandos

```bash
# Restaurar dependencias
dotnet restore Products.Api.sln

# Compilar
dotnet build Products.Api.sln

# Ejecutar
dotnet run --project Products.Api/Products.Api.csproj
```

### Acceso

| Recurso | URL |
|---------|-----|
| Swagger UI | http://localhost:5289 |
| Health Check | http://localhost:5289/api/v1/health |

> Ver guía completa en [`RUN_LOCAL.md`](./RUN_LOCAL.md)

### Ejecución con Docker

#### Opción 1: Solo Docker (sin .NET SDK)

```bash
cd Products.Api/RunProject

# Windows
.\run-docker-only.ps1

# Linux/macOS
chmod +x run-docker-only.sh && ./run-docker-only.sh
```

#### Opción 2: Con .NET SDK (build híbrida - más rápida)

**Windows (PowerShell):**
```powershell
cd RunProject
.\run.ps1
```

**Windows (CMD):**
```cmd
cd RunProject
run.bat
```

**Linux/Mac:**
```bash
cd RunProject
chmod +x run.sh
./run.sh
```
---

## 🧪 Testing

### Ejecutar todos los tests

```bash
dotnet test Products.Api.sln
```

### Ejecutar por proyecto

```bash
# Tests unitarios de servicios
dotnet test Products.Api.Application.Test

# Tests unitarios de repositorios
dotnet test Products.Api.Persistence.Test

# Tests de integración
dotnet test Products.Api.Integration.Test
```
### Generar reporte de tests

```bash
# Ejecutar tests con cobertura
dotnet test Products.Api.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Instalar ReportGenerator (una sola vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generar reporte HTML
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:Html

# Abrir reporte
start ./CoverageReport/index.html
```

---

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

---

## ✔️ Validaciones con FluentValidation

El proyecto utiliza **FluentValidation** para validación expresiva de entrada:

| Validator | Campo | Reglas |
|-----------|-------|--------|
| `CreateProductInputValidator` | Name | Requerido, 3-200 caracteres |
| | Description | Requerido |
| | Price | Mayor a 0 |
| | Stock | Mayor o igual a 0 |
| | CategoryId | Mayor a 0 |
| `UpdateProductInputValidator` | (mismas reglas que Create) | |
| `CreateCategoryInputValidator` | Name | Requerido, 2-100 caracteres |

**Ubicación**: `Products.Api/Validators/`

**Integración**: Los validadores se registran automáticamente en el pipeline de ASP.NET Core y validan antes de llegar al controller.

---

## ⚠️ Supuestos y Limitaciones

### Supuestos

- API diseñada para un solo tenant (sin multitenancy).
- Los datos enriquecidos del endpoint `/detail` (vendedor, shipping, ratings, variantes) son **simulados** con seeds determinísticos basados en el ID del producto.
- Stock máximo permitido para compra: 60% del stock disponible (regla de negocio simulada).
- Los productos relacionados se generan algorítmicamente, no basados en comportamiento real de usuarios.

### Limitaciones (por alcance de prueba técnica)

| Limitación | Razón | Solución Producción |
|------------|-------|---------------------|
| **Persistencia JSON** | No es transaccional ni escala horizontalmente | PostgreSQL + EF Core |
| **Sin autenticación** | Fuera del alcance | JWT + Identity Server |
| **Sin caché** | Simplicidad | Redis para lectura intensiva |
| **Sin rate limiting** | No implementado | ASP.NET Rate Limiting |
| **Sin circuit breaker** | No hay servicios externos reales | Polly |

### Decisiones Conscientes (Trade-offs)

Las decisiones arquitectónicas están documentadas en detalle en [`DECISIONS.md`](./DECISIONS.md):

- **ADR-001**: Clean Architecture con capas separadas
- **ADR-002**: Persistencia JSON vs Base de datos
- **ADR-003**: Manejo centralizado de errores
- **ADR-004**: Versionado de API en URL
- **ADR-005**: Correlation ID para trazabilidad
- **ADR-006**: Modelo de dominio simplificado
- **ADR-007**: Sin caching (trade-off consciente)
- **ADR-008**: Estrategia de testing

---

## 🤖 Uso de GenAI

Este proyecto fue desarrollado con asistencia de herramientas de IA generativa, como se permite y recomienda en la consigna.

Los prompts utilizados están documentados en [`PROMPTS.md`](./PROMPTS.md), cubriendo:

- Diseño de arquitectura
- Implementación de endpoints
- Manejo de errores
- Validaciones
- Testing
- Configuración de Docker

**Principio aplicado**: La IA como acelerador, no como reemplazo del criterio técnico. Toda sugerencia fue validada y refinada.

---

## 📚 Documentación Adicional

| Documento                          | Descripción |
|------------------------------------|-------------|
| [`DECISIONS.md`](./DECISIONS.md)   | Registro de Decisiones Arquitectónicas (ADR) |
| [`DOCKER_RUN.md`](./DOCKER_RUN.md) | Guía de ejecución con Docker (incluye troubleshooting) |
| [`PROMPTS.md`](./prompts.md)       | Prompts de GenAI utilizados en el desarrollo |
| [`RUN_LOCAL.md`](./RUN_LOCAL.md)   | Guía de ejecución local con .NET SDK |

---
