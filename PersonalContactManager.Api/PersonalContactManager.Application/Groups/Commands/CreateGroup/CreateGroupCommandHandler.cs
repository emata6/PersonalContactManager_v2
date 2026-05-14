using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Groups.Commands.CreateGroup;

public sealed class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, Guid>
{
    private readonly IGroupRepository _groupRepository;
    private readonly HybridCache _cache;

    public CreateGroupCommandHandler(IGroupRepository groupRepository, HybridCache cache)
    {
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task<Guid> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = Group.Create(request.Name, request.Description);
        await _groupRepository.AddAsync(group, cancellationToken);

        await _cache.RemoveByTagAsync("groups", cancellationToken);

        return group.Id;
    }
}
