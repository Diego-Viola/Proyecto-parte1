using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using Products.Api.Application;
using Products.Api.Common;
using Products.Api.Configs;
using Products.Api.Handlers;
using Products.Api.HealthChecks;
using Products.Api.Middlewares;
using Products.Api.Persistence;
using Products.Api.Swagger;

#region Configuración de builder y logging

var builder = WebApplication.CreateBuilder(args);

var useSerilog = builder.Configuration.GetValue("UseSerilog", true);
if (useSerilog)
{
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
    );
}

#endregion

#region Registro de servicios

// Configuración de controladores y comportamiento de la API
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new LowercaseControllerModelConvention());
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status400BadRequest));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status500InternalServerError));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status401Unauthorized));
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity));
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = InvalidModelStateHandler.Handle;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Versionado de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configuración de Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Servicios de aplicación e infraestructura
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureService();

// Health Checks personalizados
builder.Services.AddHealthChecks()
    .AddCheck<AppInfoHealthCheck>("app_info");

#endregion

#region Configuración del pipeline HTTP

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configuración de Swagger solo en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Products.Api API {description.GroupName.ToUpperInvariant()}"
            );
        }
    });
}

// Middlewares globales
app.UseAuthorization();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Mapeo de endpoints de controladores (incluye HealthController)
app.MapControllers();


app.Run();

#endregion

public partial class Program { }

