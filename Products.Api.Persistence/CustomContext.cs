using System.Text.Json;
using Products.Api.Persistence.Entities;

namespace Products.Api.Persistence;

public class CustomContext
{
    private readonly string _filePath;

    public List<ProductEntity> Products { get; set; } = new();
    public List<CategoryEntity> Categories { get; set; } = new();

    public CustomContext(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "Data", "data.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

        if (!File.Exists(_filePath))
        {
            var defaultData = new JsonData
            {
                Categories = new List<CategoryEntity>
                {
                    new() { Id = 1, Name = "Electrónica" },
                    new() { Id = 2, Name = "Hogar" },
                    new() { Id = 3, Name = "Deportes" }
                },
                Products = new List<ProductEntity>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Smartphone",
                        Description = "Teléfono inteligente de última generación",
                        Price = 999.99m,
                        Stock = 10,
                        CategoryId = 1
                    }
                }
            };
            var json = JsonSerializer.Serialize(defaultData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        try
        {
            var fileJson = File.ReadAllText(_filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var data = JsonSerializer.Deserialize<JsonData>(fileJson, options);
            Products = data?.Products ?? new();
            Categories = data?.Categories ?? new();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error al deserializar data.json", ex);
        }
    }

    public void SaveChanges()
    {
        var data = new JsonData
        {
            Products = Products,
            Categories = Categories
        };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}

public class JsonData
{
    public List<ProductEntity> Products { get; set; } = new();
    public List<CategoryEntity> Categories { get; set; } = new();
}