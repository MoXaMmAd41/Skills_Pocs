namespace EmployeeManagement.Domain.Common;

/// <summary>
/// Marks an entity as the consistency boundary of an aggregate.
/// Only aggregate roots are loaded and persisted through repositories, and only they raise domain events.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>Returns the events raised since the last call and clears them.</summary>
    IReadOnlyList<IDomainEvent> DequeueDomainEvents();
}
