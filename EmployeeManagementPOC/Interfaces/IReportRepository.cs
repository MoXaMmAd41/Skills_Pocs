namespace EmployeeManagementPOC.Interfaces;

public interface IReportRepository
{
    Task<int> GetTotalEmployeesAsync();

    Task<int> GetActiveEmployeesAsync();

    Task<int> GetTotalDepartmentsAsync();

    Task<decimal> GetAverageSalaryAsync();

    Task<List<DepartmentEmployeeCount>> GetEmployeesByDepartmentAsync();
}

public sealed record DepartmentEmployeeCount(
    int DepartmentId,
    string DepartmentName,
    int EmployeeCount);