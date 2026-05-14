using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.DeleteContact;

public sealed record DeleteContactCommand(Guid Id) : ICommand;
