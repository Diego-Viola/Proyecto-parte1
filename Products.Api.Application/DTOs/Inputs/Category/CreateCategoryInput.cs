using System.ComponentModel.DataAnnotations;

namespace Products.Api.Application.DTOs.Inputs.Category;

public class CreateCategoryInput
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;
}
