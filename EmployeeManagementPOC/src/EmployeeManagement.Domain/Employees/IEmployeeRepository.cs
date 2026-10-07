namespace EmployeeManagement.Domain.Employees;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsEmailTakenAsync(
        Email email,
        int? excludingEmployeeId = null,
        CancellationToken cancellationToken = default);

    void Add(Employee employee);

    void Remove(Employee employee);
}
