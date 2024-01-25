namespace CatNip.Infrastructure.Data.Entities.Interfaces;

public interface IEntity<TId> : IEntity
    where TId : IEquatable<TId>
{
    TId Id { get; set; }
}

public interface IEntity
{
}
