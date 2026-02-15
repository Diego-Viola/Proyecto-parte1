# ============================================
# Products.Api - Dockerfile Multi-Stage
# ============================================
# Optimizado para producción con imagen mínima
# Contexto de build: Proyecto-parte1/ (directorio raíz)
# Comando desde RunProject: docker build -t products-api:latest -f Dockerfile ../..
# ============================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para restaurar dependencias (cache de capas)
COPY Products.Api/Products.Api.csproj Products.Api/
COPY Products.Api.Application/Products.Api.Application.csproj Products.Api.Application/
COPY Products.Api.Domain/Products.Api.Domain.csproj Products.Api.Domain/
COPY Products.Api.Persistence/Products.Api.Persistence.csproj Products.Api.Persistence/

# Restaurar cada proyecto individualmente (no usa .sln)
RUN dotnet restore Products.Api/Products.Api.csproj --disable-parallel || true

# Copiar código fuente
COPY Products.Api/ Products.Api/
COPY Products.Api.Application/ Products.Api.Application/
COPY Products.Api.Domain/ Products.Api.Domain/
COPY Products.Api.Persistence/ Products.Api.Persistence/

# Compilar solo el proyecto principal (sin .sln que incluye tests)
WORKDIR /src/Products.Api
RUN dotnet build Products.Api.csproj -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish Products.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime (imagen mínima)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Crear usuario no-root para seguridad
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D appuser

# Copiar artefactos publicados
COPY --from=publish /app/publish .

# Copiar datos JSON desde el stage de build
COPY --from=build /src/Products.Api.Persistence/Data/ ./Data/

# Establecer permisos
RUN chown -R appuser:appgroup /app

# Cambiar a usuario no-root
USER appuser

# Exponer puerto
EXPOSE 8080

# Variables de entorno
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/api/v1/health || exit 1

# Punto de entrada
ENTRYPOINT ["dotnet", "Products.Api.dll"]
