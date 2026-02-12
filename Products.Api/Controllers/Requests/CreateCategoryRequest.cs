﻿using System.ComponentModel.DataAnnotations;

namespace Products.Api.Controllers.Requests;

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;
}
