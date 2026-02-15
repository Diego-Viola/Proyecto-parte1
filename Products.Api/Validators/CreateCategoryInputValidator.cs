using FluentValidation;
using Products.Api.Application.DTOs.Inputs.Category;

namespace Products.Api.Validators;

public class CreateCategoryInputValidator : AbstractValidator<CreateCategoryInput>
{
    public CreateCategoryInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MinimumLength(1).WithMessage("El nombre debe tener al menos 1 caracter")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
    }
}
