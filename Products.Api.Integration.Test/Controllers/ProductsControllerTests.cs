﻿using FluentAssertions;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Products.Api.Integration.Test.Support;
using Products.Api.Application.DTOs.Generics;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Common;
using Products.Api.Controllers.Requests;
using Products.Api.Controllers.Responses;

namespace Products.Api.Integration.Test.Controllers;

[Category("Integration")]
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _camelCaseOptions;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _camelCaseOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkOrNoContent()
    {
        var response = await _client.GetAsync("/api/v1/products?count=10&page=1");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var paginationResult = await response.Content.ReadFromJsonAsync<PaginationResult<ProductOutput>>(_camelCaseOptions);
            paginationResult.Should().NotBeNull();
            paginationResult!.Items.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var response = await _client.GetAsync("/api/v1/products/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_camelCaseOptions);
        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.NotFound);
        error.Title.Should().Be("Not Found");
        error.Type.Should().Be("https://yourdomain.com/errors/not-found");
    }

    [Fact]
    public async Task Create_ReturnsCreated_AndCanBeRetrieved()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();
        created!.Name.Should().Be(createRequest.Name);

        var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    public async Task GetById_ReturnsProductDetails_WhenProductExists()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v1/products/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();

        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 20.0m,
            Stock = 10,
            CategoryId = 1
        };

        var putResponse = await _client.PutAsync(
            $"/api/v1/products/{created!.Id}",
            JsonContent.Create(updateRequest, options: _camelCaseOptions)
        );
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be(updateRequest.Name);
        fetched.Description.Should().Be(updateRequest.Description);
        fetched.Price.Should().Be(updateRequest.Price);
        fetched.Stock.Should().Be(updateRequest.Stock);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/products/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #region GetDetailById Tests

    [Fact]
    public async Task GetDetailById_ReturnsEnrichedProductDetails_WhenProductExists()
    {
        // Arrange: Crear un producto primero
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description for Enriched Detail",
            Price = 999.99m,
            Stock = 10,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);

        // Act: Obtener el detalle enriquecido
        var response = await _client.GetAsync($"/api/v1/products/{created!.Id}/detail");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var enrichedProduct = await response.Content.ReadFromJsonAsync<ProductDetailEnrichedResponse>(_camelCaseOptions);
        enrichedProduct.Should().NotBeNull();
        enrichedProduct!.Id.Should().Be(created.Id);
        enrichedProduct.Name.Should().Be(createRequest.Name);
        enrichedProduct.Description.Should().Be(createRequest.Description);
        enrichedProduct.Sku.Should().NotBeNullOrEmpty();

        // Verificar que tiene datos enriquecidos
        enrichedProduct.Price.Should().NotBeNull();
        enrichedProduct.Price.Amount.Should().Be(createRequest.Price);
        enrichedProduct.Price.Currency.Should().Be("ARS");
        enrichedProduct.Price.PaymentMethods.Should().NotBeEmpty();

        enrichedProduct.Stock.Should().NotBeNull();
        enrichedProduct.Stock.AvailableQuantity.Should().Be(createRequest.Stock);
        enrichedProduct.Stock.Status.Should().Be("available");

        enrichedProduct.Images.Should().NotBeEmpty();
        enrichedProduct.Images.Should().HaveCount(5);
        enrichedProduct.Images.First().IsPrimary.Should().BeTrue();

        enrichedProduct.Category.Should().NotBeNull();
        enrichedProduct.Category.Id.Should().Be(createRequest.CategoryId);

        enrichedProduct.Breadcrumbs.Should().NotBeEmpty();
        enrichedProduct.Breadcrumbs.Should().HaveCountGreaterThan(0);

        enrichedProduct.Seller.Should().NotBeNull();
        enrichedProduct.Seller.Id.Should().BeGreaterThan(0);
        enrichedProduct.Seller.Name.Should().NotBeNullOrEmpty();
        enrichedProduct.Seller.Reputation.Should().NotBeNull();

        enrichedProduct.Attributes.Should().NotBeEmpty();
        enrichedProduct.Shipping.Should().NotBeNull();
        enrichedProduct.Shipping.Options.Should().NotBeEmpty();

        enrichedProduct.Rating.Should().NotBeNull();
        enrichedProduct.Rating.Average.Should().BeInRange(0, 5);

        enrichedProduct.RelatedProducts.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetDetailById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/99999/detail");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_camelCaseOptions);
        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDetailById_ReturnsConsistentDataStructure()
    {
        // Arrange: Crear producto
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Consistency Test",
            Price = 500m,
            Stock = 0, // Stock cero para verificar status
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{created!.Id}/detail");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var enrichedProduct = await response.Content.ReadFromJsonAsync<ProductDetailEnrichedResponse>(_camelCaseOptions);
        enrichedProduct.Should().NotBeNull();

        // Con stock 0 el status debería ser out_of_stock
        enrichedProduct!.Stock.Status.Should().Be("out_of_stock");
        enrichedProduct.Stock.AvailableQuantity.Should().Be(0);

        // Verificar que el permalink está bien formado
        enrichedProduct.Permalink.Should().Contain($"product/{enrichedProduct.Id}");

        // Metadatos deben estar presentes
        enrichedProduct.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        enrichedProduct.UpdatedAt.Should().BeBefore(DateTime.UtcNow);
    }

    #endregion

    #region GetRelatedProducts Tests

    [Fact]
    public async Task GetRelatedProducts_ReturnsListOfProducts_WhenProductExists()
    {
        // Arrange: Crear un producto
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Product for Related Test",
            Price = 1000m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{created!.Id}/related");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var relatedProducts = await response.Content.ReadFromJsonAsync<List<ProductSummaryResponse>>(_camelCaseOptions);
        relatedProducts.Should().NotBeNull();
        relatedProducts.Should().NotBeEmpty();
        relatedProducts.Should().HaveCount(6); // Default limit

        // Verificar estructura de cada producto relacionado
        foreach (var product in relatedProducts!)
        {
            product.Id.Should().BeGreaterThan(0);
            product.Id.Should().NotBe(created.Id); // No debe incluir el producto actual
            product.Name.Should().NotBeNullOrEmpty();
            product.Price.Should().BeGreaterThan(0);
            product.ThumbnailUrl.Should().NotBeNullOrEmpty();
            product.Rating.Should().BeInRange(0, 5);
        }
    }

    [Fact]
    public async Task GetRelatedProducts_RespectsLimitParameter()
    {
        // Arrange: Crear un producto
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Product for Limit Test",
            Price = 1500m,
            Stock = 3,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);

        // Act: Solicitar solo 3 productos relacionados
        var response = await _client.GetAsync($"/api/v1/products/{created!.Id}/related?limit=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var relatedProducts = await response.Content.ReadFromJsonAsync<List<ProductSummaryResponse>>(_camelCaseOptions);
        relatedProducts.Should().NotBeNull();
        relatedProducts.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetRelatedProducts_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/99999/related");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_camelCaseOptions);
        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRelatedProducts_ValidatesLimitRange()
    {
        // Arrange: Crear un producto
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Product for Validation Test",
            Price = 800m,
            Stock = 2,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);

        // Act: Intentar con límite fuera de rango (mayor a 20)
        var response = await _client.GetAsync($"/api/v1/products/{created!.Id}/related?limit=25");

        // Assert: Debería fallar la validación
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRelatedProducts_ReturnsConsistentResultsForSameProduct()
    {
        // Arrange: Crear un producto
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Product for Consistency Test",
            Price = 2000m,
            Stock = 8,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);

        // Act: Llamar dos veces al mismo endpoint
        var response1 = await _client.GetAsync($"/api/v1/products/{created!.Id}/related?limit=5");
        var response2 = await _client.GetAsync($"/api/v1/products/{created.Id}/related?limit=5");

        // Assert: Ambas respuestas deben ser OK
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var relatedProducts1 = await response1.Content.ReadFromJsonAsync<List<ProductSummaryResponse>>(_camelCaseOptions);
        var relatedProducts2 = await response2.Content.ReadFromJsonAsync<List<ProductSummaryResponse>>(_camelCaseOptions);

        // Los resultados deberían ser consistentes (mismos IDs debido al seed basado en ID de producto)
        relatedProducts1.Should().HaveCount(5);
        relatedProducts2.Should().HaveCount(5);

        for (int i = 0; i < 5; i++)
        {
            relatedProducts1![i].Id.Should().Be(relatedProducts2![i].Id);
        }
    }

    #endregion
}