using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.RemoveTag;

public sealed record RemoveTagCommand(Guid ContactId, Guid TagId) : ICommand;
