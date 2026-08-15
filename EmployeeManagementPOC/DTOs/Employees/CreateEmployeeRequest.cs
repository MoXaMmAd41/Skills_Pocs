using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementPOC.DTOs.Employees;

public class CreateEmployeeRequest
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(30)]
    public string? Phone { get; set; }

    [Range(0, 1_000_000)]
    public decimal Salary { get; set; }

    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }
}