using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Domain.Enums;

namespace PersonalContactManager.Application.Reminders.Commands.CreateReminder;

public sealed record CreateReminderCommand(
    Guid ContactId,
    string Title,
    string? Note,
    DateTime DueAt,
    string? RecurrenceRule,
    ReminderChannel Channel) : ICommand<Guid>;
