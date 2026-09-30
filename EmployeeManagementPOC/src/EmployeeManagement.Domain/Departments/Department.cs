using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Departments;

public sealed class Department : Entity<int>, IAggregateRoot
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    // Required by EF Core for materialization.
    private Department()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public static Department Create(string name, string? description)
    {
        Department department = new();

        department.Update(name, description);

        return department;
    }

    public void Update(string name, string? description)
    {
        Name = Guard.RequiredText(name, NameMaxLength, DepartmentErrorCodes.InvalidName, "Department name");
        Description = Guard.OptionalText(
            description,
            DescriptionMaxLength,
            DepartmentErrorCodes.InvalidDescription,
            "Description");
    }
}
