﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;
using Products.Api.Middlewares;
using Products.Api.Application.Exceptions;
using Products.Api.Domain.Exceptions;
using Products.Api.Exceptions;

namespace Products.Api.Test.Unit.Middlewares;

/// <summary>
/// Tests unitarios para ExceptionHandlerMiddleware.
/// </summary>
public class ExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock;
    private readonly IOptions<JsonOptions> _jsonOptions;

    public ExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        _jsonOptions = Options.Create(new JsonOptions());
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var nextCalled = false;
        
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };
        
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200); // Default status
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundExceptionThrown_Returns404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = _ => throw new NotFoundException("Resource not found", "NOT_FOUND");
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_WhenBusinessExceptionThrown_Returns422()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = _ => throw new BusinessException("Business rule violated", "BUSINESS_ERROR");
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(422);
    }

    [Fact]
    public async Task InvokeAsync_WhenBadRequestExceptionThrown_Returns400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = _ => throw new BadRequestException("Bad request", "BAD_REQUEST");
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_WhenInputExceptionThrown_Returns400WithErrors()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } },
            { "Price", new[] { "Price must be greater than zero" } }
        };
        
        RequestDelegate next = _ => throw new InputException(errors);
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_WhenGenericExceptionThrown_Returns500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = _ => throw new Exception("Unexpected error");
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task InvokeAsync_WhenTimeoutExceptionThrown_Returns503()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = _ => throw new TimeoutException("Operation timed out");
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(503);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var exception = new Exception("Test error");
        RequestDelegate next = _ => throw exception;
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_SetsContentTypeToJson()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        RequestDelegate next = _ => throw new Exception("Error");
        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object, _jsonOptions);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.ContentType.Should().Be("application/json");
    }
}
