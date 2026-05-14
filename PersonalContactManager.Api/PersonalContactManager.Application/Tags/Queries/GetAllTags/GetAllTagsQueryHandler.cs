using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Tags.Queries.GetAllTags;

public sealed class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<TagDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly HybridCache _cache;

    public GetAllTagsQueryHandler(ITagRepository tagRepository, HybridCache cache)
    {
        _tagRepository = tagRepository;
        _cache = cache;
    }

    public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            "tags:all",
            async ct =>
            {
                var tags = await _tagRepository.GetAllAsync(ct);
                return tags.Adapt<List<TagDto>>();
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30) },
            tags: ["tags"],
            cancellationToken: cancellationToken);
    }
}
