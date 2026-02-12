# WebApiTest

## Prompts

A continuación se presentan algunos prompts utilizados para desarrollar la API. No estarán todos, pero servirá para mostrar cómo se consultó la información.

- Necesito que sobre el proyecto creado #Solution me muestres algunas estructuras de carpetas para una arquitectura CQRS.
- Necesito configurar Serilog en el proyecto #Program.cs y quiero que la configuración esté parametrizada en el #appsettings.json. Los logs deben guardarse dentro de una carpeta llamada Logs.
- Necesito configurar Swagger en el proyecto #Program.cs, este tiene que soportar versionado para los endpoints. Tengo las configuraciones de los controllers generalizadas en #BaseApiController.cs.
- Necesito que crees una clase para utilizarla como respuesta estandarizada de errores en REST.
- Analiza el proyecto, específicamente la capa de Aplicación, y dime si es necesario validar algo más con respecto a la creación de Products.
- Necesito hacer la cobertura de test para el feature #CreateProductCommandHandler.cs, necesito que me crees los tests necesarios utilizando Moq y FluentAssertions. No necesito comentarios y generaliza la creación del handler y la de los repositorios.
- Tengo el siguiente error al utilizar WebApplicationFactory: 'This file is required for functional tests to run properly. There should be a copy of the file on your source project bin folder. If that is not the case, make sure that the property PreserveCompilationContext is set to true....' Necesito que me indiques, analizando #Program.cs, si necesito alguna configuración adicional.
- Necesito que me crees un README.md para el proyecto con la estructura básica para especificar el diseño de la API.
- Mejora la redacción del README.md, también revisa por errores de ortografía y gramática.

Estos son algunos ejemplos de cómo estuve utilizando Copilot para desarrollar y crear la documentación.
