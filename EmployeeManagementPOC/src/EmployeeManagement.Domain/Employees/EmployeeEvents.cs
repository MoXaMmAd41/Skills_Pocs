using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Domain.Employees;

/// <summary>
/// Carries the aggregate rather than its id: the id is assigned by the database,
/// and events are dispatched only after the save that assigns it.
/// </summary>
public sealed record EmployeeHired(Employee Employee) : IDomainEvent;

public sealed record EmployeeSalaryChanged(int EmployeeId, Salary Previous, Salary Current) : IDomainEvent
{
    public bool IsRaise => Current.CompareTo(Previous) > 0;
}

public sealed record EmployeeTransferred(int EmployeeId, int FromDepartmentId, int ToDepartmentId) : IDomainEvent;

public sealed record EmployeeDeactivated(int EmployeeId) : IDomainEvent;

public sealed record EmployeeReactivated(int EmployeeId) : IDomainEvent;
