using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Pagination;
using CatNip.Domain.Query.Sorting;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Repositories;

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

    public async Task<int> CountAsync(QueryRequest<TFiltering> request, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueriable();
        var filteringQuery = BuildFilteringQuery(baseQuery, request.Filter);

        return await filteringQuery.AsNoTracking().CountAsync(cancellation);
    }

    protected virtual IQueryable<TEntity> BuildPaginationQuery(IQueryable<TEntity> query, IPaginationRequest paginationRequest)
    {
        int page = paginationRequest.Page ?? 1;
        int size = paginationRequest.Size ?? int.MaxValue;

        return query.Skip((page - 1) * size).Take(size);
    }

    protected virtual IQueryable<TEntity> BuildSortingQuery(IQueryable<TEntity> query, ISortingRequest sortingRequest)
    {
        string sortBy = sortingRequest.SortBy ?? "id";
        var sortDirection = sortingRequest.SortDirection ?? SortDirection.Ascending;

        query = (sortBy, sortDirection) switch
        {
            ("id", SortDirection.Ascending) => query.OrderBy(_ => _.Id),
            ("id", SortDirection.Descending) => query.OrderByDescending(_ => _.Id),
            _ => query
        };

        return query;
    }

    protected abstract IQueryable<TEntity> BuildFilteringQuery(IQueryable<TEntity> query, TFiltering request);
}
