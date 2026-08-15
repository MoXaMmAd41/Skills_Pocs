using EmployeeManagementPOC.DTOs.Departments;
using EmployeeManagementPOC.DTOs.Employees;

namespace EmployeeManagementPOC.ViewModels.Employees;

public class EmployeeDetailsViewModel
{
    public EmployeeResponse Employee { get; set; } = new();

    public List<DepartmentResponse> Departments { get; set; } = [];
}