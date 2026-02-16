using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Products.Api.Persistence;

namespace Products.Api.Integration.Test.Support;

[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory> { }

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _testDataPath;

    public CustomWebApplicationFactory()
    {
        _testDataPath = Path.Combine(Path.GetTempPath(), $"ProductsApiTest_{Guid.NewGuid()}", "data.json");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UseSerilog"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(CustomContext));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddSingleton(new CustomContext(_testDataPath));
        });

        builder.UseEnvironment("Testing");

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        if (disposing)
        {
            // Limpiar archivo de datos de test
            try
            {
                var directory = Path.GetDirectoryName(_testDataPath);
                if (directory != null && Directory.Exists(directory))
                {
                    Directory.Delete(directory, recursive: true);
                }
            }
            catch
            {
                // Ignorar errores de limpieza
            }
        }
    }
}
