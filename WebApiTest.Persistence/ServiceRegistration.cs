using Microsoft.Extensions.DependencyInjection;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Adapters;
using WebApiTest.Persistence.Entities;
using WebApiTest.Persistence.Interfaces;
using WebApiTest.Persistence.Repositories;

namespace WebApiTest.Persistence;
public static class ServiceRegistration
{
    public static void AddInfrastructureService(this IServiceCollection services)
    {
        services.AddSingleton<CustomContext>();
        services.AddSingleton<IAdapter<ProductEntity, Product>, ProductAdapter>();
        services.AddSingleton<IAdapter<CategoryEntity, Category>, CategoryAdapter>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}
