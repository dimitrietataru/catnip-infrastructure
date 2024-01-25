namespace CatNip.Infrastructure.Data.Entities.Interfaces.Trace;

public interface IEditTraceable<TTraceId>
    where TTraceId : IEquatable<TTraceId>
{
    DateTimeOffset? UpdatedAt { get; set; }
    TTraceId? UpdatedBy { get; set; }
}
