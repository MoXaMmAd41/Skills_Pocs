namespace EmployeeManagement.Application.Features.Departments;

public sealed record DepartmentResponse(int Id, string Name, string? Description, int EmployeesCount);

public sealed record DepartmentRequest(string Name, string? Description);
