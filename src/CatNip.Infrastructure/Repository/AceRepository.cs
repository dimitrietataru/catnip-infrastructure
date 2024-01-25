using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Pagination;
using CatNip.Domain.Query.Sorting;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Repository;
using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Repository;

public abstract class AceRepository<TDbContext, TEntity, TModel, TId, TFiltering>
    : CrudRepository<TDbContext, TEntity, TModel, TId>, IAceRepository<TModel, TId, TFiltering>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TId>
    where TModel : IModel<TId>
    where TId : IEquatable<TId>
    where TFiltering : IFilteringRequest
{
    protected AceRepository(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public async Task<QueryResponse<TModel>> GetAsync(QueryRequest<TFiltering> request, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();

        var filteringQuery = ComposeFilteringQuery(baseQuery, request.Filter);
        int count = await filteringQuery.AsNoTracking().CountAsync(cancellation);

        var sortQuery = ComposeSortingQuery(filteringQuery, request);
        var paginationQuery = ApplyPagination(sortQuery, request);

        var items = await paginationQuery
            .AsNoTracking()
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellation);

        return new QueryResponse<TModel>(request.Page, request.Size, count, items);
    }

    public async Task<int> CountAsync(QueryRequest<TFiltering> request, CancellationToken cancellation = default)
    {
        var query = GetQueriable();
        query = ComposeFilteringQuery(query, request.Filter);

        return await query.AsNoTracking().CountAsync(cancellation);
    }

    protected virtual IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query, IPaginationRequest request)
    {
        int page = request.Page ?? 1;
        int size = request.Size ?? int.MaxValue;

        return query.Skip((page - 1) * size).Take(size);
    }

    protected virtual IQueryable<TEntity> ComposeSortingQuery(IQueryable<TEntity> query, ISortingRequest request)
    {
        string sortBy = request.SortBy ?? "id";
        var sortDirection = request.SortDirection ?? SortDirection.Ascending;

        if (string.Equals(sortBy, "id", StringComparison.OrdinalIgnoreCase))
        {
            return sortDirection switch
            {
                SortDirection.Ascending => query.OrderBy(e => e.Id),
                SortDirection.Descending => query.OrderByDescending(e => e.Id),
                _ => query
            };
        }

        return query;
    }

    protected abstract IQueryable<TEntity> ComposeFilteringQuery(IQueryable<TEntity> query, TFiltering queryParams);
}
