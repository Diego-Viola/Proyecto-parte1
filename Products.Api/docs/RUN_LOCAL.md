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

### Perfiles de Ejecución Disponibles

El proyecto tiene configurados dos perfiles en `launchSettings.json`:

#### Perfil 1: HTTP (Recomendado para desarrollo)
- **Puerto**: http://localhost:5289
- **Comando**: `dotnet run` (usa este perfil por defecto)
- **Swagger**: http://localhost:5289

#### Perfil 2: IIS Express
- **Puerto**: http://localhost:62999
- **Comando**: Se ejecuta desde Visual Studio seleccionando "IIS Express"
- **Swagger**: http://localhost:62999/swagger

### Opción A: Desde el directorio raíz del proyecto

```bash
# Restaurar dependencias
dotnet restore Products.Api.sln

# Compilar la solución
dotnet build Products.Api.sln

# Ejecutar la API (usa perfil HTTP por defecto)
dotnet run --project Products.Api/Products.Api.csproj
```

### Opción B: Desde el directorio Products.Api

```bash
# Navegar al directorio
cd Products.Api

# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar la API (usa perfil HTTP por defecto)
dotnet run
```

### URLs de Acceso

Después de ejecutar, la API estará disponible en:

| Perfil | URL Base | Swagger UI |
|--------|----------|------------|
| **HTTP** (dotnet run) | http://localhost:5289 | http://localhost:5289 |
| **IIS Express** | http://localhost:62999 | http://localhost:62999/swagger |

> **Nota**: El perfil HTTP redirige automáticamente a Swagger desde la raíz.

---

## 2. Acceder a Swagger

Una vez ejecutada la API, accede a la documentación interactiva:

**Con perfil HTTP (dotnet run)**:
```
http://localhost:5289
```

**Con perfil IIS Express**:
```
http://localhost:62999/swagger
```

Swagger UI mostrará todos los endpoints disponibles con ejemplos de request/response.

---

## 3. Verificar Health Check

**Con perfil HTTP**:
```bash
curl http://localhost:5289/api/v1/health
```

**Con perfil IIS Express**:
```bash
curl http://localhost:62999/api/v1/health
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
