using FluentAssertions;
using System.ComponentModel;
using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Adapters;
using WebApiTest.Persistence.Entities;

namespace WebApiTest.Persistence.Test.Adapters;

[Category("Unit")]
public class CategoryAdapterTests
{
    [Fact]
    public void ToDomainModel_Should_Map_Entity_To_DomainModel_Correctly()
    {
        var entity = new CategoryEntity
        {
            Id = 1,
            Name = "Electronics"
        };
        var adapter = new CategoryAdapter();

        var result = adapter.ToDomainModel(entity);

        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be(entity.Name);
    }
}
