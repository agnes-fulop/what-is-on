using System.Net;
using System.Text.Json;
using WhatIsOn.Domain.Exceptions;

namespace WhatIsOn.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogInformation(
                "Domain exception of type {ExceptionType} on {Path}: {Message}",
                exception.GetType().Name, context.Request.Path, exception.Message);
        }

        var problemDetails = new
        {
            type = $"https://httpstatuses.io/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail = statusCode == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : exception.Message
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static (HttpStatusCode StatusCode, string Title) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Access forbidden"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),
            ValidationException => (HttpStatusCode.BadRequest, "Validation failed"),
            _ => (HttpStatusCode.InternalServerError, "Internal server error")
        };
}
