using CatNip.Infrastructure.Data.Entities.Interfaces;

namespace CatNip.Infrastructure.Data.Entities;

public abstract class AceEntity<TId> : AceEntity<TId, TId>, IAceEntity<TId>
    where TId : IEquatable<TId>
{
}

public abstract class AceEntity<TId, TTraceId> : CrudEntity<TId>, IAceEntity<TId, TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
{
    public virtual DateTimeOffset? CreatedAt { get; set; }
    public virtual TTraceId? CreatedBy { get; set; }

    public virtual DateTimeOffset? UpdatedAt { get; set; }
    public virtual TTraceId? UpdatedBy { get; set; }

    public virtual bool IsDeleted { get; set; }
    public virtual DateTimeOffset? DeletedAt { get; set; }
    public virtual TTraceId? DeletedBy { get; set; }
}
