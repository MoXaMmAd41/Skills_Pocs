using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace EmployeeManagement.Infrastructure.Queries;

internal sealed class SqlConnectionFactory(string connectionString)
{
    public async Task<DbConnection> OpenAsync(CancellationToken cancellationToken)
    {
        SqlConnection connection = new(connectionString);

        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
