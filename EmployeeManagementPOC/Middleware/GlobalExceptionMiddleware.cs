using System.Net;
using System.Text.Json;

namespace EmployeeManagementPOC.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(
                ex,
                "Unhandled exception occurred while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();

        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;

        context.Response.ContentType =
            "application/json";

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = "An unexpected error occurred."
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}