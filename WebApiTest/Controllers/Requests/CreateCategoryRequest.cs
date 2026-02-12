using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Controllers.Requests;

public class CreateCategoryRequest
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}
