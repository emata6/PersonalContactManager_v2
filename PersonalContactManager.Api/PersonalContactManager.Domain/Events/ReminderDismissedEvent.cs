using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Events;

public sealed record ReminderDismissedEvent(
    Guid ReminderId,
    Guid ContactId,
    DateTime DismissedAt) : IDomainEvent;
