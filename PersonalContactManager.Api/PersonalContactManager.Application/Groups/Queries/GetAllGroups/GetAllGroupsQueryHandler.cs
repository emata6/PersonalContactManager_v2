using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Groups.Queries.GetAllGroups;

public sealed class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, List<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly HybridCache _cache;

    public GetAllGroupsQueryHandler(IGroupRepository groupRepository, HybridCache cache)
    {
        _groupRepository = groupRepository;
        _cache = cache;
    }

    public async Task<List<GroupDto>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            "groups:all",
            async ct =>
            {
                var groups = await _groupRepository.GetAllAsync(ct);
                return groups.Adapt<List<GroupDto>>();
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30) },
            tags: ["groups"],
            cancellationToken: cancellationToken);
    }
}
