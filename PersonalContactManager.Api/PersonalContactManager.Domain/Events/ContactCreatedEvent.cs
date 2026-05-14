using PersonalContactManager.Domain.Common;

namespace PersonalContactManager.Domain.Events;

public sealed record ContactCreatedEvent(
    Guid ContactId,
    string FullName,
    string? Email,
    DateTime CreatedAt) : IDomainEvent;
