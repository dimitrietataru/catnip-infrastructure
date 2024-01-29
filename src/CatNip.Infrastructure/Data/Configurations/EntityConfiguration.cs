using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Configurations;

public abstract class EntityConfiguration<TEntity, TId> : EntityConfiguration<TEntity>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        ConfigureKeys(builder);
        ConfigureColumns(builder);
        ConfigureIndexes(builder);
        ConfigureGlobalFilters(builder);
    }

    protected virtual void ConfigureKeys(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
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

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    protected abstract string TableName { get; }

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ConfigureTable(builder);
        ConfigureSeed(builder);
    }

    protected virtual void ConfigureTable(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(TableName, ConfigureTable);
    }

    protected virtual void ConfigureTable(TableBuilder<TEntity> tableBuilder)
    {
    }

    protected virtual void ConfigureSeed(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasData(Seed);
    }

    protected virtual IEnumerable<TEntity> Seed => Enumerable.Empty<TEntity>();
}
