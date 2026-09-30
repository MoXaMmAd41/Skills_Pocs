using System.Data.Common;
using Dapper;
using EmployeeManagement.Application.Features.Departments;

namespace EmployeeManagement.Infrastructure.Queries;

internal sealed class DepartmentQueries(SqlConnectionFactory connectionFactory) : IDepartmentQueries
{
    private const string SelectColumns = """
        SELECT
            d.Id,
            d.Name,
            d.Description,
            (SELECT COUNT(*) FROM Employees e WHERE e.DepartmentId = d.Id) AS EmployeesCount
        FROM Departments d
        """;

    public async Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = $"{SelectColumns} ORDER BY d.Name;";

        await using DbConnection connection = await connectionFactory.OpenAsync(cancellationToken);

        IEnumerable<DepartmentResponse> departments = await connection.QueryAsync<DepartmentResponse>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return departments.AsList();
    }

    public async Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{SelectColumns} WHERE d.Id = @Id;";

        await using DbConnection connection = await connectionFactory.OpenAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<DepartmentResponse>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }
}
