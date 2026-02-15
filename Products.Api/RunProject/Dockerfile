# ============================================
# Products.Api - Dockerfile Multi-Stage
# ============================================
# Este Dockerfile funciona SOLO con Docker instalado + acceso a internet.
# No requiere .NET SDK en la máquina host.
# Toda la compilación se hace dentro del contenedor.
# ============================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Configurar NuGet para reintentar en caso de fallos temporales
ENV NUGET_XMLDOC_MODE=skip
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Copiar archivos de proyecto para restaurar dependencias (cache de capas)
COPY Products.Api/Products.Api.csproj Products.Api/
COPY Products.Api.Application/Products.Api.Application.csproj Products.Api.Application/
COPY Products.Api.Domain/Products.Api.Domain.csproj Products.Api.Domain/
COPY Products.Api.Persistence/Products.Api.Persistence.csproj Products.Api.Persistence/

# Restaurar dependencias con reintentos (3 intentos)
RUN for i in 1 2 3; do \
        dotnet restore Products.Api/Products.Api.csproj \
        --disable-parallel \
        --verbosity minimal && break || sleep 5; \
    done

# Copiar código fuente
COPY Products.Api/ Products.Api/
COPY Products.Api.Application/ Products.Api.Application/
COPY Products.Api.Domain/ Products.Api.Domain/
COPY Products.Api.Persistence/ Products.Api.Persistence/

# Compilar solo el proyecto principal
WORKDIR /src/Products.Api
RUN dotnet build Products.Api.csproj -c Release -o /app/build --no-restore

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish Products.Api.csproj -c Release -o /app/publish /p:UseAppHost=false --no-restore

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
