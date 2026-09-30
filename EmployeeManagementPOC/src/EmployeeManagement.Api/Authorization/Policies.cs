using EmployeeManagement.Application.Common.Security;

namespace EmployeeManagement.Api.Authorization;

public static class Policies
{
    public const string DashboardAccess = nameof(DashboardAccess);
    public const string EmployeeView = nameof(EmployeeView);
    public const string EmployeeManage = nameof(EmployeeManage);
    public const string DepartmentView = nameof(DepartmentView);
    public const string DepartmentManage = nameof(DepartmentManage);

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(DashboardAccess, policy => policy.RequireRole(Roles.All))
            .AddPolicy(EmployeeView, policy => policy.RequireRole(Roles.All))
            .AddPolicy(EmployeeManage, policy => policy.RequireRole(Roles.Admin, Roles.HR))
            .AddPolicy(DepartmentView, policy => policy.RequireRole(Roles.Admin, Roles.HR, Roles.Manager))
            .AddPolicy(DepartmentManage, policy => policy.RequireRole(Roles.Admin, Roles.HR));

        return services;
    }
}
