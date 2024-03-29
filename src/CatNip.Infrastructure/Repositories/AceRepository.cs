using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Pagination;
using CatNip.Domain.Query.Sorting;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Repositories;

public abstract class AceRepository<TDbContext, TEntity, TModel, TId, TFiltering> :
    CrudRepository<TDbContext, TEntity, TModel, TId>, IAceRepository<TModel, TId, TFiltering>
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

    public virtual async Task<QueryResponse<TModel>> GetAsync(
        QueryRequest<TFiltering> request, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();

        var filteringQuery = BuildFilteringQuery(baseQuery, request.Filter);
        int count = await filteringQuery.AsNoTracking().CountAsync(cancellation);

        var sortQuery = BuildSortingQuery(filteringQuery, request);
        var paginationQuery = BuildPaginationQuery(sortQuery, request);
        var items = await paginationQuery
            .AsNoTracking()
            .ProjectTo<TModel>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellation);

        return new QueryResponse<TModel>(request.Page, request.Size, count, items);
    }

    public virtual async Task<int> CountAsync(
        TFiltering filter, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        var filteringQuery = BuildFilteringQuery(baseQuery, filter);

        int count = await filteringQuery.AsNoTracking().CountAsync(cancellation);

        return count;
    }

    public virtual async Task<bool> ExistsAsync(
        TFiltering filter, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        var filteringQuery = BuildFilteringQuery(baseQuery, filter);

        bool exists = await filteringQuery.AsNoTracking().AnyAsync(cancellation);

        return exists;
    }

    protected virtual IQueryable<TEntity> BuildPaginationQuery(
        IQueryable<TEntity> query, IPaginationRequest paginationRequest)
    {
        if (!paginationRequest.HasPaginationData())
        {
            return query;
        }

        int page = paginationRequest.Page!.Value;
        int size = paginationRequest.Size!.Value;

        return query.Skip((page - 1) * size).Take(size);
    }

    protected virtual IQueryable<TEntity> BuildSortingQuery(
        IQueryable<TEntity> query, ISortingRequest sortingRequest)
    {
        if (!sortingRequest.HasSortingData())
        {
            return BuildDefaultSortingQuery(query);
        }

        string sortBy = sortingRequest.SortBy!;
        var sortDirection = sortingRequest.SortDirection!.Value;

        query = (sortBy, sortDirection) switch
        {
            ("id", SortDirection.Ascending) => query.OrderBy(_ => _.Id),
            ("id", SortDirection.Descending) => query.OrderByDescending(_ => _.Id),
            _ => BuildDefaultSortingQuery(query)
        };

        return query;
    }

    protected abstract IQueryable<TEntity> BuildFilteringQuery(
        IQueryable<TEntity> query, TFiltering request);
}
