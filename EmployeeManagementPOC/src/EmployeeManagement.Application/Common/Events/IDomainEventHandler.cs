using EmployeeManagement.Domain.Common;

namespace EmployeeManagement.Application.Common.Events;

/// <summary>
/// Reacts to a domain event after the unit of work that raised it has committed.
/// Implementations are discovered and registered automatically.
/// </summary>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
