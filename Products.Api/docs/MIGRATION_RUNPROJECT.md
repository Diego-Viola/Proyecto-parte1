# 📦 Migración de Archivos Docker a RunProject

## ✅ ¿Qué se hizo?

Se movieron todos los archivos relacionados con Docker y scripts de ejecución desde la raíz de `Products.Api/` hacia la carpeta `RunProject/`.

### Archivos migrados:
- ✅ `docker-compose.yml` → `RunProject/docker-compose.yml`
- ✅ `Dockerfile` → `RunProject/Dockerfile`
- ✅ `Dockerfile.test` → `RunProject/Dockerfile.test`
- ✅ `run.bat` → `RunProject/run.bat`
- ✅ `run.ps1` → `RunProject/run.ps1`
- ✅ `run.sh` → `RunProject/run.sh`

## 🎯 Beneficios

1. **Organización**: Todos los archivos de ejecución están en un solo lugar
2. **Claridad**: Separación entre código fuente y scripts de deployment
3. **Mantenibilidad**: Más fácil localizar y actualizar archivos Docker

## 🚀 Cómo usar ahora

### Antes (desde Products.Api/):
```bash
.\run.ps1
```

### Ahora (desde Products.Api/RunProject/):
```bash
cd RunProject
.\run.ps1
```

## 🧹 Limpieza de archivos antiguos

Los archivos antiguos en la raíz de `Products.Api/` ya no son necesarios.

**Opción 1: Script automatizado (PowerShell)**
```powershell
.\cleanup-old-docker-files.ps1
```

**Opción 2: Manual**

Elimina los siguientes archivos de `Products.Api/` (si existen):
- docker-compose.yml
- Dockerfile
- Dockerfile.test
- Dockerfile.simple
- Dockerfile.prebuilt
- run.bat
- run.ps1
- run.sh

**Comando PowerShell:**
```powershell
Remove-Item -Path "docker-compose.yml","Dockerfile","Dockerfile.test","run.bat","run.ps1","run.sh" -Force -ErrorAction SilentlyContinue
```

## 📝 Contexto de Build

Los Dockerfiles usan como contexto el directorio raíz del proyecto (`Proyecto-parte1/`):

```
Proyecto-parte1/
├── Products.Api/
│   ├── RunProject/          ← Scripts y Dockerfiles aquí
│   │   ├── Dockerfile       (contexto: ../..)
│   │   ├── docker-compose.yml
│   │   └── run.ps1
│   └── Products.Api.csproj
├── Products.Api.Application/
├── Products.Api.Domain/
└── Products.Api.Persistence/
```

### Comando de build:
```bash
cd RunProject
docker build -t products-api:latest -f Dockerfile ..\..
```

El `../..` indica que el contexto es dos niveles arriba (directorio raíz `Proyecto-parte1/`).

## 📖 Documentación actualizada

Se actualizaron los siguientes archivos:
- ✅ `Docs/DOCKER_RUN.md` - Instrucciones de ejecución con Docker
- ✅ `Docs/README.md` - README principal
- ✅ `RunProject/README.md` - Nuevo README específico de RunProject

## ⚠️ Importante

Los archivos en `RunProject/` **ya están configurados** con las rutas correctas. No es necesario modificarlos.

Si encuentras referencias a los archivos antiguos en otros lugares, simplemente actualiza las rutas:
- De: `Products.Api/Dockerfile`
- A: `Products.Api/RunProject/Dockerfile`

## 🆘 Problemas?

Si algo no funciona:

1. Verifica que estás ejecutando desde `RunProject/`:
   ```bash
   cd Products.Api/RunProject
   ```

2. Verifica que Docker esté corriendo:
   ```bash
   docker --version
   ```

3. Si prefieres ejecutar sin Docker:
   ```bash
   cd Products.Api
   dotnet run --project Products.Api.csproj
   ```
