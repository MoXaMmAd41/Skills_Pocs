using EmployeeManagement.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence.Repositories;

internal sealed class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<bool> IsEmailTakenAsync(
        Email email,
        int? excludingEmployeeId = null,
        CancellationToken cancellationToken = default) =>
        context.Employees.AnyAsync(
            e => e.Email == email && e.Id != excludingEmployeeId,
            cancellationToken);

    public void Add(Employee employee) => context.Employees.Add(employee);

    public void Remove(Employee employee) => context.Employees.Remove(employee);
}
