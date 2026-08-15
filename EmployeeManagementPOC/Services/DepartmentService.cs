using EmployeeManagementPOC.DTOs.Departments;
using EmployeeManagementPOC.Interfaces;
using EmployeeManagementPOC.Models;
using EmployeeManagementPOC.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementPOC.Services;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    private const string CacheKey = "departments:all";

    public DepartmentService(
        ApplicationDbContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<List<DepartmentResponse>> GetAllAsync()
    {
        List<DepartmentResponse>? cached =
            await _cacheService.GetAsync<List<DepartmentResponse>>(CacheKey);

        if (cached is not null)
        {
            return cached;
        }

        List<DepartmentResponse> departments =
            await _context.Departments
                .AsNoTracking()
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EmployeesCount = d.Employees.Count
                })
                .OrderBy(d => d.Name)
                .ToListAsync();

        await _cacheService.SetAsync(
            CacheKey,
            departments,
            TimeSpan.FromMinutes(10));

        return departments;
    }

    public Task<Department?> GetByIdAsync(int id)
    {
        return _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task CreateAsync(
        string name,
        string? description)
    {
        Department department = new()
        {
            Name = name.Trim(),
            Description = description?.Trim()
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync(CacheKey);
    }

    public async Task UpdateAsync(
        int id,
        string name,
        string? description)
    {
        Department? department =
            await _context.Departments.FindAsync(id);

        if (department is null)
        {
            return;
        }

        department.Name = name.Trim();
        department.Description = description?.Trim();

        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync(CacheKey);
    }

    public async Task DeleteAsync(int id)
    {
        Department? department =
            await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

        if (department is null)
        {
            return;
        }

        if (department.Employees.Count > 0)
        {
            throw new InvalidOperationException(
                "Cannot delete a department that contains employees.");
        }

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync(CacheKey);
    }
}