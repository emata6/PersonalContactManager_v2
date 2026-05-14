using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.AssignTag;

public sealed class AssignTagCommandHandler : IRequestHandler<AssignTagCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly ITagRepository _tagRepository;
    private readonly HybridCache _cache;

    public AssignTagCommandHandler(IContactRepository contactRepository, ITagRepository tagRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _tagRepository = tagRepository;
        _cache = cache;
    }

    public async Task Handle(AssignTagCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken)
            ?? throw new NotFoundException("Tag", request.TagId);

        contact.AssignTag(tag.Id);
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.ContactId}", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
