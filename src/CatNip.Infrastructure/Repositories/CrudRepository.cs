using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Exceptions;

namespace CatNip.Infrastructure.Repositories;

public abstract class CrudRepository<TDbContext, TEntity, TModel, TId> : Repository<TDbContext, TEntity, TModel>, ICrudRepository<TModel, TId>
    where TDbContext : DbContext
    where TEntity : class, ICrudEntity<TId>
    where TModel : ICrudModel<TId>
    where TId : IEquatable<TId>
{
    protected CrudRepository(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public override async Task UpdateAsync(TModel model, CancellationToken cancellation = default)
    {
        await UpdateAsync(model.Id, model, cancellation);
    }

    public override async Task DeleteAsync(TModel model, CancellationToken cancellation = default)
    {
        await DeleteAsync(model.Id, cancellation);
    }

    public virtual async Task<TModel> GetByIdAsync(TId id, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueryable();
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
        var baseQuery = GetQueryable();
        bool exists = await baseQuery.AsNoTracking().AnyAsync(e => e.Id.Equals(id), cancellation);

        return exists;
    }

    public virtual async Task UpdateAsync(TId id, TModel model, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id, cancellation);
        Mapper.Map(model, entity);

        await UpdateAsync(entity, cancellation);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id, cancellation);

        await DeleteAsync(entity, cancellation);
    }

    protected abstract IQueryable<TEntity> BuildIncludeQuery(IQueryable<TEntity> query);

    protected override IQueryable<TEntity> BuildDefaultSortingQuery(IQueryable<TEntity> query)
    {
        return query.OrderBy(e => e.Id);
    }

    private protected virtual async Task<TEntity> FindAsync(TId id, CancellationToken cancellation = default)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync([id], cancellation);
        _ = entity ?? throw new EntityNotFoundException<TEntity, TId>(id);

        return entity;
    }
}
