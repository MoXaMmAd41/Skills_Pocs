using Asp.Versioning;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.Versioning;
using EmployeeManagement.Application.Common.Paging;
using EmployeeManagement.Application.Features.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers.V1;

[ApiVersion(ApiVersions.V1)]
[Authorize(Policy = Policies.EmployeeView)]
public sealed class EmployeesController(IEmployeeService employeeService) : ApiControllerBase
{
    /// <summary>Searches, filters, sorts and pages employees.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<EmployeeResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<PagedResult<EmployeeResponse>> Search(
        [FromQuery] EmployeeQuery query,
        CancellationToken cancellationToken) =>
        employeeService.SearchAsync(query, cancellationToken);

    /// <summary>Search-as-you-type: employees whose first name, last name, full name or email starts with the prefix.</summary>
    [HttpGet("suggestions")]
    [ProducesResponseType<IReadOnlyList<EmployeeSuggestion>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IReadOnlyList<EmployeeSuggestion>> Suggest(
        [FromQuery] EmployeeSuggestionQuery query,
        CancellationToken cancellationToken) =>
        employeeService.SuggestAsync(query, cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType<EmployeeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<EmployeeResponse> GetById(int id, CancellationToken cancellationToken) =>
        employeeService.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Policy = Policies.EmployeeManage)]
    [ProducesResponseType<EmployeeResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EmployeeResponse>> Create(
        EmployeeRequest request,
        ApiVersion version,
        CancellationToken cancellationToken)
    {
        EmployeeResponse employee = await employeeService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = employee.Id, version = version.ToString() },
            employee);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.EmployeeManage)]
    [ProducesResponseType<EmployeeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<EmployeeResponse> Update(int id, EmployeeRequest request, CancellationToken cancellationToken) =>
        employeeService.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.EmployeeManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await employeeService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
