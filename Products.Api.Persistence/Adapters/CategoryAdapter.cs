using Products.Api.Persistence.Entities;
using Products.Api.Persistence.Interfaces;
using Products.Api.Domain.Models;

namespace Products.Api.Persistence.Adapters;
public class CategoryAdapter : IAdapter<CategoryEntity, Category>
{
    public Category ToDomainModel(CategoryEntity entity)
        => new Category
        {
            Id = entity.Id,
            Name = entity.Name
        };
}
