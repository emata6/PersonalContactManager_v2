using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Domain.Enums;

namespace PersonalContactManager.Application.Reminders.Commands.UpdateReminder;

public sealed record UpdateReminderCommand(
    Guid Id,
    string Title,
    string? Note,
    DateTime DueAt,
    string? RecurrenceRule,
    ReminderChannel Channel) : ICommand;
