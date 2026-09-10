using EmployeeManagementPOC.DTOs.Employees;
using EmployeeManagementPOC.Interfaces;
using EmployeeManagementPOC.ViewModels.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPOC.Controllers;

[Authorize(Policy = "EmployeeView")]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(
        IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        EmployeeFilterViewModel filter)
    {
        EmployeeListViewModel model =
            await _employeeService.GetPagedAsync(filter);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        EmployeeFilterViewModel filter)
    {
        EmployeeListViewModel model =
            await _employeeService.GetPagedAsync(filter);

        return PartialView(
            "~/Views/Shared/_EmployeeTable.cshtml",
            model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        EmployeeResponse? employee =
            await _employeeService.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpGet]
    [Authorize(Policy = "EmployeeManage")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Departments =
            await GetDepartmentsAsync();

        return View(
            new CreateEmployeeRequest
            {
                HireDate = DateTime.Today
            });
    }

    [HttpPost]
    [Authorize(Policy = "EmployeeManage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateEmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments =
                await GetDepartmentsAsync();

            return View(request);
        }

        bool created =
            await _employeeService.CreateAsync(request);

        if (!created)
        {
            ModelState.AddModelError(
                nameof(request.Email),
                "An employee with this email already exists.");

            ViewBag.Departments =
                await GetDepartmentsAsync();

            return View(request);
        }

        TempData["SuccessMessage"] =
            "Employee created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "EmployeeManage")]
    public async Task<IActionResult> Edit(int id)
    {
        EmployeeResponse? employee =
            await _employeeService.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        ViewBag.Departments =
            await GetDepartmentsAsync();

        return View(
            new UpdateEmployeeRequest
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive,
                DepartmentId = employee.DepartmentId
            });
    }

    [HttpPost]
    [Authorize(Policy = "EmployeeManage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        UpdateEmployeeRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Departments =
                await GetDepartmentsAsync();

            return View(request);
        }

        bool updated =
            await _employeeService.UpdateAsync(request);

        if (!updated)
        {
            ModelState.AddModelError(
                string.Empty,
                "Employee could not be updated. Check the email.");

            ViewBag.Departments =
                await GetDepartmentsAsync();

            return View(request);
        }

        TempData["SuccessMessage"] =
            "Employee updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Policy = "EmployeeManage")]
    public async Task<IActionResult> Delete(int id)
    {
        EmployeeResponse? employee =
            await _employeeService.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost]
    [Authorize(Policy = "EmployeeManage")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted =
            await _employeeService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] =
            "Employee deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    private async Task<
        List<DTOs.Departments.DepartmentResponse>>
        GetDepartmentsAsync()
    {
        return await HttpContext.RequestServices
            .GetRequiredService<IDepartmentService>()
            .GetAllAsync();
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> MockTest()
    {
        using HttpClient client = new();

        HttpResponseMessage response =
            await client.GetAsync("http://localhost:3001/employees");

        string data =
            await response.Content.ReadAsStringAsync();

        return Content(data, "application/json");
    }
}