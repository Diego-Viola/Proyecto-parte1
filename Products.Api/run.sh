#!/bin/bash
# ============================================
# Products.Api - Script de Ejecución
# ============================================
# Uso: ./run.sh
# ============================================

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}"
echo "╔════════════════════════════════════════╗"
echo "║       Products.Api - Docker Run        ║"
echo "╚════════════════════════════════════════╝"
echo -e "${NC}"

# Verificar Docker
if ! command -v docker &> /dev/null; then
    echo -e "${RED}Error: Docker no está instalado${NC}"
    echo "Instala Docker desde: https://docs.docker.com/get-docker/"
    exit 1
fi

# Variables
IMAGE_NAME="products-api"
CONTAINER_NAME="products-api"
PORT="5000"

# Detener contenedor existente si existe
if [ "$(docker ps -aq -f name=$CONTAINER_NAME)" ]; then
    echo -e "${YELLOW}→ Deteniendo contenedor existente...${NC}"
    docker stop $CONTAINER_NAME > /dev/null 2>&1 || true
    docker rm $CONTAINER_NAME > /dev/null 2>&1 || true
fi

# Construir imagen
echo -e "${YELLOW}→ Construyendo imagen Docker...${NC}"
docker build -t $IMAGE_NAME:latest -f Dockerfile .. 

echo -e "${GREEN}✓ Imagen construida exitosamente${NC}"

# Ejecutar contenedor
echo -e "${YELLOW}→ Iniciando contenedor...${NC}"
docker run -d \
    -p $PORT:8080 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --name $CONTAINER_NAME \
    $IMAGE_NAME:latest

# Esperar a que la API esté lista
echo -e "${YELLOW}→ Esperando a que la API esté lista...${NC}"
sleep 3

# Verificar health
if curl -s http://localhost:$PORT/api/v1/health > /dev/null 2>&1; then
    echo -e "${GREEN}✓ API iniciada correctamente${NC}"
else
    echo -e "${YELLOW}⚠ La API está iniciando, espera unos segundos...${NC}"
fi

# Mostrar información
echo ""
echo -e "${GREEN}╔════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║         ✓ API LISTA                    ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════╝${NC}"
echo ""
echo -e "  ${BLUE}Swagger UI:${NC}    http://localhost:$PORT"
echo -e "  ${BLUE}Health Check:${NC}  http://localhost:$PORT/api/v1/health"
echo -e "  ${BLUE}Products:${NC}      http://localhost:$PORT/api/v1/products"
echo ""
echo -e "${YELLOW}Comandos útiles:${NC}"
echo "  docker logs -f $CONTAINER_NAME    # Ver logs"
echo "  docker stop $CONTAINER_NAME       # Detener"
echo "  docker rm $CONTAINER_NAME         # Eliminar"
echo ""
