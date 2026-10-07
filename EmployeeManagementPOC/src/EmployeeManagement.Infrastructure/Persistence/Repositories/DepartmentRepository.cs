using EmployeeManagement.Domain.Departments;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Repositories;

internal sealed class DepartmentRepository(ApplicationDbContext context) : IDepartmentRepository
{
    public Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.Departments.AnyAsync(d => d.Id == id, cancellationToken);

    public Task<bool> IsNameTakenAsync(
        string name,
        int? excludingDepartmentId = null,
        CancellationToken cancellationToken = default) =>
        context.Departments.AnyAsync(
            d => d.Name == name && d.Id != excludingDepartmentId,
            cancellationToken);

    public Task<bool> HasEmployeesAsync(int id, CancellationToken cancellationToken = default) =>
        context.Employees.AnyAsync(e => e.DepartmentId == id, cancellationToken);

    public void Add(Department department) => context.Departments.Add(department);

    public void Remove(Department department) => context.Departments.Remove(department);
}
