using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.ErrorHandling;

/// <summary>
/// Translates exceptions into RFC 9457 problem details. Expected failures keep their message;
/// anything unexpected is logged and returned as an opaque 500 so internals never leak.
/// </summary>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        (int status, string title, string? code, string detail) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Resource not found", e.Code, e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflict", e.Code, e.Message),
            AuthenticationFailedException e => (StatusCodes.Status401Unauthorized, "Authentication failed", e.Code, e.Message),
            DomainException e => (StatusCodes.Status422UnprocessableEntity, "Business rule violated", e.Code, e.Message),
            OperationCanceledException when httpContext.RequestAborted.IsCancellationRequested =>
                (StatusCodes.Status499ClientClosedRequest, "Request cancelled", null, "The client closed the request."),
            _ => (StatusCodes.Status500InternalServerError, "Server error", null, "An unexpected error occurred.")
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}.",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        ProblemDetails problem = new() { Status = status, Title = title, Detail = detail };

        if (code is not null)
        {
            problem.Extensions["code"] = code;
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }
}
