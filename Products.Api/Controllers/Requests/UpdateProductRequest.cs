﻿using System.ComponentModel.DataAnnotations;

namespace Products.Api.Controllers.Requests;

public class UpdateProductRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(4000, ErrorMessage = "La descripción no puede exceder 4000 caracteres")]
    public string Description { get; set; }

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    public long CategoryId { get; set; }
}
