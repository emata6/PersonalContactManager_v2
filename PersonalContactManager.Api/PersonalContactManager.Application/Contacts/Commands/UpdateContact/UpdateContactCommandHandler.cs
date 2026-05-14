using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Application.Contacts.Commands.UpdateContact;

public sealed class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public UpdateContactCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Contact", request.Id);

        if (request.Email is not null && request.Email != contact.Email)
        {
            var duplicate = await _contactRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (duplicate is not null)
                throw new ConflictException($"A contact with email '{request.Email}' already exists.");
        }

        contact.Update(request.FirstName, request.LastName, request.Email, request.Birthday, request.Notes, request.Iban);
        contact.SetAddress(BuildAddress(request.Address));
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.Id}", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }

    private static Address BuildAddress(AddressInput input) =>
        Address.Create(input.Street, input.City, input.State, input.PostalCode, input.Country);
}
