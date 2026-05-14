using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.RemoveTag;

public sealed class RemoveTagCommandHandler : IRequestHandler<RemoveTagCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public RemoveTagCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(RemoveTagCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        contact.RemoveTag(request.TagId);
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.ContactId}", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
