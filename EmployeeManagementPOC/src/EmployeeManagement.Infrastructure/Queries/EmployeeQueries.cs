using System.Data.Common;
using Dapper;
using EmployeeManagement.Application.Common.Paging;
using EmployeeManagement.Application.Features.Employees;

namespace EmployeeManagement.Infrastructure.Queries;

internal sealed class EmployeeQueries(SqlConnectionFactory connectionFactory) : IEmployeeQueries
{
    private const string SelectColumns = """
        SELECT
            e.Id,
            e.FirstName,
            e.LastName,
            e.FirstName + ' ' + e.LastName AS FullName,
            e.Email,
            e.Phone,
            e.Salary,
            e.HireDate,
            e.IsActive,
            e.DepartmentId,
            d.Name AS DepartmentName
        FROM Employees e
        INNER JOIN Departments d ON d.Id = e.DepartmentId
        """;

    public async Task<PagedResult<EmployeeResponse>> SearchAsync(
        EmployeeQuery query,
        CancellationToken cancellationToken = default)
    {
        (string where, DynamicParameters parameters) = BuildFilter(query);

        parameters.Add("Offset", (query.Page - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        string sql = $"""
            SELECT COUNT(*) FROM Employees e {where};

            {SelectColumns}
            {where}
            ORDER BY {BuildOrderBy(query)}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using DbConnection connection = await connectionFactory.OpenAsync(cancellationToken);

        await using SqlMapper.GridReader grid = await connection.QueryMultipleAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        int totalCount = await grid.ReadSingleAsync<int>();
        List<EmployeeResponse> items = (await grid.ReadAsync<EmployeeResponse>()).AsList();

        return new PagedResult<EmployeeResponse>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{SelectColumns} WHERE e.Id = @Id;";

        await using DbConnection connection = await connectionFactory.OpenAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<EmployeeResponse>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    private static (string Where, DynamicParameters Parameters) BuildFilter(EmployeeQuery query)
    {
        List<string> conditions = [];
        DynamicParameters parameters = new();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            // Full name is included so a picked autocomplete suggestion ("Jane Doe") finds the employee.
            conditions.Add(
                "(e.FirstName + ' ' + e.LastName LIKE @Search OR e.Email LIKE @Search)");
            parameters.Add("Search", $"%{EscapeLikePattern(query.Search.Trim())}%");
        }

        if (query.DepartmentId.HasValue)
        {
            conditions.Add("e.DepartmentId = @DepartmentId");
            parameters.Add("DepartmentId", query.DepartmentId.Value);
        }

        if (query.IsActive.HasValue)
        {
            conditions.Add("e.IsActive = @IsActive");
            parameters.Add("IsActive", query.IsActive.Value);
        }

        string where = conditions.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", conditions);

        return (where, parameters);
    }

    // Column names come from a closed enum, never from user input, so interpolation is safe here.
    private static string BuildOrderBy(EmployeeQuery query)
    {
        string direction = query.SortDescending ? "DESC" : "ASC";

        return query.SortBy switch
        {
            EmployeeSortField.Salary => $"e.Salary {direction}, e.Id",
            EmployeeSortField.HireDate => $"e.HireDate {direction}, e.Id",
            _ => $"e.FirstName {direction}, e.LastName {direction}, e.Id"
        };
    }

    private static string EscapeLikePattern(string value) =>
        value.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
}
