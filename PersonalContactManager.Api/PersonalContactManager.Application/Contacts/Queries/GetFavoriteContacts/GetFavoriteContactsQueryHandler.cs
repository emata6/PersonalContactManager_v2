using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Queries.GetFavoriteContacts;

public sealed class GetFavoriteContactsQueryHandler : IRequestHandler<GetFavoriteContactsQuery, List<ContactSummaryDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public GetFavoriteContactsQueryHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<List<ContactSummaryDto>> Handle(GetFavoriteContactsQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            "contacts:favorites",
            async ct =>
            {
                var searchParams = new ContactSearchParams(
                    SearchTerm: null,
                    TagId: null,
                    GroupId: null,
                    FavoritesOnly: true,
                    HasBirthday: null,
                    SortBy: null,
                    SortDirection: null,
                    Page: 1,
                    PageSize: 100);

                var result = await _contactRepository.SearchAsync(searchParams, ct);
                return result.Items.Adapt<List<ContactSummaryDto>>();
            },
            tags: ["contacts"],
            cancellationToken: cancellationToken);
    }
}
