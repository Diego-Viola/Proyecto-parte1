#!/bin/bash
# ============================================
# Products.Api - Script de Ejecución SOLO con Docker
# ============================================
# Requisito: SOLO Docker instalado + acceso a internet
# NO requiere .NET SDK en la máquina host
# ============================================

set -e

# Colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}"
echo "╔════════════════════════════════════════╗"
echo "║   Products.Api - Docker Only Build     ║"
echo "╚════════════════════════════════════════╝"
echo -e "${NC}"

# Verificar Docker
if ! command -v docker &> /dev/null; then
    echo -e "${RED}Error: Docker no está instalado${NC}"
    exit 1
fi

# Variables
IMAGE_NAME="products-api"
CONTAINER_NAME="products-api"
PORT="5000"
ROOT_DIR="../.."

# Detener contenedor existente
if [ "$(docker ps -aq -f name=$CONTAINER_NAME)" ]; then
    echo -e "${YELLOW}→ Deteniendo contenedor existente...${NC}"
    docker stop $CONTAINER_NAME > /dev/null 2>&1 || true
    docker rm $CONTAINER_NAME > /dev/null 2>&1 || true
fi

# Construir imagen (todo dentro de Docker)
echo -e "${YELLOW}→ Construyendo imagen (compilación dentro de Docker)...${NC}"
echo -e "${YELLOW}   Esto puede tomar varios minutos la primera vez${NC}"

if docker build -t $IMAGE_NAME:latest -f Dockerfile $ROOT_DIR; then
    echo -e "${GREEN}✓ Imagen construida exitosamente${NC}"
else
    echo -e "${RED}Error al construir la imagen${NC}"
    echo ""
    echo -e "${YELLOW}Posibles soluciones:${NC}"
    echo "1. Verifica tu conexión a internet"
    echo "2. Configura DNS en Docker:"
    echo '   Docker Desktop > Settings > Docker Engine'
    echo '   Agregar: "dns": ["8.8.8.8", "8.8.4.4"]'
    exit 1
fi

# Ejecutar contenedor
echo -e "${YELLOW}→ Iniciando contenedor...${NC}"
docker run -d \
    -p $PORT:8080 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --name $CONTAINER_NAME \
    $IMAGE_NAME:latest

sleep 3

# Verificar health
if curl -s http://localhost:$PORT/api/v1/health > /dev/null 2>&1; then
    echo -e "${GREEN}✓ API iniciada correctamente${NC}"
else
    echo -e "${YELLOW}⚠ La API está iniciando...${NC}"
fi

# Mostrar información
echo ""
echo -e "${GREEN}╔════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║         ✓ API LISTA                    ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════╝${NC}"
echo ""
echo -e "  ${BLUE}Swagger UI:${NC}    http://localhost:$PORT"
echo -e "  ${BLUE}Health Check:${NC}  http://localhost:$PORT/api/v1/health"
echo ""
