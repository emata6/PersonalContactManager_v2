using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.Common.Exceptions;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Groups.Commands.DeleteGroup;

public sealed class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly HybridCache _cache;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository, HybridCache cache)
    {
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Group", request.Id);

        _groupRepository.Remove(group);

        await _cache.RemoveByTagAsync("groups", cancellationToken);
        await _cache.RemoveByTagAsync("contacts", cancellationToken);
    }
}
