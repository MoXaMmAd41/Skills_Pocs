using System.Reflection;
using EmployeeManagement.Application.Common.Events;
using EmployeeManagement.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure.Persistence;

/// <summary>
/// Commits tracked changes, then publishes the domain events the saved aggregates raised.
/// Events are dispatched after the commit so handlers never react to changes that were rolled back.
/// </summary>
internal sealed class UnitOfWork(ApplicationDbContext context, IServiceProvider services) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<IDomainEvent> events = context.ChangeTracker
            .Entries<IAggregateRoot>()
            .SelectMany(entry => entry.Entity.DequeueDomainEvents())
            .ToList();

        int written = await context.SaveChangesAsync(cancellationToken);

        foreach (IDomainEvent domainEvent in events)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }

        return written;
    }

    private async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        MethodInfo handle = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

        foreach (object? handler in services.GetServices(handlerType))
        {
            await (Task)handle.Invoke(handler, [domainEvent, cancellationToken])!;
        }
    }
}
