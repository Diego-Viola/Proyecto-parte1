# 📝 Prompts Utilizados en el Desarrollo de Products.Api

## Introducción

Este documento registra los prompts estratégicos utilizados durante el desarrollo de **Products.Api** con asistencia de herramientas de IA generativa (GitHub Copilot).

**Principios seguidos:**
- Validación humana de cada respuesta generada.
- Iteración sobre resultados para refinamiento.
- Pensamiento crítico sobre las sugerencias recibidas.
- Uso de IA como acelerador, no como reemplazo del criterio técnico.

---

## 1. Diseño de Arquitectura

### Objetivo
Definir la estructura del proyecto siguiendo Clean Architecture.

### Prompt
```
Diseña una arquitectura Clean Architecture para una API REST en .NET 8 que gestione productos 
y categorías. El proyecto debe tener separación clara en capas: Presentación, Aplicación, 
Dominio y Persistencia. Incluye inyección de dependencias y Repository Pattern.
```

### Resultado Esperado
- Estructura de 4 proyectos separados con responsabilidades definidas.
- Inversión de dependencias correcta.
- Interfaces en capa de aplicación, implementaciones en persistencia.

---

## 2. Implementación del Endpoint Principal

### Objetivo
Crear endpoint de detalle de producto con información enriquecida estilo marketplace.

### Prompt
```
Implementa un endpoint GET /api/v1/products/{id}/detail que retorne información completa 
de un producto incluyendo: datos básicos, imágenes, vendedor, envío, variantes, atributos, 
ratings y productos relacionados. El response debe modelar una página de detalle de ítem 
como en un marketplace.
```

### Resultado Esperado
- Endpoint REST con respuesta enriquecida `ProductDetailEnrichedOutput`
- Helper de enriquecimiento con datos simulados.
- Estructura JSON completa para frontend

---

## 3. Manejo de Errores

### Objetivo
Implementar manejo centralizado de excepciones con respuestas estandarizadas.

### Prompt
```
Crea un middleware de manejo de excepciones para .NET 8 que capture errores y retorne 
respuestas JSON estandarizadas siguiendo RFC 7807. Mapea: InputException→400, 
NotFoundException→404, BusinessException→422, excepciones genéricas→500.
```

### Resultado Esperado
- `ExceptionHandlerMiddleware` con mapeo de códigos HTTP.
- `ErrorResponse` con campos: type, status, code, title, detail, traceId.
- Logging estructurado de errores.

---

## 4. Validaciones con FluentValidation

### Objetivo
Implementar validaciones expresivas y mantenibles.

### Prompt
```
Implementa validadores usando FluentValidation para CreateProductRequest y 
CreateCategoryRequest en .NET 8. Incluye validaciones de: campos requeridos, 
longitud de strings, valores numéricos positivos, y mensajes de error claros.
```

### Resultado Esperado
- Validadores separados por request.
- Reglas expresivas con mensajes personalizados.

---

## 5. Health Checks

### Objetivo
Implementar endpoint de health check con información de la aplicación.

### Prompt
```
Crea un Health Check personalizado en .NET 8 que retorne información de la aplicación: 
versión, nombre del servicio, entorno, y timestamp. Exponerlo en /api/v1/health.
```

### Resultado Esperado
- `AppInfoHealthCheck` con información del sistema.
- `HealthController` con endpoint documentado en Swagger.
- Respuesta estructurada con status y checks.

---

## 6. Testing

### Objetivo
Implementar tests unitarios y de integración con buena cobertura.

### Prompt
```
Genera tests unitarios para ProductsController, CategoriesController, y 
ExceptionHandlerMiddleware usando xUnit, Moq y FluentAssertions. Incluye casos: 
exitosos, errores 404, validaciones, y casos de borde.
```

### Resultado Esperado
- Tests de controllers con mocks de servicios.
- Tests de middlewares con HttpContext simulado.
- Tests de integración con WebApplicationFactory.

---
