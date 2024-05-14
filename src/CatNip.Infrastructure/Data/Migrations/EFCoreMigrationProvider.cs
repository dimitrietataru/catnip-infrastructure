namespace CatNip.Infrastructure.Data.Migrations;

public abstract class EFCoreMigrationProvider<TDbContext> : IDbMigrationProvider
    where TDbContext : DbContext
{
    protected EFCoreMigrationProvider(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected virtual TDbContext DbContext { get; init; }

    public virtual async Task MigrateAsync(CancellationToken cancellation = default)
    {
        await DbContext.Database.MigrateAsync(cancellation);
    }

    public virtual async Task EnsureCreatedAsync(CancellationToken cancellation = default)
    {
        await DbContext.Database.EnsureCreatedAsync(cancellation);
    }

    public virtual async Task EnsureDeletedAsync(CancellationToken cancellation = default)
    {
        await DbContext.Database.EnsureDeletedAsync(cancellation);
    }
}
