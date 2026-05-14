using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Reminders.Commands.CancelReminder;

public sealed record CancelReminderCommand(Guid Id) : ICommand;
