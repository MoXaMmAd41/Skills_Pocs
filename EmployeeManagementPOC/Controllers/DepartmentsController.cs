using EmployeeManagementPOC.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPOC.Controllers;

[Authorize(Policy = "DepartmentView")]
public class DepartmentsController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(
        IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(
            await _departmentService.GetAllAsync());
    }

    [HttpGet]
    [Authorize(Policy = "DepartmentManage")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "DepartmentManage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string name,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "name",
                "Department name is required.");

            return View();
        }

        await _departmentService.CreateAsync(
            name,
            description);

        TempData["SuccessMessage"] =
            "Department created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "DepartmentManage")]
    public async Task<IActionResult> Edit(int id)
    {
        var department =
            await _departmentService.GetByIdAsync(id);

        if (department is null)
        {
            return NotFound();
        }

        return View(department);
    }

    [HttpPost]
    [Authorize(Policy = "DepartmentManage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        string name,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(
                "name",
                "Department name is required.");

            var department =
                await _departmentService.GetByIdAsync(id);

            return View(department);
        }

        await _departmentService.UpdateAsync(
            id,
            name,
            description);

        TempData["SuccessMessage"] =
            "Department updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = "DepartmentManage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _departmentService.DeleteAsync(id);

            TempData["SuccessMessage"] =
                "Department deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}