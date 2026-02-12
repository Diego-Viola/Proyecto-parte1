using FluentAssertions;
using System.ComponentModel;
using WebApiTest.Persistence.Adapters;
using WebApiTest.Persistence.Entities;

namespace WebApiTest.Persistence.Test.Adapters;

[Category("Unit")]
public class ProductAdapterTests
{
    [Fact]
    public void ToDomainModel_Should_Map_Entity_To_DomainModel_Correctly()
    {
        var entity = new ProductEntity
        {
            Id = 10,
            Name = "Laptop",
            Description = "Gaming laptop",
            Price = 1500.99m,
            Stock = 5,
            CategoryId = 2
        };
        var adapter = new ProductAdapter();

        var result = adapter.ToDomainModel(entity);

        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be(entity.Name);
        result.Description.Should().Be(entity.Description);
        result.Price.Should().Be(entity.Price);
        result.Stock.Should().Be(entity.Stock);
        result.CategoryId.Should().Be(entity.CategoryId);
    }
}
