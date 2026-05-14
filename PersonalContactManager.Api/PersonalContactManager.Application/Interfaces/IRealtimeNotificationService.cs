namespace PersonalContactManager.Application.Interfaces;

public interface IRealtimeNotificationService
{
    Task NotifyContactCreatedAsync(Guid contactId, string fullName, CancellationToken cancellationToken = default);
    Task NotifyContactUpdatedAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task NotifyContactDeletedAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task NotifyReminderFiredAsync(Guid reminderId, Guid contactId, string title, CancellationToken cancellationToken = default);
}
