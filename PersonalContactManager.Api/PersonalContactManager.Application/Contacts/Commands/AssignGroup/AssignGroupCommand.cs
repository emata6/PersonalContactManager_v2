using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.AssignGroup;

public sealed record AssignGroupCommand(Guid ContactId, Guid GroupId) : ICommand;
