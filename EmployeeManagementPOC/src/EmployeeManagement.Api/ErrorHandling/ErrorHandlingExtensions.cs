using System.Diagnostics;

namespace EmployeeManagement.Api.ErrorHandling;

public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            // Every problem response (ours, model binding, 404s, auth challenges) carries a trace id
            // that support can correlate with server logs.
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance ??=
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd(
                    "traceId",
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
