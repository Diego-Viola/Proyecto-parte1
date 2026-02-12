using FluentAssertions;
using Moq;
using System.ComponentModel;
using System.Text.Json;
using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Entities;
using WebApiTest.Persistence.Interfaces;
using WebApiTest.Persistence.Repositories;

namespace WebApiTest.Persistence.Test.Repositories;

[Category("Unit")]
public class CategoryRepositoryTests : IDisposable
{
    private readonly string _tempDataDir;
    private readonly string _tempJsonPath;

    private Mock<IAdapter<CategoryEntity, Category>> _adapterMock;

    public CategoryRepositoryTests()
    {
        _tempDataDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataDir);
        _tempJsonPath = Path.Combine(_tempDataDir, "data.json");

        _adapterMock = new Mock<IAdapter<CategoryEntity, Category>>();
    }

    private CustomContext CreateContextWithCategories(List<CategoryEntity> categories)
    {
        var jsonData = new
        {
            Categories = categories,
            Products = new List<ProductEntity>()
        };
        var json = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_tempJsonPath, json);
        return new CustomContext(_tempJsonPath);
    }

    [Fact]
    public async Task Add_Should_Add_Category_And_Assign_Id()
    {
        var context = CreateContextWithCategories(new List<CategoryEntity>());
        var category = new Category { Name = "Test" };

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<CategoryEntity>()))
            .Returns<CategoryEntity>(e => new Category { Id = e.Id, Name = e.Name });

        var repository = new CategoryRepository(context, _adapterMock.Object);

        var result = await repository.AddAsync(category);

        context.Categories.Count.Should().Be(1);
        result.Id.Should().Be(1);
        context.Categories[0].Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAll_Should_Return_Paged_Categories()
    {
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Id = 1, Name = "A" },
            new CategoryEntity { Id = 2, Name = "B" },
            new CategoryEntity { Id = 3, Name = "C" }
        };
        var context = CreateContextWithCategories(categories);

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<CategoryEntity>()))
            .Returns<CategoryEntity>(e => new Category { Id = e.Id, Name = e.Name });

        var repository = new CategoryRepository(context, _adapterMock.Object);

        var result = await repository.GetAllAsync(2, 1);

        result.Items.Should().HaveCount(2);

        for (int i = 0; i < result.Items.Count(); i++)
        {
            result.Items.ElementAt(i).Id.Should().Be(categories[i].Id);
            result.Items.ElementAt(i).Name.Should().Be(categories[i].Name);
        }
    }

    [Fact]
    public async Task GetById_Should_Return_Category_When_Exists()
    {
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Id = 1, Name = "A" }
        };
        var context = CreateContextWithCategories(categories);

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<CategoryEntity>()))
            .Returns<CategoryEntity>(e => new Category { Id = e.Id, Name = e.Name });

        var repository = new CategoryRepository(context, _adapterMock.Object);

        var result = await repository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("A");
    }

    [Fact]
    public async Task GetById_Should_Return_Null_When_Not_Exists()
    {
        var context = CreateContextWithCategories(new List<CategoryEntity>());

        var repository = new CategoryRepository(context, _adapterMock.Object);

        var result = await repository.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByName_Should_Return_Category_When_Exists()
    {
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Id = 1, Name = "Test" }
        };
        var context = CreateContextWithCategories(categories);

        _adapterMock
            .Setup(a => a.ToDomainModel(It.IsAny<CategoryEntity>()))
            .Returns<CategoryEntity>(e => new Category { Id = e.Id, Name = e.Name });

        var repository = new CategoryRepository(context, _adapterMock.Object);

        var result = await repository.GetByNameAsync("test");

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByName_Should_Return_Null_When_Not_Exists()
    {
        var context = CreateContextWithCategories(new List<CategoryEntity>());

        var repository = new CategoryRepository(context, _adapterMock.Object);

        var result = await repository.GetByNameAsync("nope");

        result.Should().BeNull();
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDataDir))
            Directory.Delete(_tempDataDir, true);
    }
}