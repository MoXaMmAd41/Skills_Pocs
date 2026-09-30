using Asp.Versioning;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.Versioning;
using EmployeeManagement.Application.Features.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers.V2;

/// <summary>
/// Adds inactive head-count and each department's relative size, so clients no longer compute them.
/// </summary>
[ApiVersion(ApiVersions.V2)]
[Authorize(Policy = Policies.DashboardAccess)]
public sealed class DashboardController(IDashboardService dashboardService) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<DashboardResponse>(StatusCodes.Status200OK)]
    public Task<DashboardResponse> Get(CancellationToken cancellationToken) =>
        dashboardService.GetAsync(cancellationToken);
}
