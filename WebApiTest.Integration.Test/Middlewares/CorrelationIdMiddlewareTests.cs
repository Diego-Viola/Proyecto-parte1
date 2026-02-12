using FluentAssertions;
using System.ComponentModel;
using System.Net;
using WebApiTest.Integration.Test.Support;

namespace WebApiTest.Integration.Test.Middlewares;

[Category("Integration")]
public class CorrelationIdMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CorrelationIdMiddlewareTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Returns_CorrelationId_When_Header_Is_Present()
    {
        var correlationId = Guid.NewGuid().ToString();
        _client.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);
        var count = 1;
        var page = 1;

        var response = await _client.GetAsync($"/api/v1/products?count={count}&page={page}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.TryGetValues("X-Correlation-ID", out var values).Should().BeTrue();
        values.Should().NotBeNull();
        values!.First().Should().Be(correlationId);
    }

    [Fact]
    public async Task Generates_CorrelationId_When_Header_Is_Missing()
    {
        var count = 1;
        var page = 1;

        var response = await _client.GetAsync($"/api/v1/products?count={count}&page={page}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.TryGetValues("X-Correlation-ID", out var values).Should().BeTrue();
        values.Should().NotBeNull();
        var correlationId = values!.First();
        correlationId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }
}