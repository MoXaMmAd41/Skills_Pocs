using EmployeeManagementPOC.Models;
using EmployeeManagementPOC.ViewModels.Employees;

namespace EmployeeManagementPOC.Interfaces;

public interface IEmployeeRepository
{
    Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(
        EmployeeFilterViewModel filter);

    Task<Employee?> GetByIdAsync(int id);

    Task<Employee?> GetByEmailAsync(string email);

    Task AddAsync(Employee employee);

    Task UpdateAsync(Employee employee);

    Task DeleteAsync(Employee employee);
}