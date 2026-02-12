using Microsoft.Extensions.DependencyInjection;
using WebApiTest.Application.Interfaces.IServices;
using WebApiTest.Application.Services;

namespace WebApiTest.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}
