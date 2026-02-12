using Microsoft.Extensions.DependencyInjection;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Application.Services;

namespace Products.Api.Application;

public static class ServiceRegistration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
    }
}
