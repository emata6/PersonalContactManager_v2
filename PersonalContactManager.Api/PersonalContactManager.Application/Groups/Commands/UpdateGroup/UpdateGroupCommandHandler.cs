using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Groups.Commands.UpdateGroup;

public sealed class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly HybridCache _cache;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository, HybridCache cache)
    {
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Group", request.Id);

        group.Update(request.Name, request.Description);

        await _cache.RemoveByTagAsync("groups", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
