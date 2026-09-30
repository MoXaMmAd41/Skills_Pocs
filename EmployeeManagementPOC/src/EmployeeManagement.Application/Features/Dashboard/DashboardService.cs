namespace EmployeeManagement.Application.Features.Dashboard;

public interface IDashboardQueries
{
    Task<DashboardSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync(CancellationToken cancellationToken = default);
}

internal sealed class DashboardService(IDashboardQueries queries) : IDashboardService
{
    public async Task<DashboardResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        DashboardSnapshot snapshot = await queries.GetSnapshotAsync(cancellationToken);

        int largestDepartment = snapshot.Departments.Count == 0
            ? 0
            : snapshot.Departments.Max(d => d.EmployeeCount);

        return new DashboardResponse(
            snapshot.TotalEmployees,
            snapshot.ActiveEmployees,
            snapshot.TotalEmployees - snapshot.ActiveEmployees,
            snapshot.TotalDepartments,
            snapshot.AverageSalary,
            snapshot.Departments
                .Select(d => new DepartmentDistribution(
                    d.DepartmentId,
                    d.DepartmentName,
                    d.EmployeeCount,
                    RelativeSize(d.EmployeeCount, largestDepartment)))
                .ToList());
    }

    private static int RelativeSize(int count, int largest) =>
        largest == 0 ? 0 : (int)Math.Round(count / (double)largest * 100);
}
