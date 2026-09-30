using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Employees;

public sealed class Employee : Entity<int>, IAggregateRoot
{
    public const int NameMaxLength = 50;
    public const int PhoneMaxLength = 30;
    public const decimal MaxSalary = 1_000_000m;

    // Required by EF Core for materialization.
    private Employee()
    {
    }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public Email Email { get; private set; } = null!;

    public string? Phone { get; private set; }

    public decimal Salary { get; private set; }

    public DateTime HireDate { get; private set; }

    public bool IsActive { get; private set; }

    public int DepartmentId { get; private set; }

    public static Employee Hire(
        string firstName,
        string lastName,
        Email email,
        string? phone,
        decimal salary,
        DateTime hireDate,
        int departmentId,
        bool isActive = true)
    {
        Employee employee = new() { IsActive = isActive };

        employee.UpdateDetails(firstName, lastName, email, phone, salary, hireDate, departmentId);

        return employee;
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        Email email,
        string? phone,
        decimal salary,
        DateTime hireDate,
        int departmentId)
    {
        FirstName = Guard.RequiredText(firstName, NameMaxLength, EmployeeErrorCodes.InvalidName, "First name");
        LastName = Guard.RequiredText(lastName, NameMaxLength, EmployeeErrorCodes.InvalidName, "Last name");
        Email = email;
        Phone = Guard.OptionalText(phone, PhoneMaxLength, EmployeeErrorCodes.InvalidPhone, "Phone");
        Salary = EnsureValidSalary(salary);
        HireDate = hireDate.Date;
        DepartmentId = EnsureValidDepartment(departmentId);
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static decimal EnsureValidSalary(decimal salary) =>
        salary is < 0 or > MaxSalary
            ? throw new DomainException(
                EmployeeErrorCodes.InvalidSalary,
                $"Salary must be between 0 and {MaxSalary:N0}.")
            : salary;

    private static int EnsureValidDepartment(int departmentId) =>
        departmentId <= 0
            ? throw new DomainException(EmployeeErrorCodes.InvalidDepartment, "A valid department is required.")
            : departmentId;
}
