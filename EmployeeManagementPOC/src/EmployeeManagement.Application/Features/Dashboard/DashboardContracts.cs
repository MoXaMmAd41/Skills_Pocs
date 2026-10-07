namespace EmployeeManagement.Application.Features.Dashboard;

/// <summary>
/// Raw aggregates as read from the store.
/// </summary>
public sealed record DashboardSnapshot(
    int TotalEmployees,
    int ActiveEmployees,
    int TotalDepartments,
    decimal AverageSalary,
    IReadOnlyList<DepartmentHeadcount> Departments);

public sealed record DepartmentHeadcount(int DepartmentId, string DepartmentName, int EmployeeCount);

public sealed record DashboardResponse(
    int TotalEmployees,
    int ActiveEmployees,
    int InactiveEmployees,
    int TotalDepartments,
    decimal AverageSalary,
    IReadOnlyList<DepartmentDistribution> EmployeesByDepartment);

/// <param name="RelativeSize">Headcount as a percentage of the largest department (0-100).</param>
public sealed record DepartmentDistribution(
    int DepartmentId,
    string DepartmentName,
    int EmployeeCount,
    int RelativeSize);
