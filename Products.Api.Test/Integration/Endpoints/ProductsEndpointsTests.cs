using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Products.Api.Test.Integration.Endpoints;

/// <summary>
/// Tests de integración para los endpoints de Products.
/// </summary>
public class ProductsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    #region GET /api/v1/products

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectPageSize()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion

    #region GET /api/v1/products/{id}

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsOk()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task GetById_WhenProductNotExists_ReturnsNotFound()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion

    #region GET /api/v1/products/{id}/detail

    [Fact]
    public async Task GetDetailById_WhenProductExists_ReturnsEnrichedProduct()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion
}
