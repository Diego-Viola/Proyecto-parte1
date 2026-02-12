using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Entities;
using WebApiTest.Persistence.Interfaces;

namespace WebApiTest.Persistence.Adapters;
public class CategoryAdapter : IAdapter<CategoryEntity, Category>
{
    public Category ToDomainModel(CategoryEntity entity)
        => new Category
        {
            Id = entity.Id,
            Name = entity.Name
        };
}
