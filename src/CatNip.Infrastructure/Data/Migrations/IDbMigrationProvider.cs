namespace CatNip.Infrastructure.Data.Migrations;

public interface IDbMigrationProvider
{
    Task MigrateAsync(CancellationToken cancellation = default);
}
