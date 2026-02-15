# 🐳 Ejecución con Docker - Products.Api

Esta guía permite ejecutar el proyecto **sin necesidad de tener .NET SDK instalado**.

## Requisitos

| Requisito | Versión |
|-----------|---------|
| Docker | 20.10+ |
| Docker Compose | 2.0+ (opcional) |

---

## Opción 1: Ejecución Rápida (Recomendada)

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

```bash
docker-compose up --build
```

Para detener:
```bash
docker-compose down
```

---

## Opción 3: Comandos Manuales

### Construir imagen
```bash
docker build -t products-api:latest -f Dockerfile ..
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

```bash
docker build -t products-api-test:latest -f Dockerfile.test ..
docker run --rm products-api-test:latest
```

---

## Estructura de Archivos Docker

```
Products.Api/
├── Dockerfile           # Imagen de producción multi-stage
├── Dockerfile.test      # Imagen para ejecutar tests
├── docker-compose.yml   # Orquestación
├── run.sh              # Script Linux/macOS
├── run.ps1             # Script PowerShell
├── run.bat             # Script Windows CMD
└── .dockerignore       # Archivos a excluir
```

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
