using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using WebApiTest.Application.Exceptions;
using WebApiTest.Common;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Exceptions;

namespace WebApiTest.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> logger;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly RequestDelegate next;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IOptions<JsonOptions> jsonOptions)
    {
        this.logger = logger;
        this.jsonOptions = jsonOptions.Value.SerializerOptions;
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InputException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            await CreateResponse(context, ex, string.Empty, ex.Errors);
        }
        catch (BadRequestException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            await CreateResponse(context, ex);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            await CreateResponse(context, ex);
        }
        catch (BusinessException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.UnprocessableEntity);
            await CreateResponse(context, ex, ex.Code);
        }
        catch (DataIntegrationException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            await CreateResponse(context, ex);
        }
        catch (TimeoutException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.ServiceUnavailable);
            await CreateResponse(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            await CreateResponse(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
    {
        await Task.Run(() => logger.LogError(ex, "{message}", ex.Message));
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
    }

    private async Task CreateResponse(HttpContext context, Exception ex, string code = "", IDictionary<string, string[]>? errors = null)
    {
        var traceId = context.TraceIdentifier;
        var instance = context.Request.Path;
        var status = context.Response.StatusCode;

        string detail;
        if (errors != null && errors.Any())
        {
            detail = string.Join(" | ", errors.Select(e =>
                $"{e.Key}: {string.Join(", ", e.Value)}"
            ));
        }
        else
        {
            detail = ex.Message;
        }

        var error = new ErrorResponse
        {
            Status = status,
            Code = string.IsNullOrEmpty(code) ? status.ToString() : code,
            Detail = detail,
            Instance = instance,
            TraceId = traceId
        };

        switch (ex)
        {
            case InputException:
            case BadRequestException:
                error.Type = "https://yourdomain.com/errors/bad-request";
                error.Title = "Bad Request";
                break;
            case NotFoundException:
                error.Type = "https://yourdomain.com/errors/not-found";
                error.Title = "Not Found";
                break;
            case BusinessException:
                error.Type = "https://yourdomain.com/errors/business";
                error.Title = "Business rule violated";
                break;
            case TimeoutException:
                error.Type = "https://yourdomain.com/errors/timeout";
                error.Title = "Timeout";
                break;
            default:
                error.Type = "https://yourdomain.com/errors/internal";
                error.Title = "Internal server error";
                break;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(error, jsonOptions));
    }
}
