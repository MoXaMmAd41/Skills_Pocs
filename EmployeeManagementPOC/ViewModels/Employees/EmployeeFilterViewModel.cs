namespace EmployeeManagementPOC.ViewModels.Employees;

public class EmployeeFilterViewModel
{
    public string? Search { get; set; }

    public int? DepartmentId { get; set; }

    public bool? IsActive { get; set; }

    public string SortBy { get; set; } = "Name";

    public bool SortDescending { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 5;
}