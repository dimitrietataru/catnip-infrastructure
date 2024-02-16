using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Exceptions;

namespace CatNip.Infrastructure.Repositories;

public abstract class CrudRepository<TDbContext, TEntity, TModel, TId>
    : UnitOfWork<TDbContext, TEntity, TModel>, ICrudRepository<TModel, TId>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    protected CrudRepository(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public IUnitOfWork<TModel> UnitOfWork => this;

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        var sortQuery = BuildDefaultSortQuery(baseQuery);

        var result = await sortQuery
            .AsNoTracking()
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellation);

        return result;
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        int count = await baseQuery.AsNoTracking().CountAsync(cancellation);

        return count;
    }

    public virtual async Task<TModel> GetByIdAsync(TId id, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        var includeQuery = BuildIncludeQuery(baseQuery);

        var result = await includeQuery
            .AsNoTracking()
            .Where(e => e.Id.Equals(id))
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellation);
        _ = result ?? throw new EntityNotFoundException<TEntity, TId>(id);

        return result;
    }

    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        bool exists = await baseQuery.AsNoTracking().AnyAsync(e => e.Id.Equals(id), cancellation);

        return exists;
    }

    public virtual async Task<TModel> CreateAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);

        Create(entity);
        await CommitAsync(cancellation);

        return Mapper.Map(entity, model);
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);

        await UpdateAsync(entity, cancellation);
    }

    public virtual async Task UpdateAsync(TId id, TModel model, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id);
        Mapper.Map(model, entity);

        await UpdateAsync(entity, cancellation);
    }

    public virtual async Task DeleteAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);

        await DeleteAsync(entity, cancellation);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id);

        await DeleteAsync(entity, cancellation);
    }

    protected virtual IQueryable<TEntity> BuildDefaultSortQuery(IQueryable<TEntity> query)
    {
        return query.OrderBy(e => e.Id);
    }

    protected abstract IQueryable<TEntity> BuildIncludeQuery(IQueryable<TEntity> query);

    private protected virtual async Task<TEntity> FindAsync(TId id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        _ = entity ?? throw new EntityNotFoundException<TEntity, TId>(id);

        return entity;
    }
}
