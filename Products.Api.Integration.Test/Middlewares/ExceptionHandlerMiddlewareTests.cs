﻿using FluentAssertions;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using Products.Api.Integration.Test.Support;

namespace Products.Api.Integration.Test.Middlewares;

[Category("Integration")]
[Collection("IntegrationTests")]
public class ExceptionHandlerMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExceptionHandlerMiddlewareTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Returns_BadRequest_When_ModelValidation_Fails_On_Create()
    {

        var request = new
        {
            Name = "",
            Description = "Test",
            CategoryId = 1,
            Price = 100,
            Stock = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"title\":\"Bad Request\"");
        content.Should().Contain("\"type\":\"https://yourdomain.com/errors/bad-request\"");
        content.Should().Contain("/api/v1/products");
        content.Should().Contain("traceId");
        content.Should().Contain("Name");
    }

    [Fact]
    public async Task Returns_BadRequest_When_FluentValidation_Fails_On_Price()
    {
        var request = new
        {
            Name = "Test Product",
            Description = "Test",
            CategoryId = 1,
            Price = -100,
            Stock = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"title\":\"Bad Request\"");
        content.Should().Contain("\"type\":\"https://yourdomain.com/errors/bad-request\"");
        content.Should().Contain("/api/v1/products");
        content.Should().Contain("traceId");
    }

    [Fact]
    public async Task Returns_NotFound_When_NotFoundException_Is_Thrown()
    {
        var id = long.MaxValue;

        var response = await _client.GetAsync($"/api/v1/products/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"title\":\"Not Found\"");
        content.Should().Contain("\"type\":\"https://yourdomain.com/errors/not-found\"");
        content.Should().Contain("/api/v1/products");
        content.Should().Contain("traceId");
    }

    [Fact]
    public async Task Returns_UnprocessableEntity_When_BusinessException_Is_Thrown()
    {
        var request = new
        {
            Name = "TestCategoryDuplicate",
        };
        
        await _client.PostAsJsonAsync("/api/v1/categories", request);
        
        var response = await _client.PostAsJsonAsync("/api/v1/categories", request);
        
        response.StatusCode.Should().Be((HttpStatusCode)422);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"title\":\"Business rule violated\"");
        content.Should().Contain("\"type\":\"https://yourdomain.com/errors/business\"");
        content.Should().Contain("/api/v1/categories");
        content.Should().Contain("traceId");
        content.Should().Contain("Ya existe una categoria con el mismo nombre");
    }
}