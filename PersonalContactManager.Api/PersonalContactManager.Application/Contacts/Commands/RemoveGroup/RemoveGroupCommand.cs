using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.RemoveGroup;

public sealed record RemoveGroupCommand(Guid ContactId, Guid GroupId) : ICommand;
