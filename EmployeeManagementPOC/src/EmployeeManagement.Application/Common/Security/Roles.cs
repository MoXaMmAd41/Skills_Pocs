namespace EmployeeManagement.Application.Common.Security;

public static class Roles
{
    public const string Admin = "Admin";
    public const string HR = "HR";
    public const string Manager = "Manager";
    public const string Employee = "Employee";

    public static readonly IReadOnlyList<string> All = [Admin, HR, Manager, Employee];
}
