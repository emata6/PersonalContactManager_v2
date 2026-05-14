using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Reminders.Commands.DismissReminder;

public sealed record DismissReminderCommand(Guid Id) : ICommand;
