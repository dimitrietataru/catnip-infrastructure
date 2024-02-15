using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Exceptions;

namespace CatNip.Infrastructure.Repositories;

public abstract class CrudRepository<TDbContext, TEntity, TModel, TId>
    : UnitOfWork<TDbContext, TEntity, TModel, TId>, ICrudRepository<TModel, TId>
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
        UnitOfWork.Update(model);
        await UnitOfWork.CommitAsync(cancellation);

        return model;
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellation = default)
    {
        UnitOfWork.Update(model);
        await UnitOfWork.CommitAsync(cancellation);
    }

    public virtual async Task UpdateAsync(TId id, TModel model, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id);
        Mapper.Map(model, entity);

        Update(entity);
        await UnitOfWork.CommitAsync(cancellation);
    }

    public virtual async Task DeleteAsync(TId id, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(id);

        Delete(entity);
        await UnitOfWork.CommitAsync(cancellation);
    }

    public virtual async Task DeleteAsync(TModel model, CancellationToken cancellation = default)
    {
        UnitOfWork.Delete(model);
        await UnitOfWork.CommitAsync(cancellation);
    }

    protected abstract IQueryable<TEntity> BuildIncludeQuery(IQueryable<TEntity> query);
}
