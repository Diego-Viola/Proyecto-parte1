using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Products.Api.Test.Integration.Endpoints;

/// <summary>
/// Tests de integración para los endpoints de Categories.
/// </summary>
public class CategoriesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    #region GET /api/v1/categories

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        var response = await _client.GetAsync("/api/v1/categories");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    #endregion

    #region GET /api/v1/categories/{id}

    [Fact]
    public async Task GetById_WhenCategoryExists_ReturnsOk()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task GetById_WhenCategoryNotExists_ReturnsNotFound()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion

    #region POST /api/v1/categories

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
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
