using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Employees;

public sealed class Employee : AggregateRoot<int>
{
    public const int NameMaxLength = 50;
    public const int PhoneMaxLength = 30;

    // Required by EF Core for materialization.
    private Employee()
    {
    }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public Email Email { get; private set; } = null!;

    public string? Phone { get; private set; }

    public Salary Salary { get; private set; } = null!;

    public DateTime HireDate { get; private set; }

    public bool IsActive { get; private set; }

    public int DepartmentId { get; private set; }

    public static Employee Hire(
        string firstName,
        string lastName,
        Email email,
        string? phone,
        Salary salary,
        DateTime hireDate,
        int departmentId,
        bool isActive = true)
    {
        Employee employee = new()
        {
            Salary = salary,
            DepartmentId = EnsureValidDepartment(departmentId),
            IsActive = isActive
        };

        employee.UpdateProfile(firstName, lastName, email, phone, hireDate);
        employee.Raise(new EmployeeHired(employee));

        return employee;
    }

    /// <summary>Corrects personal and contact details. Not a business event, so nothing is raised.</summary>
    public void UpdateProfile(string firstName, string lastName, Email email, string? phone, DateTime hireDate)
    {
        FirstName = Guard.RequiredText(firstName, NameMaxLength, EmployeeErrorCodes.InvalidName, "First name");
        LastName = Guard.RequiredText(lastName, NameMaxLength, EmployeeErrorCodes.InvalidName, "Last name");
        Email = email;
        Phone = Guard.OptionalText(phone, PhoneMaxLength, EmployeeErrorCodes.InvalidPhone, "Phone");
        HireDate = hireDate.Date;
    }

    public void ChangeSalary(Salary newSalary)
    {
        if (newSalary == Salary)
        {
            return;
        }

        Salary previous = Salary;
        Salary = newSalary;

        Raise(new EmployeeSalaryChanged(Id, previous, newSalary));
    }

    public void TransferTo(int departmentId)
    {
        EnsureValidDepartment(departmentId);

        if (departmentId == DepartmentId)
        {
            return;
        }

        int previous = DepartmentId;
        DepartmentId = departmentId;

        Raise(new EmployeeTransferred(Id, previous, departmentId));
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        Raise(new EmployeeReactivated(Id));
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        Raise(new EmployeeDeactivated(Id));
    }

    private static int EnsureValidDepartment(int departmentId) =>
        departmentId <= 0
            ? throw new DomainException(EmployeeErrorCodes.InvalidDepartment, "A valid department is required.")
            : departmentId;
}
