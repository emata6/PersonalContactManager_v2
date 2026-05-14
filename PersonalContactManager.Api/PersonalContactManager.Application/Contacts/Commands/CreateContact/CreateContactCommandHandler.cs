using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Application.Contacts.Commands.CreateContact;

public sealed class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Guid>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public CreateContactCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        if (request.Email is not null)
        {
            var duplicate = await _contactRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (duplicate is not null)
                throw new ConflictException($"A contact with email '{request.Email}' already exists.");
        }

        var contact = Contact.Create(
            request.FirstName,
            request.LastName,
            request.Birthday,
            request.Iban,
            request.Email,
            request.Notes);

        contact.AddPhoneNumber(PhoneNumber.Create(request.Phone.Number, request.Phone.Label));
        contact.SetAddress(BuildAddress(request.Address));

        await _contactRepository.AddAsync(contact, cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);

        return contact.Id;
    }

    private static Address BuildAddress(AddressInput input) =>
        Address.Create(input.Street, input.City, input.State, input.PostalCode, input.Country);
}
