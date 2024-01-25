namespace CatNip.Infrastructure.Data.Entities.Interfaces.Trace;

public interface IAddTraceable<TTraceId>
    where TTraceId : IEquatable<TTraceId>
{
    DateTimeOffset? CreatedAt { get; set; }
    TTraceId? CreatedBy { get; set; }
}
