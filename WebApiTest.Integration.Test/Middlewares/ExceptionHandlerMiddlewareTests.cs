using FluentAssertions;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using WebApiTest.Integration.Test.Support;

namespace WebApiTest.Integration.Test.Middlewares;

[Category("Integration")]
public class ExceptionHandlerMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExceptionHandlerMiddlewareTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Returns_BadRequest_When_ModelValidation_Fails()
    {
        var request = new
        {
            Name = "",
            CategoryId = 1,
            Price = 100
        };

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
    public async Task Returns_BadRequest_When_BadRequestException_Is_Thrown()
    {
        var count = -1;
        var page = 1;

        var response = await _client.GetAsync($"/api/v1/products?count={count}&page={page}");

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
            Name = "Hogar",
        };

        var response = await _client.PostAsJsonAsync("/api/v1/categories", request);

        //response.StatusCode.Should().Be((HttpStatusCode)422);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("\"title\":\"Business rule violated\"");
        content.Should().Contain("\"type\":\"https://yourdomain.com/errors/business\"");
        content.Should().Contain("/api/v1/categories");
        content.Should().Contain("traceId");
    }
}