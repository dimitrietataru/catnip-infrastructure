using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;

namespace CatNip.Infrastructure.Repositories;

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

    protected virtual IQueryable<TEntity> GetQueriable()
    {
        return DbContext.Set<TEntity>();
    }

    private protected async Task CreateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Create(entity);
        await CommitAsync(cancellation);
    }

    private protected void Create(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    private protected async Task UpdateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Update(entity);
        await CommitAsync(cancellation);
    }

    private protected void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    private protected async Task DeleteAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Delete(entity);
        await CommitAsync(cancellation);
    }

    private protected void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }
}
