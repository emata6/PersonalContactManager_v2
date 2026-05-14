using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Enums;

namespace PersonalContactManager.Domain.Events;

public sealed record ReminderFiredEvent(
    Guid ReminderId,
    Guid ContactId,
    string Title,
    ReminderChannel Channel,
    DateTime FiredAt) : IDomainEvent;
