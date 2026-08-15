using EmployeeManagementPOC.Data;
using EmployeeManagementPOC.Interfaces;
using EmployeeManagementPOC.Models;
using EmployeeManagementPOC.ViewModels.Employees;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementPOC.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(
        EmployeeFilterViewModel filter)
    {
        IQueryable<Employee> query =
            _context.Employees
                .AsNoTracking()
                .Include(e => e.Department);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim();

            query = query.Where(e =>
                e.FirstName.Contains(search) ||
                e.LastName.Contains(search) ||
                e.Email.Contains(search));
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(e =>
                e.DepartmentId == filter.DepartmentId.Value);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(e =>
                e.IsActive == filter.IsActive.Value);
        }

        query = filter.SortBy.ToLowerInvariant() switch
        {
            "salary" => filter.SortDescending
                ? query.OrderByDescending(e => e.Salary)
                : query.OrderBy(e => e.Salary),

            "hiredate" => filter.SortDescending
                ? query.OrderByDescending(e => e.HireDate)
                : query.OrderBy(e => e.HireDate),

            _ => filter.SortDescending
                ? query.OrderByDescending(e => e.FirstName)
                    .ThenByDescending(e => e.LastName)
                : query.OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
        };

        int totalCount = await query.CountAsync();

        int page = Math.Max(filter.Page, 1);
        int pageSize = Math.Clamp(filter.PageSize, 1, 100);

        List<Employee> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<Employee?> GetByIdAsync(int id)
    {
        return _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<Employee?> GetByEmailAsync(string email)
    {
        return _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}