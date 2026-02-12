using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Entities;
using WebApiTest.Persistence.Interfaces;

namespace WebApiTest.Persistence.Adapters;
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
