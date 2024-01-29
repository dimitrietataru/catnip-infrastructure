using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Exceptions;

namespace CatNip.Infrastructure.Repositories;

public abstract class CrudRepository<TDbContext, TEntity, TModel, TId>
    : Repository<TDbContext, TEntity, TId>, ICrudRepository<TModel, TId>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
{
    protected CrudRepository(TDbContext dbContext, IMapper mapper)
        : base(dbContext)
    {
        Mapper = mapper;
    }

    protected virtual IMapper Mapper { get; init; }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellation = default)
    {
        var result = await GetQueriable()
            .AsNoTracking()
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellation);

        return result;
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellation = default)
    {
        int count = await GetQueriable()
            .AsNoTracking()
            .CountAsync(cancellation);

        return count;
    }

    public virtual async Task<TModel> GetByIdAsync(TId id, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        var includeQuery = BuildIncludeQuery(baseQuery);

        var result = await includeQuery
            .AsNoTracking()
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellation);
        _ = result ?? throw new EntityNotFoundException<TEntity, TId>(id);

        return result;
    }

    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellation = default)
    {
        bool exists = await GetQueriable()
            .AsNoTracking()
            .AnyAsync(e => e.Id.Equals(id), cancellation);

        return exists;
    }

    public virtual async Task<TModel> CreateAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);

        await CreateAsync(model, cancellation).ConfigureAwait(false);

        return Mapper.Map<TModel>(entity);
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(model.Id);
        Mapper.Map(model, entity);

        await UpdateAsync(entity, cancellation).ConfigureAwait(false);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id);

        await DeleteAsync(entity, cancellation).ConfigureAwait(false);
    }

    protected abstract IQueryable<TEntity> BuildIncludeQuery(IQueryable<TEntity> query);
}
