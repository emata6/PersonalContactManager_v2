using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Commands.AssignGroup;

public sealed class AssignGroupCommandHandler : IRequestHandler<AssignGroupCommand>
{
    private readonly IContactRepository _contactRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly HybridCache _cache;

    public AssignGroupCommandHandler(IContactRepository contactRepository, IGroupRepository groupRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task Handle(AssignGroupCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new NotFoundException("Contact", request.ContactId);

        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException("Group", request.GroupId);

        contact.AssignGroup(group.Id);
        _contactRepository.Update(contact);

        await _cache.RemoveAsync($"contact:{request.ContactId}", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
