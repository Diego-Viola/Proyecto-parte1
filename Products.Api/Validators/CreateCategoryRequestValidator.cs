﻿using FluentValidation;
using Products.Api.Controllers.Requests;

namespace Products.Api.Validators;

/// <summary>
/// Validador FluentValidation para CreateCategoryRequest.
/// </summary>
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MinimumLength(1).WithMessage("El nombre debe tener al menos 1 caracter")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
    }
}
