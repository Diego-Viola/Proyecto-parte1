#!/bin/bash
# ============================================
# Products.Api - Script de Ejecución
# ============================================
# Uso: ./run.sh
# Ejecutar desde: Products.Api/RunProject/
#
# Esta versión compila localmente para evitar
# problemas de red (NU1301) en Docker
# ============================================

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
GRAY='\033[0;90m'
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

# Verificar .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}Error: .NET SDK no está instalado${NC}"
    echo "Instala desde: https://dotnet.microsoft.com/download"
    echo ""
    echo -e "${YELLOW}Alternativa: intentar build dentro de Docker (puede fallar por red)${NC}"
    echo "  docker build -t products-api:latest -f Dockerfile ../.."
    exit 1
fi

# Variables
IMAGE_NAME="products-api"
CONTAINER_NAME="products-api"
PORT="5000"
ROOT_DIR="../.."

# Detener contenedor existente si existe
if [ "$(docker ps -aq -f name=$CONTAINER_NAME)" ]; then
    echo -e "${YELLOW}→ Deteniendo contenedor existente...${NC}"
    docker stop $CONTAINER_NAME > /dev/null 2>&1 || true
    docker rm $CONTAINER_NAME > /dev/null 2>&1 || true
fi

# Paso 1: Compilar localmente (evita problemas de red en Docker)
echo ""
echo -e "${YELLOW}Paso 1: Compilando aplicación localmente...${NC}"
echo -e "${GRAY}         (esto evita errores NU1301 en Docker)${NC}"

pushd $ROOT_DIR > /dev/null
if ! dotnet publish Products.Api/Products.Api.csproj -c Release -o ./publish --nologo; then
    echo -e "${RED}Error al compilar.${NC}"
    echo "Verifica que .NET SDK esté instalado y las dependencias sean correctas."
    popd > /dev/null
    exit 1
fi
echo -e "${GREEN}✓ Compilación exitosa${NC}"
popd > /dev/null

# Paso 2: Construir imagen Docker con binarios pre-compilados
echo ""
echo -e "${YELLOW}Paso 2: Construyendo imagen Docker...${NC}"
if ! docker build -t $IMAGE_NAME:latest -f Dockerfile.prebuilt $ROOT_DIR ; then
    echo -e "${RED}Error al construir la imagen${NC}"
    echo ""
    echo -e "${YELLOW}Alternativa: ejecutar localmente sin Docker${NC}"
    echo "  cd ../"
    echo "  dotnet run --project Products.Api.csproj"
    exit 1
fi

echo -e "${GREEN}✓ Imagen construida exitosamente${NC}"

# Paso 3: Ejecutar contenedor
echo ""
echo -e "${YELLOW}Paso 3: Iniciando contenedor...${NC}"
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

# Limpiar carpeta publish
echo -e "${GRAY}→ Limpiando archivos temporales...${NC}"
rm -rf $ROOT_DIR/publish

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
