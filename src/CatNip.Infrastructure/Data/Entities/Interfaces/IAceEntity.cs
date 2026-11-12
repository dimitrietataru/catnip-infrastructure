using CatNip.Infrastructure.Data.Entities.Interfaces.Trace;

namespace CatNip.Infrastructure.Data.Entities.Interfaces;

public interface IAceEntity<TId> : IAceEntity<TId, TId>
    where TId : IEquatable<TId>
{
}

public interface IAceEntity<TId, TTraceId> : ICrudEntity<TId>, IAddTraceable<TTraceId>, IEditTraceable<TTraceId>, IRemoveTraceable<TTraceId>
    where TId : IEquatable<TId>
    where TTraceId : IEquatable<TTraceId>
{
}
