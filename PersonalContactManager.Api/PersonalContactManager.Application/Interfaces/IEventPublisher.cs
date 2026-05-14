using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
