using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Events;

public sealed record ReminderCreatedEvent(
    Guid ReminderId,
    Guid ContactId,
    string Title,
    DateTime DueAt) : IDomainEvent;
