using EmployeeManagementPOC.DTOs.Departments;
using EmployeeManagementPOC.Models;

namespace EmployeeManagementPOC.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentResponse>> GetAllAsync();

    Task<Department?> GetByIdAsync(int id);

    Task CreateAsync(string name, string? description);

    Task UpdateAsync(int id, string name, string? description);

    Task DeleteAsync(int id);
}