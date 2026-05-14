using PersonalContactManager.Domain.Enums;

namespace PersonalContactManager.Application.DTOs;

public sealed class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public string ContactFullName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime DueAt { get; set; }
    public string? RecurrenceRule { get; set; }
    public ReminderChannel Channel { get; set; }
    public ReminderStatus Status { get; set; }
    public DateTime? FiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
