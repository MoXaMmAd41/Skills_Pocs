using EmployeeManagementPOC.DTOs.Employees;
using EmployeeManagementPOC.ViewModels.Employees;

namespace EmployeeManagementPOC.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeListViewModel> GetPagedAsync(
        EmployeeFilterViewModel filter);

    Task<EmployeeResponse?> GetByIdAsync(int id);

    Task<bool> CreateAsync(CreateEmployeeRequest request);

    Task<bool> UpdateAsync(UpdateEmployeeRequest request);

    Task<bool> DeleteAsync(int id);
}