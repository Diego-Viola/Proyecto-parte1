# RunProject - Ejecución con Docker

Esta carpeta contiene todos los archivos necesarios para ejecutar el proyecto Products.Api con Docker.

## 📁 Contenido

- `Dockerfile` - ✅ **Imagen Docker multi-stage (SOLO requiere Docker + internet)**
- `Dockerfile.prebuilt` - Imagen con binarios pre-compilados (requiere .NET SDK)
- `Dockerfile.test` - Imagen Docker para ejecutar tests
- `docker-compose.yml` - Orquestación de contenedores
- `run-docker-only.ps1` / `run-docker-only.sh` - ✅ **Scripts SOLO con Docker** (recomendado)
- `run.ps1` / `run.bat` / `run.sh` - Scripts con compilación local (requieren .NET SDK)

## ⚙️ Dos Estrategias de Ejecución

### Estrategia 1: SOLO Docker (Recomendado para máquinas sin .NET SDK)

**Requisitos**: SOLO Docker + acceso a internet

La compilación ocurre **completamente dentro del contenedor Docker** usando la imagen `mcr.microsoft.com/dotnet/sdk:8.0`.

### Estrategia 2: Compilación Híbrida (Más rápida si tienes .NET SDK)

**Requisitos**: Docker + .NET 8 SDK

Compila localmente primero, luego crea imagen Docker con binarios.

## 🚀 Cómo ejecutar

### Opción 1A: SOLO Docker (sin .NET SDK) - RECOMENDADO

**Windows (PowerShell):**
```powershell
cd RunProject
.\run-docker-only.ps1
```

**Linux/Mac:**
```bash
cd RunProject
chmod +x run-docker-only.sh
./run-docker-only.sh
```

### Opción 1B: Con .NET SDK (build híbrida - más rápida)

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

### Opción 2: Docker Compose

**Requiere compilación local previa:**

```bash
# Paso 1: Compilar localmente
cd ../..
dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish

# Paso 2: Volver a RunProject y ejecutar compose
cd Products.Api/RunProject
docker-compose up --build
```

**Detener:**
```bash
docker-compose down
```

> **Nota**: Los scripts automatizados (`run.ps1`, `run.bat`, `run.sh`) hacen todo esto automáticamente. Se recomienda usarlos en lugar de docker-compose manual.

### Opción 3: Docker manual

```bash
cd RunProject
docker build -t products-api:latest -f Dockerfile ..\..
docker run -d -p 5000:8080 -e ASPNETCORE_ENVIRONMENT=Development --name products-api products-api:latest
```

## 🧪 Ejecutar Tests con Docker

```bash
cd RunProject
docker build -t products-api-test:latest -f Dockerfile.test ..\..
docker run --rm products-api-test:latest
```

## 📝 URLs del servicio

Una vez iniciado el contenedor:

- **Swagger UI:** http://localhost:5000
- **Health Check:** http://localhost:5000/api/v1/health
- **API Products:** http://localhost:5000/api/v1/products

## 🔧 Comandos útiles

```bash
# Ver logs del contenedor
docker logs -f products-api

# Detener el contenedor
docker stop products-api

# Eliminar el contenedor
docker rm products-api

# Ver contenedores en ejecución
docker ps

# Acceder al contenedor
docker exec -it products-api sh
```

## 🆘 Troubleshooting

### Error NU1301 al compilar dentro de Docker

**Síntoma**: `Unable to load the service index for source https://api.nuget.org/v3/index.json`

**Causa**: Docker no puede acceder a NuGet.

**Solución 1**: Configurar DNS en Docker Desktop
```json
// Docker Desktop > Settings > Docker Engine
{
  "dns": ["8.8.8.8", "8.8.4.4"]
}
```