using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Events;

public sealed record ContactUpdatedEvent(
    Guid ContactId,
    string FullName,
    DateTime UpdatedAt) : IDomainEvent;
