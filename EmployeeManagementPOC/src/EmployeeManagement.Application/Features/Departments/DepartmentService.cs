using EmployeeManagement.Application.Common.Caching;
using EmployeeManagement.Application.Common.Exceptions;
using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Departments;

namespace EmployeeManagement.Application.Features.Departments;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DepartmentResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<DepartmentResponse> CreateAsync(DepartmentRequest request, CancellationToken cancellationToken = default);

    Task<DepartmentResponse> UpdateAsync(
        int id,
        DepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

internal sealed class DepartmentService(
    IDepartmentRepository departments,
    IDepartmentQueries queries,
    IUnitOfWork unitOfWork,
    ICacheService cache) : IDepartmentService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        cache.GetOrCreateAsync(CacheKeys.Departments, queries.GetAllAsync, CacheDuration, cancellationToken);

    public async Task<DepartmentResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await queries.GetByIdAsync(id, cancellationToken) ?? throw DepartmentNotFound(id);

    public async Task<DepartmentResponse> CreateAsync(
        DepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureNameIsAvailableAsync(request.Name, excludingDepartmentId: null, cancellationToken);

        Department department = Department.Create(request.Name, request.Description);

        departments.Add(department);
        await CommitAsync(cancellationToken);

        return await GetByIdAsync(department.Id, cancellationToken);
    }

    public async Task<DepartmentResponse> UpdateAsync(
        int id,
        DepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        Department department = await departments.GetByIdAsync(id, cancellationToken) ?? throw DepartmentNotFound(id);

        await EnsureNameIsAvailableAsync(request.Name, excludingDepartmentId: id, cancellationToken);

        department.Update(request.Name, request.Description);
        await CommitAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Department department = await departments.GetByIdAsync(id, cancellationToken) ?? throw DepartmentNotFound(id);

        if (await departments.HasEmployeesAsync(id, cancellationToken))
        {
            throw new ConflictException(
                DepartmentErrorCodes.HasEmployees,
                "Cannot delete a department that contains employees.");
        }

        departments.Remove(department);
        await CommitAsync(cancellationToken);
    }

    private async Task EnsureNameIsAvailableAsync(
        string name,
        int? excludingDepartmentId,
        CancellationToken cancellationToken)
    {
        if (await departments.IsNameTakenAsync(name.Trim(), excludingDepartmentId, cancellationToken))
        {
            throw new ConflictException(
                DepartmentErrorCodes.DuplicateName,
                $"A department named '{name.Trim()}' already exists.");
        }
    }

    private async Task CommitAsync(CancellationToken cancellationToken)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(CacheKeys.Departments, cancellationToken);
    }

    private static NotFoundException DepartmentNotFound(int id) =>
        new(DepartmentErrorCodes.NotFound, $"Department with id {id} was not found.");
}
