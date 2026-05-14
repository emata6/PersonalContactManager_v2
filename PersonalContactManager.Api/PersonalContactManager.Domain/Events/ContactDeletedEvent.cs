using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Events;

public sealed record ContactDeletedEvent(
    Guid ContactId,
    DateTime DeletedAt) : IDomainEvent;
