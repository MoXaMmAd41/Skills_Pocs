using Asp.Versioning;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.Versioning;
using EmployeeManagement.Application.Features.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers.V1;

/// <summary>
/// Original dashboard contract. Superseded by v2, which adds server-computed fields;
/// kept for existing clients and advertised as deprecated via the api-deprecated-versions header.
/// </summary>
[ApiVersion(ApiVersions.V1, Deprecated = true)]
[Authorize(Policy = Policies.DashboardAccess)]
public sealed class DashboardController(IDashboardService dashboardService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<DashboardV1Response>(StatusCodes.Status200OK)]
    public async Task<DashboardV1Response> Get(CancellationToken cancellationToken)
    {
        DashboardResponse dashboard = await dashboardService.GetAsync(cancellationToken);

        return new DashboardV1Response(
            dashboard.TotalEmployees,
            dashboard.ActiveEmployees,
            dashboard.TotalDepartments,
            dashboard.AverageSalary,
            dashboard.EmployeesByDepartment
                .Select(d => new DepartmentEmployeeCountV1(d.DepartmentId, d.DepartmentName, d.EmployeeCount))
                .ToList());
    }
}

public sealed record DashboardV1Response(
    int TotalEmployees,
    int ActiveEmployees,
    int TotalDepartments,
    decimal AverageSalary,
    IReadOnlyList<DepartmentEmployeeCountV1> EmployeesByDepartment);

public sealed record DepartmentEmployeeCountV1(int DepartmentId, string DepartmentName, int EmployeeCount);
