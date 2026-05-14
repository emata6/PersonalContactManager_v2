using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonalContactManager.Application.Interfaces;
using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Enums;
using PersonalContactManager.Domain.Events;
using PersonalContactManager.Infrastructure.Email;
using PersonalContactManager.Infrastructure.Persistence;

namespace PersonalContactManager.Infrastructure.Messaging;

public sealed class DirectEventDispatcher : IEventPublisher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DirectEventDispatcher> _logger;

    public DirectEventDispatcher(IServiceScopeFactory scopeFactory, ILogger<DirectEventDispatcher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        switch (domainEvent)
        {
            case ContactCreatedEvent e:
            {
                var notifications = scope.ServiceProvider.GetRequiredService<IRealtimeNotificationService>();
                await notifications.NotifyContactCreatedAsync(e.ContactId, e.FullName, cancellationToken);
                break;
            }
            case ContactUpdatedEvent e:
            {
                var notifications = scope.ServiceProvider.GetRequiredService<IRealtimeNotificationService>();
                await notifications.NotifyContactUpdatedAsync(e.ContactId, cancellationToken);
                break;
            }
            case ContactDeletedEvent e:
            {
                var notifications = scope.ServiceProvider.GetRequiredService<IRealtimeNotificationService>();
                await notifications.NotifyContactDeletedAsync(e.ContactId, cancellationToken);
                break;
            }
            case ReminderFiredEvent e:
            {
                var notifications = scope.ServiceProvider.GetRequiredService<IRealtimeNotificationService>();
                var emailService  = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var db            = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (e.Channel is ReminderChannel.SignalR or ReminderChannel.Both)
                    await notifications.NotifyReminderFiredAsync(e.ReminderId, e.ContactId, e.Title, cancellationToken);

                if (e.Channel is ReminderChannel.Email or ReminderChannel.Both)
                {
                    var contact = await db.Contacts.FirstOrDefaultAsync(c => c.Id == e.ContactId, cancellationToken);
                    if (contact?.Email is not null)
                    {
                        var reminder = await db.Reminders.FirstOrDefaultAsync(r => r.Id == e.ReminderId, cancellationToken);
                        await emailService.SendReminderAsync(
                            new ReminderEmailDto(contact.Email, contact.FullName, e.Title, reminder?.Note, e.FiredAt),
                            cancellationToken);
                    }
                    else
                    {
                        _logger.LogWarning("Reminder {ReminderId}: contact {ContactId} has no email address",
                            e.ReminderId, e.ContactId);
                    }
                }
                break;
            }
            default:
                _logger.LogDebug("No handler for domain event {EventType}", typeof(TEvent).Name);
                break;
        }
    }
}
