using EmployeeManagementPOC.DTOs.Employees;
using EmployeeManagementPOC.Interfaces;
using EmployeeManagementPOC.Models;
using EmployeeManagementPOC.ViewModels.Employees;

namespace EmployeeManagementPOC.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentService _departmentService;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentService departmentService)
    {
        _employeeRepository = employeeRepository;
        _departmentService = departmentService;
    }

    public async Task<EmployeeListViewModel> GetPagedAsync(
        EmployeeFilterViewModel filter)
    {
        filter.Page = Math.Max(filter.Page, 1);
        filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);

        var result =
            await _employeeRepository.GetPagedAsync(filter);

        List<EmployeeResponse> employees = result.Items
            .Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? string.Empty
            })
            .ToList();

        return new EmployeeListViewModel
        {
            Employees = employees,
            Departments = await _departmentService.GetAllAsync(),
            Filter = filter,
            TotalCount = result.TotalCount
        };
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        Employee? employee =
            await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return null;
        }

        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Salary = employee.Salary,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? string.Empty
        };
    }

    public async Task<bool> CreateAsync(
        CreateEmployeeRequest request)
    {
        Employee? existing =
            await _employeeRepository.GetByEmailAsync(request.Email);

        if (existing is not null)
        {
            return false;
        }

        Employee employee = new()
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone?.Trim(),
            Salary = request.Salary,
            HireDate = request.HireDate,
            IsActive = request.IsActive,
            DepartmentId = request.DepartmentId
        };

        await _employeeRepository.AddAsync(employee);

        return true;
    }

    public async Task<bool> UpdateAsync(
        UpdateEmployeeRequest request)
    {
        Employee? employee =
            await _employeeRepository.GetByIdAsync(request.Id);

        if (employee is null)
        {
            return false;
        }

        Employee? existing =
            await _employeeRepository.GetByEmailAsync(request.Email);

        if (existing is not null && existing.Id != request.Id)
        {
            return false;
        }

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim();
        employee.Phone = request.Phone?.Trim();
        employee.Salary = request.Salary;
        employee.HireDate = request.HireDate;
        employee.IsActive = request.IsActive;
        employee.DepartmentId = request.DepartmentId;

        await _employeeRepository.UpdateAsync(employee);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Employee? employee =
            await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return false;
        }

        await _employeeRepository.DeleteAsync(employee);

        return true;
    }
}