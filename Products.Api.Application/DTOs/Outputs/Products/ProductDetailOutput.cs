using Products.Api.Application.DTOs.Outputs.Categories;

namespace Products.Api.Application.DTOs.Outputs.Products;

public class ProductDetailOutput
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public CategoryOutput Category { get; set; }
}
