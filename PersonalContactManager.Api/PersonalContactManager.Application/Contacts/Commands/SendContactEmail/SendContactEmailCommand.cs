using PersonalContactManager.Application.Common.Interfaces;

namespace PersonalContactManager.Application.Contacts.Commands.SendContactEmail;

public sealed record SendContactEmailCommand(
    Guid ContactId,
    string Subject,
    string Body,
    string? ReplyTo) : ICommand;
