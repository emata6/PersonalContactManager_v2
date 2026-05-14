using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.AddPhoneNumber;

public sealed record AddPhoneNumberCommand(Guid ContactId, string Number, string Label) : ICommand;
