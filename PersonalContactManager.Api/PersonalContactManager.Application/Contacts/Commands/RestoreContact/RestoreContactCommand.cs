using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.RestoreContact;

public sealed record RestoreContactCommand(Guid Id) : ICommand;
