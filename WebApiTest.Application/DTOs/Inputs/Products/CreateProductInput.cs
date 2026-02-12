namespace WebApiTest.Application.DTOs.Inputs.Products;
public class CreateProductInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public long CategoryId { get; set; }
}
