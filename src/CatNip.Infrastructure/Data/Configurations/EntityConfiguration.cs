using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Configurations;

public abstract class EntityConfiguration<TEntity, TId> : EntityConfiguration<TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    protected override void ConfigureKeys(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
    }
}

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    protected abstract string TableName { get; }
    protected abstract string? TableSchema { get; }
    protected virtual IEnumerable<TEntity> Seed { get; } = [];

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureTable(builder);
        ConfigureSeed(builder);

        ConfigureKeys(builder);
        ConfigureRelationships(builder);

        ConfigureColumns(builder);
        ConfigureIndexes(builder);
        ConfigureGlobalFilters(builder);
    }

    protected virtual void ConfigureTable(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(TableName, TableSchema, ConfigureTable);
    }

    protected virtual void ConfigureTable(TableBuilder<TEntity> tableBuilder)
    {
    }

    protected virtual void ConfigureSeed(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasData(Seed);
    }

    protected virtual void ConfigureKeys(EntityTypeBuilder<TEntity> builder)
    {
    }

    protected virtual void ConfigureRelationships(EntityTypeBuilder<TEntity> builder)
    {
    }

    protected virtual void ConfigureColumns(EntityTypeBuilder<TEntity> builder)
    {
    }

    protected virtual void ConfigureIndexes(EntityTypeBuilder<TEntity> builder)
    {
    }

    protected virtual void ConfigureGlobalFilters(EntityTypeBuilder<TEntity> builder)
    {
    }
}
