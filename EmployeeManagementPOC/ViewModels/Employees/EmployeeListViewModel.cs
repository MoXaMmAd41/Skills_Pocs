using EmployeeManagementPOC.DTOs.Employees;
using EmployeeManagementPOC.DTOs.Departments;

namespace EmployeeManagementPOC.ViewModels.Employees;

public class EmployeeListViewModel
{
    public List<EmployeeResponse> Employees { get; set; } = [];

    public List<DepartmentResponse> Departments { get; set; } = [];

    public EmployeeFilterViewModel Filter { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages =>
        Filter.PageSize <= 0
            ? 1
            : (int)Math.Ceiling(
                TotalCount / (double)Filter.PageSize);

    public bool HasPreviousPage => Filter.Page > 1;

    public bool HasNextPage => Filter.Page < TotalPages;
}