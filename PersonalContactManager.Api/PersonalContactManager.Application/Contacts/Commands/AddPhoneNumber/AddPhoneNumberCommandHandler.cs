using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;
using PersonalContactManager.Domain.ValueObjects;

namespace PersonalContactManager.Application.Contacts.Commands.AddPhoneNumber;

public sealed class AddPhoneNumberCommandHandler : IRequestHandler<AddPhoneNumberCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public AddPhoneNumberCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        var phoneNumber = PhoneNumber.Create(request.Number, request.Label);
        contact.AddPhoneNumber(phoneNumber);
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.ContactId}", cancellationToken);
    }
}
