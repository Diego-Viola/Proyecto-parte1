# 📋 Registro de Decisiones Arquitectónicas (ADR)

Este documento registra las decisiones técnicas tomadas durante el desarrollo del proyecto Products.Api, junto con su contexto, alternativas consideradas y trade-offs aceptados.

---

## ADR-001: Arquitectura Clean Architecture con Capas Separadas

### Estado
**Aceptada**

### Contexto
Se necesita una arquitectura que permita:
- Testabilidad independiente de cada capa
- Flexibilidad para cambiar implementaciones (ej: cambiar persistencia JSON por SQL)
- Separación clara de responsabilidades
- Facilidad de mantenimiento a largo plazo

### Decisión
Implementar **Clean Architecture** con 4 proyectos separados:
- `Products.Api` - Capa de presentación (Controllers, Middlewares)
- `Products.Api.Application` - Capa de aplicación (Services, DTOs, Interfaces)
- `Products.Api.Domain` - Capa de dominio (Models, Excepciones de negocio)
- `Products.Api.Persistence` - Capa de infraestructura (Repositories, Context)

### Consecuencias
✅ **Positivas:**
- Inversión de dependencias correcta (las capas internas no conocen las externas)
- Fácil de testear con mocks
- Preparado para escalar a microservicios si fuera necesario

⚠️ **Trade-offs:**
- Mayor complejidad inicial para un CRUD simple
- Más archivos y carpetas que un proyecto monolítico
- Curva de aprendizaje para desarrolladores junior

### Alternativas Consideradas
1. **Arquitectura en capas tradicional**: Descartada por acoplamiento alto entre capas
2. **Vertical Slices**: Considerada pero descartada para mantener familiaridad del equipo
3. **Minimal API sin capas**: Descartada por falta de estructura para escalar

---

## ADR-002: Persistencia con JSON en lugar de Base de Datos

### Estado
**Aceptada (Temporal - Solo para prueba técnica)**

### Contexto
La consigna especifica:
> "Simular persistencia con JSON/CSV o base en memoria (SQLite, etc). No se requiere base de datos real."

### Decisión
Usar un archivo `data.json` como almacenamiento persistente, gestionado por `CustomContext`.

### Implementación
```csharp
public class CustomContext
{
    private readonly string _filePath;
    public List<ProductEntity> Products { get; set; }
    public List<CategoryEntity> Categories { get; set; }
    
    public void SaveChanges() => // Serializa a JSON
}
```

### Consecuencias
✅ **Positivas:**
- Cero configuración de infraestructura
- Datos persisten entre reinicios de la aplicación
- Fácil de inspeccionar/modificar manualmente
- Cumple con el requisito de la consigna

⚠️ **Trade-offs:**
- **Sin transacciones ACID**: Si falla a mitad de operación, estado inconsistente
- **Lock global**: Serializa todas las operaciones (bottleneck en concurrencia)
- **Sin índices**: Búsquedas son O(n)
- **Sin relaciones enforcement**: La integridad referencial es manual

### En Producción Se Reemplazaría Por
1. **PostgreSQL + Entity Framework Core** para transacciones ACID
2. **Redis** para caché distribuida
3. **Elasticsearch** para búsquedas complejas de productos

### Alternativas Consideradas
1. **SQLite**: Más cercano a producción, pero agrega complejidad de EF Core
2. **LiteDB**: Base de datos NoSQL embebida, buena opción pero menos conocida
3. **In-Memory List sin persistencia**: Descartada porque los datos se pierden al reiniciar

---

## ADR-003: Manejo Centralizado de Errores con Middleware

### Estado
**Aceptada**

### Contexto
Se necesita:
- Respuestas de error consistentes en toda la API
- Evitar try-catch repetitivo en cada controller
- Logging centralizado de errores
- Mapeo de excepciones de dominio a HTTP status codes

### Decisión
Implementar `ExceptionHandlerMiddleware` que intercepta todas las excepciones y las transforma en `ErrorResponse` siguiendo RFC 7807 (Problem Details).

### Mapeo de Excepciones
| Excepción | HTTP Status | Uso |
|-----------|-------------|-----|
| `InputException` | 400 Bad Request | Validación de entrada |
| `BadRequestException` | 400 Bad Request | Datos inválidos |
| `NotFoundException` | 404 Not Found | Recurso no existe |
| `BusinessException` | 422 Unprocessable Entity | Regla de negocio violada |
| `DataIntegrationException` | 500 Internal Server Error | Error de persistencia |
| `TimeoutException` | 503 Service Unavailable | Timeout externo |
| `Exception` | 500 Internal Server Error | Errores no controlados |

### Consecuencias
✅ **Positivas:**
- Código de controllers limpio (sin try-catch)
- Formato de error consistente
- Códigos de error únicos para debugging (`API-GPD-01`)
- TraceId para correlación

⚠️ **Trade-offs:**
- Las excepciones son costosas en rendimiento (pero solo en casos de error)
- Algunas validaciones podrían usar Result pattern en lugar de excepciones

---

## ADR-004: API Versioning en URL

### Estado
**Aceptada**

### Contexto
Se necesita poder evolucionar la API sin romper clientes existentes.

### Decisión
Usar versionado en URL: `/api/v1/products`, `/api/v2/products`

### Alternativas Consideradas
1. **Header versioning** (`Api-Version: 1.0`): Menos visible, más difícil de probar
2. **Query string** (`?api-version=1.0`): Contamina la URL con metadata
3. **Media type** (`Accept: application/vnd.api.v1+json`): Complejo para clientes

### Consecuencias
✅ La URL es auto-documentada y fácil de compartir
⚠️ Rompe el principio REST de que la URL identifica el recurso

---

## ADR-005: Correlation ID para Trazabilidad

### Estado
**Aceptada**

### Contexto
En sistemas distribuidos (o preparándose para serlo), es crítico poder rastrear una petición a través de todos los servicios.

### Decisión
Implementar `CorrelationIdMiddleware`:
1. Lee el header `X-Correlation-ID` si existe
2. Si no existe, genera un nuevo UUID
3. Lo propaga en el response y en todos los logs

### Consecuencias
✅ **Positivas:**
- Debugging facilitado en producción
- Preparado para arquitectura de microservicios
- Compatible con herramientas de observabilidad (Jaeger, Zipkin)

---

## ADR-006: Modelo de Dominio Simplificado (Trade-off Consciente)

### Estado
**Aceptada con Reservas**

### Contexto
La consigna pide "detalles del producto para una página tipo marketplace". Un modelo completo incluiría:
- Imágenes, Seller, Variantes, Atributos, Reviews, Shipping, etc.

### Decisión
Implementar un modelo simplificado (Product + Category) para:
1. Demostrar la arquitectura correctamente
2. Mantener el scope manejable para una prueba técnica
3. Permitir foco en aspectos no funcionales (testing, error handling, logging)

### Modelo Actual
```csharp
Product: { Id, Name, Description, Price, Stock, CategoryId }
Category: { Id, Name }
```

### Modelo Ideal para Producción
```csharp
Product: { Id, Name, Description, Prices, Condition, Sku, SellerId, ... }
ProductImage: { Id, ProductId, Url, Order, IsPrimary }
Seller: { Id, Name, Reputation, Location }
ProductAttribute: { Id, ProductId, Name, Value }
ProductVariant: { Id, ProductId, Sku, Attributes, Stock, Price }
Review: { Id, ProductId, UserId, Rating, Comment, Date }
ShippingOption: { Id, Method, Cost, EstimatedDays }
```

### Consecuencias
⚠️ **Trade-off aceptado:**
- El modelo no refleja la complejidad real de un marketplace
- Demuestra capacidad técnica pero no modelado de dominio complejo
- En una entrevista, se explicaría este trade-off verbalmente

---

## ADR-007: Sin Caching (Decisión Consciente)

### Estado
**Diferida para Producción**

### Contexto
Una página de detalle de producto es un caso de uso de **lectura intensiva** que se beneficiaría de caching.

### Decisión
No implementar caching en esta versión para:
1. Mantener el scope de la prueba técnica
2. Evitar complejidad de invalidación de caché
3. La persistencia JSON ya está en memoria

### Estrategia para Producción
```csharp
// Nivel 1: Memory Cache (5 minutos)
services.AddMemoryCache();

// En ProductService
public async Task<ProductDetailOutput> GetByIdAsync(long id)
{
    var cacheKey = $"product:{id}";
    
    if (!_cache.TryGetValue(cacheKey, out ProductDetailOutput result))
    {
        result = await BuildProductDetail(id);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
    }
    
    return result;
}

// Nivel 2: Distributed Cache (Redis)
services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = "redis:6379";
});

// Nivel 3: HTTP Caching
[ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
public async Task<IActionResult> GetById(long id)
```

### Invalidación
```csharp
// En Update/Delete
await _cache.RemoveAsync($"product:{id}");
await _cache.RemoveAsync("products:list:*"); // Patrón
```

---

## ADR-008: Testing Strategy

### Estado
**Aceptada**

### Contexto
Se necesita balance entre cobertura y velocidad de desarrollo.

### Decisión
Implementar pirámide de testing:

```
        /\
       /  \      E2E Tests (manual/Postman)
      /----\
     /      \    Integration Tests (WebApplicationFactory)
    /--------\
   /          \  Unit Tests (Services, Repositories)
  /------------\
```

### Distribución
- **Unit Tests**: Services, Repositories, Adapters con Moq
- **Integration Tests**: Controllers, Middlewares con WebApplicationFactory
- **E2E**: Manual con Swagger UI o archivo `.http`

### Consecuencias
✅ Alta confianza en refactoring
✅ Tests rápidos (unit) + tests realistas (integration)
⚠️ Los tests de integración comparten estado (mejorable con test containers)

---

## Decisiones Futuras Pendientes

### Para Producción
- [ ] Migrar a PostgreSQL + EF Core
- [ ] Implementar Redis para caché distribuida
- [ ] Agregar autenticación JWT
- [ ] Implementar rate limiting
- [ ] Agregar métricas con Prometheus
- [ ] Configurar OpenTelemetry para tracing distribuido
- [ ] Implementar Circuit Breaker para llamadas externas

---

## Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)
- [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines)
- [ADR Template by Michael Nygard](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
