namespace EmployeeManagement.Domain.Departments;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsNameTakenAsync(
        string name,
        int? excludingDepartmentId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken = default);

    void Add(Department department);

    void Remove(Department department);
}
