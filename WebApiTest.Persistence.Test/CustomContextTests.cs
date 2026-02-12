using FluentAssertions;
using System.Text.Json;
using WebApiTest.Persistence.Entities;

namespace WebApiTest.Persistence.Test;

public class CustomContextTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _tempJsonPath;

    public CustomContextTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _tempJsonPath = Path.Combine(_tempDir, "data.json");
    }

    [Fact]
    public void Constructor_Should_Create_DefaultData_When_File_Does_Not_Exist()
    {
        File.Exists(_tempJsonPath).Should().BeFalse();

        var context = new CustomContext(_tempJsonPath);

        File.Exists(_tempJsonPath).Should().BeTrue();
        context.Categories.Should().NotBeNullOrEmpty();
        context.Products.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_Should_Load_Data_From_Existing_File()
    {
        var categories = new List<CategoryEntity>
        {
            new CategoryEntity { Id = 10, Name = "TestCat" }
        };
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 20, Name = "TestProd", Description = "Desc", Price = 1, Stock = 2, CategoryId = 10 }
        };
        var jsonData = new
        {
            Categories = categories,
            Products = products
        };
        var json = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_tempJsonPath, json);

        var context = new CustomContext(_tempJsonPath);

        context.Categories.Should().ContainSingle(c => c.Id == 10 && c.Name == "TestCat");
        context.Products.Should().ContainSingle(p => p.Id == 20 && p.Name == "TestProd");
    }

    [Fact]
    public void SaveChanges_Should_Write_Updated_Data_To_File()
    {
        var context = new CustomContext(_tempJsonPath);
        context.Categories.Clear();
        context.Products.Clear();
        context.Categories.Add(new CategoryEntity { Id = 99, Name = "Nueva" });
        context.Products.Add(new ProductEntity { Id = 88, Name = "Nuevo", Description = "D", Price = 2, Stock = 3, CategoryId = 99 });

        context.SaveChanges();

        var fileJson = File.ReadAllText(_tempJsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<JsonData>(fileJson, options);

        data.Should().NotBeNull();
        data!.Categories.Should().ContainSingle(c => c.Id == 99 && c.Name == "Nueva");
        data.Products.Should().ContainSingle(p => p.Id == 88 && p.Name == "Nuevo");
    }

    [Fact]
    public void Constructor_Should_Throw_When_File_Is_Invalid()
    {
        File.WriteAllText(_tempJsonPath, "invalid json");

        Action act = () => new CustomContext(_tempJsonPath);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Error al deserializar data.json*");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private class JsonData
    {
        public List<ProductEntity> Products { get; set; } = new();
        public List<CategoryEntity> Categories { get; set; } = new();
    }
}
