using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Entities;

public abstract class CrudEntity<TId> : Entity, ICrudEntity<TId>
    where TId : IEquatable<TId>
{
    public virtual TId Id { get; set; } = default!;

    public override bool Equals(Entity? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is not CrudEntity<TId> entity || other.GetType() != GetType())
        {
            return false;
        }

        if (EqualityComparer<TId>.Default.Equals(Id, default) || EqualityComparer<TId>.Default.Equals(entity.Id, default))
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
