namespace Nutrio.Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; }

    // Для сутностей, де ми хочемо самі керувати ID (наприклад, Guid)
    protected Entity() { }

    // Конструктор для випадків, коли ID приходить ззовні (наприклад, int для Product)
    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id?.Equals(other.Id) ?? false;
    }

    public override int GetHashCode() => Id?.GetHashCode() ?? 0;
}