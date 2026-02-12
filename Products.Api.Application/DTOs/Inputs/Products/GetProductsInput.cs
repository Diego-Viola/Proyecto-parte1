namespace Products.Api.Application.DTOs.Inputs.Products;

public class GetProductsInput
{
    public int Page { get; set; }
    public int Count { get; set; }
    public string? Name { get; set; }
    public long? CategoryId { get; set; }
}
