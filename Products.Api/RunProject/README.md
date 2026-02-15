# RunProject - Ejecución con Docker

Esta carpeta contiene todos los archivos necesarios para ejecutar el proyecto Products.Api con Docker.

## 📁 Contenido

- `Dockerfile` - Imagen Docker multi-stage (requiere acceso a NuGet)
- `Dockerfile.prebuilt` - Imagen Docker con binarios pre-compilados (recomendado)
- `Dockerfile.test` - Imagen Docker para ejecutar tests
- `docker-compose.yml` - Orquestación de contenedores
- `run.bat` - Script de ejecución para Windows (CMD)
- `run.ps1` - Script de ejecución para Windows (PowerShell)
- `run.sh` - Script de ejecución para Linux/Mac

## 🔧 Estrategia de Build

Los scripts usan una **estrategia de compilación híbrida** para evitar el error `NU1301` (Unable to load service index for NuGet):

1. **Compilan localmente** usando el .NET SDK instalado en tu máquina
2. **Construyen la imagen Docker** con los binarios ya compilados

Esto evita que Docker necesite acceder a internet durante el build.

## 🚀 Cómo ejecutar

### Opción 1: Script automatizado (recomendado)

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

## ⚠️ Migración desde archivos antiguos

Los archivos Docker y scripts que estaban en la raíz de `Products.Api` ahora están aquí.

**Para eliminar los archivos antiguos (opcional):**

```powershell
# Desde la raíz de Products.Api
Remove-Item -Path "docker-compose.yml","Dockerfile","Dockerfile.test","run.bat","run.ps1","run.sh" -Force
```

O manualmente elimina estos archivos de `Products.Api/`:
- docker-compose.yml
- Dockerfile
- Dockerfile.test
- run.bat
- run.ps1
- run.sh

## 📐 Contexto de Build

Todos los Dockerfiles usan como contexto el directorio raíz del proyecto (`Proyecto-parte1/`), permitiendo acceder a todos los proyectos necesarios:

```
Proyecto-parte1/
├── Products.Api/
│   ├── RunProject/          ← Scripts y Dockerfiles aquí
│   │   ├── Dockerfile
│   │   ├── docker-compose.yml
│   │   └── run.ps1
│   └── Products.Api.csproj
├── Products.Api.Application/
├── Products.Api.Domain/
└── Products.Api.Persistence/
```

## 🆘 Solución de problemas

### Error NU1301 (Unable to load service index)

Este error ocurre cuando Docker no puede acceder a NuGet para descargar paquetes.

**Solución aplicada**: Los scripts (`run.ps1`, `run.bat`, `run.sh`) compilan localmente antes de construir la imagen Docker, evitando este problema.

**Requisito**: Necesitas tener **.NET 8 SDK** instalado localmente.

### Si no tienes .NET SDK instalado

**Opción 1**: Configura DNS en Docker Desktop:
1. Docker Desktop → Settings → Docker Engine
2. Agrega:
```json
{
  "dns": ["8.8.8.8", "8.8.4.4"]
}
```
3. Usa el Dockerfile estándar:
```bash
docker build -t products-api:latest -f Dockerfile ../..
```

**Opción 2**: Instala .NET SDK desde https://dotnet.microsoft.com/download

### Ejecutar sin Docker

Si Docker falla completamente, ejecuta localmente:

```bash
cd ..
dotnet run --project Products.Api.csproj
```

### El contenedor se inicia pero la API no responde

Espera unos segundos más. Si persiste:
```bash
docker logs products-api
```
