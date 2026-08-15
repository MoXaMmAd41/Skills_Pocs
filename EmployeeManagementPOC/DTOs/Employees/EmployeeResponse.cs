namespace EmployeeManagementPOC.DTOs.Employees;

public class EmployeeResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public decimal Salary { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; }

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;
}