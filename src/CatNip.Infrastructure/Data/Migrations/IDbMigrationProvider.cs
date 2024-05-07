namespace CatNip.Infrastructure.Data.Migrations;

public interface IDbMigrationProvider<TDbContext>
    where TDbContext : DbContext
{
    Task MigrateAsync(CancellationToken cancellation = default);
}
