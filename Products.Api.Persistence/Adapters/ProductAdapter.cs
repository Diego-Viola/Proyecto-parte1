using Products.Api.Persistence.Entities;
using Products.Api.Persistence.Interfaces;
using Products.Api.Domain.Models;

namespace Products.Api.Persistence.Adapters;
public class ProductAdapter : IAdapter<ProductEntity, Product>
{
    public Product ToDomainModel(ProductEntity entity)
        => new Product
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Stock = entity.Stock,
            CategoryId = entity.CategoryId
        };
}
