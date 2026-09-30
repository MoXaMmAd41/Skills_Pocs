namespace EmployeeManagement.Application.Features.Employees;

public sealed record EmployeeResponse(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Phone,
    decimal Salary,
    DateTime HireDate,
    bool IsActive,
    int DepartmentId,
    string DepartmentName);

/// <summary>
/// Payload used both to hire a new employee and to update an existing one.
/// </summary>
public sealed record EmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    decimal Salary,
    DateTime HireDate,
    bool IsActive,
    int DepartmentId);

public enum EmployeeSortField
{
    Name,
    Salary,
    HireDate
}

public sealed record EmployeeQuery
{
    public const int MaxPageSize = 100;

    public string? Search { get; init; }

    public int? DepartmentId { get; init; }

    public bool? IsActive { get; init; }

    public EmployeeSortField SortBy { get; init; } = EmployeeSortField.Name;

    public bool SortDescending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
