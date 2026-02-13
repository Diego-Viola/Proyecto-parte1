using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Products.Api.Test.Integration.Endpoints;

/// <summary>
/// Tests de integración para el endpoint de Health Check.
/// </summary>
public class HealthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthCheck_ReturnsAppInfo()
    {
        // Arrange
        // TODO: Implementar test

        // Act
        await Task.CompletedTask; // Placeholder

        // Assert
        Assert.True(true); // Placeholder
    }
}
