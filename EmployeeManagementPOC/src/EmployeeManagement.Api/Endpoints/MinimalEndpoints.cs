using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Application.Features.Dashboard;
using EmployeeManagement.Application.Features.Departments;
using EmployeeManagement.Application.Features.Employees;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Read-only minimal API endpoints that sit beside the controllers and reuse the same application services.
/// Unversioned, so they appear in every Swagger document.
/// </summary>
public static class MinimalEndpoints
{
    public static IEndpointRouteBuilder MapMinimalEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/minimal")
            .WithTags("Minimal")
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/ping", () => TypedResults.Ok(new PingResponse("pong", DateTimeOffset.UtcNow)))
            .AllowAnonymous();

        group.MapGet("/departments", (IDepartmentService departments, CancellationToken cancellationToken) =>
                departments.GetAllAsync(cancellationToken))
            .RequireAuthorization(Policies.EmployeeView);

        group.MapGet("/departments/{id:int}", (int id, IDepartmentService departments, CancellationToken cancellationToken) =>
                departments.GetByIdAsync(id, cancellationToken))
            .RequireAuthorization(Policies.DepartmentView)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/employees/{id:int}", (int id, IEmployeeService employees, CancellationToken cancellationToken) =>
                employees.GetByIdAsync(id, cancellationToken))
            .RequireAuthorization(Policies.EmployeeView)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/dashboard", (IDashboardService dashboard, CancellationToken cancellationToken) =>
                dashboard.GetAsync(cancellationToken))
            .RequireAuthorization(Policies.DashboardAccess);

        return app;
    }
}

public sealed record PingResponse(string Message, DateTimeOffset ServerTime);
