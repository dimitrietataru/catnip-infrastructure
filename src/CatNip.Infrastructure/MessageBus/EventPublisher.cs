using CatNip.Domain.Events;
using MassTransit;

namespace CatNip.Infrastructure.MessageBus;

public sealed class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint publishEndpoint;

    public EventPublisher(IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellation)
        where TMessage : class, IEvent
    {
        await publishEndpoint.Publish(message, cancellation);
    }
}
