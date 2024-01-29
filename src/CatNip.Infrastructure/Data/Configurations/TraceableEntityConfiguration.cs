using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Configurations;

public abstract class TraceableEntityConfiguration<TEntity, TId> : TraceableEntityConfiguration<TEntity, TId, TId>
    where TEntity : class, ITraceableEntity<TId>
    where TId : IEquatable<TId>
{
}

public abstract class TraceableEntityConfiguration<TEntity, TEntityId, TTraceId> : EntityConfiguration<TEntity, TEntityId>
    where TEntity : class, ITraceableEntity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        ConfigureTraceableTable(builder);
    }

    protected virtual void ConfigureTraceableTable(EntityTypeBuilder<TEntity> builder)
    {
        builder
            .Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }

    protected override void ConfigureGlobalFilters(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(e => e.IsDeleted == false);
    }
}
