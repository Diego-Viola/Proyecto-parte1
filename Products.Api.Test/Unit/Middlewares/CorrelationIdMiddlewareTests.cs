using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Products.Api.Middlewares;

namespace Products.Api.Test.Unit.Middlewares;

public class CorrelationIdMiddlewareTests
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    [Fact]
    public async Task InvokeAsync_WhenNoCorrelationIdHeader_GeneratesNewCorrelationId()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Items[CorrelationIdHeader].Should().NotBeNull();
        context.Response.Headers[CorrelationIdHeader].ToString().Should().NotBeNullOrEmpty();
        
        // Verificar que es un GUID válido
        var correlationId = context.Items[CorrelationIdHeader]?.ToString();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenCorrelationIdHeaderExists_UsesExistingCorrelationId()
    {
        // Arrange
        var existingCorrelationId = "existing-correlation-id-123";
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdHeader] = existingCorrelationId;
        
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Items[CorrelationIdHeader].Should().Be(existingCorrelationId);
        context.Response.Headers[CorrelationIdHeader].ToString().Should().Be(existingCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_AddsCorrelationIdToResponseHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers.Should().ContainKey(CorrelationIdHeader);
        context.Response.Headers[CorrelationIdHeader].ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_SetsTraceIdentifier()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.TraceIdentifier.Should().NotBeNullOrEmpty();
        context.TraceIdentifier.Should().Be(context.Items[CorrelationIdHeader]?.ToString());
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyCorrelationIdHeader_GeneratesNewOne()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdHeader] = "";
        
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var correlationId = context.Items[CorrelationIdHeader]?.ToString();
        correlationId.Should().NotBeNullOrEmpty();
        Guid.TryParse(correlationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WithWhitespaceCorrelationIdHeader_GeneratesNewOne()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdHeader] = "   ";
        
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var correlationId = context.Items[CorrelationIdHeader]?.ToString();
        correlationId.Should().NotBeNullOrEmpty();
        correlationId.Should().NotBe("   ");
    }
}
