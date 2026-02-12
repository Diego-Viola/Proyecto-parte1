namespace Products.Api.Application.DTOs.Outputs.Products;
public class ProductOutput
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public long CategoryId { get; set; }
}
