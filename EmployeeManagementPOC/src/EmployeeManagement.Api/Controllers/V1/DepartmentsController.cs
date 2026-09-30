using Asp.Versioning;
using EmployeeManagement.Api.Authorization;
using EmployeeManagement.Api.Versioning;
using EmployeeManagement.Application.Features.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers.V1;

[ApiVersion(ApiVersions.V1)]
public sealed class DepartmentsController(IDepartmentService departmentService) : ApiControllerBase
{
    /// <summary>
    /// Lists all departments. Readable by every authenticated user because the
    /// employee filters and forms need the department lookup.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Policies.EmployeeView)]
    [ProducesResponseType<IReadOnlyList<DepartmentResponse>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<DepartmentResponse>> GetAll(CancellationToken cancellationToken) =>
        departmentService.GetAllAsync(cancellationToken);

    [HttpGet("{id:int}")]
    [Authorize(Policy = Policies.DepartmentView)]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<DepartmentResponse> GetById(int id, CancellationToken cancellationToken) =>
        departmentService.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Policy = Policies.DepartmentManage)]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DepartmentResponse>> Create(
        DepartmentRequest request,
        ApiVersion version,
        CancellationToken cancellationToken)
    {
        DepartmentResponse department = await departmentService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = department.Id, version = version.ToString() },
            department);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.DepartmentManage)]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<DepartmentResponse> Update(int id, DepartmentRequest request, CancellationToken cancellationToken) =>
        departmentService.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.DepartmentManage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await departmentService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
