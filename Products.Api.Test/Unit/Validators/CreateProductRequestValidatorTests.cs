using FluentAssertions;
using FluentValidation.TestHelper;
using Products.Api.Controllers.Requests;
using Products.Api.Validators;

namespace Products.Api.Test.Unit.Validators;

/// <summary>
/// Tests unitarios para CreateProductRequestValidator.
/// </summary>
public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator;

    public CreateProductRequestValidatorTests()
    {
        _validator = new CreateProductRequestValidator();
    }

    #region Name Validation

    [Fact]
    public void Name_WhenEmpty_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre es requerido");
    }

    [Fact]
    public void Name_WhenTooShort_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "A" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre debe tener al menos 2 caracteres");
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Name = new string('a', 201) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("El nombre no puede exceder 200 caracteres");
    }

    [Fact]
    public void Name_WhenValid_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "Valid Product Name" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Description_WhenEmpty_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Description = "" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("La descripción es requerida");
    }

    [Fact]
    public void Description_WhenTooLong_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Description = new string('a', 4001) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("La descripción no puede exceder 4000 caracteres");
    }

    [Fact]
    public void Description_WhenValid_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Description = "Valid description" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    #endregion

    #region Price Validation

    [Fact]
    public void Price_WhenZero_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Price = 0 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("El precio debe ser mayor a cero");
    }

    [Fact]
    public void Price_WhenNegative_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Price = -10 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorMessage("El precio debe ser mayor a cero");
    }

    [Fact]
    public void Price_WhenValid_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Price = 99.99m };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    #endregion

    #region Stock Validation

    [Fact]
    public void Stock_WhenNegative_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Stock = -1 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock)
            .WithErrorMessage("El stock no puede ser negativo");
    }

    [Fact]
    public void Stock_WhenZero_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Stock = 0 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void Stock_WhenPositive_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Stock = 100 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Stock);
    }

    #endregion

    #region CategoryId Validation

    [Fact]
    public void CategoryId_WhenZero_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { CategoryId = 0 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("La categoría debe ser un ID válido mayor a cero");
    }

    [Fact]
    public void CategoryId_WhenNegative_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { CategoryId = -1 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("La categoría debe ser un ID válido mayor a cero");
    }

    [Fact]
    public void CategoryId_WhenValid_ShouldNotHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { CategoryId = 1 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
    }

    #endregion

    #region Full Object Validation

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Valid Product",
            Description = "Valid Description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidRequest_ShouldHaveMultipleErrors()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "",
            Description = "",
            Price = -10,
            Stock = -5,
            CategoryId = 0
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(5);
    }

    #endregion
}
