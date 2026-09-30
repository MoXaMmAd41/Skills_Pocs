namespace EmployeeManagement.Application.Features.Departments;

public interface IDepartmentQueries
{
    Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
