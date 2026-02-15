using FluentAssertions;
using Moq;
using System.ComponentModel;
using System.Text.Json;
using Products.Api.Domain.Models;
using Products.Api.Persistence.Entities;
using Products.Api.Persistence.Interfaces;
using Products.Api.Persistence.Repositories;

namespace Products.Api.Persistence.Test.Repositories;

[Category("Unit")]
public class ProductRepositoryTests : IDisposable
{
    private readonly string _tempDataDir;
    private readonly string _tempJsonPath;

    private Mock<IAdapter<ProductEntity, Product>> _adapterMock;

    public ProductRepositoryTests()
    {
        _tempDataDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataDir);
        _tempJsonPath = Path.Combine(_tempDataDir, "data.json");

        _adapterMock = new Mock<IAdapter<ProductEntity, Product>>();
    }

    private CustomContext CreateContextWithProducts(List<ProductEntity> products)
    {
        var jsonData = new
        {
            Categories = new List<CategoryEntity>(),
            Products = products
        };
        var json = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_tempJsonPath, json);
        return new CustomContext(_tempJsonPath);
    }

    [Fact]
    public async Task Add_Should_Add_Product_And_Assign_Id()
    {
        var context = CreateContextWithProducts(new List<ProductEntity>());
        var product = new Product
        {
            Name = "Test",
            Description = "Desc",
            Price = 10.5m,
            Stock = 2,
            CategoryId = 1
        };

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<ProductEntity>()))
            .Returns<ProductEntity>(e => new Product
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Price = e.Price,
                Stock = e.Stock,
                CategoryId = e.CategoryId
            });

        var repository = new ProductRepository(context, _adapterMock.Object);

        var result = await repository.AddAsync(product);

        context.Products.Count.Should().Be(1);
        result.Id.Should().Be(1);
        context.Products[0].Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAll_Should_Return_Paged_And_Filtered_Products()
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "A", Description = "D1", Price = 1, Stock = 1, CategoryId = 1 },
            new ProductEntity { Id = 2, Name = "B", Description = "D2", Price = 2, Stock = 2, CategoryId = 2 },
            new ProductEntity { Id = 3, Name = "C", Description = "D3", Price = 3, Stock = 3, CategoryId = 1 }
        };
        var context = CreateContextWithProducts(products);

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<ProductEntity>()))
            .Returns<ProductEntity>(e => new Product
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Price = e.Price,
                Stock = e.Stock,
                CategoryId = e.CategoryId
            });

        var repository = new ProductRepository(context, _adapterMock.Object);

        var result = await repository.GetAllAsync(1, 2, null, null);

        result.Items.Should().HaveCount(2);
        result.Items.ElementAt(0).Id.Should().Be(products[0].Id);
        result.Items.ElementAt(1).Id.Should().Be(products[1].Id);
        result.Total.Should().Be(3);

        var filtered = await repository.GetAllAsync(1, 2, "C", null);
        filtered.Items.Should().HaveCount(1);
        filtered.Items.First().Name.Should().Be("C");
        filtered.Total.Should().Be(1);

        var filteredByCategory = await repository.GetAllAsync(1, 5, null, 1);
        filteredByCategory.Items.Should().OnlyContain(p => p.CategoryId == 1);
        filteredByCategory.Total.Should().Be(2);
    }

    [Fact]
    public async Task GetById_Should_Return_Product_When_Exists()
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "A", Description = "D", Price = 1, Stock = 1, CategoryId = 1 }
        };
        var context = CreateContextWithProducts(products);

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<ProductEntity>()))
            .Returns<ProductEntity>(e => new Product
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Price = e.Price,
                Stock = e.Stock,
                CategoryId = e.CategoryId
            });

        var repository = new ProductRepository(context, _adapterMock.Object);

        var result = await repository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("A");
    }

    [Fact]
    public async Task GetById_Should_Return_Null_When_Not_Exists()
    {
        var context = CreateContextWithProducts(new List<ProductEntity>());

        var repository = new ProductRepository(context, _adapterMock.Object);

        var result = await repository.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_Should_Return_True_And_Update_Entity_When_Exists()
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "A", Description = "D", Price = 1, Stock = 1, CategoryId = 1 }
        };
        var context = CreateContextWithProducts(products);

        var repository = new ProductRepository(context, _adapterMock.Object);

        var updated = new Product
        {
            Id = 1,
            Name = "Updated",
            Description = "NewDesc",
            Price = 99,
            Stock = 10,
            CategoryId = 2
        };

        var result = await repository.UpdateAsync(updated);

        result.Should().BeTrue();
        context.Products[0].Name.Should().Be("Updated");
        context.Products[0].Description.Should().Be("NewDesc");
        context.Products[0].Price.Should().Be(99);
        context.Products[0].Stock.Should().Be(10);
        context.Products[0].CategoryId.Should().Be(2);
    }

    [Fact]
    public async Task Update_Should_Return_False_When_Entity_Not_Exists()
    {
        var context = CreateContextWithProducts(new List<ProductEntity>());

        var repository = new ProductRepository(context, _adapterMock.Object);

        var updated = new Product
        {
            Id = 1,
            Name = "Updated",
            Description = "NewDesc",
            Price = 99,
            Stock = 10,
            CategoryId = 2
        };

        var result = await repository.UpdateAsync(updated);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Should_Return_True_And_Remove_Entity_When_Exists()
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "A", Description = "D", Price = 1, Stock = 1, CategoryId = 1 }
        };
        var context = CreateContextWithProducts(products);

        var repository = new ProductRepository(context, _adapterMock.Object);

        var result = await repository.DeleteAsync(1);

        result.Should().BeTrue();
        context.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_Should_Return_False_When_Entity_Not_Exists()
    {
        var context = CreateContextWithProducts(new List<ProductEntity>());

        var repository = new ProductRepository(context, _adapterMock.Object);

        var result = await repository.DeleteAsync(1);

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDataDir))
            Directory.Delete(_tempDataDir, true);
    }
}
