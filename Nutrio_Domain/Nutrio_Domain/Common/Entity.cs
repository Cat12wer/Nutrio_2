namespace Nutrio.Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;

    // Для сутностей, де ми хочемо самі керувати ID (наприклад, Guid)
    protected Entity() { }
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id?.Equals(other.Id) ?? false;
    }

    public override int GetHashCode() => Id?.GetHashCode() ?? 0;
}