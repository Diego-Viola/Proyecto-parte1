using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net.Http.Json;
using Products.Api.Integration.Test.Support;

namespace Products.Api.Integration.Test.Middlewares;

[Category("Integration")]
public class RequestResponseLoggingMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestLoggerProvider _loggerProvider;

    public RequestResponseLoggingMiddlewareTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _loggerProvider = new TestLoggerProvider();
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(_loggerProvider);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Logs_Request_And_Response_On_GetAll()
    {
        var count = 1;
        var page = 1;

        var response = await _client.GetAsync($"/api/v1/categories?count={count}&page={page}");

        var logs = _loggerProvider.GetLogs();
        logs.Should().ContainSingle(x => x.Contains("Incoming Request: GET /api/v1/categories"));
        logs.Should().Contain(x => x.Contains("Outgoing Response:"));
    }

    [Fact]
    public async Task Logs_Request_And_Response_On_GetById()
    {
        var id = 1L;

        var response = await _client.GetAsync($"/api/v1/categories/{id}");

        var logs = _loggerProvider.GetLogs();
        logs.Should().ContainSingle(x => x.Contains($"Incoming Request: GET /api/v1/categories/{id}"));
        logs.Should().Contain(x => x.Contains("Outgoing Response:"));
    }

    [Fact]
    public async Task Logs_Request_And_Response_On_Create()
    {
        var request = new { name = "TestCategory" };

        var response = await _client.PostAsJsonAsync("/api/v1/categories", request);

        var logs = _loggerProvider.GetLogs();
        logs.Should().ContainSingle(x => x.Contains("Incoming Request: POST /api/v1/categories"));
        logs.Should().Contain(x => x.Contains("Outgoing Response:"));
        logs.Should().Contain(x => x.Contains("TestCategory"));
    }
}
