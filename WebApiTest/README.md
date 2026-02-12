# WebApiTest

## Descripción General

Esta API RESTful implementa la gestión de productos para un marketplace similar a MercadoLibre.

El diseño sigue el patrón **CQRS (Command Query Responsibility Segregation)**, separando las operaciones de consulta (queries) de las de comando (commands) para mejorar la escalabilidad, mantenibilidad y claridad del código.  
Si bien el patrón CQRS puede agregar complejidad inicial, los beneficios a largo plazo en términos de flexibilidad y rendimiento justifican su adopción en este proyecto.  
Se eligió este patrón de arquitectura para facilitar la evolución futura del sistema, permitiendo optimizaciones específicas para cada tipo de operación y mejorando la capacidad de respuesta bajo cargas variables.  
Por ejemplo, dividir la lógica de lectura y escritura permite aplicar diferentes estrategias de manejo de datos, como el uso de bases de datos optimizadas para lectura en queries y para escritura en commands, según sea necesario.

Algunas de las características clave incluyen:

- **Patrón CQRS (Command Query Responsibility Segregation):**  
  Permite escalar y mantener el sistema de manera eficiente, robusta y con una clara definición de cada funcionalidad.

- **Patrón Repository:**  
  Proporciona una abstracción sobre la capa de acceso a datos, permitiendo que el dominio de la aplicación permanezca independiente de los detalles de la base de datos.  
  Aunque se utiliza un archivo JSON como base de datos, el repositorio está definido con operaciones asíncronas, lo que permite modificar su implementación fácilmente para acceder a una base de datos real.  
  Además, en esta capa se implementan operaciones de paginación y filtrado, así como entidades que representan la "base de datos", manteniendo la separación respecto a los modelos de dominio.

- **Inyección de Dependencias:**  
  Fundamental para la construcción de una API o cualquier aplicación, cumpliendo con el principio de inversión de dependencias (DIP) de SOLID, y permitiendo que los componentes sean fácilmente testeables y mantenibles.

- **MediatR:**  
  Facilita la implementación del patrón CQRS actuando como mediador entre comandos/consultas y sus respectivos manejadores, reduciendo el acoplamiento y mejorando la organización del código.

- **Mapster:**  
  Biblioteca para mapear objetos entre diferentes tipos (por ejemplo, de entidades a DTOs y viceversa).  
  Es importante que estas bibliotecas sean eficientes y no utilicen reflexión en exceso. Alternativamente, se puede implementar el patrón Adapter manualmente.

- **Versionado de API:**  
  Permite mantener la compatibilidad hacia atrás mientras se introducen nuevas funcionalidades o cambios, facilitando que los clientes de la API se adapten a las actualizaciones sin interrupciones.

- **Swagger/OpenAPI:**  
  Proporciona documentación interactiva para la API, facilitando la comprensión y prueba de los endpoints disponibles.

- **Manejo Global de Errores:**  
  Garantiza que todas las excepciones sean capturadas y gestionadas de manera consistente, proporcionando respuestas claras y útiles a los clientes.  
  Se definieron varios tipos de excepciones según el contexto de validación, utilizando correctamente los códigos de estado HTTP.

- **Validaciones de Negocio:**  
  Aseguran que los datos procesados por la API cumplan con las reglas y restricciones definidas, mejorando la integridad y confiabilidad del sistema.

- **Health Checks:**  
  Permiten monitorear el estado de la aplicación y sus dependencias, facilitando la detección temprana de problemas y asegurando la disponibilidad del servicio.  
  Útil para integraciones con herramientas de monitoreo como Dynatrace.

- **Logging estructurado (Serilog):**  
  Permite registrar eventos de manera organizada y fácil de analizar, mejorando la capacidad de diagnóstico y monitoreo de la aplicación.  
  Es indispensable para aplicaciones en producción y su integración con herramientas como ElasticSearch.

- **Middlewares personalizados:**  
  Implementados para manejar tareas transversales como el logging de solicitudes y respuestas, inserción de un correlation ID para trazabilidad y manejo global de errores.

- **Convenciones de Controladores y Serialización:**  
  Se siguen convenciones claras para la definición de controladores y la serialización de datos, asegurando consistencia entre los requests y responses de la aplicación.

- **Pruebas Unitarias e Integración:**  
  Se implementaron pruebas unitarias e integración para asegurar la calidad del código y la correcta funcionalidad de los endpoints y la lógica de negocio.

---

> **Nota:**  
> Aunque se han implementado varias buenas prácticas y patrones de diseño, este proyecto es una demostración sencilla basada en experiencias del día a día.  
> En un proyecto real, se debería profundizar más en los modelos de dominio, repositorios y servicios, e incorporar patrones adicionales como Event Sourcing o CQRS avanzado con Event Bus, según la necesidad.  
> En cuanto a la seguridad, no se implementaron medidas como autenticación y autorización, ya que suelen ser responsabilidad de un gateway o capa adicional.

> **Nota sobre Seguridad:**  
> En este proyecto de ejemplo no se implementaron mecanismos de autenticación ni autorización directamente en la API.  
> 
> En entornos productivos, el acceso a las APIs suele estar gestionado por plataformas especializadas como **IBM API Connect**, que actúan como gateway y punto de entrada seguro para todos los servicios. Estas plataformas no solo restringen el acceso a los endpoints y la documentación (como Swagger), sino que también validan la identidad y los permisos de los consumidores antes de permitir cualquier interacción.
> 
> Un ejemplo común es el uso de un identificador como `IbmClientId`, que se gestiona como un secreto en herramientas como Consul y es validado por IBM API Connect antes de redirigir la solicitud a la API interna. De esta forma, solo sistemas o aplicaciones autorizadas pueden interactuar con la API, centralizando y estandarizando la seguridad para todos los servicios.
> 
> Por este motivo, en este proyecto la seguridad no se implementa a nivel de la propia API, ya que en escenarios reales esta responsabilidad recae en una capa superior gestionada por el gateway.

## Tecnologías Utilizadas

El proyecto está construido sobre **.NET 8** y C# 12, utilizando MediatR para la mediación de comandos y consultas, Mapster para el mapeo de DTOs y Swagger para la documentación interactiva.

Copilot también fue utilizado para acelerar el desarrollo y mejorar la calidad del código.

## Uso de Copilot

Copilot, integrado en el IDE Visual Studio, fue una herramienta fundamental para acelerar el desarrollo de esta API. Facilitó la autocompletación de código, la configuración inicial de las distintas dependencias utilizadas, la detección de bugs y la identificación de posibles mejoras en el diseño.  
Además, contribuyó a la automatización de pruebas unitarias e integración, generando ejemplos de tests para los diferentes casos de uso y optimizando la redacción de esta documentación.
En mi dia a dia lo utilizo constantemente como apoyo para mejorar la productividad, la calidad del código, detectar posibles fallos y como herramienta de aprendizaje continuo.

## ¿Por qué elegí .NET para esta API?

La elección de **.NET** para el desarrollo de una API orientada a un marketplace como MercadoLibre se basa en varios motivos:

- **Alto rendimiento y escalabilidad:**  
  .NET ofrece un excelente desempeño y permite escalar aplicaciones de manera eficiente, manteniendo siempre buenas prácticas de desarrollo.

- **Ecosistema y soporte:**  
  Cuenta con una comunidad muy activa y un sólido respaldo por parte de Microsoft, lo que facilita la resolución de problemas y el acceso a recursos actualizados.

- **Seguridad y buenas prácticas:**  
  El framework incorpora mecanismos integrados para la seguridad y promueve el uso de patrones de diseño modernos, como la inyección de dependencias y la separación de responsabilidades.

- **Herramientas y facilidad de configuración:**  
  .NET proporciona herramientas robustas para testing, profiling y despliegue, así como una configuración sencilla y flexible para adaptarse a distintos entornos.

- **Experiencia personal:**  
  Además, tengo amplia experiencia trabajando con esta tecnología, lo que me permite aprovechar al máximo sus capacidades y mejores prácticas.

---

## Puntos de mejoras

Al ser un proyecto de ejemplo, hay varios puntos que podrían mejorarse o ampliarse en un entorno real, por ejemplo el apartado de seguridad, la implementación de una base de datos real, la optimización del patrón CQRS con Event Sourcing, entre otros.

## Principales Endpoints

Los endpoints principales de la API son los siguientes:

### Productos (`/api/v1/products`)

- **GET `/api/v1/products`**
- **GET `/api/v1/products/{id}`**
- **POST `/api/v1/products`**
- **PUT `/api/v1/products/{id}`**
- **DELETE `/api/v1/products/{id}`**

### Categorías (`/api/v1/categories`)

- **GET `/api/v1/categories`**
- **GET `/api/v1/categories/{id}`**
- **POST `/api/v1/categories`**

Si bien, me pidiero un fetch de los productos, también implementé los endpoints de categorías para mostrar un ejemplo más completo de la API.

---

## Otros archivos .md

prompts.md : Contiene los prompts utilizados para generar partes del código y esta documentación.
diagrams.md : Contiene diagramas de arquitectura y diseño del sistema.
run.md : Instrucciones para ejecutar la API localmente.

## Agradecimientos

Agradezco la oportunidad que me dieron y espero que este proyecto cumpla con sus expectativas. Quedo a disposición para cualquier consulta, saludos.