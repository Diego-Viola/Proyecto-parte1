# WebApiTest - Clean Architecture .NET 8 API

API RESTful desarrollada con ASP.NET Core 8.0 siguiendo principios de Clean Architecture y mejores prácticas de desarrollo.

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura en capas limpia** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────────┐
│                    WebApiTest (API)                          │
│                  Capa de Presentación                        │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│              WebApiTest.Application                          │
│           Capa de Lógica de Negocio                         │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Services/                                            │   │
│  │  ├── ProductService                                  │   │
│  │  └── CategoryService                                 │   │
│  └─────────────────────────────────────────────────────┘   │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│              WebApiTest.Persistence                          │
│            Capa de Acceso a Datos                           │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Repositories/                                        │   │
│  │  ├── ProductRepository                               │   │
│  │  └── CategoryRepository                              │   │
│  └─────────────────────────────────────────────────────┘   │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────────┐
│                WebApiTest.Domain                             │
│                 Capa de Dominio                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Models/ (Product, Category)                          │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Estructura del Proyecto

```
WebApiTest.sln                           # Archivo de solución principal
├── WebApiTest/                          # API Web (Presentación)
│   ├── Controllers/                     # Controladores REST
│   ├── Middlewares/                     # Middleware personalizado
│   ├── Swagger/                         # Configuración Swagger
│   ├── HealthChecks/                    # Health checks
│   └── Program.cs                       # Punto de entrada
├── WebApiTest.Application/              # Lógica de Negocio
│   ├── Services/                        # Servicios de aplicación
│   ├── Interfaces/IServices/            # Contratos de servicios
│   ├── Interfaces/IRepositories/        # Contratos de repositorios
│   ├── DTOs/                           # Objetos de transferencia
│   └── Exceptions/                      # Excepciones de negocio
├── WebApiTest.Domain/                   # Dominio
│   ├── Models/                          # Entidades de dominio
│   └── Exceptions/                      # Excepciones de dominio
├── WebApiTest.Persistence/              # Acceso a Datos
│   ├── Repositories/                    # Implementaciones de repositorios
│   ├── Entities/                        # Entidades de base de datos
│   ├── Data/                           # Configuración de EF Core
│   └── CustomContext.cs                 # DbContext
├── WebApiTest.Application.Test/         # Tests unitarios de aplicación
├── WebApiTest.Persistence.Test/         # Tests unitarios de persistencia
└── WebApiTest.Integration.Test/         # Tests de integración
```

## 🚀 Características

- ✅ **Clean Architecture** - Separación clara de responsabilidades
- ✅ **RESTful API** - Endpoints siguiendo estándares REST
- ✅ **Swagger/OpenAPI** - Documentación interactiva de la API
- ✅ **Versionado de API** - Control de versiones de endpoints
- ✅ **Health Checks** - Monitoreo de estado de la aplicación
- ✅ **Logging estructurado** - Serilog con logs en archivo y consola
- ✅ **Manejo global de excepciones** - Middleware centralizado
- ✅ **Correlation ID** - Trazabilidad de requests
- ✅ **Request/Response Logging** - Logging de peticiones y respuestas
- ✅ **Validación de modelos** - Validación automática con Data Annotations
- ✅ **DTOs** - Separación entre modelos de dominio y API
- ✅ **Repository Pattern** - Abstracción del acceso a datos
- ✅ **Service Layer** - Lógica de negocio encapsulada
- ✅ **Dependency Injection** - Inyección nativa de ASP.NET Core
- ✅ **Entity Framework Core** - ORM para acceso a datos
- ✅ **Tests Unitarios** - Cobertura con xUnit y Moq
- ✅ **Tests de Integración** - Validación E2E

## 🛠️ Tecnologías

- **.NET 8.0** - Framework principal
- **ASP.NET Core 8.0** - Web API
- **Entity Framework Core** - ORM
- **Serilog** - Logging estructurado
- **Swagger/Swashbuckle** - Documentación API
- **Asp.Versioning** - Versionado de API
- **xUnit** - Framework de testing
- **Moq** - Mocking para tests
- **FluentAssertions** - Assertions fluidas para tests

## 📋 Requisitos

- .NET 8.0 SDK o superior
- IDE: Visual Studio 2022, JetBrains Rider, o VS Code
- SQL Server (LocalDB o instancia completa)

## 🎯 Inicio Rápido

### 1. Clonar el repositorio

```bash
git clone <repository-url>
cd Proyecto-parte1
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Compilar la solución

```bash
dotnet build
```

### 4. Ejecutar tests

```bash
dotnet test
```

### 5. Ejecutar la aplicación

```bash
cd WebApiTest
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7XXX`
- HTTP: `http://localhost:5XXX`
- Swagger: `https://localhost:7XXX/swagger`

## 📡 Endpoints Principales

### Products

- `GET /api/products` - Listar productos (con paginación y filtros)
- `GET /api/products/{id}` - Obtener producto por ID
- `POST /api/products` - Crear nuevo producto
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/products/{id}` - Eliminar producto

### Categories

- `GET /api/categories` - Listar categorías (con paginación)
- `GET /api/categories/{id}` - Obtener categoría por ID
- `POST /api/categories` - Crear nueva categoría

### Health

- `GET /health` - Estado de salud de la aplicación

## 🧪 Testing

### Ejecutar todos los tests

```bash
dotnet test
```

### Ejecutar tests con cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Ejecutar tests específicos

```bash
# Tests de servicios
dotnet test --filter "FullyQualifiedName~Services"

# Tests de repositorios
dotnet test --filter "FullyQualifiedName~Repositories"

# Tests de integración
dotnet test --filter "FullyQualifiedName~Integration"
```

## 📊 Patrones y Prácticas

### Patrones de Diseño

- **Repository Pattern** - Abstracción del acceso a datos
- **Service Layer Pattern** - Encapsulación de lógica de negocio
- **Dependency Injection** - Inversión de control
- **DTO Pattern** - Transferencia de datos entre capas
- **Middleware Pipeline** - Procesamiento de requests

### Principios SOLID

- ✅ **Single Responsibility** - Cada clase tiene una única responsabilidad
- ✅ **Open/Closed** - Abierto para extensión, cerrado para modificación
- ✅ **Liskov Substitution** - Interfaces correctamente implementadas
- ✅ **Interface Segregation** - Interfaces específicas por necesidad
- ✅ **Dependency Inversion** - Dependencias hacia abstracciones

### Buenas Prácticas

- ✅ Separación de responsabilidades por capas
- ✅ Manejo centralizado de excepciones
- ✅ Logging estructurado y trazabilidad
- ✅ Validación de entrada de datos
- ✅ Versionado de API
- ✅ Documentación con Swagger
- ✅ Tests unitarios y de integración
- ✅ Código limpio y legible
- ✅ Configuración por entorno

## 🔧 Configuración

La aplicación utiliza `appsettings.json` y `appsettings.Development.json` para configuración:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebApiTestDb;Trusted_Connection=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## 📝 Middleware Personalizado

1. **CorrelationIdMiddleware** - Añade ID de correlación a cada request
2. **ExceptionHandlerMiddleware** - Manejo global de excepciones
3. **RequestResponseLoggingMiddleware** - Logging de requests/responses

## 🏥 Health Checks

La aplicación incluye health checks para monitorear:
- Estado general de la aplicación
- Información de la versión
- Disponibilidad de servicios

Acceder en: `GET /health`

## 📖 Documentación Adicional

- `RESUMEN-EJECUTIVO.md` - Resumen de la arquitectura actual
- `README-MIGRACION.md` - Guía de la migración de CQRS a Services
- `VALIDACION-FINAL.md` - Validación técnica completa
- `COMANDOS-VERIFICACION.md` - Comandos útiles para validar el proyecto

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es parte de un ejercicio académico/de capacitación.

## 👥 Autores

- Equipo de Desarrollo

---

**Última actualización:** 2026-02-12  
**Versión:** 1.0.0  
**.NET Version:** 8.0
