# Solución: Health Check visible en Swagger UI

## 🔍 Análisis del Problema

El endpoint `/health` creado con `MapHealthChecks()` NO aparecía en Swagger porque:

1. **Minimal APIs no se documentan automáticamente**: Los endpoints creados con `MapHealthChecks()`, `MapGet()`, `MapPost()`, etc., no son detectados por Swagger/Swashbuckle por defecto.
2. **Swashbuckle está configurado solo para controladores**: La configuración de `AddSwaggerGen()` con API Explorer solo detecta controladores MVC/API.

## ✅ Solución Implementada: Health Check Controller

### Ventajas de esta solución:
- ✅ **Aparece automáticamente en Swagger UI**
- ✅ **Documentación completa con XML comments**
- ✅ **Respeta buenas prácticas de .NET 8**
- ✅ **No requiere configuración adicional de Swashbuckle para Minimal APIs**
- ✅ **Tipo de retorno fuertemente tipado**
- ✅ **Códigos de estado HTTP apropiados (200 OK / 503 Service Unavailable)**

## 📝 Cambios Realizados

### 1. **Nuevo archivo: `Controllers/HealthController.cs`**

```csharp
[ApiController]
[Route("[controller]")]
[ApiVersionNeutral] // No requiere versión específica
[Tags("Health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        // Ejecuta todos los health checks registrados
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken);
        
        // Retorna 200 si está sano, 503 si hay problemas
        return report.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
```

**Características destacadas:**
- `[ApiVersionNeutral]`: El health check no necesita versionado
- `[Tags("Health")]`: Agrupa el endpoint en Swagger bajo "Health"
- Documentación XML completa para mejores descripciones en Swagger
- Retorna tipos fuertemente tipados (`HealthCheckResponse`)

### 2. **Actualizado: `Program.cs`**

**ELIMINADO:**
```csharp
app.MapHealthChecks("/health", new HealthCheckOptions { ... });
```

**RAZÓN:** Ya no es necesario porque el `HealthController` maneja esta funcionalidad automáticamente.

### 3. **Actualizado: `Swagger/ConfigureSwaggerOptions.cs`**

**AGREGADO:**
```csharp
// Habilitar comentarios XML para mejor documentación
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
if (File.Exists(xmlPath))
{
    options.IncludeXmlComments(xmlPath);
}
```

Esto permite que Swagger muestre las descripciones de los comentarios `/// <summary>`.

### 4. **Actualizado: `Products.Api.csproj`**

**AGREGADO:**
```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
<NoWarn>$(NoWarn);1591</NoWarn>
```

- `GenerateDocumentationFile`: Genera el archivo XML con la documentación
- `NoWarn 1591`: Suprime advertencias de miembros sin documentar

## 🚀 Cómo probarlo

1. **Ejecutar la aplicación:**
   ```powershell
   dotnet run
   ```

2. **Abrir Swagger UI:**
   ```
   https://localhost:{puerto}/
   ```

3. **Buscar el endpoint Health:**
   - Aparecerá en el grupo **"Health"**
   - Endpoint: `GET /health`
   - Documentación completa visible

4. **Probar el endpoint:**
   - Click en "Try it out"
   - Click en "Execute"
   - Verás la respuesta JSON con el estado de salud

## 📊 Ejemplo de Respuesta

**200 OK (Healthy):**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "app_info",
      "status": "Healthy",
      "description": "App info OK",
      "data": {
        "appVersion": "1.0.0.0",
        "serverTimeUtc": "2026-02-12T10:30:00Z"
      }
    }
  ]
}
```

**503 Service Unavailable (Unhealthy):**
```json
{
  "status": "Unhealthy",
  "checks": [
    {
      "name": "database",
      "status": "Unhealthy",
      "description": "Database connection failed",
      "data": null
    }
  ]
}
```

## 🎯 Buenas Prácticas Aplicadas

### ✅ **Separación de Concerns**
- El controller solo orquesta
- `HealthCheckService` (inyectado por DI) ejecuta los checks
- Cada check individual es una clase separada (`AppInfoHealthCheck`)

### ✅ **Respuestas HTTP Semánticamente Correctas**
- `200 OK`: Sistema saludable
- `503 Service Unavailable`: Sistema con problemas (apropiado para health checks)

### ✅ **Versionado Neutral**
- `[ApiVersionNeutral]`: El health check no cambia entre versiones de API

### ✅ **Documentación Completa**
- XML comments en todos los métodos y propiedades
- Swagger genera documentación automática de alta calidad

### ✅ **Tipos Fuertemente Tipados**
- `HealthCheckResponse` y `HealthCheckItem` son clases concretas
- No se usan tipos anónimos

## 🔐 Consideraciones de Seguridad

### ⚠️ **¿Debería el Health Check estar público?**

**Depende de tu caso de uso:**

#### **SÍ exponer públicamente si:**
- Usas orquestadores como Kubernetes, Docker Swarm, AWS ECS
- Necesitas monitoring externo (Datadog, New Relic, etc.)
- Herramientas de load balancing necesitan saber el estado

#### **NO exponer públicamente si:**
- El health check expone información sensible (versiones, configuraciones)
- Solo necesitas monitoring interno

### 🛡️ **Recomendación Profesional:**

Si necesitas **health checks públicos Y privados**:

1. **Health Check Público** (`/health`): Solo status básico
   ```csharp
   .AddCheck("basic", () => HealthCheckResult.Healthy())
   ```

2. **Health Check Detallado** (`/health/detailed`): Con autenticación
   ```csharp
   [Authorize] // Requiere autenticación
   [HttpGet("detailed")]
   public async Task<IActionResult> GetDetailed()
   ```

## 🔄 Alternativa: Mantener Minimal API

Si prefieres mantener `MapHealthChecks()`, necesitas:

1. **Instalar paquete adicional:**
   ```xml
   <PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="9.0.3" />
   ```

2. **Modificar Program.cs:**
   ```csharp
   app.MapHealthChecks("/health")
      .WithTags("Health")
      .WithOpenApi(operation => new(operation)
      {
          Summary = "Health Check",
          Description = "Verifica el estado de la aplicación"
      });
   ```

**PERO esto requiere:**
- Configuración adicional de OpenAPI para Minimal APIs
- Menos control sobre tipos de respuesta
- Documentación menos rica

**Por eso la solución con Controller es RECOMENDADA.**

## 📚 Referencias

- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Swagger/OpenAPI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)
- [API Versioning Best Practices](https://github.com/dotnet/aspnet-api-versioning)

---

## 🐛 Errores Corregidos Durante la Implementación

### Error 1: Conversión de IReadOnlyDictionary a IDictionary
**Error:**
```
No se puede convertir implícitamente el tipo 'IReadOnlyDictionary<string, object>' 
en 'IDictionary<string, object>'
```

**Solución:**
El tipo `e.Value.Data` es `IReadOnlyDictionary`, así que necesitamos convertirlo:
```csharp
Data = e.Value.Data.Count > 0 
    ? e.Value.Data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) 
    : null
```

### Error 2: JsonNamingPolicy no existe en el contexto
**Error:**
```
El nombre 'JsonNamingPolicy' no existe en el contexto actual
```

**Solución:**
Agregar el using necesario en `Program.cs`:
```csharp
using System.Text.Json;
```

---

## ✅ Resultado Final

Ahora tienes:
- ✅ Health Check visible en Swagger UI
- ✅ Documentación completa y profesional
- ✅ Respuestas HTTP correctas
- ✅ Código mantenible y testeable
- ✅ Cumple con estándares de .NET 8
