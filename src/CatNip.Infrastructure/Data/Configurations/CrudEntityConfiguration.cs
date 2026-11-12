using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Configurations;

public abstract class CrudEntityConfiguration<TEntity, TId> : EntityConfiguration<TEntity>
    where TEntity : class, ICrudEntity<TId>
    where TId : IEquatable<TId>
{
    protected override void ConfigureKeys(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
    }
}
