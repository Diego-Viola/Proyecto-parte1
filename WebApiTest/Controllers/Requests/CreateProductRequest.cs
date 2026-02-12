using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Controllers.Requests;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative integer.")]
    public int Stock { get; set; }

    [Required]
    public long CategoryId { get; set; }
}
