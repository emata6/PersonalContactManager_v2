using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.RemovePhoneNumber;

public sealed record RemovePhoneNumberCommand(Guid ContactId, string Number, string Label) : ICommand;
