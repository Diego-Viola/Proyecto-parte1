using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace Products.Api.HealthChecks;

public class AppInfoHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
        var serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        var data = new Dictionary<string, object>
        {
            { "appVersion", version },
            { "serverTimeUtc", serverTime }
        };

        return Task.FromResult(
            HealthCheckResult.Healthy("App info OK", data)
        );
    }
}
