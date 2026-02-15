using FluentValidation;
using Products.Api.Application.DTOs.Inputs.Products;

namespace Products.Api.Validators;

public class CreateProductInputValidator : AbstractValidator<CreateProductInput>
{
    public CreateProductInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres")
            .MinimumLength(2).WithMessage("El nombre debe tener al menos 2 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(4000).WithMessage("La descripción no puede exceder 4000 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a cero")
            .LessThanOrEqualTo(decimal.MaxValue).WithMessage("El precio excede el valor máximo permitido");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo")
            .LessThanOrEqualTo(int.MaxValue).WithMessage("El stock excede el valor máximo permitido");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("La categoría debe ser un ID válido mayor a cero");
    }
}
