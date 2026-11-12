using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Configurations;

public abstract class AceEntityConfiguration<TEntity, TId> : AceEntityConfiguration<TEntity, TId, TId>
    where TEntity : class, IAceEntity<TId>
    where TId : IEquatable<TId>
{
}

public abstract class AceEntityConfiguration<TEntity, TId, TTraceId> : CrudEntityConfiguration<TEntity, TId>
    where TEntity : class, IAceEntity<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        ConfigureTraceEntity(builder);
    }

    protected override void ConfigureGlobalFilters(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(e => e.IsDeleted == false);
    }

    protected virtual void ConfigureTraceEntity(EntityTypeBuilder<TEntity> builder)
    {
        builder
            .Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
