using Microsoft.Extensions.DependencyInjection;
using Products.Api.Persistence.Adapters;
using Products.Api.Persistence.Entities;
using Products.Api.Persistence.Interfaces;
using Products.Api.Persistence.Repositories;
using Products.Api.Application.Interfaces.IRepositories;
using Products.Api.Domain.Models;

namespace Products.Api.Persistence;
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