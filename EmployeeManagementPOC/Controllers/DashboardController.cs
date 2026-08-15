using EmployeeManagementPOC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPOC.Controllers;

[Authorize(Policy = "DashboardAccess")]
public class DashboardController : Controller
{
    private readonly ReportService _reportService;

    public DashboardController(
        ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(
            await _reportService.GetDashboardAsync());
    }
}