using System.Data.Common;
using Dapper;
using EmployeeManagement.Application.Features.Dashboard;

namespace EmployeeManagement.Infrastructure.Queries;

internal sealed class DashboardQueries(SqlConnectionFactory connectionFactory) : IDashboardQueries
{
    private const string Sql = """
        SELECT
            COUNT(*) AS TotalEmployees,
            COALESCE(SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END), 0) AS ActiveEmployees,
            (SELECT COUNT(*) FROM Departments) AS TotalDepartments,
            CAST(COALESCE(AVG(Salary), 0) AS decimal(18, 2)) AS AverageSalary
        FROM Employees;

        SELECT
            d.Id AS DepartmentId,
            d.Name AS DepartmentName,
            COUNT(e.Id) AS EmployeeCount
        FROM Departments d
        LEFT JOIN Employees e ON e.DepartmentId = d.Id
        GROUP BY d.Id, d.Name
        ORDER BY d.Name;
        """;

    public async Task<DashboardSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await connectionFactory.OpenAsync(cancellationToken);

        await using SqlMapper.GridReader grid = await connection.QueryMultipleAsync(
            new CommandDefinition(Sql, cancellationToken: cancellationToken));

        Totals totals = await grid.ReadSingleAsync<Totals>();
        List<DepartmentHeadcount> departments = (await grid.ReadAsync<DepartmentHeadcount>()).AsList();

        return new DashboardSnapshot(
            totals.TotalEmployees,
            totals.ActiveEmployees,
            totals.TotalDepartments,
            totals.AverageSalary,
            departments);
    }

    private sealed record Totals(int TotalEmployees, int ActiveEmployees, int TotalDepartments, decimal AverageSalary);
}
