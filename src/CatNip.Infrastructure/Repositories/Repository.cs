using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Exceptions;

namespace CatNip.Infrastructure.Repositories;

public abstract class Repository<TDbContext, TEntity, TId> : Repository<TDbContext, TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    protected Repository(TDbContext dbContext)
        : base(dbContext)
    {
    }

    protected virtual IQueryable<TEntity> GetQueriable()
    {
        return DbContext.Set<TEntity>();
    }

    private protected virtual async Task<TEntity> FindAsync(TId id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id).ConfigureAwait(false);
        _ = entity ?? throw new EntityNotFoundException<TEntity, TId>(id);

        return entity;
    }
}

public abstract class Repository<TDbContext, TEntity> : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IEntity
{
    protected Repository(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected virtual TDbContext DbContext { get; init; }

    public virtual void Create(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public virtual async Task CommitAsync(CancellationToken cancellation = default)
    {
        _ = await DbContext.SaveChangesAsync(cancellation).ConfigureAwait(false);
    }

    private protected virtual async Task CreateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Create(entity);
        await CommitAsync(cancellation).ConfigureAwait(false);
    }

    private protected virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Update(entity);
        await CommitAsync(cancellation).ConfigureAwait(false);
    }

    private protected virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Delete(entity);
        await CommitAsync(cancellation).ConfigureAwait(false);
    }
}
