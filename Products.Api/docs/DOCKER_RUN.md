﻿﻿﻿﻿# 🐳 Ejecución con Docker - Products.Api

Esta guía permite ejecutar el proyecto con Docker usando una estrategia de compilación híbrida.

## Requisitos

| Requisito | Versión |
|-----------|---------|
| Docker | 20.10+ |
| Docker Compose | 2.0+ (opcional) |

---

## Opción 1: Ejecución Rápida (Recomendada)

**Primero, navega a la carpeta RunProject:**
```bash
cd RunProject
```

### Linux / macOS / Git Bash
```bash
chmod +x run.sh
./run.sh
```

### Windows (PowerShell)
```powershell
.\run.ps1
```

### Windows (CMD)
```cmd
run.bat
```

Esto automáticamente:
1. Construye la imagen Docker
2. Levanta el contenedor
3. Muestra la URL de Swagger

---

## Opción 2: Docker Compose

**Desde la carpeta RunProject:**

```bash
cd RunProject

# Paso 1: Compilar localmente
cd ../..
dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish
cd Products.Api/RunProject

# Paso 2: Ejecutar con compose
docker-compose up --build
```

Para detener:
```bash
docker-compose down
```

> **Nota**: Docker Compose usa `Dockerfile.prebuilt` que requiere compilación local previa.

---

## Opción 3: Comandos Manuales

**Desde la carpeta RunProject:**

### Construir imagen
```bash
cd RunProject
docker build -t products-api:latest -f Dockerfile ..\..
```

### Ejecutar contenedor
```bash
docker run -d -p 5000:8080 --name products-api products-api:latest
```

### Ver logs
```bash
docker logs -f products-api
```

### Detener contenedor
```bash
docker stop products-api
docker rm products-api
```

---

## Acceder a la API

Una vez ejecutado, accede a:

| Recurso | URL |
|---------|-----|
| **Swagger UI** | http://localhost:5000 |
| **Health Check** | http://localhost:5000/api/v1/health |
| **Products** | http://localhost:5000/api/v1/products |
| **Categories** | http://localhost:5000/api/v1/categories |

---

## Verificar que funciona

```bash
# Health check
curl http://localhost:5000/api/v1/health

# Listar productos
curl http://localhost:5000/api/v1/products

# Detalle de producto (endpoint principal)
curl http://localhost:5000/api/v1/products/1/detail
```

---

## Ejecutar Tests en Docker

**Desde la carpeta RunProject:**
```bash
cd RunProject
docker build -t products-api-test:latest -f Dockerfile.test ..\..
docker run --rm products-api-test:latest
```

---

## Estructura de Archivos Docker

```
Products.Api/
├── RunProject/                # ← Todos los archivos Docker y scripts aquí
│   ├── Dockerfile            # Imagen de producción multi-stage optimizada
│   ├── Dockerfile.test       # Imagen para ejecutar tests
│   ├── docker-compose.yml    # Orquestación
│   ├── run.sh               # Script Linux/macOS
│   ├── run.ps1              # Script PowerShell
│   ├── run.bat              # Script Windows CMD
│   └── README.md            # Documentación de ejecución
└── Products.Api.csproj
```

> **Contexto de Build**: Todos los Dockerfiles usan `Proyecto-parte1/` como contexto para acceder a todos los proyectos (Application, Domain, Persistence).

---

## Variables de Entorno

| Variable | Default | Descripción |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Production | Entorno de ejecución |
| `ASPNETCORE_URLS` | http://+:8080 | URLs de escucha |

Ejemplo:
```bash
docker run -d -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  --name products-api \
  products-api:latest
```

---

## Troubleshooting

### Error: .NET SDK no instalado

**Síntoma**: Los scripts (`run.ps1`, `run.bat`, `run.sh`) fallan con "dotnet not found"

**Solución 1**: Instala .NET 8 SDK desde https://dotnet.microsoft.com/download

**Solución 2**: Ejecutar sin Docker
```bash
cd Products.Api
dotnet run --project Products.Api.csproj
```

### Error: Unable to load service index for NuGet (NU1301)

**Causa**: Docker no puede acceder a NuGet para descargar paquetes durante el build.

**Solución aplicada**: Los scripts ya manejan esto compilando localmente primero. Si aún ves este error, asegúrate de:
1. Tener .NET SDK instalado
2. Poder ejecutar `dotnet restore` exitosamente fuera de Docker
3. Usar los scripts (`run.ps1`, `run.bat`, `run.sh`) en lugar de comandos Docker manuales

**Alternativa - Configurar DNS de Docker**:
```bash
# Windows: Docker Desktop > Settings > Docker Engine > Agregar:
{
  "dns": ["8.8.8.8", "8.8.4.4"]
}
```

### Sin .NET SDK instalado

Si no puedes instalar .NET SDK, tienes dos opciones:

**Opción 1**: Usar imagen Docker pre-construida (si está disponible en registro)

**Opción 2**: Configurar DNS en Docker y usar `Dockerfile` estándar:
1. Docker Desktop → Settings → Docker Engine
2. Agregar DNS públicos:
```json
{
  "dns": ["8.8.8.8", "8.8.4.4"]
}
```
3. Construir manualmente:
```bash
cd RunProject
docker build -t products-api:latest -f Dockerfile ../..
docker run -d -p 5000:8080 --name products-api products-api:latest
```

### Error: file with no instructions (Dockerfile.prebuilt)

**Causa**: Problema de BOM (Byte Order Mark) en el Dockerfile.

**Solución**: Los scripts `run.ps1`, `run.bat`, `run.sh` generan automáticamente `Dockerfile.prebuilt` sin BOM. Si usas comandos manuales, ejecuta primero:
```powershell
.\fix-dockerfile.ps1
```

### Error: Puerto 5000 en uso
```bash
# Usar otro puerto
docker run -d -p 8080:8080 --name products-api products-api:latest
# Acceder en http://localhost:8080
```

### Error: Contenedor ya existe
```bash
docker rm -f products-api
```

### Ver logs de error
```bash
docker logs products-api
```

### Reconstruir sin caché
```bash
docker build --no-cache -t products-api:latest -f Dockerfile ..
```
