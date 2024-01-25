using CatNip.Domain.Exceptions;
using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Exceptions;

public class EntityNotFoundException<TEntity, TKey> : NotFoundException
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public EntityNotFoundException(TKey id)
        : base($"Entity {typeof(TEntity).Name} {{ id: {id}}} not found.")
    {
    }

    public EntityNotFoundException(TKey id, string message)
        : base($"Entity {typeof(TEntity).Name} {{ id: {id}}} not found. {message}")
    {
    }

    public EntityNotFoundException(TKey id, string message, Exception innerException)
        : base($"Entity {typeof(TEntity).Name} {{ id: {id}}} not found. {message}", innerException)
    {
    }
}
