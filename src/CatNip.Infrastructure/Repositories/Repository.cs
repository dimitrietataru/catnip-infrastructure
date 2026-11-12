using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Repositories;
using CatNip.Domain.Repositories.UnitOfWork;
using CatNip.Infrastructure.Data.Entities.Interfaces;
using CatNip.Infrastructure.Repositories.UnitOfWork;

namespace CatNip.Infrastructure.Repositories;

public abstract class Repository<TDbContext, TEntity, TModel> : DbUnitOfWork<TDbContext, TEntity, TModel>, IRepository<TModel>
    where TDbContext : DbContext
    where TEntity : class, IEntity
    where TModel : IModel
{
    protected Repository(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public IUnitOfWork<TModel> UnitOfWork => this;

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellation = default)
    {
        var baseQuery = GetQueryable();
        var sortQuery = BuildDefaultSortingQuery(baseQuery);

        var result = await sortQuery
            .AsNoTracking()
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellation);

        return result;
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellation = default)
    {
        var baseQuery = GetQueryable();
        int count = await baseQuery.AsNoTracking().CountAsync(cancellation);

        return count;
    }

    public virtual async Task<TModel> CreateAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);
        await CreateAsync(entity, cancellation);

        return Mapper.Map(entity, model);
    }

    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);
        await UpdateAsync(entity, cancellation);
    }

    public virtual async Task DeleteAsync(TModel model, CancellationToken cancellation = default)
    {
        var entity = Mapper.Map<TEntity>(model);
        await DeleteAsync(entity, cancellation);
    }

    protected abstract IQueryable<TEntity> BuildDefaultSortingQuery(IQueryable<TEntity> query);
}
