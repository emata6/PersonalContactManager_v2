using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Application.Contacts.Commands.RemovePhoneNumber;

public sealed class RemovePhoneNumberCommandHandler : IRequestHandler<RemovePhoneNumberCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public RemovePhoneNumberCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        var phoneNumber = PhoneNumber.Create(request.Number, request.Label);
        contact.RemovePhoneNumber(phoneNumber);
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.ContactId}", cancellationToken);
    }
}
