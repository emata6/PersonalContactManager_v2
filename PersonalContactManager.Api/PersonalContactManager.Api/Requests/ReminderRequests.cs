using PersonalContactManager.Domain.Enums;

namespace PersonalContactManager.Api.Requests;

public sealed record CreateReminderRequest(
    string Title,
    string? Note,
    DateTime DueAt,
    string? RecurrenceRule,
    ReminderChannel Channel);

public sealed record UpdateReminderRequest(
    string Title,
    string? Note,
    DateTime DueAt,
    string? RecurrenceRule,
    ReminderChannel Channel);
