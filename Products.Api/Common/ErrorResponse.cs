namespace Products.Api.Common;

/// <summary>
/// Modelo de respuesta de error estandarizado siguiendo RFC 7807 (Problem Details).
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// URI que identifica el tipo de problema.
    /// </summary>
    public string Type { get; set; } = "about:blank";

    /// <summary>
    /// Código de estado HTTP.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Código de error específico de la aplicación.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Título breve y legible del error.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Explicación detallada del error.
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// URI de la instancia específica donde ocurrió el error.
    /// </summary>
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único de trazabilidad de la petición.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Errores de validación por campo (opcional).
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }
}
