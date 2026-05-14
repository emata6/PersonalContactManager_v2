using PersonalContactManager.Application.Common.Interfaces;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application.Contacts.Commands.CreateContact;

public sealed record CreateContactCommand(
    string FirstName,
    string LastName,
    string? Email,
    DateOnly Birthday,
    string? Notes,
    string Iban,
    PhoneInput Phone,
    AddressInput Address) : ICommand<Guid>;
