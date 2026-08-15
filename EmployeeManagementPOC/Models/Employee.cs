using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementPOC.Models;

public class Employee
{
    public int Id { get; set; }

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
    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int DepartmentId { get; set; }

    public Department? Department { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}