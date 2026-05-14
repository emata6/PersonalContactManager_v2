using Microsoft.AspNetCore.SignalR;
using PersonalContactManager.Api.Hubs;
using PersonalContactManager.Application.Interfaces;

namespace PersonalContactManager.Api.Services;

public sealed class RealtimeNotificationService(IHubContext<ContactsHub> hub) : IRealtimeNotificationService
{
    public Task NotifyContactCreatedAsync(Guid contactId, string fullName, CancellationToken cancellationToken = default)
        => hub.Clients.Group("contacts").SendAsync("ContactCreated", new { contactId, fullName }, cancellationToken);

    public Task NotifyContactUpdatedAsync(Guid contactId, CancellationToken cancellationToken = default)
        => hub.Clients.Group("contacts").SendAsync("ContactUpdated", new { contactId }, cancellationToken);

    public Task NotifyContactDeletedAsync(Guid contactId, CancellationToken cancellationToken = default)
        => hub.Clients.Group("contacts").SendAsync("ContactDeleted", new { contactId }, cancellationToken);

    public Task NotifyReminderFiredAsync(Guid reminderId, Guid contactId, string title, CancellationToken cancellationToken = default)
        => hub.Clients.Group("contacts").SendAsync("ReminderFired", new { reminderId, contactId, title }, cancellationToken);
}
