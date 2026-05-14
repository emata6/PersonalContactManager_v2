using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using PersonalContactManager.Application.DTOs;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Application.Contacts.Queries.GetUpcomingBirthdays;

public sealed class GetUpcomingBirthdaysQueryHandler : IRequestHandler<GetUpcomingBirthdaysQuery, List<ContactSummaryDto>>
{
    private readonly IContactRepository _contactRepository;
    private readonly HybridCache _cache;

    public GetUpcomingBirthdaysQueryHandler(IContactRepository contactRepository, HybridCache cache)
    {
        _contactRepository = contactRepository;
        _cache = cache;
    }

    public async Task<List<ContactSummaryDto>> Handle(GetUpcomingBirthdaysQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            $"contacts:birthdays:{request.DaysAhead}",
            async ct =>
            {
                var contacts = await _contactRepository.GetUpcomingBirthdaysAsync(request.DaysAhead, ct);
                return contacts.Adapt<List<ContactSummaryDto>>();
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(1) },
            tags: ["contacts"],
            cancellationToken: cancellationToken);
    }
}
