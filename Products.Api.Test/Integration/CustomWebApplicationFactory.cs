using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Products.Api.Test.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(_ =>
        {
            // Acá se puede reemplazar servicios reales por mocks o implementaciones de test
            // Ejemplo:
            // services.RemoveAll<IProductService>();
            // services.AddScoped<IProductService, MockProductService>();

            // Configurar base de datos en memoria para tests si es necesario
            // services.RemoveAll<DbContext>();
            // services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        });
    }
}