using CatNip.Domain.ImportExport;
using CatNip.Domain.ImportExport.Csv;
using CatNip.Domain.ImportExport.Excel;
using CatNip.Domain.Models.Interfaces;
using CatNip.Domain.Query;
using CatNip.Domain.Query.Filtering;
using CatNip.Domain.Query.Pagination;
using CatNip.Domain.Query.Sorting;
using CatNip.Domain.Query.Sorting.Symbols;
using CatNip.Domain.Repositories;
using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Repositories;

public abstract class AceRepository<TDbContext, TEntity, TModel, TId, TModelRoot, TFiltering, TExchange>
    : AceRepository<TDbContext, TEntity, TModel, TId, TId, TModelRoot, TFiltering, TExchange>, IAceRepository<TModel, TId, TModelRoot, TFiltering, TExchange>
    where TDbContext : DbContext
    where TEntity : class, IAceEntity<TId>
    where TModel : IAceModel<TId>
    where TId : IEquatable<TId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
    protected AceRepository(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }
}

public abstract class AceRepository<TDbContext, TEntity, TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange>
    : CrudRepository<TDbContext, TEntity, TModel, TId>, IAceRepository<TModel, TId, TTraceId, TModelRoot, TFiltering, TExchange>
    where TDbContext : DbContext
    where TEntity : class, IAceEntity<TId, TTraceId>
    where TModel : IAceModel<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
    where TModelRoot : ICrudModel<TId>
    where TFiltering : IFilteringRequest
    where TExchange : ICsvMappable, IExcelMappable
{
    protected AceRepository(TDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public virtual async Task<QueryResponse<TModelRoot>> GetAsync(QueryRequest<TFiltering> request, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueryable();

        var filteringQuery = BuildFilteringQuery(baseQuery, request.Filter);
        int count = await filteringQuery.AsNoTracking().CountAsync(cancellation);

        var sortQuery = BuildSortingQuery(filteringQuery, request);
        var paginationQuery = BuildPaginationQuery(sortQuery, request);
        var items = await paginationQuery
            .AsNoTracking()
            .ProjectTo<TModelRoot>(Mapper.ConfigurationProvider)
            .ToListAsync(cancellation);

        return new QueryResponse<TModelRoot>(request.Page, request.Size, count, items);
    }

    public virtual async Task<int> CountAsync(TFiltering filter, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueryable();
        var filteringQuery = BuildFilteringQuery(baseQuery, filter);

        return await filteringQuery.AsNoTracking().CountAsync(cancellation);
    }

    public virtual async Task<bool> ExistsAsync(TFiltering filter, CancellationToken cancellation = default)
    {
        var baseQuery = GetQueryable();
        var filteringQuery = BuildFilteringQuery(baseQuery, filter);

        return await filteringQuery.AsNoTracking().AnyAsync(cancellation);
    }

    public abstract Task<ImportResponse> ImportAsync(ICollection<TExchange> records, CancellationToken cancellation = default);

    protected abstract IQueryable<TEntity> BuildFilteringQuery(IQueryable<TEntity> query, TFiltering filter);

    protected virtual IQueryable<TEntity> BuildPaginationQuery(IQueryable<TEntity> query, IPaginationRequest paginationRequest)
    {
        if (!paginationRequest.HasPaginationData())
        {
            return query;
        }

        int page = paginationRequest.Page!.Value;
        int size = paginationRequest.Size!.Value;

        return query.Skip((page - 1) * size).Take(size);
    }

    protected virtual IQueryable<TEntity> BuildSortingQuery(IQueryable<TEntity> query, ISortingRequest sortingRequest)
    {
        if (!sortingRequest.HasSortingData())
        {
            return BuildDefaultSortingQuery(query);
        }

        string sortBy = sortingRequest.SortBy!;
        var sortDirection = sortingRequest.SortDirection!.Value;

        query = (sortBy, sortDirection) switch
        {
            ("id", SortDirection.Ascending) => query.OrderBy(e => e.Id),
            ("id", SortDirection.Descending) => query.OrderByDescending(e => e.Id),
            _ => BuildDefaultSortingQuery(query)
        };

        return query;
    }
}
