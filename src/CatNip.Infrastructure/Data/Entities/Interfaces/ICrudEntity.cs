namespace CatNip.Infrastructure.Data.Entities.Interfaces;

public interface ICrudEntity<TId> : IEntity
    where TId : IEquatable<TId>
{
    TId Id { get; set; }
}
