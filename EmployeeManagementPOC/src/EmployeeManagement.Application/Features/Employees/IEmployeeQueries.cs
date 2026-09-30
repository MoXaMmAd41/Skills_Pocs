using EmployeeManagement.Application.Common.Paging;

namespace EmployeeManagement.Application.Features.Employees;

/// <summary>
/// Read-side port: returns projections shaped for clients, bypassing the aggregate.
/// </summary>
public interface IEmployeeQueries
{
    Task<PagedResult<EmployeeResponse>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken = default);

    Task<EmployeeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
