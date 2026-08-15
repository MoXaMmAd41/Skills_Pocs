using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagementPOC.Data;

public class DapperDbContext
{
    private readonly string _connectionString;

    public DapperDbContext(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is not configured.");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}