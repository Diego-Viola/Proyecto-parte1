﻿using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Products.Api.Application.DTOs.Generics;
using Products.Api.Application.DTOs.Outputs.Categories;

namespace Products.Api.Test.Integration.Endpoints;

/// <summary>
/// Tests de integración para los endpoints de Categories.
/// </summary>
public class CategoriesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public CategoriesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    #region GET /api/v1/categories

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetAll_ReturnsCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadFromJsonAsync<PaginationResult<CategoryOutput>>(_jsonOptions);
            content.Should().NotBeNull();
            content!.Items.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetAll_WithPagination_RespectsPageSize()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories?count=2&page=1");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadFromJsonAsync<PaginationResult<CategoryOutput>>(_jsonOptions);
            content.Should().NotBeNull();
            content!.Items.Count().Should().BeLessThanOrEqualTo(2);
        }
    }

    #endregion

    #region GET /api/v1/categories/{id}

    [Fact]
    public async Task GetById_WhenCategoryExists_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var category = await response.Content.ReadFromJsonAsync<CategoryOutput>(_jsonOptions);
        category.Should().NotBeNull();
        category!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_WhenCategoryNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/categories/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/categories

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newCategory = new { Name = $"Integration Test Category {Guid.NewGuid()}" };
        var content = new StringContent(
            JsonSerializer.Serialize(newCategory),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/categories", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdCategory = await response.Content.ReadFromJsonAsync<CategoryOutput>(_jsonOptions);
        createdCategory.Should().NotBeNull();
        createdCategory!.Name.Should().Contain("Integration Test Category");
    }

    [Fact]
    public async Task Create_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var invalidCategory = new { Name = "" };
        var content = new StringContent(
            JsonSerializer.Serialize(invalidCategory),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/categories", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithTooLongName_ReturnsBadRequest()
    {
        // Arrange
        var invalidCategory = new { Name = new string('a', 101) }; // Exceeds 100 char limit
        var content = new StringContent(
            JsonSerializer.Serialize(invalidCategory),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/categories", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}
