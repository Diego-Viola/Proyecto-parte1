# 🚀 Guía de Ejecución Local - Products.Api

## Requisitos Mínimos

| Requisito | Versión |
|-----------|---------|
| .NET SDK | 8.0+ |
| Sistema Operativo | Windows 10+, macOS 12+, Linux |
| RAM | 4GB mínimo |
| Espacio en disco | 500MB |

---

## 1. Ejecutar el Proyecto

### Restaurar dependencias
```bash
dotnet restore Products.Api.sln
```

### Compilar la solución
```bash
dotnet build Products.Api.sln
```

### Ejecutar la API
```bash
dotnet run --project Products.Api.csproj
```

La API estará disponible en:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

---

## 2. Acceder a Swagger

Una vez ejecutada la API, accede a la documentación interactiva:

```
http://localhost:5000
```

Swagger UI mostrará todos los endpoints disponibles con ejemplos de request/response.

---

## 3. Verificar Health Check

```bash
curl http://localhost:5000/api/v1/health
```

Respuesta esperada:
```json
{
  "status": "Healthy",
  "checks": [...],
  "appVersion": "1.0.0",
  "serverTime": "2026-02-15T..."
}
```

---

## 4. Ejecutar Tests

### Todos los tests
```bash
dotnet test Products.Api.sln
```

### Solo tests unitarios
```bash
dotnet test ../Products.Api.Test/Products.Api.Test.csproj
dotnet test ../Products.Api.Application.Test/Products.Api.Application.Test.csproj
dotnet test ../Products.Api.Persistence.Test/Products.Api.Persistence.Test.csproj
```

### Solo tests de integración
```bash
dotnet test ../Products.Api.Integration.Test/Products.Api.Integration.Test.csproj
```

### Con output detallado
```bash
dotnet test Products.Api.sln --verbosity normal
```

---

## 5. Generar Reporte de Cobertura

### Paso 1: Ejecutar tests con cobertura
```bash
dotnet test Products.Api.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### Paso 2: Instalar ReportGenerator (una sola vez)
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Paso 3: Generar reporte HTML
```bash
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:Html
```

### Paso 4: Abrir reporte
- Windows: `start ./CoverageReport/index.html`
- macOS: `open ./CoverageReport/index.html`
- Linux: `xdg-open ./CoverageReport/index.html`

---

## 6. Cobertura Estimada por Capa

| Capa | Cobertura Estimada | Componentes Cubiertos |
|------|-------------------|----------------------|
| **API (Presentación)** | ~85% | Controllers, Middlewares, Validators |
| **Application** | ~80% | Services, DTOs, Mappers |
| **Domain** | ~90% | Entities, Exceptions |
| **Persistence** | ~75% | Repositories, Context |

### Áreas con mayor cobertura:
- ✅ Controllers (ProductsController, CategoriesController)
- ✅ Middlewares (ExceptionHandler, CorrelationId)
- ✅ Validators (FluentValidation)
- ✅ Helpers (ProductEnricherHelper)

### Áreas con menor cobertura:
- ⚠️ Logging (difícil de testear)
- ⚠️ Startup/Program.cs (configuración)

---

## 7. Comandos Rápidos

```bash
# Ejecutar todo rápidamente
dotnet restore && dotnet build && dotnet run --project Products.Api.csproj

# Tests rápidos
dotnet test --no-build --verbosity minimal

# Limpiar y reconstruir
dotnet clean && dotnet build
```

---

## Troubleshooting

### Error: Puerto en uso
```bash
# Cambiar puerto en launchSettings.json o usar:
dotnet run --project Products.Api.csproj --urls="http://localhost:5050"
```

### Error: Certificado HTTPS
```bash
dotnet dev-certs https --trust
```

### Error: Dependencias no encontradas
```bash
dotnet restore --force
```
