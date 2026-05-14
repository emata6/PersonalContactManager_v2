using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Commands.UpdateContact;

public sealed record UpdateContactCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    DateOnly Birthday,
    string? Notes,
    string Iban,
    AddressInput Address) : ICommand;
