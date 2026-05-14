using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.RemoveGroup;

public sealed class RemoveGroupCommandHandler : IRequestHandler<RemoveGroupCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public RemoveGroupCommandHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task Handle(RemoveGroupCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        contact.RemoveGroup(request.GroupId);
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.ContactId}", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
