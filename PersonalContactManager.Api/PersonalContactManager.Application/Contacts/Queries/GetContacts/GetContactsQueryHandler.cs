using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Queries.GetContacts;

public sealed class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, PagedResultDto<ContactSummaryDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public GetContactsQueryHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<PagedResultDto<ContactSummaryDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"contacts:list:{request.SearchTerm}:{request.TagId}:{request.GroupId}:{request.FavoritesOnly}:{request.HasBirthday}:{request.SortBy}:{request.SortDirection}:{request.Page}:{request.PageSize}";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var searchParams = new ContactSearchParams(
                    request.SearchTerm,
                    request.TagId,
                    request.GroupId,
                    request.FavoritesOnly,
                    request.HasBirthday,
                    request.SortBy,
                    request.SortDirection,
                    request.Page,
                    request.PageSize);

                var result = await _contactRepository.SearchAsync(searchParams, ct);

                return new PagedResultDto<ContactSummaryDto>
                {
                    Items = result.Items.Adapt<List<ContactSummaryDto>>(),
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages,
                    HasNextPage = result.HasNextPage,
                    HasPreviousPage = result.HasPreviousPage
                };
            },
            tags: ["contacts"],
            cancellationToken: cancellationToken);
    }
}
