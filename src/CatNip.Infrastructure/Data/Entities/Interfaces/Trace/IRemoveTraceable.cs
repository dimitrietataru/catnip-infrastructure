namespace CatNip.Infrastructure.Data.Entities.Interfaces.Trace;

public interface IRemoveTraceable<TTraceId>
    where TTraceId : IEquatable<TTraceId>
{
    bool IsDeleted { get; set; }

    DateTimeOffset? DeletedAt { get; set; }
    TTraceId? DeletedBy { get; set; }
}
