using System.ComponentModel.DataAnnotations;

namespace Products.Api.Controllers.Requests;

public class CreateCategoryRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}
