using FluentAssertions;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Application.Helpers;

namespace Products.Api.Application.Test.Helpers;

/// <summary>
/// Tests unitarios para ProductEnricherHelper.
/// Verifica que el enriquecimiento de productos genere datos consistentes
/// y completos para la página de detalle del marketplace.
/// </summary>
public class ProductEnricherHelperTests
{
    private ProductDetailOutput CreateBasicProduct(long id = 1, string name = "Test Product", 
        decimal price = 1000m, int stock = 10, long categoryId = 1, string categoryName = "Electronics")
    {
        return new ProductDetailOutput
        {
            Id = id,
            Name = name,
            Description = $"Description for {name}",
            Price = price,
            Stock = stock,
            Category = new CategoryOutput
            {
                Id = categoryId,
                Name = categoryName
            }
        };
    }

    [Fact]
    public void EnrichProduct_WithValidProduct_ReturnsEnrichedProduct()
    {
        // Arrange
        var basicProduct = CreateBasicProduct();

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(basicProduct.Id);
        result.Name.Should().Be(basicProduct.Name);
        result.Description.Should().Be(basicProduct.Description);
    }

    [Fact]
    public void EnrichProduct_GeneratesConsistentDataForSameId()
    {
        // Arrange
        var product1 = CreateBasicProduct(id: 42);
        var product2 = CreateBasicProduct(id: 42);

        // Act
        var result1 = ProductEnricherHelper.EnrichProduct(product1);
        var result2 = ProductEnricherHelper.EnrichProduct(product2);

        // Assert - Same ID should produce same random data (seeded)
        result1.Seller.Id.Should().Be(result2.Seller.Id);
        result1.Seller.Name.Should().Be(result2.Seller.Name);
    }

    [Fact]
    public void EnrichProduct_GeneratesValidSku()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(id: 123, categoryId: 5);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Sku.Should().Be("SKU-005-000123");
    }

    [Fact]
    public void EnrichProduct_GeneratesValidPermalink()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(id: 1, name: "Test Product");

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Permalink.Should().Contain("/product/1/");
        result.Permalink.Should().Contain("test-product");
    }

    [Fact]
    public void EnrichProduct_GeneratesImages()
    {
        // Arrange
        var basicProduct = CreateBasicProduct();

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Images.Should().NotBeEmpty();
        result.Images.Should().HaveCount(5);
        result.Images.First().IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void EnrichProduct_GeneratesBreadcrumbs()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(categoryName: "Electronics");

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Breadcrumbs.Should().NotBeEmpty();
        result.Breadcrumbs.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Breadcrumbs.Last().Name.Should().Be("Electronics");
    }

    [Fact]
    public void EnrichProduct_GeneratesSellerInfo()
    {
        // Arrange
        var basicProduct = CreateBasicProduct();

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Seller.Should().NotBeNull();
        result.Seller.Name.Should().NotBeNullOrEmpty();
        result.Seller.Reputation.Should().NotBeNull();
    }

    [Fact]
    public void EnrichProduct_GeneratesShippingInfo()
    {
        // Arrange
        var basicProduct = CreateBasicProduct();

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Shipping.Should().NotBeNull();
        result.Shipping.Options.Should().NotBeEmpty();
    }

    [Fact]
    public void EnrichProduct_WithZeroStock_ReturnsOutOfStockStatus()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(stock: 0);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Stock.Status.Should().Be("out_of_stock");
        result.Stock.AvailableQuantity.Should().Be(0);
    }

    [Fact]
    public void EnrichProduct_WithLowStock_ReturnsLastUnitsStatus()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(stock: 3);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Stock.Status.Should().Be("last_units");
    }

    [Fact]
    public void EnrichProduct_WithNormalStock_ReturnsAvailableStatus()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(stock: 50);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Stock.Status.Should().Be("available");
        result.Stock.MaxPurchaseQuantity.Should().BeLessThanOrEqualTo(6);
    }

    [Fact]
    public void EnrichProduct_GeneratesRelatedProducts()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(id: 10);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.RelatedProducts.Should().NotBeEmpty();
        result.RelatedProducts.Should().HaveCountGreaterThanOrEqualTo(4);
        // Related products should not include the original product
        result.RelatedProducts.Should().NotContain(p => p.Id == 10);
    }

    [Fact]
    public void EnrichProduct_GeneratesPriceInfo()
    {
        // Arrange
        var basicProduct = CreateBasicProduct(price: 10000m);

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Price.Should().NotBeNull();
        result.Price.Amount.Should().Be(10000m);
        result.Price.Currency.Should().Be("ARS");
        result.Price.PaymentMethods.Should().NotBeEmpty();
    }

    [Fact]
    public void EnrichProduct_GeneratesRatingInfo()
    {
        // Arrange
        var basicProduct = CreateBasicProduct();

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Rating.Should().NotBeNull();
        result.Rating.Average.Should().BeGreaterThan(0);
        result.Rating.TotalReviews.Should().BeGreaterThan(0);
        result.Rating.Distribution.Should().NotBeEmpty();
    }

    [Fact]
    public void EnrichProduct_GeneratesWarrantyInfo()
    {
        // Arrange
        var basicProduct = CreateBasicProduct();

        // Act
        var result = ProductEnricherHelper.EnrichProduct(basicProduct);

        // Assert
        result.Warranty.Should().NotBeNull();
        result.Warranty.DurationMonths.Should().BeGreaterThan(0);
    }
}
