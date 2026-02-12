using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Products.Api.Controllers;

/// <summary>
/// Health Check endpoint para monitoreo de la aplicación
/// </summary>
[ApiController]
[Route("[controller]")]
[ApiVersionNeutral] // No requiere versión
[Tags("Health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Verifica el estado de salud de la aplicación
    /// </summary>
    /// <returns>Estado de salud y detalles de checks individuales</returns>
    /// <response code="200">La aplicación está saludable</response>
    /// <response code="503">La aplicación tiene problemas de salud</response>
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken);

        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(e => new HealthCheckItem
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Data = e.Value.Data.Count > 0 
                    ? e.Value.Data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) 
                    : null
            }).ToList()
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}

/// <summary>
/// Respuesta del health check
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Estado general de salud (Healthy, Degraded, Unhealthy)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Detalles de cada health check individual
    /// </summary>
    public List<HealthCheckItem> Checks { get; set; } = new();
}

/// <summary>
/// Detalle de un health check individual
/// </summary>
public class HealthCheckItem
{
    /// <summary>
    /// Nombre del check
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Estado del check (Healthy, Degraded, Unhealthy)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Descripción o mensaje del check
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Datos adicionales del check
    /// </summary>
    public IDictionary<string, object>? Data { get; set; }
}
