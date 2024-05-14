namespace CatNip.Infrastructure.Data.Migrations;

public interface IDbMigrationProvider
{
    Task MigrateAsync(CancellationToken cancellation = default);

    Task EnsureCreatedAsync(CancellationToken cancellation = default);
    Task EnsureDeletedAsync(CancellationToken cancellation = default);
}
