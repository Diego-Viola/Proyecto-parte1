## Diagrama de Arquitectura

A continuacion dispongo el diagrama de la arquitectura utilizada, la misma se puede visualizar en https://www.mermaidchart.com

---
config:
  layout: dagre
---
flowchart TD
 subgraph Cliente["Cliente"]
        A["Cliente/API Consumer"]
  end
 subgraph API["API"]
        H["Middleware: Logging, CorrelationId, Errores"]
        B["Controlador"]
        C["MediatR"]
        D1["Command Handler: Create/Update/Delete"]
        D2["Query Handler: Get/List"]
        E1["Repositorio"]
        E2["Repositorio"]
        F1["Base de Datos: JSON/File"]
        F2["Base de Datos: JSON/File"]
        G["Mapster: DTO ↔ Entidad"]
        I["Swagger/OpenAPI"]
        J["Health Checks"]
  end
    A -- HTTP Request --> H
    H -- Pasa la petición --> B
    B -- Llama a MediatR --> C
    C -- Command --> D1
    C -- Query --> D2
    D1 -- Acceso a datos --> E1
    D2 -- Acceso a datos --> E2
    E1 -- Lee/Escribe --> F1
    E2 -- Lee --> F2
    D1 -- Mapeo --> G
    D2 -- Mapeo --> G
    B -- Responde --> H
    H -- HTTP Response --> A
    B -- Documentación --> I
    B -- Health --> J