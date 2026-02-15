using FluentAssertions;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.Helpers;

namespace Products.Api.Test.Unit.Helpers;

/// <summary>
/// Tests unitarios para ProductEnricherHelper.
/// </summary>
public class ProductEnricherHelperTests
{
    [Fact]
    public void EnrichProduct_WithValidProduct_ReturnsEnrichedOutput()
    {
        // Arrange
        var basicProduct = CreateTestProduct(1);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Product");
        result.Description.Should().Be("Test Description");
    }

    [Fact]
    public void EnrichProduct_GeneratesConsistentDataForSameProductId()
    {
        // Arrange
        var product1 = CreateTestProduct(1);
        var product2 = CreateTestProduct(1);

        // Act
        var result1 = ProductEnricherHelper.EnrichProduct(product1);
        var result2 = ProductEnricherHelper.EnrichProduct(product2);

        // Assert - El seed basado en ID debe generar los mismos datos
        result1.Seller.Id.Should().Be(result2.Seller.Id);
        result1.Seller.Name.Should().Be(result2.Seller.Name);
        result1.Sku.Should().Be(result2.Sku);
    }

    [Fact]
    public void EnrichProduct_IncludesAllRequiredFields()
    {
        // Arrange
        var basicProduct = CreateTestProduct(1);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert - Verificar que todos los campos requeridos están presentes
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().NotBeNullOrEmpty();
        result.Sku.Should().NotBeNullOrEmpty();
        result.Condition.Should().NotBeNullOrEmpty();
        
        // Precio
        result.Price.Should().NotBeNull();
        result.Price.Amount.Should().BeGreaterThan(0);
        result.Price.Currency.Should().Be("ARS");
        result.Price.PaymentMethods.Should().NotBeEmpty();
        
        // Stock
        result.Stock.Should().NotBeNull();
        result.Stock.AvailableQuantity.Should().BeGreaterThanOrEqualTo(0);
        result.Stock.Status.Should().NotBeNullOrEmpty();
        
        // Imágenes
        result.Images.Should().NotBeEmpty();
        result.Images.Should().AllSatisfy(img =>
        {
            img.Url.Should().NotBeNullOrEmpty();
            img.ThumbnailUrl.Should().NotBeNullOrEmpty();
        });
        
        // Categoría
        result.Category.Should().NotBeNull();
        result.Category.Name.Should().NotBeNullOrEmpty();
        
        // Breadcrumbs
        result.Breadcrumbs.Should().NotBeEmpty();
        
        // Vendedor
        result.Seller.Should().NotBeNull();
        result.Seller.Name.Should().NotBeNullOrEmpty();
        result.Seller.Reputation.Should().NotBeNull();
        result.Seller.Location.Should().NotBeNull();
        
        // Atributos
        result.Attributes.Should().NotBeEmpty();
        
        // Envío
        result.Shipping.Should().NotBeNull();
        result.Shipping.Options.Should().NotBeEmpty();
        
        // Rating
        result.Rating.Should().NotBeNull();
        result.Rating.Average.Should().BeGreaterThanOrEqualTo(0);
        result.Rating.Distribution.Should().NotBeEmpty();
        
        // Garantía
        result.Warranty.Should().NotBeNull();
        
        // Productos relacionados
        result.RelatedProducts.Should().NotBeEmpty();
        
        // Metadatos
        result.Permalink.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void EnrichProduct_GeneratesCorrectSku()
    {
        // Arrange
        var product = CreateTestProduct(123, categoryId: 5);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Sku.Should().Be("SKU-005-000123");
    }

    [Fact]
    public void EnrichProduct_GeneratesCorrectBreadcrumbs()
    {
        // Arrange
        var product = CreateTestProduct(1);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Breadcrumbs.Should().HaveCount(3);
        result.Breadcrumbs[0].Name.Should().Be("Inicio");
        result.Breadcrumbs[0].Level.Should().Be(0);
        result.Breadcrumbs[2].Name.Should().Be("Electronics");
        result.Breadcrumbs[2].Level.Should().Be(2);
    }

    [Fact]
    public void EnrichProduct_WithZeroStock_SetsOutOfStockStatus()
    {
        // Arrange
        var product = CreateTestProduct(1, stock: 0);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Stock.Status.Should().Be("out_of_stock");
        result.Stock.AvailableQuantity.Should().Be(0);
    }

    [Fact]
    public void EnrichProduct_WithLowStock_SetsLastUnitsStatus()
    {
        // Arrange
        var product = CreateTestProduct(1, stock: 3);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Stock.Status.Should().Be("last_units");
    }

    [Fact]
    public void EnrichProduct_WithHighStock_SetsAvailableStatus()
    {
        // Arrange
        var product = CreateTestProduct(1, stock: 100);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Stock.Status.Should().Be("available");
    }

    [Fact]
    public void EnrichProduct_GeneratesValidPermalink()
    {
        // Arrange
        var product = CreateTestProduct(1);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Permalink.Should().Contain("marketplace.com/product/1/");
        result.Permalink.Should().Contain("test-product");
    }

    [Fact]
    public void EnrichProduct_GeneratesMultipleImages()
    {
        // Arrange
        var product = CreateTestProduct(1);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.Images.Should().HaveCount(5);
        result.Images.First().IsPrimary.Should().BeTrue();
        result.Images.Skip(1).Should().AllSatisfy(img => img.IsPrimary.Should().BeFalse());
    }

    [Fact]
    public void EnrichProduct_RelatedProductsDoNotIncludeOriginal()
    {
        // Arrange
        var product = CreateTestProduct(1);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(product);

        // Assert
        result.RelatedProducts.Should().AllSatisfy(rp => rp.Id.Should().NotBe(1));
    }

    #region Helper Methods

    private static ProductDetailOutput CreateTestProduct(
        long id,
        string name = "Test Product",
        decimal price = 1000m,
        int stock = 10,
        long categoryId = 1)
    {
        return new ProductDetailOutput
        {
            Id = id,
            Name = name,
            Description = "Test Description",
            Price = price,
            Stock = stock,
            Category = new CategoryOutput
            {
                Id = categoryId,
                Name = "Electronics"
            }
        };
    }

    #endregion
}
