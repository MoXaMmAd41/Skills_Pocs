using EmployeeManagement.Application.Common.Caching;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Application.Common.Paging;
using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Departments;
using EmployeeManagement.Domain.Employees;

namespace EmployeeManagement.Application.Features.Employees;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponse>> SearchAsync(EmployeeQuery query, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmployeeSuggestion>> SuggestAsync(
        EmployeeSuggestionQuery query,
        CancellationToken cancellationToken = default);

    Task<EmployeeResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<EmployeeResponse> CreateAsync(EmployeeRequest request, CancellationToken cancellationToken = default);

    Task<EmployeeResponse> UpdateAsync(int id, EmployeeRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

internal sealed class EmployeeService(
    IEmployeeRepository employees,
    IDepartmentRepository departments,
    IEmployeeQueries queries,
    IUnitOfWork unitOfWork,
    ICacheService cache,
    IEmployeeSearchIndex searchIndex) : IEmployeeService
{
    public Task<PagedResult<EmployeeResponse>> SearchAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken = default) =>
        queries.SearchAsync(query, cancellationToken);

    public Task<IReadOnlyList<EmployeeSuggestion>> SuggestAsync(
        EmployeeSuggestionQuery query,
        CancellationToken cancellationToken = default) =>
        searchIndex.SuggestAsync(query.Prefix, query.Limit, cancellationToken);

    public async Task<EmployeeResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await queries.GetByIdAsync(id, cancellationToken) ?? throw EmployeeNotFound(id);

    public async Task<EmployeeResponse> CreateAsync(
        EmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        Email email = Email.Create(request.Email);

        await EnsureEmailIsAvailableAsync(email, excludingEmployeeId: null, cancellationToken);
        await EnsureDepartmentExistsAsync(request.DepartmentId, cancellationToken);

        Employee employee = Employee.Hire(
            request.FirstName,
            request.LastName,
            email,
            request.Phone,
            request.Salary,
            request.HireDate,
            request.DepartmentId,
            request.IsActive);

        employees.Add(employee);
        await CommitAsync(cancellationToken);

        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task<EmployeeResponse> UpdateAsync(
        int id,
        EmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        Employee employee = await employees.GetByIdAsync(id, cancellationToken) ?? throw EmployeeNotFound(id);
        Email email = Email.Create(request.Email);

        await EnsureEmailIsAvailableAsync(email, excludingEmployeeId: id, cancellationToken);

        if (employee.DepartmentId != request.DepartmentId)
        {
            await EnsureDepartmentExistsAsync(request.DepartmentId, cancellationToken);
        }

        employee.UpdateDetails(
            request.FirstName,
            request.LastName,
            email,
            request.Phone,
            request.Salary,
            request.HireDate,
            request.DepartmentId);

        if (request.IsActive)
        {
            employee.Activate();
        }
        else
        {
            employee.Deactivate();
        }

        await CommitAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Employee employee = await employees.GetByIdAsync(id, cancellationToken) ?? throw EmployeeNotFound(id);

        employees.Remove(employee);
        await CommitAsync(cancellationToken);
    }

    private async Task EnsureEmailIsAvailableAsync(
        Email email,
        int? excludingEmployeeId,
        CancellationToken cancellationToken)
    {
        if (await employees.IsEmailTakenAsync(email, excludingEmployeeId, cancellationToken))
        {
            throw new ConflictException(
                EmployeeErrorCodes.DuplicateEmail,
                $"An employee with the email '{email}' already exists.");
        }
    }

    private async Task EnsureDepartmentExistsAsync(int departmentId, CancellationToken cancellationToken)
    {
        if (!await departments.ExistsAsync(departmentId, cancellationToken))
        {
            throw new NotFoundException(
                DepartmentErrorCodes.NotFound,
                $"Department with id {departmentId} was not found.");
        }
    }

    // Department head-counts are cached and names are indexed for autocomplete,
    // so any employee write invalidates both.
    private async Task CommitAsync(CancellationToken cancellationToken)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKeys.Departments, cancellationToken);
        searchIndex.Invalidate();
    }

    private static NotFoundException EmployeeNotFound(int id) =>
        new(EmployeeErrorCodes.NotFound, $"Employee with id {id} was not found.");
}
