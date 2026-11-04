using EmployeeManagement.Application.Common.Events;
using EmployeeManagement.Domain.Employees;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Features.Employees;

/// <summary>
/// Writes an audit trail of HR-relevant changes. Profile corrections are not business events and are not logged.
/// </summary>
internal sealed partial class EmployeeActivityLog(ILogger<EmployeeActivityLog> logger) :
    IDomainEventHandler<EmployeeHired>,
    IDomainEventHandler<EmployeeSalaryChanged>,
    IDomainEventHandler<EmployeeTransferred>,
    IDomainEventHandler<EmployeeDeactivated>,
    IDomainEventHandler<EmployeeReactivated>
{
    public Task HandleAsync(EmployeeHired domainEvent, CancellationToken cancellationToken = default)
    {
        LogHired(domainEvent.Employee.Id, domainEvent.Employee.DepartmentId, domainEvent.Employee.Salary.Amount);
        return Task.CompletedTask;
    }

    public Task HandleAsync(EmployeeSalaryChanged domainEvent, CancellationToken cancellationToken = default)
    {
        LogSalaryChanged(
            domainEvent.EmployeeId,
            domainEvent.IsRaise ? "raised" : "reduced",
            domainEvent.Previous.Amount,
            domainEvent.Current.Amount);
        return Task.CompletedTask;
    }

    public Task HandleAsync(EmployeeTransferred domainEvent, CancellationToken cancellationToken = default)
    {
        LogTransferred(domainEvent.EmployeeId, domainEvent.FromDepartmentId, domainEvent.ToDepartmentId);
        return Task.CompletedTask;
    }

    public Task HandleAsync(EmployeeDeactivated domainEvent, CancellationToken cancellationToken = default)
    {
        LogStatusChanged(domainEvent.EmployeeId, "deactivated");
        return Task.CompletedTask;
    }

    public Task HandleAsync(EmployeeReactivated domainEvent, CancellationToken cancellationToken = default)
    {
        LogStatusChanged(domainEvent.EmployeeId, "reactivated");
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Employee {EmployeeId} hired into department {DepartmentId} at {Salary}.")]
    private partial void LogHired(int employeeId, int departmentId, decimal salary);

    [LoggerMessage(LogLevel.Information, "Employee {EmployeeId} salary {Direction} from {Previous} to {Current}.")]
    private partial void LogSalaryChanged(int employeeId, string direction, decimal previous, decimal current);

    [LoggerMessage(LogLevel.Information, "Employee {EmployeeId} transferred from department {From} to {To}.")]
    private partial void LogTransferred(int employeeId, int from, int to);

    [LoggerMessage(LogLevel.Information, "Employee {EmployeeId} {Status}.")]
    private partial void LogStatusChanged(int employeeId, string status);
}
