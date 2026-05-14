using PersonalContactManager.Domain.Common;
using PersonalContactManager.Domain.Enums;
using PersonalContactManager.Domain.Events;

namespace PersonalContactManager.Domain.Entities;

public sealed class Reminder : AggregateRoot
{
    public Guid ContactId { get; private set; }
    public Contact Contact { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public DateTime DueAt { get; private set; }
    public string? RecurrenceRule { get; private set; }
    public ReminderChannel Channel { get; private set; }
    public ReminderStatus Status { get; private set; }
    public DateTime? FiredAt { get; private set; }

    private Reminder() { }

    private Reminder(Guid contactId, string title, string? note, DateTime dueAt, string? recurrenceRule, ReminderChannel channel)
    {
        ContactId = contactId;
        Title = title;
        Note = note;
        DueAt = dueAt;
        RecurrenceRule = recurrenceRule;
        Channel = channel;
        Status = ReminderStatus.Pending;

        AddDomainEvent(new ReminderCreatedEvent(Id, ContactId, Title, DueAt));
    }

    public static Reminder Create(Guid contactId, string title, string? note, DateTime dueAt, string? recurrenceRule, ReminderChannel channel)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Reminder title cannot be empty.", nameof(title));

        if (dueAt <= DateTime.UtcNow)
            throw new ArgumentException("Reminder due date must be in the future.", nameof(dueAt));

        return new Reminder(contactId, title, note, dueAt, recurrenceRule, channel);
    }

    public void Update(string title, string? note, DateTime dueAt, string? recurrenceRule, ReminderChannel channel)
    {
        if (Status != ReminderStatus.Pending)
            throw new InvalidOperationException("Only pending reminders can be updated.");

        Title = title;
        Note = note;
        DueAt = dueAt;
        RecurrenceRule = recurrenceRule;
        Channel = channel;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fire()
    {
        if (Status != ReminderStatus.Pending)
            throw new InvalidOperationException("Only pending reminders can be fired.");

        Status = ReminderStatus.Fired;
        FiredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReminderFiredEvent(Id, ContactId, Title, Channel, FiredAt.Value));
    }

    public void Dismiss()
    {
        if (Status == ReminderStatus.Cancelled)
            throw new InvalidOperationException("Cancelled reminders cannot be dismissed.");

        Status = ReminderStatus.Dismissed;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReminderDismissedEvent(Id, ContactId, DateTime.UtcNow));
    }

    public void Cancel()
    {
        if (Status is ReminderStatus.Fired or ReminderStatus.Dismissed)
            throw new InvalidOperationException("Fired or dismissed reminders cannot be cancelled.");

        Status = ReminderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
