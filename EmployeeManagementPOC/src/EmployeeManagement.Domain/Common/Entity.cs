namespace EmployeeManagement.Domain.Common;

public abstract class Entity<TId>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other || other.GetType() != GetType())
        {
            return false;
        }

        if (IsTransient() || other.IsTransient())
        {
            return ReferenceEquals(this, other);
        }

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() =>
        IsTransient() ? base.GetHashCode() : HashCode.Combine(GetType(), Id);

    private bool IsTransient() =>
        EqualityComparer<TId>.Default.Equals(Id, default!);
}
