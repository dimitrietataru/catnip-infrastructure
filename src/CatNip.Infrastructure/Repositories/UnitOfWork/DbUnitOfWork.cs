using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories.UnitOfWork;

namespace CatNip.Infrastructure.Repositories.UnitOfWork;

public abstract class DbUnitOfWork<TDbContext, TEntity, TModel> : DbUnitOfWork<TDbContext, TEntity>, IUnitOfWork<TModel>
    where TDbContext : DbContext
    where TEntity : class
    where TModel : IModel
{
    private protected DbUnitOfWork(TDbContext dbContext, IMapper mapper)
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

public abstract class DbUnitOfWork<TDbContext, TEntity> : DbUnitOfWork<TDbContext>
    where TDbContext : DbContext
    where TEntity : class
{
    private protected DbUnitOfWork(TDbContext dbContext)
        : base(dbContext)
    {
    }

    protected virtual IQueryable<TEntity> GetQueryable()
    {
        return DbContext.Set<TEntity>();
    }

    private protected void Create(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    private protected void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    private protected void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    private protected async Task CreateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Create(entity);
        await CommitAsync(cancellation);
    }

    private protected async Task UpdateAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Update(entity);
        await CommitAsync(cancellation);
    }

    private protected async Task DeleteAsync(TEntity entity, CancellationToken cancellation = default)
    {
        Delete(entity);
        await CommitAsync(cancellation);
    }
}

public abstract class DbUnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : DbContext
{
    private protected DbUnitOfWork(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected virtual TDbContext DbContext { get; init; }

    public virtual async Task CommitAsync(CancellationToken cancellation = default)
    {
        await DbContext.SaveChangesAsync(cancellation);
    }
}
