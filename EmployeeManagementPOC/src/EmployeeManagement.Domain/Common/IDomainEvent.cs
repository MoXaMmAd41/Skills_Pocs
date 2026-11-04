namespace EmployeeManagement.Domain.Common;

/// <summary>
/// A fact that already happened inside an aggregate. Raised by the aggregate,
/// dispatched after the unit of work commits so handlers only see persisted state.
/// </summary>
public interface IDomainEvent;
