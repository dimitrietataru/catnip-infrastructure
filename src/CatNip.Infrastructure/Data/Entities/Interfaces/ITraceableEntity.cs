using CatNip.Infrastructure.Data.Entities.Interfaces.Trace;

namespace CatNip.Infrastructure.Data.Entities.Interfaces;

public interface ITraceableEntity<TId> : ITraceableEntity<TId, TId>
    where TId : IEquatable<TId>
{
}

public interface ITraceableEntity<TEntityId, TTraceId> : IEntity<TEntityId>, ITraceable<TTraceId>
    where TEntityId : IEquatable<TEntityId>
    where TTraceId : IEquatable<TTraceId>
{
}
