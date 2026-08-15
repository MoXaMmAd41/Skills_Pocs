namespace EmployeeManagementPOC.DTOs.Departments;

public class DepartmentResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int EmployeesCount { get; set; }
}