namespace EmployeeManagementPOC.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalEmployees { get; set; }

    public int ActiveEmployees { get; set; }

    public int InactiveEmployees =>
        TotalEmployees - ActiveEmployees;

    public int TotalDepartments { get; set; }

    public decimal AverageSalary { get; set; }

    public List<DepartmentEmployeeCountViewModel> EmployeesByDepartment { get; set; } = [];
}

public class DepartmentEmployeeCountViewModel
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public int EmployeeCount { get; set; }
}