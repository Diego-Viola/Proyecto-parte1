using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace WebApiTest.Swagger;

public class SwaggerDefaultValues : Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter
{
    public void Apply(OpenApiOperation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        operation.Deprecated = apiDescription.IsDeprecated();
    }
}