using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Exceptions;

namespace CatNip.Infrastructure.Repositories;

public abstract class UnitOfWork<TDbContext, TEntity, TModel, TId> : UnitOfWork<TDbContext, TEntity, TModel>
    where TDbContext : DbContext
    where TModel : IModel<TId>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    protected UnitOfWork(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    protected virtual IQueryable<TEntity> GetQueriable()
    {
        return DbContext.Set<TEntity>();
    }

    private protected virtual async Task<TEntity> FindAsync(TId id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        _ = entity ?? throw new EntityNotFoundException<TEntity, TId>(id);

        return entity;
    }
}

public abstract class UnitOfWork<TDbContext, TEntity, TModel> : UnitOfWork<TDbContext, TEntity>, IUnitOfWork<TModel>
    where TDbContext : DbContext
    where TModel : IModel
    where TEntity : class
{
    protected UnitOfWork(TDbContext dbContext, IMapper mapper)
        : base(dbContext)
    {
        Mapper = mapper;
    }

    protected virtual IMapper Mapper { get; init; }

    public virtual void Create(TModel model)
    {
        var entity = Mapper.Map<TEntity>(model);
        Create(entity);

        Mapper.Map(entity, model);
    }

    public virtual void Update(TModel model)
    {
        var entity = Mapper.Map<TEntity>(model);
        Update(entity);
    }

    public virtual void Delete(TModel model)
    {
        var entity = Mapper.Map<TEntity>(model);
        Delete(entity);
    }
}

public abstract class UnitOfWork<TDbContext, TEntity> : IUnitOfWork
    where TDbContext : DbContext
    where TEntity : class
{
    protected UnitOfWork(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected virtual TDbContext DbContext { get; init; }

    public virtual async Task CommitAsync(CancellationToken cancellation = default)
    {
        await DbContext.SaveChangesAsync(cancellation);
    }

    private protected virtual void Create(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    private protected virtual void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    private protected virtual void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }
}
