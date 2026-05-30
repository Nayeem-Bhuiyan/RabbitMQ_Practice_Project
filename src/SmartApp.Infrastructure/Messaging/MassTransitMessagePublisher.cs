namespace SmartApp.Infrastructure.Messaging;

using MassTransit;
using SmartApp.Application.Abstractions;

public sealed class MassTransitMessagePublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        await publishEndpoint.Publish(message, cancellationToken);
    }
}