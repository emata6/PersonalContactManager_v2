using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.AssignTag;

public sealed record AssignTagCommand(Guid ContactId, Guid TagId) : ICommand;
