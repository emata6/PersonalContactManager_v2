using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Groups.Queries.GetContactsInGroup;

public sealed class GetContactsInGroupQueryHandler : IRequestHandler<GetContactsInGroupQuery, List<ContactSummaryDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public GetContactsInGroupQueryHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<List<ContactSummaryDto>> Handle(GetContactsInGroupQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"groups:contacts:{request.GroupId}",
            async ct =>
            {
                var searchParams = new ContactSearchParams(
                    SearchTerm: null,
                    TagId: null,
                    GroupId: request.GroupId,
                    FavoritesOnly: null,
                    HasBirthday: null,
                    SortBy: null,
                    SortDirection: null,
                    Page: 1,
                    PageSize: 500);

                var result = await _contactRepository.SearchAsync(searchParams, ct);
                return result.Items.Adapt<List<ContactSummaryDto>>();
            },
            tags: ["contacts", "groups"],
            cancellationToken: cancellationToken);
    }
}
