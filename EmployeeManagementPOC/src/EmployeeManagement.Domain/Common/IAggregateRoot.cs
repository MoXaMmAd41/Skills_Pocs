namespace EmployeeManagement.Domain.Common;

/// <summary>
/// Marks an entity as the consistency boundary of an aggregate.
/// Only aggregate roots are loaded and persisted through repositories.
/// </summary>
public interface IAggregateRoot;
