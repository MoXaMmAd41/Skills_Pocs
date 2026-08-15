using EmployeeManagementPOC.Interfaces;
using EmployeeManagementPOC.ViewModels.Dashboard;

namespace EmployeeManagementPOC.Services;

public class ReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        List<DepartmentEmployeeCount> departments =
            await _reportRepository
                .GetEmployeesByDepartmentAsync();

        return new DashboardViewModel
        {
            TotalEmployees =
                await _reportRepository.GetTotalEmployeesAsync(),

            ActiveEmployees =
                await _reportRepository.GetActiveEmployeesAsync(),

            TotalDepartments =
                await _reportRepository.GetTotalDepartmentsAsync(),

            AverageSalary =
                await _reportRepository.GetAverageSalaryAsync(),

            EmployeesByDepartment = departments
                .Select(d => new DepartmentEmployeeCountViewModel
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    EmployeeCount = d.EmployeeCount
                })
                .ToList()
        };
    }
}