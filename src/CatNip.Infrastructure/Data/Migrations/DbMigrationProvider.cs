namespace CatNip.Infrastructure.Data.Migrations;

public abstract class DbMigrationProvider<TDbContext> : IDbMigrationProvider<TDbContext>
    where TDbContext : DbContext
{
    protected DbMigrationProvider(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected virtual TDbContext DbContext { get; init; }

    public virtual async Task MigrateAsync(CancellationToken cancellation = default)
    {
        await DbContext.Database.EnsureCreatedAsync(cancellation);
        await DbContext.Database.MigrateAsync(cancellation);
    }
}
