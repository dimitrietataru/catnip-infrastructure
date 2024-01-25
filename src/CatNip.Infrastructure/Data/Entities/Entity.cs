using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Entities;

public abstract class Entity<TId> : Entity, IEntity<TId>
    where TId : IEquatable<TId>
{
    public virtual TId Id { get; set; } = default!;

    public override bool Equals(Entity? other)
    {
        if (other is not Entity<TId> entity)
        {
            return false;
        }

        if (Id.Equals(default) || entity.Id.Equals(default))
        {
            return false;
        }

        return Id.Equals(entity.Id);
    }

    protected override int DetermineHashCode()
    {
        return HashCode.Combine(Id);
    }
}

public abstract class Entity : IEquatable<Entity>
{
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
        {
            return false;
        }

        return Equals(other);
    }

    public override int GetHashCode()
    {
        return DetermineHashCode();
    }

    public abstract bool Equals(Entity? other);

    protected abstract int DetermineHashCode();
}
