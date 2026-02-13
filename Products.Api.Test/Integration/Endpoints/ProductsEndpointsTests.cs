using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Products.Api.Application.DTOs.Generics;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Controllers.Responses;
using Products.Api.Common;

namespace Products.Api.Test.Integration.Endpoints;

/// <summary>
/// Tests de integración para los endpoints de Products.
/// </summary>
public class ProductsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProductsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    #region GET /api/v1/products

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectStructure()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products?count=5&page=1");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadFromJsonAsync<PaginationResult<ProductOutput>>(_jsonOptions);
            content.Should().NotBeNull();
            content!.Items.Should().NotBeNull();
            content.Items.Count().Should().BeLessThanOrEqualTo(5);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        }
    }

    #endregion

    #region GET /api/v1/products/{id}

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var product = await response.Content.ReadFromJsonAsync<ProductDetailOutput>(_jsonOptions);
        product.Should().NotBeNull();
        product!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_WhenProductNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ReturnsProductWithCategory()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var product = await response.Content.ReadFromJsonAsync<ProductDetailOutput>(_jsonOptions);
        product.Should().NotBeNull();
        product!.Category.Should().NotBeNull();
        product.Category.Name.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region GET /api/v1/products/{id}/detail

    [Fact]
    public async Task GetDetailById_WhenProductExists_ReturnsEnrichedProduct()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/1/detail");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var enrichedProduct = await response.Content.ReadFromJsonAsync<ProductDetailEnrichedResponse>(_jsonOptions);
        enrichedProduct.Should().NotBeNull();
        enrichedProduct!.Id.Should().Be(1);
        enrichedProduct.Images.Should().NotBeEmpty();
        enrichedProduct.Seller.Should().NotBeNull();
        enrichedProduct.Shipping.Should().NotBeNull();
        enrichedProduct.Rating.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetailById_WhenProductNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/999999/detail");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDetailById_IncludesAllMarketplaceData()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/1/detail");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        
        // Verificar que contiene todos los campos de marketplace
        content.Should().Contain("images");
        content.Should().Contain("seller");
        content.Should().Contain("shipping");
        content.Should().Contain("rating");
        content.Should().Contain("attributes");
        content.Should().Contain("breadcrumbs");
        content.Should().Contain("relatedProducts");
    }

    #endregion

    #region GET /api/v1/products/{id}/related

    [Fact]
    public async Task GetRelatedProducts_WhenProductExists_ReturnsRelatedProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/1/related?limit=6");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var relatedProducts = await response.Content.ReadFromJsonAsync<List<ProductSummaryResponse>>(_jsonOptions);
        relatedProducts.Should().NotBeNull();
        relatedProducts!.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetRelatedProducts_WhenProductNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/999999/related");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/products

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newProduct = new
        {
            Name = $"Integration Test Product {Guid.NewGuid()}",
            Description = "Test description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = 1
        };
        var content = new StringContent(
            JsonSerializer.Serialize(newProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidProduct = new
        {
            Name = "", // Invalid - empty name
            Description = "Test",
            Price = -10, // Invalid - negative price
            Stock = 10,
            CategoryId = 1
        };
        var content = new StringContent(
            JsonSerializer.Serialize(invalidProduct),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithNonExistentCategory_ReturnsError()
    {
        // Arrange
        var productWithBadCategory = new
        {
            Name = $"Product with bad category {Guid.NewGuid()}",
            Description = "Test description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = 999999 // Non-existent category
        };
        var content = new StringContent(
            JsonSerializer.Serialize(productWithBadCategory),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/products", content);

        // Assert
        // Puede ser 400 (BadRequest por validación de negocio), 404 (NotFound) o 422 (Unprocessable)
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, 
            HttpStatusCode.NotFound, 
            HttpStatusCode.UnprocessableEntity);
    }

    #endregion

    #region DELETE /api/v1/products/{id}

    [Fact]
    public async Task Delete_WhenProductNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/v1/products/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Correlation ID Tests

    [Fact]
    public async Task AllEndpoints_ReturnCorrelationIdHeader()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.Headers.Should().ContainKey("X-Correlation-ID");
        response.Headers.GetValues("X-Correlation-ID").First().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AllEndpoints_UseProvidedCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/products");
        request.Headers.Add("X-Correlation-ID", correlationId);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.Headers.GetValues("X-Correlation-ID").First().Should().Be(correlationId);
    }

    #endregion
}
