using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Products.Api.Middlewares;

namespace Products.Api.Test.Unit.Middlewares;

/// <summary>
/// Tests unitarios para ExceptionHandlerMiddleware.
/// </summary>
public class ExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextMiddleware()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task InvokeAsync_WhenInputExceptionThrown_ReturnsBadRequest()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }
}
