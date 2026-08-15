using Dapper;
using EmployeeManagementPOC.Data;
using EmployeeManagementPOC.Interfaces;

namespace EmployeeManagementPOC.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly DapperDbContext _dapperDbContext;

    public ReportRepository(DapperDbContext dapperDbContext)
    {
        _dapperDbContext = dapperDbContext;
    }

    public async Task<int> GetTotalEmployeesAsync()
    {
        using var connection = _dapperDbContext.CreateConnection();

        const string sql = """
            SELECT COUNT(*)
            FROM Employees;
            """;

        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<int> GetActiveEmployeesAsync()
    {
        using var connection = _dapperDbContext.CreateConnection();

        const string sql = """
            SELECT COUNT(*)
            FROM Employees
            WHERE IsActive = 1;
            """;

        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<int> GetTotalDepartmentsAsync()
    {
        using var connection = _dapperDbContext.CreateConnection();

        const string sql = """
            SELECT COUNT(*)
            FROM Departments;
            """;

        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<decimal> GetAverageSalaryAsync()
    {
        using var connection = _dapperDbContext.CreateConnection();

        const string sql = """
            SELECT ISNULL(AVG(Salary), 0)
            FROM Employees;
            """;

        return await connection.ExecuteScalarAsync<decimal>(sql);
    }

    public async Task<List<DepartmentEmployeeCount>>
        GetEmployeesByDepartmentAsync()
    {
        using var connection = _dapperDbContext.CreateConnection();

        const string sql = """
            SELECT
                d.Id AS DepartmentId,
                d.Name AS DepartmentName,
                COUNT(e.Id) AS EmployeeCount
            FROM Departments d
            LEFT JOIN Employees e
                ON e.DepartmentId = d.Id
            GROUP BY d.Id, d.Name
            ORDER BY d.Name;
            """;

        IEnumerable<DepartmentEmployeeCount> result =
            await connection.QueryAsync<DepartmentEmployeeCount>(sql);

        return result.ToList();
    }
}